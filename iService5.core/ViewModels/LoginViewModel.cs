// Decompiled with JetBrains decompiler
// Type: iService5.Core.ViewModels.LoginViewModel
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Backend;
using iService5.Core.Services.Data;
using iService5.Core.Services.Helpers;
using iService5.Core.Services.Localisation;
using iService5.Core.Services.Platform;
using iService5.Core.Services.User;
using iService5.Core.Services.User.ActivityLogging;
using iService5.Core.Services.VersionReport;
using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using Plugin.BreachDetector;
using Serilog.Core;
using System;
using System.Globalization;
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

public class LoginViewModel : MvxViewModel
{
  private readonly ILoggingService _loggingService;
  private readonly IUserSession _userSession;
  private readonly IPlatformSpecificServiceLocator _locator;
  private readonly IAlertService _alertService;
  private readonly IMetadataService _metadataService;
  private readonly ISecureStorageService _secureStorageService;
  private readonly IUserAccount _userAccount;
  private readonly IMvxNavigationService _navigationService;
  private bool _isbusy;
  private bool _EnableLoggedIn;
  private bool _StayLoggedIn;
  private string _loginValidationColor = "#ffffff";
  private bool _LoginFailed = false;
  private bool _MetadataFailed = false;
  private ushort _BiometricTriesRemain = 3;
  private string _LogLabelPrompt;
  private string _AutoLoginLabel;
  private string _LoginButtonLabel;
  private string _LoginErrorLabel;
  private string _OpecUserName = "";
  private string _Password = "";
  private string _AppVersionLabel;
  private bool _CanContinue;
  private bool _IsBtnDisabled;
  private bool _IsUsernameReadOnly = false;
  private bool _IsPasswordReadOnly = false;
  private bool _NeedAccountBtnVisible = true;
  private bool _LegalInfoBtnVisible = true;
  private bool _areButtonsEnabled = true;

  public LoginViewModel(
    IMvxNavigationService navigationService,
    IPlatformSpecificServiceLocator locator,
    IUserSession userSession,
    ILoggingService loggingService,
    IShortTextsService _ShortTextsService,
    IVersionReport versionReport,
    IAlertService alertService,
    IMetadataService metadataService,
    ISecureStorageService secureStorageService,
    IUserAccount userAccount)
  {
    this.CanContinue = false;
    if (_ShortTextsService != null)
    {
      this.LogLabelPrompt = AppResource.LOGIN_PAGE_LOGIN_PROMPT;
      this.LoginButtonLabel = AppResource.LOGIN_PAGE_LOGIN_BUTTON;
      this.AutoLoginLabel = AppResource.LOGIN_PAGE_AUTO_LOGIN_SWITCH;
      this.LoginErrorLabel = AppResource.LOGIN_PAGE_LOGIN_FAILURE_INDICATION_LOCAL;
    }
    this.AppVersionLabel = versionReport.getVersion();
    this._locator = locator;
    this._metadataService = metadataService;
    IPlatformSpecificService platformSpecificService = this._locator.GetPlatformSpecificService();
    this.EnableLoggedIn = platformSpecificService.isFingerprintSupported() && platformSpecificService.isDeviceSecured() && platformSpecificService.isFingerprintAvailable() && platformSpecificService.isFingerprintPermissionGranted();
    this._loggingService = loggingService;
    this._userSession = userSession;
    this._secureStorageService = secureStorageService;
    this._userAccount = userAccount;
    if (this._userSession.isActive())
      this._userSession.deactivate(new deactivationCompletionCallback(this.deactivationAutoCompletionCallback));
    if (this._userSession.isMetadataSessionActive())
      this._userSession.cancelDownload();
    if (this._userSession.IsGetValuesOpen())
      this._userSession.SetIsGetValuesOpen(false);
    this._navigationService = navigationService;
    this._alertService = alertService;
    this.SubmitCommand = (ICommand) new Command(new Action(this.OnSubmit));
    this.LegalInfoCommand = (ICommand) new Command(new Action(this.OnClickingLegalInfo));
    this.NeedAccountCommand = (ICommand) new Command((Action) (() => Device.BeginInvokeOnMainThread((Action) (() => this.OnClickingNeedAccount()))));
    string result1 = this._secureStorageService.getUsername().Result;
    string result2 = this._secureStorageService.getPassword().Result;
    CoreApp.settings.DeleteItem(new Settings("User", ""));
    Settings settings = CoreApp.settings.GetItem("AutoLogin");
    MessagingCenter.Subscribe<StatusViewModel>((object) this, CoreApp.EventsNames.MetadataFailed.ToString(), (Action<StatusViewModel>) (sender => this.MetadataFailed = true), (StatusViewModel) null);
    if (settings != null && settings.Value == "TRUE")
    {
      if (result1 != null && result2 != null)
      {
        this.OpecUserName = result1;
        this.Password = result2;
      }
      this.StayLoggedIn = true;
    }
    else
    {
      if (result1 != null)
        this.OpecUserName = result1;
      this.StayLoggedIn = true;
    }
    this.CheckLocalMigration();
  }

