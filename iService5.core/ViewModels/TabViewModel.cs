// Decompiled with JetBrains decompiler
// Type: iService5.Core.ViewModels.TabViewModel
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Data;
using iService5.Core.Services.Helpers;
using iService5.Core.Services.User;
using MvvmCross.Navigation;
using MvvmCross.Presenters.Hints;
using MvvmCross.ViewModels;
using System;
using System.Collections;
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

public class TabViewModel : MvxViewModel<DetailNavigationArgs>
{
  private readonly IMvxNavigationService _navigationService;
  private bool _firstTime = true;
  private bool _toSettingsTab;
  private readonly IAlertService _alertService;
  private bool _MetadataFailedHomePage = false;
  private string _WifiStatus;
  private string _ConnectionColor = "Green";
  private string _NavTitle = "";
  private string _Bridge_feature;
  private List<Task> _vmtasks = new List<Task>();
  private IEnumerable _Itsrc;
  private bool nonsmmarrival;

  public override void Prepare(DetailNavigationArgs param)
  {
    this._toSettingsTab = true;
    if (!param.ReturnToSettingsPage)
      this._toSettingsTab = false;
    if (param.bridgeDisplay)
      this.Bridge_feature = "true";
    else
      this.Bridge_feature = "false";
  }

  public TabViewModel(IMvxNavigationService navigationService, IAlertService alertService)
  {
    this._navigationService = navigationService;
    this._alertService = alertService;
    MessagingCenter.Subscribe<StatusViewModel>((object) this, CoreApp.EventsNames.MetadataFailed.ToString(), (Action<StatusViewModel>) (sender => this.MetadataFailedHomePage = true), (StatusViewModel) null);
    CoreApp.EventsNames eventsNames = CoreApp.EventsNames.MetadataUpdated;
    MessagingCenter.Subscribe<StatusViewModel>((object) this, eventsNames.ToString(), (Action<StatusViewModel>) (async sender => await Task.Run((Action) (() =>
    {
      CoreApp.settings.DeleteItem(new Settings("lastUpdate", ""));
      CoreApp.settings.UpdateItem(new Settings("lastUpdate", DateTime.Now.ToString("G", (IFormatProvider) CultureInfo.InvariantCulture)));
    }))), (StatusViewModel) null);
    eventsNames = CoreApp.EventsNames.BridgeConnected;
    MessagingCenter.Subscribe<CatViewModel>((object) this, eventsNames.ToString(), (Action<CatViewModel>) (sender =>
    {
      this.WifiStatus = "Connected";
      this.ConnectionColor = "Green";
      this.NavTitle = AppResource.BRIDGE_HEADER;
    }), (CatViewModel) null);
    eventsNames = CoreApp.EventsNames.BridgeDisconnected;
    MessagingCenter.Subscribe<CatViewModel>((object) this, eventsNames.ToString(), (Action<CatViewModel>) (sender =>
    {
      this.WifiStatus = "Disconnected";
      this.ConnectionColor = "#808080";
      this.NavTitle = AppResource.BRIDGE_HEADER;
    }), (CatViewModel) null);
    eventsNames = CoreApp.EventsNames.FeedBackFormPending;
    MessagingCenter.Subscribe<UserSession>((object) this, eventsNames.ToString(), (Action<UserSession>) (sender => this._alertService.ShowMessageAlertWithKey("FEEDBACK_PAGE_SENT", AppResource.INFORMATION_TITLE)), (UserSession) null);
    eventsNames = CoreApp.EventsNames.FeedBackFormPending;
    MessagingCenter.Subscribe<FeedbackFormViewModel>((object) this, eventsNames.ToString(), (Action<FeedbackFormViewModel>) (sender => { }), (FeedbackFormViewModel) null);
    try
    {
      string str = CoreApp.settings.GetItem("SelectedDeviceClass").Value;
      if (str == AppResource.NON_SMM_TEXT)
        CoreApp.settings.UpdateItem(new Settings("SelectedDeviceClass", DeviceClass.NON_SMM.ToString()));
      else if (str == AppResource.ALL_TEXT)
        CoreApp.settings.UpdateItem(new Settings("SelectedDeviceClass", DeviceClass.ALL.ToString()));
      else if (str == AppResource.SMM_TEXT)
        CoreApp.settings.UpdateItem(new Settings("SelectedDeviceClass", DeviceClass.SMM.ToString()));
    }
    catch
    {
    }
    try
    {
      string str = CoreApp.settings.GetItem("DownloadSettings").Value;
      if (str == AppResource.COUNTRY_RELEVANT_TEXT)
        CoreApp.settings.UpdateItem(new Settings("DownloadSettings", DownloadOption.COUNTRY_RELEVANT.ToString()));
      else if (str == AppResource.FULL_DOWNLOAD_TEXT)
        CoreApp.settings.UpdateItem(new Settings("DownloadSettings", DownloadOption.FULL_DOWNLOAD.ToString()));
    }
    catch
    {
    }
    try
    {
      string str = CoreApp.settings.GetItem("catTimeout").Value;
    }
    catch (Exception ex)
    {
      CoreApp.settings.SaveItem(new Settings("catTimeout", "19"));
    }
    try
    {
      string str = CoreApp.settings.GetItem("localNetworkPopupAppeared").Value;
    }
    catch (Exception ex)
    {
      CoreApp.settings.SaveItem(new Settings("localNetworkPopupAppeared", "false"));
    }
    try
    {
      string str = CoreApp.settings.GetItem("rpeCalSucceeded").Value;
    }
    catch (Exception ex)
    {
      CoreApp.settings.SaveItem(new Settings("rpeCalSucceeded", "false"));
    }
    try
    {
      string str = CoreApp.settings.GetItem("rpeCalFailed").Value;
    }
    catch (Exception ex)
    {
      CoreApp.settings.SaveItem(new Settings("rpeCalFailed", "false"));
    }
    try
    {
      string str = CoreApp.settings.GetItem("rpeCalMeasurement").Value;
    }
    catch (Exception ex)
    {
      CoreApp.settings.SaveItem(new Settings("rpeCalMeasurement", ""));
    }
    try
    {
      string str = CoreApp.settings.GetItem("CountryCodes").Value;
    }
    catch (Exception ex)
    {
      CoreApp.settings.SaveItem(new Settings("CountryCodes", ""));
    }
    try
    {
      string str = CoreApp.settings.GetItem("clampCalSucceeded").Value;
    }
    catch (Exception ex)
    {
      CoreApp.settings.SaveItem(new Settings("clampCalSucceeded", "false"));
    }
    try
    {
      string str = CoreApp.settings.GetItem("clampCalFailed").Value;
    }
    catch (Exception ex)
    {
      CoreApp.settings.SaveItem(new Settings("clampCalFailed", "false"));
    }
    try
    {
      string str = CoreApp.settings.GetItem("clampCalMeasurement").Value;
    }
    catch (Exception ex)
    {
      CoreApp.settings.SaveItem(new Settings("clampCalMeasurement", ""));
    }
    try
    {
      string str = CoreApp.settings.GetItem("SelectedCallibrationOption").Value;
    }
    catch (Exception ex)
    {
      CoreApp.settings.SaveItem(new Settings("SelectedCallibrationOption", BuildProperties.CallibrationInterval));
    }
    this.GoBack = (ICommand) new Command((Action) (() => Device.BeginInvokeOnMainThread((Action) (() => this._navigationService.Navigate<TabViewModel>((IMvxBundle) null, new CancellationToken())))));
    if (!UtilityFunctions.BridgeSettingExists())
    {
      CoreApp.settings.SaveItem(new Settings("BridgeOff", "true"));
      this.Bridge_feature = "false";
    }
    else
    {
      IReadOnlyList<Page> navigationStack = Application.Current.MainPage.Navigation.NavigationStack;
      this.nonsmmarrival = false;
      foreach (BindableObject bindableObject in (IEnumerable<Page>) navigationStack)
      {
        if (bindableObject.BindingContext.ToString().ToLower().Contains("nonsmmcon"))
          this.nonsmmarrival = true;
      }
      if (this.nonsmmarrival)
      {
        this.bridgeSwitched = true;
        this.Bridge_feature = "true";
        CoreApp.settings.UpdateItem(new Settings("BridgeOff", "false"));
      }
      else
        this.Bridge_feature = CoreApp.settings.GetItem("BridgeOff").Value.ToLower().Equals("false") ? "true" : "false";
    }
    MessagingCenter.Subscribe<CompactApplianceTesterViewModel>((object) this, "ClosingCatTesterPage", (Action<CompactApplianceTesterViewModel>) (sender => this.bridgeSwitched = false), (CompactApplianceTesterViewModel) null);
    MessagingCenter.Subscribe<ApplianceDatabaseViewModel>((object) this, "BridgeDisplaySwitched", (Action<ApplianceDatabaseViewModel>) (sender =>
    {
      this.bridgeSwitched = true;
      NavigationPage mainPage = (NavigationPage) Application.Current.MainPage;
      IReadOnlyList<Page> navigationStack = Application.Current.MainPage.Navigation.NavigationStack;
      Element parent = mainPage.Parent;
      int count = navigationStack.Count;
      TabbedPage u = (TabbedPage) mainPage.RootPage;
      int index = 0;
      Page catvm = (Page) null;
      IList<Page> children = u.Children;
      int num;
      if (sender.BridgeSettingSwitchToggled && this.vmtasks.Count == 3)
      {
        foreach (BindableObject bindableObject in (IEnumerable<Page>) children)
        {
          if (bindableObject.BindingContext.ToString().ToLower().Contains("catv"))
          {
            num = index;
            catvm = u.Children[index];
            break;
          }
          ++index;
        }
        if (catvm == null)
        {
          try
          {
            Device.BeginInvokeOnMainThread((Action) (() =>
            {
              u.Children.Clear();
              this.vmtasks.Clear();
              this.ShowInitialViewModels();
              this.NavigateToSettings();
            }));
          }
          catch (Exception ex)
          {
            string message = ex.Message;
          }
        }
        this.Bridge_feature = "true";
      }
      else if (sender.BridgeSettingSwitchToggled && this.vmtasks.Count == 4)
      {
        foreach (BindableObject bindableObject in (IEnumerable<Page>) children)
        {
          if (bindableObject.BindingContext.ToString().ToLower().Contains("catv"))
          {
            num = index;
            catvm = u.Children[index];
            break;
          }
          ++index;
        }
        if (catvm == null)
        {
          try
          {
            Device.BeginInvokeOnMainThread((Action) (() =>
            {
              u.Children.Clear();
              this.vmtasks.Clear();
              this.ShowInitialViewModels();
              this.NavigateToSettings();
            }));
          }
          catch (Exception ex)
          {
            string message = ex.Message;
          }
        }
        this.Bridge_feature = "true";
      }
      else
      {
        if (sender.BridgeSettingSwitchToggled || this.vmtasks.Count < 3)
          return;
        foreach (BindableObject child in (IEnumerable<Page>) u.Children)
        {
          if (child.BindingContext.ToString().ToLower().Contains("catv"))
          {
            num = index;
            catvm = u.Children[index];
            break;
          }
          ++index;
        }
        if (catvm != null)
          Task.Run((Action) (() => Device.BeginInvokeOnMainThread((Action) (() => u.Children.Remove(catvm)))));
        this.Bridge_feature = "false";
      }
    }), (ApplianceDatabaseViewModel) null);
  }

