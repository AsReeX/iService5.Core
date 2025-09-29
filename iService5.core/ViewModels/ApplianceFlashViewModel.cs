// Decompiled with JetBrains decompiler
// Type: iService5.Core.ViewModels.ApplianceFlashViewModel
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
using iService5.Ssh.models;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
//using Xamarin.Forms;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

#nullable disable
namespace iService5.Core.ViewModels;

public class ApplianceFlashViewModel : MvxViewModel<DetailNavigationArgs>
{
  private readonly IMvxNavigationService _navigationService;
  private readonly IUserSession _userSession;
  private readonly IMetadataService _metadataService;
  private readonly Is5SshWrapper _sshWrapper;
  private readonly IAlertService _alertService;
  private readonly IPlatformSpecificServiceLocator _locator;
  private readonly IAppliance _appliance;
  private readonly IApplianceSession Session;
  private readonly ILoggingService _loggingService;
  internal bool tapped = false;
  internal bool isDisconnected = false;
  private bool isFlashingAfterReboot = false;
  private bool finishBtnTapped = false;
  private bool simpleBootToRecovery;
  internal string BootMode = "";
  internal int _Step;
  private bool isBackButtonVisible;
  private bool isScrollDisabled;
  internal int StepTotal = 10;
  internal readonly Action<object> InitiateBootModeChange = (Action<object>) (obj =>
  {
    //Microsoft.AppCenter.Analytics.Analytics.TrackEvent("FUNCTION_BOOT_MODE_SWITCH_CLICK_EVENT", (IDictionary<string, string>) null);
    ApplianceFlashViewModel applianceFlashViewModel = (ApplianceFlashViewModel) obj;
    if (applianceFlashViewModel.BootMode.ToLower().Contains("elp") || applianceFlashViewModel.BootMode.ToLower().Contains("normal"))
    {
      if (applianceFlashViewModel.SetBootMode(iService5.Ssh.enums.BootMode.Maintenance))
        return;
      applianceFlashViewModel.SetBootMode(iService5.Ssh.enums.BootMode.Recovery);
    }
    else
      applianceFlashViewModel.SetBootMode(iService5.Ssh.enums.BootMode.Elp);
  });
  private bool _RepairLabelVisibility;
  private string _RepairEnumber;
  private string _FlashingHeader;
  private string _BulletHeader;
  private string _StatusOfConnection;
  private string _ProgramButtonLabel;
  private string finishButtonLabel;
  private double _ProgressBarValue;
  private string _StepProgressForLabel;
  private string _ProgressBarValueText;
  private string _ProgramLogLabel = AppResource.APPLIANCE_FLASH_LOG;
  internal string _ConnectedColor;
  private string _FWversion;
  private string _Failures;
  private string _ProgramProgressLabel = "";
  private string programProgressDoneLabel;
  private bool displayProgramProgressDoneLabel;
  private bool displayFinishButton;
  private bool displayProgramErrorLabel = false;
  private string _CancelBackText;
  private bool _ProgressVisibility;
  private bool _OrangeBarVisibility;
  private bool _InstallRepairMessageVisibility;
  private string _InstallRepairMessage;
  private string _OrangeBarText;
  private List<AugmentedModule> UIModuleList;
  private List<AugmentedModule> fwnonameList;
  internal bool _requestedModeChangeReboot = false;
  private string _WifiStatus;
  private bool _IsFlashBtnEnabled;
  private bool isFinishBtnEnabled;
  private bool isProgrammingInProgress;
  private bool displayActivityIndicator = false;
  private int callsCounter = 0;

  public virtual async Task Initialize() => await base.Initialize();

  public override void Prepare(DetailNavigationArgs parameter)
  {
    this.ReturnAfterReboot = false;
    this.isFlashingAfterReboot = false;
    if (parameter == null || !parameter.ReturnToFlashing)
      return;
    this.isFlashingAfterReboot = true;
  }

