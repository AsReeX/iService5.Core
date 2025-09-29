// Decompiled with JetBrains decompiler
// Type: iService5.Core.ViewModels.ErrorLogViewModel
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

public class ErrorLogViewModel : MvxViewModel<SshResponse<object>>
{
  private readonly IMvxNavigationService _navigationService;
  private readonly IUserSession _userSession;
  private readonly IMetadataService _metadataService;
  private readonly ILoggingService _loggingService;
  private readonly IPlatformSpecificServiceLocator _locator;
  private readonly IAlertService _alertService;
  private readonly IAppliance _appliance;
  internal SshResponse<object> _response;
  internal bool _displayErrorLog = false;
  private string _RepairEnumber = nameof (RepairEnumber);
  private string _TimestampLabel = AppResource.ERROR_LOG_TS_LABEL;
  private string _ErrorCodeLabel = AppResource.ERROR_LOG_EC_LABEL;
  private string noErrorLog = AppResource.NO_ERROR_LOG_TO_DISPLAY;
  private string _WifiStatus = AppResource.CONNECTED_TEXT;
  private string _NavText = AppResource.APPLIANCE_FLASH_BACK_TEXT;
  private string _ErrorLogArea = "";
  private string _ConnectedColor = "Green";
  private string _Instruction = AppResource.GRAPHIC_PAGE_CONNECT_TEXT;
  private ErrorLogViewModel.ErrorEntry _ItemSelected;
  private List<ErrorLogViewModel.ErrorEntry> _errorEntries;
  private bool _areButtonsEnabled = true;

  public virtual async Task Initialize()
  {
    await base.Initialize();
    this._appliance.CheckConnectivity(new connectivityUpdateCallback(this.updateStatus));
    if (this._response.Success)
    {
      try
      {
        JObject jobj = JObject.Parse(this._response.Response?.ToString());
        IEnumerable<JToken> errorTokens = jobj.SelectTokens("LOG_MESSAGES[?(@..PCS_Error != null)]", false);
        List<ErrorLogViewModel.ErrorEntry> errorLogEntries = errorTokens.OrderByDescending<JToken, JToken>((Func<JToken, JToken>) (entry => entry[(object) "TIME_STAMP"])).Select<JToken, ErrorLogViewModel.ErrorEntry>((Func<JToken, ErrorLogViewModel.ErrorEntry>) (entry =>
        {
          ErrorLogViewModel.ErrorEntry errorEntry = new ErrorLogViewModel.ErrorEntry();
          DateTime dateTime = ErrorLogViewModel.UnixTimeStampToDateTime(Convert.ToDouble((object) entry[(object) "TIME_STAMP"], (IFormatProvider) new NumberFormatInfo()));
          string str1 = dateTime.ToString(UtilityFunctions.GetCurrentCultureDateFormat());
          dateTime = ErrorLogViewModel.UnixTimeStampToDateTime(Convert.ToDouble((object) entry[(object) "TIME_STAMP"], (IFormatProvider) new NumberFormatInfo()));
          string str2 = dateTime.ToString(UtilityFunctions.GetCurrentCultureTimeFormat());
          errorEntry.timestamp = $"{str1} {str2}";
          errorEntry.pcsError = this.getErrorMessage(entry.SelectToken("LOG_VALUE..PCS_Error", false).ToString());
          errorEntry.pcsErrorHasDescription = this.hasErrorCodeDescription(entry.SelectToken("LOG_VALUE..PCS_Error", false).ToString());
          errorEntry.pcsErrorDescription = this.getErrorDescription(entry.SelectToken("LOG_VALUE..PCS_Error", false).ToString());
          return errorEntry;
        })).ToList<ErrorLogViewModel.ErrorEntry>();
        this.HasNoErrorLogEntries = errorLogEntries.Count == 0;
        if (errorLogEntries.Count > 0)
        {
          foreach (ErrorLogViewModel.ErrorEntry item in errorLogEntries)
            this._loggingService.getLogger().LogAppInformation(LoggingContext.ERRORSMM, $"{item.timestamp}; {item.pcsError}; {item.pcsErrorDescription}", memberName: nameof (Initialize), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ErrorLogViewModel.cs", sourceLineNumber: 50);
          this._displayErrorLog = true;
        }
        else
          this._loggingService.getLogger().LogAppInformation(LoggingContext.ERRORSMM, "No errors retrieved from appliance", memberName: nameof (Initialize), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ErrorLogViewModel.cs", sourceLineNumber: 56);
        this.errorEntries = new List<ErrorLogViewModel.ErrorEntry>((IEnumerable<ErrorLogViewModel.ErrorEntry>) errorLogEntries);
        jobj = (JObject) null;
        errorTokens = (IEnumerable<JToken>) null;
        errorLogEntries = (List<ErrorLogViewModel.ErrorEntry>) null;
      }
      catch (Exception ex)
      {
        this._loggingService.getLogger().LogAppError(LoggingContext.APPLIANCE, "Failed with " + ex.Message, ex, nameof (Initialize), "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ErrorLogViewModel.cs", 63 /*0x3F*/);
      }
      StringBuilder sb = new StringBuilder();
      int i = 0;
      foreach (ErrorLogViewModel.ErrorEntry _ee in this.errorEntries)
      {
        ++i;
        this.ErrorLogArea = $"{this.ErrorLogArea}{_ee.timestamp}, {_ee.pcsError}\n";
        if (i == this.errorEntries.Count)
          sb.Append($"{_ee.timestamp} - {_ee.pcsError}");
        else
          sb.AppendLine($"{_ee.timestamp} - {_ee.pcsError}");
      }
      await this.SaveToHistoryDB(sb.ToString());
      sb = (StringBuilder) null;
    }
    else
    {
      this.ErrorLogArea = "";
      this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Error log attempt stderr " + this._response.ErrorMessage, memberName: nameof (Initialize), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ErrorLogViewModel.cs", sourceLineNumber: 86);
      await this._alertService.ShowMessageAlertWithKey("ERROR_PAGE_NOTSUPPORTED", AppResource.INFORMATION_TEXT);
    }
  }

  private async Task SaveToHistoryDB(string dataTobeSaved)
  {
    try
    {
      string sessionID = UtilityFunctions.getSessionIDForHistoryTable();
      CoreApp.history.SaveItem(new History(this.RepairEnumber, DateTime.Now, sessionID, HistoryDBInfoType.ErrorLog.ToString(), dataTobeSaved));
      sessionID = (string) null;
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Failed to Error data in the History DB, " + ex?.ToString(), memberName: nameof (SaveToHistoryDB), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ErrorLogViewModel.cs", sourceLineNumber: 100);
    }
  }

