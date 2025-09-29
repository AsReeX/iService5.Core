// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Data.CertificateData
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Helpers;
using System.IO;
using System.Reflection;

#nullable disable
namespace iService5.Core.Services.Data;

public class CertificateData
{
  public string password;
  public Stream clientCertStream;
  public Stream serviceCACertStream;
  public Stream privateKeyStream;

  public CertificateData(ISecureStorageService secureStorageService, bool isRealBoard)
  {
    if (isRealBoard)
    {
      string result1 = secureStorageService.GetSignedCertificate(SecureStorageKeys.CERTIFICATE_CLIENT).Result;
      string result2 = secureStorageService.GetSignedCertificate(SecureStorageKeys.CERTIFICATE_SERVICE_ACCESS_CA).Result;
      string result3 = secureStorageService.getPrivateKeyInfo().Result;
      this.clientCertStream = UtilityFunctions.GenerateStreamFromString(result1);
      this.serviceCACertStream = UtilityFunctions.GenerateStreamFromString(result2);
      this.privateKeyStream = UtilityFunctions.GenerateStreamFromString(result3);
      this.password = secureStorageService.getPrivateKeyInfoPassword().Result;
    }
    else
    {
      string str1 = "SignedCertificate.pem";
      string str2 = "CACertificate.pem";
      string str3 = "CertificateKey.pem";
      string str4 = "iService5.Core.resources.MockCertificates.";
      Assembly executingAssembly = Assembly.GetExecutingAssembly();
      this.clientCertStream = executingAssembly.GetManifestResourceStream(str4 + str1);
      this.serviceCACertStream = executingAssembly.GetManifestResourceStream(str4 + str2);
      this.privateKeyStream = executingAssembly.GetManifestResourceStream(str4 + str3);
      this.password = "changeit";
    }
  }
}
