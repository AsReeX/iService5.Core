// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Data.enumber_modules
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

#nullable disable
namespace iService5.Core.Services.Data;

public class enumber_modules
{
  public document Document { get; private set; }

  public long moduleid { get; set; }

  public string version { get; set; }

  public int node { get; set; }

  public string fileId { get; set; }

  public string md5 { get; set; }

  public long fileSize { get; set; }

  public string type { get; set; }

  public string vib { get; set; }

  public string ki { get; set; }

  public string harnessOK { get; set; }

  public bool IsDownloaded => this.harnessOK == "TRUE";

  public string toFileName() => $"{this.moduleid.ToString()}.{this.version}.bin";

  public string toFileNameNewFdsExtension(string fileExtension)
  {
    return $"{this.moduleid.ToString()}.{this.version}{fileExtension}";
  }

  public enumber_modules()
  {
  }

  public enumber_modules(
    long _modId,
    string _version,
    int _node,
    string _name,
    string _fileId,
    string _md5,
    long _fileSize,
    string _type)
  {
    this.Document = (document) null;
    this.moduleid = _modId;
    this.version = _version;
    this.node = _node;
    this.fileId = _fileId;
    this.md5 = _md5;
    this.fileSize = _fileSize;
    this.type = _type;
  }

  public enumber_modules(
    long _modId,
    string _version,
    int _node,
    string _name,
    string _fileId,
    string _md5,
    document doc,
    long _fileSize,
    string _type,
    string _harnessOK)
  {
    this.Document = doc;
    this.moduleid = _modId;
    this.version = _version;
    this.node = _node;
    this.fileId = _fileId;
    this.md5 = _md5;
    this.fileSize = _fileSize;
    this.type = _type;
    this.harnessOK = _harnessOK;
  }
}
