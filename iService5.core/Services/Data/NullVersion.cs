// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Data.NullVersion
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

#nullable disable
namespace iService5.Core.Services.Data;

public class NullVersion : Version
{
  public static readonly NullVersion Instance = new NullVersion();

  public override string ToString() => "Not Available";

  public override bool IsMajorMoreOrEqual(int major) => false;

  public override bool IsVersionEqual(Version cmp) => cmp is NullVersion;

  public override bool Equals(object obj) => obj is NullVersion;

  public override int GetHashCode() => 0;

  private NullVersion()
  {
  }
}
