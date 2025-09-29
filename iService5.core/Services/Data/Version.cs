// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Data.Version
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using System.Collections.Generic;

#nullable disable
namespace iService5.Core.Services.Data;

public class Version
{
  private static List<string> nullVersionsStrings = new List<string>()
  {
    (string) null,
    "",
    "0",
    "0.0.0",
    "0.0.0.0",
    "Unknown",
    "-1"
  };
  private readonly int major;
  private readonly int minor;
  private readonly int revision;

  public static Version FromString(string content)
  {
    return Version.nullVersionsStrings.Contains(content) ? (Version) NullVersion.Instance : new Version(content);
  }

  public Version()
  {
    this.major = 0;
    this.minor = 0;
    this.revision = 0;
  }

  public Version(int M, int m, int r)
  {
    this.major = M;
    this.minor = m;
    this.revision = r;
  }

  public Version(string content)
    : this()
  {
    content = content.Trim();
    string[] strArray = content.Split('.');
    if (strArray.Length >= 1)
      int.TryParse(strArray[0], out this.major);
    if (strArray.Length >= 2)
      int.TryParse(strArray[1], out this.minor);
    if (strArray.Length < 3)
      return;
    int.TryParse(strArray[2], out this.revision);
  }

  public static bool operator >(Version c, Version cmp)
  {
    return !(c is NullVersion) && (cmp is NullVersion || c.major > cmp.major || c.major.Equals(cmp.major) && (c.minor > cmp.minor || c.minor.Equals(cmp.minor) && c.revision > cmp.revision));
  }

  public static bool operator <(Version c, Version cmp)
  {
    return c is NullVersion || !(cmp is NullVersion) && (c.major < cmp.major || c.major.Equals(cmp.major) && (c.minor < cmp.minor || c.minor.Equals(cmp.minor) && c.revision < cmp.revision));
  }

  public static bool operator >=(Version c, Version cmp)
  {
    if (c is NullVersion && cmp is NullVersion)
      return true;
    if (c is NullVersion)
      return false;
    if (cmp is NullVersion || c.major == cmp.major && c.minor == cmp.minor && c.revision == cmp.revision || c.major > cmp.major)
      return true;
    int num = c.major;
    if (num.Equals(cmp.major))
    {
      if (c.minor > cmp.minor)
        return true;
      num = c.minor;
      if (num.Equals(cmp.minor) && c.revision > cmp.revision)
        return true;
    }
    return false;
  }

  public static bool operator <=(Version c, Version cmp)
  {
    if (c is NullVersion && cmp is NullVersion || c is NullVersion)
      return true;
    if (cmp is NullVersion)
      return false;
    if (c.major == cmp.major && c.minor == cmp.minor && c.revision == cmp.revision || c.major < cmp.major)
      return true;
    int num = c.major;
    if (num.Equals(cmp.major))
    {
      if (c.minor < cmp.minor)
        return true;
      num = c.minor;
      if (num.Equals(cmp.minor) && c.revision < cmp.revision)
        return true;
    }
    return false;
  }

  public virtual bool IsVersionEqual(Version cmp)
  {
    return !(cmp is NullVersion) && this.major == cmp.major && this.minor == cmp.minor && this.revision == cmp.revision;
  }

  public virtual bool IsMajorMoreOrEqual(int major) => this.major > major;

  public override string ToString()
  {
    return $"{this.major.ToString()}.{this.minor.ToString()}.{this.revision.ToString()}";
  }

  public override bool Equals(object obj)
  {
    int num1;
    if (obj is Version version)
    {
      int num2 = this.major;
      if (num2.Equals(version.major))
      {
        num2 = this.minor;
        if (num2.Equals(version.minor))
        {
          num2 = this.revision;
          num1 = num2.Equals(version.revision) ? 1 : 0;
          goto label_5;
        }
      }
    }
    num1 = 0;
label_5:
    return num1 != 0;
  }

//  public override int GetHashCode()
//  {
//    return ((-1803368456 * -1521134295 + this.major.GetHashCode()) * -1521134295 + this.minor.GetHashCode()) * -1521134295 + this.revision.GetHashCode();
//  }
}