  public bool MetadataFailedHomePage
  {
    get => this._MetadataFailedHomePage;
    set
    {
      this._MetadataFailedHomePage = value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.MetadataFailedHomePage));
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

  public string ConnectionColor
  {
    get => this._ConnectionColor;
    internal set
    {
      this._ConnectionColor = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.ConnectionColor));
    }
  }

  public string NavTitle
  {
    get => this._NavTitle;
    internal set
    {
      this._NavTitle = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.NavTitle));
    }
  }

  public string Bridge_feature
  {
    get => this._Bridge_feature;
    set
    {
      this._Bridge_feature = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.Bridge_feature));
    }
  }

  public List<Task> vmtasks
  {
    get => this._vmtasks;
    set
    {
      this._vmtasks = value;
      this.RaisePropertyChanged<List<Task>>((Expression<Func<List<Task>>>) (() => this.vmtasks));
    }
  }

  public ICommand GoBack { internal set; get; }

  public ICommand PropagateEvent { internal set; get; }

  private bool bridgeSwitched { get; set; }

  public IEnumerable TabPages
  {
    get => this._Itsrc;
    set
    {
      this._Itsrc = value;
      this.RaisePropertyChanged<IEnumerable>((Expression<Func<IEnumerable>>) (() => this.TabPages));
    }
  }

  public override void ViewAppearing()
  {
    base.ViewAppearing();
    if (this._firstTime)
    {
      this.ShowInitialViewModels();
      this._firstTime = false;
      if (this.bridgeSwitched)
        this.bridgeSwitched = false;
    }
    if (this.MetadataFailedHomePage)
    {
      this.MetadataFailedHomePage = false;
      this._alertService.ShowMessageAlertWithKey("METADATA_DL_FAIL_MSG", AppResource.WARNING_TEXT);
    }
    if (!this._toSettingsTab)
      return;
    this.NavigateToSettings();
  }

  private void NavigateToSettings()
  {
    this._navigationService.ChangePresentation((MvxPresentationHint) new MvxPagePresentationHint(typeof (ApplianceDatabaseViewModel)), new CancellationToken());
  }

  public Task ShowInitialViewModels()
  {
    if (this.Bridge_feature.Equals("true"))
    {
      try
      {
        this.vmtasks = new List<Task>()
        {
          (Task) this._navigationService.Navigate<HomePageViewModel>((IMvxBundle) null, new CancellationToken()),
          (Task) this._navigationService.Navigate<CatViewModel>((IMvxBundle) null, new CancellationToken()),
          (Task) this._navigationService.Navigate<HistoryViewModel>((IMvxBundle) null, new CancellationToken()),
          (Task) this._navigationService.Navigate<ApplianceDatabaseViewModel, bool>(true, (IMvxBundle) null, new CancellationToken())
        };
      }
      catch (Exception ex)
      {
        string message = ex.Message;
      }
    }
    else
      this.vmtasks = new List<Task>()
      {
        (Task) this._navigationService.Navigate<HomePageViewModel>((IMvxBundle) null, new CancellationToken()),
        (Task) this._navigationService.Navigate<HistoryViewModel>((IMvxBundle) null, new CancellationToken()),
        (Task) this._navigationService.Navigate<ApplianceDatabaseViewModel, bool>(false, (IMvxBundle) null, new CancellationToken())
      };
    return Task.WhenAll((IEnumerable<Task>) this.vmtasks);
  }
}
