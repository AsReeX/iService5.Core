// Decompiled with JetBrains decompiler
// Type: iService5.Core.ViewModels.SmmConnectionViewModel
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Appliance;
using iService5.Core.Services.Data;
using iService5.Core.Services.Helpers;
using iService5.Core.Services.Platform;
using iService5.Core.Services.User;
using iService5.Core.Services.User.ActivityLogging;
using iService5.Ssh.DTO;
using iService5.Ssh.models;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
//using Xamarin.Essentials;
//using Xamarin.Forms;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

#nullable disable
namespace iService5.Core.ViewModels;

public class SmmConnectionViewModel : MvxViewModel
{
  private readonly IMvxNavigationService _navigationService;
  private readonly IUserSession _userSession;
  private readonly IMetadataService _metadataService;
  private readonly IPlatformSpecificServiceLocator _locator;
  private readonly IAlertService _alertService;
  private readonly IAppliance _appliance;
  private readonly Is5SshWrapper _sshWrapper;
  internal bool _fltapped = false;
  internal bool ViewActive = false;
  private const string UnicodeString = " ";
  private bool elpRecoveryBoot = true;
  private bool switchBootMode = false;
  internal readonly Action<object> InitiateBootModeChange = (Action<object>) (obj =>
  {
    //Microsoft.AppCenter.Analytics.Analytics.TrackEvent("FUNCTION_BOOT_MODE_SWITCH_CLICK_EVENT", (IDictionary<string, string>) null);
    SmmConnectionViewModel connectionViewModel = (SmmConnectionViewModel) obj;
    if (connectionViewModel.BootMode.ToLower().Contains("elp"))
    {
      if (connectionViewModel.SetBootMode(iService5.Ssh.enums.BootMode.Maintenance))
        return;
      connectionViewModel.SetBootMode(iService5.Ssh.enums.BootMode.Recovery);
    }
    else
      connectionViewModel.SetBootMode(iService5.Ssh.enums.BootMode.Elp);
  });
  private readonly ILoggingService _loggingService;
  private string _RepairEnumber = nameof (RepairEnumber);
  private string _WifiStatus = "• " + AppResource.CONNECTED_TEXT;
  private string _ActivityIndicatorMessage;
  private bool _ActivityIndicatorVisible;
  private string _ConnectedColor = "Green";
  private string _NavText = AppResource.APPLIANCE_FLASH_BACK_TEXT;
  private string _LogArea;
  private string _BootMode;
  private string _DatabaseInventory = " ".PadRight(40);
  private string _AvailableData = " ".PadRight(40);
  private int _ListHeight = 0;
  private string _LogButtonLabel = AppResource.SMM_LOG_BUTTON;
  private string _ProgramButtonLabel = AppResource.PROGRAM_TEXT;
  private string _ModeSwitchButtonLabel = AppResource.APPLIANCE_FLASH_MODE_SWITCH_BUTTON;
  private ObservableCollection<HaInfoItems> _items = new ObservableCollection<HaInfoItems>();
  private bool _IsFlashBtnDisabled = true;
  private bool _IsLogBtnDisabled = true;
  private bool _IsModeBtnDisabled = true;
  private bool _OrangeBarVisibility = false;
  private string _OrangeBarText = "Recovery System";
  private string _FlashInfoMessage = "";
  private bool _LogInfoVisibility;
  private bool _ModeInfoVisibility;
  private bool _EnumberMismatchInfoVisibility;
  private string _LogInfoMessage = "";
  private string _ModeInfoMessage = "";
  private bool _FlashInfoVisibility;
  private bool _areButtonsEnabled = true;
  internal bool _requestedReboot = false;
  internal CancellationTokenSource _tokenSource = (CancellationTokenSource) null;
  internal Task _DisconnectionTask = (Task) null;

  public IApplianceSession Session { get; }

  public SmmConnectionViewModel(
    IMvxNavigationService navigationService,
    ILoggingService loggingService,
    IMetadataService metadataService,
    IPlatformSpecificServiceLocator locator,
    IUserSession userSession,
    IAlertService alertService,
    Is5SshWrapper sshWrapper,
    IAppliance appliance,
    IApplianceSession session)
  {
    this._userSession = userSession;
    this.RepairEnumber = this._userSession.getEnumberSession();
    this._metadataService = metadataService;
    this._alertService = alertService;
    this._sshWrapper = sshWrapper;
    this._appliance = appliance;
    this.Session = session;
    this.GoToHome = (ICommand) new Command(new Action(this.VisitHomePage));
    this.FlashTapped = (ICommand) new Command(new Action(this.GoToFlashPage));
    this.SwitchBootModeTapped = (ICommand) new Command((Action) (async () => await this.ExecuteBootModeSwitch()));
    this.LogTapped = (ICommand) new Command(new Action(this.GoToLogPage));
    this._loggingService = loggingService;
    this._navigationService = navigationService;
    this._locator = locator;
    this._fltapped = false;
    this.Items = new ObservableCollection<HaInfoItems>();
    this.switchBootMode = false;
  }

