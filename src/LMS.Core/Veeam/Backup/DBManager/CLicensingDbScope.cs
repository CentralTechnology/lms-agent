using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LMS.Core.Extensions;
using LMS.Core.Veeam.Common;
using LMS.Core.Veeam.DBManager;

namespace LMS.Core.Veeam.Backup.DBManager
{
  public class CLicensingDbScope
  {
    private static readonly ISqlFieldDescriptor<Guid> InstanceIdField = SqlFieldDescriptor.UniqueIdentifier("instance_id");
    private static readonly ISqlFieldDescriptor<int> InstanceWeightField = SqlFieldDescriptor.Int("instance_weight");
    private static readonly ISqlFieldDescriptor<int> LicenseWeightField = SqlFieldDescriptor.Int("license_weight");
    private static readonly ISqlFieldDescriptor<bool> ExcludeNewInstancesField = SqlFieldDescriptor.Bit("exclude_new_instances");
    private static readonly ISqlFieldDescriptor<Guid?> TenantIdField = SqlFieldDescriptor.UniqueIdentifierNullable("tenant_id");
    private static readonly ISqlFieldDescriptor<ELicensePlatform> PlatformField = SqlFieldDescriptor.IntEnum<ELicensePlatform>("platform");
    private static readonly ISqlFieldDescriptor<EEpLicenseMode?> EpLicenseModeField = SqlFieldDescriptor.IntNullableEnum<EEpLicenseMode>("ep_license_mode");
    private static readonly ISqlFieldDescriptor<int> ObjectsCounttField = SqlFieldDescriptor.Int("objects_count");
    private static readonly ISqlFieldDescriptor<int> RentalObjectsCounttField = SqlFieldDescriptor.Int("rental_objects_count");
    private static readonly ISqlFieldDescriptor<int> WeightField = SqlFieldDescriptor.Int("weight");
    private static readonly ISqlFieldDescriptor<long> UsnField = SqlFieldDescriptor.BigInt("usn");
    private static readonly ISqlFieldDescriptor<DateTime> LastProcessingTimeField = SqlFieldDescriptor.DateTimeUtc("last_active_time");
    private static readonly ISqlFieldDescriptor<int> LastResultField = SqlFieldDescriptor.Int("last_result");
    private readonly IDatabaseAccessor _dbAccessor;

    internal CLicensingDbScope(IDatabaseAccessor dbAccessor)
    {
      this._dbAccessor = dbAccessor;
      this.Instances = new CInstanceLicensingDbScope(dbAccessor);
      this.Reports = new CLicensedReportsDbScope(dbAccessor);
    }

    public CInstanceLicensingDbScope Instances { get; private set; }

    public CLicensedReportsDbScope Reports { get; private set; }

    public IReadOnlyList<Guid> RemoveExcessLicensedAgents(
      VInstancesValue licenseWeight,
      SqlLicensePlatformWeightsTableType platformWeights,
      bool excludeNewInstances)
    {
      using (ReadableTable readableTable = this._dbAccessor.GetReadableTable("[dbo].[Licensing.RemoveExcessLicensedAgents]", CLicensingDbScope.LicenseWeightField.MakeParam(licenseWeight.ToInternal()), platformWeights.ToSqlParameter("platform_weights_table"), CLicensingDbScope.ExcludeNewInstancesField.MakeParam(excludeNewInstances)))
        return (IReadOnlyList<Guid>) readableTable.ReadAll<Guid>((Func<DataTableReader, Guid>) (reader => reader.GetValue<Guid>("id")));
    }

    public bool CanProcessInstances(
      IReadOnlyList<ILicensedInstance> instances,
      VInstancesValue instanceWeight,
      VInstancesValue licenseWeight,
      SqlLicensePlatformWeightsTableType platformWeights,
      bool excludeNewInstances)
    {
      using (SqlGuidTableType sqlGuidTableType = new SqlGuidTableType(instances.Select<ILicensedInstance, Guid>((Func<ILicensedInstance, Guid>) (x => x.InstanceId))))
      {
        using (ReadableTable readableTable = this._dbAccessor.GetReadableTable("[dbo].[Licensing.CanProcessInstance]", sqlGuidTableType.ToSqlParameter("@instances_ids"), CLicensingDbScope.InstanceWeightField.MakeParam(instanceWeight.ToInternal()), CLicensingDbScope.LicenseWeightField.MakeParam(licenseWeight.ToInternal()), platformWeights.ToSqlParameter("platform_weights_table"), CLicensingDbScope.ExcludeNewInstancesField.MakeParam(excludeNewInstances)))
          return readableTable.ReadSingleOrDefault<bool>((Func<DataTableReader, bool>) (x => x.GetValue<bool>("result", false)));
      }
    }

