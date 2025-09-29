// Decompiled with JetBrains decompiler
// Type: iService5.Core.ViewModels.ApplianceNonSMMFlashViewModel
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
using iService5.Ssh.DTO;
using iService5.Ssh.models;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
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

public class ApplianceNonSMMFlashViewModel : MvxViewModel<FlashingParameter>
{
  private readonly IMvxNavigationService _navigationService;
  private readonly IMetadataService _metadataService;
  private readonly Is5SshWrapper _sshWrapper;
  private readonly IAlertService _alertService;
  private readonly IPlatformSpecificServiceLocator _locator;
  private readonly IAppliance _appliance;
  private readonly ILoggingService _loggingService;
  private readonly string PopupString;
  internal bool tapped = false;
  private readonly IUserSession _userSession;
  internal FlashingItem FlashingItem;
  private bool ComingFromCoding = false;
  private string ProgressForCoding;
  private int _Step = 0;
  private bool _IsBusy = true;
  private bool isBackButtonVisible = true;
  private bool isScrollDisabled = true;
  internal string _senderScreen = "";
  internal int StepTotal = 0;
  private string lastLine = "";
  private bool IsBatchNotAuto = false;
  private bool IsCodingNext = false;
  private FlashingItem nonAutoItem = (FlashingItem) null;
  private bool _RepairLabelVisibility = true;
  private string _CheckingApplianceStatus = "";
  private string _RepairEnumber;
  private string _FlashingHeader;
  private string _BulletHeader;
  private string _StatusOfConnection;
  private string _ProgramButtonLabel = AppResource.APPLIANCE_FLASH_START_BUTTON;
  private string finishButtonLabel = AppResource.APPLIANCE_FLASH_FINISH_BUTTON;
  private double _ProgressBarValue;
  private string _StepProgressForLabel;
  private string _ProgressBarValueText;
  private string _ProgramLogLabel = AppResource.APPLIANCE_FLASH_LOG;
  internal string _ConnectedColor = "Green";
  private string _ProgrammingLog = "";
  private string _ProgramProgressLabel = AppResource.FLASH;
  private string programProgressDoneLabel = AppResource.APPLIANCE_FLASH_PROGRAMMING_DONE;
  private string _CancelBackText = AppResource.APPLIANCE_FLASH_BACK_TEXT;
  private bool _ProgressVisibility = false;
  private string _OrangeBarText = AppResource.APPLIANCE_BOOT_MODE_RECOVERY;
  private List<NonSmmInstructions> _InstructionList = new List<NonSmmInstructions>();
  private List<string> _StartInstructionList;
  private List<string> _EndInstructionList;
  private bool _showInstructionListGrid = true;
  private bool IsBatchAuto = false;
  private bool firstAutoBatchPass = true;
  private string _WifiStatus;
  private bool _IsFlashBtnEnabled = true;
  private bool isFinishBtnEnabled = true;
  private bool isProgrammingInProgress = false;
  private string _InstructionSteps = "";
  private string _InstructionIndex = "";
  private string _FinishedProgrammingMsg = AppResource.MSG_FLASHING_IS_FINISHED;
  private string _FinishedProgrammingMsgColour = "Green";
  private bool _ShowFinishedProgrammingMsg = false;
  private string _ProgressBarColor = "Black";
  private bool _IsFinishBtnVisible = false;
  private bool _IsFlashBtnVisible = true;

  public virtual async Task Initialize() => await base.Initialize();

  public override void Prepare(FlashingParameter parameter)
  {
    this.Prepare();
    this.FlashingItem = parameter.FlashingItem;
    this.IsBatchAuto = parameter.IsBatchAuto;
    this.ComingFromCoding = parameter.ComingFromCoding;
    if (!this.ComingFromCoding)
      return;
    this.IsBackButtonVisible = false;
  }

