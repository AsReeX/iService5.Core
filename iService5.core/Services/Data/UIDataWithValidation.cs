// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Data.UIDataWithValidation
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using System;
using System.ComponentModel;
//using Xamarin.Forms;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

#nullable disable
namespace iService5.Core.Services.Data;

public class UIDataWithValidation : INotifyPropertyChanged
{
  private Color entryColor = Colors.Black;
  private bool isValid;

  public static Action<bool> OnDataValidationChange { get; set; }

  public static Action<string, string> OnPropertyValueChange { get; set; }

  public event PropertyChangedEventHandler PropertyChanged;

  protected void RaisePropertyChanged(string propertyName = "")
  {
    PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
    if (propertyChanged != null)
      propertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    if (!(this is BridgeSettingUI) || !(propertyName != "IsValid"))
      return;
    PropertyChangedEventArgs changedEventArgs = new PropertyChangedEventArgs(propertyName);
    BridgeSettingUI bridgeSettingsUI = (BridgeSettingUI) this;
    UIDataWithValidation.OnPropertyValueChange(bridgeSettingsUI.Id, this.GetChangedValue(bridgeSettingsUI, propertyName));
  }

  public string GetChangedValue(BridgeSettingUI bridgeSettingsUI, string propertyName)
  {
    BridgeSettingUI bridgeSettingUi = bridgeSettingsUI;
    return !bridgeSettingUi.IsBoolean ? (!bridgeSettingUi.IsBoolean && !bridgeSettingUi.IsInt && !bridgeSettingUi.IsRange || bridgeSettingUi.IntValue == null ? (!bridgeSettingUi.IsText && !bridgeSettingUi.IsSelection || !(bridgeSettingUi.StringValue != "") ? "" : bridgeSettingUi.StringValue.ToString()) : bridgeSettingUi.IntValue.ToString()) : bridgeSettingUi.BoolValue.ToString();
  }

  public bool IsLocked { get; set; }

  public Color EntryBackColor { get; set; }

  public int EntryLength { get; set; }

  public Color EntryColor
  {
    get => this.entryColor;
    set
    {
      if (!(this.entryColor != value))
        return;
      this.entryColor = value;
      this.RaisePropertyChanged(nameof (EntryColor));
    }
  }

  public bool IsValid
  {
    get => this.isValid;
    set
    {
      if (this.isValid != value)
      {
        this.isValid = value;
        UIDataWithValidation.OnDataValidationChange(value);
      }
      this.RaisePropertyChanged(nameof (IsValid));
    }
  }
}
