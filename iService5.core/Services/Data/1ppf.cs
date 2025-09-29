// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Data.ppf
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using SQLite;

#nullable disable
namespace iService5.Core.Services.Data;

public class ppf
{
  public string enumber { get; set; }

  [Column("ppfid")]
  public string ppfid { get; set; }

  [Column("vib")]
  public string vib { get; set; }

  [Column("ki")]
  public string ki { get; set; }

  [Column("moduleid")]
  public long moduleid { get; set; }

  [Column("version")]
  public string version { get; set; }

  [Column("expirydate")]
  public long expirydate { get; set; }

  [Column("type")]
  public long type { get; set; }

  [Column("ca")]
  public string ca { get; set; }

  [Column("ppffile")]
  public string ppffile { get; set; }

  public DownloadProxy GetDownloadProxyFromPpf()
  {
    return new DownloadProxy()
    {
      EnumberModule = new enumber_modules()
      {
        moduleid = this.moduleid,
        version = this.version
      },
      ki = this.ki,
      vib = this.vib
    };
  }
}
