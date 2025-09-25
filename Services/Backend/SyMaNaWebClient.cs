// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Backend.SyMaNaWebClient
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Data;
using iService5.Core.Services.Data.SyMaNa;
using iService5.Core.Services.Helpers;
using iService5.Core.Services.User.ActivityLogging;
using Rebex.Net;
using Rebex.Security.Certificates;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

#nullable disable
namespace iService5.Core.Services.Backend;

public class SyMaNaWebClient : ISyMaNaWebClient
{
  public ILoggingService _loggingService;
  public ISecureStorageService _secureStorageService;
  public Rebex.Net.WebClient webClient;
  public List<string> trustedThumbprints;
  private CertificateIssuerData iS5CertificateData;

  public event SyMaNaWebClient.UploadProgressHandler UploadProgressUpdate;

  private void SetCertificates(string uploadURL)
  {
    CertificateData certificateData = this.GetCertificateData(uploadURL);
    Certificate certificate = Certificate.LoadDerWithKey(certificateData.clientCertStream, certificateData.privateKeyStream, certificateData.password);
    this.webClient.Settings.SslClientCertificateRequestHandler = CertificateRequestHandler.CreateRequestHandler(certificate);
    if (FeatureConfiguration.LodisSslAcceptAllCertificates)
      return;
    this.iS5CertificateData = RebexHelper.GetCertificateIssuerData(certificate);
    this.webClient.ValidatingCertificate += new EventHandler<SslCertificateValidationEventArgs>(this.client_ValidatingCertificate);
    this._loggingService.getLogger().LogAppDebug(LoggingContext.PROGRAMSYMANA, "Registered custom validation event handler for certificate validation", memberName: nameof (SetCertificates), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/SyMaNaWebClient.cs", sourceLineNumber: 51);
  }

  public void client_ValidatingCertificate(object sender, SslCertificateValidationEventArgs e)
  {
    Certificate certificate1 = e.CertificateChain[0];
    Certificate certificate2 = e.CertificateChain[1];
    if (this.trustedThumbprints.Contains(certificate1.Thumbprint) || this.trustedThumbprints.Contains(certificate2.Thumbprint))
    {
      e.Accept();
      this._loggingService.getLogger().LogAppDebug(LoggingContext.PROGRAMSYMANA, "Certificate Validation Successful, Thumbprint matched!", memberName: nameof (client_ValidatingCertificate), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/SyMaNaWebClient.cs", sourceLineNumber: 63 /*0x3F*/);
    }
    else if (RebexHelper.IsLodisCertValid(this.iS5CertificateData, RebexHelper.GetCertificateIssuerData(certificate2)))
    {
      e.Accept();
      this.trustedThumbprints.Add(certificate1.Thumbprint);
      this.trustedThumbprints.Add(certificate2.Thumbprint);
      this._loggingService.getLogger().LogAppDebug(LoggingContext.PROGRAMSYMANA, "Certificate Validation Successful, Accepted!", memberName: nameof (client_ValidatingCertificate), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/SyMaNaWebClient.cs", sourceLineNumber: 73);
    }
    else
    {
      e.Reject();
      this._loggingService.getLogger().LogAppDebug(LoggingContext.PROGRAMSYMANA, "Certificate validation is Failed, Hence REJECTED!", memberName: nameof (client_ValidatingCertificate), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/SyMaNaWebClient.cs", sourceLineNumber: 78);
    }
  }

  public Task UploadBinaryToLodis(SyMaNaFirmwareUploadModel binaryUploadModel)
  {
    string uploadUrl = binaryUploadModel.uploadURL;
    ADNetworkRequestTracker networkRequestTracker = new ADNetworkRequestTracker(new Uri(uploadUrl));
    SyMaNaWebClientResponse webClientResponse = new SyMaNaWebClientResponse();
    try
    {
      if (this.webClient == null)
      {
        this.webClient = new Rebex.Net.WebClient();
        this.SetCertificates(uploadUrl);
        RebexHelper.RegisterCipherSuitesForHTTPSUpload(this.webClient);
      }
      int length = System.IO.File.ReadAllBytes(binaryUploadModel.path).Length;
      this.webClient.Headers[HttpRequestHeader.ContentType] = "application/octet-stream";
      this.webClient.Headers[HttpRequestHeader.ContentLength] = length.ToString();
      this.webClient.UploadProgressChanged += new EventHandler<WebClientProgressChangedEventArgs>(this.UploadProgressCallback);
      string str = Encoding.UTF8.GetString(this.webClient.UploadFile(uploadUrl, HTTPMethodType.PUT.ToString(), binaryUploadModel.path));
      if (str.Contains("Package uploaded successfully") || str.Equals("OK", StringComparison.OrdinalIgnoreCase))
      {
        webClientResponse.isSuccess = true;
        this._loggingService.getLogger().LogAppInformation(LoggingContext.LOCAL, "Package Upload Status: " + str, memberName: nameof (UploadBinaryToLodis), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/SyMaNaWebClient.cs", sourceLineNumber: 109);
      }
      else
      {
        webClientResponse.error = new ServiceError(ErrorType.SyMaNa_Websocket_Error, AppResource.BINARY_UPLOAD_FAILED, false);
        webClientResponse.isSuccess = false;
        webClientResponse.message = AppResource.BINARY_UPLOAD_FAILED;
        this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSYMANA, $"Failed to upload Module: {binaryUploadModel.FileName}, responseBody = {str} ", memberName: nameof (UploadBinaryToLodis), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/SyMaNaWebClient.cs", sourceLineNumber: 116);
      }
      binaryUploadModel.response = webClientResponse;
    }
    catch (WebException ex)
    {
      webClientResponse.isSuccess = false;
      webClientResponse.message = AppResource.BINARY_UPLOAD_FAILED;
      binaryUploadModel.response = webClientResponse;
      this._loggingService.getLogger().LogAppError(LoggingContext.LOCAL, "Exception while Uploading binary File: " + ex.Message, memberName: nameof (UploadBinaryToLodis), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/SyMaNaWebClient.cs", sourceLineNumber: 125);
      this._loggingService.getLogger().LogAppError(LoggingContext.LOCAL, $"Binary File upload fail: {binaryUploadModel.FileName},{binaryUploadModel.path}", memberName: nameof (UploadBinaryToLodis), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/SyMaNaWebClient.cs", sourceLineNumber: 126);
      networkRequestTracker.ADTrackException((Exception) ex);
    }
    return Task.CompletedTask;
  }

  private void UploadProgressCallback(object sender, WebClientProgressChangedEventArgs e)
  {
    this.UploadProgressUpdate(e.BytesTransferred);
  }

  private CertificateData GetCertificateData(string uploadURL)
  {
    return uploadURL.Contains("https://127.0.0.1:2844") ? new CertificateData((ISecureStorageService) null, false) : new CertificateData(this._secureStorageService, true);
  }

  public delegate void UploadProgressHandler(long progressInBytes);
}
