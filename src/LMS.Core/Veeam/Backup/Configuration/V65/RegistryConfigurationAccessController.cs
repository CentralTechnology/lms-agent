using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace LMS.Core.Veeam.Backup.Configuration.V65
{
  internal class RegistryConfigurationAccessController
  {
    private readonly RegistryKey _rootRegistryKey;
    private SecurityIdentifier[] _identities;

    public SecurityIdentifier Owner { get; set; }

    public RegistryConfigurationAccessController(RegistryKey rootRegistryKey)
    {
      if (rootRegistryKey == null)
        throw new ArgumentNullException(nameof (rootRegistryKey));
      this._rootRegistryKey = rootRegistryKey;
      this._identities = new SecurityIdentifier[0];
    }

    public SecurityIdentifier[] Identities
    {
      get
      {
        return this._identities;
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException(nameof (value));
        this._identities = value;
      }
    }

    public void Set()
    {
      Stack<RegistryKey> registryKeyStack = new Stack<RegistryKey>();
      registryKeyStack.Push(this._rootRegistryKey);
      while (registryKeyStack.Count != 0)
      {
        RegistryKey registryKey1 = registryKeyStack.Pop();
        try
        {
          if (registryKey1 == this._rootRegistryKey)
          {
            RegistrySecurity accessControl = this._rootRegistryKey.GetAccessControl();
            foreach (RegistryAccessRule accessRule in (ReadOnlyCollectionBase) accessControl.GetAccessRules(true, false, typeof (SecurityIdentifier)))
              accessControl.RemoveAccessRuleSpecific(accessRule);
            foreach (RegistryAccessRule rule in ((IEnumerable<SecurityIdentifier>) this.Identities).Select<SecurityIdentifier, RegistryAccessRule>((Func<SecurityIdentifier, RegistryAccessRule>) (identity => new RegistryAccessRule((IdentityReference) identity, RegistryRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow))))
              accessControl.AddAccessRule(rule);
            if (this.Owner != (SecurityIdentifier) null)
              accessControl.SetOwner((IdentityReference) this.Owner);
            accessControl.SetAccessRuleProtection(true, false);
            this._rootRegistryKey.SetAccessControl(accessControl);
          }
          else if (this.Owner != (SecurityIdentifier) null)
          {
            RegistrySecurity accessControl = registryKey1.GetAccessControl();
            accessControl.SetOwner((IdentityReference) this.Owner);
            accessControl.SetAccessRuleProtection(false, true);
            registryKey1.SetAccessControl(accessControl);
          }
          foreach (string subKeyName in registryKey1.GetSubKeyNames())
          {
            try
            {
              using (RegistryKey registryKey2 = registryKey1.OpenSubKey(subKeyName, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.ReadPermissions | RegistryRights.ChangePermissions))
              {
                if (registryKey2 != null)
                {
                  RegistrySecurity accessControl = registryKey2.GetAccessControl();
                  foreach (RegistryAccessRule accessRule in (ReadOnlyCollectionBase) accessControl.GetAccessRules(true, false, typeof (SecurityIdentifier)))
                    accessControl.RemoveAccessRuleSpecific(accessRule);
                  accessControl.SetAccessRuleProtection(false, true);
                  registryKey2.SetAccessControl(accessControl);
                }
              }
              registryKeyStack.Push(registryKey1.OpenSubKey(subKeyName, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl));
            }
            catch (Exception ex)
            {
            }
          }
        }
        finally
        {
          if (registryKey1 != this._rootRegistryKey)
            registryKey1.Dispose();
        }
      }
    }
  }
}
