// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Data.VariantCodingDataToJson
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

#nullable disable
namespace iService5.Core.Services.Data;

public class VariantCodingDataToJson
{
  private string _FieldValue;
  private string _FieldVariable;

  public VariantCodingDataToJson(string fieldVariable, string fieldValue)
  {
  }

  public VariantCodingDataToJson()
  {
  }

  public string FieldValue
  {
    get => this._FieldValue;
    set => this._FieldValue = value;
  }

  public string FieldVariable
  {
    get => this._FieldVariable;
    set => this._FieldVariable = value;
  }
}
