// Decompiled with JetBrains decompiler
// Type: iService5.Core.ViewModels.ApplianceDatabaseViewModel
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
using iService5.Core.Services.VersionReport;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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

public class ApplianceDatabaseViewModel : MvxViewModel<bool>
{
  private readonly IMvxNavigationService _navigationService;
  private readonly IPlatformSpecificServiceLocator _locator;
  private readonly IUserSession _userSession;
  private readonly IMetadataService _metadataService;
  private readonly IAlertService _alertService;
  private readonly ISecureStorageService _secureStorageService;
  private readonly IAppliance _appliance;
  private const string MATERIAL_STATISTICS_TABLE = "materialStatistics";
  private Dictionary<DeviceClass, List<MaterialStatistics>> relevantMaterials = new Dictionary<DeviceClass, List<MaterialStatistics>>();
  private DownloadFilesStatistics statistics = new DownloadFilesStatistics();
  private Is5SshWrapper _sshWrapper;
  internal List<DownloadProxy>.Enumerator _downloadEnumerator;
  internal List<DownloadProxy>.Enumerator _binaryEnumerator;
  internal List<DownloadProxy>.Enumerator _downloadEnumeratorF;
  internal List<DownloadProxy> failedFilesToDownload = new List<DownloadProxy>();
  internal List<DownloadProxy> failedFilesToDownloadAtRepeat = new List<DownloadProxy>();
  private MultiPage<Page> p;
  private List<DownloadProxy> notFoundFileIds = new List<DownloadProxy>();
  private int errorsDownloadFiles = 0;
  private int errorsDownloadFailedFiles = 0;
  private List<DownloadProxy> corruptedFiles = new List<DownloadProxy>();
  private List<string> downloadedSMMEnumbers = new List<string>();
  private List<string> smmMaterials = new List<string>();
  internal string FileSizeToBeDownloaded = string.Empty;
  internal long downloadableFileSize = 0;
  internal string reportAProblemTitle = AppResource.REPORT_A_PROBLEM_TEXT;
  private string _remoteFiles = "random";
  private string _numberOfRemoteFiles = "random";
  private string _microtext = "random";
  private readonly ILoggingService _loggingService;
  private string _HeaderLabelText;
  private bool _OfflineSwitchToggled = false;
  private bool _IsPpfRefetchRequired = false;
  private bool _WifiOnly = true;
  private bool _WiFiSwitchToggled = false;
  private bool _DBDownloadRequired = true;
  private string _DbRequiredLabelText;
  private string _LastUpdateLabelText;
  private string _DownloadableFileSizeLabelText;
  private bool _DisplayPendingFileSize = true;
  private string _UsedStorageLabelText;
  private string _NumOfFilesText;
  private string _DownloadedApplianceFile;
  private string _DownloadDataButtonText;
  private bool _CancelActive;
  private string _ScheduleDownloadLabelText;
  private string _wifilabel;
  private string _availableStorage = "";
  private string _offlinelabel;
  private string _AppVersionLabel;
  private bool _showAuxiliaryFileStatus = false;
  private bool _OrangeBarVisibility = false;
  private string _OrangeBarText = AppResource.Binary_Download_ProgressAlert;
  private string _BridgeSettingText;
  internal bool _CATSettingsVisible = FeatureConfiguration.DisplayCATFeature;
  internal bool _DisplayBridgeSettings = FeatureConfiguration.DisplayNativeBridgeProperties;
  internal bool _StartWebSocketVisibility = false;
  internal bool _Bridge_feature = true;
  private bool _BridgeSettingSwitchToggled;
  private bool _IsBusy = false;
  internal bool repeatDownloadingOfFailedFiles;
  internal bool normalDownloadingFiles;
  internal bool downloadSettingsConfigured = false;
  internal bool downloadDisabled = true;
  internal bool shouldDisplayDownloadSettings;
  internal bool shouldDisplayDownloadSettingsLabel = false;
  internal bool displayFileStatus = false;
  private bool displayActivityIndicator = false;
  internal string selectedDeviceClass = AppResource.ALL_TEXT;
  internal bool isCountryRelevant = false;
  internal bool isFileSizeSwitchToggled = false;
  private CancellationTokenSource _cancellationToken = new CancellationTokenSource();
  private bool _areButtonsEnabled = true;
  private bool _cameFromCatTester = false;
  private bool tokenExpirePopUpAlreadyCalled = false;
  private List<string> DownloadedBinariesForEnumbers = new List<string>();

  public virtual async Task Initialize()
  {
    await base.Initialize();
    IReadOnlyList<Page> t = Application.Current.MainPage.Navigation.NavigationStack;
    t = (IReadOnlyList<Page>) null;
  }

  public ApplianceDatabaseViewModel(
    IMvxNavigationService navigationService,
    IShortTextsService _SService,
    IAppliance appliance,
    ILoggingService loggingService,
    IMetadataService metadataService,
    IUserSession userSession,
    IPlatformSpecificServiceLocator locator,
    IVersionReport versionReport,
    IAlertService alertService,
    ISecureStorageService secureStorageService,
    Is5SshWrapper sshWrapper)
  {
    this.AppVersionLabel = versionReport.getVersion();
    this._locator = locator;
    this._userSession = userSession;
    this._metadataService = metadataService;
    this._navigationService = navigationService;
    this._alertService = alertService;
    this._secureStorageService = secureStorageService;
    this._appliance = appliance;
    this._sshWrapper = sshWrapper;
    this.GoToFeedbackForm = (ICommand) new Command((Action) (() => Device.BeginInvokeOnMainThread((Action) (() => this.VisitFeedbackForm()))));
    this.GoToStartWebSocket = (ICommand) new Command((Action) (() => Device.BeginInvokeOnMainThread((Action) (() => this.VisitStartWebSocket()))));
    this.GoToDownloadSettings = (ICommand) new Command((Action) (() => Device.BeginInvokeOnMainThread((Action) (() => this.VisitDownloadSettings()))));
    this.DownloadNowCommand = (ICommand) new Command((Action) (async () => await this.DownloadNowButtonPressed()));
    this.DeleteApplianceFiles = (ICommand) new Command(new Action(this.OnClickingDeleteFiles));
    this.LegalInfoCommand = (ICommand) new Command((Action) (() => Device.BeginInvokeOnMainThread((Action) (() => this.OnClickingLegalInfo()))));
    this.CompactApplianceTesterCommand = (ICommand) new Command((Action) (() => Device.BeginInvokeOnMainThread((Action) (async () => await this.OnClickingCompactApplianceTester()))));
    this.BridgeSettingsTapCommand = (ICommand) new Command(new Action(this.OnClickingBridgeSettings));
    this.PrepareWorkCommand = (ICommand) new Command(new Action(this.NavigateToPrepare));
    this.TapReDownloadEnumberCommand = (ICommand) new Command(new Action(this.NavigateToReDownloadView));
    this.PpfRefetchCommand = (ICommand) new Command(new Action(this.NavigateToStatusView));
    if (_SService != null)
    {
      this._loggingService = loggingService;
      this.HeaderLabelText = AppResource.ST_PAGE_HEADER;
      this.DbRequiredLabelText = AppResource.ST_PAGE_DWL_STATUS;
      this.DownloadDataButtonText = this.GetDownloadDataButtonTextWithSize();
      this.ScheduleDownloadLabelText = AppResource.ST_PAGE_DWL_SCHD;
      this.WiFiLabel = AppResource.ST_PAGE_DWL_WIFI_ONLY;
      this.OfflineLabel = AppResource.ST_PAGE_OFFLINE_USE;
      this.DownloadedApplianceFile = AppResource.ST_PAGE_DWL_NO_FILES;
      this.BridgeSettingText = AppResource.ST_PAGE_BRIDGE_TOGGLE;
    }
    MessagingCenter.Subscribe<Application>((object) this, CoreApp.EventsNames.ForegroundEvent.ToString(), (Action<Application>) (async sender =>
    {
      if (!this._userSession.isDownloadActive())
        return;
      await this.DownloadNowButtonPressed();
    }), (Application) null);
    MessagingCenter.Subscribe<StatusViewModel>((object) this, CoreApp.EventsNames.MetadataFailed.ToString(), (Action<StatusViewModel>) (async sender =>
    {
      if (!this._userSession.isDownloadActive())
        return;
      await this.DownloadNowButtonPressed();
    }), (StatusViewModel) null);
    MessagingCenter.Subscribe<StatusViewModel>((object) this, CoreApp.EventsNames.MetadataUpdated.ToString(), (Action<StatusViewModel>) (sender =>
    {
      this.DBDownloadRequired = false;
      CoreApp.settings.DeleteItem(new Settings("lastUpdate", ""));
      CoreApp.settings.UpdateItem(new Settings("lastUpdate", DateTime.Now.ToString("G", (IFormatProvider) CultureInfo.InvariantCulture)));
    }), (StatusViewModel) null);
    MessagingCenter.Subscribe<StatusViewModel>((object) this, CoreApp.EventsNames.TokenExpiredPopUpShown.ToString(), (Action<StatusViewModel>) (sender => this.tokenExpirePopUpAlreadyCalled = true), (StatusViewModel) null);
    Settings settings1 = CoreApp.settings.GetItem(nameof (WifiOnly));
    Settings settings2 = CoreApp.settings.GetItem("OfflineBinaries");
    if (settings2 != null && settings2.Value != "" && settings2.Value == "FALSE")
    {
      this.OfflineSwitchToggled = false;
      this.ShouldDisplayDownloadSettings = false;
      this.downloadSettingsConfigured = true;
    }
    else
      this.OfflineSwitchToggled = true;
    this.WiFiSwitchToggled = settings1 == null || !(settings1.Value != "") || !(settings1.Value == "FALSE");
    this.ranfromConstructor = true;
    this.CATSettingsVisible = FeatureConfiguration.DisplayCATFeature;
    this.DisplayBridgeSettings = FeatureConfiguration.DisplayNativeBridgeProperties;
    this.normalDownloadingFiles = true;
    this.repeatDownloadingOfFailedFiles = false;
    this.numberOfFilesDownloaded = 0;
    this.NumberOfFilesToBeDownloaded = 0;
  }

