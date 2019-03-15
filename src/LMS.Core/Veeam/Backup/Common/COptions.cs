using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LMS.Core.Veeam.Backup.Configuration.V65;
using LMS.Core.Veeam.Backup.Configuration.V80;
using Microsoft.Win32;
using Serilog;

namespace LMS.Core.Veeam.Backup.Common
{
  public sealed class COptions
  {
    public static bool UseCachedSshConnectionsDefault = false;
    public static int CloudConnectionProcessingThreadsDefault = 1;
    private static int _uiThreadId = -1;
    public static bool DisableProxyClientLoggingDefault = false;
    public static bool SkipLinuxAgentRemovalDefault = false;
    private static volatile bool ForceAbandonedSessionListCleanupInvoked = false;
    public readonly bool ShowSplashScreen = true;
    public int VBRServiceRestartWaitTimeSec = 180;
    public int AgentManagementWindowsServiceTimoutSec = 300;
    public readonly int AgentLogging = 1;
    public readonly int AgentMaxOpenFiles = 4096;
    public readonly string UseVSphereAPIVersion = "6.5";
    public readonly string Dev_ForceBackupCopyVbk = string.Empty;
    public readonly bool ConnectToGuestWithDNSName = true;
    public readonly int LicensingStateUpdatePeriod = 5;
    public TimeSpan StagedRestoreScriptExecutionTimeout = TimeSpan.FromHours(24.0);
    public TimeSpan StagedRestoreShutdownTimeout = TimeSpan.FromMinutes(15.0);
    public int StagedRestoreParallelMachineProcessing = 3;
    public readonly int Dev_ForeignJobCacheTtlMin = 10;
    public readonly int Dev_ForeignRepositoryPathCacheTtlMin = 25;
    public readonly int DebugBreakpointId = -1;
    public readonly int RenciSftpOpTimeoutInSec = 60;
    public readonly string LinAgentFolder = string.Empty;
    public readonly string LinAgentExecutableFolder = string.Empty;
    public readonly string LinAgentLogFolder = "/var/log/VeeamBackup";
    public readonly string LinAgentLogFolderAlt = "/tmp/VeeamBackup";
    public readonly string AgentAdvOptions = string.Empty;
    public readonly string AgentLogOptions = string.Empty;
    public readonly int VssPreparationTimeout = 900000;
    public readonly int GuestProcessingTimeout = 1800000;
    public readonly int GuestServiceRegistrationTimeoutMs = 600000;
    public readonly int GuestServiceRegistrationRetryCount = 10;
    public readonly int GuestServiceUninstallAfterRestoreTimeout = 600000;
    public readonly bool CheckFreeSpace = true;
    public readonly TimeSpan EndPointStartupBackupDelay = TimeSpan.FromMinutes(1.0);
    public readonly TimeSpan MissedPeriodTimeTimeout = TimeSpan.FromMinutes(3.0);
    public readonly TimeSpan MissedDailyTimeTimeout = TimeSpan.FromMinutes(15.0);
    public readonly TimeSpan MissedJobChainTimeTimeout = TimeSpan.FromMinutes(15.0);
    public readonly TimeSpan MissedMonthlyTimeTimeout = TimeSpan.FromMinutes(15.0);
    public readonly TimeSpan HvVmReadyToBackupTimeout = TimeSpan.FromHours(23.0);
    public readonly TimeSpan HvVmBusyTimeout = TimeSpan.FromHours(2.0);
    public readonly TimeSpan VwVmReadyToBackupTimeout = TimeSpan.FromDays(7.0);
    public readonly int VmReloadDelay = 15000;
    public readonly int StogareLockTimeoutSec = 300;
    public readonly int CreateSnapshotTimeout = 1800000;
    public readonly int RemoveSnapshotTimeout = 3600000;
    public readonly int ShutdownTimeout = 900000;
    public readonly string BackupServerName = "127.0.0.1";
    public readonly int BackupServerPort = 9392;
    public readonly int BackupServerSslPort = 9401;
    public readonly string BackupServerUser = string.Empty;
    public readonly string BackupServerPassword = string.Empty;
    public readonly int CloudServerPort = 10003;
    public readonly int RemoteMountInstallerPort = 6160;
    public readonly int VddkReadBufferSize = 4194304;
    public readonly int MaxVimSoapOperationTimeout = 200000;
    public readonly int MaxVimSoapRescanOperationTimeout = 900000;
    public readonly int MaxPerlSoapOperationTimeout = 100000;
    public readonly int MaxViVMPowerOnOffOperationTimeoutMin = 60;
    public readonly bool TreatDeleteSnapshotErrorAsWarning = true;
    public readonly int VddkWaitTimeoutInSec = 300;
    public readonly bool VddkVmSearchPatch = true;
    public readonly int Smb2RetryCount = 15;
    public readonly int BlockSnapshotThreshold = 2;
    public readonly bool FlrBatchTransfer = true;
    public readonly int FlrVmToolsWaitingTimeInSec = 1800;
    public readonly int FlrApplianceIpWaitingTimeInSec = 600;
    public readonly bool MultiSessionMode = true;
    public readonly bool FilterDatastores = true;
    public readonly int StorageAlignmentLogarithm = 3;
    public readonly int VdkMaxDisksCount = 20;
    public readonly TimeSpan TimeoutForVmShutdown = TimeSpan.FromSeconds(120.0);
    public readonly bool EnableHvVdk = true;
    public readonly int NetworkIdleTimeoutInHours = 24;
    public readonly int EnableTrafficControl = 1;
    public readonly int RepoBusyTimeoutSec = 300;
    public readonly int StgOversizeGbLimit = 15360;
    public readonly int ViHostConcurrentNfcConnections = 28;
    public readonly int ViHost65ConcurrentNfcConnections = 20;
    public readonly int CsvOwnershipSwitchTimeout = 20;
    public readonly int MaxCsv12SoftwareSnapshotsNum = 4;
    public readonly int MaxSmb3SnapshotsNum = 4;
    public readonly int MaxCsvHardwareSnapshotsNum = 4;
    public readonly int MaxVolHardwareSnapshotsNum = 4;
    public readonly int MaxVolSoftSnapshotsNum = 4;
    public readonly int HyperVIRCacheBlockSizeKb = 256;
    public readonly int HyperVIRCacheMemoryPerFileMb = 64;
    public readonly int HyperVIRAutoRetryCount = 3;
    public readonly bool LowerAgentPriority = true;
    public readonly int FileCacheLimitPercent = 25;
    public readonly int MaxPendingBlocks = 10000;
    public readonly int HyperVVssRestoreTimeout = 900000;
    public readonly int HyperV2016VSSCopyOnly = -1;
    public readonly int RpcMaxBinaryDataBlockSizeKb = 1024;
    public readonly int RpcRequestTimeoutSec = 3600;
    public readonly int RpcSqlLogDownloadViaWebServiceTimeoutSec = 10800;
    public readonly int GuestWebServiceSessionKeepAliveTimeoutSec = 900;
    public readonly int FLRMountPointType = 2;
    public readonly int AntivirusDefaultAction = -1;
    public readonly int AntivirusLockAttemptTimeout = 24;
    public readonly string FLRMountPoint = string.Empty;
    public readonly bool FLRHideSystemBootVolume = true;
    public readonly int ReparsePointsUpdateLimit = 1000;
    public readonly TimeSpan HvVmSizesPopulateTimeout = TimeSpan.FromMinutes(10.0);
    public readonly int LocalAgentStopTimeoutSec = 60;
    public readonly int VDDKLogLevel = 1;
    public readonly int AntivirusLogLevel = 3;
    public readonly bool SanProxyDetectEnableInquiryMatching = true;
    public readonly bool SanProxyRestoreLUNIdentifyByInquiry = true;
    public readonly int MaxEvacuateTaskCount = 1000;
    public readonly int MaxEvacuateRepositorySlots = 100;
    public readonly int VixOperationTimeoutInSeconds = 900;
    private readonly List<string> _azureEnvironmentVariables = new List<string>();
    public readonly int DeleteBackupJobThreadCount = Environment.ProcessorCount;
    public readonly string AzureEmulatorBlobEndpoint = string.Empty;
    public readonly string AzureDefaultTenantIdGUID = string.Empty;
    public readonly bool AzureManualResourceNameCheckAllowed = true;
    public readonly COptions.EPrepareAutoMount HvPrepareAutoMountOnHost = COptions.EPrepareAutoMount.Enable;
    public readonly COptions.EPrepareAutoMount HvPrepareAutoMountOffHost = COptions.EPrepareAutoMount.Enable;
    public readonly TimeSpan TapeAgentScriptTimeout = TimeSpan.FromSeconds(1800.0);
    public readonly TimeSpan TapeLibraryStateReportPeriodicity = TimeSpan.FromMinutes(60.0);
    public readonly TimeSpan ChangerElementFillCompletionTimeout = TimeSpan.FromMinutes(1.0);
    public readonly TimeSpan ChangerElementDefaultFillTimeout = TimeSpan.FromSeconds(1.0);
    public readonly TimeSpan TapeDeviceWatcherTimeout = TimeSpan.FromSeconds(30.0);
    public readonly TimeSpan TapeDeviceNotReadyTimeout = TimeSpan.FromSeconds(60.0);
    public readonly bool TapeSkipEmptyLibraryWorkaround = true;
    public readonly TimeSpan TapeLibrariesDiscoverPeriod = TimeSpan.FromMinutes(3.0);
    public readonly TimeSpan TapeLibraryDiscoverRetryTimeout = TimeSpan.FromSeconds(10.0);
    public readonly int TapeLibraryDiscoverRetryCount = 2;
    public readonly TimeSpan WaitForValidTapeTimeout = TimeSpan.FromDays(3.0);
    public readonly TimeSpan WaitForMediasetTimeout = TimeSpan.FromDays(3.0);
    public readonly TimeSpan WaitForDriveTapeTimeout = TimeSpan.FromDays(3.0);
    public readonly bool? IsStopByBackupJobAllowed = new bool?();
    public readonly TimeSpan SourceJobWaitForTapeJobTimeout = TimeSpan.FromDays(30.0);
    public readonly TimeSpan AwsCreatorStartInstanceTimeout = TimeSpan.FromMinutes(10.0);
    public readonly TimeSpan AwsCreatorStopInstanceTimeout = TimeSpan.FromMinutes(10.0);
    public readonly TimeSpan AwsAppliancePoolCheckTimeout = TimeSpan.FromMinutes(30.0);
    public readonly TimeSpan AwsVolumeAttachTimeout = TimeSpan.FromMinutes(10.0);
    public readonly TimeSpan AwsSnapshotCreationTimeout = TimeSpan.FromMinutes(240.0);
    public readonly int AwsSearchAmiMode = 2;
    public readonly bool AwsSearchAmiOnEbsOnly = true;
    public readonly int AwsDefaultSsdIops = 10000;
    public readonly int AwsMaxIopsRatio = 50;
    public readonly int SshConnectionRetryCount = 5;
    public readonly TimeSpan SshConnectionRetryTimeout = TimeSpan.FromSeconds(30.0);
    public readonly TimeSpan SshAgentStartTimeout = TimeSpan.FromMinutes(5.0);
    public readonly int AwsApiRetryCount = 5;
    public readonly TimeSpan AwsApiRetryTimeout = TimeSpan.FromSeconds(5.0);
    public readonly TimeSpan AmazonRestoreDiskAttachTimeout = TimeSpan.FromMinutes(10.0);
    public readonly TimeSpan TapeWaitForCleaningTapeTimeout = TimeSpan.FromMinutes(5.0);
    public readonly bool EnableLargeMeta = true;
    public readonly TimeSpan TapeLockResourceTimeout = TimeSpan.FromMinutes(2.0);
    public readonly TimeSpan TapeLibrarySwitchWaitForDriveLockTimeoutMin = TimeSpan.FromMinutes(1.0);
    public readonly TimeSpan TapeLibrarySwitchWaitForMediumTimeoutMin = TimeSpan.FromMinutes(1.0);
    public readonly int TapeClientTrafficCompressionLevel = 5;
    public readonly int TapeGfsTaskRetryPeriodInMins = 10;
    public readonly int TapeLockedStorageWaitingTimeoutInMins = 10080;
    public readonly ulong MessageCountBeforeCumulative = 1;
    public readonly int TapeDefaultMinSlotsToRequest = 3;
    public readonly int TapeDefaultMaxSlotsToRequest = 70;
    public readonly int TapeDatabaseCommandTimeoutSeconds = 86400;
    public readonly int TapeGfsJobRetryPeriodInHours = 48;
    public readonly TimeSpan MainRefreshInterval = TimeSpan.FromSeconds(3.0);
    public readonly TimeSpan ReporterRefreshInterval = TimeSpan.FromSeconds(3.0);
    public readonly TimeSpan TapesRefreshInterval = TimeSpan.FromSeconds(60.0);
    public readonly TimeSpan TapeWaitForDriveCleaningInterval = TimeSpan.FromMinutes(2.0);
    public readonly int TapeSequentialRestoreCacheSize = 200;
    public readonly TimeSpan TapeRestoreWaitForDriveLockTimeoutMin = TimeSpan.FromDays(3.0);
    public readonly bool TapeChainParalleling = true;
    public readonly int NDMPIncrementsLimit = 9;
    public readonly int ResourceCheckIntervalMs = 5000;
    public readonly string RestrictedNetworkSymbols = "~`!@#$%^&*+=;'><|?*:\"";
    public readonly int MaxSmbOffhostProxyCount = 10;
    public readonly int StgBlockSizeDivisor = 1;
    public readonly int SanSnapshotTimeoutMinutes = 20;
    public readonly int SanMonitorTimeoutInSeconds = 600;
    public readonly int SanMonitorConcurrentRescans = 3;
    public readonly bool SanCheckSnapshotAvailabilityOnRestore = true;
    public readonly int SanOperationTimeoutSec = 600;
    public readonly int SanMaxCommandOutputInMb = 10;
    public readonly int SanRetryCommandCount = 3;
    public readonly int SanRetryCommandDelaySec = 1;
    public readonly int SanRescanHostLeaseInitialTimeInMin = 10;
    public readonly int SanRescanFCBusTimeoutSec = 5;
    public readonly int SanRescanFcBusCheckCount = 1;
    public readonly int SanRescanFCBusAttemptCount = 100;
    public readonly int SanRescanISCSIBusByConnectTimeoutSec = 3;
    public readonly int SanRescanISCSIBusTimeoutSec = 5;
    public readonly int SanRescanISCSIBusByConnectAttemptCount = 20;
    public readonly int SanRescanISCSIBusAttemptCount = 10;
    public readonly int SanProxyLunIdTtlMin = 20;
    public readonly int SanRescanSnapshotsCountInGroup = 8;
    public readonly bool SanRescanSnapshotsThroughFcp = true;
    public readonly bool SanRescanSnapshotsThroughiSCSI = true;
    public readonly bool SanRescanSnapshotsParseFS = true;
    public readonly int SanRescanStorageSnapshotsAgentsCount = 4;
    public readonly TimeSpan SanRescanStorageSnapshotReadyTimeout = TimeSpan.FromHours(23.0);
    public readonly bool SanRescaniSCSIPriorityOverFC = true;
    public readonly bool SanMonitorFilterObjectsByRunningSessions = true;
    public readonly int SanMonitorFailedHostSkipRescanCount = 10;
    public readonly int SanVolumesProxyExportLockTimeout = 3600;
    public readonly int SanBackupSnapshotCreateLockTimeout = 3600;
    public readonly bool SanBackupFcpThroughIscsiPriority = true;
    public readonly bool SanBackupIscsiThroughFcpEnabled = true;
    public readonly int SanSnapshotTransferWaitTimeoutMin = 1440;
    public readonly int SanVolumeCopyLockTimeoutMin = 180;
    public readonly int SanViProxyMountFCLUNTimeoutMin = 180;
    public readonly int SanRescanLaunchTimeHour = 4;
    public readonly int SanRescanHostDeletionWaitTimeoutMin = 60;
    public readonly bool SanRestoreUseSharedSnapshotClone = true;
    public readonly bool SanRestoreNFSCopyVmdkFolders = true;
    public readonly int SanRestoreCreateSnapshotCloneCheckTimeoutSec = 10;
    public readonly int SanRestoreCreateSnapshotCloneTimeoutMin = 360;
    public readonly int SanRestoreMountSnapshotCloneTimeoutMin = 360;
    public readonly int SanRestoreUnmountSnapshotCloneTimeoutMin = 360;
    public readonly bool SanBackupWaitVMSnapshotDeletion = true;
    public readonly int SanBackupPrepareAgentCount = 8;
    public readonly bool SanSnapshotTransferWaitVMSnapshotDeletion = true;
    public readonly TimeSpan SanVmMountTimeoutMin = TimeSpan.FromMinutes(60.0);
    public readonly int SanRestoreUnresolvedVmfsRetryCount = 30;
    public readonly int SanRestoreAddNFSRetryCount = 3;
    public readonly bool SanBackupIscsiCheckInitiatorIqn = true;
    public readonly bool DirectNFSCheckTtlToDatastore = true;
    public readonly bool DirectNFSCheckDatastoresFromProxy = true;
    public readonly bool DirectSanForNotSCSI = true;
    public readonly bool SanBackupFromSecondaryFailIfNotLuns = true;
    public readonly bool CiscoHXUpdateSentinelSnapshot = true;
    public readonly string SanStoragePluginsPath = "C:\\Program Files\\Veeam\\Backup and Replication\\Plugins";
    public readonly string SanVeeamIqnTrimSymbolList = string.Empty;
    public readonly bool SanCleanRegistryFromUnmountDevices = true;
    public readonly bool SanModernProxyScheduling = true;
    public readonly int SanMaxMountedLunsToProxy = (int) byte.MaxValue;
    public readonly int SanMaxSnapshotCount = 100;
    public readonly int SanRescanMaxXmlMessageCount = 100;
    public readonly bool VNXBlockUseCreationTimeInSnapshotComparison = true;
    public readonly bool ViDisableDRSAtTaskBuilding = true;
    public readonly bool UseStorageSnapshotsInVeeamZip = true;
    public readonly bool SanStopRescanSessionsOnConflictingBackup = true;
    public readonly int JobLeaseTtlInMin = 60;
    public readonly int JobLeaseUpdateRate = 10;
    public readonly int PluginEmailNotifications = 1;
    public readonly int VcdConnectionTimeoutSec = 300;
    public readonly int VcdDefaultTaskTimeoutSec = 300;
    public readonly int VcdLongTaskTimeoutSec = 600;
    public readonly int VcdRestoreWaitResourcesMin = 60;
    public readonly int VcdCommandRetryCount = 3;
    public readonly int VcdRefreshVCStorageProfileCount = 3;
    public readonly int VcdStorageProfileWaitAppearRetryCount = 10;
    public readonly int VcdVAppStorageSizeInMb = 64;
    public readonly bool VcdImportAfterRestoreDisksFromStoragePod = true;
    public readonly int VcdMultiTenancyOrgCacheAfterCloseTimeoutSec = 120;
    public readonly int VcdMultiTenancyOrgCacheAfterInitTimeoutSec = 1800;
    public readonly TimeSpan UnmanagedAgentsRediscoverTime = TimeSpan.FromMinutes(7.0);
    public readonly TimeSpan DebugAgentCertificateValidityPeriodMin = TimeSpan.FromMinutes(30.0);
    public readonly string DebugAgentCertificateSignatureAlgorithmId = string.Empty;
    public readonly string DebugAgentCertificateCspProviderName = string.Empty;
    public readonly int DebugAgentCertificateCspProviderType = -1;
    public readonly TimeSpan DebugEpAgentDeletedRetentionMin = TimeSpan.FromMinutes(15.0);
    public readonly bool RetryAsyncSoapTasks = true;
    public readonly int AsyncSoapTasksRetryCount = 5;
    public readonly int SoapMaxLoginRetries = 5;
    public readonly bool BackupVcEnabled = true;
    public readonly bool ExcludeConfigurationDbFromSnapshot = true;
    public readonly int StorageLockConflictResolveTimeoutInSec = 600;
    public readonly int OrchestratedTaskLockResolveTimeoutSec = 1800;
    public readonly string HvLinuxVLRTempFolder = "C:\\VeeamFLR";
    public readonly int PowerShellManagerPort = 8732;
    public readonly int HVPSPingRetryWaitSec = 3;
    public readonly int HVPSPingRetryCount = 5;
    public readonly int MaxHvConcurrentDeletingCheckpointsForHost = 4;
    public readonly int MaxConcurrentDeletingSnapshotsForHost = 2;
    public readonly int MaxConcurrentDeletingSnapshotsForCluster = 4;
    public readonly int SnapshotDeleteSemaphoreTimeoutSec = 10800;
    public readonly int GuestVssSnapshotTtlSec = 1200;
    public readonly int MicroChangesFeederMaxLoopAttempts = 60;
    public readonly int MicroChangesFeederLoopTimeoutSec = 5;
    public readonly int BackupSyncMaxRetriesPerOib = 5;
    public readonly int BackupCopyMaxEntryBuildRetries = 5;
    public readonly int BackupCopyEntryBuildInitialRetryTimeout = 5;
    public readonly int HvSnapshotLifeTimeHour = 72;
    public readonly string[] BackupCopyCustomGfsStoreConfig = new string[0];
    public readonly bool CopyLocalRegistryHiveBeforeLoad = true;
    public readonly bool UseExportedRegistryHive = true;
    public readonly int MaxExportedRegistryHiveTimeDiffMs = 3600000;
    public readonly bool OverrideRegSeqNumber = true;
    public readonly int WanServiceTaskStateRequestDelaySec = 15;
    public readonly int BackupCopyJobFlushPeriod = 120;
    public readonly int GuestInventoryGatherWritersMetadataTimeoutMs = 3600000;
    public readonly string[] DisableBackupComponentsXmlCollectionForJobs = new string[0];
    public readonly bool UseCsvCoordinatorForHv2012OnHostBackup = true;
    public readonly string CorePath = string.Empty;
    public readonly int SourceOibLogIntervalMinutes = 60;
    public readonly int BackupCopyMaxTransformRetries = 5;
    public readonly int HvSnapshotModeCheckerTimeoutSec = 300;
    public readonly int HvSnapshotModeCheckerPeriodicitySec = 60;
    public readonly bool VMwareFailoverToVmPathFromConfig = true;
    public readonly bool VMware65CbtSnapshotCheckEnabled = true;
    public readonly int StgAfterRenameVerificationTimeoutSec = 600;
    public readonly int MaxRepositoryConnectionsRetryCount = 6;
    public readonly int MaxRepositoryConnectionsRetryTimeoutSec = 300;
    public readonly int RepositoryWriteRetryCount = 6;
    public readonly int RepositoryWriteRetryTimeoutSec = 300;
    public bool RediscoverHvClusterVmOwnerHost = true;
    public readonly bool HvSBCutVmmEthernetFeatures = true;
    public readonly bool HvSBCutBandwidthFeature = true;
    public readonly bool HvRestoreCutVmmEthernetFeatures = true;
    public readonly int SanVmSnapshotCreateSemaphoreTimeoutSec = 10800;
    public readonly int SanMaxConcurrentCreatingVmSnapshotsPerEsx = 10;
    public readonly int SanMaxConcurrentCreatingVmSnapshotsPerVc = 20;
    public readonly int SanMaxConcurrentMapDiskRegion = 8;
    public readonly int SanMapDiskRegionTimeoutSec = 10800;
    public readonly int AgentMaxReconnectRetries = 30;
    public readonly int AgentReconnectRetryIntervalSec = 10;
    public readonly bool EnableNewAgentReconnectEngine = true;
    public readonly int NewAgentReconnectEngineTimeoutSec = 1800;
    public readonly bool IsReconnectableTapeAgents = true;
    public readonly bool DisableEjectWarning = true;
    public readonly int MaxSnapshotsPerDatastore = 4;
    public readonly int AnytimePermittedSnapshotCommitsPerDatastore = 1;
    public readonly int AnytimePermittedCommonTasksPerDatastore = 1;
    public readonly bool LatencyThrottlerLoggingLevelIsNormal = true;
    public readonly int LatencyBlockTimeoutSec = 600;
    public readonly string LatencyThrottlerCoeff = "0.5";
    public readonly int SqlBackupProxySlots = 4;
    public readonly string SqlBackupInstanceDatabaseDelimiter = ":";
    public readonly string SqlBackupInstanceDatabasePairsDelimiter = ";";
    public readonly string SqlBackupDatabasesToSkip = string.Empty;
    public readonly int SqlBackupFailedIntervalsBeforeErrorPerDb = 4;
    public readonly int SqlBackupMaxPrepareRetriesPerInterval = 5;
    public readonly int SqlBackupPrepareRetryTimeoutSec = 15;
    public readonly int SqlBackupMaxTasksLoopRetriesPerInterval = 5;
    public readonly int SqlBackupTasksLoopRetryTimeoutSec = 15;
    public readonly int SqlBackupTruncateOldLogsAgeDays = 30;
    public readonly int SqlLogsAgeDaysToSkipTruncate = 7;
    public readonly int SqlLogsAgeDaysToSkipLogBackup = 7;
    public readonly bool SkipSqlConnectionCheck = true;
    public readonly int LogBackupJobWaitTimeoutMinutes = 15;
    public readonly TimeSpan IrVcdMountLeaseTimeout = TimeSpan.FromMinutes(45.0);
    public readonly TimeSpan IrMountLeaseTimeout = TimeSpan.FromMinutes(30.0);
    public readonly TimeSpan IrUnmountLeaseTimeout = TimeSpan.FromMinutes(30.0);
    public readonly TimeSpan AgentLeaseTimeout = TimeSpan.FromMinutes(5.0);
    public readonly TimeSpan RemoteAgentLeaseTimeout = TimeSpan.FromMinutes(30.0);
    public readonly TimeSpan RestoreLeaseTimeout = TimeSpan.FromMinutes(20.0);
    public readonly double AgentThrottlingAlgMixing = 0.5;
    public readonly double AgentThrottlingAlgWeightMultiplier = 32.0;
    public readonly int AgentThrottlingAlgUpdateThresholdKb = 8;
    public readonly int AgentRepositoryAlgUpdateThresholdKb = 256;
    public readonly int HyperVSnapshotCreateTimeoutMin = 60;
    public readonly int HyperVSnapshotImportTimeoutMin = 60;
    public readonly int HyperVSnapshotDeleteTimeoutMin = 60;
    public readonly bool LinuxFlrOptionCompress = true;
    public readonly bool LinuxFlrOptionCheckSum = true;
    public readonly string DebugFailureSimulator = string.Empty;
    public readonly int ConnectByIPsTimeoutSec = 300;
    public readonly int CloudSvcPortStart = 6169;
    public readonly int CloudSvcPortEnd = 6169;
    public readonly string SkipCloudHandshake = string.Empty;
    public readonly int CloudFreeSpaceWarningThreshold = 10;
    public readonly int EndPointServerSslPort = 10005;
    public readonly bool EndpointStartSslServer = true;
    public readonly int EndPointServerPort = 10001;
    public readonly bool EndPointStartServer = true;
    public readonly bool LinuxEndPointServerAsyncServer = true;
    public readonly int LinuxEndPointServerReconnectablePort = 10006;
    public readonly bool LinuxEndPointStartReconnectableServer = true;
    public readonly int LinuxEndPointServerPort = 10002;
    public readonly bool LinuxEndPointStartServer = true;
    public readonly int LinuxEndpointPackageOperationTimeoutSec = 600;
    public readonly int ConfigurationServicePort = 9380;
    public readonly int ResourcesUsageDispatcherTimeoutSec = 5;
    public readonly int MaxVmCountOnHvSoftSnapshot = 4;
    public readonly int MaxVmCountOnHvHardSnapshot = 8;
    public readonly int SCVMMConnectionTimeoutMinutes = 10;
    public TimeSpan SchedulerGlobalStartTimeDayOffset = TimeSpan.Zero;
    public readonly int HyperVDiskLatencyPermittedTasks = 2;
    public readonly string SshCertificatePath = string.Empty;
    public readonly string SshCertificatePassphrase = string.Empty;
    public readonly int DDBoostSequentialRestoreCacheSize = 100;
    public readonly int DDBoostSequentialRestoreConnectionsCount = 32;
    public readonly int DDBoostSequentialRestoreMinAgentMemoryMb = 200;
    public readonly bool DDBoostDisableReadAheadCacheForIR = true;
    public readonly COptions.EUseCifsVirtualSynthetic UseCifsVirtualSynthetic = COptions.EUseCifsVirtualSynthetic.Automatic;
    public readonly string FakeUpdateUrl = string.Empty;
    public readonly int InactiveFLRSessionTimeout = 1800;
    public readonly int LinuxFLRApplianceMemoryLimitMb = 1024;
    public readonly TimeSpan HotaddTimeoutAfterDetach = TimeSpan.Zero;
    public readonly TimeSpan SaveJobProgressTimeout = TimeSpan.FromSeconds(10.0);
    public readonly int HvSnapInProgressRetryWaitSec = 30;
    public readonly int MultipleInstancesLicensingLockWaitSec = 3600;
    public readonly string LicenseAutoUpdateServiceUrl = "https://autolk.veeam.com/json-rpc.php";
    public readonly int LicenseAutoUpdateConnectionRetryHours = 1;
    public readonly int NoNewLicenseGeneratedIntervalHours = 24;
    public readonly int NonExpiredLicenseAutoUpdateIntervalHours = 168;
    public readonly TimeSpan MaxTimeToStartSessionProcess = TimeSpan.FromSeconds(120.0);
    public readonly string ProductAutoUpdateServiceUrl = "https://autolk.veeam.com/json-rpc.php";
    public readonly string SupportCaseServiceUrl = "https://autolk.veeam.com/json-rpc.php";
    public readonly string DeployStatServiceUrl = "https://autolk.veeam.com/ama/json-rpc.php";
    public readonly int SshReconnectRetryIntervalSec = 10;
    public readonly int SshMaxReconnectRetries = 3;
    public readonly TimeSpan SshFileTransferTimeoutInSeconds = TimeSpan.FromHours(3.0);
    public readonly TimeSpan SshApplicationTransferTimeout = TimeSpan.FromMinutes(10.0);
    public readonly TimeSpan SshMaxResponseTimeoutMin = TimeSpan.FromMinutes(10.0);
    public readonly int SshRenciLibMaxSessions = 10000;
    public readonly int SshPromptTimeoutSec = 60;
    public readonly int SshConnectionCheckElevateOpTimeoutSec = 10;
    public readonly int SshAppResponseWaitTimeoutMin = 30;
    public readonly int OracleSshChunkSizeInBytes = 2048;
    public readonly int AzureApplianceSshConnectRetries = 100;
    public readonly int AmazonApplianceSshConnectRetries = 10;
    public readonly int SshWindowSize = 131072;
    public bool UseCachedSshConnections = COptions.UseCachedSshConnectionsDefault;
    public readonly int AgentManagementCRLCheckMode = 1;
    public readonly bool AgentManagementSkipOutOfDateAgents = true;
    public readonly bool CloudIgnoreInaccessibleKey = true;
    public readonly int CloudTerminateSessionIntervalMinutes = 120;
    public readonly int CloudTerminateExpiredSessionIntervalMinutes = 10;
    public readonly int CloudKeepAliveSessionIntervalMinutes = 15;
    public readonly TimeSpan CloudConnectionTimeout = TimeSpan.FromMinutes(3.0);
    public readonly TimeSpan CloudHandshakeTimeout = TimeSpan.FromMinutes(4.0);
    public readonly TimeSpan CloudInvokerDefaultExecutionTimeout = TimeSpan.FromHours(1.0);
    public readonly TimeSpan CloudInvokerDefaultAsyncStateUpdateTimeout = TimeSpan.FromSeconds(2.0);
    public readonly TimeSpan CloudResourceRequestTimeout = TimeSpan.FromMinutes(3.0);
    public readonly TimeSpan CloudResourceRequestConnectionBlockInterval = TimeSpan.FromSeconds(120.0);
    public readonly TimeSpan CloudLogAccessorMinTimeout = TimeSpan.FromSeconds(10.0);
    public readonly TimeSpan CloudReportAccessorMinTimeout = TimeSpan.FromSeconds(15.0);
    public readonly TimeSpan CloudFileSystemSynchronizationTimeout = TimeSpan.FromHours(1.0);
    public readonly int CloudConnectCRLCheckMode = 1;
    public readonly int ObjectStorageCRLCheckMode = 1;
    public readonly string CloudTapNetwork = "169.254.0.0/16";
    public readonly int NEAIPAddressWaitTimeoutSec = 600;
    public readonly int VCCReplicaIPAddressWaitTimeoutSec = 600;
    public readonly TimeSpan? CloudConnectReportTime = new TimeSpan?();
    public readonly int ReducedLoggingModeThreshold = 500;
    public readonly int TenantToTapeSkipRetiredBackupFiles = 60;
    public readonly TimeSpan ForeignInvokerDefaultExecutionTimeout = TimeSpan.FromMinutes(15.0);
    public readonly int ForeignInvokerDefaultRetryCount = 3;
    public readonly TimeSpan ForeignInvokerDefaultRetryTimeout = TimeSpan.FromSeconds(20.0);
    public readonly int ForeignInvokerServerTcpQueueSize = 16000;
    public readonly bool UseForeignAsyncInvoker = true;
    public readonly bool ForeignInvokerUseThreadPool = true;
    public readonly int VsanBestProxyDataAmountPercent = 95;
    public readonly bool EpBackupJobRetryEnabled = true;
    public readonly int EpBackupJobRetriesCount = 3;
    public readonly bool FilterApipaAddresses = true;
    public readonly int UndoFailoverPlanTimeoutSec = 10;
    public readonly int UndoFailoverPlanPackageCount = 5;
    public readonly int FailoverPlanThreadsCount = 10;
    public readonly int FailoverPlanScriptTimeoutSec = 600;
    public readonly string NetworkExtensionAppliancePreFailoverScript = string.Empty;
    public readonly string NetworkExtensionAppliancePostFailoverScript = string.Empty;
    public readonly string NetworkExtensionAppliancePreUndoFailoverScript = string.Empty;
    public readonly string NetworkExtensionAppliancePostUndoFailoverScript = string.Empty;
    public readonly bool EnablePowerOnRetries = true;
    public readonly int PowerOnRetryCount = 3;
    public readonly int PowerOnRetryTimeout = 10;
    public readonly bool ImpersonateGuestScript = true;
    public readonly string[] NLBClusterPrimaryIps = new string[0];
    public readonly int VbServiceDesktopHeapSizeKb = 131072;
    public readonly int MaxJobNameLength = 50;
    public readonly int PreJobScriptTimeoutSec = 900;
    public readonly int PostJobScriptTimeoutSec = 900;
    public readonly bool EnableBackupRepositoryFreeSpaceCheck = true;
    public readonly int BackupRepositoryFreeSpaceThresholdPercent = 10;
    public readonly int HyperVDefaultWMITimeoutSec = 3600;
    public readonly int HyperVVmTaskRescheduleLimit = 5;
    public readonly int MaxGuestScriptTimeoutSec = 600;
    private List<string> _hvCbtTestJobs = new List<string>();
    public readonly bool IsShhConnectionDisposingDissalowedInShell = true;
    public readonly bool EnableEndPointPreVssFreezeSpaceCheck = true;
    public readonly int WaMaxConnectRetries = 5;
    public readonly int SqlBackupMaxParallelThreads = 4;
    public readonly int PostProcessorMaxParallelThreads = 4;
    public readonly int ViSnapshotConsolidationRetryIntervalMinutes = 240;
    public readonly bool SkipViConfigFilesWithoutExtension = true;
    public readonly bool TagsPriorityOverContainers = true;
    public readonly int MaxNetworkNameLength = 60;
    public readonly int EpMaxBackupServerPortRetries = 10;
    public readonly int LinuxIndexingPerCommandTimeoutSec = 600;
    public readonly int LinuxIndexingUpdateDbTimeoutSec = 3600;
    public readonly bool UseIndexingOptionForVss = true;
    public readonly int DataMoverLocalFastPath = 2;
    public readonly int StoreOnceFileSessionOverhead = 2;
    public readonly int StoreOnceResourceScanTtlSec = 60;
    public readonly int StoreOnceMinFcPortCount = 64;
    public readonly bool StoreOnceSwitchToNewFileSessionsAccounting = true;
    public readonly int StoreOnceSequentialRestoreCacheSize = 100;
    public readonly int VssProxyMngtPort = 6190;
    public readonly int VssProxyPingPeriodInSec = 60;
    public readonly int SshAppPingPeriodInSec = 180;
    public readonly bool SupportPbmProfiles = true;
    public readonly int? DisableCheckAdminBlockByUac = new int?();
    public readonly int DeleteOldDbRecordsAfterPeriodInDays = 30;
    public readonly int StatisticServiceDispatchIntervalSec = 1200;
    public readonly int HyperVVmMemoryLimitDecrementInPercent = 30;
    public readonly int HvSyncTaskRescheduleTimeoutMin = 5;
    public readonly int HvWmiReconnectsCount = 1;
    public readonly int HvVmVssComponentWaitTimeout = 600;
    public int CloudConnectionProcessingThreads = COptions.CloudConnectionProcessingThreadsDefault;
    public readonly int MaxStorageSnapshotCountPerVolume = 1000;
    public readonly int DbMaintenanceStatisticsUpdateMaxRows = 100000000;
    public readonly int DbMaintenanceFragmentationThreshold = 30;
    public readonly int TombStoneRetentionPeriod = 30;
    public readonly int IsDbMaintenanceJobEnabled = 1;
    public readonly int UpdateStatisticsTimeout = 1000;
    public readonly int LongTermOperationSecDuration = 60;
    public readonly int XmlDbDataCompressionEnabled = 1;
    public readonly int XmlDbCompessionChunkSize = 1000;
    public readonly int CompressionThresholdSizeBackupTaskSessions = 7750;
    public readonly int CompressionThresholdSizeOibs = 7800;
    public readonly int IndexDefragTimeout = 3000;
    public readonly int TombstonesRetentionTimeout = 3000;
    public readonly int XmlCompressionTimeout = 3000;
    public readonly int IndexFragmentationAnalysisTimeout = 3000;
    public readonly int AuxCloudSessionsRetentionPeriod = 7;
    public readonly int AuxCloudSessionsRetentionTimeout = 3000;
    public readonly int AuxCloudSessionsChunkSize = 500;
    public readonly int TapeCleanupChunkSize = 100000;
    public readonly int ChildBackupEntitiesRemovalTimeout = 10000;
    public readonly int AppendPreparedStorageAssociationsTimeout = 1500;
    public readonly int DatabaseDeadlockRetryNumber = 5;
    public readonly int DatabaseDeadlockRetrySleepTime = 2;
    public readonly int DatabaseTimeoutRetryNumber = 2;
    public readonly int DatabaseTimeoutRetrySleepTime = 10;
    public readonly int DatabaseBrokenConnectionRetryNumber = 5;
    public readonly int DatabaseBrokenConnectionRetrySleepTime = 10;
    public readonly int XmlExceptionRetryNumber = 3;
    public readonly int XmlExceptionRetrySleepTime = 5;
    public readonly int DatabaseGenericRetryNumber = 5;
    public readonly int DatabaseGenericRetrySleepTime = 5;
    public readonly TimeSpan SobrLogTimeout = TimeSpan.Zero;
    public readonly string SobrLogTimeScheme = "5,12";
    public readonly int SobrReserveExtentSpacePercent = 1;
    public readonly int SobrEvacuatePerStorageLogThreshold = 100;
    public readonly COptions.EAgentReadOnlyCache AgentReadOnlyCache = COptions.EAgentReadOnlyCache.DisableOnlyForIncrementalWithDedupDisabled;
    public readonly StoreName BsCertificateStoreName = StoreName.My;
    public readonly StoreLocation BsCertificateLocation = StoreLocation.LocalMachine;
    public readonly string BsCertificateFriendlyName = "Veeam Backup Server Certificate";
    public readonly bool UseAdvancedMetaGenerationAlg = true;
    public readonly bool UseNewMetaFormat = true;
    public readonly int MetaGenerationTimeout = 900;
    public readonly int AlternativeWebServerPort = 8080;
    public readonly bool EnableIscsiMount = true;
    public readonly bool UseRepositoryMountServerDuringRemoteMount = true;
    public readonly bool EagerZeroedDiskRestore = true;
    public bool CloudReplicaNoStaticIpSDetectedWarning = true;
    public readonly bool FilterVirtualNetworkCardsReIP = true;
    public bool EnableHvParallelTaskBuilding = true;
    public int HvTaskBuilderThreadCount = 64;
    public readonly int LockStoragesRetryCount = 5;
    public string VbrConnectionClientName = "Veeam Backup Service client";
    public readonly bool VMBPRemoteShellEnableSP = true;
    public readonly bool VMBPRemoteShellEnableTenant = true;
    public readonly int VMBPRemoteShellGateRulesCheckingPeriodSeconds = 300;
    public readonly int VMBPRemoteShellNetworkRedirectorRunningCheckingPeriodSeconds = 120;
    public readonly int VMBPRemoteShellUpdateRulesPeriodSeconds = 60;
    public readonly int VMBPCloudNetworkRedirectorPortForTenants = 8190;
    public readonly int VMBPCloudNetworkRedirectorPortForShells = 8191;
    public readonly int VMBPShellAndRdpNetworkRedirectorPortRangeStart = 6119;
    public readonly int VMBPShellAndRdpNetworkRedirectorPortRangeEnd = 6139;
    public string VMBPShellRdpTemplateFilename = "VmbpRdpConnection.rdp";
    public readonly int SyncTimeInterval = 3600000;
    public readonly int UserAuthTokenLiveTime = 600;
    public readonly bool UseKB2047927ForGettingMountPoints = true;
    public readonly bool DisableVpnServerFirewall = true;
    public readonly string NEACustomIPExclusions = string.Empty;
    public readonly bool UseAsyncJobSizeCalculation = true;
    public readonly TimeSpan SyncRestoreJobSizeCalculationTimeout = new TimeSpan(0, 0, 5);
    public readonly TimeSpan ResourceScanDefaultPeriod = TimeSpan.FromHours(22.0);
    public readonly TimeSpan ResourceScanDefaultTTLHost = TimeSpan.FromMinutes(15.0);
    public readonly TimeSpan ResourceScanDefaultTTLRepository = TimeSpan.FromMinutes(15.0);
    public readonly TimeSpan ResourceScanDefaultTTLVpn = TimeSpan.FromMinutes(30.0);
    public readonly TimeSpan ResourceScanVpnDefaultPeriod = TimeSpan.FromMinutes(30.0);
    public readonly TimeSpan ResourceScanCheckStopPeriod = TimeSpan.FromSeconds(10.0);
    public readonly TimeSpan ResourceScanDispatchPeriod = TimeSpan.FromMinutes(5.0);
    public readonly TimeSpan ResourceScanPeriodForTries = TimeSpan.FromMinutes(10.0);
    public readonly TimeSpan ResourceScanPeriodForTriesShort = TimeSpan.FromMinutes(2.0);
    public readonly TimeSpan ResourceScanManagerLifeTime = TimeSpan.FromDays(5.0);
    public readonly TimeSpan ResourceScanSoftTtl = TimeSpan.FromMinutes(60.0);
    public readonly int ResourceScanMaxCountToUseSeparateHandles = 62;
    public readonly int CloudConnectDbCacheResyncIntervalSec = 900;
    public readonly string CloudMaintenanceModeMessage = "Service provider is currently undergoing scheduled maintenance";
    public readonly int MaxViCloudApplianceNetworksNumber = 9;
    public readonly int MaxHvCloudApplianceNetworksNumber = 11;
    public readonly COptions.EIrDisableFullBlockRead DisableFullBlockRead = COptions.EIrDisableFullBlockRead.Automatic;
    public int AzureDiskProcessingThreadCount = 8;
    public int AzureDiskProcessingBlockSize = 1048576;
    public bool EnableAzureSystemDiskConversion = true;
    public bool AzureUpdateWindowsConnectivityParams = true;
    public int AzureApplianceSSHPort = 22;
    public string AzureApplianceVmSize = "Small";
    public string AzureApplianceRmVmSize = "Standard_A0";
    public string AzureApplianceRmVmPremiumSizeDefault = "Standard_DS1";
    public string[] AzureApplianceRmVmPremiumSizes = new string[5]
    {
      "Standard_DS1",
      "Standard_DS2",
      "Standard_DS3",
      "Standard_DS4",
      "Standard_DS5"
    };
    public string AzureDefaultContainerName = "vhds";
    public int AzureRestApiRetryCount = 20;
    public int AzureRestApiRetryTimeoutSec = 30;
    public int AzureUpdateStatusRetryCount = 100;
    public string AzureAgnetDownloadLink = "http://go.microsoft.com/fwlink/?linkid=394789&clcid=0x409";
    public string AzureASMImageName = "";
    public string AzureMpImageReference = "MicrosoftWindowsServer|WindowsServer|2012-R2-Datacenter";
    public string[] AmazonDefaultProxyImage = new string[3]
    {
      "ubuntu-bionic-18.04-amd64-server-20180522-dotnetcore-2018.07.11@ubuntu@22",
      "*ubuntu-bionic-18.04-amd64-server*@ubuntu@22",
      "*ubuntu*@ubuntu@22"
    };
    public string[] AmazonDefaultLinuxImage = new string[3]
    {
      "ubuntu-bionic-18.04-amd64-server-20180522-dotnetcore-2018.07.11",
      "*ubuntu-bionic-18.04-amd64-server*",
      "*ubuntu*"
    };
    public string[] AmazonDefaultWindowsImage = new string[4]
    {
      "Windows_Server-2012-R2_RTM-English-64Bit-Base-2018.10.14",
      "*Windows_Server-2012-R2_RTM-English-64Bit-Base*",
      "*Windows_Server-1803-English-Core-Base*",
      "*Windows_Server-1803-English-Core-Base*"
    };
    public readonly bool ClientEnableUpdate = true;
    public readonly bool ServerEnableUpdate = true;
    public readonly int ClientVersionCompatibility = 4;
    public readonly int UpdateDownloadRetryCount = 3;
    public readonly int UpdateDownloadBlockSize = 65536;
    public readonly int SOBRFullCompressRate = 50;
    public readonly int SOBRIncrementCompressRate = 10;
    public readonly int SOBRTransformRate = 10;
    public readonly int SOBRSyntheticFullCompressRate = 100;
    public readonly bool EnableImportMetaCache = true;
    public readonly bool CheckDbLocks = true;
    public readonly bool CheckLinuxKernelVersion = true;
    public readonly int IscsiMountFsCheckRetriesCount = 6;
    public readonly bool IsVmfsSanAsyncRwEnabled = true;
    public readonly bool IsHotAddAsyncRwEnabled = true;
    public readonly bool VexUseSsl = true;
    public readonly int HierarchyReloadIntervalsSec = 1;
    public readonly int HierarchyRetryNumber = 3;
    public readonly int VMwareForcedHierarchyUpdatePeriod = 900;
    public readonly string[] VMwareOverrideApiVersion = new string[0];
    public readonly int BrokerServicePort = 9501;
    public readonly bool UseFastHvVmInfoCollection = true;
    public readonly int OrchestratedTaskTTL = 10;
    public readonly int EpIrDefaultMemory = 4096;
    public readonly int EpIrDefaultCpuCount = 2;
    public readonly int SnmpTrapStrMaxLength = (int) byte.MaxValue;
    public readonly bool IsAllowedToCacheOibsInfoInVmsPanel = true;
    public readonly bool UseProxyAffinityLogic = true;
    public readonly bool DenyOldProviders = true;
    public readonly bool UseIncrementalHierarchyLoading = true;
    public readonly bool SoftFsOperationInvalidFsCharsFilter = true;
    public readonly bool UseNimbleIscsiLunId = true;
    public readonly bool HvCloudReplicaMemoryWarning = true;
    public readonly int DbCollectConnectionTimeoutSec = 3600;
    public readonly TimeSpan DbClearDeletedBackupServerTimeoutSec = TimeSpan.FromHours(6.0);
    public readonly bool HyperVGuestVSSMonitor = true;
    public readonly string AwsServiceUrl = "https://ec2.amazonaws.com";
    public readonly string AwsS3ServiceUrl = "https://s3.amazonaws.com";
    public readonly string AwsPricingUrl = "https://pricing.us-east-1.amazonaws.com";
    public readonly string AwsCheckIpUrl = "http://checkip.amazonaws.com";
    public readonly TimeSpan AwsPricingExpiration = TimeSpan.FromDays(30.0);
    public readonly int HP3ParRetryCommandCount = 10;
    public readonly int HP3ParRetryCommandTimeOut = 1;
    public readonly bool BackupQuotaRepoCompatibilityChecks = true;
    public readonly int LocalRdpTcpDefaultPort = 3389;
    public readonly int MaxConcurrentComponentUpgrades = 10;
    public readonly bool LauncherIgnoreCertificateErrors = true;
    public readonly bool ObsoleteGetMountPoints = true;
    public readonly string DefaultVeeamZipPath = "";
    public readonly int ArchiveTierConnectionPointTtl = 120;
    public readonly int SOBRArchiveConsistencyDelayMin = 1440;
    public readonly TimeSpan ArchiveJobWaitForRestoreJobTimeout = TimeSpan.FromHours(1.0);
    public readonly bool UseGroundStoragesInRestore = true;
    public readonly ushort VboPort = 9194;
    public bool DisableProxyClientLogging = COptions.DisableProxyClientLoggingDefault;
    public readonly int AgentDiscoveryThreads = 10;
    public readonly int AgentConcurrentDiscovery = 5;
    public readonly int CloudConnectQuantSizeMb = 512;
    public readonly int VcdBackupQuantSizeMb = 512;
    public readonly TimeSpan CloudIntrastructureReachingCapacityThresholdMinutes = TimeSpan.FromMinutes(5.0);
    public readonly TimeSpan CloudIntrastructureOutOfCapacityThresholdMinutes = TimeSpan.FromMinutes(10.0);
    private List<string> _azureAlwaysVisibleLocations = new List<string>();
    public bool SkipLinuxAgentRemoval = COptions.SkipLinuxAgentRemovalDefault;
    public readonly int AmazonMaxSystemDiskSizeGB = 4096;
    public readonly int AmazonMaxDiskSizeGB = 16384;
    public readonly bool AsyncFlrTransfer = true;
    public readonly bool AsyncFlrTransferCompressed = true;
    public readonly int AsyncFlrTransferThreads = 24;
    public readonly bool ExternalRepositoryEnableBackupIncrementalResync = true;
    public readonly bool ExternalRepositoryWizardStartRescan = true;
    public readonly bool ExternalRepositoryWizardWaitRescan = true;
    public readonly COptions.EVmDepatureSeverityType HvMissingVMSeverity = COptions.EVmDepatureSeverityType.Error;
    public readonly int AgentServiceStartTimeoutSec = 600;
    public readonly int EpDiscoveryEmailReportTimeOfDateHours = 8;
    public readonly int EpDiscoveryEmailReportIntervalHours = 24;
    public readonly int EpDiscoveryEmailReportSendThresholdMinutes = 10;
    public readonly int EpDiscoveryEmailDispatchIntervalMinutes = 1;
    public readonly int SOBREmailReportTimeOfDayHours = 9;
    public readonly int ArchiveBackupEmailReportIntervalHours = 24;
    public readonly int ArchiveBackupEmailReportSendThresholdMinutes = 10;
    public readonly int ArchiveBackupEmailReportDispatchIntervalMinutes = 1;
    public readonly int EpPolicyEmailReportTimeOfDateHours = 8;
    public readonly int EpPolicyEmailReportSendThresholdMinutes = 10;
    public readonly int EpPolicyEmailDispatchIntervalMinutes = 1;
    public readonly int AgentPolicyConcurrentTasks = 10;
    public readonly int AgentBackupConcurrentTasks = 10;
    public readonly int AgentBackupKeepAliveTimeoutMin = 2;
    public readonly bool HyperVIgnoreNonSnapshottableDisks = true;
    public readonly int ForeignConcurrentExecutersCount = 500;
    public readonly int ForeignExecuterRetryTimeoutSec = 20;
    public int RemotingMaxParallelCallsNum = 300;
    public int RemotingSemaphoreWaitTimeoutMs = 30000;
    public int JobEventReadyToFinishTimeoutInSec = 300;
    public int AgentManagementJobStartGroupSize = 100;
    public int JobEventReadyToWorkTimeoutInSec = 300;
    public bool SearchMountLunByScisiUniqueId = true;
    public int EpAgentManagementMinSupportedVerion = 1;
    public int EpAgentManagementMaxSupportedVerion = 2;
    public readonly int AgentDeletedMachineStartTimeHours = 23;
    public readonly int SOBRArchivingScanPeriod = 4;
    public bool UseSingleAgentBackupManager = true;
    public int EpDiskManagementServiceLeaseTtlSec = 1800;
    public bool ExternalRepositoryEnableAutoResync = true;
    public bool ExternalRepositoryEnableMaintenance = true;
    public TimeSpan ExternalRepositoryResyncStartTime = TimeSpan.Zero;
    public int ExternalRepositoryMaintenanceStartTimeoutMinutes = 480;
    public string DebugArchRepoSpec = string.Empty;
    public int MaxSkipInvocationTimeoutInSec = 5;
    public readonly string ExternalRepositoryCacheWindowsRoot = "C:\\ProgramData\\Veeam\\Backup\\AmazonS3Cache";
    public readonly string ExternalRepositoryCacheLinuxRoot = "/var/veeam/backup/AmazonS3Cache";
    public readonly int ExternalRepositoryMaxCacheSizeGb = 10;
    public readonly bool ExternalRepositoryEnableCachePurge = true;
    public readonly int ExternalRepositoryCachePurgeThreshold = 20;
    public readonly bool ArchiveTierAllowAzureBlob = true;
    public readonly int SOBRArchiveIndexCompactThresholdMB = 512;
    public readonly int DeleteBackupLogSlotsLimit = -1;
    public readonly int S3MultipartUploadMinParts = 2;
    public readonly int S3ConcurrentTaskLimit = 64;
    public readonly int S3RequestRetryTotalTimeoutSec = 1800;
    public readonly int AzureConcurrentTaskLimit = 64;
    public readonly bool ExternalRepositoryEnableAutoDecrypt = true;
    public readonly int SOBRMaxArchiveTasksPercent = 50;
    public readonly bool PublicCloudEnableLocationCheck = true;
    public readonly uint NfcMaxSizeForRewritingRetry = 33554432;
    public readonly uint NfcRewritingRetryCount = 10;
    public readonly uint NfcRewritingRetryTimeoutMs = 5000;
    public readonly int SharedLockManagerTTLCheckIntervalSec = 20;
    public readonly bool IsVersion95U4Beta = true;
    public readonly int SharePointSearcherCommandTimeoutSeconds = 600;
    public readonly int GFSFullBackupRetryMultiplicator = 1;
    public bool UseGlobalExclusions = true;
    public int InfrastructureCacheExpirationSec = 900;
    public string HighestDetectedVMCVersion = "6.7";
    private int _remotingRetryCount = 5;
    public const string SystemDatastorePrefix = "VeeamBackup_";
    public const string UPGRADE_COMPONENTS_FORBIDDEN_NAME = "UpgradeComponentsForbidden";
    public const string IS_COMPONENTS_UPDATE_REQUIRED_NAME = "IsComponentsUpdateRequired";
    public const string IS_DEPLOY_STAT_UPDATED_NAME = "IsDeployStatUpdated";
    public const bool UPGRADE_COMPONENTS_FORBIDDEN_DEFAULT = false;
    public const int NETWORK_IDLE_TIMEOUT_DEFAULT = 24;
    public const int ENABLE_TRAFFIC_CONTROL_DEFAULT = 1;
    public const int DUMP_CORRUPTED_TRAFFIC_DEFAULT = 0;
    public const int REPO_BUSY_TIMEOUT_SEC_DEFAULT = 300;
    public const int STG_OVERSIZE_GB_LIMIT_DEFAULT = 15360;
    public const bool VDDK_VM_SEARCH_PATCH_DEFAULT = true;
    public const int FSAWARE_ITEM_TO_DELETE_MIN_SIZE_DEFAULT = 0;
    public const int S3_MULTIPART_UPLOAD_MIN_PARTS_DEFAULT = 2;
    public const int S3_CONCURRENT_TASK_LIMIT = 64;
    public const int S3_REQUEST_RETRY_TOTAL_TIMEOUT_SEC_DEFAULT = 1800;
    public const int AZURE_CONCURRENT_TASK_LIMIT = 64;
    public const bool UseUnbufferedAccessDefault = false;
    public const uint NFC_MAX_SIZE_FOR_REWRITING_RETRY_DEFAULT = 33554432;
    public const uint NFC_REWRITING_RETRY_COUNT_DEFAULT = 10;
    public const uint NFC_REWRITING_RETRY_TIMOEUT_MS_DEFAULT = 5000;
    public const string CLOUD_REMOTING_PORT_OPTION_NAME = "CloudServerPort";
    public const string WaitForValidTapeTimeoutName = "WaitForValidTapeTimeoutMin";
    public const string TapeGfsExpiredMediumSharingName = "TapeGfsExpiredMediumSharing";
    public const int TapeWaitForActiveStorageTimeoutDefaultInDays = 7;
    public const string TapeLockResourceTimeoutValueName = "TapeLockResourceTimeoutSec";
    public const string MessageCountBeforeCumulativeName = "MessageCountBeforeCumulative";
    public const string CLOUD_INVOKER_PORT_OPTION_NAME = "CloudSvcPort";
    public const string CLOUD_INVOKER_PORT_END_OPTION_NAME = "CloudSvcPortEnd";
    public const string CloudTapNetworkDefault = "169.254.0.0/16";
    public const int UseAutodetectStoreOnceMaxFcFileSessions = 0;
    private const long MaxLoggingError = 10;
    public readonly bool CloudConnectVcdUseUnknownNetworkStatusAsReady;
    public bool VBRServiceRestartNeeded;
    public readonly bool RTSDetailedLogging;
    public readonly bool AgentAsyncOuputHandling;
    public readonly bool DevelopMode;
    public readonly bool Dev_UseLocalhostVssProxy;
    public readonly bool Dev_ForceBackupCopyTransform;
    public readonly int Dev_SqlBackupDailyRetentionInterval;
    public readonly int Dev_OracleBackupDailyRetentionInterval;
    public readonly bool Dev_EndpointLogBackupOnVbrSide;
    public readonly bool Dev_EnableBackupCopyLogs;
    public readonly bool Dev_EnableAnyRepository;
    public readonly bool Dev_SynthFullCreator_ThrowAfterRenameStg;
    public readonly bool Dev_StagedRestoreTroubleshootMode;
    public readonly bool Dev_DisableVawRepair;
    public int AbandonedSessionListCleanupFrequency;
    public readonly bool LinFlrUsePerlSoap;
    public readonly bool DebugMode;
    public readonly bool LaunchDebugger;
    public readonly bool UseRenciSsh;
    public readonly bool RenciSshEchoEnabled;
    public readonly bool DoOracleSshElevation;
    public readonly string DebugJobManagerRenamePrefix;
    public readonly bool BackupServerAutoLogin;
    public readonly bool OldFreeESXiLicensing;
    public readonly bool VssOtherGuest;
    public readonly bool ForceCreateMissingVBK;
    public readonly int ForceDeleteBackupFiles;
    public readonly bool DisableVBKRename;
    public readonly bool EnableLegacyProcessingModes;
    public readonly bool DisableReplicaVMXOverwrite;
    public readonly bool FLRPreservePermissions;
    public readonly bool IgnoreNFSDeleteFailure;
    public readonly bool AlternateSmtpClient;
    public readonly bool LegacyReplica;
    public readonly bool ExcludeVirtualDisksExistingOnTarget;
    public readonly bool ReconnectPassThroughDisks;
    public readonly bool EnableHvVmMultiProcessing;
    public readonly bool EnableVmWareMultiProcessing;
    public readonly bool FilterVmSwap;
    public readonly bool ViRestorePreserveMAC;
    public readonly bool ViRestoreUseCheckMACAddress;
    public readonly bool ViRestoreEjectCdRomAndFloppyFromVm;
    public readonly bool ViReplicaPreserveMAC;
    public readonly bool ViReplicaUseCheckMACAddress;
    public readonly bool ViLogAllLookupServices;
    public readonly bool HvPreserveMac;
    public readonly bool DisableSmartSwitch;
    public readonly bool EnableManualUnlock;
    public readonly int DumpCorruptedTraffic;
    public readonly bool HyperVReferencePointCleanup;
    public readonly bool RpcLogInvokerCallStack;
    public readonly bool ReplicaKeepWorkingDir;
    public readonly bool ReplicaRemoveConnectedUsbDevices;
    public bool IgnoreReparsePoints;
    public readonly bool ReplicaHvContentOnCsvOwner;
    public readonly bool LogRescanFailInJob;
    public readonly bool HyperVEnableNewStableAlg;
    public readonly bool HyperVDisableNvramUpdate;
    public readonly bool PublicCloudEnableEmulators;
    public readonly bool? BarcodeScannerInstalled;
    public readonly bool UseDataDomainTapeSpecifics;
    public readonly TapeSerialNumberFilterPolicyType FilterSerialNumbers;
    public readonly bool WaitingTapeNotifcationEnabled;
    public readonly string TapeBackupAlgorythm;
    public readonly int TapeBackupParallelMode;
    public readonly bool UseDriveByLunDetectionOnly;
    public readonly bool SkipTapeAlerts;
    public readonly bool SkipIEPortState;
    public readonly bool TapeAllSyntheticCreation;
    public readonly bool TapeForceSoftwareEncryption;
    public readonly bool TapeForceSynthesizedFull;
    public readonly TapeBackupImportPolicy TapeBackupImportPolicy;
    public readonly bool AwsImportOnlyRoot;
    public readonly bool AwsApplianceElevateToRoot;
    public readonly bool AwsDetermineOsByMount;
    public readonly int AwsNativeOibDetermineMode;
    public readonly bool AwsLeaveAppliance;
    public readonly bool AwsLeaveS3Bucket;
    public readonly int MetaGeneratorPreallocSize;
    public readonly bool TapeIgnoreReturnMediaToOriginalSlot;
    public readonly bool TapeIgnoreInconsistenceMetaCheck;
    public readonly bool TapeLibraryDiscoverSkipUnknownMediumChangers;
    public readonly int TapeMinSlotsToRequest;
    public readonly int TapeMaxSlotsToRequest;
    public readonly int TapeGFSMissedBackupNotification;
    public readonly bool ForceTransform;
    public readonly bool ForceTransformReconnects;
    public readonly bool TapesAutoRefresh;
    public readonly int TapesAutoRefreshLimit;
    public readonly bool IsNDMPIncrementsLimitSpecified;
    public readonly bool TapeGfsExpiredMediumSharing;
    public readonly bool InverseVssProtocolOrder;
    public readonly bool HvVssPowerShellDirectPriorityOverNetwork;
    public readonly bool V2VTestReplicaEnabled;
    public readonly bool UIShowAllVssProvidersForVolume;
    public readonly bool SanIbmShowApiDetails;
    public readonly bool SanRescanStorageSnapshotsParallel;
    public readonly bool SanMonitorDisabled;
    public readonly bool SanMonitorAddSessionMessages;
    public readonly bool SanBackupIscsiThroughFcpPriority;
    public readonly bool SanRestoreAlwaysToActualState;
    public readonly int SanRescanLaunchTimeDayOfWeek;
    public readonly bool SanRescanUpdateVmSizeWithoutVC;
    public readonly bool SanRestoreUpdateAbsolutePathWithoutVmRef;
    public readonly bool SanRestoreFailOnRevertSnapshot;
    public readonly bool SanLogEsxDatastoresInfo;
    public readonly bool SanCanLoadUnsignedPlugins;
    public readonly bool CiscoHXDeleteSentinelSnapshot;
    public readonly bool SanUseCreationTimeInSnapshotComparison;
    public readonly bool SanForceViSnapshotCreatingForSnapOnly;
    public readonly bool VcdSkipRestoreAdvancedCheckings;
    public readonly bool VcdImportAfterRestoreDisksFromStoragePodAnyVcdVersion;
    public readonly COptions.ETemplateProcessingMode VcdTemplatesProcessingMode;
    public readonly bool DebugAgentCertificateValidityPeriodMinEnable;
    public readonly bool DebugEpAgentDeletedRetentionEnable;
    public readonly bool SoapSessionCaching;
    public readonly bool FailViBackupJobSaving;
    public readonly bool LogVssKeepSnapshotEvents;
    public readonly int VssFreezeDelay;
    public readonly bool DebugSelfThrottlingIsAllowed;
    public readonly bool HvVmIpResolveFromFQDN;
    public readonly TimeSpan BackupCopyUnavailableResourcesRetryTimeoutSec;
    public readonly bool BackupCopyStorageOptimizationsVisible;
    public readonly bool BackupCopyLookForward;
    public readonly bool BackupCopyDisableParallelProcessing;
    public readonly int HvDelayBeforeSnapshotCompleteSec;
    public readonly int HvDelayBeforeSnapshotImportCompleteSec;
    public readonly bool BackupCopyJobRecheck;
    public readonly bool BackupJobRecheck;
    public readonly bool BackupCopyForceGfsTransform;
    public readonly bool BackupCopyEnableIntervalsDebugLogging;
    public readonly bool BackupCopyJobForceCompact;
    public readonly bool BackupJobForceCompact;
    public readonly bool CompactSkipModificationTimeCheck;
    public readonly int CompactDeletedVmRetentionDays;
    public readonly bool UseImportedBackupsAsSource;
    public readonly bool ForcedSaveRegistry;
    public readonly bool WanInlineDataValidationInconsistencyWarning;
    public readonly bool EnableDBExclusions;
    public readonly bool DoNotCreateHeartbeatFile;
    public readonly bool EnableWanTransportVrpcCrcCheck;
    public readonly bool NbdFailoverRequiresHostResource;
    public readonly EViNbdCompressionType DesiredNbdCompressionType;
    public readonly bool VMwareDisableNFC;
    public readonly bool VMwareDisableAsyncIo;
    public readonly bool VMwareDisableJitWarmingUp;
    public readonly bool VMwareBlockAsyncNFC;
    public readonly bool VMwareDisableGlobalUuidPatch;
    public readonly bool CutHvVmSecuritySettings;
    public readonly bool SkipJobSourceRepositoryCheck;
    public readonly bool DatastoreIOSoapFailBehaviourIsAllowAllCommits;
    public readonly bool EnableLatencyControlOnVvol;
    public readonly bool EnableLatencyControlOnVsan;
    public readonly bool HyperVRestrictConcurrentSnapshotCreation;
    public readonly bool UpgradeComponentsForbidden;
    public readonly int SqlBackupForceTransportMode;
    public readonly int LogBackupOverrideIntervalSeconds;
    public readonly bool SqlBackupDbDetectorDetailedLogging;
    public readonly bool SqlBackupForceVix;
    public readonly bool DisableSqlLogChainIntegrityCheck;
    public readonly bool DisableSqlLogBackupErrorReports;
    public readonly bool SkipLogBackupDbFilesPresenceCheck;
    public readonly bool SqlRestoreSelectFullLogsChain;
    public readonly bool LogBackupDisableMetaGeneration;
    public readonly int SqlBackupJobsStartDelaySeconds;
    public readonly bool HvReplicaRemoveExcludedDisksOnFailback;
    public readonly bool DisableFsItemsSizeCalculation;
    public readonly string TEMP_DisableManagerOperationsExpectThis;
    public readonly bool HvUseCsvVssWriter;
    public readonly bool ForceAgentTrafficEncryption;
    public readonly bool DisablePublicIPTrafficEncryption;
    public readonly bool CloudStartServer;
    public readonly string LinuxEndpointPackagesFolder;
    public readonly bool HyperVDisableInstantRecoveryStorVSPValidation;
    public readonly bool HyperVDisableClusterNameValidation;
    public readonly bool HyperVEnableSnapshotVolumeExclusions;
    public readonly bool SkipProxyVersionCheck;
    public readonly bool ReCreateDatabase;
    public readonly bool VcdPortalIgnoreNetworkValidationDuringRestore;
    public readonly ESameHostHotAdd EnableSameHostHotaddMode;
    public readonly ESameHostDirectNFS EnableSameHostDirectNFSMode;
    public readonly bool EnableFailoverToLegacyHotadd;
    public readonly bool SkipIpAddressRangeValidation4TrafficThrottlingRule;
    public readonly bool SshFingerprintRepositoryCheckDisable;
    public readonly bool DDBoostSyntheticTransformDisabled;
    public readonly bool DDBoostSuppressEncryptionError;
    public readonly bool DDBoostDisableSequentialRestore;
    public readonly int DDBoostDeleteIncrementStorageAfterHours;
    public readonly bool Dev_DoNotForceFullsSameDedupExtent;
    public readonly bool RefsVirtualSyntheticDisabled;
    public readonly bool RefsDedupeBlockClone;
    public readonly bool ResolveCloudGatewayAddressesToIPs;
    public readonly bool HotaddRemoverCheckIndependentNonpersistent;
    public readonly int HvSnapInProgressRetryCount;
    public readonly bool LicenseAutoUpdaterDebugLogging;
    public readonly bool IsDeployStatUpdated;
    public readonly bool DisableClientSideSshKeepalive;
    public readonly bool EnableSSLv3Failback;
    public readonly bool DisableAdditionalVBRCertificateChecks;
    public readonly bool CloudForceImportGuestIndices;
    public readonly bool CloudResyncBackupVms;
    public readonly int CloudConnectEnhancedSecurityMode;
    public readonly bool CloudDisableRetrievingStatistics;
    public readonly bool CloudAllowBetaAgentAccess;
    public readonly bool CloudConnectDisableDiskMounting;
    public readonly bool UseModifiedCloudBackupsUpgradeAlg;
    public readonly bool CloudApplianceDebug;
    public readonly bool DisableCollectingLogsFromCloudAppliances;
    public readonly bool DisableTurningOffCloudAppliances;
    public readonly bool DisableSuccessCloudConnectReport;
    public readonly bool EncryptedTenantBackupsOnly;
    public readonly bool EpBackupAllowRecursiveWildcards;
    public readonly bool EpFileBackupMountVirtualVolume;
    public readonly bool EpVSSForceCopyOnly;
    public readonly bool ReplaceDefaultWindowsRE;
    public readonly bool SwitchToPersistentVssSnapshot;
    public readonly bool HvForcePersistentSnapshot;
    public readonly bool WaTransportDebugMode;
    public readonly bool HyperVCheckBackupStateInResourceScheduler;
    public readonly bool NewJobScheduler;
    public readonly int LinuxFLRApplianceKeepAliveForDebugMins;
    public readonly bool ManageByProvider;
    public readonly bool SkipUpdateToMajorVersion;
    public readonly bool CrossConfigurationRestore;
    public readonly bool UseTapeAgent32Bit;
    public bool DisableVMwareToolsNotFoundWarning;
    public readonly bool EpEnableVirtualDiskProcessing;
    public readonly bool StuckSnapshotWarning;
    public readonly bool DisableAutoSnapshotConsolidation;
    public readonly bool SkipInstantClones;
    public readonly bool ResetCBTOnDiskResize;
    public readonly bool UseCBTOnDiskResize;
    public readonly string LogExportPath;
    public readonly int StoreOnceMaxFcFileSessions;
    public readonly bool StoreOnceRtsUseRepositoryId;
    public readonly bool StoreOnceAllowStorageTransfer;
    public readonly bool StoreOnceDisableSequentialRestore;
    public readonly bool StoreOnceDisableSharedRead;
    public readonly bool StoreOnceCheckExclusiveReadLocks;
    public readonly bool DisableHvSoftwareSnapshotWhenFailoverDisabled;
    public readonly int HvSleepAfterGuestInstalledSec;
    public readonly int RestoreAuditDataRetentionInDays;
    public readonly bool CloudConnectUseAsyncInvoke;
    public readonly COptions.EDefaultLinuxAgent DefaultLinuxAgentVersion;
    public readonly int DbMaintenanceMode;
    public readonly int DbMaintenanceStatisticsUpdateMode;
    public readonly int DbMaintenanceStatisticsUpdateMinRows;
    public readonly int DbMaintenanceIndexDefragMode;
    public readonly bool SobrInverseRestoreMasterAgent;
    public readonly bool EnableRestoreSNMPTraps;
    public readonly bool Force64BitAgent;
    public readonly bool SkipSavingSharePointInfoToVbm;
    public readonly bool EnablePerFileFLRLogging;
    public readonly bool ForceMountIpCollect;
    public readonly bool CloudEnableReplicaReIP;
    public readonly bool Dev_ThrowCloudStorageLockWarnings;
    public readonly bool UseFastLockServiceSchema;
    public readonly bool ShowNewVmsColumn;
    public bool EnableLogStartStopReport;
    public readonly bool VMBPRobocopShellHookRedirectionEnabled;
    public readonly int VMBPRDPDisconnectTimeoutSecondsAfterConsoleClose;
    public readonly bool UseAsyncPSInvoke;
    public readonly bool UseUnbufferedAccess;
    public readonly bool HyperVRemoteWmiToggleLogic2012;
    public readonly bool HyperVRemoteWmiToggleLogic2015;
    public readonly bool UseVeeamFsrDriver;
    public readonly bool UseVpnOverTcp;
    public readonly bool VcdTemplatesSupport;
    public readonly bool IsZeroViVlanAllowed;
    public readonly int FsAwareItemToDeleteMinSize;
    public bool AzureRestoreForceAlternativeEFItoBIOSConversionMethod;
    public readonly bool ThrowOnUnknownUpdateFile;
    public readonly int UpdateDownloadDelay;
    public readonly bool EnableDownloadFailureSimulation;
    public readonly bool SobrForceExtentSpaceUpdate;
    public readonly bool StrictDatastoreScope;
    public readonly bool HvSharedSnapshotGroupByVolume;
    public readonly bool EnableCloudRepositoryQuotersLogs;
    public readonly bool Dev_DisableCloudLicensingChecks;
    public readonly bool VexIgnoreCertificateErrors;
    public readonly bool GenerateBiosUuidOnResolveFail;
    public readonly int NetServerEnumIntervalSec;
    public readonly bool DisableRTSTaskLogging;
    public readonly bool HyperVRCTVerboseLogging;
    public readonly bool LogViSoapOperations;
    public readonly bool ShowNimbleCloneVolumes;
    public readonly bool SkipAzureWinProxyVmDeploy;
    public readonly bool LogBrokerUpdates;
    public readonly bool RetainGfsStoragesAfterTransform;
    public readonly bool UseScsiUniqeIdComparisonForLunsByDefault;
    public readonly bool DisableTrafficControl;
    public readonly bool IsViApi65Supported;
    public readonly bool Hp3ParFixEnabled;
    public readonly bool UseKerberosAuthenticationStrategy;
    public readonly bool SkipLocalAgentConnectionCheck;
    public readonly bool EnableAsyncMountPresent;
    public readonly bool EnableAsyncMount;
    public readonly bool HvCollectAllHostAddresses;
    public readonly bool HideUpgradeWizard;
    public readonly bool OracleCollectAllPaths;
    public readonly bool LegacyHashAlgorithm;
    public readonly bool SeparateProxyClientLogging;
    public readonly int AzureMaxDiskSizeGB;
    public readonly int AzureStackMaxDiskSizeGB;
    public bool CloudConnectEnableLowLatencyMode;
    public readonly bool EnableConfigurationMaintenance;
    public readonly bool AgentManagementUseLastLogonTimeStamp;
    public readonly COptions.EVmDepatureSeverityType VMDepartureEvent;
    public readonly bool ForceNicInfoLoadingForVm;
    public readonly bool SkipVawAndValRedistsDeploy;
    public readonly int EpDiscoveryEmailReportTimeOfDateMinutes;
    public readonly int SOBREmailReportTimeOfDayMinutes;
    public readonly int EpPolicyEmailReportTimeOfDateMinutes;
    public readonly bool AgentRescanUsingADShortNames;
    public readonly bool AgentDiscoveryIgnoreOwnership;
    public readonly bool AgentDisableDistributionServerFailover;
    public bool RemotingEnableLifeTimeLog;
    public bool RemotingEnablePerfLog;
    public bool LimitParallelHelper;
    public bool AgentsDisableRECollectionWarning;
    public bool IsAddToJobNotInitializedAgentAllowed;
    public readonly int AgentDeletedMachineStartTimeMinutes;
    public readonly bool AgentDisableDeletedMachineRetention;
    public bool AzureAllowWindowsServerLicenseTypeSelection;
    public bool AzurePreferPrivateIpAddressesForProxyandLinuxAppliance;
    public bool AzureSkipCheckCanDeployVms;
    public readonly bool AgentManagementScalabilityTestMode;
    public readonly bool SOBRArchiveS3DisableTLS;
    public readonly bool SkipCertificateCheck;
    public readonly bool vPowerLegacyWriteCache;
    public readonly int SshTtyPromptExtraTimeoutMs;
    public readonly bool S3ForceVirtualHostedStyle;
    public readonly bool CloudConnectEnableHVFileShareSupport;
    public readonly bool DEV_UseOldSPSearcher;
    public readonly bool ExtendedUILogging;
    public readonly bool QMDisableRestoringTags;
    public readonly bool ObjectStorageImportIgnoreErrors;
    public readonly bool SOBRDisableEmailReport;
    private static RegistryOptionsWatcher _watcher;
    private static long _version;
    public readonly bool TempActiveGFSIsAlwaysFull;
    public bool UseVbrCredsForRemoteMount;
    private static int? _forceSpecifiedRemotingRetryCount;
    private static long _loggingErrors;

