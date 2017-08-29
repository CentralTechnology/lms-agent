namespace Core.DirectoryServices
{
    using System;
    using System.DirectoryServices.AccountManagement;
    using System.DirectoryServices.ActiveDirectory;
    using System.Net.NetworkInformation;
    using NLog;

    public class DirectoryServicesManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public bool DomainExist()
        {
            PrincipalContext context = null;

            try
            {
                context = new PrincipalContext(ContextType.Domain);
                return true;
            }
            catch (ActiveDirectoryOperationException ex)
            {
                Logger.Debug(ex);
                return false;
            }
            finally
            {
                context?.Dispose();
            }
        }

        public bool PrimaryDomainController()
        {
            Domain domain = null;

            try
            {
                var context = new DirectoryContext(DirectoryContextType.Domain);
                domain = Domain.GetDomain(context);

                DomainController pdc = domain.PdcRoleOwner;
                string currentMachine = $"{Environment.MachineName}.{IPGlobalProperties.GetIPGlobalProperties().DomainName}";

                return pdc.Name.Equals(currentMachine, StringComparison.OrdinalIgnoreCase);
            }
            catch (ActiveDirectoryOperationException ex)
            {
                Logger.Debug(ex);
                return false;
            }
            finally
            {
                domain?.Dispose();
            }
        }
    }
}