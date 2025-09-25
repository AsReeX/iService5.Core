// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Data.AugmentedModule
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using MvvmCross;
using System;
using System.Collections.Generic;

#nullable disable
namespace iService5.Core.Services.Data;

public class AugmentedModule : module
{
  public AugmentedModule()
  {
    this.InstalledVersion = "";
    this.State = ' ';
    this.Available = Version.FromString("0.0.0.0");
    this.Installed = Version.FromString("0.0.0.0");
    this.IncludeInFlashingSequence = false;
    this.IncludeInComparison = true;
    this.FirmwareName = "";
    this.InSMM = false;
  }

  public AugmentedModule(module _module, bool isRecovery, string package)
    : base(_module)
  {
    this.InstalledVersion = "";
    this.State = ' ';
    this.StateColor = "Black";
    this.Available = Version.FromString(_module.version);
    this.Installed = Version.FromString("0.0.0.0");
    this.IncludeInFlashingSequence = false;
    this.IncludeInComparison = true;
    this.Recovery = isRecovery;
    this.FirmwareName = "";
    this.isECU = this.node != 0 && this.node != (int) ushort.MaxValue;
    this.moduleInstalled = false;
    this.Package = package;
    this.InSMM = false;
  }

  public void clearInstalled()
  {
    this.State = ' ';
    this.StateColor = "Black";
  }

  public bool IsDowngradeAllowed()
  {
    List<smm_whitelist> downgradePass = Mvx.IoCProvider.Resolve<IMetadataService>().GetDowngradePass(this.moduleid);
    try
    {
      foreach (smm_whitelist smmWhitelist in downgradePass)
      {
        if (this.Installed >= Version.FromString(smmWhitelist.version_min) && this.Installed <= Version.FromString(smmWhitelist.version_max) && this.Available >= Version.FromString(smmWhitelist.available_min) && this.Available <= Version.FromString(smmWhitelist.available_max))
          return true;
      }
    }
    catch (NullReferenceException ex)
    {
      return false;
    }
    return false;
  }

  public string InstalledVersion { get; set; }

  public char State { get; set; }

  public string StateColor { get; set; }

  public Version Available { get; set; }

  public Version Installed { get; set; }

  public bool IncludeInFlashingSequence { get; set; }

  public bool IncludeInComparison { get; set; }

  public int ModuleNo { get; internal set; }

  public bool Recovery { get; internal set; }

  public string Package { get; set; }

  public bool isECU { get; internal set; }

  public string FirmwareName { get; set; }

  public string AvailableLabelColor { get; set; }

  public string AvailableLabelDisplay { get; set; }

  public bool moduleInstalled { get; set; }

  public bool? hasCorrespondent { get; set; }

  public bool InSMM { get; set; }

  public string FirmwareInEcuVersion { get; set; }

  public string RecoveryMisMatchSystemVersion { get; set; }

  public string UIAvailable
  {
    get
    {
      return this.version == null || !(this.version != "") || !(this.version != "0.0.0.0") ? "" : this.version;
    }
  }

  public string UIInstalled => this.InstalledVersion;

  public string UIFirmwareName
  {
    get
    {
      string name = this.name;
      return name == "" || name == null ? "" : Mvx.IoCProvider.Resolve<IMetadataService>().getMessageWithShortTextMapping(name);
    }
  }

  public string UIInstalledWithLabel
  {
    get
    {
      if (!string.IsNullOrEmpty(this.InstalledVersion) && this.InstalledVersion != "Unknown")
      {
        string type1 = this.type;
        ModuleType moduleType = ModuleType.FIRMWARE;
        string str1 = moduleType.ToString();
        if (type1 == str1)
          return $"{AppResource.INSTALLED_TEXT}: {this.InstalledVersion}";
        string type2 = this.type;
        moduleType = ModuleType.SPAU_FIRMWARE;
        string str2 = moduleType.ToString();
        if (!(type2 == str2))
          return $"{AppResource.INSTALLED_TEXT}: {AppResource.INSTALLED_UNKNOWN_TEXT}";
        return this.InSMM ? $"{AppResource.STORED_SPAU}: {this.InstalledVersion}" : $"{AppResource.INSTALLED_ECU}: {this.InstalledVersion}";
      }
      if (this.InstalledVersion == "Unknown")
        return $"{AppResource.INSTALLED_TEXT}: {AppResource.INSTALLED_UNKNOWN_TEXT}";
      string type3 = this.type;
      ModuleType moduleType1 = ModuleType.SPAU_FIRMWARE;
      string str3 = moduleType1.ToString();
      if (type3 == str3 && !string.IsNullOrEmpty(this.FirmwareInEcuVersion))
        return $"{AppResource.INSTALLED_ECU}: {this.FirmwareInEcuVersion}";
      string type4 = this.type;
      moduleType1 = ModuleType.INITIAL_CONTENT;
      string str4 = moduleType1.ToString();
      return type4 == str4 ? AppResource.INSTALLED_UNKNOWN_TEXT : AppResource.NOT_INSTALLED_TEXT;
    }
  }

  public string UIAvailableLabelDisplay
  {
    get
    {
      if (this.moduleInstalled)
        return this.AvailableLabelDisplay;
      if (this.Available.IsVersionEqual(this.Installed))
        return AppResource.UP_TO_DATE_TEXT;
      return this.Available > this.Installed || this.Available < this.Installed ? $"{AppResource.AVAILABLE_TEXT} {this.UIAvailable}" : "";
    }
    set => this.AvailableLabelDisplay = value;
  }

  public string UIAvailableLabelColour
  {
    get
    {
      if (this.moduleInstalled)
        return this.AvailableLabelColor;
      if (this.Available.IsVersionEqual(this.Installed))
        this.AvailableLabelColor = "Black";
      else if (this.Available > this.Installed)
      {
        this.AvailableLabelColor = "#007AFF";
      }
      else
      {
        if (!(this.Available < this.Installed))
          return "";
        this.AvailableLabelColor = "Gray";
      }
      return this.AvailableLabelColor;
    }
    set => this.AvailableLabelColor = value;
  }
}
