// Decompiled with JetBrains decompiler
// Type: iService5.Core.ViewModels.SyMaNa.SyMaNaApplianceInstructionConnectionViewModel
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
using iService5.Core.Services.WebSocket;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
//using Xamarin.Forms;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

#nullable disable
namespace iService5.Core.ViewModels.SyMaNa;

public class SyMaNaApplianceInstructionConnectionViewModel : MvxViewModel<DetailNavigationArgs>
{
  private readonly IMvxNavigationService _navigationService;
  private readonly IUserSession _userSession;
  private readonly IMetadataService _metadataService;
  private readonly IAppliance _appliance;
  private readonly IAlertService _alertService;
  private readonly IWebSocketService _webSocketService;
  private readonly ILoggingService _loggingService;
  private const string UNDERLINE = "Underline";
  private const string NONE = "None";
  private readonly IPlatformSpecificServiceLocator _locator;
  private List<ApplianceInstruction> _InstructionList = new List<ApplianceInstruction>();
  private ApplianceInstruction _ItemSelected;
  private ushort _BiometricTriesRemain = 3;
  private bool _LoginFailed = false;
  private bool _EnableLoggedIn = false;
  private string _RepairEnumber = nameof (RepairEnumber);
  private string _ConnectButtonLabel = AppResource.CONNECT_BTN_LABEL;
  private string _WifiStatus;
  private string _ConnectedColor = "Green";
  private bool _IsWifiAvailable;
  private bool displayActivityIndicator = false;

  public virtual async Task Initialize() => await base.Initialize();

  public string _ConnectionGraphic { get; internal set; }

  public SyMaNaApplianceInstructionConnectionViewModel(
    IMvxNavigationService navigationService,
    IShortTextsService _SService,
    ILoggingService loggingService,
    IMetadataService metadataService,
    IPlatformSpecificServiceLocator locator,
    IUserSession userSession,
    IAlertService alertService,
    IAppliance appliance,
    IWebSocketService webSocketService)
  {
    this._userSession = userSession;
    this._RepairEnumber = this._userSession.getEnumberSession();
    this._metadataService = metadataService;
    this._appliance = appliance;
    this._alertService = alertService;
    this._loggingService = loggingService;
    this._webSocketService = webSocketService;
    this._ConnectionGraphic = this._metadataService.getConnectionGraphic(this._RepairEnumber);
    if (this._ConnectionGraphic != null)
      loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "File to use for connection instructions " + this._ConnectionGraphic, memberName: ".ctor", sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaApplianceInstructionConnectionViewModel.cs", sourceLineNumber: 57);
    else
      loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "Failed to get file to use for connection instructions", memberName: ".ctor", sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaApplianceInstructionConnectionViewModel.cs", sourceLineNumber: 61);
    this.GoToHome = (ICommand) new Command((Action) (async () => await this.VisitHomePage()));
    this.GoToSmm = (ICommand) new Command(new Action(this.VisitConnectionPage));
    this._navigationService = navigationService;
    this._locator = locator;
    this.InstructionList = this.GetInstructionList();
    this.IsWifiAvailable = false;
  }

  private List<ApplianceInstruction> GetInstructionList()
  {
    List<ApplianceInstruction> instructionList = new List<ApplianceInstruction>();
    ApplianceInstruction applianceInstruction1 = new ApplianceInstruction("1", AppResource.CONN_PAGE_TEXT1, "None", (string) null, ActionNames.None);
    ApplianceInstruction applianceInstruction2 = new ApplianceInstruction("2", AppResource.CONN_PAGE_TEXT2, this._ConnectionGraphic != null ? "Underline" : "None", (string) null, ActionNames.VisitGraphics);
    ApplianceInstruction applianceInstruction3 = new ApplianceInstruction("3", $"{AppResource.CONN_PAGE_GOTO} {AppResource.CONN_PAGE_SETTINGS} {AppResource.CONN_PAGE_CHOOSE_BSH_NET}", "Underline", AppResource.CONN_PAGE_SETTINGS, ActionNames.VisitWifiSettings);
    ApplianceInstruction applianceInstruction4 = new ApplianceInstruction("4", AppResource.CONN_PAGE_ENTER_WIFI_PASS, "Underline", (string) null, ActionNames.VisitWifiInfo);
    ApplianceInstruction applianceInstruction5 = new ApplianceInstruction("5", AppResource.CONN_PAGE_WAIT_FOR_CONNECTION, "None", (string) null, ActionNames.None);
    instructionList.Add(applianceInstruction1);
    instructionList.Add(applianceInstruction2);
    instructionList.Add(applianceInstruction3);
    instructionList.Add(applianceInstruction4);
    instructionList.Add(applianceInstruction5);
    return instructionList;
  }

