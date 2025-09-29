// Decompiled with JetBrains decompiler
// Type: iService5.Core.ViewModels.ImprintViewModel
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.VersionReport;
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

public class ImprintViewModel : MvxViewModel
{
  private readonly IMvxNavigationService _navigationService;
  private string _NavText = AppResource.APPLIANCE_FLASH_BACK_TEXT;
  private string _NavHeaderTitle = AppResource.IMPRINT_TEXT;
  private string _AppVersionLabel;

  public ImprintViewModel(IMvxNavigationService navigationService, IVersionReport versionReport)
  {
    this.GoToBackPage = (ICommand) new Command(new Action(this.NavigateToTheBackPage));
    this.PhoneNoCommand = (ICommand) new Command(new Action(this.OnClickingPhoneNo));
    this._navigationService = navigationService;
    this.AppVersionLabel = versionReport.getVersion();
  }

  public ICommand PhoneNoCommand { internal set; get; }

  public void OnClickingPhoneNo() => throw new NotImplementedException();

  internal void NavigateToTheBackPage()
  {
    this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
  }

  public ICommand GoToBackPage { internal set; get; }

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

  public string AppVersionLabel
  {
    get => this._AppVersionLabel;
    internal set
    {
      this._AppVersionLabel = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.AppVersionLabel));
    }
  }
}
