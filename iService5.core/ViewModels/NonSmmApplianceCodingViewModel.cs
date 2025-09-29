// Decompiled with JetBrains decompiler
// Type: iService5.Core.ViewModels.NonSmmApplianceCodingViewModel
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
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
//using Xamarin.Forms;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

#nullable disable
namespace iService5.Core.ViewModels;

public class NonSmmApplianceCodingViewModel : MvxViewModel<CodingParameter>
{
  private readonly IMvxNavigationService _navigationService;
  private readonly IMetadataService _metadataService;
  private readonly ILoggingService _loggingService;
  private readonly IPlatformSpecificServiceLocator _locator;
  private readonly IAlertService _alertService;
  private readonly IAppliance _appliance;
  private readonly string _repairEnumber;
  private readonly (string, string) _brandInfo;
  private readonly Is5SshWrapper _sshWrapper;
  internal StringBuilder historySb;
  internal CodingParameter Parameter;
  internal bool IsFlashingNext;
  private ObservableCollection<VariantCodingData> _varCodingOptionFieldsList;
  private bool _areButtonsEnabled = true;
  private string _Headertitle = AppResource.HOME_CONNECT_HEADER;
  private string _NavText = AppResource.APPLIANCE_FLASH_BACK_TEXT;
  private bool _DisplayImage = true;
  private string _WifiStatus = AppResource.CONNECTED_TEXT;
  private string _ConnectedColor = "Green";
  private VariantCodingData _ObjItemSelected;
  private bool _IsBtnDisabled = true;
  private string _ButtonLabel = AppResource.PROCEED_BUTTON_LABEL;
  private string _CancelButtonLabel = AppResource.CANCEL_LABEL;
  private string _HelpLabelColor;

  public virtual async Task Initialize()
  {
    await base.Initialize();
    this._appliance.CheckConnectivity(new connectivityUpdateCallback(this.UpdateStatus));
  }

  public bool _viewstatus { get; set; }

  public NonSmmApplianceCodingViewModel(
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
    this.ValidateCommand = (ICommand) new Command(new Action(this.testcom));
    this.SubmitCommand = (ICommand) new Command(new Action(this.OnSubmit));
    this._navigationService = navigationService;
    this._loggingService = loggingService;
    this._metadataService = metadataService;
    this._locator = locator;
    this._alertService = alertService;
    this._appliance = appliance;
    this._sshWrapper = sshWrapper;
    this._sshWrapper.IPAddress = this._locator.GetPlatformSpecificService().GetIp();
    this._repairEnumber = userSession.getEnumberSession();
    this._brandInfo = this._metadataService.getBrand(this._repairEnumber);
    this.historySb = new StringBuilder();
    this.IsBtnDisabled = true;
    this.VarCodingOptionFieldsList = new ObservableCollection<VariantCodingData>();
    UIDataWithValidation.OnDataValidationChange = (Action<bool>) (isValid =>
    {
      try
      {
        this.IsBtnDisabled = !isValid || this.VarCodingOptionFieldsList.Any<VariantCodingData>((Func<VariantCodingData, bool>) (x => !x.IsLocked && !x.IsValid));
      }
      catch
      {
        this.IsBtnDisabled = true;
      }
    });
  }

