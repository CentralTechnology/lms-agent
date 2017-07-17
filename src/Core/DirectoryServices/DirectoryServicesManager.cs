namespace Core.DirectoryServices
{
    using System;
    using System.DirectoryServices.AccountManagement;
    using System.DirectoryServices.ActiveDirectory;
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
            catch (Exception ex)
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

                return pdc.Name.Equals(Environment.MachineName, StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception ex)
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