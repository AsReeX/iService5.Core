// Decompiled with JetBrains decompiler
// Type: iService5.Core.ViewModels.BridgeSettingsViewModel
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Appliance;
using iService5.Core.Services.Data;
using iService5.Core.Services.Helpers;
using iService5.Core.Services.Platform;
using iService5.Core.Services.User.ActivityLogging;
using iService5.Ssh.DTO;
using iService5.Ssh.enums;
using iService5.Ssh.models;
using MvvmCross;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using Newtonsoft.Json;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
//using Xamarin.Forms;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

#nullable disable
namespace iService5.Core.ViewModels;

public class BridgeSettingsViewModel : MvxViewModel<DetailNavigationArgs, DetailReturnArgs>
{
  private readonly Is5SshWrapper _sshWrapper;
  private readonly IMvxNavigationService _navigationService;
  private readonly IAppliance _appliance;
  private readonly IAlertService _alertService;
  private readonly ILoggingService _loggingService;
  private readonly IMetadataService _metadataService;
  internal Dictionary<string, string> ChangedSettings = new Dictionary<string, string>();
  internal bool ReturnToSettingsPage;
  internal List<BridgeSettingUI> UiSettingsList = new List<BridgeSettingUI>();
  internal bool initialSetup = true;
  internal double oldTestTimeoutSliderValue;
  internal bool testTimeoutChanged;
  internal string oldCallibrationOption;
  internal bool callibrationChanged;
  internal List<string> CallibrationOptions = new List<string>()
  {
    AppResource.CALLIBRATION_MANUALLY,
    AppResource.CALLIBRATION_EVERY_TIME,
    AppResource.CALLIBRATION_ONCE_A_DAY,
    AppResource.CALLIBRATION_ONCE_A_WEEK
  };
  private List<BridgeSettingDto> initialUISettingsList = new List<BridgeSettingDto>();
  private ObservableCollection<BridgeSettingsViewModel.BridgeSettingsGroup> _GroupedSettingsList;
  private string _NavText = AppResource.APPLIANCE_FLASH_BACK_TEXT;
  private string _Title;
  private string _WifiStatus;
  private string _ConnectedColor;
  private bool displayActivityIndicator = true;
  private string _SaveButtonText = AppResource.BRIDGE_SETTINGS_SAVE;
  private bool _IsBtnDisabled = true;
  private string _AdvancedToggleText = AppResource.BRIDGE_SETTINGS_ADVANCED_TOGGLE_TEXT;
  private bool _IsAdvancedEnabled = false;
  private bool _BridgeSettingsMode;
  private bool _ShouldShowAdvanced;
  private string _TestTimeoutLabel = AppResource.TEST_TIMEOUT_LABEL;
  private double _TestTimeoutSliderValue;
  private string _CallibrationIntervalLabel = AppResource.CALLIBRATION_INTERVAL_LABEL;
  private string _SelectedCallibrationOption;

  public ICommand GoBackCommand { protected set; get; }

  public ICommand OpenSelectionCommand { protected set; get; }

  public ICommand CallibrationIntervalTapCommand { protected set; get; }

  public ICommand SaveSettingsCommand { protected set; get; }

  public ICommand WiFiBridgeSettingsCommand { internal set; get; }

  public override void Prepare(DetailNavigationArgs parameter)
  {
    this.ReturnToSettingsPage = parameter.ReturnToSettingsPage;
    this.BridgeSettingsMode = parameter.bridgeSettingsSelected;
    this.ShouldShowAdvanced = this.BridgeSettingsMode;
  }

