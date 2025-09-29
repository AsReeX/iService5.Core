// Decompiled with JetBrains decompiler
// Type: iService5.Core.ViewModels.ReDownloadEnumberViewModel
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Data;
using iService5.Core.Services.Helpers;
using iService5.Core.Services.User;
using iService5.Core.Services.User.ActivityLogging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
//using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Windows.Input;
//using Xamarin.Essentials;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

#nullable disable
namespace iService5.Core.ViewModels;

public class ReDownloadEnumberViewModel : MvxViewModel
{
  private readonly IMetadataService _metadataService;
  private readonly IUserSession _userSession;
  private readonly IMvxNavigationService _navigationService;
  private readonly ILoggingService _loggingService;
  private string _GoButtonText;
  private List<material> _eNumbersList;
  private bool _IsReDownloadEnabled = true;
  private bool _isrefreshing;
  private double _listViewHeightRequest = 10.0;
  private material _ItemSelected;
  private bool _displaylist;
  private string _eNumberLabelText;
  private List<material> _enumbersListInit;
  internal string _eNumber = "";

  public ICommand GoBackCommand { get; internal set; }

  public ICommand GoToDownloadViewCommand { protected set; get; }

  public ReDownloadEnumberViewModel(
    IMetadataService metadataService,
    IMvxNavigationService navigationService,
    IUserSession userSession,
    ILoggingService loggingService)
  {
    this.eNumberLabelText = AppResource.HOME_PAGE_ENUMBER_LABEL_TEXT;
    this._eNumbersList = new List<material>();
    this._metadataService = metadataService;
    this._navigationService = navigationService;
    this._loggingService = loggingService;
    this._userSession = userSession;
    this.GoButtonText = AppResource.RE_DOWNLOAD;
    this.GoBackCommand = (ICommand) new Command(new Action(this.GoBack));
    this.GoToDownloadViewCommand = (ICommand) new Command(new Action(this.GoToDownloadView));
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

  public List<material> eNumbersList
  {
    get => this._eNumbersList;
    internal set
    {
      this._eNumbersList = value;
      this.RaisePropertyChanged<List<material>>((Expression<Func<List<material>>>) (() => this.eNumbersList));
      if (this._eNumbersList.Count != 0)
        this.DisplayList = true;
      else
        this.DisplayList = false;
    }
  }

  public bool IsReDownloadEnabled
  {
    get => this._IsReDownloadEnabled;
    internal set
    {
      this._IsReDownloadEnabled = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.IsReDownloadEnabled));
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

  public double ListViewHeightRequest
  {
    get => this._listViewHeightRequest;
    set
    {
      this.SetProperty<double>(ref this._listViewHeightRequest, value, nameof (ListViewHeightRequest));
    }
  }

  public material ObjItemSelected
  {
    get => this._ItemSelected;
    set
    {
      if (this._ItemSelected == value)
        return;
      this._ItemSelected = value;
      this.eNumber = this.ObjItemSelected._material;
      this._ItemSelected = (material) null;
      this.RaisePropertyChanged<material>((Expression<Func<material>>) (() => this.ObjItemSelected));
      this.DisplayList = false;
      this.IsReDownloadEnabled = false;
    }
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

  public string eNumberLabelText
  {
    get => this._eNumberLabelText;
    internal set
    {
      this._eNumberLabelText = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.eNumberLabelText));
    }
  }

  public List<material> enumbersListInit
  {
    get => this._enumbersListInit;
    private set => this._enumbersListInit = value;
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
          this.enumbersListInit = this._metadataService.getMatchingEntries(value.Substring(0, 4), MatchPattern.PREFIX);
          this.enumbersListInit = this.enumbersListInit.OrderBy<material, string>((Func<material, string>) (x => x._material)).ToList<material>();
        }
        this.eNumbersList = this.enumbersListInit.AsParallel<material>().Where<material>((Func<material, bool>) (x => x._material.StartsWith(value, StringComparison.InvariantCultureIgnoreCase))).ToList<material>();
        this.ListViewHeightRequest = (double) this.eNumbersList.Count * 50.0;
      }
      this._eNumber = value.ToUpper();
      if (string.IsNullOrEmpty(this._eNumber) || this.eNumbersList.Count == 0 || this._eNumber.Length < 4)
      {
        this.DisplayList = false;
        this.IsReDownloadEnabled = true;
      }
      else if (this.eNumbersList.Count != 0 && this._eNumber.Length > 4)
      {
        this.DisplayList = true;
        this.IsReDownloadEnabled = false;
      }
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.eNumber));
    }
  }

  internal async void GoBack()
  {
    int num = await this._navigationService.Close((IMvxViewModel) this, new CancellationToken()) ? 1 : 0;
  }

  private async void GoToDownloadView()
  {
    try
    {
      Preferences.Set("IsReDownloadedEnabled", true);
      this._userSession.SetSenderScreen(AppResource.RE_DOWNLOAD_ENUMBER_VIEW);
      this._userSession.setEnumberSession(this.eNumber);
      this.eNumber = string.Empty;
      IMvxNavigationService navigationService = this._navigationService;
      DetailNavigationArgs detailNavigationArgs = new DetailNavigationArgs();
      detailNavigationArgs.senderScreen = AppResource.RE_DOWNLOAD_ENUMBER_VIEW;
      CancellationToken cancellationToken = new CancellationToken();
      int num = await navigationService.Navigate<ApplianceRepairViewModel, DetailNavigationArgs>(detailNavigationArgs, (IMvxBundle) null, cancellationToken) ? 1 : 0;
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppError(LoggingContext.BINARY, "Exception occurred in GoToDownloadView: " + ex.Message, memberName: nameof (GoToDownloadView), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ReDownloadEnumberViewModel.cs", sourceLineNumber: 233);
    }
  }
}
