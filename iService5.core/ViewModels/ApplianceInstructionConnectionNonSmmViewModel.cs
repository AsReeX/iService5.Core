// Decompiled with JetBrains decompiler
// Type: iService5.Core.ViewModels.ApplianceInstructionConnectionNonSmmViewModel
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
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
//using Xamarin.Forms;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

#nullable disable
namespace iService5.Core.ViewModels;

public class ApplianceInstructionConnectionNonSmmViewModel : MvxViewModel<DetailNavigationArgs>
{
  private Is5SshWrapper _sshWrapper;
  private readonly IMvxNavigationService _navigationService;
  private readonly ILoggingService _loggingService;
  private readonly IMetadataService _metadataService;
  private readonly IAppliance _appliance;
  private readonly IAlertService _alertService;
  private const string UNDERLINE = "Underline";
  private const string NONE = "None";
  private readonly IUserSession _userSession;
  private readonly IPlatformSpecificServiceLocator _locator;
  private string DestinationScreen = string.Empty;
  internal bool BridgeSettingsMode;
  internal string mode = string.Empty;
  private bool _EnableLoggedIn = false;
  private ushort _BiometricTriesRemain = 3;
  private string _LinkStatus = "None";
  internal string _RepairEnumber;
  private string _ConnText1 = "translation";
  private string _ConnText2 = "translation";
  private string _ConnText3;
  private string _ConnText4 = "translation";
  private string _ConnText5 = "translation";
  private string _ConnText6 = "translation";
  private string _NavText = AppResource.BACK_TEXT;
  private string _ConnectButtonLabel = AppResource.CONNECT_BTN_LABEL;
  private string _WifiStatus;
  private string _ConnectedColorBridge = "Green";
  private bool _SmmConnectShow = true;
  private bool _areButtonsEnabled = true;
  private string _LastInstructionNumber;
  internal string _senderScreen = "";
  private bool _AllInstructions;

  public override void Prepare(DetailNavigationArgs args)
  {
    this.Prepare();
    this.DestinationScreen = string.IsNullOrEmpty(args.destinationScreen) ? string.Empty : args.destinationScreen;
    this.BridgeSettingsMode = args.bridgeSettingsSelected;
    this.setConnectionMode(this.BridgeSettingsMode);
    if (this.DestinationScreen == "BridgeSettingsViewModel")
    {
      this.RepairEnumber = AppResource.TRANSITION_TO_BRIDGE_SETTINGS_TITLE;
      this.LastInstructionNumber = "5. ";
    }
    else if (this._senderScreen.Equals(AppResource.COMPACT_APPLIANCE_TESTER_TITLE))
    {
      this.RepairEnumber = AppResource.COMPACT_APPLIANCE_TESTER;
    }
    else
    {
      this.RepairEnumber = this._userSession.getEnumberSession();
      this.AllInstructions = true;
      this.LastInstructionNumber = "6. ";
    }
    this._ConnectionGraphic = this._metadataService.getConnectionGraphicNonSmm(this.RepairEnumber);
    if (this._ConnectionGraphic != null)
    {
      this.LinkStatus = "Underline";
      this._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "File to use for connection instructions " + this._ConnectionGraphic, memberName: nameof (Prepare), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceInstructionConnectionNonSmmViewModel.cs", sourceLineNumber: 46);
    }
    else
      this.LinkStatus = "None";
  }

  public string _ConnectionGraphic { get; internal set; }

  public bool _viewstatus { get; set; }

  public ApplianceInstructionConnectionNonSmmViewModel(
    IMvxNavigationService navigationService,
    IShortTextsService _SService,
    ILoggingService loggingService,
    IMetadataService metadataService,
    IPlatformSpecificServiceLocator locator,
    IUserSession userSession,
    IAppliance appliance,
    IAlertService alertService,
    Is5SshWrapper sshWrapper)
  {
    this._userSession = userSession;
    this._appliance = appliance;
    this._metadataService = metadataService;
    this._loggingService = loggingService;
    this._alertService = alertService;
    this._sshWrapper = sshWrapper;
    this._senderScreen = this._userSession.GetSenderScreen();
    this.GoBackCommand = (ICommand) new Command((Action) (() => Device.BeginInvokeOnMainThread((Action) (() => this.GoBack()))));
    this.OpenSettings = (ICommand) new Command(new Action(this.VisitWifiSettings));
    this.OpenGraphic = (ICommand) new Command(new Action(this.VisitGraphics));
    this.GoToNextScreen = (ICommand) new Command(new Action(this.Nextpage));
    this.RevealWifi = (ICommand) new Command(new Action(this.VisitWifiInfo));
    this._navigationService = navigationService;
    this._locator = locator;
    this._viewstatus = false;
    MessagingCenter.Subscribe<Application>((object) this, CoreApp.EventsNames.ForegroundEvent.ToString(), (Action<Application>) (sender => this._locator.GetPlatformSpecificService().GetLocationConsent()), (Application) null);
    if (_SService != null)
    {
      this.ConnText1 = AppResource.CONN_PAGE_TEXT1;
      this.ConnText2 = AppResource.CONN_PAGE_TEXT2BRIDGE;
      this.ConnText3 = AppResource.CONN_PAGE_BRIDGE_MOBILE;
      this.ConnText4 = AppResource.CONN_PAGE_WAIT_FOR_BRIDGE_DEVICE;
      this.ConnText5 = AppResource.CONN_PAGE_WAIT_FOR_CLICK;
      this.ConnText6 = AppResource.CONN_PAGE_ENTER_WIFI_PASS_OF_BRIDGE;
    }
    IPlatformSpecificService platformSpecificService = this._locator.GetPlatformSpecificService();
    if (!platformSpecificService.isFingerprintSupported() || !platformSpecificService.isDeviceSecured() || !platformSpecificService.isFingerprintAvailable() || !platformSpecificService.isFingerprintPermissionGranted())
      return;
    this.EnableLoggedIn = true;
  }

