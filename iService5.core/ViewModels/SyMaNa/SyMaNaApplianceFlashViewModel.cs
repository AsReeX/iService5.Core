// Decompiled with JetBrains decompiler
// Type: iService5.Core.ViewModels.SyMaNa.SyMaNaApplianceFlashViewModel
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Appliance;
using iService5.Core.Services.Backend;
using iService5.Core.Services.Data;
using iService5.Core.Services.Data.SyMaNa;
using iService5.Core.Services.Helpers;
using iService5.Core.Services.Localisation;
using iService5.Core.Services.Platform;
using iService5.Core.Services.User;
using iService5.Core.Services.User.ActivityLogging;
using iService5.Core.Services.WebSocket;
using iService5.Ssh.DTO;
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
//using Xamarin.Forms;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

#nullable disable
namespace iService5.Core.ViewModels.SyMaNa;

public class SyMaNaApplianceFlashViewModel : MvxViewModel<DetailNavigationArgs>
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
  private readonly IWebSocketService _webSocketService;
  private readonly ISecureStorageService _secureStorageService;
  internal bool tapped = false;
  internal bool isDisconnected = false;
  internal string BootMode = "";
  private SyMaNaWebClient symanaWebClient;
  private bool IsProgrammingInProgress;
  internal bool isDeviceReboot = false;
  private bool IsInitialCheckForInstallation;
  private long totalFileSize;
  private long progressValue;
  private bool executeHaInFo;
  private const int ProgressBarTime = 700;
  private ObservableCollection<SyMaNaECUInfoData> _moduleList;
  private List<SyMaNaFirmwareUploadModel> uploadFirmwareModels;
  private bool isBackButtonVisible;
  private double _ProgressBarValue;
  private bool _ProgressVisibility;
  private string _WifiStatus;
  private bool _isWifiConnected;
  private string _DummyLabel = "";
  private string _RepairEnumber;
  internal string _ConnectedColor;
  internal string _UploadButtonText = AppResource.PROGRAM_TEXT;
  private bool _IsUploadBtnDisabled;
  private bool _IsUploadButtonVisible = true;
  private bool _DisplayActivityIndicator;

  public event SyMaNaApplianceFlashViewModel.SyMaNaFlashViewHandler ProgressChanged;

  public ObservableCollection<SyMaNaECUInfoData> ModuleList
  {
    get => this._moduleList;
    private set
    {
      this._moduleList = value;
      this.RaisePropertyChanged<ObservableCollection<SyMaNaECUInfoData>>((Expression<Func<ObservableCollection<SyMaNaECUInfoData>>>) (() => this.ModuleList));
    }
  }

  public SyMaNaApplianceFlashViewModel(
    IMvxNavigationService navigationService,
    IShortTextsService _SService,
    ILoggingService loggingService,
    IMetadataService metadataService,
    IPlatformSpecificServiceLocator locator,
    IUserSession userSession,
    Is5SshWrapper sshWrapper,
    IAlertService alertService,
    IAppliance appliance,
    IApplianceSession session,
    IWebSocketService webSocketService,
    ISecureStorageService secureStorageService)
  {
    this._userSession = userSession;
    this._metadataService = metadataService;
    this._sshWrapper = sshWrapper;
    this._loggingService = loggingService;
    this._alertService = alertService;
    this._appliance = appliance;
    this.Session = session;
    this._locator = locator;
    this._navigationService = navigationService;
    this._webSocketService = webSocketService;
    this._secureStorageService = secureStorageService;
    this._sshWrapper.IPAddress = this._locator.GetPlatformSpecificService().GetIp();
    this.BackBtnClicked = (ICommand) new Command(new Action(this.ClosePage));
    this.UploadBtnClicked = (ICommand) new Command(new Action(this.UploadFirmwareCmd));
    this.SetInitialValues();
    this.ModuleList = new ObservableCollection<SyMaNaECUInfoData>();
    this.ResetProgressBarValues();
  }

  public override void Prepare(DetailNavigationArgs parameter)
  {
    if (parameter != null)
    {
      this.DisplayActivityIndicator = true;
      this.uploadFirmwareModels = parameter.UploadFirmwareModels;
      SyMaNaECU[] inventoryEcUs = parameter.InventoryECUs;
      if (inventoryEcUs != null && ((IEnumerable<SyMaNaECU>) inventoryEcUs).Count<SyMaNaECU>() > 0)
      {
        this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSYMANA, "Inventory data was passed from previous HaInfo screen, no need to fetch again.", memberName: nameof (Prepare), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaApplianceFlashViewModel.cs", sourceLineNumber: 123);
        this.MergeInventories(inventoryEcUs);
      }
      this.IsUploadButtonVisible = !parameter.readInventory;
      this.executeHaInFo = parameter.shouldSetHaInfo;
    }
    this.totalFileSize = this._metadataService.GetTotalFileSizeForENumber(this.RepairEnumber);
  }

  private void SetInitialValues()
  {
    this.RepairEnumber = this._userSession.getEnumberSession();
    this.DummyLabel = "Dummy Text";
    this.WifiStatus = "Connected";
    this.ConnectedColor = "Green";
    this.IsBackButtonVisible = true;
    this.Session.SetHaInfoFromMetadata();
    this.IsUploadBtnDisabled = true;
  }

  public ICommand BackBtnClicked { protected set; get; }

  public ICommand Flash_Tapped { internal set; get; }

  public ICommand Finish_Tapped { internal set; get; }

  public ICommand DismissPopup { internal set; get; }

  public ICommand UploadBtnClicked { protected set; get; }

  internal void ClosePage()
  {
    Device.BeginInvokeOnMainThread((Action) (() => this._navigationService.Close((IMvxViewModel) this, new CancellationToken())));
  }

  public void UploadFirmwareCmd()
  {
    Device.BeginInvokeOnMainThread((Action) (async () =>
    {
      if (this.UploadButtonText == AppResource.APPLIANCE_FLASH_PROCESS_FINISHED)
        this.ClosePage();
      else if (!this._appliance.boolStatusOfConnection && FeatureConfiguration.LodisTargetType() != TargetLodisType.LocalMOCK)
      {
        this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSYMANA, "No wifi connection", memberName: nameof (UploadFirmwareCmd), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaApplianceFlashViewModel.cs", sourceLineNumber: 184);
        this.DisplayActivityIndicator = false;
        this._alertService.ShowMessageAlertWithKey(AppResource.CONN_PAGE_TEXT2, "", AppResource.WARNING_OK, (Action) (() => { }));
      }
      else if (this.executeHaInFo)
      {
        if (await this.SetHaInfo())
        {
          await this.ClosingPendingInstallations();
          this.UploadPackages();
        }
      }
      else
      {
        await this.ClosingPendingInstallations();
        this.UploadPackages();
      }
    }));
  }

  private async Task<bool> SetHaInfo()
  {
    this.SetButtonTextVisibility(true, AppResource.SYMANA_SETTING_HAINFO);
    this._webSocketService.SetApplianceSessoin(this.Session);
    bool flag = await this.SetHaInfoDataSettings();
    return flag;
  }

  private async Task<bool> SetHaInfoDataSettings()
  {
    bool isSuccess = true;
    StringBuilder errorSB = new StringBuilder();
    SyMaNaLodisResponseData setHACountrySettings = await this._webSocketService.RequestDataFromLodis(SyMaNaRequestCommandName.setHaCountrySettings);
    if (setHACountrySettings.error != null || setHACountrySettings.resultCode != 0)
    {
      errorSB.AppendLine(AppResource.ERROR_SET_HA_COUNTRY_SETTINGS);
      isSuccess = false;
      this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSYMANA, "setHACountrySettings ERROR: " + setHACountrySettings.error?.ToString(), memberName: nameof (SetHaInfoDataSettings), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaApplianceFlashViewModel.cs", sourceLineNumber: 226);
    }
    SyMaNaLodisResponseData setHACi = await this._webSocketService.RequestDataFromLodis(SyMaNaRequestCommandName.setHaCustomerIndex);
    if (setHACi.error != null || setHACi.resultCode != 0)
    {
      errorSB.AppendLine(AppResource.ERROR_SET_HA_CUSTOMER_INDEX);
      isSuccess = false;
      this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSYMANA, "setHaCustomerIndex ERROR: " + setHACi.error?.ToString(), memberName: nameof (SetHaInfoDataSettings), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaApplianceFlashViewModel.cs", sourceLineNumber: 234);
    }
    SyMaNaLodisResponseData setHaVib = await this._webSocketService.RequestDataFromLodis(SyMaNaRequestCommandName.setHaVib);
    if (setHaVib.error != null || setHaVib.resultCode != 0)
    {
      errorSB.AppendLine(AppResource.ERROR_SET_HA_VIB);
      isSuccess = false;
      this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSYMANA, "setHaVib ERROR: " + setHaVib.error?.ToString(), memberName: nameof (SetHaInfoDataSettings), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaApplianceFlashViewModel.cs", sourceLineNumber: 242);
    }
    SyMaNaLodisResponseData setHaManufacturingTS = await this._webSocketService.RequestDataFromLodis(SyMaNaRequestCommandName.setHaManufacturingTimestamp);
    if (setHaManufacturingTS.error != null || setHaManufacturingTS.resultCode != 0)
    {
      errorSB.AppendLine(AppResource.ERROR_SET_HA_TIMESTAMP);
      isSuccess = false;
      this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSYMANA, "setHaManufacturingTS ERROR: " + setHaManufacturingTS.error?.ToString(), memberName: nameof (SetHaInfoDataSettings), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaApplianceFlashViewModel.cs", sourceLineNumber: 250);
    }
    SyMaNaLodisResponseData setHaBrand = await this._webSocketService.RequestDataFromLodis(SyMaNaRequestCommandName.setHaBrand);
    if (setHaBrand.error != null || setHaBrand.resultCode != 0)
    {
      errorSB.AppendLine(AppResource.ERROR_SET_HA_BRAND);
      isSuccess = false;
      this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSYMANA, "setHaBrand ERROR: " + setHaBrand.error?.ToString(), memberName: nameof (SetHaInfoDataSettings), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaApplianceFlashViewModel.cs", sourceLineNumber: 258);
    }
    SyMaNaLodisResponseData setHaDeviceType = await this._webSocketService.RequestDataFromLodis(SyMaNaRequestCommandName.setHaDeviceType);
    if (setHaDeviceType.error != null || setHaDeviceType.resultCode != 0)
    {
      errorSB.AppendLine(AppResource.ERROR_SET_HA_DEVICE_TYPE);
      isSuccess = false;
      this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSYMANA, "setHaDeviceType ERROR: " + setHaDeviceType.error?.ToString(), memberName: nameof (SetHaInfoDataSettings), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaApplianceFlashViewModel.cs", sourceLineNumber: 266);
    }
    if (!isSuccess)
      this.ShowPopUpWithClosePage(AppResource.ERROR_TITLE, errorSB.ToString());
    bool flag = isSuccess;
    errorSB = (StringBuilder) null;
    setHACountrySettings = (SyMaNaLodisResponseData) null;
    setHACi = (SyMaNaLodisResponseData) null;
    setHaVib = (SyMaNaLodisResponseData) null;
    setHaManufacturingTS = (SyMaNaLodisResponseData) null;
    setHaBrand = (SyMaNaLodisResponseData) null;
    setHaDeviceType = (SyMaNaLodisResponseData) null;
    return flag;
  }

  private void UploadPackages()
  {
    this.SetButtonTextVisibility(true, AppResource.APPLIANCE_FLASH_FILES_FOR_NODE + " ...");
    this.IsInitialCheckForInstallation = false;
    this.IsProgrammingInProgress = false;
    this.ProgressVisibility = true;
    this.progressValue = 0L;
    Task.Run(new Action(this.PreparePackageUpload));
  }

  private void StartProgramming()
  {
    this.IsProgrammingInProgress = true;
    this.SetButtonTextVisibility(true, AppResource.SYMANA_INSTALLATION_STARTED_BUTTON);
    if (this.isDeviceReboot)
    {
      this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSYMANA, "Re-Program button clicked'", memberName: nameof (StartProgramming), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaApplianceFlashViewModel.cs", sourceLineNumber: 292);
      this.ResumeInstallationAfterReboot();
    }
    else
    {
      this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSYMANA, "WebSocket Request Initiated with cmnd 'StartInstallation'", memberName: nameof (StartProgramming), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaApplianceFlashViewModel.cs", sourceLineNumber: 298);
      Task.Run(new Action(this.InstallAllPackages));
    }
  }

  private async Task ClosingPendingInstallations()
  {
    this.SetButtonTextVisibility(true, AppResource.SYMANA_PENDING_INSTALLATIONS_ARE_FINISHED);
    this.IsInitialCheckForInstallation = true;
    this.IsProgrammingInProgress = true;
    this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSYMANA, "Pending installations are finishing...", memberName: nameof (ClosingPendingInstallations), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaApplianceFlashViewModel.cs", sourceLineNumber: 308);
    await this.GetInstallationProgress();
  }

  private async void PreparePackageUpload()
  {
    try
    {
      this.ResetListOfModule();
      this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSYMANA, "upload starts here with preparepackageUpload request", memberName: nameof (PreparePackageUpload), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaApplianceFlashViewModel.cs", sourceLineNumber: 317);
      this.ProgressBarUpdate(1L);
      this.progressValue = 0L;
      foreach (SyMaNaFirmwareUploadModel uploadFirmwareModel in this.uploadFirmwareModels)
      {
        SyMaNaFirmwareUploadModel module = uploadFirmwareModel;
        if (await this.PreparePackageRequest(module))
        {
          if (!string.IsNullOrEmpty(module.uploadURL))
          {
            this._loggingService.getLogger().LogAppDebug(LoggingContext.PROGRAMSYMANA, "Upload started.. ", memberName: nameof (PreparePackageUpload), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaApplianceFlashViewModel.cs", sourceLineNumber: 332);
            this.UploadFirmware(module, 3);
          }
          this.progressValue += module.fileSize;
          module = (SyMaNaFirmwareUploadModel) null;
        }
      }
      Device.BeginInvokeOnMainThread(new Action(this.UploadFirmwareProcessFinished));
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSYMANA, "PreparePackageUploadForBinaries: " + ex.Message, memberName: nameof (PreparePackageUpload), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaApplianceFlashViewModel.cs", sourceLineNumber: 341);
      string errorMsg = AppResource.SYMANA_PACKAGE_UPLOAD_FAILED;
      this.ProgressBarUpdate(this.totalFileSize);
      this._alertService.ShowMessageAlertWithKeyFromService(errorMsg, AppResource.INFORMATION_TEXT, AppResource.RETRY_LABEL, AppResource.CANCEL_LABEL, (Action<bool>) (shouldRetry =>
      {
        if (shouldRetry)
        {
          this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSYMANA, "Not received upload URL for all the Binaries", memberName: nameof (PreparePackageUpload), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaApplianceFlashViewModel.cs", sourceLineNumber: 348);
          this.UploadPackages();
        }
        else
        {
          this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSYMANA, "User selected CANCEL from Popup - NO to Retry PreparePackageRequest", memberName: nameof (PreparePackageUpload), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaApplianceFlashViewModel.cs", sourceLineNumber: 353);
          this.ClosePage();
        }
      }));
      errorMsg = (string) null;
    }
  }

  private async Task<bool> PreparePackageRequest(SyMaNaFirmwareUploadModel module)
  {
    bool success = false;
    try
    {
      this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSYMANA, "\nWebSocket Request Initiated with cmnd 'PreparePackageUpload'", memberName: nameof (PreparePackageRequest), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaApplianceFlashViewModel.cs", sourceLineNumber: 365);
      if (this._webSocketService.SetPPFData(module.ppfFile))
      {
        SyMaNaLodisResponseData preparePackageData = await this._webSocketService.RequestDataFromLodis(SyMaNaRequestCommandName.preparePackageUpload);
        this._webSocketService.ResetPPFData();
        if (preparePackageData.error == null && preparePackageData.resultCode == 0)
        {
          module.uploadURL = preparePackageData.value != null ? preparePackageData.value.parameter : preparePackageData.returnValue.parameter;
          success = true;
        }
        else
          this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSYMANA, $"WebSocket RESPONSE Failed For PreparePackage request: {module.module.ToString()} {module.version}", memberName: nameof (PreparePackageRequest), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaApplianceFlashViewModel.cs", sourceLineNumber: 377);
        preparePackageData = (SyMaNaLodisResponseData) null;
      }
      else
        this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSYMANA, "SetPPFData is failed", memberName: nameof (PreparePackageRequest), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaApplianceFlashViewModel.cs", sourceLineNumber: 381);
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppError(LoggingContext.PROGRAMSYMANA, "Exception in PreparePackage request: " + ex.Message, memberName: nameof (PreparePackageRequest), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaApplianceFlashViewModel.cs", sourceLineNumber: 385);
    }
    return success;
  }

  private async void UploadFirmware(SyMaNaFirmwareUploadModel module, int retryCount)
  {
    if (this.symanaWebClient == null)
    {
      this.symanaWebClient = new SyMaNaWebClient();
      this.symanaWebClient._loggingService = this._loggingService;
      this.symanaWebClient._secureStorageService = this._secureStorageService;
      this.symanaWebClient.trustedThumbprints = this._webSocketService.GetTrustedThumbprints();
      this.symanaWebClient.UploadProgressUpdate += new SyMaNaWebClient.UploadProgressHandler(this.ProgressBarUpdate);
    }
    await this.symanaWebClient.UploadBinaryToLodis(module);
    if (module.response.isSuccess || retryCount <= 0)
      return;
    int retriesLeft = retryCount - 1;
    this._loggingService.getLogger().LogAppDebug(LoggingContext.PROGRAMSYMANA, $"Retry for module {module.module.ToString()} upload", memberName: nameof (UploadFirmware), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaApplianceFlashViewModel.cs", sourceLineNumber: 405);
    this.UploadFirmware(module, retriesLeft);
  }

  private void UploadFirmwareProcessFinished()
  {
    List<SyMaNaFirmwareUploadModel> list = this.uploadFirmwareModels.Where<SyMaNaFirmwareUploadModel>((Func<SyMaNaFirmwareUploadModel, bool>) (model => model.response == null || !model.response.isSuccess)).ToList<SyMaNaFirmwareUploadModel>();
    Device.BeginInvokeOnMainThread(new Action(this.ResetProgressBarValues));
    string str = "";
    if (list.Count == 0)
    {
      this.isDeviceReboot = false;
      Device.BeginInvokeOnMainThread(new Action(this.StartProgramming));
    }
    else
    {
      str = AppResource.SYMANA_BINARY_UPLOAD_RETRY_MESSAGE;
      StringBuilder stringBuilder = new StringBuilder(str);
      stringBuilder.AppendLine();
      stringBuilder.Append(AppResource.SYMANA_FAILED_UPLOAD_PACKAGES_MSG);
      foreach (SyMaNaFirmwareUploadModel firmwareUploadModel in list)
      {
        stringBuilder.AppendLine();
        stringBuilder.Append(firmwareUploadModel.FileName);
      }
      this.showRetryUploadPopup(stringBuilder.ToString());
    }
    this.IsUploadBtnDisabled = false;
    this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSYMANA, str ?? "", memberName: nameof (UploadFirmwareProcessFinished), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaApplianceFlashViewModel.cs", sourceLineNumber: 434);
  }

  private void showRetryUploadPopup(string errorMsg)
  {
    this._alertService.ShowMessageAlertWithKeyFromService(errorMsg, AppResource.INFORMATION_TEXT, AppResource.RETRY_LABEL, AppResource.CANCEL_LABEL, (Action<bool>) (shouldRetry =>
    {
      if (shouldRetry)
      {
        this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSYMANA, "User selected RE-TRY for failed modules upload", memberName: nameof (showRetryUploadPopup), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaApplianceFlashViewModel.cs", sourceLineNumber: 443);
        this.IsUploadBtnDisabled = true;
        if (!this._webSocketService.isConnected())
          return;
        this.UploadPackages();
      }
      else
      {
        this.ClosePage();
        this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSYMANA, "User selected CANCEL from Popup - NO to 'Retry' upload", memberName: nameof (showRetryUploadPopup), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaApplianceFlashViewModel.cs", sourceLineNumber: 453);
      }
    }));
  }

  private async void InstallAllPackages()
  {
    SyMaNaLodisResponseData responseData = await this._webSocketService.RequestDataFromLodis(SyMaNaRequestCommandName.startInstallation);
    if (responseData.error == null)
    {
      if (responseData.resultCode != 0)
      {
        this.ShowPopUpWithClosePage(AppResource.ERROR_TITLE, AppResource.SYMANA_INSTALLATION_ERROR_WITH_WRONG_RESULT_CODE + responseData.resultCode.ToString());
        responseData = (SyMaNaLodisResponseData) null;
      }
      else
      {
        await this.GetInstallationProgress();
        responseData = (SyMaNaLodisResponseData) null;
      }
    }
    else
    {
      this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSYMANA, "Cmnd 'StartInstallation' : " + responseData.error.errorMessage, memberName: nameof (InstallAllPackages), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaApplianceFlashViewModel.cs", sourceLineNumber: 474);
      this.ShowPopUpWithClosePage(AppResource.ERROR_TITLE, responseData.error.errorMessage);
      responseData = (SyMaNaLodisResponseData) null;
    }
  }

  private async Task GetInstallationProgress()
  {
    try
    {
      SyMaNaLodisResponseData data = (SyMaNaLodisResponseData) null;
      string installationState = "";
      this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSYMANA, "WebSocket Request Initiated with cmnd 'getInstallationProgress'", memberName: nameof (GetInstallationProgress), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaApplianceFlashViewModel.cs", sourceLineNumber: 490);
      do
      {
        await Task.Delay(2000);
        data = await this._webSocketService.RequestDataFromLodis(SyMaNaRequestCommandName.getInstallationProgress);
        if (data.error != null || data.resultCode != 0)
        {
          this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSYMANA, "calling HandleInstallationProgressErrors()...", memberName: nameof (GetInstallationProgress), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaApplianceFlashViewModel.cs", sourceLineNumber: 497);
          this.HandleInstallationProgressErrors(data);
          return;
        }
        installationState = data.value != null ? data.value.installationState : data.returnValue.installationState;
      }
      while (installationState.ToUpper() == "IN_PROGRESS");
      if (data.error == null && (data.resultCode == 0 || this.IsInitialCheckForInstallation))
      {
        await this.GetInstallationResult();
      }
      else
      {
        this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSYMANA, "calling HandleInstallationProgressErrors", memberName: nameof (GetInstallationProgress), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaApplianceFlashViewModel.cs", sourceLineNumber: 513);
        this.HandleInstallationProgressErrors(data);
      }
      data = (SyMaNaLodisResponseData) null;
      installationState = (string) null;
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppError(LoggingContext.PROGRAMSYMANA, ex.Message, memberName: nameof (GetInstallationProgress), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaApplianceFlashViewModel.cs", sourceLineNumber: 519);
      SyMaNaLodisResponseData data = new SyMaNaLodisResponseData();
      ServiceError err = new ServiceError(ErrorType.SyMaNa_Websocket_Error, ex.Message, true);
      data.error = err;
      this._loggingService.getLogger().LogAppError(LoggingContext.PROGRAMSYMANA, "calling  HandleInstallationProgressErrors' from catch block", memberName: nameof (GetInstallationProgress), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaApplianceFlashViewModel.cs", sourceLineNumber: 523);
      this.HandleInstallationProgressErrors(data);
      data = (SyMaNaLodisResponseData) null;
      err = (ServiceError) null;
    }
  }

  private void HandleInstallationProgressErrors(SyMaNaLodisResponseData data)
  {
    Task.Run((Action) (() =>
    {
      string message = data.error == null ? "Installation Failed, Please Try Again!!" : data.error.errorMessage;
      if (FeatureConfiguration.LodisTargetType() != TargetLodisType.LocalMOCK)
      {
        if (!this.isWifiConnected)
        {
          this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSYMANA, "Appliance WIFI disconnected", memberName: nameof (HandleInstallationProgressErrors), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaApplianceFlashViewModel.cs", sourceLineNumber: 537);
          this.RebootScreen();
        }
        else
        {
          int millisecondsTimeout = 5000;
          int num = 10;
          for (int index = 0; index < num; ++index)
          {
            Thread.Sleep(millisecondsTimeout);
            this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSYMANA, "Waiting for Wifi to get disappeared after Reboot", memberName: nameof (HandleInstallationProgressErrors), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaApplianceFlashViewModel.cs", sourceLineNumber: 547);
            if (!this.isWifiConnected)
            {
              this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSYMANA, "Device Rebooting...", memberName: nameof (HandleInstallationProgressErrors), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaApplianceFlashViewModel.cs", sourceLineNumber: 550);
              this.RebootScreen();
              return;
            }
          }
          this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSYMANA, "Device Reboot did not happen.", memberName: nameof (HandleInstallationProgressErrors), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaApplianceFlashViewModel.cs", sourceLineNumber: 562);
          this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSYMANA, message, memberName: nameof (HandleInstallationProgressErrors), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaApplianceFlashViewModel.cs", sourceLineNumber: 563);
          this._webSocketService.CloseWebSocket();
          Device.BeginInvokeOnMainThread(new Action(this.ResumeInstallationAfterReboot));
        }
      }
      else
        this.RebootScreen();
    }));
  }

  private void RebootScreen()
  {
    Device.BeginInvokeOnMainThread((Action) (() =>
    {
      this._webSocketService.CloseWebSocket();
      this.isDeviceReboot = true;
      IMvxNavigationService navigationService = this._navigationService;
      DetailNavigationArgs detailNavigationArgs = new DetailNavigationArgs();
      detailNavigationArgs._isSyMaNaReboot = this.IsProgrammingInProgress;
      CancellationToken cancellationToken = new CancellationToken();
      navigationService.Navigate<BootModeTransitionViewModel, DetailNavigationArgs>(detailNavigationArgs, (IMvxBundle) null, cancellationToken);
    }));
  }

  private void ResumeInstallationAfterReboot()
  {
    this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSYMANA, "Resume Installation After reboot..", memberName: nameof (ResumeInstallationAfterReboot), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaApplianceFlashViewModel.cs", sourceLineNumber: 585);
    this.SetButtonTextVisibility(true, "Re-Connecting...");
    try
    {
      Task.Run((Action) (() =>
      {
        bool flag = this.CheckForConnection();
        if (this.isWifiConnected || FeatureConfiguration.LodisTargetType() == TargetLodisType.LocalMOCK)
        {
          if (this.IsProgrammingInProgress & flag)
          {
            this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSYMANA, "Connection Established", memberName: nameof (ResumeInstallationAfterReboot), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaApplianceFlashViewModel.cs", sourceLineNumber: 596);
            this.SetButtonTextVisibility(true, AppResource.SYMANA_PROGRAMMING_CONTINUED);
            this.GetInstallationProgress();
            this.isDeviceReboot = false;
          }
          else
            this.ShowPopUpWithClosePage(AppResource.ERROR_TITLE, AppResource.SYMANA_WEBSOCKET_CONNECTION_INTERRUPTED);
        }
        else
          Device.BeginInvokeOnMainThread((Action) (() =>
          {
            this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSYMANA, "Show Reboot screen because iService is connected to wrong SSID.", memberName: nameof (ResumeInstallationAfterReboot), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaApplianceFlashViewModel.cs", sourceLineNumber: 610);
            this._alertService.ShowMessageAlertWithKey(AppResource.GRAPHIC_PAGE_CONNECT_TEXT, AppResource.INFORMATION_TEXT, AppResource.WARNING_OK, new Action(this.RebootScreen));
          }));
      }));
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppError(LoggingContext.PROGRAMSYMANA, ex.Message, memberName: nameof (ResumeInstallationAfterReboot), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaApplianceFlashViewModel.cs", sourceLineNumber: 618);
      this.ShowPopUpWithClosePage(AppResource.ERROR_TITLE, ex.Message);
    }
  }

  private bool CheckForConnection()
  {
    int num = 100;
    bool flag = false;
    for (int index = 0; index < num; ++index)
    {
      this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSYMANA, $"Connection attempt = {index.ToString()}\nIs Appliance Wifi connected: {this.isWifiConnected.ToString()}", memberName: nameof (CheckForConnection), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaApplianceFlashViewModel.cs", sourceLineNumber: 630);
      if (this.isWifiConnected || FeatureConfiguration.LodisTargetType() == TargetLodisType.LocalMOCK)
      {
        flag = this._webSocketService.isConnected();
        if (!flag)
        {
          Thread.Sleep(6000);
        }
        else
        {
          this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSYMANA, "Connected Successfully", memberName: nameof (CheckForConnection), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaApplianceFlashViewModel.cs", sourceLineNumber: 640);
          break;
        }
      }
      else
      {
        this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSYMANA, "Show Reboot screen because iService is connected to wrong SSID.", memberName: nameof (CheckForConnection), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaApplianceFlashViewModel.cs", sourceLineNumber: 646);
        break;
      }
    }
    return flag;
  }

  private async Task GetInstallationResult()
  {
    this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSYMANA, "WebSocket Request Initiated with cmnd 'getInstallationResult'", memberName: nameof (GetInstallationResult), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaApplianceFlashViewModel.cs", sourceLineNumber: 655);
    SyMaNaLodisResponseData data = await this._webSocketService.RequestDataFromLodis(SyMaNaRequestCommandName.getInstallationResult);
    if (this.IsInitialCheckForInstallation)
    {
      data = (SyMaNaLodisResponseData) null;
    }
    else
    {
      if (data.resultCode == 0 && data.error == null)
      {
        this.PopupWithMsg(AppResource.SYMANA_PROGRAMMING_SUCCESSFUL, AppResource.APPLIANCE_FLASH_PROCESS_FINISHED);
        this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSYMANA, "Cmnd 'getInstallationResult' : PROGRAMMING SUCCESSFULLY COMPLETED", memberName: nameof (GetInstallationResult), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaApplianceFlashViewModel.cs", sourceLineNumber: 663);
        this.IsProgrammingInProgress = false;
      }
      else
      {
        string errorMsg = data.error == null ? AppResource.PROGRAMMING_FAILED_WITH_RESULT_CODE + data.resultCode.ToString() : data.error.errorMessage;
        this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSYMANA, "Cmnd 'getInstallationResult' : " + errorMsg, memberName: nameof (GetInstallationResult), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaApplianceFlashViewModel.cs", sourceLineNumber: 669);
        if (data.resultCode != 0)
          this.PopupWithMsg(errorMsg, AppResource.APPLIANCE_FLASH_PROCESS_FINISHED);
        else
          this.ShowPopUpWithClosePage(AppResource.ERROR_TITLE, errorMsg);
        errorMsg = (string) null;
      }
      Task.Run(new Func<Task>(this.LoadInventoryInfo));
      data = (SyMaNaLodisResponseData) null;
    }
  }

  private void PopupWithMsg(string msg, string buttonText)
  {
    this.SetButtonTextVisibility(false, buttonText);
    this._alertService.ShowMessageAlertWithMessage(msg, AppResource.INFORMATION_TITLE);
  }

  private void SetButtonTextVisibility(bool disableButton, string buttonText)
  {
    Device.BeginInvokeOnMainThread((Action) (() =>
    {
      this.IsBackButtonVisible = !disableButton;
      this.IsUploadBtnDisabled = disableButton;
      this.UploadButtonText = buttonText;
      this.DisplayActivityIndicator = !this.ProgressVisibility && disableButton;
    }));
  }

  private void ShowPopUpWithClosePage(string title, string body)
  {
    Device.BeginInvokeOnMainThread((Action) (() =>
    {
      this._loggingService.getLogger().LogAppDebug(LoggingContext.LOCAL, "SSID : " + this._locator.GetPlatformSpecificService().GetSSID(), memberName: nameof (ShowPopUpWithClosePage), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaApplianceFlashViewModel.cs", sourceLineNumber: 704);
      this._webSocketService.CloseWebSocket();
      this._alertService.ShowMessageAlertWithKey(body, title, AppResource.WARNING_OK, new Action(this.ClosePage));
    }));
  }

  public void OnBackButtonPressed() => this.ClosePage();

  public bool IsBackButtonVisible
  {
    get => this.isBackButtonVisible;
    internal set
    {
      this.isBackButtonVisible = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.IsBackButtonVisible));
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

  public bool ProgressVisibility
  {
    get => this._ProgressVisibility;
    internal set
    {
      this._ProgressVisibility = value;
      if (value)
        this.DisplayActivityIndicator = false;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.ProgressVisibility));
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

  public bool isWifiConnected
  {
    get => this._isWifiConnected;
    private set
    {
      this._isWifiConnected = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.isWifiConnected));
    }
  }

  public string DummyLabel
  {
    get => this._DummyLabel;
    internal set
    {
      this._DummyLabel = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.DummyLabel));
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

  public string ConnectedColor
  {
    get => this._ConnectedColor;
    internal set
    {
      this._ConnectedColor = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.ConnectedColor));
    }
  }

  public string UploadButtonText
  {
    get => this._UploadButtonText;
    internal set
    {
      this._UploadButtonText = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.UploadButtonText));
    }
  }

  public bool IsUploadBtnDisabled
  {
    get => this._IsUploadBtnDisabled;
    internal set
    {
      this._IsUploadBtnDisabled = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.IsUploadBtnDisabled));
    }
  }

  public bool IsUploadButtonVisible
  {
    get => this._IsUploadButtonVisible;
    internal set
    {
      this._IsUploadButtonVisible = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.IsUploadButtonVisible));
    }
  }

  public bool DisplayActivityIndicator
  {
    get => this._DisplayActivityIndicator;
    set
    {
      this._DisplayActivityIndicator = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.DisplayActivityIndicator));
    }
  }

  public override void ViewAppearing()
  {
    base.ViewAppearing();
    this._locator.GetPlatformSpecificService().GetLocationConsent();
    this._appliance.CheckConnectivity(new connectivityUpdateCallback(this.updateStatusBr));
  }

  public override void ViewAppeared()
  {
    Device.BeginInvokeOnMainThread((Action) (() =>
    {
      base.ViewAppeared();
      this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSYMANA, "IsProgrammingInProgress: " + this.IsProgrammingInProgress.ToString(), memberName: nameof (ViewAppeared), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaApplianceFlashViewModel.cs", sourceLineNumber: 897);
      if (this.IsProgrammingInProgress)
        this.ResumeInstallationAfterReboot();
      else
        this.RequestInventory();
    }));
  }

  private async void RequestInventory()
  {
    if (this._moduleList != null && this._moduleList.Count != 0)
      return;
    await this.LoadInventoryInfo();
    this.DisplayActivityIndicator = false;
  }

  private async Task LoadInventoryInfo()
  {
    try
    {
      if (!this._webSocketService.isConnected())
        return;
      SyMaNaLodisResponseData syMaNaLodisResponseData = await this._webSocketService.RequestDataFromLodis(SyMaNaRequestCommandName.getInventory);
      if (syMaNaLodisResponseData != null && syMaNaLodisResponseData.error == null)
      {
        SyMaNaReturnValue returnValue = syMaNaLodisResponseData.returnValue == null ? syMaNaLodisResponseData.value : syMaNaLodisResponseData.returnValue;
        this.MergeInventories(returnValue.EcUs);
        returnValue = (SyMaNaReturnValue) null;
      }
      else
        this.ShowPopUpWithClosePage(syMaNaLodisResponseData.error.errorMessage, syMaNaLodisResponseData.error.errorType.ToString());
      syMaNaLodisResponseData = (SyMaNaLodisResponseData) null;
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppError(LoggingContext.PROGRAMSYMANA, "Exception in LoadInventoryInfo()" + ex.Message, memberName: nameof (LoadInventoryInfo), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaApplianceFlashViewModel.cs", sourceLineNumber: 936);
    }
  }

  private void MergeInventories(SyMaNaECU[] InventoryECUs)
  {
    List<smm_module> modulesDetails = this._metadataService.GetModulesDetails(this.RepairEnumber);
    this._moduleList = SyMaNaUtilityFunctions.CreateModuleList(InventoryECUs, modulesDetails, this._loggingService);
    this.ModuleList = this._moduleList;
    this.DisplayActivityIndicator = false;
    this.IsUploadBtnDisabled = false;
  }

  internal void updateStatusBr()
  {
    this.WifiStatus = this._appliance.StatusOfConnection;
    this.ConnectedColor = this._appliance.ConnectedColorBridge;
    this.isWifiConnected = this._appliance.boolStatusOfConnection;
  }

  private void ResetListOfModule()
  {
    this.uploadFirmwareModels.ForEach((Action<SyMaNaFirmwareUploadModel>) (module =>
    {
      module.uploadURL = (string) null;
      module.response = (SyMaNaWebClientResponse) null;
    }));
  }

  private void ProgressBarUpdate(long progress)
  {
    this.ProgressBarValue = (double) (this.progressValue + progress) / (double) this.totalFileSize;
    this.ProgressBarValue = Math.Truncate(this.ProgressBarValue * 1000.0) / 1000.0;
    this.ProgressChanged(this.ProgressBarValue, 700U);
  }

  private void ResetProgressBarValues()
  {
    this.progressValue = 0L;
    this.ProgressBarValue = 0.0;
    this.ProgressVisibility = false;
    if (this.ProgressChanged == null)
      return;
    this.ProgressChanged(this.ProgressBarValue, 0U);
  }

  public delegate void SyMaNaFlashViewHandler(double progress, uint ms);
}
