using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Users
{
    using Abp.Dependency;
    using Abp.Domain.Services;
    using Models;
    using ShellProgressBar;

    public interface IUserOrchestrator : IDomainService
    {
        /// <summary>
        /// Updates the uploads status to be CalledIn.
        /// </summary>
        /// <param name="uploadId"></param>
        /// <param name="pbar"></param>
        /// <returns></returns>
        Task CallIn(int uploadId, ProgressBar pbar);

        /// <summary>
        ///  Determines whether this client needs to check in or not. If a check in is required then the upload id will be returned or else a 0 will be.
        /// </summary>
        /// <param name="pbar"></param>
        /// <returns></returns>
        Task<ManagedSupport> ProcessUpload(ProgressBar pbar);

        /// <summary>
        /// Applies the upload id to all the users then performs the CRUD operations via the api
        /// </summary>
        /// <param name="uploadId"></param>
        /// <param name="pbar"></param>
        /// <returns></returns>
        Task<List<LicenseUser>> ProcessUsers(int uploadId, ProgressBar pbar);

        /// <summary>
        /// Performs CRUD operations via the api on the groups
        /// </summary>
        /// <param name="users"></param>
        /// <param name="pbar"></param>
        /// <returns></returns>
        Task ProcessGroups(List<LicenseUser> users, ProgressBar pbar);

        /// <summary>
        /// Determines the relationship between the users and groups then performs CRUD operations via the api.
        /// </summary>
        /// <param name="users"></param>
        /// <param name="pbar"></param>
        /// <returns></returns>
        Task ProcessUserGroups(List<LicenseUser> users, ProgressBar pbar);
    }
}
