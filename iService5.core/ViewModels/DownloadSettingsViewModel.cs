// Decompiled with JetBrains decompiler
// Type: iService5.Core.ViewModels.DownloadSettingsViewModel
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Data;
using iService5.Core.Services.Helpers;
using iService5.Core.Services.Platform;
using iService5.Core.Services.User.ActivityLogging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using Plugin.Toast;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
//using Xamarin.Forms;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

#nullable disable
namespace iService5.Core.ViewModels;

public class DownloadSettingsViewModel : MvxViewModel<DownloadParameter>
{
  private readonly IMvxNavigationService _navigationService;
  private readonly IAlertService _alertService;
  private readonly IPlatformSpecificServiceLocator _locator;
  private readonly ILoggingService _loggingService;
  private readonly ISecureStorageService _secureStorageService;
  private readonly IMetadataService _metadataService;
  internal List<DownloadProxy> _modulesToDowload;
  internal List<material> materialsCountryRelevant;
  internal List<material> materialsFullDownload;
  internal List<material> materialsToDownload;
  internal int numberOfFilesFromParameter = 0;
  internal string fileSizeFromParameter = "";
  internal string selectedDeviceClass = DeviceClass.SMM.ToString();
  internal string selectedDownloadOption = DownloadOption.COUNTRY_RELEVANT.ToString();
  private bool displayActivityIndicator = true;
  private List<Country> countryList;
  private bool isExpandableListBusy = false;
  private bool isEmpty = false;
  private string title = string.Empty;
  private string busyText = string.Empty;
  private List<DownloadSettings> _DownloadSettingsData = new List<DownloadSettings>();
  private bool _isbusy = false;
  private string _recommendedText = $"({AppResource.RECOMMENDED})";
  private string _selectedDeviceClassForDisplay = AppResource.SMM_TEXT;
  private string _selectedDownloadSettings = DownloadOption.COUNTRY_RELEVANT.ToString();
  private int _filesToBeDownloaded = 0;
  private string _fileSizeToBeDownloaded = "";
  private string _numberOfCountrySelected = "";
  private string _availableStorage = "";
  private bool _fileSizeSwitchToggled = false;
  private bool _fullDownloadSelected = false;
  private string _fileSizeFilterLabel = AppResource.BINARY_FILE_SIZE_FILTER_TEXT.Replace("{placeholder}", BuildProperties.BinarySizeThreshold);
  private string[] deviceClassOptions = new string[3]
  {
    AppResource.ALL_TEXT,
    AppResource.SMM_TEXT,
    AppResource.NON_SMM_TEXT
  };
  private string[] deviceClassOptions2 = new string[1]
  {
    AppResource.SMM_TEXT
  };
  private bool isCountryRelevant = true;

  public DownloadSettingsViewModel(
    IMetadataService metadataService,
    IPlatformSpecificServiceLocator locator,
    IMvxNavigationService navigationService,
    IAlertService alertService,
    ILoggingService loggingService,
    ISecureStorageService secureStorageService)
  {
    this.BackCommand = (ICommand) new Command(new Action(this.VisitSettingsPage));
    this.DeviceClassSelection = (ICommand) new Command(new Action(this.DisplayDeviceClassOptions));
    this.SaveCommand = (ICommand) new Command(new Action(this.SaveDownloadSettings));
    this._navigationService = navigationService;
    this._alertService = alertService;
    this._locator = locator;
    this._loggingService = loggingService;
    this._secureStorageService = secureStorageService;
    this._metadataService = metadataService;
    this.AvailableStorage = this._locator.GetPlatformSpecificService().GetMemoryStorage(false);
    this.CountryList = metadataService.GetCountryList();
    this.CountryPreselectAndSort();
  }