  private void FollowBootModeChange()
  {
    IMvxNavigationService navigationService = this._navigationService;
    DetailNavigationArgs detailNavigationArgs = new DetailNavigationArgs();
    detailNavigationArgs._elpRecoveryBoot = this.elpRecoveryBoot;
    detailNavigationArgs._bootModeSwitch = this.switchBootMode;
    detailNavigationArgs.ReturnToFlashing = false;
    CancellationToken cancellationToken = new CancellationToken();
    navigationService.Navigate<BootModeTransitionViewModel, DetailNavigationArgs>(detailNavigationArgs, (IMvxBundle) null, cancellationToken);
    this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
    this._fltapped = false;
    this._userSession.setProperty((object) "{\"reboot\":\"\"}");
  }

  internal void GoToLogPage()
  {
    if (this._fltapped || !this.AreButtonsEnabled)
      return;
    //Microsoft.AppCenter.Analytics.Analytics.TrackEvent("FUNCTION_ERROR_LOG_CLICK_EVENT", (IDictionary<string, string>) null);
    this.AreButtonsEnabled = false;
    this._navigationService.Navigate<ErrorLogTransitionViewModel>((IMvxBundle) null, new CancellationToken());
  }

  internal void GoToFlashPage()
  {
    if (this._fltapped)
      return;
    this._fltapped = true;
    if (!this.AreButtonsEnabled)
      return;
    this.AreButtonsEnabled = false;
    //Microsoft.AppCenter.Analytics.Analytics.TrackEvent("FUNCTION_PROGRAM_CLICK_EVENT", (IDictionary<string, string>) null);
    this._navigationService.Navigate<ApplianceFlashViewModel, DetailNavigationArgs>((DetailNavigationArgs) null, (IMvxBundle) null, new CancellationToken());
    this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
  }

  internal async Task ExecuteBootModeSwitch()
  {
    this.switchBootMode = true;
    if (this._fltapped)
      return;
    this._fltapped = true;
    if (this.BootMode != null)
    {
      bool result = false;
      string popupMessage = "";
      if (this.BootMode.ToLower().Contains("elp"))
      {
        this.elpRecoveryBoot = true;
        this.ActivityIndicatorMessage = AppResource.SMM_APPLIANCE_IS_REBOOTING_TO_RECOVERY.Replace(" (", "\n(").Replace(". ", ".\n");
        popupMessage = "FLASH_PAGE_POPUPSTRING";
      }
      else
      {
        this.elpRecoveryBoot = false;
        this.ActivityIndicatorMessage = AppResource.SMM_APPLIANCE_IS_REBOOTING_TO_ELP.Replace(" (", "\n(").Replace(". ", ".\n");
        popupMessage = "FLASH_PAGE_SWITCH_TO_ELP_MSG";
      }
      result = await this._alertService.ShowMessageAlertWithKey(popupMessage, AppResource.INFORMATION_TEXT, AppResource.WARNING_OK, AppResource.CANCEL_LABEL, (Action<bool>) (a => { }));
      if (!result)
      {
        this._fltapped = false;
      }
      else
      {
        this.ActivityIndicatorVisible = true;
        this.FlashInfoVisibility = false;
        this.LogInfoVisibility = false;
        this.LogArea = $"{this.LogArea}{AppResource.APPLIANCE_FLASH_INITIATING_BOOT_MODE_CHANGE}\n";
        Task.Factory.StartNew(this.InitiateBootModeChange, (object) this);
      }
      popupMessage = (string) null;
    }
    else
    {
      await this.SetBootModePopupDefault();
      this._fltapped = false;
    }
  }

  public void OnBackButtonPressed() => this.VisitHomePage();

  private void VisitHomePage()
  {
    if (this._fltapped)
      return;
    this._fltapped = true;
    this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
  }

  public ICommand GoToHome { protected set; get; }

  public ICommand LogTapped { protected set; get; }

  public ICommand FlashTapped { protected set; get; }

  public ICommand SwitchBootModeTapped { protected set; get; }

  public string RepairEnumber
  {
    get => this._RepairEnumber;
    internal set
    {
      this._RepairEnumber = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.RepairEnumber));
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

