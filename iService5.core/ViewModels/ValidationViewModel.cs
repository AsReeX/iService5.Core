// Decompiled with JetBrains decompiler
// Type: iService5.Core.ViewModels.ValidationViewModel
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
using iService5.Core.Services.VersionReport;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Windows.Input;
//using Xamarin.Forms;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

#nullable disable
namespace iService5.Core.ViewModels;

public class ValidationViewModel : MvxViewModel<string>
{
  private readonly IUserSession _userSession;
  private readonly IPlatformSpecificServiceLocator _locator;
  private readonly IAlertService _alertService;
  private readonly IMetadataService _metadataService;
  private readonly ISecureStorageService _secureStorageService;
  private readonly IAppliance _appliance;
  private readonly Settings user;
  private string applianceType;
  private readonly IMvxNavigationService _navigationService;
  private string _NavText = AppResource.APPLIANCE_FLASH_BACK_TEXT;
  private string _WifiStatus;
  private string _RepairEnumber;
  private bool _LoginFailed = false;
  private bool _EnableLoggedIn;
  private bool _CanContinue;
  private string _OpecUserName = "";
  private string _Password = "";
  private string _ConnectedColor = "Gray";
  private bool _SmmConnectShow = true;
  private string _LoginButtonLabel;
  private string _LogLabelPrompt = AppResource.WIFI_PAGE_LOGIN_PROMPT;

  public ILoggingService _loggingService { get; private set; }

  public bool _viewstatus { get; set; }

  public override void Prepare(string connectedApplianceType)
  {
    this.applianceType = connectedApplianceType;
    this.setPageTitle();
  }

  public ValidationViewModel(
    IMvxNavigationService navigationService,
    IPlatformSpecificServiceLocator locator,
    IUserSession userSession,
    ILoggingService loggingService,
    IAppliance appliance,
    IShortTextsService _ShortTextsService,
    IVersionReport versionReport,
    IAlertService alertService,
    IMetadataService metadataService,
    ISecureStorageService secureStorageService)
  {
    this.CanContinue = false;
    if (_ShortTextsService != null)
      this.LoginButtonLabel = AppResource.WIFI_PAGE_AUTH_BUTTON;
    this._locator = locator;
    this._metadataService = metadataService;
    this._loggingService = loggingService;
    this._userSession = userSession;
    this._appliance = appliance;
    this._navigationService = navigationService;
    this._alertService = alertService;
    this._secureStorageService = secureStorageService;
    this.SubmitCommand = (ICommand) new Command(new Action(this.OnSubmit));
    this.BackButton = (ICommand) new Command(new Action<object>(this.BackNavigation));
  }

  public ICommand SubmitCommand { protected set; get; }

  public ICommand BackButton { protected set; get; }

  private void BackNavigation(object obj)
  {
    this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
  }

  private void OnSubmit()
  {
    this.CanContinue = false;
    string result = this._secureStorageService.getUsername().Result;
    if (!this._secureStorageService.getPassword().Result.Equals(this.Password))
    {
      this._loggingService.getLogger().LogAppError(LoggingContext.LOGIN, "Login Failed - " + this._userSession.GetBackendRequestStatus().ToString(), memberName: nameof (OnSubmit), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ValidationViewModel.cs", sourceLineNumber: 91);
      this.DisplayPopup();
      this.CanContinue = false;
    }
    else
      this.AuthenticationSuccess();
  }

  public void AuthenticationSuccess()
  {
    this.CanContinue = true;
    this._navigationService.Navigate<WifiInfoViewModel, string>(this.applianceType, (IMvxBundle) null, new CancellationToken());
    this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
  }

  private void DisplayPopup()
  {
    this._alertService.ShowMessageAlertWithKey("WIFI_PAGE_LOGIN_UNAUTHORIZED", AppResource.WARNING_TEXT);
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

  public string NavText
  {
    get => this._NavText;
    internal set
    {
      this._NavText = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.NavText));
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

  public bool LoginFailed
  {
    get => this._LoginFailed;
    set
    {
      this._LoginFailed = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.LoginFailed));
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

  public bool CanContinue
  {
    protected set
    {
      this._CanContinue = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.CanContinue));
    }
    get => this._CanContinue;
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

  public string ConnectedColor
  {
    get => this._ConnectedColor;
    internal set
    {
      this._ConnectedColor = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.ConnectedColor));
    }
  }

  public bool SmmConnectShow
  {
    get => this._SmmConnectShow;
    internal set
    {
      this._SmmConnectShow = !value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.SmmConnectShow));
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

  public string LogLabelPrompt
  {
    get => this._LogLabelPrompt;
    private set
    {
      this._LogLabelPrompt = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.LogLabelPrompt));
    }
  }

  internal void updateStatus()
  {
    this.WifiStatus = this._appliance.StatusOfConnection;
    this.ConnectedColor = this._appliance.ConnectedColor;
    this.SmmConnectShow = this._appliance.boolStatusOfConnection;
  }

  internal void setPageTitle()
  {
    if (this.applianceType.Equals("BRIDGE"))
      this.RepairEnumber = AppResource.BRIDGE_HEADER;
    else
      this.RepairEnumber = this._userSession.getEnumberSession();
  }

  public bool AlertShown { get; private set; }

  public override void ViewAppeared()
  {
    base.ViewAppeared();
    this._viewstatus = true;
    this._locator.GetPlatformSpecificService().GetLocationConsent();
    this._appliance.CheckConnectivity(new connectivityUpdateCallback(this.updateStatus));
  }
}
