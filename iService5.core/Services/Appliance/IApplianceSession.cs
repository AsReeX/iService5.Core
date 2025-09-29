// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Appliance.IApplianceSession
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Data;
using iService5.Ssh.DTO;
using iService5.Ssh.enums;
using System.Collections.Generic;
using System.Threading.Tasks;

#nullable disable
namespace iService5.Core.Services.Appliance;

public interface IApplianceSession
{
  List<AugmentedModule> ModuleList { get; }

  Dictionary<string, string> DeviceDictionary { get; }

  HaInfoDto HaInfoFromMetadata { get; set; }

  List<AugmentedModule> InvalidModuleList { get; set; }

  bool AllowReboot { get; }

  bool InitialiseMetadata();

  void SetHaInfoFromMetadata();

  bool InitialiseFeatureTableSupport();

  void Reset();

  void ConnectToAppliance();

  void checkDbus2ECUUpdateAvailability();

  void RetrieveHAInfo(RequestCallback cb);

  bool RequiresMaintenanceMode();

  void RetrieveBootMode(RequestCallback cb);

  void RetrieveInventory(RequestCallback cb);

  Task Reboot(RequestCallback cb);

  void SetMode(BootMode mode, RequestCallback cb);

  void CheckIfPPF6Supported();

  BootMode Bootmode { get; }

  HaInfoDto HaInfo { get; }

  bool InventoryAvailable { get; }

  bool CachedInventoryMerged { get; }

  bool LocalInventoryAvailable { get; }

  void ApplianceIsSecure(RequestCallback cb);

  void ApplySecurity(RequestCallback cb);

  bool IsBusy();

  bool CanProceed();

  void UpdateApplianceFirmware(
    UIUpdateCallback ucb,
    MessageCallback mcb,
    ProgressCallback pcb,
    FinishCallback fcb);

  void ClearInstalled();

  void setDisconnectedStatus(bool disconnectedStatus);
}
