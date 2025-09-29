// Decompiled with JetBrains decompiler
// Type: iService5.Core.ViewModels.ErrorDetailViewModel
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
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
//using Xamarin.Forms;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

#nullable disable
namespace iService5.Core.ViewModels;

public class ErrorDetailViewModel : MvxViewModel<DetailNavigationArgs>
{
  private readonly IMvxNavigationService _navigationService;
  private readonly IAppliance _appliance;
  private const string UNDERLINE = "Underline";
  private const string NONE = "None";
  private readonly IPlatformSpecificServiceLocator _locator;
  internal ErrorLogViewModel.ErrorEntry errorDetails;
  private string _RepairEnumber;
  private string _NavText = AppResource.APPLIANCE_FLASH_BACK_TEXT;
  private string _WifiStatus;
  private string _ConnectedColor = "Green";
  private string dateTimeHeader = AppResource.ERROR_DETAIL_DATE_TIME_LABEL;
  private string errorCodeHeader = AppResource.ERROR_LOG_EC_LABEL;
  private string adviceHeader = AppResource.ERROR_DETAIL_ADVICE_LABEL;
  private string dateTime = "";
  private string errorCode = "";
  private string _Advice = "";

  public virtual async Task Initialize() => await base.Initialize();

  public bool _viewstatus { get; set; }

  public ErrorDetailViewModel(
    IMvxNavigationService navigationService,
    IShortTextsService _SService,
    ILoggingService loggingService,
    IMetadataService metadataService,
    IPlatformSpecificServiceLocator locator,
    IUserSession userSession,
    IAppliance appliance)
  {
    this._RepairEnumber = userSession.getEnumberSession();
    this._appliance = appliance;
    this.GoToErrorLogPage = (ICommand) new Command(new Action(this.VisitErrorLogPage));
    this._navigationService = navigationService;
    this._locator = locator;
    this._viewstatus = false;
    MessagingCenter.Subscribe<Application>((object) this, CoreApp.EventsNames.ForegroundEvent.ToString(), (Action<Application>) (sender => this._locator.GetPlatformSpecificService().GetLocationConsent()), (Application) null);
  }

  private void VisitErrorLogPage()
  {
    this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
  }

  public ICommand GoToErrorLogPage { protected set; get; }

  public string RepairEnumber
  {
    get => this._RepairEnumber;
    internal set
    {
      this._RepairEnumber = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.RepairEnumber));
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
    private set
    {
      this._WifiStatus = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.WifiStatus));
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

  public string DateTimeHeader
  {
    get => this.dateTimeHeader;
    internal set
    {
      this.dateTimeHeader = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.DateTimeHeader));
    }
  }

  public string ErrorCodeHeader
  {
    get => this.errorCodeHeader;
    internal set
    {
      this.errorCodeHeader = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.ErrorCodeHeader));
    }
  }

  public string AdviceHeader
  {
    get => this.adviceHeader;
    internal set
    {
      this.adviceHeader = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.AdviceHeader));
    }
  }

  public string DateTime
  {
    get => this.dateTime;
    internal set
    {
      this.dateTime = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.DateTime));
    }
  }

  public string ErrorCode
  {
    get => this.errorCode;
    internal set
    {
      this.errorCode = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.ErrorCode));
    }
  }

  public string Advice
  {
    get => this._Advice;
    internal set
    {
      this._Advice = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.Advice));
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
    this.DateTime = this.errorDetails.timestamp.Replace(Environment.NewLine, "  ");
    this.Advice = this.errorDetails.pcsErrorDescription;
    this.ErrorCode = this.errorDetails.pcsError.Replace(Environment.NewLine, "  ");
  }

  public override void ViewAppeared()
  {
    base.ViewAppeared();
    this._viewstatus = true;
    this._locator.GetPlatformSpecificService().GetLocationConsent();
    this._appliance.CheckConnectivity(new connectivityUpdateCallback(this.updateStatusBr));
  }

  internal void updateStatusBr()
  {
    this.WifiStatus = this._appliance.StatusOfConnection;
    this.ConnectedColor = this._appliance.ConnectedColor;
  }

  public override void Prepare(DetailNavigationArgs parameter)
  {
    this.errorDetails = parameter.errorDetails;
  }
}
