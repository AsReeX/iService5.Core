// Decompiled with JetBrains decompiler
// Type: iService5.Core.ViewModels.NonSMMFlasherListViewModel
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
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
//using Xamarin.Forms;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

#nullable disable
namespace iService5.Core.ViewModels;

public class NonSMMFlasherListViewModel : MvxViewModel
{
  private readonly IMvxNavigationService _navigationService;
  private readonly IMetadataService _metadataService;
  private readonly IAlertService _alertService;
  private readonly IPlatformSpecificServiceLocator _locator;
  private readonly IAppliance _appliance;
  private readonly ILoggingService _loggingService;
  private readonly string PopupString;
  private bool isBackButtonVisible = true;
  private bool isScrollDisabled = true;
  private bool _RepairLabelVisibility = true;
  private string _CheckingApplianceStatus = "";
  private string _RepairEnumber;
  private string _FlashingHeader;
  private string _BulletHeader;
  private string _StatusOfConnection;
  private string _ProgramLogLabel = AppResource.APPLIANCE_SELECT_A_FLASHER_HEADER;
  internal string _ConnectedColor = "Green";
  private string _ApplianceName = "";
  private string _ApplianceAction = "";
  private bool _OrangeBarVisibility = false;
  private string _OrangeBarText = AppResource.APPLIANCE_BOOT_MODE_RECOVERY;
  private List<FlashingItem> _ModuleList;
  private string _WifiStatus;
  private string _CancelBackText = AppResource.APPLIANCE_FLASH_BACK_TEXT;
  private FlashingItem _ItemSelected;
  private bool _areButtonsEnabled = true;

  public virtual async Task Initialize() => await base.Initialize();

  public NonSMMFlasherListViewModel(
    IMvxNavigationService navigationService,
    IShortTextsService _SService,
    ILoggingService loggingService,
    IMetadataService metadataService,
    IPlatformSpecificServiceLocator locator,
    IUserSession userSession,
    Is5SshWrapper sshWrapper,
    IAlertService alertService,
    IAppliance appliance,
    IApplianceSession session)
  {
    this._RepairEnumber = userSession.getEnumberSession();
    this._metadataService = metadataService;
    this._alertService = alertService;
    this._appliance = appliance;
    this.NavigatePreviousPage = (ICommand) new Command(new Action<object>(this.VisitPage));
    this._navigationService = navigationService;
    this._locator = locator;
    if (_SService != null)
    {
      this._loggingService = loggingService;
      this.PopupString = AppResource.FLASH_PAGE_POPUPSTRING;
    }
    SshResponse<ApplianceFlashingDto> sshResponse = sshWrapper.PrepareFlashing();
    if (!sshResponse.Success)
      this._alertService.ShowMessageAlertBoxWithMessage(AppResource.APPLIANCE_FLASH_PREPERATION_FAILED, AppResource.ERROR_TITLE);
    this.ModuleList = new List<FlashingItem>();
    foreach (FlashingItem flashingItem in sshResponse.Response.FlashingItems)
    {
      string[] strArray = this.GetShortText(flashingItem.FlashingLabel).Split(',');
      flashingItem.FlashingLabel = strArray[0];
      try
      {
        flashingItem.FlashingLabelFixed = strArray[1];
      }
      catch (Exception ex)
      {
        flashingItem.FlashingLabelFixed = "";
      }
      try
      {
        foreach (FlashingInstruction flashingInstruction in flashingItem.StartFlashingInstructions)
          flashingInstruction.InstructionText = this.GetShortText(flashingInstruction.InstructionText).Split(',')[0];
      }
      catch (NullReferenceException ex)
      {
        flashingItem.StartFlashingInstructions = new List<FlashingInstruction>();
      }
      try
      {
        foreach (FlashingInstruction flashingInstruction in flashingItem.EndFlashingInstructions)
          flashingInstruction.InstructionText = this.GetShortText(flashingInstruction.InstructionText).Split(',')[0];
      }
      catch (NullReferenceException ex)
      {
        flashingItem.EndFlashingInstructions = new List<FlashingInstruction>();
      }
      this.ModuleList.Add(flashingItem);
    }
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

  public ICommand NavigatePreviousPage { protected set; get; }

  internal void VisitPage(object obj)
  {
    this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
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

  internal void clearInstalledList() => throw new NotImplementedException();

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
      if (this._ConnectedColor.Equals("#808080"))
      {
        this.OrangeBarVisibility = false;
        this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.OrangeBarVisibility));
      }
      else
      {
        this.OrangeBarVisibility = true;
        this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.OrangeBarVisibility));
      }
    }
  }

  public string ApplianceName
  {
    get => this._ApplianceName;
    private set
    {
      this._ApplianceName = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.ApplianceName));
    }
  }

  public string ApplianceAction
  {
    get => this._ApplianceAction;
    private set
    {
      this._ApplianceAction = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.ApplianceAction));
    }
  }

  public bool OrangeBarVisibility
  {
    get => this._OrangeBarVisibility;
    internal set
    {
      this._OrangeBarVisibility = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.OrangeBarVisibility));
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

  public List<FlashingItem> ModuleList
  {
    get => this._ModuleList;
    internal set
    {
      this._ModuleList = value;
      this.RaisePropertyChanged<List<FlashingItem>>((Expression<Func<List<FlashingItem>>>) (() => this.ModuleList));
    }
  }

  public override void ViewAppeared()
  {
    base.ViewAppeared();
    this.AreButtonsEnabled = true;
    this._appliance.CheckConnectivity(new connectivityUpdateCallback(this.updateStatus));
  }

  internal void updateStatus()
  {
    this.WifiStatus = this._appliance.StatusOfBridgeConnection;
    this.ConnectedColor = this._appliance.ConnectedColorBridge;
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

  public string CancelBackText
  {
    get => this._CancelBackText;
    internal set
    {
      this._CancelBackText = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.CancelBackText));
    }
  }

  public FlashingItem ObjItemSelected
  {
    get => this._ItemSelected;
    set
    {
      if (this._ItemSelected != value)
      {
        this._ItemSelected = value;
        Device.BeginInvokeOnMainThread((Action) (() =>
        {
          if (!this.AreButtonsEnabled)
            return;
          this.AreButtonsEnabled = false;
          IMvxNavigationService navigationService = this._navigationService;
          FlashingParameter flashingParameter = new FlashingParameter();
          flashingParameter.FlashingItem = value;
          CancellationToken cancellationToken = new CancellationToken();
          navigationService.Navigate<ApplianceNonSMMFlashViewModel, FlashingParameter>(flashingParameter, (IMvxBundle) null, cancellationToken);
        }));
        this._ItemSelected = (FlashingItem) null;
      }
      this.RaisePropertyChanged<FlashingItem>((Expression<Func<FlashingItem>>) (() => this.ObjItemSelected));
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
}
