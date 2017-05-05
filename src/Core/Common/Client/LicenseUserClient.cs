namespace Core.Common.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Abp;
    using Abp.Domain.Services;
    using Extensions;
    using Models;
    using OData;
    using ShellProgressBar;
    using Simple.OData.Client;

    public class LicenseUserClient : DomainService, ILicenseUserClient
    {
        public async Task Add(List<LicenseUser> users, ChildProgressBar childProgressBar)
        {
            childProgressBar?.UpdateMessage("adding users");

            using (ChildProgressBar pbar = Environment.UserInteractive && childProgressBar != null ? childProgressBar.Spawn(users.Count, "adding users", new ProgressBarOptions
            {
                ForeGroundColor = ConsoleColor.Yellow,
                ProgressCharacter = '─',
                BackgroundColor = ConsoleColor.DarkGray
            }) : null)
            {
                var client = new ODataClient(new ODataLicenseClientSettings());

                for (int index = 0; index < users.Count; index++)
                {
                    LicenseUser user = users[index];

                    try
                    {
                        Logger.Info($"adding: {user.DisplayName}");

                        await client.For<LicenseUser>().Set(new
                        {
                            user.DisplayName,
                            user.Email,
                            user.Enabled,
                            user.FirstName,
                            user.Id,
                            user.SupportUploadId,
                            user.Surname,
                            user.WhenCreated
                        }).InsertEntryAsync();
                    }
                    catch (WebRequestException ex)
                    {
                        ex.FormatWebRequestException();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"Failed to add: {user.DisplayName}");
                        Logger.Error("Execution will continue");
                        Logger.DebugFormat("Exception: ", ex);
                    }
                    finally
                    {
                        pbar?.Tick($"adding: {user.DisplayName}");
                    }
                }
            }

            childProgressBar?.Tick();
        }

        public async Task Remove(List<LicenseUser> users, ChildProgressBar childProgressBar)
        {
            childProgressBar?.UpdateMessage("removing users");

            using (ChildProgressBar pbar = Environment.UserInteractive && childProgressBar != null ? childProgressBar.Spawn(users.Count, "removing users", new ProgressBarOptions
            {
                ForeGroundColor = ConsoleColor.Yellow,
                ProgressCharacter = '─',
                BackgroundColor = ConsoleColor.DarkGray
            }) : null)
            {
                var client = new ODataClient(new ODataLicenseClientSettings());

                for (int index = 0; index < users.Count; index++)
                {
                    LicenseUser user = users[index];

                    try
                    {
                        await client.For<LicenseUser>().Key(user.Id).DeleteEntryAsync();

                        pbar?.Tick($"removing: {user.DisplayName}");
                    }
                    catch (WebRequestException ex)
                    {
                        ex.FormatWebRequestException();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"Failed to delete: {user.DisplayName}");
                        Logger.Error("Execution will continue");
                        Logger.DebugFormat("Exception: ", ex);
                    }
                }
            }

            childProgressBar?.Tick();
        }

        public async Task Update(List<LicenseUser> users, ChildProgressBar childProgressBar)
        {
            childProgressBar?.UpdateMessage("updating users");

            using (ChildProgressBar pbar = Environment.UserInteractive && childProgressBar != null ? childProgressBar.Spawn(users.Count, "updating users", new ProgressBarOptions
            {
                ForeGroundColor = ConsoleColor.Yellow,
                ProgressCharacter = '─',
                BackgroundColor = ConsoleColor.DarkGray
            }) : null)
            {
                var client = new ODataClient(new ODataLicenseClientSettings());

                for (int index = 0; index < users.Count; index++)
                {
                    LicenseUser user = users[index];

                    try
                    {
                        await client.For<LicenseUser>().Key(user.Id).Set(new
                        {
                            user.DisplayName,
                            user.Email,
                            user.Enabled,
                            user.FirstName,
                            user.Surname,
                            user.WhenCreated
                        }).UpdateEntryAsync();

                        pbar?.Tick($"updating: {user.DisplayName}");
                    }
                    catch (WebRequestException ex)
                    {
                        ex.FormatWebRequestException();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"Failed to update: {user.DisplayName}");
                        Logger.Error($"Execution will continue");
                        Logger.DebugFormat("Exception: ", ex);
                    }
                }
            }

            childProgressBar?.Tick();
        }

        public async Task<List<LicenseUser>> GetAll()
        {
            try
            {
                var client = new ODataClient(new ODataLicenseClientSettings());
                IEnumerable<LicenseUser> users = await client.For<LicenseUser>().Expand(u => u.Groups).FindEntriesAsync();
                List<LicenseUser> licenseUsers = users.ToList();
                Logger.Debug($"{licenseUsers.Count} users returned from the api.");
                return licenseUsers;
            }
            catch (WebRequestException ex)
            {
                ex.FormatWebRequestException();
                return null;
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to obtain a list of users from the api.");
                Logger.DebugFormat("Exception: ", ex);
                return null;
            }
        }
    }
}