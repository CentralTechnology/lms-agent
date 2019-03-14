using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LMS.Core.Veeam.Common;
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

    public CProductUpdatesDbScope ProductUpdates { get; private set; }

    public ITapeTenantRestoreDbScope TapeTenantRestore { get; private set; }

    public CJobSourceRepositoryDbScope JobSourceRepository { get; private set; }

    public IDatabaseAccessor DbAccessor { get; private set; }

    public CJobsScope Jobs { get; private set; }

    public CJobProxiesDbScope JobProxies { get; private set; }

    public IBackupProxiesDbScope BackupProxies { get; private set; }

    public IBackupRepositoriesDbScope BackupRepositories { get; private set; }

    public CBackupRepositoryPermissionsDbScope BackupRepositoryPermissions { get; private set; }

    public CDataDomainRepositoryServersDbScope DataDomainRepositoryServers { get; private set; }

    public CStoreOnceRepositoryServersDbScope StoreOnceRepositoryServers { get; private set; }

    public CWindowsRepositoryDbScope WindowsRepositories { get; private set; }

    public CCifsRepositoryDbScope CifsRepositories { get; private set; }

    public IArchiveRepositoriesDbScope ArchiveRepositories { get; private set; }

    public IExternalRepositoriesDbScope ExternalRepositories { get; private set; }

    public IRepositoriesDbScope Repositories { get; private set; }

    public CBackupsDbScope Backups { get; private set; }

    public CReplicasAuxDbScope ReplicasAux { get; private set; }

    public CBackupPlatformDbScope BackupPlatforms { get; private set; }

    public IBackupJobsDbScope BackupJobs { get; private set; }

    public IOibsDbScope<COibInfo> Oibs { get; private set; }

    public IOibsDbScope<COibEncryptedInfo> EncryptedOibs { get; private set; }

    public IPointsDbScope Points { get; private set; }

    public IStoragesDbScope Storages { get; private set; }

    public CReplicasDbScope Replicas { get; private set; }

    public CReplicaConfigurationsDbScope ReplicaConfigurations { get; private set; }

    public CRotatedDrivesDbScope RotatedDrives { get; private set; }

    public CAwayStoragesDbScope AwayStorages { get; private set; }

    public IHostsDbScope Host { get; private set; }

    public CNetworkTrafficRedirectorsDbScope NetworkTrafficRedirectors { get; private set; }

    public CPhysicalHostsDbScope PhysicalHost { get; private set; }

    public CHostCompsDbScope HostComps { get; private set; }

    public IJobsSessionsDbScope JobsSessions { get; private set; }

    public CXmlLoggerDbScope XmlLogger { get; private set; }

    public IBackupJobsSessionsDbScope BackupJobsSessions { get; private set; }

    public IBackupJobsTaskSessionsDbScope BackupJobsTaskSessions { get; private set; }

    public CRestoreJobSessionsDbScope RestoreJobsSessions { get; private set; }

    public CRestoreJobTaskSessionsDbScope RestoreJobsTaskSessions { get; private set; }

    public CFoldersDbScope Folders { get; private set; }

    public CHostsInFoldersDbScope HostsInFolders { get; private set; }

    public CHostsByJobsDbScope HostsByJobs { get; private set; }

    public IObjectsDbScope Objs { get; private set; }

    public CEncryptedObjectsDbScope EncryptedObjs { get; private set; }

    public CLicensingDbScope Licensing { get; private set; }

    public CLicensedHostsDbScope LicHosts { get; private set; }

    public CInstanceAgentsDbScope InstanceAgents { get; private set; }

    public CPluginServersLicensingDbScope PluginServersLicenses { get; private set; }

    public CAgentLicenseDbScope AgentLicenses { get; private set; }

    public CApplicationGroupsDbScope ApplicationGroups { get; private set; }

    public CDrvObjectsInApplicationGroupsDbScope DrvObjectsInApplicationGroups { get; private set; }

    public CDrvJobsDbScope DrvJobs { get; private set; }

    public CDrvJobTaskSessionsDbScope DrvJobsTaskSessions { get; private set; }

    public COptionsDbScope Options { get; private set; }

    public CObjectsInJobsDbScope ObjectsInJobs { get; private set; }

    public CDrvRolesDbScope DrvRoles { get; private set; }

    public ILinkedObjectsDbScope LinkedBackupRepositories { get; private set; }

    public ILinkedJobsDbScope LinkedJobs { get; private set; }

    public ILinkedQuotasDbScope LinkedQuotas { get; private set; }

    public ILinkedTenantsDbScope LinkedTenants { get; private set; }

    public ILinkedBackupsDbScope LinkedBackups { get; private set; }

    public CDrJobSessionsDbScope DrvJobsSessions { get; private set; }

    public IAntivirusTaskSessionDbScope AntivirusTaskSessions { get; private set; }

    public CUsersAndRolesDbScope UsersAndRoles { get; private set; }

    public ISessionsDbScope SessionBase { get; private set; }

    public CVmssDbScope Vmss { get; private set; }

    public CVirtualLabsDbScopeNew VirtualLabShort { get; private set; }

    public CSbVerificationRulesDbScope SbVerificationRules { get; private set; }

    public CReplicationInfoDbScope ReplicationInfo { get; private set; }

    public CDisksDbScope Disks { get; private set; }

    public CPhysHostDiskDbScope PhysHostDisk { get; private set; }

    public CPhysHostShareDbScope PhysHostShare { get; private set; }

    public CVolumesDbScope Volumes { get; private set; }

    public CPhysHostVolumeDbScope PhysHostVolume { get; private set; }

    public CProxyAgentsDbScope ProxyAgents { get; private set; }

    public CHvSnapshotDbScope HvSnapshots { get; private set; }

    public CHvRecoveryCheckpointDbScope HvRecoveryCheckpoints { get; private set; }

    public CVmSnapshotDbScope VmSnapshots { get; private set; }

    public CMruListDbScope MruList { get; private set; }

    public CVbrInfraDbScope VbrInfra { get; private set; }

    public CConfigurationJobsDbScope ConfigurationJob { get; private set; }

    public CSmbFileSharesDbScope SmbFileShare { get; private set; }

    public CHvVirtualLabsDbScope HvVirtualLab { get; private set; }

    public CDataSourcesDbScope DataSources { get; private set; }

    public CHierarchyScanJobDbScope HierarchyScanJob { get; private set; }

    public CWanAcceleratorsDbScope WanAccelerator { get; private set; }

    public CItemRestoreAuditDbScope ItemRestoreAudits { get; private set; }

    public CJobWanAcceleratorsDbScope JobWanAccelerator { get; private set; }

    public CWanGlobalCacheCorruptedRolesDbScope WanWanGlobalCacheCorruptedRoles { get; private set; }

    public CJobObjectStateDbScope JobObjectsState { get; private set; }

    public IEventsDbScope Events { get; private set; }

    public CJobVmDiskStateDbScope JobVmDisksState { get; private set; }

    public CTapeOibsDbScope TapeOibs { get; private set; }

    public IBackupSetsDbScope BackupSets { get; private set; }

    public ICatalogObjectVersionDbScope CatalogObjectVersions { get; private set; }

    public IChangersDbScope Changers { get; private set; }

    public IDevicesDbScope Devices { get; private set; }

    public IDirectoriesDbScope Directories { get; private set; }

    public DirectoryVersionsDbScope DirectoryVersions { get; private set; }

    public IFilesDbScope Files { get; private set; }

    public IFilePartsDbScope FileParts { get; private set; }

    public IFileVersionsDbScope FileVersions { get; private set; }

    public INdmpVolumesDbScope NdmpVolumes { get; private set; }

    public INdmpVolumePartsDbScope NdmpVolumeParts { get; private set; }

    public INdmpVolumeVersionsDbScope NdmpVolumeVersions { get; private set; }

    public HighestCommittedUsnDbScope HighestCommittedUsn { get; private set; }

    public ITapeHostsDbScope TapeHosts { get; private set; }

    public ITapeJobsDbScope TapeJobs { get; private set; }

    public ILibrariesDbScope Libraries { get; private set; }

    public ILibraryDevicesDbScope LibraryDevices { get; private set; }

    public IMediaSetsDbScope MediaSets { get; private set; }

    public IMediaPoolsDbScope MediaPools { get; private set; }

    public IMediaVaultDbScope MediaVault { get; private set; }

    public IMediaPoolVaultDbScope MediaPoolVault { get; private set; }

    public ITapeSessionsDbScope TapeSessions { get; private set; }

    public ITapeDrivesDbScope TapeDrives { get; private set; }

    public ITapeMediumsDbScope TapeMediums { get; private set; }

    public ITapeBackupDbScope TapeBackups { get; private set; }

    public ITapeStoragesMapDbScope TapeStorages { get; private set; }

    public ITapeVmObjectsDbScope TapeVmObjects { get; private set; }

    public ITapeOrphanedLinkableObjectsDbScope TapeOrphanedLinkableObjects { get; private set; }

    public ITapeBackupsInfoDbScope TapeBackupsInfo { get; private set; }

    public CReplicaDiskTransferStatesDbScope ReplicaDiskTransferStates { get; private set; }

    public ICryptoKeyDbScope CryptoKeys { get; private set; }

    public CGuestDatabaseDbScope GuestDatabase { get; private set; }

    public CSqlOibsDbScope SqlOibs { get; private set; }

    public CSqlBackupIntervalSessDbScope SqlBackupIntervalSessions { get; private set; }

    public CEncryptedImportBackupDbScope EncryptedImportBackups { get; private set; }

    public CImportKeySetDbScope ImportKeySets { get; private set; }

    public CVeeamZIPRetentionScope VeeamZipRetention { get; private set; }

    public ITapeEncryptedImportedBackupDbScope TapeEncryptedImportedBackup { get; private set; }

    public CBackupServiceDbScope BackupService { get; private set; }

    public CCurrentConnectionDbScope CurrentConnections { get; private set; }

    public IVcdMultiTenancyDbScope VcdMultiTenancy { get; private set; }

    public CVSphereMultiTenancyDbScope VSphereMultiTenancy { get; private set; }

    public CCloudProviderDbScope CloudProvider { get; private set; }

    public CCloudProviderReportingDbScope CloudProviderReporting { get; private set; }

    public CCloudGateDbScope CloudGate { get; private set; }

    public CCloudGatewayPoolDbScope CloudGatewayPool { get; private set; }

    public CCloudTenantDbScope CloudTenant { get; private set; }

    public CCloudSimpleTenantDbScope CloudSimpleTenant { get; private set; }

    public CCloudVcdTenantDbScope CloudVcdTenant { get; private set; }

    public CCloudSubtenantsDbScope CloudSubtenants { get; private set; }

    public CCloudVcdDataCentersDbScope CloudVcdDataCenters { get; private set; }

    public CCloudVcdStoragePoliciesDbScope CloudVcdStoragePolicies { get; private set; }

    public CCloudVcdNetworksDbScope CloudVcdNetworksDbScope { get; private set; }

    public CCloudVcdAppliancesDbScope CloudVcdAppliances { get; private set; }

    public CQuotaBackupsDbScope QuotaBackups { get; private set; }

    public CCloudTenantWansDbScope CloudTenantWans { get; private set; }

    public CQuotaDbScope Quotas { get; private set; }

    public CRepositoryQuotaDbScope RepositoryQuotas { get; private set; }

    public CChildQuotaDbScope ChildQuotas { get; private set; }

    public CTenantQuotaDbScope TenantQuotas { get; private set; }

    public CSubtenantQuotaDbScope SubtenantQuotas { get; private set; }

    public CCloudRepositoriesDbScope CloudRepositories { get; private set; }

    public CCloudSessionsDbScope CloudSessions { get; private set; }

    public CCloudRuleDbScope CloudRule { get; private set; }

    public CCloudLicensingDbScope CloudLicensing { get; private set; }

    public CCloudTenantBackupDbScope CloudTenantBackups { get; private set; }

    public CCloudApplianceDbScope CloudAppliances { get; private set; }

    public CCloudApplianceDefaultNetworkDbScope CloudAppliancesDefaultNetworks { get; private set; }

    public CCloudTenantApplianceDbScope CloudTenantAppliances { get; private set; }

    public CCloudTenantApplianceUsagesDbScope CloudTenantApplianceUsages { get; private set; }

    public CCloudFailoverPlanDbScope CloudFailoverPlans { get; private set; }

    public CCloudPublicIpDbScope CloudPublicIps { get; private set; }

    public CCloudNetworkUsageDbScope CloudNetworkUsage { get; private set; }

    public CCloudBinFilesDbScope CloudBinFiles { get; private set; }

    public CCachedObjectsIdMappingDbScope CachedObjectsIdMappingDbScope { get; private set; }

    public CCloudSessionLogInfoDbScope CloudSessionLogInfo { get; private set; }

    public CCloudTaskSessionDbScope CloudTaskSession { get; private set; }

    public CloudGateStatisticDbScope CloudGateStatistic { get; private set; }

    public CCloudTenantDataDbScope CloudTenantData { get; private set; }

    public CCloudTenantDataLinksDbScope CloudTenantDataLinks { get; private set; }

    public CWanAcceleratorsStatisticDbScope WanAcceleratorsStatistic { get; private set; }

    public CCloudReportsDbScope CloudReports { get; private set; }

    public CViHardwarePlanDbScope ViHardwarePlans { get; private set; }

    public CViHardwarePlanDatastoreDbScope ViHardwarePlanDatastores { get; private set; }

    public CViHardwarePlanNetworkDbScope ViHardwarePlanNetworks { get; private set; }

    public CViHardwareQuotaDatastoreDbScope ViHardwareQuotaDatastores { get; private set; }

    public CViHardwareQuotaDatastoreUsagesDbScope ViHardwareQuotaDatastoreUsages { get; private set; }

    public CViHardwareQuotaNetworkDbScope ViHardwareQuotaNetworks { get; private set; }

    public CViHardwareQuotaDbScope ViHardwareQuotas { get; private set; }

    public CHvHardwarePlanDbScope HvHardwarePlans { get; private set; }

    public CHvHardwarePlanVolumeDbScope HvHardwarePlanVolumes { get; private set; }

    public CHvHardwarePlanNetworkDbScope HvHardwarePlanNetworks { get; private set; }

    public CHvHardwareQuotaVolumeDbScope HvHardwareQuotaVolumes { get; private set; }

    public CHvHardwareQuotaVolumeUsagesDbScope HvHardwareQuotaVolumeUsages { get; private set; }

    public CHvHardwareQuotaNetworkDbScope HvHardwareQuotaNetworks { get; private set; }

    public CHvHardwareQuotaDbScope HvHardwareQuotas { get; private set; }

    public CCloudConnectHostDbScope CloudConnectHosts { get; private set; }

    public CCloudConnectStorageDbScope CloudConnectStorages { get; private set; }

    public CCloudConnectNetworkDbScope CloudConnectNetworks { get; private set; }

    public CTrustedCertificatesDbScope TrustedCertificates { get; private set; }

    public CExtendableRepositoriesDbScope ExtendableRepositories { get; private set; }

    public CStorageExtentAssociationsDbScope StorageExtentAssociations { get; private set; }

    public CStorageCopiesDbScope StorageCopies { get; private set; }

    public CBackupArchiveIndicesDbScope BackupArchiveIndex { get; private set; }

    public CForeignRepositoriesDbScope ForeignRepositories { get; private set; }

    public CForeignRepositoryProviderDbScope ForeignRepositoryProviders { get; private set; }

    public CFLRApplianceConfigurationDbScope FLRApplianceConfiguration { get; private set; }

    public CUpdateNotificationsDbScope UpdateNotifications { get; private set; }

    public CAlwaysOnGroupsDbScope AlwaysOnGroups { get; private set; }

    public CAlwaysOnGuestDatabasesDbScope AlwaysOnGuestDatabases { get; private set; }

    public CAlwaysOnListenersDbScope AlwaysOnListeners { get; private set; }

    public COibsWithAlwaysOnGroupsDbScope OibsWithAlwaysOnGroups { get; private set; }

    public CJobStateDbScope JobStates { get; private set; }

    public CBackupProxyGroupsDbScope BackupProxyGroups { get; private set; }

    public CBackupProxyGroupItemsDbScope BackupProxyGroupItems { get; private set; }

    public CJobProxyGroupsDbScope JobProxyGroups { get; private set; }

    public IBackupCopyIntervalsDbScope BackupCopyIntervals { get; private set; }

    public IBackupCopyIntervalStoragesDbScope BackupCopyIntervalStorages { get; private set; }

    public CDatabaseMaintenanceDbScope DatabaseMaintenance { get; private set; }

    public CSanVolumesDbScope SanVolumes { get; private set; }

    public CSanVolumeLUNsDbScope SanVolumeLUNs { get; private set; }

    public CSanSnapshotsDbScope SanSnapshots { get; private set; }

    public CSanSnapshotLUNsDbScope SanSnapshotLUNs { get; private set; }

    public CSanHostDbScope SanHost { get; private set; }

    public CSanInitiatorsByProxyDbScope SanInitiatorsByProxy { get; private set; }

    public CSanVolumeByProxyInitiatorDbScope SanVolumeByProxyInitiators { get; private set; }

    public CSanVolumeExportInfoDbScope SanVolumeExportInfos { get; private set; }

    public CSanVolumeExportToProxyLunIdDbScope SanVolumeExportToProxyLunId { get; private set; }

    public CSanStorageLocksDbScope SanStorageLocks { get; private set; }

    public CSanSnapshotTransferResourceDbScope SanSnapshotTransferResources { get; private set; }

    public CSanTargetSnapshotLunDbScope SanTargetSnapshotLuns { get; private set; }

    public CSanVolumeLUNExportLocksDbScope SanVolumeLunExportLocks { get; private set; }

    public CSanSnapshotJobDbScope SanSnapshotJobInfos { get; private set; }

    public CSanProxiesDbScope SanProxies { get; private set; }

    public CSanSnapTransferFailoverSnapshotsDbScope SanSnapTransferFailoverSnapshots { get; private set; }

    public CStorageEsxIpsByProxyDbScope StorageEsxIpsByProxy { get; private set; }

    public CSanSnapshotTransferDbScope SanSnapshotTransfers { get; private set; }

    public CSanVolumeOnTransferDbScope SanVolumeOnTransferInfos { get; private set; }

    public CSanVmBackupOnStorageDbScope SanVmBackupsOnStorage { get; private set; }

    public CSanJobSnasphotsOnStorageBackupDbScope SanJobSnasphotsOnStorageBackup { get; private set; }

    public CSanJobSnapshotLunDbScope SanJobSnapshotLuns { get; private set; }

    public CSanVmDiskOnDatastoreDbScope SanVmDisksOnDatastore { get; private set; }

    public COijProxiesDbScope OijProxies { get; private set; }

    public CWarningsDbScope Warnings { get; private set; }

    public CDatastoreOptionsItemDbScope DatastoreOptionsItem { get; private set; }

    public CHostOperationResultDbScope HostOperationResult { get; private set; }

    public CUserNotificationDbScope UserNotifications { get; private set; }

    public CHostNetworkDbScope HostNetwork { get; private set; }

    public CTrackedActionDbScope TrackedActions { get; private set; }

    public CSharedTrackedActionDbScope SharedTrackedActions { get; private set; }

    public COracleGuestDatabaseDbScope OracleGuestDb { get; private set; }

    public COracleOibsDbScope OracleOibs { get; private set; }

    public COibOracleArchiveLogsDbScope OibOracleArchiveLogs { get; private set; }

    public CSanVolumeRescanPolicyDbScope SanVolumeRescanPolicy { get; private set; }

    public CConcurentDbScope ConcurentDb { get; private set; }

    public CResourceScanDbScope ResourceScan { get; private set; }

    public CAzureSubscriptionDbScope AzureSubscription { get; private set; }

    public CAzureAccountDbScope AzureAccount { get; private set; }

    public CAwsApplianceDbScope AmazonAppliance { get; private set; }

    public CAzureApplianceDbScope AzureAppliance { get; private set; }

    public CAzureProxyDbScope AzureProxy { get; private set; }

    public COrchestratedTasksDbScope OrchestratedTasks { get; private set; }

    public CProxyRepositoryAffinityDbScope ProxyRepositoryAffinity { get; private set; }

    public CHostCompPreferableSettingsDbScope HostCompPreferableSettings { get; private set; }

    public CCloudRobocopTenantServersDbScope CloudRobocopTenantServers { get; private set; }

    public IEpAgentManagementJobSessionsDbScope EpAgentManagementJobsSessions { get; private set; }

    public CEpAgentPolicyConfigDbScope EpAgentPolicyConfig { get; private set; }

    public CEpAgentDbScope EpAgents { get; private set; }

    public CEpAgentLinksDbScope EpAgentLinks { get; private set; }

    public CEpAgentMembershipDbScope EpAgentMembership { get; private set; }

    public CEpContainersDbScope EpContainers { get; private set; }

    public CEpAgentBackupStatsDbScope EpAgentBackupStats { get; private set; }

    public IAgentObjectsOwnershipsDbScope AgentObjectsOwnerships { get; private set; }

    public CHostSshFingerprintDbScope HostFingerprint { get; private set; }

    public CEpAgentOptionsPolicyDbScope EpAgentOptionsPolicy { get; private set; }

    public EpAgentOptionsValuesDbScope EpAgentOptionsValues { get; private set; }

    public CEpVeeamContainerExtensionsDbScope EpVeeamContainerExtensions { get; private set; }

    public CAdObjectsDbScope AdObjects { get; private set; }

    public CCsvFilesDbScope CsvFiles { get; private set; }

    public CKvpDictionaryDbScope KvpDictionary { get; private set; }

    public CVmbApiSubscriptionsDbScope VmbApiSubscriptions { get; private set; }

    public CLocationsDbScope Locations { get; private set; }

    public CVbrStatisticsDbScope StatisticScope { get; private set; }

    public CDbPluginCommonScope DbPluginCommon { get; private set; }

    public COracleRMANDbScope OracleRMAN { get; private set; }

    public CSapHanaDbScope SapHana { get; private set; }

    public CExternalInfrastructureDbScope ExternalInfrastructure { get; private set; }

    public IExternalObjectsIdMappingDbScope ExternalObjectsIdMapping { get; private set; }

    public IAmazonS3ExternalClientsDbScope AmazonS3ExternalClients { get; private set; }

    public IAmazonS3ExternalBackupCheckpointsDbScope AmazonS3ExternalBackupCheckpoints { get; private set; }

    public IAmazonS3ExternalClientBackupsDbScope AmazonS3ExternalClientBackups { get; private set; }

    public IAmazonS3ExternalCacheStatsDbScope AmazonS3ExternalCacheStats { get; private set; }

    public IAmazonS3ExternalRepositoryOwnershipDbScope AmazonS3ExternalRepositoryOwnership { get; private set; }

    public CBackupArchiveInfosDbScope BackupArchiveInfos { get; private set; }

    public CBackupMonitorsDbScope BackupMonitors { get; private set; }

    public ITapeObjectInBackupDbScope TapeObjectInBackup { get; private set; }

    public CDeleteBackupInfosDbScope DeleteBackupInfos { get; private set; }

    public CRepositoryArchiveSettingsDbScope RepositoryArchiveSettings { get; private set; }

    public CRepositoryLimitSettingsDbScope RepositoryLimitSettings { get; private set; }

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
        this.Jobs = new CJobsScope(this.DbAccessor);
        this.JobProxies = new CJobProxiesDbScope(this.DbAccessor);
        this.BackupProxies = (IBackupProxiesDbScope) new CBackupProxiesDbScope(this.DbAccessor);
        CBackupRepositoriesDbScope repositoriesDbScope = new CBackupRepositoriesDbScope(this.DbAccessor);
        this.BackupRepositories = (IBackupRepositoriesDbScope) repositoriesDbScope;
        this.BackupRepositoryPermissions = new CBackupRepositoryPermissionsDbScope(this.DbAccessor);
        this.DataDomainRepositoryServers = new CDataDomainRepositoryServersDbScope(this.DbAccessor);
        this.StoreOnceRepositoryServers = new CStoreOnceRepositoryServersDbScope(this.DbAccessor);
        this.WindowsRepositories = new CWindowsRepositoryDbScope(this.DbAccessor);
        this.CifsRepositories = new CCifsRepositoryDbScope(this.DbAccessor);
        this.ArchiveRepositories = (IArchiveRepositoriesDbScope) new CArchiveRepositoriesDbScope(this.DbAccessor);
        this.ExternalRepositories = (IExternalRepositoriesDbScope) new CExternalRepositoriesDbScope(this.DbAccessor);
        this.Repositories = (IRepositoriesDbScope) new CRepositoriesDbScope(this.DbAccessor);
        this.Backups = new CBackupsDbScope(this.DbAccessor);
        this.ReplicasAux = new CReplicasAuxDbScope(this.DbAccessor);
        this.BackupPlatforms = new CBackupPlatformDbScope(this.DbAccessor);
        this.BackupJobs = (IBackupJobsDbScope) new CBackupJobsDbScope(this.DbAccessor);
        this.Oibs = (IOibsDbScope<COibInfo>) new COibsDbScope<COibInfo>(this.DbAccessor, (IOibInfoDbHelper<COibInfo>) new COibInfoDbHelper());
        this.EncryptedOibs = (IOibsDbScope<COibEncryptedInfo>) new COibsDbScope<COibEncryptedInfo>(this.DbAccessor, (IOibInfoDbHelper<COibEncryptedInfo>) new COibEncryptedInfoDbHelper());
        this.Points = (IPointsDbScope) new CPointsDbScope(this.DbAccessor);
        this.Storages = (IStoragesDbScope) new CStoragesDbScope(this.DbAccessor);
        this.Replicas = new CReplicasDbScope(this.DbAccessor);
        this.ReplicaConfigurations = new CReplicaConfigurationsDbScope(this.DbAccessor);
        this.RotatedDrives = new CRotatedDrivesDbScope(this.DbAccessor);
        this.AwayStorages = new CAwayStoragesDbScope(this.DbAccessor);
        this.Host = (IHostsDbScope) new CHostsDbScope(this.DbAccessor);
        this.NetworkTrafficRedirectors = new CNetworkTrafficRedirectorsDbScope(this.DbAccessor);
        this.PhysicalHost = new CPhysicalHostsDbScope(this.DbAccessor);
        this.HostComps = new CHostCompsDbScope(this.DbAccessor);
        this.BackupJobsSessions = (IBackupJobsSessionsDbScope) new CBackupJobsSessionsDbScope(this.DbAccessor);
        this.BackupJobsTaskSessions = (IBackupJobsTaskSessionsDbScope) new CBackupJobsTaskSessionsDbScope(this.DbAccessor);
        this.HostsInFolders = new CHostsInFoldersDbScope(this.DbAccessor);
        this.HostsByJobs = new CHostsByJobsDbScope(this.DbAccessor);
        this.Folders = new CFoldersDbScope(this.DbAccessor);
        this.Objs = (IObjectsDbScope) new CObjectsDbScope(this.DbAccessor);
        this.EncryptedObjs = new CEncryptedObjectsDbScope(this.DbAccessor);
        this.Licensing = new CLicensingDbScope(this.DbAccessor);
        this.LicHosts = new CLicensedHostsDbScope(this.DbAccessor);
        this.InstanceAgents = new CInstanceAgentsDbScope(this.DbAccessor);
        this.PluginServersLicenses = new CPluginServersLicensingDbScope(this.DbAccessor);
        this.AgentLicenses = new CAgentLicenseDbScope(this.DbAccessor);
        this.ApplicationGroups = new CApplicationGroupsDbScope(this.DbAccessor);
        this.DrvObjectsInApplicationGroups = new CDrvObjectsInApplicationGroupsDbScope(this.DbAccessor);
        this.DrvJobs = new CDrvJobsDbScope(this.DbAccessor);
        this.DrvJobsTaskSessions = new CDrvJobTaskSessionsDbScope(this.DbAccessor);
        this.Options = new COptionsDbScope(this.DbAccessor);
        this.ObjectsInJobs = new CObjectsInJobsDbScope(this.DbAccessor);
        this.DrvRoles = new CDrvRolesDbScope(this.DbAccessor);
        this.LinkedBackupRepositories = (ILinkedObjectsDbScope) new CLinkedBackupRepositoriesDbScope(this.DbAccessor);
        this.LinkedJobs = (ILinkedJobsDbScope) new LinkedJobsDbScope(this.DbAccessor);
        this.LinkedQuotas = (ILinkedQuotasDbScope) new CLinkedQuotaDbScope(this.DbAccessor);
        this.LinkedTenants = (ILinkedTenantsDbScope) new CLinkedTenantsDbScope(this.DbAccessor);
        this.LinkedBackups = (ILinkedBackupsDbScope) new CLinkedBackupsDbScope(this.DbAccessor);
        this.RestoreJobsSessions = new CRestoreJobSessionsDbScope(this.DbAccessor);
        this.RestoreJobsTaskSessions = new CRestoreJobTaskSessionsDbScope(this.DbAccessor);
        this.DrvJobsSessions = new CDrJobSessionsDbScope(this.DbAccessor);
        this.AntivirusTaskSessions = (IAntivirusTaskSessionDbScope) new CAntivirusTaskSessionDbScope(this.DbAccessor);
        this.UsersAndRoles = new CUsersAndRolesDbScope(this.DbAccessor);
        this.SessionBase = (ISessionsDbScope) new CSessionsDbScope(this.DbAccessor);
        this.Vmss = new CVmssDbScope(this.DbAccessor);
        this.VirtualLabShort = new CVirtualLabsDbScopeNew(this.DbAccessor);
        this.JobsSessions = (IJobsSessionsDbScope) new CJobsSessionsDbScope(this.DbAccessor);
        this.SbVerificationRules = new CSbVerificationRulesDbScope(this.DbAccessor);
        this.XmlLogger = new CXmlLoggerDbScope(this.DbAccessor);
        this.ReplicationInfo = new CReplicationInfoDbScope(this.DbAccessor);
        this.Disks = new CDisksDbScope(this.DbAccessor);
        this.PhysHostDisk = new CPhysHostDiskDbScope(this.DbAccessor);
        this.PhysHostShare = new CPhysHostShareDbScope(this.DbAccessor);
        this.Volumes = new CVolumesDbScope(this.DbAccessor);
        this.PhysHostVolume = new CPhysHostVolumeDbScope(this.DbAccessor);
        this.ProxyAgents = new CProxyAgentsDbScope(this.DbAccessor);
        this.HvSnapshots = new CHvSnapshotDbScope(this.DbAccessor);
        this.HvRecoveryCheckpoints = new CHvRecoveryCheckpointDbScope(this.DbAccessor);
        this.VmSnapshots = new CVmSnapshotDbScope(this.DbAccessor);
        this.MruList = new CMruListDbScope(this.DbAccessor);
        this.VbrInfra = new CVbrInfraDbScope(this.DbAccessor);
        this.ConfigurationJob = new CConfigurationJobsDbScope(this.DbAccessor);
        this.SmbFileShare = new CSmbFileSharesDbScope(this.DbAccessor);
        this.HvVirtualLab = new CHvVirtualLabsDbScope(this.DbAccessor);
        this.DataSources = new CDataSourcesDbScope(this.DbAccessor);
        this.HierarchyScanJob = new CHierarchyScanJobDbScope(this.DbAccessor);
        this.WanAccelerator = new CWanAcceleratorsDbScope(this.DbAccessor);
        this.ItemRestoreAudits = new CItemRestoreAuditDbScope(this.DbAccessor);
        this.JobWanAccelerator = new CJobWanAcceleratorsDbScope(this.DbAccessor);
        this.WanWanGlobalCacheCorruptedRoles = new CWanGlobalCacheCorruptedRolesDbScope(this.DbAccessor);
        this.JobObjectsState = new CJobObjectStateDbScope(this.DbAccessor);
        this.CryptoKeys = (ICryptoKeyDbScope) new CCryptoKeyDbScope(this.DbAccessor);
        this.GuestDatabase = new CGuestDatabaseDbScope(this.DbAccessor);
        this.SqlOibs = new CSqlOibsDbScope(this.DbAccessor);
        this.SqlBackupIntervalSessions = new CSqlBackupIntervalSessDbScope(this.DbAccessor);
        this.EncryptedImportBackups = new CEncryptedImportBackupDbScope(this.DbAccessor);
        this.ImportKeySets = new CImportKeySetDbScope(this.DbAccessor);
        this.VeeamZipRetention = new CVeeamZIPRetentionScope(this.DbAccessor);
        this.BackupService = new CBackupServiceDbScope();
        this.CurrentConnections = new CCurrentConnectionDbScope(this.DbAccessor);
        this.VcdMultiTenancy = (IVcdMultiTenancyDbScope) new CVcdMultiTenancyDbScope(this.DbAccessor);
        this.VSphereMultiTenancy = new CVSphereMultiTenancyDbScope(this.DbAccessor);
        this.CloudProvider = new CCloudProviderDbScope(this.DbAccessor);
        this.CloudProviderReporting = new CCloudProviderReportingDbScope(this.DbAccessor);
        this.CloudGate = new CCloudGateDbScope(this.DbAccessor);
        this.CloudGatewayPool = new CCloudGatewayPoolDbScope(this.DbAccessor);
        this.CloudTenant = new CCloudTenantDbScope(this.DbAccessor);
        this.CloudSimpleTenant = new CCloudSimpleTenantDbScope(this.DbAccessor);
        this.CloudVcdTenant = new CCloudVcdTenantDbScope(this.DbAccessor);
        this.CloudTenantWans = new CCloudTenantWansDbScope(this.DbAccessor);
        this.CloudSubtenants = new CCloudSubtenantsDbScope(this.DbAccessor);
        this.CloudVcdDataCenters = new CCloudVcdDataCentersDbScope(this.DbAccessor);
        this.CloudVcdStoragePolicies = new CCloudVcdStoragePoliciesDbScope(this.DbAccessor);
        this.CloudVcdNetworksDbScope = new CCloudVcdNetworksDbScope(this.DbAccessor);
        this.CloudVcdAppliances = new CCloudVcdAppliancesDbScope(this.DbAccessor);
        this.CachedObjectsIdMappingDbScope = new CCachedObjectsIdMappingDbScope(this.DbAccessor);
        this.QuotaBackups = new CQuotaBackupsDbScope(this.DbAccessor);
        this.Quotas = new CQuotaDbScope(this.DbAccessor);
        this.RepositoryQuotas = new CRepositoryQuotaDbScope(this.DbAccessor);
        this.ChildQuotas = new CChildQuotaDbScope(this.DbAccessor);
        this.TenantQuotas = new CTenantQuotaDbScope(this.DbAccessor);
        this.SubtenantQuotas = new CSubtenantQuotaDbScope(this.DbAccessor);
        this.TapeObjectInBackup = (ITapeObjectInBackupDbScope) new TapeObjectInBackupDbScope(this.DbAccessor);
        this.CloudRepositories = new CCloudRepositoriesDbScope(this.DbAccessor, (IBackupRepositoriesDataDbScope) repositoriesDbScope);
        this.CloudSessions = new CCloudSessionsDbScope(this.DbAccessor);
        this.CloudRule = new CCloudRuleDbScope(this.DbAccessor);
        this.CloudLicensing = new CCloudLicensingDbScope(this.DbAccessor);
        this.CloudTenantBackups = new CCloudTenantBackupDbScope(this.DbAccessor);
        this.CloudAppliances = new CCloudApplianceDbScope(this.DbAccessor);
        this.CloudAppliancesDefaultNetworks = new CCloudApplianceDefaultNetworkDbScope(this.DbAccessor);
        this.CloudTenantAppliances = new CCloudTenantApplianceDbScope(this.DbAccessor);
        this.CloudTenantApplianceUsages = new CCloudTenantApplianceUsagesDbScope(this.DbAccessor);
        this.CloudFailoverPlans = new CCloudFailoverPlanDbScope(this.DbAccessor);
        this.CloudPublicIps = new CCloudPublicIpDbScope(this.DbAccessor);
        this.CloudNetworkUsage = new CCloudNetworkUsageDbScope(this.DbAccessor);
        this.CloudBinFiles = new CCloudBinFilesDbScope(this.DbAccessor);
        this.CloudSessionLogInfo = new CCloudSessionLogInfoDbScope(this.DbAccessor);
        this.CloudTaskSession = new CCloudTaskSessionDbScope(this.DbAccessor);
        this.CloudGateStatistic = new CloudGateStatisticDbScope(this.DbAccessor);
        this.CloudTenantData = new CCloudTenantDataDbScope(this.DbAccessor);
        this.CloudTenantDataLinks = new CCloudTenantDataLinksDbScope(this.DbAccessor);
        this.WanAcceleratorsStatistic = new CWanAcceleratorsStatisticDbScope(this.DbAccessor);
        this.CloudReports = new CCloudReportsDbScope(this.DbAccessor);
        this.ViHardwarePlans = new CViHardwarePlanDbScope(this.DbAccessor);
        this.ViHardwarePlanDatastores = new CViHardwarePlanDatastoreDbScope(this.DbAccessor);
        this.ViHardwarePlanNetworks = new CViHardwarePlanNetworkDbScope(this.DbAccessor);
        this.ViHardwareQuotaDatastores = new CViHardwareQuotaDatastoreDbScope(this.DbAccessor);
        this.ViHardwareQuotaDatastoreUsages = new CViHardwareQuotaDatastoreUsagesDbScope(this.DbAccessor);
        this.ViHardwareQuotaNetworks = new CViHardwareQuotaNetworkDbScope(this.DbAccessor);
        this.ViHardwareQuotas = new CViHardwareQuotaDbScope(this.DbAccessor);
        this.HvHardwarePlans = new CHvHardwarePlanDbScope(this.DbAccessor);
        this.HvHardwarePlanVolumes = new CHvHardwarePlanVolumeDbScope(this.DbAccessor);
        this.HvHardwarePlanNetworks = new CHvHardwarePlanNetworkDbScope(this.DbAccessor);
        this.HvHardwareQuotaVolumes = new CHvHardwareQuotaVolumeDbScope(this.DbAccessor);
        this.HvHardwareQuotaNetworks = new CHvHardwareQuotaNetworkDbScope(this.DbAccessor);
        this.HvHardwareQuotaVolumeUsages = new CHvHardwareQuotaVolumeUsagesDbScope(this.DbAccessor);
        this.HvHardwareQuotas = new CHvHardwareQuotaDbScope(this.DbAccessor);
        this.CloudConnectHosts = new CCloudConnectHostDbScope(this.DbAccessor);
        this.CloudConnectStorages = new CCloudConnectStorageDbScope(this.DbAccessor);
        this.CloudConnectNetworks = new CCloudConnectNetworkDbScope(this.DbAccessor);
        this.TrustedCertificates = new CTrustedCertificatesDbScope(this.DbAccessor);
        this.ExtendableRepositories = new CExtendableRepositoriesDbScope(this.DbAccessor, (IBackupRepositoriesDataDbScope) repositoriesDbScope);
        this.StorageExtentAssociations = new CStorageExtentAssociationsDbScope(this.DbAccessor);
        this.StorageCopies = new CStorageCopiesDbScope(this.DbAccessor);
        this.BackupArchiveIndex = new CBackupArchiveIndicesDbScope(this.DbAccessor);
        this.ForeignRepositories = new CForeignRepositoriesDbScope(this.DbAccessor, (IBackupRepositoriesDataDbScope) repositoriesDbScope);
        this.ForeignRepositoryProviders = new CForeignRepositoryProviderDbScope(this.DbAccessor);
        this.FLRApplianceConfiguration = new CFLRApplianceConfigurationDbScope(this.DbAccessor);
        this.ProductUpdates = new CProductUpdatesDbScope(this.DbAccessor);
        this.SanVolumes = new CSanVolumesDbScope(this.DbAccessor);
        this.SanVolumeLUNs = new CSanVolumeLUNsDbScope(this.DbAccessor);
        this.SanSnapshots = new CSanSnapshotsDbScope(this.DbAccessor);
        this.SanSnapshotLUNs = new CSanSnapshotLUNsDbScope(this.DbAccessor);
        this.SanHost = new CSanHostDbScope(this.DbAccessor);
        this.SanInitiatorsByProxy = new CSanInitiatorsByProxyDbScope(this.DbAccessor);
        this.SanVolumeByProxyInitiators = new CSanVolumeByProxyInitiatorDbScope(this.DbAccessor);
        this.SanVolumeExportInfos = new CSanVolumeExportInfoDbScope(this.DbAccessor);
        this.SanVolumeExportToProxyLunId = new CSanVolumeExportToProxyLunIdDbScope(this.DbAccessor);
        this.SanStorageLocks = new CSanStorageLocksDbScope(this.DbAccessor);
        this.SanSnapshotTransferResources = new CSanSnapshotTransferResourceDbScope(this.DbAccessor);
        this.SanTargetSnapshotLuns = new CSanTargetSnapshotLunDbScope(this.DbAccessor);
        this.SanVolumeLunExportLocks = new CSanVolumeLUNExportLocksDbScope(this.DbAccessor);
        this.SanSnapshotJobInfos = new CSanSnapshotJobDbScope(this.DbAccessor);
        this.SanProxies = new CSanProxiesDbScope(this.DbAccessor);
        this.SanSnapTransferFailoverSnapshots = new CSanSnapTransferFailoverSnapshotsDbScope(this.DbAccessor);
        this.StorageEsxIpsByProxy = new CStorageEsxIpsByProxyDbScope(this.DbAccessor);
        this.SanSnapshotTransfers = new CSanSnapshotTransferDbScope(this.DbAccessor);
        this.SanVolumeOnTransferInfos = new CSanVolumeOnTransferDbScope(this.DbAccessor);
        this.SanVmBackupsOnStorage = new CSanVmBackupOnStorageDbScope(this.DbAccessor);
        this.SanJobSnasphotsOnStorageBackup = new CSanJobSnasphotsOnStorageBackupDbScope(this.DbAccessor);
        this.SanJobSnapshotLuns = new CSanJobSnapshotLunDbScope(this.DbAccessor);
        this.SanVmDisksOnDatastore = new CSanVmDiskOnDatastoreDbScope(this.DbAccessor);
        this.BackupSets = (IBackupSetsDbScope) new BackupSetsDbScope(this.DbAccessor);
        this.CatalogObjectVersions = (ICatalogObjectVersionDbScope) new CatalogObjectVersionDbScope(this.DbAccessor);
        this.Changers = (IChangersDbScope) new ChangersDbScope(this.DbAccessor);
        this.Devices = (IDevicesDbScope) new DevicesDbScope(this.DbAccessor);
        this.Directories = (IDirectoriesDbScope) new DirectoriesDbScope(this.DbAccessor);
        this.DirectoryVersions = new DirectoryVersionsDbScope(this.DbAccessor);
        this.Files = (IFilesDbScope) new FilesDbScope(this.DbAccessor);
        this.FileParts = (IFilePartsDbScope) new FilePartsDbScope(this.DbAccessor);
        this.FileVersions = (IFileVersionsDbScope) new FileVersionsDbScope(this.DbAccessor);
        this.NdmpVolumes = (INdmpVolumesDbScope) new CNdmpVolumesDbScope(this.DbAccessor);
        this.NdmpVolumeParts = (INdmpVolumePartsDbScope) new CNdmpVolumePartsDbScope(this.DbAccessor);
        this.NdmpVolumeVersions = (INdmpVolumeVersionsDbScope) new CNdmpVolumeVersionsDbScope(this.DbAccessor);
        this.HighestCommittedUsn = new HighestCommittedUsnDbScope(this.DbAccessor);
        this.TapeHosts = (ITapeHostsDbScope) new CTapeHostsDbScope(this.DbAccessor);
        this.TapeJobs = (ITapeJobsDbScope) new TapeJobsDbScope(this.DbAccessor);
        this.Libraries = (ILibrariesDbScope) new LibrariesDbScope(this.DbAccessor);
        this.LibraryDevices = (ILibraryDevicesDbScope) new LibraryDevicesDbScope(this.DbAccessor);
        this.MediaPools = (IMediaPoolsDbScope) new MediaPoolsDbScope(this.DbAccessor);
        this.MediaVault = (IMediaVaultDbScope) new MediaVaultDbScope(this.DbAccessor);
        this.MediaPoolVault = (IMediaPoolVaultDbScope) new MediaPoolVaultDbScope(this.DbAccessor);
        this.MediaSets = (IMediaSetsDbScope) new MediaSetsDbScope(this.DbAccessor);
        this.TapeSessions = (ITapeSessionsDbScope) new TapeSessionsDbScope(this.DbAccessor);
        this.TapeDrives = (ITapeDrivesDbScope) new TapeDrivesDbScope(this.DbAccessor);
        this.TapeMediums = (ITapeMediumsDbScope) new TapeMediumsDbScope(this.DbAccessor);
        this.TapeBackups = (ITapeBackupDbScope) new TapeBackupDbScope(this.DbAccessor);
        this.TapeStorages = (ITapeStoragesMapDbScope) new TapeStoragesMapDbScope(this.DbAccessor);
        this.TapeVmObjects = (ITapeVmObjectsDbScope) new TapeVmObjectsDbScope(this.DbAccessor);
        this.TapeOrphanedLinkableObjects = (ITapeOrphanedLinkableObjectsDbScope) new CTapeOrphanedLinkableObjectsDbScope(this.DbAccessor);
        this.TapeEncryptedImportedBackup = (ITapeEncryptedImportedBackupDbScope) new CTapeEncryptedImportedBackupDbScope(this.DbAccessor);
        this.TapeBackupsInfo = (ITapeBackupsInfoDbScope) new CTapeBackupsInfoDbScope(this.DbAccessor);
        this.TapeTenantRestore = (ITapeTenantRestoreDbScope) new CTapeTenantRestoreDbScope(this.DbAccessor);
        this.JobSourceRepository = new CJobSourceRepositoryDbScope(this.DbAccessor);
        this.Events = (IEventsDbScope) new CEventsDbScope(this.DbAccessor);
        this.JobVmDisksState = new CJobVmDiskStateDbScope(this.DbAccessor);
        this.TapeOibs = new CTapeOibsDbScope(this.DbAccessor);
        this.ReplicaDiskTransferStates = new CReplicaDiskTransferStatesDbScope(this.DbAccessor);
        this.OijProxies = new COijProxiesDbScope(this.DbAccessor);
        this.UpdateNotifications = new CUpdateNotificationsDbScope(this.DbAccessor);
        this.AlwaysOnGroups = new CAlwaysOnGroupsDbScope(this.DbAccessor);
        this.AlwaysOnGuestDatabases = new CAlwaysOnGuestDatabasesDbScope(this.DbAccessor);
        this.AlwaysOnListeners = new CAlwaysOnListenersDbScope(this.DbAccessor);
        this.OibsWithAlwaysOnGroups = new COibsWithAlwaysOnGroupsDbScope(this.DbAccessor);
        this.JobStates = new CJobStateDbScope(this.DbAccessor);
        this.BackupProxyGroups = new CBackupProxyGroupsDbScope(this.DbAccessor);
        this.BackupProxyGroupItems = new CBackupProxyGroupItemsDbScope(this.DbAccessor);
        this.JobProxyGroups = new CJobProxyGroupsDbScope(this.DbAccessor);
        this.BackupCopyIntervals = (IBackupCopyIntervalsDbScope) new CBackupCopyIntervalsDbScope(this.DbAccessor);
        this.BackupCopyIntervalStorages = (IBackupCopyIntervalStoragesDbScope) new CBackupCopyIntervalStoragesDbScope(this.DbAccessor);
        this.Warnings = new CWarningsDbScope(this.DbAccessor);
        this.DatastoreOptionsItem = new CDatastoreOptionsItemDbScope(this.DbAccessor);
        this.HostOperationResult = new CHostOperationResultDbScope(this.DbAccessor);
        this.UserNotifications = new CUserNotificationDbScope(this.DbAccessor);
        this.HostNetwork = new CHostNetworkDbScope(this.DbAccessor);
        this.StoreOnceRepositoryServers = new CStoreOnceRepositoryServersDbScope(this.DbAccessor);
        this.TrackedActions = new CTrackedActionDbScope(this.DbAccessor);
        this.SharedTrackedActions = new CSharedTrackedActionDbScope(this.DbAccessor);
        this.OracleGuestDb = new COracleGuestDatabaseDbScope(this.DbAccessor);
        this.OracleOibs = new COracleOibsDbScope(this.DbAccessor);
        this.OibOracleArchiveLogs = new COibOracleArchiveLogsDbScope(this.DbAccessor);
        this.SanVolumeRescanPolicy = new CSanVolumeRescanPolicyDbScope(this.DbAccessor);
        this.DatabaseMaintenance = new CDatabaseMaintenanceDbScope(this.DbAccessor);
        this.ConcurentDb = new CConcurentDbScope(this.DbAccessor);
        this.ResourceScan = new CResourceScanDbScope(this.DbAccessor);
        this.AzureSubscription = new CAzureSubscriptionDbScope(this.DbAccessor);
        this.AzureAccount = new CAzureAccountDbScope(this.DbAccessor);
        this.AzureAppliance = new CAzureApplianceDbScope(this.DbAccessor);
        this.AzureProxy = new CAzureProxyDbScope(this.DbAccessor);
        this.AmazonAppliance = new CAwsApplianceDbScope(this.DbAccessor);
        this.OrchestratedTasks = new COrchestratedTasksDbScope(this.DbAccessor);
        this.ProxyRepositoryAffinity = new CProxyRepositoryAffinityDbScope(this.DbAccessor);
        this.HostCompPreferableSettings = new CHostCompPreferableSettingsDbScope(this.DbAccessor);
        this.EpAgentManagementJobsSessions = (IEpAgentManagementJobSessionsDbScope) new CEpAgentManagementJobsSessionsDbScope(this.DbAccessor);
        this.CloudRobocopTenantServers = new CCloudRobocopTenantServersDbScope(this.DbAccessor);
        this.EpAgentPolicyConfig = new CEpAgentPolicyConfigDbScope(this.DbAccessor);
        this.EpAgents = new CEpAgentDbScope(this.DbAccessor);
        this.EpAgentLinks = new CEpAgentLinksDbScope(this.DbAccessor);
        this.EpAgentMembership = new CEpAgentMembershipDbScope(this.DbAccessor);
        this.EpContainers = new CEpContainersDbScope(this.DbAccessor);
        this.EpAgentBackupStats = new CEpAgentBackupStatsDbScope(this.DbAccessor);
        this.AgentObjectsOwnerships = (IAgentObjectsOwnershipsDbScope) new CAgentObjectsOwnershipsDbScope(this.DbAccessor);
        this.EpAgentOptionsPolicy = new CEpAgentOptionsPolicyDbScope(this.DbAccessor);
        this.EpAgentOptionsValues = new EpAgentOptionsValuesDbScope(this.DbAccessor);
        this.HostFingerprint = new CHostSshFingerprintDbScope(this.DbAccessor);
        this.EpVeeamContainerExtensions = new CEpVeeamContainerExtensionsDbScope(this.DbAccessor);
        this.AdObjects = new CAdObjectsDbScope(this.DbAccessor);
        this.CsvFiles = new CCsvFilesDbScope(this.DbAccessor);
        this.KvpDictionary = new CKvpDictionaryDbScope(this.DbAccessor);
        this.VmbApiSubscriptions = new CVmbApiSubscriptionsDbScope(this.DbAccessor);
        this.Locations = new CLocationsDbScope(this.DbAccessor);
        this.DbPluginCommon = new CDbPluginCommonScope(this.DbAccessor);
        this.OracleRMAN = new COracleRMANDbScope(this.DbAccessor);
        this.SapHana = new CSapHanaDbScope(this.DbAccessor);
        this.StatisticScope = new CVbrStatisticsDbScope(this.DbAccessor);
        this.ExternalInfrastructure = new CExternalInfrastructureDbScope(this.DbAccessor);
        this.ExternalObjectsIdMapping = (IExternalObjectsIdMappingDbScope) new CExternalObjectsIdMappingDbScope(this.DbAccessor);
        this.AmazonS3ExternalClients = (IAmazonS3ExternalClientsDbScope) new CAmazonS3ExternalClientsDbScope(this.DbAccessor);
        this.AmazonS3ExternalBackupCheckpoints = (IAmazonS3ExternalBackupCheckpointsDbScope) new CAmazonS3ExternalBackupCheckpointsDbScope(this.DbAccessor);
        this.AmazonS3ExternalClientBackups = (IAmazonS3ExternalClientBackupsDbScope) new CAmazonS3ExternalClientBackupsDbScope(this.DbAccessor);
        this.AmazonS3ExternalCacheStats = (IAmazonS3ExternalCacheStatsDbScope) new CAmazonS3ExternalCacheStatsDbScope(this.DbAccessor);
        this.AmazonS3ExternalRepositoryOwnership = (IAmazonS3ExternalRepositoryOwnershipDbScope) new CAmazonS3ExternalRepositoryOwnershipDbScope(this.DbAccessor);
        this.BackupArchiveInfos = new CBackupArchiveInfosDbScope(this.DbAccessor);
        this.BackupMonitors = new CBackupMonitorsDbScope(this.DbAccessor);
        this.DeleteBackupInfos = new CDeleteBackupInfosDbScope(this.DbAccessor);
        this.RepositoryArchiveSettings = new CRepositoryArchiveSettingsDbScope(this.DbAccessor);
        this.RepositoryLimitSettings = new CRepositoryLimitSettingsDbScope(this.DbAccessor);
      }
      catch (Exception ex)
      {
        Log.Exception(ex, (string) null);
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
        Log.Exception(ex, (string) null);
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