  public ApplianceNonSMMFlashViewModel(
    IMvxNavigationService navigationService,
    IShortTextsService _SService,
    ILoggingService loggingService,
    IMetadataService metadataService,
    IPlatformSpecificServiceLocator locator,
    IUserSession userSession,
    Is5SshWrapper sshWrapper,
    IAlertService alertService,
    IAppliance appliance)
  {
    this._userSession = userSession;
    this._RepairEnumber = this._userSession.getEnumberSession();
    this._senderScreen = this._userSession.GetSenderScreen();
    if (this._senderScreen.Equals(AppResource.COMPACT_APPLIANCE_TESTER_TITLE))
    {
      this._RepairEnumber = AppResource.COMPACT_APPLIANCE_TESTER;
      FlashingItem flashingItem = new FlashingItem()
      {
        StartFlashingInstructions = new List<FlashingInstruction>()
        {
          new FlashingInstruction()
          {
            InstructionText = "New CAT firmware(v 1.0.81) is available,do you want to replace existing (v 1.0.80)?"
          }
        },
        EndFlashingInstructions = new List<FlashingInstruction>()
        {
          new FlashingInstruction()
          {
            InstructionText = "Device's firmware uploaded succesfully.Please reset the device by disconnecting it from mains for 2s"
          }
        }
      };
      this.StartInstructionList = new List<string>();
      foreach (FlashingInstruction flashingInstruction in flashingItem.StartFlashingInstructions)
        this.StartInstructionList.Add(flashingInstruction.InstructionText);
      this.EndInstructionList = new List<string>();
      foreach (FlashingInstruction flashingInstruction in flashingItem.EndFlashingInstructions)
        this.EndInstructionList.Add(flashingInstruction.InstructionText);
    }
    this._metadataService = metadataService;
    this._sshWrapper = sshWrapper;
    this._alertService = alertService;
    this._appliance = appliance;
    this.NavigatePreviousPage = (ICommand) new Command(new Action(this.VisitPage));
    this.FinishButtonSelected = (ICommand) new Command(new Action(this.CloseThePage));
    this.CancelCommand = (ICommand) new Command(new Action<object>(this.CancelButtonPressed));
    this._navigationService = navigationService;
    this._locator = locator;
    if (_SService != null)
    {
      this._loggingService = loggingService;
      this.PopupString = AppResource.FLASH_PAGE_POPUPSTRING;
    }
    this.Flash_Tapped = (ICommand) new Command((Action) (async () => await this.OnSubmitAsync()));
    this._ActiveView = false;
    this.ProgressVisibility = true;
    this.ProgressBarValueText = "0%";
    this.ProgressBarValue = 0.0;
    this.IsProgrammingInProgress = false;
    this._sshWrapper.IPAddress = this._locator.GetPlatformSpecificService().GetIp();
  }

  public ICommand NavigatePreviousPage { protected set; get; }

  public ICommand FinishButtonSelected { protected set; get; }