  internal void Nextpage()
  {
    if (!this.AreButtonsEnabled)
      return;
    this.CheckSSHAvailability();
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

  private async void CheckBridgeMode()
  {
    BridgeWifiResponse response = await UtilityFunctions.EnableBridgeWIFIUsingApi(this._locator, this._loggingService);
    if (response.isSuccess)
    {
      this.AreButtonsEnabled = false;
      if (this.DestinationScreen == "BridgeSettingsViewModel")
      {
        IMvxNavigationService navigationService = this._navigationService;
        DetailNavigationArgs detailNavigationArgs = new DetailNavigationArgs();
        detailNavigationArgs.ReturnToSettingsPage = true;
        detailNavigationArgs.bridgeSettingsSelected = this.BridgeSettingsMode;
        CancellationToken cancellationToken = new CancellationToken();
        navigationService.Navigate<BridgeSettingsViewModel, DetailNavigationArgs>(detailNavigationArgs, (IMvxBundle) null, cancellationToken);
        this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
        response = (BridgeWifiResponse) null;
      }
      else if (this._senderScreen.Equals(AppResource.COMPACT_APPLIANCE_TESTER_TITLE))
      {
        this._navigationService.Navigate<ApplianceNonSMMFlashViewModel>((IMvxBundle) null, new CancellationToken());
        response = (BridgeWifiResponse) null;
      }
      else
      {
        this._navigationService.Navigate<NonSmmUploadFilesTransitionViewModel>((IMvxBundle) null, new CancellationToken());
        response = (BridgeWifiResponse) null;
      }
    }
    else
    {
      this.AreButtonsEnabled = true;
      string errorMessage = !UtilityFunctions.IsLocalNetworkPemrissionError(response.errorMessage) ? AppResource.SSH_COMMAND_ERROR : "HA_INFO_RETRY_MESSAGE_IOS_14";
      this._alertService.ShowMessageAlertWithKeyFromService(errorMessage, AppResource.INFORMATION_TEXT, AppResource.RETRY_LABEL, AppResource.CANCEL_LABEL, (Action<bool>) (shouldRetry =>
      {
        if (!shouldRetry)
          return;
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Retrying to get data from home appliance.", memberName: nameof (CheckBridgeMode), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceInstructionConnectionNonSmmViewModel.cs", sourceLineNumber: 167);
        this.CheckBridgeMode();
      }));
      errorMessage = (string) null;
      response = (BridgeWifiResponse) null;
    }
  }

  internal void VisitGraphics()
  {
    if (this._ConnectionGraphic == null)
      return;
    Device.BeginInvokeOnMainThread(new Action(this.VisitGraphicsImpl));
  }

  internal void VisitGraphicsImpl()
  {
    if (!this.AreButtonsEnabled)
      return;
    this.AreButtonsEnabled = false;
    this._navigationService.Navigate<GraphicWiFiActivationViewModel>((IMvxBundle) null, new CancellationToken());
  }

  internal async void GoBack()
  {
    if (!this.AreButtonsEnabled)
      return;
    this.AreButtonsEnabled = false;
    if (this._userSession.GetSenderScreen().Equals(AppResource.RE_DOWNLOAD_ENUMBER_VIEW))
    {
      int num1 = await this._navigationService.Navigate<TabViewModel>((IMvxBundle) null, new CancellationToken()) ? 1 : 0;
    }
    else
    {
      int num2 = await this._navigationService.Close((IMvxViewModel) this, new CancellationToken()) ? 1 : 0;
    }
  }

  private void VisitWifiSettings() => this._locator.GetPlatformSpecificService().OpenWifiSettings();

