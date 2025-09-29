// Decompiled with JetBrains decompiler
// Type: iService5.Core.ViewModels.ApplianceRepairViewModel
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Appliance;
using iService5.Core.Services.Data;
using iService5.Core.Services.Helpers;
using iService5.Core.Services.Platform;
using iService5.Core.Services.User;
using iService5.Core.Services.User.ActivityLogging;
using iService5.Core.Services.WebSocket;
using iService5.Core.ViewModels.SyMaNa;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
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

public class ApplianceRepairViewModel : MvxViewModel<DetailNavigationArgs>
{
  private readonly IMvxNavigationService _navigationService;
  private readonly IUserSession _userSession;
  private readonly IMetadataService _metadataService;
  private readonly IAlertService _alertService;
  private readonly IAppliance _appliance;
  private readonly IPlatformSpecificServiceLocator _locator;
  private readonly ILoggingService _loggingService;
  private readonly IWebSocketService _webSocketService;
  internal List<DownloadProxy> _filesToDownload = new List<DownloadProxy>();
  internal List<DownloadProxy> _filesToBeReDownloaded = new List<DownloadProxy>();
  internal List<DownloadProxy> _vEnumberDownloadList = new List<DownloadProxy>();
  internal List<string> _downloadedModulesFileIds = new List<string>();
  internal List<string> _notFoundFileIds = new List<string>();
  private Is5SshWrapper _sshWrapper;
  private string vEnumber = "Z9KDKD0Z01(00)";
  internal long allfilesSizeToDownload = 0;
  internal long downloadedSize = 0;
  internal string allFilesSizeReadable;
  internal IList<string> _preparationModules = (IList<string>) new List<string>();
  internal IList<DownloadProxy> eNumbersForPPFDownload = (IList<DownloadProxy>) new List<DownloadProxy>();
  internal int numberOfEnumbersForPPFDownload = 0;
  private DownloadProxy currentEnumberForPPFDownload = new DownloadProxy();
  private FileType currentDownloadFileType = FileType.Unknown;
  internal double progressStatus = 0.0;
  internal List<DownloadProxy>.Enumerator _downloadEnumerator;
  internal int totalFiles = 0;
  internal int currentFile = 0;
  internal bool shouldContinueDownload = true;
  private int errors = 0;
  private int corruptedFilesCount = 0;
  private int ppfDownloadIndex;
  private bool exitLoop = false;
  private string strEnumber = string.Empty;
  private string _HeaderLabelText;
  internal string _senderScreen = "";
  private string _DownloadDataButtonText = string.Format(AppResource.REPAIR_PAGE_DOWNLOAD_BUTTON, (object) "");
  private string _UpdateText = AppResource.REPAIR_PAGE_APPLIANCE_DATA_UPD_REQUIRED;
  private string _ContinueButtonText = AppResource.REPAIR_PAGE_CONTINUE_WITHOUT_DOWNLOAD;
  private string _CheckingDataProcedureLabel = AppResource.REPAIR_PAGE_CHECKING_APPL_DATA;
  private string _DownloadingStatusLabel;
  internal string _RepairEnumber;
  private bool _RepairLabelVisibility = false;
  private bool _ShowImage = false;
  private bool _RepairLabel2Visibility = true;
  private bool _RepairButtonVisibility = true;
  private bool _TransparentButtonVisibility = false;
  private double _ProgressBarValue = 0.0;
  private string _NavText;
  private bool _WifiBridgeMode;
  private bool _WifiBridgeSetting;
  private string _WifiBridgeStatus;
  private bool _WifiApplianceMode;
  private bool _areButtonsEnabled = true;
  private bool _DisplayActivityIndicator = true;
  private MvxNotifyTask _myTaskNotifier;
  private bool _prepareCalled = false;

  public ICommand DownloadFilesCommand { protected set; get; }

  public ICommand NoDownload { protected set; get; }

  public ICommand GoToHomePage { protected set; get; }

  public ApplianceRepairViewModel(
    IMvxNavigationService navigationService,
    ILoggingService loggingService,
    IMetadataService metadataService,
    IUserSession userSession,
    IAlertService alertService,
    IAppliance appliance,
    IPlatformSpecificServiceLocator locator,
    IWebSocketService webSocketService,
    Is5SshWrapper sshWrapper)
  {
    this._userSession = userSession;
    this._metadataService = metadataService;
    this._alertService = alertService;
    this._appliance = appliance;
    this._locator = locator;
    this._navigationService = navigationService;
    this._loggingService = loggingService;
    this._webSocketService = webSocketService;
    this._sshWrapper = sshWrapper;
    this.RepairEnumber = this.strEnumber = this._userSession.getEnumberSession();
    this.DownloadFilesCommand = (ICommand) new Command((Action) (async () => await this.DownloadNow()));
    this.GoToHomePage = (ICommand) new Command(new Action(this.GoBack));
    this.NoDownload = (ICommand) new Command(new Action(this.ProceedToNextScreen));
    this.TransparentButtonVisibility = false;
    this.RepairLabelVisibility = false;
    this.ShowImage = false;
    this.RepairLabel2Visibility = false;
    this.RepairButtonVisibility = false;
  }

  public virtual async Task Initialize() => await base.Initialize();

