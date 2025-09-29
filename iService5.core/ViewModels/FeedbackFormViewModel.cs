// Decompiled with JetBrains decompiler
// Type: iService5.Core.ViewModels.FeedbackFormViewModel
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Backend;
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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
//using Xamarin.Essentials;
//using Xamarin.Forms;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

#nullable disable
namespace iService5.Core.ViewModels;

public class FeedbackFormViewModel : MvxViewModel
{
  private readonly IPlatformSpecificServiceLocator _locator;
  private readonly IMvxNavigationService _navigationService;
  private readonly IAlertService _alertService;
  private readonly IBinaryDownloadSession<HttpWebRequest> _binaryDownloadSession;
  private readonly IUserSession _userSession;
  private readonly ILoggingService _loggingService;
  private readonly ISecureStorageService _secureStorageService;
  private readonly IMetadataService _metadataService;
  private bool sessionLog;
  private string selectedSessionLogFileName;
  private Dictionary<string, string> feedbackFormPostParameters = new Dictionary<string, string>();
  private string fromLabel;
  private string emailAddress;
  private bool displayActivityIndicator = false;
  private bool blockSendForm = false;
  private string reportAProblemTitle = AppResource.REPORT_A_PROBLEM_TEXT;
  private string titleLabel;
  private string emailTitle = "";
  private string emailNote = "";
  private string noteLabel;
  private string attachmentLabel;
  private string sendButtonLabel;
  private string selectionText = AppResource.FEEDBACK_PAGE_NOT_RELATED_TO_ENUMBER;
  private bool noRelationSelected = true;
  private double labelHeight;
  private bool _IsBtnDisabled;
  private bool _ListDisplay = false;
  private List<History> _ListofVisits;
  private History _selectedvisit;
  private double _DeviceFontMultiplier;
  private string _NavText = AppResource.APPLIANCE_FLASH_BACK_TEXT;

  public virtual async Task Initialize() => await base.Initialize();

  public FeedbackFormViewModel(
    IMvxNavigationService navigationService,
    IShortTextsService _ShortTextsService,
    IAlertService alertService,
    IPlatformSpecificServiceLocator locator,
    IBinaryDownloadSession<HttpWebRequest> binaryDownloadSession,
    IUserSession userSession,
    IUserAccount userAccount,
    ILoggingService loggingService,
    ISecureStorageService secureStorageService,
    IMetadataService metadataService)
  {
    this._locator = locator;
    this._binaryDownloadSession = binaryDownloadSession;
    this.GoToSettingPage = (ICommand) new Command(new Action(this.VisitSettingPage));
    this.SubmitCommand = (ICommand) new Command((Action) (async () => await this.OnSubmit()));
    this.ComboBox_OnTapped = (ICommand) new Command(new Action(this.ToggleVisitListVisibility));
    this._navigationService = navigationService;
    this._alertService = alertService;
    this._userSession = userSession;
    this._loggingService = loggingService;
    this._secureStorageService = secureStorageService;
    this._metadataService = metadataService;
    this.sessionLog = false;
    if (_ShortTextsService != null)
    {
      this.fromLabel = AppResource.FEEDBACK_PAGE_FROM_LABEL + ":";
      this.TitleLabel = AppResource.FEEDBACK_PAGE_TITLE_LABEL + ":";
      this.NoteLabel = AppResource.FEEDBACK_PAGE_NOTE_LABEL + ":";
      this.IsBtnDisabled = !UtilityFunctions.fileExists("iS5Log*");
      this.AttachmentLabel = this.GetAttachmentLabelText();
      this._ListofVisits = CoreApp.history.GetHistoryList();
      this.SendButtonLabel = AppResource.FEEDBACK_PAGE_SEND_BUTTON;
    }
    if (!(DeviceInfo.Platform != DevicePlatform.Unknown))
      return;
    this.DeviceFontMultiplier = 0.7 * Device.GetNamedSize(NamedSize.Medium, typeof (Label));
    this.LabelHeight = 0.05 * Application.Current.MainPage.Height;
  }

