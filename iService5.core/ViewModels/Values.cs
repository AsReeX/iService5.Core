// Decompiled with JetBrains decompiler
// Type: iService5.Core.ViewModels.Values
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using System.Collections.Generic;

#nullable disable
namespace iService5.Core.ViewModels;

public class Values
{
  public Values()
  {
    this.measurement_results = new List<MeasurementResults>();
    this.appliance_status = new List<ApplianceStatus>();
    this.feature_availability = new List<FeatureAvailability>();
    this.additional_options = new List<AdditionalOptions>();
  }

  public List<MeasurementResults> measurement_results { get; set; }

  public List<ApplianceStatus> appliance_status { get; set; }

  public List<FeatureAvailability> feature_availability { get; set; }

  public List<AdditionalOptions> additional_options { get; set; }
}
