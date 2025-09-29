// Decompiled with JetBrains decompiler
// Type: iService5.Core.BuildProperties
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

#nullable disable
namespace iService5.Core;

[GeneratedCode("Microsoft.Build.Tasks.StronglyTypedResourceBuilder", "15.1.0.0")]
[DebuggerNonUserCode]
[CompilerGenerated]
public class BuildProperties
{
  private static ResourceManager resourceMan;
  private static CultureInfo resourceCulture;

  internal BuildProperties()
  {
  }

  [EditorBrowsable(EditorBrowsableState.Advanced)]
  public static ResourceManager ResourceManager
  {
    get
    {
      if (BuildProperties.resourceMan == null)
        BuildProperties.resourceMan = new ResourceManager("iService5.Core.BuildProperties", typeof (BuildProperties).Assembly);
      return BuildProperties.resourceMan;
    }
  }

  [EditorBrowsable(EditorBrowsableState.Advanced)]
  public static CultureInfo Culture
  {
    get => BuildProperties.resourceCulture;
    set => BuildProperties.resourceCulture = value;
  }

  public static string AndroidKey
  {
    get
    {
      return BuildProperties.ResourceManager.GetString(nameof (AndroidKey), BuildProperties.resourceCulture);
    }
  }

  public static string apikey
  {
    get
    {
      return BuildProperties.ResourceManager.GetString(nameof (apikey), BuildProperties.resourceCulture);
    }
  }

  public static string apikey_China
  {
    get
    {
      return BuildProperties.ResourceManager.GetString(nameof (apikey_China), BuildProperties.resourceCulture);
    }
  }

  public static string AppdynamicsAppKey
  {
    get
    {
      return BuildProperties.ResourceManager.GetString(nameof (AppdynamicsAppKey), BuildProperties.resourceCulture);
    }
  }

  public static string AppdynamicsCollectorUrl
  {
    get
    {
      return BuildProperties.ResourceManager.GetString(nameof (AppdynamicsCollectorUrl), BuildProperties.resourceCulture);
    }
  }

  public static string basepath
  {
    get
    {
      return BuildProperties.ResourceManager.GetString(nameof (basepath), BuildProperties.resourceCulture);
    }
  }

  public static string BinarySizeThreshold
  {
    get
    {
      return BuildProperties.ResourceManager.GetString(nameof (BinarySizeThreshold), BuildProperties.resourceCulture);
    }
  }

  public static string CallibrationInterval
  {
    get
    {
      return BuildProperties.ResourceManager.GetString(nameof (CallibrationInterval), BuildProperties.resourceCulture);
    }
  }

  public static string CAT_feature
  {
    get
    {
      return BuildProperties.ResourceManager.GetString(nameof (CAT_feature), BuildProperties.resourceCulture);
    }
  }

  public static string ControlEnabled
  {
    get
    {
      return BuildProperties.ResourceManager.GetString(nameof (ControlEnabled), BuildProperties.resourceCulture);
    }
  }

  public static string CrtIssuer
  {
    get
    {
      return BuildProperties.ResourceManager.GetString(nameof (CrtIssuer), BuildProperties.resourceCulture);
    }
  }

  public static string CrtSubject
  {
    get
    {
      return BuildProperties.ResourceManager.GetString(nameof (CrtSubject), BuildProperties.resourceCulture);
    }
  }

  public static string CrtSubjectChina
  {
    get
    {
      return BuildProperties.ResourceManager.GetString(nameof (CrtSubjectChina), BuildProperties.resourceCulture);
    }
  }

  public static string DisplayBridgeDownloadLogBtn
  {
    get
    {
      return BuildProperties.ResourceManager.GetString(nameof (DisplayBridgeDownloadLogBtn), BuildProperties.resourceCulture);
    }
  }

  public static string DisplayNativeBridgeProperties
  {
    get
    {
      return BuildProperties.ResourceManager.GetString(nameof (DisplayNativeBridgeProperties), BuildProperties.resourceCulture);
    }
  }

  public static string EnabledCrtPinning
  {
    get
    {
      return BuildProperties.ResourceManager.GetString(nameof (EnabledCrtPinning), BuildProperties.resourceCulture);
    }
  }

  public static string EnabledJailbreakCheck
  {
    get
    {
      return BuildProperties.ResourceManager.GetString(nameof (EnabledJailbreakCheck), BuildProperties.resourceCulture);
    }
  }

  public static string extractionThresholdEntries
  {
    get
    {
      return BuildProperties.ResourceManager.GetString(nameof (extractionThresholdEntries), BuildProperties.resourceCulture);
    }
  }

