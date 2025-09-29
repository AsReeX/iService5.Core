// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Helpers.AlertService
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.User.ActivityLogging;
using System;
using System.Globalization;
using System.Resources;
using System.Threading.Tasks;
//using Xamarin.Forms;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

#nullable disable
namespace iService5.Core.Services.Helpers;

public class AlertService : IAlertService
{
  private readonly ILoggingService _loggingService;
  private readonly ResourceManager _enRM;
  private readonly CultureInfo _enCultureInfo;

  public AlertService(ILoggingService loggingService)
  {
    this._loggingService = loggingService;
    this._enRM = new ResourceManager("iService5.Core.AppResource", typeof (AppResource).Assembly);
    this._enCultureInfo = CultureInfo.GetCultureInfo("en");
  }

  public string GetEnValue(string resourceKey)
  {
    return this._enRM.GetString(resourceKey, this._enCultureInfo) ?? resourceKey;
  }

  public async Task ShowErrorAlertWithKey(
    string resourceKey,
    string title,
    string buttonText,
    Action hideCallback)
  {
    string message = this.GetEnValue(resourceKey);
    this._loggingService.getLogger().LogAppInformation(LoggingContext.USERSESSION, "Error popup appeared with message: " + message, memberName: nameof (ShowErrorAlertWithKey), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/AlertService.cs", sourceLineNumber: 37);
    await Application.Current.MainPage.DisplayAlert(title, AppResource.ResourceManager.GetString(resourceKey) ?? resourceKey, buttonText);
    if (hideCallback == null)
    {
      message = (string) null;
    }
    else
    {
      hideCallback();
      message = (string) null;
    }
  }

  public async Task ShowErrorAlert(
    Exception error,
    string title,
    string buttonText,
    Action hideCallback)
  {
    this._loggingService.getLogger().LogAppInformation(LoggingContext.USERSESSION, "Error popup appeared with exception: " + error.Message, memberName: nameof (ShowErrorAlert), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/AlertService.cs", sourceLineNumber: 55);
    await Application.Current.MainPage.DisplayAlert(title, error.Message, buttonText);
    if (hideCallback == null)
      return;
    hideCallback();
  }

  public Task ShowMessageAlertWithKey(string resourceKey, string title)
  {
    Device.BeginInvokeOnMainThread((Action) (async () =>
    {
      string message = this.GetEnValue(resourceKey);
      this._loggingService.getLogger().LogAppInformation(LoggingContext.USERSESSION, "Alert popup appeared with message: " + message, memberName: nameof (ShowMessageAlertWithKey), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/AlertService.cs", sourceLineNumber: 74);
      await Application.Current.MainPage.DisplayAlert(title, AppResource.ResourceManager.GetString(resourceKey) ?? resourceKey, AppResource.WARNING_OK);
      message = (string) null;
    }));
    return Task.CompletedTask;
  }

  public Task ShowMessageAlertWithMessage(string message, string title)
  {
    Device.BeginInvokeOnMainThread((Action) (async () =>
    {
      this._loggingService.getLogger().LogAppInformation(LoggingContext.USERSESSION, "Alert popup appeared with message: " + message, memberName: nameof (ShowMessageAlertWithMessage), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/AlertService.cs", sourceLineNumber: 89);
      await Application.Current.MainPage.DisplayAlert(title, message, AppResource.WARNING_OK);
    }));
    return Task.CompletedTask;
  }

  public async Task ShowMessageAlertWithKey(
    string resourceKey,
    string title,
    string buttonText,
    Action hideCallback)
  {
    string message = this.GetEnValue(resourceKey);
    this._loggingService.getLogger().LogAppInformation(LoggingContext.USERSESSION, "Alert popup appeared with message: " + message, memberName: nameof (ShowMessageAlertWithKey), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/AlertService.cs", sourceLineNumber: 106);
    await Application.Current.MainPage.DisplayAlert(title, AppResource.ResourceManager.GetString(resourceKey) ?? resourceKey, buttonText);
    if (hideCallback == null)
    {
      message = (string) null;
    }
    else
    {
      hideCallback();
      message = (string) null;
    }
  }

  public async Task<bool> ShowMessageAlertWithKey(
    string resourceKey,
    string title,
    string buttonOKText,
    string buttonCancelText,
    Action<bool> hideCallback)
  {
    string message = this.GetEnValue(resourceKey);
    this._loggingService.getLogger().LogAppInformation(LoggingContext.USERSESSION, "User action popup appeared with message: " + message, memberName: nameof (ShowMessageAlertWithKey), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/AlertService.cs", sourceLineNumber: 126);
    bool result = await Application.Current.MainPage.DisplayAlert(title, AppResource.ResourceManager.GetString(resourceKey) ?? resourceKey, buttonOKText, buttonCancelText);
    if (hideCallback != null)
      hideCallback(result);
    bool flag = result;
    message = (string) null;
    return flag;
  }

  public async Task<bool> ShowMessageAlertWithMessage(
    string message,
    string title,
    string buttonOKText,
    string buttonCancelText,
    Action<bool> hideCallback)
  {
    this._loggingService.getLogger().LogAppInformation(LoggingContext.USERSESSION, "User action popup appeared with message: " + message, memberName: nameof (ShowMessageAlertWithMessage), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/AlertService.cs", sourceLineNumber: 147);
    bool result = await Application.Current.MainPage.DisplayAlert(title, message, buttonOKText, buttonCancelText);
    if (hideCallback != null)
      hideCallback(result);
    return result;
  }

  public async Task ShowMessageAlertBoxWithKey(string resourceKey, string title)
  {
    string message = this.GetEnValue(resourceKey);
    this._loggingService.getLogger().LogAppInformation(LoggingContext.USERSESSION, "Alert box popup appeared with message: " + message, memberName: nameof (ShowMessageAlertBoxWithKey), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/AlertService.cs", sourceLineNumber: 166);
    await Application.Current.MainPage.DisplayAlert(title, AppResource.ResourceManager.GetString(resourceKey) ?? resourceKey, AppResource.WARNING_OK);
    message = (string) null;
  }

  public async Task ShowMessageAlertBoxWithMessage(string message, string title)
  {
    this._loggingService.getLogger().LogAppInformation(LoggingContext.USERSESSION, "Alert box popup appeared with message: " + message, memberName: nameof (ShowMessageAlertBoxWithMessage), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/AlertService.cs", sourceLineNumber: 177);
    await Application.Current.MainPage.DisplayAlert(title, message, AppResource.WARNING_OK);
  }

  public void ShowMessageAlertWithKeyFromService(
    string resourceKey,
    string title,
    string buttonOKText,
    string buttonCancelText,
    Action<bool> hideCallback)
  {
    Device.BeginInvokeOnMainThread((Action) (async () =>
    {
      string message = this.GetEnValue(resourceKey);
      this._loggingService.getLogger().LogAppInformation(LoggingContext.USERSESSION, "Service User action popup appeared with message: " + message, memberName: nameof (ShowMessageAlertWithKeyFromService), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/AlertService.cs", sourceLineNumber: 189);
      bool result = await Application.Current.MainPage.DisplayAlert(title, AppResource.ResourceManager.GetString(resourceKey) ?? resourceKey, buttonOKText, buttonCancelText);
      Action<bool> action = hideCallback;
      if (action == null)
      {
        message = (string) null;
      }
      else
      {
        action(result);
        message = (string) null;
      }
    }));
  }

  public void ShowMessageAlertWithMessageFromService(
    string message,
    string title,
    string buttonOKText,
    string buttonCancelText,
    Action<bool> hideCallback)
  {
    Device.BeginInvokeOnMainThread((Action) (async () =>
    {
      this._loggingService.getLogger().LogAppInformation(LoggingContext.USERSESSION, "Service User action popup appeared with message: " + message, memberName: nameof (ShowMessageAlertWithMessageFromService), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/AlertService.cs", sourceLineNumber: 204);
      bool result = await Application.Current.MainPage.DisplayAlert(title, message, buttonOKText, buttonCancelText);
      Action<bool> action = hideCallback;
      if (action == null)
        return;
      action(result);
    }));
  }

  public void ShowMessageAlertWithKeyFromService(
    string resourceKey,
    string title,
    Action hideCallback)
  {
    Device.BeginInvokeOnMainThread((Action) (async () =>
    {
      string message = this.GetEnValue(resourceKey);
      this._loggingService.getLogger().LogAppInformation(LoggingContext.USERSESSION, "Service alert popup appeared with message: " + message, memberName: nameof (ShowMessageAlertWithKeyFromService), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/AlertService.cs", sourceLineNumber: 223);
      await Application.Current.MainPage.DisplayAlert(title, AppResource.ResourceManager.GetString(resourceKey) ?? resourceKey, AppResource.WARNING_OK);
      Action action = hideCallback;
      if (action == null)
      {
        message = (string) null;
      }
      else
      {
        action();
        message = (string) null;
      }
    }));
  }

  public void ShowMessageAlertWithMessageFromService(
    string message,
    string title,
    Action hideCallback)
  {
    Device.BeginInvokeOnMainThread((Action) (async () =>
    {
      this._loggingService.getLogger().LogAppInformation(LoggingContext.USERSESSION, "Service alert popup appeared with message: " + message, memberName: nameof (ShowMessageAlertWithMessageFromService), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/AlertService.cs", sourceLineNumber: 239);
      await Application.Current.MainPage.DisplayAlert(title, message, AppResource.WARNING_OK);
      Action action = hideCallback;
      if (action == null)
        return;
      action();
    }));
  }

  public void ShowMessageAlertWithMessageNoLog(string message, string title, Action hideCallback)
  {
    Device.BeginInvokeOnMainThread((Action) (async () =>
    {
      await Application.Current.MainPage.DisplayAlert(title, message, AppResource.WARNING_OK);
      Action action = hideCallback;
      if (action == null)
        return;
      action();
    }));
  }

  public void ShowActionSheet(
    string message,
    string title,
    string cancel,
    string[] buttons,
    Action<string> actionSheetCallback)
  {
    Device.BeginInvokeOnMainThread((Action) (async () =>
    {
      string action = await Application.Current.MainPage.DisplayActionSheet($"{title}\n{message}", cancel, (string) null, buttons);
      Action<string> action1 = actionSheetCallback;
      if (action1 == null)
      {
        action = (string) null;
      }
      else
      {
        action1(action);
        action = (string) null;
      }
    }));
  }
}
