// Decompiled with JetBrains decompiler
// Type: iService5.Core.ViewModels.LongPressedEffect
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using System;
using System.Windows.Input;
//using Xamarin.Forms;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

#nullable disable
namespace iService5.Core.ViewModels;

public class LongPressedEffect : RoutingEffect
{
  public static readonly BindableProperty CommandProperty = BindableProperty.CreateAttached("Command", typeof (ICommand), typeof (LongPressedEffect), (object) null);
  public static readonly BindableProperty CommandParameterProperty = BindableProperty.CreateAttached("CommandParameter", typeof (object), typeof (LongPressedEffect), (object) null);

  public LongPressedEffect()
    : base("iService5.LongPressedEffect")
  {
  }

  public static ICommand GetCommand(BindableObject view)
  {
    Console.WriteLine("long press Gesture recognizer has been striked");
    return (ICommand) view.GetValue(LongPressedEffect.CommandProperty);
  }

  public static void SetCommand(BindableObject view, ICommand value)
  {
    view.SetValue(LongPressedEffect.CommandProperty, (object) value);
  }

  public static object GetCommandParameter(BindableObject view)
  {
    return view.GetValue(LongPressedEffect.CommandParameterProperty);
  }

  public static void SetCommandParameter(BindableObject view, object value)
  {
    view.SetValue(LongPressedEffect.CommandParameterProperty, value);
  }
}
