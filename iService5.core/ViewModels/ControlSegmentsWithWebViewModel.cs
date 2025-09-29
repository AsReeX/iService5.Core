// Decompiled with JetBrains decompiler
// Type: iService5.Core.ViewModels.ControlSegmentsWithWebViewModel
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Appliance;
using iService5.Core.Services.Data;
using iService5.Core.Services.Helpers;
using iService5.Core.Services.Helpers.Topogram;
using iService5.Core.Services.Localisation;
using iService5.Core.Services.Platform;
using iService5.Core.Services.User;
using iService5.Core.Services.User.ActivityLogging;
using iService5.Core.Services.VersionReport;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
//using Xamarin.Forms;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

#nullable disable
namespace iService5.Core.ViewModels;

public class ControlSegmentsWithWebViewModel : MvxViewModel<bool>
{
  private readonly ILoggingService _loggingService;
  private readonly IUserSession _userSession;
  private readonly IPlatformSpecificServiceLocator _locator;
  private readonly IAlertService _alertService;
  private readonly IMetadataService _metadataService;
  private readonly IMvxNavigationService _navigationService;
  private readonly IAppliance _appliance;
  internal iService5.Core.Services.Helpers.Topogram.Topogram topogram;
  internal List<TopogramComponent> topogramComponents = new List<TopogramComponent>();
  internal MvxObservableCollection<Screen> _screenList = new MvxObservableCollection<Screen>();
  internal MvxObservableCollection<Screen> _extraList = new MvxObservableCollection<Screen>();
  internal string monitoringGraphicsEnumber = "Z9KDKD0Z01(00)";
  internal string monitoringGraphicsDirectory = "MonitoringGraphics";
  private readonly Is5SshWrapper _sshWrapper;
  internal bool FetchingComponentStates = true;
  internal Task _DisconnectionTask = (Task) null;
  internal CancellationTokenSource _tokenSource = (CancellationTokenSource) null;
  internal bool firstReception = true;
  public string tempJson = (string) null;
  public bool IntialFetch = true;
  private string _WebUrl;
  private string _NavText = AppResource.APPLIANCE_FLASH_BACK_TEXT;
  private string _RepairEnumber = nameof (RepairEnumber);
  private string _WifiBridgeStatus = "• " + AppResource.CONNECTED_TEXT;
  private string _ConnectedColor = "Green";
  private string _ScreenTitle;
  private string _NoScreensText = AppResource.MONITORING_NO_SCREENS_TEXT;
  private string _selectedItemColor;
  private int _screenIndex;
  private Screen _hItem;
  private bool displayActivityIndicator = true;

  public string monitoringGraphic { get; private set; }

  public Action RefreshViewAction { get; set; }

  public bool NoScreens { get; set; }

  public string starterJson { get; set; }

