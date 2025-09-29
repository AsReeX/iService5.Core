// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Data.CustomMemoryStream
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using System.IO;

#nullable disable
namespace iService5.Core.Services.Data;

public class CustomMemoryStream : MemoryStream
{
  private int size;
  private readonly progressCallback _cb;

  public CustomMemoryStream(progressCallback cb)
  {
    this.size = 0;
    this._cb = cb;
  }

  public override int Read(byte[] buffer, int offset, int count)
  {
    this._cb($"({((float) (100.0 * ((double) this.Position / (double) this.size))).ToString("0")}%)");
    return base.Read(buffer, offset, count);
  }

  public override void Write(byte[] buffer, int offset, int count)
  {
    base.Write(buffer, offset, count);
    this.size += count;
  }
}