  public BridgeSettingsViewModel(
    Is5SshWrapper sshWrapper,
    IPlatformSpecificServiceLocator locator,
    IMvxNavigationService navigationService,
    IAppliance appliance,
    IAlertService alertService,
    ILoggingService loggingService,
    IMetadataService metadataService)
  {
    this._sshWrapper = sshWrapper;
    this._navigationService = navigationService;
    this._appliance = appliance;
    this._alertService = alertService;
    this._loggingService = loggingService;
    this._sshWrapper.IPAddress = locator.GetPlatformSpecificService().GetIp();
    this._metadataService = metadataService;
    this.GoBackCommand = (ICommand) new Command((Action) (async () => await this.GoBack()));
    this.OpenSelectionCommand = (ICommand) new Command<BridgeSettingUI>((Action<BridgeSettingUI>) (setting => this.OpenSelection(setting)));
    this.SaveSettingsCommand = (ICommand) new Command<BridgeSettingUI>((Action<BridgeSettingUI>) (async setting => await this.SaveSettingsAsync()));
    this.CallibrationIntervalTapCommand = (ICommand) new Command(new Action(this.OpenCallibrationSelection));
    this.WiFiBridgeSettingsCommand = (ICommand) new Command(new Action(this.GoToWiFiBridgeSettingsCommand));
    UIDataWithValidation.OnDataValidationChange = (Action<bool>) (isValid =>
    {
      try
      {
        if (this.GroupedSettingsList == null)
          return;
        this.IsBtnDisabled = !isValid || this.GroupedSettingsList.Any<BridgeSettingsViewModel.BridgeSettingsGroup>((Func<BridgeSettingsViewModel.BridgeSettingsGroup, bool>) (x => x.Any<BridgeSettingUI>((Func<BridgeSettingUI, bool>) (y => !y.IsValid))));
      }
      catch (Exception ex)
      {
        this._loggingService.getLogger().LogAppInformation(LoggingContext.BRIDGESETTINGS, "Exception while data validation -" + ex.Message, memberName: ".ctor", sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/BridgeSettingsViewModel.cs", sourceLineNumber: 96 /*0x60*/);
        this.IsBtnDisabled = true;
      }
    });
    UIDataWithValidation.OnPropertyValueChange = (Action<string, string>) ((labelName, value) =>
    {
      if (this.initialSetup)
        return;
      try
      {
        if (labelName != null)
        {
          if (this.initialUISettingsList.Any<BridgeSettingDto>((Func<BridgeSettingDto, bool>) (item => item.id == labelName && item.value.ToString() != value)))
          {
            string str = value;
            if (value == "True" || value == "False")
              str = value.ToLower();
            this.ChangedSettings[labelName] = str;
          }
          else if (this.ChangedSettings.ContainsKey(labelName))
            this.ChangedSettings.Remove(labelName);
        }
        this.IsBtnDisabled = this.ChangedSettings.Count == 0;
      }
      catch (Exception ex)
      {
        this._loggingService.getLogger().LogAppInformation(LoggingContext.BRIDGESETTINGS, "Exception while adding changed settings -" + ex.Message, memberName: ".ctor", sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/BridgeSettingsViewModel.cs", sourceLineNumber: 130);
      }
    });
  }

  public ObservableCollection<BridgeSettingsViewModel.BridgeSettingsGroup> GroupedSettingsList
  {
    get => this._GroupedSettingsList;
    set
    {
      this._GroupedSettingsList = value;
      this.RaisePropertyChanged<ObservableCollection<BridgeSettingsViewModel.BridgeSettingsGroup>>((Expression<Func<ObservableCollection<BridgeSettingsViewModel.BridgeSettingsGroup>>>) (() => this.GroupedSettingsList));
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

  public string Title
  {
    get => this._Title;
    internal set
    {
      this._Title = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.Title));
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

  public string ConnectedColor
  {
    get => this._ConnectedColor;
    internal set
    {
      this._ConnectedColor = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.ConnectedColor));
    }
  }

  public bool DisplayActivityIndicator
  {
    get => this.displayActivityIndicator;
    set
    {
      this.displayActivityIndicator = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.DisplayActivityIndicator));
    }
  }