  public ApplianceFlashViewModel(
    IMvxNavigationService navigationService,
    IShortTextsService _SService,
    ILoggingService loggingService,
    IMetadataService metadataService,
    IPlatformSpecificServiceLocator locator,
    IUserSession userSession,
    Is5SshWrapper sshWrapper,
    IAlertService alertService,
    IAppliance appliance,
    IApplianceSession session)
  {
    this._userSession = userSession;
    this.RepairEnumber = userSession.getEnumberSession();
    this._metadataService = metadataService;
    this._sshWrapper = sshWrapper;
    this._alertService = alertService;
    this._appliance = appliance;
    this._locator = locator;
    this.Session = session;
    this._sshWrapper.IPAddress = this._locator.GetPlatformSpecificService().GetIp();
    this.NavigatePreviousPage = (ICommand) new Command(new Action(this.VisitPage));
    this.DismissPopup = (ICommand) new Command(new Action<object>(this.TogglePopupVisibility));
    this.Finish_Tapped = (ICommand) new Command((Action) (() =>
    {
      this.finishBtnTapped = true;
      this.VisitPage();
    }));
    this._navigationService = navigationService;
    if (_SService != null)
      this._loggingService = loggingService;
    this.Flash_Tapped = (ICommand) new Command((Action) (() => this.OnSubmit(this)));
    this._ActiveView = false;
    this._requestedReboot = false;
    this.Failures = AppResource.APPLIANCE_FLASH_LOG_INTRO + "\n";
    this.IsFinishBtnEnabled = true;
    this.Step = 0;
    this.IsBackButtonVisible = true;
    this.IsScrollDisabled = true;
    this.RepairLabelVisibility = true;
    this.ProgramButtonLabel = AppResource.APPLIANCE_FLASH_START_BUTTON;
    this.FinishButtonLabel = AppResource.APPLIANCE_FLASH_FINISH_BUTTON;
    this.ConnectedColor = "Green";
    this.ProgramProgressLabel = "";
    this.ProgramProgressDoneLabel = AppResource.APPLIANCE_FLASH_PROGRAMMING_DONE;
    this.CancelBackText = AppResource.APPLIANCE_FLASH_BACK_TEXT;
    this.OrangeBarText = AppResource.APPLIANCE_BOOT_MODE_RECOVERY;
    this.IsFlashBtnEnabled = false;
    this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSMM, "Ready for programming. Press Start button.", memberName: ".ctor", sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceFlashViewModel.cs", sourceLineNumber: 104);
    this.DisplayActivityIndicator = false;
    this.InstallRepairMessageVisibility = false;
  }

  private async void TogglePopupVisibility(object obj)
  {
    await Task.Run((Action) (() =>
    {
      this.InstallRepairMessageVisibility = false;
      MessagingCenter.Send<ApplianceFlashViewModel>(this, CoreApp.EventsNames.InstallRepairPopupDismissed.ToString());
    }));
  }

  public ICommand NavigatePreviousPage { protected set; get; }

  public ICommand Flash_Tapped { internal set; get; }

  public ICommand Finish_Tapped { internal set; get; }

  public ICommand DismissPopup { internal set; get; }

  public void OnBackButtonPressed()
  {
    if (!this.IsBackButtonVisible)
      return;
    this.finishBtnTapped = false;
    this.VisitPage();
  }

