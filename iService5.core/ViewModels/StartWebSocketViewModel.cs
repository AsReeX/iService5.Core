// Decompiled with JetBrains decompiler
// Type: iService5.Core.ViewModels.StartWebSocketViewModel
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Helpers;
using iService5.Core.Services.Platform;
using iService5.Core.Services.User;
using iService5.Core.Services.User.ActivityLogging;
using iService5.Core.Services.WebSocket;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
//using Xamarin.Forms;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

#nullable disable
namespace iService5.Core.ViewModels;

public class StartWebSocketViewModel : MvxViewModel
{
  private readonly IPlatformSpecificServiceLocator _locator;
  private readonly IMvxNavigationService _navigationService;
  private readonly IAlertService _alertService;
  private readonly IUserSession _userSession;
  private readonly ILoggingService _loggingService;
  private readonly ISecureStorageService _secureStorageService;
  private readonly ISecurityService _securityService;
  private readonly IWebSocketService _webSocketService;
  private bool displayActivityIndicator = true;

  public virtual async Task Initialize() => await base.Initialize();

  public StartWebSocketViewModel(
    IMvxNavigationService navigationService,
    IAlertService alertService,
    IPlatformSpecificServiceLocator locator,
    ILoggingService loggingService,
    IUserSession userSession,
    ISecureStorageService secureStorageService,
    ISecurityService securityService,
    IWebSocketService webSocketService)
  {
    this._locator = locator;
    this.GoToSettingPage = (ICommand) new Command(new Action(this.VisitSettingPage));
    this._navigationService = navigationService;
    this._alertService = alertService;
    this._loggingService = loggingService;
    this._userSession = userSession;
    this._secureStorageService = secureStorageService;
    this._securityService = securityService;
    this._webSocketService = webSocketService;
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

  public ICommand GoToSettingPage { internal set; get; }

  public override void ViewAppeared()
  {
    base.ViewAppeared();
    this.InitializeWebSocket();
  }

  private void InitializeWebSocket()
  {
    Task.Run((Func<Task>) (() =>
    {
      if (this._webSocketService.StartWebSocket())
        this._alertService.ShowMessageAlertWithKey("Web Socket Initialization successful", AppResource.INFORMATION_TEXT);
      else
        this._alertService.ShowMessageAlertWithKey("Web Socket couldn't be initalized succesfully", AppResource.WARNING_TEXT);
      this.DisplayActivityIndicator = false;
      this.VisitSettingPage();
      return Task.CompletedTask;
    }));
  }

  internal void VisitSettingPage()
  {
    this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
  }
}
