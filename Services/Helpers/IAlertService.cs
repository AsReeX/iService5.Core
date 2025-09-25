// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Helpers.IAlertService
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using System;
using System.Threading.Tasks;

#nullable disable
namespace iService5.Core.Services.Helpers;

public interface IAlertService
{
  Task ShowErrorAlertWithKey(
    string resourceKey,
    string title,
    string buttonText,
    Action hideCallback);

  Task ShowErrorAlert(Exception error, string title, string buttonText, Action hideCallback);

  Task ShowMessageAlertWithKey(string resourceKey, string title);

  Task ShowMessageAlertWithMessage(string message, string title);

  Task ShowMessageAlertWithKey(
    string resourceKey,
    string title,
    string buttonText,
    Action hideCallback);

  void ShowMessageAlertWithKeyFromService(
    string resourceKey,
    string title,
    string buttonOKText,
    string buttonCancelText,
    Action<bool> hideCallback);

  void ShowMessageAlertWithMessageFromService(
    string message,
    string title,
    string buttonOKText,
    string buttonCancelText,
    Action<bool> hideCallback);

  Task<bool> ShowMessageAlertWithKey(
    string resourceKey,
    string title,
    string buttonOKText,
    string buttonCancelText,
    Action<bool> hideCallback);

  Task<bool> ShowMessageAlertWithMessage(
    string message,
    string title,
    string buttonOKText,
    string buttonCancelText,
    Action<bool> hideCallback);

  Task ShowMessageAlertBoxWithKey(string resourceKey, string title);

  Task ShowMessageAlertBoxWithMessage(string message, string title);

  void ShowMessageAlertWithKeyFromService(string resourceKey, string title, Action hideCallback);

  void ShowMessageAlertWithMessageFromService(string message, string title, Action hideCallback);

  void ShowMessageAlertWithMessageNoLog(string message, string title, Action hideCallback);

  string GetEnValue(string resourceKey);

  void ShowActionSheet(
    string message,
    string title,
    string cancel,
    string[] buttons,
    Action<string> actionSheetCallback);
}
