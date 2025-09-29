// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Helpers.FeatureConfiguration
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using System;

#nullable disable
namespace iService5.Core.Services.Helpers;

public static class FeatureConfiguration
{
  public static readonly bool CrtPinningEnabled = string.Equals(BuildProperties.EnabledCrtPinning, "true", StringComparison.OrdinalIgnoreCase);
  public static readonly bool EnabledJailBreakCheck = string.Equals(BuildProperties.EnabledJailbreakCheck, "true", StringComparison.OrdinalIgnoreCase);
  public static readonly int MaxBinaryDownloadRetries = int.Parse(BuildProperties.maxDownloadRetries);
  public static readonly bool PpfRefetchEnabled = string.Equals(BuildProperties.PpfRefetch_Enabled, "true", StringComparison.OrdinalIgnoreCase);
  public static readonly bool LodisSslAcceptAllCertificates = string.Equals(BuildProperties.LodisSslAcceptAllCertificates, "true", StringComparison.OrdinalIgnoreCase);
  public static readonly bool MonitoringDebugEnabled = string.Equals(BuildProperties.Monitoring_Debug_Enabled, "true", StringComparison.OrdinalIgnoreCase);
  public static readonly bool DisplayNativeBridgeProperties = string.Equals(BuildProperties.DisplayNativeBridgeProperties, "true", StringComparison.OrdinalIgnoreCase);
  public static readonly bool DisplayCATFeature = string.Equals(BuildProperties.CAT_feature, "true", StringComparison.OrdinalIgnoreCase);
  public static readonly bool RebexLog = string.Equals(BuildProperties.RebexLog, "true", StringComparison.OrdinalIgnoreCase);
  public static readonly bool ControlEnabled = string.Equals(BuildProperties.ControlEnabled, "true", StringComparison.OrdinalIgnoreCase);

  public static TargetLodisType LodisTargetType()
  {
    TargetLodisType result;
    Enum.TryParse<TargetLodisType>(BuildProperties.LodisTarget, out result);
    return result;
  }

  public static bool DisplayBridgeDownloadLogBtn(bool displayParameter)
  {
    return BuildProperties.DisplayBridgeDownloadLogBtn.Equals("true") & displayParameter;
  }

  public static bool DisplayNativeBridgeSettings(bool displayParameter)
  {
    return FeatureConfiguration.DisplayNativeBridgeProperties & displayParameter;
  }
}
