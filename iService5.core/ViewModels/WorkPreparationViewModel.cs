// Decompiled with JetBrains decompiler
// Type: iService5.Core.ViewModels.WorkPreparationViewModel
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Data;
using iService5.Core.Services.Helpers;
using iService5.Core.Services.Platform;
using iService5.Core.Services.User;
using iService5.Core.Services.User.ActivityLogging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

public class WorkPreparationViewModel : MvxViewModel
{
  internal List<string> smmMaterials = new List<string>();
  internal List<string> nonsmmMaterials = new List<string>();
  private string _DownloadButtonText = AppResource.ST_PAGE_DWL_NOW;
  private bool _areButtonsEnabled = true;
  private readonly ILoggingService _loggingService;
  private readonly IAlertService _alertService;
  private readonly IMetadataService _metadataService;
  private readonly IMvxNavigationService _navigationService;
  private readonly IPlatformSpecificServiceLocator _locator;
  private readonly IUserSession _userSession;
  private double _listViewHeightRequest = 10.0;
  private string _typedEnumber = string.Empty;
  private material _selectedEnumber;
  private bool _isListVisible;
  private bool _isPreparationCollectionPopulated;
  private bool _isAddButtonEnabled;
  private bool _InvalidEnumberWarningVisibility = false;

  public MvxObservableCollection<material> EnumberCollection { get; internal set; } = new MvxObservableCollection<material>();

  public MvxObservableCollection<material> FilteredEnumbersCollection { get; internal set; } = new MvxObservableCollection<material>();

  public MvxObservableCollection<PrepareWorkEnumbers> PreparedEnumbersCollection { get; internal set; } = new MvxObservableCollection<PrepareWorkEnumbers>();

  public ICommand GoCommand { get; internal set; }

  public ICommand GoBackCommand { get; internal set; }

  public ICommand AddEnumberCommand { get; internal set; }

  public Command<string> DeleteEnumberCommand { get; internal set; }

