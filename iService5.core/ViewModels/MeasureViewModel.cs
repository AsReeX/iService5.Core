// Decompiled with JetBrains decompiler
// Type: iService5.Core.ViewModels.MeasureViewModel
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
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
//using Xamarin.Essentials;
//using Xamarin.Forms;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

#nullable disable
namespace iService5.Core.ViewModels;

public class MeasureViewModel : MvxViewModel<bool>
{
  private readonly IMvxNavigationService _navigationService;
  private readonly IUserSession _userSession;
  private readonly IAppliance _appliance;
  private readonly IMetadataService _metadata;
  private readonly IPlatformSpecificServiceLocator _locator;
  private readonly ILoggingService _loggingService;
  private readonly IPlatformSpecificService _platform;
  private readonly IAlertService _alert;
  private readonly Is5SshWrapper _sshWrapper;
  public const int NumberOfButtons = 17;
  public const double UnitAngle = 21.176470588235293;
  private Dictionary<int, string> measurement = new Dictionary<int, string>();
  private List<string> batchlist;
  private List<string> batchlistMetron;
  private SortedDictionary<int, string> batchDict;
  private Dictionary<int, string> batchDictSorted;
  private readonly Dictionary<CATMeasurementTitle, bool?> batchStepsResults;
  private static System.Timers.Timer catTimeOutTimer;
  private List<MeasureViewModel.MeasurementResultsP> measurementsResultsList;
  private readonly Dictionary<KeyValuePair<string, bool?>, string> batchStepsResultInfo = new Dictionary<KeyValuePair<string, bool?>, string>();
  public MvxObservableCollection<MeasureViewModel.AssesmentResult> CustomAssesmentHistoryData = new MvxObservableCollection<MeasureViewModel.AssesmentResult>();
  internal List<iService5.Core.Services.Data.CATMeasurement> catMeasurements = new List<iService5.Core.Services.Data.CATMeasurement>();
  internal List<MeasureViewModel.AssesmentResult> HistoryAssessmentResultList = new List<MeasureViewModel.AssesmentResult>();
  internal bool lostConnectivityFlag = false;
  internal bool measurementButtonTapped = false;
  internal bool backButtonTapped = false;
  internal string rpeOffset;
  internal CATMeasurementTitle selectedMeasurementTitle = CATMeasurementTitle.IDLE;
  internal bool forcedClampAmpCalibration = false;
  internal bool PolarityChangeTapped = false;
  private CancellationTokenSource tokenSource;
  private CancellationToken ct;
  internal Task _DisconnectionTask = (Task) null;
  internal CancellationTokenSource _tokenSource = (CancellationTokenSource) null;
  internal string SvgFilename;
  internal string DiagramUnit;
  private string _NavText = AppResource.APPLIANCE_FLASH_BACK_TEXT;
  private string _RepairEnumber = nameof (RepairEnumber);
  private string _WifiBridgeStatus = "• " + AppResource.CONNECTED_TEXT;
  private string _ConnectedColor = "Green";
  private string _MeasurementText = "test";
  private double _doubleVal;
  private double _KnobRotation;
  private bool _BackBtnStatus = true;
  private bool _BackBtnVis = true;
  private bool _CalLeftButtonVisible = true;
  private bool _CalLeftTickButtonVisible;
  private bool _CalLeftXButtonVisible;
  private bool _CalRightButtonVisible = true;
  private bool _CalRightTickButtonVisible;
  private bool _CalRightXButtonVisible;
  private bool _BatchButtonVisible;
  private bool _CompButtonVisible;
  private bool _U1ButtonVisible;
  private bool _TestButtonVisible;
  private bool _U2ButtonVisible;
  private bool _StandByButtonVisible;
  private bool _PButtonVisible;
  private bool _I1ButtonVisible;
  private bool _I2ButtonVisible;
  private bool _IDiffCalButtonVisible;
  private bool _IDiffButtonVisible;
  private bool _ITouchButtonVisible;
  private bool _IEquivButtonVisible;
  private bool _Rins2ButtonVisible;
  private bool _RinsButtonVisible;
  private bool _RpeButtonVisible;
  private bool _RpeCalButtonVisible;
  private string _MeasurementTitle;
  private CATMeasurementTitle _CATMeasurement;
  private string _MeasurementName;
  private bool _InstructionsVisible;
  private bool _DiagramVisible;
  private bool _BatchSummaryVisible;
  private bool _CustomSummaryVisible;
  private bool _TickButtonVisible;
  private bool _FailButtonVisible;
  private bool _VoltageButtonVisible = false;
  private bool _ZapButtonVisible = false;
  private bool _PolarityButtonVisible = false;
  private PlotModel _Chart;
  private bool _StartRecordingButtonDisabled;
  private bool _BlackPanelVisible;
  private bool _TestPanelVisible = false;
  private bool _SaveButtonDisabled = true;
  private string _DiagramTitle;
  private string _changeButtonSrc = "aUTO.png";
  private SKColor _ColorValue = SKColor.Empty;
  private bool _canDisplayBatchAssesment = false;
  private List<MeasureViewModel.AssesmentResult> _CustomAssesmentResultList = new List<MeasureViewModel.AssesmentResult>();
  private double testGridLabelFontSize;
  private string _TestItouchValidationImageSource = "";
  private string _TestIDiffValidationImageSource = "";
  private bool BackBtnClicked = false;
  internal List<DataPoint> Entries = new List<DataPoint>();
  internal int Counter = 1;
  internal bool Recording;
  internal DateTime StopRecording;
  internal bool BlackPanelUsed;
  private string _testItouch_result = "";
  private string _testItouch_peak = "";
  private string _testItouch_limit = "";
  private string _testItouch_ok = "";
  private string _testIdiff_result = "";
  private string _testIdiff_peak = "";
  private string _testIdiff_limit = "";
  private string _testIdiff_ok = "";
  private string _testU_result = "";
  private string _testU_peak = "";
  private string _testP_result = "";
  private string _testP_peak = "";
  private string _testI_result = "";
  private string _testI_peak = "";
  private int batchindex = 0;
  private int batchMaxNumber;
  private string _PolarityButtonSource = "MeasurePolarityGreen.png";
  private string _MesaurementTitle = AppResource.MEASURE_TITLE;
  private string _ValidationTitle = AppResource.VALIDATION_TITLE;
  private string _ValueTitle = AppResource.VALUE_TITLE;
  private bool _IsBusy = false;
  private string recalStr = "Please recalibrate!";

  public MvxObservableCollection<MeasureViewModel.AssesmentResult> AssesmentResultList { get; internal set; } = new MvxObservableCollection<MeasureViewModel.AssesmentResult>();

  public override void Prepare(bool toSettingsTab)
  {
    this.noenumber = true;
    this.RepairEnumber = "iService Bridge";
  }

