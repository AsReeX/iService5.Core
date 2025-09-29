// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Data.UIDataWithValidationMethods
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using System.Text.RegularExpressions;
//using Xamarin.Forms;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

#nullable disable
namespace iService5.Core.Services.Data;

public static class UIDataWithValidationMethods
{
  public static void SetValidity(this UIDataWithValidation item, bool isValid)
  {
    if (isValid)
    {
      item.EntryColor = Colors.Black;
      item.IsValid = true;
    }
    else
    {
      item.EntryColor = Colors.Red;
      item.IsValid = false;
    }
  }

    public static void ColorEnumberField(this UIDataWithValidation item)
    {
        if (item.IsLocked)
        {
            item.EntryBackColor = Colors.LightGray;
            item.EntryColor = Colors.Gray;
            item.IsValid = false;
        }
        else
        {
            item.EntryBackColor = Colors.White;
        }
    }


    public static bool IsDigitsOnly(this string str) => Regex.IsMatch(str, "^\\d+$");
}