  public override void Prepare(DetailNavigationArgs parameter)
  {
    this._userSession.setEnumberSession("");
    this.RepairEnumber = "";
    this.NavText = "";
    this.prepareCalled = true;
    this.TransparentButtonVisibility = false;
    this.RepairLabelVisibility = false;
    this.ShowImage = false;
    this.RepairLabel2Visibility = false;
    this.RepairButtonVisibility = false;
    if (string.IsNullOrEmpty(parameter.senderScreen))
      return;
    if (parameter.senderScreen == AppResource.PREPARE_YOUR_WORK_TITLE)
    {
      this._senderScreen = AppResource.PREPARE_YOUR_WORK_TITLE;
      if (parameter.preparationModules.Count == 0)
        this.ProceedToNextScreen();
      this._preparationModules = (IList<string>) parameter.preparationModules;
    }
    else if (parameter.senderScreen == AppResource.RE_DOWNLOAD_ENUMBER_VIEW)
    {
      this._senderScreen = AppResource.RE_DOWNLOAD_ENUMBER_VIEW;
      this.NavText = AppResource.BACK_TEXT;
      this.RepairEnumber = this.strEnumber;
      this._userSession.setEnumberSession(this.strEnumber);
    }
    else if (parameter.senderScreen == AppResource.COMPACT_APPLIANCE_TESTER_TITLE)
    {
      this._senderScreen = AppResource.COMPACT_APPLIANCE_TESTER_TITLE;
      this._userSession.setEnumberSession(this.vEnumber);
      this._userSession.SetSenderScreen(AppResource.COMPACT_APPLIANCE_TESTER_TITLE);
      this.RepairEnumber = AppResource.COMPACT_APPLIANCE_TESTER;
      this.NavText = AppResource.NAVIGATE_HOME_BTN_LABEL;
    }
  }

  public void OnBackButtonPressed() => this.GoBack();

  public void TerminateDownloadingSession()
  {
    this._userSession.TerminateDownloadSession();
    this._userSession.cancelDownload();
    this._userSession.signalTasks();
  }

  internal void GoBack()
  {
    this.TerminateDownloadingSession();
    Device.BeginInvokeOnMainThread((Action) (async () =>
    {
      if (this._senderScreen.Equals(AppResource.PREPARE_YOUR_WORK_TITLE) || this._senderScreen.Equals(AppResource.COMPACT_APPLIANCE_TESTER_TITLE))
      {
        if (!this.AreButtonsEnabled)
          return;
        this.AreButtonsEnabled = false;
        bool bridgeDisplay = false;
        if (UtilityFunctions.BridgeSettingExists())
        {
          string bridgeMode = CoreApp.settings.GetItem("BridgeOff").Value;
          bridgeDisplay = bridgeMode.ToLower().Equals("false");
          bridgeMode = (string) null;
        }
        IMvxNavigationService navigationService = this._navigationService;
        DetailNavigationArgs detailNavigationArgs = new DetailNavigationArgs();
        detailNavigationArgs.ReturnToSettingsPage = true;
        detailNavigationArgs.bridgeDisplay = bridgeDisplay;
        CancellationToken cancellationToken = new CancellationToken();
        int num = await navigationService.Navigate<TabViewModel, DetailNavigationArgs>(detailNavigationArgs, (IMvxBundle) null, cancellationToken) ? 1 : 0;
      }
      else if (this._senderScreen.Equals(AppResource.RE_DOWNLOAD_ENUMBER_VIEW))
      {
        this._locator.GetPlatformSpecificService().ResetSession();
        Preferences.Set("IsReDownloadedEnabled", false);
        Preferences.Remove("IsReDownloadedEnabled");
        int num = await this._navigationService.Close((IMvxViewModel) this, new CancellationToken()) ? 1 : 0;
      }
      else if (this.AreButtonsEnabled)
      {
        this.AreButtonsEnabled = false;
        this._locator.GetPlatformSpecificService().ResetSession();
        int num = await this._navigationService.Navigate<TabViewModel>((IMvxBundle) null, new CancellationToken()) ? 1 : 0;
      }
    }));
  }

  internal void ProceedToNextScreen()
  {
    Device.BeginInvokeOnMainThread((Action) (async () =>
    {
      if (!this.AreButtonsEnabled)
        return;
      this.AreButtonsEnabled = false;
      string sessionID = UtilityFunctions.getSessionIDForHistoryTable();
      if (this._senderScreen.Equals(AppResource.PREPARE_YOUR_WORK_TITLE))
      {
        IMvxNavigationService navigationService = this._navigationService;
        DetailNavigationArgs detailNavigationArgs = new DetailNavigationArgs();
        detailNavigationArgs.bridgeDisplay = this.WifiBridgeSetting;
        detailNavigationArgs.ReturnToSettingsPage = true;
        CancellationToken cancellationToken = new CancellationToken();
        int num = await navigationService.Navigate<TabViewModel, DetailNavigationArgs>(detailNavigationArgs, (IMvxBundle) null, cancellationToken) ? 1 : 0;
      }
      else if (this._RepairEnumber != null && this._metadataService.isSMMWithWifi(this._RepairEnumber))
        await this.navigateToNextViewModel();
      else if (this._RepairEnumber != null && !this._metadataService.isSMMWithWifi(this._RepairEnumber))
      {
        if (this.WifiBridgeMode)
        {
          this.CheckSSHAvailability(false);
          return;
        }
        int num = await this._navigationService.Navigate<ApplianceInstructionConnectionNonSmmViewModel, DetailNavigationArgs>(new DetailNavigationArgs(), (IMvxBundle) null, new CancellationToken()) ? 1 : 0;
      }
      int num1 = await this._navigationService.Close((IMvxViewModel) this, new CancellationToken()) ? 1 : 0;
      sessionID = (string) null;
    }));
  }

