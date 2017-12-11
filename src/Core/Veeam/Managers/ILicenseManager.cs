using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Veeam.Managers
{
    using Abp.Domain.Services;

    public interface ILicenseManager : IDomainService
    {
        Dictionary<string, string> ExtractPropertiesFromLicense();
        string GetProperty(string name);

        TResult GetProperty<TResult>(string name)
            where TResult : struct;

        string GetPropertyNoThrow(string name);

        TResult GetPropertyNoThrow<TResult>(string name)
            where TResult : struct;

        string LoadFromRegistry();

        void SetLicenseFile(string licenseFile = null);
    }
}
