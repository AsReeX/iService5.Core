// Decompiled with JetBrains decompiler
// Type: iService5.Core.ViewModels.VarCodingViewModel
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
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
//using Xamarin.Forms;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

#nullable disable
namespace iService5.Core.ViewModels;

public class VarCodingViewModel : MvxViewModel
{
  private readonly IMvxNavigationService _navigationService;
  private readonly IMetadataService _metadataService;
  private readonly ILoggingService _loggingService;
  private readonly IPlatformSpecificServiceLocator _locator;
  private readonly IAlertService _alertService;
  private readonly IAppliance _appliance;
  private readonly Is5SshWrapper _sshWrapper;
  internal bool _displayMemoryLog = false;
  private Dictionary<string, varcodes> codingList;
  private string _RepairEnumber;
  private string _DescriptionLabel = AppResource.VARCODING_OPTIONS_LABEL;
  private CodingItem _ObjItemSelected;
  private List<CodingItem> _VarCodingOptionsList = new List<CodingItem>();
  private string _WifiStatus = AppResource.CONNECTED_TEXT;
  private string _NavText = AppResource.APPLIANCE_FLASH_BACK_TEXT;
  private bool _CanNavigate = false;
  private string _ErrorLogArea = "";
  private string _ConnectedColor = "Green";
  private string _RightChevronSymbol = "\uF054";
  private bool _areButtonsEnabled = true;

