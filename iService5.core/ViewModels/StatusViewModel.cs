// Decompiled with JetBrains decompiler
// Type: iService5.Core.ViewModels.StatusViewModel
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Appliance;
using iService5.Core.Services.Data;
using iService5.Core.Services.Helpers;
using iService5.Core.Services.Localisation;
using iService5.Core.Services.Platform;
using iService5.Core.Services.User;
using iService5.Core.Services.User.ActivityLogging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
//using Xamarin.Essentials;
//using Xamarin.Forms;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

#nullable disable
namespace iService5.Core.ViewModels;

public class StatusViewModel : MvxViewModel<DetailNavigationArgs, DetailReturnArgs>
{
  private readonly IMetadataService _metadataService;
  private readonly ILoggingService _loggingService;
  private readonly IAlertService _alertService;
  private readonly IMvxNavigationService _navigationService;
  private readonly ISecureStorageService _secureStorageService;
  private const int thresholdValueForMaxDays = 15;
  private string _StatusLine;
  private readonly IPlatformSpecificServiceLocator _locator;
  private readonly IShortTextsService _ShortTextsService;
  private readonly IUserSession _userSession;
  internal List<ppf>.Enumerator _ppfEnumerator;
  internal int NumOfExpiredPpf = 0;
  private bool forceAnUpdate = false;
  private bool ReturnToSettingsPage;
  private double _statusProgress;
  private bool _auxiliaryFilesSkipStatus;
  private bool _auxiliaryFilesStatus;
  private string _numberOfAuxilaryFiles;
  private bool _noInternetLabelVisibility = false;
  private string _ActivityIndicatorText = AppResource.INITIALISING_SERVICES;
  private bool _IsBusy = true;
  private string _NavText = AppResource.HOME_PAGE_HEADER;
  private bool elementsVisibility = false;
  private bool passed15daysperiod;
  private PPFRefetchHelper ppfRefetchHelper;

  public StatusViewModel(
    IMvxNavigationService navigationService,
    IShortTextsService ShortTextsService,
    IPlatformSpecificServiceLocator locator,
    IUserSession userSession,
    ILoggingService loggingService,
    IMetadataService metadataService,
    IAlertService alertService,
    ISecureStorageService secureStorageService)
  {
    this._metadataService = metadataService;
    this._loggingService = loggingService;
    this._navigationService = navigationService;
    this._locator = locator;
    this._ShortTextsService = ShortTextsService;
    this._userSession = userSession;
    this._alertService = alertService;
    this._secureStorageService = secureStorageService;
    if (this._ShortTextsService != null)
      this.StatusLine = AppResource.STATUS_PAGE_PREPARING;
    this.SkipUpdateCommand = (ICommand) new Command(new Action(this.SkipPpfRefetchUpdate));
  }

  public ICommand SkipUpdateCommand { internal set; get; }

