// Decompiled with JetBrains decompiler
// Type: iService5.Core.ViewModels.BridgeLogViewModel
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Appliance;
using iService5.Core.Services.Helpers;
using iService5.Core.Services.User;
using iService5.Core.Services.User.ActivityLogging;
using iService5.Ssh.DTO;
using iService5.Ssh.models;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
//using Xamarin.Forms;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

#nullable disable
namespace iService5.Core.ViewModels;

public class BridgeLogViewModel : MvxViewModel
{
  private readonly ILoggingService _loggingService;
  private readonly IMvxNavigationService _navigationService;
  private readonly IAlertService _alertService;
  private readonly Is5SshWrapper _sshWrapper;
  internal bool displayLogList = false;
  private bool displayActivityIndicator = false;
  private ObservableCollection<LogSessionItems> _wifiBridgeLog = new ObservableCollection<LogSessionItems>();
  private LogSessionItems _ItemSelected;
  private ObservableCollection<LogSessionItems> _wifiBridgeMainLog;
  private string _HeaderText = "RepairEnumber";
  private string _DownloadBtnText = "Download";
  private bool _isDownloaded = true;
  private bool _areButtonsEnabled = true;
  private bool _ShowDownloadBtn = true;
  private bool _isMainLogDownloaded = false;
  private string _MainLogdownloadText = "Download Main Log";
  private bool _refreshingList = false;
  private bool _refreshingSessionsLogList = false;

  public BridgeLogViewModel(
    IMvxNavigationService navigationService,
    IUserSession userSession,
    ILoggingService loggingService,
    Is5SshWrapper sshWrapper,
    IAlertService alertService)
  {
    this._loggingService = loggingService;
    this._navigationService = navigationService;
    this.HeaderText = userSession.getEnumberSession();
    this._alertService = alertService;
    this._sshWrapper = sshWrapper;
    this.DisplayActivityIndicator = true;
    this.AreButtonsEnabled = true;
    this.DownloadSingleLogSession = new Command<object>(new Action<object>(this.DownloadLogSession));
    this.DownloadMainLogBtnCommand = new Command<object>(new Action<object>(this.DownloadMainLogSession));
    this.BackBtnClicked = (ICommand) new Command(new Action(this.VisitPage));
    this.MainLogListReload = (ICommand) new Command(new Action(this.ReloadMainLogList));
    this.SessionsListReload = (ICommand) new Command(new Action(this.ReloadSesionsList));
    ObservableCollection<LogSessionItems> observableCollection = new ObservableCollection<LogSessionItems>();
    observableCollection.Add(new LogSessionItems("Main Log", "", "", false, "Download"));
    this.wifiBridgeMainLog = observableCollection;
  }

  private void ReloadMainLogList()
  {
    Task.Run((Func<Task>) (async () =>
    {
      this.refreshingMainLogList = true;
      await Task.Delay(4000);
      this.refreshingMainLogList = false;
    }));
  }

  private void ReloadSesionsList()
  {
    Task.Run((Func<Task>) (async () =>
    {
      this.refreshingSessionsLogList = true;
      await Task.Delay(4000);
      this.refreshingSessionsLogList = false;
    }));
  }

  public ICommand BackBtnClicked { protected set; get; }

  public ICommand MainLogListReload { protected set; get; }

  public ICommand SessionsListReload { protected set; get; }

  internal void VisitPage()
  {
    this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
  }

  public Command<object> DownloadSingleLogSession { get; private set; }

  public Command<object> DownloadMainLogBtnCommand { get; private set; }

