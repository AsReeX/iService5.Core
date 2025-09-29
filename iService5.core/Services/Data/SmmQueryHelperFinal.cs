// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Data.SmmQueryHelperFinal
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using SQLite;

#nullable disable
namespace iService5.Core.Services.Data;

public class SmmQueryHelperFinal
{
  [Column("moduleid")]
  public long Moduleid { get; set; }

  [Column("version")]
  public string Version { get; set; }

  [Column("vib")]
  public string Vib { get; set; }

  [Column("ki")]
  public string Ki { get; set; }

  [Column("configid")]
  public string ConfigId { get; set; }

  [Column("devicetype")]
  public string DeviceType { get; set; }

  [Column("node")]
  public int Node { get; set; }

  [Column("name")]
  public string Name { get; set; }

  [Column("fileId")]
  public string FileId { get; set; }

  [Column("md5")]
  public string MD5 { get; set; }

  [Column("message")]
  public string Message { get; set; }

  [Column("package")]
  public string Package { get; set; }

  [Column("fileSize")]
  public long FileSize { get; set; }

  [Column("type")]
  public string Type { get; set; }

  public string toString()
  {
    return $"{this.Vib}/{this.Ki} {this.Moduleid.ToString()} {this.Version} {this.ConfigId} {this.DeviceType} {this.Node.ToString()} {this.Name} {this.FileId} {this.MD5} {this.Message} {this.Package} {this.Type}";
  }
}