  public bool _viewstatus { get; set; }

  public bool _hasPopupappeared { get; set; }

  public string _ConnectionGraphic { get; internal set; }

  private bool _hasNoErrorLogEntries { get; set; }

  public ErrorLogViewModel(
    IMvxNavigationService navigationService,
    IShortTextsService _SService,
    ILoggingService loggingService,
    IMetadataService metadataService,
    IPlatformSpecificServiceLocator locator,
    IUserSession userSession,
    IAlertService alertService,
    IAppliance appliance)
  {
    this.ReturnToSMM = (ICommand) new Command(new Action(this.ReturnToSMMFunction));
    this._navigationService = navigationService;
    this._userSession = userSession;
    this._loggingService = loggingService;
    this._metadataService = metadataService;
    this._locator = locator;
    this._alertService = alertService;
    this._appliance = appliance;
    this._RepairEnumber = this._userSession.getEnumberSession();
    this._hasPopupappeared = false;
    if (this._metadataService.isSMMWithWifi(this._RepairEnumber))
      this._ConnectionGraphic = this._metadataService.getConnectionGraphic(this._RepairEnumber);
    else
      this._ConnectionGraphic = this._metadataService.getConnectionGraphicNonSmm(this._RepairEnumber);
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

  public string TimestampLabel
  {
    get => this._TimestampLabel;
    internal set
    {
      this._TimestampLabel = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.TimestampLabel));
    }
  }

  public string ErrorCodeLabel
  {
    get => this._ErrorCodeLabel;
    internal set
    {
      this._ErrorCodeLabel = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.ErrorCodeLabel));
    }
  }

  public bool DisplayErrorLog
  {
    get => this._displayErrorLog;
    internal set
    {
      this._displayErrorLog = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.DisplayErrorLog));
    }
  }

  public bool HasNoErrorLogEntries
  {
    get => this._hasNoErrorLogEntries;
    internal set
    {
      this._hasNoErrorLogEntries = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.HasNoErrorLogEntries));
    }
  }

  public string NoErrorLog
  {
    get => this.noErrorLog;
    internal set
    {
      this.noErrorLog = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.NoErrorLog));
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
    private set
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

  public string Instruction
  {
    get => this._Instruction;
    internal set
    {
      this._Instruction = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.Instruction));
    }
  }

  private void ReturnToSMMFunction()
  {
    this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
  }

  public ICommand ReturnToSMM { protected set; get; }

  public ErrorLogViewModel.ErrorEntry ObjItemSelected
  {
    get => this._ItemSelected;
    set
    {
      if (this._ItemSelected == value)
        return;
      this._ItemSelected = value;
      this.RaisePropertyChanged<ErrorLogViewModel.ErrorEntry>((Expression<Func<ErrorLogViewModel.ErrorEntry>>) (() => this.ObjItemSelected));
      if (this._ItemSelected.pcsErrorHasDescription)
      {
        IMvxNavigationService navigationService = this._navigationService;
        DetailNavigationArgs detailNavigationArgs = new DetailNavigationArgs();
        detailNavigationArgs.errorDetails = this._ItemSelected;
        CancellationToken cancellationToken = new CancellationToken();
        navigationService.Navigate<ErrorDetailViewModel, DetailNavigationArgs>(detailNavigationArgs, (IMvxBundle) null, cancellationToken);
      }
    }
  }

  public List<ErrorLogViewModel.ErrorEntry> errorEntries
  {
    get => this._errorEntries;
    internal set
    {
      this._errorEntries = value;
      this.RaisePropertyChanged<List<ErrorLogViewModel.ErrorEntry>>((Expression<Func<List<ErrorLogViewModel.ErrorEntry>>>) (() => this.errorEntries));
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

  public override async void ViewAppeared()
  {
    base.ViewAppeared();
    this.AreButtonsEnabled = true;
    if (!this._hasPopupappeared && !this.HasNoErrorLogEntries)
    {
      await this._alertService.ShowMessageAlertWithKey("ERROR_LOG_TIMESTAMP_POPUPSTRING", AppResource.INFORMATION_TEXT, AppResource.WARNING_OK, (Action) (() => { }));
      this._hasPopupappeared = true;
    }
    this._viewstatus = true;
  }

  internal void updateStatus()
  {
    this.WifiStatus = this._appliance.StatusOfConnection;
    this.ConnectedColor = this._appliance.ConnectedColor;
  }

  public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
  {
    return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local).AddMilliseconds(unixTimeStamp).ToLocalTime();
  }

  public string getErrorMessage(string errorCode)
  {
    string errorCodeMessage = this._metadataService.getErrorCodeMessage(errorCode, this._RepairEnumber);
    if (errorCodeMessage == "" || errorCodeMessage == null)
      return errorCode;
    string shortTextMapping = this._metadataService.getMessageWithShortTextMapping(errorCodeMessage);
    return $"{errorCode} {shortTextMapping}";
  }

  public bool hasErrorCodeDescription(string errorCode)
  {
    bool flag = true;
    string errorCodeDescription = this._metadataService.getErrorCodeDescription(errorCode, this._RepairEnumber);
    if (errorCodeDescription == "" || errorCodeDescription == null)
      flag = false;
    return flag;
  }

  internal void GoToErrorDetailsPage()
  {
    this._navigationService.Navigate<ErrorDetailViewModel>((IMvxBundle) null, new CancellationToken());
  }

  public string getErrorDescription(string errorCode)
  {
    string errorCodeDescription = this._metadataService.getErrorCodeDescription(errorCode, this._RepairEnumber);
    return errorCodeDescription == "" || errorCodeDescription == null ? "" : this._metadataService.getMessageWithShortTextMapping(errorCodeDescription);
  }

  [ExcludeFromCodeCoverage]
  public class ErrorEntry
  {
    public string timestamp { get; set; }

    public string pcsError { get; set; }

    public string pcsErrorDescription { get; set; }

    public bool pcsErrorHasDescription { get; set; }
  }
}
