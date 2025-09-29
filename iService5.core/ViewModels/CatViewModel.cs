// Decompiled with JetBrains decompiler
// Type: iService5.Core.ViewModels.CatViewModel
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

public class CatViewModel : MvxViewModel
{
  private readonly IPlatformSpecificServiceLocator _locator;
  private readonly IUserSession _userSession;
  private readonly ILoggingService _loggingService;
  private readonly IMvxNavigationService _navigationService;
  private readonly IAppliance _appliance;
  private readonly IAlertService _alertService;
  private const string UNDERLINE = "Underline";
  private const string NONE = "None";
  private Is5SshWrapper _sshWrapper;
  private string _ConnectButtonLabel = AppResource.CONNECT_BTN_LABEL;
  private string _NavText = "BridgeCat";
  private string _ConnText1 = "translation";
  private string _ConnText2 = "translation";
  private string _ConnText3;
  private string _ConnText4 = "translation";
  private string _ConnText5 = "translation";
  private string _ConnText6 = "translation";
  private bool _areButtonsEnabled = true;
  private bool _EnableLoggedIn = false;
  private ushort _BiometricTriesRemain = 3;
  internal string _HeaderTitle;
  internal string _senderScreen = "";
  private string _LinkStatus = "None";
  private string _WifiStatus;
  private string _ConnectedColorBridge = "Green";
  private bool _SmmConnectShow = true;

  public bool _viewstatus { get; set; }

  public string _ConnectionGraphic { get; internal set; }

  public CatViewModel(
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
    this._locator = locator;
    this._userSession = userSession;
    this._loggingService = loggingService;
    this._navigationService = navigationService;
    this._appliance = appliance;
    this._alertService = alertService;
    this._sshWrapper = sshWrapper;
    this._ConnectionGraphic = metadataService.getConnectionGraphicNonSmm("repairenumber");
    if (this._ConnectionGraphic != null)
    {
      this.LinkStatus = "Underline";
      loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "File to use for connection instructions " + this._ConnectionGraphic, memberName: ".ctor", sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/CatViewModel.cs", sourceLineNumber: 53);
    }
    else
      this.LinkStatus = "None";
    this._senderScreen = this._userSession.GetSenderScreen();
    if (this._senderScreen.Equals(AppResource.COMPACT_APPLIANCE_TESTER_TITLE))
      this._HeaderTitle = AppResource.COMPACT_APPLIANCE_TESTER;
    if (this._senderScreen.Equals(AppResource.COMPACT_APPLIANCE_TESTER_TITLE))
      this._HeaderTitle = AppResource.COMPACT_APPLIANCE_TESTER;
    this.GoToHome = (ICommand) new Command((Action) (() => Device.BeginInvokeOnMainThread((Action) (() => this.VisitHomePage()))));
    this.OpenSettings = (ICommand) new Command(new Action(this.VisitWifiSettings));
    this.OpenGraphic = (ICommand) new Command(new Action(this.VisitGraphics));
    this.GoToNonSmm = (ICommand) new Command((Action) (() => Device.BeginInvokeOnMainThread((Action) (() => this.VisitNonSmm()))));
    this.RevealWifi = (ICommand) new Command(new Action(this.VisitWifiInfo));
    this._navigationService = navigationService;
    this._locator = locator;
    this._viewstatus = false;
    MessagingCenter.Subscribe<Application>((object) this, CoreApp.EventsNames.ForegroundEvent.ToString(), (Action<Application>) (sender => this._locator.GetPlatformSpecificService().GetLocationConsent()), (Application) null);
    if (_SService != null)
    {
      this._loggingService = loggingService;
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

  internal void VisitNonSmm()
  {
    if (!this.AreButtonsEnabled)
      return;
    if (UtilityFunctions.IsLocalNetworkPermissionNeeded() && CoreApp.settings.GetItem("localNetworkPopupAppeared").Value == "false")
    {
      this._sshWrapper.IPAddress = this._locator.GetPlatformSpecificService().GetIp();
      this._sshWrapper.GetSshAvailability();
      CoreApp.settings.UpdateItem(new Settings("localNetworkPopupAppeared", "true"));
      this.AreButtonsEnabled = true;
    }
    else
    {
      this.AreButtonsEnabled = false;
      this.HeaderTitle = AppResource.BRIDGE_HEADER;
      DateTime now = DateTime.Now;
      UtilityFunctions.setSessionIDForHistoryTable(this.HeaderTitle, now);
      string idForHistoryTable = UtilityFunctions.getSessionIDForHistoryTable();
      try
      {
        CoreApp.history.SaveItem(new History(this.HeaderTitle, now, idForHistoryTable, HistoryDBInfoType.NewSessionStarts.ToString(), "NONSMM"));
      }
      catch (Exception ex)
      {
        this._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, $"Failed to write {HistoryDBInfoType.NewSessionStarts.ToString()} in the History DB, {ex?.ToString()}", memberName: nameof (VisitNonSmm), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/CatViewModel.cs", sourceLineNumber: 122);
      }
      this.CheckBridgeMode();
    }
  }

  private async void CheckBridgeMode()
  {
    BridgeWifiResponse response = await UtilityFunctions.EnableBridgeWIFIUsingApi(this._locator, this._loggingService);
    if (response.isSuccess)
    {
      this._navigationService.Navigate<NonSmmConnectionViewModel, bool>(true, (IMvxBundle) null, new CancellationToken());
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
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Retrying to get data from home appliance.", memberName: nameof (CheckBridgeMode), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/CatViewModel.cs", sourceLineNumber: 147);
        this.CheckBridgeMode();
      }));
      errorMessage = (string) null;
      response = (BridgeWifiResponse) null;
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

