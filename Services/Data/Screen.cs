// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Data.Screen
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable disable
namespace iService5.Core.Services.Data;

public class Screen : INotifyPropertyChanged
{
  private bool _isSelected;

  public int Id { get; set; }

  public string Title { get; set; }

  public bool IsSelected
  {
    get => this._isSelected;
    set
    {
      this._isSelected = value;
      this.NotifyPropertyChanged(nameof (IsSelected));
    }
  }

  public event PropertyChangedEventHandler PropertyChanged;

  private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
  {
    if (this.PropertyChanged == null)
      return;
    this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
  }
}