  public async Task fingerprintCompletionCallback(FingerprintVerification res)
  {
    this._loggingService.getLogger().LogAppInformation(LoggingContext.LOGIN, "Called with " + res.ToString(), memberName: nameof (fingerprintCompletionCallback), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/LoginViewModel.cs", sourceLineNumber: 106);
    this.CanContinue = true;
    this.IsBusy = false;
    if (res == FingerprintVerification.SUCCESS)
    {
      CoreApp.settings.DeleteItem(new Settings("AutoLogin", ""));
      CoreApp.settings.SaveItem(new Settings("AutoLogin", "TRUE"));
      bool isUserNameStored = this._secureStorageService.setUsername(this.OpecUserName).Result;
      bool isPasswordStored = this._secureStorageService.setPassword(this.Password).Result;
      if (isUserNameStored & isPasswordStored)
        this._loggingService.getLogger().LogAppDebug(LoggingContext.LOGIN, "Username and Password are stored succesfully in secure storage", memberName: nameof (fingerprintCompletionCallback), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/LoginViewModel.cs", sourceLineNumber: 116);
      else
        this._loggingService.getLogger().LogAppInformation(LoggingContext.LOGIN, "Username and Password are NOT stored succesfully in secure storage", memberName: nameof (fingerprintCompletionCallback), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/LoginViewModel.cs", sourceLineNumber: 118);
    }
    else
    {
      this.StayLoggedIn = true;
      CoreApp.settings.DeleteItem(new Settings("AutoLogin", ""));
      CoreApp.settings.SaveItem(new Settings("AutoLogin", "FALSE"));
    }
    if (res != FingerprintVerification.ERROR && res != FingerprintVerification.SUCCESS && res != FingerprintVerification.IOS_FAILURE)
      return;
    await this.resolveNextScreenAsync();
  }

  public async Task autoLoginFingerprintCompletionCallback(FingerprintVerification res)
  {
    this._loggingService.getLogger().LogAppInformation(LoggingContext.LOGIN, $"Called with {res.ToString()} remaining {this.BiometricTriesRemain.ToString()} tries left", memberName: nameof (autoLoginFingerprintCompletionCallback), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/LoginViewModel.cs", sourceLineNumber: 139);
    if (res == FingerprintVerification.SUCCESS)
    {
      await this.handleJWTTokenExpiration();
    }
    else
    {
      this.Password = "";
      this.CanContinue = false;
      this.IsBusy = false;
      this.StayLoggedIn = true;
      CoreApp.settings.DeleteItem(new Settings("AutoLogin", ""));
      CoreApp.settings.SaveItem(new Settings("AutoLogin", "FALSE"));
    }
  }

  public async Task handleJWTTokenExpiration()
  {
    string username = this._secureStorageService.getUsername().Result;
    string password = this._secureStorageService.getPassword().Result;
    if (username != "" && password != "")
    {
      IUserAccount u_acc = Mvx.IoCProvider.Resolve<IUserAccount>();
      u_acc.setAccount(username, password);
      if (UtilityFunctions.IsTokenValid())
      {
        this._userSession.setIsUserLoggedIn(true);
        await this.resolveNextScreenAsync();
      }
      else
      {
        this.IsBusy = true;
        this.CanContinue = false;
        this._userSession.activate(new iService5.Core.Services.User.activationCompletionCallback(this.activationAutoCompletionCallback));
      }
      u_acc = (IUserAccount) null;
      username = (string) null;
      password = (string) null;
    }
    else
    {
      this._loggingService.getLogger().LogAppError(LoggingContext.LOGIN, "Could not fetch user name or password from secure storage", memberName: nameof (handleJWTTokenExpiration), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/LoginViewModel.cs", sourceLineNumber: 183);
      username = (string) null;
      password = (string) null;
    }
  }

  public async Task activationCompletionCallback(SessionActivation res)
  {
    if (res.ToString() == "SUCCESS")
    {
      IPlatformSpecificService current = this._locator.GetPlatformSpecificService();
      current.initialiseScheduledMetadataSession();
      if (this.EnableLoggedIn && this.StayLoggedIn && !this.AutoLoginSession)
      {
        current.isFingerprintValid(new iService5.Core.Services.Platform.fingerprintCompletionCallback(this.fingerprintCompletionCallback));
      }
      else
      {
        this.CanContinue = true;
        this.IsBusy = false;
        if (!this.StayLoggedIn || !this.AutoLoginSession)
        {
          CoreApp.settings.DeleteItem(new Settings("AutoLogin", ""));
          CoreApp.settings.SaveItem(new Settings("AutoLogin", "FALSE"));
        }
        await this.resolveNextScreenAsync();
      }
      int num1 = await this._secureStorageService.setUsername(this.OpecUserName) ? 1 : 0;
      int num2 = await this._secureStorageService.setPassword(this.Password) ? 1 : 0;
      this.LoginValidationColor = "LightGreen";
      current = (IPlatformSpecificService) null;
    }
    else
    {
      Logger logger1 = this._loggingService.getLogger();
      BackendRequestStatus backendRequestStatus = this._userSession.GetBackendRequestStatus();
      string message1 = "Login Failed - " + backendRequestStatus.ToString();
      logger1.LogAppError(LoggingContext.LOGIN, message1, memberName: nameof (activationCompletionCallback), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/LoginViewModel.cs", sourceLineNumber: 226);
      if (this._userSession.GetBackendRequestStatus() == BackendRequestStatus.RES_UnAuthorized)
      {
        this.LoginValidationColor = "White";
        await this._alertService.ShowMessageAlertWithKey("LOGIN_UNAUTHORIZED", AppResource.INFORMATION_TEXT);
      }
      else if (this._userSession.GetBackendRequestStatus() == BackendRequestStatus.RES_TimeOut)
      {
        this.LoginValidationColor = "White";
        Logger logger2 = this._loggingService.getLogger();
        backendRequestStatus = this._userSession.GetBackendRequestStatus();
        string message2 = "Login Failed - The operation has timed out" + backendRequestStatus.ToString();
        logger2.LogAppError(LoggingContext.LOGIN, message2, memberName: nameof (activationCompletionCallback), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/LoginViewModel.cs", sourceLineNumber: 235);
        await this._alertService.ShowMessageAlertWithKey("GENERIC_ERROR_MESSAGE", AppResource.INFORMATION_TEXT);
      }
      else if (this._userSession.GetBackendRequestStatus() == BackendRequestStatus.RES_Exception)
      {
        ServiceError serviceError = this._userSession.getServiceError();
        this._loggingService.getLogger().LogAppError(LoggingContext.BACKEND, $"There occured a problem while sending support mail: {serviceError.errorKey.ToString()}Error Message: {serviceError.errorMessage}", memberName: nameof (activationCompletionCallback), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/LoginViewModel.cs", sourceLineNumber: 241);
        await this._alertService.ShowMessageAlertWithKey("GENERIC_ERROR_MESSAGE", AppResource.INFORMATION_TEXT);
        serviceError = (ServiceError) null;
      }
      else if (this._userSession.GetBackendRequestStatus() == BackendRequestStatus.RES_NInternet)
        this.LoginValidationColor = "White";
      else if (this._userSession.GetBackendRequestStatus() == BackendRequestStatus.RES_CertificateUnknown)
      {
        this.LoginValidationColor = "Red";
        await this._alertService.ShowMessageAlertWithKey("SERVER_TRUST_FAILURE", AppResource.WARNING_TEXT);
      }
      else if (this._userSession.GetBackendRequestStatus() == BackendRequestStatus.RES_NLocalCredentials)
      {
        this.LoginValidationColor = "Yellow";
        await this._alertService.ShowMessageAlertWithKey("NO_LOCAL_CREDENTIALS", AppResource.WARNING_TEXT);
      }
      else if (this._userSession.GetBackendRequestStatus() == BackendRequestStatus.RES_NLocal)
      {
        this.LoginFailed = true;
        this.LoginValidationColor = "Red";
        await this._alertService.ShowMessageAlertWithKey("LOGIN_UNAUTHORIZED", AppResource.WARNING_TEXT);
      }
      else
      {
        this.LoginFailed = true;
        this._secureStorageService.Remove(SecureStorageKeys.USERNAME);
        this._secureStorageService.Remove(SecureStorageKeys.PASSWORD);
        await this._alertService.ShowMessageAlertWithKey("LOGIN_UNAUTHORIZED", AppResource.WARNING_TEXT);
        Logger logger3 = this._loggingService.getLogger();
        backendRequestStatus = this._userSession.GetBackendRequestStatus();
        string message3 = backendRequestStatus.ToString();
        logger3.LogAppInformation(LoggingContext.BACKEND, message3, memberName: nameof (activationCompletionCallback), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/LoginViewModel.cs", sourceLineNumber: 276);
        this.LoginValidationColor = "Pink";
      }
      this.CanContinue = true;
      this.IsBusy = false;
    }
  }

  private async Task resolveNextScreenAsync()
  {
    Settings lastUpdate;
    try
    {
      lastUpdate = CoreApp.settings.GetItem("lastCheck");
    }
    catch (Exception ex)
    {
      DateTime dateTime = DateTime.Today;
      dateTime = dateTime.AddDays(-16.0);
      lastUpdate = new Settings("lastCheck", dateTime.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    }
    DateTime now;
    int num;
    if (lastUpdate != null && lastUpdate.Value != "")
    {
      string str1 = lastUpdate.Value;
      now = DateTime.Now;
      string str2 = now.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      num = str1 == str2 ? 1 : 0;
    }
    else
      num = 0;
    if (num != 0)
    {
      Logger logger = this._loggingService.getLogger();
      string str3 = lastUpdate.Value;
      now = DateTime.Now;
      string str4 = now.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      string message = $"Last update {str3} today {str4}";
      logger.LogAppInformation(LoggingContext.BACKEND, message, memberName: nameof (resolveNextScreenAsync), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/LoginViewModel.cs", sourceLineNumber: 298);
      if (this._userSession.ShouldGoToPrepareWork())
      {
        this._navigationService.Navigate<WorkPreparationViewModel>((IMvxBundle) null, new CancellationToken());
        lastUpdate = (Settings) null;
      }
      else
      {
        this._navigationService.Navigate<TabViewModel>((IMvxBundle) null, new CancellationToken());
        lastUpdate = (Settings) null;
      }
    }
    else if (!this._userSession.IsHostReachable() && lastUpdate != null && lastUpdate.Value != "")
    {
      this._navigationService.Navigate<StatusViewModel>((IMvxBundle) null, new CancellationToken());
      lastUpdate = (Settings) null;
    }
    else
    {
      if (DeviceInfo.Platform == DevicePlatform.Unknown)
        await this.CheckHostReachable();
      else
        MainThread.BeginInvokeOnMainThread((Action) (async () => await this.CheckHostReachable()));
      lastUpdate = (Settings) null;
    }
  }

  private async Task CheckHostReachable()
  {
    bool exitLoop = false;
    while (!exitLoop)
    {
      if (this._userSession.IsHostReachable())
      {
        exitLoop = true;
        this._navigationService.Navigate<StatusViewModel>((IMvxBundle) null, new CancellationToken());
      }
      else
      {
        int num = await this._alertService.ShowMessageAlertWithKey("NO_INTERNET", AppResource.ERROR_TITLE, AppResource.CANCEL_LABEL, AppResource.RETRY_LABEL, (Action<bool>) (bobj =>
        {
          if (!bobj)
            return;
          if (this._metadataService.isDBValid())
          {
            if (this._userSession.ShouldGoToPrepareWork())
              this._navigationService.Navigate<WorkPreparationViewModel>((IMvxBundle) null, new CancellationToken());
            else
              this._navigationService.Navigate<TabViewModel>((IMvxBundle) null, new CancellationToken());
          }
          exitLoop = true;
        })) ? 1 : 0;
      }
    }
  }

  private void CheckLocalMigration()
  {
    LocalMigrationHelper localMigrationHelper = new LocalMigrationHelper();
    if (!localMigrationHelper.IsLocalMigrationRequired().Result)
      return;
    localMigrationHelper.MigrateExistingKeyValuesFromSecureStorage();
  }

  public async Task activationAutoCompletionCallback(SessionActivation res)
  {
    if (res.ToString() == "SUCCESS")
    {
      this.CanContinue = true;
      this.IsBusy = false;
      this.LoginValidationColor = "LightGreen";
      await this.resolveNextScreenAsync();
    }
    else
    {
      if (this._userSession.GetBackendRequestStatus() == BackendRequestStatus.RES_NInternet)
        this.LoginValidationColor = "White";
      else if (this._userSession.GetBackendRequestStatus() == BackendRequestStatus.RES_NLocalCredentials)
      {
        this.LoginValidationColor = "Yellow";
        await this._alertService.ShowMessageAlertWithKey("NO_LOCAL_CREDENTIALS", AppResource.WARNING_TEXT);
        this._loggingService.getLogger().LogAppInformation(LoggingContext.BACKEND, this._userSession.GetBackendRequestStatus().ToString(), memberName: nameof (activationAutoCompletionCallback), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/LoginViewModel.cs", sourceLineNumber: 386);
      }
      else if (this._userSession.GetBackendRequestStatus() == BackendRequestStatus.RES_NLocal)
      {
        this.LoginFailed = true;
        this.LoginValidationColor = "Red";
        await this._alertService.ShowMessageAlertWithKey("LOGIN_UNAUTHORIZED", AppResource.WARNING_TEXT);
      }
      else if (this._userSession.GetBackendRequestStatus() == BackendRequestStatus.RES_TimeOut)
      {
        this.LoginValidationColor = "White";
        this._loggingService.getLogger().LogAppError(LoggingContext.LOGIN, "Login Failed - The operation has timed out" + this._userSession.GetBackendRequestStatus().ToString(), memberName: nameof (activationAutoCompletionCallback), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/LoginViewModel.cs", sourceLineNumber: 397);
        await this._alertService.ShowMessageAlertWithKey("GENERIC_ERROR_MESSAGE", AppResource.INFORMATION_TEXT);
      }
      else if (this._userSession.GetBackendRequestStatus() == BackendRequestStatus.RES_Exception)
      {
        ServiceError serviceError = this._userSession.getServiceError();
        this._loggingService.getLogger().LogAppError(LoggingContext.LOGIN, $"There occured a problem while sending support mail: {serviceError.errorKey.ToString()}Error Message: {serviceError.errorMessage}", memberName: nameof (activationAutoCompletionCallback), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/LoginViewModel.cs", sourceLineNumber: 403);
        await this._alertService.ShowMessageAlertWithKey("GENERIC_ERROR_MESSAGE", AppResource.INFORMATION_TEXT);
        serviceError = (ServiceError) null;
      }
      else
      {
        this.LoginFailed = true;
        this._secureStorageService.Remove(SecureStorageKeys.USERNAME);
        this._secureStorageService.Remove(SecureStorageKeys.PASSWORD);
        this._loggingService.getLogger().LogAppInformation(LoggingContext.BACKEND, this._userSession.GetBackendRequestStatus().ToString(), memberName: nameof (activationAutoCompletionCallback), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/LoginViewModel.cs", sourceLineNumber: 412);
        this.LoginValidationColor = "Pink";
      }
      this.CanContinue = true;
      this.IsBusy = false;
    }
  }

  public void deactivationAutoCompletionCallback(SessionActivation res)
  {
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

  public bool EnableLoggedIn
  {
    get => this._EnableLoggedIn;
    set
    {
      this._EnableLoggedIn = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.EnableLoggedIn));
    }
  }

  public bool StayLoggedIn
  {
    get => this._StayLoggedIn;
    set
    {
      this._StayLoggedIn = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.StayLoggedIn));
    }
  }

  public string LoginValidationColor
  {
    get => this._loginValidationColor;
    set
    {
      this._loginValidationColor = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.LoginValidationColor));
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

  public bool MetadataFailed
  {
    get => this._MetadataFailed;
    set
    {
      this._MetadataFailed = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.MetadataFailed));
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

  public IMvxAsyncCommand NavigateCommand { get; internal set; }

  public string LogLabelPrompt
  {
    get => this._LogLabelPrompt;
    private set
    {
      this._LogLabelPrompt = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.LogLabelPrompt));
    }
  }

  public string AutoLoginLabel
  {
    get => this._AutoLoginLabel;
    private set
    {
      this._AutoLoginLabel = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.AutoLoginLabel));
    }
  }

  public string LoginButtonLabel
  {
    get => this._LoginButtonLabel;
    private set
    {
      this._LoginButtonLabel = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.LoginButtonLabel));
    }
  }

  public string LoginErrorLabel
  {
    get => this._LoginErrorLabel;
    private set
    {
      this._LoginErrorLabel = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.LoginErrorLabel));
    }
  }

  public string OpecUserName
  {
    get => this._OpecUserName;
    set
    {
      this._OpecUserName = value;
      this.CanContinue = this._Password.Length != 0 && this._OpecUserName.Length != 0;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.OpecUserName));
    }
  }

  public string Password
  {
    get => this._Password;
    set
    {
      this._Password = value;
      this.CanContinue = this._Password.Length != 0 && this._OpecUserName.Length != 0;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.Password));
    }
  }

