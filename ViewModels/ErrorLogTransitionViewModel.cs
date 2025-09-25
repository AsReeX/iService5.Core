// Decompiled with JetBrains decompiler
// Type: iService5.Core.ViewModels.ErrorLogTransitionViewModel
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Appliance;
using iService5.Core.Services.Data;
using iService5.Core.Services.Localisation;
using iService5.Core.Services.Platform;
using iService5.Core.Services.User;
using iService5.Core.Services.User.ActivityLogging;
using iService5.Ssh.models;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace iService5.Core.ViewModels;

public class ErrorLogTransitionViewModel : MvxViewModel
{
  private readonly IMvxNavigationService _navigationService;
  private readonly IUserSession _userSession;
  private readonly IShortTextsService _ShortTextsService = (IShortTextsService) null;
  private readonly IMetadataService _metadataService;
  private readonly IPlatformSpecificServiceLocator _locator;
  private readonly ILoggingService _loggingService;
  private readonly Is5SshWrapper _sshWrapper;
  private string _CollectingErrorLogLabel = AppResource.ERROR__TRANSITION_PAGE_COLLECTING;

  public virtual async Task Initialize() => await base.Initialize();

  public ErrorLogTransitionViewModel(
    IMvxNavigationService navigationService,
    IShortTextsService _SService,
    ILoggingService loggingService,
    IMetadataService metadataService,
    IPlatformSpecificServiceLocator locator,
    IUserSession userSession,
    Is5SshWrapper sshWrapper)
  {
    this._ShortTextsService = _SService;
    this._userSession = userSession;
    this._metadataService = metadataService;
    this._navigationService = navigationService;
    this._locator = locator;
    this._loggingService = loggingService;
    this._sshWrapper = sshWrapper;
  }

  public string CollectingErrorLogLabel
  {
    get => this._CollectingErrorLogLabel;
    internal set
    {
      this._CollectingErrorLogLabel = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.CollectingErrorLogLabel));
    }
  }

  public override void ViewAppeared()
  {
    base.ViewAppeared();
    Task.Factory.StartNew((Action) (() =>
    {
      Thread.Sleep(1000);
      this._sshWrapper.IPAddress = this._locator.GetPlatformSpecificService().GetIp();
      this._navigationService.Navigate<ErrorLogViewModel, SshResponse<object>>(this._sshWrapper.GetErrorLog(), (IMvxBundle) null, new CancellationToken());
      this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
    }));
  }
}
