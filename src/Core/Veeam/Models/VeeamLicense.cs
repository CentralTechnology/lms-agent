namespace LMS.Veeam.Models
{
    using System;
    using System.Globalization;
    using Abp.Dependency;
    using Abp.Extensions;
    using Abp.Logging;
    using Enums;
    using Managers;
    using Portal.LicenseMonitoringSystem.Veeam.Entities;

    public class VeeamLicense
    {
        public static LicenseEditions Edition
        {
            get
            {
                using (var licenseManager = IocManager.Instance.ResolveAsDisposable<ILicenseManager>())
                {
                    return ConvertLicEditionToAppEdition(licenseManager.Object.GetPropertyNoThrow("Edition"));
                }             
            }
        }

        public static DateTime ExpirationDate
        {
            get
            {
                try
                {
                    return DateTime.SpecifyKind(DateTime.ParseExact(ExpirationDateStr, "dd/MM/yyyy", CultureInfo.InvariantCulture), DateTimeKind.Local);
                }
                catch (Exception ex)
                {
                    LogHelper.Logger.Error(ex.Message);
                    LogHelper.LogException(ex);
                    return DateTime.MinValue;
                }
            }
        }

        public static string ExpirationDateStr
        {
            get
            {
                using (var licenseManager = IocManager.Instance.ResolveAsDisposable<ILicenseManager>())
                {
                    return licenseManager.Object.GetPropertyNoThrow("Expiration date");
                }
            }
        }

        public static ELicenseGeneration Generation => IsRental || IsPerpetual || IsSubscription ? ELicenseGeneration.V9 : ELicenseGeneration.Old;

        public static bool IsEvaluation => LicenseType.Contains("Evaluation");

        public static bool IsNfr => IsNfrType(LicenseType);

        public static bool IsPerpetual => LicenseType.Contains("Perpetual");

        public static bool IsRental => LicenseType.Contains("Rental");

        public static bool IsSubscription => LicenseType.Contains("Subscription");

        public static string LicenseType
        {
            get
            {
                using (var licenseManager = IocManager.Instance.ResolveAsDisposable<ILicenseManager>())
                {
                    return licenseManager.Object.GetPropertyNoThrow("License type");
                }
            }
        }

        public static string SupportId
        {
            get
            {
                using (var licenseManager = IocManager.Instance.ResolveAsDisposable<ILicenseManager>())
                {
                    return licenseManager.Object.GetPropertyNoThrow("Support ID");
                }
            }
        }

        public static LicenseTypeEx TypeEx
        {
            get
            {
                if (IsFull() && Generation == ELicenseGeneration.Old)
                {
                    return LicenseTypeEx.Full;
                }

                if (IsEvaluation)
                {
                    return LicenseTypeEx.Evaluation;
                }

                if (IsNfr)
                {
                    return LicenseTypeEx.NFR;
                }

                if (IsPerpetual)
                {
                    return LicenseTypeEx.Perpetual;
                }

                if (IsRental)
                {
                    return LicenseTypeEx.Rental;
                }

                return IsSubscription ? LicenseTypeEx.Subscription : LicenseTypeEx.Empty;
            }
        }

        private static LicenseEditions ConvertLicEditionToAppEdition(string licEdition)
        {
            if (licEdition.IsNullOrEmpty())
            {
                throw new NotSupportedException("License is not installed.");
            }

            if (licEdition.Contains("Enterprise") && licEdition.Contains("Plus"))
            {
                return LicenseEditions.EnterprisePlus;
            }

            if (licEdition.Contains("Enterprise"))
            {
                return LicenseEditions.Enterprise;
            }

            if (licEdition.Contains("Standard"))
            {
                return LicenseEditions.Standard;
            }

            throw new NotSupportedException("License is not installed.");
        }

        public static bool IsFull()
        {
            return LicenseType.Contains("Full") && !IsNfr;
        }

        private static bool IsNfrType(string licTypeStr)
        {
            return !licTypeStr.IsNullOrEmpty() && licTypeStr.ToUpper().StartsWith("NFR");
        }
    }
}