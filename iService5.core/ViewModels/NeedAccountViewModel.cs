// Decompiled with JetBrains decompiler
// Type: iService5.Core.ViewModels.NeedAccountViewModel
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Localisation;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Windows.Input;
//using Xamarin.Forms;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

#nullable disable
namespace iService5.Core.ViewModels;

public class NeedAccountViewModel : MvxViewModel
{
  private readonly IMvxNavigationService _navigationService;
  private string _NavText = AppResource.APPLIANCE_FLASH_BACK_TEXT;
  private string _NeedAccountPageLabel;
  private string _NavHeaderTitle = AppResource.NEED_ACCOUNT_PAGE_TITLE_LABEL;

  public NeedAccountViewModel(
    IMvxNavigationService navigationService,
    IShortTextsService _ShortTextsService)
  {
    this.GoToLoginPage = (ICommand) new Command(new Action(this.VisitLoginPage));
    this._navigationService = navigationService;
    if (_ShortTextsService == null)
      return;
    this.NeedAccountPageLabel = AppResource.NEED_ACCOUNT_PAGE_DETAILS_LABEL.Replace("/", "/\u2060");
  }

  internal void VisitLoginPage()
  {
    this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
  }

  public ICommand GoToLoginPage { protected set; get; }

  public string NavText
  {
    get => this._NavText;
    internal set
    {
      this._NavText = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.NavText));
    }
  }

  public string NeedAccountPageLabel
  {
    get => this._NeedAccountPageLabel;
    private set
    {
      this._NeedAccountPageLabel = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.NeedAccountPageLabel));
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
}