    public IReadOnlyList<CPlatformInstanceCountersInfo> GetInstanceCountersInfo(
      SqlLicensePlatformWeightsTableType platformsWeights,
      bool excludeNewInstances)
    {
      using (ReadableTable readableTable = this._dbAccessor.GetReadableTable("[dbo].[Licensing.GetInstancesCountersInfo]", platformsWeights.ToSqlParameter("platformTable"), CLicensingDbScope.ExcludeNewInstancesField.MakeParam(excludeNewInstances)))
        return (IReadOnlyList<CPlatformInstanceCountersInfo>) CLicensingDbScope.GetPlatformCountersInfos((IReadOnlyList<CLicensingDbScope.CObjectsCountInfo>) readableTable.ReadAll<CLicensingDbScope.CObjectsCountInfo>((Func<DataTableReader, CLicensingDbScope.CObjectsCountInfo>) (r => CLicensingDbScope.ParseInstancesInfo(r, false))));
    }

    public IReadOnlyList<CPlatformInstanceCountersInfo> GetInstanceCountersInfo(
      SqlLicensePlatformWeightsTableType platformsWeights,
      bool excludeNewInstances,
      ILicensedInstance licenseCheckingInstance)
    {
      using (ReadableTable readableTable = this._dbAccessor.GetReadableTable("[dbo].[Licensing.GetInstancesCountersInfo]", platformsWeights.ToSqlParameter("platformTable"), CLicensingDbScope.ExcludeNewInstancesField.MakeParam(excludeNewInstances), CLicensingDbScope.InstanceIdField.MakeParam(licenseCheckingInstance.InstanceId), CDbAccessor.MakeParam("ep_license_mode", (object) licenseCheckingInstance.Platform.LicenseMode)))
        return (IReadOnlyList<CPlatformInstanceCountersInfo>) CLicensingDbScope.GetPlatformCountersInfos((IReadOnlyList<CLicensingDbScope.CObjectsCountInfo>) readableTable.ReadAll<CLicensingDbScope.CObjectsCountInfo>((Func<DataTableReader, CLicensingDbScope.CObjectsCountInfo>) (r => CLicensingDbScope.ParseInstancesInfo(r, false))));
    }

    public IReadOnlyDictionary<Guid, IReadOnlyList<CPlatformInstanceCountersInfo>> GetCloudInstanceCountersInfoByTenants(
      SqlLicensePlatformWeightsTableType platformsWeights,
      bool excludeNewInstances)
    {
      using (ReadableTable readableTable = this._dbAccessor.GetReadableTable("[dbo].[Licensing.CC.GetInstancesCountersInfo]", platformsWeights.ToSqlParameter("platformTable"), CLicensingDbScope.ExcludeNewInstancesField.MakeParam(excludeNewInstances)))
        return this.GetCloudInstanceCountersInfoByTenantsInternal(readableTable);
    }

    public IReadOnlyDictionary<CCloudTenantInfoEx, IReadOnlyList<CPlatformInstanceCountersInfo>> GetTenantsLicenseInfoEx(
      SqlLicensePlatformWeightsTableType platformsWeights,
      bool excludeNewInstances,
      Guid tenantId)
    {
      return this.GetTenantsLicenseInfoExInternal(platformsWeights, excludeNewInstances, new Guid?(tenantId));
    }

    public IReadOnlyDictionary<CCloudTenantInfoEx, IReadOnlyList<CPlatformInstanceCountersInfo>> GetTenantsLicenseInfoEx(
      SqlLicensePlatformWeightsTableType platformsWeights,
      bool excludeNewInstances)
    {
      return this.GetTenantsLicenseInfoExInternal(platformsWeights, excludeNewInstances, new Guid?());
    }

