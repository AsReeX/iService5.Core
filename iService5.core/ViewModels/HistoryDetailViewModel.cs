// Decompiled with JetBrains decompiler
// Type: iService5.Core.ViewModels.HistoryDetailViewModel
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Appliance;
using iService5.Core.Services.Data;
using iService5.Core.Services.Helpers;
using iService5.Core.Services.Localisation;
using iService5.Core.Services.User;
using iService5.Core.Services.User.ActivityLogging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
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

public class HistoryDetailViewModel : MvxViewModel<DetailNavigationArgs>
{
  private readonly IMvxNavigationService _navigationService;
  private readonly IUserSession _userSession;
  private readonly IMetadataService _metadataService;
  private readonly IAppliance _appliance;
  internal string historySessionID;
  internal List<History> historyDetailsList;
  private string _HeaderLabel = AppResource.TAB_TITLE_HISTORY;
  private string _ENumber;
  private string _NavText = AppResource.APPLIANCE_FLASH_BACK_TEXT;
  private string _eNoTextColor = "#808080";
  private string _ApplianceData;
  private bool _IsHAInfo = true;
  private string _RISData = AppResource.NOT_YET_AVAILABLE;
  private string _DateTimeData;
  private string _ErrorLogData;
  private string _MemoryLogData;
  private string _MonitoringLogData;
  private string _CodingLogData;
  private string _MeasureLogData;
  private bool _IsErrorLog = true;
  private bool _IsMemoryLog = true;
  private bool _IsMonitoringLog = true;
  private bool _IsCodingLog = true;
  private bool _IsMeasureLog = true;
  private string _ProgramData;
  private string _FlashData;
  private bool _IsProgramLog = true;
  private bool _IsFlashLog = true;

  public ICommand GoToHistoryPage { internal set; get; }

  public virtual async Task Initialize()
  {
    await base.Initialize();
    this.historyDetailsList = CoreApp.history.GetHistoryDetails(this.historySessionID);
    this.ENumber = this.historyDetailsList[0].eNumber;
    string timeStampInString = this.historyDetailsList[0].timestamp.ToString((IFormatProvider) UtilityFunctions.GetCurrentCulture());
    this.DateTimeData = timeStampInString;
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
    UtilityFunctions.SetHistoryStringBuilders(hsbs, this.historyDetailsList);
    this.setHAInfoData(hsbs.haInfoSB);
    this.setErrorLogData(hsbs.errorLogSB);
    this.SetupProgramData(hsbs.programLogSB);
    this.setMeasureLogData(hsbs.measureLogSB);
    this.setMonitoringLogData(hsbs.monitoringLogSB);
    this.setCodingLogData(hsbs.codingLogSB);
    this.setFlashLogData(hsbs.flashLogSB);
    this.setMemoryLogData(hsbs.memoryLogSB);
    timeStampInString = (string) null;
    hsbs = (UtilityFunctions.HistoryStringBuilders) null;
  }

  internal void setHAInfoData(StringBuilder haInfoSB)
  {
    if (haInfoSB == null)
      this.IsHAInfo = false;
    else
      this.ApplianceData = haInfoSB.ToString();
  }

  internal void setErrorLogData(StringBuilder errorLogSB)
  {
    if (errorLogSB == null)
      this.IsErrorLog = false;
    else
      this.ErrorLogData = $"{AppResource.ERROR_LOG_TIMESTAMP_POPUPSTRING}\n\n{errorLogSB}";
  }

  internal void setMemoryLogData(StringBuilder memoryLogSB)
  {
    if (memoryLogSB == null)
      this.IsMemoryLog = false;
    else
      this.MemoryLogData = memoryLogSB.ToString();
  }

  internal void setMonitoringLogData(StringBuilder monitoringLogSB)
  {
    if (monitoringLogSB == null)
      this.IsMonitoringLog = false;
    else
      this.MonitoringLogData = monitoringLogSB.ToString();
  }

  internal void setCodingLogData(StringBuilder codingLogSB)
  {
    if (codingLogSB == null)
      this.IsCodingLog = false;
    else
      this.CodingLogData = codingLogSB.ToString();
  }

  internal void setMeasureLogData(StringBuilder measureLogSB)
  {
    if (measureLogSB == null)
      this.IsMeasureLog = false;
    else
      this.MeasureLogData = measureLogSB.ToString();
  }

