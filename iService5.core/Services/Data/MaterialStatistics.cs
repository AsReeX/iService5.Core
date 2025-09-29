// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Data.MaterialStatistics
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

#nullable disable
namespace iService5.Core.Services.Data;

public class MaterialStatistics
{
  public string material { get; set; }

  public int fileCount { get; set; }

  public long fileSize { get; set; }

  public string deviceClass { get; set; }

  public string country { get; set; }

  public string PPFRefetchStatus { get; set; }

  public MaterialStatistics()
  {
    this.material = "";
    this.fileCount = 0;
    this.fileSize = 0L;
    this.deviceClass = "";
    this.country = "";
    this.PPFRefetchStatus = "FALSE";
  }

  public MaterialStatistics(
    string _material,
    int _fileCount,
    long _fileSize,
    string _deviceClass,
    string _country,
    string _PPFRefetchStatus)
  {
    this.material = _material;
    this.fileCount = _fileCount;
    this.fileSize = _fileSize;
    this.deviceClass = _deviceClass;
    this.country = _country;
    this.PPFRefetchStatus = _PPFRefetchStatus;
  }
}
