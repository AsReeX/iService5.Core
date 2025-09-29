// Decompiled with JetBrains decompiler
// Type: iService5.Core.ViewModels.WifiInfoViewModel
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
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
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

public class WifiInfoViewModel : MvxViewModel<string>
{
  private string applianceType = string.Empty;
  private readonly IMvxNavigationService _navigationService;
  private readonly IPlatformSpecificServiceLocator _locator;
  private readonly IAppliance _appliance;
  private readonly IAlertService _alertService;
  private readonly IUserSession _userSession;
  private readonly ISecureStorageService _secureStorageService;
  private string _NavText = AppResource.APPLIANCE_FLASH_BACK_TEXT;
  private string _SelectColor = "";
  private string _HintForPass = AppResource.WIFI_INFO_PAGE_HINT;
  private string _PasswordLabel = AppResource.PASSWORD_TEXT;
  private string _PasswordValue;
  private string _WifiStatus;
  private string _RepairEnumber;
  private string _ConnectedColor = "Gray";
  private bool _SmmConnectShow = true;

  public virtual async Task Initialize() => await base.Initialize();

  public bool _viewstatus { get; set; }

  public bool _SmmWithWifi { get; set; }

  public string _ConnectionGraphic { get; internal set; }

  public override void Prepare(string connectedApplianceType)
  {
    this.applianceType = connectedApplianceType;
    this.setPageTitle();
    this.setWifiPasswordDescription();
    this.setPasswordValue();
  }

  public WifiInfoViewModel(
    IMvxNavigationService navigationService,
    IShortTextsService _SService,
    ILoggingService loggingService,
    IMetadataService metadataService,
    IPlatformSpecificServiceLocator locator,
    IUserSession userSession,
    IAppliance appliance,
    IAlertService alertService,
    ISecureStorageService secureStorageService)
  {
    this.CopyPasswordValue = (ICommand) new Command((Action) (async () => await this.GetPasswordValue()));
    this.BackButton = (ICommand) new Command(new Action<object>(this.BackNavigation));
    this._navigationService = navigationService;
    this._appliance = appliance;
    this._locator = locator;
    this._userSession = userSession;
    this._RepairEnumber = userSession.getEnumberSession();
    this.SelectColor = "White";
    this._alertService = alertService;
    this._secureStorageService = secureStorageService;
  }

  internal void BackNavigation(object obj)
  {
    this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
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

  public string SelectColor
  {
    get => this._SelectColor;
    internal set
    {
      this._SelectColor = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.SelectColor));
    }
  }

  public string HintForPass
  {
    get => this._HintForPass;
    internal set
    {
      this._HintForPass = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.HintForPass));
    }
  }

  public string PasswordLabel
  {
    get => this._PasswordLabel;
    internal set
    {
      this._PasswordLabel = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.PasswordLabel));
    }
  }

  public string PasswordValue
  {
    get => this._PasswordValue;
    internal set
    {
      this._PasswordValue = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.PasswordValue));
    }
  }

  internal async Task GetPasswordValue()
  {
    this.SelectColor = "#AFA3A3";
    await Task.Delay(500);
    await Clipboard.SetTextAsync(this.PasswordValue);
    this._alertService.ShowMessageAlertWithKey("WIFI_INFO_ALERT_MESSAGE", AppResource.INFORMATION_TEXT);
    this.SelectColor = "White";
  }

  public ICommand CopyPasswordValue { internal set; get; }

  public ICommand BackButton { internal set; get; }

  public override void ViewDisappearing()
  {
    base.ViewDisappearing();
    this._viewstatus = false;
  }

  public override void ViewDisappeared()
  {
    base.ViewDisappeared();
    this._viewstatus = false;
  }

  public override void ViewAppearing()
  {
    base.ViewAppearing();
    this._viewstatus = true;
  }

  public override void ViewAppeared()
  {
    base.ViewAppeared();
    this._viewstatus = true;
    this._locator.GetPlatformSpecificService().GetLocationConsent();
    this._appliance.CheckConnectivity(new connectivityUpdateCallback(this.updateStatus));
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

  internal void updateStatus()
  {
    if (this._SmmWithWifi)
    {
      this.WifiStatus = this._appliance.StatusOfConnection;
      this.ConnectedColor = this._appliance.ConnectedColor;
      this.SmmConnectShow = this._appliance.boolStatusOfConnection;
    }
    else
    {
      this.WifiStatus = this._appliance.StatusOfBridgeConnection;
      this.ConnectedColor = this._appliance.ConnectedColor;
      this.SmmConnectShow = this._appliance.boolStatusOfBridgeConnection;
    }
  }

  internal void setPageTitle()
  {
    if (this.applianceType.Equals("BRIDGE"))
      this.RepairEnumber = AppResource.BRIDGE_HEADER;
    else
      this.RepairEnumber = this._userSession.getEnumberSession();
  }

  internal void setPasswordValue()
  {
    if (this.applianceType.Equals("SMM"))
      this.PasswordValue = this._secureStorageService.GetWiFiPasswordFromSecureStorage().Result;
    else
      this.PasswordValue = this._secureStorageService.GetWiFiBridgePasswordFromSecureStorage().Result;
  }

  internal void setWifiPasswordDescription()
  {
    if (this.applianceType.Equals("SMM"))
      return;
    this.HintForPass = AppResource.BRIDGE_WIFI_INFO_PAGE_HINT;
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
}
