// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Data.IMetadataService
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Platform;
using iService5.Core.ViewModels;
using iService5.Ssh.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

#nullable disable
namespace iService5.Core.Services.Data;

public interface IMetadataService
{
  List<material> getMatchingEntries(string needle, MatchPattern criteria);

  List<uploadDocument> getMaterialDocuments(string enumber);

  IEnumerable<varcodes> getOldVarCodingStatus(string enumber);

  (string, string) getBrand(string enumber);

  bool isSMMWithWifi(string enumber);

  List<module_with_enumber> getModulesDeltaSet();

  List<document> getMissingDocuments();

  List<document> getZ9KDfiles(bool bIsDownloaded = false);

  Task<bool> resetDatabase(string checksum);

  int getLocalSize();

  int getDeltaSize();

  bool isDBValid();

  bool markAsDownloaded(DownloadProxy proxy);

  List<enumber_modules> getFilesForEnumber(string enumber);

  List<enumber_modules> getMissingModules(string enumber);

  List<enumber_modules> getModules(string enumber);

  HaInfoDto getHAInfo(string enumber);

  string getConnectionGraphic(string enumber);

  string getConnectionGraphicNonSmm(string enumber);

  string GetFileSize();

  void closeDB();

  string getSSHKeyValue();

  string getErrorCodeMessage(string errorCode, string eNumber);

  string getErrorCodeDescription(string errorCode, string eNumber);

  string getMemoryCodeDescription(string errorCode);

  string getShortText(string shortTextCode, string lang);

  List<AugmentedModule> GetSMMModuleWithLabels(string enumber);

  List<smm_module> GetModulesDetails(string enumber);

  List<module> GetModuleSpecs(smm_module smmModule);

  bool CheckRecovery(long moduleId);

  string[] getShortTextPlaceholder(string[] shortTextList);

  string getMessageWithShortTextMapping(string message);

  void updateUsedStorage();

  bool deleteAllBinaries(IPlatformSpecificServiceLocator _locator);

  List<smm_whitelist> GetDowngradePass(long module);

  string GetMetaDataFileSize();

  bool IsDocumentAvailable(string enumber, string docType);

  document GetMonitoringGraphicsDocument(string enumber);

  List<Version> GetVersionFromFeatureTable(string name);

  bool CheckFeature(string fname);

  string GetMaterialType(string enumber);

  string getFirmwareName(string moduleid, iService5.Ssh.DTO.Version version, string type);

  uploadDocument GetCodingFileAvailable(string enumber);

  string GetCodingFilePath(string enumber);

  bool IsMaterialAvailable(string enumber);

  List<module_with_enumber> getModulesDeltaSetForDownloadSetting(
    Dictionary<DeviceClass, List<MaterialStatistics>> materialList,
    bool bIsDownloaded = false);

  List<module_with_enumber> getModulesForENumber(string eNumber);

  List<document> getDocumentsForENumber(string eNumber, DeviceClass deviceClass);

  List<document> getDocumentsDeltaSetForSingleDownloadAndPrepareYourWork(
    Dictionary<DeviceClass, List<MaterialStatistics>> materialList,
    bool bIsDownloaded = false);

  DownloadFilesStatistics GetDownloadFilesStatistics(
    Dictionary<DeviceClass, List<MaterialStatistics>> deviceClassBasedMaterials);

  DownloadFilesStatistics GetDownloadFilesStatisticsForFullDownloadAndCountryRelevant(
    string deviceClass,
    Dictionary<DeviceClass, List<MaterialStatistics>> deviceClassBasedMaterials);

  List<MaterialStatistics> GetDownloadFilesStatisticsModules();

  List<MaterialStatistics> GetDownloadFilesStatisticsDocuments();

  void UpdateMaterialStatistics();

  void UpdatePpfTable(
    string _vib,
    string _ki,
    long _moduleid,
    string _version,
    long _expdate,
    long _type,
    string _ca,
    string _ppffile);

  List<MaterialStatistics> GetMaterialStatistics();

  long GetTotalFileSizeForENumber(string eNo);

  List<Country> GetCountryList();

  bool CheckIfTableExists(string tableName);

  bool UpdatePendingMoreThan14Days();

  bool IsModuleInRecoveryReboot(AugmentedModule module);

  List<smm_module> getSMMModulesRecords(string _vib, string _ki, long _fwid);

  string getNodeName(long _node, string _devicetype);

  bool isSMM(string enumber);

  bool isNONSMM(string enumber);

  bool isSMMFlintStone(string enumber);

  List<MaterialStatistics> getMaterialStatisticsOfENumbers(string eNumbers);

  void UpdateDownloadedStatus(string fileId, bool isDocument, bool setAsDownloaded);

  List<DownloadProxy> GetExpiredAndAboutToExpireENumbers();

  void DeletePpfEntries(ppf ppf);

  List<ppf> GetRelatedPpfs(
    string vib,
    string ki,
    long moduleid,
    string version,
    bool ppf6supported);

  List<ppf> GetAllEnumbersDownloadedPpfs(string enumber, bool onlyType6Ppfs = false);

  string GetDeviceTypeForSMM(string enumber);

  bool checkIfAppHasOldDB();

  void clearCacheUsedStorage();

  bool IsENumberSyMaNa(string eNumber);

  void updatePPFRefetchStatus(string material, bool setAsDownloaded);

  List<string> GetRefetchedPPFEnumbers();

  void StoreRefetchedPPFENumbersFromOldDB(List<string> refetchedPPFsEnumbers);

  List<string> GetRefetchedPPFENumbersFromOldDB();

  void DeleteExistingPPF(DownloadProxy proxy);

  List<ppf> GetPpfs();

  List<string> GetDownloadedEnumbersFromPPF();

  bool IsPPFTableMigrationNeded();

  bool MigratePPFTable();
}