  public MeasureViewModel(
    IMvxNavigationService navigationService,
    IUserSession userSession,
    IAppliance appliance,
    IMetadataService metadata,
    IPlatformSpecificServiceLocator locator,
    ILoggingService loggingService,
    IAlertService alert,
    Is5SshWrapper sshWrapper)
  {
    this._navigationService = navigationService;
    this._userSession = userSession;
    this._appliance = appliance;
    this._metadata = metadata;
    this._locator = locator;
    this._loggingService = loggingService;
    this._platform = this._locator.GetPlatformSpecificService();
    this._alert = alert;
    this._sshWrapper = sshWrapper;
    this._sshWrapper.IPAddress = this._locator.GetPlatformSpecificService().GetIp();
    Dictionary<string, int> fractionalPartAccuracyForUnit1 = new Dictionary<string, int>()
    {
      {
        "mOhm",
        0
      },
      {
        "Ohm",
        2
      },
      {
        "kOhm",
        3
      },
      {
        "MOhm",
        3
      }
    };
    Dictionary<string, int> fractionalPartAccuracyForUnit2 = new Dictionary<string, int>()
    {
      {
        "mA",
        3
      },
      {
        "A",
        2
      }
    };
    Dictionary<string, int> fractionalPartAccuracyForUnit3 = new Dictionary<string, int>()
    {
      {
        "W",
        1
      },
      {
        "kW",
        3
      }
    };
    Dictionary<string, int> fractionalPartAccuracyForUnit4 = new Dictionary<string, int>()
    {
      {
        "V",
        1
      }
    };
    this.catMeasurements.Add(new iService5.Core.Services.Data.CATMeasurement(CATMeasurementTitle.ST_OFF, "OFF", 0, "", 0.0, 0.0, false, false, (Dictionary<string, int>) null, "S1NInfo"));
    this.catMeasurements.Add(new iService5.Core.Services.Data.CATMeasurement(CATMeasurementTitle.ST_COMP, "COMP", 1, "MOhm", 0.0, 0.0, false, false, fractionalPartAccuracyForUnit1, "Info_Comp"));
    this.catMeasurements.Add(new iService5.Core.Services.Data.CATMeasurement(CATMeasurementTitle.ST_Uacdc, "AC DC PROBE", 2, "V", 0.0, 0.0, false, false, fractionalPartAccuracyForUnit4, "Info_Uacdc"));
    this.catMeasurements.Add(new iService5.Core.Services.Data.CATMeasurement(CATMeasurementTitle.ST_Test, "TEST", 3, "", 0.0, 0.0, false, false, (Dictionary<string, int>) null, "Info_Test"));
    this.catMeasurements.Add(new iService5.Core.Services.Data.CATMeasurement(CATMeasurementTitle.ST_U, "U SOCKET", 4, "V", 0.0, 0.0, false, false, fractionalPartAccuracyForUnit4, "Info_U", "0 V"));
    this.catMeasurements.Add(new iService5.Core.Services.Data.CATMeasurement(CATMeasurementTitle.ST_StandBy, "STAND BY", 5, "W", 0.0, 0.0, false, false, fractionalPartAccuracyForUnit3, "Info_StandBy"));
    this.catMeasurements.Add(new iService5.Core.Services.Data.CATMeasurement(CATMeasurementTitle.ST_P, "P SOCKET", 6, "W", 0.0, 0.0, false, false, fractionalPartAccuracyForUnit3, "Info_P", "0 W"));
    this.catMeasurements.Add(new iService5.Core.Services.Data.CATMeasurement(CATMeasurementTitle.ST_I_CLAMP, "I CLAMP", 7, "A", 0.0, 0.0, false, false, fractionalPartAccuracyForUnit2, "Info_I_CLAMP"));
    this.catMeasurements.Add(new iService5.Core.Services.Data.CATMeasurement(CATMeasurementTitle.ST_I_SOCKET, "I SOCKET", 8, "A", 0.0, 0.0, false, false, fractionalPartAccuracyForUnit2, "Info_I_Socket", "0 A"));
    this.catMeasurements.Add(new iService5.Core.Services.Data.CATMeasurement(CATMeasurementTitle.ST_IDIFF_CLAMP_CAL, "IDIFF CLAMP CAL", 9, "mA", 0.8, 0.9, true, true, fractionalPartAccuracyForUnit2, "Info_I_DIFF_CLAMP_CAL"));
    this.catMeasurements.Add(new iService5.Core.Services.Data.CATMeasurement(CATMeasurementTitle.ST_IDIFF_CLAMP, "IDIFF CLAMP", 10, "mA", 0.0, 3.5, false, true, fractionalPartAccuracyForUnit2, "Info_I_DIFF_CLAMP"));
    this.catMeasurements.Add(new iService5.Core.Services.Data.CATMeasurement(CATMeasurementTitle.ST_IDIFF_SOCKET, "IDIFF SOCKET", 11, "mA", 0.0, 3.5, false, true, fractionalPartAccuracyForUnit2, "Info_I_Diff", "0 mA"));
    this.catMeasurements.Add(new iService5.Core.Services.Data.CATMeasurement(CATMeasurementTitle.ST_IA, "I Touch", 12, "mA", 0.0, 0.5, false, true, fractionalPartAccuracyForUnit2, "Info_IA", "0 mA"));
    this.catMeasurements.Add(new iService5.Core.Services.Data.CATMeasurement(CATMeasurementTitle.ST_IEA, "I EQUIV", 13, "mA", 0.0, 3.5, false, true, fractionalPartAccuracyForUnit2, "Info_IEA_Socket"));
    this.catMeasurements.Add(new iService5.Core.Services.Data.CATMeasurement(CATMeasurementTitle.ST_RISO_PROBE, "RINS PROBE", 14, "MOhm", 2.0, 0.0, true, false, fractionalPartAccuracyForUnit1, "Info_Riso_Probe"));
    this.catMeasurements.Add(new iService5.Core.Services.Data.CATMeasurement(CATMeasurementTitle.ST_RISO_SOCKET, "RINS SOCKET", 15, "MOhm", 1.0, 0.0, true, false, fractionalPartAccuracyForUnit1, "Info_Riso_Socket"));
    this.catMeasurements.Add(new iService5.Core.Services.Data.CATMeasurement(CATMeasurementTitle.ST_SL, "RPE SOCKET", 16 /*0x10*/, "Ohm", 0.0, 1.0, false, true, fractionalPartAccuracyForUnit1, "Info_SL"));
    this.catMeasurements.Add(new iService5.Core.Services.Data.CATMeasurement(CATMeasurementTitle.ST_SL1, "RPE PROBE", 17, "Ohm", 0.0, 0.3, false, true, fractionalPartAccuracyForUnit1, "Info_RSL"));
    this.catMeasurements.Add(new iService5.Core.Services.Data.CATMeasurement(CATMeasurementTitle.ST_SL1_CAL, "RPE PROBE CAL", 18, "Ohm", 0.0, 1.0, false, true, fractionalPartAccuracyForUnit1, "Info_RSL_cal"));
    this.valueRetreivalTaskStarted = false;
    this.RepairEnumber = this._userSession.getEnumberSession();
    this.InstructionsVisible = false;
    this.DiagramVisible = false;
    this.KnobRotation = 0.0;
    this.MeasurementText = "-";
    this.CalLeftTickButtonVisible = CoreApp.settings.GetItem("rpeCalSucceeded").Value == "true";
    this.CalLeftXButtonVisible = CoreApp.settings.GetItem("rpeCalFailed").Value == "true";
    this.CalRightTickButtonVisible = CoreApp.settings.GetItem("clampCalSucceeded").Value == "true";
    this.CalRightXButtonVisible = CoreApp.settings.GetItem("clampCalFailed").Value == "true";
    this.GoBackCommand = (ICommand) new Command((Action) (() =>
    {
      this.backButtonTapped = true;
      if (this.lostConnectivityFlag)
        return;
      this.GoBack();
    }));
    this.tokenSource = new CancellationTokenSource();
    this.ct = this.tokenSource.Token;
    this.OffButtonTapped = (ICommand) new Command((Action) (() => Device.BeginInvokeOnMainThread((Action) (() =>
    {
      if (this.lostConnectivityFlag)
        return;
      if (this.modeBatchAssesment && this.batchStepsResults.Count != 0)
        this.BatchSummaryVisible = true;
      else if (this.modeCustomAssesment && this.measurementsResultsList.Count != 0)
      {
        this.CustomSummaryVisible = true;
      }
      else
      {
        this.CustomSummaryVisible = false;
        this.BatchSummaryVisible = false;
      }
      this.HistoryLog();
      this.ButtonTapped(CATMeasurementTitle.ST_OFF);
    }))));
    this.CompButtonTapped = (ICommand) new Command((Action) (() =>
    {
      if (this.lostConnectivityFlag)
        return;
      this.ButtonTapped(CATMeasurementTitle.ST_COMP);
    }));
    this.U1ButtonTapped = (ICommand) new Command((Action) (() =>
    {
      if (this.lostConnectivityFlag)
        return;
      this.ButtonTapped(CATMeasurementTitle.ST_Uacdc);
    }));
    this.TestButtonTapped = (ICommand) new Command((Action) (() =>
    {
      if (this.lostConnectivityFlag)
        return;
      this.ButtonTapped(CATMeasurementTitle.ST_Test);
    }));
    this.U2ButtonTapped = (ICommand) new Command((Action) (() =>
    {
      if (this.lostConnectivityFlag)
        return;
      this.ButtonTapped(CATMeasurementTitle.ST_U);
    }));
    this.StandByButtonTapped = (ICommand) new Command((Action) (() =>
    {
      if (this.lostConnectivityFlag)
        return;
      this.ButtonTapped(CATMeasurementTitle.ST_StandBy);
    }));
    this.PButtonTapped = (ICommand) new Command((Action) (() =>
    {
      if (this.lostConnectivityFlag)
        return;
      this.ButtonTapped(CATMeasurementTitle.ST_P);
    }));
    this.I1ButtonTapped = (ICommand) new Command((Action) (() =>
    {
      if (this.lostConnectivityFlag)
        return;
      this.ButtonTapped(CATMeasurementTitle.ST_I_CLAMP);
    }));
    this.I2ButtonTapped = (ICommand) new Command((Action) (() =>
    {
      if (this.lostConnectivityFlag)
        return;
      this.ButtonTapped(CATMeasurementTitle.ST_I_SOCKET);
    }));
    this.IDiffCalButtonTapped = (ICommand) new Command((Action) (() =>
    {
      if (this.lostConnectivityFlag)
        return;
      if (this.ShouldForceClampAmpCallibration())
      {
        this.forcedClampAmpCalibration = true;
        this.ButtonTapped(CATMeasurementTitle.ST_IDIFF_CLAMP_CAL);
      }
      else
        this.ButtonTapped(CATMeasurementTitle.ST_IDIFF_CLAMP);
    }));
    this.IDiffButtonTapped = (ICommand) new Command((Action) (() =>
    {
      if (this.lostConnectivityFlag)
        return;
      this.ButtonTapped(CATMeasurementTitle.ST_IDIFF_SOCKET);
    }));
    this.ITouchButtonTapped = (ICommand) new Command((Action) (() =>
    {
      if (this.lostConnectivityFlag)
        return;
      this.ButtonTapped(CATMeasurementTitle.ST_IA);
    }));
    this.IEquivButtonTapped = (ICommand) new Command((Action) (() =>
    {
      if (this.lostConnectivityFlag)
        return;
      this.ButtonTapped(CATMeasurementTitle.ST_IEA);
    }));
    this.Rins2ButtonTapped = (ICommand) new Command((Action) (() =>
    {
      if (this.lostConnectivityFlag)
        return;
      this.ButtonTapped(CATMeasurementTitle.ST_RISO_PROBE);
    }));
    this.RinsButtonTapped = (ICommand) new Command((Action) (() =>
    {
      if (this.lostConnectivityFlag)
        return;
      this.ButtonTapped(CATMeasurementTitle.ST_RISO_SOCKET);
    }));
    this.RpeButtonTapped = (ICommand) new Command((Action) (() =>
    {
      if (this.lostConnectivityFlag)
        return;
      this.ButtonTapped(CATMeasurementTitle.ST_SL);
    }));
    this.RpeCalButtonTapped = (ICommand) new Command((Action) (() =>
    {
      if (this.lostConnectivityFlag)
        return;
      this.ButtonTapped(CATMeasurementTitle.ST_SL1);
    }));
    this.CalLeftButtonTapped = (ICommand) new Command((Action) (() =>
    {
      if (this.lostConnectivityFlag)
        ;
    }));
    this.CalRightButtonTapped = (ICommand) new Command((Action) (() =>
    {
      if (this.lostConnectivityFlag)
        ;
    }));
    this.ZapButtonTapped = (ICommand) new Command((Action) (() =>
    {
      if (this.lostConnectivityFlag)
        ;
    }));
    this.TickButtonTapped = (ICommand) new Command((Action) (() =>
    {
      if (this.lostConnectivityFlag)
        ;
    }));
    this.FailButtonTapped = (ICommand) new Command((Action) (() =>
    {
      if (this.lostConnectivityFlag)
        ;
    }));
    this.AutoButtonTapped = (ICommand) new Command((Action) (() =>
    {
      if (this.lostConnectivityFlag)
        ;
    }));
    this.RedButtonTapped = (ICommand) new Command((Action) (() =>
    {
      if (this.lostConnectivityFlag)
        ;
    }));
    this.CalLeftTickButtonTapped = (ICommand) new Command((Action) (() =>
    {
      if (this.lostConnectivityFlag)
        ;
    }));
    this.CalLeftXButtonTapped = (ICommand) new Command((Action) (() =>
    {
      if (this.lostConnectivityFlag)
        ;
    }));
    this.CalRightTickButtonTapped = (ICommand) new Command((Action) (() =>
    {
      if (this.lostConnectivityFlag)
        ;
    }));
    this.CalRightXButtonTapped = (ICommand) new Command((Action) (() =>
    {
      if (this.lostConnectivityFlag)
        ;
    }));
    this.StartRecordingButtonTapped = (ICommand) new Command((Action) (() =>
    {
      if (this.lostConnectivityFlag)
        return;
      this.RecordButtonTapped();
    }));
    this.BatchButtonTapped = (ICommand) new Command((Action) (() =>
    {
      if (this.lostConnectivityFlag)
        return;
      this.BatchMeasurementsStart();
    }));
    this.StopRecordingButtonTapped = (ICommand) new Command((Action) (async () =>
    {
      if (this.lostConnectivityFlag)
        return;
      await this.StopButtonTapped();
    }));
    this.PolarityButtonTapped = (ICommand) new Command((Action) (() =>
    {
      if (this.lostConnectivityFlag)
        return;
      this.ChangePolarity();
    }));
    this.OffLongPressed = (ICommand) new Command((Action) (async () =>
    {
      if (this.lostConnectivityFlag)
        return;
      this.CustomAssesmentResultList.Clear();
      this.BatchSummaryVisible = false;
      this.CustomSummaryVisible = false;
      this._loggingService.getLogger().LogAppInformation(LoggingContext.USERSESSION, "Service User action popup appeared with message: " + this._metadata.getShortText("30290", "en"), memberName: ".ctor", sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/MeasureViewModel.cs", sourceLineNumber: 336);
      if (await Application.Current.MainPage.DisplayAlert("", this._metadata.getShortText("30290", CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToLowerInvariant()), AppResource.WARNING_OK, AppResource.CANCEL_LABEL))
        await Task.Factory.StartNew((Action) (() =>
        {
          try
          {
            if (!this._sshWrapper.SetMeasurement(JsonConvert.SerializeObject((object) new TargetMeasurements()
            {
              target_measurements = new List<Measurement>()
              {
                new Measurement()
                {
                  measurement = CATMeasurementTitle.ST_OFF.ToString() + ";RESET",
                  instrument = "CAT"
                }
              }
            })).Success)
              return;
            this._alert.ShowMessageAlertWithKey("MEASURE_CAT_RESET_COMMAND_SENT", AppResource.MEASURE_CAT_RESET_TITLE);
          }
          catch (Exception ex)
          {
            this._loggingService.getLogger().LogAppDebug(LoggingContext.BACKEND, ex.Message, memberName: ".ctor", sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/MeasureViewModel.cs", sourceLineNumber: 362);
          }
        }));
    }));
    this.RpeCalLongPressed = (ICommand) new Command((Action) (() =>
    {
      if (this.lostConnectivityFlag)
        return;
      this.ButtonTapped(CATMeasurementTitle.ST_SL1_CAL);
    }));
    this.ClampCalLongPressed = (ICommand) new Command((Action) (() =>
    {
      if (this.lostConnectivityFlag)
        return;
      this.ButtonTapped(CATMeasurementTitle.ST_IDIFF_CLAMP_CAL);
    }));
    this.BatchButtonVisible = false;
    this.batchDict = new SortedDictionary<int, string>();
    this.batchStepsResults = new Dictionary<CATMeasurementTitle, bool?>();
    this.BatchSummaryVisible = false;
  }

  internal void ChangePolarity()
  {
    if (!(this.PolarityButtonSource == "MeasurePolarityGreen.png") || this.PolarityChangeTapped)
      return;
    try
    {
      this.PolarityChangeTapped = true;
      this._sshWrapper.SetMeasurement(JsonConvert.SerializeObject((object) new TargetMeasurements()
      {
        target_measurements = new List<Measurement>()
        {
          new Measurement()
          {
            measurement = CATMeasurementTitle.CHANGE_POLARITY.ToString(),
            instrument = "CAT"
          }
        }
      }));
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppDebug(LoggingContext.BACKEND, ex.Message, memberName: nameof (ChangePolarity), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/MeasureViewModel.cs", sourceLineNumber: 405);
    }
  }

  internal iService5.Core.Services.Data.CATMeasurement GetCatMeasurement(CATMeasurementTitle title)
  {
    return this.catMeasurements.Single<iService5.Core.Services.Data.CATMeasurement>((Func<iService5.Core.Services.Data.CATMeasurement, bool>) (catMeasurement => catMeasurement.title == title));
  }

  internal string GetMeasurementText(string catMeasurementValue, iService5.Core.Services.Data.CATMeasurement catMeasurement)
  {
    try
    {
      if (this.IsOverFlowValue(catMeasurementValue))
        return AppResource.OVERFLOW_TEXT;
      string[] strArray = catMeasurementValue.Split(' ');
      string str1 = strArray[0];
      string str2 = strArray[1];
      this.DiagramUnit = str2;
      NumberFormatInfo provider1 = new NumberFormatInfo()
      {
        NumberDecimalSeparator = ","
      };
      string s = str1;
      CultureInfo provider2 = new CultureInfo(CultureInfo.CurrentCulture.Name);
      provider2.NumberFormat = provider1;
      Decimal d = 0m;
      ref Decimal local = ref d;
      if (!Decimal.TryParse(s, NumberStyles.Any, (IFormatProvider) provider2, out local))
        return $"{str1.ToString().Replace(".", ",")}\n{str2}";
      if (catMeasurement.fractionalPartAccuracyForUnit != null)
      {
        int decimals = 0;
        Decimal num = 0M;
        bool flag = catMeasurement.fractionalPartAccuracyForUnit.TryGetValue(str2, out decimals);
        if (catMeasurement.title.Equals((object) CATMeasurementTitle.ST_SL) || catMeasurement.title.Equals((object) CATMeasurementTitle.ST_SL1))
        {
          if (this.rpeOffset != null)
          {
            this._loggingService.getLogger().LogAppInformation(LoggingContext.MEASURE, "Precalibrated value: " + catMeasurementValue, memberName: nameof (GetMeasurementText), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/MeasureViewModel.cs", sourceLineNumber: 445);
            this._loggingService.getLogger().LogAppInformation(LoggingContext.MEASURE, "Current offset: " + this.rpeOffset.Replace("\n", " "), memberName: nameof (GetMeasurementText), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/MeasureViewModel.cs", sourceLineNumber: 446);
            double inValidationUnit = this.GetValueInValidationUnit(this.rpeOffset, str2);
            try
            {
              num = Convert.ToDecimal(inValidationUnit);
              this._loggingService.getLogger().LogAppDebug(LoggingContext.MEASURE, $"Measurement unit converted offset decimal value: {num} {str2}", memberName: nameof (GetMeasurementText), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/MeasureViewModel.cs", sourceLineNumber: 451);
            }
            catch (Exception ex)
            {
              this._loggingService.getLogger().LogAppDebug(LoggingContext.MEASURE, "Converting offset to decimal error: " + ex.Message, memberName: nameof (GetMeasurementText), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/MeasureViewModel.cs", sourceLineNumber: 455);
            }
            d -= num;
          }
          if (d < 0M)
            return this.recalStr;
        }
        if (flag)
          d = Math.Round(d, decimals);
      }
      return $"{d.ToString((IFormatProvider) provider1)}\n{str2}";
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppError(LoggingContext.LOCAL, ex.Message, memberName: nameof (GetMeasurementText), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/MeasureViewModel.cs", sourceLineNumber: 482);
      return string.Empty;
    }
  }

  internal bool IsOverFlowValue(string measurementValue)
  {
    return measurementValue.StartsWith("t") || measurementValue.StartsWith(AppResource.OVERFLOW_TEXT);
  }

  private void BatchMeasurementsStart()
  {
    this.TickButtonVisible = false;
    this.FailButtonVisible = false;
    if (this.batchindex == 0)
    {
      this.changeButtonSrc = "Continuee.png";
    }
    else
    {
      this.tokenSource.Cancel();
      this.tokenSource.Dispose();
    }
    if (this.batchindex < this.batchMaxNumber)
    {
      CATMeasurementTitle measurement = this.getMeasurement(this.batchDictSorted.ElementAt<KeyValuePair<int, string>>(this.batchindex).Value);
      ++this.batchindex;
      if (measurement == CATMeasurementTitle.ST_OFF)
        return;
      this.ButtonTappedForBatch(measurement);
    }
    else
    {
      this.batchindex = 0;
      this.changeButtonSrc = "aUTO.png";
      if (this.batchStepsResults.Any<KeyValuePair<CATMeasurementTitle, bool?>>((Func<KeyValuePair<CATMeasurementTitle, bool?>, bool>) (r =>
      {
        bool? nullable = r.Value;
        bool flag = false;
        return nullable.GetValueOrDefault() == flag & nullable.HasValue;
      })))
      {
        this.assesment = false;
        this.TickButtonVisible = false;
        this.FailButtonVisible = true;
        this._alert.ShowMessageAlertBoxWithMessage("Batch Assesment Failed", "Alert");
      }
      else
      {
        this.assesment = true;
        this.TickButtonVisible = true;
        this.FailButtonVisible = false;
        this._alert.ShowMessageAlertBoxWithMessage("Batch Assesment Succes", "Alert");
      }
      this.batchStepsResultInfo.Select<KeyValuePair<KeyValuePair<string, bool?>, string>, MeasureViewModel.AssesmentResult>((Func<KeyValuePair<KeyValuePair<string, bool?>, string>, MeasureViewModel.AssesmentResult>) (p => new MeasureViewModel.AssesmentResult()
      {
        Name = p.Key.Key,
        Status = p.Key.Value,
        LastValue = p.Value
      })).ToList<MeasureViewModel.AssesmentResult>().ToList<MeasureViewModel.AssesmentResult>().ForEach((Action<MeasureViewModel.AssesmentResult>) (item =>
      {
        item.LastValue = item.LastValue.Replace(Environment.NewLine, " ");
        ((Collection<MeasureViewModel.AssesmentResult>) this.AssesmentResultList).Add(item);
        this.HistoryAssessmentResultList.Add(item);
      }));
      this.modeBatchAssesment = true;
      this.GoBack();
    }
  }

  private CATMeasurementTitle getMeasurement(string currentBatchMeasurementValue)
  {
    try
    {
      switch (currentBatchMeasurementValue.ToUpper().Trim())
      {
        case "COMP":
          return CATMeasurementTitle.ST_COMP;
        case "IA_PROBE":
          return CATMeasurementTitle.ST_IA;
        case "IDIFF_CLAMP":
          return CATMeasurementTitle.ST_IDIFF_CLAMP;
        case "IDIFF_SOCKET":
          return CATMeasurementTitle.ST_IDIFF_SOCKET;
        case "IEA_SOCKET":
          return CATMeasurementTitle.ST_IEA;
        case "I_CLAMP":
          return CATMeasurementTitle.ST_I_CLAMP;
        case "I_SOCKET":
          return CATMeasurementTitle.ST_I_SOCKET;
        case "P_STANDBY":
          return CATMeasurementTitle.ST_StandBy;
        case "P_Socket":
          return CATMeasurementTitle.ST_P;
        case "RISO_PROBE":
          return CATMeasurementTitle.ST_RISO_PROBE;
        case "RISO_SOCKET":
          return CATMeasurementTitle.ST_RISO_SOCKET;
        case "RPE_LOOP":
          return CATMeasurementTitle.ST_SL1;
        case "RPE_SOCKET":
          return CATMeasurementTitle.ST_SL;
        case "U_ADC":
          return CATMeasurementTitle.ST_Uacdc;
        case "U_SOCKET":
          return CATMeasurementTitle.ST_U;
        default:
          return CATMeasurementTitle.ST_OFF;
      }
    }
    catch (Exception ex)
    {
      return CATMeasurementTitle.ST_OFF;
    }
  }

  internal bool? ValidateMeasurement(string measurementText, CATMeasurementTitle title)
  {
    this.TickButtonVisible = false;
    this.FailButtonVisible = false;
    if (string.IsNullOrEmpty(measurementText))
      return new bool?();
    iService5.Core.Services.Data.CATMeasurement catMeasurement = this.GetCatMeasurement(title);
    switch (title)
    {
      case CATMeasurementTitle.ST_OFF:
      case CATMeasurementTitle.ST_COMP:
      case CATMeasurementTitle.ST_Uacdc:
      case CATMeasurementTitle.ST_Test:
      case CATMeasurementTitle.ST_U:
      case CATMeasurementTitle.ST_StandBy:
      case CATMeasurementTitle.ST_P:
      case CATMeasurementTitle.ST_I_CLAMP:
      case CATMeasurementTitle.ST_I_SOCKET:
        return new bool?();
      default:
        if (this.IsOverFlowValue(measurementText))
        {
          this.TickButtonVisible = false;
          this.FailButtonVisible = true;
          return new bool?(false);
        }
        if ((!catMeasurement.isMinAvailable || this.GetValueInValidationUnit(measurementText, catMeasurement.unit) >= catMeasurement.min) & (!catMeasurement.isMaxAvailable || this.GetValueInValidationUnit(measurementText, catMeasurement.unit) <= catMeasurement.max))
        {
          if (this.selectedMeasurementTitle != CATMeasurementTitle.ST_Test)
          {
            this.TickButtonVisible = true;
            this.FailButtonVisible = false;
          }
          return new bool?(true);
        }
        if (this.selectedMeasurementTitle != CATMeasurementTitle.ST_Test)
        {
          this.TickButtonVisible = false;
          this.FailButtonVisible = true;
        }
        return new bool?(false);
    }
  }

  internal bool ShouldForceClampAmpCallibration()
  {
    bool flag = false;
    Settings settings1 = CoreApp.settings.GetItem("SelectedCallibrationOption");
    if (settings1 != null && settings1.Value != "")
    {
      string a = settings1.Value;
      if (string.Equals(a, AppResource.CALLIBRATION_EVERY_TIME))
        flag = true;
      else if (string.Equals(a, AppResource.CALLIBRATION_MANUALLY))
        flag = false;
      else if (string.Equals(a, AppResource.CALLIBRATION_ONCE_A_DAY) || string.Equals(a, AppResource.CALLIBRATION_ONCE_A_WEEK))
      {
        Settings settings2 = CoreApp.settings.GetItem("LastClampAmpCalDoneTime");
        if (settings2 != null && settings2.Value != "")
        {
          TimeSpan timeSpan = DateTime.Now - UtilityFunctions.ConvertStringToDateTime(settings2.Value);
          if (string.Equals(a, AppResource.CALLIBRATION_ONCE_A_DAY))
          {
            if (timeSpan.TotalHours > 24.0)
              flag = true;
          }
          else if (timeSpan.TotalDays > 7.0)
            flag = true;
        }
      }
      else
        flag = false;
    }
    return flag;
  }

  private double GetValueInValidationUnit(string measurementText, string validationUnit)
  {
    string[] strArray = measurementText.Split('\n');
    string str1 = strArray[1];
    string str2 = strArray[0];
    double inValidationUnit1 = 0.0;
    double inValidationUnit2 = 0.0;
    bool flag = false;
    if (str2.Contains(">"))
    {
      flag = true;
      str2 = str2.Replace(">", string.Empty);
    }
    string s = str2;
    CultureInfo provider = new CultureInfo(CultureInfo.CurrentCulture.Name);
    provider.NumberFormat = new NumberFormatInfo()
    {
      NumberDecimalSeparator = ","
    };
    ref double local = ref inValidationUnit1;
    if (double.TryParse(s, NumberStyles.Any, (IFormatProvider) provider, out local))
    {
      if (flag)
        ++inValidationUnit1;
      if (str1.Equals(validationUnit))
        return inValidationUnit1;
      CATSUnitsConverter result;
      if (Enum.TryParse<CATSUnitsConverter>($"{str1}To{validationUnit}", out result))
      {
        switch (result)
        {
          case CATSUnitsConverter.mOhmToOhm:
            inValidationUnit2 = inValidationUnit1 / 1000.0;
            break;
          case CATSUnitsConverter.mOhmTokOhm:
            inValidationUnit2 = inValidationUnit1 / 1000000.0;
            break;
          case CATSUnitsConverter.mOhmToMOhm:
            inValidationUnit2 = inValidationUnit1 / 1000000000.0;
            break;
          case CATSUnitsConverter.OhmTomOhm:
            inValidationUnit2 = inValidationUnit1 * 1000.0;
            break;
          case CATSUnitsConverter.OhmTokOhm:
            inValidationUnit2 = inValidationUnit1 / 1000.0;
            break;
          case CATSUnitsConverter.OhmToMOhm:
            inValidationUnit2 = inValidationUnit1 / 1000000.0;
            break;
          case CATSUnitsConverter.kOhmTomOhm:
            inValidationUnit2 = inValidationUnit1 * 1000.0 * 1000.0;
            break;
          case CATSUnitsConverter.kOhmToOhm:
            inValidationUnit2 = inValidationUnit1 * 1000.0;
            break;
          case CATSUnitsConverter.kOhmToMOhm:
            inValidationUnit2 = inValidationUnit1 / 1000.0;
            break;
          case CATSUnitsConverter.MohmTomOhm:
            inValidationUnit2 = inValidationUnit1 * 1000.0 * 1000.0 * 1000.0;
            break;
          case CATSUnitsConverter.MOhmToOhm:
            inValidationUnit2 = inValidationUnit1 * 1000.0 * 1000.0;
            break;
          case CATSUnitsConverter.MOhmTokOhm:
            inValidationUnit2 = inValidationUnit1 * 1000.0;
            break;
          case CATSUnitsConverter.mAToA:
            inValidationUnit2 = inValidationUnit1 / 1000.0;
            break;
          case CATSUnitsConverter.ATomA:
            inValidationUnit2 = inValidationUnit1 * 1000.0;
            break;
          default:
            inValidationUnit2 = inValidationUnit1;
            break;
        }
      }
    }
    return inValidationUnit2;
  }

  public ICommand OffLongPressed { internal set; get; }

  public ICommand ClampCalLongPressed { internal set; get; }

  public ICommand RpeCalLongPressed { internal set; get; }

  public ICommand GoBackCommand { internal set; get; }

  public ICommand OffButtonTapped { internal set; get; }

  public ICommand RpeCalButtonTapped { internal set; get; }

  public ICommand CompButtonTapped { internal set; get; }

  public ICommand RpeButtonTapped { internal set; get; }

  public ICommand U1ButtonTapped { internal set; get; }

  public ICommand RinsButtonTapped { internal set; get; }

  public ICommand TestButtonTapped { internal set; get; }

  public ICommand Rins2ButtonTapped { internal set; get; }

  public ICommand U2ButtonTapped { internal set; get; }

  public ICommand IEquivButtonTapped { internal set; get; }

  public ICommand StandByButtonTapped { internal set; get; }

  public ICommand ITouchButtonTapped { internal set; get; }

  public ICommand PButtonTapped { internal set; get; }

  public ICommand IDiffButtonTapped { internal set; get; }

  public ICommand I1ButtonTapped { internal set; get; }

  public ICommand IDiffCalButtonTapped { internal set; get; }

  public ICommand I2ButtonTapped { internal set; get; }

  public ICommand CalLeftButtonTapped { internal set; get; }

  public ICommand CalRightButtonTapped { internal set; get; }

  public ICommand ZapButtonTapped { internal set; get; }

  public ICommand TickButtonTapped { internal set; get; }

  public ICommand FailButtonTapped { internal set; get; }

  public ICommand AutoButtonTapped { internal set; get; }

  public ICommand RedButtonTapped { internal set; get; }

  public ICommand CalLeftTickButtonTapped { internal set; get; }

  public ICommand CalLeftXButtonTapped { internal set; get; }

  public ICommand CalRightTickButtonTapped { internal set; get; }

  public ICommand CalRightXButtonTapped { internal set; get; }

  public ICommand StartRecordingButtonTapped { internal set; get; }

  public ICommand StopRecordingButtonTapped { internal set; get; }

  public ICommand BatchButtonTapped { internal set; get; }

  public ICommand PolarityButtonTapped { internal set; get; }

  public Action RefreshViewAction { get; set; }

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
      if (this._RepairEnumber == null)
        this._RepairEnumber = AppResource.BRIDGE_HEADER;
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

  public string MeasurementText
  {
    get => this._MeasurementText;
    internal set
    {
      this._MeasurementText = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.MeasurementText));
    }
  }

  public double doubleVal
  {
    get => this._doubleVal;
    private set
    {
      this._doubleVal = value;
      if (this.DiagramVisible)
      {
        this.Entries.Add(new DataPoint(TimeSpanAxis.ToDouble(TimeSpan.FromSeconds((double) this.Counter)), this._doubleVal));
        if (this.Recording)
          Device.BeginInvokeOnMainThread((Action) (() => this.PopulateChart(this.Entries)));
        ++this.Counter;
      }
      this.RaisePropertyChanged<double>((Expression<Func<double>>) (() => this.doubleVal));
    }
  }

  public double KnobRotation
  {
    get => this._KnobRotation;
    internal set
    {
      this._KnobRotation = value;
      this.RaisePropertyChanged<double>((Expression<Func<double>>) (() => this.KnobRotation));
    }
  }

  public bool BackBtnStatus
  {
    get => this._BackBtnStatus;
    internal set
    {
      this._BackBtnStatus = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.BackBtnStatus));
    }
  }

  public bool BackBtnVis
  {
    get => this._BackBtnVis;
    internal set
    {
      this._BackBtnVis = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.BackBtnVis));
    }
  }

  public bool CalLeftButtonVisible
  {
    get => this._CalLeftButtonVisible;
    internal set
    {
      this._CalLeftButtonVisible = value;
      if (value)
      {
        this.CalLeftTickButtonVisible = false;
        this.CalLeftXButtonVisible = false;
      }
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.CalLeftButtonVisible));
    }
  }

