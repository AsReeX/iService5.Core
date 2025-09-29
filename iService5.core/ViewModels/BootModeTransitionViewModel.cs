// Decompiled with JetBrains decompiler
// Type: iService5.Core.ViewModels.BootModeTransitionViewModel
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
using iService5.Core.ViewModels.SyMaNa;
using iService5.Ssh.enums;
using iService5.Ssh.models;
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

public class BootModeTransitionViewModel : MvxViewModel<DetailNavigationArgs>
{
  internal bool switchBootMode = false;
  private readonly IMvxNavigationService _navigationService;
  private readonly IUserSession _userSession;
  private readonly IShortTextsService _ShortTextsService;
  private readonly IMetadataService _metadataService;
  private readonly IPlatformSpecificServiceLocator _locator;
  private readonly ILoggingService _loggingService;
  private readonly Is5SshWrapper _sshWrapper;
  private bool directToHome = false;
  private bool SimpleBootToRecovery;
  private bool _RepairLabelVisibility = true;
  private bool _ExtraInstructions = false;
  internal bool displayActiveCircleForRecoveryTranstion = false;
  internal bool displayInActiveCircleForExtraInstructions = false;
  internal bool displayActiveCircleForExtraInstructions = false;
  internal bool displayInActiveCircleForAddionalRebooting = false;
  private bool displayActiveCircleForAddionalRebooting = false;
  internal bool displayTextDescForAddionalRebooting = false;
  internal bool displayHomeScreenText = false;
  private string _WifiInstructions = "";
  private string _WifiInstructions_hl = "";
  private string _WifiInstructions_end = "";
  private string _WifiInstructions_additionalReboot = "";
  private string _WifiInstructions_hl_additionalReboot = "";
  private string _WifiInstructions_end_additionalReboot = "";
  private string _HomeScreen_Text1 = "";
  private string _HomeScreen_Text2 = "";
  internal string checkingApplianceStatus = "";
  internal string rebootingToRecoverySystemText = "";
  private string additonalRebootingToRecoverySystemText = "";
  internal string _CheckingApplianceStatusWait = AppResource.BOOT_MODE_TRANSITION_PLEASE_WAIT;
  private CancellationTokenSource _tokenSource = (CancellationTokenSource) null;
  private Task _WaitingTask = (Task) null;
  private readonly object _lock = new object();

  public bool StartFlashing { get; internal set; }

  public virtual async Task Initialize() => await base.Initialize();

