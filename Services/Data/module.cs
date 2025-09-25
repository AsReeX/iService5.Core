// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Data.module
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

#nullable disable
namespace iService5.Core.Services.Data;

public class module
{
  public long moduleid { get; set; }

  public document Document { get; set; }

  public string version { get; set; }

  public int node { get; set; }

  public string name { get; set; }

  public string fileId { get; set; }

  public string md5 { get; set; }

  public long fileSize { get; set; }

  public string type { get; set; }

  public string harnessOK { get; set; }

  public string architecture { get; set; }

  public bool IsDownloaded => this.harnessOK == "TRUE";

  public module()
  {
    this.moduleid = 0L;
    this.version = "";
    this.node = 0;
    this.name = "";
    this.fileId = "";
    this.md5 = "";
    this.fileSize = 0L;
    this.type = "";
    this.harnessOK = "FALSE";
  }

  public module(module _module)
  {
    this.moduleid = _module.moduleid;
    this.version = _module.version;
    this.node = _module.node;
    this.name = _module.name;
    this.fileId = _module.fileId;
    this.md5 = _module.md5;
    this.fileSize = _module.fileSize;
    this.type = _module.type;
    this.harnessOK = _module.harnessOK;
    this.node = _module.node;
  }

  public module(
    long _modId,
    string _version,
    int _node,
    string _name,
    string _fileId,
    string _md5,
    long _fileSize,
    string _type)
  {
    this.moduleid = _modId;
    this.version = _version;
    this.node = _node;
    this.name = _name;
    this.fileId = _fileId;
    this.md5 = _md5;
    this.fileSize = _fileSize;
    this.type = _type;
  }

  public override string ToString()
  {
    return $"{this.moduleid.ToString()} {this.version} {this.node.ToString()} {this.name} {this.fileId} {this.md5}";
  }

  public string toFileName() => $"{this.moduleid.ToString()}.{this.version}.bin";

  public string toFileNameNewFdsExtension(string fileExtension)
  {
    return $"{this.moduleid.ToString()}.{this.version}{fileExtension}";
  }
}
