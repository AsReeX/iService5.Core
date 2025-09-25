// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Data.VariantCodingData
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

#nullable disable
namespace iService5.Core.Services.Data;

public class VariantCodingData : UIDataWithValidation
{
  private string _FieldValue;

  public string Title { get; set; }

  public int Id { get; set; }

  public VariantCodingData(
    string fieldTitle,
    int? fieldLength,
    string fieldVariable,
    string fieldType,
    string fieldValue,
    bool lockEntry)
  {
    this.Title = fieldTitle;
    if (!fieldLength.HasValue || fieldLength.Value == 0)
      this.EntryLength = int.MaxValue;
    else
      this.EntryLength = fieldLength.Value;
    this.IsLocked = lockEntry;
    this.FieldVariable = fieldVariable;
    this.FieldType = fieldType;
    this.FieldValue = fieldValue;
    this.ColorEnumberField();
  }

  public VariantCodingData()
  {
  }

  public string FieldValue
  {
    get => this._FieldValue;
    set
    {
      if (!(this._FieldValue != value))
        return;
      this._FieldValue = value;
      if (!this.IsLocked)
      {
        if (this.FieldVariable.ToLowerInvariant().Equals("FDATE".ToLowerInvariant()))
          this.ValidateFDate(value);
        else if (this.FieldVariable.ToLowerInvariant().Equals("SERN".ToLowerInvariant()))
          this.ValidateSN(value);
        else if (this.FieldType.ToLowerInvariant().Equals("DEC".ToLowerInvariant()))
          this.IsDec(value);
        else if (this.FieldType.ToLowerInvariant().Equals("HEX".ToLowerInvariant()))
          this.IsHex(value);
        else if (this.FieldType.ToLowerInvariant().Equals("ASCII".ToLowerInvariant()))
          this.ValidateASCII(value);
        else
          this.ValidateLength(value);
      }
      this.RaisePropertyChanged(nameof (FieldValue));
    }
  }

  public string FieldVariable { get; set; }

  public string FieldType { get; set; }

  public string RepairEnumber { get; set; }

  public IMetadataService MetadataService { get; set; }

  public (string, string) BrandInfo { get; set; }

  private void IsHex(string value)
  {
    this.SetValidity(Regex.IsMatch(value, "\\A\\b[0-9a-fA-F]+\\b\\Z") && this.ValidateFieldLength(value));
  }

  private void IsDec(string value)
  {
    this.SetValidity(value.IsDigitsOnly() && this.ValidateFieldLength(value));
  }

  private void ValidateASCII(string value)
  {
    this.SetValidity(((IEnumerable<char>) value.ToCharArray()).Any<char>((Func<char, bool>) (x => x < '\u0080')) && this.ValidateFieldLength(value));
  }

  public void ValidateLength(string value) => this.SetValidity(this.ValidateFieldLength(value));

  public bool ValidateFieldLength(string value)
  {
    if (this.EntryLength != int.MaxValue)
    {
      if (!value.Length.Equals(this.EntryLength))
        return false;
    }
    else if (string.IsNullOrEmpty(value))
      return false;
    return true;
  }

  public void ValidateFDate(string fDateValue)
  {
    if (string.IsNullOrEmpty(this.FieldValue) || !this.ValidateFieldLength(fDateValue))
    {
      this.SetValidity(false);
    }
    else
    {
      char[] charArray = fDateValue.ToCharArray();
      int[] numArray = Array.ConvertAll<char, int>(charArray, (Converter<char, int>) (c => Convert.ToInt32(c.ToString())));
      int length = charArray.Length;
      List<bool> source = new List<bool>(7);
      for (int index = 0; index < length; ++index)
      {
        if (index == 0)
          source.Add(numArray[0].Equals(0) || numArray[0].Equals(9));
        if (index == 1)
          source.Add(this.IsInRange0to9(numArray[1]));
        if (index == 2)
          source.Add(numArray[2].Equals(0) || numArray[2].Equals(1));
        if (index == 3)
          source.Add(this.IsInRange0to9(numArray[3]));
      }
      this.SetValidity(!source.Any<bool>((Func<bool, bool>) (el => !el)));
    }
  }

  public void ValidateSN(string _newValue)
  {
    if (string.IsNullOrEmpty(this.FieldValue) || this.FieldValue.Any<char>((Func<char, bool>) (c => char.IsLetter(c))) || !this.ValidateFieldLength(this.FieldValue))
    {
      this.SetValidity(false);
    }
    else
    {
      if (!_newValue.IsDigitsOnly() || !this.IsCorrectMonthDigits(_newValue) || !this.IsCheckSumCorrect(_newValue))
        return;
      this.SetValidity(true);
    }
  }

  private bool IsCheckSumCorrect(string newValue)
  {
    string[] source = Array.ConvertAll<char, string>(newValue.ToCharArray(), new Converter<char, string>(char.ToString));
    if (!((IEnumerable<string>) source).All<string>((Func<string, bool>) (x => x.IsDigitsOnly())))
      return false;
    int num1 = 0;
    int num2 = 0;
    int num3 = -1;
    List<int> list1;
    List<int> list2;
    try
    {
      list1 = ((IEnumerable<string>) source).Select<string, int>((Func<string, int>) (c => Convert.ToInt32(c.ToString()))).ToList<int>();
      list2 = list1.Take<int>(list1.Count - 1).ToList<int>();
    }
    catch (Exception ex)
    {
      return false;
    }
    IEnumerable<int> ints = list2.Where<int>((Func<int, int, bool>) ((item, index) => index % 2 != 0));
    foreach (int num4 in list2.Where<int>((Func<int, int, bool>) ((item, index) => index % 2 == 0)))
      num1 += num4;
    int num5 = num1 * 3;
    foreach (int num6 in ints)
      num2 += num6;
    int num7 = num5 + num2;
    int num8 = num7 < 10 ? 10 : num7;
    for (int index = 0; index < num8; ++index)
    {
      if ((num7 + index) % 10 == 0)
      {
        num3 = index;
        break;
      }
    }
    return num3 != -1 && num3.Equals(list1.Last<int>());
  }

  private bool IsCorrectMonthDigits(string newValue)
  {
    int num;
    switch (newValue[3])
    {
      case '0':
      case '1':
        num = newValue.IsDigitsOnly() ? 1 : 0;
        break;
      default:
        num = 0;
        break;
    }
    return num != 0;
  }

  private bool IsInRange0to9(int _intChar) => _intChar >= 0 && _intChar <= 9;
}
