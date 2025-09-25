// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Data.DownloadProxy
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

#nullable disable
namespace iService5.Core.Services.Data;

public class DownloadProxy
{
  internal string fileExtension;
  public PpfsInfo PPFData;

  public module Module { get; set; }

  public document Document { get; set; }

  public enumber_modules EnumberModule { get; set; }

  public FileType FileType { get; set; }

  public string vib { get; set; }

  public string ki { get; set; }

  public int node { get; set; }

  public long moduleId
  {
    get
    {
      return this.Module != null ? this.Module.moduleid : (this.EnumberModule != null ? this.EnumberModule.moduleid : 0L);
    }
  }

  public string version
  {
    get
    {
      return this.Module != null ? this.Module.version : (this.EnumberModule != null ? this.EnumberModule.version : (string) null);
    }
  }

  public string GetFileId()
  {
    return this.Module?.fileId ?? this.Document?.fileId ?? this.EnumberModule?.fileId ?? (string) null;
  }

  public string GetMD5()
  {
    return this.Module?.md5 ?? this.Document?.md5 ?? this.EnumberModule?.md5 ?? (string) null;
  }

  public string GetModuleBinaryFileName()
  {
    return this.Module?.toFileName() ?? this.EnumberModule?.toFileName() ?? (string) null;
  }

  public string ToFileName()
  {
    return this.Module?.toFileNameNewFdsExtension("") ?? this.Document?.toFileName() ?? this.EnumberModule?.toFileNameNewFdsExtension("") ?? (string) null;
  }

  public string ToFileNameNewFdsExtension(string binType)
  {
    return this.Module?.toFileNameNewFdsExtension(binType) ?? this.Document?.toFileName() ?? this.EnumberModule?.toFileNameNewFdsExtension(binType) ?? (string) null;
  }

  public long GetFileSize()
  {
    module module = this.Module;
    long fileSize;
    if (module == null)
    {
      document document = this.Document;
      if (document == null)
      {
        enumber_modules enumberModule = this.EnumberModule;
        fileSize = enumberModule != null ? enumberModule.fileSize : 0L;
      }
      else
        fileSize = document.fileSize;
    }
    else
      fileSize = module.fileSize;
    return fileSize;
  }

  public bool ShouldCheckForPpfInfoFile()
  {
    return (this.EnumberModule != null || this.Module != null) && this.moduleId != 0L && !string.IsNullOrEmpty(this.version) && !string.IsNullOrEmpty(this.vib) && !string.IsNullOrEmpty(this.ki);
  }

  public bool ShouldCheckForBundlePpfInfoFile()
  {
    return !string.IsNullOrEmpty(this.vib) && !string.IsNullOrEmpty(this.ki);
  }

  public string GetPpfId()
  {
    if (!this.ShouldCheckForPpfInfoFile())
      return (string) null;
    return $"{this.vib}_{this.ki}_{this.moduleId}_{this.version}";
  }

  public string GetBundledPpfId() => $"{this.vib}_{this.ki}";

  public string GetEnumber() => this.ki == "" ? this.vib : $"{this.vib}/{this.ki}";
}