  public virtual async Task Initialize()
  {
    await base.Initialize();
    SshResponse<ApplianceCodingDto> PrepareCodingResponse = this._sshWrapper.PrepareCoding();
    this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, $"Prepare Coding Response for RepairEnumber {this._RepairEnumber} is : {JsonConvert.SerializeObject((object) PrepareCodingResponse)}", memberName: nameof (Initialize), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/VarCodingViewModel.cs", sourceLineNumber: 31 /*0x1F*/);
    try
    {
      foreach (CodingItem _codingItem in PrepareCodingResponse.Response.CodingItems)
      {
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Adding coding item", memberName: nameof (Initialize), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/VarCodingViewModel.cs", sourceLineNumber: 36);
        _codingItem.Name = this.GetShortText(_codingItem.Name);
        this.VarCodingOptionsList.Add(_codingItem);
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "VarCodingOptionsList :" + JsonConvert.SerializeObject((object) this.VarCodingOptionsList), memberName: nameof (Initialize), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/VarCodingViewModel.cs", sourceLineNumber: 39);
      }
      IEnumerable<varcodes> _codingList = this.GetCodingList(this._RepairEnumber);
      if (_codingList.Any<varcodes>())
      {
        this.codingList = new Dictionary<string, varcodes>();
        foreach (varcodes _varcode in _codingList)
        {
          string oldVarCodingEnumber = _varcode._enumber;
          this.VarCodingOptionsList.Add(new CodingItem()
          {
            Name = "Set parameter " + oldVarCodingEnumber,
            Enumber = oldVarCodingEnumber
          });
          this.codingList.Add(oldVarCodingEnumber, _varcode);
          oldVarCodingEnumber = (string) null;
        }
      }
      _codingList = (IEnumerable<varcodes>) null;
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppError(LoggingContext.APPLIANCE, "Failed with " + ex.Message, ex, nameof (Initialize), "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/VarCodingViewModel.cs", 56);
      await this._alertService.ShowMessageAlertWithKey("CODING_FAILED_TO_INITIALIZE", AppResource.ERROR_TITLE);
    }
    PrepareCodingResponse = (SshResponse<ApplianceCodingDto>) null;
  }

  public bool _viewstatus { get; set; }

  public ICommand OpenErrorDetails { internal set; get; }

  public VarCodingViewModel(
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
    this.ReturnToNonSMM = (ICommand) new Command(new Action(this.ReturnToNonSMMFunction));
    this._navigationService = navigationService;
    this._loggingService = loggingService;
    this._metadataService = metadataService;
    this._locator = locator;
    this._alertService = alertService;
    this._appliance = appliance;
    this._RepairEnumber = userSession.getEnumberSession();
    this._sshWrapper = sshWrapper;
  }

  private IEnumerable<varcodes> GetCodingList(string enumber)
  {
    return this._metadataService.getOldVarCodingStatus(enumber);
  }

  private string GetShortText(string fieldName)
  {
    string lowerInvariant = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToLowerInvariant();
    string[] strArray = fieldName.Split(',', ';');
    try
    {
      return (this._metadataService.getShortText(strArray[0], lowerInvariant) != null ? this._metadataService.getShortText(strArray[0], lowerInvariant) + (strArray.Length > 1 ? "," : "") : (strArray.Length > 1 ? "" : strArray[0])) + (strArray.Length > 1 ? strArray[1] : "");
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

  public CodingItem ObjItemSelected
  {
    get => this._ObjItemSelected;
    set
    {
      if (this._ObjItemSelected != value)
      {
        this._ObjItemSelected = value;
        if (value.Name.Contains("Set"))
        {
          Device.BeginInvokeOnMainThread((Action) (() =>
          {
            if (!this.AreButtonsEnabled)
              return;
            this.AreButtonsEnabled = false;
            try
            {
              this._navigationService.Navigate<NonSmmApplianceOldCodingViewModel, varcodes>(this.codingList[value.Enumber], (IMvxBundle) null, new CancellationToken());
            }
            catch (Exception ex)
            {
              this._loggingService.getLogger().LogAppDebug(LoggingContext.BACKEND, "cant navigate" + ex.Message, memberName: nameof (ObjItemSelected), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/VarCodingViewModel.cs", sourceLineNumber: 196);
            }
          }));
          this._ObjItemSelected = (CodingItem) null;
        }
        else
        {
          Device.BeginInvokeOnMainThread((Action) (() =>
          {
            if (!this.AreButtonsEnabled)
              return;
            this.AreButtonsEnabled = false;
            try
            {
              this._navigationService.Navigate<NonSmmCodingTransitionViewModel, CodingParameter>(new CodingParameter()
              {
                CodingItem = value,
                fromFlashing = false
              }, (IMvxBundle) null, new CancellationToken());
            }
            catch (Exception ex)
            {
              this._loggingService.getLogger().LogAppDebug(LoggingContext.BACKEND, "cant navigate" + ex.Message, memberName: nameof (ObjItemSelected), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/VarCodingViewModel.cs", sourceLineNumber: 221);
            }
          }));
          this._ObjItemSelected = (CodingItem) null;
        }
      }
      this.RaisePropertyChanged<CodingItem>((Expression<Func<CodingItem>>) (() => this.ObjItemSelected));
    }
  }

  public List<CodingItem> VarCodingOptionsList
  {
    get => this._VarCodingOptionsList;
    internal set
    {
      this._VarCodingOptionsList = value;
      this.RaisePropertyChanged<List<CodingItem>>((Expression<Func<List<CodingItem>>>) (() => this.VarCodingOptionsList));
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

  public bool CanNavigate
  {
    get => this._CanNavigate;
    internal set
    {
      this._CanNavigate = value;
      if (!this._CanNavigate)
        return;
      this._navigationService.Navigate<NonSmmApplianceOldCodingViewModel>((IMvxBundle) null, new CancellationToken());
    }
  }

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

  public string RightChevronSymbol
  {
    get => this._RightChevronSymbol;
    private set
    {
      this._RightChevronSymbol = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.RightChevronSymbol));
    }
  }

  private void ReturnToNonSMMFunction()
  {
    this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
  }

  public ICommand ReturnToNonSMM { protected set; get; }

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

  public override void ViewAppeared()
  {
    base.ViewAppeared();
    this.AreButtonsEnabled = true;
    this._appliance.CheckConnectivity(new connectivityUpdateCallback(this.updateStatus));
    this._viewstatus = true;
  }

  internal void updateStatus()
  {
    this.WifiStatus = this._appliance.StatusOfBridgeConnection;
    this.ConnectedColor = this._appliance.ConnectedColorBridge;
  }
}