  public bool EnableLoggedIn
  {
    get => this._EnableLoggedIn;
    set
    {
      this._EnableLoggedIn = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.EnableLoggedIn));
    }
  }

  internal void VisitWifiInfo()
  {
    if (this.EnableLoggedIn)
      this._locator.GetPlatformSpecificService().isFingerprintValid(new fingerprintCompletionCallback(this.autoLoginFingerprintCompletionCallback));
    else
      Device.BeginInvokeOnMainThread((Action) (() =>
      {
        if (!this.AreButtonsEnabled)
          return;
        this.AreButtonsEnabled = false;
        this._navigationService.Navigate<ValidationViewModel, string>(this.mode, (IMvxBundle) null, new CancellationToken());
      }));
  }

  public async Task autoLoginFingerprintCompletionCallback(FingerprintVerification res)
  {
    this._loggingService.getLogger().LogAppInformation(LoggingContext.LOGIN, $"Called with {res.ToString()} remaining {this.BiometricTriesRemain.ToString()} tries left", memberName: nameof (autoLoginFingerprintCompletionCallback), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceInstructionConnectionNonSmmViewModel.cs", sourceLineNumber: 247);
    if (res == FingerprintVerification.SUCCESS)
    {
      int num1 = await this._navigationService.Navigate<WifiInfoViewModel, string>(this.mode, (IMvxBundle) null, new CancellationToken()) ? 1 : 0;
    }
    else if (res == FingerprintVerification.IOS_FAILURE)
    {
      int num2 = await this._navigationService.Navigate<ValidationViewModel, string>(this.mode, (IMvxBundle) null, new CancellationToken()) ? 1 : 0;
    }
    else
    {
      int num3 = await this._navigationService.Navigate<ValidationViewModel, string>(this.mode, (IMvxBundle) null, new CancellationToken()) ? 1 : 0;
    }
  }

  public ushort BiometricTriesRemain
  {
    get => this._BiometricTriesRemain;
    set
    {
      this._BiometricTriesRemain = value;
      this.RaisePropertyChanged<ushort>((Expression<Func<ushort>>) (() => this.BiometricTriesRemain));
    }
  }

  public ICommand GoBackCommand { internal set; get; }

  public ICommand OpenSettings { internal set; get; }

  public ICommand OpenGraphic { internal set; get; }

  public ICommand GoToNextScreen { protected set; get; }

  public ICommand RevealWifi { internal set; get; }

  public string LinkStatus
  {
    get => this._LinkStatus;
    internal set
    {
      this._LinkStatus = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.LinkStatus));
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

  public string ConnText1
  {
    get => this._ConnText1;
    internal set
    {
      this._ConnText1 = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.ConnText1));
    }
  }

  public string ConnText2
  {
    get => this._ConnText2;
    internal set
    {
      this._ConnText2 = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.ConnText2));
    }
  }

  public string ConnText3
  {
    get => this._ConnText3;
    internal set
    {
      this._ConnText3 = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.ConnText3));
    }
  }

  public string ConnText4
  {
    get => this._ConnText4;
    internal set
    {
      this._ConnText4 = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.ConnText4));
    }
  }

  public string ConnText5
  {
    get => this._ConnText5;
    internal set
    {
      this._ConnText5 = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.ConnText5));
    }
  }

  public string ConnText6
  {
    get => this._ConnText6;
    internal set
    {
      this._ConnText6 = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.ConnText6));
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

  public string ConnectButtonLabel
  {
    get => this._ConnectButtonLabel;
    internal set
    {
      this._ConnectButtonLabel = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.ConnectButtonLabel));
    }
  }

  public string WifiStatus
  {
    get => this._WifiStatus;
    internal set
    {
      this._WifiStatus = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.WifiStatus));
    }
  }

  public string ConnectedColorBridge
  {
    get => this._ConnectedColorBridge;
    internal set
    {
      this._ConnectedColorBridge = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.ConnectedColorBridge));
    }
  }

  public bool SmmConnectShow
  {
    get => this._SmmConnectShow;
    internal set
    {
      this._SmmConnectShow = !value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.SmmConnectShow));
    }
  }

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

  public string LastInstructionNumber
  {
    get => this._LastInstructionNumber;
    internal set
    {
      this._LastInstructionNumber = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.LastInstructionNumber));
    }
  }

  public string SenderScreen
  {
    get => this._senderScreen;
    internal set => this.SetProperty<string>(ref this._senderScreen, value, nameof (SenderScreen));
  }

  public bool AllInstructions
  {
    get => this._AllInstructions;
    set
    {
      this._AllInstructions = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.AllInstructions));
    }
  }

  public override void ViewDisappearing()
  {
    base.ViewDisappearing();
    this._viewstatus = false;
  }

  public override void ViewDisappeared()
  {
    base.ViewDisappeared();
    this._viewstatus = false;
  }

  public override void ViewAppearing()
  {
    base.ViewAppearing();
    this._viewstatus = true;
  }

  public override void ViewAppeared()
  {
    base.ViewAppeared();
    this.AreButtonsEnabled = true;
    this._viewstatus = true;
    this._locator.GetPlatformSpecificService().GetLocationConsent();
    this._appliance.CheckConnectivity(new connectivityUpdateCallback(this.updateStatusBr));
  }

  internal void updateStatusBr()
  {
    this.WifiStatus = this._appliance.StatusOfBridgeConnection;
    this.ConnectedColorBridge = this._appliance.ConnectedColorBridge;
    this.SmmConnectShow = this._appliance.boolStatusOfBridgeConnection;
  }

  internal void setConnectionMode(bool isBridgeMode)
  {
    if (isBridgeMode)
      this.mode = "BRIDGE";
    else
      this.mode = "NON_SMM";
  }
}