  public string AppVersionLabel
  {
    get => this._AppVersionLabel;
    private set
    {
      this._AppVersionLabel = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.AppVersionLabel));
    }
  }

  public ICommand SubmitCommand { internal set; get; }

  public ICommand NeedAccountCommand { internal set; get; }

  public ICommand LegalInfoCommand { internal set; get; }

  public bool CanContinue
  {
    protected set
    {
      this._CanContinue = value;
      this.IsBtnDisabled = !value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.CanContinue));
    }
    get => this._CanContinue;
  }

  public bool IsBtnDisabled
  {
    protected set
    {
      this._IsBtnDisabled = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.IsBtnDisabled));
    }
    get => this._IsBtnDisabled;
  }

  public bool IsUsernameReadOnly
  {
    protected set
    {
      this._IsUsernameReadOnly = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.IsUsernameReadOnly));
    }
    get => this._IsUsernameReadOnly;
  }

  public bool IsPasswordReadOnly
  {
    protected set
    {
      this._IsPasswordReadOnly = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.IsPasswordReadOnly));
    }
    get => this._IsPasswordReadOnly;
  }

  public bool NeedAccountBtnVisible
  {
    protected set
    {
      this._NeedAccountBtnVisible = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.NeedAccountBtnVisible));
    }
    get => this._NeedAccountBtnVisible;
  }

  public bool LegalInfoBtnVisible
  {
    protected set
    {
      this._LegalInfoBtnVisible = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.LegalInfoBtnVisible));
    }
    get => this._LegalInfoBtnVisible;
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

  public bool AutoLoginSession { get; private set; }

  public void OnSubmit()
  {
    this.IsBusy = true;
    this.CanContinue = false;
    this.LoginFailed = false;
    Task.Run((Action) (() =>
    {
      this._userAccount.setAccount(this.OpecUserName, this.Password);
      this._userSession.activate(new iService5.Core.Services.User.activationCompletionCallback(this.activationCompletionCallback));
    }));
  }

  public void OnClickingLegalInfo()
  {
    Device.BeginInvokeOnMainThread((Action) (() =>
    {
      if (!this.AreButtonsEnabled)
        return;
      this.AreButtonsEnabled = false;
      this._navigationService.Navigate<LegalInfoListViewModel>((IMvxBundle) null, new CancellationToken());
    }));
  }

  public void OnClickingNeedAccount()
  {
    if (!this.AreButtonsEnabled)
      return;
    this.AreButtonsEnabled = false;
    this._navigationService.Navigate<NeedAccountViewModel>((IMvxBundle) null, new CancellationToken());
  }

  public virtual async Task Initialize() => await base.Initialize();

  public void Init(LoginViewModel.DetailParameters parameters)
  {
  }

  public override void ViewAppeared()
  {
    base.ViewAppeared();
    bool? nullable1 = CrossBreachDetector.Current.IsRooted();
    int num;
    if (FeatureConfiguration.EnabledJailBreakCheck)
    {
      bool? nullable2 = nullable1;
      bool flag = true;
      num = nullable2.GetValueOrDefault() == flag & nullable2.HasValue ? 1 : 0;
    }
    else
      num = 0;
    if (num != 0)
    {
      this.IsBtnDisabled = true;
      this.IsUsernameReadOnly = true;
      this.IsPasswordReadOnly = true;
      this.OpecUserName = "";
      this.Password = "";
      this.NeedAccountBtnVisible = false;
      this.LegalInfoBtnVisible = false;
      this._alertService.ShowMessageAlertWithKey("JAILBROKEN_WARNING_TEXT", AppResource.WARNING_TITLE);
    }
    else
    {
      this.AreButtonsEnabled = true;
      Settings settings = CoreApp.settings.GetItem("UserLoggedOutStatus");
      if (settings != null && settings.Value == "Logout")
      {
        this.OpecUserName = "";
        this.Password = "";
      }
      else
      {
        if (this.MetadataFailed)
        {
          this.MetadataFailed = false;
          this._alertService.ShowMessageAlertWithKey("METADATA_DL_FAIL_MSG", AppResource.WARNING_TEXT);
        }
        if (this.CanContinue && this.StayLoggedIn && !this._userSession.isActive())
          Task.Run((Action) (() => this._locator.GetPlatformSpecificService().isFingerprintValid(new iService5.Core.Services.Platform.fingerprintCompletionCallback(this.autoLoginFingerprintCompletionCallback))));
      }
    }
  }

  public class DetailParameters
  {
    public int Index { get; set; }
  }
}
