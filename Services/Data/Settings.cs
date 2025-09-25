// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Data.Settings
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using SQLite;

#nullable disable
namespace iService5.Core.Services.Data;

public class Settings
{
  [PrimaryKey]
  public string Name { get; set; }

  public string Value { get; set; }

  public Settings()
  {
  }

  public Settings(string name, string value)
  {
    this.Name = name;
    this.Value = value;
  }
}
