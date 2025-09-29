// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Helpers.LocalMigrationHelper
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Data;
using iService5.Core.Services.User.ActivityLogging;
using MvvmCross;
using System.Collections.Generic;
using System.Threading.Tasks;

#nullable disable
namespace iService5.Core.Services.Helpers;

public class LocalMigrationHelper
{
  private ISecureStorageService _secureStorageService;
  private ILoggingService _loggingService;
  private Dictionary<string, string> _keyExchangePairs = new Dictionary<string, string>()
  {
    {
      "OFFLINE_DAYS",
      "OfflineDaysCount"
    },
    {
      "LAST_USE",
      "SinceAppUsedTimestamp"
    },
    {
      "METADATA_UPDATE_TIMESTAMP",
      "MetadataPostponedTimestamp"
    },
    {
      "JWT_TOKEN_TIMESTAMP",
      "JwtTokenTimestamp"
    },
    {
      "SUPPORT_FORM_SEND_STATUS",
      "SupportFormSendStatus"
    },
    {
      "SUPPORT_FORM_SAVED_SUBJECT",
      "SupportFormSavedSubject"
    },
    {
      "SUPPORT_FORM_SAVED_NOTE",
      "SupportFormSavedNote"
    },
    {
      "SUPPORT_FORM_SAVED_SESSION_LOG_FILENAME",
      "SupportFormSavedSessionLogFileName"
    },
    {
      "NOT_LOG_OUT",
      "NotLogOut"
    },
    {
      "USER_LOGGED_OUT",
      "UserLoggedOutStatus"
    },
    {
      "VIEW_MODEL_TO_NAVIGATE",
      "UnAuthorisedStatusReceivedViewModelName"
    }
  };

  public LocalMigrationHelper()
  {
    this._secureStorageService = Mvx.IoCProvider.Resolve<ISecureStorageService>();
    this._loggingService = Mvx.IoCProvider.Resolve<ILoggingService>();
  }

  public async Task<bool> IsLocalMigrationRequired()
  {
    string str = await this._secureStorageService.GetValue(SecureStorageKeys.LAST_LOCAL_MIGRATION_0_1);
    return str == null;
  }

  public void MigrateExistingKeyValuesFromSecureStorage()
  {
    this._loggingService.getLogger().LogAppInformation(LoggingContext.USER, "Migrate Existing KeyValues From SecureStorage to Settings DB Started...", memberName: nameof (MigrateExistingKeyValuesFromSecureStorage), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/LocalMigrationHelper.cs", sourceLineNumber: 26);
    foreach (string key in new List<string>((IEnumerable<string>) this._keyExchangePairs.Keys))
      this.SaveSettingsKey(key);
    this._secureStorageService.SetValue(SecureStorageKeys.LAST_LOCAL_MIGRATION_0_1, true.ToString());
    this._loggingService.getLogger().LogAppInformation(LoggingContext.USER, "Migrate Existing KeyValues From SecureStorage to Settings DB Finished...", memberName: nameof (MigrateExistingKeyValuesFromSecureStorage), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/LocalMigrationHelper.cs", sourceLineNumber: 33);
  }

  private async void SaveSettingsKey(string key)
  {
    string value = await this._secureStorageService.GetValue(key);
    if (value == null)
    {
      value = (string) null;
    }
    else
    {
      Settings settings = new Settings();
      settings.Name = this._keyExchangePairs[key];
      settings.Value = value;
      CoreApp.settings.UpdateItem(settings);
      this._loggingService.getLogger().LogAppInformation(LoggingContext.USER, $"Key:{key} Moved to Settings db with Value:{value}", memberName: nameof (SaveSettingsKey), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/LocalMigrationHelper.cs", sourceLineNumber: 45);
      settings = (Settings) null;
      value = (string) null;
    }
  }
}
