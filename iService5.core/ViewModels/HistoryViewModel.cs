// Decompiled with JetBrains decompiler
// Type: iService5.Core.ViewModels.HistoryViewModel
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Data;
using iService5.Core.Services.Helpers;
using iService5.Core.Services.Localisation;
using iService5.Core.Services.Platform;
using iService5.Core.Services.User;
using iService5.Core.Services.User.ActivityLogging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace iService5.Core.ViewModels;

public class HistoryViewModel : MvxViewModel
{
  private bool displayHistoryList = false;
  private readonly IPlatformSpecificServiceLocator _locator;
  private readonly IUserSession _userSession;
  private readonly ILoggingService _loggingService;
  private readonly IMvxNavigationService _navigationService;
  private List<History> _Histories = new List<History>();
  private string historyTitle = AppResource.TAB_TITLE_HISTORY;
  private string noHistoryText = AppResource.NO_HISTORY_TO_DISPLAY;
  private History _ItemSelected;
  private bool _areButtonsEnabled = true;

  public HistoryViewModel(
    IMvxNavigationService navigationService,
    IShortTextsService _ShortTextsService,
    IPlatformSpecificServiceLocator locator,
    IUserSession userSession,
    ILoggingService loggingService)
  {
    this._locator = locator;
    this._userSession = userSession;
    this._loggingService = loggingService;
    this._navigationService = navigationService;
  }

  public virtual async Task Initialize()
  {
    await base.Initialize();
    this.GetHistory();
    this.AreButtonsEnabled = true;
  }

  public void GetHistory()
  {
    this.Histories = CoreApp.history.GetHistoryList();
    if (this.Histories.Count <= 0)
      return;
    this.displayHistoryList = true;
  }

  public List<History> Histories
  {
    get => this._Histories;
    set
    {
      this._Histories = value;
      this.RaisePropertyChanged<List<History>>((Expression<Func<List<History>>>) (() => this.Histories));
    }
  }

  public string HistoryTitle
  {
    get => this.historyTitle;
    internal set
    {
      this.historyTitle = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.HistoryTitle));
    }
  }

  public bool DisplayHistoryList
  {
    get => this.displayHistoryList;
    internal set
    {
      this.displayHistoryList = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.DisplayHistoryList));
    }
  }

  public string NoHistoryText
  {
    get => this.noHistoryText;
    internal set
    {
      this.noHistoryText = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.NoHistoryText));
    }
  }

  public History ObjItemSelected
  {
    get => this._ItemSelected;
    set
    {
      if (this._ItemSelected == value)
        return;
      this._ItemSelected = value;
      if (this.AreButtonsEnabled)
      {
        this.AreButtonsEnabled = false;
        try
        {
          IMvxNavigationService navigationService = this._navigationService;
          DetailNavigationArgs detailNavigationArgs = new DetailNavigationArgs();
          detailNavigationArgs.historySessionID = this._ItemSelected.sessionID;
          CancellationToken cancellationToken = new CancellationToken();
          navigationService.Navigate<HistoryDetailViewModel, DetailNavigationArgs>(detailNavigationArgs, (IMvxBundle) null, cancellationToken);
        }
        catch (Exception ex)
        {
          Debug.WriteLine("exception : " + ex.Message);
        }
      }
      this.RaisePropertyChanged<History>((Expression<Func<History>>) (() => this.ObjItemSelected));
    }
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

  public override void ViewDisappearing() => base.ViewDisappearing();

  public override void ViewAppearing()
  {
    base.ViewAppearing();
    this.GetHistory();
  }

  public override void ViewAppeared()
  {
    base.ViewAppeared();
    this.AreButtonsEnabled = true;
  }
}
