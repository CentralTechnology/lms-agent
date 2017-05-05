namespace Core.Common.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Abp.Domain.Services;
    using Extensions;
    using Models;
    using OData;
    using ShellProgressBar;
    using Simple.OData.Client;

    public class LicenseGroupClient : DomainService, ILicenseGroupClient
    {
        public async Task Add(List<LicenseGroup> groups, ChildProgressBar childProgressBar)
        {
            childProgressBar?.UpdateMessage("adding groups");

            using (ChildProgressBar pbar = Environment.UserInteractive && childProgressBar != null ? childProgressBar.Spawn(groups.Count, "adding groups", new ProgressBarOptions
            {
                ForeGroundColor = ConsoleColor.Yellow,
                ProgressCharacter = '─',
                BackgroundColor = ConsoleColor.DarkGray
            }) : null)
            {
                var client = new ODataClient(new ODataLicenseClientSettings());

                for (int index = 0; index < groups.Count; index++)
                {
                    LicenseGroup group = groups[index];

                    try
                    {
                        await client.For<LicenseGroup>().Set(new
                        {
                            group.Id,
                            group.Name,
                            group.WhenCreated
                        }).InsertEntryAsync();

                        pbar?.Tick($"adding: {group.Name}");
                    }
                    catch (WebRequestException ex)
                    {
                        ex.FormatWebRequestException();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"Failed to add: {group.Name}");
                        Logger.Error("Execution will continue");
                        Logger.DebugFormat("Exception: ", ex);
                    }
                }
            }

            childProgressBar?.Tick();
        }

        public async Task Remove(List<LicenseGroup> groups, ChildProgressBar childProgressBar)
        {
            childProgressBar?.UpdateMessage("removing groups");

            using (ChildProgressBar pbar = Environment.UserInteractive && childProgressBar != null ? childProgressBar.Spawn(groups.Count, "removing groups", new ProgressBarOptions
            {
                ForeGroundColor = ConsoleColor.Yellow,
                ProgressCharacter = '─',
                BackgroundColor = ConsoleColor.DarkGray
            }) : null)
            {
                var client = new ODataClient(new ODataLicenseClientSettings());

                for (int index = 0; index < groups.Count; index++)
                {
                    LicenseGroup group = groups[index];

                    try
                    {
                        await client.For<LicenseGroup>().Key(group.Id).DeleteEntryAsync();

                        pbar?.Tick($"removing: {group.Name}");
                    }
                    catch (WebRequestException ex)
                    {
                        ex.FormatWebRequestException();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"Failed to delete: {group.Name}");
                        Logger.Error("Execution will continue");
                        Logger.DebugFormat("Exception: ", ex);
                    }
                }
            }

            childProgressBar?.Tick();
        }

        public async Task Update(List<LicenseGroup> groups, ChildProgressBar childProgressBar)
        {
            childProgressBar?.UpdateMessage("updating groups");

            using (ChildProgressBar pbar = Environment.UserInteractive && childProgressBar != null ? childProgressBar.Spawn(groups.Count, "updating groups", new ProgressBarOptions
            {
                ForeGroundColor = ConsoleColor.Yellow,
                ProgressCharacter = '─',
                BackgroundColor = ConsoleColor.DarkGray
            }) : null)
            {
                var client = new ODataClient(new ODataLicenseClientSettings());

                for (int index = 0; index < groups.Count; index++)
                {
                    LicenseGroup group = groups[index];

                    try
                    {
                        await client.For<LicenseGroup>().Key(group.Id).Set(new
                        {
                            group.Name,
                            group.WhenCreated
                        }).UpdateEntryAsync();

                        pbar?.Tick($"updating: {group.Name}");
                    }
                    catch (WebRequestException ex)
                    {
                        ex.FormatWebRequestException();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"Failed to update: {group.Name}");
                        Logger.Error("Execution will continue");
                        Logger.DebugFormat("Exception: ", ex);
                    }
                }
            }

            childProgressBar?.Tick();
        }

        public async Task<List<LicenseGroup>> GetAll()
        {
            try
            {
                var client = new ODataClient(new ODataLicenseClientSettings());
                IEnumerable<LicenseGroup> groups = await client.For<LicenseGroup>().FindEntriesAsync();
                return groups.ToList();
            }
            catch (WebRequestException ex)
            {
                ex.FormatWebRequestException();
                return null;
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to obtain a list of groups from the api.");
                Logger.DebugFormat("Exception: ", ex);
                return null;
            }
        }
    }
}