  public bool DisplayActivityIndicator
  {
    get => this.displayActivityIndicator;
    set
    {
      this.displayActivityIndicator = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.DisplayActivityIndicator));
    }
  }

  internal void DownloadMainLogSession(object obj)
  {
    LogSessionItems logSessionItems = obj as LogSessionItems;
    string zipMainLogFilePath = Path.Combine(Constants.BridgeLogDirPath, Constants.bridgeMainLogFileName);
    try
    {
      try
      {
        if (!Directory.Exists(Constants.BridgeLogDirPath))
        {
          this._loggingService.getLogger().LogAppInformation(LoggingContext.LOCAL, "BridgeLogs folder not found. Creating new one.", memberName: nameof (DownloadMainLogSession), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/BridgeLogViewModel.cs", sourceLineNumber: 119);
          Directory.CreateDirectory(Constants.BridgeLogDirPath);
        }
        this.refreshingMainLogList = true;
        Task.Run<bool>((Func<bool>) (() => this.SaveByteArrayToFile(this._sshWrapper.GetLogFile().ListOfBytes, zipMainLogFilePath)));
        this._alertService.ShowMessageAlertWithMessage("Main BridgeLogs saved.", AppResource.INFORMATION_TEXT);
        this._loggingService.getLogger().LogAppInformation(LoggingContext.LOCAL, "Main BridgeLogs saved.", memberName: nameof (DownloadMainLogSession), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/BridgeLogViewModel.cs", sourceLineNumber: (int) sbyte.MaxValue);
        this.isMainLogDownloaded = true;
        this.MainLogdownloadText = AppResource.LOG_MAIN_REDOWNLOAD;
        logSessionItems.isDownloaded = true;
      }
      catch (Exception ex)
      {
        this._loggingService.getLogger().LogAppError(LoggingContext.BACKEND, "Exception occured while saving main log file : " + ex?.ToString(), memberName: nameof (DownloadMainLogSession), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/BridgeLogViewModel.cs", sourceLineNumber: 134);
        this._alertService.ShowMessageAlertWithMessage(AppResource.LOG_SESSION_ERROR, AppResource.WARNING_TITLE);
      }
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppError(LoggingContext.LOCAL, $"{AppResource.LOG_SESSION_ERROR}-{ex.Message}", memberName: nameof (DownloadMainLogSession), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/BridgeLogViewModel.cs", sourceLineNumber: 140);
      this._loggingService.getLogger().LogAppError(LoggingContext.LOCAL, $"{AppResource.LOG_SESSION_ERROR}-{ex.StackTrace}", memberName: nameof (DownloadMainLogSession), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/BridgeLogViewModel.cs", sourceLineNumber: 141);
      this._alertService.ShowMessageAlertWithMessage(AppResource.LOG_SESSION_ERROR, AppResource.WARNING_TITLE);
    }
    this.refreshingMainLogList = false;
  }

  internal void DownloadLogSession(object obj)
  {
    this.AreButtonsEnabled = false;
    this.DisplayActivityIndicator = true;
    LogSessionItems logSessionItems = obj as LogSessionItems;
    SessionDto sessionDetails = new SessionDto(logSessionItems.material, logSessionItems.ts, logSessionItems.ts_actual);
    string str = sessionDetails.ts.Replace(":", "_");
    string path2 = sessionDetails.material.Replace("/", "_") + str.Replace(" ", string.Empty) + Constants.zipFileSuffix;
    string zipLogSessionFilePath = Path.Combine(Constants.BridgeLogDirPath + "/", path2);
    try
    {
      try
      {
        if (!Directory.Exists(Constants.BridgeLogDirPath))
        {
          this._loggingService.getLogger().LogAppInformation(LoggingContext.LOCAL, "BridgeLogs-folder not found. Creating new one.", memberName: nameof (DownloadLogSession), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/BridgeLogViewModel.cs", sourceLineNumber: 163);
          Directory.CreateDirectory(Constants.BridgeLogDirPath);
        }
        Task.Run((Action) (() =>
        {
          bool file = this.SaveByteArrayToFile(this._sshWrapper.GetBridgeLogForSingleSession(sessionDetails).ListOfBytes, zipLogSessionFilePath);
          this.AreButtonsEnabled = true;
          this.DisplayActivityIndicator = false;
          if (file)
          {
            this._loggingService.getLogger().LogAppInformation(LoggingContext.LOCAL, AppResource.BRIDGELOG_SAVED, memberName: nameof (DownloadLogSession), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/BridgeLogViewModel.cs", sourceLineNumber: 173);
            this._alertService.ShowMessageAlertWithMessage(AppResource.BRIDGELOG_SAVED, AppResource.INFORMATION_TEXT);
          }
          else
            this._alertService.ShowMessageAlertWithMessage(AppResource.BRIDGELOG_ERROR, AppResource.INFORMATION_TEXT);
        }));
        logSessionItems.isDownloaded = true;
        this.ObjItemSelected = logSessionItems;
      }
      catch (Exception ex)
      {
        this._loggingService.getLogger().LogAppError(LoggingContext.BACKEND, "Exception occured while saving session log file :" + ex?.ToString(), memberName: nameof (DownloadLogSession), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/BridgeLogViewModel.cs", sourceLineNumber: 188);
        this._alertService.ShowMessageAlertWithMessage(AppResource.LOG_SESSION_ERROR, AppResource.WARNING_TITLE);
        this.AreButtonsEnabled = true;
        this.refreshingSessionsLogList = false;
      }
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppInformation(LoggingContext.LOCAL, AppResource.LOG_SESSION_ERROR + ex.Message, memberName: nameof (DownloadLogSession), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/BridgeLogViewModel.cs", sourceLineNumber: 196);
      this._alertService.ShowMessageAlertWithMessage(AppResource.LOG_SESSION_ERROR, AppResource.WARNING_TITLE);
      this.AreButtonsEnabled = true;
      this.refreshingSessionsLogList = false;
    }
  }

  public bool SaveByteArrayToFile(List<byte> data, string filePath)
  {
    try
    {
      File.WriteAllBytes(filePath, data.ToArray());
      return true;
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppInformation(LoggingContext.LOCAL, AppResource.LOG_SESSION_ERROR + ex.Message, memberName: nameof (SaveByteArrayToFile), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/BridgeLogViewModel.cs", sourceLineNumber: 212);
    }
    return false;
  }

  public void DownloadAllLogSessions()
  {
    Task.Run((Func<Task>) (async () =>
    {
      try
      {
        this.allsessions = this._sshWrapper.GetLogSessions();
        try
        {
          if (!Directory.Exists(Constants.BridgeLogDirPath))
          {
            this._loggingService.getLogger().LogAppInformation(LoggingContext.LOCAL, "BridgeLogs-folder not found. Creating new one.", memberName: nameof (DownloadAllLogSessions), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/BridgeLogViewModel.cs", sourceLineNumber: 228);
            Directory.CreateDirectory(Constants.BridgeLogDirPath);
          }
        }
        catch (Exception ex)
        {
          this._loggingService.getLogger().LogAppError(LoggingContext.BACKEND, "Exception occured while Creating bridge log folder : " + ex?.ToString(), memberName: nameof (DownloadAllLogSessions), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/BridgeLogViewModel.cs", sourceLineNumber: 234);
        }
        this._loggingService.getLogger().LogAppInformation(LoggingContext.LOCAL, "All BridgeLog sessions saved.", memberName: nameof (DownloadAllLogSessions), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/BridgeLogViewModel.cs", sourceLineNumber: 236);
        this.wifiBridgeLog = this.allsessions.Response.sessionsList;
        foreach (LogSessionItems item in (Collection<LogSessionItems>) this.wifiBridgeLog)
        {
          item.downloadText = AppResource.LOG_SESSION_DOWNLOAD_BUTTON;
          item.isDownloaded = false;
        }
        if (this.wifiBridgeLog.Count > 0)
          this.DisplayLogList = true;
      }
      catch (Exception ex)
      {
        this._loggingService.getLogger().LogAppInformation(LoggingContext.LOCAL, AppResource.LOG_SESSION_ERROR + ex.Message, memberName: nameof (DownloadAllLogSessions), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/BridgeLogViewModel.cs", sourceLineNumber: 250);
        await this._alertService.ShowMessageAlertWithMessage(AppResource.LOG_SESSION_ERROR, AppResource.WARNING_TITLE);
      }
      this.DisplayActivityIndicator = false;
    }));
  }

  public ObservableCollection<LogSessionItems> wifiBridgeLog
  {
    get => this._wifiBridgeLog;
    set
    {
      this._wifiBridgeLog = value;
      this.RaisePropertyChanged<ObservableCollection<LogSessionItems>>((Expression<Func<ObservableCollection<LogSessionItems>>>) (() => this.wifiBridgeLog));
    }
  }

  public LogSessionItems ObjItemSelected
  {
    get => this._ItemSelected;
    set
    {
      if (this._ItemSelected != value)
      {
        this._ItemSelected = value;
        this.RaisePropertyChanged<LogSessionItems>((Expression<Func<LogSessionItems>>) (() => this.ObjItemSelected));
      }
      else
        this._ItemSelected = (LogSessionItems) null;
    }
  }

  public ObservableCollection<LogSessionItems> wifiBridgeMainLog
  {
    get => this._wifiBridgeMainLog;
    set
    {
      this._wifiBridgeMainLog = value;
      this.RaisePropertyChanged<ObservableCollection<LogSessionItems>>((Expression<Func<ObservableCollection<LogSessionItems>>>) (() => this.wifiBridgeMainLog));
    }
  }

  public bool DisplayLogList
  {
    get => this.displayLogList;
    internal set
    {
      this.displayLogList = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.DisplayLogList));
    }
  }

  public string HeaderText
  {
    get => this._HeaderText;
    internal set
    {
      this._HeaderText = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.HeaderText));
    }
  }

  public string DownloadBtnText
  {
    get => this._DownloadBtnText;
    internal set
    {
      this._DownloadBtnText = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.DownloadBtnText));
    }
  }

  public bool isDownloaded
  {
    get => this._isDownloaded;
    set
    {
      if (this._isDownloaded == value)
        return;
      this._isDownloaded = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.isDownloaded));
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

  public bool ShowDownloadBtn
  {
    get => this._ShowDownloadBtn;
    set
    {
      if (this._ShowDownloadBtn == value)
        return;
      this._ShowDownloadBtn = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.ShowDownloadBtn));
    }
  }

  public bool isMainLogDownloaded
  {
    get => this._isMainLogDownloaded;
    set
    {
      this._isMainLogDownloaded = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.isMainLogDownloaded));
    }
  }

  public string MainLogdownloadText
  {
    get => this._MainLogdownloadText;
    set
    {
      this._MainLogdownloadText = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.MainLogdownloadText));
    }
  }

  public SshResponse<LogSessionDto> allsessions { get; private set; }

  public bool refreshingMainLogList
  {
    get => this._refreshingList;
    private set
    {
      this._refreshingList = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.refreshingMainLogList));
    }
  }

  public bool refreshingSessionsLogList
  {
    get => this._refreshingSessionsLogList;
    private set
    {
      this._refreshingSessionsLogList = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.refreshingSessionsLogList));
    }
  }

  public override void ViewAppeared()
  {
    base.ViewAppeared();
    this.AreButtonsEnabled = true;
    this.DownloadBtnText = AppResource.LOG_SESSION_DOWNLOAD_BUTTON;
    this.DownloadAllLogSessions();
  }
}
