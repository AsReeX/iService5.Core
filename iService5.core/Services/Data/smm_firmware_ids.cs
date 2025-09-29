// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Data.smm_firmware_ids
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using SQLite;

#nullable disable
namespace iService5.Core.Services.Data;

public class smm_firmware_ids
{
  [Column("id")]
  [PrimaryKey]
  public long Id { get; set; }

  [Column("package")]
  public string Package { get; set; }

  [Column("model")]
  public string Model { get; set; }
}
