using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.CentraStage
{
    using Abp.Domain.Services;
    using global::Hangfire.Server;

    public interface ICentraStageManager : IDomainService
    {
        Guid? GetIdFromRegistry();
        bool IsValid(PerformContext performContext);
    }
}
