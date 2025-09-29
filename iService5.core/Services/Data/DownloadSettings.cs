// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Data.DownloadSettings
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

#nullable disable
namespace iService5.Core.Services.Data;

public class DownloadSettings
{
  public string key { get; set; }

  public string option { get; set; }

  public bool isRecommended { get; set; }

  public string description { get; set; }

  public bool isSelected { get; set; }

  public DownloadSettings(
    string key,
    string option,
    bool isRecommended,
    string description,
    bool isSelected)
  {
    this.key = key;
    this.option = option;
    this.isRecommended = isRecommended;
    this.description = description;
    this.isSelected = isSelected;
  }
}