  public List<ApplianceInstruction> InstructionList
  {
    get => this._InstructionList;
    set
    {
      this._InstructionList = value;
      this.RaisePropertyChanged<List<ApplianceInstruction>>((Expression<Func<List<ApplianceInstruction>>>) (() => this.InstructionList));
    }
  }

  public ApplianceInstruction ObjItemSelected
  {
    get => this._ItemSelected;
    set
    {
      if (this._ItemSelected == value)
        return;
      this._ItemSelected = value;
      Logger logger = this._loggingService.getLogger();
      ActionNames action = this._ItemSelected.action;
      string message = "Item selected: " + action.ToString();
      logger.LogAppInformation(LoggingContext.LOCAL, message, memberName: nameof (ObjItemSelected), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaApplianceInstructionConnectionViewModel.cs", sourceLineNumber: 115);
      action = this._ItemSelected.action;
      Console.WriteLine(action.ToString());
      try
      {
        switch (this._ItemSelected.action)
        {
          case ActionNames.VisitGraphics:
            this.VisitGraphics();
            break;
          case ActionNames.VisitWifiSettings:
            this.VisitWifiSettings();
            break;
          case ActionNames.VisitWifiInfo:
            this.VisitWifiInfo();
            break;
        }
      }
      catch (Exception ex)
      {
        this._loggingService.getLogger().LogAppError(LoggingContext.LOCAL, "Exception " + ex.Message, memberName: nameof (ObjItemSelected), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaApplianceInstructionConnectionViewModel.cs", sourceLineNumber: 136);
      }
      this.RaisePropertyChanged<ApplianceInstruction>((Expression<Func<ApplianceInstruction>>) (() => this.ObjItemSelected));
    }
  }

  private void VisitConnectionPage()
  {
    this.DisplayActivityIndicator = true;
    this.CheckWebSocketConnection(true);
  }

  internal void InfoRetryCallback(bool shouldRetry)
  {
    if (!shouldRetry)
      return;
    this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Retrying to get data from home appliance.", memberName: nameof (InfoRetryCallback), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaApplianceInstructionConnectionViewModel.cs", sourceLineNumber: 152);
    this.CheckWebSocketConnection(true);
  }

  private async Task VisitGraphics()
  {
    if (this._ConnectionGraphic == null)
      return;
    int num = await this._navigationService.Navigate<GraphicWiFiActivationViewModel>((IMvxBundle) null, new CancellationToken()) ? 1 : 0;
  }

  private void VisitWifiSettings() => this._locator.GetPlatformSpecificService().OpenWifiSettings();

  internal async Task VisitWifiInfo()
  {
    if (this.EnableLoggedIn)
    {
      IPlatformSpecificService current = this._locator.GetPlatformSpecificService();
      current.isFingerprintValid(new iService5.Core.Services.Platform.fingerprintCompletionCallback(this.autoLoginFingerprintCompletionCallback));
      current = (IPlatformSpecificService) null;
    }
    else
    {
      int num = await this._navigationService.Navigate<ValidationViewModel, string>("SMM", (IMvxBundle) null, new CancellationToken()) ? 1 : 0;
    }
  }

  public async Task autoLoginFingerprintCompletionCallback(FingerprintVerification res)
  {
    this._loggingService.getLogger().LogAppInformation(LoggingContext.LOGIN, $"Called with {res.ToString()} remaining {this.BiometricTriesRemain.ToString()} tries left", memberName: nameof (autoLoginFingerprintCompletionCallback), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaApplianceInstructionConnectionViewModel.cs", sourceLineNumber: 191);
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

  public async Task fingerprintCompletionCallback(FingerprintVerification res)
  {
    this._loggingService.getLogger().LogAppInformation(LoggingContext.LOGIN, "Called with " + res.ToString(), memberName: nameof (fingerprintCompletionCallback), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaApplianceInstructionConnectionViewModel.cs", sourceLineNumber: 240 /*0xF0*/);
    if (res != FingerprintVerification.ERROR && res != FingerprintVerification.SUCCESS && res != FingerprintVerification.IOS_FAILURE)
      return;
    int num = await this._navigationService.Navigate<WifiInfoViewModel, string>("SMM", (IMvxBundle) null, new CancellationToken()) ? 1 : 0;
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

  private async Task VisitHomePage()
  {
    if (this._userSession.GetSenderScreen().Equals(AppResource.RE_DOWNLOAD_ENUMBER_VIEW))
    {
      int num1 = await this._navigationService.Navigate<TabViewModel>((IMvxBundle) null, new CancellationToken()) ? 1 : 0;
    }
    else
    {
      int num2 = await this._navigationService.Close((IMvxViewModel) this, new CancellationToken()) ? 1 : 0;
    }
  }

  public ICommand GoToHome { internal set; get; }

  public ICommand GoToSmm { internal set; get; }

  public string RepairEnumber
  {
    get => this._RepairEnumber;
    internal set
    {
      this._RepairEnumber = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.RepairEnumber));
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

  public bool IsWifiAvailable
  {
    get => this._IsWifiAvailable;
    private set
    {
      this._IsWifiAvailable = !value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.IsWifiAvailable));
    }
  }

  public bool DisplayActivityIndicator
  {
    get => this.displayActivityIndicator;
    internal set
    {
      this.displayActivityIndicator = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.DisplayActivityIndicator));
    }
  }

  public override void ViewAppeared()
  {
    base.ViewAppeared();
    this._locator.GetPlatformSpecificService().GetLocationConsent();
    this._appliance.CheckConnectivity(new connectivityUpdateCallback(this.updateStatusBr));
  }

  private void CheckWebSocketConnection(bool fromConnectBtn)
  {
    try
    {
      this.DisplayActivityIndicator = false;
      if (UtilityFunctions.IsLocalNetworkPermissionNeeded() && CoreApp.settings.GetItem("localNetworkPopupAppeared").Value == "false")
      {
        this._webSocketService.isConnected();
        CoreApp.settings.UpdateItem(new Settings("localNetworkPopupAppeared", "true"));
      }
      else
      {
        bool flag = this._webSocketService.isConnected();
        if (!flag)
        {
          if (UtilityFunctions.IsLocalNetworkPermissionNeeded() && CoreApp.settings.GetItem("localNetworkPopupAppeared").Value == "true" && this._webSocketService.IsNetworksPermissionError())
            this._alertService.ShowMessageAlertWithKeyFromService("HA_INFO_RETRY_MESSAGE_IOS_14", AppResource.INFORMATION_TEXT, AppResource.RETRY_LABEL, AppResource.CANCEL_LABEL, new Action<bool>(this.InfoRetryCallback));
          else
            this._alertService.ShowMessageAlertWithKey(AppResource.WEBSOCKET_CONN_NOT_INITIALIZED, AppResource.WARNING_TEXT);
        }
        else if (flag & fromConnectBtn)
          Device.BeginInvokeOnMainThread((Action) (async () =>
          {
            await this._alertService.ShowMessageAlertWithKey(AppResource.WEBSOCKET_CONN_INITIALIZED, AppResource.INFORMATION_TEXT);
            int num = await this._navigationService.Navigate<SyMaNaConnectionViewModel>((IMvxBundle) null, new CancellationToken()) ? 1 : 0;
          }));
        else
          this._loggingService.getLogger().LogAppInformation(LoggingContext.LOCAL, $"Reaching to else condition with isConnected Status:{flag.ToString()} And fromConnectBtn:{fromConnectBtn.ToString()}", memberName: nameof (CheckWebSocketConnection), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaApplianceInstructionConnectionViewModel.cs", sourceLineNumber: 410);
      }
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppInformation(LoggingContext.LOCAL, "Exception:" + ex.Message, memberName: nameof (CheckWebSocketConnection), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/SyMaNa/SyMaNaApplianceInstructionConnectionViewModel.cs", sourceLineNumber: 415);
    }
  }

  internal void updateStatusBr()
  {
    this.WifiStatus = this._appliance.StatusOfConnection;
    this.ConnectedColor = this._appliance.ConnectedColorBridge;
    this.IsWifiAvailable = this._appliance.boolStatusOfConnection || FeatureConfiguration.LodisTargetType() == TargetLodisType.LocalMOCK;
  }

  public void OnBackButtonPressed() => throw new NotImplementedException();

  public override void Prepare(DetailNavigationArgs parameter)
  {
    throw new NotImplementedException();
  }
}
