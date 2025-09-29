// Decompiled with JetBrains decompiler
// Type: iService5.Core.ViewModels.InAppBrowserViewModel
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Helpers;
using iService5.Core.Services.Platform;
using iService5.Core.Services.User.ActivityLogging;
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

public class InAppBrowserViewModel : MvxViewModel<DetailNavigationArgs>
{
  private readonly IMvxNavigationService _navigationService;
  private readonly IAlertService _alertService;
  private readonly IPlatformSpecificServiceLocator _locator;
  private readonly ILoggingService _loggingService;
  private bool _isFromControl = false;
  private string _WebUrl;
  private string _NavigationTitle;

  public ICommand GoToBridgePage { internal set; get; }

  public InAppBrowserViewModel(
    IMvxNavigationService navigationService,
    IAlertService alertService,
    IPlatformSpecificServiceLocator locator,
    ILoggingService loggingService)
  {
    this.GoToBridgePage = (ICommand) new Command(new Action(this.VisitBridgePage));
    this._navigationService = navigationService;
    this._alertService = alertService;
    this._locator = locator;
    this._loggingService = loggingService;
  }

  public bool IsFromControl
  {
    get => this._isFromControl;
    internal set
    {
      this._isFromControl = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.IsFromControl));
    }
  }

  public override void Prepare(DetailNavigationArgs parameter)
  {
    string str = "http://" + this._locator.GetPlatformSpecificService().GetIp();
    switch (parameter.detailNavigationPageType)
    {
      case DetailNavigationPageType.BridgeSettings:
        str += "/Admin.php";
        this.NavigationTitle = AppResource.INAPPBROWSER_BRIDGE_SETTING_TEXT;
        break;
      case DetailNavigationPageType.DataLogger:
        str += "/datalog/";
        this.NavigationTitle = AppResource.DATA_LOGGER_TEXT;
        break;
      case DetailNavigationPageType.BridgeUpgrade:
        str += "/Admin.php";
        this.NavigationTitle = AppResource.BRIDGE_UPGRADE_TEXT;
        break;
    }
    this.WebUrl = str;
  }

  public string WebUrl
  {
    get => this._WebUrl;
    internal set
    {
      this._WebUrl = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.WebUrl));
    }
  }

  public string NavigationTitle
  {
    get => this._NavigationTitle;
    internal set
    {
      this._NavigationTitle = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.NavigationTitle));
    }
  }

  internal void VisitBridgePage()
  {
    if (this.IsFromControl)
      this.CheckBridgeMode();
    else
      this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
  }

  public void WebPageError()
  {
    this._alertService.ShowMessageAlertWithKey("URL_FAIL_MSG", AppResource.ERROR_TITLE);
  }

  private async void CheckBridgeMode()
  {
    BridgeWifiResponse response = await UtilityFunctions.EnableBridgeWIFIUsingApi(this._locator, this._loggingService);
    if (!response.isSuccess)
    {
      string errorMessage = !UtilityFunctions.IsLocalNetworkPemrissionError(response.errorMessage) ? AppResource.SSH_COMMAND_ERROR : "HA_INFO_RETRY_MESSAGE_IOS_14";
      this._alertService.ShowMessageAlertWithKeyFromService(errorMessage, AppResource.INFORMATION_TEXT, AppResource.RETRY_LABEL, AppResource.CANCEL_LABEL, (Action<bool>) (shouldRetry =>
      {
        if (!shouldRetry)
          return;
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Retrying to get data from home appliance.", memberName: nameof (CheckBridgeMode), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/InAppBrowserViewModel.cs", sourceLineNumber: 122);
        this.CheckBridgeMode();
      }));
      errorMessage = (string) null;
      response = (BridgeWifiResponse) null;
    }
    else
    {
      this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
      response = (BridgeWifiResponse) null;
    }
  }
}
