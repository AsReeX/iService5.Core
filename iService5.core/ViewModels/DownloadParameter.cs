// Decompiled with JetBrains decompiler
// Type: iService5.Core.ViewModels.DownloadParameter
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

#nullable disable
namespace iService5.Core.ViewModels;

public class DownloadParameter
{
  public bool bridgeSettingSwitchToggled { get; set; }

  public bool downloadSettingsConfigured { get; set; }

  public int numberOfFilesToBeDownloaded { get; set; }

  public string fileSizeToBeDownloaded { get; set; }
}
