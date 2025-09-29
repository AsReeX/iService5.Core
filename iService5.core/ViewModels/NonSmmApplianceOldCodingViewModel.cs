// Decompiled with JetBrains decompiler
// Type: iService5.Core.ViewModels.NonSmmApplianceOldCodingViewModel
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
using System;
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

public class NonSmmApplianceOldCodingViewModel : MvxViewModel<varcodes>
{
  private readonly IMvxNavigationService _navigationService;
  private readonly ILoggingService _loggingService;
  private readonly IPlatformSpecificServiceLocator _locator;
  private readonly IAlertService _alertService;
  private readonly IAppliance _appliance;
  private readonly Is5SshWrapper _sshWrapper;
  internal StringBuilder historySb;
  private string _RepairEnumber;
  private string _Headertitle = AppResource.HOME_CONNECT_HEADER;
  private string _NavText = AppResource.APPLIANCE_FLASH_BACK_TEXT;
  private string _WifiStatus = AppResource.CONNECTED_TEXT;
  private string _ConnectedColor = "Green";
  private bool _IsBtnDisabled = true;
  private string _WriteButtonLabel = "Write";
  private string _ResetButtonLabel = "Reset";
  private varcodes SelectedCodes;

  public virtual async Task Initialize()
  {
    await base.Initialize();
    this._appliance.CheckConnectivity(new connectivityUpdateCallback(this.UpdateStatus));
  }

  public override void Prepare(varcodes parameter) => this.SelectedCodes = parameter;

  public bool _viewstatus { get; set; }

  public NonSmmApplianceOldCodingViewModel(
    IMvxNavigationService navigationService,
    IShortTextsService _SService,
    ILoggingService loggingService,
    IMetadataService metadataService,
    IPlatformSpecificServiceLocator locator,
    IUserSession userSession,
    IAlertService alertService,
    IAppliance appliance,
    Is5SshWrapper sshWrapper)
  {
    this.ReturnToVarCoding = (ICommand) new Command(new Action<object>(this.ReturnToVarCodingFunction));
    this.WriteCodes = (ICommand) new Command((Action) (() => this.write(false)));
    this.ResetCodes = (ICommand) new Command((Action) (() => this.write(true)));
    this._navigationService = navigationService;
    this._loggingService = loggingService;
    this._locator = locator;
    this._alertService = alertService;
    this._appliance = appliance;
    this.RepairEnumber = userSession.getEnumberSession();
    this.IsBtnDisabled = false;
    this._sshWrapper = sshWrapper;
    this.historySb = new StringBuilder();
  }

  public ICommand WriteCodes { internal set; get; }

  public ICommand ResetCodes { internal set; get; }

  public void write(bool reset)
  {
    Task.Factory.StartNew((Action) (() =>
    {
      this.IsBtnDisabled = true;
      Thread.Sleep(1000);
      this._sshWrapper.IPAddress = this._locator.GetPlatformSpecificService().GetIp();
      try
      {
        SshResponse json = this._sshWrapper.UploadFieldsToJson((reset ? "RESET " : "WRITE ") + this.SelectedCodes._enumber, (iService5.Ssh.models.ProgressCallback) ((progress, step, total) => { }));
        this.historySb.AppendLine(reset ? "RESET" : "WRITE");
        if (json.Success)
        {
          this._alertService.ShowMessageAlertWithKeyFromService("CODING_SUCCESSFUL", "", (Action) (() => { }));
          this._loggingService.getLogger().LogAppDebug(LoggingContext.BACKEND, "Successfully sent codes", memberName: nameof (write), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/NonSmmApplianceOldCodingViewModel.cs", sourceLineNumber: 94);
          this.historySb.AppendLine(AppResource.CODING_SUCCESSFUL);
        }
        else
        {
          this._alertService.ShowMessageAlertWithKeyFromService("CODING_FAILED", "", (Action) (() => { }));
          this._loggingService.getLogger().LogAppDebug(LoggingContext.BACKEND, "Error sending codes", memberName: nameof (write), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/NonSmmApplianceOldCodingViewModel.cs", sourceLineNumber: 102);
          this.historySb.AppendLine(AppResource.CODING_FAILED);
        }
      }
      catch (Exception ex)
      {
        this._loggingService.getLogger().LogAppDebug(LoggingContext.BACKEND, "error sending codes:  " + ex.Message, memberName: nameof (write), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/NonSmmApplianceOldCodingViewModel.cs", sourceLineNumber: 108);
        this.historySb.AppendLine(AppResource.CODING_ERROR);
      }
      this.IsBtnDisabled = false;
      try
      {
        this.historySb.AppendLine();
        CoreApp.history.SaveItem(new History(this.RepairEnumber, DateTime.Now, UtilityFunctions.getSessionIDForHistoryTable(), HistoryDBInfoType.CodingLog.ToString(), this.historySb.ToString()));
      }
      catch (Exception ex)
      {
        this._loggingService.getLogger().LogAppInformation(LoggingContext.CODING, "Failed to save item in the History DB, " + ex?.ToString(), memberName: nameof (write), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/NonSmmApplianceOldCodingViewModel.cs", sourceLineNumber: 120);
      }
      this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
    }));
  }

  private void ReturnToVarCodingFunction(object obj)
  {
    if (this.IsBtnDisabled)
      return;
    this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
  }

  public ICommand ReturnToVarCoding { protected set; get; }

  public string RepairEnumber
  {
    get => this._RepairEnumber;
    internal set
    {
      this._RepairEnumber = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.RepairEnumber));
    }
  }

  public string Headertitle
  {
    get => this._Headertitle;
    internal set
    {
      this._Headertitle = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.Headertitle));
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

  public string WifiStatus
  {
    get => this._WifiStatus;
    internal set
    {
      this._WifiStatus = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.WifiStatus));
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

  public bool IsBtnDisabled
  {
    protected set
    {
      this._IsBtnDisabled = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.IsBtnDisabled));
    }
    get => this._IsBtnDisabled;
  }

  public string WriteButtonLabel
  {
    get => this._WriteButtonLabel;
    private set
    {
      this._WriteButtonLabel = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.WriteButtonLabel));
    }
  }

  public string ResetButtonLabel
  {
    get => this._ResetButtonLabel;
    private set
    {
      this._ResetButtonLabel = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.ResetButtonLabel));
    }
  }

  public override void ViewAppeared()
  {
    base.ViewAppeared();
    this._viewstatus = true;
    this.IsBtnDisabled = false;
    this._appliance.CheckConnectivity(new connectivityUpdateCallback(this.UpdateStatus));
  }

  internal void UpdateStatus()
  {
    this.WifiStatus = this._appliance.StatusOfBridgeConnection;
    this.ConnectedColor = this._appliance.ConnectedColorBridge;
  }
}