    private IReadOnlyDictionary<CCloudTenantInfoEx, IReadOnlyList<CPlatformInstanceCountersInfo>> GetTenantsLicenseInfoExInternal(
      SqlLicensePlatformWeightsTableType platformsWeights,
      bool excludeNewInstances,
      Guid? tenantId)
    {
      using (ReadableSet readableSet = this._dbAccessor.GetReadableSet("[dbo].[Licensing.CC.GetInstancesCountersInfoEx]", platformsWeights.ToSqlParameter("platformTable"), CLicensingDbScope.ExcludeNewInstancesField.MakeParam(excludeNewInstances), CLicensingDbScope.TenantIdField.MakeParam(tenantId)))
      {
        using (ReadableTable currentTable1 = readableSet.GetCurrentTable())
        {
          IReadOnlyDictionary<Guid, IReadOnlyList<CPlatformInstanceCountersInfo>> byTenantsInternal = this.GetCloudInstanceCountersInfoByTenantsInternal(currentTable1);
          readableSet.BeginNextTable();
          using (ReadableTable currentTable2 = readableSet.GetCurrentTable())
          {
            IReadOnlyList<CCloudTenantInfoEx> tenantAdditionalInfo = this.GetTenantAdditionalInfo(currentTable2);
            return this.JoinLicensingAndTenantsInfo(byTenantsInternal, tenantAdditionalInfo);
          }
        }
      }
    }

    private IReadOnlyDictionary<CCloudTenantInfoEx, IReadOnlyList<CPlatformInstanceCountersInfo>> JoinLicensingAndTenantsInfo(
      IReadOnlyDictionary<Guid, IReadOnlyList<CPlatformInstanceCountersInfo>> countersInfo,
      IReadOnlyList<CCloudTenantInfoEx> additionalInfos)
    {
      Dictionary<CCloudTenantInfoEx, IReadOnlyList<CPlatformInstanceCountersInfo>> dictionary = new Dictionary<CCloudTenantInfoEx, IReadOnlyList<CPlatformInstanceCountersInfo>>();
      foreach (CCloudTenantInfoEx additionalInfo in (IEnumerable<CCloudTenantInfoEx>) additionalInfos)
      {
        IReadOnlyList<CPlatformInstanceCountersInfo> instanceCountersInfoList;
        if (!countersInfo.TryGetValue(additionalInfo.TenantId, out instanceCountersInfoList))
          instanceCountersInfoList = (IReadOnlyList<CPlatformInstanceCountersInfo>) SArrayExtensions.Empty<CPlatformInstanceCountersInfo>();
        dictionary.Add(additionalInfo, instanceCountersInfoList);
      }
      return (IReadOnlyDictionary<CCloudTenantInfoEx, IReadOnlyList<CPlatformInstanceCountersInfo>>) dictionary;
    }

    private IReadOnlyDictionary<Guid, IReadOnlyList<CPlatformInstanceCountersInfo>> GetCloudInstanceCountersInfoByTenantsInternal(
      ReadableTable table)
    {
      return CLicensingDbScope.GroupByTenantId((IReadOnlyList<CLicensingDbScope.CObjectsCountInfo>) table.ReadAll<CLicensingDbScope.CObjectsCountInfo>((Func<DataTableReader, CLicensingDbScope.CObjectsCountInfo>) (r => CLicensingDbScope.ParseInstancesInfo(r, true))));
    }

    public IReadOnlyList<CPlatformInstanceCountersInfo> GetNewInstancesInPreviousMonth(
      SqlLicensePlatformWeightsTableType platformsWeights)
    {
      using (ReadableTable readableTable = this._dbAccessor.GetReadableTable("[dbo].[Licensing.GetNewInstancesInPrevMonth]", platformsWeights.ToSqlParameter("platformTable")))
        return (IReadOnlyList<CPlatformInstanceCountersInfo>) readableTable.ReadAll<CPlatformInstanceCountersInfo>(new Func<DataTableReader, CPlatformInstanceCountersInfo>(CLicensingDbScope.ParseNewInstancesInPreviousMonthInfo));
    }

    public IReadOnlyDictionary<Guid, IReadOnlyList<CPlatformInstanceCountersInfo>> GetCloudInstancesSummaryInPreviousMonth(
      SqlLicensePlatformWeightsTableType platformsWeights)
    {
      using (ReadableTable readableTable = this._dbAccessor.GetReadableTable("[dbo].[Licensing.Reporting.CC.GetInstancesSummaryInPrevMonth]", platformsWeights.ToSqlParameter("platformTable")))
        return CLicensingDbScope.GroupByTenantId((IReadOnlyList<CLicensingDbScope.CObjectsCountInfo>) readableTable.ReadAll<CLicensingDbScope.CObjectsCountInfo>((Func<DataTableReader, CLicensingDbScope.CObjectsCountInfo>) (r => CLicensingDbScope.ParseInstancesInfo(r, true))));
    }