  public string SaveButtonText
  {
    get => this._SaveButtonText;
    internal set
    {
      this._SaveButtonText = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.SaveButtonText));
    }
  }

  public bool IsBtnDisabled
  {
    get => this._IsBtnDisabled;
    internal set
    {
      this._IsBtnDisabled = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.IsBtnDisabled));
    }
  }

  public string AdvancedToggleText
  {
    get => this._AdvancedToggleText;
    internal set
    {
      this._AdvancedToggleText = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.AdvancedToggleText));
    }
  }

  public bool IsAdvancedEnabled
  {
    get => this._IsAdvancedEnabled;
    internal set
    {
      this._IsAdvancedEnabled = value;
      foreach (BridgeSettingUI uiSettings in this.UiSettingsList)
      {
        int num = this._IsAdvancedEnabled ? 0 : (uiSettings.IsAdvanced ? 1 : 0);
        uiSettings.IsVisible = num == 0;
      }
      this.SetGroupedSettingsList(this.UiSettingsList.Where<BridgeSettingUI>((Func<BridgeSettingUI, bool>) (x => x.IsVisible)).ToList<BridgeSettingUI>());
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.IsAdvancedEnabled));
    }
  }

  public bool BridgeSettingsMode
  {
    get => this._BridgeSettingsMode;
    internal set
    {
      this._BridgeSettingsMode = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.BridgeSettingsMode));
    }
  }

  public bool ShouldShowAdvanced
  {
    get => this._ShouldShowAdvanced;
    internal set
    {
      this._ShouldShowAdvanced = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.ShouldShowAdvanced));
    }
  }

  public string TestTimeoutLabel
  {
    get => this._TestTimeoutLabel;
    internal set
    {
      this._TestTimeoutLabel = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.TestTimeoutLabel));
    }
  }

  public double TestTimeoutSliderValue
  {
    get => this._TestTimeoutSliderValue;
    internal set
    {
      this._TestTimeoutSliderValue = value;
      if (this.oldTestTimeoutSliderValue == value)
      {
        this.testTimeoutChanged = false;
        this.IsBtnDisabled = this.ChangedSettings.Count == 0 && !this.callibrationChanged;
      }
      else
      {
        this.testTimeoutChanged = true;
        this.IsBtnDisabled = false;
      }
      this.RaisePropertyChanged<double>((Expression<Func<double>>) (() => this.TestTimeoutSliderValue));
    }
  }

  public string CallibrationIntervalLabel
  {
    get => this._CallibrationIntervalLabel;
    internal set
    {
      this._CallibrationIntervalLabel = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.CallibrationIntervalLabel));
    }
  }

  public string SelectedCallibrationOption
  {
    get => this._SelectedCallibrationOption;
    internal set
    {
      this._SelectedCallibrationOption = value;
      if (this.oldCallibrationOption == value)
      {
        this.callibrationChanged = false;
        this.IsBtnDisabled = this.ChangedSettings.Count == 0 && !this.testTimeoutChanged;
      }
      else
      {
        this.callibrationChanged = true;
        this.IsBtnDisabled = false;
      }
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.SelectedCallibrationOption));
    }
  }

  public override void ViewAppeared()
  {
    base.ViewAppeared();
    this._appliance.CheckConnectivity(new connectivityUpdateCallback(this.UpdateStatus));
    Task.Run((Func<Task>) (async () =>
    {
      if (!this.BridgeSettingsMode)
      {
        this.Title = AppResource.COMPACT_APPLIANCE_TESTER_TITLE;
        this.oldTestTimeoutSliderValue = Convert.ToDouble(CoreApp.settings.GetItem("catTimeout").Value);
        this.TestTimeoutSliderValue = this.oldTestTimeoutSliderValue;
        this.oldCallibrationOption = CoreApp.settings.GetItem("SelectedCallibrationOption").Value;
        this.SelectedCallibrationOption = this.oldCallibrationOption;
      }
      else
        this.Title = AppResource.BRIDGE_SETTINGS_TITLE;
      this.GetBridgeSettings();
      await Task.Delay(1000);
      this.initialSetup = false;
      this.DisplayActivityIndicator = false;
    }));
  }

  internal async Task GoBack()
  {
    if (!this.IsBtnDisabled)
    {
      int num;
      Device.BeginInvokeOnMainThread((Action) (async () => num = await this._alertService.ShowMessageAlertWithKey("BRIDGE_SETTINGS_BACK_ALERT_MESSAGE", AppResource.BRIDGE_SETTINGS_BACK_ALERT_TITLE, AppResource.WARNING_CANCEL, AppResource.WARNING_CONFIRM, (Action<bool>) (async shouldCancel =>
      {
        if (shouldCancel)
          return;
        await this.ClosePage();
      })) ? 1 : 0));
    }
    else
      await this.ClosePage();
  }

  internal async Task ClosePage(bool settingsChanged = false)
  {
    this.DisplayActivityIndicator = true;
    if (settingsChanged)
    {
      IMvxNavigationService navigationService = this._navigationService;
      DetailReturnArgs detailReturnArgs = new DetailReturnArgs();
      detailReturnArgs.IsBridgeSettingsSaved = settingsChanged;
      CancellationToken cancellationToken = new CancellationToken();
      int num = await navigationService.Close<DetailReturnArgs>((IMvxViewModelResult<DetailReturnArgs>) this, detailReturnArgs, cancellationToken) ? 1 : 0;
    }
    else
    {
      int num1 = await this._navigationService.Close((IMvxViewModel) this, new CancellationToken()) ? 1 : 0;
    }
  }

  internal void GoToWiFiBridgeSettingsCommand()
  {
    IMvxNavigationService navigationService = this._navigationService;
    DetailNavigationArgs detailNavigationArgs = new DetailNavigationArgs();
    detailNavigationArgs.detailNavigationPageType = DetailNavigationPageType.BridgeUpgrade;
    CancellationToken cancellationToken = new CancellationToken();
    navigationService.Navigate<InAppBrowserViewModel, DetailNavigationArgs>(detailNavigationArgs, (IMvxBundle) null, cancellationToken);
  }

  internal void UpdateStatus()
  {
    this.WifiStatus = this._appliance.StatusOfBridgeConnection;
    this.ConnectedColor = this._appliance.ConnectedColorBridge;
    if (this._appliance.boolStatusOfBridgeConnection)
      return;
    Task.Run((Func<Task>) (async () =>
    {
      if (this.ReturnToSettingsPage)
      {
        IMvxNavigationService navigationService = this._navigationService;
        DetailNavigationArgs detailNavigationArgs = new DetailNavigationArgs();
        detailNavigationArgs.destinationScreen = nameof (BridgeSettingsViewModel);
        CancellationToken cancellationToken = new CancellationToken();
        int num = await navigationService.Navigate<ApplianceInstructionConnectionNonSmmViewModel, DetailNavigationArgs>(detailNavigationArgs, (IMvxBundle) null, cancellationToken) ? 1 : 0;
      }
      int num1 = await this._navigationService.Close((IMvxViewModel) this, new CancellationToken()) ? 1 : 0;
    }));
  }

  internal void GetBridgeSettings()
  {
    SshResponse<List<BridgeSettingDto>> sshResponse = !this.BridgeSettingsMode ? this._sshWrapper.GetCATSettings() : this._sshWrapper.GetBridgeSettings();
    if (sshResponse.Success)
    {
      try
      {
        if (sshResponse.Response == null || sshResponse.Response.Count == 0)
        {
          this.ShowErrorAlertAndReturn();
        }
        else
        {
          this.initialUISettingsList = sshResponse.Response;
          if (this.BridgeSettingsMode)
          {
            this.ShouldShowAdvanced = sshResponse.Response.Any<BridgeSettingDto>((Func<BridgeSettingDto, bool>) (x => BridgeSettingsConstAndMethods.AdvancedSettings.Any<string>((Func<string, bool>) (s => x.group.Contains(s)))));
            foreach (BridgeSettingDto setting in sshResponse.Response.Where<BridgeSettingDto>((Func<BridgeSettingDto, bool>) (x => x.group.ToLower() != "tester")))
            {
              if (!BridgeSettingsConstAndMethods.HiddenSettings.Contains(setting.group))
              {
                BridgeSettingUI bridgeSettingUi = new BridgeSettingUI(setting);
                if (bridgeSettingUi.Type == SettingType.unknown.ToString())
                  this._loggingService.getLogger().LogAppInformation(LoggingContext.BRIDGESETTINGS, $"Bridge setting with id {bridgeSettingUi.Id} is of unknown type", memberName: nameof (GetBridgeSettings), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/BridgeSettingsViewModel.cs", sourceLineNumber: 463);
                else
                  this.UiSettingsList.Add(bridgeSettingUi);
              }
            }
          }
          else
          {
            foreach (BridgeSettingDto setting in sshResponse.Response)
            {
              if (!BridgeSettingsConstAndMethods.HiddenSettings.Contains(setting.group))
              {
                BridgeSettingUI bridgeSettingUi = new BridgeSettingUI(setting);
                if (bridgeSettingUi.Type == SettingType.unknown.ToString())
                  this._loggingService.getLogger().LogAppInformation(LoggingContext.BRIDGESETTINGS, $"CAT setting with id {bridgeSettingUi.Id} is of unknown type", memberName: nameof (GetBridgeSettings), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/BridgeSettingsViewModel.cs", sourceLineNumber: 476);
                else
                  this.UiSettingsList.Add(bridgeSettingUi);
              }
            }
          }
          this.SetGroupedSettingsList(this.UiSettingsList.Where<BridgeSettingUI>((Func<BridgeSettingUI, bool>) (x => x.IsVisible)).ToList<BridgeSettingUI>());
        }
      }
      catch (Exception ex)
      {
        if (this.BridgeSettingsMode)
          this._loggingService.getLogger().LogAppError(LoggingContext.BRIDGESETTINGS, "Bridge settings failed to get parsed: " + ex.Message, memberName: nameof (GetBridgeSettings), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/BridgeSettingsViewModel.cs", sourceLineNumber: 486);
        else
          this._loggingService.getLogger().LogAppError(LoggingContext.BRIDGESETTINGS, "CAT settings failed to get parsed: " + ex.Message, memberName: nameof (GetBridgeSettings), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/BridgeSettingsViewModel.cs", sourceLineNumber: 488);
        this.ShowErrorAlertAndReturn();
      }
    }
    else
      this.ShowErrorAlertAndReturn();
  }

  internal void OpenSelection(BridgeSettingUI setting)
  {
    if (setting.IsLocked)
      return;
    this._alertService.ShowActionSheet(setting.Label, "", AppResource.CANCEL_LABEL, setting.Options.ToArray(), (Action<string>) (selectedOption =>
    {
      if (string.Equals(selectedOption, setting.StringValue) || string.Equals(selectedOption, AppResource.CANCEL_LABEL))
        return;
      setting.StringValue = selectedOption;
    }));
  }

  internal void OpenCallibrationSelection()
  {
    this._alertService.ShowActionSheet(this.CallibrationIntervalLabel, "", AppResource.CANCEL_LABEL, this.CallibrationOptions.ToArray(), (Action<string>) (selectedOption =>
    {
      if (string.Equals(selectedOption, this.SelectedCallibrationOption) || string.Equals(selectedOption, AppResource.CANCEL_LABEL))
        return;
      this.SelectedCallibrationOption = selectedOption;
    }));
  }

  public void OnEntryTextChanged(string Id, string Value)
  {
    if (Id == null)
      return;
    if (this.initialUISettingsList.Any<BridgeSettingDto>((Func<BridgeSettingDto, bool>) (item => item.id == Id && (string) item.value != Value)))
      this.ChangedSettings[Id] = Value;
    else if (this.ChangedSettings.ContainsKey(Id))
      this.ChangedSettings.Remove(Id);
  }

  internal async Task SaveSettingsAsync()
  {
    bool isSettingsSaved = false;
    string successMessage;
    string errorMessage;
    if (!this.BridgeSettingsMode)
    {
      successMessage = AppResource.CAT_SETTINGS_SAVED_SUCCESSFULLY_TEXT;
      errorMessage = AppResource.CAT_SETTINGS_NOT_SAVED_SUCCESSFULLY_TEXT;
      if (this.testTimeoutChanged)
      {
        iService5.Core.Services.Data.SettingsDB settings1 = CoreApp.settings;
        double timeoutSliderValue = this.TestTimeoutSliderValue;
        Settings settings2 = new Settings("catTimeout", timeoutSliderValue.ToString());
        settings1.UpdateItem(settings2);
        Logger logger = this._loggingService.getLogger();
        timeoutSliderValue = this.TestTimeoutSliderValue;
        string message = "Stored CAT timeout is :" + timeoutSliderValue.ToString();
        logger.LogAppDebug(LoggingContext.LOCAL, message, memberName: nameof (SaveSettingsAsync), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/BridgeSettingsViewModel.cs", sourceLineNumber: 545);
      }
      if (this.callibrationChanged)
      {
        CoreApp.settings.UpdateItem(new Settings("SelectedCallibrationOption", this.SelectedCallibrationOption));
        this._loggingService.getLogger().LogAppDebug(LoggingContext.LOCAL, "Stored calibration interval is :" + this.SelectedCallibrationOption, memberName: nameof (SaveSettingsAsync), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/BridgeSettingsViewModel.cs", sourceLineNumber: 550);
      }
    }
    else
    {
      successMessage = AppResource.BRIDGE_SETTINGS_SAVED_SUCCESSFULLY_TEXT;
      errorMessage = AppResource.BRIDGE_SETTINGS_NOT_SAVED_SUCCESSFULLY_TEXT;
    }
    if (this.ChangedSettings.Count > 0)
    {
      SshResponse<Dictionary<string, string>> setResponse = this._sshWrapper.SetBridgeSettings(JsonConvert.SerializeObject((object) this.ChangedSettings));
      if (setResponse.Success)
      {
        isSettingsSaved = true;
        this.ReestablishBridgeConnection();
        await this._alertService.ShowMessageAlertWithMessage(successMessage, AppResource.INFORMATION_TEXT);
      }
      else
        await this._alertService.ShowMessageAlertWithMessage(errorMessage, AppResource.INFORMATION_TEXT);
      setResponse = (SshResponse<Dictionary<string, string>>) null;
    }
    else
      await this._alertService.ShowMessageAlertWithMessage(successMessage, AppResource.INFORMATION_TEXT);
    await this.ClosePage(isSettingsSaved);
    successMessage = (string) null;
    errorMessage = (string) null;
  }

  private void ReestablishBridgeConnection()
  {
    this._loggingService.getLogger().LogAppDebug(LoggingContext.BRIDGESETTINGS, "ReStarted SSH Connection", memberName: nameof (ReestablishBridgeConnection), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/BridgeSettingsViewModel.cs", sourceLineNumber: 581);
    Mvx.IoCProvider.RegisterSingleton<Is5SshWrapper>(new Is5SshWrapper(BuildProperties.LoginUser, UtilityFunctions.GenerateStreamFromString(this._metadataService.getSSHKeyValue()), this._loggingService));
  }

  internal void ShowErrorAlertAndReturn()
  {
    string message = !this.BridgeSettingsMode ? "CAT_SETTINGS_ERROR_MESSAGE" : "BRIDGE_SETTINGS_ERROR_MESSAGE";
    Device.BeginInvokeOnMainThread((Action) (async () => await this._alertService.ShowMessageAlertWithKey(message, AppResource.ERROR_TITLE, AppResource.WARNING_OK, (Action) (async () => await this.ClosePage()))));
  }

  internal void SetGroupedSettingsList(List<BridgeSettingUI> settingsList)
  {
    this.GroupedSettingsList = new ObservableCollection<BridgeSettingsViewModel.BridgeSettingsGroup>(settingsList.GroupBy<BridgeSettingUI, string>((Func<BridgeSettingUI, string>) (x => x.Group)).Select<IGrouping<string, BridgeSettingUI>, BridgeSettingsViewModel.BridgeSettingsGroup>((Func<IGrouping<string, BridgeSettingUI>, BridgeSettingsViewModel.BridgeSettingsGroup>) (x => new BridgeSettingsViewModel.BridgeSettingsGroup(x.Key, settingsList.Where<BridgeSettingUI>((Func<BridgeSettingUI, bool>) (y => y.Group == x.Key)).ToList<BridgeSettingUI>()))).ToList<BridgeSettingsViewModel.BridgeSettingsGroup>());
  }

  public class BridgeSettingsGroup : List<BridgeSettingUI>
  {
    public string Group { get; set; }

    public bool IsVisible { get; set; }

    public BridgeSettingsGroup(string group, List<BridgeSettingUI> settings)
      : base((IEnumerable<BridgeSettingUI>) settings)
    {
      this.Group = group;
      this.IsVisible = !string.IsNullOrEmpty(group) && settings.Any<BridgeSettingUI>((Func<BridgeSettingUI, bool>) (x => x.IsVisible));
    }
  }
}