  internal void VisitPage()
  {
    this.IsFinishBtnEnabled = false;
    if (this.tapped)
      return;
    this.tapped = true;
    if (this.finishBtnTapped && !string.IsNullOrEmpty(this.BootMode) && (this.BootMode.ToLowerInvariant().Contains("recovery") || this.BootMode.ToLowerInvariant().Contains("maintenance")))
    {
      this.finishBtnTapped = false;
      if (!this.ReturnAfterReboot)
      {
        this.Failures = $"{this.Failures}{AppResource.APPLIANCE_FLASH_INITIATING_BOOT_MODE_CHANGE}\n";
        this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSMM, "Initiating Boot Mode Change", memberName: nameof (VisitPage), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceFlashViewModel.cs", sourceLineNumber: 152);
        Task.Factory.StartNew<Task>((Func<Task>) (async () =>
        {
          if (!this._requestedReboot && this._appliance.boolStatusOfConnection)
          {
            this.SetElpBootMode();
            this.Failures = $"{this.Failures}{AppResource.APPLIANCE_FLASH_WAITING}\n";
            this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSMM, "Waiting ...", memberName: nameof (VisitPage), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceFlashViewModel.cs", sourceLineNumber: 159);
            Thread.Sleep(3000);
          }
          this._ActiveView = false;
          TaskScheduler scheduler = TaskScheduler.Default;
          if (this._requestedReboot)
          {
            int timeout = 15000;
            Task timeoutTask = new Task((Action) (() =>
            {
              while (this._appliance.boolStatusOfConnection)
                Thread.Sleep(500);
            }));
            timeoutTask.Start();
            Task task = await Task.WhenAny(timeoutTask, Task.Delay(timeout));
            if (task == timeoutTask)
            {
              task = (Task) null;
              timeoutTask.Dispose();
              IMvxNavigationService navigationService = this._navigationService;
              DetailNavigationArgs detailNavigationArgs = new DetailNavigationArgs();
              detailNavigationArgs._elpRecoveryBoot = false;
              detailNavigationArgs.ReturnToFlashing = false;
              CancellationToken cancellationToken = new CancellationToken();
              int num1 = await navigationService.Navigate<BootModeTransitionViewModel, DetailNavigationArgs>(detailNavigationArgs, (IMvxBundle) null, cancellationToken) ? 1 : 0;
              int num2 = await this._navigationService.Close((IMvxViewModel) this, new CancellationToken()) ? 1 : 0;
            }
            else
              this._loggingService.getLogger().LogAppWarning(LoggingContext.PROGRAMSMM, "Disconnection from SMM AP was not detected within the 5sec timeout", memberName: nameof (VisitPage), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceFlashViewModel.cs", sourceLineNumber: 185);
            timeoutTask = (Task) null;
          }
          else
          {
            int num3 = await this._navigationService.Navigate<TabViewModel>((IMvxBundle) null, new CancellationToken()) ? 1 : 0;
            int num4 = await this._navigationService.Close((IMvxViewModel) this, new CancellationToken()) ? 1 : 0;
          }
          this._requestedReboot = false;
          scheduler = (TaskScheduler) null;
        }));
      }
      else if (this.Session.AllowReboot)
        this.Session.Reboot((RequestCallback) (rebootResponse =>
        {
          IMvxNavigationService navigationService = this._navigationService;
          DetailNavigationArgs detailNavigationArgs = new DetailNavigationArgs();
          detailNavigationArgs._elpRecoveryBoot = true;
          detailNavigationArgs.ReturnToFlashing = true;
          CancellationToken cancellationToken = new CancellationToken();
          navigationService.Navigate<BootModeTransitionViewModel, DetailNavigationArgs>(detailNavigationArgs, (IMvxBundle) null, cancellationToken);
          this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
        }));
      else
        this.OnSubmit(this);
    }
    else
    {
      this._navigationService.Navigate<SmmConnectionViewModel>((IMvxBundle) null, new CancellationToken());
      this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
    }
    this.tapped = false;
  }

  public int Step
  {
    get => this._Step;
    set
    {
      this._Step = value;
      this.ProgressBarValue = (double) this._Step / (double) this.StepTotal;
    }
  }

  public bool IsBackButtonVisible
  {
    get => this.isBackButtonVisible;
    internal set
    {
      this.isBackButtonVisible = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.IsBackButtonVisible));
    }
  }

  public bool IsScrollDisabled
  {
    get => this.isScrollDisabled;
    internal set
    {
      this.isScrollDisabled = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.IsScrollDisabled));
    }
  }

  internal void clearInstalledList() => throw new NotImplementedException();

  internal void OnSubmit(ApplianceFlashViewModel _this)
  {
    if (this.tapped)
      return;
    this.tapped = true;
    if (this.BootMode.ToLowerInvariant().Contains("recovery") || this.BootMode.ToLowerInvariant().Contains("maintenance"))
    {
      this.Session.checkDbus2ECUUpdateAvailability();
      this.simpleBootToRecovery = false;
      this.IsBackButtonVisible = false;
      this.IsProgrammingInProgress = true;
      this.IsScrollDisabled = true;
      this.ReturnAfterReboot = false;
      this.Session.ClearInstalled();
      this.StepTotal = 0;
      this.Step = 0;
      this.ModuleList = (List<AugmentedModule>) null;
      _this.ProgressBarValueText = "";
      _this.ProgramProgressLabel = AppResource.APPLIANCE_FLASH_PROGRESS;
      this.DisplayProgramProgressDoneLabel = false;
      this.DisplayProgramErrorLabel = false;
      this.DisplayFinishButton = false;
      this.ProgressVisibility = true;
      this.InstallRepairMessageVisibility = false;
      this.DisplayProgramErrorLabel = false;
      this.createExtractPpfsFolder();
      if (!this.isFlashingAfterReboot)
      {
        try
        {
          this.logHistoryData(AppResource.INSTALLED_FIRMWARE_BEFORE_PROGRAMMING, this.UIModuleList, HistoryDBInfoType.ProgramLogBefore);
        }
        catch (Exception ex)
        {
          this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSMM, "Failed to write before programming data in the History DB, " + ex?.ToString(), memberName: nameof (OnSubmit), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceFlashViewModel.cs", sourceLineNumber: 299);
        }
      }
      this.isFlashingAfterReboot = false;
      this.Session.UpdateApplianceFirmware((UIUpdateCallback) (() => this.ModuleList = (List<AugmentedModule>) null), (MessageCallback) (text =>
      {
        if (text.Equals("installRepairPopup off"))
        {
          _this.InstallRepairMessageVisibility = false;
          text = AppResource.APPLIANCE_INSTALL_REPAIR_POPUP_CLOSING + "\n";
        }
        _this.Failures += text;
      }), (iService5.Core.Services.Appliance.ProgressCallback) ((progress, step, total) =>
      {
        this.StepTotal = total;
        this.Step = step;
        _this.ProgressVisibility = true;
        if (progress.Equals(AppResource.APPLIANCE_INSTALL_REPAIR_REPSONSE_WAIT))
        {
          _this.ProgressVisibility = false;
          _this.DisplayActivityIndicator = true;
          this.ProgressBarValueText = "";
        }
        if (progress.Equals(AppResource.APPLIANCE_INSTALL_REPAIR_REPSONSE_ARRIVED))
        {
          _this.ProgressVisibility = false;
          _this.InstallRepairMessageVisibility = true;
          if (step == 1)
          {
            this.ProgressBarValueText = "";
            _this.InstallRepairMessage = AppResource.APPLIANCE_ECU_SUCCESS;
          }
          else
            _this.InstallRepairMessage = AppResource.APPLIANCE_ECU_FAIL;
        }
        if (progress.Contains("SPAU"))
          return;
        this.ProgressBarValueText = $"{string.Format(AppResource.APPLIANCE_FLASH_STEP, (object) this.Step, (object) this.StepTotal)} {progress}";
      }), (FinishCallback) ((withReboot, isError, errorMessage) =>
      {
        if (errorMessage.Equals("InventoryUpToDate"))
        {
          _this.InventoryUpToDate = true;
          _this.ProgressVisibility = false;
        }
        if (!withReboot)
        {
          if (isError)
          {
            this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSMM, errorMessage, memberName: nameof (OnSubmit), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceFlashViewModel.cs", sourceLineNumber: 357);
            this.DisplayProgramErrorLabel = true;
            this.DisplayProgramProgressDoneLabel = false;
            ApplianceFlashViewModel applianceFlashViewModel = _this;
            applianceFlashViewModel.Failures = $"{applianceFlashViewModel.Failures}{errorMessage}\n";
          }
          else
            this.Session.RetrieveInventory((RequestCallback) (InventorySuccess =>
            {
              if (InventorySuccess)
              {
                this.UpdateFinished = true;
                this.ModuleList = new List<AugmentedModule>();
                this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSMM, "Retrieved Inventory from Session:", memberName: nameof (OnSubmit), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceFlashViewModel.cs", sourceLineNumber: 370);
                foreach (AugmentedModule module in this.ModuleList)
                  this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSMM, $"ID: {module.moduleid}, Installed Version: {module.InstalledVersion}, Available Version: {module.Available.ToString()}, Type: {module.type}", memberName: nameof (OnSubmit), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceFlashViewModel.cs", sourceLineNumber: 373);
              }
              try
              {
                this.logHistoryData(AppResource.INSTALLED_FIRMWARE_AFTER_PROGRAMMING, this.ModuleList, HistoryDBInfoType.ProgramLogAfter);
              }
              catch (Exception ex)
              {
                this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSMM, "Failed to write after programming data in the History DB, " + ex?.ToString(), memberName: nameof (OnSubmit), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceFlashViewModel.cs", sourceLineNumber: 383);
              }
              this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSMM, "Finished", memberName: nameof (OnSubmit), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceFlashViewModel.cs", sourceLineNumber: 385);
              if (this.InventoryUpToDate)
              {
                this.DisplayProgramErrorLabel = false;
                this.DisplayProgramProgressDoneLabel = false;
              }
              else
                this.DisplayProgramProgressDoneLabel = true;
              _this.Failures += "Finished\n";
            }));
          _this.ProgressBarValueText = AppResource.APPLIANCE_FLASH_PROCESS_FINISHED;
          _this.ProgramProgressLabel = AppResource.APPLIANCE_FLASH_FINISH_INSTRUCTION;
          this.IsScrollDisabled = false;
          this.IsFinishBtnEnabled = true;
          this.DisplayFinishButton = true;
          _this.ProgramButtonLabel = AppResource.APPLIANCE_FLASH_RESTART_PROGRAMMING;
          this.IsProgrammingInProgress = false;
          _this.ProgressVisibility = false;
          this.tapped = false;
          this.ClearTemporaryFolders();
        }
        else
        {
          _this.ProgressBarValueText = AppResource.APPLIANCE_FLASH_PROCESS_FINISHED;
          this.IsScrollDisabled = false;
          _this.ProgramProgressLabel = "";
          _this.ProgressVisibility = false;
          _this.Failures += "Finished\n";
          this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSMM, "Finished", memberName: nameof (OnSubmit), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceFlashViewModel.cs", sourceLineNumber: 414);
          this.ReturnAfterReboot = true;
          foreach (AugmentedModule module in this.ModuleList)
            this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSMM, $"Not with reboot-ID: {module.moduleid}, Installed Version: {module.InstalledVersion}, Available Version: {module.Available.ToString()}, Type: {module.type}", memberName: nameof (OnSubmit), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceFlashViewModel.cs", sourceLineNumber: 417);
          this.tapped = false;
          this.IsProgrammingInProgress = false;
          this.finishBtnTapped = true;
          this.VisitPage();
        }
      }));
    }
    else if (this.BootMode.ToLower().Contains("elp") || this.BootMode.ToLower().Contains("normal"))
    {
      this._alertService.ShowMessageAlertWithKey("FLASH_PAGE_POPUPSTRING", AppResource.INFORMATION_TEXT, AppResource.WARNING_OK, AppResource.CANCEL_LABEL, (Action<bool>) (async ok =>
      {
        if (ok)
        {
          this.simpleBootToRecovery = true;
          await Task.Factory.StartNew(this.InitiateBootModeChange, (object) this);
        }
        else
          this.tapped = false;
      }));
    }
    else
    {
      this._alertService.ShowMessageAlertWithKey("APPLIANCE_FLASH_FAILED_TO_GET_BOOTMODE", AppResource.ERROR_TITLE);
      this.finishBtnTapped = false;
      this.tapped = false;
      this.VisitPage();
    }
  }

  private void ClearTemporaryFolders()
  {
    foreach (string folder in new List<string>()
    {
      "binarySession",
      "extractedPpfs"
    })
    {
      string pathOf = UtilityFunctions.getPathOf(this._locator, folder);
      if (Directory.Exists(pathOf))
        Directory.Delete(pathOf, true);
    }
    this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSMM, "Cleared Temporary Folders", memberName: nameof (ClearTemporaryFolders), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceFlashViewModel.cs", sourceLineNumber: 463);
  }

  internal bool SetBootMode(iService5.Ssh.enums.BootMode mode)
  {
    try
    {
      if (!this.DeviceSecured().Success || !this._sshWrapper.SetBootMode(mode).Success || !this._sshWrapper.Reboot().Success)
        return false;
      this._requestedModeChangeReboot = true;
      return true;
    }
    catch (Exception ex)
    {
      if (mode == iService5.Ssh.enums.BootMode.Elp)
        this._sshWrapper.IPAddress = "127.0.0.1";
      this._loggingService.getLogger().LogAppDebug(LoggingContext.APPLIANCE, "failed to connect", ex, nameof (SetBootMode), "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceFlashViewModel.cs", 514);
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

  private void logHistoryData(
    string programText,
    List<AugmentedModule> moduleList,
    HistoryDBInfoType infoType)
  {
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.AppendLine(programText);
    int num = 0;
    foreach (AugmentedModule module in moduleList)
    {
      ++num;
      if (string.IsNullOrEmpty(module.UIFirmwareName))
        stringBuilder.Append("SystemMaster ");
      else
        stringBuilder.Append(module.UIFirmwareName + " ");
      stringBuilder.Append($"({module.moduleid}) {module.type}: ");
      if (num == moduleList.Count)
        stringBuilder.Append(module.UIInstalled);
      else
        stringBuilder.AppendLine(module.UIInstalled);
    }
    try
    {
      CoreApp.history.SaveItem(new History(this.RepairEnumber, DateTime.Now, UtilityFunctions.getSessionIDForHistoryTable(), infoType.ToString(), stringBuilder.ToString()));
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSMM, "Failed to save item to History DB. Exception: " + ex?.ToString(), memberName: nameof (logHistoryData), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceFlashViewModel.cs", sourceLineNumber: 551);
    }
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

  public string RepairEnumber
  {
    get => this._RepairEnumber;
    internal set
    {
      this._RepairEnumber = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.RepairEnumber));
    }
  }

  public string FlashingHeader
  {
    get => this._FlashingHeader;
    internal set
    {
      this._FlashingHeader = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.FlashingHeader));
    }
  }

  public string BulletHeader
  {
    get => this._BulletHeader;
    internal set
    {
      this._BulletHeader = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.BulletHeader));
    }
  }

  public string StatusOfConnection
  {
    get => this._StatusOfConnection;
    internal set
    {
      this._StatusOfConnection = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.StatusOfConnection));
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

  public string FinishButtonLabel
  {
    get => this.finishButtonLabel;
    internal set
    {
      this.finishButtonLabel = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.FinishButtonLabel));
    }
  }

  public double ProgressBarValue
  {
    get => this._ProgressBarValue;
    internal set
    {
      this._ProgressBarValue = value;
      this.RaisePropertyChanged<double>((Expression<Func<double>>) (() => this.ProgressBarValue));
      this.ProgressBarValueText = string.Format(AppResource.APPLIANCE_FLASH_STEP, (object) this.Step, (object) this.StepTotal);
    }
  }

  public string ProgressBarStep
  {
    get => this._StepProgressForLabel;
    internal set
    {
      this._StepProgressForLabel = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.ProgressBarStep));
    }
  }

  public string ProgressBarValueText
  {
    get => this._ProgressBarValueText;
    internal set
    {
      this._ProgressBarValueText = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.ProgressBarValueText));
    }
  }

  public string ProgramLogLabel
  {
    get => this._ProgramLogLabel;
    internal set
    {
      this._ProgramLogLabel = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.ProgramLogLabel));
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

  public string FWversion
  {
    get => this._FWversion;
    internal set
    {
      this._FWversion = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.FWversion));
    }
  }

  public string Failures
  {
    get => this._Failures;
    private set
    {
      this._Failures = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.Failures));
    }
  }

  public string ProgramProgressLabel
  {
    get => this._ProgramProgressLabel;
    internal set
    {
      this._ProgramProgressLabel = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.ProgramProgressLabel));
    }
  }

  public string ProgramProgressDoneLabel
  {
    get => this.programProgressDoneLabel;
    internal set
    {
      this.programProgressDoneLabel = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.ProgramProgressDoneLabel));
    }
  }

  public bool DisplayProgramProgressDoneLabel
  {
    get => this.displayProgramProgressDoneLabel;
    internal set
    {
      this.displayProgramProgressDoneLabel = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.DisplayProgramProgressDoneLabel));
    }
  }

  public bool DisplayFinishButton
  {
    get => this.displayFinishButton;
    internal set
    {
      this.displayFinishButton = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.DisplayFinishButton));
    }
  }

  public bool DisplayProgramErrorLabel
  {
    get => this.displayProgramErrorLabel;
    internal set
    {
      this.displayProgramErrorLabel = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.DisplayProgramErrorLabel));
    }
  }

  public string CancelBackText
  {
    get => this._CancelBackText;
    internal set
    {
      this._CancelBackText = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.CancelBackText));
    }
  }

  public bool ProgressVisibility
  {
    get => this._ProgressVisibility;
    internal set
    {
      this._ProgressVisibility = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.ProgressVisibility));
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

  public bool InstallRepairMessageVisibility
  {
    get => this._InstallRepairMessageVisibility;
    internal set
    {
      this._InstallRepairMessageVisibility = value;
      if (value)
      {
        this.DisplayActivityIndicator = false;
        Task.Factory.StartNew<Task>((Func<Task>) (async () =>
        {
          int timeout = 60000;
          Task timeoutTask = new Task((Action) (() =>
          {
            MessagingCenter.Send<ApplianceFlashViewModel>(this, CoreApp.EventsNames.InstallRepairPopupRaised.ToString());
            while (this.InstallRepairMessageVisibility)
              Thread.Sleep(500);
          }));
          timeoutTask.Start();
          Task task = await Task.WhenAny(timeoutTask, Task.Delay(timeout));
          if (task == timeoutTask)
          {
            task = (Task) null;
            this.InstallRepairMessageVisibility = false;
            MessagingCenter.Send<ApplianceFlashViewModel>(this, CoreApp.EventsNames.InstallRepairPopupDismissed.ToString());
            timeoutTask.Dispose();
            timeoutTask = (Task) null;
          }
          else
          {
            Device.BeginInvokeOnMainThread((Action) (async () => this.InstallRepairMessageVisibility = !value));
            MessagingCenter.Send<ApplianceFlashViewModel>(this, CoreApp.EventsNames.InstallRepairPopupDismissed.ToString());
            timeoutTask = (Task) null;
          }
        }));
      }
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.InstallRepairMessageVisibility));
    }
  }

  public string InstallRepairMessage
  {
    get => this._InstallRepairMessage;
    internal set
    {
      this._InstallRepairMessage = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.InstallRepairMessage));
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

  public bool _ActiveView { get; internal set; }

  public bool _requestedReboot { get; internal set; }

  public List<AugmentedModule> ModuleList
  {
    get => this.UIModuleList;
    set
    {
      this.UIModuleList = new List<AugmentedModule>();
      int num = 1;
      foreach (AugmentedModule module in this.Session.ModuleList)
      {
        module.ModuleNo = num++;
        if (module.IncludeInFlashingSequence)
        {
          List<AugmentedModule> uiModuleList = this.UIModuleList;
          AugmentedModule augmentedModule = new AugmentedModule();
          augmentedModule.moduleid = module.moduleid;
          augmentedModule.type = module.type;
          augmentedModule.State = module.State;
          augmentedModule.StateColor = module.StateColor;
          augmentedModule.name = module.name;
          augmentedModule.Available = module.Available;
          augmentedModule.Installed = module.Installed;
          augmentedModule.moduleInstalled = module.moduleInstalled;
          augmentedModule.InstalledVersion = module.InstalledVersion;
          augmentedModule.version = module.version;
          augmentedModule.node = module.node;
          augmentedModule.IncludeInFlashingSequence = module.IncludeInFlashingSequence;
          augmentedModule.InSMM = module.InSMM;
          augmentedModule.FirmwareInEcuVersion = module.FirmwareInEcuVersion;
          uiModuleList.Add(augmentedModule);
        }
      }
      this.fwnonameList = this.UIModuleList.FindAll((Predicate<AugmentedModule>) (x => x.name == "" && x.moduleid.ToString() == ""));
      foreach (AugmentedModule fwnoname in this.fwnonameList)
        this._loggingService.getLogger().Information<AugmentedModule>("Item having empty name property will be removed  : {@fwitem}", fwnoname);
      this.UIModuleList.RemoveAll((Predicate<AugmentedModule>) (x => x.name == "" && x.moduleid.ToString() == ""));
      List<AugmentedModule> deleteDuplicates = new List<AugmentedModule>();
      foreach (AugmentedModule uiModule in this.UIModuleList)
      {
        AugmentedModule mod = uiModule;
        string type1 = mod.type;
        ModuleType moduleType = ModuleType.FIRMWARE;
        string str1 = moduleType.ToString();
        if (type1 == str1)
        {
          List<AugmentedModule> list = this.UIModuleList.Where<AugmentedModule>((Func<AugmentedModule, bool>) (x => x.moduleid == mod.moduleid && x.type == ModuleType.SPAU_FIRMWARE.ToString())).ToList<AugmentedModule>();
          if (list != null && list.Count > 0)
            deleteDuplicates.Add(mod);
        }
        else
        {
          string type2 = mod.type;
          moduleType = ModuleType.SPAU_FIRMWARE;
          string str2 = moduleType.ToString();
          if (type2 == str2)
          {
            mod.name = UtilityFunctions.GetProperSpauNaming(this.RepairEnumber, mod.moduleid, this._metadataService, this.Session, this._loggingService);
            if (mod.InSMM)
            {
              List<AugmentedModule> list = this.UIModuleList.Where<AugmentedModule>((Func<AugmentedModule, bool>) (x => x.moduleid == mod.moduleid && x.type == ModuleType.SPAU_FIRMWARE.ToString() && !x.InSMM)).ToList<AugmentedModule>();
              if (list.Count > 0)
                deleteDuplicates.AddRange((IEnumerable<AugmentedModule>) list);
            }
          }
        }
      }
      this.UIModuleList.RemoveAll((Predicate<AugmentedModule>) (x => deleteDuplicates.Contains(x)));
      this.RaisePropertyChanged<List<AugmentedModule>>((Expression<Func<List<AugmentedModule>>>) (() => this.ModuleList));
    }
  }

  internal void SetElpBootMode()
  {
    this.Failures = AppResource.APPLIANCE_FLASH_CHANNGING_BOOT_MODE + "\n";
    this.Session.SetMode(iService5.Ssh.enums.BootMode.Elp, (RequestCallback) (async response =>
    {
      if (response)
      {
        this.Failures = $"{this.Failures}{AppResource.APPLIANCE_FLASH_BOOT_MODE_SET} {AppResource.APPLIANCE_BOOT_MODE_NORMAL}\n";
        this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSMM, "Boot mode set to Normal System", memberName: nameof (SetElpBootMode), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceFlashViewModel.cs", sourceLineNumber: 1042);
        this._loggingService.getLogger().LogAppInformation(LoggingContext.USER, "Boot mode " + response.ToString(), memberName: nameof (SetElpBootMode), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceFlashViewModel.cs", sourceLineNumber: 1043);
        this.Failures = $"{this.Failures}{AppResource.APPLIANCE_FLASH_REQUESTED_REBOOT}\n";
        this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSMM, "Requested reboot", memberName: nameof (SetElpBootMode), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceFlashViewModel.cs", sourceLineNumber: 1045);
        await this.Session.Reboot((RequestCallback) (rebootResponse => this._requestedReboot = true));
      }
      else
      {
        this.Failures = $"{this.Failures}{AppResource.APPLIANCE_FLASH_FAILED_TO_SET_BOOTMODE}\n";
        this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSMM, "Failed to set bootmode", memberName: nameof (SetElpBootMode), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceFlashViewModel.cs", sourceLineNumber: 1054);
        this._loggingService.getLogger().LogAppInformation(LoggingContext.USER, "Could not set boot mode " + iService5.Ssh.enums.BootMode.Recovery.ToString(), memberName: nameof (SetElpBootMode), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceFlashViewModel.cs", sourceLineNumber: 1055);
      }
    }));
  }

  public override void ViewAppeared()
  {
    base.ViewAppeared();
    this.DisplayProgramErrorLabel = false;
    this.Session.setDisconnectedStatus(false);
  }

  public override void ViewAppearing()
  {
    base.ViewAppearing();
    this._appliance.CheckConnectivity(new connectivityUpdateCallback(this.UpdateStatus));
    this.tapped = false;
    if (this._ActiveView)
      return;
    this._ActiveView = true;
    try
    {
      this.Session.ConnectToAppliance();
      this.Session.RetrieveBootMode((RequestCallback) (bootModeSuccess =>
      {
        if (bootModeSuccess)
        {
          this.BootMode = this.Session.Bootmode.ToString();
          if (!string.IsNullOrEmpty(this.BootMode) && (this.BootMode.ToLowerInvariant().Contains("recovery") || this.BootMode.ToLowerInvariant().Contains("maintenance")))
            this.OrangeBarVisibility = true;
          if (this.OrangeBarVisibility)
          {
            if (!this.Session.LocalInventoryAvailable)
            {
              this.Session.RetrieveInventory((RequestCallback) (localInventorySuccess =>
              {
                if (localInventorySuccess)
                {
                  if (!this.Session.CachedInventoryMerged)
                  {
                    this.Session.RetrieveInventory((RequestCallback) (cachedInventorySuccess =>
                    {
                      this.ModuleList = (List<AugmentedModule>) null;
                      this.ContinueWithProgramming();
                    }));
                  }
                  else
                  {
                    this.ModuleList = (List<AugmentedModule>) null;
                    this.ContinueWithProgramming();
                  }
                }
                else
                {
                  this.ModuleList = (List<AugmentedModule>) null;
                  this.ContinueWithProgramming();
                }
              }));
            }
            else
            {
              this.ModuleList = (List<AugmentedModule>) null;
              this.ContinueWithProgramming();
            }
          }
          else if (!this.Session.InventoryAvailable)
          {
            this.Session.RetrieveInventory((RequestCallback) (inventorySuccess =>
            {
              this.ModuleList = (List<AugmentedModule>) null;
              this.ContinueWithProgramming();
            }));
          }
          else
          {
            this.ModuleList = (List<AugmentedModule>) null;
            this.ContinueWithProgramming();
          }
        }
        else
          this.ContinueWithProgramming();
      }));
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppError(LoggingContext.APPLIANCE, ex.Message, ex, nameof (ViewAppearing), "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceFlashViewModel.cs", 1151);
    }
  }

  internal void ContinueWithProgramming()
  {
    foreach (AugmentedModule module in this.ModuleList)
      this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSMM, $"ID: {module.moduleid}, Installed Version: {module.InstalledVersion}, Version Available: {module.Available.ToString()}, Type: {module.type}", memberName: nameof (ContinueWithProgramming), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceFlashViewModel.cs", sourceLineNumber: 1158);
    this.IsFlashBtnEnabled = true;
    if (!this.isFlashingAfterReboot)
      return;
    this.OnSubmit(this);
  }

  internal void UpdateStatus()
  {
    this.WifiStatus = this._appliance.StatusOfConnection;
    this.ConnectedColor = this._appliance.ConnectedColor;
    this.IsFlashBtnEnabled = this._appliance.boolStatusOfConnection && !this.IsProgrammingInProgress;
    if (this.IsProgrammingInProgress && !this._appliance.boolStatusOfConnection && !this.isDisconnected)
    {
      this.isDisconnected = true;
      this._alertService.ShowMessageAlertWithKeyFromService("APPLIANCE_FLASH_CONNECTION_LOST", AppResource.INFORMATION_TEXT, (Action) (() =>
      {
        this.Session.setDisconnectedStatus(true);
        this._ActiveView = false;
        this._navigationService.Navigate<ApplianceInstructionConnectionViewModel>((IMvxBundle) null, new CancellationToken());
        this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
      }));
    }
    else
    {
      if (!this._requestedModeChangeReboot)
        return;
      this._requestedModeChangeReboot = false;
      this.FollowBootModeChange();
    }
  }

  private void FollowBootModeChange()
  {
    this.tapped = false;
    this._userSession.setProperty((object) "{\"reboot\":\"\"}");
    this._ActiveView = false;
    IMvxNavigationService navigationService = this._navigationService;
    DetailNavigationArgs detailNavigationArgs = new DetailNavigationArgs();
    detailNavigationArgs._elpRecoveryBoot = true;
    detailNavigationArgs._bootModeSwitch = false;
    detailNavigationArgs.ReturnToFlashing = true;
    detailNavigationArgs._simpleBootToRecovery = this.simpleBootToRecovery;
    CancellationToken cancellationToken = new CancellationToken();
    navigationService.Navigate<BootModeTransitionViewModel, DetailNavigationArgs>(detailNavigationArgs, (IMvxBundle) null, cancellationToken);
    this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
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

  public bool IsFlashBtnEnabled
  {
    get => this._IsFlashBtnEnabled;
    private set
    {
      this._IsFlashBtnEnabled = !value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.IsFlashBtnEnabled));
    }
  }

  public bool IsFinishBtnEnabled
  {
    get => this.isFinishBtnEnabled;
    internal set
    {
      this.isFinishBtnEnabled = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.IsFinishBtnEnabled));
    }
  }

  public bool IsProgrammingInProgress
  {
    get => this.isProgrammingInProgress;
    internal set
    {
      this.isProgrammingInProgress = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.IsProgrammingInProgress));
    }
  }

  public bool DisplayActivityIndicator
  {
    get => this.displayActivityIndicator;
    set
    {
      this.displayActivityIndicator = value;
      if (value)
        this.InstallRepairMessageVisibility = false;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.DisplayActivityIndicator));
    }
  }

  public bool ReturnAfterReboot { get; internal set; }

  public bool UpdateFinished { get; set; }

  public bool InventoryUpToDate { get; private set; }

  public void createExtractPpfsFolder()
  {
    string pathOf = UtilityFunctions.getPathOf(this._locator, "extractedPpfs");
    if (Directory.Exists(pathOf))
      Directory.Delete(pathOf, true);
    Directory.CreateDirectory(pathOf);
  }
}
