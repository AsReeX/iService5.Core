// Decompiled with JetBrains decompiler
// Type: iService5.Core.ViewModels.ApplianceInstructionConnectionViewModel
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

public class ApplianceInstructionConnectionViewModel : MvxViewModel
{
  private readonly IMvxNavigationService _navigationService;
  private readonly IUserSession _userSession;
  private readonly IShortTextsService _ShortTextsService;
  private readonly IMetadataService _metadataService;
  private readonly IAppliance _appliance;
  private readonly IAlertService _alertService;
  private readonly Is5SshWrapper _sshWrapper;
  private const string UNDERLINE = "Underline";
  private const string NONE = "None";
  private readonly IPlatformSpecificServiceLocator _locator;
  private ushort _BiometricTriesRemain = 3;
  private bool _LoginFailed = false;
  private bool _EnableLoggedIn = false;
  private string _LinkStatus = "None";
  private string _RepairEnumber = nameof (RepairEnumber);
  private string _NavText = AppResource.NAVIGATE_HOME_BTN_LABEL;
  private string _WifiStatus;
  private string _ConnectedColor = "Green";
  private bool _SmmConnectShow = true;
  private bool _areButtonsEnabled = true;
  private readonly ILoggingService _loggingService;

  public virtual async Task Initialize() => await base.Initialize();

  public string _ConnectionGraphic { get; internal set; }

  public bool _viewstatus { get; set; }

  public ApplianceInstructionConnectionViewModel(
    IMvxNavigationService navigationService,
    IShortTextsService _SService,
    ILoggingService loggingService,
    Is5SshWrapper sshWrapper,
    IMetadataService metadataService,
    IPlatformSpecificServiceLocator locator,
    IUserSession userSession,
    IAlertService alertService,
    IAppliance appliance)
  {
    this._ShortTextsService = _SService;
    this._userSession = userSession;
    this._RepairEnumber = this._userSession.getEnumberSession();
    this._metadataService = metadataService;
    this._appliance = appliance;
    this._alertService = alertService;
    this._sshWrapper = sshWrapper;
    this._ConnectionGraphic = this._metadataService.getConnectionGraphic(this._RepairEnumber);
    if (this._ConnectionGraphic != null)
    {
      this.LinkStatus = "Underline";
      loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "File to use for connection instructions " + this._ConnectionGraphic, memberName: ".ctor", sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceInstructionConnectionViewModel.cs", sourceLineNumber: 57);
    }
    else
    {
      this.LinkStatus = "None";
      loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "Failed to get file to use for connection instructions", memberName: ".ctor", sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceInstructionConnectionViewModel.cs", sourceLineNumber: 62);
    }
    this.GoToHome = (ICommand) new Command(new Action(this.VisitHomePage));
    this.OpenSettings = (ICommand) new Command(new Action(this.VisitWifiSettings));
    this.RevealWifi = (ICommand) new Command(new Action(this.VisitWifiInfo));
    this.OpenGraphic = (ICommand) new Command(new Action(this.VisitGraphics));
    this.GoToSmm = (ICommand) new Command(new Action(this.VisitSmm));
    this._navigationService = navigationService;
    this._locator = locator;
    this._viewstatus = false;
    if (this._ShortTextsService != null)
      this._loggingService = loggingService;
    IPlatformSpecificService platformSpecificService = this._locator.GetPlatformSpecificService();
    if (!platformSpecificService.isFingerprintSupported() || !platformSpecificService.isDeviceSecured() || !platformSpecificService.isFingerprintAvailable() || !platformSpecificService.isFingerprintPermissionGranted())
      return;
    this.EnableLoggedIn = true;
  }

  private void VisitSmm()
  {
    Device.BeginInvokeOnMainThread((Action) (() =>
    {
      this.AreButtonsEnabled = false;
      this._navigationService.Navigate<SmmConnectionViewModel>((IMvxBundle) null, new CancellationToken());
    }));
  }

  private void VisitGraphics()
  {
    if (this._ConnectionGraphic == null)
      return;
    Device.BeginInvokeOnMainThread((Action) (() =>
    {
      if (!this.AreButtonsEnabled)
        return;
      this.AreButtonsEnabled = false;
      this._navigationService.Navigate<GraphicWiFiActivationViewModel>((IMvxBundle) null, new CancellationToken());
    }));
  }

  public void OnBackButtonPressed() => this.VisitHomePage();

  private void VisitWifiSettings() => this._locator.GetPlatformSpecificService().OpenWifiSettings();