    public int GetProtectedInstancesCounter(SqlLicensePlatformTableType platforms)
    {
      using (ReadableTable readableTable = this._dbAccessor.GetReadableTable("[dbo].[Licensing.GetProtectedVmsCount]", platforms.ToSqlParameter("platformTable")))
        return readableTable.ReadSingleOrDefault<int>((Func<DataTableReader, int>) (reader => reader.GetValue<int>("protected_vms_count")));
    }

    private static CPlatformInstanceCountersInfo[] GetPlatformCountersInfos(
      IReadOnlyList<CLicensingDbScope.CObjectsCountInfo> objectsCountInfos)
    {
      if (objectsCountInfos.Count == 0)
        return new CPlatformInstanceCountersInfo[0];
      ILookup<CLicensePlatform, CLicensingDbScope.CObjectsCountInfo> lookup = objectsCountInfos.ToLookup<CLicensingDbScope.CObjectsCountInfo, CLicensePlatform, CLicensingDbScope.CObjectsCountInfo>((Func<CLicensingDbScope.CObjectsCountInfo, CLicensePlatform>) (x => x.Platform), (Func<CLicensingDbScope.CObjectsCountInfo, CLicensingDbScope.CObjectsCountInfo>) (x => x));
      CPlatformInstanceCountersInfo[] instanceCountersInfoArray = new CPlatformInstanceCountersInfo[lookup.Count];
      int num = 0;
      foreach (IGrouping<CLicensePlatform, CLicensingDbScope.CObjectsCountInfo> source in (IEnumerable<IGrouping<CLicensePlatform, CLicensingDbScope.CObjectsCountInfo>>) lookup)
      {
        int weightInternal = source.First<CLicensingDbScope.CObjectsCountInfo>().WeightInternal;
        int objectsCountOrZero1 = CLicensingDbScope.GetObjectsCountOrZero(source.FirstOrDefault<CLicensingDbScope.CObjectsCountInfo>((Func<CLicensingDbScope.CObjectsCountInfo, bool>) (x => x.InfoType == CLicensingDbScope.CCounterInfoType.Used)));
        int objectsCountOrZero2 = CLicensingDbScope.GetObjectsCountOrZero(source.FirstOrDefault<CLicensingDbScope.CObjectsCountInfo>((Func<CLicensingDbScope.CObjectsCountInfo, bool>) (x => x.InfoType == CLicensingDbScope.CCounterInfoType.New)));
        int objectsCountOrZero3 = CLicensingDbScope.GetObjectsCountOrZero(source.FirstOrDefault<CLicensingDbScope.CObjectsCountInfo>((Func<CLicensingDbScope.CObjectsCountInfo, bool>) (x => x.InfoType == CLicensingDbScope.CCounterInfoType.Rental)));
        instanceCountersInfoArray[num++] = new CPlatformInstanceCountersInfo(source.Key, weightInternal, objectsCountOrZero1, objectsCountOrZero2, objectsCountOrZero3);
      }
      return ((IEnumerable<CPlatformInstanceCountersInfo>) instanceCountersInfoArray).ToArray<CPlatformInstanceCountersInfo>();
    }

    private static IReadOnlyDictionary<Guid, IReadOnlyList<CPlatformInstanceCountersInfo>> GroupByTenantId(
      IReadOnlyList<CLicensingDbScope.CObjectsCountInfo> objectsCountInfos)
    {
      ILookup<Guid, CLicensingDbScope.CObjectsCountInfo> lookup = objectsCountInfos.ToLookup<CLicensingDbScope.CObjectsCountInfo, Guid, CLicensingDbScope.CObjectsCountInfo>((Func<CLicensingDbScope.CObjectsCountInfo, Guid>) (x => x.TenantId.Value), (Func<CLicensingDbScope.CObjectsCountInfo, CLicensingDbScope.CObjectsCountInfo>) (x => x));
      if (lookup.Count == 0)
        return (IReadOnlyDictionary<Guid, IReadOnlyList<CPlatformInstanceCountersInfo>>) new Dictionary<Guid, IReadOnlyList<CPlatformInstanceCountersInfo>>();
      Dictionary<Guid, IReadOnlyList<CPlatformInstanceCountersInfo>> dictionary = new Dictionary<Guid, IReadOnlyList<CPlatformInstanceCountersInfo>>(lookup.Count);
      foreach (IGrouping<Guid, CLicensingDbScope.CObjectsCountInfo> source in (IEnumerable<IGrouping<Guid, CLicensingDbScope.CObjectsCountInfo>>) lookup)
      {
        CPlatformInstanceCountersInfo[] platformCountersInfos = CLicensingDbScope.GetPlatformCountersInfos((IReadOnlyList<CLicensingDbScope.CObjectsCountInfo>) source.ToArray<CLicensingDbScope.CObjectsCountInfo>());
        dictionary.Add(source.Key, (IReadOnlyList<CPlatformInstanceCountersInfo>) platformCountersInfos);
      }
      return (IReadOnlyDictionary<Guid, IReadOnlyList<CPlatformInstanceCountersInfo>>) dictionary;
    }

