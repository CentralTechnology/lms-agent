using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Autotask
{
    using Abp.Domain.Services;

    public interface IAutotaskManager : IDomainService
    {
        int GetId();
        bool IsValid();
    }
}
