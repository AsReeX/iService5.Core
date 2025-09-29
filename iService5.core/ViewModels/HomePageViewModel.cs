// Decompiled with JetBrains decompiler
// Type: iService5.Core.ViewModels.HomePageViewModel
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
using MvvmCross.Presenters.Hints;
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

public class HomePageViewModel : MvxViewModel
{
  private readonly IUserSession _userSession;
  private readonly IMetadataService _metadataList;
  private readonly IAlertService _alertService;
  private readonly IMetadataService _metadataService;
  private readonly IAppliance _appliance;
  private readonly IPlatformSpecificServiceLocator _locator;
  private readonly ILoggingService _loggingService;
  private readonly ISecureStorageService _secureStorageService;
  private readonly ISecurityService _securityService;
  private bool isSMMEnumber;
  private bool displayActivityIndicator = true;
  private string _HeaderLabelText;
  private string _HeaderNameLabelText;
  private string _eNumberLabelText;
  private string _GoButtonText;
  private string _DatabaseUpdateLabel = AppResource.HOME_PAGE_DATABASE_STATUS;
  private List<material> _eNumbersList;
  internal string _eNumber = "";
  private bool _isfocused;
  private List<material> _enumbersListInit;
  private bool _displaylist;
  private bool _isrefreshing;
  private material _ItemSelected;
  private bool _HomeBtnIsEnabled = false;
  private bool _HomeBtnVisibility = true;
  private bool _UpdateBtnVisibility;
  private string _WifiBridgeStatus;
  private bool _WifiBridgeMode;
  private double _listViewHeightRequest = 10.0;

  public virtual async Task Initialize() => await base.Initialize();

  protected virtual IMvxNavigationService _navigationService { get; private set; }

  public HomePageViewModel(
    IUserSession userSession,
    IMvxNavigationService navigationService,
    IShortTextsService _ShortTextsService,
    IMetadataService metadataService,
    IAlertService alertService,
    IAppliance appliance,
    IPlatformSpecificServiceLocator locator,
    ILoggingService loggingService,
    ISecureStorageService secureStorageService,
    ISecurityService securityService)
  {
    this.HomeBtnIsEnabled = false;
    this._userSession = userSession;
    this.HeaderLabelText = AppResource.HOME_PAGE_HEADER;
    this.GoButtonText = AppResource.HOME_PAGE_BUTTON;
    this.eNumberLabelText = AppResource.HOME_PAGE_ENUMBER_LABEL_TEXT;
    this._metadataService = metadataService;
    this._metadataList = metadataService;
    this._eNumbersList = new List<material>();
    this.GoToRepairPage = (ICommand) new Command(new Action(this.NextScreen));
    this.GoToSettingsPage = (ICommand) new Command(new Action(this.SettingsPageTransition));
    this._filteredText = (string) null;
    this._navigationService = navigationService;
    this._alertService = alertService;
    this._appliance = appliance;
    this._locator = locator;
    this._loggingService = loggingService;
    this._secureStorageService = secureStorageService;
    MessagingCenter.Subscribe<ApplianceDatabaseViewModel>((object) this, CoreApp.EventsNames.ApplianceBinariesFinished.ToString(), (Action<ApplianceDatabaseViewModel>) (sender => this.UpdateBtnVisibility = false), (ApplianceDatabaseViewModel) null);
    this._securityService = securityService;
    this.certificateHasBeenUpdated = false;
  }

  public ICommand GoToRepairPage { protected set; get; }

  public ICommand GoToSettingsPage { protected set; get; }

  public ICommand GoToNonSmmConnectionVMPage { protected set; get; }