    public ITapeDevicesRegistryValue TapeDevices { get; private set; }

    public IEnumerable<string> AzureEnvironmentVariables
    {
      get
      {
        return (IEnumerable<string>) this._azureEnvironmentVariables;
      }
    }

    public TimeSpan VcdLongTaskTimeout
    {
      get
      {
        return TimeSpan.FromSeconds((double) this.VcdLongTaskTimeoutSec);
      }
    }

    public IEnumerable<string> HvCbtTestJobs
    {
      get
      {
        return (IEnumerable<string>) this._hvCbtTestJobs;
      }
    }

    public int UiThreadId
    {
      get
      {
        return COptions._uiThreadId;
      }
      set
      {
        COptions._uiThreadId = value;
      }
    }

    public bool IsComponentsUpdateRequired
    {
      get
      {
        using (IRegistryConfigurationController registryController = SProduct.Instance.CreateRegistryController(false, RegistryView.Default))
        {
          if (!registryController.IsReady)
            return false;
          return registryController.GetValue(nameof (IsComponentsUpdateRequired), false);
        }
      }
    }

    public IEnumerable<string> AzureAlwaysVisibleLocations
    {
      get
      {
        return (IEnumerable<string>) this._azureAlwaysVisibleLocations;
      }
    }

    public void SetIsComponentsUpdateRequired(bool value)
    {
      using (IRegistryConfigurationController registryController = SProduct.Instance.CreateRegistryController(true, RegistryView.Default))
      {
        if (!registryController.IsReady)
          return;
        registryController.SetValue("IsComponentsUpdateRequired", value);
      }
    }

