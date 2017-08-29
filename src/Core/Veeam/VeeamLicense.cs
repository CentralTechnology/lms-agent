namespace Core.Veeam
{
    using System;
    using System.Globalization;
    using Abp.Extensions;
    using NLog;

    public class VeeamLicense
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly LicenseManager LicenseManager = new LicenseManager();

        public static LicenseEditions Edition => ConvertLicEditionToAppEdition(LicenseManager.GetPropertyNoThrow("Edition"));

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
                    Logger.Error(ex.Message);
                    Logger.Debug(ex);
                    return DateTime.MinValue;
                }
            }
        }

        public static string ExpirationDateStr => LicenseManager.GetPropertyNoThrow("Expiration date");

        public static ELicenseGeneration Generation => IsRental || IsPerpetual || IsSubscription ? ELicenseGeneration.V9 : ELicenseGeneration.Old;

        public static bool IsEvaluation => LicenseType.Contains("Evaluation");

        public static bool IsNfr => IsNfrType(LicenseType);

        public static bool IsPerpetual => LicenseType.Contains("Perpetual");

        public static bool IsRental => LicenseType.Contains("Rental");

        public static bool IsSubscription => LicenseType.Contains("Subscription");

        public static string LicenseType => LicenseManager.GetPropertyNoThrow("License type");

        public static string SupportId => LicenseManager.GetPropertyNoThrow("Support ID");

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