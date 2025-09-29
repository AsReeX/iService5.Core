// Decompiled with JetBrains decompiler
// Type: iService5.Core.is5Entry
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

//using Xamarin.Forms;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

#nullable disable
namespace iService5.Core;

public class is5Entry : Entry
{
  public new static readonly BindableProperty BackgroundColorProperty = BindableProperty.Create(nameof (BackgroundColor), typeof (Color), typeof (is5Entry), (object) Colors.White, BindingMode.TwoWay);

  public new Color BackgroundColor
  {
    get => (Color) this.GetValue(is5Entry.BackgroundColorProperty);
    set => this.SetValue(is5Entry.BackgroundColorProperty, (object) value);
  }
}
