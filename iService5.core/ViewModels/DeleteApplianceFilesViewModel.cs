// Decompiled with JetBrains decompiler
// Type: iService5.Core.ViewModels.DeleteApplianceFilesViewModel
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
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Windows.Input;
//using Xamarin.Forms;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

#nullable disable
namespace iService5.Core.ViewModels;

public class DeleteApplianceFilesViewModel : MvxViewModel
{
  private readonly IMvxNavigationService _navigationService;
  private readonly IAlertService _alertService;
  private readonly IMetadataService _metadataService;
  private readonly IUserSession _userSession;
  private readonly IPlatformSpecificServiceLocator _locator;
  private readonly ILoggingService _loggingService;
  private bool _IsPpfRefetchRequired = true;
  private bool _isDeleteButtonEnabled = true;
  private string _BackButtonText = AppResource.APPLIANCE_FLASH_BACK_TEXT;
  private string _NavHeaderTitle = AppResource.DELETE_APPLIANCE_FILES;
  private string _DeleteAllFileBtntext = AppResource.DELETE_ALL_APPLIANCE_FILES_BUTTON_TEXT;
  private string _StorageStatusText = AppResource.ST_PAGE_DWL_LOCAL_FILES;
  private bool _isbusy = false;

  public DeleteApplianceFilesViewModel(
    IMetadataService metadataService,
    IPlatformSpecificServiceLocator locator,
    IUserSession userSession,
    IMvxNavigationService navigationService,
    IAlertService alertService,
    ILoggingService loggingService)
  {
    this.DeleteFileCommand = (ICommand) new Command(new Action(this.DeleteFilesButtonClicked));
    this.PpfRefetchCommand = (ICommand) new Command(new Action(this.NavigateToStatusView));
    this.BackCommand = (ICommand) new Command(new Action(this.VisitSettingsPage));
    this._navigationService = navigationService;
    this._alertService = alertService;
    this._userSession = userSession;
    this._metadataService = metadataService;
    this._locator = locator;
    this._loggingService = loggingService;
    StringBuilder stringBuilder = new StringBuilder(AppResource.ST_PAGE_DWL_LOCAL_FILES);
    stringBuilder.Append(" ");
    stringBuilder.AppendLine(this._metadataService.getLocalSize().ToString());
    stringBuilder.Append(AppResource.ST_PAGE_USED_STORAGE);
    stringBuilder.Append(" ");
    stringBuilder.AppendLine(this._metadataService.GetFileSize());
    stringBuilder.Append(AppResource.AVAILABLE_STORAGE_ON_DEVICE);
    stringBuilder.AppendLine(this._locator.GetPlatformSpecificService().GetMemoryStorage(false));
    this.StorageStatusText = stringBuilder.ToString();
  }

  public bool IsPpfRefetchRequired
  {
    get => this._IsPpfRefetchRequired;
    set
    {
      this._IsPpfRefetchRequired = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.IsPpfRefetchRequired));
    }
  }

  internal void VisitSettingsPage()
  {
    this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
  }

  public ICommand BackCommand { internal set; get; }

  public ICommand DeleteFileCommand { internal set; get; }

  public ICommand PpfRefetchCommand { internal set; get; }

  internal void DeleteFilesButtonClicked()
  {
    this._alertService.ShowMessageAlertWithKey("DELETE_ALL_APPLIANCE_FILES_ALERT_TEXT", AppResource.WARNING_TEXT, AppResource.NO_TEXT, AppResource.YES_TEXT, (Action<bool>) (cancel =>
    {
      if (cancel)
        return;
      this.IsBusy = true;
      this.IsDeleteButtonEnabled = false;
      if (this._metadataService.deleteAllBinaries(this._locator))
      {
        this._metadataService.clearCacheUsedStorage();
        this._loggingService.getLogger().LogAppInformation(LoggingContext.BINARY, "All Binary files have been deleted.", memberName: nameof (DeleteFilesButtonClicked), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/DeleteApplianceFilesViewModel.cs", sourceLineNumber: 86);
        this.IsDeleteButtonEnabled = true;
        this.VisitSettingsPage();
      }
      else
      {
        this._loggingService.getLogger().LogAppInformation(LoggingContext.ERRORSMM, "Error / Exception in deleteing all Binary files from Setting's page.", memberName: nameof (DeleteFilesButtonClicked), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/DeleteApplianceFilesViewModel.cs", sourceLineNumber: 92);
        this._alertService.ShowMessageAlertWithKeyFromService("GENERIC_ERROR_MESSAGE", AppResource.INFORMATION_TEXT, (Action) (() => this.VisitSettingsPage()));
      }
      this.IsBusy = false;
    }));
  }

  private void NavigateToStatusView()
  {
    this._alertService.ShowMessageAlertWithKey("PPF_REFETCH_INFORMATION_TEXT", AppResource.INFORMATION_TITLE, AppResource.ACCEPT_LABEL, AppResource.CANCEL_LABEL, (Action<bool>) (async ok =>
    {
      if (!ok)
        return;
      IMvxNavigationService navigationService = this._navigationService;
      DetailNavigationArgs detailNavigationArgs = new DetailNavigationArgs();
      detailNavigationArgs.ReturnToSettingsPage = true;
      CancellationToken cancellationToken = new CancellationToken();
      int num = await navigationService.Navigate<StatusViewModel, DetailNavigationArgs>(detailNavigationArgs, (IMvxBundle) null, cancellationToken) ? 1 : 0;
    }));
  }

  private List<string> GetEnumbersForPpfs()
  {
    return UtilityFunctions.getDownloadedEnumbersFromDB(this._metadataService);
  }

  public bool IsDeleteButtonEnabled
  {
    get => this._isDeleteButtonEnabled;
    set
    {
      this.SetProperty<bool>(ref this._isDeleteButtonEnabled, value, nameof (IsDeleteButtonEnabled));
    }
  }

  public string BackButtonText
  {
    get => this._BackButtonText;
    internal set
    {
      this._BackButtonText = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.BackButtonText));
    }
  }

  public string NavHeaderTitle
  {
    get => this._NavHeaderTitle;
    internal set
    {
      this._NavHeaderTitle = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.NavHeaderTitle));
    }
  }

  public string DeleteAllFileBtntext
  {
    get => this._DeleteAllFileBtntext;
    internal set
    {
      this._DeleteAllFileBtntext = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.DeleteAllFileBtntext));
    }
  }

  public string StorageStatusText
  {
    get => this._StorageStatusText;
    internal set
    {
      this._StorageStatusText = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.StorageStatusText));
    }
  }

  public bool IsBusy
  {
    get => this._isbusy;
    set
    {
      this._isbusy = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.IsBusy));
    }
  }
}
