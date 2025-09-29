// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Helpers.RebexHelper
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Data.SyMaNa;
using Newtonsoft.Json;
using Rebex.Net;
using Rebex.Security.Certificates;
using Rebex.Security.Cryptography;
using System;
using System.Collections.Generic;

#nullable disable
namespace iService5.Core.Services.Helpers;

public static class RebexHelper
{
  private static readonly TlsVersion tlsVersion = TlsVersion.TLS12;
  private static readonly TlsCipherSuite tlsCipherSuite = TlsCipherSuite.Secure;
  private static readonly TlsSignatureScheme tlsSignatureScheme = TlsSignatureScheme.Ed25519;
  private const string LODIS_CERTIFICATE_VALIDATION_TEXT1 = "Appliance CA";
  private const string LODIS_CERTIFICATE_VALIDATION_TEXT2 = " SMM ";

  public static bool IsTargetUsingRealBoard(TargetLodisType lodisType)
  {
    return lodisType == TargetLodisType.SyMaNaBoard_USB || lodisType == TargetLodisType.SyMaNaBoard_WIFI;
  }

  public static string GetBaseURL(TargetLodisType lodisType)
  {
    switch (lodisType)
    {
      case TargetLodisType.SyMaNaBoard_WIFI:
        return "wss://192.168.0.1:28441/maintenance";
      case TargetLodisType.SyMaNaBoard_USB:
        return "wss://192.168.217.2:28441/maintenance";
      case TargetLodisType.RPI:
        return "wss://192.168.0.1:28441/maintenance";
      case TargetLodisType.LocalMOCK:
        return "wss://127.0.0.1:28441/maintenance";
      default:
        return (string) null;
    }
  }

  private static void RegisterProtocols()
  {
    AsymmetricKeyAlgorithm.Register(new Func<string, object>(Ed25519.Create));
    AsymmetricKeyAlgorithm.Register(new Func<string, object>(Curve25519.Create));
    AsymmetricKeyAlgorithm.Register(new Func<string, object>(EllipticCurveAlgorithm.Create));
    if (FeatureConfiguration.LodisSslAcceptAllCertificates)
      return;
    CertificateEngine.SetCurrentEngine((CertificateEngine) new NativeCertificateEngine());
  }

  public static void RegisterCipherSuitesForWebSocket(WebSocketClient websocketClient)
  {
    RebexHelper.RegisterProtocols();
    websocketClient.Settings.SslAllowedVersions = RebexHelper.tlsVersion;
    websocketClient.Settings.SslAllowedSuites = RebexHelper.tlsCipherSuite;
    websocketClient.Settings.SetSignatureSchemes(RebexHelper.tlsSignatureScheme);
    websocketClient.Settings.SslAcceptAllCertificates = FeatureConfiguration.LodisSslAcceptAllCertificates;
  }

  public static void RegisterCipherSuitesForHTTPSUpload(WebClient webClient)
  {
    RebexHelper.RegisterProtocols();
    webClient.Settings.SslAllowedVersions = RebexHelper.tlsVersion;
    webClient.Settings.SslAllowedSuites = RebexHelper.tlsCipherSuite;
    webClient.Settings.SetSignatureSchemes(RebexHelper.tlsSignatureScheme);
    webClient.Settings.SslAcceptAllCertificates = FeatureConfiguration.LodisSslAcceptAllCertificates;
  }

  public static string InitialValuesForWebsocketSetup(
    SyMaNaWebsocketData websocketData,
    string deviceID,
    string deviceName)
  {
    return JsonConvert.SerializeObject((object) new InitialValuesSending(websocketData)
    {
      data = new List<InitialValuesSendingData>()
      {
        new InitialValuesSendingData("Application", deviceName, deviceID)
      }
    });
  }

  public static CertificateIssuerData GetCertificateIssuerData(Certificate certificate)
  {
    CertificateIssuerData certificateIssuerData = new CertificateIssuerData();
    certificateIssuerData.ExpiryDate = certificate.GetExpirationDate();
    certificateIssuerData.subject = ((object) certificate.GetSubject()).ToString();
    string str1 = ((object) certificate.GetIssuer()).ToString();
    char[] chArray1 = new char[1]{ ',' };
    foreach (string str2 in str1.Split(chArray1))
    {
      char[] chArray2 = new char[1]{ '=' };
      string[] strArray = str2.Split(chArray2);
      string str3 = strArray[1];
      if (strArray[0] == "CN")
        certificateIssuerData.CN = str3;
      else if (strArray[0] == " OU")
        certificateIssuerData.OU = str3;
      else if (strArray[0] == " O")
        certificateIssuerData.O = str3;
    }
    return certificateIssuerData;
  }

  public static bool IsLodisCertValid(
    CertificateIssuerData iS5ServiceCACert,
    CertificateIssuerData LodisInterMCert)
  {
    return LodisInterMCert.CN.Contains("Appliance CA") && LodisInterMCert.OU == iS5ServiceCACert.OU && LodisInterMCert.O == iS5ServiceCACert.O && LodisInterMCert.subject.Contains(" SMM ");
  }
}