  public string ActivityIndicatorMessage
  {
    get => this._ActivityIndicatorMessage;
    internal set
    {
      this._ActivityIndicatorMessage = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.ActivityIndicatorMessage));
    }
  }

  public bool ActivityIndicatorVisible
  {
    get => this._ActivityIndicatorVisible;
    internal set
    {
      this._ActivityIndicatorVisible = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.ActivityIndicatorVisible));
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

  public string NavText
  {
    get => this._NavText;
    internal set
    {
      this._NavText = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.NavText));
    }
  }

  public string LogArea
  {
    get => this._LogArea;
    private set
    {
      this._LogArea = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.LogArea));
    }
  }

  public string BootMode
  {
    get => this._BootMode;
    internal set
    {
      this._BootMode = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.BootMode));
    }
  }

  public string DatabaseInventory
  {
    get => this._DatabaseInventory;
    private set
    {
      this._DatabaseInventory = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.DatabaseInventory));
    }
  }

  public string AvailableData
  {
    get => this._AvailableData;
    private set
    {
      this._AvailableData = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.AvailableData));
    }
  }

  public int ListHeight
  {
    get => this._ListHeight;
    internal set
    {
      this._ListHeight = value;
      this.RaisePropertyChanged<int>((Expression<Func<int>>) (() => this.ListHeight));
    }
  }

  public string LogButtonLabel
  {
    get => this._LogButtonLabel;
    internal set
    {
      this._LogButtonLabel = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.LogButtonLabel));
    }
  }

  public string ProgramButtonLabel
  {
    get => this._ProgramButtonLabel;
    internal set
    {
      this._ProgramButtonLabel = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.ProgramButtonLabel));
    }
  }

  public string ModeSwitchButtonLabel
  {
    get => this._ModeSwitchButtonLabel;
    internal set
    {
      this._ModeSwitchButtonLabel = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.ModeSwitchButtonLabel));
    }
  }

  public ObservableCollection<HaInfoItems> Items
  {
    get => this._items;
    private set
    {
      this._items = value;
      this.RaisePropertyChanged<ObservableCollection<HaInfoItems>>((Expression<Func<ObservableCollection<HaInfoItems>>>) (() => this.Items));
    }
  }

  private async Task SetBootModePopupDefault()
  {
    await this._alertService.ShowMessageAlertWithKey("SMM_CONN_BOOTMODE_NO_VALUE", AppResource.ALERT_TEXT);
  }

  internal void ConnectivityCheck()
  {
    this.WifiStatus = this._appliance.StatusOfConnection;
    this.ConnectedColor = this._appliance.ConnectedColor;
    if (this._appliance.boolStatusOfConnection)
    {
      if (this._DisconnectionTask == null || this._tokenSource == null || this._DisconnectionTask.IsCompleted)
        return;
      this._tokenSource.Cancel();
    }
    else if (this._requestedReboot)
    {
      this._requestedReboot = false;
      this.FollowBootModeChange();
    }
    else
    {
      this._tokenSource = new CancellationTokenSource();
      this._DisconnectionTask = Task.Factory.StartNew((Action) (() =>
      {
        Thread.Sleep(3000);
        if (this._tokenSource.Token.IsCancellationRequested)
          return;
        this.VisitHomePage();
      }), this._tokenSource.Token);
    }
  }

  internal bool SetSecureOption()
  {
    try
    {
      this._sshWrapper.IPAddress = this._locator.GetPlatformSpecificService().GetIp();
      (bool Success, string Error) tuple = this.DeviceSecured();
      if (!tuple.Success)
      {
        this.LogArea = $"{this.LogArea}{AppResource.SMM_CONN_DEVICE_IS_NOT_SECURED} ({tuple.Error.Trim()})\n";
        SshResponse sshResponse = this._sshWrapper.SecureDevice();
        if (!sshResponse.Success)
        {
          this.LogArea = $"{this.LogArea}{AppResource.SMM_CONN_DEVICE_COULD_NOT_SECURED} ({sshResponse.ErrorOut.Trim()})\n";
          return false;
        }
        this.LogArea = $"{this.LogArea}{AppResource.SMM_CONN_DEVICE_HAS_BEEN_SECURED} ({sshResponse.ErrorOut.Trim()})\n";
        return true;
      }
      this.LogArea = $"{this.LogArea}{AppResource.SMM_CONN_DEVICE_IS_SECURED} ({tuple.Error.Trim()})\n";
      return true;
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppDebug(LoggingContext.APPLIANCE, "failed to connect", ex, nameof (SetSecureOption), "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SmmConnectionViewModel.cs", 503);
      return false;
    }
  }

  public (bool Success, string Error) DeviceSecured()
  {
    if (this.BootMode.ToLower().Contains("elp"))
      return (true, "");
    SshResponse sshResponse = this._sshWrapper.IsDeviceSecure();
    return (sshResponse.Success, sshResponse.ErrorOut);
  }

  internal bool SetBootMode(iService5.Ssh.enums.BootMode mode)
  {
    try
    {
      this._sshWrapper.IPAddress = this._locator.GetPlatformSpecificService().GetIp();
      if (this.DeviceSecured().Success)
      {
        this.LogArea = AppResource.APPLIANCE_FLASH_CHANNGING_BOOT_MODE + "\n";
        if (this._sshWrapper.SetBootMode(mode).Success)
        {
          this.LogArea = $"{this.LogArea}{AppResource.APPLIANCE_FLASH_BOOT_MODE_SET}{UtilityFunctions.GetBootModeResponseText(mode)}\n{AppResource.SMM_CONN_DEVICE_REQUESTING_REBOOT}\n";
          if (this._sshWrapper.Reboot().Success)
          {
            this.LogArea = $"{this.LogArea}{AppResource.SMM_CONN_DEVICE_REQUESTED_REBOOT} - {AppResource.SMM_CONN_DEVICE_WAITING}...\n";
            this._requestedReboot = true;
            return true;
          }
          this.LogArea = $"{this.LogArea}{AppResource.SMM_CONN_DEVICE_REBOOT_FAILED}\n";
          return false;
        }
        this.LogArea = $"{this.LogArea}{AppResource.APPLIANCE_FLASH_FAILED_TO_SET_BOOTMODE}\n";
        return false;
      }
      this.LogArea = $"{this.LogArea}{AppResource.SMM_CONN_DEVICE_CANNOT_PROCEED}\n";
      return false;
    }
    catch (Exception ex)
    {
      if (mode == iService5.Ssh.enums.BootMode.Elp)
        this._sshWrapper.IPAddress = "127.0.0.1";
      this._loggingService.getLogger().LogAppDebug(LoggingContext.APPLIANCE, "failed to connect", ex, nameof (SetBootMode), "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SmmConnectionViewModel.cs", 561);
      return false;
    }
  }

  public bool IsFlashBtnDisabled
  {
    protected set
    {
      this._IsFlashBtnDisabled = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.IsFlashBtnDisabled));
    }
    get => this._IsFlashBtnDisabled;
  }

  public bool IsLogBtnDisabled
  {
    internal set
    {
      this._IsLogBtnDisabled = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.IsLogBtnDisabled));
    }
    get => this._IsLogBtnDisabled;
  }

  public bool IsModeBtnDisabled
  {
    protected set
    {
      this._IsModeBtnDisabled = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.IsModeBtnDisabled));
    }
    get => this._IsModeBtnDisabled;
  }

  public bool OrangeBarVisibility
  {
    get => this._OrangeBarVisibility;
    internal set
    {
      if (!this.IsLogBtnDisabled & value && (this.BootMode.ToLowerInvariant().Contains("recovery") || this.BootMode.ToLowerInvariant().Contains("maintenance")))
      {
        this.IsLogBtnDisabled = true;
        this.LogInfoVisibility = true;
        this.LogInfoMessage = AppResource.CONN_PAGE_LOGGING_DISABLED_IN_RECOVERY;
        this._loggingService.getLogger().LogAppInformation(LoggingContext.USERSESSION, "Error log not available in recovery system", memberName: nameof (OrangeBarVisibility), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SmmConnectionViewModel.cs", sourceLineNumber: 628);
      }
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

  public string FlashInfoMessage
  {
    get => this._FlashInfoMessage;
    internal set
    {
      this._FlashInfoMessage = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.FlashInfoMessage));
    }
  }

  public bool LogInfoVisibility
  {
    get => this._LogInfoVisibility;
    private set
    {
      this._LogInfoVisibility = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.LogInfoVisibility));
    }
  }

  public bool ModeInfoVisibility
  {
    get => this._ModeInfoVisibility;
    private set
    {
      this._ModeInfoVisibility = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.ModeInfoVisibility));
    }
  }

  public bool EnumberMismatchInfoVisibility
  {
    get => this._EnumberMismatchInfoVisibility;
    private set
    {
      this._EnumberMismatchInfoVisibility = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.EnumberMismatchInfoVisibility));
    }
  }

  public string LogInfoMessage
  {
    get => this._LogInfoMessage;
    internal set
    {
      this._LogInfoMessage = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.LogInfoMessage));
    }
  }

  public string ModeInfoMessage
  {
    get => this._ModeInfoMessage;
    internal set
    {
      this._ModeInfoMessage = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.ModeInfoMessage));
    }
  }

  public bool FlashInfoVisibility
  {
    get => this._FlashInfoVisibility;
    private set
    {
      this._FlashInfoVisibility = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.FlashInfoVisibility));
    }
  }

  public bool AreButtonsEnabled
  {
    get => this._areButtonsEnabled;
    set
    {
      this._areButtonsEnabled = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.AreButtonsEnabled));
    }
  }

  public override void ViewAppearing()
  {
    base.ViewAppearing();
    this.AreButtonsEnabled = true;
  }

  public override void ViewAppeared()
  {
    base.ViewAppeared();
    this.ViewActive = true;
    this._appliance.CheckConnectivity(new connectivityUpdateCallback(this.ConnectivityCheck));
    this.Session.Reset();
    this.Session.InitialiseMetadata();
    try
    {
      this.Session.ConnectToAppliance();
      this.DatabaseInventory = new string(' ', 50);
      this.IsFlashBtnDisabled = true;
      this.FlashInfoVisibility = false;
      this.IsLogBtnDisabled = true;
      this.LogInfoVisibility = false;
      this.IsModeBtnDisabled = true;
      this.ModeInfoVisibility = false;
      this.EnumberMismatchInfoVisibility = false;
      this.AvailableData = AppResource.AVAILABLE_DATA_TEXT;
      this.GetApplianceData();
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppError(LoggingContext.APPLIANCE, ex.Message, ex, nameof (ViewAppeared), "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SmmConnectionViewModel.cs", 802);
    }
  }

  public override void ViewDisappearing()
  {
    base.ViewDisappearing();
    this.ViewActive = false;
  }

  internal void GetApplianceData()
  {
    this.Session.RetrieveBootMode((RequestCallback) (bootModeSuccess =>
    {
      this.Session.RetrieveHAInfo((RequestCallback) (haSuccess =>
      {
        if (this.Session.HaInfo == null)
        {
          if (this.ViewActive)
          {
            if (UtilityFunctions.IsLocalNetworkPermissionNeeded() && CoreApp.settings.GetItem("localNetworkPopupAppeared").Value == "true")
              this._alertService.ShowMessageAlertWithKeyFromService("HA_INFO_RETRY_MESSAGE_IOS_14", AppResource.INFORMATION_TEXT, AppResource.RETRY_LABEL, AppResource.CANCEL_LABEL, (Action<bool>) (shouldRetry => this.InfoRetryCallback(shouldRetry)));
            else if (DeviceInfo.Platform == DevicePlatform.Android && Connectivity.ConnectionProfiles.Contains<ConnectionProfile>(ConnectionProfile.Cellular))
              this._alertService.ShowMessageAlertWithKeyFromService("HA_INFO_RETRY_MESSAGE_MOBILE_DATA_ENABLED", AppResource.INFORMATION_TEXT, AppResource.RETRY_LABEL, AppResource.CANCEL_LABEL, (Action<bool>) (shouldRetry => this.InfoRetryCallback(shouldRetry)));
            else
              this._alertService.ShowMessageAlertWithKeyFromService("HA_INFO_RETRY_MESSAGE", AppResource.INFORMATION_TEXT, AppResource.RETRY_LABEL, AppResource.CANCEL_LABEL, (Action<bool>) (shouldRetry => this.InfoRetryCallback(shouldRetry)));
          }
        }
        else
          this._loggingService.getLogger().LogAppInformation(LoggingContext.LOCAL, "Location service enabled Status: " + this._locator.GetPlatformSpecificService().IsLocationServiceEnabled().ToString(), memberName: nameof (GetApplianceData), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SmmConnectionViewModel.cs", sourceLineNumber: 847);
        this.GetApplianceInfo();
        if (haSuccess)
          return;
        this._loggingService.getLogger().LogAppInformation(LoggingContext.USER, "Could not get ha-info", memberName: nameof (GetApplianceData), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SmmConnectionViewModel.cs", sourceLineNumber: 852);
        this._loggingService.getLogger().LogAppInformation(LoggingContext.USER, "Could not contact Appliance", memberName: nameof (GetApplianceData), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SmmConnectionViewModel.cs", sourceLineNumber: 853);
        this._loggingService.getLogger().LogAppInformation(LoggingContext.LOCAL, "Location service enabled Status: " + this._locator.GetPlatformSpecificService().IsLocationServiceEnabled().ToString(), memberName: nameof (GetApplianceData), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SmmConnectionViewModel.cs", sourceLineNumber: 854);
        this.IsFlashBtnDisabled = true;
        this.FlashInfoVisibility = true;
        this.FlashInfoMessage = AppResource.CONN_PAGE_FLASHING_DISABLED_SMM_CONN_DEVICE_NOT_COMPLETE;
        this._loggingService.getLogger().LogAppInformation(LoggingContext.USERSESSION, "Program button was disabled. Communication with appliance is not complete.", memberName: nameof (GetApplianceData), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SmmConnectionViewModel.cs", sourceLineNumber: 858);
        this.IsLogBtnDisabled = true;
        this.LogInfoVisibility = true;
        this.LogInfoMessage = AppResource.CONN_PAGE_LOGGING_DISABLED_SMM_CONN_DEVICE_NOT_COMPLETE;
        this._loggingService.getLogger().LogAppInformation(LoggingContext.USERSESSION, "Logging button was disabled. Communication with appliance is not complete.", memberName: nameof (GetApplianceData), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SmmConnectionViewModel.cs", sourceLineNumber: 862);
        this.IsModeBtnDisabled = true;
        this.ModeInfoVisibility = true;
        this.ModeInfoMessage = AppResource.CONN_PAGE_MODE_CHANGE_DISABLED_SMM_CONN_DEVICE_NOT_COMPLETE;
        this._loggingService.getLogger().LogAppInformation(LoggingContext.USERSESSION, "Mode switch button was disabled. Communication with appliance is not complete.", memberName: nameof (GetApplianceData), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SmmConnectionViewModel.cs", sourceLineNumber: 866);
        this.LogArea = AppResource.SMM_CONN_DEVICE_NOT_COMPLETE + "\n";
      }));
      if (bootModeSuccess)
        return;
      this._loggingService.getLogger().LogAppError(LoggingContext.USER, "Could not get boot mode", memberName: nameof (GetApplianceData), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SmmConnectionViewModel.cs", sourceLineNumber: 872);
      this._loggingService.getLogger().LogAppInformation(LoggingContext.LOCAL, "Location service enabled Status: " + this._locator.GetPlatformSpecificService().IsLocationServiceEnabled().ToString(), memberName: nameof (GetApplianceData), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SmmConnectionViewModel.cs", sourceLineNumber: 873);
      this.IsFlashBtnDisabled = true;
      this.FlashInfoVisibility = true;
      this.FlashInfoMessage = AppResource.CONN_PAGE_FLASHING_DISABLED_SMM_CONN_DEVICE_FAILED_TO_COMMUNICATE;
      this._loggingService.getLogger().LogAppInformation(LoggingContext.USERSESSION, "Program button was disabled. Failed to communicate with Appliance.", memberName: nameof (GetApplianceData), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SmmConnectionViewModel.cs", sourceLineNumber: 877);
      this.IsModeBtnDisabled = true;
      this.ModeInfoVisibility = true;
      this.ModeInfoMessage = AppResource.CONN_PAGE_MODE_CHANGE_DISABLED_SMM_CONN_DEVICE_FAILED_TO_COMMUNICATE;
      this._loggingService.getLogger().LogAppInformation(LoggingContext.USERSESSION, "Mode switch button was disabled. Failed to communicate with Appliance.", memberName: nameof (GetApplianceData), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SmmConnectionViewModel.cs", sourceLineNumber: 881);
      this.LogArea = AppResource.SMM_CONN_DEVICE_FAILED_TO_COMMUNICATE + "\n";
    }));
  }

  internal void InfoRetryCallback(bool shouldRetry)
  {
    if (shouldRetry)
    {
      this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Retrying to get data from home appliance.", memberName: nameof (InfoRetryCallback), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SmmConnectionViewModel.cs", sourceLineNumber: 891);
      this.GetApplianceData();
    }
    else
      this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
  }

  internal void RetrieveInventory()
  {
    if (this.OrangeBarVisibility)
    {
      if (!this.Session.LocalInventoryAvailable)
        this.Session.RetrieveInventory((RequestCallback) (localInventorySuccess =>
        {
          if (localInventorySuccess && !this.Session.CachedInventoryMerged)
            this.Session.RetrieveInventory((RequestCallback) (cachedInventorySuccess => this.SetLogUIAndProperties()));
          else
            this.SetLogUIAndProperties();
        }));
      else
        this.SetLogUIAndProperties();
    }
    else if (!this.Session.InventoryAvailable)
      this.Session.RetrieveInventory((RequestCallback) (inventorySuccess => this.SetLogUIAndProperties()));
    else
      this.SetLogUIAndProperties();
  }

  internal void GetApplianceInfo()
  {
    try
    {
      this.LogArea = this._userSession.getProperty("reboot") == null ? "" : this._userSession.getProperty("reboot") + "\n";
      this._userSession.setProperty((object) "{\"reboot\":\"\"}");
      this.BootMode = this.Session.Bootmode.ToString();
      this._loggingService.getLogger().LogAppInformation(LoggingContext.USER, "Boot mode " + this.BootMode, memberName: nameof (GetApplianceInfo), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SmmConnectionViewModel.cs", sourceLineNumber: 950);
      this.OrangeBarVisibility = this.BootMode != null && (this.BootMode.ToLowerInvariant().Contains("recovery") || this.BootMode.ToLowerInvariant().Contains("maintenance"));
      this.RetrieveInventory();
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppError(LoggingContext.USER, "Failed to connect.", ex, nameof (GetApplianceInfo), "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SmmConnectionViewModel.cs", 959);
    }
  }

  internal void SetLogUIAndProperties()
  {
    List<AugmentedModule> moduleList = this.Session.ModuleList;
    this.DatabaseInventory = "";
    int num = 1;
    StringBuilder stringBuilder1 = new StringBuilder();
    foreach (AugmentedModule augmentedModule in moduleList)
    {
      stringBuilder1.Append($"Module {num++.ToString()} ");
      stringBuilder1.Append(augmentedModule.moduleid);
      stringBuilder1.Append(".");
      stringBuilder1.Append(augmentedModule.version);
      stringBuilder1.Append("\n");
    }
    this.DatabaseInventory = stringBuilder1.ToString();
    string modeResponseText = UtilityFunctions.GetBootModeResponseText(this.Session.Bootmode);
    HaInfoDto haInfo = this.Session.HaInfo;
    string eNumber = $"{haInfo.Vib}/{haInfo.CustomerIndex}";
    this._loggingService.getLogger().LogAppInformation(LoggingContext.USER, "HA info " + haInfo.ToString(), memberName: nameof (SetLogUIAndProperties), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SmmConnectionViewModel.cs", sourceLineNumber: 981);
    ObservableCollection<HaInfoItems> FWversion = new ObservableCollection<HaInfoItems>();
    StringBuilder stringBuilder2 = UtilityFunctions.SetStringBuilderForHistoryEntry(haInfo, FWversion, modeResponseText);
    this.LogArea += "\nSystemMaster Info:\n";
    bool sshCommandsStatus = UtilityFunctions.getSShCommandsStatus(this._sshWrapper.GetSSHCommands(), "get-system-information");
    this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, $"get-system-information command availablity:{sshCommandsStatus}", memberName: nameof (SetLogUIAndProperties), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SmmConnectionViewModel.cs", sourceLineNumber: 988);
    if (sshCommandsStatus)
    {
      SshResponse<SystemInfoDto> systemInfo = this._sshWrapper.GetSystemInfo();
      if (systemInfo.Success)
      {
        this.IsRecModuleVersionMatchWithSystemInfo(systemInfo.Response);
        FWversion.Add(new HaInfoItems()
        {
          Name = systemInfo.Response.system,
          Data = systemInfo.Response.version
        });
        stringBuilder2.Append($"{systemInfo.Response.system}: {systemInfo.Response.version}");
        this.logSystemInformation(systemInfo.Response);
      }
      else
        this.LogArea = $"{this.LogArea}Failed: {systemInfo.ErrorMessage}";
    }
    else
    {
      this.LogArea += "MTS: \n";
      this.LogArea += "TID: \n";
    }
    string idForHistoryTable = UtilityFunctions.getSessionIDForHistoryTable();
    try
    {
      CoreApp.history.SaveItem(new History(eNumber, DateTime.Now, idForHistoryTable, HistoryDBInfoType.HAInfo.ToString(), stringBuilder2.ToString()));
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppError(LoggingContext.APPLIANCE, "Failed to save HA Info data in the History DB, " + ex?.ToString(), memberName: nameof (SetLogUIAndProperties), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SmmConnectionViewModel.cs", sourceLineNumber: 1018);
    }
    this.Items = FWversion;
    this.ListHeight = (this.Items.Count + 1) * 30;
    SshResponse<ProductionInfoDto> productionInfo = this._sshWrapper.GetProductionInfo();
    this.LogArea += "\nSystemMaster Production Info:\n";
    if (productionInfo.Success)
    {
      this.LogArea = $"{this.LogArea}Manufacturing TimeStamp: {productionInfo.Response.manufacturingTimeStamp}\n";
      this.LogArea = $"{this.LogArea}Tracing ID: {productionInfo.Response.tracingId}\n";
      this._userSession.setTracingID(productionInfo.Response.tracingId);
    }
    else
    {
      this.LogArea += "MTS: \n";
      this.LogArea += "TID: \n";
    }
    this.SetSecureOption();
    this.ActivityIndicatorMessage = AppResource.CONN_PAGE_INITIALISING_UI;
    this.ActivityIndicatorVisible = true;
    if (!this._metadataService.isSMMWithWifi(this.RepairEnumber))
    {
      this.ActivityIndicatorVisible = false;
      this.IsFlashBtnDisabled = true;
      this.FlashInfoVisibility = true;
      this.FlashInfoMessage = AppResource.CONN_PAGE_FLASHING_DISABLED_NOTSMM;
      this.LogArea = AppResource.CONN_PAGE_NOTSMM;
      this.IsLogBtnDisabled = true;
      this.LogInfoVisibility = true;
      this.LogInfoMessage = AppResource.CONN_PAGE_LOGGING_DISABLED_CONN_PAGE_NOTSMM;
      this._loggingService.getLogger().LogAppInformation(LoggingContext.USERSESSION, "Program and Logging buttons were disabled. This is not a SystemMaster appliance with WiFi.", memberName: nameof (SetLogUIAndProperties), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SmmConnectionViewModel.cs", sourceLineNumber: 1051);
    }
    else
    {
      Dictionary<DeviceClass, List<MaterialStatistics>> materialList = new Dictionary<DeviceClass, List<MaterialStatistics>>()
      {
        {
          DeviceClass.SMM,
          new List<MaterialStatistics>()
          {
            new MaterialStatistics(this.RepairEnumber, 0, 0L, (string) null, (string) null, "FALSE")
          }
        }
      };
      bool flag = haInfo.DeviceType.ToLower() == "appliance" && haInfo.Brand.ToLower() == "homeconnect" && haInfo.Vib == "DummyVIB";
      Tuple<string, string> vibAndKi = UtilityFunctions.getVibAndKi(this.RepairEnumber);
      if (!flag && vibAndKi.Item1.ToUpper() != haInfo.Vib.ToUpper())
      {
        this.ActivityIndicatorVisible = false;
        this.EnumberMismatchInfoVisibility = true;
        this.IsFlashBtnDisabled = true;
        this._loggingService.getLogger().LogAppInformation(LoggingContext.USERSESSION, "Program button was disabled. VIB mismatch.", memberName: nameof (SetLogUIAndProperties), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SmmConnectionViewModel.cs", sourceLineNumber: 1066);
        this.LogArea = AppResource.ENUMBER_MISMATCH_PROGRAMMING_DISABLED;
      }
      else if (this._metadataService.getModulesDeltaSetForDownloadSetting(materialList).Count > 0)
      {
        this.ActivityIndicatorVisible = false;
        this.IsFlashBtnDisabled = true;
        this.FlashInfoVisibility = true;
        this.FlashInfoMessage = AppResource.CONN_PAGE_FLASHING_DISABLED_MISSINGBINARY;
        this._loggingService.getLogger().LogAppInformation(LoggingContext.USERSESSION, "Program button was disabled. Files are missing.", memberName: nameof (SetLogUIAndProperties), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SmmConnectionViewModel.cs", sourceLineNumber: 1075);
        this.LogArea = AppResource.CONN_PAGE_MISSINGBINARY;
      }
      else if (this.AreNeededPpfsMissing(moduleList))
      {
        this.ActivityIndicatorVisible = false;
        this.IsFlashBtnDisabled = true;
        this.FlashInfoVisibility = true;
        this.FlashInfoMessage = AppResource.CONN_PAGE_FLASHING_DISABLED_PPF_NOT_AVAILABLE;
        this.LogArea = AppResource.CONN_PAGE_MISSING_PPF_FILES;
      }
      else
      {
        this.ActivityIndicatorVisible = false;
        this.IsFlashBtnDisabled = false;
        this.FlashInfoVisibility = false;
      }
      this.IsLogBtnDisabled = false;
      this.LogInfoVisibility = false;
    }
    this.IsModeBtnDisabled = false;
    this.ModeInfoVisibility = false;
  }

  private void IsRecModuleVersionMatchWithSystemInfo(SystemInfoDto systemInfo)
  {
    AugmentedModule augmentedModule = this.Session.ModuleList.Find((Predicate<AugmentedModule>) (m => m.Recovery));
    this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, $"Active Recovery Module Id:{augmentedModule.moduleid} and Installed Version:{augmentedModule.InstalledVersion}", memberName: nameof (IsRecModuleVersionMatchWithSystemInfo), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SmmConnectionViewModel.cs", sourceLineNumber: 1102);
    this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, $"SystemInfo:\nBootMode:{systemInfo.bootmode} \nSystem:{systemInfo.system} \nVersion:{systemInfo.version}", memberName: nameof (IsRecModuleVersionMatchWithSystemInfo), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SmmConnectionViewModel.cs", sourceLineNumber: 1103);
    if ((systemInfo.system == "recovery" || systemInfo.system == "maintenance") && augmentedModule.version != systemInfo.version)
    {
      this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Recovery Module and Get system info verison Mis-matched", memberName: nameof (IsRecModuleVersionMatchWithSystemInfo), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SmmConnectionViewModel.cs", sourceLineNumber: 1106);
      augmentedModule.RecoveryMisMatchSystemVersion = systemInfo.version;
      this.Session.CheckIfPPF6Supported();
    }
    else
      augmentedModule.RecoveryMisMatchSystemVersion = string.Empty;
  }

  internal bool AreNeededPpfsMissing(List<AugmentedModule> modulesAvailable)
  {
    List<AugmentedModule> list = modulesAvailable.Where<AugmentedModule>((Func<AugmentedModule, bool>) (x => x.Available != NullVersion.Instance)).ToList<AugmentedModule>();
    List<iService5.Core.Services.Data.Version> availableVersionList = this._metadataService.GetVersionFromFeatureTable(SMMFeatures.PPF6.ToString());
    bool ppf6supported = list.Exists((Predicate<AugmentedModule>) (p => p.Recovery && UtilityFunctions.ISAvailableWithInRange(p.Installed, availableVersionList)));
    List<ppf> enumbersDownloadedPpfs = this._metadataService.GetAllEnumbersDownloadedPpfs(this.RepairEnumber);
    bool flag = false;
    foreach (AugmentedModule augmentedModule in list)
    {
      AugmentedModule module = augmentedModule;
      if (!enumbersDownloadedPpfs.Any<ppf>((Func<ppf, bool>) (ppf => (ppf6supported ? (ppf.type == 6L ? 1 : 0) : (ppf.type == 5L ? 1 : 0)) != 0 && ppf.moduleid == module.moduleid && ppf.version == module.version)))
      {
        this._loggingService.getLogger().LogAppInformation(LoggingContext.USERSESSION, $"Program button was disabled. PPF6 {(ppf6supported ? (object) "supported" : (object) "not supported")} but ppf files of type {(ValueType) (char) (ppf6supported ? 54 : 53)} are missing.", memberName: nameof (AreNeededPpfsMissing), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SmmConnectionViewModel.cs", sourceLineNumber: 1128);
        flag = true;
        break;
      }
    }
    if (flag)
      this.LogAnyPpfMissingInTheModuleList(list, enumbersDownloadedPpfs);
    return flag;
  }

  private void LogAnyPpfMissingInTheModuleList(List<AugmentedModule> validModules, List<ppf> ppfs)
  {
    List<bool> boolList = new List<bool>();
    foreach (AugmentedModule validModule in validModules)
    {
      AugmentedModule module = validModule;
      List<ppf> all = ppfs.FindAll((Predicate<ppf>) (ppf => ppf.moduleid == module.moduleid && ppf.version == module.version));
      bool flag1 = false;
      bool flag2 = false;
      foreach (ppf ppf in all)
      {
        if (ppf.type == 5L)
          flag1 = true;
        else if (ppf.type == 6L)
          flag2 = true;
      }
      if (!(flag1 & flag2))
      {
        string str = flag1 || flag2 ? (!flag1 || flag2 ? "Having Ppf 6 & Ppf 5 is unavailable!" : "Having Ppf 5 & Ppf 6 is unavailable!") : "Both Ppf 5 & Ppf 6 are unavailable!";
        this._loggingService.getLogger().LogAppError(LoggingContext.METADATA, $"{this._RepairEnumber} ModuleId={module.moduleid.ToString()} Version={module.version} {str}", memberName: nameof (LogAnyPpfMissingInTheModuleList), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SmmConnectionViewModel.cs", sourceLineNumber: 1176);
      }
    }
  }

  private void logSystemInformation(SystemInfoDto dto)
  {
    this.LogArea = $"{this.LogArea}System: {dto.system}\n";
    this.LogArea = $"{this.LogArea}Version: {dto.version}\n";
    this.LogArea = $"{this.LogArea}Bootmode: {dto.bootmode}\n";
    this.LogArea = $"{this.LogArea}CA: {dto.ca?.ToString()}\n";
  }
}