  public string NavText
  {
    get => this._NavText;
    internal set
    {
      this._NavText = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.NavText));
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

  internal void VisitGraphics()
  {
    if (this._ConnectionGraphic == null)
      return;
    Device.BeginInvokeOnMainThread((Action) (() => this.VisitGraphicsImpl()));
  }

  internal void VisitGraphicsImpl()
  {
    if (!this.AreButtonsEnabled)
      return;
    this.AreButtonsEnabled = false;
    this._navigationService.Navigate<GraphicWiFiActivationViewModel>((IMvxBundle) null, new CancellationToken());
  }

  public void OnBackButtonPressed() => this.VisitHomePage();

  internal void VisitHomePage()
  {
    if (!this.AreButtonsEnabled)
      return;
    this.AreButtonsEnabled = false;
    this._navigationService.Navigate<TabViewModel>((IMvxBundle) null, new CancellationToken());
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
        this._navigationService.Navigate<ValidationViewModel, string>("BRIDGE", (IMvxBundle) null, new CancellationToken());
      }));
  }

  public async Task autoLoginFingerprintCompletionCallback(FingerprintVerification res)
  {
    this._loggingService.getLogger().LogAppInformation(LoggingContext.LOGIN, $"Called with {res.ToString()} remaining {this.BiometricTriesRemain.ToString()} tries left", memberName: nameof (autoLoginFingerprintCompletionCallback), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/CatViewModel.cs", sourceLineNumber: 365);
    if (res == FingerprintVerification.SUCCESS)
    {
      int num1 = await this._navigationService.Navigate<WifiInfoViewModel, string>("BRIDGE", (IMvxBundle) null, new CancellationToken()) ? 1 : 0;
    }
    else if (res == FingerprintVerification.IOS_FAILURE)
    {
      int num2 = await this._navigationService.Navigate<ValidationViewModel, string>("BRIDGE", (IMvxBundle) null, new CancellationToken()) ? 1 : 0;
    }
    else
    {
      int num3 = await this._navigationService.Navigate<ValidationViewModel, string>("BRIDGE", (IMvxBundle) null, new CancellationToken()) ? 1 : 0;
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

  public ICommand GoToHome { internal set; get; }

  public ICommand OpenSettings { internal set; get; }

  public ICommand OpenGraphic { internal set; get; }

  public ICommand GoToNonSmm { protected set; get; }

  public ICommand RevealWifi { internal set; get; }

  public string HeaderTitle
  {
    get => this._HeaderTitle;
    internal set
    {
      this._HeaderTitle = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.HeaderTitle));
    }
  }

  public string LinkStatus
  {
    get => this._LinkStatus;
    internal set
    {
      this._LinkStatus = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.LinkStatus));
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

  public virtual async Task Initialize() => await base.Initialize();

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

  public override void ViewAppearing() => base.ViewAppearing();

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
    if (this.WifiStatus == "• Connected")
      MessagingCenter.Send<CatViewModel>(this, CoreApp.EventsNames.BridgeConnected.ToString());
    else
      MessagingCenter.Send<CatViewModel>(this, CoreApp.EventsNames.BridgeDisconnected.ToString());
  }
}
