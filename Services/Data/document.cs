// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Data.document
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using SQLite;

#nullable disable
namespace iService5.Core.Services.Data;

public class document
{
  [Column("document")]
  public long _document { get; set; }

  public string version { get; set; }

  public string type { get; set; }

  public string fileId { get; set; }

  public string md5 { get; set; }

  public long fileSize { get; set; }

  public string harnessOK { get; set; }

  public bool IsDownloaded => this.harnessOK == "TRUE";

  public string toFileName() => $"{this._document.ToString()}.{this.version}.bin";
}