  public ControlSegmentsWithWebViewModel(
    IMvxNavigationService navigationService,
    IPlatformSpecificServiceLocator locator,
    IUserSession userSession,
    ILoggingService loggingService,
    IShortTextsService _ShortTextsService,
    IVersionReport versionReport,
    IAlertService alertService,
    IMetadataService metadataService,
    IAppliance appliance,
    Is5SshWrapper sshWrapper)
  {
    this._userSession = userSession;
    this._locator = locator;
    this._alertService = alertService;
    this._metadataService = metadataService;
    this._navigationService = navigationService;
    this._appliance = appliance;
    this._loggingService = loggingService;
    this._sshWrapper = sshWrapper;
    this.GoBackCommand = (ICommand) new Command(new Action(this.GoBack));
    this.RepairEnumber = this._userSession.getEnumberSession();
    try
    {
      this.topogram = new TopogramParser(this.RepairEnumber, this._loggingService).GetTopogram();
      List<TopogramScreen> allScreens = this.topogram.GetAllScreens();
      this.screenList = new MvxObservableCollection<Screen>();
      for (int index = 0; index < allScreens.Count; ++index)
        ((Collection<Screen>) this.screenList).Add(new Screen()
        {
          Id = index,
          Title = allScreens[index].GetScreenName(),
          IsSelected = index == 0
        });
      this.extraList = this.screenList;
      this.screenIndex = 0;
      this.selItem = ((IEnumerable<Screen>) this.screenList).First<Screen>();
    }
    catch (Exception ex)
    {
      this._alertService.ShowMessageAlertWithKey("MONITORING_PARSING_ERROR", AppResource.ERROR_TITLE);
      loggingService.getLogger().LogAppError(LoggingContext.MONITORING, "Error while parsing topogram: " + ex.Message, memberName: ".ctor", sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ControlSegmentsWithWebViewModel.cs", sourceLineNumber: 90);
      this.DisplayActivityIndicator = false;
      this.NoScreens = true;
    }
    try
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine();
      CoreApp.history.SaveItem(new History(this.RepairEnumber, DateTime.Now, UtilityFunctions.getSessionIDForHistoryTable(), HistoryDBInfoType.MonitoringLog.ToString(), stringBuilder.ToString()));
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppInformation(LoggingContext.MONITORING, "Failed to save item in the History DB, " + ex?.ToString(), memberName: ".ctor", sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ControlSegmentsWithWebViewModel.cs", sourceLineNumber: 103);
    }
  }

  public void RefreshWebViewForSelectedTab()
  {
    string str1 = "http://" + this._locator.GetPlatformSpecificService().GetIp() ?? "";
    if (!string.IsNullOrEmpty(this.ScreenTitle))
    {
      string str2 = $"{str1}/?control={this.ScreenTitle}";
      this._loggingService.getLogger().LogAppInformation(LoggingContext.BRIDGESETTINGS, "URL : " + str2, memberName: nameof (RefreshWebViewForSelectedTab), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ControlSegmentsWithWebViewModel.cs", sourceLineNumber: 114);
      this.WebUrl = str2;
    }
    this.DisplayActivityIndicator = false;
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

  public void WebPageError()
  {
    this._loggingService.getLogger().LogAppInformation(LoggingContext.BRIDGESETTINGS, AppResource.URL_FAIL_MSG, memberName: nameof (WebPageError), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ControlSegmentsWithWebViewModel.cs", sourceLineNumber: 136);
    this._alertService.ShowMessageAlertWithKey("URL_FAIL_MSG", AppResource.ERROR_TITLE);
    this.DisplayActivityIndicator = false;
  }

  public ICommand GoBackCommand { internal set; get; }

  public ICommand DisplayMonitoringGraphics { internal set; get; }

  public ICommand SwipeLeftCommand { internal set; get; }

  public ICommand SwipeRightCommand { internal set; get; }

  public MvxObservableCollection<Screen> screenList
  {
    get => this._screenList;
    set
    {
      this._screenList = value;
      this.RaisePropertyChanged<MvxObservableCollection<Screen>>((Expression<Func<MvxObservableCollection<Screen>>>) (() => this.screenList));
    }
  }

  public MvxObservableCollection<Screen> extraList
  {
    get => this._extraList;
    set
    {
      this._extraList = value;
      this.RaisePropertyChanged<MvxObservableCollection<Screen>>((Expression<Func<MvxObservableCollection<Screen>>>) (() => this.extraList));
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

  public string WifiBridgeStatus
  {
    get => this._WifiBridgeStatus;
    internal set
    {
      this._WifiBridgeStatus = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.WifiBridgeStatus));
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

  public string ScreenTitle
  {
    get => this._ScreenTitle;
    internal set
    {
      this._ScreenTitle = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.ScreenTitle));
    }
  }

  public string NoScreensText
  {
    get => this._NoScreensText;
    internal set
    {
      this._NoScreensText = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.NoScreensText));
    }
  }

  public string selectedItemColor
  {
    get => this._selectedItemColor;
    set
    {
      this._selectedItemColor = value;
      Device.BeginInvokeOnMainThread((Action) (() =>
      {
        for (int index = 0; index < ((Collection<Screen>) this.screenList).Count; ++index)
        {
          if (this.screenIndex == ((Collection<Screen>) this.extraList)[index].Id)
            ((Collection<Screen>) this.extraList)[index].IsSelected = true;
          else
            ((Collection<Screen>) this.extraList)[index].IsSelected = false;
        }
        this.screenList = new MvxObservableCollection<Screen>((IEnumerable<Screen>) this.extraList);
        this.RaisePropertyChanged<MvxObservableCollection<Screen>>((Expression<Func<MvxObservableCollection<Screen>>>) (() => this.extraList));
      }));
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.selectedItemColor));
    }
  }

  public int screenIndex
  {
    get => this._screenIndex;
    set
    {
      this._screenIndex = value;
      this.RaisePropertyChanged<int>((Expression<Func<int>>) (() => this.screenIndex));
    }
  }

  public Screen selItem
  {
    get => this._hItem;
    set
    {
      this.DisplayActivityIndicator = true;
      this._hItem = value;
      this.screenIndex = value.Id;
      this.ScreenTitle = value.Title;
      this.selectedItemColor = "Red";
      this.RefreshWebViewForSelectedTab();
      this.RaisePropertyChanged<Screen>((Expression<Func<Screen>>) (() => this.selItem));
    }
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

  public override void ViewAppeared()
  {
    base.ViewAppeared();
    this._appliance.CheckConnectivity(new connectivityUpdateCallback(this.ConnectivityCheck));
  }

  public override void ViewDisappearing() => base.ViewDisappearing();

  public void OnBackButtonPressed() => this.GoBack();

  internal void GoBack()
  {
    this.FetchingComponentStates = false;
    this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
  }

  internal void ConnectivityCheck()
  {
    this.WifiBridgeStatus = this._appliance.StatusOfBridgeConnection;
    this.ConnectedColor = this._appliance.ConnectedColorBridge;
    if (this._appliance.boolStatusOfBridgeConnection)
    {
      if (this._DisconnectionTask == null || this._tokenSource == null || this._DisconnectionTask.IsCompleted)
        return;
      this._tokenSource.Cancel();
    }
    else
    {
      this._tokenSource = new CancellationTokenSource();
      this._DisconnectionTask = Task.Factory.StartNew((Action) (() =>
      {
        Thread.Sleep(3000);
        if (this._tokenSource.Token.IsCancellationRequested)
          return;
        this.GoBack();
      }), this._tokenSource.Token);
    }
  }

  public override void Prepare(bool parameter) => throw new NotImplementedException();
}
