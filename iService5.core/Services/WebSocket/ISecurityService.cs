// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.WebSocket.ISecurityService
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Platform;
using iService5.Core.Services.User.ActivityLogging;

#nullable disable
namespace iService5.Core.Services.WebSocket;

public interface ISecurityService
{
  string GetNewRandomPassword();

  string GetNewEd25519KeyInfo(string password = null);

  string GetNewCertificateSigningRequest(string privateKeyInfo, string password = null);

  bool ValidateCertificateSigningRequest(string csr);

  void SaveCertificateSigningRequest(string csr, IPlatformSpecificServiceLocator locator);

  void LoadSignedCertificate(IPlatformSpecificServiceLocator locator);

  void LoadClientCertificate(IPlatformSpecificServiceLocator locator);

  bool SaveSignedCertificateFile(
    IPlatformSpecificServiceLocator locator,
    ILoggingService loggingService,
    string signedCertificate,
    string fileName);
}
