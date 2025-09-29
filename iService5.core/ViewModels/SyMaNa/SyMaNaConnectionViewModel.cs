// Decompiled with JetBrains decompiler
// Type: iService5.Core.ViewModels.SyMaNa.SyMaNaConnectionViewModel
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Appliance;
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
using Serilog.Core;
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
namespace iService5.Core.ViewModels.SyMaNa;

public class SyMaNaConnectionViewModel : MvxViewModel<DetailNavigationArgs>
{
  private readonly IMvxNavigationService _navigationService;
  private readonly IUserSession _userSession;
  private readonly IShortTextsService _ShortTextsService = (IShortTextsService) null;
  private readonly IMetadataService _metadataService;
  private readonly IPlatformSpecificServiceLocator _locator;
  private readonly IAlertService _alertService;
  private readonly ILoggingService _loggingService;
  private readonly ISecureStorageService _secureStorageService;
  private readonly IAppliance _appliance;
  private readonly IWebSocketService _webSocketService;
  private List<ppf> ppfsListForEno;
  private List<SyMaNaFirmwareUploadModel> uploadFirmwareModels;
  private List<smm_module> modulesFromDB;
  private SyMaNaECU[] InventoryECUs;
  private bool executeSetHaInfo = true;
  private bool readInventory = false;
  private string _RepairEnumber = nameof (RepairEnumber);
  private string _WifiStatus;
  private string _ConnectedColor;
  private bool _IsProgramBtnDisabled = false;
  private ObservableCollection<SyMaNaHaInfoItems> _items = new ObservableCollection<SyMaNaHaInfoItems>();
  private bool _ProgramInfoVisibility = false;
  private bool _areButtonsEnabled = true;
  private bool _ActivityIndicatorVisible;
  private string _ActivityIndicatorMessage;
  private string _ProgramInfoMessage = "";
  private bool _IsWifiAvailable;
  private string _EnoInfoMessage = "";
  private string _dataSupplyMessage = "";
  private bool _ProgramInfoMsgVisibility;
  private bool _EnumberMismatchInfoVisibility;
  private bool _dataSupplyMsgVisibility;

  public virtual async Task Initialize() => await base.Initialize();

  public SyMaNaConnectionViewModel(
    IMvxNavigationService navigationService,
    IShortTextsService _SService,
    ILoggingService loggingService,
    IMetadataService metadataService,
    IPlatformSpecificServiceLocator locator,
    IUserSession userSession,
    IAlertService alertService,
    ISecureStorageService secureStorageService,
    IAppliance appliance,
    IWebSocketService webSocketService)
  {
    this._ShortTextsService = _SService;
    this._userSession = userSession;
    this.RepairEnumber = this._userSession.getEnumberSession();
    this._metadataService = metadataService;
    this._alertService = alertService;
    this._secureStorageService = secureStorageService;
    this._appliance = appliance;
    this._webSocketService = webSocketService;
    this.GoToHome = (ICommand) new Command(new Action(this.OnBackButtonPressed));
    this.ProgramTapped = (ICommand) new Command((Action) (() => this.Program()));
    this.ReadInventoryTapped = (ICommand) new Command((Action) (() => this.ReadInventory()));
    this._loggingService = loggingService;
    this._navigationService = navigationService;
    this._locator = locator;
    this.Items = new ObservableCollection<SyMaNaHaInfoItems>();
  }

  public ICommand GoToHome { protected set; get; }

  public ICommand ProgramTapped { protected set; get; }

  public ICommand ReadInventoryTapped { protected set; get; }

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