  public bool CalLeftTickButtonVisible
  {
    get => this._CalLeftTickButtonVisible;
    internal set
    {
      this._CalLeftTickButtonVisible = value;
      if (value)
      {
        this.CalLeftButtonVisible = false;
        this.CalLeftXButtonVisible = false;
      }
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.CalLeftTickButtonVisible));
    }
  }

  public bool CalLeftXButtonVisible
  {
    get => this._CalLeftXButtonVisible;
    internal set
    {
      this._CalLeftXButtonVisible = value;
      if (value)
      {
        this.CalLeftButtonVisible = false;
        this.CalLeftTickButtonVisible = false;
      }
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.CalLeftXButtonVisible));
    }
  }

  public bool CalRightButtonVisible
  {
    get => this._CalRightButtonVisible;
    internal set
    {
      this._CalRightButtonVisible = value;
      if (value)
      {
        this.CalRightTickButtonVisible = false;
        this.CalRightXButtonVisible = false;
      }
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.CalRightButtonVisible));
    }
  }

  public bool CalRightTickButtonVisible
  {
    get => this._CalRightTickButtonVisible;
    internal set
    {
      this._CalRightTickButtonVisible = value;
      if (value)
      {
        this.CalRightButtonVisible = false;
        this.CalRightXButtonVisible = false;
      }
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.CalRightTickButtonVisible));
    }
  }

  public bool CalRightXButtonVisible
  {
    get => this._CalRightXButtonVisible;
    internal set
    {
      this._CalRightXButtonVisible = value;
      if (value)
      {
        this.CalRightButtonVisible = false;
        this.CalRightTickButtonVisible = false;
      }
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.CalRightXButtonVisible));
    }
  }

  public bool BatchButtonVisible
  {
    get => this._BatchButtonVisible;
    set
    {
      this._BatchButtonVisible = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.BatchButtonVisible));
    }
  }

  public bool CompButtonVisible
  {
    get => this._CompButtonVisible;
    internal set
    {
      this._CompButtonVisible = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.CompButtonVisible));
    }
  }

  public bool U1ButtonVisible
  {
    get => this._U1ButtonVisible;
    internal set
    {
      this._U1ButtonVisible = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.U1ButtonVisible));
    }
  }

  public bool TestButtonVisible
  {
    get => this._TestButtonVisible;
    internal set
    {
      this._TestButtonVisible = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.TestButtonVisible));
    }
  }

  public bool U2ButtonVisible
  {
    get => this._U2ButtonVisible;
    internal set
    {
      this._U2ButtonVisible = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.U2ButtonVisible));
    }
  }

  public bool StandByButtonVisible
  {
    get => this._StandByButtonVisible;
    internal set
    {
      this._StandByButtonVisible = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.StandByButtonVisible));
    }
  }

  public bool PButtonVisible
  {
    get => this._PButtonVisible;
    internal set
    {
      this._PButtonVisible = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.PButtonVisible));
    }
  }

  public bool I1ButtonVisible
  {
    get => this._I1ButtonVisible;
    internal set
    {
      this._I1ButtonVisible = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.I1ButtonVisible));
    }
  }

  public bool I2ButtonVisible
  {
    get => this._I2ButtonVisible;
    internal set
    {
      this._I2ButtonVisible = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.I2ButtonVisible));
    }
  }

  public bool IDiffCalButtonVisible
  {
    get => this._IDiffCalButtonVisible;
    internal set
    {
      this._IDiffCalButtonVisible = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.IDiffCalButtonVisible));
    }
  }

  public bool IDiffButtonVisible
  {
    get => this._IDiffButtonVisible;
    internal set
    {
      this._IDiffButtonVisible = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.IDiffButtonVisible));
    }
  }

  public bool ITouchButtonVisible
  {
    get => this._ITouchButtonVisible;
    internal set
    {
      this._ITouchButtonVisible = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.ITouchButtonVisible));
    }
  }

  public bool IEquivButtonVisible
  {
    get => this._IEquivButtonVisible;
    internal set
    {
      this._IEquivButtonVisible = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.IEquivButtonVisible));
    }
  }

  public bool Rins2ButtonVisible
  {
    get => this._Rins2ButtonVisible;
    internal set
    {
      this._Rins2ButtonVisible = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.Rins2ButtonVisible));
    }
  }

  public bool RinsButtonVisible
  {
    get => this._RinsButtonVisible;
    internal set
    {
      this._RinsButtonVisible = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.RinsButtonVisible));
    }
  }

  public bool RpeButtonVisible
  {
    get => this._RpeButtonVisible;
    internal set
    {
      this._RpeButtonVisible = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.RpeButtonVisible));
    }
  }

  public bool RpeCalButtonVisible
  {
    get => this._RpeCalButtonVisible;
    internal set
    {
      this._RpeCalButtonVisible = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.RpeCalButtonVisible));
    }
  }

  public string MeasurementTitle
  {
    get => this._MeasurementTitle;
    internal set
    {
      this._MeasurementTitle = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.MeasurementTitle));
    }
  }

  public CATMeasurementTitle CATMeasurement
  {
    get => this._CATMeasurement;
    internal set
    {
      this._CATMeasurement = value;
      this.RaisePropertyChanged<CATMeasurementTitle>((Expression<Func<CATMeasurementTitle>>) (() => this.CATMeasurement));
    }
  }

  public string MeasurementName
  {
    get => this._MeasurementName;
    internal set
    {
      this._MeasurementName = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.MeasurementName));
    }
  }

  public bool InstructionsVisible
  {
    get => this._InstructionsVisible;
    internal set
    {
      this._InstructionsVisible = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.InstructionsVisible));
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.ButtonsVisible));
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.RecordButtonsVisible));
    }
  }

  public bool RecordButtonsVisible => this.InstructionsVisible;

  public bool DiagramVisible
  {
    get => this._DiagramVisible;
    internal set
    {
      this._DiagramVisible = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.DiagramVisible));
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.ButtonsVisible));
    }
  }

  public bool ButtonsVisible
  {
    get
    {
      return !this.InstructionsVisible && !this.DiagramVisible && !this.BatchSummaryVisible && !this.CustomSummaryVisible;
    }
  }

  public bool BatchSummaryVisible
  {
    get => this._BatchSummaryVisible;
    internal set
    {
      this._BatchSummaryVisible = value;
      this.MeasurementText = "";
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.BatchSummaryVisible));
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.ButtonsVisible));
    }
  }

  public bool CustomSummaryVisible
  {
    get => this._CustomSummaryVisible;
    internal set
    {
      this._CustomSummaryVisible = value;
      this.MeasurementText = "";
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.CustomSummaryVisible));
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.ButtonsVisible));
    }
  }

  public bool TickButtonVisible
  {
    get => this._TickButtonVisible;
    internal set
    {
      this._TickButtonVisible = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.TickButtonVisible));
    }
  }

  public bool FailButtonVisible
  {
    get => this._FailButtonVisible;
    internal set
    {
      this._FailButtonVisible = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.FailButtonVisible));
    }
  }

  public bool VoltageButtonVisible
  {
    get => this._VoltageButtonVisible;
    internal set
    {
      this._VoltageButtonVisible = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.VoltageButtonVisible));
    }
  }

  public bool ZapButtonVisible
  {
    get => this._ZapButtonVisible;
    internal set
    {
      this._ZapButtonVisible = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.ZapButtonVisible));
    }
  }

  public bool PolarityButtonVisible
  {
    get => this._PolarityButtonVisible;
    internal set
    {
      this._PolarityButtonVisible = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.PolarityButtonVisible));
    }
  }

  public PlotModel Chart
  {
    get => this._Chart;
    internal set
    {
      this._Chart = value;
      this.RaisePropertyChanged<PlotModel>((Expression<Func<PlotModel>>) (() => this.Chart));
    }
  }

  public bool StartRecordingButtonDisabled
  {
    get => this._StartRecordingButtonDisabled;
    internal set
    {
      this._StartRecordingButtonDisabled = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.StartRecordingButtonDisabled));
    }
  }

  public bool BlackPanelVisible
  {
    get => this._BlackPanelVisible;
    internal set
    {
      this._BlackPanelVisible = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.BlackPanelVisible));
    }
  }

  public bool TestPanelVisible
  {
    get => this._TestPanelVisible;
    internal set
    {
      this._TestPanelVisible = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.TestPanelVisible));
    }
  }

  public bool SaveButtonDisabled
  {
    get => this._SaveButtonDisabled;
    internal set
    {
      this._SaveButtonDisabled = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.SaveButtonDisabled));
    }
  }

  public string DiagramTitle
  {
    get => this._DiagramTitle;
    internal set
    {
      this._DiagramTitle = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.DiagramTitle));
    }
  }

  public string changeButtonSrc
  {
    get => this._changeButtonSrc;
    internal set
    {
      this._changeButtonSrc = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.changeButtonSrc));
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

  public bool modeBatchAssesment
  {
    get => this._canDisplayBatchAssesment;
    internal set
    {
      this._canDisplayBatchAssesment = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.modeBatchAssesment));
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.modeCustomAssesment));
    }
  }

  public bool modeCustomAssesment => !this.modeBatchAssesment;

  public List<MeasureViewModel.AssesmentResult> CustomAssesmentResultList
  {
    get => this._CustomAssesmentResultList;
    set
    {
      this._CustomAssesmentResultList = value;
      this.RaisePropertyChanged<List<MeasureViewModel.AssesmentResult>>((Expression<Func<List<MeasureViewModel.AssesmentResult>>>) (() => this.CustomAssesmentResultList));
    }
  }

  public double TestGridLabelFontSize => Device.Idiom == TargetIdiom.Tablet ? 25.0 : 15.0;

  public string TestItouchValidationImageSource
  {
    get => this._TestItouchValidationImageSource;
    set
    {
      this._TestItouchValidationImageSource = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.TestItouchValidationImageSource));
    }
  }

  public string TestIDiffValidationImageSource
  {
    get => this._TestIDiffValidationImageSource;
    set
    {
      this._TestIDiffValidationImageSource = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.TestIDiffValidationImageSource));
    }
  }

  public override void ViewAppearing()
  {
    base.ViewAppearing();
    this.measurementsResultsList = new List<MeasureViewModel.MeasurementResultsP>();
  }

  public override void ViewAppeared()
  {
    base.ViewAppeared();
    this._appliance.CheckConnectivity(new connectivityUpdateCallback(this.ConnectivityCheck));
    this.GetAvailableMeasurements();
  }

  public override void ViewDisappearing() => base.ViewDisappearing();

  public void OnBackButtonPressed() => this.GoBack();

  internal void HistoryLog()
  {
    StringBuilder stringBuilder = new StringBuilder();
    foreach (MeasureViewModel.AssesmentResult assessmentResult in this.HistoryAssessmentResultList)
    {
      string str = "";
      if (assessmentResult.Status.HasValue)
        str = !assessmentResult.Status.Value ? "NOT OK" : "OK";
      stringBuilder.AppendLine($"{assessmentResult.mName} {assessmentResult.LastValue} {str}");
    }
    if (string.IsNullOrEmpty(stringBuilder.ToString()))
      return;
    try
    {
      stringBuilder.AppendLine();
      CoreApp.history.SaveItem(new History(this.RepairEnumber, DateTime.Now, UtilityFunctions.getSessionIDForHistoryTable(), HistoryDBInfoType.MeasureLog.ToString(), stringBuilder.ToString()));
      this.HistoryAssessmentResultList.Clear();
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppError(LoggingContext.MEASURE, "Failed to save item in the History DB, " + ex?.ToString(), memberName: nameof (HistoryLog), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/MeasureViewModel.cs", sourceLineNumber: 1829);
    }
  }

  internal void GoBack()
  {
    MessagingCenter.Unsubscribe<NonSmmConnectionViewModel, Values>((object) this, "ValuesRetrieved");
    try
    {
      this.tokenSource.Cancel();
      this.tokenSource.Dispose();
    }
    catch
    {
    }
    if (this.DiagramVisible)
    {
      this.StopRecording = DateTime.Now;
      this.Recording = false;
      this.StartRecordingButtonDisabled = false;
      if (this.BlackPanelUsed)
      {
        this.BlackPanelUsed = false;
        this.BlackPanelVisible = true;
        this.TestPanelVisible = false;
      }
      this.DisableSaveButton();
      this.DiagramVisible = false;
    }
    else if (this.measurementButtonTapped)
    {
      if (this.CustomSummaryVisible)
        this.CustomSummaryVisible = false;
      else if (this.BatchSummaryVisible)
      {
        this.modeBatchAssesment = false;
        this.BatchSummaryVisible = false;
        this.batchStepsResultInfo.Clear();
        this.batchStepsResults.Clear();
        ((Collection<MeasureViewModel.AssesmentResult>) this.AssesmentResultList).Clear();
      }
      else if (this.InstructionsVisible)
      {
        string measurement = JsonConvert.SerializeObject((object) new TargetMeasurements()
        {
          target_measurements = new List<Measurement>()
          {
            new Measurement()
            {
              measurement = CATMeasurementTitle.IDLE.ToString(),
              instrument = "CAT"
            }
          }
        });
        try
        {
          if (this._sshWrapper.SetMeasurement(measurement).Success)
          {
            this.currentMeasurement = CATMeasurementTitle.ST_OFF;
            this.Entries.Clear();
            this.Counter = 1;
            this.InstructionsVisible = false;
            this.TestPanelVisible = true;
            this.PolarityChangeTapped = false;
          }
        }
        catch (Exception ex)
        {
          this._loggingService.getLogger().LogAppError(LoggingContext.BACKEND, "The measurement value that failed to get set is the IDLE value - " + ex.Message, memberName: nameof (GoBack), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/MeasureViewModel.cs", sourceLineNumber: 1901);
        }
      }
      this.PolarityButtonVisible = false;
      this.measurementButtonTapped = false;
    }
    else
      this.ClosePage();
    this.backButtonTapped = false;
  }

  public void ClosePage()
  {
    this.IsBusy = true;
    if (this.BackBtnClicked)
      return;
    try
    {
      this.tokenSource.Cancel();
      this.tokenSource.Dispose();
    }
    catch
    {
    }
    this.BackBtnClicked = true;
    this.DiagramVisible = false;
    this.InstructionsVisible = false;
    this.BatchSummaryVisible = false;
    this.CustomSummaryVisible = false;
    if (this.measurementsResultsList.Count > 0)
      this.CustomAssesmentResultList = this.UpdateCustomAssessmentResultsList(this.measurementsResultsList);
    this.measurementsResultsList.Clear();
    this.HistoryLog();
    if (this.noenumber)
      this._navigationService.Navigate<NonSmmConnectionViewModel, bool>(true, (IMvxBundle) null, new CancellationToken());
    else
      this._navigationService.Navigate<NonSmmConnectionViewModel>((IMvxBundle) null, new CancellationToken());
    this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
  }

  internal void ResetTestMeasurementValues()
  {
    this.testItouch_result = "";
    this.testItouch_peak = "";
    this.testItouch_limit = "";
    this.TestItouchValidationImageSource = "";
    this.testIdiff_result = "";
    this.testIdiff_peak = "";
    this.testIdiff_limit = "";
    this.TestIDiffValidationImageSource = "";
    this.testI_result = "";
    this.testI_peak = "";
    this.testU_result = "";
    this.testU_peak = "";
    this.testP_result = "";
    this.testP_peak = "";
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
        this.ClosePage();
      }), this._tokenSource.Token);
    }
  }

  public List<MeasureViewModel.AssesmentResult> UpdateCustomAssessmentResultsList(
    List<MeasureViewModel.MeasurementResultsP> _items)
  {
    List<MeasureViewModel.AssesmentResult> list = _items.Select<MeasureViewModel.MeasurementResultsP, MeasureViewModel.AssesmentResult>((Func<MeasureViewModel.MeasurementResultsP, MeasureViewModel.AssesmentResult>) (p => new MeasureViewModel.AssesmentResult()
    {
      Name = p._CATMeasurementTitle.ToString(),
      Status = p._valresult,
      LastValue = p._mestext
    })).ToList<MeasureViewModel.AssesmentResult>();
    list.ToList<MeasureViewModel.AssesmentResult>().ForEach((Action<MeasureViewModel.AssesmentResult>) (item =>
    {
      item.LastValue = item.LastValue.Replace(Environment.NewLine, " ");
      this.HistoryAssessmentResultList.Add(item);
    }));
    return list.ToList<MeasureViewModel.AssesmentResult>();
  }

  internal void BlackPanelVoltageButtonVisibility(iService5.Core.Services.Data.CATMeasurement catMeasurement)
  {
    this.BlackPanelVisible = catMeasurement.position == 11 || catMeasurement.position == 12;
    this.VoltageButtonVisible = catMeasurement.position >= 3 && catMeasurement.position <= 12 && catMeasurement.position != 9 && catMeasurement.position != 10;
    this.TestPanelVisible = false;
    this.TickButtonVisible = false;
    this.FailButtonVisible = false;
    this.PolarityButtonVisible = false;
  }

  internal void CheckPolarityChangeAvailability(
    iService5.Core.Services.Data.CATMeasurement catMeasurement,
    Values valuesFeedback)
  {
    if (catMeasurement.position == 11 || catMeasurement.position == 12)
    {
      if (valuesFeedback.feature_availability.Any<FeatureAvailability>((Func<FeatureAvailability, bool>) (x => x.feature == FeatureAvailabilityFeature.POLARITY_DIRECT.ToString() && x.status == FeatureAvailabilityStatus.AVAILABLE.ToString())))
      {
        this.PolarityButtonSource = "MeasurePolarityGreen.png";
        this.PolarityButtonVisible = true;
      }
      else
      {
        if (!valuesFeedback.feature_availability.Any<FeatureAvailability>((Func<FeatureAvailability, bool>) (x => x.feature == FeatureAvailabilityFeature.POLARITY_REVERSE.ToString() && x.status == FeatureAvailabilityStatus.AVAILABLE.ToString())))
          return;
        this.PolarityButtonSource = "MeasurePolarityRed.png";
        this.PolarityButtonVisible = true;
      }
    }
    else
      this.PolarityButtonVisible = false;
  }

  public void ButtonTapped(CATMeasurementTitle catMeasurementTitle)
  {
    this.measurementButtonTapped = true;
    this.tokenSource = new CancellationTokenSource();
    this.ct = this.tokenSource.Token;
    iService5.Core.Services.Data.CATMeasurement catMeasurement = this.GetCatMeasurement(catMeasurementTitle);
    this.selectedMeasurementTitle = catMeasurementTitle;
    this.DiagramUnit = catMeasurement.unit;
    this.KnobRotation = (catMeasurement.position > 9 ? (double) (catMeasurement.position - 1) : (double) catMeasurement.position) * (360.0 / 17.0);
    this.MeasurementText = catMeasurement.unit;
    this.SvgFilename = catMeasurement.svg + ".svg";
    this.MeasurementTitle = catMeasurement.title.ToString();
    this.MeasurementName = catMeasurement.name;
    this.BlackPanelVoltageButtonVisibility(catMeasurement);
    if (catMeasurement.position != 0)
    {
      if (DeviceInfo.Platform == DevicePlatform.iOS)
        this.RefreshViewAction();
      else
        this.InstructionsVisible = true;
      if (catMeasurement.CatTimeout != "notset")
        this.tokenSource.CancelAfter(1000 * Convert.ToInt32(Convert.ToDouble(catMeasurement.CatTimeout)));
      if (catMeasurementTitle == CATMeasurementTitle.ST_Test)
      {
        this.ResetTestMeasurementValues();
        this.TestPanelVisible = true;
      }
      Task.Factory.StartNew<Task>((Func<Task>) (async () =>
      {
        try
        {
          TargetMeasurements targetMeasurements = new TargetMeasurements()
          {
            target_measurements = new List<Measurement>()
            {
              new Measurement()
              {
                measurement = catMeasurementTitle.ToString(),
                instrument = "CAT"
              }
            }
          };
          string measurementDetailsJson = JsonConvert.SerializeObject((object) targetMeasurements);
          SshResponse _Response = this._sshWrapper.SetMeasurement(measurementDetailsJson);
          if (_Response.Success)
            this.currentMeasurement = catMeasurement.title;
          if (_Response.Success && !this.valueRetreivalTaskStarted)
            await Task.Factory.StartNew((Action) (() =>
            {
              this.valueRetreivalTaskStarted = true;
              try
              {
                this._sshWrapper.GetValues((Stream) null, (iService5.Ssh.models.ProgressCallback) ((progress, step, total) =>
                {
                  if (this.ct.IsCancellationRequested)
                    this.ct.ThrowIfCancellationRequested();
                  try
                  {
                    Values valuesFeedback = JsonConvert.DeserializeObject<Values>(progress);
                    this.CheckPolarityChangeAvailability(catMeasurement, valuesFeedback);
                    foreach (MeasurementResults measurementResult in valuesFeedback.measurement_results)
                    {
                      if (measurementResult.instrument == "ST" || measurementResult.instrument == "CAT" || measurementResult.instrument == "LT")
                      {
                        if (catMeasurementTitle == CATMeasurementTitle.ST_Test)
                        {
                          this.UpdateTestMeasurementsValues(measurementResult);
                        }
                        else
                        {
                          this.MeasurementText = this.GetMeasurementText(measurementResult.value, catMeasurement);
                          this.doubleVal = this.GetPlotValue(this.MeasurementText);
                        }
                      }
                    }
                    if (catMeasurementTitle == CATMeasurementTitle.ST_SL1_CAL && valuesFeedback.feature_availability != null)
                      this.RpeCalMethod(this.MeasurementText, valuesFeedback.feature_availability);
                    else if (catMeasurementTitle == CATMeasurementTitle.ST_IDIFF_CLAMP_CAL && valuesFeedback.feature_availability != null)
                      this.ClampCalMethod(this.MeasurementText, valuesFeedback.feature_availability);
                    this.ct.ThrowIfCancellationRequested();
                  }
                  catch (Exception ex)
                  {
                    this._loggingService.getLogger().LogAppError(LoggingContext.APPLIANCE, "Error during JSON response parsing: " + ex.Message, memberName: nameof (ButtonTapped), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/MeasureViewModel.cs", sourceLineNumber: 2144);
                  }
                }));
              }
              catch (Exception ex)
              {
                this._loggingService.getLogger().LogAppError(LoggingContext.APPLIANCE, "Error during GetValues: " + ex.Message, memberName: nameof (ButtonTapped), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/MeasureViewModel.cs", sourceLineNumber: 2151);
              }
              this.valueRetreivalTaskStarted = false;
              try
              {
                if (catMeasurementTitle == CATMeasurementTitle.ST_Test)
                {
                  if (!string.IsNullOrEmpty(this.testItouch_result))
                    this.measurementsResultsList.Add(new MeasureViewModel.MeasurementResultsP()
                    {
                      _CATMeasurementTitle = CATMeasurementTitle.ST_IA,
                      _mestext = this.testItouch_result,
                      _valresult = new bool?(this.TestItouchValidationImageSource == "MeasureTickACH.png")
                    });
                  if (!string.IsNullOrEmpty(this.testIdiff_result))
                    this.measurementsResultsList.Add(new MeasureViewModel.MeasurementResultsP()
                    {
                      _CATMeasurementTitle = CATMeasurementTitle.ST_IDIFF_SOCKET,
                      _mestext = this.testIdiff_result,
                      _valresult = new bool?(this.TestIDiffValidationImageSource == "MeasureTickACH.png")
                    });
                  if (!string.IsNullOrEmpty(this.testI_result))
                    this.measurementsResultsList.Add(new MeasureViewModel.MeasurementResultsP()
                    {
                      _CATMeasurementTitle = CATMeasurementTitle.ST_I_SOCKET,
                      _mestext = this.testI_result,
                      _valresult = new bool?()
                    });
                  if (!string.IsNullOrEmpty(this.testU_result))
                    this.measurementsResultsList.Add(new MeasureViewModel.MeasurementResultsP()
                    {
                      _CATMeasurementTitle = CATMeasurementTitle.ST_U,
                      _mestext = this.testU_result,
                      _valresult = new bool?()
                    });
                  if (string.IsNullOrEmpty(this.testP_result))
                    return;
                  this.measurementsResultsList.Add(new MeasureViewModel.MeasurementResultsP()
                  {
                    _CATMeasurementTitle = CATMeasurementTitle.ST_P,
                    _mestext = this.testP_result,
                    _valresult = new bool?()
                  });
                }
                else if (!string.IsNullOrEmpty(this.MeasurementText))
                  this.measurementsResultsList.Add(new MeasureViewModel.MeasurementResultsP()
                  {
                    _CATMeasurementTitle = catMeasurement.title,
                    _mestext = this.MeasurementText,
                    _valresult = this.ValidateMeasurement(this.MeasurementText, catMeasurement.title)
                  });
                else
                  this._loggingService.getLogger().LogAppDebug(LoggingContext.BACKEND, "Not ST_Test or no measurement text value available", memberName: nameof (ButtonTapped), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/MeasureViewModel.cs", sourceLineNumber: 2213);
              }
              catch (Exception ex)
              {
                this._loggingService.getLogger().LogAppError(LoggingContext.BACKEND, ex.Message, memberName: nameof (ButtonTapped), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/MeasureViewModel.cs", sourceLineNumber: 2218);
              }
            }), this.ct).ContinueWith((Action<Task>) (t2 =>
            {
              this.valueRetreivalTaskStarted = false;
              t2.Exception.Handle((Func<Exception, bool>) (e => true));
            }), TaskContinuationOptions.OnlyOnCanceled);
          targetMeasurements = (TargetMeasurements) null;
          measurementDetailsJson = (string) null;
          _Response = (SshResponse) null;
        }
        catch (Exception ex)
        {
          this._loggingService.getLogger().LogAppError(LoggingContext.BACKEND, ex.Message, memberName: nameof (ButtonTapped), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/MeasureViewModel.cs", sourceLineNumber: 2232);
          if (this.backButtonTapped || this.currentMeasurement == CATMeasurementTitle.ST_OFF || !(catMeasurement.CatTimeout != "notset"))
            return;
          this.GoBack();
        }
      }), this.ct);
    }
    else
    {
      try
      {
        if (this.modeCustomAssesment && this.CustomSummaryVisible)
        {
          this.ColorValue = SKColor.Parse("#ff0000");
          this.MeasurementText = "Summary";
          this.CustomAssesmentResultList = this.UpdateCustomAssessmentResultsList(this.measurementsResultsList);
        }
        else if (this.modeBatchAssesment)
        {
          this.MeasurementText = "Summary";
        }
        else
        {
          this.MeasurementText = "";
          this.CustomSummaryVisible = false;
        }
        this.measurementsResultsList.Clear();
      }
      catch (Exception ex)
      {
        this._loggingService.getLogger().LogAppDebug(LoggingContext.BACKEND, " error  measurement" + ex.Message, memberName: nameof (ButtonTapped), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/MeasureViewModel.cs", sourceLineNumber: 2264);
      }
    }
  }

  internal void SetTimer(string timeout)
  {
    MeasureViewModel.catTimeOutTimer = new System.Timers.Timer((double) (1000 * Convert.ToInt32(Convert.ToDouble(timeout))));
    MeasureViewModel.catTimeOutTimer.Elapsed += new ElapsedEventHandler(this.OnTimedEvent);
    MeasureViewModel.catTimeOutTimer.AutoReset = true;
    MeasureViewModel.catTimeOutTimer.Enabled = true;
  }

  private void OnTimedEvent(object source, ElapsedEventArgs e) => this.GoBack();

  internal void FinalizeMeasurementResults()
  {
    try
    {
      if (this.CATMeasurement == CATMeasurementTitle.ST_Test)
      {
        if (!string.IsNullOrEmpty(this.testItouch_result))
          this.measurementsResultsList.Add(new MeasureViewModel.MeasurementResultsP()
          {
            _CATMeasurementTitle = CATMeasurementTitle.ST_IA,
            _mestext = this.testItouch_result,
            _valresult = new bool?(this.TestItouchValidationImageSource == "MeasureTickACH.png")
          });
        if (!string.IsNullOrEmpty(this.testIdiff_result))
          this.measurementsResultsList.Add(new MeasureViewModel.MeasurementResultsP()
          {
            _CATMeasurementTitle = CATMeasurementTitle.ST_IDIFF_SOCKET,
            _mestext = this.testIdiff_result,
            _valresult = new bool?(this.TestIDiffValidationImageSource == "MeasureTickACH.png")
          });
        if (!string.IsNullOrEmpty(this.testI_result))
          this.measurementsResultsList.Add(new MeasureViewModel.MeasurementResultsP()
          {
            _CATMeasurementTitle = CATMeasurementTitle.ST_I_SOCKET,
            _mestext = this.testI_result,
            _valresult = new bool?()
          });
        if (!string.IsNullOrEmpty(this.testU_result))
          this.measurementsResultsList.Add(new MeasureViewModel.MeasurementResultsP()
          {
            _CATMeasurementTitle = CATMeasurementTitle.ST_U,
            _mestext = this.testU_result,
            _valresult = new bool?()
          });
        if (string.IsNullOrEmpty(this.testP_result))
          return;
        this.measurementsResultsList.Add(new MeasureViewModel.MeasurementResultsP()
        {
          _CATMeasurementTitle = CATMeasurementTitle.ST_P,
          _mestext = this.testP_result,
          _valresult = new bool?()
        });
      }
      else if (!string.IsNullOrEmpty(this.MeasurementText))
        this.measurementsResultsList.Add(new MeasureViewModel.MeasurementResultsP()
        {
          _CATMeasurementTitle = this.CATMeasurement,
          _mestext = this.MeasurementText,
          _valresult = this.ValidateMeasurement(this.MeasurementText, this.CATMeasurement)
        });
      else
        this._loggingService.getLogger().LogAppDebug(LoggingContext.BACKEND, "Not ST_Test or no measurement text value available", memberName: nameof (FinalizeMeasurementResults), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/MeasureViewModel.cs", sourceLineNumber: 2341);
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppDebug(LoggingContext.BACKEND, ex.Message, memberName: nameof (FinalizeMeasurementResults), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/MeasureViewModel.cs", sourceLineNumber: 2346);
    }
  }

  internal void RpeCalMethod(string measurement, List<FeatureAvailability> featureAvailability)
  {
    if (featureAvailability.Any<FeatureAvailability>((Func<FeatureAvailability, bool>) (x => x.feature == "PROBE_CAL" && x.status == "AVAILABLE")))
    {
      this.rpeOffset = measurement;
      string str = CoreApp.settings.GetItem("rpeCalMeasurement").Value;
      this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, string.IsNullOrEmpty(str) ? "RPE Calibration successful. New measurement: " + this.rpeOffset.Replace("\n", " ") : $"RPE Calibration successful. Previous measurement: {str.Replace("\n", " ")}. New measurement: {this.rpeOffset.Replace("\n", " ")}", memberName: nameof (RpeCalMethod), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/MeasureViewModel.cs", sourceLineNumber: 2356);
      CoreApp.settings.UpdateItem(new Settings("rpeCalSucceeded", "true"));
      CoreApp.settings.UpdateItem(new Settings("rpeCalFailed", "false"));
      CoreApp.settings.UpdateItem(new Settings("rpeCalMeasurement", this.rpeOffset));
      this.CalLeftTickButtonVisible = true;
      this.GoBack();
    }
    else if (featureAvailability.Any<FeatureAvailability>((Func<FeatureAvailability, bool>) (x => x.feature == "PROBE_CAL" && x.status == "MISSING")))
    {
      this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "RPE Calibration feature missing.", memberName: nameof (RpeCalMethod), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/MeasureViewModel.cs", sourceLineNumber: 2366);
    }
    else
    {
      if (!featureAvailability.Any<FeatureAvailability>((Func<FeatureAvailability, bool>) (x => x.feature == "PROBE_CAL" && x.status == "FAILED")))
        return;
      this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "RPE Calibration failed.", memberName: nameof (RpeCalMethod), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/MeasureViewModel.cs", sourceLineNumber: 2370);
      CoreApp.settings.UpdateItem(new Settings("rpeCalFailed", "true"));
      CoreApp.settings.UpdateItem(new Settings("rpeCalSucceeded", "false"));
      this.CalLeftXButtonVisible = true;
      this.GoBack();
    }
  }

  internal void ClampCalMethod(string measurement, List<FeatureAvailability> featureAvailability)
  {
    if (featureAvailability.Any<FeatureAvailability>((Func<FeatureAvailability, bool>) (x => x.feature == "CLAMP_CAL" && x.status == "AVAILABLE")))
    {
      string str = CoreApp.settings.GetItem("clampCalMeasurement").Value;
      this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, string.IsNullOrEmpty(str) ? "Clamp Calibration successful. New measurement: " + measurement.Replace("\n", " ") : $"Clamp Calibration successful. Previous measurement: {str.Replace("\n", " ")}. New measurement: {measurement.Replace("\n", " ")}", memberName: nameof (ClampCalMethod), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/MeasureViewModel.cs", sourceLineNumber: 2383);
      CoreApp.settings.UpdateItem(new Settings("clampCalSucceeded", "true"));
      CoreApp.settings.UpdateItem(new Settings("clampCalFailed", "false"));
      CoreApp.settings.UpdateItem(new Settings("clampCalMeasurement", measurement));
      CoreApp.settings.UpdateItem(new Settings("LastClampAmpCalDoneTime", UtilityFunctions.ConvertDateTimeToString(DateTime.Now)));
      this.CalRightTickButtonVisible = true;
      if (this.forcedClampAmpCalibration)
      {
        this.forcedClampAmpCalibration = false;
        this.ButtonTapped(CATMeasurementTitle.ST_IDIFF_CLAMP);
      }
      else
        this.GoBack();
    }
    else if (featureAvailability.Any<FeatureAvailability>((Func<FeatureAvailability, bool>) (x => x.feature == "CLAMP_CAL" && x.status == "MISSING")))
    {
      this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Clamp Calibration feature missing.", memberName: nameof (ClampCalMethod), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/MeasureViewModel.cs", sourceLineNumber: 2403);
    }
    else
    {
      if (!featureAvailability.Any<FeatureAvailability>((Func<FeatureAvailability, bool>) (x => x.feature == "CLAMP_CAL" && x.status == "FAILED")))
        return;
      this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Clamp Calibration failed.", memberName: nameof (ClampCalMethod), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/MeasureViewModel.cs", sourceLineNumber: 2407);
      CoreApp.settings.UpdateItem(new Settings("clampCalFailed", "true"));
      CoreApp.settings.UpdateItem(new Settings("clampCalSucceeded", "false"));
      this.CalRightXButtonVisible = true;
      this.GoBack();
    }
  }

  internal void UpdateTestMeasurementsValues(MeasurementResults result)
  {
    CATMeasurementTitle result1;
    Enum.TryParse<CATMeasurementTitle>(result.measurement, out result1);
    iService5.Core.Services.Data.CATMeasurement catMeasurement = this.GetCatMeasurement(result1);
    string measurementText1 = this.GetMeasurementText(result.value, catMeasurement);
    string measurementText2 = this.GetMeasurementText($"{catMeasurement.max.ToString()} {catMeasurement.validationUnit}", catMeasurement);
    this.doubleVal = Convert.ToDouble(new string(measurementText1.Where<char>((Func<char, bool>) (x => char.IsDigit(x) || x.Equals('.'))).ToArray<char>()));
    string measurementText3 = this.GetMeasurementText(catMeasurement.peakValue, catMeasurement);
    double inValidationUnit1 = this.GetValueInValidationUnit(measurementText1, catMeasurement.unit);
    double inValidationUnit2 = this.GetValueInValidationUnit(measurementText3, catMeasurement.unit);
    string str1 = measurementText1.Replace("\n", " ");
    string str2 = measurementText2.Replace("\n", " ");
    if (inValidationUnit1 > inValidationUnit2)
      catMeasurement.peakValue = result.value;
    string str3 = this.GetMeasurementText(catMeasurement.peakValue, catMeasurement).Replace("\n", " ");
    switch (result1)
    {
      case CATMeasurementTitle.ST_U:
        this.testU_result = str1;
        this.testU_peak = str3;
        break;
      case CATMeasurementTitle.ST_P:
        this.testP_result = str1;
        this.testP_peak = str3;
        break;
      case CATMeasurementTitle.ST_I_SOCKET:
        this.testI_result = str1;
        this.testI_peak = str3;
        break;
      case CATMeasurementTitle.ST_IDIFF_SOCKET:
        this.testIdiff_result = str1;
        this.testIdiff_limit = str2;
        this.testIdiff_peak = str3;
        this.TestIDiffValidationImageSource = this.GetTestValidationImageSource(measurementText1, result1);
        break;
      case CATMeasurementTitle.ST_IA:
        this._testItouch_result = str1;
        this.testItouch_result = str1;
        this.testItouch_limit = str2;
        this.testItouch_peak = str3;
        this.TestItouchValidationImageSource = this.GetTestValidationImageSource(measurementText1, result1);
        break;
    }
  }

  internal string GetTestValidationImageSource(
    string measurementValue,
    CATMeasurementTitle measurement)
  {
    string validationImageSource = "MeasureFailACH.png";
    this.ValidateMeasurement(measurementValue, measurement);
    if (this.ValidateMeasurement(measurementValue, measurement).Value)
      validationImageSource = "MeasureTickACH.png";
    return validationImageSource;
  }

  internal void ButtonTappedForBatch(CATMeasurementTitle title)
  {
    this.measurementButtonTapped = true;
    this.tokenSource = new CancellationTokenSource();
    this.ct = this.tokenSource.Token;
    iService5.Core.Services.Data.CATMeasurement catMeasurement = this.GetCatMeasurement(title);
    this.DiagramUnit = catMeasurement.unit;
    this.KnobRotation = (catMeasurement.position > 9 ? (double) (catMeasurement.position - 1) : (double) catMeasurement.position) * (360.0 / 17.0);
    this.MeasurementText = catMeasurement.unit;
    this.SvgFilename = catMeasurement.svg + ".svg";
    this.MeasurementTitle = title.ToString();
    this.MeasurementName = catMeasurement.name;
    this.BlackPanelVoltageButtonVisibility(catMeasurement);
    if (catMeasurement.position != 0)
    {
      if (DeviceInfo.Platform == DevicePlatform.iOS)
        this.RefreshViewAction();
      else
        this.InstructionsVisible = true;
    }
    Task.Factory.StartNew((Action) (() =>
    {
      try
      {
        SshResponse sshResponse = this._sshWrapper.SetMeasurement(JsonConvert.SerializeObject((object) new TargetMeasurements()
        {
          target_measurements = new List<Measurement>()
          {
            new Measurement()
            {
              measurement = title.ToString(),
              instrument = "CAT"
            }
          }
        }));
        if (sshResponse.Success)
          this.currentMeasurement = title;
        if (!sshResponse.Success || this.valueRetreivalTaskStarted || !Task.Factory.StartNew((Action) (() =>
        {
          this.valueRetreivalTaskStarted = true;
          string lastValue = string.Empty;
          this._sshWrapper.GetValues((Stream) null, (iService5.Ssh.models.ProgressCallback) ((progress, step, total) =>
          {
            this.ct.ThrowIfCancellationRequested();
            try
            {
              Values valuesFeedback = JsonConvert.DeserializeObject<Values>(progress);
              this.CheckPolarityChangeAvailability(catMeasurement, valuesFeedback);
              foreach (MeasurementResults measurementResult in valuesFeedback.measurement_results)
              {
                if (measurementResult.instrument == "ST" || measurementResult.instrument == "CAT" || measurementResult.instrument == "LT")
                {
                  this.MeasurementText = this.GetMeasurementText(measurementResult.value, catMeasurement);
                  lastValue = this.MeasurementText;
                  this.doubleVal = this.GetPlotValue(this.MeasurementText);
                }
              }
            }
            catch (Exception ex)
            {
              this._loggingService.getLogger().LogAppError(LoggingContext.APPLIANCE, "Error during JSON response parsing: " + ex.Message, memberName: nameof (ButtonTappedForBatch), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/MeasureViewModel.cs", sourceLineNumber: 2536);
            }
          }));
          this.valueRetreivalTaskStarted = false;
          if (string.IsNullOrEmpty(lastValue))
            return;
          this.batchStepsResults.Add(title, this.ValidateMeasurement(lastValue, title));
          this.batchStepsResultInfo.Add(new KeyValuePair<string, bool?>(catMeasurement.name, this.batchStepsResults[title]), lastValue);
        }), this.ct).ContinueWith((Action<Task>) (t2 =>
        {
          this.valueRetreivalTaskStarted = false;
          t2.Exception.Handle((Func<Exception, bool>) (e => true));
        }), TaskContinuationOptions.OnlyOnCanceled).Wait(TimeSpan.FromSeconds(25.0)))
          ;
      }
      catch (Exception ex)
      {
        this._loggingService.getLogger().LogAppError(LoggingContext.BACKEND, ex.Message, memberName: nameof (ButtonTappedForBatch), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/MeasureViewModel.cs", sourceLineNumber: 2559 /*0x09FF*/);
      }
    }), this.ct);
  }

  internal void GetAvailableMeasurements()
  {
    try
    {
      uploadDocument uploadDocument = this._metadata.getMaterialDocuments(this.RepairEnumber).Where<uploadDocument>((Func<uploadDocument, bool>) (x => x.type == "14129")).First<uploadDocument>();
      if (uploadDocument != null)
      {
        string path = Path.Combine(this._locator.GetPlatformSpecificService().getFolder(), uploadDocument.toFileName());
        if (File.Exists(path))
        {
          string[] strArray1 = File.ReadAllLines(path);
          this.batchlist = Enumerable.Repeat<string>(string.Empty, 20).ToList<string>();
          this.batchlist[0] = "batchinit";
          this.batchlistMetron = Enumerable.Repeat<string>(string.Empty, 20).ToList<string>();
          this.batchlistMetron[0] = this.batchlist[0];
          double result1 = 0.0;
          double result2 = 0.0;
          string str1 = "";
          bool flag1 = false;
          bool flag2 = false;
          CATMeasurementTitle title = CATMeasurementTitle.ST_OFF;
          for (int index = 1; index < 20; ++index)
            this.batchlistMetron[index] = "batch" + index.ToString();
          foreach (string str2 in strArray1)
          {
            if (!string.IsNullOrWhiteSpace(str2) && !str2.StartsWith("#") && str2.Contains("available"))
            {
              Dictionary<string, string> dictionary = new Dictionary<string, string>();
              string[] strArray2 = (str2 + ";").Split(';');
              if (!string.IsNullOrEmpty(strArray2[5]))
              {
                if (!string.IsNullOrEmpty(strArray2[3]) && !string.IsNullOrEmpty(strArray2[5]) && double.TryParse(strArray2[3], out result1))
                  flag1 = true;
                if (!string.IsNullOrEmpty(strArray2[4]) && !string.IsNullOrEmpty(strArray2[5]) && double.TryParse(strArray2[4], out result2))
                  flag2 = true;
                str1 = strArray2[5];
              }
              if (str2.Contains("U_Socket"))
                this.U2ButtonVisible = true;
              if (str2.Contains("P_Socket"))
                this.PButtonVisible = true;
              if (str2.Contains("I_Socket"))
                this.I2ButtonVisible = true;
              if (str2.Contains("I_Clamp"))
                this.I1ButtonVisible = true;
              if (str2.Contains("IDIFF_Clamp"))
              {
                title = CATMeasurementTitle.ST_IDIFF_CLAMP;
                this.IDiffCalButtonVisible = true;
              }
              if (str2.Contains("IDIFF_Socket"))
              {
                title = CATMeasurementTitle.ST_IDIFF_SOCKET;
                this.IDiffButtonVisible = true;
              }
              if (str2.Contains("IA_Probe"))
              {
                title = CATMeasurementTitle.ST_IA;
                this.ITouchButtonVisible = true;
              }
              if (str2.Contains("IEA_Socket"))
              {
                title = CATMeasurementTitle.ST_IEA;
                this.IEquivButtonVisible = true;
              }
              if (str2.Contains("RISO_Probe"))
              {
                title = CATMeasurementTitle.ST_RISO_PROBE;
                this.Rins2ButtonVisible = true;
              }
              if (str2.Contains("RISO_Socket"))
              {
                title = CATMeasurementTitle.ST_RISO_SOCKET;
                this.RinsButtonVisible = true;
              }
              if (str2.Contains("RPE_Loop"))
              {
                title = CATMeasurementTitle.ST_SL1;
                this.RpeButtonVisible = true;
              }
              if (str2.Contains("RPE_Socket"))
              {
                title = CATMeasurementTitle.ST_SL;
                this.RpeCalButtonVisible = true;
              }
              if (str2.Contains("COMP"))
                this.CompButtonVisible = true;
              if (str2.Contains("batch"))
              {
                int num = int.Parse(new string(strArray2[2].Where<char>(new Func<char, bool>(char.IsDigit)).ToArray<char>()));
                this.batchlist[num] = strArray2[2].Trim();
                try
                {
                  this.batchDict.Add(num, strArray2[0]);
                }
                catch (Exception ex)
                {
                  if (ex.Message.Contains("key has already been added"))
                    return;
                }
              }
              iService5.Core.Services.Data.CATMeasurement catMeasurement = this.GetCatMeasurement(title);
              if (flag1)
                catMeasurement.min = result1;
              if (flag2)
                catMeasurement.max = result2;
              if (flag1 | flag2)
                catMeasurement.validationUnit = str1;
            }
          }
          this.batchDictSorted = this.batchDict.OrderBy<KeyValuePair<int, string>, int>((Func<KeyValuePair<int, string>, int>) (x => x.Key)).ToDictionary<KeyValuePair<int, string>, int, string>((Func<KeyValuePair<int, string>, int>) (kvp => kvp.Key), (Func<KeyValuePair<int, string>, string>) (kvp => kvp.Value));
          this.batchMaxNumber = this.batchDictSorted.Count;
          if (this.BatchListIsEligible(this.batchlist))
            this.BatchButtonVisible = true;
          else
            this.BatchButtonVisible = false;
        }
        else
          this.FileNotFound();
      }
      else
        this.FileNotFound();
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppError(LoggingContext.MEASURE, "Exception while fetching available measurements" + ex.Message, memberName: nameof (GetAvailableMeasurements), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/MeasureViewModel.cs", sourceLineNumber: 2710);
    }
  }

  private bool BatchListIsEligible(List<string> _batchlist)
  {
    List<string> list = _batchlist.Where<string>((Func<string, bool>) (s => s != string.Empty)).ToList<string>();
    int lastIndex = _batchlist.FindLastIndex((Predicate<string>) (x => x != string.Empty));
    for (int index = 0; index < lastIndex + 1; ++index)
    {
      if (list[index] != this.batchlistMetron[index])
        return false;
    }
    return true;
  }

  private void CheckListForConinuity(List<string> batchlist) => throw new NotImplementedException();

  public void FileNotFound()
  {
    this.CompButtonVisible = true;
    this.U1ButtonVisible = true;
    this.TestButtonVisible = true;
    this.U2ButtonVisible = true;
    this.StandByButtonVisible = true;
    this.PButtonVisible = true;
    this.I1ButtonVisible = true;
    this.I2ButtonVisible = true;
    this.IDiffCalButtonVisible = true;
    this.IDiffButtonVisible = true;
    this.ITouchButtonVisible = true;
    this.IEquivButtonVisible = true;
    this.Rins2ButtonVisible = true;
    this.RinsButtonVisible = true;
    this.RpeButtonVisible = true;
    this.RpeCalButtonVisible = true;
  }

  public void OnCanvasViewPaintInstructions(Assembly assembly, SKPaintSurfaceEventArgs args)
  {
    this.InstructionsVisible = false;
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
      SKMatrix scale = SKMatrix.CreateScale((float) info.Width / 351f, (float) info.Height / 250f);
      canvas.DrawPicture(skSvg.Picture, ref scale);
      this.InstructionsVisible = true;
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppError(LoggingContext.METADATA, "Failed to use svg file" + this.SvgFilename, ex, nameof (OnCanvasViewPaintInstructions), "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/MeasureViewModel.cs", 2773);
    }
  }

  public void RecordButtonTapped()
  {
    this.DiagramTitle = string.IsNullOrEmpty(this.DiagramUnit) ? $"{this.RepairEnumber} {this.MeasurementName} Diagram" : $"{this.MeasurementName} Diagram ({this.DiagramUnit})";
    this.EnableSaveButton();
    if (this.BlackPanelVisible)
    {
      this.BlackPanelVisible = false;
      this.BlackPanelUsed = true;
    }
    else
      this.BlackPanelUsed = false;
    DateTime now = DateTime.Now;
    if (this.StopRecording != DateTime.MinValue)
      this.Counter += (now - this.StopRecording).Seconds;
    this.StartRecordingButtonDisabled = true;
    this.Recording = true;
    this.DiagramVisible = true;
  }

  public async Task StopButtonTapped()
  {
    this.Recording = false;
    this.StopRecording = DateTime.Now;
    await Task.Delay(1100);
    this.StartRecordingButtonDisabled = false;
  }

  public void PopulateChart(List<DataPoint> entries)
  {
    List<DataPoint> dataPointList = entries;
    LineSeries lineSeries = new LineSeries();
    lineSeries.ItemsSource = (IEnumerable) dataPointList;
    LinearAxis linearAxis1 = new LinearAxis();
    linearAxis1.MajorGridlineStyle = LineStyle.Solid;
    linearAxis1.MinorGridlineStyle = LineStyle.Dot;
    linearAxis1.Position = AxisPosition.Bottom;
    linearAxis1.Title = AppResource.MEASURE_DIAGRAM_X_AXIS;
    LinearAxis linearAxis2 = new LinearAxis();
    linearAxis2.MajorGridlineStyle = LineStyle.Solid;
    linearAxis2.MinorGridlineStyle = LineStyle.Dot;
    linearAxis2.Title = this.DiagramUnit;
    this.Chart = new PlotModel();
    this.Chart.Axes.Add((Axis) linearAxis1);
    this.Chart.Axes.Add((Axis) linearAxis2);
    this.Chart.Series.Add((OxyPlot.Series.Series) lineSeries);
    this.Chart.TouchStarted += (EventHandler<OxyTouchEventArgs>) (async (a, b) => await this.StopButtonTapped());
    this.Chart.InvalidatePlot(true);
  }

  public async Task SaveToGallery(Stream stream)
  {
    string[] strArray = new string[8];
    DateTime now = DateTime.Now;
    strArray[0] = now.ToString("yyyyMMdd");
    strArray[1] = "_";
    strArray[2] = this.RepairEnumber.Replace('/', '-');
    strArray[3] = "_";
    strArray[4] = this.MeasurementTitle.Replace(' ', '-');
    strArray[5] = "_";
    now = DateTime.Now;
    strArray[6] = now.ToString("HHmmss");
    strArray[7] = ".jpg";
    string fileName = string.Concat(strArray);
    MemoryStream memoryStream = new MemoryStream();
    stream.CopyTo((Stream) memoryStream);
    byte[] imageArray = memoryStream.ToArray();
    PermissionStatus status = await Permissions.RequestAsync<Permissions.StorageWrite>();
    if (status == PermissionStatus.Granted)
    {
      this._platform.SaveImageFromByte(imageArray, fileName);
      await this._alert.ShowMessageAlertWithKey("SAVE_GRAPH_ALERT_MESSAGE", AppResource.SAVE_GRAPH_ALERT_TITLE);
      fileName = (string) null;
      memoryStream = (MemoryStream) null;
      imageArray = (byte[]) null;
    }
    else
    {
      await this._alert.ShowMessageAlertWithKey("SAVE_GRAPH_PERMISSION_DENIED_ALERT_MESSAGE", AppResource.SAVE_GRAPH_ALERT_TITLE);
      fileName = (string) null;
      memoryStream = (MemoryStream) null;
      imageArray = (byte[]) null;
    }
  }

  public void EnableSaveButton() => this.SaveButtonDisabled = false;

  public void DisableSaveButton() => this.SaveButtonDisabled = true;

  public double GetPlotValue(string text)
  {
    double result;
    return double.TryParse(new string(text.Where<char>((Func<char, bool>) (x => char.IsDigit(x) || x.Equals('.'))).ToArray<char>()), out result) ? result : 0.0;
  }

  public string testItouch_result
  {
    get => this._testItouch_result;
    set
    {
      this._testItouch_result = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.testItouch_result));
    }
  }

  public string testItouch_peak
  {
    get => this._testItouch_peak;
    internal set
    {
      this._testItouch_peak = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.testItouch_peak));
    }
  }

  public string testItouch_limit
  {
    get => this._testItouch_limit;
    internal set
    {
      this._testItouch_limit = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.testItouch_limit));
    }
  }

  public string testItouch_ok
  {
    get => this._testItouch_ok;
    internal set
    {
      this._testItouch_ok = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.testItouch_ok));
    }
  }

  public string testIdiff_result
  {
    get => this._testIdiff_result;
    internal set
    {
      this._testIdiff_result = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.testIdiff_result));
    }
  }

  public string testIdiff_peak
  {
    get => this._testIdiff_peak;
    internal set
    {
      this._testIdiff_peak = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.testIdiff_peak));
    }
  }

  public string testIdiff_limit
  {
    get => this._testIdiff_limit;
    internal set
    {
      this._testIdiff_limit = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.testIdiff_limit));
    }
  }

  public string testIdiff_ok
  {
    get => this._testIdiff_ok;
    internal set
    {
      this._testIdiff_ok = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.testIdiff_ok));
    }
  }

  public string testU_result
  {
    get => this._testU_result;
    internal set
    {
      this._testU_result = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.testU_result));
    }
  }

  public string testU_peak
  {
    get => this._testU_peak;
    internal set
    {
      this._testU_peak = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.testU_peak));
    }
  }

  public string testP_result
  {
    get => this._testP_result;
    internal set
    {
      this._testP_result = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.testP_result));
    }
  }

  public string testP_peak
  {
    get => this._testP_peak;
    internal set
    {
      this._testP_peak = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.testP_peak));
    }
  }

  public string testI_result
  {
    get => this._testI_result;
    internal set
    {
      this._testI_result = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.testI_result));
    }
  }

  public string testI_peak
  {
    get => this._testI_peak;
    internal set
    {
      this._testI_peak = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.testI_peak));
    }
  }

  public bool valueRetreivalTaskStarted { get; private set; }

  public CATMeasurementTitle currentMeasurement { get; private set; }

  public bool nextbatch { get; private set; }

  public bool assesment { get; private set; }

  public string PolarityButtonSource
  {
    get => this._PolarityButtonSource;
    set
    {
      this._PolarityButtonSource = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.PolarityButtonSource));
    }
  }

  public string MesaurementTitle
  {
    get => this._MesaurementTitle;
    set
    {
      this._MesaurementTitle = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.MesaurementTitle));
    }
  }

  public string ValidationTitle
  {
    get => this._ValidationTitle;
    set
    {
      this._ValidationTitle = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.ValidationTitle));
    }
  }

  public string ValueTitle
  {
    get => this._ValueTitle;
    set
    {
      this._ValueTitle = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.ValueTitle));
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

  public bool noenumber { get; private set; }

  public class MeasurementResultsP
  {
    public CATMeasurementTitle _CATMeasurementTitle { get; set; }

    public bool? _valresult { get; set; }

    public string _mestext { get; set; }
  }

  public class AssesmentResult
  {
    private string _Name;
    private string _mname;

    public string Name
    {
      get => this._Name;
      set
      {
        this._Name = value;
        this.mName = this._Name;
      }
    }

    public string mName
    {
      get => this._mname;
      set => this._mname = value;
    }

    public bool? Status { get; set; }

    public string LastValue { get; set; }
  }
}