  private async void CheckBridgeMode(bool fromLookupTask)
  {
    BridgeWifiResponse response = await UtilityFunctions.EnableBridgeWIFIUsingApi(this._locator, this._loggingService);
    if (response.isSuccess)
    {
      if (this._senderScreen.Equals(AppResource.COMPACT_APPLIANCE_TESTER_TITLE))
      {
        int num1 = await this._navigationService.Navigate<ApplianceNonSMMFlashViewModel>((IMvxBundle) null, new CancellationToken()) ? 1 : 0;
      }
      else
      {
        int num2 = await this._navigationService.Navigate<NonSmmUploadFilesTransitionViewModel>((IMvxBundle) null, new CancellationToken()) ? 1 : 0;
      }
      int num3 = await this._navigationService.Close((IMvxViewModel) this, new CancellationToken()) ? 1 : 0;
      if (!fromLookupTask)
      {
        response = (BridgeWifiResponse) null;
      }
      else
      {
        this.TransparentButtonVisibility = !this._senderScreen.Equals(AppResource.COMPACT_APPLIANCE_TESTER_TITLE);
        this.PrepareForDownloadingSession();
        response = (BridgeWifiResponse) null;
      }
    }
    else
    {
      this.AreButtonsEnabled = true;
      string errorMessage = !UtilityFunctions.IsLocalNetworkPemrissionError(response.errorMessage) ? AppResource.SSH_COMMAND_ERROR : "HA_INFO_RETRY_MESSAGE_IOS_14";
      this._alertService.ShowMessageAlertWithKeyFromService(errorMessage, AppResource.INFORMATION_TEXT, AppResource.RETRY_LABEL, AppResource.CANCEL_LABEL, (Action<bool>) (shouldRetry =>
      {
        if (shouldRetry)
        {
          this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Retrying to get data from home appliance.", memberName: nameof (CheckBridgeMode), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceRepairViewModel.cs", sourceLineNumber: 257);
          this.CheckBridgeMode(fromLookupTask);
        }
        else
          this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
      }));
      errorMessage = (string) null;
      response = (BridgeWifiResponse) null;
    }
  }

  public string HeaderLabelText
  {
    get => this._HeaderLabelText;
    internal set
    {
      this._HeaderLabelText = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.HeaderLabelText));
    }
  }

  public string SenderScreen
  {
    get => this._senderScreen;
    internal set => this.SetProperty<string>(ref this._senderScreen, value, nameof (SenderScreen));
  }

  public string DownloadDataButtonText
  {
    get => this._DownloadDataButtonText;
    private set
    {
      this._DownloadDataButtonText = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.DownloadDataButtonText));
    }
  }

  public string UpdateText
  {
    get => this._UpdateText;
    private set
    {
      this._UpdateText = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.UpdateText));
    }
  }

  public string ContinueButtonText
  {
    get => this._ContinueButtonText;
    private set
    {
      this._ContinueButtonText = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.ContinueButtonText));
    }
  }

  public string CheckingDataProcedureLabel
  {
    get => this._CheckingDataProcedureLabel;
    private set
    {
      this._CheckingDataProcedureLabel = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.CheckingDataProcedureLabel));
    }
  }

  public string DownloadingStatusLabel
  {
    get => this._DownloadingStatusLabel;
    private set
    {
      this._DownloadingStatusLabel = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.DownloadingStatusLabel));
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

  public bool RepairLabelVisibility
  {
    get => this._RepairLabelVisibility;
    internal set
    {
      this._RepairLabelVisibility = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.RepairLabelVisibility));
    }
  }

  public bool ShowImage
  {
    get => this._ShowImage;
    internal set
    {
      this._ShowImage = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.ShowImage));
    }
  }

  public bool RepairLabel2Visibility
  {
    get => this._RepairLabel2Visibility;
    internal set
    {
      this._RepairLabel2Visibility = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.RepairLabel2Visibility));
    }
  }

  public bool RepairButtonVisibility
  {
    get => this._RepairButtonVisibility;
    internal set
    {
      this._RepairButtonVisibility = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.RepairButtonVisibility));
    }
  }

  public bool TransparentButtonVisibility
  {
    get => this._TransparentButtonVisibility;
    internal set
    {
      this._TransparentButtonVisibility = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.TransparentButtonVisibility));
    }
  }

  public double ProgressBarValue
  {
    get => this._ProgressBarValue;
    internal set
    {
      this._ProgressBarValue = value;
      this.RaisePropertyChanged<double>((Expression<Func<double>>) (() => this.ProgressBarValue));
    }
  }

  public string NavText
  {
    get => this._NavText;
    internal set => this.SetProperty<string>(ref this._NavText, value, nameof (NavText));
  }

  public bool WifiBridgeMode
  {
    get => this._WifiBridgeMode;
    internal set
    {
      this._WifiBridgeMode = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.WifiBridgeMode));
    }
  }

  public bool WifiBridgeSetting
  {
    get => this._WifiBridgeSetting;
    internal set
    {
      this._WifiBridgeSetting = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.WifiBridgeSetting));
    }
  }

  public string WifiBridgeStatus
  {
    get => this._WifiBridgeStatus;
    private set
    {
      this._WifiBridgeStatus = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.WifiBridgeStatus));
    }
  }

  public bool WifiApplianceMode
  {
    get => this._WifiApplianceMode;
    internal set
    {
      this._WifiApplianceMode = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.WifiApplianceMode));
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

  public bool DisplayActivityIndicator
  {
    get => this._DisplayActivityIndicator;
    set
    {
      if (this._DisplayActivityIndicator == value)
        return;
      this._DisplayActivityIndicator = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.DisplayActivityIndicator));
    }
  }

  public MvxNotifyTask MyTaskNotifier
  {
    get => this._myTaskNotifier;
    private set
    {
      this.SetProperty<MvxNotifyTask>(ref this._myTaskNotifier, value, nameof (MyTaskNotifier));
    }
  }

  public bool prepareCalled
  {
    get => this._prepareCalled;
    private set
    {
      this._prepareCalled = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.prepareCalled));
    }
  }

  public override void ViewDisappeared()
  {
    if (!this._userSession.isDownloadActive())
      return;
    this.TerminateDownloadingSession();
    this.shouldContinueDownload = false;
  }

  public override void ViewAppeared()
  {
    if (!this.prepareCalled)
    {
      this.TransparentButtonVisibility = false;
      this.RepairLabelVisibility = false;
      this.ShowImage = false;
      this.RepairLabel2Visibility = false;
      this.RepairButtonVisibility = false;
      Task.Delay(3000);
    }
    base.ViewAppeared();
    this.AreButtonsEnabled = true;
    MvxNotifyTask.Create(new Func<Task>(this.LookupTask), (Action<Exception>) (ex => this.OnLookUpException(ex)));
    if (this.totalFiles != 0)
    {
      if (this._senderScreen.Equals(AppResource.PREPARE_YOUR_WORK_TITLE))
      {
        this.RepairEnumber = AppResource.PREPARE_YOUR_WORK_TITLE;
        this.NavText = AppResource.APPLIANCE_FLASH_BACK_TEXT;
        this.MyTaskNotifier = MvxNotifyTask.Create((Func<Task>) (async () => await this.DownloadNow()), (Action<Exception>) (ex => this._loggingService.getLogger().LogAppError(LoggingContext.USERSESSION, $"Error while downloading files: {ex}", memberName: nameof (ViewAppeared), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceRepairViewModel.cs", sourceLineNumber: 596)));
      }
      else if (this._senderScreen.Equals(AppResource.COMPACT_APPLIANCE_TESTER_TITLE))
      {
        this.RepairEnumber = AppResource.COMPACT_APPLIANCE_TESTER;
        this.NavText = AppResource.NAVIGATE_HOME_BTN_LABEL;
      }
      else
      {
        this.RepairEnumber = this._userSession.getEnumberSession();
        if (this._userSession.GetSenderScreen() == AppResource.RE_DOWNLOAD_ENUMBER_VIEW)
        {
          this.NavText = AppResource.BACK_TEXT;
          this._userSession.SetSenderScreen(AppResource.RE_DOWNLOAD_ENUMBER_VIEW);
        }
        else
        {
          this.NavText = AppResource.NAVIGATE_HOME_BTN_LABEL;
          this._userSession.SetSenderScreen("RepairView");
        }
        this._locator.GetPlatformSpecificService().setEnumber(this.RepairEnumber);
        this._locator.GetPlatformSpecificService().GetLocationConsent();
        this._appliance.CheckConnectivity(new connectivityUpdateCallback(this.updateStatus));
      }
    }
    this._loggingService.getLogger().LogAppInformation(LoggingContext.USERSESSION, "Visited repair page for " + this.RepairEnumber, memberName: nameof (ViewAppeared), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceRepairViewModel.cs", sourceLineNumber: 622);
    this.CheckingDataProcedureLabel = AppResource.REPAIR_PAGE_CHECKING_APPL_DATA;
    this.DownloadingStatusLabel = $"(0.0 bytes/{this.allFilesSizeReadable})";
  }

  internal void OnLookUpException(Exception ex)
  {
    Device.BeginInvokeOnMainThread((Action) (() => this.DisplayActivityIndicator = false));
    string str = this._senderScreen.Equals(AppResource.PREPARE_YOUR_WORK_TITLE) ? "Failed to get files for Prepared Enumbers " : "Failed to get files for enumber " + this._userSession.getEnumberSession();
    this._loggingService.getLogger().LogAppError(LoggingContext.BINARY, $"{str} {ex.Message}", memberName: nameof (OnLookUpException), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceRepairViewModel.cs", sourceLineNumber: 636);
  }

  internal void InitializeFilesToGetDownloaded()
  {
    try
    {
      if (this._filesToDownload == null)
        this._filesToDownload = new List<DownloadProxy>();
      else
        this._filesToDownload.Clear();
      this._filesToBeReDownloaded = new List<DownloadProxy>();
      if (Preferences.Get("IsReDownloadedEnabled", false))
        this._filesToBeReDownloaded.AddRange((IEnumerable<DownloadProxy>) this._metadataService.getZ9KDfiles(true).Select<document, DownloadProxy>((Func<document, DownloadProxy>) (x => new DownloadProxy()
        {
          Document = x
        })).ToList<DownloadProxy>());
      this._filesToDownload.AddRange((IEnumerable<DownloadProxy>) this._metadataService.getZ9KDfiles().Select<document, DownloadProxy>((Func<document, DownloadProxy>) (x => new DownloadProxy()
      {
        Document = x
      })).ToList<DownloadProxy>());
      if (!this._senderScreen.Equals(AppResource.COMPACT_APPLIANCE_TESTER_TITLE))
      {
        if (!this._senderScreen.Equals(AppResource.PREPARE_YOUR_WORK_TITLE))
          this._preparationModules.Add(this.RepairEnumber);
        List<string> values1 = new List<string>();
        List<string> values2 = new List<string>();
        List<string> values3 = new List<string>();
        foreach (string preparationModule in (IEnumerable<string>) this._preparationModules)
        {
          if (this._metadataService.isSMM(preparationModule))
            values1.Add(preparationModule);
          else if (this._metadataService.isSMMFlintStone(preparationModule))
            values3.Add(preparationModule);
          else
            values2.Add(preparationModule);
        }
        string commaSeperatedValues1 = UtilityFunctions.getCommaSeperatedValues<string>(values1);
        string commaSeperatedValues2 = UtilityFunctions.getCommaSeperatedValues<string>(values2);
        string commaSeperatedValues3 = UtilityFunctions.getCommaSeperatedValues<string>(values3);
        List<MaterialStatistics> statisticsOfEnumbers1 = this._metadataService.getMaterialStatisticsOfENumbers(commaSeperatedValues1);
        List<MaterialStatistics> statisticsOfEnumbers2 = this._metadataService.getMaterialStatisticsOfENumbers(commaSeperatedValues3);
        List<MaterialStatistics> statisticsOfEnumbers3 = this._metadataService.getMaterialStatisticsOfENumbers(commaSeperatedValues2);
        Dictionary<DeviceClass, List<MaterialStatistics>> materialList = new Dictionary<DeviceClass, List<MaterialStatistics>>();
        materialList[DeviceClass.SMM] = statisticsOfEnumbers1;
        materialList[DeviceClass.NON_SMM] = statisticsOfEnumbers3;
        materialList[DeviceClass.SMM_FLINTSTONE] = statisticsOfEnumbers2;
        if (Preferences.Get("IsReDownloadedEnabled", false))
        {
          this._filesToBeReDownloaded.AddRange((IEnumerable<DownloadProxy>) this._metadataService.getModulesDeltaSetForDownloadSetting(materialList, true).Select<module_with_enumber, DownloadProxy>((Func<module_with_enumber, DownloadProxy>) (x => new DownloadProxy()
          {
            Module = (module) x,
            vib = x.vib,
            ki = x.ki,
            node = x.node
          })).ToList<DownloadProxy>());
          this._filesToBeReDownloaded.AddRange(this._metadataService.getDocumentsDeltaSetForSingleDownloadAndPrepareYourWork(materialList, true).Select<document, DownloadProxy>((Func<document, DownloadProxy>) (x => new DownloadProxy()
          {
            Document = x
          })));
        }
        this._filesToDownload.AddRange((IEnumerable<DownloadProxy>) this._metadataService.getModulesDeltaSetForDownloadSetting(materialList).Select<module_with_enumber, DownloadProxy>((Func<module_with_enumber, DownloadProxy>) (x => new DownloadProxy()
        {
          Module = (module) x,
          vib = x.vib,
          ki = x.ki,
          node = x.node
        })).ToList<DownloadProxy>());
        this._filesToDownload.AddRange(this._metadataService.getDocumentsDeltaSetForSingleDownloadAndPrepareYourWork(materialList).Select<document, DownloadProxy>((Func<document, DownloadProxy>) (x => new DownloadProxy()
        {
          Document = x
        })));
        if (Preferences.Get("IsReDownloadedEnabled", false))
          this._filesToDownload.AddRange((IEnumerable<DownloadProxy>) this._filesToBeReDownloaded);
      }
      foreach (string preparationModule in (IEnumerable<string>) this._preparationModules)
      {
        if (this._metadataService.isSMM(preparationModule) && !this.AreAllPPFsAvailableForENumber(preparationModule))
        {
          Tuple<string, string> vibAndKi = UtilityFunctions.getVibAndKi(preparationModule);
          this.eNumbersForPPFDownload.Add(new DownloadProxy()
          {
            vib = vibAndKi.Item1,
            ki = vibAndKi.Item2
          });
        }
      }
      Device.BeginInvokeOnMainThread((Action) (() => this.DisplayActivityIndicator = false));
      this.numberOfEnumbersForPPFDownload = this.eNumbersForPPFDownload.Count;
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppError(LoggingContext.BINARY, "Exception occurred in InitializeFilesToGetDownloaded: " + ex.Message, memberName: nameof (InitializeFilesToGetDownloaded), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceRepairViewModel.cs", sourceLineNumber: 739);
    }
  }

  private void CleanPreviousBinaryFiles()
  {
    try
    {
      if (this._filesToBeReDownloaded == null || this._filesToBeReDownloaded.Count <= 0)
        return;
      foreach (DownloadProxy downloadProxy in this._filesToBeReDownloaded)
      {
        string empty = string.Empty;
        string path1 = string.Empty;
        long num;
        string path2;
        if (downloadProxy.Document != null)
        {
          this._metadataService.UpdateDownloadedStatus(downloadProxy.Document.fileId, true, false);
          string folder = this._locator.GetPlatformSpecificService().getFolder();
          num = downloadProxy.Document._document;
          string path2_1 = $"{num.ToString()}.{downloadProxy.Document.version}.bin";
          path2 = Path.Combine(folder, path2_1);
        }
        else
        {
          this._metadataService.UpdateDownloadedStatus(downloadProxy.Module.fileId, false, false);
          string folder1 = this._locator.GetPlatformSpecificService().getFolder();
          num = downloadProxy.Module.moduleid;
          string path2_2 = $"{num.ToString()}.{downloadProxy.Module.version}.cpio";
          path2 = Path.Combine(folder1, path2_2);
          string folder2 = this._locator.GetPlatformSpecificService().getFolder();
          num = downloadProxy.Module.moduleid;
          string path2_3 = $"{num.ToString()}.{downloadProxy.Module.version}.tar.gz";
          path1 = Path.Combine(folder2, path2_3);
        }
        if (File.Exists(path2))
          File.Delete(path2);
        if (File.Exists(path1))
          File.Delete(path1);
      }
      this._filesToBeReDownloaded.Clear();
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppError(LoggingContext.BINARY, "Exception occurred in CleanPreviousBinaryFiles: " + ex.Message, memberName: nameof (CleanPreviousBinaryFiles), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceRepairViewModel.cs", sourceLineNumber: 783);
    }
  }

  internal async Task LookupTask()
  {
    this.WifiBridgeMode = this._appliance.isConnectedToBridgeWifi();
    this.WifiApplianceMode = this._appliance.isConnectedToApplianceWifi();
    this.WifiBridgeSetting = !CoreApp.settings.GetItem("BridgeOff").Value.ToLower().Equals("true");
    this.InitializeFilesToGetDownloaded();
    this.totalFiles = this.NumberOfFilesToGetDownloaded();
    if (this.totalFiles == 0 && this.eNumbersForPPFDownload.Count == 0)
    {
      if (this._senderScreen.Equals(AppResource.PREPARE_YOUR_WORK_TITLE))
      {
        IMvxNavigationService navigationService = this._navigationService;
        DetailNavigationArgs detailNavigationArgs = new DetailNavigationArgs();
        detailNavigationArgs.bridgeDisplay = this.WifiBridgeSetting;
        detailNavigationArgs.ReturnToSettingsPage = true;
        CancellationToken cancellationToken = new CancellationToken();
        int num = await navigationService.Navigate<TabViewModel, DetailNavigationArgs>(detailNavigationArgs, (IMvxBundle) null, cancellationToken) ? 1 : 0;
        return;
      }
      this.RepairLabelVisibility = false;
      this.RepairLabel2Visibility = false;
      this.RepairButtonVisibility = false;
      this.TransparentButtonVisibility = false;
      this.ShowImage = false;
      if (this._metadataService.isSMMWithWifi(this._userSession.getEnumberSession()))
      {
        await this.navigateToNextViewModel();
      }
      else
      {
        if (this.WifiBridgeMode)
        {
          this.CheckSSHAvailability(true);
          return;
        }
        int num = await this._navigationService.Navigate<ApplianceInstructionConnectionNonSmmViewModel, DetailNavigationArgs>(new DetailNavigationArgs(), (IMvxBundle) null, new CancellationToken()) ? 1 : 0;
      }
      int num1 = await this._navigationService.Close((IMvxViewModel) this, new CancellationToken()) ? 1 : 0;
    }
    this.TransparentButtonVisibility = !this._senderScreen.Equals(AppResource.COMPACT_APPLIANCE_TESTER_TITLE);
    this.PrepareForDownloadingSession();
  }

  private void CheckSSHAvailability(bool fromLookup)
  {
    if (UtilityFunctions.IsLocalNetworkPermissionNeeded() && CoreApp.settings.GetItem("localNetworkPopupAppeared").Value == "false")
    {
      this._sshWrapper.IPAddress = this._locator.GetPlatformSpecificService().GetIp();
      this._sshWrapper.GetSshAvailability();
      CoreApp.settings.UpdateItem(new Settings("localNetworkPopupAppeared", "true"));
      this.AreButtonsEnabled = true;
    }
    else
      this.CheckBridgeMode(fromLookup);
  }

  private async Task navigateToNextViewModel()
  {
    if (this._metadataService.IsENumberSyMaNa(this.RepairEnumber))
    {
      if (this.WifiApplianceMode && UtilityFunctions.IsLocalNetworkPermissionNeeded() && CoreApp.settings.GetItem("localNetworkPopupAppeared").Value == "true")
      {
        int num1 = await this._navigationService.Navigate<SyMaNaConnectionViewModel>((IMvxBundle) null, new CancellationToken()) ? 1 : 0;
      }
      else
      {
        int num2 = await this._navigationService.Navigate<SyMaNaApplianceInstructionConnectionViewModel>((IMvxBundle) null, new CancellationToken()) ? 1 : 0;
      }
    }
    else if (this.WifiApplianceMode)
    {
      int num3 = await this._navigationService.Navigate<SmmConnectionViewModel>((IMvxBundle) null, new CancellationToken()) ? 1 : 0;
    }
    else
    {
      int num4 = await this._navigationService.Navigate<ApplianceInstructionConnectionViewModel>((IMvxBundle) null, new CancellationToken()) ? 1 : 0;
    }
  }

  internal void PrepareForDownloadingSession()
  {
    this.RepairLabelVisibility = false;
    this.ShowImage = false;
    this.RepairLabel2Visibility = this.totalFiles > 0;
    this.RepairButtonVisibility = true;
    this.currentFile = 1;
    this._downloadEnumerator = this._filesToDownload.GetEnumerator();
    this._downloadEnumerator.MoveNext();
    this.allfilesSizeToDownload = 0L;
    this.totalFiles = this.NumberOfFilesToGetDownloaded();
    foreach (string str in this._filesToDownload.Select<DownloadProxy, string>((Func<DownloadProxy, string>) (x => x.GetFileId())).Distinct<string>())
    {
      string id = str;
      this.allfilesSizeToDownload += this._filesToDownload.First<DownloadProxy>((Func<DownloadProxy, bool>) (x => x.GetFileId() == id)).GetFileSize();
    }
    this.allFilesSizeReadable = this.GetFormattedFileSize(this.allfilesSizeToDownload);
    this.DownloadDataButtonText = this.totalFiles != 0 || this.eNumbersForPPFDownload.Count <= 0 ? $"{string.Format(AppResource.REPAIR_PAGE_DOWNLOAD_BUTTON, (object) this.totalFiles.ToString())} ({this.allFilesSizeReadable})" : AppResource.PPF_FILES_MISSING;
    this.DisplayActivityIndicator = false;
  }

  internal string GetFormattedFileSize(long fileSize) => FileSizeFormatter.FormatSize(fileSize);

  internal int NumberOfFilesToGetDownloaded() => this._filesToDownload.Count;

  internal bool AreAllPPFsAvailableForENumber(string eNumber)
  {
    List<smm_module> modulesDetails = this._metadataService.GetModulesDetails(eNumber);
    List<ppf> enumbersDownloadedPpfs = this._metadataService.GetAllEnumbersDownloadedPpfs(eNumber);
    foreach (smm_module smmModule in modulesDetails)
    {
      smm_module module = smmModule;
      if (!enumbersDownloadedPpfs.Any<ppf>((Func<ppf, bool>) (ppf => ppf.moduleid == module.moduleid && ppf.version == module.version)))
        return false;
    }
    return true;
  }

  public void progressCBForBinary(double progress)
  {
    this.ProgressBarValue = (this.progressStatus + progress) / (double) this.allfilesSizeToDownload;
    if (progress != (double) this._downloadEnumerator.Current.GetFileSize())
      return;
    this.progressStatus += progress;
  }

  public void progressCBForPPF(double progress)
  {
  }

  public void downloadCBForBinary(DownloadStatus status)
  {
    if (status != DownloadStatus.COMPLETED)
    {
      this.ProcessDownloadFailureStatus(status);
    }
    else
    {
      if (this._metadataService.markAsDownloaded(this._downloadEnumerator.Current))
      {
        if (this.currentFile < this.totalFiles)
        {
          ++this.currentFile;
          this.CheckingDataProcedureLabel = string.Format(AppResource.REPAIR_PAGE_DOWNLOAD_FILES_PROGRESS, (object) this.currentFile, (object) this.totalFiles);
        }
        this.downloadedSize += this._downloadEnumerator.Current.GetFileSize();
        this.DownloadingStatusLabel = $"({this.GetFormattedFileSize(this.downloadedSize)}/{this.allFilesSizeReadable})";
      }
      else
        ++this.corruptedFilesCount;
      this.ContinueWithNextFile();
    }
  }

  public void downloadCBForPPF(DownloadStatus status)
  {
    this.eNumbersForPPFDownload.Remove(this.currentEnumberForPPFDownload);
    if (status != DownloadStatus.COMPLETED)
    {
      this.ProcessDownloadFailureStatus(status);
    }
    else
    {
      UtilityFunctions.ContinueWithPpfExtraction(this.currentEnumberForPPFDownload, status, this._userSession, this._loggingService, this._locator, this._metadataService);
      this.ContinueWithNextPPFFile();
    }
  }

  internal void DownloadPpfFiles()
  {
    this.currentEnumberForPPFDownload = this.eNumbersForPPFDownload.First<DownloadProxy>();
    if (this.currentEnumberForPPFDownload.ShouldCheckForBundlePpfInfoFile())
    {
      this.currentDownloadFileType = FileType.BundledPpf;
      this._userSession.getBinary(this.currentEnumberForPPFDownload, this.currentDownloadFileType, new downloadCallback(this.downloadCBForPPF), new iService5.Core.Services.User.progressCallback(this.progressCBForPPF));
    }
    else
    {
      this.eNumbersForPPFDownload.Remove(this.currentEnumberForPPFDownload);
      this.ContinueWithNextPPFFile();
    }
  }

  internal void ProcessDownloadFailureStatus(DownloadStatus status)
  {
    try
    {
      string fileId;
      if (this.currentDownloadFileType == FileType.Binary)
      {
        this._loggingService.getLogger().LogAppError(LoggingContext.BACKEND, "Failed to download " + this._downloadEnumerator.Current.ToFileName(), memberName: nameof (ProcessDownloadFailureStatus), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceRepairViewModel.cs", sourceLineNumber: 993);
        fileId = this._downloadEnumerator.Current.GetFileId();
      }
      else
      {
        this._loggingService.getLogger().LogAppError(LoggingContext.BACKEND, "Failed to download " + this.currentEnumberForPPFDownload.ToFileName(), memberName: nameof (ProcessDownloadFailureStatus), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceRepairViewModel.cs", sourceLineNumber: 998);
        fileId = this.currentEnumberForPPFDownload.GetFileId();
      }
      int num;
      switch (status)
      {
        case DownloadStatus.FAILED:
        case DownloadStatus.TIMEOUT:
          num = 1;
          break;
        case DownloadStatus.FILE_NOT_FOUND:
          this._notFoundFileIds.Add(fileId);
          this.ContinueWithNextDownload();
          return;
        default:
          num = status == DownloadStatus.EXCEPTION ? 1 : 0;
          break;
      }
      if (num != 0)
      {
        ++this.errors;
        this.ContinueWithNextDownload();
      }
      else if (status == DownloadStatus.UNAUTHORIZED)
      {
        this._alertService.ShowMessageAlertWithKey("JWT_TOKEN_EXPIRED", AppResource.WARNING_TEXT);
        this._navigationService.Navigate<LoginViewModel>((IMvxBundle) null, new CancellationToken());
      }
      else if (status == DownloadStatus.SERVER_CERTIFICATE_UNKNOWN)
      {
        this._loggingService.getLogger().LogAppError(LoggingContext.BACKEND, "Server certificate validation failed", memberName: nameof (ProcessDownloadFailureStatus), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceRepairViewModel.cs", sourceLineNumber: 1019);
        this._alertService.ShowMessageAlertWithKey("SERVER_TRUST_FAILURE", AppResource.WARNING_TEXT);
        this._navigationService.Navigate<LoginViewModel>((IMvxBundle) null, new CancellationToken());
      }
      else
      {
        this._notFoundFileIds.Add(fileId);
        this.ContinueWithNextDownload();
      }
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppInformation(LoggingContext.BACKEND, "Exception during error handling " + ex.Message, memberName: nameof (ProcessDownloadFailureStatus), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceRepairViewModel.cs", sourceLineNumber: 1031);
    }
  }

  internal void ContinueWithNextDownload()
  {
    if (this.currentDownloadFileType == FileType.Binary)
      this.ContinueWithNextFile();
    else
      this.ContinueWithNextPPFFile();
  }

  internal void ContinueWithNextFile()
  {
    if (this._downloadEnumerator.MoveNext() && this.shouldContinueDownload)
      this.DownloadFile();
    else if (this.eNumbersForPPFDownload.Count > 0)
    {
      this.DisplayActivityIndicator = true;
      this.InitiatePPFFileDownload();
    }
    else
      this.DisplayErrorOrProceed();
  }

  internal void InitiatePPFFileDownload()
  {
    this.ProgressBarValue = 0.0;
    this.ppfDownloadIndex = 1;
    this.DownloadPpfFiles();
  }

  internal void ContinueWithNextPPFFile()
  {
    ++this.ppfDownloadIndex;
    if (this.eNumbersForPPFDownload.Count > 0)
      this.DownloadPpfFiles();
    else
      this.DisplayErrorOrProceed();
  }

  internal void DisplayErrorOrProceed()
  {
    if (this.errors > 0 || this.corruptedFilesCount > 0 || this._notFoundFileIds.Count > 0)
      this.UpdateStatusUponError();
    else
      this.ProceedToNextScreen();
  }

  internal void DownloadFile()
  {
    if (this._notFoundFileIds.Contains(this._downloadEnumerator.Current.GetFileId()))
    {
      this.ContinueWithNextFile();
    }
    else
    {
      this.currentDownloadFileType = FileType.Binary;
      this._userSession.getBinary(this._downloadEnumerator.Current, this.currentDownloadFileType, new downloadCallback(this.downloadCBForBinary), new iService5.Core.Services.User.progressCallback(this.progressCBForBinary));
    }
  }

  internal void UpdateStatusUponError()
  {
    if (this.errors > 0 || this.corruptedFilesCount > 0)
      this._alertService.ShowMessageAlertWithKey("FIRMWARE_SINGLE_DL_FAIL_MSG", AppResource.WARNING_TEXT);
    else
      this._alertService.ShowMessageAlertWithKey("FIRMWARE_DL_NOT_FOUND_MSG", AppResource.WARNING_TEXT);
    this.InitializeFilesToGetDownloaded();
    this.TransparentButtonVisibility = true;
    this.progressStatus = 0.0;
    this.ProgressBarValue = 0.0;
    this.errors = 0;
    this.corruptedFilesCount = 0;
    this.PrepareForDownloadingSession();
    this.DisplayActivityIndicator = false;
    this.DownloadingStatusLabel = $"(0.0 bytes/{this.allFilesSizeReadable})";
  }

  public async Task DownloadNow()
  {
    this.exitLoop = false;
    if (Preferences.Get("IsReDownloadedEnabled", false))
    {
      this.TransparentButtonVisibility = this.RepairButtonVisibility = this.RepairLabel2Visibility = false;
      this.DisplayActivityIndicator = true;
      this.CleanPreviousBinaryFiles();
      this.DisplayActivityIndicator = false;
      Preferences.Set("IsReDownloadedEnabled", false);
    }
    while (!this.exitLoop)
    {
      if (await this._locator.GetPlatformSpecificService().IsNetworkAvailable())
      {
        this.exitLoop = true;
        this._userSession.designalTasks();
        this._userSession.StartDownloadSession();
        this.RepairLabelVisibility = true;
        this.RepairLabel2Visibility = false;
        this.RepairButtonVisibility = false;
        this.TransparentButtonVisibility = false;
        this._notFoundFileIds.Clear();
        this.ShowImage = true;
        try
        {
          if (this.totalFiles > 0)
          {
            this.CheckingDataProcedureLabel = string.Format(AppResource.REPAIR_PAGE_DOWNLOAD_FILES_PROGRESS, (object) this.currentFile, (object) this.totalFiles);
            this.DownloadFile();
          }
          else if (this.eNumbersForPPFDownload.Count > 0)
          {
            this.DisplayActivityIndicator = true;
            this.InitiatePPFFileDownload();
          }
          else
            this._loggingService.getLogger().LogAppInformation(LoggingContext.BACKEND, "No file to download", memberName: nameof (DownloadNow), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceRepairViewModel.cs", sourceLineNumber: 1167);
        }
        catch (Exception ex)
        {
          this._loggingService.getLogger().LogAppInformation(LoggingContext.BACKEND, "Exception while file download " + ex.Message, memberName: nameof (DownloadNow), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceRepairViewModel.cs", sourceLineNumber: 1172);
        }
        this.exitLoop = true;
      }
      else
        await this.HostNotReachable();
    }
  }

  internal void updateStatus()
  {
    this.WifiBridgeStatus = this._appliance.StatusOfBridgeConnection;
    this.WifiBridgeMode = this._appliance.boolStatusOfBridgeConnection;
    this.WifiApplianceMode = this._appliance.boolStatusOfConnection;
  }

  internal async Task HostNotReachable()
  {
    this._loggingService.getLogger().LogAppInformation(LoggingContext.BACKEND, "Host is not reachable due to no internet access", memberName: nameof (HostNotReachable), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceRepairViewModel.cs", sourceLineNumber: 1191);
    int num = await this._alertService.ShowMessageAlertWithKey("NO_INTERNET", AppResource.ERROR_TITLE, AppResource.CANCEL_LABEL, AppResource.RETRY_LABEL, (Action<bool>) (bobj =>
    {
      if (!bobj)
        return;
      this.exitLoop = true;
    })) ? 1 : 0;
  }
}
