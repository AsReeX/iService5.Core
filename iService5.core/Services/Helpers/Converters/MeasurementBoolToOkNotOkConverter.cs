// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Helpers.Converters.MeasurementBoolToOkNotOkConverter
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using System;
using System.Globalization;
//using Xamarin.Forms;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

#nullable disable
namespace iService5.Core.Services.Helpers.Converters;

public class MeasurementBoolToOkNotOkConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
    bool? nullable1 = (bool?) value;
    bool flag1 = true;
    if (nullable1.GetValueOrDefault() == flag1 & nullable1.HasValue)
      return (object) "OK";
    bool? nullable2 = (bool?) value;
    bool flag2 = false;
    return nullable2.GetValueOrDefault() == flag2 & nullable2.HasValue ? (object) "NOT OK" : (object) "n/a";
  }

  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
  {
    throw new NotImplementedException();
  }
}
