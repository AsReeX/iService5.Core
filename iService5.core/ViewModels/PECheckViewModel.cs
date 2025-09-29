// Decompiled with JetBrains decompiler
// Type: iService5.Core.ViewModels.PECheckViewModel
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Appliance;
using iService5.Core.Services.Data;
using iService5.Core.Services.Helpers;
using iService5.Core.Services.Platform;
using iService5.Core.Services.User;
using iService5.Core.Services.User.ActivityLogging;
using iService5.Ssh.models;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using Newtonsoft.Json;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
//using Xamarin.Forms;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

#nullable disable
namespace iService5.Core.ViewModels;

public class PECheckViewModel : MvxViewModel<bool>
{
  private readonly IMvxNavigationService _navigationService;
  private readonly IUserSession _userSession;
  private readonly IAppliance _appliance;
  private readonly IMetadataService _metadataService;
  private readonly IPlatformSpecificServiceLocator _locator;
  private readonly ILoggingService _loggingService;
  private readonly IPlatformSpecificService _platform;
  private readonly IAlertService _alert;
  private readonly Is5SshWrapper _sshWrapper;
  private readonly IAlertService _alertService;
  internal Task _DisconnectionTask = (Task) null;
  internal CancellationTokenSource _tokenSource = (CancellationTokenSource) null;
  private CancellationTokenSource tokenSource;
  private CancellationToken ct;
  internal bool lostConnectivityFlag = false;
  internal bool peCheckCalled = false;
  internal bool ViewActive = false;
  internal bool peCheckDoneAtleastOnce = false;
  internal bool processingCompletedStatus = false;
  private const string TOUCH_OK = "TOUCH_OK";
  private const string TOUCH_FAIL = "TOUCH_FAIL";
  private const string CONTINUE = "CONTINUE";
  internal string PeSuccessMessageShortTextNum = "30288";
  internal string PeContinueMessageShortTextNum = "30103";
  internal string touchOkFailSvgFilename = "PE_TEST_DEVICE_TOUCH_OK_FAIL.svg";
  internal string continueSvgFilename = "PE_TEST_DEVICE_CONTINUE.svg";
  internal string SvgFilename;
  internal string lang;
  private string _WifiBridgeStatus = "• " + AppResource.CONNECTED_TEXT;
  private CancellationTokenSource peCheckTokenSource;
  private CancellationToken peCheckToken;
  private static System.Timers.Timer transitionMessageTimer;
  private Progress<Values> progress;
  private string _ConnectedColor = "Green";
  private SKColor _ColorValue = SKColor.Empty;
  private bool _IsBusy = true;
  private bool _displayPartialBridgePage = false;
  private string _RepairEnumber = nameof (RepairEnumber);
  private List<InstructionMessage> _instructionMessages = new List<InstructionMessage>();
  private bool _displayTouchOkBtn = false;
  private bool _displayTouchFailBtn = false;
  private bool _displayContinueBtn = false;
  private bool _displayChoiceBtn = false;
  private bool _DisplayPeTransitionMessage;
  private bool displayActivityIndicator = true;
  private bool _areButtonsEnabled = true;
  private string _PeTransitionMessage;

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

  public SKColor ColorValue
  {
    get => this._ColorValue;
    internal set
    {
      this._ColorValue = value;
      this.RaisePropertyChanged<SKColor>((Expression<Func<SKColor>>) (() => this.ColorValue));
    }
  }

