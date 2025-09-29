// Decompiled with JetBrains decompiler
// Type: iService5.Core.AppResource
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

#nullable disable
namespace iService5.Core;

[GeneratedCode("Microsoft.Build.Tasks.StronglyTypedResourceBuilder", "15.1.0.0")]
[DebuggerNonUserCode]
[CompilerGenerated]
public class AppResource
{
  private static ResourceManager resourceMan;
  private static CultureInfo resourceCulture;

  internal AppResource()
  {
  }

  [EditorBrowsable(EditorBrowsableState.Advanced)]
  public static ResourceManager ResourceManager
  {
    get
    {
      if (AppResource.resourceMan == null)
        AppResource.resourceMan = new ResourceManager("iService5.Core.AppResource", typeof (AppResource).Assembly);
      return AppResource.resourceMan;
    }
  }

  [EditorBrowsable(EditorBrowsableState.Advanced)]
  public static CultureInfo Culture
  {
    get => AppResource.resourceCulture;
    set => AppResource.resourceCulture = value;
  }

  public static string ACCEPT_LABEL
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (ACCEPT_LABEL), AppResource.resourceCulture);
    }
  }

  public static string ALERT_TEXT
  {
    get => AppResource.ResourceManager.GetString(nameof (ALERT_TEXT), AppResource.resourceCulture);
  }

  public static string ALL_TEXT
  {
    get => AppResource.ResourceManager.GetString(nameof (ALL_TEXT), AppResource.resourceCulture);
  }

  public static string APPLIANCE_BOOT_MODE_NORMAL
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_BOOT_MODE_NORMAL), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_BOOT_MODE_RECOVERY
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_BOOT_MODE_RECOVERY), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_ECU_CHECKING_CONDITIONS
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_ECU_CHECKING_CONDITIONS), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_ECU_FAIL
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_ECU_FAIL), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_ECU_REPSONSE_ARRIVED
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_ECU_REPSONSE_ARRIVED), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_ECU_SUCCESS
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_ECU_SUCCESS), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_ECU_TASK_RESULT_FAIL
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_ECU_TASK_RESULT_FAIL), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_ECU_TASK_RESULT_SUCCESS
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_ECU_TASK_RESULT_SUCCESS), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_AFTER_REBOOT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_AFTER_REBOOT), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_ALL
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_ALL), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_ALL_VERSIONS
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_ALL_VERSIONS), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_ALREADY_ANYWAY
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_ALREADY_ANYWAY), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_ALREADY_UPD
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_ALREADY_UPD), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_APPLIANCE_REBOOTED
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_APPLIANCE_REBOOTED), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_APPLY_HA_SETTINGS
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_APPLY_HA_SETTINGS), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_AVAILABLE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_AVAILABLE), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_BACK_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_BACK_TEXT), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_BOOT_MODE_SET
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_BOOT_MODE_SET), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_BUTTON
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_BUTTON), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_CANCEL_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_CANCEL_TEXT), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_CHANNGING_BOOT_MODE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_CHANNGING_BOOT_MODE), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_CODING_AFTER_FLASH_EXCEPTION
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_CODING_AFTER_FLASH_EXCEPTION), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_CODING_AFTER_FLASH_WRONG
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_CODING_AFTER_FLASH_WRONG), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_CODING_FAILED_ACTIVATION
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_CODING_FAILED_ACTIVATION), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_CONNECTED
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_CONNECTED), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_CONNECTION_LOST
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_CONNECTION_LOST), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_DATA_NOT_WHITELISTED
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_DATA_NOT_WHITELISTED), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_DATA_SUPPLY_OUTDATED
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_DATA_SUPPLY_OUTDATED), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_DATA_SUPPLY_PROBLEM
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_DATA_SUPPLY_PROBLEM), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_DATA_SUPPLY_UPD_DB
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_DATA_SUPPLY_UPD_DB), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_DELETE_NOT_YET
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_DELETE_NOT_YET), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_DIFF_VERSIONS
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_DIFF_VERSIONS), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_DISCONNECTED
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_DISCONNECTED), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_DISCONNECTION_ERROR
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_DISCONNECTION_ERROR), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_DONE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_DONE), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_EXTRACTING
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_EXTRACTING), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_FAILED
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_FAILED), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_FAILED_TO_GET_BOOTMODE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_FAILED_TO_GET_BOOTMODE), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_FAILED_TO_SET
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_FAILED_TO_SET), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_FAILED_TO_SET_BOOTMODE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_FAILED_TO_SET_BOOTMODE), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_FAILURES
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_FAILURES), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_FILES_FOR_NODE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_FILES_FOR_NODE), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_FILES_FW_FILE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_FILES_FW_FILE), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_FINISH_BUTTON
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_FINISH_BUTTON), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_FINISH_INSTRUCTION
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_FINISH_INSTRUCTION), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_FIRMWARE_LABEL
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_FIRMWARE_LABEL), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_HA_NA
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_HA_NA), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_ID_LABEL
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_ID_LABEL), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_INFO
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_INFO), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_INFORMATION
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_INFORMATION), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_INITIATING_BOOT_MODE_CHANGE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_INITIATING_BOOT_MODE_CHANGE), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_INSTALLED
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_INSTALLED), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_INSTALLING
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_INSTALLING), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_LOG
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_LOG), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_LOG_INTRO
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_LOG_INTRO), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_MODE_SWITCH_BUTTON
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_MODE_SWITCH_BUTTON), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_NEW
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_NEW), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_NO_FW_VERSIONS_TO_UPGRADE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_NO_FW_VERSIONS_TO_UPGRADE), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_NO_NEWER_VERSION
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_NO_NEWER_VERSION), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_NODE_NA
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_NODE_NA), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_ONLY_NEW
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_ONLY_NEW), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_ONLY_OPTIONAL
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_ONLY_OPTIONAL), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_OPT_VERSIONS
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_OPT_VERSIONS), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_PREPERATION_FAILED
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_PREPERATION_FAILED), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_PROCESS_FINISHED
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_PROCESS_FINISHED), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_PROGRAM_ALL
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_PROGRAM_ALL), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_PROGRAM_NEW
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_PROGRAM_NEW), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_PROGRAM_NEW_ALL
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_PROGRAM_NEW_ALL), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_PROGRAM_NOT_YET
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_PROGRAM_NOT_YET), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_PROGRAMMING_DONE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_PROGRAMMING_DONE), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_PROGRESS
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_PROGRESS), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_PURGE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_PURGE), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_QUERY_ALL_NEW
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_QUERY_ALL_NEW), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_QUERY_FLASH_ALL
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_QUERY_FLASH_ALL), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_QUERY_FLASH_ALL_AGAIN
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_QUERY_FLASH_ALL_AGAIN), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_QUERY_FLASH_CANCEL
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_QUERY_FLASH_CANCEL), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_QUERY_FLASH_DATA_SUPPLY_PROBLEM
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_QUERY_FLASH_DATA_SUPPLY_PROBLEM), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_QUERY_FLASH_ONLY_NEW
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_QUERY_FLASH_ONLY_NEW), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_QUERY_NONE_NEW
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_QUERY_NONE_NEW), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_QUERY_OLD
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_QUERY_OLD), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_QUERY_SOME_NEW
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_QUERY_SOME_NEW), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_QUESTION
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_QUESTION), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_REQUESTED_REBOOT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_REQUESTED_REBOOT), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_RESTART_PROGRAMMING
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_RESTART_PROGRAMMING), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_RETRY
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_RETRY), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_RETURN
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_RETURN), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_SOFTWARE_PACKAGE_NOT_VALID
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_SOFTWARE_PACKAGE_NOT_VALID), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_START_BUTTON
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_START_BUTTON), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_STEP
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_STEP), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_SUCCESSFULLY_SET
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_SUCCESSFULLY_SET), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_TO
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_TO), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_UPLOADING
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_UPLOADING), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FLASH_WAITING
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FLASH_WAITING), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_FW_INFO
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_FW_INFO), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_INSTALL_REPAIR_FAILED
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_INSTALL_REPAIR_FAILED), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_INSTALL_REPAIR_POPUP_CLOSING
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_INSTALL_REPAIR_POPUP_CLOSING), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_INSTALL_REPAIR_POPUP_READY_TO_DISPLAY
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_INSTALL_REPAIR_POPUP_READY_TO_DISPLAY), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_INSTALL_REPAIR_REPSONSE_ARRIVED
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_INSTALL_REPAIR_REPSONSE_ARRIVED), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_INSTALL_REPAIR_REPSONSE_WAIT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_INSTALL_REPAIR_REPSONSE_WAIT), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_INSTALL_REPAIR_SUCCESS
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_INSTALL_REPAIR_SUCCESS), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_INSTRUCTIONS_CONNECTED
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_INSTRUCTIONS_CONNECTED), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_INSTRUCTIONS_DISCONNECTED
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_INSTRUCTIONS_DISCONNECTED), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_PROGRAMMING_NOT_SUCCESSFUL
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_PROGRAMMING_NOT_SUCCESSFUL), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_SELECT_A_FLASHER_HEADER
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_SELECT_A_FLASHER_HEADER), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_SORT_FAILURES
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_SORT_FAILURES), AppResource.resourceCulture);
    }
  }

  public static string APPLIANCE_UNZIP_FAILURES
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (APPLIANCE_UNZIP_FAILURES), AppResource.resourceCulture);
    }
  }

  public static string AUXILIARY_FILES
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (AUXILIARY_FILES), AppResource.resourceCulture);
    }
  }

  public static string AVAILABLE_DATA_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (AVAILABLE_DATA_TEXT), AppResource.resourceCulture);
    }
  }

  public static string AVAILABLE_STORAGE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (AVAILABLE_STORAGE), AppResource.resourceCulture);
    }
  }

  public static string AVAILABLE_STORAGE_ON_DEVICE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (AVAILABLE_STORAGE_ON_DEVICE), AppResource.resourceCulture);
    }
  }

  public static string AVAILABLE_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (AVAILABLE_TEXT), AppResource.resourceCulture);
    }
  }

  public static string BACK_TEXT
  {
    get => AppResource.ResourceManager.GetString(nameof (BACK_TEXT), AppResource.resourceCulture);
  }

  public static string Binary_Download_ProgressAlert
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (Binary_Download_ProgressAlert), AppResource.resourceCulture);
    }
  }

  public static string BINARY_FILE_SIZE_FILTER_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (BINARY_FILE_SIZE_FILTER_TEXT), AppResource.resourceCulture);
    }
  }

  public static string BINARY_UPLOAD_FAILED
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (BINARY_UPLOAD_FAILED), AppResource.resourceCulture);
    }
  }

  public static string BOOT_MODE_SET_RECOVERY_FAILED
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (BOOT_MODE_SET_RECOVERY_FAILED), AppResource.resourceCulture);
    }
  }

  public static string BOOT_MODE_TRANSITION_APPLIANCE_ADDITIONAL_REBOOTING
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (BOOT_MODE_TRANSITION_APPLIANCE_ADDITIONAL_REBOOTING), AppResource.resourceCulture);
    }
  }

  public static string BOOT_MODE_TRANSITION_APPLIANCE_ADDITIONAL_REBOOTING_PART_2
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (BOOT_MODE_TRANSITION_APPLIANCE_ADDITIONAL_REBOOTING_PART_2), AppResource.resourceCulture);
    }
  }

  public static string BOOT_MODE_TRANSITION_APPLIANCE_IS_REBOOTING
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (BOOT_MODE_TRANSITION_APPLIANCE_IS_REBOOTING), AppResource.resourceCulture);
    }
  }

  public static string BOOT_MODE_TRANSITION_APPLIANCE_IS_REBOOTING_TO_ELP
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (BOOT_MODE_TRANSITION_APPLIANCE_IS_REBOOTING_TO_ELP), AppResource.resourceCulture);
    }
  }

  public static string BOOT_MODE_TRANSITION_APPLIANCE_IS_REBOOTING_TO_RECOVERY
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (BOOT_MODE_TRANSITION_APPLIANCE_IS_REBOOTING_TO_RECOVERY), AppResource.resourceCulture);
    }
  }

  public static string BOOT_MODE_TRANSITION_APPLIANCE_IS_REBOOTING_TO_RECOVERY_EXTRA_PRT1
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (BOOT_MODE_TRANSITION_APPLIANCE_IS_REBOOTING_TO_RECOVERY_EXTRA_PRT1), AppResource.resourceCulture);
    }
  }

  public static string BOOT_MODE_TRANSITION_APPLIANCE_IS_REBOOTING_TO_RECOVERY_EXTRA_PRT2
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (BOOT_MODE_TRANSITION_APPLIANCE_IS_REBOOTING_TO_RECOVERY_EXTRA_PRT2), AppResource.resourceCulture);
    }
  }

  public static string BOOT_MODE_TRANSITION_APPLIANCE_RECONNECT_AFTERWARDS
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (BOOT_MODE_TRANSITION_APPLIANCE_RECONNECT_AFTERWARDS), AppResource.resourceCulture);
    }
  }

  public static string BOOT_MODE_TRANSITION_APPLIANCE_THIS_MAY_TAKE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (BOOT_MODE_TRANSITION_APPLIANCE_THIS_MAY_TAKE), AppResource.resourceCulture);
    }
  }

  public static string BOOT_MODE_TRANSITION_APPLIANCE_THIS_MAY_TAKE_FEW_MINUTES
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (BOOT_MODE_TRANSITION_APPLIANCE_THIS_MAY_TAKE_FEW_MINUTES), AppResource.resourceCulture);
    }
  }

  public static string BOOT_MODE_TRANSITION_FURTHER
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (BOOT_MODE_TRANSITION_FURTHER), AppResource.resourceCulture);
    }
  }

  public static string BOOT_MODE_TRANSITION_PLEASE_WAIT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (BOOT_MODE_TRANSITION_PLEASE_WAIT), AppResource.resourceCulture);
    }
  }

  public static string BOOT_MODE_TRANSITION_WIFI_FAILED
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (BOOT_MODE_TRANSITION_WIFI_FAILED), AppResource.resourceCulture);
    }
  }

  public static string BRIDGE_HEADER
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (BRIDGE_HEADER), AppResource.resourceCulture);
    }
  }

  public static string BRIDGE_SETTING_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (BRIDGE_SETTING_TEXT), AppResource.resourceCulture);
    }
  }

  public static string BRIDGE_SETTINGS_ADVANCED_TOGGLE_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (BRIDGE_SETTINGS_ADVANCED_TOGGLE_TEXT), AppResource.resourceCulture);
    }
  }

  public static string BRIDGE_SETTINGS_BACK_ALERT_MESSAGE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (BRIDGE_SETTINGS_BACK_ALERT_MESSAGE), AppResource.resourceCulture);
    }
  }

  public static string BRIDGE_SETTINGS_BACK_ALERT_TITLE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (BRIDGE_SETTINGS_BACK_ALERT_TITLE), AppResource.resourceCulture);
    }
  }

  public static string BRIDGE_SETTINGS_EMPTY_HELP
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (BRIDGE_SETTINGS_EMPTY_HELP), AppResource.resourceCulture);
    }
  }

  public static string BRIDGE_SETTINGS_ERROR_MESSAGE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (BRIDGE_SETTINGS_ERROR_MESSAGE), AppResource.resourceCulture);
    }
  }

  public static string BRIDGE_SETTINGS_HEX_HELP
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (BRIDGE_SETTINGS_HEX_HELP), AppResource.resourceCulture);
    }
  }

  public static string BRIDGE_SETTINGS_INTEGER_HELP
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (BRIDGE_SETTINGS_INTEGER_HELP), AppResource.resourceCulture);
    }
  }

  public static string BRIDGE_SETTINGS_NOT_SAVED_SUCCESSFULLY_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (BRIDGE_SETTINGS_NOT_SAVED_SUCCESSFULLY_TEXT), AppResource.resourceCulture);
    }
  }

  public static string BRIDGE_SETTINGS_NUMBER_HELP
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (BRIDGE_SETTINGS_NUMBER_HELP), AppResource.resourceCulture);
    }
  }

  public static string BRIDGE_SETTINGS_RANGE_HELP
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (BRIDGE_SETTINGS_RANGE_HELP), AppResource.resourceCulture);
    }
  }

  public static string BRIDGE_SETTINGS_SAVE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (BRIDGE_SETTINGS_SAVE), AppResource.resourceCulture);
    }
  }

  public static string BRIDGE_SETTINGS_SAVED_SUCCESSFULLY_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (BRIDGE_SETTINGS_SAVED_SUCCESSFULLY_TEXT), AppResource.resourceCulture);
    }
  }

  public static string BRIDGE_SETTINGS_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (BRIDGE_SETTINGS_TEXT), AppResource.resourceCulture);
    }
  }

  public static string BRIDGE_SETTINGS_TITLE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (BRIDGE_SETTINGS_TITLE), AppResource.resourceCulture);
    }
  }

  public static string BRIDGE_SETTINGS_URL_HELP
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (BRIDGE_SETTINGS_URL_HELP), AppResource.resourceCulture);
    }
  }

  public static string BRIDGE_SETTINGS_ZIP_HELP
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (BRIDGE_SETTINGS_ZIP_HELP), AppResource.resourceCulture);
    }
  }

  public static string BRIDGE_UPGRADE_INSTRUCTION_1
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (BRIDGE_UPGRADE_INSTRUCTION_1), AppResource.resourceCulture);
    }
  }

  public static string BRIDGE_UPGRADE_INSTRUCTION_2
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (BRIDGE_UPGRADE_INSTRUCTION_2), AppResource.resourceCulture);
    }
  }

  public static string BRIDGE_UPGRADE_INSTRUCTION_3
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (BRIDGE_UPGRADE_INSTRUCTION_3), AppResource.resourceCulture);
    }
  }

  public static string BRIDGE_UPGRADE_INSTRUCTION_4
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (BRIDGE_UPGRADE_INSTRUCTION_4), AppResource.resourceCulture);
    }
  }

  public static string BRIDGE_UPGRADE_INSTRUCTION_5
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (BRIDGE_UPGRADE_INSTRUCTION_5), AppResource.resourceCulture);
    }
  }

  public static string BRIDGE_UPGRADE_INSTRUCTION_6
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (BRIDGE_UPGRADE_INSTRUCTION_6), AppResource.resourceCulture);
    }
  }

  public static string BRIDGE_UPGRADE_INSTRUCTION_HEADER
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (BRIDGE_UPGRADE_INSTRUCTION_HEADER), AppResource.resourceCulture);
    }
  }

  public static string BRIDGE_UPGRADE_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (BRIDGE_UPGRADE_TEXT), AppResource.resourceCulture);
    }
  }

  public static string BRIDGE_WIFI_INFO_PAGE_HINT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (BRIDGE_WIFI_INFO_PAGE_HINT), AppResource.resourceCulture);
    }
  }

  public static string BRIDGELOG_ERROR
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (BRIDGELOG_ERROR), AppResource.resourceCulture);
    }
  }

  public static string BRIDGELOG_SAVED
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (BRIDGELOG_SAVED), AppResource.resourceCulture);
    }
  }

  public static string CALLIBRATION_EVERY_TIME
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CALLIBRATION_EVERY_TIME), AppResource.resourceCulture);
    }
  }

  public static string CALLIBRATION_INTERVAL_LABEL
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CALLIBRATION_INTERVAL_LABEL), AppResource.resourceCulture);
    }
  }

  public static string CALLIBRATION_MANUALLY
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CALLIBRATION_MANUALLY), AppResource.resourceCulture);
    }
  }

  public static string CALLIBRATION_ONCE_A_DAY
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CALLIBRATION_ONCE_A_DAY), AppResource.resourceCulture);
    }
  }

  public static string CALLIBRATION_ONCE_A_WEEK
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CALLIBRATION_ONCE_A_WEEK), AppResource.resourceCulture);
    }
  }

  public static string CANCEL_LABEL
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CANCEL_LABEL), AppResource.resourceCulture);
    }
  }

  public static string CAT_SETTINGS_ERROR_MESSAGE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CAT_SETTINGS_ERROR_MESSAGE), AppResource.resourceCulture);
    }
  }

  public static string CAT_SETTINGS_NOT_SAVED_SUCCESSFULLY_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CAT_SETTINGS_NOT_SAVED_SUCCESSFULLY_TEXT), AppResource.resourceCulture);
    }
  }

  public static string CAT_SETTINGS_SAVED_SUCCESSFULLY_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CAT_SETTINGS_SAVED_SUCCESSFULLY_TEXT), AppResource.resourceCulture);
    }
  }

  public static string CHECK_DBUS_CONNECTION
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CHECK_DBUS_CONNECTION), AppResource.resourceCulture);
    }
  }

  public static string CODING
  {
    get => AppResource.ResourceManager.GetString(nameof (CODING), AppResource.resourceCulture);
  }

  public static string CODING_ERROR
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CODING_ERROR), AppResource.resourceCulture);
    }
  }

  public static string CODING_FAILED
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CODING_FAILED), AppResource.resourceCulture);
    }
  }

  public static string CODING_FAILED_TO_INITIALIZE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CODING_FAILED_TO_INITIALIZE), AppResource.resourceCulture);
    }
  }

  public static string CODING_SUCCESSFUL
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CODING_SUCCESSFUL), AppResource.resourceCulture);
    }
  }

  public static string CODING_TRANSITION
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CODING_TRANSITION), AppResource.resourceCulture);
    }
  }

  public static string COMPACT_APPLIANCE_TESTER
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (COMPACT_APPLIANCE_TESTER), AppResource.resourceCulture);
    }
  }

  public static string COMPACT_APPLIANCE_TESTER_TITLE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (COMPACT_APPLIANCE_TESTER_TITLE), AppResource.resourceCulture);
    }
  }

  public static string CONN_PAGE_BRIDGE_MOBILE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CONN_PAGE_BRIDGE_MOBILE), AppResource.resourceCulture);
    }
  }

  public static string CONN_PAGE_CHOOSE_BSH_NET
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CONN_PAGE_CHOOSE_BSH_NET), AppResource.resourceCulture);
    }
  }

  public static string CONN_PAGE_CODING_DISABLED_MISSING_FILE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CONN_PAGE_CODING_DISABLED_MISSING_FILE), AppResource.resourceCulture);
    }
  }

  public static string CONN_PAGE_CODING_DISABLED_NO_VARCODE_ENTRY
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CONN_PAGE_CODING_DISABLED_NO_VARCODE_ENTRY), AppResource.resourceCulture);
    }
  }

  public static string CONN_PAGE_CODING_DISABLED_NOT_AVAILABLE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CONN_PAGE_CODING_DISABLED_NOT_AVAILABLE), AppResource.resourceCulture);
    }
  }

  public static string CONN_PAGE_CONNECT_DBUS
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CONN_PAGE_CONNECT_DBUS), AppResource.resourceCulture);
    }
  }

  public static string CONN_PAGE_CONTROL_DISABLED_DOCUMENT_NOT_AVAILABLE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CONN_PAGE_CONTROL_DISABLED_DOCUMENT_NOT_AVAILABLE), AppResource.resourceCulture);
    }
  }

  public static string CONN_PAGE_CONTROL_DISABLED_DOCUMENT_NOT_FOUND
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CONN_PAGE_CONTROL_DISABLED_DOCUMENT_NOT_FOUND), AppResource.resourceCulture);
    }
  }

  public static string CONN_PAGE_CONTROL_DISABLED_MISSING_FILES
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CONN_PAGE_CONTROL_DISABLED_MISSING_FILES), AppResource.resourceCulture);
    }
  }

  public static string CONN_PAGE_CONTROL_DISABLED_MISSING_MONITORING_FILES
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CONN_PAGE_CONTROL_DISABLED_MISSING_MONITORING_FILES), AppResource.resourceCulture);
    }
  }

  public static string CONN_PAGE_CONTROL_DISABLED_TOPOGRAM_NOT_AVAILABLE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CONN_PAGE_CONTROL_DISABLED_TOPOGRAM_NOT_AVAILABLE), AppResource.resourceCulture);
    }
  }

  public static string CONN_PAGE_CONTROL_DISABLED_TOPOGRAM_NOT_FOUND
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CONN_PAGE_CONTROL_DISABLED_TOPOGRAM_NOT_FOUND), AppResource.resourceCulture);
    }
  }

  public static string CONN_PAGE_DBUS_DISCONNECTED
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CONN_PAGE_DBUS_DISCONNECTED), AppResource.resourceCulture);
    }
  }

  public static string CONN_PAGE_ENTER_WIFI_PASS
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CONN_PAGE_ENTER_WIFI_PASS), AppResource.resourceCulture);
    }
  }

  public static string CONN_PAGE_ENTER_WIFI_PASS_OF_BRIDGE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CONN_PAGE_ENTER_WIFI_PASS_OF_BRIDGE), AppResource.resourceCulture);
    }
  }

  public static string CONN_PAGE_FLASHING_DISABLED_MISSING_FILES
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CONN_PAGE_FLASHING_DISABLED_MISSING_FILES), AppResource.resourceCulture);
    }
  }

  public static string CONN_PAGE_FLASHING_DISABLED_MISSINGBINARY
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CONN_PAGE_FLASHING_DISABLED_MISSINGBINARY), AppResource.resourceCulture);
    }
  }

  public static string CONN_PAGE_FLASHING_DISABLED_NOTSMM
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CONN_PAGE_FLASHING_DISABLED_NOTSMM), AppResource.resourceCulture);
    }
  }

  public static string CONN_PAGE_FLASHING_DISABLED_PPF_NOT_AVAILABLE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CONN_PAGE_FLASHING_DISABLED_PPF_NOT_AVAILABLE), AppResource.resourceCulture);
    }
  }

  public static string CONN_PAGE_FLASHING_DISABLED_SMM_CONN_DEVICE_FAILED_TO_COMMUNICATE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CONN_PAGE_FLASHING_DISABLED_SMM_CONN_DEVICE_FAILED_TO_COMMUNICATE), AppResource.resourceCulture);
    }
  }

  public static string CONN_PAGE_FLASHING_DISABLED_SMM_CONN_DEVICE_NOT_COMPLETE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CONN_PAGE_FLASHING_DISABLED_SMM_CONN_DEVICE_NOT_COMPLETE), AppResource.resourceCulture);
    }
  }

  public static string CONN_PAGE_GOTO
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CONN_PAGE_GOTO), AppResource.resourceCulture);
    }
  }

  public static string CONN_PAGE_INITIALISING_UI
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CONN_PAGE_INITIALISING_UI), AppResource.resourceCulture);
    }
  }

  public static string CONN_PAGE_LOGGING_DISABLED_CONN_PAGE_NOTSMM
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CONN_PAGE_LOGGING_DISABLED_CONN_PAGE_NOTSMM), AppResource.resourceCulture);
    }
  }

  public static string CONN_PAGE_LOGGING_DISABLED_IN_RECOVERY
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CONN_PAGE_LOGGING_DISABLED_IN_RECOVERY), AppResource.resourceCulture);
    }
  }

  public static string CONN_PAGE_LOGGING_DISABLED_SMM_CONN_DEVICE_NOT_COMPLETE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CONN_PAGE_LOGGING_DISABLED_SMM_CONN_DEVICE_NOT_COMPLETE), AppResource.resourceCulture);
    }
  }

  public static string CONN_PAGE_MEASURE_AND_BRIDGE_SETTING_DISABLED
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CONN_PAGE_MEASURE_AND_BRIDGE_SETTING_DISABLED), AppResource.resourceCulture);
    }
  }

  public static string CONN_PAGE_MISSING_PPF_FILES
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CONN_PAGE_MISSING_PPF_FILES), AppResource.resourceCulture);
    }
  }

  public static string CONN_PAGE_MISSINGBINARY
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CONN_PAGE_MISSINGBINARY), AppResource.resourceCulture);
    }
  }

  public static string CONN_PAGE_MODE_CHANGE_DISABLED_SMM_CONN_DEVICE_FAILED_TO_COMMUNICATE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CONN_PAGE_MODE_CHANGE_DISABLED_SMM_CONN_DEVICE_FAILED_TO_COMMUNICATE), AppResource.resourceCulture);
    }
  }

  public static string CONN_PAGE_MODE_CHANGE_DISABLED_SMM_CONN_DEVICE_NOT_COMPLETE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CONN_PAGE_MODE_CHANGE_DISABLED_SMM_CONN_DEVICE_NOT_COMPLETE), AppResource.resourceCulture);
    }
  }

  public static string CONN_PAGE_MONITORING_DISABLED_MISSING_TOPOGRAM
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CONN_PAGE_MONITORING_DISABLED_MISSING_TOPOGRAM), AppResource.resourceCulture);
    }
  }

  public static string CONN_PAGE_MONITORING_DISABLED_TOPOGRAM_NOT_AVAILABLE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CONN_PAGE_MONITORING_DISABLED_TOPOGRAM_NOT_AVAILABLE), AppResource.resourceCulture);
    }
  }

  public static string CONN_PAGE_MONITORING_DISABLED_TOPOGRAM_NOT_FOUND
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CONN_PAGE_MONITORING_DISABLED_TOPOGRAM_NOT_FOUND), AppResource.resourceCulture);
    }
  }

  public static string CONN_PAGE_NOTSMM
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CONN_PAGE_NOTSMM), AppResource.resourceCulture);
    }
  }

  public static string CONN_PAGE_PROGRAMMING_DISABLED_DATA_SUPPLY
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CONN_PAGE_PROGRAMMING_DISABLED_DATA_SUPPLY), AppResource.resourceCulture);
    }
  }

  public static string CONN_PAGE_SETTINGS
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CONN_PAGE_SETTINGS), AppResource.resourceCulture);
    }
  }

  public static string CONN_PAGE_TEXT1
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CONN_PAGE_TEXT1), AppResource.resourceCulture);
    }
  }

  public static string CONN_PAGE_TEXT2
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CONN_PAGE_TEXT2), AppResource.resourceCulture);
    }
  }

  public static string CONN_PAGE_TEXT2BRIDGE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CONN_PAGE_TEXT2BRIDGE), AppResource.resourceCulture);
    }
  }

  public static string CONN_PAGE_WAIT_FOR_BRIDGE_DEVICE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CONN_PAGE_WAIT_FOR_BRIDGE_DEVICE), AppResource.resourceCulture);
    }
  }

  public static string CONN_PAGE_WAIT_FOR_CLICK
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CONN_PAGE_WAIT_FOR_CLICK), AppResource.resourceCulture);
    }
  }

  public static string CONN_PAGE_WAIT_FOR_CONNECTION
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CONN_PAGE_WAIT_FOR_CONNECTION), AppResource.resourceCulture);
    }
  }

  public static string CONNECT_BTN_LABEL
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CONNECT_BTN_LABEL), AppResource.resourceCulture);
    }
  }

  public static string CONNECTED_BRIDGE_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CONNECTED_BRIDGE_TEXT), AppResource.resourceCulture);
    }
  }

  public static string CONNECTED_DBUS_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CONNECTED_DBUS_TEXT), AppResource.resourceCulture);
    }
  }

  public static string CONNECTED_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CONNECTED_TEXT), AppResource.resourceCulture);
    }
  }

  public static string CONTINUE_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CONTINUE_TEXT), AppResource.resourceCulture);
    }
  }

  public static string CONTROL
  {
    get => AppResource.ResourceManager.GetString(nameof (CONTROL), AppResource.resourceCulture);
  }

  public static string CONTROL_START_FAILED
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (CONTROL_START_FAILED), AppResource.resourceCulture);
    }
  }

  public static string COUNTRY_ASIA_PACIFIC
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (COUNTRY_ASIA_PACIFIC), AppResource.resourceCulture);
    }
  }

  public static string COUNTRY_EUROPE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (COUNTRY_EUROPE), AppResource.resourceCulture);
    }
  }

  public static string COUNTRY_NORTH_AMERICA
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (COUNTRY_NORTH_AMERICA), AppResource.resourceCulture);
    }
  }

  public static string COUNTRY_RELEVANT_DESCRIPTION_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (COUNTRY_RELEVANT_DESCRIPTION_TEXT), AppResource.resourceCulture);
    }
  }

  public static string COUNTRY_RELEVANT_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (COUNTRY_RELEVANT_TEXT), AppResource.resourceCulture);
    }
  }

  public static string COUNTRY_SELECTED
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (COUNTRY_SELECTED), AppResource.resourceCulture);
    }
  }

  public static string DATA_LOGGER_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (DATA_LOGGER_TEXT), AppResource.resourceCulture);
    }
  }

  public static string DATA_PRIVACY_POLICY_DATA
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (DATA_PRIVACY_POLICY_DATA), AppResource.resourceCulture);
    }
  }

  public static string DATA_PRIVACY_POLICY_DATA_1
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (DATA_PRIVACY_POLICY_DATA_1), AppResource.resourceCulture);
    }
  }

  public static string DATA_PRIVACY_POLICY_DATA_2
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (DATA_PRIVACY_POLICY_DATA_2), AppResource.resourceCulture);
    }
  }

  public static string DATA_PRIVACY_POLICY_DATA_3
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (DATA_PRIVACY_POLICY_DATA_3), AppResource.resourceCulture);
    }
  }

  public static string DATA_PRIVACY_POLICY_DATA_4
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (DATA_PRIVACY_POLICY_DATA_4), AppResource.resourceCulture);
    }
  }

  public static string DATA_PRIVACY_POLICY_DATA_5
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (DATA_PRIVACY_POLICY_DATA_5), AppResource.resourceCulture);
    }
  }

  public static string DATA_PRIVACY_POLICY_DATA_6
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (DATA_PRIVACY_POLICY_DATA_6), AppResource.resourceCulture);
    }
  }

  public static string DATA_PRIVACY_POLICY_DATA_7
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (DATA_PRIVACY_POLICY_DATA_7), AppResource.resourceCulture);
    }
  }

  public static string DATA_PRIVACY_POLICY_HEADER_1
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (DATA_PRIVACY_POLICY_HEADER_1), AppResource.resourceCulture);
    }
  }

  public static string DATA_PRIVACY_POLICY_HEADER_2
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (DATA_PRIVACY_POLICY_HEADER_2), AppResource.resourceCulture);
    }
  }

  public static string DATA_PRIVACY_POLICY_HEADER_3
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (DATA_PRIVACY_POLICY_HEADER_3), AppResource.resourceCulture);
    }
  }

  public static string DATA_PRIVACY_POLICY_HEADER_4
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (DATA_PRIVACY_POLICY_HEADER_4), AppResource.resourceCulture);
    }
  }

  public static string DATA_PRIVACY_POLICY_HEADER_5
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (DATA_PRIVACY_POLICY_HEADER_5), AppResource.resourceCulture);
    }
  }

  public static string DATA_PRIVACY_POLICY_HEADER_6
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (DATA_PRIVACY_POLICY_HEADER_6), AppResource.resourceCulture);
    }
  }

  public static string DATA_PRIVACY_POLICY_HEADER_7
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (DATA_PRIVACY_POLICY_HEADER_7), AppResource.resourceCulture);
    }
  }

  public static string DATA_PRIVACY_POLICY_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (DATA_PRIVACY_POLICY_TEXT), AppResource.resourceCulture);
    }
  }

  public static string DATA_SETUP_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (DATA_SETUP_TEXT), AppResource.resourceCulture);
    }
  }

  public static string DELETE_ALL_APPLIANCE_FILES_ALERT_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (DELETE_ALL_APPLIANCE_FILES_ALERT_TEXT), AppResource.resourceCulture);
    }
  }

  public static string DELETE_ALL_APPLIANCE_FILES_BUTTON_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (DELETE_ALL_APPLIANCE_FILES_BUTTON_TEXT), AppResource.resourceCulture);
    }
  }

  public static string DELETE_APPLIANCE_FILES
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (DELETE_APPLIANCE_FILES), AppResource.resourceCulture);
    }
  }

  public static string DELETE_APPLIANCE_FILES_WARNIG_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (DELETE_APPLIANCE_FILES_WARNIG_TEXT), AppResource.resourceCulture);
    }
  }

  public static string DEVICE_CLASS_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (DEVICE_CLASS_TEXT), AppResource.resourceCulture);
    }
  }

  public static string DIRECTLY_GO_TO_HOME_SCREEN_PART1
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (DIRECTLY_GO_TO_HOME_SCREEN_PART1), AppResource.resourceCulture);
    }
  }

  public static string DIRECTLY_GO_TO_HOME_SCREEN_PART2
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (DIRECTLY_GO_TO_HOME_SCREEN_PART2), AppResource.resourceCulture);
    }
  }

  public static string DISCONNECTED_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (DISCONNECTED_TEXT), AppResource.resourceCulture);
    }
  }

  public static string DOWNLOAD_LOG_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (DOWNLOAD_LOG_TEXT), AppResource.resourceCulture);
    }
  }

  public static string DOWNLOAD_SETTINGS_CONFIGURE_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (DOWNLOAD_SETTINGS_CONFIGURE_TEXT), AppResource.resourceCulture);
    }
  }

  public static string DOWNLOAD_SETTINGS_LABEL_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (DOWNLOAD_SETTINGS_LABEL_TEXT), AppResource.resourceCulture);
    }
  }

  public static string DOWNLOADING_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (DOWNLOADING_TEXT), AppResource.resourceCulture);
    }
  }

  public static string EMAIL_TEXT
  {
    get => AppResource.ResourceManager.GetString(nameof (EMAIL_TEXT), AppResource.resourceCulture);
  }

  public static string EMPTY_FEEDBACK_ALERT_MESSAGE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (EMPTY_FEEDBACK_ALERT_MESSAGE), AppResource.resourceCulture);
    }
  }

  public static string ENUMBER_MISMATCH
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (ENUMBER_MISMATCH), AppResource.resourceCulture);
    }
  }

  public static string ENUMBER_MISMATCH_PROGRAMMING_DISABLED
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (ENUMBER_MISMATCH_PROGRAMMING_DISABLED), AppResource.resourceCulture);
    }
  }

  public static string ERROR__TRANSITION_PAGE_COLLECTING
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (ERROR__TRANSITION_PAGE_COLLECTING), AppResource.resourceCulture);
    }
  }

  public static string ERROR_DETAIL_ADVICE_LABEL
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (ERROR_DETAIL_ADVICE_LABEL), AppResource.resourceCulture);
    }
  }

  public static string ERROR_DETAIL_DATE_TIME_LABEL
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (ERROR_DETAIL_DATE_TIME_LABEL), AppResource.resourceCulture);
    }
  }

  public static string ERROR_IN_CODING_MESSAGE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (ERROR_IN_CODING_MESSAGE), AppResource.resourceCulture);
    }
  }

  public static string ERROR_LOG_EC_LABEL
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (ERROR_LOG_EC_LABEL), AppResource.resourceCulture);
    }
  }

  public static string ERROR_LOG_LABEL
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (ERROR_LOG_LABEL), AppResource.resourceCulture);
    }
  }

  public static string ERROR_LOG_NO_LOG
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (ERROR_LOG_NO_LOG), AppResource.resourceCulture);
    }
  }

  public static string ERROR_LOG_TIMESTAMP_POPUPHEADER
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (ERROR_LOG_TIMESTAMP_POPUPHEADER), AppResource.resourceCulture);
    }
  }

  public static string ERROR_LOG_TIMESTAMP_POPUPSTRING
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (ERROR_LOG_TIMESTAMP_POPUPSTRING), AppResource.resourceCulture);
    }
  }

  public static string ERROR_LOG_TS_LABEL
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (ERROR_LOG_TS_LABEL), AppResource.resourceCulture);
    }
  }

  public static string ERROR_PAGE_NOTSUPPORTED
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (ERROR_PAGE_NOTSUPPORTED), AppResource.resourceCulture);
    }
  }

  public static string ERROR_SET_HA_BRAND
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (ERROR_SET_HA_BRAND), AppResource.resourceCulture);
    }
  }

  public static string ERROR_SET_HA_COUNTRY_SETTINGS
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (ERROR_SET_HA_COUNTRY_SETTINGS), AppResource.resourceCulture);
    }
  }

  public static string ERROR_SET_HA_CUSTOMER_INDEX
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (ERROR_SET_HA_CUSTOMER_INDEX), AppResource.resourceCulture);
    }
  }

  public static string ERROR_SET_HA_DEVICE_TYPE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (ERROR_SET_HA_DEVICE_TYPE), AppResource.resourceCulture);
    }
  }

  public static string ERROR_SET_HA_TIMESTAMP
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (ERROR_SET_HA_TIMESTAMP), AppResource.resourceCulture);
    }
  }

  public static string ERROR_SET_HA_VIB
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (ERROR_SET_HA_VIB), AppResource.resourceCulture);
    }
  }

  public static string ERROR_TITLE
  {
    get => AppResource.ResourceManager.GetString(nameof (ERROR_TITLE), AppResource.resourceCulture);
  }

  public static string FEEDBACK_PAGE_ACKNOWLEDGEMENT_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (FEEDBACK_PAGE_ACKNOWLEDGEMENT_TEXT), AppResource.resourceCulture);
    }
  }

  public static string FEEDBACK_PAGE_ATTACHMENT_LABEL
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (FEEDBACK_PAGE_ATTACHMENT_LABEL), AppResource.resourceCulture);
    }
  }

  public static string FEEDBACK_PAGE_ATTACHMENT_LABEL_NONE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (FEEDBACK_PAGE_ATTACHMENT_LABEL_NONE), AppResource.resourceCulture);
    }
  }

  public static string FEEDBACK_PAGE_ATTACHMENT_PLUS_HISTORY_LABEL
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (FEEDBACK_PAGE_ATTACHMENT_PLUS_HISTORY_LABEL), AppResource.resourceCulture);
    }
  }

  public static string FEEDBACK_PAGE_ERROR_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (FEEDBACK_PAGE_ERROR_TEXT), AppResource.resourceCulture);
    }
  }

  public static string FEEDBACK_PAGE_FROM_LABEL
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (FEEDBACK_PAGE_FROM_LABEL), AppResource.resourceCulture);
    }
  }

  public static string FEEDBACK_PAGE_GOONLINE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (FEEDBACK_PAGE_GOONLINE), AppResource.resourceCulture);
    }
  }

  public static string FEEDBACK_PAGE_NOT_RELATED_TO_ENUMBER
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (FEEDBACK_PAGE_NOT_RELATED_TO_ENUMBER), AppResource.resourceCulture);
    }
  }

  public static string FEEDBACK_PAGE_NOTE_LABEL
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (FEEDBACK_PAGE_NOTE_LABEL), AppResource.resourceCulture);
    }
  }

  public static string FEEDBACK_PAGE_SEND_BUTTON
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (FEEDBACK_PAGE_SEND_BUTTON), AppResource.resourceCulture);
    }
  }

  public static string FEEDBACK_PAGE_SENT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (FEEDBACK_PAGE_SENT), AppResource.resourceCulture);
    }
  }

  public static string FEEDBACK_PAGE_TITLE_LABEL
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (FEEDBACK_PAGE_TITLE_LABEL), AppResource.resourceCulture);
    }
  }

  public static string FEEDBACK_PAGE_TO_LABEL
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (FEEDBACK_PAGE_TO_LABEL), AppResource.resourceCulture);
    }
  }

  public static string FILE_SIZE_TO_BE_DOWNLOADED_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (FILE_SIZE_TO_BE_DOWNLOADED_TEXT), AppResource.resourceCulture);
    }
  }

  public static string FILTERING_OPTIONS_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (FILTERING_OPTIONS_TEXT), AppResource.resourceCulture);
    }
  }

  public static string FINISHED_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (FINISHED_TEXT), AppResource.resourceCulture);
    }
  }

  public static string FINISHED_TEXT_CODING
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (FINISHED_TEXT_CODING), AppResource.resourceCulture);
    }
  }

  public static string FIRMWARE_DL_FAIL_MSG
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (FIRMWARE_DL_FAIL_MSG), AppResource.resourceCulture);
    }
  }

  public static string FIRMWARE_DL_NOT_FOUND_MSG
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (FIRMWARE_DL_NOT_FOUND_MSG), AppResource.resourceCulture);
    }
  }

  public static string FIRMWARE_LANGUAGE_LABEL
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (FIRMWARE_LANGUAGE_LABEL), AppResource.resourceCulture);
    }
  }

  public static string FIRMWARE_SINGLE_DL_FAIL_MSG
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (FIRMWARE_SINGLE_DL_FAIL_MSG), AppResource.resourceCulture);
    }
  }

  public static string FLASH
  {
    get => AppResource.ResourceManager.GetString(nameof (FLASH), AppResource.resourceCulture);
  }

  public static string FLASH_PAGE_POPUPSTRING
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (FLASH_PAGE_POPUPSTRING), AppResource.resourceCulture);
    }
  }

  public static string FLASH_PAGE_SWITCH_TO_ELP_MSG
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (FLASH_PAGE_SWITCH_TO_ELP_MSG), AppResource.resourceCulture);
    }
  }

  public static string FLASH_SUB_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (FLASH_SUB_TEXT), AppResource.resourceCulture);
    }
  }

  public static string FLASHING_TRANSITION
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (FLASHING_TRANSITION), AppResource.resourceCulture);
    }
  }

  public static string FLASHING_TRANSITION_FROM_CODING
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (FLASHING_TRANSITION_FROM_CODING), AppResource.resourceCulture);
    }
  }

  public static string FP_POPUP_CANCEL
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (FP_POPUP_CANCEL), AppResource.resourceCulture);
    }
  }

  public static string FP_POPUP_HEADER
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (FP_POPUP_HEADER), AppResource.resourceCulture);
    }
  }

  public static string FP_POPUP_PROMPT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (FP_POPUP_PROMPT), AppResource.resourceCulture);
    }
  }

  public static string FULL_DOWNLOAD_DESCRIPTION_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (FULL_DOWNLOAD_DESCRIPTION_TEXT), AppResource.resourceCulture);
    }
  }

  public static string FULL_DOWNLOAD_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (FULL_DOWNLOAD_TEXT), AppResource.resourceCulture);
    }
  }

  public static string FUNCTION_USED_ON
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (FUNCTION_USED_ON), AppResource.resourceCulture);
    }
  }

  public static string GENERIC_ERROR_MESSAGE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (GENERIC_ERROR_MESSAGE), AppResource.resourceCulture);
    }
  }

  public static string Go_TO_WIFI_BRIDGE_SETTINGS
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (Go_TO_WIFI_BRIDGE_SETTINGS), AppResource.resourceCulture);
    }
  }

  public static string GRAPHIC_PAGE_CONNECT_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (GRAPHIC_PAGE_CONNECT_TEXT), AppResource.resourceCulture);
    }
  }

  public static string GRAPHIC_PAGE_CONNECT_TEXT_SUBHEADER
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (GRAPHIC_PAGE_CONNECT_TEXT_SUBHEADER), AppResource.resourceCulture);
    }
  }

  public static string HA_INFO_RETRY_MESSAGE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (HA_INFO_RETRY_MESSAGE), AppResource.resourceCulture);
    }
  }

  public static string HA_INFO_RETRY_MESSAGE_IOS_14
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (HA_INFO_RETRY_MESSAGE_IOS_14), AppResource.resourceCulture);
    }
  }

  public static string HA_INFO_RETRY_MESSAGE_MOBILE_DATA_ENABLED
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (HA_INFO_RETRY_MESSAGE_MOBILE_DATA_ENABLED), AppResource.resourceCulture);
    }
  }

  public static string HISTORY_DETAILS_HEADER_CODING_LOG
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (HISTORY_DETAILS_HEADER_CODING_LOG), AppResource.resourceCulture);
    }
  }

  public static string HISTORY_DETAILS_HEADER_DATE_TIME_OF_REPAIR
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (HISTORY_DETAILS_HEADER_DATE_TIME_OF_REPAIR), AppResource.resourceCulture);
    }
  }

  public static string HISTORY_DETAILS_HEADER_ERROR_LOG
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (HISTORY_DETAILS_HEADER_ERROR_LOG), AppResource.resourceCulture);
    }
  }

  public static string HISTORY_DETAILS_HEADER_FLASH_LOG
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (HISTORY_DETAILS_HEADER_FLASH_LOG), AppResource.resourceCulture);
    }
  }

  public static string HISTORY_DETAILS_HEADER_HOME_APPLIANCE_DATA
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (HISTORY_DETAILS_HEADER_HOME_APPLIANCE_DATA), AppResource.resourceCulture);
    }
  }

  public static string HISTORY_DETAILS_HEADER_MEASURE_LOG
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (HISTORY_DETAILS_HEADER_MEASURE_LOG), AppResource.resourceCulture);
    }
  }

  public static string HISTORY_DETAILS_HEADER_MEMORY_LOG
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (HISTORY_DETAILS_HEADER_MEMORY_LOG), AppResource.resourceCulture);
    }
  }

  public static string HISTORY_DETAILS_HEADER_MONITORING_LOG
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (HISTORY_DETAILS_HEADER_MONITORING_LOG), AppResource.resourceCulture);
    }
  }

  public static string HISTORY_DETAILS_HEADER_RIS
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (HISTORY_DETAILS_HEADER_RIS), AppResource.resourceCulture);
    }
  }

  public static string HISTORY_UNDER_CONSTRUCTION
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (HISTORY_UNDER_CONSTRUCTION), AppResource.resourceCulture);
    }
  }

  public static string HOME_CONNECT_HEADER
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (HOME_CONNECT_HEADER), AppResource.resourceCulture);
    }
  }

  public static string HOME_PAGE_BUTTON
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (HOME_PAGE_BUTTON), AppResource.resourceCulture);
    }
  }

  public static string HOME_PAGE_DATABASE_STATUS
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (HOME_PAGE_DATABASE_STATUS), AppResource.resourceCulture);
    }
  }

  public static string HOME_PAGE_ENUMBER_LABEL_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (HOME_PAGE_ENUMBER_LABEL_TEXT), AppResource.resourceCulture);
    }
  }

  public static string HOME_PAGE_HEADER
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (HOME_PAGE_HEADER), AppResource.resourceCulture);
    }
  }

  public static string IMPRINT_DATA_1
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (IMPRINT_DATA_1), AppResource.resourceCulture);
    }
  }

  public static string IMPRINT_DATA_2
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (IMPRINT_DATA_2), AppResource.resourceCulture);
    }
  }

  public static string IMPRINT_DATA_3_EMAIL
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (IMPRINT_DATA_3_EMAIL), AppResource.resourceCulture);
    }
  }

  public static string IMPRINT_DATA_3_PHONENO
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (IMPRINT_DATA_3_PHONENO), AppResource.resourceCulture);
    }
  }

  public static string IMPRINT_DATA_4
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (IMPRINT_DATA_4), AppResource.resourceCulture);
    }
  }

  public static string IMPRINT_DATA_5
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (IMPRINT_DATA_5), AppResource.resourceCulture);
    }
  }

  public static string IMPRINT_DATA_6
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (IMPRINT_DATA_6), AppResource.resourceCulture);
    }
  }

  public static string IMPRINT_HEADER_1
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (IMPRINT_HEADER_1), AppResource.resourceCulture);
    }
  }

  public static string IMPRINT_HEADER_2
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (IMPRINT_HEADER_2), AppResource.resourceCulture);
    }
  }

  public static string IMPRINT_HEADER_3
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (IMPRINT_HEADER_3), AppResource.resourceCulture);
    }
  }

  public static string IMPRINT_HEADER_4
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (IMPRINT_HEADER_4), AppResource.resourceCulture);
    }
  }

  public static string IMPRINT_HEADER_5
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (IMPRINT_HEADER_5), AppResource.resourceCulture);
    }
  }

  public static string IMPRINT_HEADER_6
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (IMPRINT_HEADER_6), AppResource.resourceCulture);
    }
  }

  public static string IMPRINT_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (IMPRINT_TEXT), AppResource.resourceCulture);
    }
  }

  public static string INAPPBROWSER_BRIDGE_SETTING_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (INAPPBROWSER_BRIDGE_SETTING_TEXT), AppResource.resourceCulture);
    }
  }

  public static string INCORRECT_CREDENTIALS
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (INCORRECT_CREDENTIALS), AppResource.resourceCulture);
    }
  }

  public static string INFORMATION_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (INFORMATION_TEXT), AppResource.resourceCulture);
    }
  }

  public static string INFORMATION_TITLE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (INFORMATION_TITLE), AppResource.resourceCulture);
    }
  }

  public static string INITIALISING_SERVICES
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (INITIALISING_SERVICES), AppResource.resourceCulture);
    }
  }

  public static string INITIALIZAING_WEBSOCKET_CON
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (INITIALIZAING_WEBSOCKET_CON), AppResource.resourceCulture);
    }
  }

  public static string INSTALL_REPAIR_ECU
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (INSTALL_REPAIR_ECU), AppResource.resourceCulture);
    }
  }

  public static string INSTALLED_ECU
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (INSTALLED_ECU), AppResource.resourceCulture);
    }
  }

  public static string INSTALLED_FIRMWARE_AFTER_PROGRAMMING
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (INSTALLED_FIRMWARE_AFTER_PROGRAMMING), AppResource.resourceCulture);
    }
  }

  public static string INSTALLED_FIRMWARE_BEFORE_PROGRAMMING
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (INSTALLED_FIRMWARE_BEFORE_PROGRAMMING), AppResource.resourceCulture);
    }
  }

  public static string INSTALLED_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (INSTALLED_TEXT), AppResource.resourceCulture);
    }
  }

  public static string INSTALLED_UNKNOWN_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (INSTALLED_UNKNOWN_TEXT), AppResource.resourceCulture);
    }
  }

  public static string INTERNET_ERROR_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (INTERNET_ERROR_TEXT), AppResource.resourceCulture);
    }
  }

  public static string JAILBROKEN_WARNING_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (JAILBROKEN_WARNING_TEXT), AppResource.resourceCulture);
    }
  }

  public static string JWT_TOKEN_EXPIRED
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (JWT_TOKEN_EXPIRED), AppResource.resourceCulture);
    }
  }

  public static string LEGAL_INFO_HEADER
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (LEGAL_INFO_HEADER), AppResource.resourceCulture);
    }
  }

  public static string LEGAL_INFO_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (LEGAL_INFO_TEXT), AppResource.resourceCulture);
    }
  }

  public static string LOCAION_PERMISSION_RATIONALE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (LOCAION_PERMISSION_RATIONALE), AppResource.resourceCulture);
    }
  }

  public static string LOCAL_NETWORK_PERMISSION_POPUP
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (LOCAL_NETWORK_PERMISSION_POPUP), AppResource.resourceCulture);
    }
  }

  public static string LOCATION_ENABLE_ALERT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (LOCATION_ENABLE_ALERT), AppResource.resourceCulture);
    }
  }

  public static string LOCATION_ENABLE_ALERT_MSG
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (LOCATION_ENABLE_ALERT_MSG), AppResource.resourceCulture);
    }
  }

  public static string LOCATION_ENABLE_ALERT_TITLE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (LOCATION_ENABLE_ALERT_TITLE), AppResource.resourceCulture);
    }
  }

  public static string LOCATION_PERMISSION_RATIONALE_LATEST_ANDROID
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (LOCATION_PERMISSION_RATIONALE_LATEST_ANDROID), AppResource.resourceCulture);
    }
  }

  public static string LOG_MAIN_REDOWNLOAD
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (LOG_MAIN_REDOWNLOAD), AppResource.resourceCulture);
    }
  }

  public static string LOG_SESSION_DOWNLOAD_BUTTON
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (LOG_SESSION_DOWNLOAD_BUTTON), AppResource.resourceCulture);
    }
  }

  public static string LOG_SESSION_DOWNLOADED
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (LOG_SESSION_DOWNLOADED), AppResource.resourceCulture);
    }
  }

  public static string LOG_SESSION_ERROR
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (LOG_SESSION_ERROR), AppResource.resourceCulture);
    }
  }

  public static string LOGIN_PAGE_AUTO_LOGIN_SWITCH
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (LOGIN_PAGE_AUTO_LOGIN_SWITCH), AppResource.resourceCulture);
    }
  }

  public static string LOGIN_PAGE_LOGIN_BUTTON
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (LOGIN_PAGE_LOGIN_BUTTON), AppResource.resourceCulture);
    }
  }

  public static string LOGIN_PAGE_LOGIN_FAILURE_INDICATION_LOCAL
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (LOGIN_PAGE_LOGIN_FAILURE_INDICATION_LOCAL), AppResource.resourceCulture);
    }
  }

  public static string LOGIN_PAGE_LOGIN_PROMPT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (LOGIN_PAGE_LOGIN_PROMPT), AppResource.resourceCulture);
    }
  }

  public static string LOGIN_PAGE_NEED_ACCOUNT_BUTTON
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (LOGIN_PAGE_NEED_ACCOUNT_BUTTON), AppResource.resourceCulture);
    }
  }

  public static string LOGIN_PAGE_PASSWORD_PLACEHOLDER
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (LOGIN_PAGE_PASSWORD_PLACEHOLDER), AppResource.resourceCulture);
    }
  }

  public static string LOGIN_PAGE_USERNAME_PLACEHOLDER
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (LOGIN_PAGE_USERNAME_PLACEHOLDER), AppResource.resourceCulture);
    }
  }

  public static string LOGIN_UNAUTHORIZED
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (LOGIN_UNAUTHORIZED), AppResource.resourceCulture);
    }
  }

  public static string LoginSuccess
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (LoginSuccess), AppResource.resourceCulture);
    }
  }

  public static string MEASURE
  {
    get => AppResource.ResourceManager.GetString(nameof (MEASURE), AppResource.resourceCulture);
  }

  public static string MEASURE_CAT_RESET_COMMAND_SENT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (MEASURE_CAT_RESET_COMMAND_SENT), AppResource.resourceCulture);
    }
  }

  public static string MEASURE_CAT_RESET_TITLE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (MEASURE_CAT_RESET_TITLE), AppResource.resourceCulture);
    }
  }

  public static string MEASURE_DIAGRAM_X_AXIS
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (MEASURE_DIAGRAM_X_AXIS), AppResource.resourceCulture);
    }
  }

  public static string MEASURE_TITLE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (MEASURE_TITLE), AppResource.resourceCulture);
    }
  }

  public static string MEMORY_PAGE_NOTSUPPORTED
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (MEMORY_PAGE_NOTSUPPORTED), AppResource.resourceCulture);
    }
  }

  public static string MEMORY_PAGE_NOTSUPPORTED_TITLE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (MEMORY_PAGE_NOTSUPPORTED_TITLE), AppResource.resourceCulture);
    }
  }

  public static string MEMORY_TRANSITION_PAGE_COLLECTING
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (MEMORY_TRANSITION_PAGE_COLLECTING), AppResource.resourceCulture);
    }
  }

  public static string METADATA_DL_CHECK_SUM_FAIL
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (METADATA_DL_CHECK_SUM_FAIL), AppResource.resourceCulture);
    }
  }

  public static string METADATA_DL_FAIL_MSG
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (METADATA_DL_FAIL_MSG), AppResource.resourceCulture);
    }
  }

  public static string METADATA_DL_FAIL_TITLE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (METADATA_DL_FAIL_TITLE), AppResource.resourceCulture);
    }
  }

  public static string METADATA_DOWNLOAD_FIRST_TIME_PERMISSION_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (METADATA_DOWNLOAD_FIRST_TIME_PERMISSION_TEXT), AppResource.resourceCulture);
    }
  }

  public static string METADATA_DOWNLOAD_PERMISSION_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (METADATA_DOWNLOAD_PERMISSION_TEXT), AppResource.resourceCulture);
    }
  }

  public static string METADATA_DOWNLOAD_UPDATE_PERMISSION_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (METADATA_DOWNLOAD_UPDATE_PERMISSION_TEXT), AppResource.resourceCulture);
    }
  }

  public static string MONITORING
  {
    get => AppResource.ResourceManager.GetString(nameof (MONITORING), AppResource.resourceCulture);
  }

  public static string MONITORING_NO_SCREENS_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (MONITORING_NO_SCREENS_TEXT), AppResource.resourceCulture);
    }
  }

  public static string MONITORING_PARSING_ERROR
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (MONITORING_PARSING_ERROR), AppResource.resourceCulture);
    }
  }

  public static string MONITORING_START_FAILED
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (MONITORING_START_FAILED), AppResource.resourceCulture);
    }
  }

  public static string MSG_FLASHING_ERROR
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (MSG_FLASHING_ERROR), AppResource.resourceCulture);
    }
  }

  public static string MSG_FLASHING_ERROR_TIMEOUT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (MSG_FLASHING_ERROR_TIMEOUT), AppResource.resourceCulture);
    }
  }

  public static string MSG_FLASHING_IS_FINISHED
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (MSG_FLASHING_IS_FINISHED), AppResource.resourceCulture);
    }
  }

  public static string NAVIGATE_HOME_BTN_LABEL
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (NAVIGATE_HOME_BTN_LABEL), AppResource.resourceCulture);
    }
  }

  public static string NAVIGATE_REPAIR_BTN_LABEL
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (NAVIGATE_REPAIR_BTN_LABEL), AppResource.resourceCulture);
    }
  }

  public static string NEED_ACCOUNT_PAGE_DETAILS_LABEL
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (NEED_ACCOUNT_PAGE_DETAILS_LABEL), AppResource.resourceCulture);
    }
  }

  public static string NEED_ACCOUNT_PAGE_TITLE_LABEL
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (NEED_ACCOUNT_PAGE_TITLE_LABEL), AppResource.resourceCulture);
    }
  }

  public static string NO_ERROR_LOG_TO_DISPLAY
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (NO_ERROR_LOG_TO_DISPLAY), AppResource.resourceCulture);
    }
  }

  public static string NO_HISTORY_TO_DISPLAY
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (NO_HISTORY_TO_DISPLAY), AppResource.resourceCulture);
    }
  }

  public static string NO_INTERNET
  {
    get => AppResource.ResourceManager.GetString(nameof (NO_INTERNET), AppResource.resourceCulture);
  }

  public static string NO_LOCAL_CREDENTIALS
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (NO_LOCAL_CREDENTIALS), AppResource.resourceCulture);
    }
  }

  public static string NO_MEMORY_LOG_TO_DISPLAY
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (NO_MEMORY_LOG_TO_DISPLAY), AppResource.resourceCulture);
    }
  }

  public static string NO_METADATA
  {
    get => AppResource.ResourceManager.GetString(nameof (NO_METADATA), AppResource.resourceCulture);
  }

  public static string NO_MODULES_FOUND_IN_DB
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (NO_MODULES_FOUND_IN_DB), AppResource.resourceCulture);
    }
  }

  public static string NO_TEXT
  {
    get => AppResource.ResourceManager.GetString(nameof (NO_TEXT), AppResource.resourceCulture);
  }

  public static string NON_SMM_LOG_BUTTON
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (NON_SMM_LOG_BUTTON), AppResource.resourceCulture);
    }
  }

  public static string NON_SMM_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (NON_SMM_TEXT), AppResource.resourceCulture);
    }
  }

  public static string NONSMM_MEMORY_LOG_MSGD_LABEL
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (NONSMM_MEMORY_LOG_MSGD_LABEL), AppResource.resourceCulture);
    }
  }

  public static string NONSMM_MEMORY_LOG_MSGV_LABEL
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (NONSMM_MEMORY_LOG_MSGV_LABEL), AppResource.resourceCulture);
    }
  }

  public static string NONSMM_UPLOAD_TRANSITION_PAGE_COLLECTING
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (NONSMM_UPLOAD_TRANSITION_PAGE_COLLECTING), AppResource.resourceCulture);
    }
  }

  public static string NOT_INSTALLED_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (NOT_INSTALLED_TEXT), AppResource.resourceCulture);
    }
  }

  public static string NOT_NOW_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (NOT_NOW_TEXT), AppResource.resourceCulture);
    }
  }

  public static string NOT_YET_AVAILABLE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (NOT_YET_AVAILABLE), AppResource.resourceCulture);
    }
  }

  public static string NUMBER_OF_FILES_TO_BE_DOWNLOADED_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (NUMBER_OF_FILES_TO_BE_DOWNLOADED_TEXT), AppResource.resourceCulture);
    }
  }

  public static string OFFLINE_SESSION_WARNING
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (OFFLINE_SESSION_WARNING), AppResource.resourceCulture);
    }
  }

  public static string OPTION_TEXT
  {
    get => AppResource.ResourceManager.GetString(nameof (OPTION_TEXT), AppResource.resourceCulture);
  }

  public static string OVERFLOW_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (OVERFLOW_TEXT), AppResource.resourceCulture);
    }
  }

  public static string PASSWORD_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (PASSWORD_TEXT), AppResource.resourceCulture);
    }
  }

  public static string PE_CHECK_FAILED_WARNING_MESSAGE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (PE_CHECK_FAILED_WARNING_MESSAGE), AppResource.resourceCulture);
    }
  }

  public static string PE_CHECK_FAILED_WARNING_TITLE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (PE_CHECK_FAILED_WARNING_TITLE), AppResource.resourceCulture);
    }
  }

  public static string PLEASE_CHOOSE_DOWNLOAD_SETTINGS_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (PLEASE_CHOOSE_DOWNLOAD_SETTINGS_TEXT), AppResource.resourceCulture);
    }
  }

  public static string PPF_FILES_MISSING
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (PPF_FILES_MISSING), AppResource.resourceCulture);
    }
  }

  public static string PPF_REFETCH_IN_SETTINGS_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (PPF_REFETCH_IN_SETTINGS_TEXT), AppResource.resourceCulture);
    }
  }

  public static string PPF_REFETCH_INFORMATION_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (PPF_REFETCH_INFORMATION_TEXT), AppResource.resourceCulture);
    }
  }

  public static string PPF_REFETCH_SKIP_MESSAGE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (PPF_REFETCH_SKIP_MESSAGE), AppResource.resourceCulture);
    }
  }

  public static string PREPARE_RE_DOWNLOAD_INSTRUCTIONS
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (PREPARE_RE_DOWNLOAD_INSTRUCTIONS), AppResource.resourceCulture);
    }
  }

  public static string PREPARE_WORK_ADD_BUTTON
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (PREPARE_WORK_ADD_BUTTON), AppResource.resourceCulture);
    }
  }

  public static string PREPARE_WORK_INSTRUCTIONS
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (PREPARE_WORK_INSTRUCTIONS), AppResource.resourceCulture);
    }
  }

  public static string PREPARE_YOUR_WORK_INVALID_ENUMBERS
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (PREPARE_YOUR_WORK_INVALID_ENUMBERS), AppResource.resourceCulture);
    }
  }

  public static string PREPARE_YOUR_WORK_TITLE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (PREPARE_YOUR_WORK_TITLE), AppResource.resourceCulture);
    }
  }

  public static string PROCEED_BUTTON_LABEL
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (PROCEED_BUTTON_LABEL), AppResource.resourceCulture);
    }
  }

  public static string PROCESSING_DATA
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (PROCESSING_DATA), AppResource.resourceCulture);
    }
  }

  public static string PROGRAM
  {
    get => AppResource.ResourceManager.GetString(nameof (PROGRAM), AppResource.resourceCulture);
  }

  public static string PROGRAM_ALL_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (PROGRAM_ALL_TEXT), AppResource.resourceCulture);
    }
  }

  public static string PROGRAM_ONLY_NEW_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (PROGRAM_ONLY_NEW_TEXT), AppResource.resourceCulture);
    }
  }

  public static string PROGRAM_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (PROGRAM_TEXT), AppResource.resourceCulture);
    }
  }

  public static string PROGRAMMING_FAILED
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (PROGRAMMING_FAILED), AppResource.resourceCulture);
    }
  }

  public static string PROGRAMMING_FAILED_WITH_RESULT_CODE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (PROGRAMMING_FAILED_WITH_RESULT_CODE), AppResource.resourceCulture);
    }
  }

  public static string PROGRAMMING_RETURN_FROM_UPTODATE_POPUP
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (PROGRAMMING_RETURN_FROM_UPTODATE_POPUP), AppResource.resourceCulture);
    }
  }

  public static string RE_DOWNLOAD
  {
    get => AppResource.ResourceManager.GetString(nameof (RE_DOWNLOAD), AppResource.resourceCulture);
  }

  public static string RE_DOWNLOAD_ENUMBER_VIEW
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (RE_DOWNLOAD_ENUMBER_VIEW), AppResource.resourceCulture);
    }
  }

  public static string RE_DOWNLOAD_ENUMBERS
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (RE_DOWNLOAD_ENUMBERS), AppResource.resourceCulture);
    }
  }

  public static string READ_INVENTORY_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (READ_INVENTORY_TEXT), AppResource.resourceCulture);
    }
  }

  public static string RECOMMENDED
  {
    get => AppResource.ResourceManager.GetString(nameof (RECOMMENDED), AppResource.resourceCulture);
  }

  public static string RECOVERY_PAGE_POPUP_MSG
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (RECOVERY_PAGE_POPUP_MSG), AppResource.resourceCulture);
    }
  }

  public static string REFETCH_PPFs
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (REFETCH_PPFs), AppResource.resourceCulture);
    }
  }

  public static string REFRESH_CERTIFICATE_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (REFRESH_CERTIFICATE_TEXT), AppResource.resourceCulture);
    }
  }

  public static string REINSTALLED_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (REINSTALLED_TEXT), AppResource.resourceCulture);
    }
  }

  public static string RELATED_REPAIR_VIST
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (RELATED_REPAIR_VIST), AppResource.resourceCulture);
    }
  }

  public static string REPAIR_PAGE_APPLIANCE_DATA_UPD_REQUIRED
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (REPAIR_PAGE_APPLIANCE_DATA_UPD_REQUIRED), AppResource.resourceCulture);
    }
  }

  public static string REPAIR_PAGE_CHECKING_APPL_DATA
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (REPAIR_PAGE_CHECKING_APPL_DATA), AppResource.resourceCulture);
    }
  }

  public static string REPAIR_PAGE_CONTINUE_WITHOUT_DOWNLOAD
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (REPAIR_PAGE_CONTINUE_WITHOUT_DOWNLOAD), AppResource.resourceCulture);
    }
  }

  public static string REPAIR_PAGE_DOWNLOAD
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (REPAIR_PAGE_DOWNLOAD), AppResource.resourceCulture);
    }
  }

  public static string REPAIR_PAGE_DOWNLOAD_BUTTON
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (REPAIR_PAGE_DOWNLOAD_BUTTON), AppResource.resourceCulture);
    }
  }

  public static string REPAIR_PAGE_DOWNLOAD_FILES_PROGRESS
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (REPAIR_PAGE_DOWNLOAD_FILES_PROGRESS), AppResource.resourceCulture);
    }
  }

  public static string REPAIR_PAGE_DOWNLOADING
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (REPAIR_PAGE_DOWNLOADING), AppResource.resourceCulture);
    }
  }

  public static string REPAIR_PAGE_DOWNLOADING2
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (REPAIR_PAGE_DOWNLOADING2), AppResource.resourceCulture);
    }
  }

  public static string REPAIR_PAGE_FILES
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (REPAIR_PAGE_FILES), AppResource.resourceCulture);
    }
  }

  public static string REPAIR_PAGE_FILES2
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (REPAIR_PAGE_FILES2), AppResource.resourceCulture);
    }
  }

  public static string REPORT_A_PROBLEM_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (REPORT_A_PROBLEM_TEXT), AppResource.resourceCulture);
    }
  }

  public static string REPORT_PROBLEM_COMMUNICATION_ERROR
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (REPORT_PROBLEM_COMMUNICATION_ERROR), AppResource.resourceCulture);
    }
  }

  public static string RETRIEVING_CERTIFICATE_NOT_SUCCESSFUL_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (RETRIEVING_CERTIFICATE_NOT_SUCCESSFUL_TEXT), AppResource.resourceCulture);
    }
  }

  public static string RETRIEVING_CERTIFICATE_SUCCESSFUL_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (RETRIEVING_CERTIFICATE_SUCCESSFUL_TEXT), AppResource.resourceCulture);
    }
  }

  public static string RETRIEVING_CERTIFICATE_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (RETRIEVING_CERTIFICATE_TEXT), AppResource.resourceCulture);
    }
  }

  public static string RETRY_LABEL
  {
    get => AppResource.ResourceManager.GetString(nameof (RETRY_LABEL), AppResource.resourceCulture);
  }

  public static string SAVE_GRAPH_ALERT_MESSAGE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (SAVE_GRAPH_ALERT_MESSAGE), AppResource.resourceCulture);
    }
  }

  public static string SAVE_GRAPH_ALERT_TITLE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (SAVE_GRAPH_ALERT_TITLE), AppResource.resourceCulture);
    }
  }

  public static string SAVE_GRAPH_PERMISSION_DENIED_ALERT_MESSAGE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (SAVE_GRAPH_PERMISSION_DENIED_ALERT_MESSAGE), AppResource.resourceCulture);
    }
  }

  public static string SAVE_TEXT
  {
    get => AppResource.ResourceManager.GetString(nameof (SAVE_TEXT), AppResource.resourceCulture);
  }

  public static string SCHEMA_CHANGE_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (SCHEMA_CHANGE_TEXT), AppResource.resourceCulture);
    }
  }

  public static string SELECT_DEVICE_CLASS_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (SELECT_DEVICE_CLASS_TEXT), AppResource.resourceCulture);
    }
  }

  public static string SELECTED_FOR_DOWNLOAD_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (SELECTED_FOR_DOWNLOAD_TEXT), AppResource.resourceCulture);
    }
  }

  public static string SERVER_CERTIFICATE_EXPIRED
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (SERVER_CERTIFICATE_EXPIRED), AppResource.resourceCulture);
    }
  }

  public static string SERVER_CERTIFICATE_UNSECURE_CONNECTION
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (SERVER_CERTIFICATE_UNSECURE_CONNECTION), AppResource.resourceCulture);
    }
  }

  public static string SERVER_TRUST_FAILURE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (SERVER_TRUST_FAILURE), AppResource.resourceCulture);
    }
  }

  public static string SKIP_UPDATE
  {
    get => AppResource.ResourceManager.GetString(nameof (SKIP_UPDATE), AppResource.resourceCulture);
  }

  public static string SMM_APPLIANCE_IS_REBOOTING_TO_ELP
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (SMM_APPLIANCE_IS_REBOOTING_TO_ELP), AppResource.resourceCulture);
    }
  }

  public static string SMM_APPLIANCE_IS_REBOOTING_TO_RECOVERY
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (SMM_APPLIANCE_IS_REBOOTING_TO_RECOVERY), AppResource.resourceCulture);
    }
  }

  public static string SMM_CONN_BOOT_MODE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (SMM_CONN_BOOT_MODE), AppResource.resourceCulture);
    }
  }

  public static string SMM_CONN_BOOTMODE_NO_VALUE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (SMM_CONN_BOOTMODE_NO_VALUE), AppResource.resourceCulture);
    }
  }

  public static string SMM_CONN_BOSCH
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (SMM_CONN_BOSCH), AppResource.resourceCulture);
    }
  }

  public static string SMM_CONN_BRAND
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (SMM_CONN_BRAND), AppResource.resourceCulture);
    }
  }

  public static string SMM_CONN_COUNTRY_SETTINGS
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (SMM_CONN_COUNTRY_SETTINGS), AppResource.resourceCulture);
    }
  }

  public static string SMM_CONN_CUST_INDEX
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (SMM_CONN_CUST_INDEX), AppResource.resourceCulture);
    }
  }

  public static string SMM_CONN_DEVICE_BOOT_MODE_SET
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (SMM_CONN_DEVICE_BOOT_MODE_SET), AppResource.resourceCulture);
    }
  }

  public static string SMM_CONN_DEVICE_CANNOT_PROCEED
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (SMM_CONN_DEVICE_CANNOT_PROCEED), AppResource.resourceCulture);
    }
  }

  public static string SMM_CONN_DEVICE_CONNECTED
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (SMM_CONN_DEVICE_CONNECTED), AppResource.resourceCulture);
    }
  }

  public static string SMM_CONN_DEVICE_COULD_NOT_SECURED
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (SMM_CONN_DEVICE_COULD_NOT_SECURED), AppResource.resourceCulture);
    }
  }

  public static string SMM_CONN_DEVICE_COULD_NOT_SET_BOOTMODE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (SMM_CONN_DEVICE_COULD_NOT_SET_BOOTMODE), AppResource.resourceCulture);
    }
  }

  public static string SMM_CONN_DEVICE_DISCONNECTED
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (SMM_CONN_DEVICE_DISCONNECTED), AppResource.resourceCulture);
    }
  }

  public static string SMM_CONN_DEVICE_FAILED_TO_COMMUNICATE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (SMM_CONN_DEVICE_FAILED_TO_COMMUNICATE), AppResource.resourceCulture);
    }
  }

  public static string SMM_CONN_DEVICE_HAS_BEEN_SECURED
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (SMM_CONN_DEVICE_HAS_BEEN_SECURED), AppResource.resourceCulture);
    }
  }

  public static string SMM_CONN_DEVICE_IS_NOT_SECURED
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (SMM_CONN_DEVICE_IS_NOT_SECURED), AppResource.resourceCulture);
    }
  }

  public static string SMM_CONN_DEVICE_IS_SECURED
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (SMM_CONN_DEVICE_IS_SECURED), AppResource.resourceCulture);
    }
  }

  public static string SMM_CONN_DEVICE_NOT_COMPLETE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (SMM_CONN_DEVICE_NOT_COMPLETE), AppResource.resourceCulture);
    }
  }

  public static string SMM_CONN_DEVICE_REBOOT_FAILED
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (SMM_CONN_DEVICE_REBOOT_FAILED), AppResource.resourceCulture);
    }
  }

  public static string SMM_CONN_DEVICE_REQUESTED_REBOOT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (SMM_CONN_DEVICE_REQUESTED_REBOOT), AppResource.resourceCulture);
    }
  }

  public static string SMM_CONN_DEVICE_REQUESTING_REBOOT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (SMM_CONN_DEVICE_REQUESTING_REBOOT), AppResource.resourceCulture);
    }
  }

  public static string SMM_CONN_DEVICE_TYPE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (SMM_CONN_DEVICE_TYPE), AppResource.resourceCulture);
    }
  }

  public static string SMM_CONN_DEVICE_WAITING
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (SMM_CONN_DEVICE_WAITING), AppResource.resourceCulture);
    }
  }

  public static string SMM_CONN_DISHWASHER
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (SMM_CONN_DISHWASHER), AppResource.resourceCulture);
    }
  }

  public static string SMM_CONN_E_NUMBER
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (SMM_CONN_E_NUMBER), AppResource.resourceCulture);
    }
  }

  public static string SMM_CONN_INITIATING
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (SMM_CONN_INITIATING), AppResource.resourceCulture);
    }
  }

  public static string SMM_CONN_MANUF_TIME
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (SMM_CONN_MANUF_TIME), AppResource.resourceCulture);
    }
  }

  public static string SMM_CONN_SERIAL_NUMBER
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (SMM_CONN_SERIAL_NUMBER), AppResource.resourceCulture);
    }
  }

  public static string SMM_CONN_SIEMENS
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (SMM_CONN_SIEMENS), AppResource.resourceCulture);
    }
  }

  public static string SMM_CONN_VIB
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (SMM_CONN_VIB), AppResource.resourceCulture);
    }
  }

  public static string SMM_LOG_BUTTON
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (SMM_LOG_BUTTON), AppResource.resourceCulture);
    }
  }

  public static string SMM_TEXT
  {
    get => AppResource.ResourceManager.GetString(nameof (SMM_TEXT), AppResource.resourceCulture);
  }

  public static string SSH_COMMAND_ERROR
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (SSH_COMMAND_ERROR), AppResource.resourceCulture);
    }
  }

  public static string SSH_COMMAND_ERROR_IOS_14
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (SSH_COMMAND_ERROR_IOS_14), AppResource.resourceCulture);
    }
  }

  public static string ST_NO_INTERNET
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (ST_NO_INTERNET), AppResource.resourceCulture);
    }
  }

  public static string ST_PAGE_APL_FILES
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (ST_PAGE_APL_FILES), AppResource.resourceCulture);
    }
  }

  public static string ST_PAGE_BRIDGE_TOGGLE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (ST_PAGE_BRIDGE_TOGGLE), AppResource.resourceCulture);
    }
  }

  public static string ST_PAGE_CANCEL
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (ST_PAGE_CANCEL), AppResource.resourceCulture);
    }
  }

  public static string ST_PAGE_DOWNLOADING
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (ST_PAGE_DOWNLOADING), AppResource.resourceCulture);
    }
  }

  public static string ST_PAGE_DOWNLOADING2
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (ST_PAGE_DOWNLOADING2), AppResource.resourceCulture);
    }
  }

  public static string ST_PAGE_DWL_ALL
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (ST_PAGE_DWL_ALL), AppResource.resourceCulture);
    }
  }

  public static string ST_PAGE_DWL_BS_HOURS
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (ST_PAGE_DWL_BS_HOURS), AppResource.resourceCulture);
    }
  }

  public static string ST_PAGE_DWL_LOCAL_FILES
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (ST_PAGE_DWL_LOCAL_FILES), AppResource.resourceCulture);
    }
  }

  public static string ST_PAGE_DWL_NO
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (ST_PAGE_DWL_NO), AppResource.resourceCulture);
    }
  }

  public static string ST_PAGE_DWL_NO_FILES
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (ST_PAGE_DWL_NO_FILES), AppResource.resourceCulture);
    }
  }

  public static string ST_PAGE_DWL_NOW
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (ST_PAGE_DWL_NOW), AppResource.resourceCulture);
    }
  }

  public static string ST_PAGE_DWL_REMOTE_FILES
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (ST_PAGE_DWL_REMOTE_FILES), AppResource.resourceCulture);
    }
  }

  public static string ST_PAGE_DWL_REQ
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (ST_PAGE_DWL_REQ), AppResource.resourceCulture);
    }
  }

  public static string ST_PAGE_DWL_SCHD
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (ST_PAGE_DWL_SCHD), AppResource.resourceCulture);
    }
  }

  public static string ST_PAGE_DWL_START
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (ST_PAGE_DWL_START), AppResource.resourceCulture);
    }
  }

  public static string ST_PAGE_DWL_STATUS
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (ST_PAGE_DWL_STATUS), AppResource.resourceCulture);
    }
  }

  public static string ST_PAGE_DWL_TODAY
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (ST_PAGE_DWL_TODAY), AppResource.resourceCulture);
    }
  }

  public static string ST_PAGE_DWL_TOMORROW
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (ST_PAGE_DWL_TOMORROW), AppResource.resourceCulture);
    }
  }

  public static string ST_PAGE_DWL_WH
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (ST_PAGE_DWL_WH), AppResource.resourceCulture);
    }
  }

  public static string ST_PAGE_DWL_WIFI_ONLY
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (ST_PAGE_DWL_WIFI_ONLY), AppResource.resourceCulture);
    }
  }

  public static string ST_PAGE_FAILED_DB
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (ST_PAGE_FAILED_DB), AppResource.resourceCulture);
    }
  }

  public static string ST_PAGE_FAILED_XSUM
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (ST_PAGE_FAILED_XSUM), AppResource.resourceCulture);
    }
  }

  public static string ST_PAGE_FILES
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (ST_PAGE_FILES), AppResource.resourceCulture);
    }
  }

  public static string ST_PAGE_FILES2
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (ST_PAGE_FILES2), AppResource.resourceCulture);
    }
  }

  public static string ST_PAGE_HEADER
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (ST_PAGE_HEADER), AppResource.resourceCulture);
    }
  }

  public static string ST_PAGE_LAST_UPD
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (ST_PAGE_LAST_UPD), AppResource.resourceCulture);
    }
  }

  public static string ST_PAGE_NEVER
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (ST_PAGE_NEVER), AppResource.resourceCulture);
    }
  }

  public static string ST_PAGE_OF
  {
    get => AppResource.ResourceManager.GetString(nameof (ST_PAGE_OF), AppResource.resourceCulture);
  }

  public static string ST_PAGE_OFFLINE_USE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (ST_PAGE_OFFLINE_USE), AppResource.resourceCulture);
    }
  }

  public static string ST_PAGE_USED_STORAGE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (ST_PAGE_USED_STORAGE), AppResource.resourceCulture);
    }
  }

  public static string ST_WIFI_RESTRICTION
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (ST_WIFI_RESTRICTION), AppResource.resourceCulture);
    }
  }

  public static string START_WEB_SOCKET_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (START_WEB_SOCKET_TEXT), AppResource.resourceCulture);
    }
  }

  public static string STATUS_PAGE_CHECKING_METADATA
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (STATUS_PAGE_CHECKING_METADATA), AppResource.resourceCulture);
    }
  }

  public static string STATUS_PAGE_GOONLINE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (STATUS_PAGE_GOONLINE), AppResource.resourceCulture);
    }
  }

  public static string STATUS_PAGE_PREPARING
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (STATUS_PAGE_PREPARING), AppResource.resourceCulture);
    }
  }

  public static string STATUS_PAGE_UPDATE_EXPIRED_PPF
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (STATUS_PAGE_UPDATE_EXPIRED_PPF), AppResource.resourceCulture);
    }
  }

  public static string STATUS_PAGE_UPDATING
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (STATUS_PAGE_UPDATING), AppResource.resourceCulture);
    }
  }

  public static string STATUS_PPF_REFETCHING
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (STATUS_PPF_REFETCHING), AppResource.resourceCulture);
    }
  }

  public static string STORAGE_STATUS
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (STORAGE_STATUS), AppResource.resourceCulture);
    }
  }

  public static string STORED_SPAU
  {
    get => AppResource.ResourceManager.GetString(nameof (STORED_SPAU), AppResource.resourceCulture);
  }

  public static string SVG_PARSING_FAILED
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (SVG_PARSING_FAILED), AppResource.resourceCulture);
    }
  }

  public static string SYMANA_ALL_MODULES_ARE_UP_TO_DATE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (SYMANA_ALL_MODULES_ARE_UP_TO_DATE), AppResource.resourceCulture);
    }
  }

  public static string SYMANA_BINARY_UPLOAD_RETRY_MESSAGE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (SYMANA_BINARY_UPLOAD_RETRY_MESSAGE), AppResource.resourceCulture);
    }
  }

  public static string SYMANA_BINARY_UPLOAD_SUCCESS
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (SYMANA_BINARY_UPLOAD_SUCCESS), AppResource.resourceCulture);
    }
  }

  public static string SYMANA_FAILED_UPLOAD_PACKAGES_MSG
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (SYMANA_FAILED_UPLOAD_PACKAGES_MSG), AppResource.resourceCulture);
    }
  }

  public static string SYMANA_INSTALLATION_ERROR_WITH_WRONG_RESULT_CODE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (SYMANA_INSTALLATION_ERROR_WITH_WRONG_RESULT_CODE), AppResource.resourceCulture);
    }
  }

  public static string SYMANA_INSTALLATION_STARTED_BUTTON
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (SYMANA_INSTALLATION_STARTED_BUTTON), AppResource.resourceCulture);
    }
  }

  public static string SYMANA_PACKAGE_UPLOAD_FAILED
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (SYMANA_PACKAGE_UPLOAD_FAILED), AppResource.resourceCulture);
    }
  }

  public static string SYMANA_PENDING_INSTALLATIONS_ARE_FINISHED
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (SYMANA_PENDING_INSTALLATIONS_ARE_FINISHED), AppResource.resourceCulture);
    }
  }

  public static string SYMANA_PROGRAMMING_CONTINUED
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (SYMANA_PROGRAMMING_CONTINUED), AppResource.resourceCulture);
    }
  }

  public static string SYMANA_PROGRAMMING_SUCCESSFUL
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (SYMANA_PROGRAMMING_SUCCESSFUL), AppResource.resourceCulture);
    }
  }

  public static string SYMANA_SETTING_HAINFO
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (SYMANA_SETTING_HAINFO), AppResource.resourceCulture);
    }
  }

  public static string SYMANA_WEBSOCKET_CONNECTION_INTERRUPTED
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (SYMANA_WEBSOCKET_CONNECTION_INTERRUPTED), AppResource.resourceCulture);
    }
  }

  public static string TAB_TITLE_HISTORY
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (TAB_TITLE_HISTORY), AppResource.resourceCulture);
    }
  }

  public static string TAB_TITLE_REPAIR
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (TAB_TITLE_REPAIR), AppResource.resourceCulture);
    }
  }

  public static string TAB_TITLE_SETTINGS
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (TAB_TITLE_SETTINGS), AppResource.resourceCulture);
    }
  }

  public static string TEST_TIMEOUT_LABEL
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (TEST_TIMEOUT_LABEL), AppResource.resourceCulture);
    }
  }

  public static string TIMEOUT_MESSAGE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (TIMEOUT_MESSAGE), AppResource.resourceCulture);
    }
  }

  public static string TOTAL_FILE_SIZE_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (TOTAL_FILE_SIZE_TEXT), AppResource.resourceCulture);
    }
  }

  public static string TOTAL_FILES_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (TOTAL_FILES_TEXT), AppResource.resourceCulture);
    }
  }

  public static string TOUCH_FAIL_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (TOUCH_FAIL_TEXT), AppResource.resourceCulture);
    }
  }

  public static string TOUCH_OK_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (TOUCH_OK_TEXT), AppResource.resourceCulture);
    }
  }

  public static string TRANSITION_TO_BRIDGE_SETTINGS_TITLE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (TRANSITION_TO_BRIDGE_SETTINGS_TITLE), AppResource.resourceCulture);
    }
  }

  public static string UP_TO_DATE_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (UP_TO_DATE_TEXT), AppResource.resourceCulture);
    }
  }

  public static string UPDATE_TEXT
  {
    get => AppResource.ResourceManager.GetString(nameof (UPDATE_TEXT), AppResource.resourceCulture);
  }

  public static string UPGRADE_TESTER_FIRMWARE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (UPGRADE_TESTER_FIRMWARE), AppResource.resourceCulture);
    }
  }

  public static string UPLOAD_TEXT
  {
    get => AppResource.ResourceManager.GetString(nameof (UPLOAD_TEXT), AppResource.resourceCulture);
  }

  public static string URL_FAIL_MSG
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (URL_FAIL_MSG), AppResource.resourceCulture);
    }
  }

  public static string USED_LIBRARIES_HEADER
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (USED_LIBRARIES_HEADER), AppResource.resourceCulture);
    }
  }

  public static string VALIDATION_TITLE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (VALIDATION_TITLE), AppResource.resourceCulture);
    }
  }

  public static string VALUE_TITLE
  {
    get => AppResource.ResourceManager.GetString(nameof (VALUE_TITLE), AppResource.resourceCulture);
  }

  public static string VARCODING_OPTIONS_LABEL
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (VARCODING_OPTIONS_LABEL), AppResource.resourceCulture);
    }
  }

  public static string WARNING_CANCEL
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (WARNING_CANCEL), AppResource.resourceCulture);
    }
  }

  public static string WARNING_CONFIRM
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (WARNING_CONFIRM), AppResource.resourceCulture);
    }
  }

  public static string WARNING_OK
  {
    get => AppResource.ResourceManager.GetString(nameof (WARNING_OK), AppResource.resourceCulture);
  }

  public static string WARNING_TEXT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (WARNING_TEXT), AppResource.resourceCulture);
    }
  }

  public static string WARNING_TITLE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (WARNING_TITLE), AppResource.resourceCulture);
    }
  }

  public static string WEBSOCKET_CONN_INITIALIZED
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (WEBSOCKET_CONN_INITIALIZED), AppResource.resourceCulture);
    }
  }

  public static string WEBSOCKET_CONN_NOT_INITIALIZED
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (WEBSOCKET_CONN_NOT_INITIALIZED), AppResource.resourceCulture);
    }
  }

  public static string WIFI_INFO_ALERT_MESSAGE
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (WIFI_INFO_ALERT_MESSAGE), AppResource.resourceCulture);
    }
  }

  public static string WIFI_INFO_PAGE_HINT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (WIFI_INFO_PAGE_HINT), AppResource.resourceCulture);
    }
  }

  public static string WIFI_INFO_PAGE_LABEL
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (WIFI_INFO_PAGE_LABEL), AppResource.resourceCulture);
    }
  }

  public static string WIFI_PAGE_AUTH_BUTTON
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (WIFI_PAGE_AUTH_BUTTON), AppResource.resourceCulture);
    }
  }

  public static string WIFI_PAGE_LOGIN_PROMPT
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (WIFI_PAGE_LOGIN_PROMPT), AppResource.resourceCulture);
    }
  }

  public static string WIFI_PAGE_LOGIN_UNAUTHORIZED
  {
    get
    {
      return AppResource.ResourceManager.GetString(nameof (WIFI_PAGE_LOGIN_UNAUTHORIZED), AppResource.resourceCulture);
    }
  }

  public static string YES_TEXT
  {
    get => AppResource.ResourceManager.GetString(nameof (YES_TEXT), AppResource.resourceCulture);
  }
}