  internal void VisitPage()
  {
    if (this._senderScreen.Equals(AppResource.COMPACT_APPLIANCE_TESTER_TITLE))
    {
      this._navigationService.Navigate<TabViewModel>((IMvxBundle) null, new CancellationToken());
    }
    else
    {
      this._loggingService.getLogger().LogAppDebug(LoggingContext.APPLIANCE, "Programming Status:" + this.IsProgrammingInProgress.ToString(), memberName: nameof (VisitPage), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceNonSMMFlashViewModel.cs", sourceLineNumber: 140);
      try
      {
        if (!this._sshWrapper.DeactivateFlasher().Success)
          this._alertService.ShowMessageAlertWithMessageFromService(AppResource.APPLIANCE_PROGRAMMING_NOT_SUCCESSFUL, AppResource.ERROR_TITLE, AppResource.WARNING_OK, AppResource.CANCEL_LABEL, (Action<bool>) (buttonOKText => this.CloseThePage()));
        else
          this.CloseThePage();
      }
      catch (Exception ex)
      {
        this._loggingService.getLogger().LogAppError(LoggingContext.APPLIANCE, ex.Message, ex, nameof (VisitPage), "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceNonSMMFlashViewModel.cs", 155);
        this.CloseThePage();
      }
    }
  }

  private void CloseThePage()
  {
    MainThread.BeginInvokeOnMainThread((Action) (() => this._navigationService.Close((IMvxViewModel) this, new CancellationToken())));
  }

  public ICommand CancelCommand { internal set; get; }

  internal void CancelButtonPressed(object obj) => throw new NotImplementedException();

  public ICommand Flash_Tapped { internal set; get; }

  public int Step
  {
    get => this._Step;
    set
    {
      this._Step = value;
      this.ProgressBarValue = (double) this._Step / (double) this.StepTotal;
    }
  }

  public bool IsBusy
  {
    get => this._IsBusy;
    internal set
    {
      this._IsBusy = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.IsBusy));
    }
  }

  public bool IsBackButtonVisible
  {
    get => this.isBackButtonVisible;
    internal set
    {
      this.isBackButtonVisible = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.IsBackButtonVisible));
    }
  }

  public bool IsScrollDisabled
  {
    get => this.isScrollDisabled;
    internal set
    {
      this.isScrollDisabled = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.IsScrollDisabled));
    }
  }

  public string SenderScreen
  {
    get => this._senderScreen;
    internal set => this.SetProperty<string>(ref this._senderScreen, value, nameof (SenderScreen));
  }

  public void OnBackButtonPressed()
  {
    if (!this.IsBackButtonVisible)
      return;
    this.VisitPage();
  }

  public string FlasherActivation
  {
    get
    {
      return !this.ComingFromCoding ? AppResource.FLASHING_TRANSITION : AppResource.FLASHING_TRANSITION_FROM_CODING;
    }
  }

  internal async Task OnSubmitAsync()
  {
    if (this.IsBatchNotAuto && this.nonAutoItem != null)
    {
      this.IsBusy = true;
      this.InitializeVariables();
      this.IsProgrammingInProgress = false;
      this.IsBatchNotAuto = false;
      this.IsFlashBtnVisible = true;
      this.IsFinishBtnVisible = false;
      this.ProgramButtonLabel = AppResource.APPLIANCE_FLASH_START_BUTTON;
      this.ProgrammingLog = string.Empty;
      this.ShowFinishedProgrammingMsg = false;
      this.WriteInstructions(this.nonAutoItem);
      this.nonAutoItem = (FlashingItem) null;
      this.IsBusy = false;
    }
    else if (this.IsCodingNext)
    {
      this.GoToCoding(this.ProgressForCoding);
    }
    else
    {
      if (this.tapped)
        return;
      this.tapped = true;
      this.InitializeVariables();
      if (this._senderScreen.Equals(AppResource.COMPACT_APPLIANCE_TESTER_TITLE))
      {
        List<NonSmmInstructions> _progressInstructionList = new List<NonSmmInstructions>();
        _progressInstructionList.Add(new NonSmmInstructions()
        {
          InstructionIndex = "",
          InstructionSteps = "CAT Update started, please wait."
        });
        this.InstructionList = new List<NonSmmInstructions>((IEnumerable<NonSmmInstructions>) _progressInstructionList);
        _progressInstructionList = (List<NonSmmInstructions>) null;
      }
      this.AddHistoryBeforeFlashing();
      await Task.Factory.StartNew((Action) (() =>
      {
        try
        {
          if (this._senderScreen.Equals(AppResource.COMPACT_APPLIANCE_TESTER_TITLE))
          {
            for (int index = 0; index <= 100; ++index)
            {
              Thread.Sleep(300);
              this.ProgressBarValueText = index.ToString() + "%";
              this.ProgressBarValue = Convert.ToDouble(index) / 100.0;
              this.ProgrammingLog = $"{this.ProgrammingLog}Program Log {index.ToString()}\n";
            }
          }
          else
          {
            this._loggingService.getLogger().LogAppDebug(LoggingContext.BACKEND, "Flashing appliance module", memberName: nameof (OnSubmitAsync), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceNonSMMFlashViewModel.cs", sourceLineNumber: 318);
            SshResponse sshResponse = this._sshWrapper.InstallUpdateNonSmm((Stream) null, (iService5.Ssh.models.ProgressCallback) ((progress, step, total) =>
            {
              if (this.IsCodingNext)
                this.ProgressForCoding = progress;
              FlashingItem flashingItem = (FlashingItem) null;
              try
              {
                flashingItem = JsonConvert.DeserializeObject<FlashingItem>(progress);
              }
              catch (JsonSerializationException ex)
              {
                this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Ignoring invalid json response.", memberName: nameof (OnSubmitAsync), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceNonSMMFlashViewModel.cs", sourceLineNumber: 334);
              }
              catch (Exception ex)
              {
                this._loggingService.getLogger().LogAppError(LoggingContext.APPLIANCE, ex.Message, memberName: nameof (OnSubmitAsync), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceNonSMMFlashViewModel.cs", sourceLineNumber: 338);
              }
              if (flashingItem == null)
                return;
              if (flashingItem.FlashProgress != null)
              {
                if (!string.IsNullOrEmpty(flashingItem.FlashProgress.ProgressPercent))
                {
                  this.EditProgress(flashingItem.FlashProgress);
                  this.IsBatchNotAuto = false;
                }
                if (flashingItem.FlashProgress.InfoMessages != null && flashingItem.FlashProgress.InfoMessages.Count > 0)
                {
                  this.AddString(flashingItem.FlashProgress.InfoMessages);
                  this.IsBatchNotAuto = false;
                }
                if (flashingItem.FlashProgress.CtrlMessages != null && flashingItem.FlashProgress.CtrlMessages.Any<CtrlMessage>((Func<CtrlMessage, bool>) (x => x.NextFunction == "FLASHING")) && !flashingItem.FlashProgress.CtrlMessages.Any<CtrlMessage>((Func<CtrlMessage, bool>) (x => x.Mode != null)))
                {
                  this.IsBatchNotAuto = true;
                  this._loggingService.getLogger().LogAppInformation(LoggingContext.BACKEND, "Non auto batch detected.", memberName: nameof (OnSubmitAsync), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceNonSMMFlashViewModel.cs", sourceLineNumber: 359);
                }
                else if (flashingItem.FlashProgress.CtrlMessages != null && flashingItem.FlashProgress.CtrlMessages.Any<CtrlMessage>((Func<CtrlMessage, bool>) (x => x.NextFunction == "CODING")))
                {
                  this.IsCodingNext = true;
                  this._loggingService.getLogger().LogAppInformation(LoggingContext.BACKEND, "Coding next detected.", memberName: nameof (OnSubmitAsync), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceNonSMMFlashViewModel.cs", sourceLineNumber: 365);
                }
              }
              else if (this.IsBatchNotAuto && !string.IsNullOrEmpty(flashingItem.FlashingIndex))
                this.nonAutoItem = flashingItem;
            }));
            if (!sshResponse.Success)
            {
              this._loggingService.getLogger().LogAppError(LoggingContext.APPLIANCE, sshResponse.ErrorMessage, memberName: nameof (OnSubmitAsync), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceNonSMMFlashViewModel.cs", sourceLineNumber: 376);
              this.lastLine = "error";
            }
          }
        }
        catch (Exception ex)
        {
          this._loggingService.getLogger().LogAppError(LoggingContext.APPLIANCE, "Error during non-SMM flash programming: " + ex.Message, memberName: nameof (OnSubmitAsync), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceNonSMMFlashViewModel.cs", sourceLineNumber: 383);
        }
        this.AfterFlashing();
      }));
    }
  }

  internal void GoToCoding(string progress)
  {
    try
    {
      CodingItem codingItem = JsonConvert.DeserializeObject<CodingItem>(progress);
      this._navigationService.Navigate<NonSmmCodingTransitionViewModel, CodingParameter>(new CodingParameter()
      {
        CodingItem = codingItem,
        IsBatchAuto = this.IsBatchAuto,
        fromFlashing = true
      }, (IMvxBundle) null, new CancellationToken());
    }
    catch (JsonSerializationException ex)
    {
      this._loggingService.getLogger().LogAppError(LoggingContext.APPLIANCE, ex.Message, (Exception) ex, nameof (GoToCoding), "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceNonSMMFlashViewModel.cs", 406);
      this._alertService.ShowMessageAlertWithKey("APPLIANCE_FLASH_CODING_AFTER_FLASH_WRONG", AppResource.ERROR_TITLE);
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppError(LoggingContext.APPLIANCE, ex.Message, ex, nameof (GoToCoding), "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceNonSMMFlashViewModel.cs", 411);
      this._alertService.ShowMessageAlertWithKey("APPLIANCE_FLASH_CODING_AFTER_FLASH_EXCEPTION", AppResource.ERROR_TITLE);
    }
    this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
  }

  private void EditProgress(FlashProgress flashProgress)
  {
    this.ProgressBarValueText = flashProgress.ProgressPercent + "%";
    this.ProgressBarValue = Convert.ToDouble(flashProgress.ProgressPercent.Replace('.', NumberFormatInfo.CurrentInfo.NumberDecimalSeparator[0]).Replace(',', NumberFormatInfo.CurrentInfo.NumberDecimalSeparator[0])) / 100.0;
  }

  private void AddString(List<InfoMessage> infoMessages)
  {
    StringBuilder stringBuilder = new StringBuilder();
    foreach (InfoMessage infoMessage in infoMessages)
    {
      if (infoMessage.Message.StartsWith("ENDADVICE"))
        this.EndInstructionList.Add(infoMessage.Message.Substring(9));
      else
        stringBuilder.Append(infoMessage.Message + "\n");
    }
    if (stringBuilder.Length != 0)
      this.lastLine = stringBuilder.ToString();
    if (!(this.lastLine != ""))
      return;
    this.ProgrammingLog += this.lastLine;
  }

  private void WriteInstructions(FlashingItem valuesFeedback)
  {
    this.FlashingItem = valuesFeedback;
    try
    {
      this.StartInstructionList = new List<string>();
      if (this.FlashingItem.StartFlashingInstructions != null)
      {
        foreach (FlashingInstruction flashingInstruction in this.FlashingItem.StartFlashingInstructions)
          this.StartInstructionList.Add(flashingInstruction.InstructionText);
      }
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppError(LoggingContext.APPLIANCE, ex.Message, ex, nameof (WriteInstructions), "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceNonSMMFlashViewModel.cs", 460);
    }
    try
    {
      this.EndInstructionList = new List<string>();
      if (this.FlashingItem.EndFlashingInstructions != null)
      {
        foreach (FlashingInstruction flashingInstruction in this.FlashingItem.EndFlashingInstructions)
          this.EndInstructionList.Add(flashingInstruction.InstructionText);
      }
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppError(LoggingContext.APPLIANCE, ex.Message, ex, nameof (WriteInstructions), "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceNonSMMFlashViewModel.cs", 475);
    }
    try
    {
      this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, JsonConvert.SerializeObject((object) this.FlashingItem, Formatting.None), memberName: nameof (WriteInstructions), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceNonSMMFlashViewModel.cs", sourceLineNumber: 479);
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppError(LoggingContext.APPLIANCE, ex.Message, ex, nameof (WriteInstructions), "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceNonSMMFlashViewModel.cs", 483);
    }
    if (this.IsBatchAuto)
      return;
    List<NonSmmInstructions> collection = new List<NonSmmInstructions>();
    foreach (string startInstruction in this.StartInstructionList)
    {
      this.ShowInstructionListGrid = true;
      if (this._senderScreen.Equals(AppResource.COMPACT_APPLIANCE_TESTER_TITLE))
      {
        this.ShowInstructionListGrid = false;
        collection.Add(new NonSmmInstructions()
        {
          InstructionIndex = "",
          InstructionSteps = startInstruction
        });
      }
      else
        collection.Add(new NonSmmInstructions()
        {
          InstructionIndex = (this.StartInstructionList.IndexOf(startInstruction) + 1).ToString(),
          InstructionSteps = startInstruction
        });
    }
    this.InstructionList = new List<NonSmmInstructions>((IEnumerable<NonSmmInstructions>) collection);
  }

  private void InitializeVariables()
  {
    this.IsBackButtonVisible = false;
    this.IsProgrammingInProgress = true;
    this.IsScrollDisabled = true;
    this.ReturnAfterReboot = false;
    this.StepTotal = 0;
    this.Step = 0;
    this.ProgressVisibility = true;
    this.lastLine = "";
    this.ProgressBarValueText = "0%";
    this.ProgressBarValue = 0.0;
  }

  private void AddHistoryBeforeFlashing()
  {
    StringBuilder stringBuilder = new StringBuilder();
    foreach (NonSmmInstructions instruction in this.InstructionList)
      stringBuilder.AppendLine(instruction.InstructionSteps);
    stringBuilder.AppendLine("Programming started on " + DateTime.Now.ToString());
    try
    {
      this.logHistoryData(stringBuilder.ToString(), HistoryDBInfoType.FlashLogBefore);
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMNONSMM, "Failed to write before programming data in the History DB, " + ex?.ToString(), memberName: nameof (AddHistoryBeforeFlashing), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceNonSMMFlashViewModel.cs", sourceLineNumber: 532);
    }
  }

  private void AfterFlashing(bool isBatchAuto = false)
  {
    this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMNONSMM, "In after flashing method with isBatchAuto value: " + isBatchAuto.ToString(), memberName: nameof (AfterFlashing), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceNonSMMFlashViewModel.cs", sourceLineNumber: 538);
    this.InstructionList = new List<NonSmmInstructions>();
    if (!isBatchAuto)
    {
      if (this.lastLine.ToLower().Contains("error"))
      {
        this._alertService.ShowMessageAlertWithKey("APPLIANCE_FLASH_FAILURES", AppResource.WARNING_TEXT);
        this.ShowFinishedProgrammingMsg = true;
        this.FinishedProgrammingMsg = AppResource.MSG_FLASHING_ERROR;
        this.FinishedProgrammingMsgColour = "Red";
        this.ProgressBarColor = "Red";
        this.ProgressBarValueText = $"{AppResource.FINISHED_TEXT} - {AppResource.ERROR_TITLE}";
      }
      else
      {
        this.ShowFinishedProgrammingMsg = true;
        if (this.IsBatchNotAuto)
          this.ProgressBarColor = "Black";
        else if (!this.IsCodingNext)
        {
          this.ProgressBarColor = "Green";
          this.ProgressBarValueText = AppResource.FINISHED_TEXT;
        }
        else
        {
          this.ProgressBarColor = "Black";
          this.ProgressBarValueText = AppResource.FINISHED_TEXT_CODING;
        }
      }
      if (!this.lastLine.ToLower().Contains("error") && (this.IsBatchNotAuto || this.IsCodingNext))
      {
        this.IsFlashBtnVisible = true;
        this.IsFinishBtnVisible = false;
        this.ProgramButtonLabel = AppResource.CONTINUE_TEXT;
      }
      else
      {
        this.IsFlashBtnVisible = false;
        this.IsFinishBtnVisible = true;
      }
      this.IsScrollDisabled = false;
      this.IsProgrammingInProgress = false;
      this.ProgressVisibility = true;
      this.ProgressBarValue = 1.0;
    }
    if (!isBatchAuto && !this.IsBatchNotAuto)
    {
      List<NonSmmInstructions> collection = new List<NonSmmInstructions>();
      if (this.EndInstructionList != null)
      {
        this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMNONSMM, "EndInstructionList has value.", memberName: nameof (AfterFlashing), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceNonSMMFlashViewModel.cs", sourceLineNumber: 590);
        foreach (string endInstruction in this.EndInstructionList)
        {
          if (this._senderScreen.Equals(AppResource.COMPACT_APPLIANCE_TESTER_TITLE))
            collection.Add(new NonSmmInstructions()
            {
              InstructionIndex = "",
              InstructionSteps = endInstruction
            });
          else
            collection.Add(new NonSmmInstructions()
            {
              InstructionIndex = (this.EndInstructionList.IndexOf(endInstruction) + 1).ToString(),
              InstructionSteps = endInstruction
            });
        }
      }
      this.InstructionList = new List<NonSmmInstructions>((IEnumerable<NonSmmInstructions>) collection);
    }
    try
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine(this.ProgrammingLog);
      if (!isBatchAuto && !this.IsBatchNotAuto)
      {
        foreach (NonSmmInstructions instruction in this.InstructionList)
          stringBuilder.AppendLine(instruction.InstructionSteps);
      }
      stringBuilder.AppendLine("Programming finished on " + DateTime.Now.ToString());
      this.logHistoryData(stringBuilder.ToString(), HistoryDBInfoType.FlashLogAfter);
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMNONSMM, "Failed to write after programming data in the History DB, " + ex?.ToString(), memberName: nameof (AfterFlashing), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceNonSMMFlashViewModel.cs", sourceLineNumber: 619);
    }
    this.tapped = false;
  }

  private void logHistoryData(string programmingLog, HistoryDBInfoType infoType)
  {
    try
    {
      CoreApp.history.SaveItem(new History(this.RepairEnumber, DateTime.Now, UtilityFunctions.getSessionIDForHistoryTable(), infoType.ToString(), programmingLog));
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSMM, "Failed to save item in the History DB, " + ex?.ToString(), memberName: nameof (logHistoryData), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceNonSMMFlashViewModel.cs", sourceLineNumber: 633);
    }
  }

  public bool RepairLabelVisibility
  {
    get => this._RepairLabelVisibility;
    internal set
    {
      this._RepairLabelVisibility = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.RepairLabelVisibility));
    }
  }

  public string CheckingApplianceStatus
  {
    get => this._CheckingApplianceStatus;
    internal set
    {
      this._CheckingApplianceStatus = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.CheckingApplianceStatus));
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

  public string FlashingHeader
  {
    get => this._FlashingHeader;
    internal set
    {
      this._FlashingHeader = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.FlashingHeader));
    }
  }

  public string BulletHeader
  {
    get => this._BulletHeader;
    internal set
    {
      this._BulletHeader = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.BulletHeader));
    }
  }

  public string StatusOfConnection
  {
    get => this._StatusOfConnection;
    internal set
    {
      this._StatusOfConnection = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.StatusOfConnection));
    }
  }

  public string ProgramButtonLabel
  {
    get => this._ProgramButtonLabel;
    internal set
    {
      this._ProgramButtonLabel = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.ProgramButtonLabel));
    }
  }

  public string FinishButtonLabel
  {
    get => this.finishButtonLabel;
    internal set
    {
      this.finishButtonLabel = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.FinishButtonLabel));
    }
  }

  public double ProgressBarValue
  {
    get => this._ProgressBarValue;
    internal set
    {
      this._ProgressBarValue = value;
      this.RaisePropertyChanged<double>((Expression<Func<double>>) (() => this.ProgressBarValue));
    }
  }

  public string ProgressBarStep
  {
    get => this._StepProgressForLabel;
    internal set
    {
      this._StepProgressForLabel = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.ProgressBarStep));
    }
  }

  public string ProgressBarValueText
  {
    get => this._ProgressBarValueText;
    internal set
    {
      this._ProgressBarValueText = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.ProgressBarValueText));
    }
  }

  public string ProgramLogLabel
  {
    get => this._ProgramLogLabel;
    internal set
    {
      this._ProgramLogLabel = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.ProgramLogLabel));
    }
  }

  public string ConnectedColor
  {
    get => this._ConnectedColor;
    private set
    {
      this._ConnectedColor = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.ConnectedColor));
    }
  }

  public string ProgrammingLog
  {
    get => this._ProgrammingLog;
    internal set
    {
      this._ProgrammingLog = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.ProgrammingLog));
    }
  }

  public string ProgramProgressLabel
  {
    get => this._ProgramProgressLabel;
    internal set
    {
      this._ProgramProgressLabel = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.ProgramProgressLabel));
    }
  }

  public string ProgramProgressDoneLabel
  {
    get => this.programProgressDoneLabel;
    internal set
    {
      this.programProgressDoneLabel = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.ProgramProgressDoneLabel));
    }
  }

  public string CancelBackText
  {
    get => this._CancelBackText;
    internal set
    {
      this._CancelBackText = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.CancelBackText));
    }
  }

  public bool ProgressVisibility
  {
    get => this._ProgressVisibility;
    internal set
    {
      this._ProgressVisibility = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.ProgressVisibility));
    }
  }

  public string OrangeBarText
  {
    get => this._OrangeBarText;
    set
    {
      this._OrangeBarText = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.OrangeBarText));
    }
  }

  public bool _ActiveView { get; internal set; }

  public List<NonSmmInstructions> InstructionList
  {
    get => this._InstructionList;
    internal set
    {
      this._InstructionList = value;
      this.RaisePropertyChanged<List<NonSmmInstructions>>((Expression<Func<List<NonSmmInstructions>>>) (() => this.InstructionList));
    }
  }

  public List<string> StartInstructionList
  {
    get => this._StartInstructionList;
    internal set => this._StartInstructionList = value;
  }

  public List<string> EndInstructionList
  {
    get => this._EndInstructionList;
    internal set => this._EndInstructionList = value;
  }

  public bool ShowInstructionListGrid
  {
    get => this._showInstructionListGrid;
    internal set => this._showInstructionListGrid = value;
  }

  public override void ViewAppeared()
  {
    base.ViewAppeared();
    if (this._ActiveView)
      return;
    this._ActiveView = true;
    this._appliance.CheckConnectivity(new connectivityUpdateCallback(this.updateStatus));
    this.ActivateFlasher(this.FlashingItem.FlashingIndex);
  }

  private void ActivateFlasher(string index)
  {
    Task.Factory.StartNew((Action) (() =>
    {
      try
      {
        if (!this._sshWrapper.ActivateFlasher(index, (Stream) null, (iService5.Ssh.models.ProgressCallback) ((progress, step, total) =>
        {
          if (this.IsCodingNext)
            this.GoToCoding(progress);
          FlashingItem valuesFeedback = (FlashingItem) null;
          try
          {
            this._loggingService.getLogger().LogAppDebug(LoggingContext.BACKEND, "Response received for activate flasher is : " + progress, memberName: nameof (ActivateFlasher), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceNonSMMFlashViewModel.cs", sourceLineNumber: 1047);
            valuesFeedback = JsonConvert.DeserializeObject<FlashingItem>(progress);
          }
          catch (JsonSerializationException ex)
          {
            this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Ignoring invalid json response.", memberName: nameof (ActivateFlasher), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceNonSMMFlashViewModel.cs", sourceLineNumber: 1052);
          }
          catch (Exception ex)
          {
            this._loggingService.getLogger().LogAppError(LoggingContext.APPLIANCE, ex.Message, memberName: nameof (ActivateFlasher), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceNonSMMFlashViewModel.cs", sourceLineNumber: 1056);
          }
          if (valuesFeedback == null)
            return;
          if (valuesFeedback.FlashProgress != null && valuesFeedback.FlashProgress.CtrlMessages != null && valuesFeedback.FlashProgress.CtrlMessages.Any<CtrlMessage>((Func<CtrlMessage, bool>) (x => x.NextFunction == "FLASHING")) && valuesFeedback.FlashProgress.CtrlMessages.Any<CtrlMessage>((Func<CtrlMessage, bool>) (x => x.Mode == "AUTO")))
          {
            if (!this.firstAutoBatchPass)
            {
              this.AfterFlashing(true);
              this.ProgrammingLog = string.Empty;
            }
            this.IsBatchAuto = true;
            this._loggingService.getLogger().LogAppInformation(LoggingContext.BACKEND, "Auto batch detected.", memberName: nameof (ActivateFlasher), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceNonSMMFlashViewModel.cs", sourceLineNumber: 1070);
            this.IsFlashBtnVisible = false;
            this.InitializeVariables();
            this.firstAutoBatchPass = false;
            this.IsBusy = false;
          }
          else if (valuesFeedback.FlashProgress != null && valuesFeedback.FlashProgress.CtrlMessages != null && valuesFeedback.FlashProgress.CtrlMessages.Any<CtrlMessage>((Func<CtrlMessage, bool>) (x => x.NextFunction == "CODING")))
          {
            this.IsCodingNext = true;
            this._loggingService.getLogger().LogAppInformation(LoggingContext.BACKEND, "Coding next detected.", memberName: nameof (ActivateFlasher), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceNonSMMFlashViewModel.cs", sourceLineNumber: 1080);
          }
          else if (this.IsBatchAuto)
          {
            if (!string.IsNullOrEmpty(valuesFeedback.FlashingIndex))
            {
              this.WriteInstructions(valuesFeedback);
              this.AddHistoryBeforeFlashing();
            }
            else if (valuesFeedback.FlashProgress != null)
            {
              if (!string.IsNullOrEmpty(valuesFeedback.FlashProgress.ProgressPercent))
                this.EditProgress(valuesFeedback.FlashProgress);
              this.AddString(valuesFeedback.FlashProgress.InfoMessages);
            }
          }
          else
          {
            this.WriteInstructions(valuesFeedback);
            this.IsBusy = false;
          }
        })).Success)
          this._alertService.ShowMessageAlertWithMessage(AppResource.APPLIANCE_PROGRAMMING_NOT_SUCCESSFUL, AppResource.ERROR_TITLE);
        if (this.IsBatchAuto)
          this.AfterFlashing();
        this.IsBusy = false;
      }
      catch (Exception ex)
      {
        this._loggingService.getLogger().LogAppError(LoggingContext.APPLIANCE, ex.Message, ex, nameof (ActivateFlasher), "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/ApplianceNonSMMFlashViewModel.cs", 1117);
      }
    }));
  }

  internal void updateStatus()
  {
    this.WifiStatus = this._appliance.StatusOfBridgeConnection;
    this.ConnectedColor = this._appliance.ConnectedColorBridge;
    this.IsFlashBtnEnabled = this._appliance.boolStatusOfBridgeConnection && !this.IsProgrammingInProgress;
  }

  public string WifiStatus
  {
    get => this._WifiStatus;
    private set
    {
      this._WifiStatus = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.WifiStatus));
    }
  }

  public bool IsFlashBtnEnabled
  {
    get => this._IsFlashBtnEnabled;
    private set
    {
      this._IsFlashBtnEnabled = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.IsFlashBtnEnabled));
    }
  }

  public bool IsFinishBtnEnabled
  {
    get => this.isFinishBtnEnabled;
    internal set
    {
      this.isFinishBtnEnabled = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.IsFinishBtnEnabled));
    }
  }

  public bool IsProgrammingInProgress
  {
    get => this.isProgrammingInProgress;
    internal set
    {
      this.isProgrammingInProgress = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.IsProgrammingInProgress));
    }
  }

  public bool ReturnAfterReboot { get; internal set; }

  public string InstructionSteps
  {
    get => this._InstructionSteps;
    internal set
    {
      this._InstructionSteps = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.InstructionSteps));
    }
  }

  public string InstructionIndex
  {
    get => this._InstructionIndex;
    internal set
    {
      this._InstructionIndex = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.InstructionIndex));
    }
  }

  public string FinishedProgrammingMsg
  {
    get => this._FinishedProgrammingMsg;
    internal set
    {
      this._FinishedProgrammingMsg = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.FinishedProgrammingMsg));
    }
  }

  public string FinishedProgrammingMsgColour
  {
    get => this._FinishedProgrammingMsgColour;
    internal set
    {
      this._FinishedProgrammingMsgColour = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.FinishedProgrammingMsgColour));
    }
  }

  public bool ShowFinishedProgrammingMsg
  {
    get => this._ShowFinishedProgrammingMsg;
    internal set
    {
      this._ShowFinishedProgrammingMsg = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.ShowFinishedProgrammingMsg));
    }
  }

  public string ProgressBarColor
  {
    get => this._ProgressBarColor;
    internal set
    {
      this._ProgressBarColor = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.ProgressBarColor));
    }
  }

  public bool IsFinishBtnVisible
  {
    get => this._IsFinishBtnVisible;
    internal set
    {
      this._IsFinishBtnVisible = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.IsFinishBtnVisible));
    }
  }

  public bool IsFlashBtnVisible
  {
    get => this._IsFlashBtnVisible;
    internal set
    {
      this._IsFlashBtnVisible = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.IsFlashBtnVisible));
    }
  }
}
