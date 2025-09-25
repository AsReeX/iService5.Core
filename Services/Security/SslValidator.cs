// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Security.SslValidator
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Backend;
using iService5.Core.Services.Helpers;
using iService5.Core.Services.User.ActivityLogging;
using MvvmCross;
using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

#nullable disable
namespace iService5.Core.Services.Security;

public static class SslValidator
{
  private static ILoggingService loggingService = Mvx.IoCProvider.Resolve<ILoggingService>();
  public static readonly IAlertService alertService = Mvx.IoCProvider.Resolve<IAlertService>();
  private static readonly ISecureStorageService secureStorageService = Mvx.IoCProvider.Resolve<ISecureStorageService>();

  public static void initialize()
  {
    ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls13;
    ServicePointManager.Expect100Continue = false;
    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(SslValidator.onValidateCertificate);
  }

  public static bool onValidateCertificate(
    object sender,
    X509Certificate certificate,
    X509Chain chain,
    SslPolicyErrors sslPolicyErrors)
  {
    if (FeatureConfiguration.CrtPinningEnabled)
    {
      HttpWebRequest httpWebRequest = sender as HttpWebRequest;
      IBackendDetails<HttpWebRequest> backendDetails = Mvx.IoCProvider.Resolve<IBackendDetails<HttpWebRequest>>();
      if (httpWebRequest.RequestUri == backendDetails.getAuthenticateRequest().RequestUri)
      {
        string str = UtilityFunctions.isDeviceTimeZoneChina() ? BuildProperties.CrtSubjectChina : BuildProperties.CrtSubject;
        if (certificate.Subject == str)
        {
          DateTime dateTime = DateTime.Parse(certificate.GetExpirationDateString());
          if (sslPolicyErrors == SslPolicyErrors.None)
          {
            if (dateTime < DateTime.Now)
            {
              SslValidator.loggingService.getLogger().LogAppError(LoggingContext.BACKEND, $"The SSL certificate for {httpWebRequest.RequestUri?.ToString()} has expired", memberName: nameof (onValidateCertificate), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Security/SslValidator.cs", sourceLineNumber: 44);
              SslValidator.alertService.ShowMessageAlertWithKey("SERVER_CERTIFICATE_EXPIRED", AppResource.INFORMATION_TEXT);
            }
          }
          else
          {
            SslValidator.loggingService.getLogger().LogAppError(LoggingContext.BACKEND, "KeyMismatch and SSL Policy Errors: " + sslPolicyErrors.ToString(), memberName: nameof (onValidateCertificate), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Security/SslValidator.cs", sourceLineNumber: 50);
            SslValidator.loggingService.getLogger().LogAppError(LoggingContext.BACKEND, $"The SSL certificate validation for {httpWebRequest.RequestUri?.ToString()} has failed", memberName: nameof (onValidateCertificate), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Security/SslValidator.cs", sourceLineNumber: 51);
            SslValidator.alertService.ShowMessageAlertWithKey("SERVER_CERTIFICATE_UNSECURE_CONNECTION", AppResource.INFORMATION_TEXT);
          }
        }
        else
        {
          SslValidator.loggingService.getLogger().LogAppError(LoggingContext.BACKEND, $"The SSL certificate validation for {httpWebRequest.RequestUri?.ToString()} has failed./n Certificate Subject Mismatched", memberName: nameof (onValidateCertificate), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Security/SslValidator.cs", sourceLineNumber: 57);
          SslValidator.alertService.ShowMessageAlertWithKey("SERVER_CERTIFICATE_UNSECURE_CONNECTION", AppResource.INFORMATION_TEXT);
        }
      }
    }
    return true;
  }
}
