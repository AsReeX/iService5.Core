// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Helpers.Topogram.Topogram
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using System.Collections.Generic;

#nullable disable
namespace iService5.Core.Services.Helpers.Topogram;

public class Topogram
{
  private List<TopogramScreen> _topogramScreens = new List<TopogramScreen>();

  public Topogram() => this._topogramScreens = new List<TopogramScreen>();

  public Topogram(List<TopogramScreen> topogramScreens) => this._topogramScreens = topogramScreens;

  public List<TopogramScreen> GetAllScreens() => this._topogramScreens;

  public List<string> GetScreenNames()
  {
    List<string> screenNames = new List<string>();
    foreach (TopogramScreen topogramScreen in this._topogramScreens)
      screenNames.Add(topogramScreen.GetScreenName());
    return screenNames;
  }

  public TopogramScreen GetScreen(int atIndex) => this._topogramScreens[atIndex];
}