    public bool Dev_ThrowExceptionOnSynthFullCreate
    {
      get
      {
        using (IRegistryConfigurationController registryController = SProduct.Instance.CreateRegistryController(false, RegistryView.Default))
        {
          if (!registryController.IsReady)
            return false;
          return registryController.GetValue(nameof (Dev_ThrowExceptionOnSynthFullCreate), false);
        }
      }
    }

    public bool Dev_ThrowExceptionOnGfsCompact
    {
      get
      {
        using (IRegistryConfigurationController registryController = SProduct.Instance.CreateRegistryController(false, RegistryView.Default))
        {
          if (!registryController.IsReady)
            return false;
          return registryController.GetValue(nameof (Dev_ThrowExceptionOnGfsCompact), false);
        }
      }
    }

    public bool Dev_ThrowExceptionOnStorageRename
    {
      get
      {
        using (IRegistryConfigurationController registryController = SProduct.Instance.CreateRegistryController(false, RegistryView.Default))
        {
          if (!registryController.IsReady)
            return false;
          return registryController.GetValue(nameof (Dev_ThrowExceptionOnStorageRename), false);
        }
      }
    }

    public CLoggingOptionsScope Logging { get; private set; }

    public CCloudConnectOptionsScope CloudConnect { get; private set; }

    public CCloudBinOptionsScope CloudBin { get; private set; }

    public CSobrOptionsScope Sobr { get; private set; }

    public CVeeamZipOptionsScope VeeamZip { get; private set; }

    public bool ThrottleAgentBackup { get; private set; }

    public int RemotingRetryCount
    {
      get
      {
        return this._remotingRetryCount;
      }
      set
      {
        this._remotingRetryCount = value;
        COptions._forceSpecifiedRemotingRetryCount = new int?(value);
      }
    }

