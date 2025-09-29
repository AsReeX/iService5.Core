// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Helpers.DetailNavigationArgs
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Data.SyMaNa;
using iService5.Core.ViewModels;
using iService5.Ssh.DTO;
using System.Collections.Generic;

#nullable disable
namespace iService5.Core.Services.Helpers;

public class DetailNavigationArgs
{
  internal string historySessionID;
  internal List<string> preparationModules;
  internal string senderScreen;

  public bool _calledFromDB { get; set; }

  public bool _elpRecoveryBoot { get; set; }

  public bool _isSyMaNaReboot { get; set; }

  public bool _bootModeSwitch { get; set; }

  public bool _simpleBootToRecovery { get; set; }

  public bool ReturnToFlashing { get; set; }

  public ErrorLogViewModel.ErrorEntry errorDetails { get; set; }

  public bool ReturnToSettingsPage { get; set; }

  public bool bridgeDisplay { get; internal set; }

  public string destinationScreen { get; set; }

  public bool bridgeSettingsSelected { get; set; }

  public DetailNavigationPageType detailNavigationPageType { get; set; }

  public List<SyMaNaFirmwareUploadModel> UploadFirmwareModels { get; set; }

  public SyMaNaECU[] InventoryECUs { get; set; }

  public bool readInventory { get; set; }

  public bool shouldSetHaInfo { get; set; }

  public List<string> ControlUrlParameters { get; set; }
}
