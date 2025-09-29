// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Data.smm_whitelist
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using SQLite;

#nullable disable
namespace iService5.Core.Services.Data;

public class smm_whitelist
{
  [Column("moduleid")]
  public long moduleid { get; set; }

  [Column("version_min")]
  public string version_min { get; set; }

  [Column("version_max")]
  public string version_max { get; set; }

  [Column("available_min")]
  public string available_min { get; set; }

  [Column("available_max")]
  public string available_max { get; set; }

  public override string ToString()
  {
    return $"{this.moduleid.ToString()} {this.version_min} {this.version_max} {this.available_min} {this.available_max}";
  }
}
