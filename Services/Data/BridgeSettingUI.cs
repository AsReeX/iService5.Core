// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Data.BridgeSettingUI
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Ssh.DTO;
using iService5.Ssh.enums;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

#nullable disable
namespace iService5.Core.Services.Data;

public class BridgeSettingUI : UIDataWithValidation
{
  private string stringValue;
  private bool boolValue;
  private string intValue;
  private bool isHex;
  private bool isZip;
  private bool isUrl;
  private bool isNum;

  public string Id { get; set; }

  public string Label { get; set; }

  public string StringValue
  {
    get => this.stringValue;
    set
    {
      this.stringValue = value;
      if (BridgeSettingsConstAndMethods.SettingsWithHexValue.Contains(this.Id))
      {
        if (!this.isHex)
          this.isHex = true;
        this.SetValidity(Regex.IsMatch(value, "\\A\\b(0[xX])?[0-9a-fA-F]+\\b\\Z") && Convert.ToInt32(value.Substring(2), 16 /*0x10*/) <= Convert.ToInt32("FF", 16 /*0x10*/));
      }
      else if (BridgeSettingsConstAndMethods.SettingsWithZipFilenameValue.Contains(this.Id))
      {
        if (!this.isZip)
          this.isZip = true;
        this.SetValidity(value.Length <= 16 /*0x10*/ && value.EndsWith(".zip") && Regex.IsMatch(value.Replace(".zip", ""), "^[a-zA-Z]+$"));
      }
      else if (BridgeSettingsConstAndMethods.SettingsWithUrlValue.Contains(this.Id))
      {
        if (!this.isUrl)
          this.isUrl = true;
        Uri result;
        this.SetValidity(Uri.TryCreate(value, UriKind.Absolute, out result) && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps));
      }
      else if (BridgeSettingsConstAndMethods.SettingsWithNumeralValue.Contains(this.Id))
      {
        if (!this.isNum)
          this.isNum = true;
        this.SetValidity(value.ToString().IsDigitsOnly());
      }
      else if (BridgeSettingsConstAndMethods.SettingsThatShouldNotBeEmpty.Contains(this.Id))
        this.SetValidity(!string.IsNullOrWhiteSpace(value));
      else
        this.SetValidity(true);
      this.RaisePropertyChanged(nameof (StringValue));
    }
  }

  public bool BoolValue
  {
    get => this.boolValue;
    set
    {
      this.boolValue = value;
      this.RaisePropertyChanged(nameof (BoolValue));
    }
  }

  public string IntValue
  {
    get => this.intValue;
    set
    {
      this.intValue = value;
      if (this.Type == SettingType.range.ToString())
      {
        int result;
        this.SetValidity(value.ToString().IsDigitsOnly() && int.TryParse(value, out result) && result >= this.Min && result <= this.Max);
      }
      else if (this.Type == SettingType.integer.ToString())
        this.SetValidity(value.ToString().IsDigitsOnly() && int.TryParse(value, out int _));
      this.RaisePropertyChanged(nameof (IntValue));
    }
  }

  public string Type { get; set; }

  public int Min { get; set; }

  public int Max { get; set; }

  public string Group { get; set; }

  public List<string> Options { get; set; }

  public bool IsRange { get; set; }

  public bool IsSelection { get; set; }

  public bool IsMultipleSelection { get; set; }

  public bool IsBoolean { get; set; }

  public bool IsText { get; set; }

  public bool IsInt { get; set; }

  public string HelpText { get; set; }

  public string typeBasedOnValue { get; set; }

  public bool IsAdvanced { get; set; }

  public bool IsVisible { get; set; }

  public BridgeSettingUI(BridgeSettingDto setting)
  {
    this.Id = setting.id;
    this.Label = setting.label;
    this.Type = setting.type;
    this.Group = setting.group;
    this.Options = setting.options;
    this.Min = (int) setting.min;
    this.Max = (int) setting.max;
    this.StringValue = setting.TryParseToString(this);
    this.BoolValue = setting.TryParseToBool(this);
    this.IntValue = setting.TryParseToInt(this);
    this.IsLocked = BridgeSettingsConstAndMethods.LockedSettings.Contains(this.Id);
    if (this.Type == SettingType.range.ToString())
    {
      this.IsRange = true;
      string str = AppResource.BRIDGE_SETTINGS_RANGE_HELP.Replace("{0}", this.Min.ToString());
      int max = this.Max;
      string newValue = max.ToString();
      this.HelpText = str.Replace("{1}", newValue);
      max = this.Max;
      this.EntryLength = max.ToString().Length;
    }
    else if (this.Type == SettingType.select.ToString() || this.Options != null && this.Options.Count > 0)
    {
      if (BridgeSettingsConstAndMethods.SettingsWithMultiSelect.Contains(this.Id))
        this.IsMultipleSelection = true;
      else
        this.IsSelection = true;
      this.SetValidity(true);
    }
    else if (this.Type == SettingType.boolean.ToString() || this.typeBasedOnValue == SettingType.boolean.ToString())
    {
      this.IsBoolean = true;
      this.SetValidity(true);
    }
    else if (this.Type == SettingType.text.ToString() || this.typeBasedOnValue == SettingType.text.ToString())
    {
      this.IsText = true;
      this.HelpText = !this.isHex ? (!this.isZip ? (!this.isUrl ? (!this.isNum ? AppResource.BRIDGE_SETTINGS_EMPTY_HELP : AppResource.BRIDGE_SETTINGS_NUMBER_HELP) : AppResource.BRIDGE_SETTINGS_URL_HELP) : AppResource.BRIDGE_SETTINGS_ZIP_HELP.Replace("{0}", 12.ToString())) : AppResource.BRIDGE_SETTINGS_HEX_HELP.Replace("{0}", "FF");
      this.EntryLength = int.MaxValue;
    }
    else if (this.Type == SettingType.integer.ToString() || this.typeBasedOnValue == SettingType.integer.ToString())
    {
      this.IsInt = true;
      this.HelpText = AppResource.BRIDGE_SETTINGS_INTEGER_HELP.Replace("{0}", int.MaxValue.ToString());
      this.EntryLength = int.MaxValue;
    }
    else
      this.Type = SettingType.unknown.ToString();
    this.IsAdvanced = BridgeSettingsConstAndMethods.AdvancedSettings.Contains(this.Group);
    this.IsVisible = !this.IsAdvanced;
    this.ColorEnumberField();
  }
}