  private void CountryPreselectAndSort()
  {
    try
    {
      this.CheckSelectedDownloadSettings();
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppError(LoggingContext.LOCAL, ex.Message, memberName: nameof (CountryPreselectAndSort), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/DownloadSettingsViewModel.cs", sourceLineNumber: 63 /*0x3F*/);
    }
    Settings settings = CoreApp.settings.GetItem("CountryCodes");
    List<string> stringList = new List<string>();
    if (!string.IsNullOrEmpty(settings.Value))
    {
      stringList = Enumerable.OfType<string>(settings.Value.ToString().Split(',')).ToList<string>();
    }
    else
    {
      string upperInvariant = UtilityFunctions.GetUserCountryCode(this._secureStorageService).ToUpperInvariant();
      stringList.Add(upperInvariant);
    }
    this.CountryList.Sort((Comparison<Country>) ((x, y) => string.Compare(x.country, y.country)));
    foreach (Country country in this.CountryList)
    {
      if (stringList.Contains(country.country))
        country.isSelected = true;
    }
  }

  public ICommand BackCommand { internal set; get; }

  public ICommand DeviceClassSelection { internal set; get; }

  public ICommand SaveCommand { internal set; get; }

  public bool DisplayActivityIndicator
  {
    get => this.displayActivityIndicator;
    set
    {
      this.displayActivityIndicator = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.DisplayActivityIndicator));
    }
  }

  public List<Country> CountryList
  {
    get => this.countryList;
    set
    {
      this.countryList = value;
      this.RaisePropertyChanged<List<Country>>((Expression<Func<List<Country>>>) (() => this.CountryList));
    }
  }

  public bool IsExpandableListBusy
  {
    get => this.isExpandableListBusy;
    set
    {
      this.SetProperty<bool>(ref this.isExpandableListBusy, value, nameof (IsExpandableListBusy), (Action) null);
    }
  }

  public bool IsEmpty
  {
    get => this.isEmpty;
    set
    {
      this.isEmpty = value;
      this.OnEmptyChanged();
    }
  }

  private void OnEmptyChanged() => CrossToastPopUp.Current.ShowToastMessage("No Data Found");

  public string Title
  {
    get => this.title;
    set => this.SetProperty<string>(ref this.title, value, nameof (Title), (Action) null);
  }

  public string BusyText
  {
    get => this.busyText;
    set => this.SetProperty<string>(ref this.busyText, value, nameof (BusyText), (Action) null);
  }

  protected bool SetProperty<T>(
    ref T backingStore,
    T value,
    [CallerMemberName] string propertyName = "",
    Action onChanged = null)
  {
    if (EqualityComparer<T>.Default.Equals(backingStore, value))
      return false;
    backingStore = value;
    if (onChanged != null)
      onChanged();
    this.OnPropertyChanged(propertyName);
    return true;
  }

  public event PropertyChangedEventHandler PropertyChanged;

  protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
  {
    PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
    if (propertyChanged == null)
      return;
    propertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
  }

  public List<DownloadSettings> DownloadSettingsData
  {
    get => this._DownloadSettingsData;
    set
    {
      this._DownloadSettingsData = value;
      this.RaisePropertyChanged<List<DownloadSettings>>((Expression<Func<List<DownloadSettings>>>) (() => this.DownloadSettingsData));
    }
  }

  public bool IsBusy
  {
    get => this._isbusy;
    set
    {
      this._isbusy = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.IsBusy));
    }
  }

  public string RecommendedText
  {
    get => this._recommendedText;
    set
    {
      this._recommendedText = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.RecommendedText));
    }
  }

  public string SelectedDeviceClassForDisplay
  {
    get => this._selectedDeviceClassForDisplay;
    set
    {
      this._selectedDeviceClassForDisplay = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.SelectedDeviceClassForDisplay));
    }
  }

  public string SelectedDownloadSettings
  {
    get => this._selectedDownloadSettings;
    set
    {
      this._selectedDownloadSettings = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.SelectedDownloadSettings));
    }
  }

  public int FilesToBeDownloaded
  {
    get => this._filesToBeDownloaded;
    set
    {
      this._filesToBeDownloaded = value;
      this.RaisePropertyChanged<int>((Expression<Func<int>>) (() => this.FilesToBeDownloaded));
    }
  }

  public string FileSizeToBeDownloaded
  {
    get => this._fileSizeToBeDownloaded;
    set
    {
      this._fileSizeToBeDownloaded = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.FileSizeToBeDownloaded));
    }
  }

  public string NumberOfCountrySelected
  {
    get => this._numberOfCountrySelected;
    set
    {
      this._numberOfCountrySelected = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.NumberOfCountrySelected));
    }
  }

  public string AvailableStorage
  {
    get => this._availableStorage;
    set
    {
      this._availableStorage = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.AvailableStorage));
    }
  }

  public bool FileSizeSwitchToggled
  {
    get => this._fileSizeSwitchToggled;
    set
    {
      this._fileSizeSwitchToggled = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.FileSizeSwitchToggled));
    }
  }

  public bool FullDownloadSelected
  {
    get => this._fullDownloadSelected;
    set
    {
      this._fullDownloadSelected = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.FullDownloadSelected));
    }
  }

  public string FileSizeFilterLabel
  {
    get => this._fileSizeFilterLabel;
    private set
    {
      this._fileSizeFilterLabel = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.FileSizeFilterLabel));
    }
  }

  public bool NonSmmExlude { get; private set; }

  internal void VisitSettingsPage()
  {
    this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
  }

  internal void DisplayDeviceClassOptions()
  {
    if (this.NonSmmExlude)
      this._alertService.ShowActionSheet(AppResource.SELECT_DEVICE_CLASS_TEXT, "", AppResource.CANCEL_LABEL, this.deviceClassOptions2, (Action<string>) (selectedOption => this.UpdateUIUponDeviceClassSelection(selectedOption)));
    else
      this._alertService.ShowActionSheet(AppResource.SELECT_DEVICE_CLASS_TEXT, "", AppResource.CANCEL_LABEL, this.deviceClassOptions, (Action<string>) (selectedOption => this.UpdateUIUponDeviceClassSelection(selectedOption)));
  }

  internal void UpdateUIUponDeviceClassSelection(string selectedOption)
  {
    string deviceClassForStorage = this.GetDeviceClassForStorage(selectedOption);
    if (string.Equals(deviceClassForStorage, this.selectedDeviceClass) || string.Equals(selectedOption, AppResource.CANCEL_LABEL))
      return;
    this.selectedDeviceClass = deviceClassForStorage;
    this.SelectedDeviceClassForDisplay = selectedOption;
    this.UpdateStatisticsUI();
    this._loggingService.getLogger().LogAppInformation(LoggingContext.LOCAL, "Selected device class: " + selectedOption, memberName: nameof (UpdateUIUponDeviceClassSelection), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/DownloadSettingsViewModel.cs", sourceLineNumber: 364);
  }

  internal void CheckSelectedDownloadSettings()
  {
    Settings settings1 = CoreApp.settings.GetItem("DownloadSettings");
    if (settings1 != null && settings1.Value != "")
    {
      List<DownloadSettings> downloadSettingsList1 = new List<DownloadSettings>();
      List<DownloadSettings> downloadSettingsList2 = downloadSettingsList1;
      DownloadOption downloadOption = DownloadOption.FULL_DOWNLOAD;
      string key1 = downloadOption.ToString();
      string fullDownloadText = AppResource.FULL_DOWNLOAD_TEXT;
      string downloadDescriptionText = AppResource.FULL_DOWNLOAD_DESCRIPTION_TEXT;
      string str1 = settings1.Value;
      downloadOption = DownloadOption.FULL_DOWNLOAD;
      string str2 = downloadOption.ToString();
      int num1 = str1 == str2 ? 1 : 0;
      DownloadSettings downloadSettings1 = new DownloadSettings(key1, fullDownloadText, false, downloadDescriptionText, num1 != 0);
      downloadSettingsList2.Add(downloadSettings1);
      List<DownloadSettings> downloadSettingsList3 = downloadSettingsList1;
      downloadOption = DownloadOption.COUNTRY_RELEVANT;
      string key2 = downloadOption.ToString();
      string countryRelevantText = AppResource.COUNTRY_RELEVANT_TEXT;
      string relevantDescriptionText = AppResource.COUNTRY_RELEVANT_DESCRIPTION_TEXT;
      string str3 = settings1.Value;
      downloadOption = DownloadOption.COUNTRY_RELEVANT;
      string str4 = downloadOption.ToString();
      int num2 = str3 == str4 ? 1 : 0;
      DownloadSettings downloadSettings2 = new DownloadSettings(key2, countryRelevantText, true, relevantDescriptionText, num2 != 0);
      downloadSettingsList3.Add(downloadSettings2);
      this.DownloadSettingsData = downloadSettingsList1;
      Settings settings2 = CoreApp.settings.GetItem("FileSizeSwitchToggled");
      if (settings2 != null && settings2.Value != "")
        this.FileSizeSwitchToggled = bool.Parse(settings2.Value);
      Settings settings3 = CoreApp.settings.GetItem("SelectedDeviceClass");
      if (settings3 != null && settings3.Value != "")
      {
        string a = settings1.Value;
        downloadOption = DownloadOption.COUNTRY_RELEVANT;
        string b = downloadOption.ToString();
        this.isCountryRelevant = string.Equals(a, b);
        this.SelectedDeviceClassForDisplay = this.GetDeviceClassForDisplay(settings3.Value);
        this.selectedDeviceClass = settings3.Value;
      }
    }
    else
    {
      this._loggingService.getLogger().LogAppInformation(LoggingContext.LOCAL, "No configured settings found.", memberName: nameof (CheckSelectedDownloadSettings), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/DownloadSettingsViewModel.cs", sourceLineNumber: 400);
      DeviceClass deviceClass = DeviceClass.SMM;
      this.SelectedDeviceClassForDisplay = this.GetDeviceClassForDisplay(deviceClass.ToString());
      deviceClass = DeviceClass.SMM;
      this.selectedDeviceClass = deviceClass.ToString();
      this.isCountryRelevant = true;
      this.FileSizeSwitchToggled = false;
      List<DownloadSettings> downloadSettingsList4 = new List<DownloadSettings>();
      List<DownloadSettings> downloadSettingsList5 = downloadSettingsList4;
      DownloadOption downloadOption = DownloadOption.FULL_DOWNLOAD;
      DownloadSettings downloadSettings3 = new DownloadSettings(downloadOption.ToString(), AppResource.FULL_DOWNLOAD_TEXT, false, AppResource.FULL_DOWNLOAD_DESCRIPTION_TEXT, true);
      downloadSettingsList5.Add(downloadSettings3);
      List<DownloadSettings> downloadSettingsList6 = downloadSettingsList4;
      downloadOption = DownloadOption.COUNTRY_RELEVANT;
      DownloadSettings downloadSettings4 = new DownloadSettings(downloadOption.ToString(), AppResource.COUNTRY_RELEVANT_TEXT, true, AppResource.COUNTRY_RELEVANT_DESCRIPTION_TEXT, false);
      downloadSettingsList6.Add(downloadSettings4);
      this.DownloadSettingsData = downloadSettingsList4;
    }
    this.DisplayActivityIndicator = false;
  }

  public void OnDownloadSettingsChanged(string value, bool isChecked)
  {
    try
    {
      DownloadSettings downloadSettings = this.DownloadSettingsData.FirstOrDefault<DownloadSettings>((Func<DownloadSettings, bool>) (x => x.option.Contains(value)));
      if (downloadSettings == null)
        return;
      downloadSettings.isSelected = isChecked;
      this.SelectedDownloadSettings = downloadSettings.key;
      this.isCountryRelevant = string.Equals(downloadSettings.key, DownloadOption.COUNTRY_RELEVANT.ToString());
      this.FullDownloadSelected = !this.isCountryRelevant;
      this.UpdateStatisticsUI();
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppError(LoggingContext.LOCAL, "Exception during handle changed download settings" + ex.Message, memberName: nameof (OnDownloadSettingsChanged), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/DownloadSettingsViewModel.cs", sourceLineNumber: 428);
    }
  }

  public void OnFileSizeFilterToggled(bool isToggled)
  {
    this.FileSizeSwitchToggled = isToggled;
    this.UpdateStatisticsUI();
  }

  internal void SaveDownloadSettings()
  {
    CoreApp.settings.UpdateItem(new Settings("DownloadSettings", this.SelectedDownloadSettings));
    CoreApp.settings.UpdateItem(new Settings("SelectedDeviceClass", this.selectedDeviceClass));
    CoreApp.settings.UpdateItem(new Settings("FileSizeSwitchToggled", this.FileSizeSwitchToggled.ToString()));
    CoreApp.settings.UpdateItem(new Settings("CountryCodes", this.getCountryCodes().Trim()));
    this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
  }

  public void TapCountry(Country country) => this.UpdateStatisticsUI();

  private string getCountryCodes()
  {
    StringBuilder stringBuilder = new StringBuilder();
    foreach (Country country in this.CountryList)
    {
      if (country.isSelected)
      {
        stringBuilder.Append(country.country);
        stringBuilder.Append(",");
      }
    }
    return stringBuilder.ToString();
  }

  internal string GetFormattedFileSize(long fileSize) => FileSizeFormatter.FormatSize(fileSize);

  public void UpdateStatisticsUI()
  {
    this.DisplayActivityIndicator = true;
    Task.Run((Action) (() =>
    {
      DownloadFilesStatistics downloadStatistics = UtilityFunctions.GetDownloadStatistics(this.isCountryRelevant, this.selectedDeviceClass, this.FileSizeSwitchToggled, this._secureStorageService, this.countryList.Where<Country>((Func<Country, bool>) (country => country.isSelected)).ToList<Country>());
      this.FilesToBeDownloaded = downloadStatistics.fileCount;
      this.FileSizeToBeDownloaded = FileSizeFormatter.FormatSize(downloadStatistics.fileSize);
      this.NumberOfCountrySelected = this.CountryList.Where<Country>((Func<Country, bool>) (country => country.isSelected)).ToList<Country>().Count.ToString();
      this.DisplayActivityIndicator = false;
    }));
  }

  public override void Prepare(DownloadParameter parameter)
  {
    if (parameter.bridgeSettingSwitchToggled)
      this.NonSmmExlude = false;
    else
      this.NonSmmExlude = true;
  }

  public string GetDeviceClassForStorage(string deviceClassOption)
  {
    if (string.Equals(deviceClassOption, AppResource.SMM_TEXT))
      return DeviceClass.SMM.ToString();
    return string.Equals(deviceClassOption, AppResource.NON_SMM_TEXT) ? DeviceClass.NON_SMM.ToString() : DeviceClass.ALL.ToString();
  }

  public string GetDeviceClassForDisplay(string deviceClassOption)
  {
    if (string.Equals(deviceClassOption, DeviceClass.SMM.ToString()))
      return AppResource.SMM_TEXT;
    return string.Equals(deviceClassOption, DeviceClass.NON_SMM.ToString()) ? AppResource.NON_SMM_TEXT : AppResource.ALL_TEXT;
  }

  public string GetDownloadOptionForStorage(string downloadOption)
  {
    return string.Equals(downloadOption, AppResource.FULL_DOWNLOAD_TEXT) ? DownloadOption.FULL_DOWNLOAD.ToString() : DownloadOption.COUNTRY_RELEVANT.ToString();
  }

  public string GetDownloadoptionForDisplay(string downloadOption)
  {
    return string.Equals(downloadOption, DownloadOption.FULL_DOWNLOAD.ToString()) ? AppResource.FULL_DOWNLOAD_TEXT : AppResource.COUNTRY_RELEVANT_TEXT;
  }
}