  private void NavigateToPrepare()
  {
    Device.BeginInvokeOnMainThread((Action) (() =>
    {
      if (!this.AreButtonsEnabled)
        return;
      this.AreButtonsEnabled = false;
      this._navigationService.Navigate<WorkPreparationViewModel>((IMvxBundle) null, new CancellationToken());
    }));
  }

  private async void NavigateToReDownloadView()
  {
    int num = await this._navigationService.Navigate<ReDownloadEnumberViewModel>((IMvxBundle) null, new CancellationToken()) ? 1 : 0;
  }

  private void NavigateToStatusView()
  {
    if (this._userSession.IsHostReachable())
    {
      this._alertService.ShowMessageAlertWithKey("PPF_REFETCH_INFORMATION_TEXT", AppResource.INFORMATION_TITLE, AppResource.ACCEPT_LABEL, AppResource.CANCEL_LABEL, (Action<bool>) (async ok =>
      {
        if (!ok)
          return;
        IMvxNavigationService navigationService = this._navigationService;
        DetailNavigationArgs detailNavigationArgs = new DetailNavigationArgs();
        detailNavigationArgs.ReturnToSettingsPage = true;
        CancellationToken cancellationToken = new CancellationToken();
        int num = await navigationService.Navigate<StatusViewModel, DetailNavigationArgs>(detailNavigationArgs, (IMvxBundle) null, cancellationToken) ? 1 : 0;
      }));
    }
    else
    {
      this.LogNetworkDetails();
      Device.BeginInvokeOnMainThread((Action) (async () => await this._alertService.ShowMessageAlertWithKey("NO_INTERNET", AppResource.ERROR_TITLE)));
    }
  }

  public ICommand GoToFeedbackForm { internal set; get; }

  public ICommand GoToStartWebSocket { internal set; get; }

  public ICommand PrepareWorkCommand { internal set; get; }

  public ICommand PpfRefetchCommand { internal set; get; }

  public ICommand GoToDownloadSettings { internal set; get; }

  public ICommand TapReDownloadEnumberCommand { internal set; get; }

  internal void VisitFeedbackForm()
  {
    if (!this.AreButtonsEnabled)
      return;
    this.AreButtonsEnabled = false;
    this._navigationService.Navigate<FeedbackFormViewModel>((IMvxBundle) null, new CancellationToken());
  }

  internal void VisitStartWebSocket()
  {
    if (!this.AreButtonsEnabled)
      return;
    this.AreButtonsEnabled = false;
    this._navigationService.Navigate<StartWebSocketViewModel>((IMvxBundle) null, new CancellationToken());
  }

  internal void VisitDownloadSettings()
  {
    if (!this.AreButtonsEnabled)
      return;
    this.AreButtonsEnabled = false;
    this._navigationService.Navigate<DownloadSettingsViewModel, DownloadParameter>(new DownloadParameter()
    {
      bridgeSettingSwitchToggled = this.BridgeSettingSwitchToggled,
      numberOfFilesToBeDownloaded = this.NumberOfFilesToBeDownloaded,
      fileSizeToBeDownloaded = this.FileSizeToBeDownloaded,
      downloadSettingsConfigured = this.DownloadSettingsConfigured
    }, (IMvxBundle) null, new CancellationToken());
  }

  public ICommand LegalInfoCommand { internal set; get; }

  public void OnClickingLegalInfo()
  {
    if (!this.AreButtonsEnabled)
      return;
    this.AreButtonsEnabled = false;
    this._navigationService.Navigate<LegalInfoListViewModel>((IMvxBundle) null, new CancellationToken());
  }

  public ICommand CompactApplianceTesterCommand { internal set; get; }

  public async Task OnClickingCompactApplianceTester()
  {
    if (!this.AreButtonsEnabled)
      return;
    this.AreButtonsEnabled = false;
    if (this._appliance.isConnectedToBridgeWifi())
    {
      IMvxNavigationService navigationService = this._navigationService;
      DetailNavigationArgs detailNavigationArgs = new DetailNavigationArgs();
      detailNavigationArgs.ReturnToSettingsPage = true;
      CancellationToken cancellationToken = new CancellationToken();
      int num = await navigationService.Navigate<BridgeSettingsViewModel, DetailNavigationArgs>(detailNavigationArgs, (IMvxBundle) null, cancellationToken) ? 1 : 0;
    }
    else
    {
      IMvxNavigationService navigationService = this._navigationService;
      DetailNavigationArgs detailNavigationArgs = new DetailNavigationArgs();
      detailNavigationArgs.destinationScreen = "BridgeSettingsViewModel";
      CancellationToken cancellationToken = new CancellationToken();
      int num = await navigationService.Navigate<ApplianceInstructionConnectionNonSmmViewModel, DetailNavigationArgs>(detailNavigationArgs, (IMvxBundle) null, cancellationToken) ? 1 : 0;
    }
  }

  public ICommand BridgeSettingsTapCommand { internal set; get; }

  public void OnClickingBridgeSettings()
  {
    if (!this.AreButtonsEnabled)
      return;
    this.AreButtonsEnabled = false;
    if (this._appliance.isConnectedToBridgeWifi())
    {
      this.CheckSSHAvailability();
    }
    else
    {
      IMvxNavigationService navigationService = this._navigationService;
      DetailNavigationArgs detailNavigationArgs = new DetailNavigationArgs();
      detailNavigationArgs.destinationScreen = "BridgeSettingsViewModel";
      detailNavigationArgs.bridgeSettingsSelected = true;
      CancellationToken cancellationToken = new CancellationToken();
      navigationService.Navigate<ApplianceInstructionConnectionNonSmmViewModel, DetailNavigationArgs>(detailNavigationArgs, (IMvxBundle) null, cancellationToken);
    }
  }