    private static int GetObjectsCountOrZero(CLicensingDbScope.CObjectsCountInfo info)
    {
      if (info != null)
        return info.ObjectsCount;
      return 0;
    }

    private static CLicensingDbScope.CObjectsCountInfo ParseInstancesInfo(
      DataTableReader reader,
      bool parseTenantId)
    {
      CLicensingDbScope.CCounterInfoType infoType = reader.GetValue<CLicensingDbScope.CCounterInfoType>("counter_type");
      Guid? tenantId = parseTenantId ? CLicensingDbScope.TenantIdField.Read((IDataReader) reader) : new Guid?();
      int objectsCount = reader.GetValue<int>("instances_count");
      ELicensePlatform platform1 = reader.GetValue<ELicensePlatform>("platform");
      EEpLicenseMode? nullable = reader.GetNullable<EEpLicenseMode>("ep_license_mode", new EEpLicenseMode?());
      int weightInternal = reader.GetValue<int>("weight");
      CLicensePlatform platform2 = !nullable.HasValue ? SLicensePlatformController.Get(platform1) : SLicensePlatformController.Get(platform1, nullable.Value);
      switch (infoType)
      {
        case CLicensingDbScope.CCounterInfoType.Used:
        case CLicensingDbScope.CCounterInfoType.New:
        case CLicensingDbScope.CCounterInfoType.Rental:
          return new CLicensingDbScope.CObjectsCountInfo(infoType, tenantId, platform2, weightInternal, objectsCount);
        default:
          throw new Exception("Unknown counter type: " + (object) infoType);
      }
    }

    private static CPlatformInstanceCountersInfo ParseNewInstancesInPreviousMonthInfo(
      DataTableReader reader)
    {
      ELicensePlatform platform = CLicensingDbScope.PlatformField.Read((IDataReader) reader);
      EEpLicenseMode? licenseMode = CLicensingDbScope.EpLicenseModeField.Read((IDataReader) reader);
      int newObjectsCount = CLicensingDbScope.ObjectsCounttField.Read((IDataReader) reader);
      int rentalObjectsCount = CLicensingDbScope.RentalObjectsCounttField.Read((IDataReader) reader);
      int weightInternal = CLicensingDbScope.WeightField.Read((IDataReader) reader);
      return new CPlatformInstanceCountersInfo(SLicensePlatformController.Get(platform, licenseMode), weightInternal, 0, newObjectsCount, rentalObjectsCount);
    }

    private IReadOnlyList<CCloudTenantInfoEx> GetTenantAdditionalInfo(
      ReadableTable table)
    {
      return (IReadOnlyList<CCloudTenantInfoEx>) table.ReadAll<CCloudTenantInfoEx>((Func<DataTableReader, CCloudTenantInfoEx>) (reader => this.ParseAdditionalTenantInfo(reader)));
    }

    private CCloudTenantInfoEx ParseAdditionalTenantInfo(DataTableReader reader)
    {
      return new CCloudTenantInfoEx(CLicensingDbScope.TenantIdField.Read((IDataReader) reader).Value, (ulong) CLicensingDbScope.UsnField.Read((IDataReader) reader), CLicensingDbScope.LastProcessingTimeField.Read((IDataReader) reader), CLicensingDbScope.LastResultField.Read((IDataReader) reader));
    }

    private enum CCounterInfoType
    {
      Used,
      New,
      Rental,
    }

    private class CObjectsCountInfo
    {
      public CObjectsCountInfo(
        CLicensingDbScope.CCounterInfoType infoType,
        Guid? tenantId,
        CLicensePlatform platform,
        int weightInternal,
        int objectsCount)
      {
        this.InfoType = infoType;
        this.TenantId = tenantId;
        this.Platform = platform;
        this.WeightInternal = weightInternal;
        this.ObjectsCount = objectsCount;
      }

      public CLicensingDbScope.CCounterInfoType InfoType { get; private set; }

      public Guid? TenantId { get; private set; }

      public CLicensePlatform Platform { get; private set; }

      public int WeightInternal { get; private set; }

      public int ObjectsCount { get; private set; }
    }
  }
}