  public void OnSubmit()
  {
    foreach (VariantCodingData codingOptionFields in (Collection<VariantCodingData>) this.VarCodingOptionFieldsList)
    {
      if (!codingOptionFields.IsLocked)
        codingOptionFields.IsLocked = true;
    }
    this.VarCodingOptionFieldsList.Add(new VariantCodingData());
    this.VarCodingOptionFieldsList.RemoveAt(this.VarCodingOptionFieldsList.Count - 1);
    foreach (VariantCodingData codingOptionFields in (Collection<VariantCodingData>) this.VarCodingOptionFieldsList)
      this.historySb.AppendLine($"{codingOptionFields.Title}: {codingOptionFields.FieldValue}");
    List<VariantCodingDataToJson> codingDataToJsonList1 = new List<VariantCodingDataToJson>();
    VariantCodingDataToJsonList codingDataToJsonList2 = new VariantCodingDataToJsonList();
    foreach (VariantCodingData codingOptionFields in (Collection<VariantCodingData>) this.VarCodingOptionFieldsList)
      codingDataToJsonList1.Add(JsonConvert.DeserializeObject<VariantCodingDataToJson>(JsonConvert.SerializeObject((object) codingOptionFields)));
    codingDataToJsonList2.name = this.Headertitle;
    codingDataToJsonList2.userEntryFields = codingDataToJsonList1;
    string jsonToSend = JsonConvert.SerializeObject((object) codingDataToJsonList2);
    Task.Factory.StartNew((Action) (() =>
    {
      this.AreButtonsEnabled = true;
      Thread.Sleep(1000);
      try
      {
        this._loggingService.getLogger().LogAppDebug(LoggingContext.BACKEND, "try to upload json : " + jsonToSend, memberName: nameof (OnSubmit), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/NonSmmApplianceCodingViewModel.cs", sourceLineNumber: 130);
        bool showSuccessPopup = true;
        SshResponse json = this._sshWrapper.UploadFieldsToJson(jsonToSend, (iService5.Ssh.models.ProgressCallback) ((progress, step, total) =>
        {
          showSuccessPopup = false;
          FlashingItem flashingItem = (FlashingItem) null;
          try
          {
            flashingItem = JsonConvert.DeserializeObject<FlashingItem>(progress);
          }
          catch (Exception ex)
          {
            this._loggingService.getLogger().LogAppError(LoggingContext.APPLIANCE, "Error during JSON response parsing: " + ex.Message, memberName: nameof (OnSubmit), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/NonSmmApplianceCodingViewModel.cs", sourceLineNumber: 144 /*0x90*/);
          }
          int num;
          if (flashingItem == null)
          {
            num = 0;
          }
          else
          {
            FlashProgress flashProgress = flashingItem.FlashProgress;
            bool? nullable3;
            if (flashProgress == null)
            {
              nullable3 = new bool?();
            }
            else
            {
              List<CtrlMessage> ctrlMessages = flashProgress.CtrlMessages;
              nullable3 = ctrlMessages != null ? new bool?(ctrlMessages.Any<CtrlMessage>((Func<CtrlMessage, bool>) (x => x.NextFunction == "FLASHING"))) : new bool?();
            }
            bool? nullable4 = nullable3;
            bool flag = true;
            num = nullable4.GetValueOrDefault() == flag & nullable4.HasValue ? 1 : 0;
          }
          if (num != 0)
          {
            this.IsFlashingNext = true;
          }
          else
          {
            if (string.IsNullOrEmpty(flashingItem?.FlashingIndex))
              return;
            IMvxNavigationService navigationService = this._navigationService;
            FlashingParameter flashingParameter = new FlashingParameter();
            flashingParameter.FlashingItem = flashingItem;
            flashingParameter.IsBatchAuto = this.Parameter.IsBatchAuto;
            flashingParameter.ComingFromCoding = true;
            CancellationToken cancellationToken = new CancellationToken();
            navigationService.Navigate<ApplianceNonSMMFlashViewModel, FlashingParameter>(flashingParameter, (IMvxBundle) null, cancellationToken);
          }
        }));
        if (json.Success & showSuccessPopup)
        {
          this._alertService.ShowMessageAlertWithKeyFromService("CODING_SUCCESSFUL", "", (Action) (() => { }));
          this._loggingService.getLogger().LogAppDebug(LoggingContext.BACKEND, "Successfully uploaded json: " + jsonToSend, memberName: nameof (OnSubmit), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/NonSmmApplianceCodingViewModel.cs", sourceLineNumber: 165);
          this.historySb.AppendLine(AppResource.CODING_SUCCESSFUL);
        }
        else if (!json.Success)
        {
          this._alertService.ShowMessageAlertWithKeyFromService("CODING_FAILED", "", (Action) (() => { }));
          this._loggingService.getLogger().LogAppDebug(LoggingContext.BACKEND, "Error uploaded json: " + jsonToSend, memberName: nameof (OnSubmit), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/NonSmmApplianceCodingViewModel.cs", sourceLineNumber: 173);
          this.historySb.AppendLine(AppResource.CODING_FAILED);
        }
      }
      catch (Exception ex)
      {
        this._loggingService.getLogger().LogAppDebug(LoggingContext.BACKEND, "json upload error: " + ex.Message, memberName: nameof (OnSubmit), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/NonSmmApplianceCodingViewModel.cs", sourceLineNumber: 179);
        this.historySb.AppendLine(AppResource.CODING_ERROR);
      }
      this.AreButtonsEnabled = false;
      try
      {
        this.historySb.AppendLine();
        CoreApp.history.SaveItem(new History(this._repairEnumber, DateTime.Now, UtilityFunctions.getSessionIDForHistoryTable(), HistoryDBInfoType.CodingLog.ToString(), this.historySb.ToString()));
      }
      catch (Exception ex)
      {
        this._loggingService.getLogger().LogAppInformation(LoggingContext.CODING, "Failed to save item in the History DB, " + ex?.ToString(), memberName: nameof (OnSubmit), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/NonSmmApplianceCodingViewModel.cs", sourceLineNumber: 192 /*0xC0*/);
      }
      this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
    }));
  }

  internal void testcom() => this._alertService.ShowMessageAlertWithMessage("mess", "title");

  public ICommand ValidateCommand { internal set; get; }

  public ICommand SubmitCommand { internal set; get; }

  internal void ReturnToVarCodingFunction(object obj)
  {
    this._sshWrapper.DeactivateCoder();
    this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
  }

  public ICommand ReturnToVarCoding { internal set; get; }

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

  public ObservableCollection<VariantCodingData> VarCodingOptionFieldsList
  {
    get => this._varCodingOptionFieldsList;
    set
    {
      this._varCodingOptionFieldsList = value;
      this.RaisePropertyChanged<ObservableCollection<VariantCodingData>>((Expression<Func<ObservableCollection<VariantCodingData>>>) (() => this.VarCodingOptionFieldsList));
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

  public bool DisplayImage
  {
    get => this._DisplayImage;
    internal set
    {
      this._DisplayImage = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.DisplayImage));
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

  public VariantCodingData ObjItemSelected
  {
    get => this._ObjItemSelected;
    set
    {
      if (this._ObjItemSelected == value)
        return;
      this._ObjItemSelected = value;
      this.RaisePropertyChanged<VariantCodingData>((Expression<Func<VariantCodingData>>) (() => this.ObjItemSelected));
    }
  }

  public bool IsBtnDisabled
  {
    get => this._IsBtnDisabled;
    internal set
    {
      this._IsBtnDisabled = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.IsBtnDisabled));
    }
  }

  public string ButtonLabel
  {
    get => this._ButtonLabel;
    internal set
    {
      this._ButtonLabel = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.ButtonLabel));
    }
  }

  public string CancelButtonLabel
  {
    get => this._CancelButtonLabel;
    internal set
    {
      this._CancelButtonLabel = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.CancelButtonLabel));
    }
  }

  public string HelpLabelColor
  {
    get => this._HelpLabelColor;
    set
    {
      this._HelpLabelColor = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.HelpLabelColor));
    }
  }

