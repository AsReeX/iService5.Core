// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Data.module_downloaded
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using SQLite;

#nullable disable
namespace iService5.Core.Services.Data;

public class module_downloaded
{
  public module_downloaded()
  {
    this.fileName = "";
    this.fileId = "";
    this.md5 = "";
    this.fileSize = 0L;
  }

  public module_downloaded(string fileName, string fileId, string md5)
  {
    this.fileName = fileName;
    this.fileId = fileId;
    this.md5 = md5;
  }

  [PrimaryKey]
  public string fileName { get; set; }

  public string fileId { get; set; }

  public string md5 { get; set; }

  public long fileSize { get; set; }

  public string toString() => $"{this.fileName} {this.fileId} {this.md5}";
}