  public string DownloadButtonText
  {
    get => this._DownloadButtonText;
    internal set
    {
      this._DownloadButtonText = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.DownloadButtonText));
    }
  }

  public WorkPreparationViewModel(
    IMvxNavigationService navigationService,
    IPlatformSpecificServiceLocator locator,
    ILoggingService loggingService,
    IAlertService alertService,
    IMetadataService metadataService,
    IUserSession userSession)
  {
    this._loggingService = loggingService;
    this._alertService = alertService;
    this._metadataService = metadataService;
    this._navigationService = navigationService;
    this._locator = locator;
    this._userSession = userSession;
    this.GoCommand = (ICommand) new Command(new Action(this.ResolveNextScreen));
    this.GoBackCommand = (ICommand) new Command(new Action(this.GoBack));
    this.DeleteEnumberCommand = new Command<string>((Action<string>) (enumber =>
    {
      ((Collection<PrepareWorkEnumbers>) this.PreparedEnumbersCollection).Remove(((IEnumerable<PrepareWorkEnumbers>) this.PreparedEnumbersCollection).Where<PrepareWorkEnumbers>((Func<PrepareWorkEnumbers, bool>) (x => x.enumber == enumber)).Single<PrepareWorkEnumbers>());
      this.RemoveEnumberInMaterial(enumber);
      this.UpdateDownloadButtonText();
      this.InvalidEnumberWarningVisibility = ((IEnumerable<PrepareWorkEnumbers>) this.PreparedEnumbersCollection).Any<PrepareWorkEnumbers>((Func<PrepareWorkEnumbers, bool>) (x => !x.isValid));
      this.IsPreparationCollectionPopulated = ((IEnumerable<PrepareWorkEnumbers>) this.PreparedEnumbersCollection).Any<PrepareWorkEnumbers>() && !this.InvalidEnumberWarningVisibility;
    }));
    this.AddEnumberCommand = (ICommand) new Command<string>((Action<string>) (enumber => this.AddEnumber(enumber, true)));
  }

  public virtual async Task Initialize()
  {
    await base.Initialize();
    List<string> prepareWorkeNumberList = this._userSession.GetURLSchemePrepareWorkEnumbers();
    foreach (string eNumber in prepareWorkeNumberList)
    {
      if (this._metadataService.IsMaterialAvailable(eNumber))
      {
        this.AddEnumber(eNumber?.Trim().ToUpperInvariant(), true);
      }
      else
      {
        this.AddEnumber(eNumber?.Trim().ToUpperInvariant(), false);
        this._loggingService.getLogger().LogAppInformation(LoggingContext.LOCAL, "Enumber not present in database : " + eNumber, memberName: nameof (Initialize), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/WorkPreparationViewModel.cs", sourceLineNumber: 93);
      }
    }
    this._userSession.SetURLSchemePrepareWorkEnumbers(new List<string>());
    CoreApp.settings.UpdateItem(new Settings("SelectedURLScheme", ""));
    prepareWorkeNumberList = (List<string>) null;
  }

  public override void ViewAppearing()
  {
    this.ViewAppeared();
    this.AreButtonsEnabled = true;
  }

  internal void ResolveNextScreen()
  {
    Device.BeginInvokeOnMainThread((Action) (async () =>
    {
      if (!this.AreButtonsEnabled)
        return;
      this.AreButtonsEnabled = false;
      IMvxNavigationService navigationService = this._navigationService;
      DetailNavigationArgs detailNavigationArgs = new DetailNavigationArgs();
      detailNavigationArgs.preparationModules = this.GetPreparationModules();
      detailNavigationArgs.senderScreen = AppResource.PREPARE_YOUR_WORK_TITLE;
      CancellationToken cancellationToken = new CancellationToken();
      int num = await navigationService.Navigate<ApplianceRepairViewModel, DetailNavigationArgs>(detailNavigationArgs, (IMvxBundle) null, cancellationToken) ? 1 : 0;
    }));
  }

  internal void GoBack()
  {
    this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
  }

  internal void AddEnumber(string enumber, bool isValid)
  {
    if (!((IEnumerable<PrepareWorkEnumbers>) this.PreparedEnumbersCollection).Any<PrepareWorkEnumbers>((Func<PrepareWorkEnumbers, bool>) (x => x.enumber.Equals(enumber))) && enumber != string.Empty)
    {
      ((Collection<PrepareWorkEnumbers>) this.PreparedEnumbersCollection).Add(new PrepareWorkEnumbers(enumber, isValid));
      this.AddEnumberInMaterial(enumber);
      this.UpdateDownloadButtonText();
      this.TypedEnumber = string.Empty;
      this.IsAddButtonEnabled = false;
    }
    this.InvalidEnumberWarningVisibility = ((IEnumerable<PrepareWorkEnumbers>) this.PreparedEnumbersCollection).Any<PrepareWorkEnumbers>((Func<PrepareWorkEnumbers, bool>) (x => !x.isValid));
    this.IsPreparationCollectionPopulated = ((IEnumerable<PrepareWorkEnumbers>) this.PreparedEnumbersCollection).Any<PrepareWorkEnumbers>() && !this.InvalidEnumberWarningVisibility;
  }

  public void UpdateDownloadButtonText()
  {
    long filesSize = this.GetFilesSize();
    string str = FileSizeFormatter.FormatSize(filesSize);
    if (filesSize != 0L)
      this.DownloadButtonText = $"{AppResource.ST_PAGE_DWL_NOW} ({str})";
    else
      this.DownloadButtonText = AppResource.ST_PAGE_DWL_NOW;
  }

  internal long GetFilesSize()
  {
    if (this.smmMaterials.Count == 0 && this.nonsmmMaterials.Count == 0)
      return 0;
    string commaSeperatedValues1 = UtilityFunctions.getCommaSeperatedValues<string>(this.smmMaterials);
    string commaSeperatedValues2 = UtilityFunctions.getCommaSeperatedValues<string>(this.nonsmmMaterials);
    List<MaterialStatistics> statisticsOfEnumbers1 = this._metadataService.getMaterialStatisticsOfENumbers(commaSeperatedValues1);
    List<MaterialStatistics> statisticsOfEnumbers2 = this._metadataService.getMaterialStatisticsOfENumbers(commaSeperatedValues2);
    return this._metadataService.GetDownloadFilesStatistics(new Dictionary<DeviceClass, List<MaterialStatistics>>()
    {
      [DeviceClass.SMM] = statisticsOfEnumbers1,
      [DeviceClass.NON_SMM] = statisticsOfEnumbers2
    }).fileSize;
  }

  internal void AddEnumberInMaterial(string enumber)
  {
    if (this._metadataService.isSMM(enumber))
      this.smmMaterials.Add(enumber);
    else
      this.nonsmmMaterials.Add(enumber);
  }

  internal void RemoveEnumberInMaterial(string enumber)
  {
    if (this._metadataService.isSMM(enumber))
      this.smmMaterials.Remove(enumber);
    else
      this.nonsmmMaterials.Remove(enumber);
  }

  internal void EntryTextChanged(string newText)
  {
    if (newText.Length < 4)
      return;
    if (!this.TypedEnumber.StartsWith(newText.Substring(0, 4)))
      this.EnumberCollection.ReplaceWith((IEnumerable<material>) this._metadataService.getMatchingEntries(newText, MatchPattern.PREFIX).OrderBy<material, string>((Func<material, string>) (x => x._material)));
    this.FilteredEnumbersCollection.ReplaceWith((IEnumerable<material>) ((IEnumerable<material>) this.EnumberCollection).Where<material>((Func<material, bool>) (x => x._material.StartsWith(newText, StringComparison.InvariantCultureIgnoreCase))).OrderBy<material, string>((Func<material, string>) (y => y._material)));
    this.ListViewHeightRequest = (double) ((Collection<material>) this.FilteredEnumbersCollection).Count * 50.0;
    this.IsListVisible = ((IEnumerable<material>) this.FilteredEnumbersCollection).Any<material>();
    this.IsAddButtonEnabled = ((IEnumerable<material>) this.EnumberCollection).Any<material>((Func<material, bool>) (x => x._material.Equals(newText)));
  }

  internal List<string> GetPreparationModules()
  {
    List<string> preparationModules = new List<string>();
    foreach (PrepareWorkEnumbers preparedEnumbers in (Collection<PrepareWorkEnumbers>) this.PreparedEnumbersCollection)
      preparationModules.Add(preparedEnumbers.enumber);
    return preparationModules;
  }

  public string TypedEnumber
  {
    get => this._typedEnumber;
    set
    {
      this.EntryTextChanged(value?.Trim().ToUpperInvariant());
      this.SetProperty<string>(ref this._typedEnumber, value?.Trim().ToUpperInvariant(), nameof (TypedEnumber));
    }
  }

  public material SelectedEnumber
  {
    get => this._selectedEnumber;
    set
    {
      this.SetProperty<material>(ref this._selectedEnumber, value, nameof (SelectedEnumber));
      this.TypedEnumber = this.SelectedEnumber?._material.Trim().ToUpperInvariant();
    }
  }

  public bool IsListVisible
  {
    get => this._isListVisible;
    set => this.SetProperty<bool>(ref this._isListVisible, value, nameof (IsListVisible));
  }

  public double ListViewHeightRequest
  {
    get => this._listViewHeightRequest;
    set
    {
      this.SetProperty<double>(ref this._listViewHeightRequest, value, nameof (ListViewHeightRequest));
    }
  }

  public bool IsAddButtonEnabled
  {
    get => this._isAddButtonEnabled;
    set => this.SetProperty<bool>(ref this._isAddButtonEnabled, value, nameof (IsAddButtonEnabled));
  }

  public bool IsPreparationCollectionPopulated
  {
    get => this._isPreparationCollectionPopulated;
    set
    {
      this.SetProperty<bool>(ref this._isPreparationCollectionPopulated, value, nameof (IsPreparationCollectionPopulated));
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

  public bool InvalidEnumberWarningVisibility
  {
    get => this._InvalidEnumberWarningVisibility;
    set
    {
      this.SetProperty<bool>(ref this._InvalidEnumberWarningVisibility, value, nameof (InvalidEnumberWarningVisibility));
    }
  }
}