  internal void SetupProgramData(StringBuilder programLogSB)
  {
    if (programLogSB == null)
      this.IsProgramLog = false;
    else
      this.ProgramData = programLogSB.ToString();
  }

  internal void setFlashLogData(StringBuilder flashLogSB)
  {
    if (flashLogSB == null)
      this.IsFlashLog = false;
    else
      this.FlashData = flashLogSB.ToString();
  }

  public IApplianceSession _session { get; }

  public HistoryDetailViewModel(
    IMvxNavigationService navigationService,
    IShortTextsService _SService,
    ILoggingService loggingService,
    IMetadataService metadataService,
    IUserSession userSession,
    IAppliance appliance,
    IApplianceSession session)
  {
    this._userSession = userSession;
    this._metadataService = metadataService;
    this._appliance = appliance;
    this.GoToHistoryPage = (ICommand) new Command(new Action(this.VisitHistoryLogPage));
    this._navigationService = navigationService;
    this._session = session;
  }

  internal void VisitHistoryLogPage()
  {
    this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
  }

  public string HeaderLabel
  {
    get => this._HeaderLabel;
    internal set
    {
      this._HeaderLabel = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.HeaderLabel));
    }
  }

  public string ENumber
  {
    get => this._ENumber;
    internal set
    {
      this._ENumber = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.ENumber));
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

  public string eNoTextColor
  {
    get => this._eNoTextColor;
    internal set
    {
      this._eNoTextColor = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.eNoTextColor));
    }
  }

  public string ApplianceData
  {
    get => this._ApplianceData;
    internal set
    {
      this._ApplianceData = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.ApplianceData));
    }
  }

  public bool IsHAInfo
  {
    get => this._IsHAInfo;
    internal set
    {
      this._IsHAInfo = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.IsHAInfo));
    }
  }

  public string RISData
  {
    get => this._RISData;
    internal set
    {
      this._RISData = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.RISData));
    }
  }

  public string DateTimeData
  {
    get => this._DateTimeData;
    internal set
    {
      this._DateTimeData = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.DateTimeData));
    }
  }

  public string ErrorLogData
  {
    get => this._ErrorLogData;
    internal set
    {
      this._ErrorLogData = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.ErrorLogData));
    }
  }

  public string MemoryLogData
  {
    get => this._MemoryLogData;
    internal set
    {
      this._MemoryLogData = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.MemoryLogData));
    }
  }

  public string MonitoringLogData
  {
    get => this._MonitoringLogData;
    internal set
    {
      this._MonitoringLogData = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.MonitoringLogData));
    }
  }

  public string CodingLogData
  {
    get => this._CodingLogData;
    internal set
    {
      this._CodingLogData = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.CodingLogData));
    }
  }

  public string MeasureLogData
  {
    get => this._MeasureLogData;
    internal set
    {
      this._MeasureLogData = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.MeasureLogData));
    }
  }

  public bool IsErrorLog
  {
    get => this._IsErrorLog;
    internal set
    {
      this._IsErrorLog = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.IsErrorLog));
    }
  }

  public bool IsMemoryLog
  {
    get => this._IsMemoryLog;
    internal set
    {
      this._IsMemoryLog = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.IsMemoryLog));
    }
  }

  public bool IsMonitoringLog
  {
    get => this._IsMonitoringLog;
    internal set
    {
      this._IsMonitoringLog = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.IsMonitoringLog));
    }
  }

  public bool IsCodingLog
  {
    get => this._IsCodingLog;
    internal set
    {
      this._IsCodingLog = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.IsCodingLog));
    }
  }

  public bool IsMeasureLog
  {
    get => this._IsMeasureLog;
    internal set
    {
      this._IsMeasureLog = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.IsMeasureLog));
    }
  }

  public string ProgramData
  {
    get => this._ProgramData;
    internal set
    {
      this._ProgramData = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.ProgramData));
    }
  }

  public string FlashData
  {
    get => this._FlashData;
    internal set
    {
      this._FlashData = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.FlashData));
    }
  }

  public bool IsProgramLog
  {
    get => this._IsProgramLog;
    internal set
    {
      this._IsProgramLog = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.IsProgramLog));
    }
  }

  public bool IsFlashLog
  {
    get => this._IsFlashLog;
    internal set
    {
      this._IsFlashLog = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.IsFlashLog));
    }
  }

  public override void ViewAppeared() => base.ViewAppeared();

  public override void Prepare(DetailNavigationArgs parameter)
  {
    this.historySessionID = parameter.historySessionID;
  }
}
