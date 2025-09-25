// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Helpers.Topogram.TopogramScreen
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace iService5.Core.Services.Helpers.Topogram;

public class TopogramScreen
{
  private string _screenName;
  private List<TopogramComponent> _components = new List<TopogramComponent>();

  public TopogramScreen()
  {
    this._screenName = "";
    this._components = new List<TopogramComponent>();
  }

  public TopogramScreen(string screenName, List<TopogramComponent> components)
  {
    this._screenName = screenName;
    this._components = components;
  }

  public TopogramScreen(TopogramScreen topogramScreen)
  {
    this._screenName = topogramScreen._screenName;
    this._components = topogramScreen._components;
  }

  public string GetScreenName() => this._screenName;

  public List<TopogramComponent> GetComponents() => this._components;

  public List<TopogramComponent> GetPassiveComponents()
  {
    return this._components.Where<TopogramComponent>((Func<TopogramComponent, bool>) (component => component.GetComponentType() == TopogramComponentType.PASSIVE)).ToList<TopogramComponent>();
  }

  public List<TopogramComponent> GetPassiveComponentsWithoutDependencies()
  {
    return this._components.Where<TopogramComponent>((Func<TopogramComponent, bool>) (component =>
    {
      if (component.GetComponentType() != TopogramComponentType.PASSIVE)
        return false;
      return component.GetDependencies() == "" || component.GetDependencies() == "\r";
    })).ToList<TopogramComponent>();
  }

  public List<TopogramComponent> GetSensorComponents()
  {
    return this._components.Where<TopogramComponent>((Func<TopogramComponent, bool>) (component => component.GetComponentType() == TopogramComponentType.SENSOR)).ToList<TopogramComponent>();
  }

  public List<TopogramComponent> GetCONSUMERComponents()
  {
    return this._components.Where<TopogramComponent>((Func<TopogramComponent, bool>) (component => component.GetComponentType() == TopogramComponentType.CONSUMER)).ToList<TopogramComponent>();
  }

  public TopogramComponent GetComponent(int atIndex) => this._components[atIndex];

  public TopogramComponent GetComponentWithMaxXPositionValue()
  {
    return this._components.Aggregate<TopogramComponent>((Func<TopogramComponent, TopogramComponent, TopogramComponent>) ((x1, x2) => x1.GetXPosition() <= x2.GetXPosition() ? x2 : x1));
  }

  public TopogramComponent GetComponentWithMaxYPositionValue()
  {
    return this._components.Aggregate<TopogramComponent>((Func<TopogramComponent, TopogramComponent, TopogramComponent>) ((y1, y2) => y1.GetYPosition() <= y2.GetYPosition() ? y2 : y1));
  }
}
