// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Data.DownloadFilesStatistics
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

#nullable disable
namespace iService5.Core.Services.Data;

public class DownloadFilesStatistics
{
  public int fileCount { get; set; }

  public long fileSize { get; set; }

  public DownloadFilesStatistics()
  {
    this.fileCount = 0;
    this.fileSize = 0L;
  }

  public DownloadFilesStatistics(int _fileCount, long _fileSize)
  {
    this.fileCount = _fileCount;
    this.fileSize = _fileSize;
  }

  public bool isEmpty() => this.fileCount == 0 && this.fileSize == 0L;
}