  public static string extractionThresholdRatio
  {
    get
    {
      return BuildProperties.ResourceManager.GetString(nameof (extractionThresholdRatio), BuildProperties.resourceCulture);
    }
  }

  public static string extractionThresholdSize
  {
    get
    {
      return BuildProperties.ResourceManager.GetString(nameof (extractionThresholdSize), BuildProperties.resourceCulture);
    }
  }

  public static string FileSizeLimit
  {
    get
    {
      return BuildProperties.ResourceManager.GetString(nameof (FileSizeLimit), BuildProperties.resourceCulture);
    }
  }

  public static string IOSKey
  {
    get
    {
      return BuildProperties.ResourceManager.GetString(nameof (IOSKey), BuildProperties.resourceCulture);
    }
  }

  public static string LodisSslAcceptAllCertificates
  {
    get
    {
      return BuildProperties.ResourceManager.GetString(nameof (LodisSslAcceptAllCertificates), BuildProperties.resourceCulture);
    }
  }

  public static string LodisTarget
  {
    get
    {
      return BuildProperties.ResourceManager.GetString(nameof (LodisTarget), BuildProperties.resourceCulture);
    }
  }

  public static string LoginUser
  {
    get
    {
      return BuildProperties.ResourceManager.GetString(nameof (LoginUser), BuildProperties.resourceCulture);
    }
  }

  public static string maxDownloadRetries
  {
    get
    {
      return BuildProperties.ResourceManager.GetString(nameof (maxDownloadRetries), BuildProperties.resourceCulture);
    }
  }

  public static string maxOfflineDays
  {
    get
    {
      return BuildProperties.ResourceManager.GetString(nameof (maxOfflineDays), BuildProperties.resourceCulture);
    }
  }

  public static string maxOfflineDaysWithNotification
  {
    get
    {
      return BuildProperties.ResourceManager.GetString(nameof (maxOfflineDaysWithNotification), BuildProperties.resourceCulture);
    }
  }

  public static string Monitoring_Debug_Enabled
  {
    get
    {
      return BuildProperties.ResourceManager.GetString(nameof (Monitoring_Debug_Enabled), BuildProperties.resourceCulture);
    }
  }

  public static string PpfRefetch_Enabled
  {
    get
    {
      return BuildProperties.ResourceManager.GetString(nameof (PpfRefetch_Enabled), BuildProperties.resourceCulture);
    }
  }

  public static string protocol
  {
    get
    {
      return BuildProperties.ResourceManager.GetString(nameof (protocol), BuildProperties.resourceCulture);
    }
  }

  public static string RebexLog
  {
    get
    {
      return BuildProperties.ResourceManager.GetString(nameof (RebexLog), BuildProperties.resourceCulture);
    }
  }

  public static string reconnectTimeoutToElp
  {
    get
    {
      return BuildProperties.ResourceManager.GetString(nameof (reconnectTimeoutToElp), BuildProperties.resourceCulture);
    }
  }

  public static string reconnectTimeoutToRecovery
  {
    get
    {
      return BuildProperties.ResourceManager.GetString(nameof (reconnectTimeoutToRecovery), BuildProperties.resourceCulture);
    }
  }

  public static string reconnectTimeoutToSyMaNa
  {
    get
    {
      return BuildProperties.ResourceManager.GetString(nameof (reconnectTimeoutToSyMaNa), BuildProperties.resourceCulture);
    }
  }

  public static string RetainedFileCountLimit
  {
    get
    {
      return BuildProperties.ResourceManager.GetString(nameof (RetainedFileCountLimit), BuildProperties.resourceCulture);
    }
  }

  public static string SelectedStorage
  {
    get
    {
      return BuildProperties.ResourceManager.GetString(nameof (SelectedStorage), BuildProperties.resourceCulture);
    }
  }

  public static string server
  {
    get
    {
      return BuildProperties.ResourceManager.GetString(nameof (server), BuildProperties.resourceCulture);
    }
  }

  public static string server_China
  {
    get
    {
      return BuildProperties.ResourceManager.GetString(nameof (server_China), BuildProperties.resourceCulture);
    }
  }

  public static string stage
  {
    get
    {
      return BuildProperties.ResourceManager.GetString(nameof (stage), BuildProperties.resourceCulture);
    }
  }

  public static string stage_China
  {
    get
    {
      return BuildProperties.ResourceManager.GetString(nameof (stage_China), BuildProperties.resourceCulture);
    }
  }

  public static string supportEmail
  {
    get
    {
      return BuildProperties.ResourceManager.GetString(nameof (supportEmail), BuildProperties.resourceCulture);
    }
  }

  public static string version
  {
    get
    {
      return BuildProperties.ResourceManager.GetString(nameof (version), BuildProperties.resourceCulture);
    }
  }
}
