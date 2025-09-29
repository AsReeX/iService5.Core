// Decompiled with JetBrains decompiler
// Type: iService5.Core.ViewModels.NonSmmConnectionViewModel
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Appliance;
using iService5.Core.Services.Data;
using iService5.Core.Services.Helpers;
using iService5.Core.Services.Helpers.Topogram;
using iService5.Core.Services.Localisation;
using iService5.Core.Services.Platform;
using iService5.Core.Services.User;
using iService5.Core.Services.User.ActivityLogging;
using iService5.Ssh.models;
using MvvmCross;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
//using Xamarin.Forms;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

#nullable disable
namespace iService5.Core.ViewModels;

public class NonSmmConnectionViewModel : MvxViewModel<bool>
{
  private readonly IMvxNavigationService _navigationService;
  private readonly IUserSession _userSession;
  private readonly IShortTextsService _ShortTextsService = (IShortTextsService) null;
  private readonly IMetadataService _metadataService;
  private readonly IPlatformSpecificServiceLocator _locator;
  private readonly IAlertService _alertService;
  private readonly IAppliance _appliance;
  private Is5SshWrapper _sshWrapper;
  internal bool _fltapped = false;
  private CancellationTokenSource tokenSource;
  private CancellationToken token;
  private CancellationTokenSource dbustokenSource;
  private CancellationToken dbustoken;
  private int connectivityCounter;
  private bool dBusConnectionResponseReturned = true;
  internal bool isCatFeatureDisabled = false;
  internal bool isBridgeModeDisabled = false;
  internal bool shouldRestartDBus = false;
  private const int DBUS_TIMEOUT = 35;
  private string _controlUrlParameter;
  private readonly ILoggingService _loggingService;
  private string _MonitoringInfoMessage = AppResource.CONN_PAGE_MONITORING_DISABLED_MISSING_TOPOGRAM;
  private string _ControlInfoMessage = AppResource.CONN_PAGE_CONTROL_DISABLED_MISSING_FILES;
  private string _FlashingInfoMessage = AppResource.CONN_PAGE_FLASHING_DISABLED_MISSING_FILES;
  private string _RepairEnumber = nameof (RepairEnumber);
  private bool _shiftPos = false;
  private string _WifiBridgeStatus = "• " + AppResource.CONNECTED_TEXT;
  private string _ConnectedColor = "Green";
  private string _DbusStatus = "• " + AppResource.CONNECTED_TEXT;
  private string _ConnectedDbusColor = "Green";
  private string _NavText = AppResource.APPLIANCE_FLASH_BACK_TEXT;
  private string logButtonLabel = AppResource.NON_SMM_LOG_BUTTON;
  private string flashButtonLabel = AppResource.FLASH;
  private string flashButtonSubLabel = AppResource.FLASH_SUB_TEXT;
  private string monitoringButtonLabel = AppResource.MONITORING;
  private string controlButtonLabel = AppResource.CONTROL;
  private string measureButtonLabel = AppResource.MEASURE;
  private string codingButtonLabel = AppResource.CODING;
  private string _BridgeButtonLabel = AppResource.BRIDGE_SETTINGS_TEXT;
  private bool _IsBusy;
  private bool _displayPartialBridgePage = false;
  private bool _displayNativeBridgeSettings = false;
  private bool _displayDownloadLogBtn = false;
  private bool _MonitoringInfoVisibility;
  private bool _ControlInfoVisibility;
  private bool _FlashingInfoVisibility;
  private ObservableCollection<HaInfoItems> _items = new ObservableCollection<HaInfoItems>();
  private bool isLogBtnDisabled = true;
  private bool isFlashBtnDisabled = true;
  private bool isMonitoringBtnDisabled;
  private bool isControlBtnDisabled = true;
  private bool isMeasureBtnDisabled = true;
  private bool isBridgeSettingBtnDisabled = true;
  private bool isCodingBtnDisabled;
  private string _CodingInfoMessage;
  private bool _CodingInfoVisibility;
  private bool _BridgeModeWarningInfoVisibility = false;
  private bool _DbusInfoVisibility;
  private bool _CheckingDbusConnection = false;
  private string _DbusInfoMessage = AppResource.CONN_PAGE_DBUS_DISCONNECTED;
  private string _DbusButtonMessage = AppResource.CONN_PAGE_CONNECT_DBUS;
  private bool _areButtonsEnabled = true;
  private bool _canGoBack = true;
  internal bool _requestedReboot = false;
  internal CancellationTokenSource _tokenSource = (CancellationTokenSource) null;
  internal Task _DisconnectionTask = (Task) null;
  private readonly Action<object> GetApplianceInfo = (Action<object>) (obj =>
  {
    NonSmmConnectionViewModel connectionViewModel1 = (NonSmmConnectionViewModel) obj;
    if (connectionViewModel1._metadataService.getFilesForEnumber(connectionViewModel1._userSession.getEnumberSession()).Where<enumber_modules>((Func<enumber_modules, bool>) (x => !x.IsDownloaded)).ToList<enumber_modules>().Count != 0)
    {
      connectionViewModel1.IsFlashBtnDisabled = true;
      connectionViewModel1.FlashingInfoVisibility = true;
      connectionViewModel1._loggingService.getLogger().LogAppInformation(LoggingContext.USERSESSION, "Flash button was disabled. Missing Files.", memberName: nameof (GetApplianceInfo), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/NonSmmConnectionViewModel.cs", sourceLineNumber: 1355);
    }
    else
    {
      connectionViewModel1.IsFlashBtnDisabled = false;
      connectionViewModel1.FlashingInfoVisibility = false;
    }
    if (connectionViewModel1._metadataService.isSMMWithWifi(connectionViewModel1._userSession.getEnumberSession()))
    {
      connectionViewModel1._loggingService.getLogger().LogAppInformation(LoggingContext.USERSESSION, "Memory button was disabled. Appliance is SMM with WiFi.", memberName: nameof (GetApplianceInfo), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/NonSmmConnectionViewModel.cs", sourceLineNumber: 1365);
    }
    else
    {
      if (!connectionViewModel1.DbusStatus.Equals("Connected"))
        return;
      connectionViewModel1.IsLogBtnDisabled = false;
    }
  });

  public virtual async Task Initialize() => await base.Initialize();

  public override void Prepare(bool arrivedfromcatview)
  {
    this.displayPartialBridgePage = arrivedfromcatview;
    this.displayNativeBridgeSettings = arrivedfromcatview;
    this.displayDownloadLogBtn = arrivedfromcatview;
    this.IsBridgeSettingBtnDisabled = !arrivedfromcatview;
  }

  public NonSmmConnectionViewModel(
    IMvxNavigationService navigationService,
    IShortTextsService _SService,
    ILoggingService loggingService,
    IMetadataService metadataService,
    IPlatformSpecificServiceLocator locator,
    IUserSession userSession,
    IAlertService alertService,
    Is5SshWrapper sshWrapper,
    IAppliance appliance)
  {
    NonSmmConnectionViewModel connectionViewModel = this;
    this._ShortTextsService = _SService;
    this._userSession = userSession;
    this.RepairEnumber = this._userSession.getEnumberSession();
    this._metadataService = metadataService;
    this._alertService = alertService;
    this._sshWrapper = sshWrapper;
    this._appliance = appliance;
    this._locator = locator;
    this._sshWrapper.IPAddress = this._locator.GetPlatformSpecificService().GetIp();
    this.GoToHome = (ICommand) new Command(new Action(this.VisitHomePage));
    this.LogBtnTapped = (ICommand) new Command((Action) (() => connectionViewModel.GoTo_Mem_LogPage()));
    this.FlashBtnTapped = (ICommand) new Command(new Action(this.FlashButtonClicked));
    this.GoToBridgeSettings = (ICommand) new Command((Action) (async () => await connectionViewModel.GoToBridgeSettingsPage()));
    this.ConnectToDbusNowCommand = (ICommand) new Command(new Action(this.CheckDBusConnectionVMmethod));
    this.GoToDataLogger = (ICommand) new Command((Action) (async () => await connectionViewModel.VisitDataLoggerWebInterfacePage()));
    this.GoToBridgeUpgrade = (ICommand) new Command((Action) (async () => await connectionViewModel.VisitBridgeUpgrade()));
    this.GoToDownloadLog = (ICommand) new Command((Action) (async () => await connectionViewModel.VisitListOfAvailableLogPage()));
    this.MonitoringBtnTapped = (ICommand) new Command((Action) (async () =>
    {
      connectionViewModel.IsBusy = true;
      //Microsoft.AppCenter.Analytics.Analytics.TrackEvent("FUNCTION_MONITORING_CLICK_EVENT", (IDictionary<string, string>) null);
      SshResponse response = connectionViewModel._sshWrapper.StartMonitoring();
      if (response.Success)
      {
        int num = await connectionViewModel._navigationService.Navigate<MonitoringGraphicsViewModel>((IMvxBundle) null, new CancellationToken()) ? 1 : 0;
        response = (SshResponse) null;
      }
      else
      {
        connectionViewModel.IsBusy = false;
        await connectionViewModel._alertService.ShowMessageAlertWithKey("MONITORING_START_FAILED", AppResource.ALERT_TEXT);
        response = (SshResponse) null;
      }
    }));
    this.ControlBtnTapped = (ICommand) new Command((Action) (async () =>
    {
      if (FeatureConfiguration.ControlEnabled)
      {
        connectionViewModel.IsBusy = true;
        //Microsoft.AppCenter.Analytics.Analytics.TrackEvent("FUNCTION_CONTROL_CLICK_EVENT", (IDictionary<string, string>) null);
        SshResponse response = connectionViewModel._sshWrapper.StartMonitoring();
        if (response.Success)
        {
          int num = await connectionViewModel._navigationService.Navigate<ControlSegmentsWithWebViewModel>((IMvxBundle) null, new CancellationToken()) ? 1 : 0;
        }
        else
        {
          connectionViewModel.IsBusy = false;
          await connectionViewModel._alertService.ShowMessageAlertWithKey("CONTROL_START_FAILED", AppResource.ALERT_TEXT);
        }
        response = (SshResponse) null;
      }
      else
      {
        //Microsoft.AppCenter.Analytics.Analytics.TrackEvent("FUNCTION_CONTROL_CLICK_EVENT", (IDictionary<string, string>) null);
        await alertService.ShowMessageAlertWithMessage("Control Button Clicked", AppResource.WARNING_TEXT);
      }
    }));
    this.MeasureBtnTapped = (ICommand) new Command((Action) (async () =>
    {
      //Microsoft.AppCenter.Analytics.Analytics.TrackEvent("FUNCTION_MEASURE_CLICK_EVENT", (IDictionary<string, string>) null);
      int num = await connectionViewModel._navigationService.Navigate<PECheckViewModel, bool>(connectionViewModel.displayPartialBridgePage, (IMvxBundle) null, new CancellationToken()) ? 1 : 0;
    }));
    this.CodingBtnTapped = (ICommand) new Command((Action) (async () => await connectionViewModel.GoTo_Var_CodingPage()));
    this._loggingService = loggingService;
    this._navigationService = navigationService;
    this._ActiveView = false;
    this._fltapped = false;
    this.Items = new ObservableCollection<HaInfoItems>();
    this.tokenSource = new CancellationTokenSource();
    this.token = this.tokenSource.Token;
    this.dbustokenSource = new CancellationTokenSource();
    this.dbustoken = this.dbustokenSource.Token;
    this.connectivityCounter = 0;
    this.IsMonitoringBtnDisabled = true;
    this.IsCodingBtnDisabled = true;
    this.IsLogBtnDisabled = true;
    this.IsFlashBtnDisabled = true;
    this.DbusStatus = "Disconnected";
    this.isCatFeatureDisabled = !FeatureConfiguration.DisplayCATFeature;
  }

  private void GoTo_Mem_LogPage()
  {
    if (this._fltapped)
      return;
    Device.BeginInvokeOnMainThread((Action) (() =>
    {
      if (!this.AreButtonsEnabled)
        return;
      //Microsoft.AppCenter.Analytics.Analytics.TrackEvent("FUNCTION_MEMORY_LOG_CLICK_EVENT", (IDictionary<string, string>) null);
      this.AreButtonsEnabled = false;
      this._navigationService.Navigate<NonSmmErrorLogTransitionViewModel>((IMvxBundle) null, new CancellationToken());
    }));
  }

  private void FlashButtonClicked()
  {
    //Microsoft.AppCenter.Analytics.Analytics.TrackEvent("FUNCTION_FLASH_CLICK_EVENT", (IDictionary<string, string>) null);
    this._navigationService.Navigate<NonSMMFlasherListViewModel>((IMvxBundle) null, new CancellationToken());
  }

  internal async Task VisitDataLoggerWebInterfacePage()
  {
    IMvxNavigationService navigationService = this._navigationService;
    DetailNavigationArgs detailNavigationArgs = new DetailNavigationArgs();
    detailNavigationArgs.detailNavigationPageType = DetailNavigationPageType.DataLogger;
    CancellationToken cancellationToken = new CancellationToken();
    int num = await navigationService.Navigate<InAppBrowserViewModel, DetailNavigationArgs>(detailNavigationArgs, (IMvxBundle) null, cancellationToken) ? 1 : 0;
  }

  internal async Task VisitBridgeUpgrade()
  {
    int num = await this._navigationService.Navigate<BridgeUpgradeInstructionsViewModel>((IMvxBundle) null, new CancellationToken()) ? 1 : 0;
  }

  internal async Task VisitListOfAvailableLogPage()
  {
    int num = await this._navigationService.Navigate<BridgeLogViewModel>((IMvxBundle) null, new CancellationToken()) ? 1 : 0;
  }

  internal async Task GoToBridgeSettingsPage()
  {
    IMvxNavigationService navigationService = this._navigationService;
    DetailNavigationArgs detailNavigationArgs = new DetailNavigationArgs();
    detailNavigationArgs.bridgeSettingsSelected = true;
    CancellationToken cancellationToken = new CancellationToken();
    DetailReturnArgs result = await navigationService.Navigate<BridgeSettingsViewModel, DetailNavigationArgs, DetailReturnArgs>(detailNavigationArgs, (IMvxBundle) null, cancellationToken);
    if (result == null || !result.IsBridgeSettingsSaved)
    {
      result = (DetailReturnArgs) null;
    }
    else
    {
      this._sshWrapper = Mvx.IoCProvider.Resolve<Is5SshWrapper>();
      result = (DetailReturnArgs) null;
    }
  }

  private async Task GoTo_Var_CodingPage()
  {
    Thread.Sleep(1000);
    //Microsoft.AppCenter.Analytics.Analytics.TrackEvent("FUNCTION_CODING_CLICK_EVENT", (IDictionary<string, string>) null);
    int num = await this._navigationService.Navigate<VarCodingViewModel>((IMvxBundle) null, new CancellationToken()) ? 1 : 0;
  }

  public void OnBackButtonPressed() => this.VisitHomePage();

  private void VisitHomePage()
  {
    if (this._fltapped)
      return;
    this._ActiveView = false;
    if (this.RepairEnumber == "iService Bridge")
    {
      this.dbustokenSource.Cancel();
      IMvxNavigationService navigationService = this._navigationService;
      DetailNavigationArgs detailNavigationArgs = new DetailNavigationArgs();
      detailNavigationArgs.bridgeDisplay = true;
      CancellationToken cancellationToken = new CancellationToken();
      navigationService.Navigate<TabViewModel, DetailNavigationArgs>(detailNavigationArgs, (IMvxBundle) null, cancellationToken);
    }
    else
    {
      this.dbustokenSource.Cancel();
      this._navigationService.Navigate<TabViewModel>((IMvxBundle) null, new CancellationToken());
    }
    this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
  }

  public ICommand GoToHome { protected set; get; }

  public ICommand LogBtnTapped { protected set; get; }

  public ICommand FlashBtnTapped { protected set; get; }

  public ICommand MonitoringBtnTapped { protected set; get; }

  public ICommand ControlBtnTapped { protected set; get; }

  public ICommand MeasureBtnTapped { protected set; get; }

  public ICommand CodingBtnTapped { protected set; get; }

  public ICommand GoToBridgeSettings { protected set; get; }

  public ICommand GoToDataLogger { protected set; get; }

  public ICommand GoToBridgeUpgrade { protected set; get; }

  public ICommand GoToDownloadLog { protected set; get; }

  public ICommand ConnectToDbusNowCommand { protected set; get; }

  public bool TopogramFileExists { get; private set; }

  public bool ControlFileExists { get; private set; }

  public string MonitoringInfoMessage
  {
    get => this._MonitoringInfoMessage;
    internal set
    {
      this._MonitoringInfoMessage = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.MonitoringInfoMessage));
    }
  }

