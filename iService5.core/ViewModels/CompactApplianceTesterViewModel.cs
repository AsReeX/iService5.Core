// Decompiled with JetBrains decompiler
// Type: iService5.Core.ViewModels.CompactApplianceTesterViewModel
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Data;
using iService5.Core.Services.Helpers;
using iService5.Core.Services.User.ActivityLogging;
using iService5.Ssh.enums;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
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

public class CompactApplianceTesterViewModel : MvxViewModel
{
  private readonly IMvxNavigationService _navigationService;
  private readonly ILoggingService _loggingService;
  private string _NavText = AppResource.APPLIANCE_FLASH_BACK_TEXT;
  private string _NavHeaderTitle = AppResource.COMPACT_APPLIANCE_TESTER_TITLE;
  internal string _TestTimeoutLabel = AppResource.TEST_TIMEOUT_LABEL;
  internal double _TestTimeoutSliderValue;
  internal string _CallibrationIntervalLabel = AppResource.CALLIBRATION_INTERVAL_LABEL;
  internal string _FirmwareLanguageLabel = AppResource.FIRMWARE_LANGUAGE_LABEL;
  private bool _CallibrationOptionsListDisplay = false;
  private bool _LanguageOptionsListDisplay = false;
  private List<string> _ListOfCallibrationOptions;
  private CATLanguageOptions[] _ListOfLanguageOptions;
  private string _SelectedCallibrationOption = AppResource.CALLIBRATION_ONCE_A_DAY;
  private CATLanguageOptions _SelectedLanguageOption = CATLanguageOptions.DE;
  private bool _OneNpeSwitchToggled = true;
  private bool _TwoLpeSwitchToggled = false;
  private bool _areButtonsEnabled = true;

  public ICommand GoBackCommand { get; internal set; }

  public ICommand NavigatePreviousPage { internal set; get; }

  public ICommand CallibrationIntervalTapCommand { get; internal set; }

  public ICommand FirmwareLanguageTapCommand { get; internal set; }

  public ICommand UpgradeTesterFirmwareTapCommand { get; internal set; }

  public virtual async Task Initialize() => await base.Initialize();

  public CompactApplianceTesterViewModel(
    IMvxNavigationService navigationService,
    ILoggingService loggingService)
  {
    this._navigationService = navigationService;
    this._loggingService = loggingService;
    this.NavigatePreviousPage = (ICommand) new Command(new Action<object>(this.goToBackPage));
    this.CallibrationIntervalTapCommand = (ICommand) new Command((Action) (() =>
    {
      this.CallibrationOptionsListDisplay = !this.CallibrationOptionsListDisplay;
      this.LanguageOptionsListDisplay = false;
    }));
    this.FirmwareLanguageTapCommand = (ICommand) new Command((Action) (() =>
    {
      this.LanguageOptionsListDisplay = !this.LanguageOptionsListDisplay;
      this.CallibrationOptionsListDisplay = false;
    }));
    this.UpgradeTesterFirmwareTapCommand = (ICommand) new Command((Action) (() =>
    {
      if (!this.AreButtonsEnabled)
        return;
      this.AreButtonsEnabled = false;
      IMvxNavigationService navigationService1 = this._navigationService;
      DetailNavigationArgs detailNavigationArgs = new DetailNavigationArgs();
      detailNavigationArgs.senderScreen = AppResource.COMPACT_APPLIANCE_TESTER_TITLE;
      CancellationToken cancellationToken = new CancellationToken();
      navigationService1.Navigate<ApplianceRepairViewModel, DetailNavigationArgs>(detailNavigationArgs, (IMvxBundle) null, cancellationToken);
    }));
    this.ListOfCallibrationOptions = new List<string>()
    {
      AppResource.CALLIBRATION_MANUALLY,
      AppResource.CALLIBRATION_EVERY_TIME,
      AppResource.CALLIBRATION_ONCE_A_DAY,
      AppResource.CALLIBRATION_ONCE_A_WEEK
    };
    this.ListOfLanguageOptions = (CATLanguageOptions[]) Enum.GetValues(typeof (CATLanguageOptions));
    this.ListOfLanguageOptions = ((IEnumerable<CATLanguageOptions>) this.ListOfLanguageOptions).Skip<CATLanguageOptions>(1).ToArray<CATLanguageOptions>();
    this.TestTimeoutSliderValue = Convert.ToDouble(CoreApp.settings.GetItem("catTimeout").Value);
    Settings settings = CoreApp.settings.GetItem(nameof (SelectedCallibrationOption));
    if (settings == null || !(settings.Value != ""))
      return;
    this.SelectedCallibrationOption = settings.Value;
  }

