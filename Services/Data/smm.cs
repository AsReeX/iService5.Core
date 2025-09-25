// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Data.smm
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

#nullable disable
namespace iService5.Core.Services.Data;

public class smm
{
  public string vib { get; set; }

  public string ki { get; set; }

  public string configid { get; set; }

  public string deviceType { get; set; }

  public string deviceTypeCode { get; set; }

  public string architecture { get; set; }

  public string toString()
  {
    return $"{this.vib} {this.ki} {this.configid} {this.deviceType} {this.deviceTypeCode}";
  }
}
