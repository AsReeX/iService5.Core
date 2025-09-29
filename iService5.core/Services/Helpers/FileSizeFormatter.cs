// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Helpers.FileSizeFormatter
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using System;

#nullable disable
namespace iService5.Core.Services.Helpers;

public static class FileSizeFormatter
{
  private static readonly string[] SizeSuffixes = new string[9]
  {
    "bytes",
    "KB",
    "MB",
    "GB",
    "TB",
    "PB",
    "EB",
    "ZB",
    "YB"
  };

  public static string FormatSize(long value)
  {
    int decimals = 1;
    if (value < 0L)
      return "-" + FileSizeFormatter.FormatSize(-value);
    if (value == 0L)
      return string.Format($"{{0:n{decimals.ToString()}}} bytes", (object) 0);
    int index = (int) Math.Log((double) value, 1024.0);
    Decimal d = (Decimal) value / (Decimal) (1L << index * 10);
    if (Math.Round(d, decimals) >= 1000M)
    {
      ++index;
      d /= 1024M;
    }
    return string.Format($"{{0:n{decimals.ToString()}}} {{1}}", (object) d, (object) FileSizeFormatter.SizeSuffixes[index]);
  }
}