  public override void Prepare(CodingParameter parameter)
  {
    this._loggingService.getLogger().LogAppDebug(LoggingContext.APPLIANCE, "In NonSmmApplianceCodingViewModel prepare method ", memberName: nameof (Prepare), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/NonSmmApplianceCodingViewModel.cs", sourceLineNumber: 385);
    try
    {
      this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "In prepare method with parameter value : " + JsonConvert.SerializeObject((object) parameter), memberName: nameof (Prepare), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/NonSmmApplianceCodingViewModel.cs", sourceLineNumber: 389);
      this.Parameter = parameter;
      this.Headertitle = this.GetShortText(this.Parameter.CodingItem.Name);
      List<string> source = new List<string>()
      {
        "ENR",
        "VIB",
        "KI",
        "SAP_BRAND",
        "BRAND"
      };
      foreach (UserEntryField userEntryField in this.Parameter.CodingItem.UserEntryFields)
      {
        UserEntryField entryField = userEntryField;
        if (source.Any<string>((Func<string, bool>) (x => x.Equals(entryField.FieldVariable))))
          entryField.LockEntry = true;
        entryField.FieldTitle = !(entryField.FieldVariable == "ENR") ? Regex.Replace(entryField.FieldTitle, " {3,}", "\n") : "ENR";
        switch (entryField.FieldVariable)
        {
          case "VIB":
          case "KI":
          case "ENR":
            if (entryField.FieldVariable == "ENR" || string.IsNullOrEmpty(entryField.FieldValue) && !entryField.LockEntry)
              this.VarCodingOptionFieldsList.Add(new VariantCodingData(this.GetShortText(entryField.FieldTitle), new int?(Convert.ToInt32(entryField.FieldLength)), entryField.FieldVariable, entryField.FieldType, entryField.FieldValue, entryField.LockEntry));
            continue;
          case "SAP_BRAND":
            entryField.FieldValue = this._brandInfo.Item1;
            goto case "VIB";
          case "BRAND":
            entryField.FieldValue = this._brandInfo.Item2;
            goto case "VIB";
          default:
            entryField.FieldValue = "";
            entryField.LockEntry = false;
            goto case "VIB";
        }
      }
      this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "VarCodingOptionFieldsList : " + JsonConvert.SerializeObject((object) this.VarCodingOptionFieldsList), memberName: nameof (Prepare), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/NonSmmApplianceCodingViewModel.cs", sourceLineNumber: 438);
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppError(LoggingContext.APPLIANCE, "Error during coding fields initilaization: " + ex.Message, memberName: nameof (Prepare), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/NonSmmApplianceCodingViewModel.cs", sourceLineNumber: 442);
      this.showPopUpAndClosePage(AppResource.ERROR_IN_CODING_MESSAGE);
    }
  }

  private void showPopUpAndClosePage(string message)
  {
    int num;
    Device.BeginInvokeOnMainThread((Action) (async () => await this._alertService.ShowMessageAlertWithKey(message, AppResource.ERROR_TITLE, AppResource.WARNING_OK, (Action) (async () => num = await this._navigationService.Close((IMvxViewModel) this, new CancellationToken()) ? 1 : 0))));
  }

  public override void ViewAppeared()
  {
    base.ViewAppeared();
    this.IsBtnDisabled = this.VarCodingOptionFieldsList.Any<VariantCodingData>((Func<VariantCodingData, bool>) (x => !x.IsLocked && !x.IsValid));
    this.AreButtonsEnabled = true;
    this._viewstatus = true;
    this._appliance.CheckConnectivity(new connectivityUpdateCallback(this.UpdateStatus));
  }

  internal void UpdateStatus()
  {
    this.WifiStatus = this._appliance.StatusOfBridgeConnection;
    this.ConnectedColor = this._appliance.ConnectedColorBridge;
  }
}