  public override void Prepare(DetailNavigationArgs parameter)
  {
    this.switchBootMode = parameter._bootModeSwitch;
    this.StartFlashing = parameter.ReturnToFlashing;
    this.SimpleBootToRecovery = parameter._simpleBootToRecovery;
    this.displayActiveCircleForRecoveryTranstion = true;
    this.DisplayInActiveCircleForExtraInstructions = true;
    this.DisplayActiveCircleForExtraInstructions = false;
    this.displayTextDescForAddionalRebooting = false;
    this.displayActiveCircleForAddionalRebooting = false;
    this.displayInActiveCircleForAddionalRebooting = false;
    if (parameter._elpRecoveryBoot.Equals(true))
    {
      this.displayHomeScreenText = false;
      this.rebootTarget = RebootTarget.FromELPToRecovery;
      if (this.SimpleBootToRecovery)
      {
        this.checkingApplianceStatus = AppResource.BOOT_MODE_TRANSITION_PLEASE_WAIT;
        this.rebootingToRecoverySystemText = AppResource.BOOT_MODE_TRANSITION_APPLIANCE_IS_REBOOTING_TO_RECOVERY;
        this.prepareWifiInstructionText();
      }
      else if (this.StartFlashing)
      {
        this.displayActiveCircleForRecoveryTranstion = true;
        this.DisplayInActiveCircleForExtraInstructions = false;
        this.DisplayActiveCircleForExtraInstructions = true;
        this.displayInActiveCircleForAddionalRebooting = true;
        this.displayTextDescForAddionalRebooting = true;
        this.checkingApplianceStatus = AppResource.BOOT_MODE_TRANSITION_PLEASE_WAIT;
        this.rebootingToRecoverySystemText = AppResource.BOOT_MODE_TRANSITION_APPLIANCE_ADDITIONAL_REBOOTING;
        this.WifiInstructions = AppResource.BOOT_MODE_TRANSITION_APPLIANCE_ADDITIONAL_REBOOTING_PART_2;
        this.WifiInstructions_additionalReboot = AppResource.BOOT_MODE_TRANSITION_APPLIANCE_IS_REBOOTING_TO_RECOVERY_EXTRA_PRT1;
        this.WifiInstructions_hl_additionalReboot = AppResource.CONN_PAGE_SETTINGS;
        this.WifiInstructions_end_additionalReboot = AppResource.BOOT_MODE_TRANSITION_APPLIANCE_IS_REBOOTING_TO_RECOVERY_EXTRA_PRT2;
      }
      else
      {
        this.checkingApplianceStatus = AppResource.BOOT_MODE_TRANSITION_PLEASE_WAIT;
        this.rebootingToRecoverySystemText = AppResource.BOOT_MODE_TRANSITION_APPLIANCE_IS_REBOOTING_TO_RECOVERY;
        this.prepareWifiInstructionText();
      }
      this.OpenSettings = (ICommand) new Command(new Action(this.VisitWifiSettings));
    }
    else if (parameter._isSyMaNaReboot.Equals(true))
    {
      this.checkingApplianceStatus = AppResource.BOOT_MODE_TRANSITION_PLEASE_WAIT;
      this.rebootingToRecoverySystemText = $"{AppResource.BOOT_MODE_TRANSITION_APPLIANCE_IS_REBOOTING}\n{AppResource.BOOT_MODE_TRANSITION_APPLIANCE_THIS_MAY_TAKE_FEW_MINUTES}";
      this.WifiInstructions = $"{AppResource.BOOT_MODE_TRANSITION_APPLIANCE_IS_REBOOTING_TO_RECOVERY_EXTRA_PRT1} {AppResource.CONN_PAGE_SETTINGS} {AppResource.CONN_PAGE_CHOOSE_BSH_NET}";
      this.displayHomeScreenText = false;
      this.rebootTarget = RebootTarget.SyMaNaReBoot;
      this.OpenSettings = (ICommand) new Command(new Action(this.VisitWifiSettings));
    }
    else
    {
      this.displayHomeScreenText = true;
      this.rebootTarget = RebootTarget.FromRecoveryToELP;
      this.checkingApplianceStatus = AppResource.BOOT_MODE_TRANSITION_PLEASE_WAIT;
      this.rebootingToRecoverySystemText = $"{AppResource.BOOT_MODE_TRANSITION_APPLIANCE_IS_REBOOTING_TO_ELP}\n{AppResource.BOOT_MODE_TRANSITION_APPLIANCE_THIS_MAY_TAKE}";
      this.WifiInstructions = AppResource.BOOT_MODE_TRANSITION_APPLIANCE_RECONNECT_AFTERWARDS;
      this.HomeScreen_Text1 = AppResource.DIRECTLY_GO_TO_HOME_SCREEN_PART1;
      this.HomeScreen_Text2 = AppResource.DIRECTLY_GO_TO_HOME_SCREEN_PART2;
      this.CheckingApplianceStatusWait = "";
    }
  }

  private void prepareWifiInstructionText()
  {
    this.WifiInstructions = AppResource.BOOT_MODE_TRANSITION_APPLIANCE_IS_REBOOTING_TO_RECOVERY_EXTRA_PRT1;
    this.WifiInstructions_hl = AppResource.CONN_PAGE_SETTINGS;
    this.WifiInstructions_end = AppResource.BOOT_MODE_TRANSITION_APPLIANCE_IS_REBOOTING_TO_RECOVERY_EXTRA_PRT2;
  }

  public ICommand OpenSettings { protected set; get; }

  public ICommand HomeScreenCommand { protected set; get; }

  private void VisitWifiSettings() => this._locator.GetPlatformSpecificService().OpenWifiSettings();

  private void NavigateToHome()
  {
    this.directToHome = true;
    Device.BeginInvokeOnMainThread((Action) (() =>
    {
      this._navigationService.Navigate<TabViewModel>((IMvxBundle) null, new CancellationToken());
      this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
    }));
  }

  public RebootTarget rebootTarget { get; set; }

  public BootModeTransitionViewModel(
    IMvxNavigationService navigationService,
    IShortTextsService _SService,
    ILoggingService loggingService,
    IMetadataService metadataService,
    IPlatformSpecificServiceLocator locator,
    IUserSession userSession,
    Is5SshWrapper sshWrapper)
  {
    this._ShortTextsService = _SService;
    this._userSession = userSession;
    this._metadataService = metadataService;
    this._navigationService = navigationService;
    this._locator = locator;
    this._loggingService = loggingService;
    this._sshWrapper = sshWrapper;
    this._Appeared = false;
    this.ExtraInstructions = false;
    this.WifiInstructions = $"{AppResource.CONN_PAGE_GOTO} {AppResource.CONN_PAGE_SETTINGS} {AppResource.CONN_PAGE_CHOOSE_BSH_NET}";
    this.HomeScreenCommand = (ICommand) new Command(new Action(this.NavigateToHome));
  }