  public bool IsBusy
  {
    get => this._IsBusy;
    set
    {
      this._IsBusy = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.IsBusy));
    }
  }

  public bool displayPartialBridgePage
  {
    get => this._displayPartialBridgePage;
    set
    {
      this._displayPartialBridgePage = value;
      if (this._displayPartialBridgePage)
        this.RepairEnumber = AppResource.BRIDGE_HEADER;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.displayPartialBridgePage));
    }
  }

  public string RepairEnumber
  {
    get => this._RepairEnumber;
    internal set
    {
      this._RepairEnumber = value;
      if (this._RepairEnumber == null)
        this._RepairEnumber = AppResource.BRIDGE_HEADER;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.RepairEnumber));
    }
  }

  public List<InstructionMessage> InstructionMessages
  {
    get => this._instructionMessages;
    set
    {
      this._instructionMessages = value;
      this.RaisePropertyChanged<List<InstructionMessage>>((Expression<Func<List<InstructionMessage>>>) (() => this.InstructionMessages));
    }
  }

  public bool DisplayTouchOkBtn
  {
    get => this._displayTouchOkBtn;
    set
    {
      this._displayTouchOkBtn = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.DisplayTouchOkBtn));
    }
  }

  public bool DisplayTouchFailBtn
  {
    get => this._displayTouchFailBtn;
    set
    {
      this._displayTouchFailBtn = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.DisplayTouchFailBtn));
    }
  }

  public bool DisplayContinueBtn
  {
    get => this._displayContinueBtn;
    set
    {
      this._displayContinueBtn = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.DisplayContinueBtn));
    }
  }

  public bool DisplayChoiceBtn
  {
    get => this._displayChoiceBtn;
    set
    {
      this._displayChoiceBtn = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.DisplayChoiceBtn));
    }
  }

  public bool DisplayPeTransitionMessage
  {
    get => this._DisplayPeTransitionMessage;
    set
    {
      this._DisplayPeTransitionMessage = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.DisplayPeTransitionMessage));
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

  public string PeTransitionMessage
  {
    get => this._PeTransitionMessage;
    internal set
    {
      this._PeTransitionMessage = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.PeTransitionMessage));
    }
  }

  public ICommand GoBackCommand { internal set; get; }

  public ICommand TouchOkButtonCommand { internal set; get; }

  public ICommand TouchFailButtonCommand { internal set; get; }

  public ICommand TouchContinueButtonCommand { internal set; get; }

  public PECheckViewModel(
    IMvxNavigationService navigationService,
    IUserSession userSession,
    IAppliance appliance,
    IMetadataService metadataService,
    IPlatformSpecificServiceLocator locator,
    ILoggingService loggingService,
    IAlertService alertService,
    Is5SshWrapper sshWrapper)
  {
    this._navigationService = navigationService;
    this._userSession = userSession;
    this._appliance = appliance;
    this._metadataService = metadataService;
    this._locator = locator;
    this._loggingService = loggingService;
    this._platform = this._locator.GetPlatformSpecificService();
    this._alertService = alertService;
    this._sshWrapper = sshWrapper;
    this._sshWrapper.IPAddress = this._locator.GetPlatformSpecificService().GetIp();
    this.tokenSource = new CancellationTokenSource();
    this.ct = this.tokenSource.Token;
    this.RepairEnumber = this._userSession.getEnumberSession();
    this.GoBackCommand = (ICommand) new Command((Action) (() =>
    {
      if (this.lostConnectivityFlag)
        return;
      this.GoBack();
    }));
    this.TouchOkButtonCommand = (ICommand) new Command(new Action(this.OnTouchOkButtonClicked));
    this.TouchFailButtonCommand = (ICommand) new Command(new Action(this.OnTouchFailButtonClicked));
    this.TouchContinueButtonCommand = (ICommand) new Command(new Action(this.OnTouchContinueButtonClicked));
    this.lang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToLowerInvariant();
    this.peCheckTokenSource = new CancellationTokenSource();
    this.peCheckToken = this.peCheckTokenSource.Token;
  }

  public override void ViewAppeared()
  {
    base.ViewAppeared();
    this._appliance.CheckConnectivity(new connectivityUpdateCallback(this.ConnectivityCheck));
    this.ViewActive = true;
    this.PerformGetvalues();
    this.PerformPECheck("");
  }

  public void PerformGetvalues()
  {
    this.progress = new Progress<Values>();
    this.GetValues((IProgress<Values>) this.progress);
    this.progress.ProgressChanged += (EventHandler<Values>) ((sender, values) => this.ProcessValues(values));
  }

  public override void ViewDisappearing()
  {
    base.ViewDisappearing();
    this.ViewActive = false;
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
        if (this._tokenSource.Token.IsCancellationRequested || this.lostConnectivityFlag)
          return;
        this.lostConnectivityFlag = true;
        this.tokenSource.Cancel();
        this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
      }), this._tokenSource.Token);
    }
  }

  public override void Prepare(bool displayPartialBridgePage)
  {
    this.displayPartialBridgePage = displayPartialBridgePage;
  }

  internal void GoBack()
  {
    this.ViewActive = false;
    this.peCheckTokenSource.Cancel();
    this.peCheckTokenSource.Dispose();
    this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
  }

  public void OnBackButtonPressed() => this.GoBack();

  public void PerformPECheck(string powerSocketTestArgument)
  {
    try
    {
      SshResponse sshResponse = this._sshWrapper.TestPowerSocket(powerSocketTestArgument);
      if (sshResponse.Success)
      {
        this.peCheckCalled = true;
      }
      else
      {
        this._loggingService.getLogger().LogAppError(LoggingContext.APPLIANCE, "Error - PE Check Call Failed" + sshResponse.ErrorMessage, memberName: nameof (PerformPECheck), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/PECheckViewModel.cs", sourceLineNumber: 421);
        this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
      }
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppError(LoggingContext.APPLIANCE, "Exception occured while performing pe check: " + ex.Message, memberName: nameof (PerformPECheck), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/PECheckViewModel.cs", sourceLineNumber: 427);
      this.GoBack();
    }
  }

  public void ProcessValues(Values values)
  {
    if (this.processingCompletedStatus)
      return;
    if (values.additional_options != null && values.additional_options.Count > 0)
      this.CheckAdditionalOptions(values.additional_options);
    else
      this.GoToMeasureView();
  }

  private void CheckAdditionalOptions(List<AdditionalOptions> additionalOptions)
  {
    if (additionalOptions != null && additionalOptions.Count > 0)
    {
      AdditionalOptions additionalOption = additionalOptions[0];
      List<string> messages = additionalOption.messages;
      List<string> choices = additionalOption.choices;
      List<InstructionMessage> translatedMessages = new List<InstructionMessage>();
      string status1 = additionalOption.status;
      PECheckStatus peCheckStatus = PECheckStatus.COMPLETED;
      string b1 = peCheckStatus.ToString();
      if (string.Equals(status1, b1))
      {
        if (this.peCheckDoneAtleastOnce)
        {
          this.processingCompletedStatus = true;
          this.PeTransitionMessage = this._metadataService.getShortText(this.PeSuccessMessageShortTextNum, this.lang);
          this.DisplayPeTransitionMessage = true;
          PECheckViewModel.transitionMessageTimer = new System.Timers.Timer(3000.0);
          PECheckViewModel.transitionMessageTimer.Elapsed += new ElapsedEventHandler(this.OnTimedEvent);
          PECheckViewModel.transitionMessageTimer.AutoReset = true;
          PECheckViewModel.transitionMessageTimer.Enabled = true;
        }
        else
          this.GoToMeasureView();
      }
      else
      {
        string status2 = additionalOption.status;
        peCheckStatus = PECheckStatus.TESTING;
        string b2 = peCheckStatus.ToString();
        if (string.Equals(status2, b2))
        {
          this.peCheckDoneAtleastOnce = true;
          for (int index = 0; index < messages.Count; ++index)
          {
            int num = index + 1;
            InstructionMessage instructionMessage = new InstructionMessage(num.ToString(), this._metadataService.getShortText(messages[index], this.lang), num == 1);
            translatedMessages.Add(instructionMessage);
          }
          Device.BeginInvokeOnMainThread((Action) (() =>
          {
            this.DisplayActivityIndicator = false;
            this.InstructionMessages = translatedMessages;
            this.EnableChoiceButtons(choices);
          }));
        }
        else
        {
          string status3 = additionalOption.status;
          peCheckStatus = PECheckStatus.FAILED;
          string b3 = peCheckStatus.ToString();
          if (string.Equals(status3, b3))
          {
            this.peCheckDoneAtleastOnce = true;
            this.ViewActive = false;
            List<InstructionMessage> instructionMessageList = new List<InstructionMessage>();
            string failedMessages = "";
            for (int index = 0; index < messages.Count; ++index)
            {
              InstructionMessage instructionMessage = new InstructionMessage((index + 1).ToString(), this._metadataService.getShortText(messages[index], this.lang), false);
              instructionMessageList.Add(instructionMessage);
              failedMessages = $"{failedMessages}{instructionMessageList[index].message}\n";
            }
            this.DisplayPeTransitionMessage = false;
            Device.BeginInvokeOnMainThread((Action) (() => this._alertService.ShowMessageAlertWithMessageFromService(failedMessages.ToString(), AppResource.WARNING_TEXT, (Action) (() => this.GoBack()))));
          }
          else
            this._loggingService.getLogger().LogAppError(LoggingContext.APPLIANCE, "Received a non defined status for pe check", memberName: nameof (CheckAdditionalOptions), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/PECheckViewModel.cs", sourceLineNumber: 512 /*0x0200*/);
        }
      }
    }
    else
      this.GoToMeasureView();
  }

  private void OnTimedEvent(object source, ElapsedEventArgs e) => this.GoToMeasureView();

  private void GoToMeasureView()
  {
    this.ViewActive = false;
    if (!this.AreButtonsEnabled)
      return;
    this.AreButtonsEnabled = false;
    this.peCheckTokenSource.Cancel();
    this.peCheckTokenSource.Dispose();
    if (this.displayPartialBridgePage)
      this._navigationService.Navigate<MeasureViewModel, bool>(true, (IMvxBundle) null, new CancellationToken());
    else
      this._navigationService.Navigate<MeasureViewModel>((IMvxBundle) null, new CancellationToken());
    this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
  }

  public void EnableChoiceButtons(List<string> choices)
  {
    if (choices.Contains("TOUCH_OK") || choices.Contains("TOUCH_FAIL"))
    {
      this.DisplayTouchOkBtn = true;
      this.DisplayTouchFailBtn = true;
      this.DisplayContinueBtn = false;
      this.SvgFilename = this.touchOkFailSvgFilename;
    }
    else if (choices.Contains("CONTINUE"))
    {
      this.DisplayTouchOkBtn = false;
      this.DisplayTouchFailBtn = false;
      this.DisplayContinueBtn = true;
      this.SvgFilename = this.continueSvgFilename;
    }
    else
    {
      this.DisplayTouchOkBtn = false;
      this.DisplayTouchFailBtn = false;
      this.DisplayContinueBtn = false;
      this.SvgFilename = "";
    }
    this.DisplayChoiceBtn = this.DisplayTouchOkBtn || this.DisplayContinueBtn || this.DisplayContinueBtn;
    this.DisplayPeTransitionMessage = false;
  }

  private void OnTouchOkButtonClicked() => this.PerformPECheck("TOUCH_OK");

  private void OnTouchFailButtonClicked() => this.PerformPECheck("TOUCH_FAIL");

  private void OnTouchContinueButtonClicked() => this.PerformPECheck("CONTINUE");

  public void OnCanvasViewPaintInstructions(Assembly assembly, SKPaintSurfaceEventArgs args)
  {
    SKImageInfo info = args.Info;
    SKCanvas canvas = args.Surface.Canvas;
    canvas.Clear();
    if (string.IsNullOrEmpty(this.SvgFilename))
      return;
    try
    {
      Stream manifestResourceStream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Resources.{this.SvgFilename}");
      SkiaSharp.Extended.Svg.SKSvg skSvg = new SkiaSharp.Extended.Svg.SKSvg();
      skSvg.Load(manifestResourceStream);
      canvas.Translate((float) info.Width / 2f, (float) info.Height / 2f);
      SKRect viewBox = skSvg.ViewBox;
      float s = Math.Min((float) info.Width / viewBox.Width, (float) info.Height / viewBox.Height);
      canvas.Scale(s);
      canvas.Translate(-viewBox.MidX, -viewBox.MidY);
      canvas.DrawPicture(skSvg.Picture);
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppError(LoggingContext.METADATA, "Failed to use svg file" + this.SvgFilename, ex, nameof (OnCanvasViewPaintInstructions), "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/PECheckViewModel.cs", 612);
    }
  }

  public async Task GetValues(IProgress<Values> progress)
  {
    await Task.Run((Action) (() =>
    {
      this._loggingService.getLogger().LogAppDebug(LoggingContext.LOCAL, "Get values called", memberName: nameof (GetValues), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/PECheckViewModel.cs", sourceLineNumber: 619);
      this._sshWrapper.GetValues((Stream) null, (iService5.Ssh.models.ProgressCallback) ((progressValue, step, total) =>
      {
        this.peCheckToken.ThrowIfCancellationRequested();
        try
        {
          this._loggingService.getLogger().LogAppDebug(LoggingContext.BACKEND, progressValue, memberName: nameof (GetValues), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/PECheckViewModel.cs", sourceLineNumber: 626);
          Values valuesFeedback = JsonConvert.DeserializeObject<Values>(progressValue);
          Device.BeginInvokeOnMainThread((Action) (() => progress.Report(valuesFeedback)));
        }
        catch (Exception ex)
        {
          this._loggingService.getLogger().LogAppError(LoggingContext.APPLIANCE, "Error during JSON response parsing: " + ex.Message, memberName: nameof (GetValues), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/PECheckViewModel.cs", sourceLineNumber: 635);
        }
      }));
    }), this.peCheckToken);
  }
}