  public bool DisplayActivityIndicator
  {
    get => this.displayActivityIndicator;
    internal set
    {
      this.displayActivityIndicator = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.DisplayActivityIndicator));
    }
  }

  private void NextScreen()
  {
    this.CloseList();
    DateTime now = DateTime.Now;
    UtilityFunctions.setSessionIDForHistoryTable(this.eNumber, now);
    string idForHistoryTable = UtilityFunctions.getSessionIDForHistoryTable();
    try
    {
      CoreApp.history.SaveItem(new History(this.eNumber, now, idForHistoryTable, HistoryDBInfoType.NewSessionStarts.ToString(), this.isSMMEnumber ? "SMM" : "NONSMM"));
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, $"Failed to write {HistoryDBInfoType.NewSessionStarts.ToString()} in the History DB, {ex?.ToString()}", memberName: nameof (NextScreen), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/HomePageViewModel.cs", sourceLineNumber: 105);
    }
    this._userSession.setEnumberSession(this.eNumber);
    this._userSession.SetSenderScreen(AppResource.NAVIGATE_HOME_BTN_LABEL);
    this._navigationService.Navigate<ApplianceRepairViewModel>((IMvxBundle) null, new CancellationToken());
  }

  private void SettingsPageTransition()
  {
    this._navigationService.ChangePresentation((MvxPresentationHint) new MvxPagePresentationHint(typeof (ApplianceDatabaseViewModel)), new CancellationToken());
  }

  public override void ViewAppearing()
  {
    base.ViewAppearing();
    Task.Run((Action) (() => this._appliance.CheckConnectivity(new connectivityUpdateCallback(this.updateStatus))));
  }

  public override void ViewAppeared()
  {
    base.ViewAppeared();
    if (UtilityFunctions.IsPPFTableMigrationNeeded())
    {
      this.DisplayActivityIndicator = true;
      if (UtilityFunctions.PPFTableMigration())
        this._loggingService.getLogger().LogAppError(LoggingContext.LOCAL, "PPF table migration successful", memberName: nameof (ViewAppeared), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/HomePageViewModel.cs", sourceLineNumber: 134);
      else
        this._loggingService.getLogger().LogAppError(LoggingContext.LOCAL, "PPF table migration failed", memberName: nameof (ViewAppeared), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/HomePageViewModel.cs", sourceLineNumber: 138);
    }
    this.DisplayActivityIndicator = false;
    Task.Run((Func<Task>) (async () =>
    {
      await UtilityFunctions.CheckAndSendPendingSupportForm(this._loggingService);
      this.CheckCertificateRefreshPeriod(this._secureStorageService);
      if (!this._metadataService.CheckIfTableExists("materialStatistics"))
        this._metadataService.UpdateMaterialStatistics();
      this._userSession.SetMaterialStatistics(this._metadataService.GetMaterialStatistics());
      this.eNumber = "";
      string urlSchemeEnumber = this._userSession.GetURLSchemeEnumber();
      if (!string.IsNullOrEmpty(urlSchemeEnumber))
      {
        this.eNumber = urlSchemeEnumber;
        this._userSession.SetURLSchemeEnumber("");
        CoreApp.settings.UpdateItem(new Settings("SelectedURLScheme", ""));
      }
      Settings settings = CoreApp.settings.GetItem("UnAuthorisedStatusReceivedViewModelName");
      if (settings != null && settings.Value == "ApplianceDatabaseViewModel")
      {
        CoreApp.settings.UpdateItem(new Settings("UnAuthorisedStatusReceivedViewModelName", ""));
        this.SettingsPageTransition();
      }
      this.SetUpdateBtnVisibility();
      urlSchemeEnumber = (string) null;
      settings = (Settings) null;
    }));
  }

  private async void CheckCertificateRefreshPeriod(ISecureStorageService _secureStorageService)
  {
    if (!((UserSession) this._userSession).metadataUpdated)
      return;
    bool isCertificateValid = await UtilityFunctions.isCertificateValid(_secureStorageService, this._loggingService);
    if (!isCertificateValid)
    {
      this._loggingService.getLogger().LogAppInformation(LoggingContext.CSR, "Certificate Update is needed", memberName: nameof (CheckCertificateRefreshPeriod), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/HomePageViewModel.cs", sourceLineNumber: 181);
      this.GetSignedCertificate();
    }
  }

  private void GetSignedCertificate()
  {
    Task.Run((Func<Task>) (async () =>
    {
      string csr = await this.GetValidCSR();
      this._userSession.signCertificate(csr, new iService5.Core.Services.User.signCertificateCompletionCallback(this.signCertificateCompletionCallback));
      csr = (string) null;
    }));
  }

  public void signCertificateCompletionCallback(SignCertificateStatus res)
  {
    try
    {
      LoggingContext context = LoggingContext.BACKEND;
      string message;
      int num;
      switch (res)
      {
        case SignCertificateStatus.SUCCESS:
          this.certificateHasBeenUpdated = true;
          message = "The certificate has been updated";
          goto label_10;
        case SignCertificateStatus.INTERNET_ISSUE:
          message = "There occured a problem while retrieving certificate due to internet issue";
          goto label_9;
        case SignCertificateStatus.TIMEOUT:
          num = 1;
          break;
        default:
          num = res == SignCertificateStatus.EXCEPTION ? 1 : 0;
          break;
      }
      if (num != 0)
      {
        ServiceError serviceError = this._userSession.getServiceError();
        message = serviceError == null ? "There occured a problem while retrieving certificate" : $"There occured a problem while retrieving certificate: {serviceError.errorKey.ToString()}Error Message: {serviceError.errorMessage}";
      }
      else
        message = "There occured a problem while retrieving certificate";
label_9:
label_10:
      this._loggingService.getLogger().LogAppDebug(context, message, memberName: nameof (signCertificateCompletionCallback), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/HomePageViewModel.cs", sourceLineNumber: 225);
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppDebug(LoggingContext.CSR, ex.ToString(), memberName: nameof (signCertificateCompletionCallback), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/HomePageViewModel.cs", sourceLineNumber: 229);
    }
  }

  private async Task<string> GetValidCSR()
  {
    string password = this._securityService.GetNewRandomPassword();
    string privateKeyInfo = this._securityService.GetNewEd25519KeyInfo(password);
    await this.StoreKeypair(privateKeyInfo, password);
    string csr = this._securityService.GetNewCertificateSigningRequest(privateKeyInfo, password);
    if (this._securityService.ValidateCertificateSigningRequest(csr))
    {
      this._loggingService.getLogger().LogAppDebug(LoggingContext.CSR, "CSR is Valid", memberName: nameof (GetValidCSR), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/HomePageViewModel.cs", sourceLineNumber: 241);
      return csr;
    }
    this._loggingService.getLogger().LogAppDebug(LoggingContext.CSR, "CSR is Invalid", memberName: nameof (GetValidCSR), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/HomePageViewModel.cs", sourceLineNumber: 246);
    return (string) null;
  }

  private async Task StoreKeypair(string privateKeyInfo, string password)
  {
    bool privateKeyInfoStored = await this._secureStorageService.setPrivateKeyInfo(privateKeyInfo);
    bool passwordStored = await this._secureStorageService.setPrivateKeyInfoPassword(password);
    this._loggingService.getLogger().LogAppDebug(LoggingContext.SECURESTORAGE, "Status: Keys information stored successfully - " + privateKeyInfoStored.ToString(), memberName: nameof (StoreKeypair), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/HomePageViewModel.cs", sourceLineNumber: (int) byte.MaxValue);
    this._loggingService.getLogger().LogAppDebug(LoggingContext.SECURESTORAGE, "Status: Keys information password stored successfully - " + passwordStored.ToString(), memberName: nameof (StoreKeypair), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/HomePageViewModel.cs", sourceLineNumber: 256 /*0x0100*/);
  }

  internal void SetUpdateBtnVisibility()
  {
    Settings settings1 = CoreApp.settings.GetItem("DownloadSettings");
    Settings settings2 = CoreApp.settings.GetItem("SelectedDeviceClass");
    string selectedDeviceClass = settings2 == null || string.IsNullOrEmpty(settings2.Value) ? DeviceClass.SMM.ToString() : settings2.Value;
    Settings settings3 = CoreApp.settings.GetItem("FileSizeSwitchToggled");
    bool isFileSizeSwitchToggled = settings3 != null && settings3.Value == "true";
    List<Country> selectedCountryList = UtilityFunctions.GetSelectedCountryList(this._metadataService);
    this.UpdateBtnVisibility = CoreApp.settings.GetItem("OfflineBinaries").Value == "TRUE" && ((settings1 == null || string.IsNullOrEmpty(settings1.Value)) && UtilityFunctions.GetDownloadStatistics(true, DeviceClass.SMM.ToString(), false, this._secureStorageService, selectedCountryList).fileCount > 0 || settings1 != null && !string.IsNullOrEmpty(settings1.Value) && UtilityFunctions.GetDownloadStatistics(settings1.Value == DownloadOption.COUNTRY_RELEVANT.ToString(), selectedDeviceClass, isFileSizeSwitchToggled, this._secureStorageService, selectedCountryList).fileCount > 0);
  }

  private void CloseList()
  {
    this.DisplayList = false;
    this.HomeBtnVisibility = true;
  }

  public string HeaderLabelText
  {
    get => this._HeaderLabelText;
    private set
    {
      this._HeaderLabelText = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.HeaderLabelText));
    }
  }

  public string HeaderNameLabelText
  {
    get => this._HeaderNameLabelText;
    internal set
    {
      this._HeaderNameLabelText = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.HeaderNameLabelText));
    }
  }

  public string eNumberLabelText
  {
    get => this._eNumberLabelText;
    internal set
    {
      this._eNumberLabelText = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.eNumberLabelText));
    }
  }

  public string GoButtonText
  {
    get => this._GoButtonText;
    private set
    {
      this._GoButtonText = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.GoButtonText));
    }
  }

  public string DatabaseUpdateLabel
  {
    get => this._DatabaseUpdateLabel;
    internal set
    {
      this._DatabaseUpdateLabel = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.DatabaseUpdateLabel));
    }
  }

  public List<material> eNumbersList
  {
    get => this._eNumbersList;
    internal set
    {
      this._eNumbersList = value;
      this.RaisePropertyChanged<List<material>>((Expression<Func<List<material>>>) (() => this.eNumbersList));
      if (this._eNumbersList.Count != 0)
      {
        this.HomeBtnVisibility = false;
        this.DisplayList = true;
      }
      else
      {
        this.DisplayList = false;
        this.HomeBtnVisibility = true;
        this.HomeBtnIsEnabled = false;
      }
    }
  }

  public string eNumber
  {
    get => this._eNumber;
    internal set
    {
      if (value.Length > 3)
      {
        if (value.Substring(0, 4) != (this._eNumber.Length > 3 ? this._eNumber.Substring(0, 4) : ""))
        {
          this.enumbersListInit = this._metadataList.getMatchingEntries(value.Substring(0, 4), MatchPattern.PREFIX);
          this.enumbersListInit = this.enumbersListInit.OrderBy<material, string>((Func<material, string>) (x => x._material)).ToList<material>();
        }
        this.eNumbersList = this.enumbersListInit.AsParallel<material>().Where<material>((Func<material, bool>) (x => x._material.StartsWith(value, StringComparison.InvariantCultureIgnoreCase))).ToList<material>();
        this.ListViewHeightRequest = (double) this.eNumbersList.Count * 50.0;
      }
      this._eNumber = value.ToUpper();
      if (string.IsNullOrEmpty(this._eNumber) || this.eNumbersList.Count == 0 || this._eNumber.Length < 4)
      {
        this.DisplayList = false;
        this.HomeBtnVisibility = true;
        this.HomeBtnIsEnabled = false;
      }
      else if (this.eNumbersList.Count != 0 && this._eNumber.Length > 4)
      {
        this.DisplayList = true;
        this.HomeBtnVisibility = false;
        this.HomeBtnIsEnabled = this.eNumbersList.Select<material, string>((Func<material, string>) (x => x._material)).Contains<string>(this._eNumber);
      }
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.eNumber));
    }
  }

  public bool IsVFocused
  {
    get => this._isfocused;
    internal set
    {
      this._isfocused = value;
      if (this.eNumbersList.Count == 0 || this.eNumber.Length < 4)
        this.DisplayList = false;
      else if (this.eNumbersList.Count != 0 || this.eNumber.Length < 4)
      {
        this.DisplayList = true;
        this.HomeBtnVisibility = false;
      }
      else if (!this._isfocused)
      {
        this.DisplayList = false;
        this.HomeBtnVisibility = true;
      }
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.IsVFocused));
    }
  }

  public string _filteredText { get; internal set; }

  public List<material> enumbersListInit
  {
    get => this._enumbersListInit;
    private set => this._enumbersListInit = value;
  }

  public bool DisplayList
  {
    get => this._displaylist;
    internal set
    {
      this._displaylist = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.DisplayList));
    }
  }

  public bool IsVRefreshing
  {
    get => this._isrefreshing;
    internal set
    {
      this._isrefreshing = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.IsVRefreshing));
    }
  }

  public material ObjItemSelected
  {
    get
    {
      if (this._ItemSelected != null)
        this.isSMMEnumber = this._ItemSelected.DisplayIcon;
      return this._ItemSelected;
    }
    set
    {
      if (this._ItemSelected == value)
        return;
      this._ItemSelected = value;
      this.eNumber = this.ObjItemSelected._material;
      this._ItemSelected = (material) null;
      this.RaisePropertyChanged<material>((Expression<Func<material>>) (() => this.ObjItemSelected));
      this.DisplayList = false;
      this.HomeBtnVisibility = true;
      this.HomeBtnIsEnabled = true;
    }
  }

  public bool HomeBtnIsEnabled
  {
    get => this._HomeBtnIsEnabled;
    internal set
    {
      this._HomeBtnIsEnabled = !value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.HomeBtnIsEnabled));
    }
  }

  public bool HomeBtnVisibility
  {
    get => this._HomeBtnVisibility;
    internal set
    {
      this._HomeBtnVisibility = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.HomeBtnVisibility));
    }
  }

  public bool UpdateBtnVisibility
  {
    get => this._UpdateBtnVisibility;
    private set
    {
      this._UpdateBtnVisibility = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.UpdateBtnVisibility));
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

  public bool WifiBridgeMode
  {
    get => this._WifiBridgeMode;
    private set
    {
      this._WifiBridgeMode = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.WifiBridgeMode));
    }
  }

  public double ListViewHeightRequest
  {
    get => this._listViewHeightRequest;
    set
    {
      this.SetProperty<double>(ref this._listViewHeightRequest, value, nameof (ListViewHeightRequest));
    }
  }

  public bool certificateHasBeenUpdated { get; private set; }

  internal void updateStatus()
  {
    this.WifiBridgeStatus = this._appliance.StatusOfBridgeConnection;
    this.WifiBridgeMode = this._appliance.boolStatusOfBridgeConnection;
  }
}