  internal void VisitWifiInfo()
  {
    if (this.EnableLoggedIn)
      this._locator.GetPlatformSpecificService().isFingerprintValid(new iService5.Core.Services.Platform.fingerprintCompletionCallback(this.autoLoginFingerprintCompletionCallback));
    else
      Device.BeginInvokeOnMainThread((Action) (() =>
      {
        if (!this.AreButtonsEnabled)
          return;
        this.AreButtonsEnabled = false;
        this._navigationService.Navigate<ValidationViewModel, string>("SMM", (IMvxBundle) null, new CancellationToken());
      }));
  }

  public async Task autoLoginFingerprintCompletionCallback(FingerprintVerification res)
  {
    this._loggingService.getLogger().LogAppInformation(LoggingContext.LOGIN, $"Called with {res.ToString()} remaining {this.BiometricTriesRemain.ToString()} tries left", memberName: nameof (autoLoginFingerprintCompletionCallback), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceInstructionConnectionViewModel.cs", sourceLineNumber: 149);
    if (res == FingerprintVerification.SUCCESS)
    {
      int num1 = await this._navigationService.Navigate<WifiInfoViewModel, string>("SMM", (IMvxBundle) null, new CancellationToken()) ? 1 : 0;
    }
    else if (res == FingerprintVerification.IOS_FAILURE)
    {
      int num2 = await this._navigationService.Navigate<ValidationViewModel, string>("SMM", (IMvxBundle) null, new CancellationToken()) ? 1 : 0;
    }
    else
    {
      int num3 = await this._navigationService.Navigate<ValidationViewModel, string>("SMM", (IMvxBundle) null, new CancellationToken()) ? 1 : 0;
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

  public bool LoginFailed
  {
    get => this._LoginFailed;
    set
    {
      this._LoginFailed = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.LoginFailed));
    }
  }

  public void fingerprintCompletionCallback(FingerprintVerification res)
  {
    this._loggingService.getLogger().LogAppInformation(LoggingContext.LOGIN, "Called with " + res.ToString(), memberName: nameof (fingerprintCompletionCallback), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceInstructionConnectionViewModel.cs", sourceLineNumber: 201);
    if (res != FingerprintVerification.ERROR && res != FingerprintVerification.SUCCESS && res != FingerprintVerification.IOS_FAILURE)
      return;
    this._navigationService.Navigate<WifiInfoViewModel, string>("SMM", (IMvxBundle) null, new CancellationToken());
  }

  public bool EnableLoggedIn
  {
    get => this._EnableLoggedIn;
    set
    {
      this._EnableLoggedIn = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.EnableLoggedIn));
    }
  }

  private void VisitHomePage()
  {
    Device.BeginInvokeOnMainThread((Action) (() =>
    {
      if (!this.AreButtonsEnabled)
        return;
      this.AreButtonsEnabled = false;
      this._navigationService.Navigate<TabViewModel>((IMvxBundle) null, new CancellationToken());
    }));
  }

  public ICommand GoToHome { protected set; get; }

  public ICommand OpenSettings { protected set; get; }

  public ICommand RevealWifi { internal set; get; }

  public ICommand OpenGraphic { protected set; get; }

  public ICommand GoToSmm { protected set; get; }

  public string LinkStatus
  {
    get => this._LinkStatus;
    private set
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

  public string NavText
  {
    get => this._NavText;
    internal set
    {
      this._NavText = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.NavText));
    }
  }

  public string WifiStatus
  {
    get => this._WifiStatus;
    private set
    {
      this._WifiStatus = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.WifiStatus));
    }
  }

  public string ConnectedColor
  {
    get => this._ConnectedColor;
    private set
    {
      this._ConnectedColor = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.ConnectedColor));
    }
  }

  public bool SmmConnectShow
  {
    get => this._SmmConnectShow;
    private set
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
    this._appliance.CheckConnectivity(new connectivityUpdateCallback(this.updateStatus));
  }

  public override void ViewAppeared()
  {
    base.ViewAppeared();
    this.AreButtonsEnabled = true;
    this._viewstatus = true;
    this._locator.GetPlatformSpecificService().GetLocationConsent();
    this._appliance.CheckConnectivity(new connectivityUpdateCallback(this.updateStatus));
    if (!UtilityFunctions.IsLocalNetworkPermissionNeeded() || !(CoreApp.settings.GetItem("localNetworkPopupAppeared").Value == "false"))
      return;
    CoreApp.settings.UpdateItem(new Settings("localNetworkPopupAppeared", "true"));
    this._sshWrapper.IPAddress = this._locator.GetPlatformSpecificService().GetIp();
    this._sshWrapper.GetSshAvailability();
  }

  internal void updateStatus()
  {
    this.WifiStatus = this._appliance.StatusOfConnection;
    this.ConnectedColor = this._appliance.ConnectedColor;
    this.SmmConnectShow = this._appliance.boolStatusOfConnection;
  }
}
