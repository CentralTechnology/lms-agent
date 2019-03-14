using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LMS.Core.Veeam.Backup.Common;
using LMS.Core.Veeam.Backup.Configuration.V65;
using LMS.Core.Veeam.DBManager;
using Microsoft.Win32;
using Serilog;

namespace LMS.Core.Veeam.Backup.DBManager
{
  public class CDBManager : IDisposable, IDBManager
  {
    private static Lazy<IDBManager> _privateInstance = CDBManager.CreateInstance();

    private static Lazy<IDBManager> CreateInstance()
    {
      return new Lazy<IDBManager>((Func<IDBManager>) (() => (IDBManager) new CDBManager()));
    }

    public static void InitByPrivateConfiguration(IDatabaseConfiguration databaseConfiguration)
    {
      CDBManager._privateInstance = new Lazy<IDBManager>((Func<IDBManager>) (() => (IDBManager) new CDBManager(databaseConfiguration)));
    }

    public static void InitByPrivateInstance(IDBManager manager)
    {
      CDBManager._privateInstance = new Lazy<IDBManager>((Func<IDBManager>) (() => manager));
    }

    public static void InitInstance()
    {
      CDBManager._privateInstance = CDBManager.CreateInstance();
    }

    public static void InitInstance(IDatabaseAccessor accessor)
    {
      CDBManager._privateInstance = new Lazy<IDBManager>((Func<IDBManager>) (() => (IDBManager) new CDBManager(SProduct.Instance.LoadDatabaseConfiguration(RegistryView.Default), accessor)));
    }

    public static IDBManager Instance
    {
      get
      {
        return CDBManager._privateInstance.Value;
      }
    }

    public IDatabaseConfiguration DatabaseConfiguration { get; private set; }

    public IDatabaseAccessor DbAccessor { get; private set; }

    public CLicensingDbScope Licensing { get; private set; }


    public static CDBManager CreateNewInstance()
    {
      return new CDBManager();
    }

    private CDBManager()
      : this(SProduct.Instance.LoadDatabaseConfiguration(RegistryView.Default))
    {
    }

    private CDBManager(IDatabaseConfiguration databaseConfiguration)
      : this(databaseConfiguration, (IDatabaseAccessor) new LocalDbAccessor(databaseConfiguration.ConnectionString, databaseConfiguration))
    {
    }

    private CDBManager(IDatabaseConfiguration databaseConfiguration, IDatabaseAccessor accessor)
    {
      if (databaseConfiguration == null)
        throw new ArgumentNullException(nameof (databaseConfiguration));
      try
      {
        this.DatabaseConfiguration = databaseConfiguration;
        this.DbAccessor = accessor;
        this.Licensing = new CLicensingDbScope(this.DbAccessor);
      }
      catch (Exception ex)
      {
        Log.Error(ex, (string) null);
        throw;
      }
    }

    public void Dispose()
    {
      try
      {
        this.DbAccessor.Dispose();
        this.DbAccessor = (IDatabaseAccessor) null;
      }
      catch (Exception ex)
      {
        Log.Error(ex, (string) null);
      }
    }

    public static void CloseAll()
    {
      if (CDBManager._privateInstance == null || !CDBManager._privateInstance.IsValueCreated)
        return;
      CDBManager._privateInstance.Value.Dispose();
      CDBManager._privateInstance = (Lazy<IDBManager>) null;
    }

    public CPersistentDbConnection CreatePersistantDbConnection()
    {
      return new CPersistentDbConnection(new CDbConnectionImpl(this.DatabaseConfiguration.ConnectionString, TimeSpan.FromSeconds((double) this.DatabaseConfiguration.Info.StatementTimeout)));
    }
  }
}
