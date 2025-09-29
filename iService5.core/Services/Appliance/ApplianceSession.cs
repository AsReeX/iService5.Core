// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Appliance.ApplianceSession
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Data;
using iService5.Core.Services.Helpers;
using iService5.Core.Services.Platform;
using iService5.Core.Services.User;
using iService5.Core.Services.User.ActivityLogging;
using iService5.Core.ViewModels;
using iService5.Ssh.DTO;
using iService5.Ssh.enums;
using iService5.Ssh.models;
using Microsoft.IdentityModel.Tokens;
using MvvmCross;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
//using Xamarin.Forms;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

#nullable disable
namespace iService5.Core.Services.Appliance;

public class ApplianceSession : IApplianceSession
{
  private readonly IUserSession UserSession;
  private readonly IMetadataService MetadataService;
  internal List<AugmentedModule> Modules;
  public readonly ILoggingService LogService;
  internal Dictionary<string, string> _deviceDictionary = new Dictionary<string, string>()
  {
    {
      "20",
      "Dishwasher"
    },
    {
      "40",
      "Oven"
    },
    {
      "41",
      "Hob"
    },
    {
      "42",
      "Hood"
    },
    {
      "43",
      "RangeCooker"
    },
    {
      "44",
      "Microwave"
    },
    {
      "45",
      "Steamer"
    },
    {
      "46",
      "VentingCooktop"
    },
    {
      "47",
      "Waterbase"
    },
    {
      "48",
      "WarmingDrawer"
    },
    {
      "60",
      "Refrigerator"
    },
    {
      "61",
      "Freezer"
    },
    {
      "62",
      "FridgeFreezer"
    },
    {
      "63",
      "WineCooler"
    },
    {
      "80",
      "Washer"
    },
    {
      "81",
      "Dryer"
    },
    {
      "82",
      "WasherDryer"
    },
    {
      "A0",
      "CoffeeMaker"
    },
    {
      "A1",
      "WaterHeater"
    },
    {
      "A2",
      "HotWaterHeaterPump"
    },
    {
      "A4",
      "CleaningRobot"
    },
    {
      "A5",
      "CookProcessor"
    },
    {
      "A6",
      "KitchenRobot"
    },
    {
      "A7",
      "Blender"
    },
    {
      "A8",
      "SmartGrow"
    },
    {
      "A9",
      "KitchenMachine"
    },
    {
      "C0",
      "Appliance"
    },
    {
      "C1",
      "DataLogger"
    }
  };
  private readonly IPlatformSpecificServiceLocator Locator;
  private readonly IAlertService Alert;
  private const char OK = '✓';
  private const char FAIL = 'X';
  private int TotalFlashingSteps = 0;
  private int CurrentFlashingStep = 0;
  private string CurrentModule = "";
  private const string elpPackage = "elp";
  private string signatureFile = "";
  private FirmwareInstallMode currentFWInstallMode = FirmwareInstallMode.None;
  private List<AugmentedModule> invalidModules = new List<AugmentedModule>();
  internal InventoryDto InventoryInfo;
  internal List<iService5.Core.Services.Data.Version> versionRangeForPurging = new List<iService5.Core.Services.Data.Version>();
  internal List<iService5.Core.Services.Data.Version> versionRangeForECUFlashing = new List<iService5.Core.Services.Data.Version>();
  internal List<iService5.Core.Services.Data.Version> versionRangeForInstallRepair = new List<iService5.Core.Services.Data.Version>();
  internal List<iService5.Core.Services.Data.Version> versionRangeForDowngradeAll = new List<iService5.Core.Services.Data.Version>();
  internal List<iService5.Core.Services.Data.Version> versionRangeForHACountrySetting = new List<iService5.Core.Services.Data.Version>();
  internal List<iService5.Core.Services.Data.Version> versionRangeForType1Check = new List<iService5.Core.Services.Data.Version>();
  internal List<iService5.Core.Services.Data.Version> versionRangeForType3Check = new List<iService5.Core.Services.Data.Version>();
  internal List<iService5.Core.Services.Data.Version> versionRangeForType6Check = new List<iService5.Core.Services.Data.Version>();
  internal List<iService5.Core.Services.Data.Version> versionRangeForPPF6 = new List<iService5.Core.Services.Data.Version>();
  private bool _installRepairPopupActive = false;
  private bool isDisconnected = false;
  private bool initializedMetadataOnce;
  private bool CachedInventoryCmdExecutedSuccessfully;
  private bool LocalInventoryRetrieved;
  private bool InventoryRetrieved;
  private bool isReprogram = false;
  private bool installRepairExecuted = false;
  private bool installRepairSuccess = false;
  private bool emptySMM = false;
  private bool spauDetectedFlag = false;
  private bool fwInstalledFlag = false;
  private bool spauUploadedFlag = false;
  private AutoResetEvent autoEvent = new AutoResetEvent(false);
    internal readonly Action<object> ComparisonTask = (Action<object>) (obj =>
  {
    try
    {
      if (obj.GetType().Equals(typeof (ApplianceTask<FirmwareIDComparisonDecision>)))
      {
        ApplianceTask<FirmwareIDComparisonDecision> taskObject = (ApplianceTask<FirmwareIDComparisonDecision>) obj;
        Serilog.Core.Logger logger1 = taskObject.Session.LogService.getLogger();
        int num1 = taskObject.Session.Modules.Count;
        string message1 = $"Analyzing modulelist (Count: {num1.ToString()})";
        logger1.LogAppInformation(LoggingContext.BINARY, message1, memberName: nameof (ComparisonTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 452);
        foreach (AugmentedModule module in taskObject.Session.Modules)
        {
          Serilog.Core.Logger logger2 = taskObject.Session.LogService.getLogger();
          string[] strArray = new string[6]
          {
            "moduleId ",
            module.moduleid.ToString(),
            "(",
            module.version,
            ") at node ",
            null
          };
          num1 = module.node;
          strArray[5] = num1.ToString();
          string message2 = string.Concat(strArray);
          logger2.LogAppInformation(LoggingContext.BINARY, message2, memberName: nameof (ComparisonTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 455);
          if (module.type == ModuleType.SPAU_FIRMWARE.ToString())
            module.IncludeInComparison = false;
        }
        List<AugmentedModule> list1 = taskObject.Session.Modules.Where<AugmentedModule>((Func<AugmentedModule, bool>) (p => taskObject.Session.OnlyFW(p))).ToList<AugmentedModule>();
        List<AugmentedModule> list2 = list1.Where<AugmentedModule>((Func<AugmentedModule, bool>) (p => p.IncludeInComparison && !string.IsNullOrEmpty(p.InstalledVersion) && p.InstalledVersion != "Unknown")).ToList<AugmentedModule>();
        List<AugmentedModule> list3 = list1.Where<AugmentedModule>((Func<AugmentedModule, bool>) (p => p.IncludeInComparison && !string.IsNullOrEmpty(p.InstalledVersion) && p.InstalledVersion != "Unknown" && p.Available == null)).ToList<AugmentedModule>();
        List<AugmentedModule> list4 = list1.Where<AugmentedModule>((Func<AugmentedModule, bool>) (p => p.IncludeInComparison && p.Available != null)).ToList<AugmentedModule>();
        List<AugmentedModule> list5 = list1.Where<AugmentedModule>((Func<AugmentedModule, bool>) (p => p.IncludeInComparison && p.Available != null && p.InstalledVersion == null)).ToList<AugmentedModule>();
        Serilog.Core.Logger logger3 = taskObject.Session.LogService.getLogger();
        string[] strArray1 = new string[5]
        {
          "Number of installed Firmware: ",
          null,
          null,
          null,
          null
        };
        int count1 = list2.Count;
        strArray1[1] = count1.ToString();
        strArray1[2] = " (";
        count1 = list3.Count;
        strArray1[3] = count1.ToString();
        strArray1[4] = " without available Firmware";
        string message3 = string.Concat(strArray1);
        logger3.LogAppInformation(LoggingContext.BINARY, message3, memberName: nameof (ComparisonTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 471);
        Serilog.Core.Logger logger4 = taskObject.Session.LogService.getLogger();
        string[] strArray2 = new string[5]
        {
          "Number of available Firmware: ",
          null,
          null,
          null,
          null
        };
        int count2 = list4.Count;
        strArray2[1] = count2.ToString();
        strArray2[2] = " (";
        count2 = list5.Count;
        strArray2[3] = count2.ToString();
        strArray2[4] = " without installed Firmware:)";
        string message4 = string.Concat(strArray2);
        logger4.LogAppInformation(LoggingContext.BINARY, message4, memberName: nameof (ComparisonTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 472);
        bool flag1 = list2.OrderBy<AugmentedModule, long>((Func<AugmentedModule, long>) (e => e.moduleid)).SequenceEqual<AugmentedModule>((IEnumerable<AugmentedModule>) list4.OrderBy<AugmentedModule, long>((Func<AugmentedModule, long>) (e => e.moduleid)));
        List<long> installedModuleIds = list2.Select<AugmentedModule, long>((Func<AugmentedModule, long>) (p => p.moduleid)).ToList<long>();
        List<long> availableModuleIds = list4.Select<AugmentedModule, long>((Func<AugmentedModule, long>) (p => p.moduleid)).ToList<long>();
        availableModuleIds.All<long>((Func<long, bool>) (p => installedModuleIds.Contains(p)));
        bool flag2 = availableModuleIds.All<long>((Func<long, bool>) (p => installedModuleIds.Contains(p))) && installedModuleIds.All<long>((Func<long, bool>) (p => availableModuleIds.Contains(p)));
        bool flag3 = !installedModuleIds.Except<long>((IEnumerable<long>) availableModuleIds).Any<long>();
        bool flag4 = availableModuleIds.Count >= installedModuleIds.Count;
        bool flag5 = flag3 & flag4;
        bool flag6 = Math.Abs(availableModuleIds.Count - installedModuleIds.Count) > 0;
        bool flag7 = Math.Abs(availableModuleIds.Count - installedModuleIds.Count) == 0;
        bool flag8 = availableModuleIds.Count - installedModuleIds.Count < 0;
        int num2 = taskObject.Session.Modules.Count<AugmentedModule>((Func<AugmentedModule, bool>) (p => p.IncludeInComparison));
        int num3 = taskObject.Session.Modules.Count<AugmentedModule>((Func<AugmentedModule, bool>) (p => p.type == ModuleType.FIRMWARE.ToString() && !string.IsNullOrEmpty(p.InstalledVersion)));
        bool flag9 = num2 < num3;
        if (((list2.Count == 0 ? 1 : (list2.Count != 1 ? 0 : (list2[0].Recovery ? 1 : 0))) | (flag1 ? 1 : 0) | (flag5 ? 1 : 0)) != 0)
        {
          taskObject.Session.LogService.getLogger().LogAppInformation(LoggingContext.BINARY, "Decision: Proceed", memberName: nameof (ComparisonTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 503);
          taskObject.TaskCallback((object) FirmwareIDComparisonDecision.Proceed);
        }
        else if (!flag3 && flag6 | flag7)
        {
          taskObject.Session.LogService.getLogger().LogAppInformation(LoggingContext.BINARY, "Decision: Purge - modules names are not equal", memberName: nameof (ComparisonTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 508);
          taskObject.TaskCallback((object) FirmwareIDComparisonDecision.Purge);
        }
        else
        {
          if (!flag9)
            return;
          taskObject.Session.LogService.getLogger().LogAppInformation(LoggingContext.BINARY, $"Decision: MaterialComparison - DB-modules({num2.ToString()}) are less than the installed modules({num3.ToString()})", memberName: nameof (ComparisonTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 513);
          taskObject.TaskCallback((object) FirmwareIDComparisonDecision.MaterialComparison);
        }
      }
      else
      {
        if (!obj.GetType().Equals(typeof (ApplianceTask<FirmwareVersionComparisonDecision>)))
          return;
        ApplianceTask<FirmwareVersionComparisonDecision> taskObject = (ApplianceTask<FirmwareVersionComparisonDecision>) obj;
        taskObject.Session.LogService.getLogger().LogAppInformation(LoggingContext.BINARY, "In FirmwareVersionComparisonDecision", memberName: nameof (ComparisonTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 520);
        if (!taskObject.Session.ElpSupportsEcuFlashing || !taskObject.Session.IsType6CheckSupported())
        {
          foreach (AugmentedModule module in taskObject.Session.Modules)
          {
            if (module.type == ModuleType.SPAU_FIRMWARE.ToString())
            {
              module.IncludeInComparison = false;
              taskObject.Session.LogService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, $"Excluding module=({module.moduleid.ToString()}) from comparison, because Type is SPAU", memberName: nameof (ComparisonTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 529);
            }
          }
        }
        bool downgradeAllSupport = taskObject.Session.DowngradeAllSupport;
        List<AugmentedModule> list = taskObject.Session.Modules.Where<AugmentedModule>((Func<AugmentedModule, bool>) (p => p.IncludeInComparison && p.Recovery && !p.Installed.IsVersionEqual(p.Available))).ToList<AugmentedModule>();
        taskObject.Session.AllowReboot = list.Any<AugmentedModule>((Func<AugmentedModule, bool>) (x => taskObject.Session.MetadataService.IsModuleInRecoveryReboot(x)));
        bool flag10 = list.Count > 0;
        taskObject.Session.RecoveryDifferenceDetected = flag10;
        bool flag11 = taskObject.Session.Modules.Count<AugmentedModule>((Func<AugmentedModule, bool>) (p => p.IncludeInComparison && p.Package == "elp" && !p.Installed.IsVersionEqual(p.Available))) > 0;
        int num4 = taskObject.Session.Modules.Count<AugmentedModule>((Func<AugmentedModule, bool>) (p => p.IncludeInComparison && p.version != ""));
        bool flag12 = taskObject.Session.Modules.Count<AugmentedModule>((Func<AugmentedModule, bool>) (p => p.IncludeInComparison && !p.Installed.Equals((object) p.Available))) > 0;
        int num5 = taskObject.Session.Modules.Count<AugmentedModule>((Func<AugmentedModule, bool>) (p => p.IncludeInComparison && p.Installed.IsVersionEqual(p.Available)));
        bool flag13 = taskObject.Session.Modules.All<AugmentedModule>((Func<AugmentedModule, bool>) (p => p.Installed > p.Available));
        bool flag14 = taskObject.Session.Modules.All<AugmentedModule>((Func<AugmentedModule, bool>) (p => p.IncludeInComparison && p.Installed.IsVersionEqual(p.Available)));
        bool flag15 = taskObject.Session.Modules.Count<AugmentedModule>((Func<AugmentedModule, bool>) (p => p.IncludeInComparison && p.Recovery && p.Installed > p.Available)) == 1;
        bool flag16 = taskObject.Session.Modules.Count<AugmentedModule>((Func<AugmentedModule, bool>) (p => p.IncludeInComparison && p.Package == "elp" && p.Installed > p.Available)) == 1;
        bool flag17 = taskObject.Session.IsRecoveryDowngradeAllowed();
        bool flag18 = taskObject.Session.IsELPDowngradeAllowed();
        bool flag19 = flag15 | flag16;
        if (num5 == num4)
        {
          taskObject.Session.LogService.getLogger().LogAppInformation(LoggingContext.BINARY, "Number of available and installed versions are matching. Proceeding.", memberName: nameof (ComparisonTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 555);
          taskObject.TaskCallback((object) FirmwareVersionComparisonDecision.AskForRepeat);
        }
        else if (flag12 && !flag13)
        {
          if (((!flag19 ? 0 : (flag18 | flag17 ? 1 : 0)) & (flag10 ? 1 : 0)) != 0 || !flag19 & flag10)
          {
            taskObject.Session.LogService.getLogger().LogAppInformation(LoggingContext.BINARY, "Decision: FlashRecovery", memberName: nameof (ComparisonTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 563);
            taskObject.TaskCallback((object) FirmwareVersionComparisonDecision.FlashRecovery);
          }
          else if (flag19 && flag18 | flag17 && !flag10)
          {
            taskObject.Session.LogService.getLogger().LogAppInformation(LoggingContext.BINARY, "Decision: AskForNewAll", memberName: nameof (ComparisonTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 568);
            taskObject.TaskCallback((object) FirmwareVersionComparisonDecision.AskForNewAll);
          }
          else if (flag19 && !(flag18 | flag17))
          {
            taskObject.Session.LogService.getLogger().LogAppInformation(LoggingContext.BINARY, "Decision: NotWhitelisted", memberName: nameof (ComparisonTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 573);
            taskObject.TaskCallback((object) FirmwareVersionComparisonDecision.NotWhitelisted);
          }
          else if (!flag19 && !flag10)
          {
            taskObject.Session.LogService.getLogger().LogAppInformation(LoggingContext.BINARY, "Decision: AskForNewAll", memberName: nameof (ComparisonTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 578);
            taskObject.TaskCallback((object) FirmwareVersionComparisonDecision.AskForNewAll);
          }
          else
          {
            taskObject.Session.LogService.getLogger().LogAppInformation(LoggingContext.BINARY, "Decision: AskForRepeat", memberName: nameof (ComparisonTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 583);
            taskObject.TaskCallback((object) FirmwareVersionComparisonDecision.AskForRepeat);
          }
        }
        else if (flag13)
        {
          if (downgradeAllSupport)
          {
            taskObject.Session.LogService.getLogger().LogAppInformation(LoggingContext.BINARY, "Programming continued, as downgrade - feature is enabled", memberName: nameof (ComparisonTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 591);
            if (flag19 & flag10 && flag18 | flag17)
            {
              taskObject.Session.LogService.getLogger().LogAppInformation(LoggingContext.BINARY, "Decision: FlashRecovery", memberName: nameof (ComparisonTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 594);
              taskObject.TaskCallback((object) FirmwareVersionComparisonDecision.FlashRecovery);
            }
            else if (flag19 && !flag18 && !flag17)
            {
              taskObject.Session.LogService.getLogger().LogAppInformation(LoggingContext.BINARY, "Decision: NotWhitelisted", memberName: nameof (ComparisonTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 599);
              taskObject.TaskCallback((object) FirmwareVersionComparisonDecision.NotWhitelisted);
            }
            else if (flag19 && !flag10 && flag18 | flag17)
            {
              taskObject.Session.LogService.getLogger().LogAppInformation(LoggingContext.BINARY, "Decision: AskForNewAll", memberName: nameof (ComparisonTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 604);
              taskObject.TaskCallback((object) FirmwareVersionComparisonDecision.AskForNewAll);
            }
          }
          else
          {
            taskObject.Session.LogService.getLogger().LogAppInformation(LoggingContext.BINARY, "Programming aborted, as downgrade-feature is disabled", memberName: nameof (ComparisonTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 610);
            taskObject.TaskCallback((object) FirmwareVersionComparisonDecision.Abort);
          }
        }
        else if (flag14)
        {
          taskObject.Session.LogService.getLogger().LogAppInformation(LoggingContext.BINARY, "All version are equal.", memberName: nameof (ComparisonTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 616);
          taskObject.TaskCallback((object) FirmwareVersionComparisonDecision.AskForRepeat);
        }
        else
        {
          taskObject.Session.LogService.getLogger().LogAppInformation(LoggingContext.BINARY, "Aborting Programming. No match in decision-tree", memberName: nameof (ComparisonTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 621);
          taskObject.TaskCallback((object) FirmwareVersionComparisonDecision.Abort);
        }
      }
    }
    catch (Exception ex)
    {
      ((ApplianceTaskException) obj.GetType().GetProperty("ExceptionCallback").GetValue(obj))(ex.Message);
    }
  });
  internal readonly Action<object> ApplianceInfoTask = (Action<object>) (obj =>
  {
    try
    {
      Is5SshWrapper is5SshWrapper = Mvx.IoCProvider.Resolve<Is5SshWrapper>();
      if (obj.GetType().Equals(typeof (ApplianceTask<EcuUpdate>)))
      {
        ApplianceTask<EcuUpdate> applianceTask = (ApplianceTask<EcuUpdate>) obj;
        applianceTask.Session.LogService.getLogger().LogAppDebug(LoggingContext.APPLIANCE, "Decision for install-repair execution ", memberName: nameof (ApplianceInfoTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 669);
        bool featureAvailable = applianceTask.Session.InstallRepairFeatureAvailable;
        bool ecuUpdateAvailable = applianceTask.Session.isECUUpdateAvailable;
        applianceTask.Session.LogService.getLogger().LogAppDebug(LoggingContext.APPLIANCE, $"ECUUpdateFeature = {ecuUpdateAvailable.ToString()} for this appliance", memberName: nameof (ApplianceInfoTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 675);
        if (ecuUpdateAvailable)
        {
          applianceTask.Session.LogService.getLogger().LogAppDebug(LoggingContext.APPLIANCE, "ECUUpdateFeature available. According to the suggested flow, Skipping install-repair", memberName: nameof (ApplianceInfoTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 678);
          applianceTask.ExceptionCallback("Failed to install-repair Firmware: ECUUpdate available. Skipping install-repair");
        }
        else
        {
          applianceTask.Session.ModalBox("INSTALL_REPAIR_ECU");
          applianceTask.Session.AttemptMessageCallback(AppResource.INSTALL_REPAIR_ECU + "\n");
          if (featureAvailable)
          {
            applianceTask.Session.AttemptProgressCallback(AppResource.APPLIANCE_INSTALL_REPAIR_REPSONSE_WAIT, 0, 1);
            applianceTask.Session.AttemptMessageCallback(AppResource.APPLIANCE_INSTALL_REPAIR_REPSONSE_WAIT + "\n");
            applianceTask.Session.LogService.getLogger().LogAppDebug(LoggingContext.APPLIANCE, "install-repair trying to execute", memberName: nameof (ApplianceInfoTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 689);
            SshResponse sshResponse = is5SshWrapper.InstallRepair();
            applianceTask.Session.LogService.getLogger().LogAppDebug(LoggingContext.APPLIANCE, "install-repair response arrived : " + sshResponse.RawResponse?.ToString(), memberName: nameof (ApplianceInfoTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 691);
            if (sshResponse.Success)
            {
              applianceTask.Session.AttemptMessageCallback(AppResource.APPLIANCE_INSTALL_REPAIR_SUCCESS + "\n");
              applianceTask.TaskCallback((object) sshResponse.Success);
              applianceTask.Session.LogService.getLogger().LogAppDebug(LoggingContext.APPLIANCE, "install-repair Success\n", memberName: nameof (ApplianceInfoTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 696);
            }
            else
            {
              applianceTask.Session.AttemptMessageCallback($"{AppResource.APPLIANCE_INSTALL_REPAIR_FAILED} : {sshResponse.ErrorMessage}\n");
              applianceTask.ExceptionCallback($"{AppResource.APPLIANCE_INSTALL_REPAIR_FAILED} : {sshResponse.ErrorMessage}");
              applianceTask.Session.LogService.getLogger().LogAppError(LoggingContext.APPLIANCE, "install-repair Failed  : " + sshResponse.ErrorMessage, memberName: nameof (ApplianceInfoTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 702);
            }
          }
          else
          {
            applianceTask.ExceptionCallback("installRepair not available");
            applianceTask.Session.LogService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "install-repair not available", memberName: nameof (ApplianceInfoTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 708);
          }
        }
      }
      else if (obj.GetType().Equals(typeof (ApplianceTask<iService5.Core.Services.Appliance.Reboot>)))
      {
        ApplianceTask<iService5.Core.Services.Appliance.Reboot> applianceTask = (ApplianceTask<iService5.Core.Services.Appliance.Reboot>) obj;
        SshResponse sshResponse = is5SshWrapper.Reboot();
        if (sshResponse.Success)
          applianceTask.TaskCallback((object) sshResponse.Success);
        else
          applianceTask.ExceptionCallback("Failed to reboot: " + sshResponse.ErrorMessage);
      }
      else if (obj.GetType().Equals(typeof (ApplianceTask<EnableEcuUpdate>)))
      {
        ApplianceTask<EnableEcuUpdate> applianceTask = (ApplianceTask<EnableEcuUpdate>) obj;
        SshResponse sshResponse = is5SshWrapper.EnableDbusECUUpdate();
        if (sshResponse.Success)
          applianceTask.TaskCallback((object) sshResponse.Success);
        else
          applianceTask.ExceptionCallback("Failed to EnableDbusECUUpdate: " + sshResponse.ErrorMessage);
      }
      else if (obj.GetType().Equals(typeof (ApplianceTask<BootModeChange>)))
      {
        ApplianceTask<BootModeChange> applianceTask = (ApplianceTask<BootModeChange>) obj;
        SshResponse sshResponse = is5SshWrapper.SetBootMode(applianceTask.ContextType.Mode);
        if (!sshResponse.Success)
        {
          applianceTask.ExceptionCallback("Failed to set bootmode: " + sshResponse.ErrorMessage);
          if (applianceTask.ContextType.Mode == BootMode.Maintenance)
            sshResponse = is5SshWrapper.SetBootMode(BootMode.Recovery);
        }
        if (sshResponse.Success)
          applianceTask.TaskCallback((object) sshResponse.Success);
        else
          applianceTask.ExceptionCallback("Failed to set bootmode: " + sshResponse.ErrorMessage);
      }
      else if (obj.GetType().Equals(typeof (ApplianceTask<BootMode>)))
      {
        ApplianceTask<BootMode> applianceTask = (ApplianceTask<BootMode>) obj;
        int num = 0;
        do
        {
          SshResponse<BootMode> bootMode = is5SshWrapper.GetBootMode();
          ++num;
          if (bootMode.Success)
          {
            num = 2;
            applianceTask.TaskCallback((object) bootMode.Response);
          }
          else if (num > 1)
            applianceTask.ExceptionCallback("Failed to get bootmode: " + bootMode.ErrorMessage);
        }
        while (num < 2);
      }
      else if (obj.GetType().Equals(typeof (ApplianceTask<Security>)))
      {
        ApplianceTask<Security> applianceTask = (ApplianceTask<Security>) obj;
        SshResponse sshResponse = is5SshWrapper.IsDeviceSecure();
        if (sshResponse.Success)
          applianceTask.TaskCallback((object) sshResponse.ErrorOut);
        else
          applianceTask.ExceptionCallback(sshResponse.ErrorOut);
      }
      else if (obj.GetType().Equals(typeof (ApplianceTask<iService5.Core.Services.Appliance.ApplySecurity>)))
      {
        ApplianceTask<iService5.Core.Services.Appliance.ApplySecurity> applianceTask = (ApplianceTask<iService5.Core.Services.Appliance.ApplySecurity>) obj;
        SshResponse sshResponse = is5SshWrapper.SecureDevice();
        if (sshResponse.Success)
          applianceTask.TaskCallback((object) sshResponse.ErrorOut);
        else
          applianceTask.ExceptionCallback(sshResponse.ErrorOut);
      }
      else if (obj.GetType().Equals(typeof (ApplianceTask<HAEnum>)))
      {
        ApplianceTask<HAEnum> applianceTask = (ApplianceTask<HAEnum>) obj;
        SshResponse sshResponse = is5SshWrapper.SetHa(applianceTask.ContextType, applianceTask.Session.GetHAValue(applianceTask.ContextType));
        if (sshResponse.Success)
          applianceTask.TaskCallback((object) sshResponse.ErrorOut);
        else
          applianceTask.ExceptionCallback("Failed to get bootmode: " + sshResponse.ErrorMessage);
      }
      else if (obj.GetType().Equals(typeof (ApplianceTask<HaInfoDto>)))
      {
        ApplianceTask<HaInfoDto> applianceTask = (ApplianceTask<HaInfoDto>) obj;
        int num = 0;
        do
        {
          SshResponse<HaInfoDto> haInfo = is5SshWrapper.GetHaInfo();
          ++num;
          if (haInfo.Success && haInfo.Response != null)
          {
            num = 2;
            applianceTask.TaskCallback((object) haInfo.Response);
          }
          else if (num > 1)
            applianceTask.ExceptionCallback("Failed to get HA Info: " + haInfo.ErrorMessage);
        }
        while (num < 2);
      }
      else if (obj.GetType().Equals(typeof (ApplianceTask<InventoryDto>)))
      {
        ApplianceTask<InventoryDto> applianceTask = (ApplianceTask<InventoryDto>) obj;
        SshResponse<InventoryDto> sshResponse = new SshResponse<InventoryDto>()
        {
          Success = false
        };
        bool flag20 = false;
        bool flag21 = applianceTask.Session.CurrentBootMode == BootMode.Recovery || applianceTask.Session.CurrentBootMode == BootMode.Maintenance;
        if (!flag21 && !applianceTask.Session.InventoryAvailable)
        {
          sshResponse = is5SshWrapper.GetInventory();
          applianceTask.Session.InventoryRetrieved = true;
        }
        else if (flag21 && !applianceTask.Session.LocalInventoryAvailable)
        {
          sshResponse = is5SshWrapper.GetLocalInventory();
          applianceTask.Session.LocalInventoryRetrieved = true;
        }
        else if (flag21 && !applianceTask.Session.CachedInventoryMerged)
        {
          sshResponse = is5SshWrapper.GetCachedInventory();
          applianceTask.Session.CachedInventoryCmdExecutedSuccessfully = true;
        }
        else
          flag20 = true;
        if (flag20)
          applianceTask.TaskCallback((object) null);
        else if (sshResponse.Success)
        {
          applianceTask.TaskCallback((object) sshResponse.Response);
        }
        else
        {
          applianceTask.Session.InventoryRetrieved = false;
          applianceTask.Session.LocalInventoryRetrieved = false;
          applianceTask.Session.CachedInventoryCmdExecutedSuccessfully = false;
          applianceTask.ExceptionCallback("Failed to get Inventory: " + sshResponse.ErrorMessage);
        }
      }
      else if (obj.GetType().Equals(typeof (ApplianceTask<Purge>)))
        ((ApplianceTask<Purge>) obj).TaskCallback((object) is5SshWrapper.Purge().Success);
      else if (obj.GetType().Equals(typeof (ApplianceTask<UploadFirmware>)))
      {
        ApplianceTask<UploadFirmware> applianceTask = (ApplianceTask<UploadFirmware>) obj;
        SshResponse sshResponse = is5SshWrapper.UploadUpdate(applianceTask.ContextType.ModuleToUpload);
        if (sshResponse.Success)
        {
          if (applianceTask.Session.CurrentModule.Contains("SPAU"))
            applianceTask.Session.spauUploadedFlag = true;
          applianceTask.TaskCallback((object) sshResponse.Success);
        }
        else
          applianceTask.ExceptionCallback("Failed to Upload Firmware: error=" + sshResponse.ErrorMessage);
      }
      else
      {
        if (!obj.GetType().Equals(typeof (ApplianceTask<InstallFirmware>)))
          return;
        ApplianceTask<InstallFirmware> applianceTask = (ApplianceTask<InstallFirmware>) obj;
        SshResponse sshResponse = is5SshWrapper.InstallUpdate(applianceTask.ContextType.ModuleToInstall);
        if (sshResponse.Success)
        {
          if (applianceTask.Session.CurrentModule.Contains("SPAU"))
            applianceTask.Session.spauDetectedFlag = true;
          applianceTask.TaskCallback((object) sshResponse.Success);
        }
        else
          applianceTask.ExceptionCallback("Failed to Install Firmware, ModuleToInstall:  " + sshResponse.ErrorMessage);
      }
    }
    catch (Exception ex)
    {
      ((ApplianceTaskException) obj.GetType().GetProperty("ExceptionCallback").GetValue(obj))(ex.Message);
    }
  });
  internal readonly Action<object> FirmwarePackageTask = (Action<object>) (obj =>
  {
    try
    {
      if (obj.GetType().Equals(typeof (ApplianceTask<FirmwareInstallation>)))
      {
        ApplianceTask<FirmwareInstallation> taskObject = (ApplianceTask<FirmwareInstallation>) obj;
        KeyValuePair<string, AugmentedModule> mod = taskObject.ContextType.FolderToUse;
        Path.Combine(UtilityFunctions.getPathOf(taskObject.Session.Locator, "binarySession"), $"{mod.Value.moduleid.ToString()}.{mod.Value.Available.ToString()}");
        string str = $"{mod.Value.moduleid.ToString()} ({mod.Value.Available.ToString()})";
        if (!taskObject.Session.emptySMM && taskObject.Session.isECUUpdateAvailable && taskObject.Session.ElpSupportsEcuFlashing && mod.Value.type.Equals("SPAU_FIRMWARE"))
        {
          taskObject.Session.LogService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, $"'EnableDbusEcuUpdate' execution begins for Module ID {mod.Value.moduleid.ToString()}({mod.Value.version})", memberName: nameof (FirmwarePackageTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 955);
          bool dbusEcuUpdateResponse = false;
          
        }
        ++taskObject.Session.CurrentFlashingStep;
        taskObject.Session.AttemptProgressCallback("", taskObject.Session.CurrentFlashingStep, taskObject.Session.TotalFlashingSteps);
        taskObject.Session.LogService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Module file used: " + mod.Key, memberName: nameof (FirmwarePackageTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 969);
        Stream binaryStream = (Stream) new CustomMemoryStream((iService5.Core.Services.Data.progressCallback) (res => taskObject.Session.AttemptProgressCallback(res, taskObject.Session.CurrentFlashingStep, taskObject.Session.TotalFlashingSteps)));
        long length = new FileInfo(mod.Key).Length;
        binaryStream.Write(File.ReadAllBytes(mod.Key), 0, (int) length);
        binaryStream.Flush();
        binaryStream.Position = 0L;
        taskObject.Session.CurrentModule = mod.Value.type;
        taskObject.Session.AttemptMessageCallback($"{AppResource.APPLIANCE_FLASH_FILES_FW_FILE} {str}(size (bytes): {length.ToString()})\n");
        taskObject.Session.AttemptMessageCallback($"{AppResource.APPLIANCE_FLASH_UPLOADING} {str} ...");
        Serilog.Core.Logger logger5 = taskObject.Session.LogService.getLogger();
        string[] strArray3 = new string[8]
        {
          "Starting task UploadFirmware for module=",
          str,
          " node=",
          null,
          null,
          null,
          null,
          null
        };
        int node = mod.Value.node;
        strArray3[3] = node.ToString();
        strArray3[4] = " type=";
        strArray3[5] = mod.Value.type;
        strArray3[6] = " and fileSize in bytes is:";
        strArray3[7] = length.ToString();
        string message5 = string.Concat(strArray3);
        logger5.LogAppInformation(LoggingContext.APPLIANCE, message5, memberName: nameof (FirmwarePackageTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 982);
        bool errorless = true;
        ApplianceTask<UploadFirmware> state1 = new ApplianceTask<UploadFirmware>();
        AutoResetEvent autoEvent = new AutoResetEvent(false);
        state1.ContextType = new UploadFirmware(binaryStream);
        state1.TaskCallback = (ApplianceTaskCallback<UploadFirmware>) (taskRet =>
        {
          errorless = true;
          taskObject.Session.AttemptMessageCallback(AppResource.APPLIANCE_FLASH_DONE + "\n");
          autoEvent.Set();
        });
        state1.Session = taskObject.Session;
        state1.ExceptionCallback = (ApplianceTaskException) (error =>
        {
          mod.Value.State = 'X';
          mod.Value.StateColor = "Red";
          taskObject.Session.AttemptUIUpdateRefresh();
          taskObject.Session.AttemptMessageCallback($" {AppResource.APPLIANCE_FLASH_FAILED}\n");
          taskObject.Session.LogService.getLogger().LogAppError(LoggingContext.APPLIANCE, "UploadFirmware ExceptionCallback: " + error, memberName: nameof (FirmwarePackageTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1000);
          errorless = false;
          autoEvent.Set();
        });
        state1.Cancellation = (CancellationTokenSource) null;
        Task.Factory.StartNew(state1.Session.ApplianceInfoTask, (object) state1);
        autoEvent.WaitOne();
        if (!errorless)
        {
          taskObject.ExceptionCallback($"Failed to upload package:{str} ");
          return;
        }
        ++taskObject.Session.CurrentFlashingStep;
        taskObject.Session.AttemptMessageCallback($"{AppResource.APPLIANCE_FLASH_INSTALLING} {str} ...");
        taskObject.Session.AttemptProgressCallback("", taskObject.Session.CurrentFlashingStep, taskObject.Session.TotalFlashingSteps);
        taskObject.Session.signatureFile = taskObject.Session.GetPackageProperties(mod.Value.moduleid.ToString(), mod.Value.Available.ToString());
        taskObject.Session.LogService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, $"ppf file details: {mod.Value.moduleid.ToString()}.{mod.Value.Available.ToString()}", memberName: nameof (FirmwarePackageTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1016);
        if (!string.IsNullOrEmpty(taskObject.Session.signatureFile))
        {
          taskObject.Session.LogService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Reading from " + Path.GetFileName(taskObject.Session.signatureFile), memberName: nameof (FirmwarePackageTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1020);
          Stream ppfStream = (Stream) new CustomMemoryStream((iService5.Core.Services.Data.progressCallback) (res => { }));
          ppfStream.Write(File.ReadAllBytes(taskObject.Session.signatureFile), 0, (int) new FileInfo(taskObject.Session.signatureFile).Length);
          ppfStream.Flush();
          ppfStream.Position = 0L;
          Serilog.Core.Logger logger6 = taskObject.Session.LogService.getLogger();
          string[] strArray4 = new string[8]
          {
            "Starting task InstallFirmware for module=",
            str,
            "; node=",
            null,
            null,
            null,
            null,
            null
          };
          node = mod.Value.node;
          strArray4[3] = node.ToString();
          strArray4[4] = "; type=";
          strArray4[5] = mod.Value.type;
          strArray4[6] = "; includeInFlashing=";
          strArray4[7] = mod.Value.IncludeInFlashingSequence.ToString();
          string message6 = string.Concat(strArray4);
          logger6.LogAppInformation(LoggingContext.APPLIANCE, message6, memberName: nameof (FirmwarePackageTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1025);
          ApplianceTask<InstallFirmware> state2 = new ApplianceTask<InstallFirmware>();
          autoEvent = new AutoResetEvent(false);
          state2.ContextType = new InstallFirmware(ppfStream);
          state2.TaskCallback = (ApplianceTaskCallback<InstallFirmware>) (taskRet =>
          {
            if (!mod.Value.Recovery)
              taskObject.Session.fwInstalledFlag = true;
            errorless = true;
            taskObject.Session.AttemptMessageCallback($" {AppResource.APPLIANCE_FLASH_DONE}\n");
            mod.Value.State = '✓';
            mod.Value.StateColor = "Green";
            mod.Value.moduleInstalled = true;
            if (mod.Value.Available.IsVersionEqual(mod.Value.Installed))
            {
              mod.Value.UIAvailableLabelDisplay = AppResource.REINSTALLED_TEXT;
              mod.Value.UIAvailableLabelColour = "#007AFF";
            }
            else if (mod.Value.Available > mod.Value.Installed)
            {
              mod.Value.UIAvailableLabelDisplay = AppResource.UP_TO_DATE_TEXT;
              mod.Value.UIAvailableLabelColour = "#007AFF";
            }
            taskObject.Session.AttemptUIUpdateRefresh();
            autoEvent.Set();
          });
          state2.Session = taskObject.Session;
          state2.ExceptionCallback = (ApplianceTaskException) (error =>
          {
            mod.Value.State = 'X';
            mod.Value.StateColor = "Red";
            taskObject.Session.AttemptUIUpdateRefresh();
            taskObject.Session.AttemptMessageCallback($" {AppResource.APPLIANCE_FLASH_FAILED}\n");
            taskObject.Session.LogService.getLogger().LogAppError(LoggingContext.APPLIANCE, "InstallFirmware ExceptionCallback: " + error, memberName: nameof (FirmwarePackageTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1062);
            errorless = false;
            autoEvent.Set();
          });
          state2.Cancellation = (CancellationTokenSource) null;
          Task.Factory.StartNew(state1.Session.ApplianceInfoTask, (object) state2);
          autoEvent.WaitOne();
          if (!errorless)
            taskObject.ExceptionCallback("Failed to install package in InstallFirmware:" + str);
          else
            taskObject.TaskCallback((object) true);
        }
        else
          taskObject.Session.LogService.getLogger().LogAppError(LoggingContext.APPLIANCE, "Signature file couldnt be found for " + mod.Value.moduleid.ToString(), memberName: nameof (FirmwarePackageTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1080);
        if (!errorless)
        {
          taskObject.Session.LogService.getLogger().LogAppError(LoggingContext.APPLIANCE, "Failed to install package:" + str, memberName: nameof (FirmwarePackageTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1084);
          taskObject.ExceptionCallback("Failed to install package:" + str);
        }
        else
          taskObject.TaskCallback((object) true);
      }
      if (!obj.GetType().Equals(typeof (ApplianceTask<EcuUpdate>)))
        return;
      ApplianceTask<EcuUpdate> taskObject1 = (ApplianceTask<EcuUpdate>) obj;
      ApplianceTask<EcuUpdate> state = new ApplianceTask<EcuUpdate>();
      AutoResetEvent autoEvent1 = new AutoResetEvent(false);
      state.ContextType = new EcuUpdate();
      bool errorless1 = true;
      state.TaskCallback = (ApplianceTaskCallback<EcuUpdate>) (taskRet =>
      {
        errorless1 = true;
        taskObject1.Session.AttemptMessageCallback(AppResource.APPLIANCE_INSTALL_REPAIR_POPUP_READY_TO_DISPLAY + "\n");
        taskObject1.Session.AttemptProgressCallback(AppResource.APPLIANCE_INSTALL_REPAIR_REPSONSE_ARRIVED, 1, 1);
        taskObject1.Session.AttemptUIUpdateRefresh();
        taskObject1.Session.LogService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "ECU Programming Successfull task", memberName: nameof (FirmwarePackageTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1107);
        taskObject1.Session.fwInstalledFlag = false;
        taskObject1.Session.spauDetectedFlag = false;
        taskObject1.Session.spauUploadedFlag = false;
        autoEvent1.Set();
      });
      state.Session = taskObject1.Session;
      state.ExceptionCallback = (ApplianceTaskException) (error =>
      {
        taskObject1.Session.AttemptUIUpdateRefresh();
        taskObject1.Session.AttemptMessageCallback(AppResource.APPLIANCE_INSTALL_REPAIR_POPUP_READY_TO_DISPLAY + "\n");
        taskObject1.Session.AttemptProgressCallback(AppResource.APPLIANCE_INSTALL_REPAIR_REPSONSE_ARRIVED, 0, 1);
        taskObject1.Session.AttemptMessageCallback(AppResource.APPLIANCE_ECU_TASK_RESULT_FAIL + "\n");
        taskObject1.Session.LogService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "ECU Programming Failed task", memberName: nameof (FirmwarePackageTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1120);
        taskObject1.Session.LogService.getLogger().LogAppError(LoggingContext.APPLIANCE, error, memberName: nameof (FirmwarePackageTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1121);
        errorless1 = false;
        taskObject1.Session.fwInstalledFlag = false;
        taskObject1.Session.spauDetectedFlag = false;
        taskObject1.Session.spauUploadedFlag = false;
        autoEvent1.Set();
      });
      state.Cancellation = (CancellationTokenSource) null;
      if (taskObject1.Session.fwInstalledFlag)
        Task.Factory.StartNew(state.Session.ApplianceInfoTask, (object) state);
      autoEvent1.WaitOne();
      if (!errorless1)
        taskObject1.ExceptionCallback(AppResource.APPLIANCE_ECU_TASK_RESULT_FAIL);
      else
        taskObject1.TaskCallback((object) true);
    }
    catch (Exception ex)
    {
      ((ApplianceTaskException) obj.GetType().GetProperty("ExceptionCallback").GetValue(obj))(ex.Message);
    }
  });
  internal readonly Action<object> FileSystemTask = (Action<object>) (obj =>
  {
    try
    {
      if (obj.GetType().Equals(typeof (ApplianceTask<Unzip>)))
      {
        ApplianceTask<Unzip> applianceTask = (ApplianceTask<Unzip>) obj;
        if (applianceTask != null)
          applianceTask.TaskCallback((object) null);
        else
          applianceTask.ExceptionCallback("Failed to Unzip files");
      }
      else if (obj.GetType().Equals(typeof (ApplianceTask<iService5.Core.Services.Appliance.Sort>)))
      {
        ApplianceTask<iService5.Core.Services.Appliance.Sort> applianceTask = (ApplianceTask<iService5.Core.Services.Appliance.Sort>) obj;
        if (applianceTask.Session.SortBinariesCpio(applianceTask.ContextType.Mode))
          applianceTask.TaskCallback((object) null);
        else
          applianceTask.ExceptionCallback("Failed to sort files");
      }
      else
      {
        if (!obj.GetType().Equals(typeof (ApplianceTask<ApplySortedFirmwareList>)))
          return;
        ApplianceTask<ApplySortedFirmwareList> taskObject = (ApplianceTask<ApplySortedFirmwareList>) obj;
        taskObject.Session.TotalFlashingSteps = 0;
        taskObject.Session.CurrentFlashingStep = 0;
        bool errorless = true;
        foreach (int key1 in taskObject.Session.SortedFlashingList.Keys)
        {
          foreach (long key2 in taskObject.Session.SortedFlashingList[key1].Keys)
            taskObject.Session.TotalFlashingSteps += 2;
        }
        foreach (int key3 in taskObject.Session.SortedFlashingList.Keys)
        {
          taskObject.Session.AttemptMessageCallback($"{AppResource.APPLIANCE_FLASH_FILES_FOR_NODE} for node {key3.ToString()}\n");
          foreach (long key4 in taskObject.Session.SortedFlashingList[key3].Keys)
          {
            KeyValuePair<string, AugmentedModule> keyValuePair = taskObject.Session.SortedFlashingList[key3][key4];
            bool recovery = keyValuePair.Value.Recovery;
            keyValuePair = taskObject.Session.SortedFlashingList[key3][key4];
            bool flag = keyValuePair.Value.Package.Equals("elp");
            if (recovery | flag)
            {
              ApplianceTask<FirmwareInstallation> state = new ApplianceTask<FirmwareInstallation>();
              AutoResetEvent autoEvent = new AutoResetEvent(false);
              state.ContextType = new FirmwareInstallation(taskObject.Session.SortedFlashingList[key3][key4]);
              taskObject.Session.LogService.getLogger().LogAppError(LoggingContext.APPLIANCE, $"FirmwareInstallation initiated for {key3} and {key4}", memberName: nameof (FileSystemTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1212);
              state.TaskCallback = (ApplianceTaskCallback<FirmwareInstallation>) (response => autoEvent.Set());
              state.Session = taskObject.Session;
              state.ExceptionCallback = (ApplianceTaskException) (error =>
              {
                errorless = false;
                taskObject.Session.LogService.getLogger().LogAppError(LoggingContext.APPLIANCE, "FirmwareInstallation ExceptionCallback: " + error, memberName: nameof (FileSystemTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1221);
                autoEvent.Set();
              });
              state.Cancellation = (CancellationTokenSource) null;
              if (recovery && !taskObject.Session.RecoveryDifferenceDetected)
                autoEvent.Set();
              else
                Task.Factory.StartNew(taskObject.Session.FirmwarePackageTask, (object) state);
              autoEvent.WaitOne();
            }
          }
        }
        foreach (int key5 in taskObject.Session.SortedFlashingList.Keys)
        {
          taskObject.Session.AttemptMessageCallback($"{AppResource.APPLIANCE_FLASH_FILES_FOR_NODE} for node {key5.ToString()}\n");
          foreach (long key6 in taskObject.Session.SortedFlashingList[key5].Keys)
          {
            KeyValuePair<string, AugmentedModule> keyValuePair = taskObject.Session.SortedFlashingList[key5][key6];
            bool recovery = keyValuePair.Value.Recovery;
            keyValuePair = taskObject.Session.SortedFlashingList[key5][key6];
            bool flag = keyValuePair.Value.Package.Equals("elp");
            ApplianceTask<FirmwareInstallation> state = new ApplianceTask<FirmwareInstallation>();
            AutoResetEvent autoEvent = new AutoResetEvent(false);
            state.ContextType = new FirmwareInstallation(taskObject.Session.SortedFlashingList[key5][key6]);
            state.TaskCallback = (ApplianceTaskCallback<FirmwareInstallation>) (response => autoEvent.Set());
            state.Session = taskObject.Session;
            state.ExceptionCallback = (ApplianceTaskException) (error =>
            {
              errorless = false;
              taskObject.Session.LogService.getLogger().LogAppError(LoggingContext.APPLIANCE, error, memberName: nameof (FileSystemTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1252);
              autoEvent.Set();
            });
            state.Cancellation = (CancellationTokenSource) null;
            if (recovery | flag)
              autoEvent.Set();
            else
              Task.Factory.StartNew(taskObject.Session.FirmwarePackageTask, (object) state);
            autoEvent.WaitOne();
          }
        }
        taskObject.Session.LogService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Checking conditions for task ECU Programming", memberName: nameof (FileSystemTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1265);
        ApplianceTask<EcuUpdate> state3 = new ApplianceTask<EcuUpdate>();
        AutoResetEvent autoEvent1 = new AutoResetEvent(false);
        state3.ContextType = new EcuUpdate();
        state3.TaskCallback = (ApplianceTaskCallback<EcuUpdate>) (taskRet =>
        {
          taskObject.Session.LogService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "ECU Programming Successfull task", memberName: nameof (FileSystemTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1271);
          taskObject.Session.installRepairSuccess = true;
          taskObject.Session.AttemptUIUpdateRefresh();
          taskObject.Session.spauUploadedFlag = false;
          taskObject.Session.fwInstalledFlag = false;
          taskObject.Session.spauDetectedFlag = false;
          autoEvent1.Set();
        });
        state3.Session = taskObject.Session;
        state3.ExceptionCallback = (ApplianceTaskException) (error =>
        {
          errorless = false;
          taskObject.Session.LogService.getLogger().LogAppError(LoggingContext.APPLIANCE, error, memberName: nameof (FileSystemTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1283);
          taskObject.Session.LogService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "ECU Programming Failed task", memberName: nameof (FileSystemTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1284);
          taskObject.Session.installRepairSuccess = false;
          taskObject.Session.spauUploadedFlag = false;
          taskObject.Session.fwInstalledFlag = false;
          taskObject.Session.spauDetectedFlag = false;
          autoEvent1.Set();
        });
        state3.Cancellation = (CancellationTokenSource) null;
        taskObject.Session.LogService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "spauDetectedFlag is " + taskObject.Session.spauDetectedFlag.ToString(), memberName: nameof (FileSystemTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1293);
        taskObject.Session.LogService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "currentFWInstallMode is " + taskObject.Session.currentFWInstallMode.ToString(), memberName: nameof (FileSystemTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1294);
        taskObject.Session.LogService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "fwInstalledFlag is " + taskObject.Session.fwInstalledFlag.ToString(), memberName: nameof (FileSystemTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1295);
        taskObject.Session.LogService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "spauUploadedFlag is " + taskObject.Session.spauUploadedFlag.ToString(), memberName: nameof (FileSystemTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1296);
        taskObject.Session.LogService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "emptySMM is " + taskObject.Session.emptySMM.ToString(), memberName: nameof (FileSystemTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1297);
        taskObject.Session.LogService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "isECUUpdateAvailable is " + taskObject.Session.isECUUpdateAvailable.ToString(), memberName: nameof (FileSystemTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1298);
        taskObject.Session.LogService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "InstallRepairFeatureAvailable is " + taskObject.Session.InstallRepairFeatureAvailable.ToString(), memberName: nameof (FileSystemTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1299);
        if (taskObject.Session.currentFWInstallMode != FirmwareInstallMode.Recovery && taskObject.Session.spauDetectedFlag && taskObject.Session.fwInstalledFlag && !taskObject.Session.emptySMM && !taskObject.Session.isECUUpdateAvailable && taskObject.Session.InstallRepairFeatureAvailable && taskObject.Session.spauUploadedFlag)
        {
          Task.Factory.StartNew(taskObject.Session.FirmwarePackageTask, (object) state3);
          taskObject.Session.installRepairExecuted = true;
          autoEvent1.WaitOne();
        }
        else
          taskObject.Session.LogService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Skipped task Install Repair", memberName: nameof (FileSystemTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1309);
        taskObject.TaskCallback((object) errorless);
      }
    }
    catch (Exception ex)
    {
      ((ApplianceTaskException) obj.GetType().GetProperty("ExceptionCallback").GetValue(obj))(ex.Message);
    }
  });

  public Dictionary<int, Dictionary<long, KeyValuePair<string, AugmentedModule>>> SortedFlashingList { get; set; } = new Dictionary<int, Dictionary<long, KeyValuePair<string, AugmentedModule>>>();

  public List<AugmentedModule> ModuleList => this.Modules;

  public List<AugmentedModule> InvalidModuleList { get; set; }

  public bool ElpSupportsCountrySettings { get; internal set; }

  public bool InventoryAvailable { get; internal set; }

  public bool InFlashing { get; internal set; }

  public bool StartedFlashing { get; internal set; }

  public bool AllowedToProceed { get; internal set; }

  public bool Busy { get; internal set; }

  public ProgressCallback AttemptProgressCallback { get; internal set; }

  public FinishCallback AttemptFinishCallback { get; internal set; }

  public MessageCallback AttemptMessageCallback { get; internal set; }

  public UIUpdateCallback AttemptUIUpdateRefresh { get; internal set; }

  public bool ElpSupportsPurging { get; internal set; }

  public bool InstallRepairFeatureAvailable { get; internal set; }

  public bool RecoverySupportsPPF6 { get; internal set; }

  public bool isECUUpdateAvailable { get; internal set; }

  public bool ElpSupportsEcuFlashing { get; internal set; }

  public bool RecoverySupportsHACountrySettings { get; internal set; }

  public bool DowngradeAllSupport { get; internal set; }

  public BootMode CurrentBootMode { get; internal set; }

  public BootMode Bootmode => this.CurrentBootMode;

  public HaInfoDto HaInfo { get; internal set; }

  public HaInfoDto HaInfoFromMetadata { get; set; }

  public Dictionary<string, string> DeviceDictionary => this._deviceDictionary;

  public bool installRepairPopupActive
  {
    get => this._installRepairPopupActive;
    set => this._installRepairPopupActive = value;
  }

  public bool AllowReboot { get; set; }

  public bool RecoveryDifferenceDetected { get; set; }

  public bool CachedInventoryMerged { get; set; }

  public bool LocalInventoryAvailable { get; set; }

  public ApplianceSession(
    IUserSession userSession,
    IAppliance appliance,
    IMetadataService metadataService,
    IPlatformSpecificServiceLocator locator,
    ILoggingService logService,
    IAlertService alertService)
  {
    this.UserSession = userSession;
    this.MetadataService = metadataService;
    this.Modules = new List<AugmentedModule>();
    this.Locator = locator;
    this.LogService = logService;
    this.Alert = alertService;
    this.Reset();
    MessagingCenter.Subscribe<ApplianceFlashViewModel>((object) this, CoreApp.EventsNames.InstallRepairPopupDismissed.ToString(), (Action<ApplianceFlashViewModel>) (async sender =>
    {
      this.installRepairPopupActive = false;
      this.LogService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "installRepairPopup on", memberName: ".ctor", sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 254);
    }), (ApplianceFlashViewModel) null);
    MessagingCenter.Subscribe<ApplianceFlashViewModel>((object) this, CoreApp.EventsNames.InstallRepairPopupRaised.ToString(), (Action<ApplianceFlashViewModel>) (async sender =>
    {
      this.installRepairPopupActive = true;
      this.LogService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "installRepairPopup off", memberName: ".ctor", sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 260);
    }), (ApplianceFlashViewModel) null);
  }

  public void ApplianceIsSecure(RequestCallback cb)
  {
    this.Busy = true;
    Task.Factory.StartNew(this.ApplianceInfoTask, (object) new ApplianceTask<Security>()
    {
      TaskCallback = (ApplianceTaskCallback<Security>) (obj =>
      {
        this.LogService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Callback received: " + (string) obj, memberName: nameof (ApplianceIsSecure), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 269);
        cb(true);
      }),
      Session = this,
      ExceptionCallback = (ApplianceTaskException) (error =>
      {
        this.LogService.getLogger().LogAppError(LoggingContext.APPLIANCE, "Error while callback: " + error, memberName: nameof (ApplianceIsSecure), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 275);
        this.Busy = false;
        cb(false);
      }),
      Cancellation = (CancellationTokenSource) null
    });
  }

  public void ApplySecurity(RequestCallback cb)
  {
    this.Busy = true;
    Task.Factory.StartNew(this.ApplianceInfoTask, (object) new ApplianceTask<iService5.Core.Services.Appliance.ApplySecurity>()
    {
      TaskCallback = (ApplianceTaskCallback<iService5.Core.Services.Appliance.ApplySecurity>) (error =>
      {
        this.LogService.getLogger().LogAppError(LoggingContext.APPLIANCE, $"Task call back error {error}", memberName: nameof (ApplySecurity), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 287);
        cb(true);
      }),
      Session = this,
      ExceptionCallback = (ApplianceTaskException) (error =>
      {
        this.LogService.getLogger().LogAppError(LoggingContext.APPLIANCE, "Exception call back error " + error, memberName: nameof (ApplySecurity), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 293);
        this.Busy = false;
        cb(false);
      }),
      Cancellation = (CancellationTokenSource) null
    });
  }

  public bool CanProceed() => throw new NotImplementedException();

  public void ConnectToAppliance()
  {
    Is5SshWrapper is5SshWrapper = Mvx.IoCProvider.Resolve<Is5SshWrapper>();
    if (is5SshWrapper == null)
      throw new ArgumentNullException();
    is5SshWrapper.IPAddress = this.Locator.GetPlatformSpecificService().GetIp() ?? throw new ArgumentNullException();
    this.checkDbus2ECUUpdateAvailability();
  }

  public void checkDbus2ECUUpdateAvailability()
  {
    this.isECUUpdateAvailable = UtilityFunctions.getSShCommandsStatus(Mvx.IoCProvider.Resolve<Is5SshWrapper>().GetSSHCommands(), "enable-dbus2-ecu-update");
  }

  public bool InitialiseMetadata()
  {
    try
    {
      this.Modules = this.MetadataService.GetSMMModuleWithLabels(this.UserSession.getEnumberSession());
      this.SetHaInfoFromMetadata();
    }
    catch (Exception ex)
    {
      this.LogService.getLogger().LogAppError(LoggingContext.APPLIANCE, ex.Message, ex, nameof (InitialiseMetadata), "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", 336);
      return false;
    }
    return true;
  }

  public void SetHaInfoFromMetadata()
  {
    this.HaInfoFromMetadata = this.MetadataService.getHAInfo(this.UserSession.getEnumberSession());
  }

  public bool InitialiseFeatureTableSupport()
  {
    try
    {
      if (!this.initializedMetadataOnce)
      {
        this.initializedMetadataOnce = true;
        IMetadataService metadataService1 = this.MetadataService;
        SMMFeatures smmFeatures = SMMFeatures.ECUFLASHING;
        string name1 = smmFeatures.ToString();
        this.versionRangeForECUFlashing = metadataService1.GetVersionFromFeatureTable(name1);
        IMetadataService metadataService2 = this.MetadataService;
        smmFeatures = SMMFeatures.INSTALLREPAIR;
        string name2 = smmFeatures.ToString();
        this.versionRangeForInstallRepair = metadataService2.GetVersionFromFeatureTable(name2);
        IMetadataService metadataService3 = this.MetadataService;
        smmFeatures = SMMFeatures.DOWNGRADEALL;
        string name3 = smmFeatures.ToString();
        this.versionRangeForDowngradeAll = metadataService3.GetVersionFromFeatureTable(name3);
        IMetadataService metadataService4 = this.MetadataService;
        smmFeatures = SMMFeatures.PURGING;
        string name4 = smmFeatures.ToString();
        this.versionRangeForPurging = metadataService4.GetVersionFromFeatureTable(name4);
        IMetadataService metadataService5 = this.MetadataService;
        smmFeatures = SMMFeatures.HACOUNTRYSETTINGS;
        string name5 = smmFeatures.ToString();
        this.versionRangeForHACountrySetting = metadataService5.GetVersionFromFeatureTable(name5);
        IMetadataService metadataService6 = this.MetadataService;
        smmFeatures = SMMFeatures.TYPE1CHECK;
        string name6 = smmFeatures.ToString();
        this.versionRangeForType1Check = metadataService6.GetVersionFromFeatureTable(name6);
        IMetadataService metadataService7 = this.MetadataService;
        smmFeatures = SMMFeatures.TYPE3CHECK;
        string name7 = smmFeatures.ToString();
        this.versionRangeForType3Check = metadataService7.GetVersionFromFeatureTable(name7);
        IMetadataService metadataService8 = this.MetadataService;
        smmFeatures = SMMFeatures.TYPE6CHECK;
        string name8 = smmFeatures.ToString();
        this.versionRangeForType6Check = metadataService8.GetVersionFromFeatureTable(name8);
        IMetadataService metadataService9 = this.MetadataService;
        smmFeatures = SMMFeatures.PPF6;
        string name9 = smmFeatures.ToString();
        this.versionRangeForPPF6 = metadataService9.GetVersionFromFeatureTable(name9);
      }
      this.ElpSupportsEcuFlashing = this.IsECUFlashingSupported();
      this.InstallRepairFeatureAvailable = this.IsInstallRepairSupported();
      this.ElpSupportsPurging = this.IsPurgingSupported();
      this.RecoverySupportsPPF6 = this.IsPPF6Supported();
      foreach (AugmentedModule module in this.Modules)
      {
        if (module.node != 0 && module.node != (int) ushort.MaxValue)
        {
          if (module.type == ModuleType.SPAU_FIRMWARE.ToString())
          {
            module.IncludeInFlashingSequence = this.ElpSupportsEcuFlashing;
          }
          else
          {
            module.IncludeInFlashingSequence = false;
            module.IncludeInComparison = false;
          }
        }
        else
          module.IncludeInFlashingSequence = true;
        module.IncludeInComparison = module.IncludeInFlashingSequence;
      }
    }
    catch (Exception ex)
    {
      this.LogService.getLogger().LogAppError(LoggingContext.APPLIANCE, ex.Message, ex, nameof (InitialiseFeatureTableSupport), "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", 391);
      return false;
    }
    return true;
  }

  public void CheckIfPPF6Supported() => this.RecoverySupportsPPF6 = this.IsPPF6Supported();

  public bool RequiresMaintenanceMode()
  {
    return this.Modules.Any<AugmentedModule>((Func<AugmentedModule, bool>) (x => x.Recovery && x.Available.IsMajorMoreOrEqual(16 /*0x10*/)));
  }

  public bool IsBusy() => this.Busy;

  public async Task Reboot(RequestCallback cb)
  {
    this.Busy = true;
    ApplianceTask<iService5.Core.Services.Appliance.Reboot> task = new ApplianceTask<iService5.Core.Services.Appliance.Reboot>();
    task.TaskCallback = (ApplianceTaskCallback<iService5.Core.Services.Appliance.Reboot>) (obj =>
    {
      this.Busy = false;
      cb(true);
    });
    task.Session = this;
    task.ExceptionCallback = (ApplianceTaskException) (error =>
    {
      this.LogService.getLogger().LogAppError(LoggingContext.APPLIANCE, error, memberName: nameof (Reboot), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 423);
      this.Busy = false;
      cb(false);
    });
    task.Cancellation = (CancellationTokenSource) null;
    await Task.Factory.StartNew(this.ApplianceInfoTask, (object) task);
    task = (ApplianceTask<iService5.Core.Services.Appliance.Reboot>) null;
  }

  public void Reset()
  {
    this.HaInfo = (HaInfoDto) null;
    this.Modules.Clear();
    this.InventoryAvailable = false;
    this.CachedInventoryMerged = false;
    this.LocalInventoryAvailable = false;
    this.InFlashing = false;
    this.StartedFlashing = false;
    this.AllowedToProceed = false;
    this.ElpSupportsPurging = false;
    this.ElpSupportsEcuFlashing = false;
    this.RecoverySupportsHACountrySettings = false;
    this.RecoverySupportsPPF6 = false;
    this.CachedInventoryCmdExecutedSuccessfully = false;
    this.InventoryRetrieved = false;
    this.LocalInventoryRetrieved = false;
  }

  internal bool OnlyFW(AugmentedModule aum)
  {
    if (aum.type == ModuleType.FIRMWARE.ToString() && (aum.node == 0 || aum.node == (int) ushort.MaxValue))
      return true;
    this.LogService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, $"Removing from firmware only list: {aum.moduleid.ToString()} ({aum.version}) as it is not FIRMWARE or node does not comply: {aum.type} {aum.node.ToString()}", memberName: nameof (OnlyFW), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 639);
    return false;
  }

  internal bool IsRecoveryDowngradeAllowed()
  {
    return this.Modules.Count<AugmentedModule>((Func<AugmentedModule, bool>) (p => p.IncludeInComparison && p.Recovery && p.Installed > p.Available && p.IsDowngradeAllowed())) > 0;
  }

  internal bool IsELPDowngradeAllowed()
  {
    return this.Modules.Count<AugmentedModule>((Func<AugmentedModule, bool>) (p => p.IncludeInComparison && p.Recovery && p.Installed > p.Available && p.IsDowngradeAllowed())) > 0;
  }

  internal string GetHAValue(HAEnum contextType)
  {
    switch (contextType)
    {
      case HAEnum.brand:
        return this.HaInfoFromMetadata.Brand.ToUpper();
      case HAEnum.ki:
        return this.HaInfoFromMetadata.CustomerIndex;
      case HAEnum.type:
        return this.HaInfoFromMetadata.DeviceType;
      case HAEnum.ts:
        return this.HaInfoFromMetadata.ManufacturingTimeStamp;
      case HAEnum.vib:
        return this.HaInfoFromMetadata.Vib;
      case HAEnum.countrySettings:
        return this.HaInfoFromMetadata.CountrySettings.ToString();
      default:
        return "";
    }
  }

  public void setDisconnectedStatus(bool disconnectedStatus)
  {
    this.isDisconnected = disconnectedStatus;
    this.AttemptProgressCallback = (ProgressCallback) null;
    this.AttemptFinishCallback = (FinishCallback) null;
    this.AttemptMessageCallback = (MessageCallback) null;
    this.AttemptUIUpdateRefresh = (UIUpdateCallback) null;
  }

  public void RetrieveBootMode(RequestCallback cb)
  {
    this.Busy = true;
    Task.Factory.StartNew(this.ApplianceInfoTask, (object) new ApplianceTask<BootMode>()
    {
      TaskCallback = (ApplianceTaskCallback<BootMode>) (obj =>
      {
        this.CurrentBootMode = (BootMode) obj;
        this.Busy = false;
        cb(true);
      }),
      Session = this,
      ExceptionCallback = (ApplianceTaskException) (error =>
      {
        this.LogService.getLogger().LogAppError(LoggingContext.APPLIANCE, error, memberName: nameof (RetrieveBootMode), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1331);
        this.Busy = false;
        cb(false);
      }),
      Cancellation = (CancellationTokenSource) null
    });
  }

  public void RetrieveHAInfo(RequestCallback cb)
  {
    this.Busy = true;
    Task.Factory.StartNew(this.ApplianceInfoTask, (object) new ApplianceTask<HaInfoDto>()
    {
      TaskCallback = (ApplianceTaskCallback<HaInfoDto>) (obj =>
      {
        this.HaInfo = (HaInfoDto) obj;
        this.Busy = false;
        cb(true);
      }),
      Session = this,
      ExceptionCallback = (ApplianceTaskException) (error =>
      {
        this.LogService.getLogger().LogAppError(LoggingContext.APPLIANCE, error, memberName: nameof (RetrieveHAInfo), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1346);
        this.Busy = false;
        cb(false);
      }),
      Cancellation = (CancellationTokenSource) null
    });
  }

  internal void EnableDbusEcuUpdate(RequestCallback cb)
  {
    this.Busy = true;
    Task.Factory.StartNew(this.ApplianceInfoTask, (object) new ApplianceTask<EnableEcuUpdate>()
    {
      TaskCallback = (ApplianceTaskCallback<EnableEcuUpdate>) (obj =>
      {
        this.LogService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "EnableEcuUpdate : succesfully executed", memberName: nameof (EnableDbusEcuUpdate), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1356);
        cb((bool) obj);
      }),
      Session = this,
      ExceptionCallback = (ApplianceTaskException) (error =>
      {
        this.LogService.getLogger().LogAppError(LoggingContext.APPLIANCE, error, memberName: nameof (EnableDbusEcuUpdate), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1363);
        this.Busy = false;
        cb(false);
      }),
      Cancellation = (CancellationTokenSource) null
    });
  }

  public void RetrieveInventory(RequestCallback cb)
  {
    this.Busy = true;
    Task.Factory.StartNew(this.ApplianceInfoTask, (object) new ApplianceTask<InventoryDto>()
    {
      TaskCallback = (ApplianceTaskCallback<InventoryDto>) (obj =>
      {
        if (obj != null)
        {
          this.InventoryInfo = (InventoryDto) obj;
          this.LogService.getLogger().LogAppDebug(LoggingContext.APPLIANCE, "Successfully retrieved inventory: " + JsonConvert.SerializeObject((object) this.InventoryInfo), memberName: nameof (RetrieveInventory), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1378);
          try
          {
            this.MergeInventory();
            this.DowngradeAllSupport = this.IsDowngradeAllSupported();
            this.InitialiseFeatureTableSupport();
          }
          catch (Exception ex)
          {
            this.LogService.getLogger().LogAppError(LoggingContext.APPLIANCE, ex.Message, memberName: nameof (RetrieveInventory), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1387);
            this.LogService.getLogger().LogAppError(LoggingContext.APPLIANCE, ex.StackTrace, memberName: nameof (RetrieveInventory), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1388);
            throw new Exception("Problem with MergeInventory");
          }
          if (this.InventoryRetrieved)
          {
            this.InventoryRetrieved = false;
            this.CachedInventoryMerged = false;
            this.LocalInventoryAvailable = false;
            this.InventoryAvailable = true;
          }
          else if (this.LocalInventoryRetrieved)
          {
            this.LocalInventoryRetrieved = false;
            this.InventoryAvailable = false;
            this.CachedInventoryMerged = false;
            this.LocalInventoryAvailable = true;
          }
          else if (this.CachedInventoryCmdExecutedSuccessfully)
          {
            this.CachedInventoryCmdExecutedSuccessfully = false;
            this.CachedInventoryMerged = true;
          }
        }
        this.Busy = false;
        cb(true);
      }),
      Session = this,
      ExceptionCallback = (ApplianceTaskException) (error =>
      {
        this.LogService.getLogger().LogAppError(LoggingContext.APPLIANCE, error, memberName: nameof (RetrieveInventory), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1417);
        this.Busy = false;
        cb(false);
      }),
      Cancellation = (CancellationTokenSource) null
    });
  }

  public void PurgeAfterComparison()
  {
    this.Busy = true;
    Task.Factory.StartNew(this.ApplianceInfoTask, (object) new ApplianceTask<Purge>()
    {
      TaskCallback = (ApplianceTaskCallback<Purge>) (obj =>
      {
        if ((bool) obj)
          this.CheckAllowedTypes();
        else
          this.Alert.ShowMessageAlertWithKeyFromService("APPLIANCE_FLASH_PROGRAM_NOT_YET", "", (Action) (() =>
          {
            this.Busy = false;
            this.AttemptFinishCallback(isError: true, errorMessage: AppResource.APPLIANCE_FLASH_PROGRAM_NOT_YET);
          }));
      }),
      Session = this,
      ExceptionCallback = (ApplianceTaskException) (error =>
      {
        this.LogService.getLogger().LogAppError(LoggingContext.APPLIANCE, error, memberName: nameof (PurgeAfterComparison), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1448);
        this.Busy = false;
        this.AttemptFinishCallback(isError: true, errorMessage: error);
      }),
      Cancellation = (CancellationTokenSource) null
    });
  }

  internal void CleanPurgedWithoutInventoryUpdate()
  {
    foreach (AugmentedModule module in this.Modules)
    {
      if (!module.Recovery && !module.isECU)
      {
        module.InstalledVersion = "";
        module.Installed = iService5.Core.Services.Data.Version.FromString("0.0.0.0");
        if (module.version == "")
        {
          module.moduleid = 0L;
          module.IncludeInComparison = false;
          module.IncludeInFlashingSequence = false;
          this.LogService.getLogger().LogAppError(LoggingContext.APPLIANCE, $"Excluding module=({module.moduleid.ToString()}) for comparision and flashing due to Purging", memberName: nameof (CleanPurgedWithoutInventoryUpdate), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1468);
        }
      }
    }
    try
    {
      this.Modules.RemoveAll((Predicate<AugmentedModule>) (s => s.moduleid == 0L));
    }
    catch (Exception ex)
    {
      this.LogService.getLogger().LogAppError(LoggingContext.APPLIANCE, ex.Message, memberName: nameof (CleanPurgedWithoutInventoryUpdate), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1479);
    }
    this.AttemptUIUpdateRefresh();
  }

  public void PurgeBeforeProgrammingFlow()
  {
    this.Busy = true;
    Task.Factory.StartNew(this.ApplianceInfoTask, (object) new ApplianceTask<Purge>()
    {
      TaskCallback = (ApplianceTaskCallback<Purge>) (obj =>
      {
        if (!(bool) obj)
        {
          this.Alert.ShowMessageAlertWithKeyFromService("APPLIANCE_FLASH_DELETE_NOT_YET", "", (Action) (() => this.Flash(FirmwareInstallMode.All)));
        }
        else
        {
          try
          {
            if (this.Modules.FirstOrDefault<AugmentedModule>((Func<AugmentedModule, bool>) (p => p.Package == "elp")) != null)
              this.Flash(FirmwareInstallMode.All);
            else if (this.Modules.OrderByDescending<AugmentedModule, bool>((Func<AugmentedModule, bool>) (p => p.type == ModuleType.FIRMWARE.ToString())).FirstOrDefault<AugmentedModule>() != null)
              this.LogService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "fwID is not NULL", memberName: nameof (PurgeBeforeProgrammingFlow), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1515);
          }
          catch (Exception ex)
          {
            this.LogService.getLogger().LogAppError(LoggingContext.APPLIANCE, "Exception while selecting firmware id for elp" + ex.Message, memberName: nameof (PurgeBeforeProgrammingFlow), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1521);
          }
          this.CleanPurgedWithoutInventoryUpdate();
          this.CompareVersions();
          this.Flash(FirmwareInstallMode.All);
        }
      }),
      Session = this,
      ExceptionCallback = (ApplianceTaskException) (error =>
      {
        this.LogService.getLogger().LogAppError(LoggingContext.APPLIANCE, error, memberName: nameof (PurgeBeforeProgrammingFlow), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1531);
        this.Busy = false;
        this.AttemptFinishCallback(isError: true, errorMessage: error);
      }),
      Cancellation = (CancellationTokenSource) null
    });
  }

  internal void MergeFirmwareToMetadata(
    FirmwareItem appliancefirmware,
    bool isSMM,
    bool isRecovery)
  {
    long firmwareId = appliancefirmware.firmwareId;
    if (!isSMM)
    {
      List<AugmentedModule> list = this.Modules.Where<AugmentedModule>((Func<AugmentedModule, bool>) (x => x.type == ModuleType.SPAU_FIRMWARE.ToString() && x.moduleid == appliancefirmware.firmwareId)).ToList<AugmentedModule>();
      if (list == null || list.Count <= 0)
        return;
      foreach (AugmentedModule augmentedModule in list)
        augmentedModule.FirmwareInEcuVersion = appliancefirmware.version.ToString();
    }
    else
    {
      bool flag = this.Modules.Any<AugmentedModule>((Func<AugmentedModule, bool>) (item => item.moduleid == firmwareId && item.type == ModuleType.FIRMWARE.ToString()));
      if (flag)
        this.LogService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, $"Firmware {firmwareId.ToString()} found in db", memberName: nameof (MergeFirmwareToMetadata), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1558);
      else
        this.LogService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, $"Firmware {firmwareId.ToString()} NOT found in db", memberName: nameof (MergeFirmwareToMetadata), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1562);
      foreach (AugmentedModule augmentedModule in this.Modules.Where<AugmentedModule>((Func<AugmentedModule, bool>) (x => x.type == ModuleType.FIRMWARE.ToString())))
      {
        if (augmentedModule.moduleid == appliancefirmware.firmwareId)
        {
          augmentedModule.InstalledVersion = appliancefirmware.version.ToString();
          augmentedModule.InSMM = true;
          try
          {
            augmentedModule.Installed = iService5.Core.Services.Data.Version.FromString(augmentedModule.InstalledVersion);
            return;
          }
          catch (Exception ex)
          {
            this.LogService.getLogger().LogAppError(LoggingContext.APPLIANCE, ex.Message, memberName: nameof (MergeFirmwareToMetadata), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1576);
            this.LogService.getLogger().LogAppError(LoggingContext.APPLIANCE, augmentedModule.InstalledVersion, memberName: nameof (MergeFirmwareToMetadata), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1577);
            this.LogService.getLogger().LogAppError(LoggingContext.APPLIANCE, augmentedModule.FirmwareName, memberName: nameof (MergeFirmwareToMetadata), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1578);
            this.LogService.getLogger().LogAppError(LoggingContext.APPLIANCE, augmentedModule.moduleid.ToString(), memberName: nameof (MergeFirmwareToMetadata), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1579);
            augmentedModule.Installed = iService5.Core.Services.Data.Version.FromString("0.0.0.0");
            return;
          }
        }
      }
      AugmentedModule augmentedModule1 = new AugmentedModule();
      augmentedModule1.moduleid = appliancefirmware.firmwareId;
      augmentedModule1.moduleInstalled = true;
      augmentedModule1.InstalledVersion = appliancefirmware.version.ToString();
      augmentedModule1.Installed = iService5.Core.Services.Data.Version.FromString(augmentedModule1.InstalledVersion);
      augmentedModule1.Recovery = isRecovery;
      augmentedModule1.IncludeInComparison = false;
      augmentedModule1.Available = iService5.Core.Services.Data.Version.FromString("-1");
      augmentedModule1.AvailableLabelDisplay = "Not Available";
      augmentedModule1.IncludeInFlashingSequence = false;
      augmentedModule1.isECU = false;
      augmentedModule1.hasCorrespondent = new bool?(flag);
      augmentedModule1.InSMM = true;
      this.Modules.Add(augmentedModule1);
      string firmwareName = this.MetadataService.getFirmwareName(appliancefirmware.firmwareId.ToString(), appliancefirmware.version, ModuleType.FIRMWARE.ToString());
      if (firmwareName == "Unknown")
        augmentedModule1.name = "Unknown " + augmentedModule1.moduleid.ToString();
      else
        augmentedModule1.name = this.MetadataService.getMessageWithShortTextMapping(firmwareName);
      this.LogService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, $"Inventory firmware item : {appliancefirmware.firmwareId.ToString()} added to module list", memberName: nameof (MergeFirmwareToMetadata), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1609);
    }
  }

  internal void MergeSPAUToMetadata(BackUpFirmwareItem spau, bool isSMM)
  {
    long spauId = spau.firmwareId;
    bool flag = this.Modules.Any<AugmentedModule>((Func<AugmentedModule, bool>) (item => item.moduleid == spauId && item.type == ModuleType.SPAU_FIRMWARE.ToString()));
    this.LogService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Merging Inventory for backup firmware: " + spauId.ToString(), memberName: nameof (MergeSPAUToMetadata), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1616);
    if (flag)
      this.LogService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, $"Backup firmware {spauId.ToString()} found in db", memberName: nameof (MergeSPAUToMetadata), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1619);
    else
      this.LogService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, $"Backup firmware {spauId.ToString()} NOT found in db", memberName: nameof (MergeSPAUToMetadata), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1623);
    this.LogService.getLogger().Information<BackUpFirmwareItem>("Details for backup firmware: {@Appliancefirmware}", spau);
    foreach (AugmentedModule augmentedModule in this.Modules.Where<AugmentedModule>((Func<AugmentedModule, bool>) (x => x.type == ModuleType.SPAU_FIRMWARE.ToString())))
    {
      if (augmentedModule.moduleid == spau.firmwareId)
      {
        augmentedModule.InstalledVersion = spau.version.ToString();
        try
        {
          augmentedModule.Installed = iService5.Core.Services.Data.Version.FromString(augmentedModule.InstalledVersion);
        }
        catch (Exception ex)
        {
          this.LogService.getLogger().LogAppError(LoggingContext.APPLIANCE, ex.Message, memberName: nameof (MergeSPAUToMetadata), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1637);
          this.LogService.getLogger().LogAppError(LoggingContext.APPLIANCE, augmentedModule.InstalledVersion, memberName: nameof (MergeSPAUToMetadata), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1638);
          this.LogService.getLogger().LogAppError(LoggingContext.APPLIANCE, augmentedModule.FirmwareName, memberName: nameof (MergeSPAUToMetadata), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1639);
          this.LogService.getLogger().LogAppError(LoggingContext.APPLIANCE, augmentedModule.moduleid.ToString(), memberName: nameof (MergeSPAUToMetadata), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1640);
          augmentedModule.Installed = iService5.Core.Services.Data.Version.FromString("0.0.0.0");
        }
        augmentedModule.InSMM = isSMM;
        augmentedModule.name = spau.nodename;
        return;
      }
    }
    AugmentedModule augmentedModule1 = new AugmentedModule();
    augmentedModule1.moduleid = spau.firmwareId;
    augmentedModule1.moduleInstalled = true;
    augmentedModule1.InstalledVersion = spau.version.ToString();
    augmentedModule1.Installed = iService5.Core.Services.Data.Version.FromString(augmentedModule1.InstalledVersion);
    augmentedModule1.Recovery = false;
    augmentedModule1.IncludeInComparison = false;
    augmentedModule1.Available = iService5.Core.Services.Data.Version.FromString("-1");
    augmentedModule1.AvailableLabelDisplay = "Not Available";
    augmentedModule1.IncludeInFlashingSequence = false;
    augmentedModule1.isECU = false;
    augmentedModule1.hasCorrespondent = new bool?(flag);
    augmentedModule1.InSMM = isSMM;
    this.Modules.Add(augmentedModule1);
    IMetadataService metadataService = this.MetadataService;
    long num = spau.firmwareId;
    string moduleid = num.ToString();
    iService5.Ssh.DTO.Version version = spau.version;
    string type = ModuleType.SPAU_FIRMWARE.ToString();
    string firmwareName = metadataService.getFirmwareName(moduleid, version, type);
    if (firmwareName == "Unknown")
    {
      AugmentedModule augmentedModule2 = augmentedModule1;
      num = augmentedModule1.moduleid;
      string str = "Unknown " + num.ToString();
      augmentedModule2.name = str;
    }
    else
      augmentedModule1.name = this.MetadataService.getMessageWithShortTextMapping(firmwareName);
    Serilog.Core.Logger logger = this.LogService.getLogger();
    num = spau.firmwareId;
    string message = $"Inventory backup firmware item : {num.ToString()} added to module list";
    logger.LogAppInformation(LoggingContext.APPLIANCE, message, memberName: nameof (MergeSPAUToMetadata), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1672);
  }

  private void MergeInventory()
  {
    if (this.InventoryInfo == null)
      throw new ArgumentNullException("Inventory is null");
    if (this.InventoryInfo.ECUs == null)
      throw new ArgumentNullException("ECUs list in Inventory is null");
    if (this.InventoryInfo.ECUs.Count <= 0)
      throw new ArgumentException("Number of ECUs in Inventory is not greater than zero");
    bool isRecovery = this.InventoryInfo.ECUs.Count<ECU>((Func<ECU, bool>) (p =>
    {
      if (p.name != null && !(p.name.ToUpper() == "SMM"))
        return false;
      List<FirmwareItem> firmware = p.firmware;
      // ISSUE: explicit non-virtual call
      return firmware != null && (firmware.Count) == 1;
    })) == 1;
    this.LogService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "OnlyRecovery=" + isRecovery.ToString(), memberName: nameof (MergeInventory), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1683);
    foreach (ECU ecU in this.InventoryInfo.ECUs)
    {
      bool isSMM = ecU.name != null && ecU.name.ToUpper() == "SMM";
      if (!isSMM || isSMM && (this.InventoryRetrieved || this.LocalInventoryRetrieved))
      {
        if (ecU.firmware != null)
        {
          if (isSMM && ecU.firmware.Count > 0)
          {
            List<BackUpFirmwareItem> backUpFirmware = ecU.backUpFirmware;
            int num;
            // ISSUE: explicit non-virtual call
            if ((backUpFirmware != null ? ( (backUpFirmware.Count) > 0 ? 1 : 0) : 0) == 0)
            {
              List<FirmwareItem> firmware = ecU.firmware;
              // ISSUE: explicit non-virtual call
              if ((firmware != null ? ((firmware.Count) == 1 ? 1 : 0) : 0) != 0)
              {
                num = this.MetadataService.CheckRecovery(ecU.firmware.First<FirmwareItem>().firmwareId) ? 1 : 0;
                goto label_12;
              }
            }
            num = 0;
label_12:
            this.emptySMM = num != 0;
          }
          foreach (FirmwareItem appliancefirmware in ecU.firmware)
            this.MergeFirmwareToMetadata(appliancefirmware, isSMM, isRecovery);
        }
        if (ecU.backUpFirmware != null)
        {
          foreach (BackUpFirmwareItem spau in ecU.backUpFirmware)
          {
            try
            {
              spau.nodename = UtilityFunctions.GetProperSpauNaming(this.UserSession.getEnumberSession(), spau.firmwareId, this.MetadataService, (IApplianceSession) this, this.LogService);
            }
            catch (Exception ex)
            {
              this.LogService.getLogger().LogAppError(LoggingContext.APPLIANCE, $"Spau naming error : {ex.Message} - Display ModuleID instead", memberName: nameof (MergeInventory), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1709);
              spau.nodename = spau.firmwareId.ToString();
            }
            this.MergeSPAUToMetadata(spau, isSMM);
          }
        }
      }
    }
  }

  public void SetMode(BootMode mode, RequestCallback cb)
  {
    this.Busy = true;
    Task.Factory.StartNew(this.ApplianceInfoTask, (object) new ApplianceTask<BootModeChange>()
    {
      ContextType = new BootModeChange(mode),
      TaskCallback = (ApplianceTaskCallback<BootModeChange>) (obj =>
      {
        this.Busy = false;
        cb(true);
      }),
      Session = this,
      ExceptionCallback = (ApplianceTaskException) (error =>
      {
        this.LogService.getLogger().LogAppError(LoggingContext.APPLIANCE, error, memberName: nameof (SetMode), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1746);
        this.Busy = false;
        cb(false);
      }),
      Cancellation = (CancellationTokenSource) null
    });
  }

  public void UpdateApplianceFirmware(
    UIUpdateCallback ucb,
    MessageCallback mcb,
    ProgressCallback pcb,
    FinishCallback fcb)
  {
    this.AttemptProgressCallback = pcb;
    this.AttemptFinishCallback = fcb;
    this.AttemptMessageCallback = mcb;
    this.AttemptUIUpdateRefresh = ucb;
    this.Busy = true;
    this.RecoverySupportsHACountrySettings = this.IsHACountrySettingsSupported();
    this.ApplyHAInfo();
    this.CompareFirmwareIDs();
  }

  public void ClearInstalled()
  {
    foreach (AugmentedModule module in this.ModuleList)
      module.clearInstalled();
  }

  public void CheckAllowedTypes()
  {
    if (this.IsAllowedTypes())
    {
      this.Type1Check();
    }
    else
    {
      this.LogService.getLogger().LogAppError(LoggingContext.APPLIANCE, "Check Allowed Types: Modules of type 1,3,6 not found", memberName: nameof (CheckAllowedTypes), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1778);
      this.ShowPopupPackageNotValid(this.invalidModules);
    }
  }

  public bool IsAllowedTypes()
  {
    this.invalidModules.Clear();
    foreach (AugmentedModule augmentedModule in this.Modules.Where<AugmentedModule>((Func<AugmentedModule, bool>) (m => !m.moduleInstalled)))
    {
      string type1 = augmentedModule.type;
      ModuleType moduleType = ModuleType.FIRMWARE;
      string str1 = moduleType.ToString();
      int num;
      if (!(type1 == str1))
      {
        string type2 = augmentedModule.type;
        moduleType = ModuleType.INITIAL_CONTENT;
        string str2 = moduleType.ToString();
        if (!(type2 == str2))
        {
          string type3 = augmentedModule.type;
          moduleType = ModuleType.SPAU_FIRMWARE;
          string str3 = moduleType.ToString();
          num = !(type3 == str3) ? 1 : 0;
          goto label_6;
        }
      }
      num = 0;
label_6:
      if (num != 0)
        this.invalidModules.Add(augmentedModule);
    }
    return this.invalidModules.Count <= 0;
  }

  public void Type1Check()
  {
    if (this.IsType1CheckSupported())
      this.CheckRecoveryID();
    else
      this.Type3Check();
  }

  public void CheckRecoveryID()
  {
    if (this.IsRecoveryIDCheckOk())
    {
      this.CheckELPID();
    }
    else
    {
      this.LogService.getLogger().LogAppError(LoggingContext.APPLIANCE, "Check Recovery ID: Recovery-module not available or invalid", memberName: nameof (CheckRecoveryID), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1819);
      this.LogService.getLogger().LogAppError(LoggingContext.APPLIANCE, "Number of invalidModules: " + this.invalidModules.Count.ToString(), memberName: nameof (CheckRecoveryID), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1820);
      this.ShowPopupPackageNotValid(this.invalidModules, "No recovery module");
      foreach (AugmentedModule invalidModule in this.invalidModules)
        this.LogService.getLogger().Information<AugmentedModule>(" invalidModule : {@M}", invalidModule);
    }
  }

  public bool IsRecoveryIDCheckOk()
  {
    if (this.Modules.Count<AugmentedModule>((Func<AugmentedModule, bool>) (p =>
    {
      if (!p.Recovery || !(p.type == ModuleType.FIRMWARE.ToString()))
        return false;
      return p.node == 0 || p.node == (int) ushort.MaxValue;
    })) > 0)
      return true;
    this.invalidModules.Clear();
    this.invalidModules = this.Modules.Where<AugmentedModule>((Func<AugmentedModule, bool>) (module => module.Recovery)).ToList<AugmentedModule>();
    if (this.invalidModules.Count > 0)
      this.LogService.getLogger().LogAppError(LoggingContext.APPLIANCE, "Invalid recovery modules found : " + this.invalidModules.Count.ToString(), memberName: nameof (IsRecoveryIDCheckOk), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1839);
    return false;
  }

  public void CheckELPID()
  {
    if (this.IsELPIDCheckOk())
    {
      this.CheckFirmwareTypeIDsNodes();
    }
    else
    {
      this.LogService.getLogger().LogAppError(LoggingContext.APPLIANCE, "Check ECP ID: ELP-module not available or invalid", memberName: nameof (CheckELPID), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1852);
      this.ShowPopupPackageNotValid(this.invalidModules);
    }
  }

  public bool IsELPIDCheckOk()
  {
    if (this.Modules.Count<AugmentedModule>((Func<AugmentedModule, bool>) (p =>
    {
      if (!p.Recovery || !(p.type == ModuleType.FIRMWARE.ToString()))
        return false;
      return p.node == 0 || p.node == (int) ushort.MaxValue;
    })) > 0)
    {
      this.LogService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Is ELP ID ok: ELP-module available", memberName: nameof (IsELPIDCheckOk), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1860);
      return true;
    }
    this.invalidModules.Clear();
    this.invalidModules = this.Modules.Where<AugmentedModule>((Func<AugmentedModule, bool>) (module => module.Package == "elp")).ToList<AugmentedModule>();
    this.LogService.getLogger().LogAppError(LoggingContext.APPLIANCE, $"Is ELP ID ok: Found {this.invalidModules.Count.ToString()} as invalid", memberName: nameof (IsELPIDCheckOk), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1867);
    foreach (AugmentedModule invalidModule in this.invalidModules)
      this.LogService.getLogger().LogAppError(LoggingContext.APPLIANCE, $"Found {invalidModule.FirmwareName} as invalid", memberName: nameof (IsELPIDCheckOk), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1870);
    return false;
  }

  public void ShowPopupPackageNotValid(List<AugmentedModule> invalidModules, string _s = "")
  {
    string modulesIDs = "";
    foreach (AugmentedModule invalidModule in invalidModules)
      modulesIDs = $"{modulesIDs}ID {invalidModule.moduleid.ToString()}\n";
    this.LogService.getLogger().LogAppInformation(LoggingContext.USERSESSION, $"Service alert popup appeared with message: {this.Alert.GetEnValue("APPLIANCE_FLASH_SOFTWARE_PACKAGE_NOT_VALID")}\n{modulesIDs}", memberName: nameof (ShowPopupPackageNotValid), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1880);
    this.Alert.ShowMessageAlertWithMessageNoLog($"{AppResource.APPLIANCE_FLASH_SOFTWARE_PACKAGE_NOT_VALID}\n{modulesIDs}{_s}", "", (Action) (() =>
    {
      this.Busy = false;
      this.AttemptFinishCallback(isError: true, errorMessage: $"{AppResource.APPLIANCE_FLASH_SOFTWARE_PACKAGE_NOT_VALID}\n{modulesIDs}{_s}");
    }));
  }

  public void CheckFirmwareTypeIDsNodes()
  {
    if (this.IsTypeFirmwareIDsCheckOk())
    {
      this.CheckEveryIDforAdditionalNode();
    }
    else
    {
      foreach (AugmentedModule invalidModule in this.invalidModules)
      {
        AugmentedModule module = invalidModule;
        module.IncludeInComparison = false;
        module.IncludeInFlashingSequence = false;
        this.LogService.getLogger().LogAppInformation(LoggingContext.USERSESSION, $"Excluding module=({module.moduleid.ToString()}) from comparison and flashing, as node is invalid and feature node-check is enabled", memberName: nameof (CheckFirmwareTypeIDsNodes), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1906);
        int index = this.Modules.FindIndex((Predicate<AugmentedModule>) (c => c.moduleid == module.moduleid && c.node != 0 && c.node != (int) ushort.MaxValue));
        this.Modules[index].IncludeInComparison = false;
        this.Modules[index].IncludeInFlashingSequence = false;
      }
      this.InvalidModuleList = this.invalidModules;
      this.Type3Check();
    }
  }

  public bool IsTypeFirmwareIDsCheckOk()
  {
    if (this.Modules.Count<AugmentedModule>((Func<AugmentedModule, bool>) (p =>
    {
      if (!(p.type == ModuleType.FIRMWARE.ToString()))
        return false;
      return p.node == 0 || p.node == (int) ushort.MaxValue;
    })) == this.Modules.Count<AugmentedModule>((Func<AugmentedModule, bool>) (p => p.type == ModuleType.FIRMWARE.ToString())))
      return true;
    this.invalidModules.Clear();
    this.invalidModules = this.Modules.Where<AugmentedModule>((Func<AugmentedModule, bool>) (p => p.type == ModuleType.FIRMWARE.ToString() && p.node != 0 && p.node != (int) ushort.MaxValue)).ToList<AugmentedModule>();
    return false;
  }

  public void CheckEveryIDforAdditionalNode()
  {
    if (!this.IsAdditionalNodeCheckOk())
    {
      if (this.Modules.Where<AugmentedModule>((Func<AugmentedModule, bool>) (m => m.type == ModuleType.SPAU_FIRMWARE.ToString())).ToList<AugmentedModule>().Where<AugmentedModule>((Func<AugmentedModule, bool>) (e => this.invalidModules.All<AugmentedModule>((Func<AugmentedModule, bool>) (d => d.moduleid == e.moduleid)))).ToList<AugmentedModule>().Count == 0)
      {
        foreach (AugmentedModule invalidModule in this.invalidModules)
          this.Modules.Remove(invalidModule);
      }
      else
        this.Type3Check();
    }
    else
      this.Type3Check();
  }

  public bool IsAdditionalNodeCheckOk()
  {
    this.invalidModules.Clear();
    this.invalidModules = this.Modules.Where<AugmentedModule>((Func<AugmentedModule, bool>) (p => this.Modules.Any<AugmentedModule>((Func<AugmentedModule, bool>) (i => p.moduleid == i.moduleid && p.node != i.node)))).ToList<AugmentedModule>();
    return this.invalidModules.Count <= 0;
  }

  public void Type3Check()
  {
    if (this.IsType3CheckSupported())
      this.CheckContentTypeIDNodes();
    else
      this.ECUFeatureCheck();
  }

  private void CheckContentTypeIDNodes()
  {
    this.invalidModules = this.Modules.Where<AugmentedModule>((Func<AugmentedModule, bool>) (p => p.type == ModuleType.INITIAL_CONTENT.ToString() && p.node != 0 && p.node != (int) ushort.MaxValue)).ToList<AugmentedModule>();
    if (this.invalidModules.Count == 0)
    {
      this.ECUFeatureCheck();
    }
    else
    {
      this.LogService.getLogger().LogAppError(LoggingContext.APPLIANCE, "Found content module with invalid node", memberName: nameof (CheckContentTypeIDNodes), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 1989);
      this.ShowPopupPackageNotValid(this.invalidModules);
    }
  }

  public void ECUFeatureCheck()
  {
    if (this.IsECUFlashingSupported())
      this.Type6Check();
    else
      this.CompareVersions();
  }

  public void Type6Check()
  {
    if (this.IsType6CheckSupported())
      this.CheckTypeSPAUIDs();
    else
      this.CompareVersions();
  }

  public void CheckTypeSPAUIDs()
  {
    if (this.IsTypeSPAUIDsCheckOk())
    {
      this.CompareVersions();
    }
    else
    {
      this.LogService.getLogger().LogAppError(LoggingContext.APPLIANCE, "Check Type for SPAU: Modules invalid", memberName: nameof (CheckTypeSPAUIDs), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 2023);
      this.ShowPopupPackageNotValid(this.invalidModules);
    }
  }

  public bool IsTypeSPAUIDsCheckOk()
  {
    if (this.Modules.Count<AugmentedModule>((Func<AugmentedModule, bool>) (p =>
    {
      if (!(p.type == ModuleType.SPAU_FIRMWARE.ToString()))
        return false;
      return p.node == 0 || p.node == (int) ushort.MaxValue;
    })) == this.Modules.Count<AugmentedModule>((Func<AugmentedModule, bool>) (p => p.type == ModuleType.SPAU_FIRMWARE.ToString())))
      return true;
    this.invalidModules.Clear();
    this.invalidModules = this.Modules.Where<AugmentedModule>((Func<AugmentedModule, bool>) (p => p.type == ModuleType.SPAU_FIRMWARE.ToString() && p.node != 0 && p.node != (int) ushort.MaxValue)).ToList<AugmentedModule>();
    return false;
  }

  public void CompareFirmwareIDs()
  {
    Task.Factory.StartNew(this.ComparisonTask, (object) new ApplianceTask<FirmwareIDComparisonDecision>()
    {
      TaskCallback = (ApplianceTaskCallback<FirmwareIDComparisonDecision>) (obj =>
      {
        switch ((FirmwareIDComparisonDecision) obj)
        {
          case FirmwareIDComparisonDecision.Proceed:
            this.CheckAllowedTypes();
            break;
          case FirmwareIDComparisonDecision.Purge:
            if (this.ElpSupportsPurging)
            {
              this.PurgeOnly();
              break;
            }
            this.LogService.getLogger().LogAppError(LoggingContext.APPLIANCE, "Purging needed, but disabled. Programming not allowed", memberName: nameof (CompareFirmwareIDs), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 2056);
            this.Alert.ShowMessageAlertWithKeyFromService("APPLIANCE_FLASH_PROGRAM_NOT_YET", "", (Action) (() =>
            {
              this.Busy = false;
              this.AttemptFinishCallback(isError: true, errorMessage: AppResource.APPLIANCE_FLASH_PROGRAM_NOT_YET);
            }));
            break;
          case FirmwareIDComparisonDecision.MaterialComparison:
            if ($"{this.HaInfo.Vib}/{this.HaInfo.CustomerIndex}" == this.UserSession.getEnumberSession())
            {
              this.Alert.ShowMessageAlertWithKeyFromService("APPLIANCE_FLASH_DATA_SUPPLY_PROBLEM", "", (Action) (() =>
              {
                this.Busy = false;
                this.AttemptFinishCallback(isError: true, errorMessage: AppResource.APPLIANCE_FLASH_DATA_SUPPLY_PROBLEM);
              }));
              break;
            }
            this.PurgeAfterComparison();
            break;
          case FirmwareIDComparisonDecision.Abort:
            this.Alert.ShowMessageAlertWithKeyFromService("APPLIANCE_FLASH_DATA_SUPPLY_OUTDATED", "", (Action) (() =>
            {
              this.Busy = false;
              this.AttemptFinishCallback(isError: true, errorMessage: AppResource.APPLIANCE_FLASH_DATA_SUPPLY_OUTDATED);
            }));
            break;
        }
      }),
      ExceptionCallback = (ApplianceTaskException) (error =>
      {
        this.LogService.getLogger().LogAppError(LoggingContext.APPLIANCE, error, memberName: nameof (CompareFirmwareIDs), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 2097);
        this.Alert.ShowMessageAlertWithKeyFromService("APPLIANCE_FLASH_DATA_SUPPLY_OUTDATED", "", (Action) (() =>
        {
          this.Busy = false;
          this.AttemptFinishCallback(isError: true, errorMessage: AppResource.APPLIANCE_FLASH_DATA_SUPPLY_OUTDATED);
        }));
      }),
      Session = this,
      Cancellation = (CancellationTokenSource) null
    });
  }

  private void PurgeOnly()
  {
    this.Busy = true;
    Task.Factory.StartNew(this.ApplianceInfoTask, (object) new ApplianceTask<Purge>()
    {
      TaskCallback = (ApplianceTaskCallback<Purge>) (obj =>
      {
        if (!(bool) obj)
          this.Alert.ShowMessageAlertWithKeyFromService("APPLIANCE_FLASH_DELETE_NOT_YET", "", (Action) (() =>
          {
            this.Busy = false;
            this.AttemptFinishCallback(isError: true, errorMessage: AppResource.APPLIANCE_FLASH_DELETE_NOT_YET);
          }));
        else
          this.CheckAllowedTypes();
      }),
      Session = this,
      ExceptionCallback = (ApplianceTaskException) (error =>
      {
        this.LogService.getLogger().LogAppError(LoggingContext.APPLIANCE, error, memberName: nameof (PurgeOnly), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 2131);
        this.Busy = false;
        this.AttemptFinishCallback(isError: true, errorMessage: error);
      }),
      Cancellation = (CancellationTokenSource) null
    });
  }

  public void CompareVersions()
  {
    Task.Factory.StartNew(this.ComparisonTask, (object) new ApplianceTask<FirmwareVersionComparisonDecision>()
    {
      TaskCallback = (ApplianceTaskCallback<FirmwareVersionComparisonDecision>) (obj =>
      {
        switch ((FirmwareVersionComparisonDecision) obj)
        {
          case FirmwareVersionComparisonDecision.FlashRecovery:
            this.Flash(FirmwareInstallMode.Recovery);
            break;
          case FirmwareVersionComparisonDecision.AskForRepeat:
            this.Alert.ShowMessageAlertWithKeyFromService("APPLIANCE_FLASH_ALREADY_UPD", "", AppResource.APPLIANCE_FLASH_RETURN, AppResource.APPLIANCE_FLASH_ALREADY_ANYWAY, (Action<bool>) (userReply =>
            {
              try
              {
                this.LogService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "userReply in AskForRepeat: " + userReply.ToString(), memberName: nameof (CompareVersions), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 2199);
                if (userReply)
                {
                  this.Busy = false;
                  this.AttemptFinishCallback(errorMessage: "InventoryUpToDate");
                }
                else if (this.ElpSupportsPurging)
                {
                  this.PurgeAndProgram();
                }
                else
                {
                  this.isReprogram = true;
                  this.Flash(FirmwareInstallMode.Recovery);
                }
              }
              catch (Exception ex)
              {
                this.LogService.getLogger().LogAppError(LoggingContext.APPLIANCE, "AskForRepeat UserAction is failed" + ex.Message, memberName: nameof (CompareVersions), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 2217);
                this.Busy = false;
                this.AttemptFinishCallback(errorMessage: "InventoryUpToDate");
              }
            }));
            break;
          case FirmwareVersionComparisonDecision.AskForNewAll:
            this.Alert.ShowMessageAlertWithKeyFromService("APPLIANCE_FLASH_PROGRAM_NEW_ALL", "", AppResource.APPLIANCE_FLASH_PROGRAM_NEW, AppResource.APPLIANCE_FLASH_PROGRAM_ALL, (Action<bool>) (userReply =>
            {
              try
              {
                this.LogService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "userReply: " + userReply.ToString(), memberName: nameof (CompareVersions), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 2162);
                if (userReply)
                  this.Flash(FirmwareInstallMode.New);
                else if (this.ElpSupportsPurging)
                  this.PurgeAndProgram();
                else
                  this.Flash(FirmwareInstallMode.All);
              }
              catch (Exception ex)
              {
                this.LogService.getLogger().LogAppError(LoggingContext.APPLIANCE, "AskForNewAll UserAction is failed" + ex.Message, memberName: nameof (CompareVersions), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 2181);
                this.Busy = false;
                this.AttemptFinishCallback(isError: true, errorMessage: AppResource.APPLIANCE_FLASH_FAILURES);
              }
            }));
            break;
          case FirmwareVersionComparisonDecision.AllNeeded:
            this.Flash(FirmwareInstallMode.All);
            break;
          case FirmwareVersionComparisonDecision.NotWhitelisted:
            this.Alert.ShowMessageAlertWithKeyFromService("APPLIANCE_FLASH_DATA_NOT_WHITELISTED", "", (Action) (() =>
            {
              this.Busy = false;
              this.AttemptFinishCallback(isError: true, errorMessage: AppResource.APPLIANCE_FLASH_DATA_NOT_WHITELISTED);
            }));
            break;
          case FirmwareVersionComparisonDecision.NewNeeded:
            this.Flash(FirmwareInstallMode.New);
            break;
          case FirmwareVersionComparisonDecision.Abort:
            this.Alert.ShowMessageAlertWithKeyFromService("APPLIANCE_FLASH_DATA_SUPPLY_OUTDATED", "", (Action) (() =>
            {
              this.Busy = false;
              this.AttemptFinishCallback(isError: true, errorMessage: AppResource.APPLIANCE_FLASH_DATA_SUPPLY_OUTDATED);
            }));
            break;
        }
      }),
      ExceptionCallback = (ApplianceTaskException) (error =>
      {
        this.LogService.getLogger().LogAppError(LoggingContext.APPLIANCE, error, memberName: nameof (CompareVersions), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 2251);
        this.Busy = false;
        this.AttemptFinishCallback(isError: true, errorMessage: error);
      }),
      Session = this,
      Cancellation = (CancellationTokenSource) null
    });
  }

  private void ModalBox(string resourceKey)
  {
    AutoResetEvent autoEvent = new AutoResetEvent(false);
    autoEvent.Reset();
    this.Alert.ShowMessageAlertWithKeyFromService(resourceKey, "", (Action) (() => autoEvent.Set()));
    autoEvent.WaitOne();
  }

  internal void Flash(FirmwareInstallMode selection)
  {
    this.currentFWInstallMode = selection;
    this.Busy = true;
    ApplianceTask<Unzip> state = new ApplianceTask<Unzip>();
    state.ContextType = new Unzip(selection);
    this.LogService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Flash mode: " + selection.ToString(), memberName: nameof (Flash), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 2277);
    state.TaskCallback = (ApplianceTaskCallback<Unzip>) (obj => this.Sort(selection));
    state.Session = this;
    state.ExceptionCallback = (ApplianceTaskException) (error =>
    {
      this.LogService.getLogger().LogAppError(LoggingContext.BINARY, error, memberName: nameof (Flash), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 2285);
      this.ModalBox("APPLIANCE_UNZIP_FAILURES");
      this.Busy = false;
      this.AttemptFinishCallback(isError: true, errorMessage: AppResource.APPLIANCE_UNZIP_FAILURES);
    });
    state.Cancellation = (CancellationTokenSource) null;
    Task.Factory.StartNew(this.FileSystemTask, (object) state);
  }

  internal void Sort(FirmwareInstallMode selection)
  {
    this.Busy = true;
    Task.Factory.StartNew(this.FileSystemTask, (object) new ApplianceTask<iService5.Core.Services.Appliance.Sort>()
    {
      ContextType = new iService5.Core.Services.Appliance.Sort(selection),
      TaskCallback = (ApplianceTaskCallback<iService5.Core.Services.Appliance.Sort>) (obj => this.ApplySorted(selection == FirmwareInstallMode.Recovery)),
      Session = this,
      ExceptionCallback = (ApplianceTaskException) (error =>
      {
        this.LogService.getLogger().LogAppError(LoggingContext.BINARY, error, memberName: nameof (Sort), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 2305);
        this.ModalBox("APPLIANCE_SORT_FAILURES");
        this.Busy = false;
        this.AttemptFinishCallback(isError: true, errorMessage: AppResource.APPLIANCE_SORT_FAILURES);
      }),
      Cancellation = (CancellationTokenSource) null
    });
  }

  internal void ApplySorted(bool onlyRecovery)
  {
    this.Busy = true;
    ApplianceTask<ApplySortedFirmwareList> task = new ApplianceTask<ApplySortedFirmwareList>();
    task.TaskCallback = (ApplianceTaskCallback<ApplySortedFirmwareList>) (errorless =>
    {
      if (task.Session.installRepairExecuted)
      {
        if (task.Session.installRepairSuccess)
          this.LogService.getLogger().LogAppDebug(LoggingContext.APPLIANCE, AppResource.APPLIANCE_ECU_SUCCESS, memberName: nameof (ApplySorted), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 2325);
        else
          this.LogService.getLogger().LogAppDebug(LoggingContext.APPLIANCE, AppResource.APPLIANCE_ECU_FAIL, memberName: nameof (ApplySorted), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 2329);
        while (this.installRepairPopupActive)
          Thread.Sleep(500);
        this.AttemptMessageCallback("installRepairPopup off");
      }
      if ((bool) errorless)
      {
        if (!onlyRecovery)
        {
          this.InventoryAvailable = false;
          this.CachedInventoryMerged = false;
          this.LocalInventoryAvailable = false;
          this.ApplianceIsSecure((RequestCallback) (response =>
          {
            if (response)
            {
              this.Busy = false;
              this.AttemptFinishCallback();
            }
            else
              this.ApplySecurity((RequestCallback) (_ =>
              {
                this.Busy = false;
                this.AttemptFinishCallback();
              }));
          }));
        }
        else if (this.AllowReboot)
        {
          this.SetMode(this.RequiresMaintenanceMode() ? BootMode.Maintenance : BootMode.Recovery, (RequestCallback) (changedMode =>
          {
            if (!changedMode)
              return;
            this.ChangeRecoveryVersion();
            this.Busy = false;
            this.AttemptFinishCallback(true);
          }));
        }
        else
        {
          this.ChangeRecoveryVersion();
          if (this.isReprogram)
          {
            this.Flash(FirmwareInstallMode.All);
            this.isReprogram = false;
          }
          else
            this.CompareFirmwareIDs();
        }
      }
      else if (!this.isDisconnected)
        this.Alert.ShowMessageAlertWithKeyFromService("APPLIANCE_FLASH_FAILURES", "", AppResource.APPLIANCE_FLASH_RETRY, AppResource.APPLIANCE_FLASH_RETURN, (Action<bool>) (userReply =>
        {
          if (userReply)
          {
            this.LogService.getLogger().LogAppDebug(LoggingContext.APPLIANCE, "User Selected \"RETRY\" Programming", memberName: nameof (ApplySorted), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 2404);
            this.ApplySorted(onlyRecovery);
          }
          else
          {
            this.LogService.getLogger().LogAppDebug(LoggingContext.APPLIANCE, "User Selected \"RETURN\"", memberName: nameof (ApplySorted), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 2409);
            this.ApplianceIsSecure((RequestCallback) (response =>
            {
              if (response)
              {
                this.Busy = false;
                this.AttemptFinishCallback(isError: true, errorMessage: AppResource.APPLIANCE_FLASH_FAILURES);
              }
              else
                this.ApplySecurity((RequestCallback) (_ =>
                {
                  this.Busy = false;
                  this.AttemptFinishCallback(isError: true, errorMessage: AppResource.APPLIANCE_FLASH_FAILURES);
                }));
            }));
          }
        }));
    });
    task.Session = this;
    task.ExceptionCallback = (ApplianceTaskException) (error =>
    {
      this.LogService.getLogger().LogAppError(LoggingContext.BINARY, "ExceptionCallback in ApplySorted: " + error, memberName: nameof (ApplySorted), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 2434);
      this.Busy = false;
      this.AttemptFinishCallback(isError: true, errorMessage: error);
    });
    task.Cancellation = (CancellationTokenSource) null;
    Task.Factory.StartNew(this.FileSystemTask, (object) task);
  }

  internal void ChangeRecoveryVersion()
  {
    foreach (AugmentedModule augmentedModule in this.Modules.Where<AugmentedModule>((Func<AugmentedModule, bool>) (x => x.Recovery)))
    {
      augmentedModule.InstalledVersion = augmentedModule.version;
      augmentedModule.Installed = iService5.Core.Services.Data.Version.FromString(augmentedModule.version);
      if (augmentedModule.InstalledVersion == "")
        augmentedModule.moduleid = 0L;
    }
    try
    {
      this.Modules.RemoveAll((Predicate<AugmentedModule>) (s => s.moduleid == 0L));
    }
    catch (Exception ex)
    {
      this.LogService.getLogger().LogAppError(LoggingContext.APPLIANCE, "Exception thrown while module-removal: " + ex.Message, memberName: nameof (ChangeRecoveryVersion), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 2458);
    }
  }

  public void ApplyHAInfo()
  {
    foreach (HAEnum haEnum in (HAEnum[]) Enum.GetValues(typeof (HAEnum)))
    {
      HAEnum ha = haEnum;
      ApplianceTask<HAEnum> state = new ApplianceTask<HAEnum>();
      state.Session = this;
      if (ha == HAEnum.countrySettings && !state.Session.RecoverySupportsHACountrySettings)
        break;
      autoEvent = new AutoResetEvent(false);
      this.Busy = true;
      state.ContextType = ha;
      state.TaskCallback = (ApplianceTaskCallback<HAEnum>) (error =>
      {
        if (ha == HAEnum.countrySettings)
          this.AttemptMessageCallback($"HC{this.GetHAValue(ha)}" + "\n");
        else
          this.AttemptMessageCallback(string.Format(AppResource.APPLIANCE_FLASH_SUCCESSFULLY_SET, (object) ha.ToString(), (object) this.GetHAValue(ha)) + "\n");
        this.LogService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, $"Successfully set {ha.ToString()}  to {this.GetHAValue(ha)} {(string) error}", memberName: nameof (ApplyHAInfo), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 2483);
        autoEvent.Set();
      });
      state.ExceptionCallback = (ApplianceTaskException) (error =>
      {
        if (ha == HAEnum.countrySettings)
          this.AttemptMessageCallback("HC error\n");
        else
          this.AttemptMessageCallback(string.Format(AppResource.APPLIANCE_FLASH_FAILED_TO_SET, (object) ha.ToString(), (object) this.GetHAValue(ha)) + "\n");
        this.LogService.getLogger().LogAppError(LoggingContext.APPLIANCE, $"Failed to set {ha.ToString()}  to {this.GetHAValue(ha)} {error}", memberName: nameof (ApplyHAInfo), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 2496);
        autoEvent.Set();
      });
      state.Cancellation = (CancellationTokenSource) null;
      Task.Factory.StartNew(this.ApplianceInfoTask, (object) state);
      autoEvent.WaitOne();
    }
  }

  public void PurgeAndProgram()
  {
    this.Alert.ShowMessageAlertWithKeyFromService("APPLIANCE_FLASH_PURGE", "", "Yes", "No", (Action<bool>) (userReply =>
    {
      if (userReply)
        this.PurgeBeforeProgrammingFlow();
      else
        this.Flash(FirmwareInstallMode.All);
    }));
  }

  internal bool UnzipBinaries(FirmwareInstallMode unzipMode, bool keepSession)
  {
    try
    {
      string pathOf = UtilityFunctions.getPathOf(this.Locator, "binarySession");
      if (Directory.Exists(pathOf) && !keepSession)
        Directory.Delete(pathOf, true);
      if (!keepSession)
        Directory.CreateDirectory(pathOf);
      foreach (AugmentedModule module in this.Modules)
      {
        if (module.IncludeInFlashingSequence && (unzipMode != FirmwareInstallMode.New || !module.Available.IsVersionEqual(module.Installed)) && (unzipMode != FirmwareInstallMode.Recovery || module.Recovery) && (unzipMode == FirmwareInstallMode.Recovery || !module.Recovery))
        {
          string str1 = $"{module.moduleid.ToString()}.{module.Available.ToString()}";
          string path2 = str1 + ".bin";
          string archiveFileName = Path.Combine(this.Locator.GetPlatformSpecificService().getFolder(), path2);
          string str2 = Path.Combine(pathOf, str1 ?? "");
          this.AttemptMessageCallback($"{AppResource.APPLIANCE_FLASH_EXTRACTING} {path2} ...");
          this.LogService.getLogger().LogAppInformation(LoggingContext.BINARY, $"Extracting {path2} in {str2}", memberName: nameof (UnzipBinaries), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 2548);
          if (Directory.Exists(str2))
            Directory.Delete(str2, true);
          if (!Directory.Exists(str2))
            Directory.CreateDirectory(str2);
          ZipArchive archive = ZipFile.OpenRead(archiveFileName);
          if (!UtilityFunctions.CheckCompressedFile(archive))
            throw new ArgumentOutOfRangeException("Uncompressed file exceeds compression thresholds.");
          foreach (ZipArchiveEntry entry in archive.Entries)
          {
            try
            {
              entry.ExtractToFile(Path.Combine(str2, entry.Name), true);
            }
            catch (UnauthorizedAccessException ex)
            {
            }
          }
          this.AttemptMessageCallback(AppResource.APPLIANCE_FLASH_DONE + "\n");
        }
      }
      return true;
    }
    catch (Exception ex)
    {
      this.AttemptMessageCallback(AppResource.APPLIANCE_FLASH_FAILED + "\n");
      this.LogService.getLogger().LogAppError(LoggingContext.BINARY, "Unzip failed ", ex, nameof (UnzipBinaries), "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", 2584);
    }
    return false;
  }

  internal bool SortBinaries(FirmwareInstallMode mode)
  {
    try
    {
      this.SortedFlashingList.Clear();
      string pathOf = UtilityFunctions.getPathOf(this.Locator, "binarySession");
      AugmentedModule _elpID = this.Modules.FirstOrDefault<AugmentedModule>((Func<AugmentedModule, bool>) (p => p.Package == "elp"));
      if (_elpID != null)
      {
        if ((mode == FirmwareInstallMode.Recovery && !_elpID.Recovery || mode != FirmwareInstallMode.Recovery && _elpID.Recovery) | (!_elpID.IncludeInFlashingSequence || mode == FirmwareInstallMode.New && _elpID.Available.IsVersionEqual(_elpID.Installed)))
        {
          this.LogService.getLogger().LogAppInformation(LoggingContext.BINARY, "FirmwareInstallMode is RECOVERY or excluding elp id for programming", memberName: nameof (SortBinaries), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 2606);
        }
        else
        {
          string _moduleFolder = Path.Combine(pathOf, $"{_elpID.moduleid.ToString()}.{_elpID.Available.ToString()}");
          this.LogService.getLogger().LogAppInformation(LoggingContext.BINARY, "Scanning folder: " + _moduleFolder, memberName: nameof (SortBinaries), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 2611);
          int num = _elpID.node;
          if (num == (int) ushort.MaxValue)
            num = 0;
          if (!this.SortedFlashingList.ContainsKey(num))
            this.SortedFlashingList[num] = new Dictionary<long, KeyValuePair<string, AugmentedModule>>();
          if (!this.StoreElpBinInFlashList(_moduleFolder, _elpID, num))
          {
            this.LogService.getLogger().LogAppInformation(LoggingContext.BINARY, "Elp skipped, programming only recovery module : ", memberName: nameof (SortBinaries), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 2625);
            return false;
          }
        }
      }
      if (!this.IterateOverModules(mode, pathOf, ModuleType.FIRMWARE) || !this.IterateOverModules(mode, pathOf, ModuleType.INITIAL_CONTENT) || !this.IterateOverModules(mode, pathOf, ModuleType.SPAU_FIRMWARE))
        return false;
      this.AttemptMessageCallback("Ordering finished\n");
      return true;
    }
    catch (Exception ex)
    {
      this.AttemptMessageCallback("Ordering failed\n");
      this.LogService.getLogger().LogAppError(LoggingContext.BINARY, "Ordering failed. error:  ", ex, nameof (SortBinaries), "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", 2646);
    }
    return false;
  }

  private bool StoreElpBinInFlashList(string _moduleFolder, AugmentedModule _elpID, int _node)
  {
    long moduleid = _elpID.moduleid;
    string str = $"{moduleid.ToString()}.{_elpID.Available.ToString()}";
    foreach (string file in Directory.GetFiles(_moduleFolder))
    {
      if (file.Contains(str))
      {
        long length = new FileInfo(file).Length;
        this.SortedFlashingList[_node][moduleid] = new KeyValuePair<string, AugmentedModule>(file, _elpID);
        if (!string.IsNullOrEmpty(this.GetPackageProperties(moduleid.ToString(), _elpID.Available.ToString())))
          return true;
        this.AttemptMessageCallback("Ordering failed\n");
        this.LogService.getLogger().LogAppError(LoggingContext.BINARY, "Ordering failed - missing signature in " + _moduleFolder, memberName: nameof (StoreElpBinInFlashList), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 2667);
        return false;
      }
    }
    return false;
  }

  internal bool SortBinariesCpio(FirmwareInstallMode mode)
  {
    try
    {
      this.SortedFlashingList.Clear();
      string empty = string.Empty;
      string str1 = Path.Combine(this.Locator.GetPlatformSpecificService().getFolder(), "");
      AugmentedModule _elpId = this.Modules.FirstOrDefault<AugmentedModule>((Func<AugmentedModule, bool>) (p => p.Package == "elp"));
      if (_elpId != null)
      {
        if ((mode == FirmwareInstallMode.Recovery && !_elpId.Recovery || mode != FirmwareInstallMode.Recovery && _elpId.Recovery) | (!_elpId.IncludeInFlashingSequence || mode == FirmwareInstallMode.New && _elpId.Available.IsVersionEqual(_elpId.Installed)))
        {
          this.LogService.getLogger().LogAppInformation(LoggingContext.BINARY, "FirmwareInstallMode is RECOVERY or excluding elp id for programming", memberName: nameof (SortBinariesCpio), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 2697);
        }
        else
        {
          string str2 = Path.Combine(str1, $"{_elpId.moduleid.ToString()}.{_elpId.Available.ToString()}");
          this.LogService.getLogger().LogAppInformation(LoggingContext.BINARY, "Scanning folder: " + str2, memberName: nameof (SortBinariesCpio), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 2702);
          int num = _elpId.node;
          if (num == (int) ushort.MaxValue)
            num = 0;
          if (!this.SortedFlashingList.ContainsKey(num))
            this.SortedFlashingList[num] = new Dictionary<long, KeyValuePair<string, AugmentedModule>>();
          if (!this.StoreElpCpioInFlashList(str1, _elpId, num))
          {
            this.LogService.getLogger().LogAppInformation(LoggingContext.BINARY, "Elp skipped, programming only recovery module : ", memberName: nameof (SortBinariesCpio), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 2715);
            return false;
          }
        }
      }
      if (!this.IterateOverModulesCpio(mode, str1, ModuleType.FIRMWARE) || !this.IterateOverModulesCpio(mode, str1, ModuleType.INITIAL_CONTENT) || !this.IterateOverModulesCpio(mode, str1, ModuleType.SPAU_FIRMWARE))
        return false;
      this.AttemptMessageCallback("Ordering finished\n");
      return true;
    }
    catch (Exception ex)
    {
      this.AttemptMessageCallback("Ordering failed\n");
      this.LogService.getLogger().LogAppError(LoggingContext.BINARY, "Ordering failed. error:  ", ex, nameof (SortBinariesCpio), "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", 2736);
    }
    return false;
  }

  private bool StoreElpCpioInFlashList(string _sesfolder, AugmentedModule _elpId, int _node)
  {
    long moduleid = _elpId.moduleid;
    string str1 = $"{moduleid.ToString()}.{_elpId.Available.ToString()}";
    string str2 = Path.Combine(_sesfolder, str1 + ".cpio");
    if (File.Exists(str2))
    {
      this.SortedFlashingList[_node][moduleid] = new KeyValuePair<string, AugmentedModule>(str2, _elpId);
      if (!string.IsNullOrEmpty(this.GetPackageProperties(moduleid.ToString(), _elpId.Available.ToString())))
        return true;
      this.AttemptMessageCallback("Ordering failed\n");
      this.LogService.getLogger().LogAppError(LoggingContext.BINARY, "Ordering failed - missing signature in " + _sesfolder, memberName: nameof (StoreElpCpioInFlashList), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 2753);
    }
    return false;
  }

  private bool IterateOverModules(
    FirmwareInstallMode _mode,
    string _sessionFolder,
    ModuleType _moduleType)
  {
    foreach (AugmentedModule augmentedModule in this.Modules.FindAll((Predicate<AugmentedModule>) (it => it.type == _moduleType.ToString())).OrderByDescending<AugmentedModule, long>((Func<AugmentedModule, long>) (x => x.fileSize)).ToList<AugmentedModule>())
    {
      if (!(augmentedModule.Package == "elp") && augmentedModule.IncludeInFlashingSequence && (_mode != FirmwareInstallMode.New || !augmentedModule.Available.IsVersionEqual(augmentedModule.Installed)) && (_mode != FirmwareInstallMode.Recovery || augmentedModule.Recovery) && (_mode == FirmwareInstallMode.Recovery || !augmentedModule.Recovery) && augmentedModule.type == _moduleType.ToString())
      {
        string path = Path.Combine(_sessionFolder, $"{augmentedModule.moduleid.ToString()}.{augmentedModule.Available.ToString()}");
        int key = augmentedModule.node;
        if (key == (int) ushort.MaxValue)
          key = 0;
        if (!this.SortedFlashingList.ContainsKey(key))
          this.SortedFlashingList[key] = new Dictionary<long, KeyValuePair<string, AugmentedModule>>();
        bool flag = false;
        foreach (string file in Directory.GetFiles(path))
        {
          if (!file.Contains("packageProperties"))
          {
            long length = new FileInfo(file).Length;
            while (this.SortedFlashingList[key].ContainsKey(augmentedModule.moduleid))
              ++length;
            this.SortedFlashingList[key][augmentedModule.moduleid] = new KeyValuePair<string, AugmentedModule>(file, augmentedModule);
          }
          else
            flag = true;
        }
        if (!flag)
        {
          this.AttemptMessageCallback("Ordering failed\n");
          this.LogService.getLogger().LogAppError(LoggingContext.BINARY, "Ordering failed - missing signature in module-Folder=" + path, memberName: nameof (IterateOverModules), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 2809);
          return false;
        }
      }
    }
    return true;
  }

  private bool IterateOverModulesCpio(
    FirmwareInstallMode _mode,
    string _sessionFolder,
    ModuleType _moduleType)
  {
    foreach (AugmentedModule augmentedModule in this.Modules.FindAll((Predicate<AugmentedModule>) (it => it.type == _moduleType.ToString())).OrderByDescending<AugmentedModule, long>((Func<AugmentedModule, long>) (x => x.fileSize)).ToList<AugmentedModule>())
    {
      if (!(augmentedModule.Package == "elp") && augmentedModule.IncludeInFlashingSequence && (_mode != FirmwareInstallMode.New || !augmentedModule.Available.IsVersionEqual(augmentedModule.Installed)) && (_mode != FirmwareInstallMode.Recovery || augmentedModule.Recovery) && (_mode == FirmwareInstallMode.Recovery || !augmentedModule.Recovery) && augmentedModule.type == _moduleType.ToString())
      {
        long moduleid1;
        if (augmentedModule.Recovery && _mode == FirmwareInstallMode.Recovery)
        {
          Serilog.Core.Logger logger = this.LogService.getLogger();
          moduleid1 = augmentedModule.moduleid;
          string message = $"Programmin Recovery module : {moduleid1.ToString()} {augmentedModule.version}";
          logger.LogAppInformation(LoggingContext.BINARY, message, memberName: nameof (IterateOverModulesCpio), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 2834);
        }
        string path1 = _sessionFolder;
        moduleid1 = augmentedModule.moduleid;
        string path2 = $"{moduleid1.ToString()}.{augmentedModule.Available.ToString()}";
        string str1 = Path.Combine(path1, path2);
        int key = augmentedModule.node;
        if (key == (int) ushort.MaxValue)
          key = 0;
        if (!this.SortedFlashingList.ContainsKey(key))
          this.SortedFlashingList[key] = new Dictionary<long, KeyValuePair<string, AugmentedModule>>();
        bool flag = false;
        long moduleid2 = augmentedModule.moduleid;
        string str2 = $"{moduleid2.ToString()}.{augmentedModule.Available.ToString()}";
        string str3 = Path.Combine(_sessionFolder, str2 + ".cpio");
        if (File.Exists(str3))
        {
          this.SortedFlashingList[key][moduleid2] = new KeyValuePair<string, AugmentedModule>(str3, augmentedModule);
          flag = !string.IsNullOrEmpty(this.GetPackageProperties(moduleid2.ToString(), augmentedModule.Available.ToString()));
        }
        if (!flag)
        {
          this.AttemptMessageCallback("Ordering failed\n");
          this.LogService.getLogger().LogAppError(LoggingContext.BINARY, "Ordering failed - missing signature in module-Folder=" + str1, memberName: nameof (IterateOverModulesCpio), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 2862);
          return false;
        }
      }
    }
    return true;
  }

  public bool IsECUFlashingSupported()
  {
    return this.Modules.Any<AugmentedModule>((Func<AugmentedModule, bool>) (p => p.Recovery && UtilityFunctions.ISAvailableWithInRange(p.Installed, this.versionRangeForECUFlashing)));
  }

  public bool IsInstallRepairSupported()
  {
    return this.Modules.Any<AugmentedModule>((Func<AugmentedModule, bool>) (p => p.Recovery && UtilityFunctions.ISAvailableWithInRange(p.Installed, this.versionRangeForInstallRepair)));
  }

  public bool IsDowngradeAllSupported()
  {
    return this.Modules.Any<AugmentedModule>((Func<AugmentedModule, bool>) (p => p.Recovery && UtilityFunctions.ISAvailableWithInRange(p.Installed, this.versionRangeForDowngradeAll)));
  }

  public bool IsPurgingSupported()
  {
    return this.Modules.Any<AugmentedModule>((Func<AugmentedModule, bool>) (p => p.Recovery && UtilityFunctions.ISAvailableWithInRange(p.Installed, this.versionRangeForPurging)));
  }

  public bool IsHACountrySettingsSupported()
  {
    return this.Modules.Any<AugmentedModule>((Func<AugmentedModule, bool>) (p => p.Recovery && UtilityFunctions.ISAvailableWithInRange(p.Installed, this.versionRangeForHACountrySetting)));
  }

  public bool IsType1CheckSupported()
  {
    return this.Modules.Any<AugmentedModule>((Func<AugmentedModule, bool>) (p => p.Recovery && UtilityFunctions.ISAvailableWithInRange(p.Installed, this.versionRangeForType1Check)));
  }

  public bool IsType3CheckSupported()
  {
    return this.versionRangeForType3Check != null && this.Modules.Any<AugmentedModule>((Func<AugmentedModule, bool>) (p => p.Recovery && UtilityFunctions.ISAvailableWithInRange(p.Installed, this.versionRangeForType3Check)));
  }

  public bool IsType6CheckSupported()
  {
    return this.Modules.Any<AugmentedModule>((Func<AugmentedModule, bool>) (p => p.Recovery && UtilityFunctions.ISAvailableWithInRange(p.Installed, this.versionRangeForType6Check)));
  }

  public bool IsPPF6Supported()
  {
    AugmentedModule augmentedModule = this.Modules.Find((Predicate<AugmentedModule>) (m => m.Recovery));
    iService5.Core.Services.Data.Version availableVersion = augmentedModule.Installed;
    if (!string.IsNullOrEmpty(augmentedModule.RecoveryMisMatchSystemVersion))
      availableVersion = iService5.Core.Services.Data.Version.FromString(augmentedModule.RecoveryMisMatchSystemVersion);
    return UtilityFunctions.ISAvailableWithInRange(availableVersion, this.versionRangeForPPF6);
  }

  public string GetPackageProperties(string moduleid, string version)
  {
    try
    {
      Tuple<string, string> vibAndKi = UtilityFunctions.getVibAndKi(this.UserSession.getEnumberSession());
      long result = 0;
      long.TryParse(moduleid, out result);
      List<ppf> relatedPpfs1 = this.MetadataService.GetRelatedPpfs(vibAndKi.Item1, vibAndKi.Item2, result, version, true);
      List<ppf> relatedPpfs2 = this.MetadataService.GetRelatedPpfs(vibAndKi.Item1, vibAndKi.Item2, result, version, false);
      bool flag1 = false;
      bool flag2 = false;
      string packageProperties = "";
      try
      {
        if (this.RecoverySupportsPPF6 && relatedPpfs1.Count > 0)
        {
          packageProperties = this.ExtractPpfAndGetPath(relatedPpfs1[0]);
          flag2 = true;
        }
        else if (!this.RecoverySupportsPPF6 && relatedPpfs2.Count > 0)
        {
          packageProperties = this.ExtractPpfAndGetPath(relatedPpfs2[0]);
          flag2 = true;
        }
        else
          this.LogService.getLogger().LogAppError(LoggingContext.APPLIANCE, "No PPF Available", memberName: nameof (GetPackageProperties), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 2977);
      }
      catch (Exception ex)
      {
        this.LogService.getLogger().LogAppError(LoggingContext.APPLIANCE, ex.Message, memberName: nameof (GetPackageProperties), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 2982);
        flag1 = true;
      }
      return packageProperties;
    }
    catch (Exception ex)
    {
      this.LogService.getLogger().LogAppError(LoggingContext.APPLIANCE, $"Exception while fetching signature file from {this.signatureFile} - ex-message: {ex.Message}", memberName: nameof (GetPackageProperties), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 2990);
      return "";
    }
  }

  public string ExtractPpfAndGetPath(ppf _ppf)
  {
    string path2 = "";
    if (!string.IsNullOrEmpty(_ppf.vib) || !string.IsNullOrEmpty(_ppf.ki) || !string.IsNullOrEmpty(_ppf.version))
      path2 = $"{_ppf.vib}_{_ppf.ki}_{_ppf.moduleid.ToString()}_{_ppf.version}_{_ppf.type.ToString()}";
    try
    {
      string path = Path.Combine(UtilityFunctions.getPathOf(this.Locator, "extractedPpfs"), path2);
      if (File.Exists(path))
        return path;
      byte[] bytes = Base64UrlEncoder.DecodeBytes(_ppf.ppffile);
      if (bytes.Length != 0)
        File.WriteAllBytes(path, bytes);
      else
        this.LogService.getLogger().LogAppError(LoggingContext.APPLIANCE, $"PPF data is empty for  ppf : {path} of type {_ppf.type}", memberName: nameof (ExtractPpfAndGetPath), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 3019);
      return path;
    }
    catch (Exception ex)
    {
      throw new FormatException($"Failed to extract ppf file with id {path2} of type {_ppf.type}. Error: {ex.Message}");
    }
  }

  public string GetFallbackPpfPath(string moduleid, string version)
  {
    this.signatureFile = Path.Combine(Path.Combine(UtilityFunctions.getPathOf(this.Locator, "binarySession"), $"{moduleid}.{version}"), "packageProperties");
    if (File.Exists(this.signatureFile))
      return this.signatureFile;
    this.LogService.getLogger().LogAppError(LoggingContext.APPLIANCE, "No packaged PPF found.", memberName: nameof (GetFallbackPpfPath), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/ApplianceSession.cs", sourceLineNumber: 3037);
    return string.Empty;
  }
}