  public string StatusLine
  {
    get => this._StatusLine;
    private set
    {
      this._StatusLine = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.StatusLine));
    }
  }

  public double StatusProgress
  {
    get => this._statusProgress;
    private set
    {
      this._statusProgress = value;
      this.RaisePropertyChanged<double>((Expression<Func<double>>) (() => this.StatusProgress));
    }
  }

  public bool AuxiliaryFilesSkipStatus
  {
    get => this._auxiliaryFilesSkipStatus;
    private set
    {
      this._auxiliaryFilesSkipStatus = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.AuxiliaryFilesSkipStatus));
    }
  }

  public bool AuxiliaryFilesStatus
  {
    get => this._auxiliaryFilesStatus;
    private set
    {
      this._auxiliaryFilesStatus = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.AuxiliaryFilesStatus));
    }
  }

  public string NumberOfAuxilaryFiles
  {
    get => this._numberOfAuxilaryFiles;
    private set
    {
      this._numberOfAuxilaryFiles = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.NumberOfAuxilaryFiles));
    }
  }

  public bool NoInternetLabelVisibility
  {
    get => this._noInternetLabelVisibility;
    private set
    {
      this._noInternetLabelVisibility = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.NoInternetLabelVisibility));
    }
  }

  public string ActivityIndicatorText
  {
    get => this._ActivityIndicatorText;
    private set
    {
      this._ActivityIndicatorText = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.ActivityIndicatorText));
    }
  }

  public bool IsBusy
  {
    get => this._IsBusy;
    private set
    {
      this._IsBusy = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.IsBusy));
    }
  }

  public override void ViewAppeared()
  {
    base.ViewAppeared();
    if (UtilityFunctions.IsPPFTableMigrationNeeded())
    {
      this.ElementsVisibility = false;
      if (UtilityFunctions.PPFTableMigration())
        this._loggingService.getLogger().LogAppError(LoggingContext.LOCAL, "PPF table migration successful", memberName: nameof (ViewAppeared), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/StatusViewModel.cs", sourceLineNumber: 188);
      else
        this._loggingService.getLogger().LogAppError(LoggingContext.LOCAL, "PPF table migration failed", memberName: nameof (ViewAppeared), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/StatusViewModel.cs", sourceLineNumber: 192 /*0xC0*/);
      this.ElementsVisibility = true;
    }
    if (this.ReturnToSettingsPage)
    {
      this.ElementsVisibility = true;
      this.ShowPpfRefetchStatus(AppResource.STATUS_PPF_REFETCHING);
      this.ppfRefetchHelper = new PPFRefetchHelper();
      this.ppfRefetchHelper.RequestPpfs(new UpdatePPFCompletion(this.DidPpfFetchComplete), true);
    }
    else
    {
      this.ElementsVisibility = false;
      this.CheckForNewDB();
    }
  }

  private void CheckForNewDB()
  {
    if (this._metadataService.checkIfAppHasOldDB())
    {
      this.forceAnUpdate = true;
      this.RequestForCheckSumMetadata();
    }
    else
    {
      this.forceAnUpdate = false;
      this.DownloadCheckSumMetadata();
    }
  }

  public void DownloadCheckSumMetadata()
  {
    DateTime? nullable1 = new DateTime?();
    DateTime? nullable2 = new DateTime?();
    DateTime? nullable3 = new DateTime?();
    this._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "*** DownloadCheckSumMetadata ***", memberName: nameof (DownloadCheckSumMetadata), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/StatusViewModel.cs", sourceLineNumber: 232);
    StatusViewModel.SettingsDBUpdateCheckDetails updateCheckDetails = new StatusViewModel.SettingsDBUpdateCheckDetails();
    updateCheckDetails.DatePair = this.GetSettingsDBTimeInfo();
    if (this._userSession.isMetadataSessionActive())
      return;
    DateTime dateTime1;
    if (updateCheckDetails.LastUpdate != null && updateCheckDetails.LastCheck != null)
    {
      try
      {
        this.ExtractDateTimeFromSettings(updateCheckDetails.LastCheck.Value);
      }
      catch (Exception ex)
      {
        this._loggingService.getLogger().LogAppError(LoggingContext.METADATA, "Checksum entry : " + ex.Message, memberName: nameof (DownloadCheckSumMetadata), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/StatusViewModel.cs", sourceLineNumber: 246);
        ref DateTime? local = ref nullable2;
        dateTime1 = DateTime.Today;
        DateTime dateTime2 = dateTime1.AddDays(-16.0);
        local = new DateTime?(dateTime2);
      }
      try
      {
        nullable1 = this.ExtractDateTimeFromSettings(updateCheckDetails.LastUpdate.Value);
        nullable3 = this.ExtractDateTimeFromSettings(updateCheckDetails.LastUpdate.Value);
      }
      catch (Exception ex)
      {
        this._loggingService.getLogger().LogAppError(LoggingContext.METADATA, "Checksum entry error : " + ex.Message, memberName: nameof (DownloadCheckSumMetadata), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/StatusViewModel.cs", sourceLineNumber: 258);
        ref DateTime? local = ref nullable1;
        dateTime1 = DateTime.Today;
        DateTime dateTime3 = dateTime1.AddDays(-16.0);
        local = new DateTime?(dateTime3);
      }
    }
    if (File.Exists(Path.Combine(this._locator.GetPlatformSpecificService().getFolder(), "Metadata.db3")))
    {
      DateTime? nullable4;
      try
      {
        nullable4 = new DateTime?(File.GetLastWriteTime(Path.Combine(this._locator.GetPlatformSpecificService().getFolder(), "Metadata.db3")));
      }
      catch (Exception ex)
      {
        nullable4 = new DateTime?(DateTime.Today.AddDays(-16.0));
      }
      if (!nullable1.HasValue)
      {
        CultureInfo invariantCulture = CultureInfo.InvariantCulture;
        nullable1 = nullable4;
        nullable3 = nullable4;
        iService5.Core.Services.Data.SettingsDB settings1 = CoreApp.settings;
        string str;
        if (!nullable1.HasValue)
        {
          str = (string) null;
        }
        else
        {
          dateTime1 = nullable1.GetValueOrDefault();
          str = dateTime1.ToString("G", (IFormatProvider) invariantCulture);
        }
        Settings settings2 = new Settings("lastUpdate", str);
        settings1.UpdateItem(settings2);
      }
      DateTime? nullable5 = new DateTime?(DateTime.Now);
      DateTime? nullable6 = nullable1;
      if ((nullable5.HasValue & nullable6.HasValue ? new TimeSpan?(nullable5.GetValueOrDefault() - nullable6.GetValueOrDefault()) : new TimeSpan?()).Value.TotalDays > 15.0)
      {
        this.passed15daysperiod = true;
        this.DaysSurpassedThreshold = 15;
      }
    }
    else
    {
      this._userSession.DbDetected(false);
      this.passed15daysperiod = true;
      this.DaysSurpassedThreshold = 15;
    }
    if (this.CheckIsHostReachable().Result)
      this.RequestForCheckSumMetadata();
    else if (this.passed15daysperiod)
      this.DeviceIsOfflineAlert();
    else
      this._navigationService.Navigate<TabViewModel>((IMvxBundle) null, new CancellationToken());
  }

  private void RequestForCheckSumMetadata()
  {
    if (this.CalledFromDB)
      this.ElementsVisibility = true;
    this._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "Device is online, checking for Metadata checksum", memberName: nameof (RequestForCheckSumMetadata), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/StatusViewModel.cs", sourceLineNumber: 317);
    if (this._ShortTextsService != null)
      this.StatusLine = AppResource.STATUS_PAGE_CHECKING_METADATA;
    this._userSession.getMetadataAvailable(new metadataUpdateCallback(this.MetadataRetrieval));
  }

  private Settings[] GetSettingsDBTimeInfo()
  {
    CultureInfo invariantCulture = CultureInfo.InvariantCulture;
    Settings settings1 = new Settings();
    Settings settings2 = new Settings();
    try
    {
      settings1 = CoreApp.settings.GetItem("lastUpdate");
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "Getting lastupdate details error " + ex.Message, memberName: nameof (GetSettingsDBTimeInfo), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/StatusViewModel.cs", sourceLineNumber: 336);
      iService5.Core.Services.Data.SettingsDB settings3 = CoreApp.settings;
      DateTime dateTime = DateTime.Now;
      dateTime = dateTime.AddDays(-16.0);
      Settings settings4 = new Settings("lastUpdate", dateTime.ToString("G", (IFormatProvider) invariantCulture));
      settings3.UpdateItem(settings4);
    }
    finally
    {
      settings1 = CoreApp.settings.GetItem("lastUpdate");
    }
    try
    {
      settings2 = CoreApp.settings.GetItem("lastCheck");
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "Getting lastcheck details error " + ex.Message, memberName: nameof (GetSettingsDBTimeInfo), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/StatusViewModel.cs", sourceLineNumber: 350);
      iService5.Core.Services.Data.SettingsDB settings5 = CoreApp.settings;
      DateTime dateTime = DateTime.Now;
      dateTime = dateTime.AddDays(-16.0);
      Settings settings6 = new Settings("lastCheck", dateTime.ToString("G", (IFormatProvider) invariantCulture));
      settings5.UpdateItem(settings6);
    }
    finally
    {
      settings2 = CoreApp.settings.GetItem("lastCheck");
    }
    Settings settings7 = settings2;
    DateTime dateTime1;
    if (settings7 == null)
    {
      dateTime1 = DateTime.Today;
      dateTime1 = dateTime1.AddDays(-16.0);
      settings7 = new Settings("lastCheck", dateTime1.ToString((IFormatProvider) invariantCulture));
    }
    Settings settings8 = settings7;
    Settings settings9 = settings1;
    if (settings9 == null)
    {
      dateTime1 = DateTime.Today;
      dateTime1 = dateTime1.AddDays(-16.0);
      settings9 = new Settings("lastUpdate", dateTime1.ToString((IFormatProvider) invariantCulture));
    }
    return new Settings[2]{ settings9, settings8 };
  }

  private DateTime? ExtractDateTimeFromSettings(string _dt)
  {
    DateTime? nullable = new DateTime?();
    CultureInfo invariantCulture = CultureInfo.InvariantCulture;
    string str = _dt;
    DateTime? timeFromSettings;
    try
    {
      if (_dt != null && _dt.StartsWith("USE_TEXT"))
      {
        switch (_dt.Split(':')[1])
        {
          case "ST_PAGE_FAILED_XSUM":
            timeFromSettings = new DateTime?();
            break;
          case "ST_PAGE_FAILED_DB":
            timeFromSettings = new DateTime?();
            break;
          default:
            timeFromSettings = new DateTime?();
            break;
        }
      }
      else
        timeFromSettings = new DateTime?(Convert.ToDateTime(str, (IFormatProvider) invariantCulture));
    }
    catch (FormatException ex)
    {
      this._loggingService.getLogger().LogAppError(LoggingContext.METADATA, $"{str} is not in the correct format :{ex.Message}", memberName: nameof (ExtractDateTimeFromSettings), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/StatusViewModel.cs", sourceLineNumber: 398);
      timeFromSettings = new DateTime?();
    }
    return timeFromSettings;
  }

  public void DownloadMetadata()
  {
    this.ElementsVisibility = true;
    if (this._userSession.isMetadataSessionActive())
      return;
    Settings settings = CoreApp.settings.GetItem("lastUpdate");
    if ((settings == null || !(settings.Value != "") ? new DateTime?() : this.ExtractDateTimeFromSettings(settings.Value)).HasValue)
      this._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "Last DB Check was on: " + settings.Value, memberName: nameof (DownloadMetadata), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/StatusViewModel.cs", sourceLineNumber: 415);
    if (this.CheckIsHostReachable().Result)
    {
      if (this._ShortTextsService != null)
        this.StatusLine = AppResource.STATUS_PAGE_UPDATING;
      this._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "Begin Metadata Update", memberName: nameof (DownloadMetadata), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/StatusViewModel.cs", sourceLineNumber: 421);
      this._userSession.getMetadata(new metadataUpdateCallback(this.MetadataRetrieval), new iService5.Core.Services.User.progressCallback(this.ProgressCallback));
    }
    else
      this.DeviceIsOfflineAlert();
  }

  internal void DeviceIsOfflineAlert()
  {
    this._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "Device is offline, asking for connection", memberName: nameof (DeviceIsOfflineAlert), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/StatusViewModel.cs", sourceLineNumber: 434);
    if (this._ShortTextsService == null)
      return;
    this.StatusLine = AppResource.STATUS_PAGE_GOONLINE;
    this._locator.GetPlatformSpecificService().setConnectivityCallback(new NetworkConnected(this.ConnectivityCallback));
    this._alertService.ShowMessageAlertWithKey("STATUS_PAGE_GOONLINE", AppResource.INFORMATION_TEXT, "OK", (Action) (() => this.NoInternetLabelVisibility = !this.ElementsVisibility));
  }

  public async Task<bool> CheckIsHostReachable() => this._userSession.IsHostReachable();

  internal void ConnectivityCallback(
    NetworkAccess status,
    IEnumerable<ConnectionProfile> connectionProfile)
  {
    if (status != NetworkAccess.Internet)
      return;
    this.NoInternetLabelVisibility = false;
    this._locator.GetPlatformSpecificService().setConnectivityCallback((NetworkConnected) null);
    IMvxNavigationService navigationService = this._navigationService;
    DetailReturnArgs detailReturnArgs = new DetailReturnArgs();
    detailReturnArgs._returnFromDB = true;
    CancellationToken cancellationToken = new CancellationToken();
    navigationService.Close<DetailReturnArgs>((IMvxViewModelResult<DetailReturnArgs>) this, detailReturnArgs, cancellationToken);
  }

  public void ProgressCallback(double progress) => this.StatusProgress = progress;

  internal async Task ShowPermissionAlert(int _daysInactive)
  {
    string metaDataSize = this._metadataService.GetMetaDataFileSize();
    bool downloadSelected = false;
    if (!File.Exists(Path.Combine(this._locator.GetPlatformSpecificService().getFolder(), "Metadata.db3")))
    {
      string cancelstr = _daysInactive.Equals(15) ? (string) null : AppResource.CANCEL_LABEL;
      string alertHeader = AppResource.METADATA_DOWNLOAD_FIRST_TIME_PERMISSION_TEXT.Replace("100 MB", metaDataSize);
      string alertHeaderForLog = this._alertService.GetEnValue("METADATA_DOWNLOAD_FIRST_TIME_PERMISSION_TEXT").Replace("100 MB", metaDataSize);
      this._loggingService.getLogger().LogAppInformation(LoggingContext.USERSESSION, "User action popup appeared with message: " + alertHeaderForLog, memberName: nameof (ShowPermissionAlert), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/StatusViewModel.cs", sourceLineNumber: 478);
      if (!this.DaysSurpassedThreshold.Equals(15))
      {
        downloadSelected = await Application.Current.MainPage.DisplayAlert(AppResource.INFORMATION_TEXT, alertHeader, AppResource.ST_PAGE_DWL_NOW, cancelstr);
      }
      else
      {
        await Application.Current.MainPage.DisplayAlert(AppResource.INFORMATION_TEXT, alertHeader, AppResource.UPDATE_TEXT);
        downloadSelected = true;
      }
      if (downloadSelected)
      {
        CoreApp.settings.UpdateItem(new Settings("UserLoggedOutStatus", "NotLogOut"));
        this.DownloadMetadata();
      }
      else
      {
        CoreApp.settings.UpdateItem(new Settings("UserLoggedOutStatus", "Logout"));
        this._secureStorageService.Remove(SecureStorageKeys.USERNAME);
        this._secureStorageService.Remove(SecureStorageKeys.PASSWORD);
        CoreApp.settings.DeleteItem(new Settings("AutoLogin", ""));
        CoreApp.settings.SaveItem(new Settings("AutoLogin", "FALSE"));
        int num = await this._navigationService.Close((IMvxViewModel) this, new CancellationToken()) ? 1 : 0;
      }
      cancelstr = (string) null;
      alertHeader = (string) null;
      alertHeaderForLog = (string) null;
      metaDataSize = (string) null;
    }
    else
    {
      if (!_daysInactive.Equals(15) && !this.forceAnUpdate)
      {
        string alertHeader = AppResource.METADATA_DOWNLOAD_UPDATE_PERMISSION_TEXT.Replace("100 MB", metaDataSize);
        string alertHeaderForLog = this._alertService.GetEnValue("METADATA_DOWNLOAD_UPDATE_PERMISSION_TEXT").Replace("100 MB", metaDataSize);
        this._loggingService.getLogger().LogAppInformation(LoggingContext.USERSESSION, "User action popup appeared with message: " + alertHeaderForLog, memberName: nameof (ShowPermissionAlert), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/StatusViewModel.cs", sourceLineNumber: 514);
        downloadSelected = await Application.Current.MainPage.DisplayAlert(AppResource.INFORMATION_TEXT, alertHeader, AppResource.UPDATE_TEXT, AppResource.NOT_NOW_TEXT);
        if (downloadSelected)
        {
          this.DownloadMetadata();
        }
        else
        {
          CoreApp.settings.UpdateItem(new Settings("MetadataPostponedTimestamp", UtilityFunctions.ConvertDateTimeToString(DateTime.Now)));
          this.CheckForExpiredPpf(false);
        }
        alertHeader = (string) null;
        alertHeaderForLog = (string) null;
      }
      else
      {
        string alertHeader = AppResource.METADATA_DOWNLOAD_UPDATE_PERMISSION_TEXT.Replace("100 MB", metaDataSize);
        string alertHeaderForLog = this._alertService.GetEnValue("METADATA_DOWNLOAD_UPDATE_PERMISSION_TEXT").Replace("100 MB", metaDataSize);
        this._loggingService.getLogger().LogAppInformation(LoggingContext.USERSESSION, "User action popup appeared with message: " + alertHeaderForLog, memberName: nameof (ShowPermissionAlert), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/StatusViewModel.cs", sourceLineNumber: 534);
        bool onebuttonSelected = await Application.Current.MainPage.DisplayAlert(AppResource.INFORMATION_TEXT, alertHeader, (string) null, AppResource.UPDATE_TEXT);
        if (!onebuttonSelected)
          this.DownloadMetadata();
        alertHeader = (string) null;
        alertHeaderForLog = (string) null;
      }
      metaDataSize = (string) null;
    }
  }

  internal void MetadataRetrieval(MetadataStatus res)
  {
    switch (res)
    {
      case MetadataStatus.ABSENT:
      case MetadataStatus.OLD:
        if (this.CalledFromDB)
        {
          if (this._ShortTextsService != null)
            this.StatusLine = AppResource.STATUS_PAGE_UPDATING;
          this._userSession.getMetadata(new metadataUpdateCallback(this.MetadataRetrieval), new iService5.Core.Services.User.progressCallback(this.ProgressCallback));
          break;
        }
        MainThread.InvokeOnMainThreadAsync((Func<Task>) (async () => await this.ShowPermissionAlert(this.DaysSurpassedThreshold)));
        break;
      case MetadataStatus.UPDATED:
        if (!this.CalledFromDB)
        {
          this.CheckForExpiredPpf(false);
          MessagingCenter.Send<StatusViewModel>(this, CoreApp.EventsNames.MetadataUpdated.ToString());
          break;
        }
        this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
        break;
      case MetadataStatus.SUCCESS:
        this._userSession.MetadataStatusSuccess(true);
        CoreApp.settings.DeleteItem(new Settings("lastUpdate", ""));
        CoreApp.settings.UpdateItem(new Settings("lastUpdate", DateTime.Now.ToString("G", (IFormatProvider) CultureInfo.InvariantCulture)));
        CoreApp.settings.UpdateItem(new Settings("certificateUpdate", DateTime.Now.ToString("G", (IFormatProvider) CultureInfo.InvariantCulture)));
        Logger logger1 = this._loggingService.getLogger();
        IMetadataService metadataService1 = this._metadataService;
        SMMFeatures smmFeatures = SMMFeatures.ECUFLASHING;
        string name1 = smmFeatures.ToString();
        string message1 = "FeatureTable for ECUFlashing: " + StatusViewModel.GetVersionList(metadataService1.GetVersionFromFeatureTable(name1));
        logger1.LogAppInformation(LoggingContext.APPLIANCE, message1, memberName: nameof (MetadataRetrieval), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/StatusViewModel.cs", sourceLineNumber: 584);
        Logger logger2 = this._loggingService.getLogger();
        IMetadataService metadataService2 = this._metadataService;
        smmFeatures = SMMFeatures.DOWNGRADEALL;
        string name2 = smmFeatures.ToString();
        string message2 = "FeatureTable for DowngradeAll: " + StatusViewModel.GetVersionList(metadataService2.GetVersionFromFeatureTable(name2));
        logger2.LogAppInformation(LoggingContext.APPLIANCE, message2, memberName: nameof (MetadataRetrieval), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/StatusViewModel.cs", sourceLineNumber: 585);
        Logger logger3 = this._loggingService.getLogger();
        IMetadataService metadataService3 = this._metadataService;
        smmFeatures = SMMFeatures.PURGING;
        string name3 = smmFeatures.ToString();
        string message3 = "FeatureTable for Purging: " + StatusViewModel.GetVersionList(metadataService3.GetVersionFromFeatureTable(name3));
        logger3.LogAppInformation(LoggingContext.APPLIANCE, message3, memberName: nameof (MetadataRetrieval), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/StatusViewModel.cs", sourceLineNumber: 586);
        Logger logger4 = this._loggingService.getLogger();
        IMetadataService metadataService4 = this._metadataService;
        smmFeatures = SMMFeatures.HACOUNTRYSETTINGS;
        string name4 = smmFeatures.ToString();
        string message4 = "FeatureTable for HACountrySettings: " + StatusViewModel.GetVersionList(metadataService4.GetVersionFromFeatureTable(name4));
        logger4.LogAppInformation(LoggingContext.APPLIANCE, message4, memberName: nameof (MetadataRetrieval), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/StatusViewModel.cs", sourceLineNumber: 587);
        Logger logger5 = this._loggingService.getLogger();
        IMetadataService metadataService5 = this._metadataService;
        smmFeatures = SMMFeatures.TYPE1CHECK;
        string name5 = smmFeatures.ToString();
        string message5 = "FeatureTable for Type1Check: " + StatusViewModel.GetVersionList(metadataService5.GetVersionFromFeatureTable(name5));
        logger5.LogAppInformation(LoggingContext.APPLIANCE, message5, memberName: nameof (MetadataRetrieval), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/StatusViewModel.cs", sourceLineNumber: 588);
        Logger logger6 = this._loggingService.getLogger();
        IMetadataService metadataService6 = this._metadataService;
        smmFeatures = SMMFeatures.TYPE3CHECK;
        string name6 = smmFeatures.ToString();
        string message6 = "FeatureTable for Type3Check: " + StatusViewModel.GetVersionList(metadataService6.GetVersionFromFeatureTable(name6));
        logger6.LogAppInformation(LoggingContext.APPLIANCE, message6, memberName: nameof (MetadataRetrieval), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/StatusViewModel.cs", sourceLineNumber: 589);
        Logger logger7 = this._loggingService.getLogger();
        IMetadataService metadataService7 = this._metadataService;
        smmFeatures = SMMFeatures.TYPE6CHECK;
        string name7 = smmFeatures.ToString();
        string message7 = "FeatureTable for Type6Check: " + StatusViewModel.GetVersionList(metadataService7.GetVersionFromFeatureTable(name7));
        logger7.LogAppInformation(LoggingContext.APPLIANCE, message7, memberName: nameof (MetadataRetrieval), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/StatusViewModel.cs", sourceLineNumber: 590);
        if (this.CalledFromDB)
        {
          IMvxNavigationService navigationService = this._navigationService;
          DetailReturnArgs detailReturnArgs = new DetailReturnArgs();
          detailReturnArgs._returnFromDB = true;
          CancellationToken cancellationToken = new CancellationToken();
          navigationService.Close<DetailReturnArgs>((IMvxViewModelResult<DetailReturnArgs>) this, detailReturnArgs, cancellationToken);
          break;
        }
        this.CheckForExpiredPpf();
        MessagingCenter.Send<StatusViewModel>(this, CoreApp.EventsNames.MetadataSuccess.ToString());
        break;
      case MetadataStatus.FAILURE:
        MessagingCenter.Send<StatusViewModel>(this, CoreApp.EventsNames.MetadataFailed.ToString());
        if (this._metadataService.isDBValid() && !this.CalledFromDB)
        {
          this.CheckForExpiredPpf(false);
          break;
        }
        IMvxNavigationService navigationService1 = this._navigationService;
        DetailReturnArgs detailReturnArgs1 = new DetailReturnArgs();
        detailReturnArgs1._returnFromDB = true;
        CancellationToken cancellationToken1 = new CancellationToken();
        navigationService1.Close<DetailReturnArgs>((IMvxViewModelResult<DetailReturnArgs>) this, detailReturnArgs1, cancellationToken1);
        break;
      case MetadataStatus.UNAUTHORIZED:
        this._alertService.ShowMessageAlertWithKey("JWT_TOKEN_EXPIRED", AppResource.WARNING_TEXT);
        MessagingCenter.Send<StatusViewModel>(this, CoreApp.EventsNames.TokenExpiredPopUpShown.ToString());
        this._navigationService.Navigate<LoginViewModel>((IMvxBundle) null, new CancellationToken());
        break;
      case MetadataStatus.SERVER_CERTIFICATE_UNKNOWN:
        this._alertService.ShowMessageAlertWithKey("SERVER_TRUST_FAILURE", AppResource.WARNING_TEXT);
        this._navigationService.Navigate<LoginViewModel>((IMvxBundle) null, new CancellationToken());
        break;
      case MetadataStatus.EXCEPTION:
        this._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "The MetaData download operation entered Exception", memberName: nameof (MetadataRetrieval), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/StatusViewModel.cs", sourceLineNumber: 634);
        ServiceError serviceError = this._userSession.getServiceError();
        this._loggingService.getLogger().LogAppError(LoggingContext.METADATA, $"There occured a problem while MetaData downloading: {serviceError.errorKey.ToString()}Error Message: {serviceError.errorMessage}Error Type: {serviceError.errorType.ToString()}", memberName: nameof (MetadataRetrieval), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/StatusViewModel.cs", sourceLineNumber: 636);
        this.ShowAlertForMetaDataException();
        break;
      case MetadataStatus.TIMEOUT:
        this._loggingService.getLogger().LogAppError(LoggingContext.METADATA, "The operation has timed out" + this._userSession.GetBackendRequestStatus().ToString(), memberName: nameof (MetadataRetrieval), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/StatusViewModel.cs", sourceLineNumber: 625);
        this._alertService.ShowMessageAlertWithKey("GENERIC_ERROR_MESSAGE", AppResource.INFORMATION_TEXT);
        if (this._metadataService.isDBValid() && !this.CalledFromDB)
        {
          this.CheckForExpiredPpf(false);
          break;
        }
        IMvxNavigationService navigationService2 = this._navigationService;
        DetailReturnArgs detailReturnArgs2 = new DetailReturnArgs();
        detailReturnArgs2._returnFromDB = true;
        CancellationToken cancellationToken2 = new CancellationToken();
        navigationService2.Close<DetailReturnArgs>((IMvxViewModelResult<DetailReturnArgs>) this, detailReturnArgs2, cancellationToken2);
        break;
      case MetadataStatus.PROCESSING:
        this.StatusLine = AppResource.PROCESSING_DATA;
        break;
    }
  }

  public void ShowAlertForMetaDataException()
  {
    this._alertService.ShowMessageAlertWithKeyFromService(AppResource.GENERIC_ERROR_MESSAGE, AppResource.INFORMATION_TEXT, AppResource.RETRY_LABEL, AppResource.CANCEL_LABEL, (Action<bool>) (async shouldRetry =>
    {
      if (shouldRetry)
      {
        this._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "MetaData download restarted", memberName: nameof (ShowAlertForMetaDataException), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/StatusViewModel.cs", sourceLineNumber: 650);
        if (!this.CalledFromDB)
        {
          await this.ShowPermissionAlert(this.DaysSurpassedThreshold);
        }
        else
        {
          this._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "MetaData download restart cancelled", memberName: nameof (ShowAlertForMetaDataException), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/StatusViewModel.cs", sourceLineNumber: 657);
          this.ElementsVisibility = false;
          this.CheckForNewDB();
        }
      }
      else
      {
        IMvxNavigationService navigationService = this._navigationService;
        DetailReturnArgs detailReturnArgs = new DetailReturnArgs();
        detailReturnArgs._returnFromDB = true;
        CancellationToken cancellationToken = new CancellationToken();
        int num = await navigationService.Close<DetailReturnArgs>((IMvxViewModelResult<DetailReturnArgs>) this, detailReturnArgs, cancellationToken) ? 1 : 0;
      }
    }));
  }

  private static string GetVersionList(List<iService5.Core.Services.Data.Version> versionRange)
  {
    string versionList = "";
    switch (versionRange.Count)
    {
      case 1:
        versionList = versionRange[0].ToString();
        break;
      case 2:
        versionList = $"{versionRange[0]} - {versionRange[1]}";
        break;
    }
    return versionList;
  }

  public bool CalledFromDB { get; set; }

  public override void Prepare(DetailNavigationArgs parameter)
  {
    if (parameter._calledFromDB)
      this.CalledFromDB = true;
    this.ReturnToSettingsPage = parameter.ReturnToSettingsPage;
  }

  public string NavText
  {
    get => this._NavText;
    internal set
    {
      this._NavText = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.NavText));
    }
  }

  public bool ElementsVisibility
  {
    get => this.elementsVisibility;
    internal set
    {
      this.elementsVisibility = value;
      if (this.elementsVisibility)
        this.IsBusy = false;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.ElementsVisibility));
    }
  }

  public bool ChecksumReset { get; private set; }

  public int DaysSurpassedThreshold { get; private set; }

  public void CheckForExpiredPpf(bool dbGotDownloaded = true)
  {
    if (this._userSession.GetMaterialStatistics().Count < 1)
    {
      this._loggingService.getLogger().LogAppInformation(LoggingContext.LOCAL, "Set data for USersession SetMaterialStatistics", memberName: nameof (CheckForExpiredPpf), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/StatusViewModel.cs", sourceLineNumber: 745);
      Task.Run((Action) (() =>
      {
        this._userSession.SetMaterialStatistics(this._metadataService.GetMaterialStatistics());
        this._loggingService.getLogger().LogAppInformation(LoggingContext.LOCAL, "Data has been set for MaterialStatistics", memberName: nameof (CheckForExpiredPpf), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/StatusViewModel.cs", sourceLineNumber: 750);
      }));
    }
    if (!this.IsBusy)
      this.ElementsVisibility = true;
    if (dbGotDownloaded)
    {
      if (FeatureConfiguration.PpfRefetchEnabled)
      {
        this.ShowPpfRefetchStatus(AppResource.STATUS_PPF_REFETCHING);
        this.ppfRefetchHelper = new PPFRefetchHelper();
        this.ppfRefetchHelper.RequestPpfs(new UpdatePPFCompletion(this.DidPpfFetchComplete), false);
      }
      else
        Task.Run(new Action(this.CheckCloseToExpiryPpfsAndNavigate));
    }
    else
      Task.Run(new Action(this.CheckCloseToExpiryPpfsAndNavigate));
  }

  public void DidPpfFetchComplete(PPFRefetchStatus status, int dIndex, int total)
  {
    MainThread.BeginInvokeOnMainThread((Action) (() =>
    {
      switch (status)
      {
        case PPFRefetchStatus.Started:
          this.NumberOfAuxilaryFiles = $"{dIndex}/{total}";
          this.ProgressCallback((double) dIndex / (double) total);
          break;
        case PPFRefetchStatus.Completed:
          if (this.ReturnToSettingsPage)
          {
            this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
            break;
          }
          this.NavigateToNextScreen();
          break;
        case PPFRefetchStatus.Failed:
        case PPFRefetchStatus.Interrupted:
          if (this.ReturnToSettingsPage || status != PPFRefetchStatus.Interrupted)
            break;
          this.CheckCloseToExpiryPpfsAndNavigate();
          break;
      }
    }));
  }

  private void ShowPpfRefetchStatus(string statusLine, bool allowSkip = true)
  {
    MainThread.BeginInvokeOnMainThread((Action) (() =>
    {
      this.StatusLine = statusLine;
      this.AuxiliaryFilesStatus = true;
      this.ProgressCallback(0.0);
      this.AuxiliaryFilesSkipStatus = allowSkip;
    }));
  }

  public void SkipPpfRefetchUpdate()
  {
    this.ppfRefetchHelper.CancelActive = true;
    if (this.StatusLine == AppResource.STATUS_PPF_REFETCHING && this.ReturnToSettingsPage)
      this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
    else
      this.NavigateToNextScreen();
  }

  private void CheckCloseToExpiryPpfsAndNavigate()
  {
    List<DownloadProxy> toExpireEnumbers = this._metadataService.GetExpiredAndAboutToExpireENumbers();
    this._loggingService.getLogger().LogAppInformation(LoggingContext.LOCAL, $"Expired Enumbers count:{toExpireEnumbers.Count}", memberName: nameof (CheckCloseToExpiryPpfsAndNavigate), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/StatusViewModel.cs", sourceLineNumber: 840);
    if (toExpireEnumbers.Count > 0)
    {
      this.IsBusy = false;
      this.ElementsVisibility = true;
      this.ShowPpfRefetchStatus(AppResource.STATUS_PAGE_UPDATE_EXPIRED_PPF);
      this.ppfRefetchHelper = new PPFRefetchHelper();
      this.ppfRefetchHelper.RequestPpfsToExpire(new UpdatePPFCompletion(this.DidPpfFetchComplete), toExpireEnumbers);
    }
    else
      this.NavigateToNextScreen();
  }

  public void NavigateToNextScreen()
  {
    this._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, nameof (NavigateToNextScreen), memberName: nameof (NavigateToNextScreen), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/StatusViewModel.cs", sourceLineNumber: 857);
    MainThread.BeginInvokeOnMainThread((Action) (() =>
    {
      this.ElementsVisibility = false;
      this.IsBusy = false;
      if (this._userSession.ShouldGoToPrepareWork())
        this._navigationService.Navigate<WorkPreparationViewModel>((IMvxBundle) null, new CancellationToken());
      else
        this._navigationService.Navigate<TabViewModel>((IMvxBundle) null, new CancellationToken());
    }));
  }

  private class SettingsDBUpdateCheckDetails
  {
    private Settings _lastUpdate;
    private Settings _lastCheck;
    private Settings[] _timeEntries;

    public SettingsDBUpdateCheckDetails()
    {
      this.LastUpdate = new Settings();
      this.LastCheck = new Settings();
    }

    public Settings LastUpdate
    {
      get => this._lastUpdate;
      set => this._lastUpdate = value;
    }

    public Settings LastCheck
    {
      get => this._lastCheck;
      set => this._lastCheck = value;
    }

    public Settings[] DatePair
    {
      get => this._timeEntries;
      set
      {
        this._timeEntries = value;
        this.LastUpdate = this._timeEntries[0];
        this.LastCheck = this._timeEntries[1];
      }
    }
  }
}