    public COptions()
    {
      using (IRegistryConfigurationController registryController = SProduct.Instance.CreateRegistryController(false, RegistryView.Default))
      {
        if (!registryController.IsReady)
          return;
        this.UpgradeOracleArchiveLogCollectOptionSafe(registryController);
        COptions.Watch(registryController);
        RegistryKey registryKey = registryController.RegistryKey;
        IOptionsReader reader = COptions._watcher.GetReader(registryKey);
        this.Logging = new CLoggingOptionsScope(reader);
        this.CloudConnect = new CCloudConnectOptionsScope(reader);
        this.CloudBin = new CCloudBinOptionsScope(reader);
        this.Sobr = new CSobrOptionsScope(reader);
        this.VeeamZip = new CVeeamZipOptionsScope(reader);
        this.AbandonedSessionListCleanupFrequency = reader.GetOptionalInt32(nameof (AbandonedSessionListCleanupFrequency), this.AbandonedSessionListCleanupFrequency);
        this.OnAbandonedSessionListCleanupFrequencyReaded();
        this.RTSDetailedLogging = reader.GetOptionalBoolean(nameof (RTSDetailedLogging), this.RTSDetailedLogging);
        this.AgentLogging = reader.GetOptionalInt32(nameof (AgentLogging), this.AgentLogging);
        this.AgentAsyncOuputHandling = reader.GetOptionalBoolean(nameof (AgentAsyncOuputHandling), this.AgentAsyncOuputHandling);
        this.AgentMaxOpenFiles = reader.GetOptionalInt32(nameof (AgentMaxOpenFiles), this.AgentMaxOpenFiles);
        this.DevelopMode = reader.GetOptionalBoolean(nameof (DevelopMode), this.DevelopMode);
        this.DevelopMode = reader.GetOptionalBoolean(nameof (DevelopMode), this.DevelopMode);
        this.Dev_UseLocalhostVssProxy = reader.GetOptionalBoolean(nameof (Dev_UseLocalhostVssProxy), this.Dev_UseLocalhostVssProxy);
        this.UseVSphereAPIVersion = reader.GetOptionalString(nameof (UseVSphereAPIVersion), this.UseVSphereAPIVersion);
        this.HighestDetectedVMCVersion = reader.GetOptionalString(nameof (HighestDetectedVMCVersion), this.HighestDetectedVMCVersion);
        this.Dev_ForceBackupCopyVbk = reader.GetOptionalString(nameof (Dev_ForceBackupCopyVbk), this.Dev_ForceBackupCopyVbk);
        this.Dev_ForceBackupCopyTransform = reader.GetOptionalBoolean(nameof (Dev_ForceBackupCopyTransform), this.Dev_ForceBackupCopyTransform);
        this.Dev_SqlBackupDailyRetentionInterval = reader.GetOptionalInt32(nameof (Dev_SqlBackupDailyRetentionInterval), this.Dev_SqlBackupDailyRetentionInterval);
        this.Dev_OracleBackupDailyRetentionInterval = reader.GetOptionalInt32(nameof (Dev_OracleBackupDailyRetentionInterval), this.Dev_OracleBackupDailyRetentionInterval);
        this.Dev_EndpointLogBackupOnVbrSide = reader.GetOptionalBoolean(nameof (Dev_EndpointLogBackupOnVbrSide), this.Dev_EndpointLogBackupOnVbrSide);
        this.Dev_EnableBackupCopyLogs = reader.GetOptionalBoolean(nameof (Dev_EnableBackupCopyLogs), this.Dev_EnableBackupCopyLogs);
        this.Dev_EnableAnyRepository = reader.GetOptionalBoolean(nameof (Dev_EnableAnyRepository), this.Dev_EnableAnyRepository);
        this.ConnectToGuestWithDNSName = reader.GetOptionalBoolean(nameof (ConnectToGuestWithDNSName), this.ConnectToGuestWithDNSName);
        this.Dev_SynthFullCreator_ThrowAfterRenameStg = reader.GetOptionalBoolean(nameof (Dev_SynthFullCreator_ThrowAfterRenameStg), this.Dev_SynthFullCreator_ThrowAfterRenameStg);
        this.LicensingStateUpdatePeriod = reader.GetOptionalInt32(nameof (LicensingStateUpdatePeriod), this.LicensingStateUpdatePeriod);
        this.Dev_ForeignJobCacheTtlMin = reader.GetOptionalInt32(nameof (Dev_ForeignJobCacheTtlMin), this.Dev_ForeignJobCacheTtlMin);
        this.Dev_ForeignRepositoryPathCacheTtlMin = reader.GetOptionalInt32(nameof (Dev_ForeignRepositoryPathCacheTtlMin), this.Dev_ForeignRepositoryPathCacheTtlMin);
        this.Dev_DisableVawRepair = reader.GetOptionalBoolean(nameof (Dev_DisableVawRepair), this.Dev_DisableVawRepair);
        this.StogareLockTimeoutSec = reader.GetOptionalInt32("StorageLockTimeout", this.StogareLockTimeoutSec);
        this.StagedRestoreScriptExecutionTimeout = TimeSpan.FromHours((double) reader.GetOptionalInt32(nameof (StagedRestoreScriptExecutionTimeout), (int) this.StagedRestoreScriptExecutionTimeout.TotalHours));
        this.StagedRestoreShutdownTimeout = TimeSpan.FromMinutes((double) reader.GetOptionalInt32(nameof (StagedRestoreShutdownTimeout), (int) this.StagedRestoreShutdownTimeout.TotalMinutes));
        this.StagedRestoreParallelMachineProcessing = reader.GetOptionalInt32(nameof (StagedRestoreParallelMachineProcessing), this.StagedRestoreParallelMachineProcessing);
        this.Dev_StagedRestoreTroubleshootMode = reader.GetOptionalBoolean(nameof (Dev_StagedRestoreTroubleshootMode), false);
        this.LinFlrUsePerlSoap = reader.GetOptionalBoolean(nameof (LinFlrUsePerlSoap), false);
        this.UseRenciSsh = reader.GetOptionalBoolean(nameof (UseRenciSsh), false);
        this.RenciSshEchoEnabled = reader.GetOptionalBoolean(nameof (RenciSshEchoEnabled), false);
        this.DoOracleSshElevation = reader.GetOptionalBoolean(nameof (DoOracleSshElevation), true);
        this.RenciSftpOpTimeoutInSec = reader.GetOptionalInt32(nameof (RenciSftpOpTimeoutInSec), this.RenciSftpOpTimeoutInSec);
        this.VssProxyMngtPort = reader.GetOptionalInt32(nameof (VssProxyMngtPort), this.VssProxyMngtPort);
        this.VssProxyPingPeriodInSec = reader.GetOptionalInt32(nameof (VssProxyPingPeriodInSec), this.VssProxyPingPeriodInSec);
        this.SshAppPingPeriodInSec = reader.GetOptionalInt32(nameof (SshAppPingPeriodInSec), this.SshAppPingPeriodInSec);
        this.PluginEmailNotifications = reader.GetOptionalInt32(nameof (PluginEmailNotifications), this.PluginEmailNotifications);
        this.LinAgentExecutableFolder = reader.GetOptionalString(nameof (LinAgentExecutableFolder), this.LinAgentExecutableFolder);
        this.LinAgentFolder = reader.GetOptionalString(nameof (LinAgentFolder), this.LinAgentFolder);
        this.LinAgentLogFolder = reader.GetOptionalString(nameof (LinAgentLogFolder), this.LinAgentLogFolder);
        this.LinAgentLogFolderAlt = reader.GetOptionalString(nameof (LinAgentLogFolderAlt), this.LinAgentLogFolderAlt);
        this.DebugMode = reader.GetOptionalBoolean(nameof (DebugMode), false);
        this.LaunchDebugger = reader.GetOptionalBoolean(nameof (LaunchDebugger), false);
        this.DebugJobManagerRenamePrefix = reader.GetOptionalString(nameof (DebugJobManagerRenamePrefix), this.DebugJobManagerRenamePrefix);
        this.AgentAdvOptions = reader.GetOptionalString(nameof (AgentAdvOptions), this.AgentAdvOptions, RegistryValueOptions.DoNotExpandEnvironmentNames);
        this.AgentLogOptions = reader.GetOptionalString(nameof (AgentLogOptions), this.AgentLogOptions, RegistryValueOptions.DoNotExpandEnvironmentNames);
        this.VssPreparationTimeout = reader.GetOptionalInt32(nameof (VssPreparationTimeout), this.VssPreparationTimeout);
        this.GuestProcessingTimeout = reader.GetOptionalInt32(nameof (GuestProcessingTimeout), this.GuestProcessingTimeout);
        this.GuestServiceRegistrationTimeoutMs = reader.GetOptionalInt32(nameof (GuestServiceRegistrationTimeoutMs), this.GuestServiceRegistrationTimeoutMs);
        this.GuestServiceRegistrationRetryCount = reader.GetOptionalInt32(nameof (GuestServiceRegistrationRetryCount), this.GuestServiceRegistrationRetryCount);
        this.GuestServiceUninstallAfterRestoreTimeout = reader.GetOptionalInt32(nameof (GuestServiceUninstallAfterRestoreTimeout), this.GuestServiceUninstallAfterRestoreTimeout);
        this.CheckFreeSpace = reader.GetOptionalBoolean(nameof (CheckFreeSpace), this.CheckFreeSpace);
        this.SobrInverseRestoreMasterAgent = reader.GetOptionalBoolean(nameof (SobrInverseRestoreMasterAgent), this.SobrInverseRestoreMasterAgent);
        this.SobrReserveExtentSpacePercent = reader.GetOptionalInt32(nameof (SobrReserveExtentSpacePercent), this.SobrReserveExtentSpacePercent);
        this.SobrEvacuatePerStorageLogThreshold = reader.GetOptionalInt32(nameof (SobrEvacuatePerStorageLogThreshold), this.SobrEvacuatePerStorageLogThreshold);
        string optionalString1 = reader.GetOptionalString(nameof (MissedPeriodTimeTimeout), (string) null);
        TimeSpan result1;
        if (optionalString1 != null && TimeSpan.TryParse(optionalString1, out result1))
          this.MissedPeriodTimeTimeout = result1;
        this.MissedPeriodTimeTimeout = TimeSpan.FromMinutes((double) reader.GetOptionalInt32("MissedPeriodTimeTimeoutMin", (int) this.MissedPeriodTimeTimeout.TotalMinutes));
        this.SobrLogTimeout = TimeSpan.FromSeconds((double) reader.GetOptionalInt32("SobrLogTimeoutSec", (int) this.SobrLogTimeout.TotalSeconds));
        string optionalString2 = reader.GetOptionalString(nameof (SobrLogTimeScheme), this.SobrLogTimeScheme, RegistryValueOptions.DoNotExpandEnvironmentNames);
        try
        {
          ((IEnumerable<string>) optionalString2.Split(',')).Select<string, int>((Func<string, int>) (n => Convert.ToInt32(n))).ToArray<int>();
          this.SobrLogTimeScheme = optionalString2;
        }
        catch (Exception ex)
        {
          this.HandleError("Exception at parsing SobrLogTimeScheme");
        }
        string optionalString3 = reader.GetOptionalString(nameof (EndPointStartupBackupDelay), (string) null);
        if (optionalString3 != null && TimeSpan.TryParse(optionalString3, out result1))
          this.EndPointStartupBackupDelay = result1;
        this.EndPointStartupBackupDelay = TimeSpan.FromMinutes((double) reader.GetOptionalInt32("EndPointStartupBackupDelayMin", (int) this.EndPointStartupBackupDelay.TotalMinutes));
        string optionalString4 = reader.GetOptionalString(nameof (MissedDailyTimeTimeout), (string) null);
        if (optionalString4 != null && TimeSpan.TryParse(optionalString4, out result1))
          this.MissedDailyTimeTimeout = result1;
        this.MissedDailyTimeTimeout = TimeSpan.FromMinutes((double) reader.GetOptionalInt32("MissedDailyTimeTimeoutMin", (int) this.MissedDailyTimeTimeout.TotalMinutes));
        string optionalString5 = reader.GetOptionalString(nameof (MissedMonthlyTimeTimeout), (string) null);
        if (optionalString5 != null && TimeSpan.TryParse(optionalString5, out result1))
          this.MissedMonthlyTimeTimeout = result1;
        this.MissedMonthlyTimeTimeout = TimeSpan.FromMinutes((double) reader.GetOptionalInt32("MissedMonthlyTimeTimeoutMin", (int) this.MissedMonthlyTimeTimeout.TotalMinutes));
        string optionalString6 = reader.GetOptionalString(nameof (MissedJobChainTimeTimeout), (string) null);
        if (optionalString6 != null && TimeSpan.TryParse(optionalString6, out result1))
          this.MissedJobChainTimeTimeout = result1;
        this.MissedJobChainTimeTimeout = TimeSpan.FromMinutes((double) reader.GetOptionalInt32(nameof (MissedJobChainTimeTimeout), (int) this.MissedJobChainTimeTimeout.TotalMinutes));
        string optionalString7 = reader.GetOptionalString(nameof (HvVmReadyToBackupTimeout), (string) null);
        if (optionalString7 != null && TimeSpan.TryParse(optionalString7, out result1))
          this.HvVmReadyToBackupTimeout = result1;
        this.HvVmReadyToBackupTimeout = TimeSpan.FromMinutes((double) reader.GetOptionalInt32("HvVmReadyToBackupTimeoutMin", (int) this.HvVmReadyToBackupTimeout.TotalMinutes));
        string optionalString8 = reader.GetOptionalString("HvVmIsBusyTimeout", (string) null);
        if (optionalString8 != null && TimeSpan.TryParse(optionalString8, out result1))
          this.HvVmBusyTimeout = result1;
        this.HvVmBusyTimeout = TimeSpan.FromMinutes((double) reader.GetOptionalInt32("HvVmIsBusyTimeoutMin", (int) this.HvVmBusyTimeout.TotalMinutes));
        string optionalString9 = reader.GetOptionalString(nameof (VwVmReadyToBackupTimeout), (string) null);
        if (optionalString9 != null && TimeSpan.TryParse(optionalString9, out result1))
          this.VwVmReadyToBackupTimeout = result1;
        this.VwVmReadyToBackupTimeout = TimeSpan.FromMinutes((double) reader.GetOptionalInt32("VwVmReadyToBackupTimeoutMin", (int) this.VwVmReadyToBackupTimeout.TotalMinutes));
        string optionalString10 = reader.GetOptionalString(nameof (TimeoutForVmShutdown), (string) null);
        if (optionalString10 != null && TimeSpan.TryParse(optionalString10, out result1))
          this.TimeoutForVmShutdown = result1;
        this.TimeoutForVmShutdown = TimeSpan.FromSeconds((double) reader.GetOptionalInt32("TimeoutForVmShutdownSec", (int) this.TimeoutForVmShutdown.TotalSeconds));
        this.UnmanagedAgentsRediscoverTime = TimeSpan.FromSeconds((double) reader.GetOptionalInt32("UnmanagedAgentsRediscoverTimeSec", (int) this.UnmanagedAgentsRediscoverTime.TotalSeconds));
        this.DebugAgentCertificateValidityPeriodMin = TimeSpan.FromMinutes((double) reader.GetOptionalInt32(nameof (DebugAgentCertificateValidityPeriodMin), (int) this.DebugAgentCertificateValidityPeriodMin.TotalMinutes));
        this.DebugAgentCertificateValidityPeriodMinEnable = reader.GetOptionalBoolean(nameof (DebugAgentCertificateValidityPeriodMinEnable), this.DebugAgentCertificateValidityPeriodMinEnable);
        this.DebugAgentCertificateSignatureAlgorithmId = reader.GetOptionalString(nameof (DebugAgentCertificateSignatureAlgorithmId), this.DebugAgentCertificateSignatureAlgorithmId);
        this.DebugAgentCertificateCspProviderName = reader.GetOptionalString(nameof (DebugAgentCertificateCspProviderName), this.DebugAgentCertificateCspProviderName);
        this.DebugAgentCertificateCspProviderType = reader.GetOptionalInt32(nameof (DebugAgentCertificateCspProviderType), this.DebugAgentCertificateCspProviderType);
        this.DebugEpAgentDeletedRetentionMin = TimeSpan.FromMinutes((double) reader.GetOptionalInt32(nameof (DebugEpAgentDeletedRetentionMin), (int) this.DebugEpAgentDeletedRetentionMin.TotalMinutes));
        this.DebugEpAgentDeletedRetentionEnable = reader.GetOptionalBoolean(nameof (DebugEpAgentDeletedRetentionEnable), this.DebugEpAgentDeletedRetentionEnable);
        this.VmReloadDelay = reader.GetOptionalInt32(nameof (VmReloadDelay), this.VmReloadDelay);
        this.LongTermOperationSecDuration = reader.GetOptionalInt32(nameof (LongTermOperationSecDuration), this.LongTermOperationSecDuration);
        this.CreateSnapshotTimeout = reader.GetOptionalInt32(nameof (CreateSnapshotTimeout), this.CreateSnapshotTimeout);
        this.RemoveSnapshotTimeout = reader.GetOptionalInt32(nameof (RemoveSnapshotTimeout), this.RemoveSnapshotTimeout);
        this.ShutdownTimeout = reader.GetOptionalInt32(nameof (ShutdownTimeout), this.ShutdownTimeout);
        this.BackupServerName = reader.GetOptionalString(nameof (BackupServerName), this.BackupServerName);
        this.BackupServerPort = reader.GetOptionalInt32(nameof (BackupServerPort), this.BackupServerPort);
        this.BackupServerSslPort = reader.GetOptionalInt32("SecureConnectionsPort", this.BackupServerSslPort);
        this.CloudServerPort = reader.GetOptionalInt32(nameof (CloudServerPort), this.CloudServerPort);
        this.RemoteMountInstallerPort = reader.GetOptionalInt32(nameof (RemoteMountInstallerPort), this.RemoteMountInstallerPort);
        this.OldFreeESXiLicensing = reader.GetOptionalBoolean("LicenseCompatibilityMode", this.OldFreeESXiLicensing);
        this.VssOtherGuest = reader.GetOptionalBoolean(nameof (VssOtherGuest), this.VssOtherGuest);
        this.VddkReadBufferSize = reader.GetOptionalInt32("VddkPreReadBufferSize", this.VddkReadBufferSize);
        this.ForceCreateMissingVBK = reader.GetOptionalBoolean(nameof (ForceCreateMissingVBK), this.ForceCreateMissingVBK);
        this.ForceDeleteBackupFiles = reader.GetOptionalInt32(nameof (ForceDeleteBackupFiles), this.ForceDeleteBackupFiles);
        this.DisableVBKRename = reader.GetOptionalBoolean(nameof (DisableVBKRename), this.DisableVBKRename);
        this.MaxVimSoapOperationTimeout = reader.GetOptionalInt32(nameof (MaxVimSoapOperationTimeout), this.MaxVimSoapOperationTimeout);
        this.MaxVimSoapRescanOperationTimeout = reader.GetOptionalInt32(nameof (MaxVimSoapRescanOperationTimeout), this.MaxVimSoapRescanOperationTimeout);
        this.MaxPerlSoapOperationTimeout = reader.GetOptionalInt32(nameof (MaxPerlSoapOperationTimeout), this.MaxPerlSoapOperationTimeout);
        this.MaxViVMPowerOnOffOperationTimeoutMin = reader.GetOptionalInt32(nameof (MaxViVMPowerOnOffOperationTimeoutMin), this.MaxViVMPowerOnOffOperationTimeoutMin);
        this.TreatDeleteSnapshotErrorAsWarning = reader.GetOptionalBoolean(nameof (TreatDeleteSnapshotErrorAsWarning), this.TreatDeleteSnapshotErrorAsWarning);
        this.DisableReplicaVMXOverwrite = reader.GetOptionalBoolean(nameof (DisableReplicaVMXOverwrite), this.DisableReplicaVMXOverwrite);
        this.BlockSnapshotThreshold = reader.GetOptionalInt32(nameof (BlockSnapshotThreshold), this.BlockSnapshotThreshold);
        this.IgnoreNFSDeleteFailure = reader.GetOptionalBoolean(nameof (IgnoreNFSDeleteFailure), this.IgnoreNFSDeleteFailure);
        this.FLRPreservePermissions = reader.GetOptionalBoolean(nameof (FLRPreservePermissions), this.FLRPreservePermissions);
        this.FlrBatchTransfer = reader.GetOptionalBoolean(nameof (FlrBatchTransfer), this.FlrBatchTransfer);
        this.FlrVmToolsWaitingTimeInSec = reader.GetOptionalInt32("FLRVmToolsWaitingTime", this.FlrVmToolsWaitingTimeInSec);
        this.FlrApplianceIpWaitingTimeInSec = reader.GetOptionalInt32("FlrApplianceIpWaitingTime", this.FlrApplianceIpWaitingTimeInSec);
        this.MultiSessionMode = reader.GetOptionalBoolean(nameof (MultiSessionMode), this.MultiSessionMode);
        this.FilterDatastores = reader.GetOptionalBoolean(nameof (FilterDatastores), this.FilterDatastores);
        this.AlternateSmtpClient = reader.GetOptionalBoolean(nameof (AlternateSmtpClient), this.AlternateSmtpClient);
        this.LegacyReplica = reader.GetOptionalBoolean(nameof (LegacyReplica), this.LegacyReplica);
        this.StorageAlignmentLogarithm = reader.GetOptionalInt32("StorageWriteAlignment", this.StorageAlignmentLogarithm);
        this.ExcludeVirtualDisksExistingOnTarget = reader.GetOptionalBoolean(nameof (ExcludeVirtualDisksExistingOnTarget), false);
        this.ReconnectPassThroughDisks = reader.GetOptionalBoolean(nameof (ReconnectPassThroughDisks), true);
        this.EnableHvVmMultiProcessing = reader.GetOptionalBoolean(nameof (EnableHvVmMultiProcessing), true);
        this.EnableVmWareMultiProcessing = reader.GetOptionalBoolean("EnableVMWareMultiProcessing", true);
        this.FilterVmSwap = reader.GetOptionalBoolean(nameof (FilterVmSwap), true);
        this.HvPreserveMac = reader.GetOptionalBoolean("HvPreserveMAC", true);
        this.ViRestorePreserveMAC = reader.GetOptionalBoolean(nameof (ViRestorePreserveMAC), false);
        this.ViRestoreUseCheckMACAddress = reader.GetOptionalBoolean(nameof (ViRestoreUseCheckMACAddress), true);
        this.ViRestoreEjectCdRomAndFloppyFromVm = reader.GetOptionalBoolean(nameof (ViRestoreEjectCdRomAndFloppyFromVm), true);
        this.ViReplicaPreserveMAC = reader.GetOptionalBoolean(nameof (ViReplicaPreserveMAC), false);
        this.ViReplicaUseCheckMACAddress = reader.GetOptionalBoolean(nameof (ViReplicaUseCheckMACAddress), true);
        this.ViLogAllLookupServices = reader.GetOptionalBoolean(nameof (ViLogAllLookupServices), false);
        this.DisableSmartSwitch = reader.GetOptionalBoolean(nameof (DisableSmartSwitch), false);
        this.VddkWaitTimeoutInSec = reader.GetOptionalInt32("VddkWaitTimeout", this.VddkWaitTimeoutInSec);
        this.VddkVmSearchPatch = reader.GetOptionalBoolean(nameof (VddkVmSearchPatch), this.VddkVmSearchPatch);
        this.EnableManualUnlock = reader.GetOptionalBoolean(nameof (EnableManualUnlock), this.EnableManualUnlock);
        this.Smb2RetryCount = reader.GetOptionalInt32(nameof (Smb2RetryCount), this.Smb2RetryCount);
        this.EnableHvVdk = reader.GetOptionalBoolean(nameof (EnableHvVdk), this.EnableHvVdk);
        this.CsvOwnershipSwitchTimeout = reader.GetOptionalInt32(nameof (CsvOwnershipSwitchTimeout), this.CsvOwnershipSwitchTimeout);
        this.MaxCsvHardwareSnapshotsNum = reader.GetOptionalInt32("CsvMaxSnapshotsNum", this.MaxCsvHardwareSnapshotsNum);
        this.MaxVolHardwareSnapshotsNum = reader.GetOptionalInt32("VolMaxHardSnapshotsNum", this.MaxVolHardwareSnapshotsNum);
        this.MaxVolSoftSnapshotsNum = reader.GetOptionalInt32("VolMaxSoftSnapshotsNum", this.MaxVolSoftSnapshotsNum);
        this.MaxCsv12SoftwareSnapshotsNum = reader.GetOptionalInt32("Csv12MaxSoftSnapshotsNum", this.MaxCsv12SoftwareSnapshotsNum);
        this.MaxSmb3SnapshotsNum = reader.GetOptionalInt32("SmbMaxSnapshotsNum", this.MaxSmb3SnapshotsNum);
        this.HyperVIRCacheBlockSizeKb = reader.GetOptionalInt32(nameof (HyperVIRCacheBlockSizeKb), this.HyperVIRCacheBlockSizeKb);
        this.HyperVIRCacheMemoryPerFileMb = reader.GetOptionalInt32(nameof (HyperVIRCacheMemoryPerFileMb), this.HyperVIRCacheMemoryPerFileMb);
        this.HyperVReferencePointCleanup = reader.GetOptionalBoolean(nameof (HyperVReferencePointCleanup), this.HyperVReferencePointCleanup);
        this.HyperVIRAutoRetryCount = reader.GetOptionalInt32(nameof (HyperVIRAutoRetryCount), this.HyperVIRAutoRetryCount);
        if (this.HyperVIRAutoRetryCount < 1)
          this.HyperVIRAutoRetryCount = 1;
        this.VdkMaxDisksCount = reader.GetOptionalInt32("VdkMaxDisksNum", this.VdkMaxDisksCount);
        this.LowerAgentPriority = reader.GetOptionalBoolean(nameof (LowerAgentPriority), true);
        this.FileCacheLimitPercent = reader.GetOptionalInt32(nameof (FileCacheLimitPercent), 25);
        this.HyperVVssRestoreTimeout = reader.GetOptionalInt32(nameof (HyperVVssRestoreTimeout), this.HyperVVssRestoreTimeout);
        this.HyperV2016VSSCopyOnly = reader.GetOptionalInt32(nameof (HyperV2016VSSCopyOnly), this.HyperV2016VSSCopyOnly);
        this.RpcMaxBinaryDataBlockSizeKb = reader.GetOptionalInt32(nameof (RpcMaxBinaryDataBlockSizeKb), this.RpcMaxBinaryDataBlockSizeKb);
        this.RpcRequestTimeoutSec = reader.GetOptionalInt32(nameof (RpcRequestTimeoutSec), this.RpcRequestTimeoutSec);
        this.RpcSqlLogDownloadViaWebServiceTimeoutSec = reader.GetOptionalInt32(nameof (RpcSqlLogDownloadViaWebServiceTimeoutSec), this.RpcSqlLogDownloadViaWebServiceTimeoutSec);
        this.GuestWebServiceSessionKeepAliveTimeoutSec = reader.GetOptionalInt32(nameof (GuestWebServiceSessionKeepAliveTimeoutSec), this.GuestWebServiceSessionKeepAliveTimeoutSec);
        this.RpcLogInvokerCallStack = reader.GetOptionalBoolean(nameof (RpcLogInvokerCallStack), this.RpcLogInvokerCallStack);
        this.FLRMountPointType = reader.GetOptionalInt32("FLROnDriveLetter", this.FLRMountPointType);
        this.FLRMountPoint = reader.GetOptionalString("FLRMountFolder", this.FLRMountPoint);
        this.AntivirusDefaultAction = reader.GetOptionalInt32(nameof (AntivirusDefaultAction), this.AntivirusDefaultAction);
        this.AntivirusLockAttemptTimeout = reader.GetOptionalInt32(nameof (AntivirusLockAttemptTimeout), this.AntivirusLockAttemptTimeout);
        this.FLRHideSystemBootVolume = reader.GetOptionalBoolean(nameof (FLRHideSystemBootVolume), this.FLRHideSystemBootVolume);
        this.ReparsePointsUpdateLimit = reader.GetOptionalInt32("ReparsePointUpdateLimit", this.ReparsePointsUpdateLimit);
        this.ReplicaKeepWorkingDir = reader.GetOptionalBoolean(nameof (ReplicaKeepWorkingDir), false);
        this.ReplicaRemoveConnectedUsbDevices = reader.GetOptionalBoolean(nameof (ReplicaRemoveConnectedUsbDevices), false);
        this.HvVmSizesPopulateTimeout = TimeSpan.FromSeconds((double) reader.GetOptionalInt32(nameof (HvVmSizesPopulateTimeout), 600));
        this.LocalAgentStopTimeoutSec = reader.GetOptionalInt32("LocalAgentStopTimeout", 60);
        this.MaxEvacuateTaskCount = reader.GetOptionalInt32(nameof (MaxEvacuateTaskCount), this.MaxEvacuateTaskCount);
        this.MaxEvacuateRepositorySlots = reader.GetOptionalInt32(nameof (MaxEvacuateRepositorySlots), this.MaxEvacuateRepositorySlots);
        this.VixOperationTimeoutInSeconds = reader.GetOptionalInt32("vixOperationTimeoutSec", this.VixOperationTimeoutInSeconds);
        this.IgnoreReparsePoints = reader.GetOptionalBoolean(nameof (IgnoreReparsePoints), false);
        this.VDDKLogLevel = reader.GetOptionalInt32(nameof (VDDKLogLevel), this.VDDKLogLevel);
        this.AntivirusLogLevel = reader.GetOptionalInt32(nameof (AntivirusLogLevel), this.AntivirusLogLevel);
        this.ReplicaHvContentOnCsvOwner = reader.GetOptionalBoolean("ReplicateHvContentOnCsvOwner", true);
        this.LogRescanFailInJob = reader.GetOptionalBoolean(nameof (LogRescanFailInJob), true);
        this.SanProxyDetectEnableInquiryMatching = reader.GetOptionalBoolean(nameof (SanProxyDetectEnableInquiryMatching), true);
        this.SanProxyRestoreLUNIdentifyByInquiry = reader.GetOptionalBoolean(nameof (SanProxyRestoreLUNIdentifyByInquiry), true);
        this.HvPrepareAutoMountOffHost = (COptions.EPrepareAutoMount) reader.GetOptionalInt32(nameof (HvPrepareAutoMountOffHost), (int) this.HvPrepareAutoMountOffHost);
        this.HvPrepareAutoMountOnHost = (COptions.EPrepareAutoMount) reader.GetOptionalInt32(nameof (HvPrepareAutoMountOnHost), (int) this.HvPrepareAutoMountOnHost);
        CTapeDevicesRegistryValue devicesRegistryValue = new CTapeDevicesRegistryValue();
        string[] optionalMultiString = reader.GetOptionalMultiString(nameof (TapeDevices), (string[]) null);
        if (optionalMultiString != null)
          devicesRegistryValue.Deserialize(optionalMultiString);
        this.TapeDevices = (ITapeDevicesRegistryValue) devicesRegistryValue;
        object obj = registryKey.GetValue(nameof (BarcodeScannerInstalled), (object) null);
        this.BarcodeScannerInstalled = obj == null ? new bool?() : new bool?((int) obj > 0);
        this.FilterSerialNumbers = (TapeSerialNumberFilterPolicyType) reader.GetOptionalInt32(nameof (FilterSerialNumbers), 3);
        this.WaitingTapeNotifcationEnabled = reader.GetOptionalBoolean(nameof (WaitingTapeNotifcationEnabled), true);
        this.TapeBackupAlgorythm = reader.GetOptionalString(nameof (TapeBackupAlgorythm), "ffii");
        this.TapeBackupParallelMode = reader.GetOptionalInt32(nameof (TapeBackupParallelMode), 0);
        this.UseDriveByLunDetectionOnly = reader.GetOptionalBoolean(nameof (UseDriveByLunDetectionOnly), false);
        this.UseDataDomainTapeSpecifics = reader.GetOptionalBoolean(nameof (UseDataDomainTapeSpecifics), true);
        this.SkipTapeAlerts = reader.GetOptionalBoolean(nameof (SkipTapeAlerts), false);
        this.ChangerElementFillCompletionTimeout = TimeSpan.FromSeconds((double) reader.GetOptionalInt32("ChangerElementFillCompletionTimeoutSec", (int) this.ChangerElementFillCompletionTimeout.TotalSeconds));
        this.ChangerElementDefaultFillTimeout = TimeSpan.FromSeconds((double) reader.GetOptionalInt32("ChangerElementDefaultFillTimeoutSec", (int) this.ChangerElementDefaultFillTimeout.TotalSeconds));
        this.SkipIEPortState = reader.GetOptionalBoolean(nameof (SkipIEPortState), this.SkipIEPortState);
        this.TapeSkipEmptyLibraryWorkaround = reader.GetOptionalBoolean(nameof (TapeSkipEmptyLibraryWorkaround), this.TapeSkipEmptyLibraryWorkaround);
        this.TapeAllSyntheticCreation = reader.GetOptionalBoolean(nameof (TapeAllSyntheticCreation), false);
        int optionalInt32_1 = reader.GetOptionalInt32("TapeLibraryStateReportPeriodicityMin", (int) this.TapeLibraryStateReportPeriodicity.TotalMinutes);
        this.TapeLibraryStateReportPeriodicity = optionalInt32_1 > 0 ? TimeSpan.FromMinutes((double) optionalInt32_1) : this.TapeLibraryStateReportPeriodicity;
        this.TapeLibrariesDiscoverPeriod = TimeSpan.FromSeconds((double) reader.GetOptionalInt32("TapeLibrariesDiscoverPeriodSec", (int) this.TapeLibrariesDiscoverPeriod.TotalSeconds));
        this.TapeForceSoftwareEncryption = reader.GetOptionalBoolean(nameof (TapeForceSoftwareEncryption), this.TapeForceSoftwareEncryption);
        this.TapeForceSynthesizedFull = reader.GetOptionalBoolean(nameof (TapeForceSynthesizedFull), this.TapeForceSynthesizedFull);
        this.TapeLibraryDiscoverRetryTimeout = TimeSpan.FromSeconds((double) reader.GetOptionalInt32(nameof (TapeLibraryDiscoverRetryTimeout), (int) this.TapeLibraryDiscoverRetryTimeout.TotalSeconds));
        this.TapeLibraryDiscoverRetryCount = reader.GetOptionalInt32(nameof (TapeLibraryDiscoverRetryCount), this.TapeLibraryDiscoverRetryCount);
        this.WaitForValidTapeTimeout = TimeSpan.FromMinutes((double) reader.GetOptionalInt32("WaitForValidTapeTimeoutMin", (int) this.WaitForValidTapeTimeout.TotalMinutes));
        this.WaitForMediasetTimeout = TimeSpan.FromMinutes((double) reader.GetOptionalInt32("WaitForMediasetTimeoutMin", (int) this.WaitForMediasetTimeout.TotalMinutes));
        this.WaitForDriveTapeTimeout = TimeSpan.FromMinutes((double) reader.GetOptionalInt32("WaitForDriveTapeTimeoutMin", (int) this.WaitForDriveTapeTimeout.TotalMinutes));
        if (reader.IsOptionSpecified(nameof (IsStopByBackupJobAllowed)))
          this.IsStopByBackupJobAllowed = new bool?(reader.GetOptionalBoolean(nameof (IsStopByBackupJobAllowed), true));
        this.SourceJobWaitForTapeJobTimeout = TimeSpan.FromHours((double) reader.GetOptionalInt32(nameof (SourceJobWaitForTapeJobTimeout), (int) this.SourceJobWaitForTapeJobTimeout.TotalMinutes));
        this.TapeBackupImportPolicy = (TapeBackupImportPolicy) reader.GetOptionalInt32(nameof (TapeBackupImportPolicy), (int) this.TapeBackupImportPolicy);
        this.TapeGfsTaskRetryPeriodInMins = reader.GetOptionalInt32(nameof (TapeGfsTaskRetryPeriodInMins), this.TapeGfsTaskRetryPeriodInMins);
        this.TapeLockedStorageWaitingTimeoutInMins = reader.GetOptionalInt32(nameof (TapeLockedStorageWaitingTimeoutInMins), this.TapeLockedStorageWaitingTimeoutInMins);
        this.TapeIgnoreReturnMediaToOriginalSlot = reader.GetOptionalBoolean(nameof (TapeIgnoreReturnMediaToOriginalSlot), true);
        this.TapeIgnoreInconsistenceMetaCheck = reader.GetOptionalBoolean(nameof (TapeIgnoreInconsistenceMetaCheck), true);
        this.TapeLockResourceTimeout = TimeSpan.FromSeconds((double) reader.GetOptionalInt32("TapeLockResourceTimeoutSec", (int) this.TapeLockResourceTimeout.TotalSeconds));
        this.TapeLibrarySwitchWaitForDriveLockTimeoutMin = TimeSpan.FromMinutes((double) reader.GetOptionalInt32(nameof (TapeLibrarySwitchWaitForDriveLockTimeoutMin), (int) this.TapeLibrarySwitchWaitForDriveLockTimeoutMin.TotalMinutes));
        this.TapeLibrarySwitchWaitForMediumTimeoutMin = TimeSpan.FromMinutes((double) reader.GetOptionalInt32(nameof (TapeLibrarySwitchWaitForMediumTimeoutMin), (int) this.TapeLibrarySwitchWaitForMediumTimeoutMin.TotalMinutes));
        this.TapeClientTrafficCompressionLevel = reader.GetOptionalInt32(nameof (TapeClientTrafficCompressionLevel), this.TapeClientTrafficCompressionLevel);
        this.TapeLibraryDiscoverSkipUnknownMediumChangers = reader.GetOptionalBoolean(nameof (TapeLibraryDiscoverSkipUnknownMediumChangers), this.TapeLibraryDiscoverSkipUnknownMediumChangers);
        this.TapeMinSlotsToRequest = this.TapeDefaultMinSlotsToRequest;
        this.TapeMaxSlotsToRequest = reader.GetOptionalInt32(nameof (TapeMaxSlotsToRequest), this.TapeDefaultMaxSlotsToRequest);
        if (this.TapeMinSlotsToRequest > this.TapeMaxSlotsToRequest)
          this.TapeMaxSlotsToRequest = this.TapeDefaultMaxSlotsToRequest;
        this.TapeDatabaseCommandTimeoutSeconds = reader.GetOptionalInt32(nameof (TapeDatabaseCommandTimeoutSeconds), this.TapeDatabaseCommandTimeoutSeconds);
        this.MessageCountBeforeCumulative = reader.GetOptionalUInt64(nameof (MessageCountBeforeCumulative), this.MessageCountBeforeCumulative);
        this.TapeSequentialRestoreCacheSize = reader.GetOptionalInt32(nameof (TapeSequentialRestoreCacheSize), this.TapeSequentialRestoreCacheSize);
        this.TapeRestoreWaitForDriveLockTimeoutMin = TimeSpan.FromMinutes((double) reader.GetOptionalInt32(nameof (TapeRestoreWaitForDriveLockTimeoutMin), (int) this.TapeRestoreWaitForDriveLockTimeoutMin.TotalMinutes));
        this.MainRefreshInterval = TimeSpan.FromSeconds((double) reader.GetOptionalInt32("MainRefreshIntervalSec", 3));
        this.ReporterRefreshInterval = TimeSpan.FromSeconds((double) reader.GetOptionalInt32("ReporterRefreshIntervalSec", 3));
        this.TapesRefreshInterval = TimeSpan.FromSeconds((double) reader.GetOptionalInt32(nameof (TapesRefreshInterval), 60));
        this.TapeWaitForDriveCleaningInterval = TimeSpan.FromSeconds((double) reader.GetOptionalInt32(nameof (TapeWaitForDriveCleaningInterval), 120));
        this.TapeWaitForCleaningTapeTimeout = TimeSpan.FromMinutes((double) reader.GetOptionalInt32("TapeWaitForCleaningTapeInMinutes", 5));
        this.TapesAutoRefresh = reader.GetOptionalBoolean(nameof (TapesAutoRefresh), true);
        this.TapeChainParalleling = reader.GetOptionalBoolean(nameof (TapeChainParalleling), true);
        this.TapesAutoRefreshLimit = reader.GetOptionalInt32(nameof (TapesAutoRefreshLimit), 1000);
        this.TapeGfsJobRetryPeriodInHours = Math.Max(reader.GetOptionalInt32(nameof (TapeGfsJobRetryPeriodInHours), this.TapeGfsJobRetryPeriodInHours), 24);
        this.TapeGFSMissedBackupNotification = reader.GetOptionalInt32(nameof (TapeGFSMissedBackupNotification), -1);
        this.NDMPIncrementsLimit = reader.GetOptionalInt32(nameof (NDMPIncrementsLimit), 9);
        this.IsNDMPIncrementsLimitSpecified = reader.KeyExists(nameof (NDMPIncrementsLimit));
        this.TapeGfsExpiredMediumSharing = reader.GetOptionalBoolean(nameof (TapeGfsExpiredMediumSharing), false);
        this.ForceTransform = reader.GetOptionalBoolean(nameof (ForceTransform), false);
        this.ForceTransformReconnects = reader.GetOptionalBoolean(nameof (ForceTransformReconnects), this.ForceTransformReconnects);
        this.ResourceCheckIntervalMs = reader.GetOptionalInt32(nameof (ResourceCheckIntervalMs), this.ResourceCheckIntervalMs);
        this.RestrictedNetworkSymbols = reader.GetOptionalString(nameof (RestrictedNetworkSymbols), this.RestrictedNetworkSymbols);
        this.InverseVssProtocolOrder = reader.GetOptionalBoolean(nameof (InverseVssProtocolOrder), false);
        this.HvVssPowerShellDirectPriorityOverNetwork = reader.GetOptionalBoolean(nameof (HvVssPowerShellDirectPriorityOverNetwork), false);
        this.MaxSmbOffhostProxyCount = reader.GetOptionalInt32(nameof (MaxSmbOffhostProxyCount), this.MaxSmbOffhostProxyCount);
        this.StgBlockSizeDivisor = reader.GetOptionalInt32(nameof (StgBlockSizeDivisor), this.StgBlockSizeDivisor);
        this.V2VTestReplicaEnabled = reader.GetOptionalBoolean("v2vTestReplicaEnabled", this.V2VTestReplicaEnabled);
        this.UIShowAllVssProvidersForVolume = reader.GetOptionalBoolean(nameof (UIShowAllVssProvidersForVolume), this.UIShowAllVssProvidersForVolume);
        this.SanSnapshotTimeoutMinutes = reader.GetOptionalInt32("SanSnapshotTimeout", this.SanSnapshotTimeoutMinutes);
        this.SanMonitorTimeoutInSeconds = reader.GetOptionalInt32("HpSanMonitorTimeout", this.SanMonitorTimeoutInSeconds);
        this.SanMonitorTimeoutInSeconds = reader.GetOptionalInt32("SanMonitorTimeout", this.SanMonitorTimeoutInSeconds);
        this.SanMonitorConcurrentRescans = reader.GetOptionalInt32(nameof (SanMonitorConcurrentRescans), this.SanMonitorConcurrentRescans);
        this.SanCheckSnapshotAvailabilityOnRestore = reader.GetOptionalBoolean("HpCheckSnapshotAvailabilityOnRestore", this.SanCheckSnapshotAvailabilityOnRestore);
        this.SanCheckSnapshotAvailabilityOnRestore = reader.GetOptionalBoolean(nameof (SanCheckSnapshotAvailabilityOnRestore), this.SanCheckSnapshotAvailabilityOnRestore);
        this.SanOperationTimeoutSec = reader.GetOptionalInt32("HpSanOperationTimeout", this.SanOperationTimeoutSec);
        this.SanOperationTimeoutSec = reader.GetOptionalInt32("SanOperationTimeout", this.SanOperationTimeoutSec);
        this.SanMaxCommandOutputInMb = reader.GetOptionalInt32(nameof (SanMaxCommandOutputInMb), this.SanMaxCommandOutputInMb);
        this.SanRetryCommandCount = reader.GetOptionalInt32(nameof (SanRetryCommandCount), this.SanRetryCommandCount);
        this.SanRetryCommandDelaySec = reader.GetOptionalInt32(nameof (SanRetryCommandDelaySec), this.SanRetryCommandDelaySec);
        this.SanIbmShowApiDetails = reader.GetOptionalBoolean(nameof (SanIbmShowApiDetails), this.SanIbmShowApiDetails);
        this.VcdConnectionTimeoutSec = reader.GetOptionalInt32("VcdConnectionTimeout", this.VcdConnectionTimeoutSec);
        this.VcdDefaultTaskTimeoutSec = reader.GetOptionalInt32("VcdDefaultTaskTimeout", this.VcdDefaultTaskTimeoutSec);
        this.VcdLongTaskTimeoutSec = reader.GetOptionalInt32(nameof (VcdLongTaskTimeout), this.VcdLongTaskTimeoutSec);
        this.VcdRestoreWaitResourcesMin = reader.GetOptionalInt32(nameof (VcdRestoreWaitResourcesMin), this.VcdRestoreWaitResourcesMin);
        this.VcdCommandRetryCount = reader.GetOptionalInt32(nameof (VcdCommandRetryCount), this.VcdCommandRetryCount);
        this.VcdSkipRestoreAdvancedCheckings = reader.GetOptionalBoolean(nameof (VcdSkipRestoreAdvancedCheckings), this.VcdSkipRestoreAdvancedCheckings);
        this.VcdRefreshVCStorageProfileCount = reader.GetOptionalInt32(nameof (VcdRefreshVCStorageProfileCount), this.VcdRefreshVCStorageProfileCount);
        this.VcdStorageProfileWaitAppearRetryCount = reader.GetOptionalInt32(nameof (VcdStorageProfileWaitAppearRetryCount), this.VcdStorageProfileWaitAppearRetryCount);
        this.VcdVAppStorageSizeInMb = reader.GetOptionalInt32(nameof (VcdVAppStorageSizeInMb), this.VcdVAppStorageSizeInMb);
        this.VcdImportAfterRestoreDisksFromStoragePod = reader.GetOptionalBoolean(nameof (VcdImportAfterRestoreDisksFromStoragePod), this.VcdImportAfterRestoreDisksFromStoragePod);
        this.VcdImportAfterRestoreDisksFromStoragePodAnyVcdVersion = reader.GetOptionalBoolean(nameof (VcdImportAfterRestoreDisksFromStoragePodAnyVcdVersion), this.VcdImportAfterRestoreDisksFromStoragePodAnyVcdVersion);
        this.VcdMultiTenancyOrgCacheAfterCloseTimeoutSec = reader.GetOptionalInt32(nameof (VcdMultiTenancyOrgCacheAfterCloseTimeoutSec), this.VcdMultiTenancyOrgCacheAfterCloseTimeoutSec);
        this.VcdMultiTenancyOrgCacheAfterInitTimeoutSec = reader.GetOptionalInt32(nameof (VcdMultiTenancyOrgCacheAfterInitTimeoutSec), this.VcdMultiTenancyOrgCacheAfterInitTimeoutSec);
        this.VcdTemplatesProcessingMode = (COptions.ETemplateProcessingMode) reader.GetOptionalInt32(nameof (VcdTemplatesProcessingMode), (int) this.VcdTemplatesProcessingMode);
        this.SanRescanHostLeaseInitialTimeInMin = reader.GetOptionalInt32("SanRescanHostLeaseInitialTime", this.SanRescanHostLeaseInitialTimeInMin);
        this.SanRescanFCBusAttemptCount = reader.GetOptionalInt32(nameof (SanRescanFCBusAttemptCount), this.SanRescanFCBusAttemptCount);
        this.SanRescanFCBusTimeoutSec = reader.GetOptionalInt32(nameof (SanRescanFCBusTimeoutSec), this.SanRescanFCBusTimeoutSec);
        this.SanRescanISCSIBusAttemptCount = reader.GetOptionalInt32(nameof (SanRescanISCSIBusAttemptCount), this.SanRescanISCSIBusAttemptCount);
        this.SanRescanISCSIBusTimeoutSec = reader.GetOptionalInt32(nameof (SanRescanISCSIBusTimeoutSec), this.SanRescanISCSIBusTimeoutSec);
        this.SanRescanISCSIBusByConnectAttemptCount = reader.GetOptionalInt32(nameof (SanRescanISCSIBusByConnectAttemptCount), this.SanRescanISCSIBusByConnectAttemptCount);
        this.SanRescanISCSIBusByConnectTimeoutSec = reader.GetOptionalInt32(nameof (SanRescanISCSIBusByConnectTimeoutSec), this.SanRescanISCSIBusByConnectTimeoutSec);
        this.SanRescanFcBusCheckCount = reader.GetOptionalInt32("SanRescanFCBusCheckCount", this.SanRescanFcBusCheckCount);
        this.SanProxyLunIdTtlMin = reader.GetOptionalInt32(nameof (SanProxyLunIdTtlMin), this.SanProxyLunIdTtlMin);
        this.SanRescanSnapshotsCountInGroup = reader.GetOptionalInt32(nameof (SanRescanSnapshotsCountInGroup), this.SanRescanSnapshotsCountInGroup);
        this.SanRescanSnapshotsThroughFcp = reader.GetOptionalBoolean(nameof (SanRescanSnapshotsThroughFcp), this.SanRescanSnapshotsThroughFcp);
        this.SanRescanSnapshotsThroughiSCSI = reader.GetOptionalBoolean(nameof (SanRescanSnapshotsThroughiSCSI), this.SanRescanSnapshotsThroughiSCSI);
        this.SanRescanSnapshotsParseFS = reader.GetOptionalBoolean(nameof (SanRescanSnapshotsParseFS), this.SanRescanSnapshotsParseFS);
        this.SanRescanStorageSnapshotsParallel = reader.GetOptionalBoolean(nameof (SanRescanStorageSnapshotsParallel), this.SanRescanStorageSnapshotsParallel);
        this.SanRescanStorageSnapshotsAgentsCount = reader.GetOptionalInt32(nameof (SanRescanStorageSnapshotsAgentsCount), this.SanRescanStorageSnapshotsAgentsCount);
        this.SanMonitorDisabled = reader.GetOptionalBoolean(nameof (SanMonitorDisabled), this.SanMonitorDisabled);
        this.SanMonitorFilterObjectsByRunningSessions = reader.GetOptionalBoolean(nameof (SanMonitorFilterObjectsByRunningSessions), this.SanMonitorFilterObjectsByRunningSessions);
        this.SanMonitorAddSessionMessages = reader.GetOptionalBoolean(nameof (SanMonitorAddSessionMessages), this.SanMonitorAddSessionMessages);
        this.SanMonitorFailedHostSkipRescanCount = reader.GetOptionalInt32(nameof (SanMonitorFailedHostSkipRescanCount), this.SanMonitorFailedHostSkipRescanCount);
        this.SanVolumesProxyExportLockTimeout = reader.GetOptionalInt32(nameof (SanVolumesProxyExportLockTimeout), this.SanVolumesProxyExportLockTimeout);
        this.SanBackupSnapshotCreateLockTimeout = reader.GetOptionalInt32(nameof (SanBackupSnapshotCreateLockTimeout), this.SanBackupSnapshotCreateLockTimeout);
        this.SanBackupFcpThroughIscsiPriority = reader.GetOptionalBoolean(nameof (SanBackupFcpThroughIscsiPriority), this.SanBackupFcpThroughIscsiPriority);
        this.SanBackupIscsiThroughFcpEnabled = reader.GetOptionalBoolean(nameof (SanBackupIscsiThroughFcpEnabled), this.SanBackupIscsiThroughFcpEnabled);
        this.SanBackupIscsiThroughFcpPriority = reader.GetOptionalBoolean(nameof (SanBackupIscsiThroughFcpPriority), this.SanBackupIscsiThroughFcpPriority);
        this.SanSnapshotTransferWaitTimeoutMin = reader.GetOptionalInt32(nameof (SanSnapshotTransferWaitTimeoutMin), this.SanSnapshotTransferWaitTimeoutMin);
        this.SanVolumeCopyLockTimeoutMin = reader.GetOptionalInt32(nameof (SanVolumeCopyLockTimeoutMin), this.SanVolumeCopyLockTimeoutMin);
        this.SanViProxyMountFCLUNTimeoutMin = reader.GetOptionalInt32(nameof (SanViProxyMountFCLUNTimeoutMin), this.SanViProxyMountFCLUNTimeoutMin);
        this.SanRestoreAlwaysToActualState = reader.GetOptionalBoolean(nameof (SanRestoreAlwaysToActualState), this.SanRestoreAlwaysToActualState);
        this.SanRescanLaunchTimeDayOfWeek = reader.GetOptionalInt32(nameof (SanRescanLaunchTimeDayOfWeek), this.SanRescanLaunchTimeDayOfWeek);
        this.SanRescanLaunchTimeHour = reader.GetOptionalInt32(nameof (SanRescanLaunchTimeHour), this.SanRescanLaunchTimeHour);
        this.SanRescanUpdateVmSizeWithoutVC = reader.GetOptionalBoolean(nameof (SanRescanUpdateVmSizeWithoutVC), this.SanRescanUpdateVmSizeWithoutVC);
        this.SanRescanHostDeletionWaitTimeoutMin = reader.GetOptionalInt32(nameof (SanRescanHostDeletionWaitTimeoutMin), this.SanRescanHostDeletionWaitTimeoutMin);
        this.SanRescaniSCSIPriorityOverFC = reader.GetOptionalBoolean(nameof (SanRescaniSCSIPriorityOverFC), this.SanRescaniSCSIPriorityOverFC);
        this.SanRestoreUseSharedSnapshotClone = reader.GetOptionalBoolean(nameof (SanRestoreUseSharedSnapshotClone), this.SanRestoreUseSharedSnapshotClone);
        this.SanRestoreUpdateAbsolutePathWithoutVmRef = reader.GetOptionalBoolean(nameof (SanRestoreUpdateAbsolutePathWithoutVmRef), this.SanRestoreUpdateAbsolutePathWithoutVmRef);
        this.SanRestoreNFSCopyVmdkFolders = reader.GetOptionalBoolean(nameof (SanRestoreNFSCopyVmdkFolders), this.SanRestoreNFSCopyVmdkFolders);
        this.SanRestoreFailOnRevertSnapshot = reader.GetOptionalBoolean(nameof (SanRestoreFailOnRevertSnapshot), this.SanRestoreFailOnRevertSnapshot);
        this.SanRestoreCreateSnapshotCloneCheckTimeoutSec = reader.GetOptionalInt32(nameof (SanRestoreCreateSnapshotCloneCheckTimeoutSec), this.SanRestoreCreateSnapshotCloneCheckTimeoutSec);
        this.SanRestoreCreateSnapshotCloneTimeoutMin = reader.GetOptionalInt32(nameof (SanRestoreCreateSnapshotCloneTimeoutMin), this.SanRestoreCreateSnapshotCloneTimeoutMin);
        this.SanRestoreUnmountSnapshotCloneTimeoutMin = reader.GetOptionalInt32(nameof (SanRestoreUnmountSnapshotCloneTimeoutMin), this.SanRestoreUnmountSnapshotCloneTimeoutMin);
        this.SanRestoreMountSnapshotCloneTimeoutMin = reader.GetOptionalInt32(nameof (SanRestoreMountSnapshotCloneTimeoutMin), this.SanRestoreMountSnapshotCloneTimeoutMin);
        this.SanBackupWaitVMSnapshotDeletion = reader.GetOptionalBoolean(nameof (SanBackupWaitVMSnapshotDeletion), this.SanBackupWaitVMSnapshotDeletion);
        this.SanBackupPrepareAgentCount = reader.GetOptionalInt32(nameof (SanBackupPrepareAgentCount), this.SanBackupPrepareAgentCount);
        this.SanSnapshotTransferWaitVMSnapshotDeletion = reader.GetOptionalBoolean(nameof (SanSnapshotTransferWaitVMSnapshotDeletion), this.SanSnapshotTransferWaitVMSnapshotDeletion);
        this.SanRestoreUnresolvedVmfsRetryCount = reader.GetOptionalInt32(nameof (SanRestoreUnresolvedVmfsRetryCount), this.SanRestoreUnresolvedVmfsRetryCount);
        this.SanRestoreAddNFSRetryCount = reader.GetOptionalInt32(nameof (SanRestoreAddNFSRetryCount), this.SanRestoreAddNFSRetryCount);
        this.SanBackupIscsiCheckInitiatorIqn = reader.GetOptionalBoolean(nameof (SanBackupIscsiCheckInitiatorIqn), this.SanBackupIscsiCheckInitiatorIqn);
        this.SanBackupFromSecondaryFailIfNotLuns = reader.GetOptionalBoolean(nameof (SanBackupFromSecondaryFailIfNotLuns), this.SanBackupFromSecondaryFailIfNotLuns);
        this.DirectNFSCheckTtlToDatastore = reader.GetOptionalBoolean(nameof (DirectNFSCheckTtlToDatastore), this.DirectNFSCheckTtlToDatastore);
        this.DirectNFSCheckDatastoresFromProxy = reader.GetOptionalBoolean(nameof (DirectNFSCheckDatastoresFromProxy), this.DirectNFSCheckDatastoresFromProxy);
        this.DirectSanForNotSCSI = reader.GetOptionalBoolean(nameof (DirectSanForNotSCSI), this.DirectSanForNotSCSI);
        this.SanRescanStorageSnapshotReadyTimeout = TimeSpan.FromMinutes((double) reader.GetOptionalInt32("SanRescanStorageSnapshotReadyTimeoutMin", (int) this.SanRescanStorageSnapshotReadyTimeout.TotalMinutes));
        this.SanLogEsxDatastoresInfo = reader.GetOptionalBoolean(nameof (SanLogEsxDatastoresInfo), this.SanLogEsxDatastoresInfo);
        this.CiscoHXDeleteSentinelSnapshot = reader.GetOptionalBoolean(nameof (CiscoHXDeleteSentinelSnapshot), this.CiscoHXDeleteSentinelSnapshot);
        this.CiscoHXUpdateSentinelSnapshot = reader.GetOptionalBoolean(nameof (CiscoHXUpdateSentinelSnapshot), this.CiscoHXUpdateSentinelSnapshot);
        this.SanVmMountTimeoutMin = TimeSpan.FromMinutes((double) reader.GetOptionalInt32(nameof (SanVmMountTimeoutMin), 60));
        this.SanStoragePluginsPath = reader.GetOptionalString("StoragePluginsPath", this.SanStoragePluginsPath);
        this.SanCanLoadUnsignedPlugins = reader.GetOptionalBoolean(nameof (SanCanLoadUnsignedPlugins), this.SanCanLoadUnsignedPlugins);
        this.SanVeeamIqnTrimSymbolList = reader.GetOptionalString(nameof (SanVeeamIqnTrimSymbolList), this.SanVeeamIqnTrimSymbolList);
        this.SanCleanRegistryFromUnmountDevices = reader.GetOptionalBoolean(nameof (SanCleanRegistryFromUnmountDevices), this.SanCleanRegistryFromUnmountDevices);
        this.SanModernProxyScheduling = reader.GetOptionalBoolean(nameof (SanModernProxyScheduling), this.SanModernProxyScheduling);
        this.SanMaxMountedLunsToProxy = reader.GetOptionalInt32(nameof (SanMaxMountedLunsToProxy), this.SanMaxMountedLunsToProxy);
        this.SanMaxSnapshotCount = reader.GetOptionalInt32(nameof (SanMaxSnapshotCount), this.SanMaxSnapshotCount);
        this.SanRescanMaxXmlMessageCount = reader.GetOptionalInt32(nameof (SanRescanMaxXmlMessageCount), this.SanRescanMaxXmlMessageCount);
        this.SanUseCreationTimeInSnapshotComparison = reader.GetOptionalBoolean(nameof (SanUseCreationTimeInSnapshotComparison), this.SanUseCreationTimeInSnapshotComparison);
        this.VNXBlockUseCreationTimeInSnapshotComparison = reader.GetOptionalBoolean(nameof (VNXBlockUseCreationTimeInSnapshotComparison), this.VNXBlockUseCreationTimeInSnapshotComparison);
        this.ViDisableDRSAtTaskBuilding = reader.GetOptionalBoolean(nameof (ViDisableDRSAtTaskBuilding), this.ViDisableDRSAtTaskBuilding);
        this.SanForceViSnapshotCreatingForSnapOnly = reader.GetOptionalBoolean(nameof (SanForceViSnapshotCreatingForSnapOnly), this.SanForceViSnapshotCreatingForSnapOnly);
        this.UseStorageSnapshotsInVeeamZip = reader.GetOptionalBoolean(nameof (UseStorageSnapshotsInVeeamZip), this.UseStorageSnapshotsInVeeamZip);
        this.SanStopRescanSessionsOnConflictingBackup = reader.GetOptionalBoolean(nameof (SanStopRescanSessionsOnConflictingBackup), this.SanStopRescanSessionsOnConflictingBackup);
        this.JobLeaseTtlInMin = reader.GetOptionalInt32(nameof (JobLeaseTtlInMin), this.JobLeaseTtlInMin);
        this.JobLeaseUpdateRate = reader.GetOptionalInt32(nameof (JobLeaseUpdateRate), this.JobLeaseUpdateRate);
        this.MaxPendingBlocks = reader.GetOptionalInt32(nameof (MaxPendingBlocks), this.MaxPendingBlocks);
        this.FailViBackupJobSaving = reader.GetOptionalBoolean(nameof (FailViBackupJobSaving), this.FailViBackupJobSaving);
        this.LogVssKeepSnapshotEvents = reader.GetOptionalBoolean(nameof (LogVssKeepSnapshotEvents), this.LogVssKeepSnapshotEvents);
        this.RetryAsyncSoapTasks = reader.GetOptionalBoolean(nameof (RetryAsyncSoapTasks), this.RetryAsyncSoapTasks);
        this.AsyncSoapTasksRetryCount = reader.GetOptionalInt32(nameof (AsyncSoapTasksRetryCount), this.AsyncSoapTasksRetryCount);
        this.SoapMaxLoginRetries = reader.GetOptionalInt32(nameof (SoapMaxLoginRetries), this.SoapMaxLoginRetries);
        this.SoapSessionCaching = reader.GetOptionalBoolean(nameof (SoapSessionCaching), this.SoapSessionCaching);
        this.EnableTrafficControl = reader.GetOptionalInt32(nameof (EnableTrafficControl), this.EnableTrafficControl);
        this.DumpCorruptedTraffic = reader.GetOptionalInt32(nameof (DumpCorruptedTraffic), this.DumpCorruptedTraffic);
        this.NetworkIdleTimeoutInHours = reader.GetOptionalInt32("NetworkIdleTimeout", this.NetworkIdleTimeoutInHours);
        this.BackupVcEnabled = reader.GetOptionalBoolean(nameof (BackupVcEnabled), this.BackupVcEnabled);
        this.ExcludeConfigurationDbFromSnapshot = reader.GetOptionalBoolean(nameof (ExcludeConfigurationDbFromSnapshot), this.ExcludeConfigurationDbFromSnapshot);
        this.StorageLockConflictResolveTimeoutInSec = reader.GetOptionalInt32(nameof (StorageLockConflictResolveTimeoutInSec), this.StorageLockConflictResolveTimeoutInSec);
        this.HvLinuxVLRTempFolder = reader.GetOptionalString("HvLinuxFLRTempFolder", this.HvLinuxVLRTempFolder);
        this.PowerShellManagerPort = reader.GetOptionalInt32(nameof (PowerShellManagerPort), this.PowerShellManagerPort);
        this.HVPSPingRetryWaitSec = reader.GetOptionalInt32(nameof (HVPSPingRetryWaitSec), this.HVPSPingRetryWaitSec);
        this.HVPSPingRetryCount = reader.GetOptionalInt32(nameof (HVPSPingRetryCount), this.HVPSPingRetryCount);
        this.MaxHvConcurrentDeletingCheckpointsForHost = reader.GetOptionalInt32(nameof (MaxHvConcurrentDeletingCheckpointsForHost), this.MaxHvConcurrentDeletingCheckpointsForHost);
        this.MaxConcurrentDeletingSnapshotsForHost = reader.GetOptionalInt32(nameof (MaxConcurrentDeletingSnapshotsForHost), this.MaxConcurrentDeletingSnapshotsForHost);
        this.MaxConcurrentDeletingSnapshotsForCluster = reader.GetOptionalInt32(nameof (MaxConcurrentDeletingSnapshotsForCluster), this.MaxConcurrentDeletingSnapshotsForCluster);
        this.SnapshotDeleteSemaphoreTimeoutSec = reader.GetOptionalInt32(nameof (SnapshotDeleteSemaphoreTimeoutSec), this.SnapshotDeleteSemaphoreTimeoutSec);
        this.VssFreezeDelay = reader.GetOptionalInt32(nameof (VssFreezeDelay), this.VssFreezeDelay);
        this.GuestVssSnapshotTtlSec = reader.GetOptionalInt32("VSSGuestSnapshotTimeout", this.GuestVssSnapshotTtlSec);
        this.MicroChangesFeederMaxLoopAttempts = reader.GetOptionalInt32(nameof (MicroChangesFeederMaxLoopAttempts), this.MicroChangesFeederMaxLoopAttempts);
        this.MicroChangesFeederLoopTimeoutSec = reader.GetOptionalInt32(nameof (MicroChangesFeederLoopTimeoutSec), this.MicroChangesFeederLoopTimeoutSec);
        this.DebugSelfThrottlingIsAllowed = reader.GetOptionalBoolean(nameof (DebugSelfThrottlingIsAllowed), this.DebugSelfThrottlingIsAllowed);
        this.HvVmIpResolveFromFQDN = reader.GetOptionalBoolean("HvVmIpResolveFromDNS", this.HvVmIpResolveFromFQDN);
        this.BackupSyncMaxRetriesPerOib = reader.GetOptionalInt32(nameof (BackupSyncMaxRetriesPerOib), this.BackupSyncMaxRetriesPerOib);
        this.BackupCopyMaxEntryBuildRetries = reader.GetOptionalInt32(nameof (BackupCopyMaxEntryBuildRetries), this.BackupCopyMaxEntryBuildRetries);
        this.BackupCopyEntryBuildInitialRetryTimeout = reader.GetOptionalInt32(nameof (BackupCopyEntryBuildInitialRetryTimeout), this.BackupCopyEntryBuildInitialRetryTimeout);
        this.BackupCopyStorageOptimizationsVisible = reader.GetOptionalBoolean(nameof (BackupCopyStorageOptimizationsVisible), this.BackupCopyStorageOptimizationsVisible);
        this.BackupCopyLookForward = reader.GetOptionalBoolean(nameof (BackupCopyLookForward), this.BackupCopyLookForward);
        this.BackupCopyDisableParallelProcessing = reader.GetOptionalBoolean(nameof (BackupCopyDisableParallelProcessing), this.BackupCopyDisableParallelProcessing);
        this.BackupCopyUnavailableResourcesRetryTimeoutSec = TimeSpan.FromSeconds((double) reader.GetOptionalInt32(nameof (BackupCopyUnavailableResourcesRetryTimeoutSec), 10));
        this.HvDelayBeforeSnapshotCompleteSec = reader.GetOptionalInt32(nameof (HvDelayBeforeSnapshotCompleteSec), this.HvDelayBeforeSnapshotCompleteSec);
        this.HvDelayBeforeSnapshotImportCompleteSec = reader.GetOptionalInt32(nameof (HvDelayBeforeSnapshotImportCompleteSec), this.HvDelayBeforeSnapshotImportCompleteSec);
        this.TapeAgentScriptTimeout = TimeSpan.FromSeconds((double) reader.GetOptionalInt32("TapeAgentScriptTimeoutSec", (int) this.TapeAgentScriptTimeout.TotalSeconds));
        this.HvSnapshotLifeTimeHour = reader.GetOptionalInt32(nameof (HvSnapshotLifeTimeHour), this.HvSnapshotLifeTimeHour);
        this.BackupCopyJobRecheck = reader.GetOptionalBoolean("BackupCopyJobReCheck", this.BackupCopyJobRecheck);
        this.BackupJobRecheck = reader.GetOptionalBoolean("BackupJobReCheck", this.BackupJobRecheck);
        this.BackupCopyForceGfsTransform = reader.GetOptionalBoolean(nameof (BackupCopyForceGfsTransform), this.BackupCopyForceGfsTransform);
        this.BackupCopyCustomGfsStoreConfig = reader.GetOptionalMultiString(nameof (BackupCopyCustomGfsStoreConfig), this.BackupCopyCustomGfsStoreConfig);
        this.BackupCopyEnableIntervalsDebugLogging = reader.GetOptionalBoolean(nameof (BackupCopyEnableIntervalsDebugLogging), this.BackupCopyEnableIntervalsDebugLogging);
        this.BackupCopyJobForceCompact = reader.GetOptionalBoolean(nameof (BackupCopyJobForceCompact), this.BackupCopyJobForceCompact);
        this.BackupJobForceCompact = reader.GetOptionalBoolean(nameof (BackupJobForceCompact), this.BackupJobForceCompact);
        this.CompactSkipModificationTimeCheck = reader.GetOptionalBoolean(nameof (CompactSkipModificationTimeCheck), this.CompactSkipModificationTimeCheck);
        this.CompactDeletedVmRetentionDays = reader.GetOptionalInt32(nameof (CompactDeletedVmRetentionDays), this.CompactDeletedVmRetentionDays);
        this.UseImportedBackupsAsSource = reader.GetOptionalBoolean(nameof (UseImportedBackupsAsSource), this.UseImportedBackupsAsSource);
        this.TapeDeviceWatcherTimeout = TimeSpan.FromSeconds((double) reader.GetOptionalInt32("TapeDeviceWatcherTimeoutSec", (int) this.TapeDeviceWatcherTimeout.TotalSeconds));
        this.TapeDeviceNotReadyTimeout = TimeSpan.FromSeconds((double) reader.GetOptionalInt32("TapeDeviceNotReadyTimeoutSec", (int) this.TapeDeviceNotReadyTimeout.TotalSeconds));
        this.WanServiceTaskStateRequestDelaySec = reader.GetOptionalInt32(nameof (WanServiceTaskStateRequestDelaySec), this.WanServiceTaskStateRequestDelaySec);
        this.WanInlineDataValidationInconsistencyWarning = reader.GetOptionalBoolean(nameof (WanInlineDataValidationInconsistencyWarning), this.WanInlineDataValidationInconsistencyWarning);
        this.ForcedSaveRegistry = reader.GetOptionalBoolean(nameof (ForcedSaveRegistry), this.ForcedSaveRegistry);
        this.CopyLocalRegistryHiveBeforeLoad = reader.GetOptionalBoolean(nameof (CopyLocalRegistryHiveBeforeLoad), this.CopyLocalRegistryHiveBeforeLoad);
        this.UseExportedRegistryHive = reader.GetOptionalBoolean(nameof (UseExportedRegistryHive), this.UseExportedRegistryHive);
        this.MaxExportedRegistryHiveTimeDiffMs = reader.GetOptionalInt32(nameof (MaxExportedRegistryHiveTimeDiffMs), this.MaxExportedRegistryHiveTimeDiffMs);
        this.OverrideRegSeqNumber = reader.GetOptionalBoolean(nameof (OverrideRegSeqNumber), this.OverrideRegSeqNumber);
        this.EnableDBExclusions = reader.GetOptionalBoolean(nameof (EnableDBExclusions), false);
        this.BackupCopyJobFlushPeriod = reader.GetOptionalInt32("BackupCopyJobFlushPeriodSec", this.BackupCopyJobFlushPeriod);
        this.UseCsvCoordinatorForHv2012OnHostBackup = reader.GetOptionalBoolean(nameof (UseCsvCoordinatorForHv2012OnHostBackup), this.UseCsvCoordinatorForHv2012OnHostBackup);
        this.CorePath = reader.GetOptionalString(nameof (CorePath), this.CorePath);
        this.FakeUpdateUrl = reader.GetOptionalString("FakeUpdateOpt", string.Empty);
        this.ViHostConcurrentNfcConnections = reader.GetOptionalInt32(nameof (ViHostConcurrentNfcConnections), this.ViHostConcurrentNfcConnections);
        this.ViHost65ConcurrentNfcConnections = reader.GetOptionalInt32(nameof (ViHost65ConcurrentNfcConnections), this.ViHost65ConcurrentNfcConnections);
        this.EnableWanTransportVrpcCrcCheck = reader.GetOptionalBoolean("EnableWanTransportVRpcCrcCheck", this.EnableWanTransportVrpcCrcCheck);
        this.SourceOibLogIntervalMinutes = reader.GetOptionalInt32(nameof (SourceOibLogIntervalMinutes), this.SourceOibLogIntervalMinutes);
        this.BackupCopyMaxTransformRetries = reader.GetOptionalInt32(nameof (BackupCopyMaxTransformRetries), this.BackupCopyMaxTransformRetries);
        this.HvSnapshotModeCheckerTimeoutSec = reader.GetOptionalInt32(nameof (HvSnapshotModeCheckerTimeoutSec), this.HvSnapshotModeCheckerTimeoutSec);
        this.HvSnapshotModeCheckerPeriodicitySec = reader.GetOptionalInt32(nameof (HvSnapshotModeCheckerPeriodicitySec), this.HvSnapshotModeCheckerPeriodicitySec);
        this.GuestInventoryGatherWritersMetadataTimeoutMs = reader.GetOptionalInt32(nameof (GuestInventoryGatherWritersMetadataTimeoutMs), this.GuestInventoryGatherWritersMetadataTimeoutMs);
        this.DisableBackupComponentsXmlCollectionForJobs = reader.GetOptionalMultiString(nameof (DisableBackupComponentsXmlCollectionForJobs), this.DisableBackupComponentsXmlCollectionForJobs);
        this.DoNotCreateHeartbeatFile = reader.GetOptionalBoolean(nameof (DoNotCreateHeartbeatFile), this.DoNotCreateHeartbeatFile);
        this.RepoBusyTimeoutSec = reader.GetOptionalInt32(nameof (RepoBusyTimeoutSec), this.RepoBusyTimeoutSec);
        this.StgOversizeGbLimit = reader.GetOptionalInt32(nameof (StgOversizeGbLimit), this.StgOversizeGbLimit);
        this.NbdFailoverRequiresHostResource = reader.GetOptionalBoolean(nameof (NbdFailoverRequiresHostResource), this.NbdFailoverRequiresHostResource);
        this.DesiredNbdCompressionType = reader.GetOptionalInt32Enum<EViNbdCompressionType>("VMwareNBDCompressionLevel", this.DesiredNbdCompressionType);
        this.VMwareFailoverToVmPathFromConfig = reader.GetOptionalBoolean(nameof (VMwareFailoverToVmPathFromConfig), this.VMwareFailoverToVmPathFromConfig);
        this.VMware65CbtSnapshotCheckEnabled = reader.GetOptionalBoolean(nameof (VMware65CbtSnapshotCheckEnabled), this.VMware65CbtSnapshotCheckEnabled);
        this.VMwareDisableNFC = reader.GetOptionalBoolean(nameof (VMwareDisableNFC), this.VMwareDisableNFC);
        this.VMwareDisableAsyncIo = reader.GetOptionalBoolean(nameof (VMwareDisableAsyncIo), this.VMwareDisableAsyncIo);
        this.VMwareDisableJitWarmingUp = reader.GetOptionalBoolean(nameof (VMwareDisableJitWarmingUp), this.VMwareDisableJitWarmingUp);
        this.VMwareBlockAsyncNFC = reader.GetOptionalBoolean(nameof (VMwareBlockAsyncNFC), this.VMwareBlockAsyncNFC);
        this.VMwareDisableGlobalUuidPatch = reader.GetOptionalBoolean(nameof (VMwareDisableGlobalUuidPatch), this.VMwareDisableGlobalUuidPatch);
        this.CutHvVmSecuritySettings = reader.GetOptionalBoolean(nameof (CutHvVmSecuritySettings), this.CutHvVmSecuritySettings);
        this.StgAfterRenameVerificationTimeoutSec = reader.GetOptionalInt32(nameof (StgAfterRenameVerificationTimeoutSec), this.StgAfterRenameVerificationTimeoutSec);
        this.MaxRepositoryConnectionsRetryCount = reader.GetOptionalInt32(nameof (MaxRepositoryConnectionsRetryCount), this.MaxRepositoryConnectionsRetryCount);
        this.MaxRepositoryConnectionsRetryTimeoutSec = reader.GetOptionalInt32(nameof (MaxRepositoryConnectionsRetryTimeoutSec), this.MaxRepositoryConnectionsRetryTimeoutSec);
        this.RepositoryWriteRetryCount = reader.GetOptionalInt32(nameof (RepositoryWriteRetryCount), this.RepositoryWriteRetryCount);
        this.RepositoryWriteRetryTimeoutSec = reader.GetOptionalInt32(nameof (RepositoryWriteRetryTimeoutSec), this.RepositoryWriteRetryTimeoutSec);
        this.RediscoverHvClusterVmOwnerHost = reader.GetOptionalBoolean(nameof (RediscoverHvClusterVmOwnerHost), this.RediscoverHvClusterVmOwnerHost);
        this.HvSBCutVmmEthernetFeatures = reader.GetOptionalBoolean(nameof (HvSBCutVmmEthernetFeatures), this.HvSBCutVmmEthernetFeatures);
        this.HvSBCutBandwidthFeature = reader.GetOptionalBoolean(nameof (HvSBCutBandwidthFeature), this.HvSBCutBandwidthFeature);
        this.SanVmSnapshotCreateSemaphoreTimeoutSec = reader.GetOptionalInt32(nameof (SanVmSnapshotCreateSemaphoreTimeoutSec), this.SanVmSnapshotCreateSemaphoreTimeoutSec);
        this.SanMaxConcurrentCreatingVmSnapshotsPerEsx = reader.GetOptionalInt32(nameof (SanMaxConcurrentCreatingVmSnapshotsPerEsx), this.SanMaxConcurrentCreatingVmSnapshotsPerEsx);
        this.SanMaxConcurrentCreatingVmSnapshotsPerVc = reader.GetOptionalInt32(nameof (SanMaxConcurrentCreatingVmSnapshotsPerVc), this.SanMaxConcurrentCreatingVmSnapshotsPerVc);
        this.SanMaxConcurrentMapDiskRegion = reader.GetOptionalInt32(nameof (SanMaxConcurrentMapDiskRegion), this.SanMaxConcurrentMapDiskRegion);
        this.SanMapDiskRegionTimeoutSec = reader.GetOptionalInt32(nameof (SanMapDiskRegionTimeoutSec), this.SanMapDiskRegionTimeoutSec);
        this.AgentMaxReconnectRetries = reader.GetOptionalInt32(nameof (AgentMaxReconnectRetries), this.AgentMaxReconnectRetries);
        this.AgentReconnectRetryIntervalSec = reader.GetOptionalInt32(nameof (AgentReconnectRetryIntervalSec), this.AgentReconnectRetryIntervalSec);
        this.SkipJobSourceRepositoryCheck = reader.GetOptionalBoolean(nameof (SkipJobSourceRepositoryCheck), this.SkipJobSourceRepositoryCheck);
        this.MaxSnapshotsPerDatastore = reader.GetOptionalInt32(nameof (MaxSnapshotsPerDatastore), this.MaxSnapshotsPerDatastore);
        this.DatastoreIOSoapFailBehaviourIsAllowAllCommits = reader.GetOptionalBoolean(nameof (DatastoreIOSoapFailBehaviourIsAllowAllCommits), this.DatastoreIOSoapFailBehaviourIsAllowAllCommits);
        this.AnytimePermittedSnapshotCommitsPerDatastore = reader.GetOptionalInt32(nameof (AnytimePermittedSnapshotCommitsPerDatastore), this.AnytimePermittedSnapshotCommitsPerDatastore);
        this.AnytimePermittedCommonTasksPerDatastore = reader.GetOptionalInt32(nameof (AnytimePermittedCommonTasksPerDatastore), this.AnytimePermittedCommonTasksPerDatastore);
        this.LatencyThrottlerLoggingLevelIsNormal = reader.GetOptionalBoolean(nameof (LatencyThrottlerLoggingLevelIsNormal), this.LatencyThrottlerLoggingLevelIsNormal);
        this.LatencyThrottlerCoeff = reader.GetOptionalString(nameof (LatencyThrottlerCoeff), this.LatencyThrottlerCoeff);
        this.EnableLatencyControlOnVvol = reader.GetOptionalBoolean(nameof (EnableLatencyControlOnVvol), this.EnableLatencyControlOnVvol);
        this.EnableLatencyControlOnVsan = reader.GetOptionalBoolean(nameof (EnableLatencyControlOnVsan), this.EnableLatencyControlOnVsan);
        this.UpgradeComponentsForbidden = reader.GetOptionalBoolean(nameof (UpgradeComponentsForbidden), false);
        this.SqlBackupProxySlots = reader.GetOptionalInt32(nameof (SqlBackupProxySlots), this.SqlBackupProxySlots);
        this.SqlBackupInstanceDatabaseDelimiter = reader.GetOptionalString(nameof (SqlBackupInstanceDatabaseDelimiter), this.SqlBackupInstanceDatabaseDelimiter);
        this.SqlBackupInstanceDatabasePairsDelimiter = reader.GetOptionalString(nameof (SqlBackupInstanceDatabasePairsDelimiter), this.SqlBackupInstanceDatabasePairsDelimiter);
        this.SqlBackupDatabasesToSkip = reader.GetOptionalString(nameof (SqlBackupDatabasesToSkip), this.SqlBackupDatabasesToSkip);
        this.SqlBackupFailedIntervalsBeforeErrorPerDb = reader.GetOptionalInt32(nameof (SqlBackupFailedIntervalsBeforeErrorPerDb), this.SqlBackupFailedIntervalsBeforeErrorPerDb);
        this.LatencyBlockTimeoutSec = reader.GetOptionalInt32(nameof (LatencyBlockTimeoutSec), this.LatencyBlockTimeoutSec);
        this.SqlBackupMaxPrepareRetriesPerInterval = reader.GetOptionalInt32("SqlBackupMaxPrepareFailuresPerInterval", this.SqlBackupMaxPrepareRetriesPerInterval);
        this.SqlBackupPrepareRetryTimeoutSec = reader.GetOptionalInt32(nameof (SqlBackupPrepareRetryTimeoutSec), this.SqlBackupPrepareRetryTimeoutSec);
        this.SqlBackupMaxTasksLoopRetriesPerInterval = reader.GetOptionalInt32("SqlBackupMaxTasksLoopFailuresPerInterval", this.SqlBackupMaxTasksLoopRetriesPerInterval);
        this.SqlBackupTasksLoopRetryTimeoutSec = reader.GetOptionalInt32(nameof (SqlBackupTasksLoopRetryTimeoutSec), this.SqlBackupTasksLoopRetryTimeoutSec);
        this.SqlBackupForceTransportMode = reader.GetOptionalInt32(nameof (SqlBackupForceTransportMode), this.SqlBackupForceTransportMode);
        this.SqlBackupTruncateOldLogsAgeDays = reader.GetOptionalInt32(nameof (SqlBackupTruncateOldLogsAgeDays), this.SqlBackupTruncateOldLogsAgeDays);
        this.SqlLogsAgeDaysToSkipTruncate = reader.GetOptionalInt32("SqlBackupLogsAgeDaysToSkipTruncate", this.SqlLogsAgeDaysToSkipTruncate);
        this.SqlLogsAgeDaysToSkipLogBackup = reader.GetOptionalInt32("SqlBackupLogsAgeDaysToSkipLogBackup", this.SqlLogsAgeDaysToSkipLogBackup);
        this.LogBackupOverrideIntervalSeconds = reader.GetOptionalInt32(nameof (LogBackupOverrideIntervalSeconds), this.LogBackupOverrideIntervalSeconds);
        this.SqlBackupDbDetectorDetailedLogging = reader.GetOptionalBoolean(nameof (SqlBackupDbDetectorDetailedLogging), this.SqlBackupDbDetectorDetailedLogging);
        this.SqlBackupForceVix = reader.GetOptionalBoolean(nameof (SqlBackupForceVix), this.SqlBackupForceVix);
        this.DisableSqlLogChainIntegrityCheck = reader.GetOptionalBoolean(nameof (DisableSqlLogChainIntegrityCheck), this.DisableSqlLogChainIntegrityCheck);
        this.DisableSqlLogBackupErrorReports = reader.GetOptionalBoolean(nameof (DisableSqlLogBackupErrorReports), this.DisableSqlLogBackupErrorReports);
        this.SqlRestoreSelectFullLogsChain = reader.GetOptionalBoolean(nameof (SqlRestoreSelectFullLogsChain), this.SqlRestoreSelectFullLogsChain);
        this.LogBackupJobWaitTimeoutMinutes = reader.GetOptionalInt32(nameof (LogBackupJobWaitTimeoutMinutes), this.LogBackupJobWaitTimeoutMinutes);
        this.LogBackupDisableMetaGeneration = reader.GetOptionalBoolean(nameof (LogBackupDisableMetaGeneration), this.LogBackupDisableMetaGeneration);
        this.SqlBackupJobsStartDelaySeconds = reader.GetOptionalInt32(nameof (SqlBackupJobsStartDelaySeconds), this.SqlBackupJobsStartDelaySeconds);
        this.SkipLogBackupDbFilesPresenceCheck = reader.GetOptionalBoolean(nameof (SkipLogBackupDbFilesPresenceCheck), this.SkipLogBackupDbFilesPresenceCheck);
        this.IrMountLeaseTimeout = TimeSpan.FromMinutes((double) reader.GetOptionalInt32("IrMountLeaseTimeOut", (int) this.IrMountLeaseTimeout.TotalMinutes));
        this.IrVcdMountLeaseTimeout = TimeSpan.FromMinutes((double) reader.GetOptionalInt32("IrVcdMountLeaseTimeOut", (int) this.IrVcdMountLeaseTimeout.TotalMinutes));
        this.AgentLeaseTimeout = TimeSpan.FromMinutes((double) reader.GetOptionalInt32("AgentLeaseTimeoutInMinutes", (int) this.AgentLeaseTimeout.TotalMinutes));
        this.RemoteAgentLeaseTimeout = TimeSpan.FromMinutes((double) reader.GetOptionalInt32("RemoteAgentLeaseTimeoutInMinutes", (int) this.RemoteAgentLeaseTimeout.TotalMinutes));
        this.RestoreLeaseTimeout = TimeSpan.FromMinutes((double) reader.GetOptionalInt32("RestoreLeaseTimeoutInMinutes", (int) this.RestoreLeaseTimeout.TotalMinutes));
        this.HvReplicaRemoveExcludedDisksOnFailback = reader.GetOptionalBoolean(nameof (HvReplicaRemoveExcludedDisksOnFailback), this.HvReplicaRemoveExcludedDisksOnFailback);
        this.TEMP_DisableManagerOperationsExpectThis = reader.GetOptionalString(nameof (TEMP_DisableManagerOperationsExpectThis), (string) null);
        this.DisableFsItemsSizeCalculation = reader.GetOptionalBoolean(nameof (DisableFsItemsSizeCalculation), this.DisableFsItemsSizeCalculation);
        this.HyperVRestrictConcurrentSnapshotCreation = reader.GetOptionalBoolean(nameof (HyperVRestrictConcurrentSnapshotCreation), this.HyperVRestrictConcurrentSnapshotCreation);
        this.HvUseCsvVssWriter = reader.GetOptionalBoolean(nameof (HvUseCsvVssWriter), this.HvUseCsvVssWriter);
        this.ConnectByIPsTimeoutSec = reader.GetOptionalInt32(nameof (ConnectByIPsTimeoutSec), this.ConnectByIPsTimeoutSec);
        this.ForceAgentTrafficEncryption = reader.GetOptionalBoolean(nameof (ForceAgentTrafficEncryption), this.ForceAgentTrafficEncryption);
        this.DisablePublicIPTrafficEncryption = reader.GetOptionalBoolean(nameof (DisablePublicIPTrafficEncryption), this.DisablePublicIPTrafficEncryption);
        this.ShowSplashScreen = reader.GetOptionalBoolean(nameof (ShowSplashScreen), true);
        this.CloudConnectVcdUseUnknownNetworkStatusAsReady = reader.GetOptionalBoolean(nameof (CloudConnectVcdUseUnknownNetworkStatusAsReady), this.CloudConnectVcdUseUnknownNetworkStatusAsReady);
        this.HotaddRemoverCheckIndependentNonpersistent = reader.GetOptionalBoolean(nameof (HotaddRemoverCheckIndependentNonpersistent), this.HotaddRemoverCheckIndependentNonpersistent);
        this.HotaddTimeoutAfterDetach = TimeSpan.FromSeconds((double) reader.GetOptionalInt32("HotaddTimeoutAfterDetachSec", (int) this.HotaddTimeoutAfterDetach.TotalSeconds));
        this.CloudSvcPortStart = reader.GetOptionalInt32("CloudSvcPort", this.CloudSvcPortStart);
        this.CloudSvcPortEnd = reader.GetOptionalInt32(nameof (CloudSvcPortEnd), this.CloudSvcPortEnd);
        this.CloudStartServer = reader.GetOptionalBoolean(nameof (CloudStartServer), this.CloudStartServer);
        this.CloudFreeSpaceWarningThreshold = reader.GetOptionalInt32(nameof (CloudFreeSpaceWarningThreshold), this.CloudFreeSpaceWarningThreshold);
        this.EndPointServerSslPort = reader.GetOptionalInt32(nameof (EndPointServerSslPort), this.EndPointServerSslPort);
        this.EndpointStartSslServer = reader.GetOptionalBoolean(nameof (EndpointStartSslServer), this.EndpointStartSslServer);
        this.LinuxEndPointServerReconnectablePort = reader.GetOptionalInt32("LinuxEndPointServerSslPort", this.LinuxEndPointServerReconnectablePort);
        this.LinuxEndPointStartReconnectableServer = reader.GetOptionalBoolean("LinuxEndPointStartSslServer", this.LinuxEndPointStartReconnectableServer);
        this.EndPointServerPort = reader.GetOptionalInt32(nameof (EndPointServerPort), this.EndPointServerPort);
        this.EndPointStartServer = reader.GetOptionalBoolean(nameof (EndPointStartServer), this.EndPointStartServer);
        this.LinuxEndPointServerPort = reader.GetOptionalInt32(nameof (LinuxEndPointServerPort), this.LinuxEndPointServerPort);
        this.LinuxEndPointStartServer = reader.GetOptionalBoolean("LinuxEndPointStartSslServer", this.LinuxEndPointStartServer);
        this.LinuxEndPointServerAsyncServer = reader.GetOptionalBoolean(nameof (LinuxEndPointServerAsyncServer), this.LinuxEndPointServerAsyncServer);
        this.LinuxEndpointPackagesFolder = reader.GetOptionalString(nameof (LinuxEndpointPackagesFolder), this.LinuxEndpointPackagesFolder);
        this.LinuxEndpointPackageOperationTimeoutSec = reader.GetOptionalInt32(nameof (LinuxEndpointPackageOperationTimeoutSec), this.LinuxEndpointPackageOperationTimeoutSec);
        this.ResourcesUsageDispatcherTimeoutSec = reader.GetOptionalInt32(nameof (ResourcesUsageDispatcherTimeoutSec), this.ResourcesUsageDispatcherTimeoutSec);
        this.AgentThrottlingAlgMixing = reader.GetOptionalInt32AsDouble("AgentThrottlingAlgMixingD10", 10.0, this.AgentThrottlingAlgMixing);
        this.AgentThrottlingAlgWeightMultiplier = reader.GetOptionalInt32AsDouble("AgentThrottlingWeightMultiplierD10", 10.0, this.AgentThrottlingAlgWeightMultiplier);
        this.AgentThrottlingAlgUpdateThresholdKb = reader.GetOptionalInt32(nameof (AgentThrottlingAlgUpdateThresholdKb), this.AgentThrottlingAlgUpdateThresholdKb);
        this.AgentRepositoryAlgUpdateThresholdKb = reader.GetOptionalInt32(nameof (AgentRepositoryAlgUpdateThresholdKb), this.AgentRepositoryAlgUpdateThresholdKb);
        this.HyperVSnapshotCreateTimeoutMin = reader.GetOptionalInt32(nameof (HyperVSnapshotCreateTimeoutMin), this.HyperVSnapshotCreateTimeoutMin);
        this.HyperVSnapshotImportTimeoutMin = reader.GetOptionalInt32(nameof (HyperVSnapshotImportTimeoutMin), this.HyperVSnapshotImportTimeoutMin);
        this.HyperVSnapshotDeleteTimeoutMin = reader.GetOptionalInt32(nameof (HyperVSnapshotDeleteTimeoutMin), this.HyperVSnapshotDeleteTimeoutMin);
        this.LinuxFlrOptionCompress = reader.GetOptionalBoolean(nameof (LinuxFlrOptionCompress), this.LinuxFlrOptionCompress);
        this.LinuxFlrOptionCheckSum = reader.GetOptionalBoolean(nameof (LinuxFlrOptionCheckSum), this.LinuxFlrOptionCheckSum);
        this.DebugFailureSimulator = reader.GetOptionalString(nameof (DebugFailureSimulator), this.DebugFailureSimulator);
        this.MaxVmCountOnHvSoftSnapshot = reader.GetOptionalInt32(nameof (MaxVmCountOnHvSoftSnapshot), this.MaxVmCountOnHvSoftSnapshot);
        this.MaxVmCountOnHvHardSnapshot = reader.GetOptionalInt32(nameof (MaxVmCountOnHvHardSnapshot), this.MaxVmCountOnHvHardSnapshot);
        this.SCVMMConnectionTimeoutMinutes = reader.GetOptionalInt32(nameof (SCVMMConnectionTimeoutMinutes), this.SCVMMConnectionTimeoutMinutes);
        this.HyperVDisableInstantRecoveryStorVSPValidation = reader.GetOptionalBoolean(nameof (HyperVDisableInstantRecoveryStorVSPValidation), this.HyperVDisableInstantRecoveryStorVSPValidation);
        this.HyperVDisableClusterNameValidation = reader.GetOptionalBoolean(nameof (HyperVDisableClusterNameValidation), this.HyperVDisableClusterNameValidation);
        this.SchedulerGlobalStartTimeDayOffset = TimeSpan.FromMinutes((double) reader.GetOptionalInt32(nameof (SchedulerGlobalStartTimeDayOffset), (int) this.SchedulerGlobalStartTimeDayOffset.TotalMinutes));
        if (this.SchedulerGlobalStartTimeDayOffset < TimeSpan.Zero || this.SchedulerGlobalStartTimeDayOffset > TimeSpan.FromHours(23.0) || this.SchedulerGlobalStartTimeDayOffset.TotalMinutes % 60.0 != 0.0)
          throw new Exception("Option 'SchedulerGlobalStartTimeDayOffset' is invalid");
        this.HyperVEnableSnapshotVolumeExclusions = reader.GetOptionalBoolean(nameof (HyperVEnableSnapshotVolumeExclusions), this.HyperVEnableSnapshotVolumeExclusions);
        this.SkipProxyVersionCheck = reader.GetOptionalBoolean(nameof (SkipProxyVersionCheck), this.SkipProxyVersionCheck);
        this.ReCreateDatabase = reader.GetOptionalBoolean(nameof (ReCreateDatabase), this.ReCreateDatabase);
        this.VcdPortalIgnoreNetworkValidationDuringRestore = reader.GetOptionalBoolean(nameof (VcdPortalIgnoreNetworkValidationDuringRestore), this.VcdPortalIgnoreNetworkValidationDuringRestore);
        this.EnableSameHostHotaddMode = (ESameHostHotAdd) reader.GetOptionalInt32(nameof (EnableSameHostHotaddMode), (int) this.EnableSameHostHotaddMode);
        this.EnableSameHostDirectNFSMode = (ESameHostDirectNFS) reader.GetOptionalInt32(nameof (EnableSameHostDirectNFSMode), (int) this.EnableSameHostDirectNFSMode);
        this.EnableFailoverToLegacyHotadd = reader.GetOptionalBoolean(nameof (EnableFailoverToLegacyHotadd), this.EnableFailoverToLegacyHotadd);
        this.SkipIpAddressRangeValidation4TrafficThrottlingRule = reader.GetOptionalBoolean(nameof (SkipIpAddressRangeValidation4TrafficThrottlingRule), this.SkipIpAddressRangeValidation4TrafficThrottlingRule);
        this.HyperVDiskLatencyPermittedTasks = reader.GetOptionalInt32(nameof (HyperVDiskLatencyPermittedTasks), this.HyperVDiskLatencyPermittedTasks);
        this.SshCertificatePath = reader.GetOptionalString(nameof (SshCertificatePath), this.SshCertificatePath);
        this.SshCertificatePassphrase = reader.GetOptionalString(nameof (SshCertificatePassphrase), this.SshCertificatePassphrase);
        this.SshFingerprintRepositoryCheckDisable = !reader.GetOptionalBoolean("SshFingerprintCheck", !this.SshFingerprintRepositoryCheckDisable);
        this.DDBoostSyntheticTransformDisabled = reader.GetOptionalBoolean(nameof (DDBoostSyntheticTransformDisabled), this.DDBoostSyntheticTransformDisabled);
        this.DDBoostSuppressEncryptionError = reader.GetOptionalBoolean(nameof (DDBoostSuppressEncryptionError), this.DDBoostSuppressEncryptionError);
        this.DDBoostDisableSequentialRestore = reader.GetOptionalBoolean(nameof (DDBoostDisableSequentialRestore), this.DDBoostDisableSequentialRestore);
        this.DDBoostSequentialRestoreCacheSize = reader.GetOptionalInt32(nameof (DDBoostSequentialRestoreCacheSize), this.DDBoostSequentialRestoreCacheSize);
        this.DDBoostSequentialRestoreConnectionsCount = reader.GetOptionalInt32(nameof (DDBoostSequentialRestoreConnectionsCount), this.DDBoostSequentialRestoreConnectionsCount);
        this.DDBoostSequentialRestoreMinAgentMemoryMb = reader.GetOptionalInt32(nameof (DDBoostSequentialRestoreMinAgentMemoryMb), this.DDBoostSequentialRestoreMinAgentMemoryMb);
        this.DDBoostDeleteIncrementStorageAfterHours = reader.GetOptionalInt32(nameof (DDBoostDeleteIncrementStorageAfterHours), this.DDBoostDeleteIncrementStorageAfterHours);
        this.DDBoostDisableReadAheadCacheForIR = reader.GetOptionalBoolean(nameof (DDBoostDisableReadAheadCacheForIR), this.DDBoostDisableReadAheadCacheForIR);
        this.Dev_DoNotForceFullsSameDedupExtent = reader.GetOptionalBoolean(nameof (Dev_DoNotForceFullsSameDedupExtent), this.Dev_DoNotForceFullsSameDedupExtent);
        this.RefsVirtualSyntheticDisabled = reader.GetOptionalBoolean(nameof (RefsVirtualSyntheticDisabled), this.RefsVirtualSyntheticDisabled);
        this.UseCifsVirtualSynthetic = (COptions.EUseCifsVirtualSynthetic) reader.GetOptionalInt32(nameof (UseCifsVirtualSynthetic), (int) this.UseCifsVirtualSynthetic);
        this.RefsDedupeBlockClone = reader.GetOptionalBoolean("ReFSDedupeBlockClone", this.RefsDedupeBlockClone);
        this.InactiveFLRSessionTimeout = reader.GetOptionalInt32(nameof (InactiveFLRSessionTimeout), 1800);
        this.LinuxFLRApplianceMemoryLimitMb = reader.GetOptionalInt32(nameof (LinuxFLRApplianceMemoryLimitMb), 1024);
        this.HvSnapInProgressRetryCount = reader.GetOptionalInt32(nameof (HvSnapInProgressRetryCount), this.HvSnapInProgressRetryCount);
        this.HvSnapInProgressRetryWaitSec = reader.GetOptionalInt32(nameof (HvSnapInProgressRetryWaitSec), this.HvSnapInProgressRetryWaitSec);
        this.MultipleInstancesLicensingLockWaitSec = reader.GetOptionalInt32(nameof (MultipleInstancesLicensingLockWaitSec), this.MultipleInstancesLicensingLockWaitSec);
        this.LicenseAutoUpdateServiceUrl = reader.GetOptionalString(nameof (LicenseAutoUpdateServiceUrl), this.LicenseAutoUpdateServiceUrl);
        this.LicenseAutoUpdaterDebugLogging = reader.GetOptionalBoolean(nameof (LicenseAutoUpdaterDebugLogging), this.LicenseAutoUpdaterDebugLogging);
        this.LicenseAutoUpdateConnectionRetryHours = reader.GetOptionalInt32(nameof (LicenseAutoUpdateConnectionRetryHours), this.LicenseAutoUpdateConnectionRetryHours);
        this.NoNewLicenseGeneratedIntervalHours = reader.GetOptionalInt32(nameof (NoNewLicenseGeneratedIntervalHours), this.NoNewLicenseGeneratedIntervalHours);
        this.NonExpiredLicenseAutoUpdateIntervalHours = reader.GetOptionalInt32(nameof (NonExpiredLicenseAutoUpdateIntervalHours), this.NonExpiredLicenseAutoUpdateIntervalHours);
        this.MaxTimeToStartSessionProcess = TimeSpan.FromSeconds((double) reader.GetOptionalInt32(nameof (MaxTimeToStartSessionProcess), (int) this.MaxTimeToStartSessionProcess.TotalSeconds));
        this.SaveJobProgressTimeout = TimeSpan.FromSeconds((double) reader.GetOptionalInt32("SaveJobProgressTimeoutSeconds", (int) this.SaveJobProgressTimeout.TotalSeconds));
        this.ProductAutoUpdateServiceUrl = reader.GetOptionalString(nameof (ProductAutoUpdateServiceUrl), this.ProductAutoUpdateServiceUrl);
        this.SupportCaseServiceUrl = reader.GetOptionalString(nameof (SupportCaseServiceUrl), this.SupportCaseServiceUrl);
        this.DeployStatServiceUrl = reader.GetOptionalString(nameof (DeployStatServiceUrl), this.DeployStatServiceUrl);
        this.IsDeployStatUpdated = reader.GetOptionalBoolean(nameof (IsDeployStatUpdated), this.IsDeployStatUpdated);
        this.SshMaxReconnectRetries = reader.GetOptionalInt32(nameof (SshMaxReconnectRetries), this.SshMaxReconnectRetries);
        this.SshReconnectRetryIntervalSec = reader.GetOptionalInt32(nameof (SshReconnectRetryIntervalSec), this.SshReconnectRetryIntervalSec);
        this.SshFileTransferTimeoutInSeconds = TimeSpan.FromSeconds((double) reader.GetOptionalInt32(nameof (SshFileTransferTimeoutInSeconds), (int) this.SshFileTransferTimeoutInSeconds.TotalSeconds));
        this.SshApplicationTransferTimeout = TimeSpan.FromSeconds((double) reader.GetOptionalInt32("SshApplicationTransferTimeoutInSeconds", (int) this.SshApplicationTransferTimeout.TotalSeconds));
        this.SshRenciLibMaxSessions = reader.GetOptionalInt32(nameof (SshRenciLibMaxSessions), this.SshRenciLibMaxSessions);
        this.SshPromptTimeoutSec = reader.GetOptionalInt32(nameof (SshPromptTimeoutSec), this.SshPromptTimeoutSec);
        this.SshAppResponseWaitTimeoutMin = reader.GetOptionalInt32(nameof (SshAppResponseWaitTimeoutMin), this.SshAppResponseWaitTimeoutMin);
        this.SshConnectionCheckElevateOpTimeoutSec = reader.GetOptionalInt32(nameof (SshConnectionCheckElevateOpTimeoutSec), this.SshConnectionCheckElevateOpTimeoutSec);
        this.DisableClientSideSshKeepalive = reader.GetOptionalBoolean(nameof (DisableClientSideSshKeepalive), this.DisableClientSideSshKeepalive);
        this.OracleSshChunkSizeInBytes = reader.GetOptionalInt32(nameof (OracleSshChunkSizeInBytes), this.OracleSshChunkSizeInBytes);
        this.EnableSSLv3Failback = reader.GetOptionalBoolean(nameof (EnableSSLv3Failback), this.EnableSSLv3Failback);
        this.AgentManagementCRLCheckMode = reader.GetOptionalInt32(nameof (AgentManagementCRLCheckMode), this.AgentManagementCRLCheckMode);
        this.AgentManagementSkipOutOfDateAgents = reader.GetOptionalBoolean(nameof (AgentManagementSkipOutOfDateAgents), this.AgentManagementSkipOutOfDateAgents);
        this.DisableAdditionalVBRCertificateChecks = reader.GetOptionalBoolean(nameof (DisableAdditionalVBRCertificateChecks), this.DisableAdditionalVBRCertificateChecks);
        this.CloudIgnoreInaccessibleKey = reader.GetOptionalBoolean(nameof (CloudIgnoreInaccessibleKey), this.CloudIgnoreInaccessibleKey);
        this.CloudForceImportGuestIndices = reader.GetOptionalBoolean(nameof (CloudForceImportGuestIndices), this.CloudForceImportGuestIndices);
        this.CloudTerminateSessionIntervalMinutes = reader.GetOptionalInt32(nameof (CloudTerminateSessionIntervalMinutes), this.CloudTerminateSessionIntervalMinutes);
        this.CloudTerminateExpiredSessionIntervalMinutes = reader.GetOptionalInt32(nameof (CloudTerminateExpiredSessionIntervalMinutes), this.CloudTerminateExpiredSessionIntervalMinutes);
        this.CloudKeepAliveSessionIntervalMinutes = reader.GetOptionalInt32(nameof (CloudKeepAliveSessionIntervalMinutes), this.CloudKeepAliveSessionIntervalMinutes);
        this.CloudConnectionTimeout = TimeSpan.FromSeconds((double) reader.GetOptionalInt32("CloudConnectionTimeoutSeconds", (int) this.CloudConnectionTimeout.TotalSeconds));
        this.CloudHandshakeTimeout = TimeSpan.FromSeconds((double) reader.GetOptionalInt32("CloudHandshakeTimeoutSeconds", (int) this.CloudHandshakeTimeout.TotalSeconds));
        this.CloudResourceRequestTimeout = TimeSpan.FromSeconds((double) reader.GetOptionalInt32(nameof (CloudResourceRequestTimeout), (int) this.CloudResourceRequestTimeout.TotalSeconds));
        this.CloudResourceRequestConnectionBlockInterval = TimeSpan.FromSeconds((double) reader.GetOptionalInt32(nameof (CloudResourceRequestConnectionBlockInterval), (int) this.CloudResourceRequestConnectionBlockInterval.TotalSeconds));
        this.CloudInvokerDefaultExecutionTimeout = TimeSpan.FromMinutes((double) reader.GetOptionalInt32("CloudInvokerDefaultExecutionTimeoutMinutes", (int) this.CloudInvokerDefaultExecutionTimeout.TotalMinutes));
        this.CloudInvokerDefaultAsyncStateUpdateTimeout = TimeSpan.FromSeconds((double) reader.GetOptionalInt32("CloudInvokerDefaultAsyncStateUpdateTimeoutSeconds", (int) this.CloudInvokerDefaultAsyncStateUpdateTimeout.TotalSeconds));
        this.CloudConnectEnhancedSecurityMode = reader.GetOptionalInt32(nameof (CloudConnectEnhancedSecurityMode), this.CloudConnectEnhancedSecurityMode);
        this.CloudConnectCRLCheckMode = reader.GetOptionalInt32(nameof (CloudConnectCRLCheckMode), this.CloudConnectCRLCheckMode);
        this.CloudLogAccessorMinTimeout = TimeSpan.FromSeconds((double) Math.Max(5, reader.GetOptionalInt32("CloudLogAccessorMinTimeoutSeconds", (int) this.CloudLogAccessorMinTimeout.TotalSeconds)));
        this.CloudReportAccessorMinTimeout = TimeSpan.FromSeconds((double) Math.Max(10, reader.GetOptionalInt32("CloudReportAccessorMinTimeoutSeconds", (int) this.CloudReportAccessorMinTimeout.TotalSeconds)));
        this.CloudFileSystemSynchronizationTimeout = TimeSpan.FromMinutes((double) Math.Max(5, reader.GetOptionalInt32("CloudFileSystemSynchronizationTimeoutMinutes", (int) this.CloudFileSystemSynchronizationTimeout.TotalMinutes)));
        this.CloudResyncBackupVms = reader.GetOptionalBoolean(nameof (CloudResyncBackupVms), this.CloudResyncBackupVms);
        this.CloudTapNetwork = reader.GetOptionalString(nameof (CloudTapNetwork), this.CloudTapNetwork);
        this.CloudApplianceDebug = reader.GetOptionalBoolean(nameof (CloudApplianceDebug), this.CloudApplianceDebug);
        this.DisableCollectingLogsFromCloudAppliances = reader.GetOptionalBoolean(nameof (DisableCollectingLogsFromCloudAppliances), this.DisableCollectingLogsFromCloudAppliances);
        this.DisableTurningOffCloudAppliances = reader.GetOptionalBoolean(nameof (DisableTurningOffCloudAppliances), this.DisableTurningOffCloudAppliances);
        this.DisableSuccessCloudConnectReport = reader.GetOptionalBoolean(nameof (DisableSuccessCloudConnectReport), this.DisableSuccessCloudConnectReport);
        this.CloudDisableRetrievingStatistics = reader.GetOptionalBoolean(nameof (CloudDisableRetrievingStatistics), this.CloudDisableRetrievingStatistics);
        this.CloudAllowBetaAgentAccess = reader.GetOptionalBoolean("AllowBetaAgentAccess", this.CloudAllowBetaAgentAccess);
        this.CloudConnectDisableDiskMounting = reader.GetOptionalBoolean(nameof (CloudConnectDisableDiskMounting), this.CloudConnectDisableDiskMounting);
        this.NEAIPAddressWaitTimeoutSec = reader.GetOptionalInt32(nameof (NEAIPAddressWaitTimeoutSec), this.NEAIPAddressWaitTimeoutSec);
        this.VCCReplicaIPAddressWaitTimeoutSec = reader.GetOptionalInt32(nameof (VCCReplicaIPAddressWaitTimeoutSec), this.VCCReplicaIPAddressWaitTimeoutSec);
        this.UseModifiedCloudBackupsUpgradeAlg = reader.GetOptionalBoolean(nameof (UseModifiedCloudBackupsUpgradeAlg), this.UseModifiedCloudBackupsUpgradeAlg);
        string optionalString11 = reader.GetOptionalString(nameof (CloudConnectReportTime), string.Empty);
        this.CloudConnectReportTime = string.IsNullOrEmpty(optionalString11) ? new TimeSpan?() : new TimeSpan?(TimeSpan.Parse(optionalString11));
        this.ReducedLoggingModeThreshold = reader.GetOptionalInt32(nameof (ReducedLoggingModeThreshold), this.ReducedLoggingModeThreshold);
        this.EncryptedTenantBackupsOnly = reader.GetOptionalBoolean(nameof (EncryptedTenantBackupsOnly), this.EncryptedTenantBackupsOnly);
        this.TenantToTapeSkipRetiredBackupFiles = Math.Max(1, reader.GetOptionalInt32(nameof (TenantToTapeSkipRetiredBackupFiles), this.TenantToTapeSkipRetiredBackupFiles));
        this.ForeignInvokerDefaultExecutionTimeout = TimeSpan.FromMinutes((double) reader.GetOptionalInt32("ForeignInvokerDefaultExecutionTimeoutMinutes", (int) this.ForeignInvokerDefaultExecutionTimeout.TotalMinutes));
        this.ForeignInvokerServerTcpQueueSize = reader.GetOptionalInt32(nameof (ForeignInvokerServerTcpQueueSize), this.ForeignInvokerServerTcpQueueSize);
        this.UseForeignAsyncInvoker = reader.GetOptionalBoolean(nameof (UseForeignAsyncInvoker), this.UseForeignAsyncInvoker);
        this.ForeignInvokerUseThreadPool = reader.GetOptionalBoolean(nameof (ForeignInvokerUseThreadPool), this.ForeignInvokerUseThreadPool);
        this.VsanBestProxyDataAmountPercent = reader.GetOptionalInt32(nameof (VsanBestProxyDataAmountPercent), this.VsanBestProxyDataAmountPercent);
        this.EpBackupJobRetryEnabled = reader.GetOptionalBoolean(nameof (EpBackupJobRetryEnabled), this.EpBackupJobRetryEnabled);
        this.EpBackupAllowRecursiveWildcards = reader.GetOptionalBoolean(nameof (EpBackupAllowRecursiveWildcards), this.EpBackupAllowRecursiveWildcards);
        this.EpBackupJobRetriesCount = reader.GetOptionalInt32(nameof (EpBackupJobRetriesCount), this.EpBackupJobRetriesCount);
        this.EpFileBackupMountVirtualVolume = reader.GetOptionalBoolean(nameof (EpFileBackupMountVirtualVolume), this.EpFileBackupMountVirtualVolume);
        this.EpVSSForceCopyOnly = reader.GetOptionalBoolean("VSSForceCopyOnly", this.EpVSSForceCopyOnly);
        this.EnableNewAgentReconnectEngine = reader.GetOptionalBoolean(nameof (EnableNewAgentReconnectEngine), this.EnableNewAgentReconnectEngine);
        this.NewAgentReconnectEngineTimeoutSec = reader.GetOptionalInt32(nameof (NewAgentReconnectEngineTimeoutSec), this.NewAgentReconnectEngineTimeoutSec);
        this.IsReconnectableTapeAgents = reader.GetOptionalBoolean(nameof (IsReconnectableTapeAgents), this.IsReconnectableTapeAgents);
        this.DisableEjectWarning = reader.GetOptionalBoolean(nameof (DisableEjectWarning), this.DisableEjectWarning);
        this.UndoFailoverPlanPackageCount = reader.GetOptionalInt32(nameof (UndoFailoverPlanPackageCount), this.UndoFailoverPlanPackageCount);
        this.UndoFailoverPlanTimeoutSec = reader.GetOptionalInt32(nameof (UndoFailoverPlanTimeoutSec), this.UndoFailoverPlanTimeoutSec);
        this.FailoverPlanThreadsCount = reader.GetOptionalInt32(nameof (FailoverPlanThreadsCount), this.FailoverPlanThreadsCount);
        this.FailoverPlanScriptTimeoutSec = reader.GetOptionalInt32(nameof (FailoverPlanScriptTimeoutSec), this.FailoverPlanScriptTimeoutSec);
        this.NetworkExtensionAppliancePreFailoverScript = reader.GetOptionalString(nameof (NetworkExtensionAppliancePreFailoverScript), this.NetworkExtensionAppliancePreFailoverScript);
        this.NetworkExtensionAppliancePostFailoverScript = reader.GetOptionalString(nameof (NetworkExtensionAppliancePostFailoverScript), this.NetworkExtensionAppliancePostFailoverScript);
        this.NetworkExtensionAppliancePreUndoFailoverScript = reader.GetOptionalString(nameof (NetworkExtensionAppliancePreUndoFailoverScript), this.NetworkExtensionAppliancePreUndoFailoverScript);
        this.NetworkExtensionAppliancePostUndoFailoverScript = reader.GetOptionalString(nameof (NetworkExtensionAppliancePostUndoFailoverScript), this.NetworkExtensionAppliancePostUndoFailoverScript);
        this.ReplaceDefaultWindowsRE = reader.GetOptionalBoolean(nameof (ReplaceDefaultWindowsRE), this.ReplaceDefaultWindowsRE);
        this.ImpersonateGuestScript = reader.GetOptionalBoolean(nameof (ImpersonateGuestScript), this.ImpersonateGuestScript);
        this.SwitchToPersistentVssSnapshot = reader.GetOptionalBoolean(nameof (SwitchToPersistentVssSnapshot), this.SwitchToPersistentVssSnapshot);
        this.NLBClusterPrimaryIps = reader.GetOptionalMultiString(nameof (NLBClusterPrimaryIps), this.NLBClusterPrimaryIps);
        this.HvForcePersistentSnapshot = reader.GetOptionalBoolean(nameof (HvForcePersistentSnapshot), this.HvForcePersistentSnapshot);
        this.WaTransportDebugMode = reader.GetOptionalBoolean(nameof (WaTransportDebugMode), this.WaTransportDebugMode);
        this.MaxJobNameLength = reader.GetOptionalInt32(nameof (MaxJobNameLength), this.MaxJobNameLength);
        this.HyperVDefaultWMITimeoutSec = Math.Max(0, reader.GetOptionalInt32(nameof (HyperVDefaultWMITimeoutSec), this.HyperVDefaultWMITimeoutSec));
        this.HyperVVmTaskRescheduleLimit = Math.Max(0, reader.GetOptionalInt32(nameof (HyperVVmTaskRescheduleLimit), this.HyperVVmTaskRescheduleLimit));
        this.HyperVCheckBackupStateInResourceScheduler = reader.GetOptionalBoolean(nameof (HyperVCheckBackupStateInResourceScheduler), this.HyperVCheckBackupStateInResourceScheduler);
        this._hvCbtTestJobs.AddRange((IEnumerable<string>) reader.GetOptionalString(nameof (HvCbtTestJobs), "").Split(';'));
        this.VbServiceDesktopHeapSizeKb = reader.GetOptionalInt32(nameof (VbServiceDesktopHeapSizeKb), this.VbServiceDesktopHeapSizeKb);
        this.NewJobScheduler = reader.GetOptionalBoolean(nameof (NewJobScheduler), this.NewJobScheduler);
        this.LinuxFLRApplianceKeepAliveForDebugMins = reader.GetOptionalInt32(nameof (LinuxFLRApplianceKeepAliveForDebugMins), 0);
        this.PreJobScriptTimeoutSec = reader.GetOptionalInt32(nameof (PreJobScriptTimeoutSec), this.PreJobScriptTimeoutSec);
        this.PostJobScriptTimeoutSec = reader.GetOptionalInt32(nameof (PostJobScriptTimeoutSec), this.PostJobScriptTimeoutSec);
        this.EnableBackupRepositoryFreeSpaceCheck = reader.GetOptionalBoolean(nameof (EnableBackupRepositoryFreeSpaceCheck), this.EnableBackupRepositoryFreeSpaceCheck);
        this.BackupRepositoryFreeSpaceThresholdPercent = reader.GetOptionalInt32(nameof (BackupRepositoryFreeSpaceThresholdPercent), this.BackupRepositoryFreeSpaceThresholdPercent);
        this.ManageByProvider = reader.GetOptionalBoolean(nameof (ManageByProvider), this.ManageByProvider);
        this.IsShhConnectionDisposingDissalowedInShell = reader.GetOptionalBoolean(nameof (IsShhConnectionDisposingDissalowedInShell), this.IsShhConnectionDisposingDissalowedInShell);
        this.MaxGuestScriptTimeoutSec = reader.GetOptionalInt32(nameof (MaxGuestScriptTimeoutSec), this.MaxGuestScriptTimeoutSec);
        this.PreJobScriptTimeoutSec = reader.GetOptionalInt32(nameof (PreJobScriptTimeoutSec), this.PreJobScriptTimeoutSec);
        this.PostJobScriptTimeoutSec = reader.GetOptionalInt32(nameof (PostJobScriptTimeoutSec), this.PostJobScriptTimeoutSec);
        this.EnableBackupRepositoryFreeSpaceCheck = reader.GetOptionalBoolean(nameof (EnableBackupRepositoryFreeSpaceCheck), this.EnableBackupRepositoryFreeSpaceCheck);
        this.BackupRepositoryFreeSpaceThresholdPercent = reader.GetOptionalInt32(nameof (BackupRepositoryFreeSpaceThresholdPercent), this.BackupRepositoryFreeSpaceThresholdPercent);
        this.EnableEndPointPreVssFreezeSpaceCheck = reader.GetOptionalBoolean(nameof (EnableEndPointPreVssFreezeSpaceCheck), this.EnableEndPointPreVssFreezeSpaceCheck);
        this.SkipUpdateToMajorVersion = reader.GetOptionalBoolean(nameof (SkipUpdateToMajorVersion), this.SkipUpdateToMajorVersion);
        this.WaMaxConnectRetries = reader.GetOptionalInt32(nameof (WaMaxConnectRetries), this.WaMaxConnectRetries);
        this.SqlBackupMaxParallelThreads = reader.GetOptionalInt32(nameof (SqlBackupMaxParallelThreads), this.SqlBackupMaxParallelThreads);
        this.PostProcessorMaxParallelThreads = reader.GetOptionalInt32(nameof (PostProcessorMaxParallelThreads), this.PostProcessorMaxParallelThreads);
        this.CrossConfigurationRestore = reader.GetOptionalBoolean(nameof (CrossConfigurationRestore), this.CrossConfigurationRestore);
        this.UseTapeAgent32Bit = reader.GetOptionalBoolean(nameof (UseTapeAgent32Bit), this.UseTapeAgent32Bit);
        this.EpEnableVirtualDiskProcessing = reader.GetOptionalBoolean(nameof (EpEnableVirtualDiskProcessing), this.EpEnableVirtualDiskProcessing);
        this.ViSnapshotConsolidationRetryIntervalMinutes = reader.GetOptionalInt32(nameof (ViSnapshotConsolidationRetryIntervalMinutes), this.ViSnapshotConsolidationRetryIntervalMinutes);
        this.StuckSnapshotWarning = reader.GetOptionalBoolean(nameof (StuckSnapshotWarning), this.StuckSnapshotWarning);
        this.DisableAutoSnapshotConsolidation = reader.GetOptionalBoolean(nameof (DisableAutoSnapshotConsolidation), this.DisableAutoSnapshotConsolidation);
        this.SkipInstantClones = reader.GetOptionalBoolean(nameof (SkipInstantClones), this.SkipInstantClones);
        this.SkipViConfigFilesWithoutExtension = reader.GetOptionalBoolean(nameof (SkipViConfigFilesWithoutExtension), this.SkipViConfigFilesWithoutExtension);
        this.TagsPriorityOverContainers = reader.GetOptionalBoolean(nameof (TagsPriorityOverContainers), this.TagsPriorityOverContainers);
        this.ResetCBTOnDiskResize = reader.GetOptionalBoolean(nameof (ResetCBTOnDiskResize), this.ResetCBTOnDiskResize);
        this.UseCBTOnDiskResize = reader.GetOptionalBoolean(nameof (UseCBTOnDiskResize), this.UseCBTOnDiskResize);
        this.MaxNetworkNameLength = reader.GetOptionalInt32(nameof (MaxNetworkNameLength), this.MaxNetworkNameLength);
        this.EpMaxBackupServerPortRetries = reader.GetOptionalInt32(nameof (EpMaxBackupServerPortRetries), this.EpMaxBackupServerPortRetries);
        this.LinuxIndexingPerCommandTimeoutSec = reader.GetOptionalInt32(nameof (LinuxIndexingPerCommandTimeoutSec), this.LinuxIndexingPerCommandTimeoutSec);
        this.LinuxIndexingUpdateDbTimeoutSec = reader.GetOptionalInt32(nameof (LinuxIndexingUpdateDbTimeoutSec), this.LinuxIndexingUpdateDbTimeoutSec);
        this.UseIndexingOptionForVss = reader.GetOptionalBoolean(nameof (UseIndexingOptionForVss), this.UseIndexingOptionForVss);
        this.StoreOnceFileSessionOverhead = reader.GetOptionalInt32(nameof (StoreOnceFileSessionOverhead), this.StoreOnceFileSessionOverhead);
        this.StoreOnceMaxFcFileSessions = reader.GetOptionalInt32(nameof (StoreOnceMaxFcFileSessions), this.StoreOnceMaxFcFileSessions);
        this.StoreOnceRtsUseRepositoryId = reader.GetOptionalBoolean(nameof (StoreOnceRtsUseRepositoryId), this.StoreOnceRtsUseRepositoryId);
        this.StoreOnceResourceScanTtlSec = reader.GetOptionalInt32(nameof (StoreOnceResourceScanTtlSec), this.StoreOnceResourceScanTtlSec);
        this.StoreOnceMinFcPortCount = reader.GetOptionalInt32(nameof (StoreOnceMinFcPortCount), this.StoreOnceMinFcPortCount);
        this.StoreOnceAllowStorageTransfer = reader.GetOptionalBoolean(nameof (StoreOnceAllowStorageTransfer), this.StoreOnceAllowStorageTransfer);
        this.StoreOnceSwitchToNewFileSessionsAccounting = reader.GetOptionalBoolean(nameof (StoreOnceSwitchToNewFileSessionsAccounting), this.StoreOnceSwitchToNewFileSessionsAccounting);
        this.StoreOnceDisableSequentialRestore = reader.GetOptionalBoolean(nameof (StoreOnceDisableSequentialRestore), this.StoreOnceDisableSequentialRestore);
        this.StoreOnceSequentialRestoreCacheSize = reader.GetOptionalInt32(nameof (StoreOnceSequentialRestoreCacheSize), this.StoreOnceSequentialRestoreCacheSize);
        this.StoreOnceDisableSharedRead = reader.GetOptionalBoolean(nameof (StoreOnceDisableSharedRead), this.StoreOnceDisableSharedRead);
        this.StoreOnceCheckExclusiveReadLocks = reader.GetOptionalBoolean(nameof (StoreOnceCheckExclusiveReadLocks), this.StoreOnceCheckExclusiveReadLocks);
        this.DataMoverLocalFastPath = reader.GetOptionalInt32(nameof (DataMoverLocalFastPath), this.DataMoverLocalFastPath);
        this.LogExportPath = reader.GetOptionalString(nameof (LogExportPath), this.LogExportPath);
        this.TempActiveGFSIsAlwaysFull = reader.GetOptionalBoolean(nameof (TempActiveGFSIsAlwaysFull), this.TempActiveGFSIsAlwaysFull);
        this.HvSleepAfterGuestInstalledSec = reader.GetOptionalInt32(nameof (HvSleepAfterGuestInstalledSec), this.HvSleepAfterGuestInstalledSec);
        this.GFSFullBackupRetryMultiplicator = reader.GetOptionalInt32("Dev_GFSBackupRetryMultiplicator", this.GFSFullBackupRetryMultiplicator);
        this.DisableHvSoftwareSnapshotWhenFailoverDisabled = reader.GetOptionalBoolean(nameof (DisableHvSoftwareSnapshotWhenFailoverDisabled), this.DisableHvSoftwareSnapshotWhenFailoverDisabled);
        this.HvRestoreCutVmmEthernetFeatures = reader.GetOptionalBoolean(nameof (HvRestoreCutVmmEthernetFeatures), this.HvRestoreCutVmmEthernetFeatures);
        this.DeleteOldDbRecordsAfterPeriodInDays = reader.GetOptionalInt32(nameof (DeleteOldDbRecordsAfterPeriodInDays), this.DeleteOldDbRecordsAfterPeriodInDays);
        this.RestoreAuditDataRetentionInDays = reader.GetOptionalInt32(nameof (RestoreAuditDataRetentionInDays), this.RestoreAuditDataRetentionInDays);
        this.StatisticServiceDispatchIntervalSec = reader.GetOptionalInt32(nameof (StatisticServiceDispatchIntervalSec), this.StatisticServiceDispatchIntervalSec);
        this.HyperVVmMemoryLimitDecrementInPercent = reader.GetOptionalInt32(nameof (HyperVVmMemoryLimitDecrementInPercent), this.HyperVVmMemoryLimitDecrementInPercent);
        int optionalInt32_2 = reader.GetOptionalInt32(nameof (DisableCheckAdminBlockByUac), -1);
        this.DisableCheckAdminBlockByUac = optionalInt32_2 == -1 ? new int?() : new int?(optionalInt32_2);
        this.HvSyncTaskRescheduleTimeoutMin = reader.GetOptionalInt32(nameof (HvSyncTaskRescheduleTimeoutMin), this.HvSyncTaskRescheduleTimeoutMin);
        this.EnableRestoreSNMPTraps = reader.GetOptionalBoolean(nameof (EnableRestoreSNMPTraps), this.EnableRestoreSNMPTraps);
        this.DefaultLinuxAgentVersion = (COptions.EDefaultLinuxAgent) reader.GetOptionalInt32(nameof (DefaultLinuxAgentVersion), (int) this.DefaultLinuxAgentVersion);
        this.HvWmiReconnectsCount = reader.GetOptionalInt32(nameof (HvWmiReconnectsCount), this.HvWmiReconnectsCount);
        this.HvVmVssComponentWaitTimeout = reader.GetOptionalInt32(nameof (HvVmVssComponentWaitTimeout), this.HvVmVssComponentWaitTimeout);
        this.AgentReadOnlyCache = (COptions.EAgentReadOnlyCache) reader.GetOptionalInt32(nameof (AgentReadOnlyCache), (int) this.AgentReadOnlyCache);
        this.AlternativeWebServerPort = reader.GetOptionalInt32(nameof (AlternativeWebServerPort), this.AlternativeWebServerPort);
        this.DbMaintenanceMode = reader.GetOptionalInt32(nameof (DbMaintenanceMode), this.DbMaintenanceMode);
        this.DbMaintenanceStatisticsUpdateMode = reader.GetOptionalInt32(nameof (DbMaintenanceStatisticsUpdateMode), this.DbMaintenanceStatisticsUpdateMode);
        this.DbMaintenanceStatisticsUpdateMinRows = reader.GetOptionalInt32(nameof (DbMaintenanceStatisticsUpdateMinRows), this.DbMaintenanceStatisticsUpdateMinRows);
        this.DbMaintenanceStatisticsUpdateMaxRows = reader.GetOptionalInt32(nameof (DbMaintenanceStatisticsUpdateMaxRows), this.DbMaintenanceStatisticsUpdateMaxRows);
        this.DbMaintenanceFragmentationThreshold = reader.GetOptionalInt32("DbMaintenancefragmentationThreshold", this.DbMaintenanceFragmentationThreshold);
        this.DbMaintenanceIndexDefragMode = reader.GetOptionalInt32(nameof (DbMaintenanceIndexDefragMode), this.DbMaintenanceIndexDefragMode);
        this.TombStoneRetentionPeriod = reader.GetOptionalInt32(nameof (TombStoneRetentionPeriod), this.TombStoneRetentionPeriod);
        this.IsDbMaintenanceJobEnabled = reader.GetOptionalInt32(nameof (IsDbMaintenanceJobEnabled), this.IsDbMaintenanceJobEnabled);
        this.UpdateStatisticsTimeout = reader.GetOptionalInt32(nameof (UpdateStatisticsTimeout), this.UpdateStatisticsTimeout);
        this.IndexDefragTimeout = reader.GetOptionalInt32(nameof (IndexDefragTimeout), this.IndexDefragTimeout);
        this.TombstonesRetentionTimeout = reader.GetOptionalInt32(nameof (TombstonesRetentionTimeout), this.TombstonesRetentionTimeout);
        this.XmlCompressionTimeout = reader.GetOptionalInt32(nameof (XmlCompressionTimeout), this.XmlCompressionTimeout);
        this.AuxCloudSessionsRetentionTimeout = reader.GetOptionalInt32(nameof (AuxCloudSessionsRetentionTimeout), this.AuxCloudSessionsRetentionTimeout);
        this.TapeCleanupChunkSize = reader.GetOptionalInt32(nameof (TapeCleanupChunkSize), this.TapeCleanupChunkSize);
        this.AuxCloudSessionsChunkSize = reader.GetOptionalInt32(nameof (AuxCloudSessionsChunkSize), this.AuxCloudSessionsChunkSize);
        this.IndexFragmentationAnalysisTimeout = reader.GetOptionalInt32(nameof (IndexFragmentationAnalysisTimeout), this.IndexFragmentationAnalysisTimeout);
        this.XmlDbDataCompressionEnabled = reader.GetOptionalInt32(nameof (XmlDbDataCompressionEnabled), this.XmlDbDataCompressionEnabled);
        this.XmlDbCompessionChunkSize = reader.GetOptionalInt32(nameof (XmlDbCompessionChunkSize), this.XmlDbCompessionChunkSize);
        this.CompressionThresholdSizeBackupTaskSessions = reader.GetOptionalInt32(nameof (CompressionThresholdSizeBackupTaskSessions), this.CompressionThresholdSizeBackupTaskSessions);
        this.CompressionThresholdSizeOibs = reader.GetOptionalInt32(nameof (CompressionThresholdSizeOibs), this.CompressionThresholdSizeOibs);
        this.AuxCloudSessionsRetentionPeriod = reader.GetOptionalInt32(nameof (AuxCloudSessionsRetentionPeriod), this.AuxCloudSessionsRetentionPeriod);
        this.ChildBackupEntitiesRemovalTimeout = reader.GetOptionalInt32(nameof (ChildBackupEntitiesRemovalTimeout), this.ChildBackupEntitiesRemovalTimeout);
        this.AppendPreparedStorageAssociationsTimeout = reader.GetOptionalInt32(nameof (AppendPreparedStorageAssociationsTimeout), this.AppendPreparedStorageAssociationsTimeout);
        this.DatabaseDeadlockRetryNumber = reader.GetOptionalInt32(nameof (DatabaseDeadlockRetryNumber), this.DatabaseDeadlockRetryNumber);
        this.DatabaseDeadlockRetrySleepTime = reader.GetOptionalInt32(nameof (DatabaseDeadlockRetrySleepTime), this.DatabaseDeadlockRetrySleepTime);
        this.DatabaseTimeoutRetryNumber = reader.GetOptionalInt32(nameof (DatabaseTimeoutRetryNumber), this.DatabaseTimeoutRetryNumber);
        this.DatabaseTimeoutRetrySleepTime = reader.GetOptionalInt32(nameof (DatabaseTimeoutRetrySleepTime), this.DatabaseTimeoutRetrySleepTime);
        this.DatabaseBrokenConnectionRetryNumber = reader.GetOptionalInt32(nameof (DatabaseBrokenConnectionRetryNumber), this.DatabaseBrokenConnectionRetryNumber);
        this.DatabaseBrokenConnectionRetrySleepTime = reader.GetOptionalInt32(nameof (DatabaseBrokenConnectionRetrySleepTime), this.DatabaseBrokenConnectionRetrySleepTime);
        this.XmlExceptionRetryNumber = reader.GetOptionalInt32(nameof (XmlExceptionRetryNumber), this.XmlExceptionRetryNumber);
        this.XmlExceptionRetrySleepTime = reader.GetOptionalInt32(nameof (XmlExceptionRetrySleepTime), this.XmlExceptionRetrySleepTime);
        this.DatabaseGenericRetryNumber = reader.GetOptionalInt32(nameof (DatabaseGenericRetryNumber), this.DatabaseGenericRetryNumber);
        this.DatabaseGenericRetrySleepTime = reader.GetOptionalInt32(nameof (DatabaseGenericRetrySleepTime), this.DatabaseGenericRetrySleepTime);
        this.ResolveCloudGatewayAddressesToIPs = reader.GetOptionalBoolean(nameof (ResolveCloudGatewayAddressesToIPs), this.ResolveCloudGatewayAddressesToIPs);
        this.HyperVVmMemoryLimitDecrementInPercent = reader.GetOptionalInt32(nameof (HyperVVmMemoryLimitDecrementInPercent), this.HyperVVmMemoryLimitDecrementInPercent);
        this.Force64BitAgent = reader.GetOptionalBoolean(nameof (Force64BitAgent), this.Force64BitAgent);
        this.UserAuthTokenLiveTime = reader.GetOptionalInt32(nameof (UserAuthTokenLiveTime), 600);
        this.HvVmVssComponentWaitTimeout = reader.GetOptionalInt32(nameof (HvVmVssComponentWaitTimeout), this.HvVmVssComponentWaitTimeout);
        this.FilterApipaAddresses = reader.GetOptionalBoolean(nameof (FilterApipaAddresses), this.FilterApipaAddresses);
        this.SupportPbmProfiles = reader.GetOptionalBoolean(nameof (SupportPbmProfiles), this.SupportPbmProfiles);
        this.EnablePerFileFLRLogging = reader.GetOptionalBoolean(nameof (EnablePerFileFLRLogging), this.EnablePerFileFLRLogging);
        this.BsCertificateFriendlyName = reader.GetOptionalString(nameof (BsCertificateFriendlyName), this.BsCertificateFriendlyName);
        this.HyperVRemoteWmiToggleLogic2012 = reader.GetOptionalBoolean(nameof (HyperVRemoteWmiToggleLogic2012), false);
        this.HyperVRemoteWmiToggleLogic2015 = reader.GetOptionalBoolean(nameof (HyperVRemoteWmiToggleLogic2015), false);
        this.EnableIscsiMount = reader.GetOptionalBoolean(nameof (EnableIscsiMount), this.EnableIscsiMount);
        this.UseRepositoryMountServerDuringRemoteMount = reader.GetOptionalBoolean(nameof (UseRepositoryMountServerDuringRemoteMount), this.UseRepositoryMountServerDuringRemoteMount);
        this.ForceMountIpCollect = reader.GetOptionalBoolean(nameof (ForceMountIpCollect), this.ForceMountIpCollect);
        this.EagerZeroedDiskRestore = reader.GetOptionalBoolean(nameof (EagerZeroedDiskRestore), this.EagerZeroedDiskRestore);
        this.UseKB2047927ForGettingMountPoints = reader.GetOptionalBoolean(nameof (UseKB2047927ForGettingMountPoints), this.UseKB2047927ForGettingMountPoints);
        this.UseVeeamFsrDriver = reader.GetOptionalBoolean(nameof (UseVeeamFsrDriver), this.UseVeeamFsrDriver);
        this.UseVpnOverTcp = reader.GetOptionalBoolean(nameof (UseVpnOverTcp), this.UseVpnOverTcp);
        this.DisableVpnServerFirewall = reader.GetOptionalBoolean(nameof (DisableVpnServerFirewall), this.DisableVpnServerFirewall);
        this.NEACustomIPExclusions = reader.GetOptionalString(nameof (NEACustomIPExclusions), this.NEACustomIPExclusions);
        this.ResourceScanDefaultPeriod = TimeSpan.FromSeconds((double) reader.GetOptionalInt32("ResourceScanDefaultPeriodSec", (int) this.ResourceScanDefaultPeriod.TotalSeconds));
        this.ResourceScanDefaultTTLHost = TimeSpan.FromSeconds((double) reader.GetOptionalInt32("ResourceScanDefaultTTLHostSec", (int) this.ResourceScanDefaultTTLHost.TotalSeconds));
        this.ResourceScanDefaultTTLRepository = TimeSpan.FromSeconds((double) reader.GetOptionalInt32("ResourceScanDefaultTTLRepositorySec", (int) this.ResourceScanDefaultTTLRepository.TotalSeconds));
        this.ResourceScanDefaultTTLVpn = TimeSpan.FromSeconds((double) reader.GetOptionalInt32("ResourceScanDefaultTTLVPNConnectionSec", (int) this.ResourceScanDefaultTTLVpn.TotalSeconds));
        this.ResourceScanVpnDefaultPeriod = TimeSpan.FromSeconds((double) reader.GetOptionalInt32("ResourceScanVPNConnectionDefaultPeriodSec", (int) this.ResourceScanVpnDefaultPeriod.TotalSeconds));
        this.ResourceScanCheckStopPeriod = TimeSpan.FromSeconds((double) reader.GetOptionalInt32("ResourceScanCheckStopPeriodSec", (int) this.ResourceScanCheckStopPeriod.TotalSeconds));
        this.ResourceScanDispatchPeriod = TimeSpan.FromSeconds((double) reader.GetOptionalInt32("ResourceScanDispatchPeriodSec", (int) this.ResourceScanDispatchPeriod.TotalSeconds));
        this.ResourceScanPeriodForTries = TimeSpan.FromSeconds((double) reader.GetOptionalInt32("ResourceScanPeriodForTriesSec", (int) this.ResourceScanPeriodForTries.TotalSeconds));
        this.ResourceScanPeriodForTriesShort = TimeSpan.FromSeconds((double) reader.GetOptionalInt32("ResourceScanPeriodForTriesShortSec", (int) this.ResourceScanPeriodForTriesShort.TotalSeconds));
        this.ResourceScanManagerLifeTime = TimeSpan.FromSeconds((double) reader.GetOptionalInt32("ResourceScanManagerLifeTimeSec", (int) this.ResourceScanManagerLifeTime.TotalSeconds));
        this.ResourceScanSoftTtl = TimeSpan.FromSeconds((double) reader.GetOptionalInt32(nameof (ResourceScanSoftTtl), (int) this.ResourceScanSoftTtl.TotalSeconds));
        this.ResourceScanMaxCountToUseSeparateHandles = reader.GetOptionalInt32(nameof (ResourceScanMaxCountToUseSeparateHandles), this.ResourceScanMaxCountToUseSeparateHandles);
        this.CloudMaintenanceModeMessage = reader.GetOptionalString(nameof (CloudMaintenanceModeMessage), this.CloudMaintenanceModeMessage);
        this.SshMaxResponseTimeoutMin = TimeSpan.FromMinutes((double) reader.GetOptionalInt32(nameof (SshMaxResponseTimeoutMin), this.SshMaxResponseTimeoutMin.Minutes));
        this.SshWindowSize = reader.GetOptionalInt32(nameof (SshWindowSize), this.SshWindowSize);
        this.MaxViCloudApplianceNetworksNumber = reader.GetOptionalInt32("VMCCRMaxNetworkCount", this.MaxViCloudApplianceNetworksNumber);
        this.MaxHvCloudApplianceNetworksNumber = reader.GetOptionalInt32("HVCCRMaxNetworkCount", this.MaxHvCloudApplianceNetworksNumber);
        this.UseAdvancedMetaGenerationAlg = reader.GetOptionalBoolean(nameof (UseAdvancedMetaGenerationAlg), this.UseAdvancedMetaGenerationAlg);
        this.SkipSavingSharePointInfoToVbm = reader.GetOptionalBoolean(nameof (SkipSavingSharePointInfoToVbm), this.SkipSavingSharePointInfoToVbm);
        this.VcdTemplatesSupport = reader.GetOptionalBoolean(nameof (VcdTemplatesSupport), this.VcdTemplatesSupport);
        this.IsZeroViVlanAllowed = reader.GetOptionalBoolean(nameof (IsZeroViVlanAllowed), this.IsZeroViVlanAllowed);
        this.DisableFullBlockRead = (COptions.EIrDisableFullBlockRead) reader.GetOptionalInt32(nameof (DisableFullBlockRead), (int) this.DisableFullBlockRead);
        this.FsAwareItemToDeleteMinSize = reader.GetOptionalInt32(nameof (FsAwareItemToDeleteMinSize), this.FsAwareItemToDeleteMinSize);
        this.CloudReplicaNoStaticIpSDetectedWarning = reader.GetOptionalBoolean(nameof (CloudReplicaNoStaticIpSDetectedWarning), this.CloudReplicaNoStaticIpSDetectedWarning);
        this.CloudEnableReplicaReIP = reader.GetOptionalBoolean(nameof (CloudEnableReplicaReIP), this.CloudEnableReplicaReIP);
        this.FilterVirtualNetworkCardsReIP = reader.GetOptionalBoolean(nameof (FilterVirtualNetworkCardsReIP), this.FilterVirtualNetworkCardsReIP);
        this.UseAsyncJobSizeCalculation = reader.GetOptionalBoolean(nameof (UseAsyncJobSizeCalculation), this.UseAsyncJobSizeCalculation);
        this.SyncRestoreJobSizeCalculationTimeout = TimeSpan.FromSeconds((double) reader.GetOptionalInt32(nameof (SyncRestoreJobSizeCalculationTimeout), (int) this.SyncRestoreJobSizeCalculationTimeout.TotalSeconds));
        this.DisableVMwareToolsNotFoundWarning = reader.GetOptionalBoolean(nameof (DisableVMwareToolsNotFoundWarning), this.DisableVMwareToolsNotFoundWarning);
        this.ClientEnableUpdate = reader.GetOptionalBoolean(nameof (ClientEnableUpdate), this.ClientEnableUpdate);
        this.ServerEnableUpdate = reader.GetOptionalBoolean(nameof (ServerEnableUpdate), this.ServerEnableUpdate);
        this.ThrowOnUnknownUpdateFile = reader.GetOptionalBoolean(nameof (ThrowOnUnknownUpdateFile), this.ThrowOnUnknownUpdateFile);
        this.ClientVersionCompatibility = reader.GetOptionalInt32(nameof (ClientVersionCompatibility), this.ClientVersionCompatibility);
        this.UpdateDownloadDelay = reader.GetOptionalInt32(nameof (UpdateDownloadDelay), this.UpdateDownloadDelay);
        this.UpdateDownloadRetryCount = reader.GetOptionalInt32(nameof (UpdateDownloadRetryCount), this.UpdateDownloadRetryCount);
        this.UpdateDownloadBlockSize = reader.GetOptionalInt32(nameof (UpdateDownloadBlockSize), this.UpdateDownloadBlockSize);
        this.EnableDownloadFailureSimulation = reader.GetOptionalBoolean(nameof (EnableDownloadFailureSimulation), this.EnableDownloadFailureSimulation);
        this.SyncTimeInterval = reader.GetOptionalInt32(nameof (SyncTimeInterval), this.SyncTimeInterval);
        this.EnableHvParallelTaskBuilding = reader.GetOptionalBoolean(nameof (EnableHvParallelTaskBuilding), this.EnableHvParallelTaskBuilding);
        this.HvTaskBuilderThreadCount = reader.GetOptionalInt32(nameof (HvTaskBuilderThreadCount), this.HvTaskBuilderThreadCount);
        this.SobrForceExtentSpaceUpdate = reader.GetOptionalBoolean(nameof (SobrForceExtentSpaceUpdate), this.SobrForceExtentSpaceUpdate);
        this.SOBRFullCompressRate = reader.GetOptionalInt32(nameof (SOBRFullCompressRate), this.SOBRFullCompressRate);
        this.SOBRIncrementCompressRate = reader.GetOptionalInt32(nameof (SOBRIncrementCompressRate), this.SOBRIncrementCompressRate);
        this.SOBRTransformRate = reader.GetOptionalInt32(nameof (SOBRTransformRate), this.SOBRTransformRate);
        this.SOBRSyntheticFullCompressRate = reader.GetOptionalInt32(nameof (SOBRSyntheticFullCompressRate), this.SOBRSyntheticFullCompressRate);
        this.EnableImportMetaCache = reader.GetOptionalBoolean(nameof (EnableImportMetaCache), this.EnableImportMetaCache);
        this.CheckDbLocks = reader.GetOptionalBoolean(nameof (CheckDbLocks), this.CheckDbLocks);
        this.UseFastLockServiceSchema = reader.GetOptionalBoolean(nameof (UseFastLockServiceSchema), this.UseFastLockServiceSchema);
        this.LockStoragesRetryCount = reader.GetOptionalInt32(nameof (LockStoragesRetryCount), this.LockStoragesRetryCount);
        this.Dev_ThrowCloudStorageLockWarnings = reader.GetOptionalBoolean(nameof (Dev_ThrowCloudStorageLockWarnings), this.Dev_ThrowCloudStorageLockWarnings);
        this.BackupCopyEnabledCheckGFSSchedule = reader.GetOptionalBoolean("EnabledCheckGFSSchedule", true);
        this.InfrastructureCacheExpirationSec = reader.GetOptionalInt32(nameof (InfrastructureCacheExpirationSec), this.InfrastructureCacheExpirationSec);
        this.UseVbrCredsForRemoteMount = reader.GetOptionalBoolean(nameof (UseVbrCredsForRemoteMount), this.UseVbrCredsForRemoteMount);
        this.CheckLinuxKernelVersion = reader.GetOptionalBoolean(nameof (CheckLinuxKernelVersion), this.CheckLinuxKernelVersion);
        this.StrictDatastoreScope = reader.GetOptionalBoolean(nameof (StrictDatastoreScope), this.StrictDatastoreScope);
        this.UseUnbufferedAccess = reader.GetOptionalBoolean(nameof (UseUnbufferedAccess), this.UseUnbufferedAccess);
        this.AzureDiskProcessingThreadCount = reader.GetOptionalInt32(nameof (AzureDiskProcessingThreadCount), this.AzureDiskProcessingThreadCount);
        this.AzureDiskProcessingBlockSize = reader.GetOptionalInt32(nameof (AzureDiskProcessingBlockSize), this.AzureDiskProcessingBlockSize);
        this.EnableAzureSystemDiskConversion = reader.GetOptionalBoolean(nameof (EnableAzureSystemDiskConversion), this.EnableAzureSystemDiskConversion);
        this.AzureRestoreForceAlternativeEFItoBIOSConversionMethod = reader.GetOptionalBoolean(nameof (AzureRestoreForceAlternativeEFItoBIOSConversionMethod), this.AzureRestoreForceAlternativeEFItoBIOSConversionMethod);
        this.AzureUpdateWindowsConnectivityParams = reader.GetOptionalBoolean(nameof (AzureUpdateWindowsConnectivityParams), this.AzureUpdateWindowsConnectivityParams);
        this.AzureApplianceSSHPort = reader.GetOptionalInt32(nameof (AzureApplianceSSHPort), this.AzureApplianceSSHPort);
        this.AzureApplianceVmSize = reader.GetOptionalString(nameof (AzureApplianceVmSize), this.AzureApplianceVmSize);
        this.AzureApplianceRmVmSize = reader.GetOptionalString(nameof (AzureApplianceRmVmSize), this.AzureApplianceRmVmSize);
        this.AzureApplianceRmVmPremiumSizeDefault = reader.GetOptionalString(nameof (AzureApplianceRmVmPremiumSizeDefault), this.AzureApplianceRmVmPremiumSizeDefault);
        this.AzureApplianceRmVmPremiumSizes = reader.GetOptionalMultiString(nameof (AzureApplianceRmVmPremiumSizes), this.AzureApplianceRmVmPremiumSizes);
        this.AzureDefaultContainerName = reader.GetOptionalString(nameof (AzureDefaultContainerName), this.AzureDefaultContainerName);
        this.AzureRestApiRetryCount = reader.GetOptionalInt32(nameof (AzureRestApiRetryCount), this.AzureRestApiRetryCount);
        this.AzureRestApiRetryTimeoutSec = reader.GetOptionalInt32(nameof (AzureRestApiRetryTimeoutSec), this.AzureRestApiRetryTimeoutSec);
        this.AzureUpdateStatusRetryCount = reader.GetOptionalInt32(nameof (AzureUpdateStatusRetryCount), this.AzureUpdateStatusRetryCount);
        this.AzureAgnetDownloadLink = reader.GetOptionalString(nameof (AzureAgnetDownloadLink), this.AzureAgnetDownloadLink);
        this.AzureMpImageReference = reader.GetOptionalString(nameof (AzureMpImageReference), this.AzureMpImageReference);
        this.AzureASMImageName = reader.GetOptionalString(nameof (AzureASMImageName), this.AzureASMImageName);
        this.EnableCloudRepositoryQuotersLogs = reader.GetOptionalBoolean(nameof (EnableCloudRepositoryQuotersLogs), this.EnableCloudRepositoryQuotersLogs);
        this.Dev_DisableCloudLicensingChecks = reader.GetOptionalBoolean(nameof (Dev_DisableCloudLicensingChecks), this.Dev_DisableCloudLicensingChecks);
        this.NetServerEnumIntervalSec = reader.GetOptionalInt32(nameof (NetServerEnumIntervalSec), this.NetServerEnumIntervalSec);
        this.UseNewMetaFormat = reader.GetOptionalBoolean(nameof (UseNewMetaFormat), this.UseNewMetaFormat);
        this.MetaGenerationTimeout = reader.GetOptionalInt32(nameof (MetaGenerationTimeout), this.MetaGenerationTimeout);
        this.IsVmfsSanAsyncRwEnabled = reader.GetOptionalBoolean(nameof (IsVmfsSanAsyncRwEnabled), this.IsVmfsSanAsyncRwEnabled);
        this.IsHotAddAsyncRwEnabled = reader.GetOptionalBoolean(nameof (IsHotAddAsyncRwEnabled), this.IsHotAddAsyncRwEnabled);
        this.VexIgnoreCertificateErrors = reader.GetOptionalBoolean(nameof (VexIgnoreCertificateErrors), this.VexIgnoreCertificateErrors);
        this.VexUseSsl = reader.GetOptionalBoolean(nameof (VexUseSsl), this.VexUseSsl);
        this.GenerateBiosUuidOnResolveFail = reader.GetOptionalBoolean(nameof (GenerateBiosUuidOnResolveFail), this.GenerateBiosUuidOnResolveFail);
        this.SkipSqlConnectionCheck = reader.GetOptionalBoolean(nameof (SkipSqlConnectionCheck), this.SkipSqlConnectionCheck);
        this.BrokerServicePort = reader.GetOptionalInt32(nameof (BrokerServicePort), this.BrokerServicePort);
        this.HyperVRCTVerboseLogging = reader.GetOptionalBoolean(nameof (HyperVRCTVerboseLogging), this.HyperVRCTVerboseLogging);
        this.HierarchyReloadIntervalsSec = reader.GetOptionalInt32(nameof (HierarchyReloadIntervalsSec), this.HierarchyReloadIntervalsSec);
        this.UseFastHvVmInfoCollection = reader.GetOptionalBoolean(nameof (UseFastHvVmInfoCollection), this.UseFastHvVmInfoCollection);
        this.IsAdjustDueDateEnabled = reader.GetOptionalBoolean(nameof (IsAdjustDueDateEnabled), false);
        this.EpIrDefaultCpuCount = reader.GetOptionalInt32(nameof (EpIrDefaultCpuCount), this.EpIrDefaultCpuCount);
        this.EpIrDefaultMemory = reader.GetOptionalInt32(nameof (EpIrDefaultMemory), this.EpIrDefaultMemory);
        this.SnmpTrapStrMaxLength = reader.GetOptionalInt32(nameof (SnmpTrapStrMaxLength), this.SnmpTrapStrMaxLength);
        this.HvSharedSnapshotGroupByVolume = reader.GetOptionalBoolean(nameof (HvSharedSnapshotGroupByVolume), this.HvSharedSnapshotGroupByVolume);
        this.CloudConnectDbCacheResyncIntervalSec = reader.GetOptionalInt32(nameof (CloudConnectDbCacheResyncIntervalSec), this.CloudConnectDbCacheResyncIntervalSec);
        this.IsAllowedToCacheOibsInfoInVmsPanel = reader.GetOptionalBoolean(nameof (IsAllowedToCacheOibsInfoInVmsPanel), this.IsAllowedToCacheOibsInfoInVmsPanel);
        this.UseProxyAffinityLogic = reader.GetOptionalBoolean(nameof (UseProxyAffinityLogic), this.UseProxyAffinityLogic);
        this.UseGlobalExclusions = reader.GetOptionalBoolean(nameof (UseGlobalExclusions), false);
        this.DenyOldProviders = reader.GetOptionalBoolean(nameof (DenyOldProviders), true);
        this.EnableLogStartStopReport = reader.GetOptionalBoolean(nameof (EnableLogStartStopReport), false);
        this.LogViSoapOperations = reader.GetOptionalBoolean(nameof (LogViSoapOperations), this.LogViSoapOperations);
        this.UseIncrementalHierarchyLoading = reader.GetOptionalBoolean(nameof (UseIncrementalHierarchyLoading), this.UseIncrementalHierarchyLoading);
        this.UseAsyncPSInvoke = reader.GetOptionalBoolean(nameof (UseAsyncPSInvoke), this.UseAsyncPSInvoke);
        this.SoftFsOperationInvalidFsCharsFilter = reader.GetOptionalBoolean(nameof (SoftFsOperationInvalidFsCharsFilter), this.SoftFsOperationInvalidFsCharsFilter);
        this.IscsiMountFsCheckRetriesCount = reader.GetOptionalInt32(nameof (IscsiMountFsCheckRetriesCount), this.IscsiMountFsCheckRetriesCount);
        this.SoftFsOperationInvalidFsCharsFilter = reader.GetOptionalBoolean(nameof (SoftFsOperationInvalidFsCharsFilter), this.SoftFsOperationInvalidFsCharsFilter);
        this.IscsiMountFsCheckRetriesCount = reader.GetOptionalInt32(nameof (IscsiMountFsCheckRetriesCount), this.IscsiMountFsCheckRetriesCount);
        this.HvCloudReplicaMemoryWarning = reader.GetOptionalBoolean(nameof (HvCloudReplicaMemoryWarning), this.HvCloudReplicaMemoryWarning);
        this.ShowNimbleCloneVolumes = reader.GetOptionalBoolean(nameof (ShowNimbleCloneVolumes), this.ShowNimbleCloneVolumes);
        this.UseNimbleIscsiLunId = reader.GetOptionalBoolean(nameof (UseNimbleIscsiLunId), this.UseNimbleIscsiLunId);
        this.DisableRTSTaskLogging = reader.GetOptionalBoolean(nameof (DisableRTSTaskLogging), this.DisableRTSTaskLogging);
        this.EnablePowerOnRetries = reader.GetOptionalBoolean(nameof (EnablePowerOnRetries), this.EnablePowerOnRetries);
        this.PowerOnRetryCount = reader.GetOptionalInt32(nameof (PowerOnRetryCount), this.PowerOnRetryCount);
        this.PowerOnRetryTimeout = reader.GetOptionalInt32(nameof (PowerOnRetryTimeout), this.PowerOnRetryTimeout);
        this.SkipAzureWinProxyVmDeploy = reader.GetOptionalBoolean(nameof (SkipAzureWinProxyVmDeploy), this.SkipAzureWinProxyVmDeploy);
        this.LogBrokerUpdates = reader.GetOptionalBoolean(nameof (LogBrokerUpdates), this.LogBrokerUpdates);
        this.MetaGeneratorPreallocSize = reader.GetOptionalInt32(nameof (MetaGeneratorPreallocSize), this.MetaGeneratorPreallocSize);
        this.EnableLargeMeta = reader.GetOptionalBoolean(nameof (EnableLargeMeta), this.EnableLargeMeta);
        this.DbCollectConnectionTimeoutSec = reader.GetOptionalInt32(nameof (DbCollectConnectionTimeoutSec), this.DbCollectConnectionTimeoutSec);
        this.DbClearDeletedBackupServerTimeoutSec = TimeSpan.FromSeconds((double) reader.GetOptionalInt32(nameof (DbClearDeletedBackupServerTimeoutSec), (int) this.DbClearDeletedBackupServerTimeoutSec.TotalSeconds));
        this.RetainGfsStoragesAfterTransform = reader.GetOptionalBoolean(nameof (RetainGfsStoragesAfterTransform), this.RetainGfsStoragesAfterTransform);
        this.HierarchyRetryNumber = reader.GetOptionalInt32(nameof (HierarchyRetryNumber), this.HierarchyRetryNumber);
        this.HyperVGuestVSSMonitor = reader.GetOptionalBoolean(nameof (HyperVGuestVSSMonitor), this.HyperVGuestVSSMonitor);
        this.AwsServiceUrl = reader.GetOptionalString(nameof (AwsServiceUrl), this.AwsServiceUrl).TrimEnd('/');
        this.AwsS3ServiceUrl = reader.GetOptionalString(nameof (AwsS3ServiceUrl), this.AwsS3ServiceUrl).TrimEnd('/');
        this.AwsPricingUrl = reader.GetOptionalString(nameof (AwsPricingUrl), this.AwsPricingUrl).TrimEnd('/');
        this.AwsCheckIpUrl = reader.GetOptionalString(nameof (AwsCheckIpUrl), this.AwsCheckIpUrl).TrimEnd('/');
        this.AwsPricingExpiration = TimeSpan.FromHours((double) reader.GetOptionalInt32("AwsPricingExpirationHours", (int) this.AwsPricingExpiration.TotalHours));
        this.AwsCreatorStartInstanceTimeout = TimeSpan.FromMinutes((double) reader.GetOptionalInt32("AwsCreatorStartInstanceTimeoutMin", (int) this.AwsCreatorStartInstanceTimeout.TotalMinutes));
        this.AwsCreatorStopInstanceTimeout = TimeSpan.FromMinutes((double) reader.GetOptionalInt32("AwsCreatorStopInstanceTimeoutMin", (int) this.AwsCreatorStopInstanceTimeout.TotalMinutes));
        this.AwsAppliancePoolCheckTimeout = TimeSpan.FromMinutes((double) reader.GetOptionalInt32("AwsAppliancePoolCheckTimeoutMin", (int) this.AwsAppliancePoolCheckTimeout.TotalMinutes));
        this.AwsVolumeAttachTimeout = TimeSpan.FromSeconds((double) reader.GetOptionalInt32("AwsVolumeAttachTimeoutSec", (int) this.AwsVolumeAttachTimeout.TotalSeconds));
        this.AwsSnapshotCreationTimeout = TimeSpan.FromMinutes((double) reader.GetOptionalInt32("AwsSnapshotCreationTimeoutMin", (int) this.AwsSnapshotCreationTimeout.TotalMinutes));
        this.AwsImportOnlyRoot = reader.GetOptionalBoolean(nameof (AwsImportOnlyRoot), this.AwsImportOnlyRoot);
        this.AwsApplianceElevateToRoot = reader.GetOptionalBoolean(nameof (AwsApplianceElevateToRoot), this.AwsApplianceElevateToRoot);
        this.AwsSearchAmiMode = reader.GetOptionalInt32(nameof (AwsSearchAmiMode), this.AwsSearchAmiMode);
        this.AwsSearchAmiOnEbsOnly = reader.GetOptionalBoolean(nameof (AwsSearchAmiOnEbsOnly), this.AwsSearchAmiOnEbsOnly);
        this.AwsDefaultSsdIops = reader.GetOptionalInt32(nameof (AwsDefaultSsdIops), this.AwsDefaultSsdIops);
        this.AwsMaxIopsRatio = reader.GetOptionalInt32(nameof (AwsMaxIopsRatio), this.AwsMaxIopsRatio);
        this.SshConnectionRetryCount = reader.GetOptionalInt32(nameof (SshConnectionRetryCount), this.SshConnectionRetryCount);
        this.SshConnectionRetryTimeout = TimeSpan.FromSeconds((double) reader.GetOptionalInt32("SshConnectionRetryTimeoutSec", (int) this.SshConnectionRetryTimeout.TotalSeconds));
        this.SshAgentStartTimeout = TimeSpan.FromSeconds((double) reader.GetOptionalInt32("SshAgentStartTimeoutSec", (int) this.SshAgentStartTimeout.TotalSeconds));
        this.AwsDetermineOsByMount = reader.GetOptionalBoolean(nameof (AwsDetermineOsByMount), this.AwsDetermineOsByMount);
        this.AwsNativeOibDetermineMode = reader.GetOptionalInt32(nameof (AwsNativeOibDetermineMode), this.AwsNativeOibDetermineMode);
        this.AwsApiRetryCount = reader.GetOptionalInt32(nameof (AwsApiRetryCount), this.AwsApiRetryCount);
        this.AwsApiRetryTimeout = TimeSpan.FromSeconds((double) reader.GetOptionalInt32("AwsApiRetryTimeoutSec", (int) this.AwsApiRetryTimeout.TotalSeconds));
        this.AwsLeaveAppliance = reader.GetOptionalBoolean(nameof (AwsLeaveAppliance), this.AwsLeaveAppliance);
        this.AwsLeaveS3Bucket = reader.GetOptionalBoolean(nameof (AwsLeaveS3Bucket), this.AwsLeaveS3Bucket);
        this.AmazonDefaultLinuxImage = reader.GetOptionalMultiString(nameof (AmazonDefaultLinuxImage), this.AmazonDefaultLinuxImage);
        this.AmazonDefaultWindowsImage = reader.GetOptionalMultiString(nameof (AmazonDefaultWindowsImage), this.AmazonDefaultWindowsImage);
        this.AmazonDefaultProxyImage = reader.GetOptionalMultiString(nameof (AmazonDefaultProxyImage), this.AmazonDefaultProxyImage);
        this.UseScsiUniqeIdComparisonForLunsByDefault = reader.GetOptionalBoolean(nameof (UseScsiUniqeIdComparisonForLunsByDefault), this.UseScsiUniqeIdComparisonForLunsByDefault);
        this.DisableTrafficControl = reader.GetOptionalBoolean(nameof (DisableTrafficControl), this.DisableTrafficControl);
        this.ShowNewVmsColumn = reader.GetOptionalBoolean(nameof (ShowNewVmsColumn), this.ShowNewVmsColumn);
        this.IsViApi65Supported = reader.GetOptionalBoolean(nameof (IsViApi65Supported), this.IsViApi65Supported);
        this.HP3ParRetryCommandCount = reader.GetOptionalInt32(nameof (HP3ParRetryCommandCount), this.HP3ParRetryCommandCount);
        this.HP3ParRetryCommandTimeOut = reader.GetOptionalInt32(nameof (HP3ParRetryCommandTimeOut), this.HP3ParRetryCommandTimeOut);
        this.AzureApplianceSshConnectRetries = reader.GetOptionalInt32(nameof (AzureApplianceSshConnectRetries), this.AzureApplianceSshConnectRetries);
        this.BackupQuotaRepoCompatibilityChecks = reader.GetOptionalBoolean(nameof (BackupQuotaRepoCompatibilityChecks), this.BackupQuotaRepoCompatibilityChecks);
        this.UseKerberosAuthenticationStrategy = reader.GetOptionalBoolean(nameof (UseKerberosAuthenticationStrategy), false);
        this.Hp3ParFixEnabled = reader.GetOptionalBoolean(nameof (Hp3ParFixEnabled), this.Hp3ParFixEnabled);
        this.EnableAsyncMountPresent = reader.KeyExists(nameof (EnableAsyncMount));
        this.EnableAsyncMount = reader.GetOptionalBoolean(nameof (EnableAsyncMount), this.EnableAsyncMount);
        this.VMwareForcedHierarchyUpdatePeriod = reader.GetOptionalInt32(nameof (VMwareForcedHierarchyUpdatePeriod), this.VMwareForcedHierarchyUpdatePeriod);
        this.VMwareOverrideApiVersion = reader.GetOptionalMultiString(nameof (VMwareOverrideApiVersion), this.VMwareOverrideApiVersion);
        this.MaxConcurrentComponentUpgrades = reader.GetOptionalInt32(nameof (MaxConcurrentComponentUpgrades), this.MaxConcurrentComponentUpgrades);
        this.VMBPRemoteShellEnableSP = reader.GetOptionalBoolean(nameof (VMBPRemoteShellEnableSP), this.VMBPRemoteShellEnableSP);
        this.VMBPRemoteShellEnableTenant = reader.GetOptionalBoolean(nameof (VMBPRemoteShellEnableTenant), this.VMBPRemoteShellEnableTenant);
        this.VMBPRemoteShellGateRulesCheckingPeriodSeconds = reader.GetOptionalInt32(nameof (VMBPRemoteShellGateRulesCheckingPeriodSeconds), this.VMBPRemoteShellGateRulesCheckingPeriodSeconds);
        this.VMBPRemoteShellNetworkRedirectorRunningCheckingPeriodSeconds = reader.GetOptionalInt32(nameof (VMBPRemoteShellNetworkRedirectorRunningCheckingPeriodSeconds), this.VMBPRemoteShellNetworkRedirectorRunningCheckingPeriodSeconds);
        this.VMBPRemoteShellUpdateRulesPeriodSeconds = reader.GetOptionalInt32(nameof (VMBPRemoteShellUpdateRulesPeriodSeconds), this.VMBPRemoteShellUpdateRulesPeriodSeconds);
        this.VMBPCloudNetworkRedirectorPortForTenants = reader.GetOptionalInt32(nameof (VMBPCloudNetworkRedirectorPortForTenants), this.VMBPCloudNetworkRedirectorPortForTenants);
        this.VMBPCloudNetworkRedirectorPortForShells = reader.GetOptionalInt32(nameof (VMBPCloudNetworkRedirectorPortForShells), this.VMBPCloudNetworkRedirectorPortForShells);
        this.VMBPRobocopShellHookRedirectionEnabled = reader.GetOptionalBoolean(nameof (VMBPRobocopShellHookRedirectionEnabled), this.VMBPRobocopShellHookRedirectionEnabled);
        this.VMBPShellAndRdpNetworkRedirectorPortRangeStart = reader.GetOptionalInt32(nameof (VMBPShellAndRdpNetworkRedirectorPortRangeStart), this.VMBPShellAndRdpNetworkRedirectorPortRangeStart);
        this.VMBPShellAndRdpNetworkRedirectorPortRangeEnd = reader.GetOptionalInt32(nameof (VMBPShellAndRdpNetworkRedirectorPortRangeEnd), this.VMBPShellAndRdpNetworkRedirectorPortRangeEnd);
        this.VMBPShellRdpTemplateFilename = reader.GetOptionalString(nameof (VMBPShellRdpTemplateFilename), this.VMBPShellRdpTemplateFilename);
        this.VMBPRDPDisconnectTimeoutSecondsAfterConsoleClose = reader.GetOptionalInt32(nameof (VMBPRDPDisconnectTimeoutSecondsAfterConsoleClose), this.VMBPRDPDisconnectTimeoutSecondsAfterConsoleClose);
        this.HvCollectAllHostAddresses = reader.GetOptionalBoolean(nameof (HvCollectAllHostAddresses), this.HvCollectAllHostAddresses);
        this.LauncherIgnoreCertificateErrors = reader.GetOptionalBoolean(nameof (LauncherIgnoreCertificateErrors), this.LauncherIgnoreCertificateErrors);
        this.SkipLocalAgentConnectionCheck = reader.GetOptionalBoolean(nameof (SkipLocalAgentConnectionCheck), this.SkipLocalAgentConnectionCheck);
        this.VboPort = (ushort) (reader.GetOptionalInt32(nameof (VboPort), (int) this.VboPort) & (int) ushort.MaxValue);
        this.ObsoleteGetMountPoints = reader.GetOptionalBoolean(nameof (ObsoleteGetMountPoints), this.ObsoleteGetMountPoints);
        this.DefaultVeeamZipPath = reader.GetOptionalString(nameof (DefaultVeeamZipPath), this.DefaultVeeamZipPath);
        this.HideUpgradeWizard = reader.GetOptionalBoolean(nameof (HideUpgradeWizard), this.HideUpgradeWizard);
        this.OracleCollectAllPaths = reader.GetOptionalBoolean(nameof (OracleCollectAllPaths), this.OracleCollectAllPaths);
        this.LegacyHashAlgorithm = reader.GetOptionalBoolean(nameof (LegacyHashAlgorithm), this.LegacyHashAlgorithm);
        this.CloudConnectionProcessingThreads = reader.GetOptionalInt32(nameof (CloudConnectionProcessingThreads), this.CloudConnectionProcessingThreads);
        this.CloudConnectUseAsyncInvoke = reader.GetOptionalBoolean("CCUseAsyncInvoke", this.CloudConnectUseAsyncInvoke);
        this.DisableProxyClientLogging = reader.GetOptionalBoolean(nameof (DisableProxyClientLogging), this.DisableProxyClientLogging);
        this.SeparateProxyClientLogging = reader.GetOptionalBoolean(nameof (SeparateProxyClientLogging), this.SeparateProxyClientLogging);
        this.CloudConnectQuantSizeMb = reader.GetOptionalInt32(nameof (CloudConnectQuantSizeMb), this.CloudConnectQuantSizeMb);
        this.VcdBackupQuantSizeMb = reader.GetOptionalInt32(nameof (VcdBackupQuantSizeMb), this.VcdBackupQuantSizeMb);
        this._azureAlwaysVisibleLocations.AddRange((IEnumerable<string>) reader.GetOptionalMultiString(nameof (AzureAlwaysVisibleLocations), new string[0]));
        this.SkipLinuxAgentRemoval = reader.GetOptionalBoolean(nameof (SkipLinuxAgentRemoval), this.SkipLinuxAgentRemoval);
        this.AzureMaxDiskSizeGB = reader.GetOptionalInt32(nameof (AzureMaxDiskSizeGB), this.AzureMaxDiskSizeGB);
        this.AzureStackMaxDiskSizeGB = reader.GetOptionalInt32(nameof (AzureStackMaxDiskSizeGB), this.AzureStackMaxDiskSizeGB);
        this.CloudConnectEnableLowLatencyMode = reader.GetOptionalBoolean(nameof (CloudConnectEnableLowLatencyMode), this.CloudConnectEnableLowLatencyMode);
        this._azureEnvironmentVariables.AddRange((IEnumerable<string>) reader.GetOptionalMultiString(nameof (AzureEnvironmentVariables), new string[0]));
        this.AzureEmulatorBlobEndpoint = reader.GetOptionalString(nameof (AzureEmulatorBlobEndpoint), this.AzureEmulatorBlobEndpoint);
        this.AzureDefaultTenantIdGUID = reader.GetOptionalString(nameof (AzureDefaultTenantIdGUID), this.AzureDefaultTenantIdGUID);
        this.AzureManualResourceNameCheckAllowed = reader.GetOptionalBoolean(nameof (AzureManualResourceNameCheckAllowed), this.AzureManualResourceNameCheckAllowed);
        this.PublicCloudEnableEmulators = reader.GetOptionalBoolean(nameof (PublicCloudEnableEmulators), false);
        this.AgentDiscoveryThreads = reader.GetOptionalInt32(nameof (AgentDiscoveryThreads), this.AgentDiscoveryThreads);
        this.AgentConcurrentDiscovery = reader.GetOptionalInt32(nameof (AgentConcurrentDiscovery), this.AgentConcurrentDiscovery);
        this.CloudIntrastructureReachingCapacityThresholdMinutes = TimeSpan.FromMinutes((double) reader.GetOptionalInt32(nameof (CloudIntrastructureReachingCapacityThresholdMinutes), (int) this.CloudIntrastructureReachingCapacityThresholdMinutes.TotalMinutes));
        this.CloudIntrastructureOutOfCapacityThresholdMinutes = TimeSpan.FromMinutes((double) reader.GetOptionalInt32(nameof (CloudIntrastructureOutOfCapacityThresholdMinutes), (int) this.CloudIntrastructureOutOfCapacityThresholdMinutes.TotalMinutes));
        this.DebugBreakpointId = reader.GetOptionalInt32(nameof (DebugBreakpointId), -1);
        this.ThrottleAgentBackup = reader.GetOptionalBoolean(nameof (ThrottleAgentBackup), this.ThrottleAgentBackup);
        this.ForceNicInfoLoadingForVm = reader.GetOptionalBoolean(nameof (ForceNicInfoLoadingForVm), this.ForceNicInfoLoadingForVm);
        this.VMDepartureEvent = (COptions.EVmDepatureSeverityType) reader.GetOptionalInt32(nameof (VMDepartureEvent), (int) this.VMDepartureEvent);
        this.HvMissingVMSeverity = (COptions.EVmDepatureSeverityType) reader.GetOptionalInt32(nameof (HvMissingVMSeverity), (int) this.HvMissingVMSeverity);
        this.SkipCloudHandshake = reader.GetOptionalString(nameof (SkipCloudHandshake), this.SkipCloudHandshake);
        this.SkipVawAndValRedistsDeploy = reader.GetOptionalBoolean(nameof (SkipVawAndValRedistsDeploy), this.SkipVawAndValRedistsDeploy);
        this.AgentServiceStartTimeoutSec = reader.GetOptionalInt32(nameof (AgentServiceStartTimeoutSec), this.AgentServiceStartTimeoutSec);
        this.EpDiscoveryEmailReportTimeOfDateHours = reader.GetOptionalInt32(nameof (EpDiscoveryEmailReportTimeOfDateHours), this.EpDiscoveryEmailReportTimeOfDateHours);
        this.EpDiscoveryEmailReportTimeOfDateMinutes = reader.GetOptionalInt32(nameof (EpDiscoveryEmailReportTimeOfDateMinutes), this.EpDiscoveryEmailReportTimeOfDateMinutes);
        this.EpDiscoveryEmailReportIntervalHours = reader.GetOptionalInt32(nameof (EpDiscoveryEmailReportIntervalHours), this.EpDiscoveryEmailReportIntervalHours);
        this.EpDiscoveryEmailReportSendThresholdMinutes = reader.GetOptionalInt32(nameof (EpDiscoveryEmailReportSendThresholdMinutes), this.EpDiscoveryEmailReportSendThresholdMinutes);
        this.EpDiscoveryEmailDispatchIntervalMinutes = reader.GetOptionalInt32(nameof (EpDiscoveryEmailDispatchIntervalMinutes), this.EpDiscoveryEmailDispatchIntervalMinutes);
        this.SOBREmailReportTimeOfDayHours = reader.GetOptionalInt32(nameof (SOBREmailReportTimeOfDayHours), this.SOBREmailReportTimeOfDayHours);
        this.SOBREmailReportTimeOfDayMinutes = reader.GetOptionalInt32(nameof (SOBREmailReportTimeOfDayMinutes), this.SOBREmailReportTimeOfDayMinutes);
        this.ArchiveBackupEmailReportIntervalHours = reader.GetOptionalInt32(nameof (ArchiveBackupEmailReportIntervalHours), this.ArchiveBackupEmailReportIntervalHours);
        this.ArchiveBackupEmailReportSendThresholdMinutes = reader.GetOptionalInt32(nameof (ArchiveBackupEmailReportSendThresholdMinutes), this.ArchiveBackupEmailReportSendThresholdMinutes);
        this.ArchiveBackupEmailReportDispatchIntervalMinutes = reader.GetOptionalInt32(nameof (ArchiveBackupEmailReportDispatchIntervalMinutes), this.ArchiveBackupEmailReportDispatchIntervalMinutes);
        this.EpPolicyEmailReportTimeOfDateHours = reader.GetOptionalInt32(nameof (EpPolicyEmailReportTimeOfDateHours), this.EpPolicyEmailReportTimeOfDateHours);
        this.EpPolicyEmailReportTimeOfDateMinutes = reader.GetOptionalInt32(nameof (EpPolicyEmailReportTimeOfDateMinutes), this.EpPolicyEmailReportTimeOfDateMinutes);
        this.EpPolicyEmailReportSendThresholdMinutes = reader.GetOptionalInt32(nameof (EpPolicyEmailReportSendThresholdMinutes), this.EpPolicyEmailReportSendThresholdMinutes);
        this.EpPolicyEmailDispatchIntervalMinutes = reader.GetOptionalInt32(nameof (EpPolicyEmailDispatchIntervalMinutes), this.EpPolicyEmailDispatchIntervalMinutes);
        this.AgentRescanUsingADShortNames = reader.GetOptionalBoolean(nameof (AgentRescanUsingADShortNames), this.AgentRescanUsingADShortNames);
        this.AgentDiscoveryIgnoreOwnership = reader.GetOptionalBoolean(nameof (AgentDiscoveryIgnoreOwnership), this.AgentDiscoveryIgnoreOwnership);
        this.AgentDisableDistributionServerFailover = reader.GetOptionalBoolean(nameof (AgentDisableDistributionServerFailover), this.AgentDisableDistributionServerFailover);
        this.HyperVIgnoreNonSnapshottableDisks = reader.GetOptionalBoolean(nameof (HyperVIgnoreNonSnapshottableDisks), this.HyperVIgnoreNonSnapshottableDisks);
        this.AgentBackupConcurrentTasks = reader.GetOptionalInt32(nameof (AgentBackupConcurrentTasks), this.AgentBackupConcurrentTasks);
        this.AgentPolicyConcurrentTasks = reader.GetOptionalInt32(nameof (AgentPolicyConcurrentTasks), this.AgentPolicyConcurrentTasks);
        this.AgentBackupKeepAliveTimeoutMin = reader.GetOptionalInt32(nameof (AgentBackupKeepAliveTimeoutMin), this.AgentBackupKeepAliveTimeoutMin);
        this.RemotingRetryCount = COptions._forceSpecifiedRemotingRetryCount ?? reader.GetOptionalInt32(nameof (RemotingRetryCount), this.RemotingRetryCount);
        this.RemotingEnablePerfLog = reader.GetOptionalBoolean(nameof (RemotingEnablePerfLog), this.RemotingEnablePerfLog);
        this.RemotingEnableLifeTimeLog = reader.GetOptionalBoolean(nameof (RemotingEnableLifeTimeLog), this.RemotingEnableLifeTimeLog);
        this.RemotingMaxParallelCallsNum = reader.GetOptionalInt32(nameof (RemotingMaxParallelCallsNum), this.RemotingMaxParallelCallsNum);
        this.RemotingSemaphoreWaitTimeoutMs = reader.GetOptionalInt32(nameof (RemotingSemaphoreWaitTimeoutMs), this.RemotingSemaphoreWaitTimeoutMs);
        this.UseCachedSshConnections = reader.GetOptionalBoolean(nameof (UseCachedSshConnections), this.UseCachedSshConnections);
        this.ForeignConcurrentExecutersCount = reader.GetOptionalInt32(nameof (ForeignConcurrentExecutersCount), this.ForeignConcurrentExecutersCount);
        this.ForeignExecuterRetryTimeoutSec = reader.GetOptionalInt32(nameof (ForeignExecuterRetryTimeoutSec), this.ForeignExecuterRetryTimeoutSec);
        this.LimitParallelHelper = reader.GetOptionalBoolean(nameof (LimitParallelHelper), this.LimitParallelHelper);
        this.AgentsDisableRECollectionWarning = reader.GetOptionalBoolean(nameof (AgentsDisableRECollectionWarning), this.AgentsDisableRECollectionWarning);
        this.AgentManagementJobStartGroupSize = reader.GetOptionalInt32(nameof (AgentManagementJobStartGroupSize), this.AgentManagementJobStartGroupSize);
        this.JobEventReadyToFinishTimeoutInSec = reader.GetOptionalInt32(nameof (JobEventReadyToFinishTimeoutInSec), this.JobEventReadyToFinishTimeoutInSec);
        this.JobEventReadyToWorkTimeoutInSec = reader.GetOptionalInt32(nameof (JobEventReadyToWorkTimeoutInSec), this.JobEventReadyToWorkTimeoutInSec);
        this.AsyncFlrTransfer = reader.GetOptionalBoolean(nameof (AsyncFlrTransfer), this.AsyncFlrTransfer);
        this.AsyncFlrTransferCompressed = reader.GetOptionalBoolean(nameof (AsyncFlrTransferCompressed), this.AsyncFlrTransferCompressed);
        this.AsyncFlrTransferThreads = reader.GetOptionalInt32(nameof (AsyncFlrTransferThreads), this.AsyncFlrTransferThreads);
        this.SearchMountLunByScisiUniqueId = reader.GetOptionalBoolean(nameof (SearchMountLunByScisiUniqueId), this.SearchMountLunByScisiUniqueId);
        this.IsAddToJobNotInitializedAgentAllowed = reader.GetOptionalBoolean(nameof (IsAddToJobNotInitializedAgentAllowed), this.IsAddToJobNotInitializedAgentAllowed);
        this.UseSingleAgentBackupManager = reader.GetOptionalBoolean(nameof (UseSingleAgentBackupManager), this.UseSingleAgentBackupManager);
        this.AmazonMaxSystemDiskSizeGB = reader.GetOptionalInt32(nameof (AmazonMaxSystemDiskSizeGB), this.AmazonMaxSystemDiskSizeGB);
        this.AmazonMaxDiskSizeGB = reader.GetOptionalInt32(nameof (AmazonMaxDiskSizeGB), this.AmazonMaxDiskSizeGB);
        this.VBRServiceRestartNeeded = reader.GetOptionalBoolean(nameof (VBRServiceRestartNeeded), this.VBRServiceRestartNeeded);
        this.VBRServiceRestartWaitTimeSec = reader.GetOptionalInt32(nameof (VBRServiceRestartWaitTimeSec), this.VBRServiceRestartWaitTimeSec);
        this.AgentManagementWindowsServiceTimoutSec = reader.GetOptionalInt32(nameof (AgentManagementWindowsServiceTimoutSec), this.AgentManagementWindowsServiceTimoutSec);
        this.EpDiskManagementServiceLeaseTtlSec = reader.GetOptionalInt32(nameof (EpDiskManagementServiceLeaseTtlSec), this.EpDiskManagementServiceLeaseTtlSec);
        this.AzureAllowWindowsServerLicenseTypeSelection = reader.GetOptionalBoolean(nameof (AzureAllowWindowsServerLicenseTypeSelection), this.AzureAllowWindowsServerLicenseTypeSelection);
        this.EpAgentManagementMaxSupportedVerion = reader.GetOptionalInt32("Dev_EpAgentManagementMaxSupportedVerion", this.EpAgentManagementMaxSupportedVerion);
        this.AgentDeletedMachineStartTimeHours = reader.GetOptionalInt32(nameof (AgentDeletedMachineStartTimeHours), this.AgentDeletedMachineStartTimeHours);
        this.AgentDeletedMachineStartTimeMinutes = reader.GetOptionalInt32(nameof (AgentDeletedMachineStartTimeMinutes), this.AgentDeletedMachineStartTimeMinutes);
        this.AgentDisableDeletedMachineRetention = reader.GetOptionalBoolean(nameof (AgentDisableDeletedMachineRetention), this.AgentDisableDeletedMachineRetention);
        this.SOBRArchivingScanPeriod = reader.GetOptionalInt32(nameof (SOBRArchivingScanPeriod), this.SOBRArchivingScanPeriod);
        this.ExternalRepositoryEnableAutoResync = reader.GetOptionalBoolean(nameof (ExternalRepositoryEnableAutoResync), this.ExternalRepositoryEnableAutoResync);
        this.ExternalRepositoryEnableMaintenance = reader.GetOptionalBoolean(nameof (ExternalRepositoryEnableMaintenance), this.ExternalRepositoryEnableMaintenance);
        string optionalString12 = reader.GetOptionalString(nameof (ExternalRepositoryResyncStartTime), string.Empty);
        TimeSpan result2;
        if (!string.IsNullOrWhiteSpace(optionalString12) && TimeSpan.TryParseExact(optionalString12, "hh\\:mm\\:ss", (IFormatProvider) CultureInfo.InvariantCulture, out result2))
          this.ExternalRepositoryResyncStartTime = result2;
        int optionalInt32_3 = reader.GetOptionalInt32(nameof (ExternalRepositoryMaintenanceStartTimeoutMinutes), this.ExternalRepositoryMaintenanceStartTimeoutMinutes);
        if (optionalInt32_3 >= 0 || optionalInt32_3 <= (int) TimeSpan.FromDays(1.0).TotalMinutes)
          this.ExternalRepositoryMaintenanceStartTimeoutMinutes = optionalInt32_3;
        this.AzurePreferPrivateIpAddressesForProxyandLinuxAppliance = reader.GetOptionalBoolean(nameof (AzurePreferPrivateIpAddressesForProxyandLinuxAppliance), this.AzurePreferPrivateIpAddressesForProxyandLinuxAppliance);
        this.AzureSkipCheckCanDeployVms = reader.GetOptionalBoolean(nameof (AzureSkipCheckCanDeployVms), this.AzureSkipCheckCanDeployVms);
        this.DebugArchRepoSpec = reader.GetOptionalString(nameof (DebugArchRepoSpec), this.DebugArchRepoSpec);
        this.ExternalRepositoryEnableBackupIncrementalResync = reader.GetOptionalBoolean(nameof (ExternalRepositoryEnableBackupIncrementalResync), this.ExternalRepositoryEnableBackupIncrementalResync);
        this.ExternalRepositoryWizardStartRescan = reader.GetOptionalBoolean(nameof (ExternalRepositoryWizardStartRescan), this.ExternalRepositoryWizardStartRescan);
        this.ExternalRepositoryWizardWaitRescan = reader.GetOptionalBoolean(nameof (ExternalRepositoryWizardWaitRescan), this.ExternalRepositoryWizardWaitRescan);
        this.EnableConfigurationMaintenance = reader.GetOptionalBoolean(nameof (EnableConfigurationMaintenance), this.EnableConfigurationMaintenance);
        this.HyperVEnableNewStableAlg = reader.GetOptionalBoolean(nameof (HyperVEnableNewStableAlg), this.HyperVEnableNewStableAlg);
        this.HyperVDisableNvramUpdate = reader.GetOptionalBoolean(nameof (HyperVDisableNvramUpdate), this.HyperVDisableNvramUpdate);
        this.ExternalRepositoryCacheWindowsRoot = reader.GetOptionalString(nameof (ExternalRepositoryCacheWindowsRoot), this.ExternalRepositoryCacheWindowsRoot);
        this.ExternalRepositoryCacheLinuxRoot = reader.GetOptionalString(nameof (ExternalRepositoryCacheLinuxRoot), this.ExternalRepositoryCacheLinuxRoot);
        this.MaxSkipInvocationTimeoutInSec = reader.GetOptionalInt32(nameof (MaxSkipInvocationTimeoutInSec), this.MaxSkipInvocationTimeoutInSec);
        this.ExternalRepositoryMaxCacheSizeGb = reader.GetOptionalInt32(nameof (ExternalRepositoryMaxCacheSizeGb), this.ExternalRepositoryMaxCacheSizeGb);
        this.ExternalRepositoryEnableCachePurge = reader.GetOptionalBoolean(nameof (ExternalRepositoryEnableCachePurge), this.ExternalRepositoryEnableCachePurge);
        this.ExternalRepositoryCachePurgeThreshold = reader.GetOptionalInt32(nameof (ExternalRepositoryCachePurgeThreshold), this.ExternalRepositoryCachePurgeThreshold);
        this.ArchiveTierAllowAzureBlob = reader.GetOptionalBoolean(nameof (ArchiveTierAllowAzureBlob), this.ArchiveTierAllowAzureBlob);
        this.DeleteBackupLogSlotsLimit = reader.GetOptionalInt32(nameof (DeleteBackupLogSlotsLimit), this.DeleteBackupLogSlotsLimit);
        this.AgentManagementScalabilityTestMode = reader.GetOptionalBoolean(nameof (AgentManagementScalabilityTestMode), this.AgentManagementScalabilityTestMode);
        this.DeleteBackupJobThreadCount = reader.GetOptionalInt32(nameof (DeleteBackupJobThreadCount), this.DeleteBackupJobThreadCount);
        this.AmazonRestoreDiskAttachTimeout = TimeSpan.FromMinutes((double) reader.GetOptionalInt32("AmazonRestoreDiskAttachTimeoutMin", (int) this.AmazonRestoreDiskAttachTimeout.TotalMinutes));
        this.ExternalRepositoryEnableAutoDecrypt = reader.GetOptionalBoolean(nameof (ExternalRepositoryEnableAutoDecrypt), this.ExternalRepositoryEnableAutoDecrypt);
        this.S3MultipartUploadMinParts = reader.GetOptionalInt32(nameof (S3MultipartUploadMinParts), this.S3MultipartUploadMinParts);
        this.S3RequestRetryTotalTimeoutSec = reader.GetOptionalInt32(nameof (S3RequestRetryTotalTimeoutSec), this.S3RequestRetryTotalTimeoutSec);
        this.S3ConcurrentTaskLimit = reader.GetOptionalInt32(nameof (S3ConcurrentTaskLimit), this.S3ConcurrentTaskLimit);
        this.AzureConcurrentTaskLimit = reader.GetOptionalInt32(nameof (AzureConcurrentTaskLimit), this.AzureConcurrentTaskLimit);
        this.PublicCloudEnableLocationCheck = reader.GetOptionalBoolean(nameof (PublicCloudEnableLocationCheck), this.PublicCloudEnableLocationCheck);
        this.SOBRArchiveS3DisableTLS = reader.GetOptionalBoolean(nameof (SOBRArchiveS3DisableTLS), false);
        this.SOBRMaxArchiveTasksPercent = reader.GetOptionalInt32(nameof (SOBRMaxArchiveTasksPercent), this.SOBRMaxArchiveTasksPercent);
        this.SOBRArchiveIndexCompactThresholdMB = reader.GetOptionalInt32(nameof (SOBRArchiveIndexCompactThresholdMB), this.SOBRArchiveIndexCompactThresholdMB);
        this.SOBRArchiveConsistencyDelayMin = reader.GetOptionalInt32(nameof (SOBRArchiveConsistencyDelayMin), this.SOBRArchiveConsistencyDelayMin);
        this.UseGroundStoragesInRestore = reader.GetOptionalBoolean(nameof (UseGroundStoragesInRestore), this.UseGroundStoragesInRestore);
        this.AgentManagementUseLastLogonTimeStamp = reader.GetOptionalBoolean(nameof (AgentManagementUseLastLogonTimeStamp), this.AgentManagementUseLastLogonTimeStamp);
        this.vPowerLegacyWriteCache = reader.GetOptionalBoolean(nameof (vPowerLegacyWriteCache), false);
        this.ArchiveJobWaitForRestoreJobTimeout = TimeSpan.FromMinutes((double) reader.GetOptionalInt32(nameof (ArchiveJobWaitForRestoreJobTimeout), (int) this.ArchiveJobWaitForRestoreJobTimeout.TotalMinutes));
        this.SkipCertificateCheck = reader.GetOptionalBoolean(nameof (SkipCertificateCheck), this.SkipCertificateCheck);
        this.SshTtyPromptExtraTimeoutMs = reader.GetOptionalInt32(nameof (SshTtyPromptExtraTimeoutMs), 0);
        this.S3ForceVirtualHostedStyle = reader.GetOptionalBoolean(nameof (S3ForceVirtualHostedStyle), this.S3ForceVirtualHostedStyle);
        this.CloudConnectEnableHVFileShareSupport = reader.GetOptionalBoolean(nameof (CloudConnectEnableHVFileShareSupport), this.CloudConnectEnableHVFileShareSupport);
        this.IsVersion95U4Beta = reader.GetOptionalBoolean(nameof (IsVersion95U4Beta), this.IsVersion95U4Beta);
        this.MaxStorageSnapshotCountPerVolume = reader.GetOptionalInt32(nameof (MaxStorageSnapshotCountPerVolume), 1000);
        this.SharedLockManagerTTLCheckIntervalSec = reader.GetOptionalInt32(nameof (SharedLockManagerTTLCheckIntervalSec), this.SharedLockManagerTTLCheckIntervalSec);
        this.ObjectStorageCRLCheckMode = reader.GetOptionalInt32(nameof (ObjectStorageCRLCheckMode), this.ObjectStorageCRLCheckMode);
        this.DEV_UseOldSPSearcher = reader.GetOptionalBoolean(nameof (DEV_UseOldSPSearcher), this.S3ForceVirtualHostedStyle);
        this.ExtendedUILogging = reader.GetOptionalBoolean(nameof (ExtendedUILogging), this.ExtendedUILogging);
        this.SharePointSearcherCommandTimeoutSeconds = reader.GetOptionalInt32(nameof (SharePointSearcherCommandTimeoutSeconds), this.SharePointSearcherCommandTimeoutSeconds);
        this.QMDisableRestoringTags = reader.GetOptionalBoolean(nameof (QMDisableRestoringTags), this.QMDisableRestoringTags);
        this.ObjectStorageImportIgnoreErrors = reader.GetOptionalBoolean(nameof (ObjectStorageImportIgnoreErrors), this.ObjectStorageImportIgnoreErrors);
        this.SOBRDisableEmailReport = reader.GetOptionalBoolean(nameof (SOBRDisableEmailReport), this.SOBRDisableEmailReport);
      }
    }