  private async void CheckBridgeMode()
  {
    BridgeWifiResponse response = await UtilityFunctions.EnableBridgeWIFIUsingApi(this._locator, this._loggingService);
    if (response.isSuccess)
    {
      IMvxNavigationService navigationService = this._navigationService;
      DetailNavigationArgs detailNavigationArgs = new DetailNavigationArgs();
      detailNavigationArgs.ReturnToSettingsPage = true;
      detailNavigationArgs.bridgeSettingsSelected = true;
      CancellationToken cancellationToken = new CancellationToken();
      navigationService.Navigate<BridgeSettingsViewModel, DetailNavigationArgs>(detailNavigationArgs, (IMvxBundle) null, cancellationToken);
      response = (BridgeWifiResponse) null;
    }
    else
    {
      this.AreButtonsEnabled = true;
      string errorMessage = !UtilityFunctions.IsLocalNetworkPemrissionError(response.errorMessage) ? AppResource.SSH_COMMAND_ERROR : "HA_INFO_RETRY_MESSAGE_IOS_14";
      this._alertService.ShowMessageAlertWithKeyFromService(errorMessage, AppResource.INFORMATION_TEXT, AppResource.RETRY_LABEL, AppResource.CANCEL_LABEL, (Action<bool>) (shouldRetry =>
      {
        if (!shouldRetry)
          return;
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Retrying to get data from home appliance.", memberName: nameof (CheckBridgeMode), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceDatabaseViewModel.cs", sourceLineNumber: 298);
        this.CheckBridgeMode();
      }));
      errorMessage = (string) null;
      response = (BridgeWifiResponse) null;
    }
  }

  private void CheckSSHAvailability()
  {
    if (UtilityFunctions.IsLocalNetworkPermissionNeeded() && CoreApp.settings.GetItem("localNetworkPopupAppeared").Value == "false")
    {
      this._sshWrapper.IPAddress = this._locator.GetPlatformSpecificService().GetIp();
      this._sshWrapper.GetSshAvailability();
      CoreApp.settings.UpdateItem(new Settings("localNetworkPopupAppeared", "true"));
      this.AreButtonsEnabled = true;
    }
    else
      this.CheckBridgeMode();
  }

  public ICommand DeleteApplianceFiles { internal set; get; }

  public void OnClickingDeleteFiles()
  {
    Device.BeginInvokeOnMainThread((Action) (() =>
    {
      if (!this.AreButtonsEnabled)
        return;
      this.AreButtonsEnabled = false;
      this._navigationService.Navigate<DeleteApplianceFilesViewModel>((IMvxBundle) null, new CancellationToken());
    }));
  }

  public string ReportAProblemTitle
  {
    get => this.reportAProblemTitle;
    internal set
    {
      this.reportAProblemTitle = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.ReportAProblemTitle));
    }
  }

  public string RemoteFiles
  {
    get => this._remoteFiles;
    internal set
    {
      this._remoteFiles = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.RemoteFiles));
    }
  }

  public string NumberOfRemoteFiles
  {
    get => this._numberOfRemoteFiles;
    internal set
    {
      this._numberOfRemoteFiles = value;
      if (this._numberOfRemoteFiles.Contains(" 0"))
        this.WifiOnly = !this.DBDownloadRequired;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.NumberOfRemoteFiles));
    }
  }

  public string MicroText
  {
    get => this._microtext;
    internal set
    {
      this._microtext = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.MicroText));
    }
  }

  public string HeaderLabelText
  {
    get => this._HeaderLabelText;
    private set
    {
      this._HeaderLabelText = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.HeaderLabelText));
    }
  }

  public bool OfflineSwitchToggled
  {
    get => this._OfflineSwitchToggled;
    set
    {
      CoreApp.settings.UpdateItem(new Settings("OfflineBinaries", value ? "TRUE" : "FALSE"));
      this.ShouldDisplayDownloadSettings = value;
      this.ShouldDisplayDownloadSettingsLabel = !this.DownloadSettingsConfigured & value && !this.IsBusy;
      this._OfflineSwitchToggled = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.OfflineSwitchToggled));
    }
  }

  public bool IsPpfRefetchRequired
  {
    get => this._IsPpfRefetchRequired;
    set
    {
      this._IsPpfRefetchRequired = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.IsPpfRefetchRequired));
    }
  }

  public bool WifiOnly
  {
    get => this._WifiOnly;
    set
    {
      this._WifiOnly = value;
      if (value)
      {
        this.DownloadDisabled = true;
        MessagingCenter.Send<ApplianceDatabaseViewModel>(this, CoreApp.EventsNames.ApplianceBinariesFinished.ToString());
        if (this._userSession.isDownloadActive())
          this.DownloadNowButtonPressed();
      }
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.WifiOnly));
    }
  }

  public bool WiFiSwitchToggled
  {
    get => this._WiFiSwitchToggled;
    set
    {
      this._WiFiSwitchToggled = value;
      CoreApp.settings.UpdateItem(new Settings("WifiOnly", value ? "TRUE" : "FALSE"));
      this.applyWifiPolicy();
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.WiFiSwitchToggled));
    }
  }

  public bool DBDownloadRequired
  {
    get => this._DBDownloadRequired;
    set
    {
      this._DBDownloadRequired = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.DBDownloadRequired));
    }
  }

  public string DbRequiredLabelText
  {
    get => this._DbRequiredLabelText;
    private set
    {
      this._DbRequiredLabelText = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.DbRequiredLabelText));
    }
  }

  public string LastUpdateLabelText
  {
    get => this._LastUpdateLabelText;
    private set
    {
      this._LastUpdateLabelText = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.LastUpdateLabelText));
    }
  }

  public string DownloadableFileSizeLabelText
  {
    get => this._DownloadableFileSizeLabelText;
    private set
    {
      this._DownloadableFileSizeLabelText = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.DownloadableFileSizeLabelText));
    }
  }

  public bool DisplayPendingFileSize
  {
    get => this._DisplayPendingFileSize;
    set
    {
      this._DisplayPendingFileSize = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.DisplayPendingFileSize));
    }
  }

  public string UsedStorageLabelText
  {
    get => this._UsedStorageLabelText;
    private set
    {
      this._UsedStorageLabelText = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.UsedStorageLabelText));
    }
  }

  public string NumOfFilesText
  {
    get => this._NumOfFilesText;
    private set
    {
      this._NumOfFilesText = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.NumOfFilesText));
    }
  }

  public string DownloadedApplianceFile
  {
    get => this._DownloadedApplianceFile;
    private set
    {
      this._DownloadedApplianceFile = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.DownloadedApplianceFile));
    }
  }

  public bool CancelActive
  {
    get => this._CancelActive;
    internal set => this._CancelActive = value;
  }

  public string DownloadDataButtonText
  {
    get => this._DownloadDataButtonText;
    internal set
    {
      this._DownloadDataButtonText = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.DownloadDataButtonText));
    }
  }

  public string ScheduleDownloadLabelText
  {
    get => this._ScheduleDownloadLabelText;
    private set
    {
      this._ScheduleDownloadLabelText = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.ScheduleDownloadLabelText));
    }
  }

  public string WiFiLabel
  {
    get => this._wifilabel;
    private set
    {
      this._wifilabel = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.WiFiLabel));
    }
  }

  public string AvailableStorage
  {
    get => this._availableStorage;
    set
    {
      this._availableStorage = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.AvailableStorage));
    }
  }

  public string OfflineLabel
  {
    get => this._offlinelabel;
    private set
    {
      this._offlinelabel = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.OfflineLabel));
    }
  }

  public string AppVersionLabel
  {
    get => this._AppVersionLabel;
    private set
    {
      this._AppVersionLabel = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.AppVersionLabel));
    }
  }

  public bool ShowAuxiliaryFileStatus
  {
    get => this._showAuxiliaryFileStatus;
    private set
    {
      this._showAuxiliaryFileStatus = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.ShowAuxiliaryFileStatus));
    }
  }

  public bool OrangeBarVisibility
  {
    get => this._OrangeBarVisibility;
    internal set
    {
      this._OrangeBarVisibility = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.OrangeBarVisibility));
    }
  }

  public string OrangeBarText
  {
    get => this._OrangeBarText;
    set
    {
      this._OrangeBarText = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.OrangeBarText));
    }
  }

  public string BridgeSettingText
  {
    get => this._BridgeSettingText;
    set
    {
      this._BridgeSettingText = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.BridgeSettingText));
    }
  }

  public bool CATSettingsVisible
  {
    get => this._CATSettingsVisible;
    set
    {
      this._CATSettingsVisible = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.CATSettingsVisible));
    }
  }

  public bool DisplayBridgeSettings
  {
    get => this._DisplayBridgeSettings;
    set
    {
      this._DisplayBridgeSettings = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.DisplayBridgeSettings));
    }
  }

  public bool StartWebSocketVisibility
  {
    get => this._StartWebSocketVisibility;
    set
    {
      this._StartWebSocketVisibility = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.StartWebSocketVisibility));
    }
  }

  public bool Bridge_feature
  {
    get => this._Bridge_feature;
    set
    {
      this._Bridge_feature = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.Bridge_feature));
    }
  }

  public bool BridgeSettingSwitchToggled
  {
    get => this._BridgeSettingSwitchToggled;
    set
    {
      this._BridgeSettingSwitchToggled = value;
      if (!this.ranfromprepare)
      {
        CoreApp.settings.UpdateItem(new Settings("DownloadSettings", ""));
        CoreApp.settings.UpdateItem(new Settings("SelectedDeviceClass", ""));
        CoreApp.settings.UpdateItem(new Settings("FileSizeSwitchToggled", ""));
        this.ShouldDisplayDownloadSettingsLabel = true;
        this.DisplayFileStatus = false;
        this.DownloadDisabled = true;
      }
      this.ranfromprepare = false;
      this.CATSettingsVisible = FeatureConfiguration.DisplayCATFeature && this.BridgeSettingSwitchToggled;
      this.DisplayBridgeSettings = FeatureConfiguration.DisplayNativeBridgeProperties && this.BridgeSettingSwitchToggled;
      CoreApp.settings.UpdateItem(new Settings("BridgeOff", (!this._BridgeSettingSwitchToggled).ToString().ToLower()));
      MessagingCenter.Send<ApplianceDatabaseViewModel>(this, "BridgeDisplaySwitched");
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.BridgeSettingSwitchToggled));
    }
  }

  internal bool GetFileStatusVisibility()
  {
    return !this.ShouldDisplayDownloadSettingsLabel && !this._IsBusy;
  }

  public bool IsBusy
  {
    get => this._IsBusy;
    internal set
    {
      this._IsBusy = value;
      this.ShouldDisplayDownloadSettingsLabel = !this.DownloadSettingsConfigured && !this._IsBusy;
      this.DisplayFileStatus = this.GetFileStatusVisibility();
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.IsBusy));
    }
  }

  private void CheckNavigationStackList()
  {
    this.cameFromCatTester = false;
    if (!((NavigationPage) Application.Current.MainPage).CurrentPage.ToString().ToLower().Contains("appliancetester"))
      return;
    this.cameFromCatTester = true;
  }

  public bool DownloadSettingsConfigured
  {
    get => this.downloadSettingsConfigured;
    set
    {
      this.downloadSettingsConfigured = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.DownloadSettingsConfigured));
    }
  }

  public bool DownloadDisabled
  {
    get => this.downloadDisabled;
    set
    {
      this.downloadDisabled = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.DownloadDisabled));
    }
  }

  public bool ShouldDisplayDownloadSettings
  {
    get => this.shouldDisplayDownloadSettings;
    set
    {
      this.shouldDisplayDownloadSettings = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.ShouldDisplayDownloadSettings));
    }
  }

  public bool ShouldDisplayDownloadSettingsLabel
  {
    get => this.shouldDisplayDownloadSettingsLabel;
    set
    {
      this.shouldDisplayDownloadSettingsLabel = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.ShouldDisplayDownloadSettingsLabel));
    }
  }

  public bool DisplayFileStatus
  {
    get => this.displayFileStatus;
    set
    {
      this.displayFileStatus = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.DisplayFileStatus));
    }
  }

  public bool DisplayActivityIndicator
  {
    get => this.displayActivityIndicator;
    set
    {
      this.displayActivityIndicator = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.DisplayActivityIndicator));
    }
  }

  internal void connectivityCallback(
    NetworkAccess status,
    IEnumerable<ConnectionProfile> connectionProfile)
  {
    this.applyWifiPolicy();
  }

  public override void ViewDisappearing()
  {
    base.ViewDisappearing();
    if (this._userSession.isDownloadActive())
    {
      this.EndDownloadSession();
      this.OrangeBarVisibility = false;
      this.CancelActive = true;
      this.DownloadDataButtonText = this.GetDownloadDataButtonTextWithSize();
      this.showActivityIndicatorForDownloadSection(false);
    }
    this.keepScreenOn(true);
    this._loggingService.getLogger().LogAppInformation(LoggingContext.BINARY, "Removing connectivity callback", memberName: nameof (ViewDisappearing), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceDatabaseViewModel.cs", sourceLineNumber: 964);
    this._locator.GetPlatformSpecificService().setConnectivityCallback((NetworkConnected) null);
  }

  private void keepScreenOn(bool keepScreenActive)
  {
    try
    {
      DeviceDisplay.KeepScreenOn = keepScreenActive;
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppError(LoggingContext.CODING, "Failed to update screen state:" + ex.Message, memberName: nameof (keepScreenOn), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceDatabaseViewModel.cs", sourceLineNumber: 977);
    }
  }

  private void CancelTokenResource()
  {
    this._cancellationToken.Cancel();
    this._cancellationToken.Dispose();
    this._cancellationToken = (CancellationTokenSource) null;
    this._cancellationToken = new CancellationTokenSource();
  }

  internal void EndDownloadSession()
  {
    Device.BeginInvokeOnMainThread((Action) (() => this.keepScreenOn(false)));
    this.CancelTokenResource();
    this.notFoundFileIds.Clear();
    this._userSession.TerminateDownloadSession();
    this._userSession.cancelDownload();
    this._userSession.signalTasks();
  }

  internal void applyWifiPolicy()
  {
    this._locator.GetPlatformSpecificService().setConnectivityCallback(new NetworkConnected(this.connectivityCallback));
    if (this.WiFiSwitchToggled)
    {
      this._loggingService.getLogger().LogAppInformation(LoggingContext.BINARY, "Wifi Only is activated", memberName: nameof (applyWifiPolicy), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceDatabaseViewModel.cs", sourceLineNumber: 1008);
      if (!this._locator.GetPlatformSpecificService().getCurrentNetworkProfile().Contains<ConnectionProfile>(ConnectionProfile.WiFi))
      {
        this._loggingService.getLogger().LogAppInformation(LoggingContext.BINARY, "No Wifi connection is detected", memberName: nameof (applyWifiPolicy), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceDatabaseViewModel.cs", sourceLineNumber: 1011);
        this.WifiOnly = true;
        if (this.NumberOfFilesToBeDownloaded > 0 || this.DBDownloadRequired || this.IsPPFMissing())
        {
          this.RemoteFiles = AppResource.ST_WIFI_RESTRICTION;
        }
        else
        {
          this.NumberOfRemoteFiles = this.NumberOfFilesToBeDownloaded.ToString();
          this.RemoteFiles = AppResource.ST_PAGE_DWL_REMOTE_FILES;
        }
      }
      else
      {
        this._loggingService.getLogger().LogAppInformation(LoggingContext.BINARY, "Wifi connection is detected", memberName: nameof (applyWifiPolicy), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceDatabaseViewModel.cs", sourceLineNumber: 1025);
        this.WifiOnly = this.NumberOfFilesToBeDownloaded <= 0 && !this.DBDownloadRequired && !this.IsPPFMissing();
        this.NumberOfRemoteFiles = this.NumberOfFilesToBeDownloaded.ToString();
        this.RemoteFiles = AppResource.ST_PAGE_DWL_REMOTE_FILES;
        this.DownloadDisabled = false;
      }
    }
    else if (this._locator.GetPlatformSpecificService().getCurrentNetworkProfile().Contains<ConnectionProfile>(ConnectionProfile.WiFi) || this._locator.GetPlatformSpecificService().getCurrentNetworkProfile().Contains<ConnectionProfile>(ConnectionProfile.Cellular))
    {
      this._loggingService.getLogger().LogAppInformation(LoggingContext.BINARY, "No restriction in downloading", memberName: nameof (applyWifiPolicy), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceDatabaseViewModel.cs", sourceLineNumber: 1038);
      this.WifiOnly = this.NumberOfFilesToBeDownloaded <= 0 && !this.DBDownloadRequired && !this.IsPPFMissing();
      this.RemoteFiles = AppResource.ST_PAGE_DWL_REMOTE_FILES;
      this.NumberOfRemoteFiles = this.NumberOfFilesToBeDownloaded.ToString();
      this.DownloadDisabled = false;
    }
    else
    {
      this._loggingService.getLogger().LogAppInformation(LoggingContext.BINARY, "No connectivity is detected", memberName: nameof (applyWifiPolicy), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceDatabaseViewModel.cs", sourceLineNumber: 1046);
      this.OrangeBarVisibility = false;
      this.WifiOnly = true;
      if (this.NumberOfFilesToBeDownloaded > 0 || this.DBDownloadRequired || this.IsPPFMissing())
      {
        this.RemoteFiles = AppResource.ST_NO_INTERNET;
      }
      else
      {
        this.RemoteFiles = AppResource.ST_PAGE_DWL_REMOTE_FILES;
        this.NumberOfRemoteFiles = this.NumberOfFilesToBeDownloaded.ToString();
      }
    }
  }

  public override void ViewAppearing()
  {
    base.ViewAppearing();
    if (this._userSession.isDownloadActive())
      this.keepScreenOn(true);
    else
      this.keepScreenOn(false);
    this.showActivityIndicatorForDownloadSection(true);
    MessagingCenter.Subscribe<HistoryViewModel>((object) this, "CheckBridgeSwitchValue", (Action<HistoryViewModel>) (sender => this._navigationService.Close((IMvxViewModel) this, new CancellationToken())), (HistoryViewModel) null);
    this.CheckNavigationStackList();
    this.DisplayFileStatus = this.GetFileStatusVisibility();
  }

  public override void ViewAppeared()
  {
    base.ViewAppeared();
    this.DownloadDisabled = true;
    this.initializeSettingsDataAsync();
  }

  private bool IsPPFMissing() => this.GetEnumbersForPpfs().Count > 0;

  private async void initializeSettingsDataAsync()
  {
    await Task.Run((Action) (() =>
    {
      try
      {
        this.ShowAuxiliaryFileStatus = false;
        this.ShouldDisplayDownloadSettings = true;
        Settings settings1 = CoreApp.settings.GetItem("DownloadSettings");
        if (settings1 != null && !string.IsNullOrEmpty(settings1.Value))
        {
          this.showActivityIndicatorForScreen(true);
          this.DownloadSettingsConfigured = true;
          this.SetSelectedDownloadSettings();
        }
        else
        {
          this.showActivityIndicatorForScreen(false);
          this.DownloadSettingsConfigured = false;
        }
        this.numberOfFilesDownloaded = this._metadataService.getLocalSize();
        this.NumberOfFilesToBeDownloaded = this.GetTotalFilesToDownload();
        this.DownloadableFileSizeLabelText = this.FileSizeToBeDownloaded;
        this.downloadedSMMEnumbers = UtilityFunctions.getDownloadedEnumbersFromDB(this._metadataService);
        this.showActivityIndicatorForScreen(false);
        this.ShowAuxiliaryFileStatus = this.NumberOfFilesToBeDownloaded == 0 && this.IsPPFMissing();
        this.RemoteFiles = AppResource.ST_PAGE_DWL_REMOTE_FILES;
        this.NumberOfRemoteFiles = this.NumberOfFilesToBeDownloaded.ToString();
        try
        {
          this._metadataService.updateUsedStorage();
        }
        catch (Exception ex)
        {
          this._loggingService.getLogger().LogAppFatal(LoggingContext.METADATA, "metadatadb null :" + ex?.ToString(), memberName: nameof (initializeSettingsDataAsync), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceDatabaseViewModel.cs", sourceLineNumber: 1131);
        }
        this.AreButtonsEnabled = true;
        Settings settings2 = CoreApp.settings.GetItem("lastUpdate");
        try
        {
          this.DBDownloadRequired = Convert.ToDateTime(settings2.Value, (IFormatProvider) CultureInfo.InvariantCulture).ToShortDateString() != DateTime.Today.ToShortDateString();
        }
        catch (Exception ex)
        {
          this.DBDownloadRequired = true;
          this._loggingService.getLogger().LogAppFatal(LoggingContext.BINARY, "Date Check Failed:" + ex?.ToString(), memberName: nameof (initializeSettingsDataAsync), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceDatabaseViewModel.cs", sourceLineNumber: 1143);
        }
        this.applyWifiPolicy();
        if (settings2 != null && settings2.Value != "" && !settings2.Value.StartsWith("USE_TEXT"))
        {
          try
          {
            this.LastUpdateLabelText = Convert.ToDateTime(settings2.Value, (IFormatProvider) CultureInfo.InvariantCulture).ToShortDateString();
          }
          catch (Exception ex)
          {
            this._loggingService.getLogger().LogAppFatal(LoggingContext.LOCAL, "Date parsing failed:" + ex?.ToString(), memberName: nameof (initializeSettingsDataAsync), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceDatabaseViewModel.cs", sourceLineNumber: 1154);
          }
          this.UsedStorageLabelText = this._metadataService.GetFileSize();
          this.AvailableStorage = this._locator.GetPlatformSpecificService().GetMemoryStorage(false);
          CoreApp.settings.GetItem("lastCheck");
          this.DbRequiredLabelText = AppResource.ST_PAGE_DWL_STATUS;
        }
        else if (settings2 != null && settings2.Value != "" && settings2.Value.StartsWith("USE_TEXT"))
        {
          switch (settings2.Value.Split(':')[1])
          {
            case "ST_PAGE_FAILED_XSUM":
              this.LastUpdateLabelText = AppResource.ST_PAGE_FAILED_XSUM;
              break;
            case "ST_PAGE_FAILED_DB":
              this.LastUpdateLabelText = AppResource.ST_PAGE_FAILED_DB;
              break;
            default:
              this.LastUpdateLabelText = " n/a";
              break;
          }
        }
        else
          this.LastUpdateLabelText = " n/a";
        if (!this._userSession.isDownloadActive())
        {
          try
          {
            this.OrangeBarVisibility = false;
            this.DownloadedApplianceFile = AppResource.ST_PAGE_DWL_LOCAL_FILES;
            this.NumOfFilesText = this.numberOfFilesDownloaded.ToString();
            this.DownloadDataButtonText = this.GetDownloadDataButtonTextWithSize();
            this.ScheduleDownloadLabelText = AppResource.ST_PAGE_DWL_SCHD;
          }
          catch (Exception ex)
          {
            this._loggingService.getLogger().LogAppFatal(LoggingContext.METADATA, ex.Message, ex, nameof (initializeSettingsDataAsync), "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceDatabaseViewModel.cs", 1197);
          }
        }
        else
        {
          this.OrangeBarVisibility = true;
          this.DownloadDataButtonText = AppResource.ST_PAGE_CANCEL;
          this.DownloadedApplianceFile = AppResource.ST_PAGE_DWL_LOCAL_FILES;
          this.NumOfFilesText = this.numberOfFilesDownloaded.ToString();
        }
        this.DownloadDisabled = this.WifiOnly || !this.DownloadSettingsConfigured;
      }
      catch (Exception ex)
      {
        this._loggingService.getLogger().LogAppFatal(LoggingContext.METADATA, ex.Message, ex, nameof (initializeSettingsDataAsync), "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceDatabaseViewModel.cs", 1211);
      }
      this.showActivityIndicatorForDownloadSection(false);
      this.showActivityIndicatorForScreen(false);
    }));
  }

  public ICommand DownloadNowCommand { internal set; get; }

  public bool AreButtonsEnabled
  {
    get => this._areButtonsEnabled;
    set
    {
      if (this._areButtonsEnabled == value)
        return;
      this._areButtonsEnabled = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.AreButtonsEnabled));
    }
  }

  public bool ranfromConstructor { get; private set; }

  public bool ranfromprepare { get; private set; }

  public bool cameFromCatTester
  {
    get => this._cameFromCatTester;
    private set
    {
      this._cameFromCatTester = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.cameFromCatTester));
    }
  }

  public int NumberOfFilesToBeDownloaded { get; private set; }

  public int numberOfFilesDownloaded { get; private set; }

  private void SetPropertiesAfterSuccessfulDownloading(DownloadProxy proxy)
  {
    ++this.numberOfFilesDownloaded;
    this.NumOfFilesText = this.numberOfFilesDownloaded.ToString();
    --this.NumberOfFilesToBeDownloaded;
    this.NumberOfRemoteFiles = this.NumberOfFilesToBeDownloaded.ToString();
    this.downloadableFileSize -= proxy.GetFileSize();
    this.DownloadableFileSizeLabelText = FileSizeFormatter.FormatSize(this.downloadableFileSize);
    this.DownloadedApplianceFile = AppResource.ST_PAGE_DWL_LOCAL_FILES;
    this.RemoteFiles = AppResource.ST_PAGE_DWL_REMOTE_FILES;
    this.UsedStorageLabelText = this._metadataService.GetFileSize();
    this.AvailableStorage = this._locator.GetPlatformSpecificService().GetMemoryStorage(false);
  }

  internal string DownloadTypeText() => this.normalDownloadingFiles ? "Normal" : "Repeat";

  private void CheckIfDownloadSuccessful(DownloadProxy proxy, DownloadStatus status)
  {
    string enumber = proxy.GetEnumber();
    if (this.smmMaterials.Contains(enumber) && proxy.FileType == FileType.BundledPpf)
    {
      if (!UtilityFunctions.StorePpfFiles(proxy, this._userSession, this._loggingService, this._metadataService))
      {
        this.AddBinaryFileToFailedFiles(proxy);
        this._loggingService.getLogger().LogAppError(LoggingContext.LOCAL, "Bundle Ppf's failed to get extracted from PPF information file " + enumber, memberName: nameof (CheckIfDownloadSuccessful), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceDatabaseViewModel.cs", sourceLineNumber: 1287);
      }
      else
        this.downloadedSMMEnumbers.Add(enumber);
      if (this.normalDownloadingFiles)
        this.getBinariesForENumber(proxy, status, DeviceClass.SMM);
      else
        this.ContinueWithNextFile();
    }
    else
    {
      if (!this._metadataService.markAsDownloaded(proxy))
        this.AddBinaryFileToFailedFiles(proxy);
      else
        this.SetPropertiesAfterSuccessfulDownloading(proxy);
      this.ContinueWithNextFile();
    }
  }

  private List<DownloadProxy> GetNotTriedBinaries(
    List<DownloadProxy> dbBinaries,
    List<DownloadProxy> failedBinaries)
  {
    if (dbBinaries.Count <= 0)
      return new List<DownloadProxy>();
    List<string> failedBinaryFileIds = failedBinaries.Where<DownloadProxy>((Func<DownloadProxy, bool>) (y => y.Module != null || y.Document != null)).Select<DownloadProxy, string>((Func<DownloadProxy, string>) (x => x.Module == null ? x.Document.fileId : x.Module.fileId)).ToList<string>();
    return dbBinaries.Where<DownloadProxy>((Func<DownloadProxy, bool>) (x => x.Module == null ? !failedBinaryFileIds.Contains(x.Document.fileId) : !failedBinaryFileIds.Contains(x.Module.fileId))).ToList<DownloadProxy>();
  }

  private void getBinariesForENumber(
    DownloadProxy proxy,
    DownloadStatus status,
    DeviceClass deviceClass)
  {
    string enumber = proxy.GetEnumber();
    List<DownloadProxy> dbBinaries = new List<DownloadProxy>();
    if (!this.DownloadedBinariesForEnumbers.Contains(enumber))
    {
      if (deviceClass == DeviceClass.SMM)
      {
        List<DownloadProxy> list = this._metadataService.getModulesForENumber(enumber).Select<module_with_enumber, DownloadProxy>((Func<module_with_enumber, DownloadProxy>) (x => new DownloadProxy()
        {
          Module = (module) x,
          vib = proxy.vib,
          ki = proxy.ki
        })).ToList<DownloadProxy>();
        dbBinaries.AddRange((IEnumerable<DownloadProxy>) list);
      }
      List<DownloadProxy> list1 = this._metadataService.getDocumentsForENumber(enumber, deviceClass).Select<document, DownloadProxy>((Func<document, DownloadProxy>) (x => new DownloadProxy()
      {
        Document = x,
        vib = proxy.vib,
        ki = proxy.ki
      })).ToList<DownloadProxy>();
      dbBinaries.AddRange((IEnumerable<DownloadProxy>) list1);
    }
    if (this.corruptedFiles.Count > 0 || this.notFoundFileIds.Count > 0)
    {
      List<DownloadProxy> failedBinaries = new List<DownloadProxy>();
      failedBinaries.AddRange((IEnumerable<DownloadProxy>) this.corruptedFiles);
      failedBinaries.AddRange((IEnumerable<DownloadProxy>) this.notFoundFileIds);
      dbBinaries = this.GetNotTriedBinaries(dbBinaries, failedBinaries);
    }
    if (this.normalDownloadingFiles && this.failedFilesToDownload.Count > 0)
      dbBinaries = this.GetNotTriedBinaries(dbBinaries, this.failedFilesToDownload);
    else if (this.repeatDownloadingOfFailedFiles && this.failedFilesToDownloadAtRepeat.Count > 0)
      dbBinaries = this.GetNotTriedBinaries(dbBinaries, this.failedFilesToDownloadAtRepeat);
    if (dbBinaries.Count > 0)
    {
      this._binaryEnumerator = dbBinaries.GetEnumerator();
      this._binaryEnumerator.MoveNext();
      this.DownloadBinary(this._binaryEnumerator.Current);
    }
    else
    {
      if (this._binaryEnumerator.Current != null)
        this._binaryEnumerator = new List<DownloadProxy>().GetEnumerator();
      this.ContinueWithNextFile();
    }
  }

  private void DownloadBinary(DownloadProxy proxy)
  {
    this._userSession.getBinary(proxy, FileType.Binary, new downloadCallback(this.DownloadCB), (iService5.Core.Services.User.progressCallback) (progress => { }));
  }

  internal void AddBinaryFileToFailedFiles(DownloadProxy proxy)
  {
    this.corruptedFiles.Add(proxy);
    string str = proxy.FileType != FileType.BundledPpf ? (proxy.Module != null ? proxy.GetModuleBinaryFileName() : proxy.Document.toFileName()) : proxy.GetEnumber();
    this._loggingService.getLogger().LogAppInformation(LoggingContext.LOCAL, $"{this.DownloadTypeText()} Download procedure, File {str} is not marked as downloaded", memberName: nameof (AddBinaryFileToFailedFiles), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceDatabaseViewModel.cs", sourceLineNumber: 1399);
  }

  internal void ContinueWithNextFile()
  {
    if (this.CancelActive)
    {
      this._loggingService.getLogger().LogAppInformation(LoggingContext.BACKEND, "CANCELED", memberName: nameof (ContinueWithNextFile), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceDatabaseViewModel.cs", sourceLineNumber: 1407);
      this.OrangeBarVisibility = false;
      this.DownloadDataButtonText = this.GetDownloadDataButtonTextWithSize();
    }
    else if (this.normalDownloadingFiles)
    {
      string enumber = this.GetDownloadEnumeratorCurrent().GetEnumber();
      if (this._binaryEnumerator.Current != null && this._binaryEnumerator.MoveNext())
        this.DownloadBinary(this._binaryEnumerator.Current);
      else if (this._downloadEnumerator.MoveNext())
      {
        this.DownloadedBinariesForEnumbers.Add(enumber);
        if (this._binaryEnumerator.Current != null)
          this._binaryEnumerator = new List<DownloadProxy>().GetEnumerator();
        this.DownloadFile(this._downloadEnumerator.Current);
      }
      else
        this.CheckIfAnyFirmwareFails();
    }
    else
      this.CheckIfAnyFirmwareFails();
  }

  private void CheckIfAnyFirmwareFails()
  {
    if (this.failedFilesToDownload.Any<DownloadProxy>())
    {
      this.normalDownloadingFiles = false;
      if (!this.repeatDownloadingOfFailedFiles)
      {
        this.repeatDownloadingOfFailedFiles = true;
        this.errorsDownloadFiles = 0;
        Logger logger = this._loggingService.getLogger();
        int num = this.numberOfFilesDownloaded;
        string str1 = num.ToString();
        num = this.NumberOfFilesToBeDownloaded;
        string str2 = num.ToString();
        string message = $"Normal download procedure has been completed, repeat procedure started. Number of downloaded files: {str1}, residual files: {str2}";
        logger.LogAppInformation(LoggingContext.BACKEND, message, memberName: nameof (CheckIfAnyFirmwareFails), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceDatabaseViewModel.cs", sourceLineNumber: 1451);
        this._downloadEnumeratorF = this.failedFilesToDownload.GetEnumerator();
        this._downloadEnumeratorF.MoveNext();
        this.DownloadFileOnRetry(this._downloadEnumeratorF.Current);
      }
      else if (this._downloadEnumeratorF.MoveNext())
      {
        this.DownloadFileOnRetry(this._downloadEnumeratorF.Current);
      }
      else
      {
        this._loggingService.getLogger().LogAppInformation(LoggingContext.BACKEND, $"Repeat download procedure has been completed. Number of downloaded files both for normal and repeat: {this.numberOfFilesDownloaded.ToString()}, residual files: {this.NumberOfFilesToBeDownloaded.ToString()}", memberName: nameof (CheckIfAnyFirmwareFails), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceDatabaseViewModel.cs", sourceLineNumber: 1462);
        this.repeatDownloadingOfFailedFiles = false;
        this.UpdateUIUponDownloadCompletion();
      }
    }
    else
    {
      this.repeatDownloadingOfFailedFiles = false;
      this.UpdateUIUponDownloadCompletion();
    }
  }

  private void DownloadFileOnRetry(DownloadProxy proxy)
  {
    try
    {
      ThreadPool.QueueUserWorkItem((WaitCallback) (_param1 => this._userSession.getBinary(proxy, proxy.FileType, new downloadCallback(this.DownloadCB), (iService5.Core.Services.User.progressCallback) (progress => { }))), (object) this._cancellationToken);
    }
    catch (Exception ex)
    {
      this.CancelTokenResource();
      this._loggingService.getLogger().LogAppError(LoggingContext.BACKEND, ex.Message, memberName: nameof (DownloadFileOnRetry), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceDatabaseViewModel.cs", sourceLineNumber: 1485);
    }
  }

  private List<MaterialStatistics> GetEnumbersForPpfs()
  {
    List<MaterialStatistics> source = new List<MaterialStatistics>();
    if (this.relevantMaterials != null && this.relevantMaterials.Count > 0)
      source = this.relevantMaterials[DeviceClass.SMM];
    return !this.downloadSettingsConfigured || source.Count == 0 ? new List<MaterialStatistics>() : (this.downloadedSMMEnumbers.Count > 0 ? source.Where<MaterialStatistics>((Func<MaterialStatistics, bool>) (u => !this.downloadedSMMEnumbers.Contains(u.material))) : (IEnumerable<MaterialStatistics>) source).ToList<MaterialStatistics>();
  }

  private DownloadProxy GetDownloadEnumeratorCurrent()
  {
    if (!this.normalDownloadingFiles)
      return this._downloadEnumeratorF.Current;
    return this._binaryEnumerator.Current != null ? this._binaryEnumerator.Current : this._downloadEnumerator.Current;
  }

  public void DownloadCB(DownloadStatus status)
  {
    DownloadProxy enumeratorCurrent = this.GetDownloadEnumeratorCurrent();
    int num;
    switch (status)
    {
      case DownloadStatus.FAILED:
      case DownloadStatus.TIMEOUT:
        num = 1;
        break;
      case DownloadStatus.COMPLETED:
        this.CheckIfDownloadSuccessful(enumeratorCurrent, status);
        return;
      default:
        num = status == DownloadStatus.EXCEPTION ? 1 : 0;
        break;
    }
    if (num != 0)
    {
      if (this.normalDownloadingFiles)
      {
        ++this.errorsDownloadFiles;
        this.failedFilesToDownload.Add(enumeratorCurrent);
      }
      else if (this.repeatDownloadingOfFailedFiles)
      {
        ++this.errorsDownloadFailedFiles;
        this.failedFilesToDownloadAtRepeat.Add(enumeratorCurrent);
      }
      else
        this._loggingService.getLogger().LogAppInformation(LoggingContext.BACKEND, "Number of failed files : " + this.errorsDownloadFailedFiles.ToString(), memberName: nameof (DownloadCB), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceDatabaseViewModel.cs", sourceLineNumber: 1543);
      this.ContinueWithNextFile();
    }
    else
    {
      switch (status)
      {
        case DownloadStatus.UNAUTHORIZED:
          this.EndDownloadSession();
          CoreApp.settings.UpdateItem(new Settings("UnAuthorisedStatusReceivedViewModelName", ((object) this).GetType().Name));
          this._loggingService.getLogger().LogAppInformation(LoggingContext.BACKEND, "View model to navigate upon token refresh is stored successfully", memberName: nameof (DownloadCB), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceDatabaseViewModel.cs", sourceLineNumber: 1551);
          if (this.tokenExpirePopUpAlreadyCalled)
            break;
          this._alertService.ShowMessageAlertWithKey("JWT_TOKEN_EXPIRED", AppResource.WARNING_TEXT);
          this._navigationService.Navigate<LoginViewModel>((IMvxBundle) null, new CancellationToken());
          break;
        case DownloadStatus.SERVER_CERTIFICATE_UNKNOWN:
          this.EndDownloadSession();
          this._loggingService.getLogger().LogAppError(LoggingContext.BACKEND, "Server certificate validation failed", memberName: nameof (DownloadCB), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceDatabaseViewModel.cs", sourceLineNumber: 1562);
          this._alertService.ShowMessageAlertWithKey("SERVER_TRUST_FAILURE", AppResource.WARNING_TEXT);
          this._navigationService.Navigate<LoginViewModel>((IMvxBundle) null, new CancellationToken());
          break;
        default:
          if (enumeratorCurrent.FileType == FileType.Binary)
            this.notFoundFileIds.Add(enumeratorCurrent);
          this.ContinueWithNextFile();
          break;
      }
    }
  }

  public async Task DownloadNowButtonPressed()
  {
    if (this._userSession.isDownloadActive())
    {
      this.EndDownloadSession();
      this.CancelActive = true;
      this._cancellationToken = new CancellationTokenSource();
      this.GetTotalFilesToDownload();
      this.DownloadDataButtonText = this.GetDownloadDataButtonTextWithSize();
      this.showActivityIndicatorForDownloadSection(false);
      this.normalDownloadingFiles = true;
      this.repeatDownloadingOfFailedFiles = false;
      this.OrangeBarVisibility = false;
    }
    else
    {
      this.LogNetworkDetails();
      this.LogDownloadSettings();
      if (this._userSession.IsHostReachable())
      {
        this.keepScreenOn(true);
        await this.CheckIfMetadataUpdated();
        this.keepScreenOn(true);
        try
        {
          this.normalDownloadingFiles = true;
          this.repeatDownloadingOfFailedFiles = false;
          this.errorsDownloadFiles = 0;
          this.errorsDownloadFailedFiles = 0;
          this.corruptedFiles = new List<DownloadProxy>();
          this.notFoundFileIds = new List<DownloadProxy>();
          this.failedFilesToDownload = new List<DownloadProxy>();
          this.failedFilesToDownloadAtRepeat = new List<DownloadProxy>();
          this.CancelActive = false;
          this.DownloadDataButtonText = AppResource.ST_PAGE_CANCEL;
          this.RemoteFiles = AppResource.ST_PAGE_DWL_REMOTE_FILES;
          this.OrangeBarVisibility = true;
          await Task.Run((Action) (() =>
          {
            this._userSession.designalTasks();
            List<DownloadProxy> filesToDownload = this.GetFilesToDownload();
            this.NumberOfFilesToBeDownloaded = this.GetTotalFilesToDownload();
            this._userSession.StartDownloadSession();
            this._loggingService.getLogger().LogAppDebug(LoggingContext.BACKEND, "allFilesToDownload count : " + filesToDownload.Count.ToString(), memberName: nameof (DownloadNowButtonPressed), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceDatabaseViewModel.cs", sourceLineNumber: 1625);
            this._loggingService.getLogger().LogAppDebug(LoggingContext.BACKEND, "numberOfFilesToBeDownloaded count : " + this.NumberOfFilesToBeDownloaded.ToString(), memberName: nameof (DownloadNowButtonPressed), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceDatabaseViewModel.cs", sourceLineNumber: 1626);
            this.NumberOfRemoteFiles = this.NumberOfFilesToBeDownloaded.ToString();
            this._downloadEnumerator = filesToDownload.GetEnumerator();
            this._downloadEnumerator.MoveNext();
            this.DownloadFile(this._downloadEnumerator.Current);
          }));
        }
        catch (Exception ex)
        {
          this.UpdateUIUponDownloadCompletion();
          this._loggingService.getLogger().LogAppInformation(LoggingContext.BINARY, ex.Message, memberName: nameof (DownloadNowButtonPressed), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceDatabaseViewModel.cs", sourceLineNumber: 1636);
        }
      }
      else
      {
        this.OrangeBarVisibility = false;
        this.keepScreenOn(false);
        this.LogNetworkDetails();
        Device.BeginInvokeOnMainThread((Action) (async () => await this._alertService.ShowMessageAlertWithKey("NO_INTERNET", AppResource.ERROR_TITLE)));
      }
    }
  }

  internal void LogDownloadSettings()
  {
    List<Country> selectedCountryList = UtilityFunctions.GetSelectedCountryList(this._metadataService);
    this._loggingService.getLogger().LogAppInformation(LoggingContext.USERSESSION, $"Download settings: {this.selectedDeviceClass} Devices, {(this.isCountryRelevant ? (selectedCountryList.Count > 0 ? string.Join("','", (IEnumerable<string>) selectedCountryList.Select<Country, string>((Func<Country, string>) (x => x.country.ToUpper())).ToList<string>()) : UtilityFunctions.GetUserCountryCode(this._secureStorageService).ToUpperInvariant()) : "Full")} Download, {(this.isFileSizeSwitchToggled ? "Larger" : "Smaller")} than {BuildProperties.BinarySizeThreshold}MB sized E-Numbers", memberName: nameof (LogDownloadSettings), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceDatabaseViewModel.cs", sourceLineNumber: 1655);
  }

  private void LogNetworkDetails()
  {
    IEnumerable<ConnectionProfile> currentNetworkProfile = this._locator.GetPlatformSpecificService().getCurrentNetworkProfile();
    NetworkAccess currentNetworkStatus = this._locator.GetPlatformSpecificService().getCurrentNetworkStatus();
    Logger logger = this._loggingService.getLogger();
    logger.LogAppInformation(LoggingContext.USERSESSION, $"Connection profile at the moment are: {currentNetworkStatus}", memberName: nameof (LogNetworkDetails), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceDatabaseViewModel.cs", sourceLineNumber: 1667);
    string str = string.Join<ConnectionProfile>("|", currentNetworkProfile);
    logger.LogAppInformation(LoggingContext.USERSESSION, "Connection profile at the moment are: " + str, memberName: nameof (LogNetworkDetails), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceDatabaseViewModel.cs", sourceLineNumber: 1670);
    logger.LogAppInformation(LoggingContext.LOCAL, $"App is connected to appliance wifi: {this._appliance.isConnectedToApplianceWifi()}", memberName: nameof (LogNetworkDetails), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceDatabaseViewModel.cs", sourceLineNumber: 1672);
    logger.LogAppInformation(LoggingContext.LOCAL, $"App is connected to bridge wifi: {this._appliance.isConnectedToBridgeWifi()}", memberName: nameof (LogNetworkDetails), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceDatabaseViewModel.cs", sourceLineNumber: 1673);
    logger.LogAppInformation(LoggingContext.LOCAL, $"Bridge setting switch toggled status is: {this.BridgeSettingSwitchToggled}", memberName: nameof (LogNetworkDetails), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceDatabaseViewModel.cs", sourceLineNumber: 1674);
    logger.LogAppInformation(LoggingContext.LOCAL, $"Download only over wifi switch toggled status is: {this.WiFiSwitchToggled}", memberName: nameof (LogNetworkDetails), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceDatabaseViewModel.cs", sourceLineNumber: 1675);
    logger.LogAppInformation(LoggingContext.LOCAL, $"iService5 offline switch toggled status is: {this.OfflineSwitchToggled}", memberName: nameof (LogNetworkDetails), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceDatabaseViewModel.cs", sourceLineNumber: 1676);
  }

  internal async Task CheckIfMetadataUpdated()
  {
    try
    {
      Settings lastUpdate = CoreApp.settings.GetItem("lastUpdate");
      DateTime dateTime;
      int num;
      if (lastUpdate != null && !(lastUpdate.Value == "") && !lastUpdate.Value.StartsWith("USE_TEXT"))
      {
        dateTime = Convert.ToDateTime(lastUpdate.Value, (IFormatProvider) CultureInfo.InvariantCulture);
        string shortDateString1 = dateTime.ToShortDateString();
        dateTime = DateTime.Today;
        string shortDateString2 = dateTime.ToShortDateString();
        if (!(shortDateString1 != shortDateString2))
        {
          num = 0;
          goto label_5;
        }
      }
      num = !this._userSession.isMetadataSessionActive() ? 1 : 0;
label_5:
      if (num != 0)
      {
        IMvxNavigationService navigationService = this._navigationService;
        DetailNavigationArgs detailNavigationArgs = new DetailNavigationArgs();
        detailNavigationArgs.ReturnToFlashing = false;
        detailNavigationArgs._calledFromDB = true;
        CancellationToken cancellationToken = new CancellationToken();
        DetailReturnArgs detailReturnArgs = await navigationService.Navigate<StatusViewModel, DetailNavigationArgs, DetailReturnArgs>(detailNavigationArgs, (IMvxBundle) null, cancellationToken);
        lastUpdate = CoreApp.settings.GetItem("lastUpdate");
        if (lastUpdate != null && lastUpdate.Value != "" && !lastUpdate.Value.StartsWith("USE_TEXT"))
        {
          dateTime = Convert.ToDateTime(lastUpdate.Value, (IFormatProvider) CultureInfo.InvariantCulture);
          this.LastUpdateLabelText = dateTime.ToShortDateString();
        }
      }
      lastUpdate = (Settings) null;
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppInformation(LoggingContext.BINARY, ex.Message, memberName: nameof (CheckIfMetadataUpdated), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceDatabaseViewModel.cs", sourceLineNumber: 1700);
    }
  }

  internal List<DownloadProxy> GetFilesToDownload()
  {
    List<DownloadProxy> filesToDownload = new List<DownloadProxy>();
    if (!this.downloadSettingsConfigured)
      return filesToDownload;
    if (this.shouldDisplayDownloadSettings && this.relevantMaterials != null)
    {
      List<DownloadProxy> list1 = this._metadataService.getZ9KDfiles().OrderByDescending<document, long>((Func<document, long>) (z => z.fileSize)).Select<document, DownloadProxy>((Func<document, DownloadProxy>) (x => new DownloadProxy()
      {
        Document = x
      })).ToList<DownloadProxy>();
      filesToDownload.AddRange((IEnumerable<DownloadProxy>) list1);
      List<MaterialStatistics> source = new List<MaterialStatistics>();
      List<MaterialStatistics> relevantMaterial1 = this.relevantMaterials[DeviceClass.SMM];
      List<MaterialStatistics> relevantMaterial2 = this.relevantMaterials[DeviceClass.NON_SMM];
      source.AddRange((IEnumerable<MaterialStatistics>) relevantMaterial1);
      source.AddRange((IEnumerable<MaterialStatistics>) relevantMaterial2);
      this.smmMaterials = relevantMaterial1.Select<MaterialStatistics, string>((Func<MaterialStatistics, string>) (x => x.material)).ToList<string>();
      List<DownloadProxy> list2 = source.Select<MaterialStatistics, DownloadProxy>((Func<MaterialStatistics, DownloadProxy>) (x =>
      {
        Tuple<string, string> vibAndKi = UtilityFunctions.getVibAndKi(x.material);
        return new DownloadProxy()
        {
          vib = vibAndKi.Item1,
          ki = vibAndKi.Item2
        };
      })).ToList<DownloadProxy>();
      filesToDownload.AddRange((IEnumerable<DownloadProxy>) list2);
      return filesToDownload;
    }
    List<DownloadProxy> list = this._metadataService.getModulesDeltaSet().Select<module_with_enumber, DownloadProxy>((Func<module_with_enumber, DownloadProxy>) (x => new DownloadProxy()
    {
      Module = (module) x
    })).ToList<DownloadProxy>();
    list.AddRange(this._metadataService.getMissingDocuments().Select<document, DownloadProxy>((Func<document, DownloadProxy>) (x => new DownloadProxy()
    {
      Document = x
    })));
    return list;
  }

  internal void DownloadFile(DownloadProxy proxy)
  {
    try
    {
      ThreadPool.QueueUserWorkItem((WaitCallback) (_param1 =>
      {
        string enumber = proxy.GetEnumber();
        if (this.smmMaterials.Contains(enumber))
        {
          if (this.downloadedSMMEnumbers.Contains(enumber))
            this.getBinariesForENumber(proxy, DownloadStatus.STARTED, DeviceClass.SMM);
          else
            this._userSession.getBinary(proxy, FileType.BundledPpf, new downloadCallback(this.DownloadCB), (iService5.Core.Services.User.progressCallback) (progress => { }));
        }
        else if (proxy.Document != null)
          this._userSession.getBinary(proxy, FileType.Binary, new downloadCallback(this.DownloadCB), (iService5.Core.Services.User.progressCallback) (progress => { }));
        else
          this.getBinariesForENumber(proxy, DownloadStatus.STARTED, DeviceClass.NON_SMM);
      }), (object) this._cancellationToken);
    }
    catch (Exception ex)
    {
      this.CancelTokenResource();
      this._loggingService.getLogger().LogAppError(LoggingContext.LOCAL, ex.Message, memberName: nameof (DownloadFile), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceDatabaseViewModel.cs", sourceLineNumber: 1781);
    }
  }

  public void SetSelectedDownloadSettings()
  {
    try
    {
      Settings settings1 = CoreApp.settings.GetItem("DownloadSettings");
      if (settings1 != null)
      {
        if (settings1.Value != "")
        {
          Settings settings2 = CoreApp.settings.GetItem("FileSizeSwitchToggled");
          if (settings2 != null && settings2.Value != "")
            this.isFileSizeSwitchToggled = bool.Parse(settings2.Value);
          Settings settings3 = CoreApp.settings.GetItem("SelectedDeviceClass");
          if (settings3 != null && settings3.Value != "")
          {
            this.isCountryRelevant = string.Equals(settings1.Value, DownloadOption.COUNTRY_RELEVANT.ToString());
            this.selectedDeviceClass = settings3.Value;
          }
        }
      }
      else
      {
        this._loggingService.getLogger().LogAppInformation(LoggingContext.LOCAL, "No configured settings found.", memberName: nameof (SetSelectedDownloadSettings), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceDatabaseViewModel.cs", sourceLineNumber: 1812);
        this.selectedDeviceClass = DeviceClass.SMM.ToString();
        this.isCountryRelevant = true;
        this.isFileSizeSwitchToggled = false;
      }
      this.relevantMaterials = UtilityFunctions.GetRelevantMaterials(this.isCountryRelevant, this.selectedDeviceClass, this.isFileSizeSwitchToggled, this._secureStorageService, UtilityFunctions.GetSelectedCountryList(this._metadataService));
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppError(LoggingContext.LOCAL, ex.Message, memberName: nameof (SetSelectedDownloadSettings), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceDatabaseViewModel.cs", sourceLineNumber: 1822);
    }
  }

  internal void CheckSelectedDownloadSettings()
  {
    Settings settings1 = CoreApp.settings.GetItem("DownloadSettings");
    if (settings1 != null)
    {
      if (!(settings1.Value != ""))
        return;
      Settings settings2 = CoreApp.settings.GetItem("FileSizeSwitchToggled");
      if (settings2 != null && settings2.Value != "")
        this.isFileSizeSwitchToggled = bool.Parse(settings2.Value);
      Settings settings3 = CoreApp.settings.GetItem("SelectedDeviceClass");
      if (settings3 != null && settings3.Value != "")
      {
        this.isCountryRelevant = string.Equals(settings1.Value, DownloadOption.COUNTRY_RELEVANT.ToString());
        this.selectedDeviceClass = settings3.Value;
      }
    }
    else
    {
      this._loggingService.getLogger().LogAppInformation(LoggingContext.LOCAL, "No configured settings found.", memberName: nameof (CheckSelectedDownloadSettings), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceDatabaseViewModel.cs", sourceLineNumber: 1850);
      this.selectedDeviceClass = DeviceClass.SMM.ToString();
      this.isCountryRelevant = true;
      this.isFileSizeSwitchToggled = false;
    }
  }

  public int GetTotalFilesToDownload()
  {
    int totalFilesToDownload = 0;
    if (!this.BridgeSettingSwitchToggled)
      this.selectedDeviceClass = DeviceClass.SMM.ToString();
    if (this.ShouldDisplayDownloadSettings)
    {
      if (this.DownloadSettingsConfigured)
      {
        this.statistics = UtilityFunctions.GetDownloadStatistics(this.isCountryRelevant, this.selectedDeviceClass, this.isFileSizeSwitchToggled, this._secureStorageService, UtilityFunctions.GetSelectedCountryList(this._metadataService));
        if (this.statistics != null)
        {
          long fileSize = this.statistics.fileSize;
          this.FileSizeToBeDownloaded = fileSize == 0L ? "0" : FileSizeFormatter.FormatSize(fileSize);
          this.downloadableFileSize = fileSize;
          this.DisplayPendingFileSize = fileSize != 0L;
          totalFilesToDownload = this.statistics.fileCount;
        }
      }
    }
    else
      totalFilesToDownload = this._metadataService.getDeltaSize();
    return totalFilesToDownload;
  }

  public override void Prepare(bool parameter)
  {
    this.ranfromprepare = true;
    this.BridgeSettingSwitchToggled = parameter;
  }

  public void UpdateUIUponDownloadCompletion()
  {
    this.OrangeBarVisibility = false;
    this.DownloadDataButtonText = this.GetDownloadDataButtonTextWithSize();
    this._loggingService.getLogger().LogAppError(LoggingContext.BACKEND, "DownloadNow terminated", memberName: nameof (UpdateUIUponDownloadCompletion), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceDatabaseViewModel.cs", sourceLineNumber: 1891);
    if (this.errorsDownloadFailedFiles > 0 || this.corruptedFiles.Count > 0 || this.errorsDownloadFiles > 0)
      this._alertService.ShowMessageAlertWithKey("FIRMWARE_DL_FAIL_MSG", AppResource.WARNING_TEXT);
    else if (this.notFoundFileIds.Count > 0)
      this._alertService.ShowMessageAlertWithKey("FIRMWARE_DL_NOT_FOUND_MSG", AppResource.WARNING_TEXT);
    this.DownloadDisabled = false;
    this.NumberOfRemoteFiles = this.NumberOfFilesToBeDownloaded.ToString();
    if (this.NumberOfFilesToBeDownloaded <= 0)
    {
      this.ShowAuxiliaryFileStatus = this.IsPPFMissing();
      this.DownloadDisabled = true;
    }
    else
      this.DownloadDisabled = false;
    this.GetTotalFilesToDownload();
    this.DownloadDataButtonText = this.GetDownloadDataButtonTextWithSize();
    this.showActivityIndicatorForDownloadSection(false);
    this.EndDownloadSession();
  }

  public void UpdateUiAfterDownloadingSessionError()
  {
    this.EndDownloadSession();
    this.OrangeBarVisibility = false;
    this.GetTotalFilesToDownload();
    this.DownloadDataButtonText = this.GetDownloadDataButtonTextWithSize();
    this.DownloadableFileSizeLabelText = this.FileSizeToBeDownloaded;
    this.RemoteFiles = AppResource.ST_PAGE_DWL_REMOTE_FILES;
    this.NumberOfRemoteFiles = this.NumberOfFilesToBeDownloaded.ToString();
  }

  public void UpdateUIAfterEveryDOwnload()
  {
    this.statistics = UtilityFunctions.GetDownloadStatistics(this.isCountryRelevant, this.selectedDeviceClass, this.isFileSizeSwitchToggled, this._secureStorageService, UtilityFunctions.GetSelectedCountryList(this._metadataService));
  }

  internal string GetDownloadDataButtonTextWithSize()
  {
    return !string.IsNullOrEmpty(this.FileSizeToBeDownloaded) && this.FileSizeToBeDownloaded != "0" ? $"{AppResource.ST_PAGE_DWL_NOW} ({this.FileSizeToBeDownloaded})" : AppResource.ST_PAGE_DWL_NOW;
  }

  private void showActivityIndicatorForDownloadSection(bool show)
  {
    Device.BeginInvokeOnMainThread((Action) (() =>
    {
      this.DisplayActivityIndicator = show;
      if (!show)
        return;
      this.IsBusy = !this.IsBusy;
    }));
  }

  private void showActivityIndicatorForScreen(bool show)
  {
    Device.BeginInvokeOnMainThread((Action) (() =>
    {
      this.IsBusy = show;
      if (!show)
        return;
      this.DisplayActivityIndicator = !show;
    }));
  }
}
