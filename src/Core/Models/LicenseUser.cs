namespace Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Abp.Domain.Entities;
    using Abp.Timing;
    using Users.Entities;

    public class LicenseUser : LicenseBase
    {
        private DateTime _whenCreated;
        private DateTime? _lastLoginDate;

        [MaxLength(256)]
        public string DisplayName { get; set; }

        [MaxLength(256)]
        public string Email { get; set; }

        public bool Enabled { get; set; }

        [MaxLength(64)]
        public string FirstName { get; set; }

        public List<LicenseGroup> Groups { get; set; }

        public DateTime? LastLoginDate
        {
            get => _lastLoginDate;
            set
            {
                if (value == null)
                {
                    _lastLoginDate = null;
                }
                else
                {
                    _lastLoginDate = Clock.Normalize(DateTime.Parse(value.ToString()));
                }
            }
        }

        public int ManagedSupportId { get; set; }

        [MaxLength(256)]
        public string SamAccountName { get; set; }

        [MaxLength(64)]
        public string Surname { get; set; }

        public DateTime WhenCreated
        {
            get => _whenCreated;
            set => _whenCreated = Clock.Normalize(value);
        }
    }

    [Flags]
    public enum CallInStatus
    {
        CalledIn = 0,
        NotCalledIn = 1,
        NeverCalledIn = 2
    }

    [Flags]
    public enum UserFlags
    {
        // Reference - Chapter 10 (from The .NET Developer's Guide to Directory Services Programming)

        Script = 1, // 0x1
        AccountDisabled = 2, // 0x2
        HomeDirectoryRequired = 8, // 0x8
        AccountLockedOut = 16, // 0x10
        PasswordNotRequired = 32, // 0x20
        PasswordCannotChange = 64, // 0x40
        EncryptedTextPasswordAllowed = 128, // 0x80
        TempDuplicateAccount = 256, // 0x100
        NormalAccount = 512, // 0x200
        InterDomainTrustAccount = 2048, // 0x800
        WorkstationTrustAccount = 4096, // 0x1000
        ServerTrustAccount = 8192, // 0x2000
        PasswordDoesNotExpire = 65536, // 0x10000 (Also 66048 )
        MnsLogonAccount = 131072, // 0x20000
        SmartCardRequired = 262144, // 0x40000
        TrustedForDelegation = 524288, // 0x80000
        AccountNotDelegated = 1048576, // 0x100000
        UseDesKeyOnly = 2097152, // 0x200000
        DontRequirePreauth = 4194304, // 0x400000
        PasswordExpired = 8388608, // 0x800000 (Applicable only in Window 2000 and Window Server 2003)
        TrustedToAuthenticateForDelegation = 16777216, // 0x1000000
        NoAuthDataRequired = 33554432 // 0x2000000
    }
}