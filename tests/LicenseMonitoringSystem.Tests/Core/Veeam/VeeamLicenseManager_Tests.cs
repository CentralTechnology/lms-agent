namespace LicenseMonitoringSystem.Tests.Core.Veeam
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Abp;
    using LMS.Veeam.Managers;
    using Shouldly;
    using Xunit;

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class VeeamLicenseManager_Tests : LicenseMonitoringSystemTestBase
    {
        private readonly ILicenseManager _licenseManager;
        public VeeamLicenseManager_Tests()
        {
            _licenseManager = LocalIocManager.Resolve<ILicenseManager>();
            _licenseManager.SetLicenseFile(_exampleLicense);
        }

        private const string _exampleLicense = @"﻿<?xml version=""1.0"" encoding=""utf-8""?>
<Licenses><License><![CDATA[CPU sockets (vSphere)=16
Company=Central Technology Ltd
Description=Copyright 2016 Veeam, Inc. All Rights Reserved. The Software Product is protected by copyright and other intellectual property laws and treaties. Veeam or its suppliers own the title, copyright, and other intellectual property rights in the Software Product. Veeam reserves all rights not expressly granted to you in EULA. The Software Product is licensed, not sold. Veeam grants to you a nonexclusive nontransferable license to use the Software Product, provided that you agree with EULA.
E-mail=chris.barr@ct.co.uk
Edition=Enterprise
Expiration date=30/11/2017
First name=Christopher
Last name=Barr
License information=License type:\tRental\nLicensed to:\tCentral Technology Ltd\nContact person:\tChristopher Barr\nLicensed Sockets:\t0\nEdition:       \tEnterprise\nManaged VMs (vSphere):\t378\nManaged VMs (Hyper-V):\t0\nCPU sockets (vSphere):\t16\nManaged VMs (vSphere):\t378
License type=Rental
Managed VMs (Hyper-V)=0
Managed VMs (vSphere)=378
Product=Veeam Backup and Replication
Version=9.x
Installation time=30606421:-779068240]]></License></Licenses>
";

        [Fact]
        public void ExtractPropertiesFromLicense_ShouldReturnDictionary()
        {
            // act
            Dictionary<string, string> value = _licenseManager.ExtractPropertiesFromLicense();

            // assert
            value.ShouldNotBeNull();
            value.ShouldBeOfType<Dictionary<string, string>>();
        }

        [Fact]
        public void GetProperty_ShouldReturnString_WhenPropertyExists()
        {
            // act
            string sut = _licenseManager.GetProperty("Edition");

            // assert
            sut.ShouldBeOfType<string>();
            sut.ShouldBe("Enterprise");
        }

        [Fact]
        public void GetProperty_ShouldThrowException_WhenPropertyDoesNotExist()
        {
            // act && assert
            ShouldThrowExtensions.ShouldThrow<AbpException>(() => _licenseManager.GetProperty(Guid.NewGuid().ToString()));
        }

        [Fact]
        public void GetPropertyNoThrow_ShouldReturnInt_WhenUsedWithTypeParameter()
        {
            // act
            int sutInt = _licenseManager.GetPropertyNoThrow<int>("Managed VMs (Hyper-V)");

            // assert
            sutInt.ShouldBeOfType<int>();
            sutInt.ShouldBe(0);
        }

        [Fact]
        public void GetPropertyNoThrow_ShouldReturnEmptyString_WhenPropertyDoesNotExist()
        {
            // act
            string sut = _licenseManager.GetPropertyNoThrow(Guid.NewGuid().ToString());

            // assert
            sut.ShouldBeOfType<string>();
            sut.ShouldBe(string.Empty);
        }

        [Fact]
        public void GetPropertyNoThrow_ShouldReturnString_WhenPropertyExists()
        {
            // act
            string sut = _licenseManager.GetPropertyNoThrow("Expiration date");

            // assert
            sut.ShouldBeOfType<string>();
            sut.ShouldBe("30/11/2017");
        }
    }
}