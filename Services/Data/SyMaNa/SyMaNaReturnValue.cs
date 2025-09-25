// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Data.SyMaNa.SyMaNaReturnValue
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Ssh.DTO;
using System.Collections.Generic;

#nullable disable
namespace iService5.Core.Services.Data.SyMaNa;

public class SyMaNaReturnValue
{
  public string parameter { get; set; }

  public string installationState { get; set; }

  public string additionalInfo { get; set; }

  public List<VerificationResults> verificationResults { get; set; }

  public string countrySettings { get; set; }

  public string brand { get; set; }

  public string customerIndex { get; set; }

  public string deviceType { get; set; }

  public string manufacturingTimeStamp { get; set; }

  public string serialNumber { get; set; }

  public string vib { get; set; }

  public SyMaNaECU[] EcUs { get; set; }

  public int getCountrySettings() => int.Parse(this.countrySettings);
}