  public string ControlInfoMessage
  {
    get => this._ControlInfoMessage;
    internal set
    {
      this._ControlInfoMessage = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.ControlInfoMessage));
    }
  }

  public string FlashingInfoMessage
  {
    get => this._FlashingInfoMessage;
    internal set
    {
      this._FlashingInfoMessage = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.FlashingInfoMessage));
    }
  }

  public string RepairEnumber
  {
    get => this._RepairEnumber;
    internal set
    {
      this._RepairEnumber = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.RepairEnumber));
    }
  }

  public bool shiftPos
  {
    get => this._shiftPos;
    set
    {
      this._shiftPos = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.shiftPos));
    }
  }

  public string WifiBridgeStatus
  {
    get => this._WifiBridgeStatus;
    internal set
    {
      this._WifiBridgeStatus = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.WifiBridgeStatus));
    }
  }

  public string ConnectedColor
  {
    get => this._ConnectedColor;
    internal set
    {
      this._ConnectedColor = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.ConnectedColor));
    }
  }

  public string DbusStatus
  {
    get => this._DbusStatus;
    internal set
    {
      this._DbusStatus = value;
      if (this._DbusStatus.Equals("Disconnected"))
      {
        this.IsCodingBtnDisabled = true;
        this.IsMonitoringBtnDisabled = true;
        this.IsLogBtnDisabled = true;
        this.DbusInfoVisibility = true;
        this.isControlBtnDisabled = true;
      }
      else
      {
        if (!this.CodingAvailable)
        {
          this.IsCodingBtnDisabled = true;
          this.CodingInfoVisibility = true;
          this.CodingInfoMessage = AppResource.CONN_PAGE_CODING_DISABLED_NOT_AVAILABLE;
        }
        else
        {
          this.IsCodingBtnDisabled = false;
          this.CodingInfoVisibility = false;
        }
        if (this.TopogramAvailable)
        {
          if (this.TopogramPathValid)
          {
            if (this.TopogramFileExists)
            {
              if (this.ControlAvailable)
              {
                this.IsControlBtnDisabled = !this.ControlAvailable;
                this.ControlInfoVisibility = !this.ControlAvailable;
              }
              this.IsMonitoringBtnDisabled = false;
              this.MonitoringInfoVisibility = false;
            }
            else
              this.MonitorControlInfobtn(true);
          }
          else
            this.MonitorControlInfobtn(true);
        }
        else
          this.MonitorControlInfobtn(true);
        this.IsLogBtnDisabled = false;
        this.DbusInfoVisibility = false;
      }
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.DbusStatus));
    }
  }

  public void MonitorControlInfobtn(bool value)
  {
    this.IsControlBtnDisabled = value;
    this.ControlInfoVisibility = value;
    this.IsMonitoringBtnDisabled = value;
    this.MonitoringInfoVisibility = value;
  }

  public string ConnectedDbusColor
  {
    get => this._ConnectedDbusColor;
    internal set
    {
      this._ConnectedDbusColor = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.ConnectedDbusColor));
    }
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

  public string LogButtonLabel
  {
    get => this.logButtonLabel;
    internal set
    {
      this.logButtonLabel = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.LogButtonLabel));
    }
  }

  public string FlashButtonLabel
  {
    get => this.flashButtonLabel;
    internal set
    {
      this.flashButtonLabel = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.FlashButtonLabel));
    }
  }

  public string FlashButtonSubLabel
  {
    get => this.flashButtonSubLabel;
    internal set
    {
      this.flashButtonSubLabel = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.FlashButtonSubLabel));
    }
  }

  public string MonitoringButtonLabel
  {
    get => this.monitoringButtonLabel;
    internal set
    {
      this.monitoringButtonLabel = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.MonitoringButtonLabel));
    }
  }

  public string ControlButtonLabel
  {
    get => this.controlButtonLabel;
    internal set
    {
      this.controlButtonLabel = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.ControlButtonLabel));
    }
  }

  public string MeasureButtonLabel
  {
    get => this.measureButtonLabel;
    internal set
    {
      this.measureButtonLabel = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.MeasureButtonLabel));
    }
  }

  public string CodingButtonLabel
  {
    get => this.codingButtonLabel;
    internal set
    {
      this.codingButtonLabel = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.CodingButtonLabel));
    }
  }

  public string BridgeButtonLabel
  {
    get => this._BridgeButtonLabel;
    internal set
    {
      this._BridgeButtonLabel = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.BridgeButtonLabel));
    }
  }

  public bool IsBusy
  {
    get => this._IsBusy;
    set
    {
      this._IsBusy = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.IsBusy));
    }
  }

  public bool displayPartialBridgePage
  {
    get => this._displayPartialBridgePage;
    set
    {
      this._displayPartialBridgePage = value;
      if (this._displayPartialBridgePage)
        this.RepairEnumber = AppResource.BRIDGE_HEADER;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.displayPartialBridgePage));
    }
  }

  public bool displayNativeBridgeSettings
  {
    get => this._displayNativeBridgeSettings;
    set
    {
      this._displayNativeBridgeSettings = FeatureConfiguration.DisplayNativeBridgeSettings(this.displayPartialBridgePage);
      if (this._displayNativeBridgeSettings)
        this.RepairEnumber = AppResource.BRIDGE_HEADER;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.displayNativeBridgeSettings));
    }
  }

  public bool displayDownloadLogBtn
  {
    get => this._displayDownloadLogBtn;
    set
    {
      this._displayDownloadLogBtn = FeatureConfiguration.DisplayBridgeDownloadLogBtn(this.displayPartialBridgePage);
      if (this._displayDownloadLogBtn)
        this.RepairEnumber = AppResource.BRIDGE_HEADER;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.displayDownloadLogBtn));
    }
  }

  public bool MonitoringInfoVisibility
  {
    get => this._MonitoringInfoVisibility;
    set
    {
      this._MonitoringInfoVisibility = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.MonitoringInfoVisibility));
    }
  }

  public bool ControlInfoVisibility
  {
    get => this._ControlInfoVisibility;
    set
    {
      this._ControlInfoVisibility = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.ControlInfoVisibility));
    }
  }

  public bool FlashingInfoVisibility
  {
    get => this._FlashingInfoVisibility;
    set
    {
      this._FlashingInfoVisibility = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.FlashingInfoVisibility));
    }
  }

  public ObservableCollection<HaInfoItems> Items
  {
    get => this._items;
    internal set
    {
      this._items = value;
      this.RaisePropertyChanged<ObservableCollection<HaInfoItems>>((Expression<Func<ObservableCollection<HaInfoItems>>>) (() => this.Items));
    }
  }

  internal async Task SetBootModePopupDefault()
  {
    await this._alertService.ShowMessageAlertWithMessage("Boot Mode has NO VALUE yet", "Alert");
  }

  internal void ConnectivityCheck()
  {
    if (this.dBusConnectionResponseReturned)
      ++this.connectivityCounter;
    this.WifiBridgeStatus = this._appliance.StatusOfBridgeConnection;
    this.ConnectedColor = this._appliance.ConnectedColorBridge;
    if (this._appliance.boolStatusOfBridgeConnection)
    {
      if (this.connectivityCounter == 35)
      {
        this.connectivityCounter = 0;
        this.CheckDBusConnectionVMmethod();
      }
      if (this.shouldRestartDBus)
      {
        this._sshWrapper.RestartSession();
        this.shouldRestartDBus = false;
      }
      if (this._DisconnectionTask == null || this._tokenSource == null || this._DisconnectionTask.IsCompleted)
        return;
      this._tokenSource.Cancel();
    }
    else
    {
      this._tokenSource = new CancellationTokenSource();
      this._DisconnectionTask = Task.Factory.StartNew((Action) (() =>
      {
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Unknown status of DBus Connection. Please reconnet to bridge", memberName: nameof (ConnectivityCheck), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/NonSmmConnectionViewModel.cs", sourceLineNumber: 761);
        Thread.Sleep(3000);
        if (this._tokenSource.Token.IsCancellationRequested)
          return;
        this.VisitHomePage();
      }), this._tokenSource.Token);
    }
  }

  public bool IsLogBtnDisabled
  {
    internal set
    {
      this.isLogBtnDisabled = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.IsLogBtnDisabled));
    }
    get => this.isLogBtnDisabled;
  }

  public bool IsFlashBtnDisabled
  {
    internal set
    {
      this.isFlashBtnDisabled = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.IsFlashBtnDisabled));
    }
    get => this.isFlashBtnDisabled;
  }

  public bool IsMonitoringBtnDisabled
  {
    internal set
    {
      this.isMonitoringBtnDisabled = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.IsMonitoringBtnDisabled));
    }
    get => this.isMonitoringBtnDisabled;
  }

  public bool IsControlBtnDisabled
  {
    internal set
    {
      this.isControlBtnDisabled = value;
      if (!FeatureConfiguration.ControlEnabled)
        this.isControlBtnDisabled = true;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.IsControlBtnDisabled));
    }
    get => this.isControlBtnDisabled;
  }

  public bool IsMeasureBtnDisabled
  {
    internal set
    {
      this.isMeasureBtnDisabled = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.IsMeasureBtnDisabled));
    }
    get => this.isMeasureBtnDisabled;
  }

  public bool IsBridgeSettingBtnDisabled
  {
    internal set
    {
      this.isBridgeSettingBtnDisabled = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.IsBridgeSettingBtnDisabled));
    }
    get => this.isBridgeSettingBtnDisabled;
  }

  public bool IsCodingBtnDisabled
  {
    internal set
    {
      this.isCodingBtnDisabled = value;
      if (!this.DbusStatus.Equals("Connected"))
        this.isCodingBtnDisabled = true;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.IsCodingBtnDisabled));
    }
    get => this.isCodingBtnDisabled;
  }

  public string CodingInfoMessage
  {
    get => this._CodingInfoMessage;
    internal set
    {
      this._CodingInfoMessage = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.CodingInfoMessage));
    }
  }

  public bool CodingInfoVisibility
  {
    get => this._CodingInfoVisibility;
    set
    {
      this._CodingInfoVisibility = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.CodingInfoVisibility));
    }
  }

  public bool BridgeModeWarningInfoVisibility
  {
    get => this._BridgeModeWarningInfoVisibility;
    set
    {
      this._BridgeModeWarningInfoVisibility = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.BridgeModeWarningInfoVisibility));
    }
  }

  public bool DbusInfoVisibility
  {
    get => this._DbusInfoVisibility;
    set
    {
      this._DbusInfoVisibility = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.DbusInfoVisibility));
    }
  }

  public bool CheckingDbusConnection
  {
    get => this._CheckingDbusConnection;
    set
    {
      this._CheckingDbusConnection = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.CheckingDbusConnection));
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.NotCheckingDbusConnection));
    }
  }

  public bool NotCheckingDbusConnection => !this.CheckingDbusConnection;

  public string DbusInfoMessage
  {
    get => this._DbusInfoMessage;
    internal set
    {
      this._DbusInfoMessage = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.DbusInfoMessage));
    }
  }

  public string DbusButtonMessage
  {
    get => this._DbusButtonMessage;
    internal set
    {
      this._DbusButtonMessage = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.DbusButtonMessage));
    }
  }

  public bool _ActiveView { get; internal set; }

  public bool AreButtonsEnabled
  {
    get => this._areButtonsEnabled;
    set
    {
      this._areButtonsEnabled = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.AreButtonsEnabled));
    }
  }

  public bool canGoback
  {
    get => this._canGoBack;
    private set => this._canGoBack = value;
  }

  public bool CodingAvailable { get; private set; }

  public bool TopogramAvailable { get; private set; }

  public bool TopogramPathValid { get; private set; }

  public bool ControlAvailable { get; private set; }

  public bool ControlPathValid { get; private set; }

  public override void ViewAppearing()
  {
    base.ViewAppearing();
    this.CheckDBusConnectionVMmethod();
    if (!this.isBridgeModeDisabled)
      return;
    this.CheckSSHAvailability();
  }

  public override void ViewDisappearing() => base.ViewDisappearing();

  public override void ViewAppeared()
  {
    base.ViewAppeared();
    this.AreButtonsEnabled = true;
    this._appliance.CheckConnectivity(new connectivityUpdateCallback(this.ConnectivityCheck));
    if (this._ActiveView)
      return;
    this._ActiveView = true;
    this.CheckSSHAvailability();
    if (this._metadataService.IsDocumentAvailable(this._userSession.getEnumberSession(), Constants.ControlDocument))
    {
      this.ControlAvailable = true;
      this.ControlFileExists = false;
      string controlFilePath = new TopogramParser().GetControlFilePath(this.RepairEnumber);
      if (!string.IsNullOrEmpty(controlFilePath))
      {
        this.ControlPathValid = true;
        if (File.Exists(controlFilePath))
        {
          this.ControlFileExists = true;
        }
        else
        {
          this.ControlAvailable = false;
          this.ControlInfoMessage = AppResource.CONN_PAGE_CONTROL_DISABLED_MISSING_FILES;
          this.ControlInfoVisibility = true;
          this._loggingService.getLogger().LogAppInformation(LoggingContext.USERSESSION, "Control disabled. Control files is available but it was missing.", memberName: nameof (ViewAppeared), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/NonSmmConnectionViewModel.cs", sourceLineNumber: 1057);
        }
      }
      else
      {
        this.ControlAvailable = false;
        this.ControlInfoMessage = AppResource.CONN_PAGE_CONTROL_DISABLED_DOCUMENT_NOT_FOUND;
        this.ControlInfoVisibility = true;
        this._loggingService.getLogger().LogAppInformation(LoggingContext.USERSESSION, "Control disabled. Control Document not found.", memberName: nameof (ViewAppeared), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/NonSmmConnectionViewModel.cs", sourceLineNumber: 1065);
      }
    }
    else
    {
      this.ControlAvailable = false;
      this.ControlFileExists = false;
      this.ControlInfoMessage = AppResource.CONN_PAGE_CONTROL_DISABLED_DOCUMENT_NOT_AVAILABLE;
      this.ControlInfoVisibility = true;
      this._loggingService.getLogger().LogAppInformation(LoggingContext.USERSESSION, "Control disabled. Control is not avialable for this appliance.", memberName: nameof (ViewAppeared), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/NonSmmConnectionViewModel.cs", sourceLineNumber: 1074);
    }
    if (this._metadataService.IsDocumentAvailable(this._userSession.getEnumberSession(), Constants.TopogramDocument))
    {
      this.TopogramAvailable = true;
      TopogramParser topogramParser = new TopogramParser();
      string topogramFilePath = topogramParser.GetTopogramFilePath(this.RepairEnumber);
      if (!string.IsNullOrEmpty(topogramFilePath))
      {
        this.TopogramPathValid = true;
        if (File.Exists(topogramFilePath))
        {
          this.TopogramFileExists = true;
          this._controlUrlParameter = topogramParser.GetControlUrlParameter(topogramFilePath);
          if (!this.DbusStatus.Equals("Connected"))
          {
            this.IsMonitoringBtnDisabled = true;
            this.IsControlBtnDisabled = true;
          }
          else
          {
            if (this.ControlAvailable)
            {
              this.IsControlBtnDisabled = !this.ControlAvailable;
              this.ControlInfoVisibility = !this.ControlAvailable;
            }
            this.IsMonitoringBtnDisabled = false;
            this.MonitoringInfoVisibility = false;
          }
        }
        else
        {
          this.TopogramFileExists = false;
          this.MonitoringInfoMessage = AppResource.CONN_PAGE_MONITORING_DISABLED_MISSING_TOPOGRAM;
          this.MonitoringInfoVisibility = true;
          this.ControlInfoMessage = AppResource.CONN_PAGE_CONTROL_DISABLED_MISSING_MONITORING_FILES;
          this.ControlInfoVisibility = true;
          this._loggingService.getLogger().LogAppInformation(LoggingContext.USERSESSION, "Control button disabled. Topogram is available but it was missing.", memberName: nameof (ViewAppeared), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/NonSmmConnectionViewModel.cs", sourceLineNumber: 1114);
          this._loggingService.getLogger().LogAppInformation(LoggingContext.USERSESSION, "Monitoring button disabled. Topogram is available but it was missing.", memberName: nameof (ViewAppeared), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/NonSmmConnectionViewModel.cs", sourceLineNumber: 1115);
        }
      }
      else
      {
        this.TopogramPathValid = false;
        this.IsMonitoringBtnDisabled = true;
        this.MonitoringInfoMessage = AppResource.CONN_PAGE_MONITORING_DISABLED_TOPOGRAM_NOT_FOUND;
        this.MonitoringInfoVisibility = true;
        this.ControlInfoMessage = AppResource.CONN_PAGE_CONTROL_DISABLED_TOPOGRAM_NOT_FOUND;
        this.ControlInfoVisibility = true;
        this._loggingService.getLogger().LogAppInformation(LoggingContext.USERSESSION, "Control button disabled. Topogram Document not found.", memberName: nameof (ViewAppeared), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/NonSmmConnectionViewModel.cs", sourceLineNumber: 1126);
        this._loggingService.getLogger().LogAppInformation(LoggingContext.USERSESSION, "Monitoring button disabled. Topogram Document not found.", memberName: nameof (ViewAppeared), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/NonSmmConnectionViewModel.cs", sourceLineNumber: 1127);
      }
    }
    else
    {
      this.TopogramAvailable = false;
      this.MonitoringInfoMessage = AppResource.CONN_PAGE_MONITORING_DISABLED_TOPOGRAM_NOT_AVAILABLE;
      this.MonitoringInfoVisibility = true;
      this.ControlInfoMessage = AppResource.CONN_PAGE_CONTROL_DISABLED_TOPOGRAM_NOT_AVAILABLE;
      this.ControlInfoVisibility = true;
      this._loggingService.getLogger().LogAppInformation(LoggingContext.USERSESSION, "Control button disabled. Topogram is not avialable.", memberName: nameof (ViewAppeared), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/NonSmmConnectionViewModel.cs", sourceLineNumber: 1137);
      this._loggingService.getLogger().LogAppInformation(LoggingContext.USERSESSION, "Monitoring button disabled. Topogram is not available.", memberName: nameof (ViewAppeared), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/NonSmmConnectionViewModel.cs", sourceLineNumber: 1138);
    }
    this.CheckCodingBtnVisibility();
    Task.Factory.StartNew(this.GetApplianceInfo, (object) this);
  }

  private void CheckCodingBtnVisibility()
  {
    bool flag1 = false;
    bool flag2 = false;
    this.CodingAvailable = false;
    if (this._metadataService.getOldVarCodingStatus(this.RepairEnumber).Any<varcodes>())
      flag1 = true;
    bool flag3;
    if (this._metadataService.GetCodingFileAvailable(this.RepairEnumber) != null)
    {
      flag3 = true;
      flag2 = File.Exists(this._metadataService.GetCodingFilePath(this.RepairEnumber));
    }
    else
      flag3 = false;
    if (flag1 | flag2)
    {
      this.IsCodingBtnDisabled = false;
      this.CodingInfoVisibility = false;
      this.CodingAvailable = true;
    }
    else if (!flag1 && !flag2)
    {
      this.IsCodingBtnDisabled = true;
      this.CodingInfoMessage = AppResource.CONN_PAGE_CODING_DISABLED_NOT_AVAILABLE;
      this.CodingInfoVisibility = true;
      this._loggingService.getLogger().LogAppInformation(LoggingContext.USERSESSION, "Coding button disabled. Coding file missing or no varcod entry.", memberName: nameof (CheckCodingBtnVisibility), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/NonSmmConnectionViewModel.cs", sourceLineNumber: 1186);
    }
    else if (!flag3)
    {
      this.IsCodingBtnDisabled = true;
      this.CodingInfoMessage = AppResource.CONN_PAGE_CODING_DISABLED_NOT_AVAILABLE;
      this.CodingInfoVisibility = true;
      this._loggingService.getLogger().LogAppInformation(LoggingContext.USERSESSION, "Coding button disabled. Coding-file not available in DB.", memberName: nameof (CheckCodingBtnVisibility), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/NonSmmConnectionViewModel.cs", sourceLineNumber: 1193);
    }
    else if (!flag2)
    {
      this.IsCodingBtnDisabled = true;
      this.CodingInfoMessage = AppResource.CONN_PAGE_CODING_DISABLED_MISSING_FILE;
      this.CodingInfoVisibility = true;
      this._loggingService.getLogger().LogAppInformation(LoggingContext.USERSESSION, "Coding button disabled. Coding file missing.", memberName: nameof (CheckCodingBtnVisibility), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/NonSmmConnectionViewModel.cs", sourceLineNumber: 1200);
    }
    else if (!flag1)
    {
      this.IsCodingBtnDisabled = true;
      this.CodingInfoMessage = AppResource.CONN_PAGE_CODING_DISABLED_NO_VARCODE_ENTRY;
      this.CodingInfoVisibility = true;
      this._loggingService.getLogger().LogAppInformation(LoggingContext.USERSESSION, "Coding button disabled. No varcode entry.", memberName: nameof (CheckCodingBtnVisibility), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/NonSmmConnectionViewModel.cs", sourceLineNumber: 1207);
    }
    else
      this._loggingService.getLogger().LogAppInformation(LoggingContext.USERSESSION, "No matching criteria for coding btn visibilty", memberName: nameof (CheckCodingBtnVisibility), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/NonSmmConnectionViewModel.cs", sourceLineNumber: 1211);
  }

  private void CheckSSHAvailability()
  {
    if (this.displayPartialBridgePage)
      Task.Factory.StartNew<Task>((Func<Task>) (async () =>
      {
        Thread.Sleep(1000);
        string _smmip = this._locator.GetPlatformSpecificService().GetIp();
        this._sshWrapper.IPAddress = _smmip;
        if (this._sshWrapper.GetSshAvailability().Success)
        {
          this.isBridgeModeDisabled = false;
          this.SetMeasureAndBridgeSettingBtn();
          this._loggingService.getLogger().LogAppDebug(LoggingContext.BACKEND, "Successfully connected ssh  : ", memberName: nameof (CheckSSHAvailability), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/NonSmmConnectionViewModel.cs", sourceLineNumber: 1229);
          string enumber = "No";
          string timestamp = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
          string Varcodes = string.Empty;
          string sessionDetailsJson = $"{enumber}-{timestamp}-{Varcodes}";
          try
          {
            this._loggingService.getLogger().LogAppDebug(LoggingContext.BACKEND, "try to upload json", memberName: nameof (CheckSSHAvailability), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/NonSmmConnectionViewModel.cs", sourceLineNumber: 1240);
            if (this._sshWrapper.StartSession(sessionDetailsJson).Success)
              this._loggingService.getLogger().LogAppDebug(LoggingContext.BACKEND, "Successfully uploaded json for cat display : ", memberName: nameof (CheckSSHAvailability), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/NonSmmConnectionViewModel.cs", sourceLineNumber: 1244);
            else
              this._loggingService.getLogger().LogAppDebug(LoggingContext.BACKEND, "Error uploaded json for cat display : ", memberName: nameof (CheckSSHAvailability), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/NonSmmConnectionViewModel.cs", sourceLineNumber: 1247);
          }
          catch (Exception ex)
          {
            this._loggingService.getLogger().LogAppDebug(LoggingContext.BACKEND, "json upload error :  " + ex.Message, memberName: nameof (CheckSSHAvailability), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/NonSmmConnectionViewModel.cs", sourceLineNumber: 1252);
          }
          try
          {
            if (!this.canGoback)
            {
              int num = await this._navigationService.Navigate<NonSmmConnectionViewModel>((IMvxBundle) null, new CancellationToken()) ? 1 : 0;
            }
          }
          catch (Exception ex)
          {
            this._loggingService.getLogger().LogAppDebug(LoggingContext.BACKEND, "Error navigation to nonsmm view : ", ex, nameof (CheckSSHAvailability), "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/NonSmmConnectionViewModel.cs", 1263);
          }
          enumber = (string) null;
          timestamp = (string) null;
          Varcodes = (string) null;
          sessionDetailsJson = (string) null;
          _smmip = (string) null;
        }
        else
        {
          this.isBridgeModeDisabled = true;
          this.SetMeasureAndBridgeSettingBtn();
          Device.BeginInvokeOnMainThread((Action) (async () => await UtilityFunctions.ShowBridgeModeWarningPopup()));
          _smmip = (string) null;
        }
      }), this.token);
    else
      this.SetMeasureAndBridgeSettingBtn();
  }

  private void SetMeasureAndBridgeSettingBtn()
  {
    this.IsBridgeSettingBtnDisabled = this.isBridgeModeDisabled;
    this.BridgeModeWarningInfoVisibility = this.isBridgeModeDisabled;
    this.IsMeasureBtnDisabled = this.isBridgeModeDisabled || this.isCatFeatureDisabled;
  }

  private void CheckDBusConnectionVMmethod()
  {
    this.CheckingDbusConnection = true;
    Task.Run((Action) (() =>
    {
      try
      {
        this.dBusConnectionResponseReturned = false;
        SshResponse sshResponse = this._sshWrapper.CheckDBusConnection();
        BridgeResponse bridgeResponse = JsonConvert.DeserializeObject<BridgeResponse>(sshResponse.RawResponse);
        if (sshResponse.Success && bridgeResponse.dbusconnection)
        {
          this.DbusStatus = "Connected";
          this._appliance.DbusConnected = true;
          this._loggingService.getLogger().LogAppDebug(LoggingContext.APPLIANCE, "check-dbus-connection status: connected", memberName: nameof (CheckDBusConnectionVMmethod), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/NonSmmConnectionViewModel.cs", sourceLineNumber: 1306);
          this.shouldRestartDBus = false;
        }
        else if (sshResponse.Success && !bridgeResponse.dbusconnection)
        {
          this.DbusStatus = "Disconnected";
          this._appliance.DbusConnected = false;
          this._loggingService.getLogger().LogAppDebug(LoggingContext.APPLIANCE, "check-dbus-connection status: disconnected", memberName: nameof (CheckDBusConnectionVMmethod), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/NonSmmConnectionViewModel.cs", sourceLineNumber: 1313);
          this.shouldRestartDBus = true;
        }
        else
        {
          this._appliance.DbusConnected = false;
          this._loggingService.getLogger().LogAppDebug(LoggingContext.APPLIANCE, "check-dbus-connection status: unknown", memberName: nameof (CheckDBusConnectionVMmethod), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/NonSmmConnectionViewModel.cs", sourceLineNumber: 1319);
          this.shouldRestartDBus = true;
        }
      }
      catch (Exception ex)
      {
        this._loggingService.getLogger().LogAppError(LoggingContext.PROGRAMNONSMM, $"Error during GetDbusStatus: {ex.Message}. Using value from previous result.", memberName: nameof (CheckDBusConnectionVMmethod), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/NonSmmConnectionViewModel.cs", sourceLineNumber: 1325);
        this.shouldRestartDBus = true;
      }
      finally
      {
        this.dBusConnectionResponseReturned = true;
      }
    }), this.dbustoken);
    this.CheckingDbusConnection = false;
  }

  public override void ViewDisappeared()
  {
    base.ViewDisappeared();
    this.IsBusy = false;
  }
}
