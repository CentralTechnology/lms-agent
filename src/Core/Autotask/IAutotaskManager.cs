using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Autotask
{
    using Abp.Domain.Services;
    using global::Hangfire.Server;

    public interface IAutotaskManager : IDomainService
    {
        bool IsValid(PerformContext performContext);
    }
}
