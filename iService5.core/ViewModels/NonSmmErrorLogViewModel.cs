// Decompiled with JetBrains decompiler
// Type: iService5.Core.ViewModels.NonSmmErrorLogViewModel
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
using iService5.Ssh.models;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
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

public class NonSmmErrorLogViewModel : MvxViewModel<SshResponse<object>>
{
  private readonly IMvxNavigationService _navigationService;
  private readonly IMetadataService _metadataService;
  private readonly ILoggingService _loggingService;
  private readonly IPlatformSpecificServiceLocator _locator;
  private readonly IAlertService _alertService;
  private readonly IAppliance _appliance;
  private readonly IUserSession _userSession;
  internal bool _displayMemoryLog = false;
  private string _RepairEnumber;
  private string _DescriptionLabel = AppResource.NONSMM_MEMORY_LOG_MSGD_LABEL;
  private string _LogValueLabel = AppResource.NONSMM_MEMORY_LOG_MSGV_LABEL;
  private string _noMemoryorLog = AppResource.NO_MEMORY_LOG_TO_DISPLAY;
  private string _WifiStatus = AppResource.CONNECTED_TEXT;
  private string _NavText = AppResource.APPLIANCE_FLASH_BACK_TEXT;
  private string _ErrorLogArea = "";
  private string _ConnectedColor = "Green";
  private SshResponse<object> _response;
  private List<NonSmmErrorLogViewModel.LogMemoryEntry> _DisplayLogMemoryEntries;

  public bool _viewstatus { get; set; }

  public string _ConnectionGraphic { get; internal set; }

  public ICommand OpenErrorDetails { internal set; get; }

  public NonSmmErrorLogViewModel(
    IMvxNavigationService navigationService,
    IShortTextsService _SService,
    ILoggingService loggingService,
    IMetadataService metadataService,
    IPlatformSpecificServiceLocator locator,
    IUserSession userSession,
    IAlertService alertService,
    IAppliance appliance)
  {
    this.ReturnToNonSMM = (ICommand) new Command(new Action(this.ReturnToNonSMMFunction));
    this._navigationService = navigationService;
    this._loggingService = loggingService;
    this._metadataService = metadataService;
    this._locator = locator;
    this._alertService = alertService;
    this._appliance = appliance;
    this._userSession = userSession;
    this._RepairEnumber = this._userSession.getEnumberSession();
  }

  public virtual async Task Initialize()
  {
    await base.Initialize();
    this._appliance.CheckConnectivity(new connectivityUpdateCallback(this.updateStatus));
    List<NonSmmErrorLogViewModel.LogMemoryEntry> LogMemoryEntries = new List<NonSmmErrorLogViewModel.LogMemoryEntry>();
    if (this._response.Success)
    {
      try
      {
        JObject jobj = JObject.Parse(this._response.Response?.ToString());
        IEnumerable<JToken> LogTokens = jobj.SelectTokens("Log_Messages[?(@..msg_description != null)]", false);
        StringBuilder sb = new StringBuilder();
        foreach (JToken ErrorEntry in LogTokens)
        {
          NonSmmErrorLogViewModel.LogMemoryEntry LogItem = new NonSmmErrorLogViewModel.LogMemoryEntry();
          LogItem.Description = this.GetShortText(ErrorEntry[(object) "msg_description"].ToString());
          LogItem.MemoryValue = ErrorEntry[(object) "msg_value"].ToString();
          LogMemoryEntries.Add(LogItem);
          sb.AppendLine($"{LogItem.Description} {LogItem.MemoryValue}");
          LogItem = (NonSmmErrorLogViewModel.LogMemoryEntry) null;
        }
        string sessionID = UtilityFunctions.getSessionIDForHistoryTable();
        try
        {
          CoreApp.history.SaveItem(new History(this.RepairEnumber, DateTime.Now, sessionID, HistoryDBInfoType.MemoryLog.ToString(), sb.ToString()));
        }
        catch (Exception ex)
        {
          this._loggingService.getLogger().LogAppError(LoggingContext.APPLIANCE, "Failed to save HA Info data in the History DB, " + ex?.ToString(), memberName: nameof (Initialize), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/NonSmmErrorLogViewModel.cs", sourceLineNumber: 86);
        }
        if (LogMemoryEntries.Count > 0)
          this._displayMemoryLog = true;
        this.DisplayLogMemoryEntries = new List<NonSmmErrorLogViewModel.LogMemoryEntry>((IEnumerable<NonSmmErrorLogViewModel.LogMemoryEntry>) LogMemoryEntries);
        jobj = (JObject) null;
        LogTokens = (IEnumerable<JToken>) null;
        sb = (StringBuilder) null;
        sessionID = (string) null;
        LogMemoryEntries = (List<NonSmmErrorLogViewModel.LogMemoryEntry>) null;
      }
      catch (Exception ex)
      {
        this._loggingService.getLogger().LogAppError(LoggingContext.APPLIANCE, "Failed with " + ex.Message, ex, nameof (Initialize), "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/NonSmmErrorLogViewModel.cs", 98);
        LogMemoryEntries = (List<NonSmmErrorLogViewModel.LogMemoryEntry>) null;
      }
    }
    else
    {
      this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Error log attempt stderr " + this._response.ErrorMessage, memberName: nameof (Initialize), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/NonSmmErrorLogViewModel.cs", sourceLineNumber: 103);
      await this._alertService.ShowMessageAlertWithKey("MEMORY_PAGE_NOTSUPPORTED", AppResource.INFORMATION_TEXT);
      LogMemoryEntries = (List<NonSmmErrorLogViewModel.LogMemoryEntry>) null;
    }
  }

