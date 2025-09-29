// Decompiled with JetBrains decompiler
// Type: iService5.Core.ViewModels.LegalInfoListViewModel
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
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
//using Xamarin.Forms;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

#nullable disable
namespace iService5.Core.ViewModels;

public class LegalInfoListViewModel : MvxViewModel
{
  private readonly IMvxNavigationService _navigationService;
  private string _NavText = AppResource.APPLIANCE_FLASH_BACK_TEXT;
  private string _NavHeaderTitle = AppResource.LEGAL_INFO_HEADER;
  private List<string> legalInfoList;
  private string _ItemSelected;
  private bool _areButtonsEnabled = true;

  public virtual async Task Initialize() => await base.Initialize();

  public LegalInfoListViewModel(
    IMvxNavigationService navigationService,
    IShortTextsService _SService,
    ILoggingService loggingService,
    IMetadataService metadataService,
    IPlatformSpecificServiceLocator locator,
    IUserSession userSession,
    IAlertService alertService,
    IAppliance appliance,
    IApplianceSession session)
  {
    this.NavigatePreviousPage = (ICommand) new Command(new Action<object>(this.goToBackPage));
    this._navigationService = navigationService;
    this.legalInfoList = new List<string>()
    {
      AppResource.IMPRINT_TEXT,
      AppResource.DATA_PRIVACY_POLICY_TEXT,
      AppResource.USED_LIBRARIES_HEADER
    };
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

  public string NavHeaderTitle
  {
    get => this._NavHeaderTitle;
    internal set
    {
      this._NavHeaderTitle = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.NavHeaderTitle));
    }
  }

  public ICommand NavigatePreviousPage { internal set; get; }

  internal void goToBackPage(object obj)
  {
    this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
  }

  public List<string> LegalInfoList
  {
    get => this.legalInfoList;
    internal set
    {
      this.legalInfoList = value;
      this.RaisePropertyChanged<List<string>>((Expression<Func<List<string>>>) (() => this.LegalInfoList));
    }
  }

  public string ObjItemSelected
  {
    get => this._ItemSelected;
    set
    {
      if (this._ItemSelected != value)
      {
        this._ItemSelected = value;
        if (value == AppResource.IMPRINT_TEXT)
          Device.BeginInvokeOnMainThread((Action) (() =>
          {
            if (!this.AreButtonsEnabled)
              return;
            this.AreButtonsEnabled = false;
            this._navigationService.Navigate<ImprintViewModel>((IMvxBundle) null, new CancellationToken());
          }));
        else if (value == AppResource.DATA_PRIVACY_POLICY_TEXT)
          Device.BeginInvokeOnMainThread((Action) (() =>
          {
            if (!this.AreButtonsEnabled)
              return;
            this.AreButtonsEnabled = false;
            this._navigationService.Navigate<DataPrivacyViewModel>((IMvxBundle) null, new CancellationToken());
          }));
        else
          Device.BeginInvokeOnMainThread((Action) (() =>
          {
            if (!this.AreButtonsEnabled)
              return;
            this.AreButtonsEnabled = false;
            this._navigationService.Navigate<LegalInfoForLibViewModel>((IMvxBundle) null, new CancellationToken());
          }));
        this._ItemSelected = (string) null;
      }
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.ObjItemSelected));
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

  public override void ViewAppeared()
  {
    base.ViewAppeared();
    this.AreButtonsEnabled = true;
  }
}
