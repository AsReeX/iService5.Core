// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.WebSocket.SecurityService
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Helpers;
using iService5.Core.Services.Platform;
using iService5.Core.Services.User.ActivityLogging;
using Rebex;
using Rebex.Net;
using Rebex.Security.Certificates;
using Rebex.Security.Cryptography.Pkcs;
using System;
using System.IO;
using System.Text;

#nullable disable
namespace iService5.Core.Services.WebSocket;

public class SecurityService : ISecurityService
{
  private string CN = "bsh.com";
  private const string O = "BSH";
  private const PrivateKeyFormat Format = PrivateKeyFormat.Base64Pkcs8;
  private const string Filename = "csr.pem";
  private readonly ILoggingService _loggingService;
  private readonly ISecureStorageService _secureStorageService;
  private string _signedCertificateFilePath = "/";
  private string _clientCertificateFilePath = "/";

  public string signedCertificateFilePath
  {
    get => this._signedCertificateFilePath;
    set => this._signedCertificateFilePath = value;
  }

  public string clientCertificateFilePath
  {
    get => this._clientCertificateFilePath;
    set => this._clientCertificateFilePath = value;
  }

  public SecurityService(ILoggingService _logService, ISecureStorageService _securestorageService)
  {
    Licensing.Keys.Add("==FbLHiXIYgnC+Xwd/x7lsGEBy9n90RYvCSVOYma3B4FS64Jl4d6RYXimyxnQyGIJ0vluKp==");
    Licensing.Keys.Add("==FmRMUbfErhzCOpVpdoa6P9Re7Y60fS3vvaK1sy/bw0Tb7pOjtz9f+AJkTbaLxK4Uwj4dp==");
    this._loggingService = _logService;
    this._secureStorageService = _securestorageService;
  }

  public string GetNewRandomPassword() => Guid.NewGuid().ToString();

  public string GetNewEd25519KeyInfo(string password = null)
  {
    using (MemoryStream memoryStream = new MemoryStream())
    {
      PrivateKeyInfo.Generate(KeyAlgorithm.ED25519).Save((Stream) memoryStream, password, PrivateKeyFormat.Base64Pkcs8);
      return Encoding.UTF8.GetString(memoryStream.ToArray());
    }
  }

  public string GetNewCertificateSigningRequest(string privateKeyInfo, string password = null)
  {
    PrivateKeyInfo privateKey = new PrivateKeyInfo();
    CertificationRequest certificationRequest;
    using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(privateKeyInfo)))
    {
      privateKey.Load((Stream) memoryStream, password);
      this.CN = UtilityFunctions.GetUser(this._secureStorageService, this._loggingService);
      certificationRequest = new CertificationRequest(new DistinguishedName($"CN={this.CN}, O=BSH"), privateKey.GetPublicKey());
      certificationRequest.Sign(privateKey, SignatureHashAlgorithm.SHA512);
    }
    using (MemoryStream memoryStream = new MemoryStream())
    {
      certificationRequest.Save((Stream) memoryStream);
      return Encoding.UTF8.GetString(memoryStream.ToArray());
    }
  }

  public bool ValidateCertificateSigningRequest(string csr)
  {
    return new CertificationRequest(Encoding.UTF8.GetBytes(csr)).Validate();
  }

  public void SaveCertificateSigningRequest(string csr, IPlatformSpecificServiceLocator locator)
  {
    new CertificationRequest(Encoding.UTF8.GetBytes(csr)).Save(Path.Combine(locator.GetPlatformSpecificService().getFolder(), "csr.pem"));
  }

  public void LoadSignedCertificate(IPlatformSpecificServiceLocator locator)
  {
    this.signedCertificateFilePath = Path.Combine(locator.GetPlatformSpecificService().getFolder(), "signedCertificate.pem");
  }

  public void LoadClientCertificate(IPlatformSpecificServiceLocator locator)
  {
    this.clientCertificateFilePath = Path.Combine(locator.GetPlatformSpecificService().getFolder(), "clientCertificiate.pem");
  }

  public bool SaveSignedCertificateFile(
    IPlatformSpecificServiceLocator locator,
    ILoggingService loggingService,
    string signedCertificate,
    string fileName)
  {
    bool flag = false;
    try
    {
      this.signedCertificateFilePath = Path.Combine(locator.GetPlatformSpecificService().getFolder(), fileName);
      using (FileStream fileStream = File.Create(this.signedCertificateFilePath))
      {
        byte[] bytes = new UTF8Encoding(true).GetBytes(signedCertificate);
        fileStream.Write(bytes, 0, bytes.Length);
      }
      flag = true;
    }
    catch (Exception ex)
    {
      loggingService.getLogger().LogAppError(LoggingContext.CSR, ex.ToString(), memberName: nameof (SaveSignedCertificateFile), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/WebSocket/SecurityService.cs", sourceLineNumber: 143);
    }
    return flag;
  }

  public void client_ValidatingCertificate(object sender, SslCertificateValidationEventArgs e)
  {
    Certificate certificate = e.CertificateChain[0];
    this._loggingService.getLogger().LogAppDebug(LoggingContext.CSR, "Do you trust to the following certificate?", memberName: nameof (client_ValidatingCertificate), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/WebSocket/SecurityService.cs", sourceLineNumber: 154);
    this._loggingService.getLogger().LogAppDebug(LoggingContext.CSR, " Common name: {0}" + certificate.GetCommonName(), memberName: nameof (client_ValidatingCertificate), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/WebSocket/SecurityService.cs", sourceLineNumber: 155);
    this._loggingService.getLogger().LogAppDebug(LoggingContext.CSR, " Thumbprint:  {0}" + certificate.Thumbprint, memberName: nameof (client_ValidatingCertificate), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/WebSocket/SecurityService.cs", sourceLineNumber: 156);
    this._loggingService.getLogger().LogAppDebug(LoggingContext.CSR, " Expires on:  {0:d}" + certificate.GetExpirationDate().ToString(), memberName: nameof (client_ValidatingCertificate), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/WebSocket/SecurityService.cs", sourceLineNumber: 157);
  }
}
