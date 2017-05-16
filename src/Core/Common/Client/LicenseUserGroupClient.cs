namespace Core.Common.Client
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Abp.Domain.Services;
    using Extensions;
    using Models;
    using OData;
    using ShellProgressBar;
    using Simple.OData.Client;

    public class LicenseUserGroupClient : DomainService, ILicenseUserGroupClient
    {
        public async Task Add(List<LicenseUser> users, LicenseGroup group, ChildProgressBar childProgressBar)
        {
            childProgressBar?.UpdateMessage($"updating group membership - {group.Name}");

            using (ChildProgressBar pbar = Environment.UserInteractive && childProgressBar != null ? childProgressBar.Spawn(users.Count, $"adding users to group: {group.Name}", new ProgressBarOptions
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
                        await client.For<LicenseUser>().Key(user.Id).LinkEntryAsync(group, "Groups");

                        pbar?.Tick($"adding: {user.DisplayName}");
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
                }
            }
        }

        public async Task Remove(List<LicenseUser> users, LicenseGroup group, ChildProgressBar childProgressBar)
        {
            childProgressBar?.UpdateMessage("updating group membership");

            using (ChildProgressBar pbar = Environment.UserInteractive && childProgressBar != null ? childProgressBar.Spawn(users.Count, $"removing users from group: {group.Name}", new ProgressBarOptions
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
                        await client.For<LicenseUser>().Key(user.Id).UnlinkEntryAsync(group, "Groups");

                        pbar?.Tick($"removing: {user.DisplayName}");
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
                }
            }
        }
    }
}