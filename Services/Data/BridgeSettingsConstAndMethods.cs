// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Data.BridgeSettingsConstAndMethods
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Ssh.DTO;
using iService5.Ssh.enums;
using System.Collections.Generic;
using System.Collections.ObjectModel;

#nullable disable
namespace iService5.Core.Services.Data;

public static class BridgeSettingsConstAndMethods
{
  public const int MaxEntryLength = 2147483647 /*0x7FFFFFFF*/;
  public const int MaxIntValue = 2147483647 /*0x7FFFFFFF*/;
  public const int MaxFilenameLength = 16 /*0x10*/;
  public const string MaxHexValue = "FF";
  public static readonly IList<string> LockedSettings = (IList<string>) new ReadOnlyCollection<string>((IList<string>) new List<string>());
  public static readonly IList<string> SettingsThatShouldNotBeEmpty = (IList<string>) new ReadOnlyCollection<string>((IList<string>) new List<string>());
  public static readonly IList<string> SettingsWithMultiSelect = (IList<string>) new ReadOnlyCollection<string>((IList<string>) new List<string>()
  {
    "iApplianceSelection",
    "iRegionalSelection"
  });
  public static readonly IList<string> SettingsWithHexValue = (IList<string>) new ReadOnlyCollection<string>((IList<string>) new List<string>());
  public static readonly IList<string> SettingsWithZipFilenameValue = (IList<string>) new ReadOnlyCollection<string>((IList<string>) new List<string>());
  public static readonly IList<string> SettingsWithUrlValue = (IList<string>) new ReadOnlyCollection<string>((IList<string>) new List<string>());
  public static readonly IList<string> SettingsWithNumeralValue = (IList<string>) new ReadOnlyCollection<string>((IList<string>) new List<string>());
  public static readonly IList<string> AdvancedSettings = (IList<string>) new ReadOnlyCollection<string>((IList<string>) new List<string>()
  {
    "Wi-Fi",
    "DATA FILES SETUP",
    "HIP",
    "DBUS",
    "SMM"
  });
  public static readonly IList<string> HiddenSettings = (IList<string>) new ReadOnlyCollection<string>((IList<string>) new List<string>()
  {
    "PayPerUse",
    "Tester"
  });

  public static string TryParseToString(this BridgeSettingDto item, BridgeSettingUI uiItem)
  {
    try
    {
      string toString = (string) item.value;
      uiItem.typeBasedOnValue = SettingType.text.ToString();
      return toString;
    }
    catch
    {
      return (string) null;
    }
  }

  public static bool TryParseToBool(this BridgeSettingDto item, BridgeSettingUI uiItem)
  {
    bool result;
    if (bool.TryParse(item.value.ToString(), out result))
      uiItem.typeBasedOnValue = SettingType.boolean.ToString();
    return result;
  }

  public static string TryParseToInt(this BridgeSettingDto item, BridgeSettingUI uiItem)
  {
    try
    {
      int num = (int) (long) item.value;
      uiItem.typeBasedOnValue = SettingType.integer.ToString();
      return num.ToString();
    }
    catch
    {
      return (string) null;
    }
  }
}