  public bool RepairLabelVisibility
  {
    get => this._RepairLabelVisibility;
    internal set
    {
      this._RepairLabelVisibility = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.RepairLabelVisibility));
    }
  }

  public bool ExtraInstructions
  {
    get => this._ExtraInstructions;
    private set
    {
      this._ExtraInstructions = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.ExtraInstructions));
    }
  }

  public bool DisplayActiveCircleForRecoveryTranstion
  {
    get => this.displayActiveCircleForRecoveryTranstion;
    internal set
    {
      this.displayActiveCircleForRecoveryTranstion = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.DisplayActiveCircleForRecoveryTranstion));
    }
  }

  public bool DisplayInActiveCircleForExtraInstructions
  {
    get => this.displayInActiveCircleForExtraInstructions;
    internal set
    {
      this.displayInActiveCircleForExtraInstructions = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.DisplayInActiveCircleForExtraInstructions));
    }
  }

  public bool DisplayActiveCircleForExtraInstructions
  {
    get => this.displayActiveCircleForExtraInstructions;
    internal set
    {
      this.displayActiveCircleForExtraInstructions = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.DisplayActiveCircleForExtraInstructions));
    }
  }

  public bool DisplayInActiveCircleForAddionalRebooting
  {
    get => this.displayInActiveCircleForAddionalRebooting;
    internal set
    {
      this.displayInActiveCircleForAddionalRebooting = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.DisplayInActiveCircleForAddionalRebooting));
    }
  }

  public bool DisplayActiveCircleForAddionalRebooting
  {
    get => this.displayActiveCircleForAddionalRebooting;
    internal set
    {
      this.displayActiveCircleForAddionalRebooting = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.DisplayActiveCircleForAddionalRebooting));
    }
  }

  public bool DisplayTextDescForAddionalRebooting
  {
    get => this.displayTextDescForAddionalRebooting;
    internal set
    {
      this.displayTextDescForAddionalRebooting = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.DisplayTextDescForAddionalRebooting));
    }
  }

  public bool DisplayHomeScreenText
  {
    get => this.displayHomeScreenText;
    internal set
    {
      this.displayHomeScreenText = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.DisplayHomeScreenText));
    }
  }

  public string WifiInstructions
  {
    get => this._WifiInstructions;
    private set
    {
      this._WifiInstructions = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.WifiInstructions));
    }
  }

  public string WifiInstructions_hl
  {
    get => this._WifiInstructions_hl;
    private set
    {
      this._WifiInstructions_hl = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.WifiInstructions_hl));
    }
  }

  public string WifiInstructions_end
  {
    get => this._WifiInstructions_end;
    private set
    {
      this._WifiInstructions_end = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.WifiInstructions_end));
    }
  }

  public string WifiInstructions_additionalReboot
  {
    get => this._WifiInstructions_additionalReboot;
    internal set
    {
      this._WifiInstructions_additionalReboot = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.WifiInstructions_additionalReboot));
    }
  }

  public string WifiInstructions_hl_additionalReboot
  {
    get => this._WifiInstructions_hl_additionalReboot;
    internal set
    {
      this._WifiInstructions_hl_additionalReboot = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.WifiInstructions_hl_additionalReboot));
    }
  }

  public string WifiInstructions_end_additionalReboot
  {
    get => this._WifiInstructions_end_additionalReboot;
    internal set
    {
      this._WifiInstructions_end_additionalReboot = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.WifiInstructions_end_additionalReboot));
    }
  }

  public string HomeScreen_Text1
  {
    get => this._HomeScreen_Text1;
    private set
    {
      this._HomeScreen_Text1 = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.HomeScreen_Text1));
    }
  }

  public string HomeScreen_Text2
  {
    get => this._HomeScreen_Text2;
    private set
    {
      this._HomeScreen_Text2 = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.HomeScreen_Text2));
    }
  }

  public string CheckingApplianceStatus
  {
    get => this.checkingApplianceStatus;
    internal set
    {
      this.checkingApplianceStatus = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.CheckingApplianceStatus));
    }
  }

  public string RebootingToRecoverySystemText
  {
    get => this.rebootingToRecoverySystemText;
    internal set
    {
      this.rebootingToRecoverySystemText = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.RebootingToRecoverySystemText));
    }
  }

  public string AdditonalRebootingToRecoverySystemText
  {
    get => this.additonalRebootingToRecoverySystemText;
    internal set
    {
      this.additonalRebootingToRecoverySystemText = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.AdditonalRebootingToRecoverySystemText));
    }
  }

  public string CheckingApplianceStatusWait
  {
    get => this._CheckingApplianceStatusWait;
    internal set
    {
      this._CheckingApplianceStatusWait = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.CheckingApplianceStatusWait));
    }
  }

  public bool _Appeared { get; internal set; }

  public override void ViewAppeared()
  {
    base.ViewAppeared();
    this.displayHomeScreenText = false;
    if (this._Appeared)
      return;
    this._Appeared = true;
    this.ExtraInstructions = false;
    this._tokenSource = new CancellationTokenSource();
    this._WaitingTask = Task.Factory.StartNew((Action) (() =>
    {
      int result = 0;
      if (this.rebootTarget == RebootTarget.FromELPToRecovery)
        int.TryParse(BuildProperties.reconnectTimeoutToRecovery, out result);
      else if (this.rebootTarget == RebootTarget.SyMaNaReBoot)
      {
        int.TryParse(BuildProperties.reconnectTimeoutToSyMaNa, out result);
      }
      else
      {
        this.displayHomeScreenText = true;
        Thread.Sleep(2000);
        int.TryParse(BuildProperties.reconnectTimeoutToElp, out result);
        this.DisplayActiveCircleForExtraInstructions = true;
        this.DisplayInActiveCircleForExtraInstructions = false;
      }
      for (int index = 0; index < result; ++index)
      {
        Thread.Sleep(1000);
        if (this._tokenSource.Token.IsCancellationRequested)
        {
          this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Cancelled", memberName: nameof (ViewAppeared), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/BootModeTransitionViewModel.cs", sourceLineNumber: 554);
          return;
        }
        if (index >= result / 2)
        {
          this.ExtraInstructions = true;
          if (this.rebootTarget == RebootTarget.FromELPToRecovery || this.rebootTarget == RebootTarget.SyMaNaReBoot)
          {
            this.DisplayActiveCircleForRecoveryTranstion = true;
            this.DisplayInActiveCircleForExtraInstructions = false;
            this.DisplayActiveCircleForExtraInstructions = true;
            if (!this.SimpleBootToRecovery && this.StartFlashing)
            {
              this.DisplayActiveCircleForAddionalRebooting = true;
              this.DisplayInActiveCircleForAddionalRebooting = false;
            }
          }
        }
        if ((this.rebootTarget == RebootTarget.SyMaNaReBoot || this.rebootTarget == RebootTarget.FromELPToRecovery) && index % 5 == 0)
          this.ConnectivityCheck();
      }
      if (this.rebootTarget == RebootTarget.FromELPToRecovery || this.rebootTarget == RebootTarget.SyMaNaReBoot)
      {
        this.CheckingApplianceStatus = AppResource.BOOT_MODE_TRANSITION_WIFI_FAILED;
        this._CheckingApplianceStatusWait = "";
        this.RepairLabelVisibility = false;
        Thread.Sleep(2000);
      }
      if (!this.directToHome)
      {
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "directToHome is false and value of switchBootMode " + this.switchBootMode.ToString(), memberName: nameof (ViewAppeared), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/BootModeTransitionViewModel.cs", sourceLineNumber: 591);
        if (this.switchBootMode)
          this._navigationService.Navigate<ApplianceInstructionConnectionViewModel>((IMvxBundle) null, new CancellationToken());
        else
          this._navigationService.Navigate<TabViewModel>((IMvxBundle) null, new CancellationToken());
        this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
      }
      this.ExtraInstructions = false;
      this._Appeared = false;
    }));
  }

  private void NavigateToSymanaInstructionPage()
  {
    this._navigationService.Navigate<SyMaNaApplianceInstructionConnectionViewModel>((IMvxBundle) null, new CancellationToken());
  }

  internal void ConnectivityCheck()
  {
    bool lockTaken = false;
    try
    {
      Monitor.TryEnter(this._lock, 500, ref lockTaken);
      if (lockTaken)
      {
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Acquired Lock", memberName: nameof (ConnectivityCheck), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/BootModeTransitionViewModel.cs", sourceLineNumber: 625);
        try
        {
          string ssid = this._locator.GetPlatformSpecificService().GetSSID();
          if (ssid != null)
          {
            this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "SSID " + ssid, memberName: nameof (ConnectivityCheck), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/BootModeTransitionViewModel.cs", sourceLineNumber: 631);
            if (this.rebootTarget == RebootTarget.SyMaNaReBoot)
            {
              this.CheckIfIsSyMaNaWiFi(ssid);
            }
            else
            {
              if (!ssid.ToLower().StartsWith("bsh") && !ssid.ToLower().StartsWith("isb_"))
                return;
              this._sshWrapper.IPAddress = this._locator.GetPlatformSpecificService().GetIp();
              SshResponse<BootMode> bootMode = this._sshWrapper.GetBootMode();
              if (bootMode.Success)
              {
                this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Reconnection Target Boot Mode " + this.rebootTarget.ToString(), memberName: nameof (ConnectivityCheck), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/BootModeTransitionViewModel.cs", sourceLineNumber: 644);
                this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Reconnection Boot Mode " + bootMode.Response.ToString(), memberName: nameof (ConnectivityCheck), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/BootModeTransitionViewModel.cs", sourceLineNumber: 645);
                if (this._WaitingTask != null && this._tokenSource != null && !this._WaitingTask.IsCompleted)
                  this._tokenSource.Cancel();
                if (!this.switchBootMode)
                {
                  if (this.rebootTarget == RebootTarget.FromELPToRecovery)
                  {
                    if (bootMode.Response == BootMode.Recovery || bootMode.Response == BootMode.Maintenance)
                    {
                      IMvxNavigationService navigationService = this._navigationService;
                      DetailNavigationArgs detailNavigationArgs = new DetailNavigationArgs();
                      detailNavigationArgs.ReturnToFlashing = this.StartFlashing;
                      CancellationToken cancellationToken = new CancellationToken();
                      navigationService.Navigate<ApplianceFlashViewModel, DetailNavigationArgs>(detailNavigationArgs, (IMvxBundle) null, cancellationToken);
                    }
                    else
                      this._userSession.setProperty((object) $"{{\"reboot\":\"{AppResource.BOOT_MODE_SET_RECOVERY_FAILED}\"}}");
                  }
                  else if (this.rebootTarget == RebootTarget.FromRecoveryToELP)
                    this._navigationService.Navigate<ApplianceInstructionConnectionViewModel>((IMvxBundle) null, new CancellationToken());
                }
                else if (this.rebootTarget == RebootTarget.FromELPToRecovery && bootMode.Response != BootMode.Recovery && bootMode.Response != BootMode.Maintenance)
                  this._userSession.setProperty((object) $"{{\"reboot\":\"{AppResource.BOOT_MODE_SET_RECOVERY_FAILED}\"}}");
                this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
                this.ExtraInstructions = false;
                this._Appeared = false;
              }
              else
                this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Failed to get reconnection boot mode " + bootMode.ErrorMessage, memberName: nameof (ConnectivityCheck), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/BootModeTransitionViewModel.cs", sourceLineNumber: 681);
            }
          }
          else
            this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Failed to get ssid ", memberName: nameof (ConnectivityCheck), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/BootModeTransitionViewModel.cs", sourceLineNumber: 687);
        }
        catch (Exception ex)
        {
          this._loggingService.getLogger().LogAppError(LoggingContext.APPLIANCE, "Reconnnection task Failure ", ex, nameof (ConnectivityCheck), "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/BootModeTransitionViewModel.cs", 692);
        }
      }
      else
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Cannot Acquire Lock", memberName: nameof (ConnectivityCheck), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/BootModeTransitionViewModel.cs", sourceLineNumber: 696);
    }
    finally
    {
      if (lockTaken)
      {
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Released Lock", memberName: nameof (ConnectivityCheck), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/BootModeTransitionViewModel.cs", sourceLineNumber: 703);
        Monitor.Exit(this._lock);
      }
    }
  }

  private void CheckIfIsSyMaNaWiFi(string ssid)
  {
    if (!this._userSession.GetApplianceSession().isConnectedWithBSHWifi(ssid) && FeatureConfiguration.LodisTargetType() != TargetLodisType.LocalMOCK)
      return;
    this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Connected to " + ssid, memberName: nameof (CheckIfIsSyMaNaWiFi), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/BootModeTransitionViewModel.cs", sourceLineNumber: 713);
    if (FeatureConfiguration.LodisTargetType() == TargetLodisType.LocalMOCK)
      Thread.Sleep(20000);
    if (this._WaitingTask != null && this._tokenSource != null && !this._WaitingTask.IsCompleted)
      this._tokenSource.Cancel();
    this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
    this.ExtraInstructions = false;
    this._Appeared = false;
  }
}