    private static void Watch(IRegistryConfigurationController controller)
    {
      if (COptions._watcher == null)
      {
        COptions._watcher = new RegistryOptionsWatcher(controller.RegistryKey);
      }
      else
      {
        if (COptions._watcher.Version == COptions._version)
          return;
        COptions._version = COptions._watcher.Version;
        Log.Information("[Options] Apply new options.");
      }
    }

    private void OnAbandonedSessionListCleanupFrequencyReaded()
    {
      if (!COptions.ForceAbandonedSessionListCleanupInvoked)
        return;
      this.ForceAbandonedSessionListCleanup();
    }

    public void ForceAbandonedSessionListCleanup()
    {
      if (this.AbandonedSessionListCleanupFrequency == 0)
        this.AbandonedSessionListCleanupFrequency = 256;
      COptions.ForceAbandonedSessionListCleanupInvoked = true;
    }

    public bool BackupCopyEnabledCheckGFSSchedule { get; private set; }

    public bool IsAdjustDueDateEnabled { get; private set; }

    public void SetHighestDetectedVMCVersion(string version)
    {
      using (IRegistryConfigurationController registryController = SProduct.Instance.CreateRegistryController(true, RegistryView.Default))
      {
        if (!registryController.IsReady)
          return;
        registryController.SetValue("HighestDetectedVMCVersion", version, RegistryControllerValueOption.String);
        this.HighestDetectedVMCVersion = version;
      }
    }

