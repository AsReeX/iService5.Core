// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Data.SyMaNa.SyMaNaFirmwareUploadModel
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Backend;

#nullable disable
namespace iService5.Core.Services.Data.SyMaNa;

public class SyMaNaFirmwareUploadModel
{
  public string path { get; set; }

  public string FileName { get; set; }

  public long module { get; set; }

  public long fileSize { get; set; }

  public string version { get; set; }

  public string ppfFile { get; set; }

  public string ppfID { get; set; }

  public string uploadURL { get; set; }

  public SyMaNaWebClientResponse response { get; set; }
}
