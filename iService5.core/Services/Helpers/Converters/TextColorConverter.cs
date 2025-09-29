// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Helpers.Converters.TextColorConverter
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

public class TextColorConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
    string str = value as string;
    return str == AppResource.FINISHED_TEXT || str == AppResource.MSG_FLASHING_IS_FINISHED || str == "1" ? (object) Colors.Green : (object) Colors.Black;
  }

  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
  {
    throw new NotImplementedException();
  }
}