  internal string GetShortText(string fieldName)
  {
    string lowerInvariant = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToLowerInvariant();
    string[] strArray = fieldName.Split(',', ';');
    try
    {
      return (this._metadataService.getShortText(strArray[0], lowerInvariant) != null ? this._metadataService.getShortText(strArray[0], lowerInvariant) + Environment.NewLine : "") + strArray[1];
    }
    catch (Exception ex)
    {
      return strArray[0];
    }
  }

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

  public string RepairEnumber
  {
    get => this._RepairEnumber;
    internal set
    {
      this._RepairEnumber = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.RepairEnumber));
    }
  }

  public string DescriptionLabel
  {
    get => this._DescriptionLabel;
    internal set
    {
      this._DescriptionLabel = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.DescriptionLabel));
    }
  }

  public string LogValueLabel
  {
    get => this._LogValueLabel;
    internal set
    {
      this._LogValueLabel = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.LogValueLabel));
    }
  }

  public bool DisplayMemoryLog
  {
    get => this._displayMemoryLog;
    internal set
    {
      this._displayMemoryLog = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.DisplayMemoryLog));
    }
  }

  public string noMemoryorLog
  {
    get => this._noMemoryorLog;
    internal set
    {
      this._noMemoryorLog = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.noMemoryorLog));
    }
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

  public override void Prepare(SshResponse<object> parameter) => this._response = parameter;

  public string ErrorLogArea
  {
    get => this._ErrorLogArea;
    internal set
    {
      this._ErrorLogArea = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.ErrorLogArea));
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

  internal void ReturnToNonSMMFunction()
  {
    this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
  }

  public ICommand ReturnToNonSMM { protected set; get; }

  public List<NonSmmErrorLogViewModel.LogMemoryEntry> DisplayLogMemoryEntries
  {
    get => this._DisplayLogMemoryEntries;
    internal set
    {
      this._DisplayLogMemoryEntries = value;
      this.RaisePropertyChanged<List<NonSmmErrorLogViewModel.LogMemoryEntry>>((Expression<Func<List<NonSmmErrorLogViewModel.LogMemoryEntry>>>) (() => this.DisplayLogMemoryEntries));
    }
  }

  public override void ViewAppeared()
  {
    base.ViewAppeared();
    this._viewstatus = true;
  }

  internal void updateStatus()
  {
    this.WifiStatus = this._appliance.StatusOfBridgeConnection;
    this.ConnectedColor = this._appliance.ConnectedColorBridge;
  }

  [ExcludeFromCodeCoverage]
  public class LogMemoryEntry
  {
    public string Description { get; set; }

    public string MemoryValue { get; set; }
  }
}