    public void SetVBRSerivceRestartNeeded(bool isNeeded)
    {
      using (IRegistryConfigurationController registryController = SProduct.Instance.CreateRegistryController(true, RegistryView.Default))
      {
        if (!registryController.IsReady)
          return;
        registryController.SetValue("VBRServiceRestartNeeded", isNeeded);
        this.VBRServiceRestartNeeded = isNeeded;
      }
    }

    private void UpgradeOracleArchiveLogCollectOptionSafe(
      IRegistryConfigurationController readOnlyController)
    {
      try
      {
        bool? booleanValue = readOnlyController.FindBooleanValue("OracleSelectAllArchiveLogsDuringImageBackup");
        if (!booleanValue.HasValue)
          return;
        using (IRegistryConfigurationController registryController = SProduct.Instance.CreateRegistryController(true, RegistryView.Default))
        {
          if (!registryController.IsReady)
            return;
          registryController.SetValue("OracleCollectAllPaths", booleanValue.Value);
          try
          {
            registryController.DeleteValue("OracleSelectAllArchiveLogsDuringImageBackup");
          }
          catch
          {
          }
        }
      }
      catch (Exception ex)
      {
        this.HandleError(ex.Message);
      }
    }

    private void HandleError(string errorMessage)
    {
      try
      {
        if (Interlocked.Increment(ref COptions._loggingErrors) > 10L)
          return;
      }
      catch
      {
      }
    }

    public bool IsPerfLogRequired()
    {
      return this.RemotingEnablePerfLog;
    }

    public enum EPrepareAutoMount
    {
      Disable,
      Enable,
      Off,
    }

    public enum ETemplateProcessingMode
    {
      Vms,
      Templates,
      VmsAndTemplates,
    }

    public enum EUseCifsVirtualSynthetic
    {
      Disabled,
      Automatic,
      Always,
    }

    private enum EDataMoverLocalFastPathValues
    {
      NoOptimization,
      FastPathIoCtl,
      SharedMem,
    }

    public enum EDefaultLinuxAgent
    {
      VeeamAgent24,
      VeeamAgent,
      VeeamAgent64,
    }

    public enum EAgentReadOnlyCache
    {
      Disable,
      Enable,
      DisableOnlyForIncrementalWithDedupDisabled,
      Always,
    }

    public enum EIrDisableFullBlockRead
    {
      Disable,
      EnableForDdboost,
      Automatic,
      Always,
    }

    public enum EVmDepatureSeverityType
    {
      Success,
      Warning,
      Error,
    }
  }
}