  public override void ViewAppeared()
  {
    base.ViewAppeared();
    this._areButtonsEnabled = true;
  }

  internal void goToBackPage(object obj)
  {
    MessagingCenter.Send<CompactApplianceTesterViewModel>(this, "ClosingCatTesterPage");
    this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
  }

  public void CallibrationOptionSelected(string option)
  {
    this.SelectedCallibrationOption = option;
    this.CallibrationOptionsListDisplay = false;
  }

  public void LanguageOptionSelected(CATLanguageOptions option)
  {
    this.SelectedLanguageOption = option;
    this.LanguageOptionsListDisplay = false;
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

  public string NavHeaderTitle
  {
    get => this._NavHeaderTitle;
    internal set
    {
      this._NavHeaderTitle = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.NavHeaderTitle));
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
      if (this.TestTimeoutSliderValue == value)
        return;
      this._TestTimeoutSliderValue = value;
      CoreApp.settings.UpdateItem(new Settings("catTimeout", this._TestTimeoutSliderValue.ToString()));
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

  public string FirmwareLanguageLabel
  {
    get => this._FirmwareLanguageLabel;
    internal set
    {
      this._FirmwareLanguageLabel = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.FirmwareLanguageLabel));
    }
  }

  public bool CallibrationOptionsListDisplay
  {
    internal set
    {
      this._CallibrationOptionsListDisplay = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.CallibrationOptionsListDisplay));
    }
    get => this._CallibrationOptionsListDisplay;
  }

  public bool LanguageOptionsListDisplay
  {
    internal set
    {
      this._LanguageOptionsListDisplay = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.LanguageOptionsListDisplay));
    }
    get => this._LanguageOptionsListDisplay;
  }

  public List<string> ListOfCallibrationOptions
  {
    get => this._ListOfCallibrationOptions;
    internal set
    {
      this._ListOfCallibrationOptions = value;
      this.RaisePropertyChanged<List<string>>((Expression<Func<List<string>>>) (() => this.ListOfCallibrationOptions));
    }
  }

  public CATLanguageOptions[] ListOfLanguageOptions
  {
    get => this._ListOfLanguageOptions;
    internal set
    {
      this._ListOfLanguageOptions = value;
      this.RaisePropertyChanged<CATLanguageOptions[]>((Expression<Func<CATLanguageOptions[]>>) (() => this.ListOfLanguageOptions));
    }
  }

  public string SelectedCallibrationOption
  {
    get
    {
      if (this._SelectedCallibrationOption != null)
        this.SelectedCallibrationOption = this._SelectedCallibrationOption;
      return this._SelectedCallibrationOption;
    }
    internal set
    {
      if (!(this._SelectedCallibrationOption != value))
        return;
      this._SelectedCallibrationOption = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.SelectedCallibrationOption));
      CoreApp.settings.UpdateItem(new Settings(nameof (SelectedCallibrationOption), this.SelectedCallibrationOption));
      this._loggingService.getLogger().LogAppDebug(LoggingContext.LOCAL, "Stored calibration interval is :" + this.SelectedCallibrationOption, memberName: nameof (SelectedCallibrationOption), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/CompactApplianceTesterViewModel.cs", sourceLineNumber: 261);
      this.CallibrationOptionsListDisplay = false;
    }
  }

  public CATLanguageOptions SelectedLanguageOption
  {
    get
    {
      if (this._SelectedLanguageOption != 0)
        this.SelectedLanguageOption = this._SelectedLanguageOption;
      return this._SelectedLanguageOption;
    }
    internal set
    {
      if (this._SelectedLanguageOption == value)
        return;
      this._SelectedLanguageOption = value;
      this.RaisePropertyChanged<CATLanguageOptions>((Expression<Func<CATLanguageOptions>>) (() => this.SelectedLanguageOption));
      this.LanguageOptionsListDisplay = false;
    }
  }

  public bool OneNpeSwitchToggled
  {
    internal set
    {
      this._OneNpeSwitchToggled = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.OneNpeSwitchToggled));
    }
    get => this._OneNpeSwitchToggled;
  }

  public bool TwoLpeSwitchToggled
  {
    internal set
    {
      this._TwoLpeSwitchToggled = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.TwoLpeSwitchToggled));
    }
    get => this._TwoLpeSwitchToggled;
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
}