  public string ConnectedColor
  {
    get => this._ConnectedColor;
    private set
    {
      this._ConnectedColor = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.ConnectedColor));
    }
  }

  public bool IsProgramBtnDisabled
  {
    protected set
    {
      if (!value)
        this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSYMANA, "Program button enabled", memberName: nameof (IsProgramBtnDisabled), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaConnectionViewModel.cs", sourceLineNumber: 139);
      this._IsProgramBtnDisabled = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.IsProgramBtnDisabled));
    }
    get => this._IsProgramBtnDisabled;
  }

  public ObservableCollection<SyMaNaHaInfoItems> Items
  {
    get => this._items;
    private set
    {
      this._items = value;
      this.RaisePropertyChanged<ObservableCollection<SyMaNaHaInfoItems>>((Expression<Func<ObservableCollection<SyMaNaHaInfoItems>>>) (() => this.Items));
    }
  }

  public bool ProgramInfoVisibility
  {
    get => this._ProgramInfoVisibility;
    private set
    {
      this._ProgramInfoVisibility = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.ProgramInfoVisibility));
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

  public bool ActivityIndicatorVisible
  {
    get => this._ActivityIndicatorVisible;
    internal set
    {
      this._ActivityIndicatorVisible = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.ActivityIndicatorVisible));
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

  public string ProgramInfoMessage
  {
    get => this._ProgramInfoMessage;
    internal set
    {
      this._ProgramInfoMessage = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.ProgramInfoMessage));
    }
  }

  public bool IsWifiAvailable
  {
    get => this._IsWifiAvailable;
    private set
    {
      this._IsWifiAvailable = !value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.IsWifiAvailable));
    }
  }

  public string EnoInfoMessage
  {
    get => this._EnoInfoMessage;
    internal set
    {
      this._EnoInfoMessage = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.EnoInfoMessage));
    }
  }

  public string DataSupplyMessage
  {
    get => this._dataSupplyMessage;
    internal set
    {
      this._dataSupplyMessage = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.DataSupplyMessage));
    }
  }

  public bool ProgramInfoMsgVisibility
  {
    get => this._ProgramInfoMsgVisibility;
    private set
    {
      this._ProgramInfoMsgVisibility = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.ProgramInfoMsgVisibility));
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

  public bool DataSupplyMsgVisibility
  {
    get => this._dataSupplyMsgVisibility;
    private set
    {
      this._dataSupplyMsgVisibility = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.DataSupplyMsgVisibility));
    }
  }

  public override void ViewAppearing()
  {
    base.ViewAppearing();
    this._locator.GetPlatformSpecificService().GetLocationConsent();
    this._appliance.CheckConnectivity(new connectivityUpdateCallback(this.updateStatusBr));
    this.CheckWebSocketConnection();
  }

  private void LoadData()
  {
    this.CheckDependenciesForProgramming();
    this.LoadHaInfoAsync();
    if (this._items != null)
      this._items = (ObservableCollection<SyMaNaHaInfoItems>) null;
    this._items = new ObservableCollection<SyMaNaHaInfoItems>();
    this.AreButtonsEnabled = true;
  }

  private void CheckWebSocketConnection()
  {
    if (UtilityFunctions.IsLocalNetworkPermissionNeeded() && CoreApp.settings.GetItem("localNetworkPopupAppeared").Value == "false")
    {
      this._webSocketService.isConnected();
      CoreApp.settings.UpdateItem(new Settings("localNetworkPopupAppeared", "true"));
    }
    else if (!this._webSocketService.isConnected())
    {
      this.ConnectionErrorAlerts();
    }
    else
    {
      if (!this._webSocketService.isConnected())
        return;
      this.LoadData();
    }
  }

  private void ConnectionErrorAlerts()
  {
    if (UtilityFunctions.IsLocalNetworkPermissionNeeded() && CoreApp.settings.GetItem("localNetworkPopupAppeared").Value == "true" && this._webSocketService.IsNetworksPermissionError())
      this._alertService.ShowMessageAlertWithKeyFromService("HA_INFO_RETRY_MESSAGE_IOS_14", AppResource.INFORMATION_TEXT, AppResource.RETRY_LABEL, AppResource.CANCEL_LABEL, new Action<bool>(this.InfoRetryCallback));
    else if (DeviceInfo.Platform == DevicePlatform.Android && Connectivity.ConnectionProfiles.Contains<ConnectionProfile>(ConnectionProfile.Cellular))
      this._alertService.ShowMessageAlertWithKeyFromService("HA_INFO_RETRY_MESSAGE_MOBILE_DATA_ENABLED", AppResource.INFORMATION_TEXT, AppResource.RETRY_LABEL, AppResource.CANCEL_LABEL, new Action<bool>(this.InfoRetryCallback));
    else
      this._alertService.ShowMessageAlertWithKeyFromService("HA_INFO_RETRY_MESSAGE", AppResource.INFORMATION_TEXT, AppResource.RETRY_LABEL, AppResource.CANCEL_LABEL, new Action<bool>(this.InfoRetryCallback));
  }

  internal void InfoRetryCallback(bool shouldRetry)
  {
    if (!shouldRetry)
      return;
    this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Retrying to get data from home appliance.", memberName: nameof (InfoRetryCallback), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaConnectionViewModel.cs", sourceLineNumber: 381);
    this.CheckWebSocketConnection();
  }

  internal void updateStatusBr()
  {
    this.WifiStatus = this._appliance.StatusOfConnection;
    this.ConnectedColor = this._appliance.ConnectedColorBridge;
    this.IsWifiAvailable = this._appliance.boolStatusOfConnection || FeatureConfiguration.LodisTargetType() == TargetLodisType.LocalMOCK;
  }

  private void CheckDependenciesForProgramming()
  {
    this.IsProgramBtnDisabled = true;
    this.ProgressbarVisibility(true, AppResource.CONN_PAGE_INITIALISING_UI);
    this.InfoLabelForProgramVisibility(false, "");
    this.CheckPPFAndModulesAvailability();
  }

  private void InfoLabelForProgramVisibility(bool show, string msg)
  {
    this.ProgramInfoMsgVisibility = show;
    this.ProgramInfoMessage = msg;
  }

  private void SetProgramButtonVisibility()
  {
    this.IsProgramBtnDisabled = this.EnumberMismatchInfoVisibility || this.ProgramInfoMsgVisibility || this.DataSupplyMsgVisibility;
  }

  private void ProgressbarVisibility(bool ShowProgressBar, string progressBarMsg)
  {
    this.ActivityIndicatorMessage = progressBarMsg;
    this.ActivityIndicatorVisible = ShowProgressBar;
  }

  private void CheckPPFAndModulesAvailability()
  {
    bool show = false;
    string msg = "";
    if (this.modulesFromDB == null)
    {
      this.modulesFromDB = this._metadataService.GetModulesDetails(this.RepairEnumber);
      if (this.modulesFromDB.Count > 0)
      {
        StringBuilder stringBuilder = new StringBuilder();
        this.uploadFirmwareModels = SyMaNaUtilityFunctions.GetUploadFirmwareModels(this.modulesFromDB, this._loggingService, this._metadataService);
        if (this.uploadFirmwareModels.Count < this.modulesFromDB.Count)
        {
          show = true;
          stringBuilder.Append(AppResource.CONN_PAGE_FLASHING_DISABLED_MISSINGBINARY);
          this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSYMANA, $"Modules in DB: {this.modulesFromDB.Count.ToString()} and Downloaded modules: {this.uploadFirmwareModels.Count.ToString()}", memberName: nameof (CheckPPFAndModulesAvailability), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaConnectionViewModel.cs", sourceLineNumber: 434);
          this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSYMANA, AppResource.CONN_PAGE_FLASHING_DISABLED_MISSINGBINARY, memberName: nameof (CheckPPFAndModulesAvailability), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaConnectionViewModel.cs", sourceLineNumber: 435);
        }
        Tuple<string, string> vibAndKi = UtilityFunctions.getVibAndKi(this.RepairEnumber);
        this.ppfsListForEno = SyMaNaUtilityFunctions.GetPPFDataForEnumber(this.RepairEnumber, this._metadataService);
        if (!SyMaNaUtilityFunctions.SetPPFDataForAllTheModules(this.uploadFirmwareModels, vibAndKi, this.ppfsListForEno, this._loggingService))
        {
          if (show)
            stringBuilder.AppendLine();
          else
            show = true;
          stringBuilder.Append(AppResource.CONN_PAGE_FLASHING_DISABLED_PPF_NOT_AVAILABLE);
        }
        msg = stringBuilder.ToString();
      }
      else
      {
        msg = AppResource.NO_MODULES_FOUND_IN_DB;
        show = true;
      }
    }
    this.InfoLabelForProgramVisibility(show, msg);
    this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSYMANA, $"PPF & module missing Notification: {show.ToString()}{msg}", memberName: nameof (CheckPPFAndModulesAvailability), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaConnectionViewModel.cs", sourceLineNumber: 461);
    this.ProgressbarVisibility(false, "");
    this.SetProgramButtonVisibility();
  }

  public void OnBackButtonPressed()
  {
    this._webSocketService.CloseWebSocket();
    this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
  }

  private async void LoadHaInfoAsync()
  {
    try
    {
      if (!this._userSession.CheckCertificateValidity())
        return;
      SyMaNaLodisResponseData syMaNaLodisResponseData = await this._webSocketService.RequestDataFromLodis(SyMaNaRequestCommandName.getHaInfo);
      if (syMaNaLodisResponseData != null && syMaNaLodisResponseData.error == null && syMaNaLodisResponseData.resultCode == 0)
      {
        SyMaNaReturnValue haInfoData = syMaNaLodisResponseData.returnValue == null ? syMaNaLodisResponseData.value : syMaNaLodisResponseData.returnValue;
        string countryName = string.IsNullOrEmpty(haInfoData.countrySettings) ? "" : UtilityFunctions.GetCountryNameBasedOnSettings(haInfoData.getCountrySettings());
        string enumber = $"{haInfoData.vib}/{haInfoData.customerIndex}";
        string timeStampString = UtilityFunctions.getTimeStampForHAInfo(haInfoData.manufacturingTimeStamp);
        this._items.Add(new SyMaNaHaInfoItems()
        {
          Name = AppResource.SMM_CONN_BRAND,
          Data = haInfoData.brand.ToUpper()
        });
        this._items.Add(new SyMaNaHaInfoItems()
        {
          Name = AppResource.SMM_CONN_COUNTRY_SETTINGS,
          Data = countryName
        });
        this._items.Add(new SyMaNaHaInfoItems()
        {
          Name = AppResource.SMM_CONN_E_NUMBER,
          Data = enumber
        });
        this._items.Add(new SyMaNaHaInfoItems()
        {
          Name = AppResource.SMM_CONN_DEVICE_TYPE,
          Data = haInfoData.deviceType
        });
        this._items.Add(new SyMaNaHaInfoItems()
        {
          Name = AppResource.SMM_CONN_MANUF_TIME,
          Data = timeStampString
        });
        this.Items = this._items;
        this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSYMANA, $"Eno from HAInfo: {enumber} and Repair Eno: {this.RepairEnumber}", memberName: nameof (LoadHaInfoAsync), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaConnectionViewModel.cs", sourceLineNumber: 495);
        this.SetExecuteHAInfo(this.RepairEnumber, enumber);
        this.RequestInventoryToLodis(haInfoData.vib);
        this.SetProgramButtonVisibility();
        haInfoData = (SyMaNaReturnValue) null;
        countryName = (string) null;
        enumber = (string) null;
        timeStampString = (string) null;
      }
      else
        this.showPopUp(syMaNaLodisResponseData.error.errorMessage, syMaNaLodisResponseData.error.errorType.ToString());
      syMaNaLodisResponseData = (SyMaNaLodisResponseData) null;
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppError(LoggingContext.PROGRAMSYMANA, ex.Message, memberName: nameof (LoadHaInfoAsync), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaConnectionViewModel.cs", sourceLineNumber: 508);
    }
  }

  private void RequestInventoryToLodis(string enumber)
  {
    try
    {
      bool isEmptySMM = false;
      this.InventoryECUs = (SyMaNaECU[]) null;
      this.EnumberMismatchInfoVisibility = false;
      this.DataSupplyMsgVisibility = false;
      Task.Run((Func<Task>) (async () =>
      {
        if (this._webSocketService.isConnected())
        {
          SyMaNaLodisResponseData syMaNaLodisResponseData = await this._webSocketService.RequestDataFromLodis(SyMaNaRequestCommandName.getInventory);
          if (syMaNaLodisResponseData != null && syMaNaLodisResponseData.error == null)
          {
            SyMaNaReturnValue returnValue = syMaNaLodisResponseData.returnValue == null ? syMaNaLodisResponseData.value : syMaNaLodisResponseData.returnValue;
            this.InventoryECUs = returnValue.EcUs;
            List<SyMaNaECU> syMaNaECUInfoData = ((IEnumerable<SyMaNaECU>) this.InventoryECUs).Where<SyMaNaECU>((Func<SyMaNaECU, bool>) (ecu => ecu.name.ToUpper() == "SMM")).ToList<SyMaNaECU>();
            if (syMaNaECUInfoData != null && syMaNaECUInfoData.Count > 0)
            {
              List<FirmwareModule> firmwares = syMaNaECUInfoData.First<SyMaNaECU>().firmware;
              Tuple<string, string> repairEnoVIB = UtilityFunctions.getVibAndKi(this.RepairEnumber);
              bool isEnoSame = repairEnoVIB.Item1 == enumber;
              this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Inventory Firmware count is: " + firmwares.Count.ToString(), memberName: nameof (RequestInventoryToLodis), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaConnectionViewModel.cs", sourceLineNumber: 535);
              if (!isEnoSame)
              {
                isEmptySMM = firmwares.Count < 6 && !isEnoSame;
                this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "isEmptySMM: " + isEmptySMM.ToString(), memberName: nameof (RequestInventoryToLodis), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaConnectionViewModel.cs", sourceLineNumber: 540);
                if (!isEmptySMM)
                {
                  this.EnumberMismatchInfoVisibility = true;
                  this.EnoInfoMessage = AppResource.APPLIANCE_FLASH_PROGRAM_NOT_YET;
                }
              }
              else if (!this.ModulesMatchInventory(firmwares))
              {
                this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSYMANA, AppResource.CONN_PAGE_PROGRAMMING_DISABLED_DATA_SUPPLY, memberName: nameof (RequestInventoryToLodis), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaConnectionViewModel.cs", sourceLineNumber: 551);
                this.DataSupplyMsgVisibility = true;
                this.DataSupplyMessage = AppResource.CONN_PAGE_PROGRAMMING_DISABLED_DATA_SUPPLY;
              }
              firmwares = (List<FirmwareModule>) null;
              repairEnoVIB = (Tuple<string, string>) null;
            }
            else
              this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSYMANA, "There is no data in Inventory ECUs", memberName: nameof (RequestInventoryToLodis), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaConnectionViewModel.cs", sourceLineNumber: 558);
            returnValue = (SyMaNaReturnValue) null;
            syMaNaECUInfoData = (List<SyMaNaECU>) null;
          }
          else
            this._loggingService.getLogger().LogAppError(LoggingContext.PROGRAMSYMANA, "Exception while parsing Inventory Data: " + syMaNaLodisResponseData.error.errorMessage, memberName: nameof (RequestInventoryToLodis), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaConnectionViewModel.cs", sourceLineNumber: 561);
          syMaNaLodisResponseData = (SyMaNaLodisResponseData) null;
        }
        this.SetProgramButtonVisibility();
      }));
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppError(LoggingContext.PROGRAMSYMANA, "Exception in Fetching Inventory Data: " + ex.Message, memberName: nameof (RequestInventoryToLodis), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaConnectionViewModel.cs", sourceLineNumber: 568);
    }
  }

  private void SetExecuteHAInfo(string repairENumber, string eNumber)
  {
    if (!string.Equals(repairENumber, eNumber))
      return;
    this.executeSetHaInfo = false;
  }

  private bool ModulesMatchInventory(List<FirmwareModule> inventoryFirmwares)
  {
    Logger logger = this._loggingService.getLogger();
    int count = inventoryFirmwares.Count;
    string str1 = count.ToString();
    count = this.uploadFirmwareModels.Count;
    string str2 = count.ToString();
    string message = $"Inventory Modules Count: {str1} & DB Module Count: {str2}";
    logger.LogAppInformation(LoggingContext.PROGRAMSYMANA, message, memberName: nameof (ModulesMatchInventory), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaConnectionViewModel.cs", sourceLineNumber: 581);
    if (this.uploadFirmwareModels.Count < inventoryFirmwares.Count)
      return false;
    foreach (FirmwareModule inventoryFirmware in inventoryFirmwares)
    {
      FirmwareModule firmware = inventoryFirmware;
      this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSYMANA, "Inventory module: " + firmware.firmwareId.ToString(), memberName: nameof (ModulesMatchInventory), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaConnectionViewModel.cs", sourceLineNumber: 587);
      if (!this.uploadFirmwareModels.Any<SyMaNaFirmwareUploadModel>((Func<SyMaNaFirmwareUploadModel, bool>) (x => x.module == (long) firmware.firmwareId)))
      {
        this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSYMANA, "Module mismatch found!!", memberName: nameof (ModulesMatchInventory), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaConnectionViewModel.cs", sourceLineNumber: 590);
        return false;
      }
    }
    return true;
  }

  private void showPopUp(string title, string body)
  {
    this._alertService.ShowMessageAlertWithKey(body, title);
  }

  private void GoToProgramPage()
  {
    if (!this._webSocketService.isConnected())
      return;
    IMvxNavigationService navigationService = this._navigationService;
    DetailNavigationArgs detailNavigationArgs = new DetailNavigationArgs();
    detailNavigationArgs.UploadFirmwareModels = this.uploadFirmwareModels;
    detailNavigationArgs.InventoryECUs = this.InventoryECUs;
    detailNavigationArgs.readInventory = this.readInventory;
    detailNavigationArgs.shouldSetHaInfo = this.executeSetHaInfo;
    CancellationToken cancellationToken = new CancellationToken();
    navigationService.Navigate<SyMaNaApplianceFlashViewModel, DetailNavigationArgs>(detailNavigationArgs, (IMvxBundle) null, cancellationToken);
  }

  private void Program()
  {
    this.readInventory = false;
    this.GoToProgramPage();
  }

  private void ReadInventory()
  {
    this.readInventory = true;
    this.GoToProgramPage();
  }

  public override void Prepare(DetailNavigationArgs parameter)
  {
    throw new NotImplementedException();
  }
}
