// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Data.material
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using SQLite;
using System;

#nullable disable
namespace iService5.Core.Services.Data;

public class material
{
  [Column("material")]
  public string _material { get; set; }

  public string brand { get; set; }

  public string type { get; set; }

  public string parent { get; set; }

  public string wifiCountrySetting { get; set; }

  public override string ToString()
  {
    return $"{this._material} {this.brand} {this.type} {this.parent} {this.wifiCountrySetting}";
  }

  public string IsClickAllowed
  {
    get => this.type.StartsWith("SMM_WITH_WIFI") ? "Black" : "#808080";
    set => throw new InvalidOperationException();
  }

  public bool DisplayIcon
  {
    get => this.type.StartsWith("SMM_WITH_WIFI");
    set => throw new InvalidOperationException();
  }
}
