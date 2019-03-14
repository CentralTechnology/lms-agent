using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.DBManager
{
  public interface IDBManager
  {
    CProductUpdatesDbScope ProductUpdates { get; }

    CJobSourceRepositoryDbScope JobSourceRepository { get; }

    IDatabaseAccessor DbAccessor { get; }

    CJobsScope Jobs { get; }

    CJobProxiesDbScope JobProxies { get; }

    IBackupProxiesDbScope BackupProxies { get; }

    IBackupRepositoriesDbScope BackupRepositories { get; }

    CBackupRepositoryPermissionsDbScope BackupRepositoryPermissions { get; }

    CDataDomainRepositoryServersDbScope DataDomainRepositoryServers { get; }

    CStoreOnceRepositoryServersDbScope StoreOnceRepositoryServers { get; }

    CWindowsRepositoryDbScope WindowsRepositories { get; }

    CCifsRepositoryDbScope CifsRepositories { get; }

    IArchiveRepositoriesDbScope ArchiveRepositories { get; }

    IExternalRepositoriesDbScope ExternalRepositories { get; }

    IRepositoriesDbScope Repositories { get; }

    CBackupsDbScope Backups { get; }

    CReplicasAuxDbScope ReplicasAux { get; }

    CBackupPlatformDbScope BackupPlatforms { get; }

    IBackupJobsDbScope BackupJobs { get; }

    IOibsDbScope<COibInfo> Oibs { get; }

    IOibsDbScope<COibEncryptedInfo> EncryptedOibs { get; }

    IPointsDbScope Points { get; }

    IStoragesDbScope Storages { get; }

    CReplicasDbScope Replicas { get; }

    CReplicaConfigurationsDbScope ReplicaConfigurations { get; }

    CRotatedDrivesDbScope RotatedDrives { get; }

    CAwayStoragesDbScope AwayStorages { get; }

    IHostsDbScope Host { get; }

    CNetworkTrafficRedirectorsDbScope NetworkTrafficRedirectors { get; }

    CPhysicalHostsDbScope PhysicalHost { get; }

    CHostCompsDbScope HostComps { get; }

    IJobsSessionsDbScope JobsSessions { get; }

    CXmlLoggerDbScope XmlLogger { get; }

    IBackupJobsSessionsDbScope BackupJobsSessions { get; }

    IBackupJobsTaskSessionsDbScope BackupJobsTaskSessions { get; }

    CRestoreJobSessionsDbScope RestoreJobsSessions { get; }

    CRestoreJobTaskSessionsDbScope RestoreJobsTaskSessions { get; }

    CFoldersDbScope Folders { get; }

    CHostsInFoldersDbScope HostsInFolders { get; }

    CHostsByJobsDbScope HostsByJobs { get; }

    IObjectsDbScope Objs { get; }

    CEncryptedObjectsDbScope EncryptedObjs { get; }

    CLicensingDbScope Licensing { get; }

    CLicensedHostsDbScope LicHosts { get; }

    CInstanceAgentsDbScope InstanceAgents { get; }

    CPluginServersLicensingDbScope PluginServersLicenses { get; }

    CAgentLicenseDbScope AgentLicenses { get; }

    CApplicationGroupsDbScope ApplicationGroups { get; }

    CDrvObjectsInApplicationGroupsDbScope DrvObjectsInApplicationGroups { get; }

    CDrvJobsDbScope DrvJobs { get; }

    CDrvJobTaskSessionsDbScope DrvJobsTaskSessions { get; }

    COptionsDbScope Options { get; }

    CObjectsInJobsDbScope ObjectsInJobs { get; }

    CDrvRolesDbScope DrvRoles { get; }

    ILinkedObjectsDbScope LinkedBackupRepositories { get; }

    ILinkedJobsDbScope LinkedJobs { get; }

    ILinkedQuotasDbScope LinkedQuotas { get; }

    ILinkedTenantsDbScope LinkedTenants { get; }

    ILinkedBackupsDbScope LinkedBackups { get; }

    CDrJobSessionsDbScope DrvJobsSessions { get; }

    IAntivirusTaskSessionDbScope AntivirusTaskSessions { get; }

    CUsersAndRolesDbScope UsersAndRoles { get; }

    ISessionsDbScope SessionBase { get; }

    CVmssDbScope Vmss { get; }

    CVirtualLabsDbScopeNew VirtualLabShort { get; }

    CSbVerificationRulesDbScope SbVerificationRules { get; }

    CReplicationInfoDbScope ReplicationInfo { get; }

    CDisksDbScope Disks { get; }

    CPhysHostDiskDbScope PhysHostDisk { get; }

    CPhysHostShareDbScope PhysHostShare { get; }

    CVolumesDbScope Volumes { get; }

    CPhysHostVolumeDbScope PhysHostVolume { get; }

    CProxyAgentsDbScope ProxyAgents { get; }

    CHvSnapshotDbScope HvSnapshots { get; }

    CHvRecoveryCheckpointDbScope HvRecoveryCheckpoints { get; }

    CVmSnapshotDbScope VmSnapshots { get; }

    CMruListDbScope MruList { get; }

    CVbrInfraDbScope VbrInfra { get; }

    CConfigurationJobsDbScope ConfigurationJob { get; }

    CSmbFileSharesDbScope SmbFileShare { get; }

    CHvVirtualLabsDbScope HvVirtualLab { get; }

    CDataSourcesDbScope DataSources { get; }

    CHierarchyScanJobDbScope HierarchyScanJob { get; }

    CWanAcceleratorsDbScope WanAccelerator { get; }

    CItemRestoreAuditDbScope ItemRestoreAudits { get; }

    CJobWanAcceleratorsDbScope JobWanAccelerator { get; }

    CWanGlobalCacheCorruptedRolesDbScope WanWanGlobalCacheCorruptedRoles { get; }

    CJobObjectStateDbScope JobObjectsState { get; }

    IEventsDbScope Events { get; }

    CJobVmDiskStateDbScope JobVmDisksState { get; }

    CTapeOibsDbScope TapeOibs { get; }

    IBackupSetsDbScope BackupSets { get; }

    ICatalogObjectVersionDbScope CatalogObjectVersions { get; }

    IChangersDbScope Changers { get; }

    IDevicesDbScope Devices { get; }

    IDirectoriesDbScope Directories { get; }

    DirectoryVersionsDbScope DirectoryVersions { get; }

    IFilesDbScope Files { get; }

    IFilePartsDbScope FileParts { get; }

    IFileVersionsDbScope FileVersions { get; }

    INdmpVolumesDbScope NdmpVolumes { get; }

    INdmpVolumePartsDbScope NdmpVolumeParts { get; }

    INdmpVolumeVersionsDbScope NdmpVolumeVersions { get; }

    HighestCommittedUsnDbScope HighestCommittedUsn { get; }

    ITapeHostsDbScope TapeHosts { get; }

    ITapeJobsDbScope TapeJobs { get; }

    ILibrariesDbScope Libraries { get; }

    ILibraryDevicesDbScope LibraryDevices { get; }

    IMediaSetsDbScope MediaSets { get; }

    IMediaPoolsDbScope MediaPools { get; }

    IMediaVaultDbScope MediaVault { get; }

    IMediaPoolVaultDbScope MediaPoolVault { get; }

    ITapeSessionsDbScope TapeSessions { get; }

    ITapeDrivesDbScope TapeDrives { get; }

    ITapeMediumsDbScope TapeMediums { get; }

    ITapeBackupDbScope TapeBackups { get; }

    ITapeStoragesMapDbScope TapeStorages { get; }

    ITapeVmObjectsDbScope TapeVmObjects { get; }

    ITapeOrphanedLinkableObjectsDbScope TapeOrphanedLinkableObjects { get; }

    ITapeBackupsInfoDbScope TapeBackupsInfo { get; }

    IDatabaseConfiguration DatabaseConfiguration { get; }

    ITapeTenantRestoreDbScope TapeTenantRestore { get; }

    CReplicaDiskTransferStatesDbScope ReplicaDiskTransferStates { get; }

    CSanHostDbScope SanHost { get; }

    CSanInitiatorsByProxyDbScope SanInitiatorsByProxy { get; }

    CSanVolumeByProxyInitiatorDbScope SanVolumeByProxyInitiators { get; }

    CSanVolumesDbScope SanVolumes { get; }

    CSanVolumeExportInfoDbScope SanVolumeExportInfos { get; }

    CSanVolumeLUNsDbScope SanVolumeLUNs { get; }

    CSanSnapshotsDbScope SanSnapshots { get; }

    CSanSnapshotLUNsDbScope SanSnapshotLUNs { get; }

    CSanVolumeExportToProxyLunIdDbScope SanVolumeExportToProxyLunId { get; }

    CSanStorageLocksDbScope SanStorageLocks { get; }

    CSanSnapshotTransferResourceDbScope SanSnapshotTransferResources { get; }

    CSanTargetSnapshotLunDbScope SanTargetSnapshotLuns { get; }

    CSanVolumeLUNExportLocksDbScope SanVolumeLunExportLocks { get; }

    CSanProxiesDbScope SanProxies { get; }

    CSanSnapshotJobDbScope SanSnapshotJobInfos { get; }

    CSanSnapTransferFailoverSnapshotsDbScope SanSnapTransferFailoverSnapshots { get; }

    CStorageEsxIpsByProxyDbScope StorageEsxIpsByProxy { get; }

    CSanSnapshotTransferDbScope SanSnapshotTransfers { get; }

    CSanVolumeOnTransferDbScope SanVolumeOnTransferInfos { get; }

    CSanVmBackupOnStorageDbScope SanVmBackupsOnStorage { get; }

    CSanJobSnasphotsOnStorageBackupDbScope SanJobSnasphotsOnStorageBackup { get; }

    CSanJobSnapshotLunDbScope SanJobSnapshotLuns { get; }

    CSanVmDiskOnDatastoreDbScope SanVmDisksOnDatastore { get; }

    ICryptoKeyDbScope CryptoKeys { get; }

    CGuestDatabaseDbScope GuestDatabase { get; }

    CSqlOibsDbScope SqlOibs { get; }

    CSqlBackupIntervalSessDbScope SqlBackupIntervalSessions { get; }

    CDatabaseMaintenanceDbScope DatabaseMaintenance { get; }

    CEncryptedImportBackupDbScope EncryptedImportBackups { get; }

    CImportKeySetDbScope ImportKeySets { get; }

    CVeeamZIPRetentionScope VeeamZipRetention { get; }

    ITapeEncryptedImportedBackupDbScope TapeEncryptedImportedBackup { get; }

    COijProxiesDbScope OijProxies { get; }

    CWanAcceleratorsStatisticDbScope WanAcceleratorsStatistic { get; }

    CBackupServiceDbScope BackupService { get; }

    CCurrentConnectionDbScope CurrentConnections { get; }

    IVcdMultiTenancyDbScope VcdMultiTenancy { get; }

    CVSphereMultiTenancyDbScope VSphereMultiTenancy { get; }

    CCloudProviderDbScope CloudProvider { get; }

    CCloudProviderReportingDbScope CloudProviderReporting { get; }

    CCloudGateDbScope CloudGate { get; }

    CCloudGatewayPoolDbScope CloudGatewayPool { get; }

    CCloudTenantDbScope CloudTenant { get; }

    CCloudSimpleTenantDbScope CloudSimpleTenant { get; }

    CCloudVcdTenantDbScope CloudVcdTenant { get; }

    CCloudSubtenantsDbScope CloudSubtenants { get; }

    CCloudVcdDataCentersDbScope CloudVcdDataCenters { get; }

    CCloudVcdStoragePoliciesDbScope CloudVcdStoragePolicies { get; }

    CCloudVcdNetworksDbScope CloudVcdNetworksDbScope { get; }

    CCloudVcdAppliancesDbScope CloudVcdAppliances { get; }

    CQuotaBackupsDbScope QuotaBackups { get; }

    CCloudTenantWansDbScope CloudTenantWans { get; }

    CQuotaDbScope Quotas { get; }

    CRepositoryQuotaDbScope RepositoryQuotas { get; }

    CChildQuotaDbScope ChildQuotas { get; }

    CTenantQuotaDbScope TenantQuotas { get; }

    CSubtenantQuotaDbScope SubtenantQuotas { get; }

    CCloudRepositoriesDbScope CloudRepositories { get; }

    CCloudSessionsDbScope CloudSessions { get; }

    CCloudRuleDbScope CloudRule { get; }

    CCloudLicensingDbScope CloudLicensing { get; }

    CCloudTenantBackupDbScope CloudTenantBackups { get; }

    CCloudApplianceDbScope CloudAppliances { get; }

    CCloudApplianceDefaultNetworkDbScope CloudAppliancesDefaultNetworks { get; }

    CCloudTenantApplianceDbScope CloudTenantAppliances { get; }

    CCloudTenantApplianceUsagesDbScope CloudTenantApplianceUsages { get; }

    CCloudFailoverPlanDbScope CloudFailoverPlans { get; }

    CCloudPublicIpDbScope CloudPublicIps { get; }

    CCloudNetworkUsageDbScope CloudNetworkUsage { get; }

    CloudGateStatisticDbScope CloudGateStatistic { get; }

    CCloudBinFilesDbScope CloudBinFiles { get; }

    CCloudTenantDataDbScope CloudTenantData { get; }

    CCloudTenantDataLinksDbScope CloudTenantDataLinks { get; }

    CCloudSessionLogInfoDbScope CloudSessionLogInfo { get; }

    CCloudTaskSessionDbScope CloudTaskSession { get; }

    CCachedObjectsIdMappingDbScope CachedObjectsIdMappingDbScope { get; }

    CCloudReportsDbScope CloudReports { get; }

    CViHardwarePlanDbScope ViHardwarePlans { get; }

    CViHardwarePlanDatastoreDbScope ViHardwarePlanDatastores { get; }

    CViHardwarePlanNetworkDbScope ViHardwarePlanNetworks { get; }

    CViHardwareQuotaDatastoreDbScope ViHardwareQuotaDatastores { get; }

    CViHardwareQuotaDatastoreUsagesDbScope ViHardwareQuotaDatastoreUsages { get; }

    CViHardwareQuotaNetworkDbScope ViHardwareQuotaNetworks { get; }

    CViHardwareQuotaDbScope ViHardwareQuotas { get; }

    CHvHardwarePlanDbScope HvHardwarePlans { get; }

    CHvHardwarePlanVolumeDbScope HvHardwarePlanVolumes { get; }

    CHvHardwarePlanNetworkDbScope HvHardwarePlanNetworks { get; }

    CHvHardwareQuotaVolumeDbScope HvHardwareQuotaVolumes { get; }

    CHvHardwareQuotaVolumeUsagesDbScope HvHardwareQuotaVolumeUsages { get; }

    CHvHardwareQuotaNetworkDbScope HvHardwareQuotaNetworks { get; }

    CHvHardwareQuotaDbScope HvHardwareQuotas { get; }

    CCloudConnectHostDbScope CloudConnectHosts { get; }

    CCloudConnectStorageDbScope CloudConnectStorages { get; }

    CCloudConnectNetworkDbScope CloudConnectNetworks { get; }

    CExtendableRepositoriesDbScope ExtendableRepositories { get; }

    CStorageExtentAssociationsDbScope StorageExtentAssociations { get; }

    CStorageCopiesDbScope StorageCopies { get; }

    CBackupArchiveIndicesDbScope BackupArchiveIndex { get; }

    CForeignRepositoriesDbScope ForeignRepositories { get; }

    CForeignRepositoryProviderDbScope ForeignRepositoryProviders { get; }

    CTrustedCertificatesDbScope TrustedCertificates { get; }

    CFLRApplianceConfigurationDbScope FLRApplianceConfiguration { get; }

    void Dispose();

    CPersistentDbConnection CreatePersistantDbConnection();

    CUpdateNotificationsDbScope UpdateNotifications { get; }

    CAlwaysOnGroupsDbScope AlwaysOnGroups { get; }

    CAlwaysOnGuestDatabasesDbScope AlwaysOnGuestDatabases { get; }

    CAlwaysOnListenersDbScope AlwaysOnListeners { get; }

    COibsWithAlwaysOnGroupsDbScope OibsWithAlwaysOnGroups { get; }

    CJobStateDbScope JobStates { get; }

    CBackupProxyGroupsDbScope BackupProxyGroups { get; }

    CBackupProxyGroupItemsDbScope BackupProxyGroupItems { get; }

    CJobProxyGroupsDbScope JobProxyGroups { get; }

    IBackupCopyIntervalsDbScope BackupCopyIntervals { get; }

    IBackupCopyIntervalStoragesDbScope BackupCopyIntervalStorages { get; }

    CWarningsDbScope Warnings { get; }

    CDatastoreOptionsItemDbScope DatastoreOptionsItem { get; }

    CHostOperationResultDbScope HostOperationResult { get; }

    CUserNotificationDbScope UserNotifications { get; }

    CHostNetworkDbScope HostNetwork { get; }

    CTrackedActionDbScope TrackedActions { get; }

    CSharedTrackedActionDbScope SharedTrackedActions { get; }

    COracleGuestDatabaseDbScope OracleGuestDb { get; }

    COracleOibsDbScope OracleOibs { get; }

    COibOracleArchiveLogsDbScope OibOracleArchiveLogs { get; }

    CSanVolumeRescanPolicyDbScope SanVolumeRescanPolicy { get; }

    CConcurentDbScope ConcurentDb { get; }

    CResourceScanDbScope ResourceScan { get; }

    CAzureSubscriptionDbScope AzureSubscription { get; }

    CAzureAccountDbScope AzureAccount { get; }

    CAzureApplianceDbScope AzureAppliance { get; }

    CAzureProxyDbScope AzureProxy { get; }

    CAwsApplianceDbScope AmazonAppliance { get; }

    COrchestratedTasksDbScope OrchestratedTasks { get; }

    CProxyRepositoryAffinityDbScope ProxyRepositoryAffinity { get; }

    CHostCompPreferableSettingsDbScope HostCompPreferableSettings { get; }

    CCloudRobocopTenantServersDbScope CloudRobocopTenantServers { get; }

    IEpAgentManagementJobSessionsDbScope EpAgentManagementJobsSessions { get; }

    CEpAgentPolicyConfigDbScope EpAgentPolicyConfig { get; }

    CEpAgentDbScope EpAgents { get; }

    CEpAgentLinksDbScope EpAgentLinks { get; }

    CEpAgentMembershipDbScope EpAgentMembership { get; }

    CEpContainersDbScope EpContainers { get; }

    CHostSshFingerprintDbScope HostFingerprint { get; }

    CEpAgentBackupStatsDbScope EpAgentBackupStats { get; }

    IAgentObjectsOwnershipsDbScope AgentObjectsOwnerships { get; }

    CEpAgentOptionsPolicyDbScope EpAgentOptionsPolicy { get; }

    EpAgentOptionsValuesDbScope EpAgentOptionsValues { get; }

    CEpVeeamContainerExtensionsDbScope EpVeeamContainerExtensions { get; }

    CAdObjectsDbScope AdObjects { get; }

    CCsvFilesDbScope CsvFiles { get; }

    CKvpDictionaryDbScope KvpDictionary { get; }

    CVmbApiSubscriptionsDbScope VmbApiSubscriptions { get; }

    CLocationsDbScope Locations { get; }

    CDbPluginCommonScope DbPluginCommon { get; }

    COracleRMANDbScope OracleRMAN { get; }

    CSapHanaDbScope SapHana { get; }

    CVbrStatisticsDbScope StatisticScope { get; }

    CExternalInfrastructureDbScope ExternalInfrastructure { get; }

    IExternalObjectsIdMappingDbScope ExternalObjectsIdMapping { get; }

    IAmazonS3ExternalClientsDbScope AmazonS3ExternalClients { get; }

    IAmazonS3ExternalBackupCheckpointsDbScope AmazonS3ExternalBackupCheckpoints { get; }

    IAmazonS3ExternalClientBackupsDbScope AmazonS3ExternalClientBackups { get; }

    IAmazonS3ExternalCacheStatsDbScope AmazonS3ExternalCacheStats { get; }

    IAmazonS3ExternalRepositoryOwnershipDbScope AmazonS3ExternalRepositoryOwnership { get; }

    CBackupArchiveInfosDbScope BackupArchiveInfos { get; }

    CBackupMonitorsDbScope BackupMonitors { get; }

    ITapeObjectInBackupDbScope TapeObjectInBackup { get; }

    CDeleteBackupInfosDbScope DeleteBackupInfos { get; }

    CRepositoryArchiveSettingsDbScope RepositoryArchiveSettings { get; }

    CRepositoryLimitSettingsDbScope RepositoryLimitSettings { get; }
  }
}
