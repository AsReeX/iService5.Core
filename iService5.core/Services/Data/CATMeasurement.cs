// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Data.CATMeasurement
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace iService5.Core.Services.Data;

public class CATMeasurement
{
  private List<CATMeasurementTitle> timeoutMeasurements = new List<CATMeasurementTitle>()
  {
    CATMeasurementTitle.ST_SL1,
    CATMeasurementTitle.ST_SL,
    CATMeasurementTitle.ST_RISO_PROBE,
    CATMeasurementTitle.ST_RISO_SOCKET,
    CATMeasurementTitle.ST_IEA
  };
  public string CatTimeout;

  public CATMeasurementTitle title { get; set; }

  public string name { get; set; }

  public string svg { get; set; }

  public int position { get; set; }

  public string unit { get; set; }

  public string validationUnit { get; set; }

  public double min { get; set; }

  public double max { get; set; }

  public bool isMinAvailable { get; set; }

  public bool isMaxAvailable { get; set; }

  public Dictionary<string, int> fractionalPartAccuracyForUnit { get; set; }

  public string peakValue { get; set; }

  public CATMeasurement(
    CATMeasurementTitle title,
    string name,
    int position,
    string unit,
    double min,
    double max,
    bool isMinAvailable,
    bool isMaxAvailable,
    Dictionary<string, int> fractionalPartAccuracyForUnit,
    string svg,
    string peakValue = "0")
  {
    this.title = title;
    this.name = name;
    this.svg = svg;
    this.position = position;
    this.unit = unit;
    this.validationUnit = this.validationUnit;
    this.min = min;
    this.max = max;
    this.isMinAvailable = isMinAvailable;
    this.isMaxAvailable = isMaxAvailable;
    this.svg = svg;
    this.validationUnit = unit;
    this.fractionalPartAccuracyForUnit = fractionalPartAccuracyForUnit;
    this.peakValue = peakValue;
    if (this.timeoutMeasurements.IndexOf(this.title) >= 0)
    {
      try
      {
        this.CatTimeout = CoreApp.settings.GetItem("catTimeout").Value;
      }
      catch (Exception ex)
      {
        CoreApp.settings.UpdateItem(new Settings("catTimeout", "19"));
        this.CatTimeout = CoreApp.settings.GetItem("catTimeout").Value;
      }
    }
    else
      this.CatTimeout = "notset";
  }
}
