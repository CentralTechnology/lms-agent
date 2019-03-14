using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace LMS.Core.Veeam.Backup.Configuration.V65
{
  public static class ExtensionMethodsRegistryKey
  {
    private static readonly Func<RegistryKey, int> StateGetter = ExtensionMethodsRegistryKey.CreateStateGetter();

    [SecurityCritical]
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public static RegistryKey Reopen(this RegistryKey key)
    {
      RegistryKeyState state = key.GetState();
      if ((state & RegistryKeyState.SystemKey) == RegistryKeyState.SystemKey)
        return RegistryKey.FromHandle(key.Handle, key.View);
      string[] strArray = key.Name.Split(Path.DirectorySeparatorChar);
      if (strArray.Length < 2)
        throw new ArgumentException("Invalid name of the registry key: " + key.Name, nameof (key));
      RegistryHive hKey;
      switch (strArray[0])
      {
        case "HKEY_CLASSES_ROOT":
          hKey = RegistryHive.ClassesRoot;
          break;
        case "HKEY_CURRENT_CONFIG":
          hKey = RegistryHive.CurrentConfig;
          break;
        case "HKEY_CURRENT_USER":
          hKey = RegistryHive.CurrentUser;
          break;
        case "HKEY_DYN_DATA":
          hKey = RegistryHive.DynData;
          break;
        case "HKEY_LOCAL_MACHINE":
          hKey = RegistryHive.LocalMachine;
          break;
        case "HKEY_PERFORMANCE_DATA":
          hKey = RegistryHive.PerformanceData;
          break;
        case "HKEY_USERS":
          hKey = RegistryHive.Users;
          break;
        default:
          throw new Win32Exception(6, string.Format("Invalid hive [{0}] of the registry key [{1}].", (object) strArray[0], (object) key.Name));
      }
      RegistryKey registryKey = RegistryKey.OpenBaseKey(hKey, key.View);
      try
      {
        string name = string.Join(Path.DirectorySeparatorChar.ToString(), ((IEnumerable<string>) strArray).Skip<string>(1));
        return registryKey.OpenSubKey(name, (state & RegistryKeyState.WriteAccess) == RegistryKeyState.WriteAccess);
      }
      catch
      {
        registryKey.Dispose();
        throw;
      }
    }

    public static RegistryKeyState GetState(this RegistryKey key)
    {
      return (RegistryKeyState) ExtensionMethodsRegistryKey.StateGetter(key);
    }

    private static Func<RegistryKey, int> CreateStateGetter()
    {
      Type owner = typeof (RegistryKey);
      FieldInfo field = owner.GetField("state", BindingFlags.Instance | BindingFlags.NonPublic);
      if (field == (FieldInfo) null)
        throw new InvalidDataException("Cannot find [state] field in the [RegistryKey] type.");
      DynamicMethod dynamicMethod = new DynamicMethod("Get_" + field.Name, field.FieldType, new Type[1]
      {
        owner
      }, owner, true);
      ILGenerator ilGenerator = dynamicMethod.GetILGenerator();
      ilGenerator.Emit(OpCodes.Ldarg_0);
      ilGenerator.Emit(OpCodes.Ldfld, field);
      ilGenerator.Emit(OpCodes.Ret);
      return (Func<RegistryKey, int>) dynamicMethod.CreateDelegate(typeof (Func<RegistryKey, int>));
    }
  }
}