  public string FromLabel
  {
    get => this.fromLabel;
    private set
    {
      this.fromLabel = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.FromLabel));
    }
  }

  public string EmailAddress
  {
    get => this.emailAddress;
    internal set
    {
      this.emailAddress = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.EmailAddress));
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

  public bool BlockSendForm
  {
    get => this.blockSendForm;
    internal set
    {
      this.blockSendForm = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.BlockSendForm));
    }
  }

  public string ReportAProblemTitle
  {
    get => this.reportAProblemTitle;
    internal set
    {
      this.reportAProblemTitle = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.ReportAProblemTitle));
    }
  }

  public string TitleLabel
  {
    get => this.titleLabel;
    private set
    {
      this.titleLabel = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.TitleLabel));
    }
  }

  public string EmailTitle
  {
    get => this.emailTitle;
    set
    {
      this.emailTitle = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.EmailTitle));
    }
  }

  public string EmailNote
  {
    get => this.emailNote;
    set
    {
      this.emailNote = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.EmailNote));
    }
  }

  public string NoteLabel
  {
    get => this.noteLabel;
    private set
    {
      this.noteLabel = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.NoteLabel));
    }
  }

  public string AttachmentLabel
  {
    get => this.attachmentLabel;
    private set
    {
      this.attachmentLabel = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.AttachmentLabel));
    }
  }

  public string SendButtonLabel
  {
    get => this.sendButtonLabel;
    private set
    {
      this.sendButtonLabel = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.SendButtonLabel));
    }
  }

  public string SelectionText
  {
    get => this.selectionText;
    private set
    {
      this.selectionText = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.SelectionText));
    }
  }

  public bool NoRelationSelected
  {
    get => this.noRelationSelected;
    internal set
    {
      this.noRelationSelected = value;
      this.SelectionText = !this.noRelationSelected ? $"{this.SelectedVisit.timestamp.ToString()} {this.SelectedVisit.eNumber}" : AppResource.FEEDBACK_PAGE_NOT_RELATED_TO_ENUMBER;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.NoRelationSelected));
    }
  }

  public double LabelHeight
  {
    get => this.labelHeight;
    internal set
    {
      this.labelHeight = value;
      this.RaisePropertyChanged<double>((Expression<Func<double>>) (() => this.LabelHeight));
    }
  }

  public ICommand SubmitCommand { internal set; get; }

  public ICommand ComboBox_OnTapped { internal set; get; }

  public bool IsBtnDisabled
  {
    internal set
    {
      this._IsBtnDisabled = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.IsBtnDisabled));
    }
    get => this._IsBtnDisabled;
  }

  internal void VisitSettingPage()
  {
    this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
  }

  internal void ToggleVisitListVisibility()
  {
    this.DropDownTapped = true;
    if (this.ListDisplay)
      this.ListDisplay = false;
    else
      this.ListDisplay = true;
  }

  public bool ListDisplay
  {
    internal set
    {
      this._ListDisplay = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.ListDisplay));
    }
    get => this._ListDisplay;
  }

  public List<History> ListofVisits
  {
    internal set
    {
      this._ListofVisits = value;
      this.RaisePropertyChanged<List<History>>((Expression<Func<List<History>>>) (() => this.ListofVisits));
    }
    get => this._ListofVisits;
  }

  public History SelectedVisit
  {
    get => this._selectedvisit;
    set
    {
      this._selectedvisit = value;
      if (this._selectedvisit != null)
      {
        this.NoRelationSelected = false;
        this.SelectionText = $"{this._selectedvisit.timestamp.ToString()}    {this._selectedvisit.eNumber}";
        this.ListDisplay = false;
        this.selectedSessionLogFileName = this.selectAppropriateSessionLogFilename();
      }
      else
      {
        this.NoRelationSelected = true;
        this.SelectionText = AppResource.FEEDBACK_PAGE_NOT_RELATED_TO_ENUMBER;
        this.ListDisplay = false;
        this.sessionLog = false;
        this.selectedSessionLogFileName = "";
      }
      this.AttachmentLabel = this.GetAttachmentLabelText();
      this.RaisePropertyChanged<History>((Expression<Func<History>>) (() => this.SelectedVisit));
    }
  }

  public double DeviceFontMultiplier
  {
    internal set
    {
      this._DeviceFontMultiplier = value;
      this.RaisePropertyChanged<double>((Expression<Func<double>>) (() => this.DeviceFontMultiplier));
    }
    get => this._DeviceFontMultiplier;
  }

  public ICommand GoToSettingPage { internal set; get; }

  public string NavText
  {
    get => this._NavText;
    internal set
    {
      this._NavText = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.NavText));
    }
  }

  public bool DropDownTapped { get; internal set; }

  public async Task OnSubmit()
  {
    this.DisplayActivityIndicator = true;
    if (!this.IsBtnDisabled)
    {
      if (!this.BlockSendForm)
        await this.SendFeedback();
    }
    else
    {
      this.DisplayActivityIndicator = false;
      this._alertService.ShowMessageAlertWithKey("FEEDBACK_PAGE_ATTACHMENT_LABEL_NONE", AppResource.WARNING_TEXT);
    }
    CoreApp.settings.UpdateItem(new Settings("FeedbackFormEmailAddress", this.emailAddress));
  }

  public string GetAttachmentLabelText()
  {
    bool flag1 = false;
    if (this._selectedvisit != null && this._selectedvisit.eNumber != null)
      flag1 = this._metadataService.isSMM(this._selectedvisit.eNumber);
    string filename = $"{Constants.BridgeLogDirName}/{Constants.bridgeMainLogFileName}";
    bool flag2 = false;
    if (Directory.Exists(Constants.BridgeLogDirPath) && UtilityFunctions.fileExists(filename))
    {
      this._loggingService.getLogger().LogAppInformation(LoggingContext.LOCAL, "Main bridge log file exists", memberName: nameof (GetAttachmentLabelText), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/FeedbackFormViewModel.cs", sourceLineNumber: 463);
      flag2 = true;
    }
    bool flag3 = true;
    if (!UtilityFunctions.fileExists("iS5Log*"))
    {
      this._loggingService.getLogger().LogAppError(LoggingContext.LOCAL, "iS5Log* file(s) do not exist", memberName: nameof (GetAttachmentLabelText), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/FeedbackFormViewModel.cs", sourceLineNumber: 470);
      flag3 = false;
    }
    string attachmentLabelText = "";
    if (!flag3 && !flag2 && !this.sessionLog)
    {
      attachmentLabelText = AppResource.FEEDBACK_PAGE_ATTACHMENT_LABEL_NONE;
    }
    else
    {
      if (flag3)
        attachmentLabelText = AppResource.FEEDBACK_PAGE_ATTACHMENT_LABEL;
      if (this._selectedvisit != null & flag3)
        attachmentLabelText = AppResource.FEEDBACK_PAGE_ATTACHMENT_PLUS_HISTORY_LABEL;
      if (flag2 && !flag1)
        attachmentLabelText = !string.IsNullOrEmpty(attachmentLabelText) ? $"{attachmentLabelText}, {Constants.bridgeLogFileAttachmentName}" : Constants.bridgeLogFileAttachmentName;
      if (this.sessionLog && !flag1)
      {
        this._loggingService.getLogger().LogAppInformation(LoggingContext.LOCAL, "Session log file exists", memberName: nameof (GetAttachmentLabelText), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/FeedbackFormViewModel.cs", sourceLineNumber: 506);
        attachmentLabelText = !string.IsNullOrEmpty(attachmentLabelText) ? $"{attachmentLabelText}, {Constants.sessionLogFileAttachmentName}" : Constants.sessionLogFileAttachmentName;
      }
    }
    return attachmentLabelText;
  }

  private string selectAppropriateSessionLogFilename()
  {
    string eNumber = this._selectedvisit.eNumber;
    bool flag = this._metadataService.isSMM(eNumber);
    if (Directory.Exists(Constants.BridgeLogDirPath) && !flag)
    {
      string str1 = eNumber.Replace("/", "_");
      string str2 = this._selectedvisit.timestamp.ToString("yyyy-MM-ddhh_mm_ss");
      if (UtilityFunctions.fileExists($"{Constants.BridgeLogDirName}/local_{str1}{str2}{Constants.zipFileSuffix}" ?? ""))
      {
        this.sessionLog = true;
        return str1 + str2 + Constants.zipFileSuffix;
      }
      DateTime dateTime1 = this._selectedvisit.timestamp;
      dateTime1 = dateTime1.Date;
      string str3 = dateTime1.ToString("yyyy-MM-dd");
      string str4 = $"{Constants.BridgeLogDirName}/local_{str1}{str3}";
      FileInfo[] array = ((IEnumerable<FileInfo>) UtilityFunctions.GetDirectory().GetFiles(str4 + "*")).ToArray<FileInfo>();
      if (((IEnumerable<FileInfo>) array).Count<FileInfo>() > 0)
      {
        DateTime timestamp = this._selectedvisit.timestamp;
        DateTime dateTime2 = this._selectedvisit.timestamp.AddMinutes(60.0);
        try
        {
          foreach (FileSystemInfo fileSystemInfo in array)
          {
            string name = fileSystemInfo.Name;
            int startIndex = name.IndexOf("-") - 4;
            int length = name.Length - startIndex;
            DateTime exact = DateTime.ParseExact(name.Substring(startIndex, length).Replace(".zip", ""), "yyyy-MM-ddHH_mm_ss", (IFormatProvider) CultureInfo.InvariantCulture);
            if (exact >= timestamp && exact <= dateTime2)
            {
              this.sessionLog = true;
              return name;
            }
            this.sessionLog = false;
          }
        }
        catch (Exception ex)
        {
          this._loggingService.getLogger().LogAppError(LoggingContext.BACKEND, "Exception occured while trying the getting the matching session log file existance: " + ex?.ToString(), memberName: nameof (selectAppropriateSessionLogFilename), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/FeedbackFormViewModel.cs", sourceLineNumber: 569);
        }
      }
      else
        this.sessionLog = false;
    }
    return "";
  }

  public async Task SendFeedback()
  {
    try
    {
      if (!string.IsNullOrWhiteSpace(this.emailTitle) || !string.IsNullOrWhiteSpace(this.emailNote))
      {
        string emailMessage = "";
        this.BlockSendForm = true;
        this.feedbackFormPostParameters.Add("subject", this.emailTitle);
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("\nEmail:");
        if (string.IsNullOrEmpty(this.emailAddress))
          sb.AppendLine("Not Available");
        else
          sb.AppendLine(this.emailAddress);
        if (this.NoRelationSelected)
        {
          sb.AppendLine("\n\n" + this.SelectionText);
          sb.AppendLine("\n\nProblem description:");
          sb.AppendLine(this.emailNote);
        }
        else
        {
          sb.AppendLine("\n\nRelated repair visit:");
          StringBuilder stringBuilder1 = sb;
          DateTime timestamp = this.SelectedVisit.timestamp;
          string str1 = $"{timestamp.ToString()} {this.SelectedVisit.eNumber}";
          stringBuilder1.AppendLine(str1);
          sb.AppendLine("\n\nProblem description:");
          sb.AppendLine(this.emailNote);
          sb.AppendLine("\n\n***Information about related repair visit***\n");
          sb.AppendLine(AppResource.HISTORY_DETAILS_HEADER_DATE_TIME_OF_REPAIR);
          List<History> historyDetailsList = CoreApp.history.GetHistoryDetails(this.SelectedVisit.sessionID);
          if (historyDetailsList.Count != 0)
          {
            StringBuilder stringBuilder2 = sb;
            timestamp = historyDetailsList[0].timestamp;
            string str2 = timestamp.ToString((IFormatProvider) UtilityFunctions.GetCurrentCulture()) + "\n";
            stringBuilder2.AppendLine(str2);
            sb.AppendLine(AppResource.HISTORY_DETAILS_HEADER_RIS);
            sb.AppendLine(AppResource.NOT_YET_AVAILABLE + "\n");
            UtilityFunctions.HistoryStringBuilders hsbs = new UtilityFunctions.HistoryStringBuilders()
            {
              haInfoSB = (StringBuilder) null,
              errorLogSB = (StringBuilder) null,
              programLogSB = (StringBuilder) null,
              measureLogSB = (StringBuilder) null,
              monitoringLogSB = (StringBuilder) null,
              codingLogSB = (StringBuilder) null,
              flashLogSB = (StringBuilder) null,
              memoryLogSB = (StringBuilder) null
            };
            UtilityFunctions.SetHistoryStringBuilders(hsbs, historyDetailsList);
            if (hsbs.haInfoSB != null)
            {
              sb.AppendLine(AppResource.HISTORY_DETAILS_HEADER_HOME_APPLIANCE_DATA);
              sb.AppendLine(hsbs.haInfoSB.ToString() + "\n");
            }
            if (hsbs.errorLogSB != null)
            {
              sb.AppendLine(AppResource.HISTORY_DETAILS_HEADER_ERROR_LOG);
              sb.AppendLine(hsbs.errorLogSB.ToString() + "\n");
            }
            if (hsbs.memoryLogSB != null)
            {
              sb.AppendLine(AppResource.HISTORY_DETAILS_HEADER_MEMORY_LOG);
              sb.AppendLine(hsbs.memoryLogSB.ToString() + "\n");
            }
            if (hsbs.programLogSB != null)
            {
              sb.AppendLine(AppResource.PROGRAM_TEXT);
              sb.AppendLine(hsbs.programLogSB.ToString() + "\n");
            }
            if (hsbs.flashLogSB != null)
            {
              sb.AppendLine(AppResource.HISTORY_DETAILS_HEADER_FLASH_LOG);
              sb.AppendLine(hsbs.flashLogSB.ToString() + "\n");
            }
            if (hsbs.measureLogSB != null)
            {
              sb.AppendLine(AppResource.HISTORY_DETAILS_HEADER_MEASURE_LOG);
              sb.AppendLine(hsbs.measureLogSB.ToString() + "\n");
            }
            if (hsbs.monitoringLogSB != null)
            {
              sb.AppendLine(AppResource.HISTORY_DETAILS_HEADER_MONITORING_LOG);
              sb.AppendLine(hsbs.monitoringLogSB.ToString() + "\n");
            }
            if (hsbs.codingLogSB != null)
            {
              sb.AppendLine(AppResource.HISTORY_DETAILS_HEADER_CODING_LOG);
              sb.AppendLine(hsbs.codingLogSB.ToString() + "\n");
            }
            hsbs = (UtilityFunctions.HistoryStringBuilders) null;
          }
          historyDetailsList = (List<History>) null;
        }
        emailMessage = sb.ToString();
        this.feedbackFormPostParameters.Add("message", emailMessage);
        this.feedbackFormPostParameters.Add("sessionLogFilename", this.selectedSessionLogFileName);
        if (this.AttachmentLabel.Contains(Constants.bridgeLogFileAttachmentName))
          this.feedbackFormPostParameters.Add("MainLogFilename", Constants.bridgeLogFileAttachmentName);
        if (await this._locator.GetPlatformSpecificService().IsNetworkAvailable())
        {
          this._loggingService.getLogger().LogAppInformation(LoggingContext.USER, "Connection available..Trying to send support mail", memberName: nameof (SendFeedback), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/FeedbackFormViewModel.cs", sourceLineNumber: 681);
          this.sendSupportForm();
        }
        else
        {
          this._loggingService.getLogger().LogAppInformation(LoggingContext.USER, "Connection Not available..Setting Send Status to NOT_SENT", memberName: nameof (SendFeedback), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/FeedbackFormViewModel.cs", sourceLineNumber: 686);
          this.DisplayActivityIndicator = false;
          MessagingCenter.Send<FeedbackFormViewModel>(this, CoreApp.EventsNames.FeedBackFormPending.ToString());
          this._alertService.ShowMessageAlertWithKey("FEEDBACK_PAGE_GOONLINE", AppResource.WARNING_TEXT);
          Settings settings = new Settings("SupportFormSendStatus", "NOT_SENT");
          CoreApp.settings.UpdateItem(new Settings("SupportFormSendStatus", "NOT_SENT"));
          settings.Name = "SupportFormSavedSubject";
          settings.Value = this.emailTitle;
          CoreApp.settings.UpdateItem(settings);
          settings.Name = "SupportFormSavedNote";
          settings.Value = emailMessage;
          CoreApp.settings.UpdateItem(settings);
          settings.Name = "SupportFormSavedSessionLogFileName";
          settings.Value = this.selectedSessionLogFileName;
          CoreApp.settings.UpdateItem(settings);
          this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
          settings = (Settings) null;
        }
        emailMessage = (string) null;
        sb = (StringBuilder) null;
      }
      else
      {
        this.DisplayActivityIndicator = false;
        await this._alertService.ShowMessageAlertWithKey("EMPTY_FEEDBACK_ALERT_MESSAGE", AppResource.INFORMATION_TEXT);
      }
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppError(LoggingContext.LOCAL, "Issue in Send Feedback method due to " + ex.Message, memberName: nameof (SendFeedback), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/FeedbackFormViewModel.cs", sourceLineNumber: 712);
      this.DisplayActivityIndicator = false;
      this._alertService.ShowMessageAlertWithKey("FEEDBACK_PAGE_ERROR_TEXT", AppResource.INFORMATION_TEXT);
      int num = await this._navigationService.Close((IMvxViewModel) this, new CancellationToken()) ? 1 : 0;
    }
  }

  public override void ViewAppeared() => base.ViewAppeared();

  public override void ViewAppearing()
  {
    base.ViewAppearing();
    Settings settings = CoreApp.settings.GetItem("FeedbackFormEmailAddress");
    if (settings == null || !(settings.Value != ""))
      return;
    this.EmailAddress = settings.Value;
  }

  public void sendSupportForm()
  {
    this._userSession.sendSupportForm(this.feedbackFormPostParameters, new iService5.Core.Services.User.sendSupportFormCompletionCallback(this.sendSupportFormCompletionCallback));
  }

  public void sendSupportFormCompletionCallback(SupportMailSendStatus res)
  {
    this.DisplayActivityIndicator = false;
    CoreApp.settings.UpdateItem(new Settings("SupportFormSendStatus", "SENT"));
    switch (res)
    {
      case SupportMailSendStatus.SUCCESS:
        this._alertService.ShowMessageAlertWithKey("FEEDBACK_PAGE_ACKNOWLEDGEMENT_TEXT", AppResource.INFORMATION_TEXT);
        this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
        break;
      case SupportMailSendStatus.EXCEPTION:
        ServiceError serviceError = this._userSession.getServiceError();
        this._loggingService.getLogger().LogAppError(LoggingContext.BACKEND, $"There occured a problem while sending support mail: {serviceError.errorKey.ToString()}Error Message: {serviceError.errorMessage}", memberName: nameof (sendSupportFormCompletionCallback), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/FeedbackFormViewModel.cs", sourceLineNumber: 760);
        this._alertService.ShowMessageAlertWithKey("REPORT_PROBLEM_COMMUNICATION_ERROR", AppResource.INFORMATION_TEXT);
        this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
        break;
      case SupportMailSendStatus.INTERNET_ISSUE:
        CoreApp.settings.UpdateItem(new Settings("SupportFormSendStatus", "NOT_SENT"));
        this._alertService.ShowMessageAlertWithKey("INTERNET_ERROR_TEXT", AppResource.INFORMATION_TEXT);
        this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
        break;
      case SupportMailSendStatus.TIMEOUT:
        this._loggingService.getLogger().LogAppError(LoggingContext.BACKEND, "The operation has timed out", memberName: nameof (sendSupportFormCompletionCallback), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/FeedbackFormViewModel.cs", sourceLineNumber: 753);
        this._alertService.ShowMessageAlertWithKey("REPORT_PROBLEM_COMMUNICATION_ERROR", AppResource.INFORMATION_TEXT);
        this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
        break;
      default:
        this._alertService.ShowMessageAlertWithKey("FEEDBACK_PAGE_ERROR_TEXT", AppResource.INFORMATION_TEXT);
        this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
        break;
    }
  }
}
