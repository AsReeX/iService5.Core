// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Backend.BackendDetails
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Helpers;
using System.Net;

#nullable disable
namespace iService5.Core.Services.Backend;

public class BackendDetails : IBackendDetails<HttpWebRequest>
{
  private readonly string m_Protocol;
  private readonly string m_BasePath;
  private readonly string m_Server;
  private readonly string m_APIVersion;
  private readonly string m_Stage;
  public readonly string m_X_API_Key;
  private readonly string authenticateRequestRoute = "authenticate";
  private readonly string metadataChecksumRequestRoute = "metadata/checksum";
  private readonly string metadataRequestRoute = "metadata";
  private readonly string binaryRequestRoute = "firmware/files/";
  private readonly string passPhraseRequestRoute = "security";
  private readonly string feedbackFormRoute = "support/mail";
  private readonly string signCertificateRoute = "security/sign-certificate";

  public BackendDetails(
    string Protocol,
    string Server,
    string BasePath,
    string APIVersion,
    string Stage,
    string X_API_Key)
  {
    this.m_Protocol = Protocol;
    this.m_Server = Server;
    this.m_BasePath = BasePath;
    this.m_APIVersion = APIVersion;
    this.m_Stage = Stage;
    this.m_X_API_Key = X_API_Key;
  }

  public BackendDetails(
    string Protocol,
    string Server,
    string APIVersion,
    string Stage,
    string X_API_Key)
  {
    this.m_Protocol = Protocol;
    this.m_Server = Server;
    this.m_BasePath = "";
    this.m_APIVersion = APIVersion;
    this.m_Stage = Stage;
    this.m_X_API_Key = X_API_Key;
  }

  public string getRequestRoute(RequestType requestType)
  {
    switch (requestType)
    {
      case RequestType.AuthenticateRequest:
        return this.authenticateRequestRoute;
      case RequestType.MetadataChecksumRequest:
        return this.metadataChecksumRequestRoute;
      case RequestType.MetadataRequest:
        return this.metadataRequestRoute;
      case RequestType.BinaryRequest:
        return this.binaryRequestRoute;
      case RequestType.MetadataPassPhraseRequest:
        return this.passPhraseRequestRoute;
      case RequestType.PostFeedbackFormRequest:
        return this.feedbackFormRoute;
      case RequestType.PostSignCertificate:
        return this.signCertificateRoute;
      default:
        return (string) null;
    }
  }

  private HttpWebRequest getRequest(string path, bool useVersion, bool useBasePath = false, bool useStage = false)
  {
    HttpWebRequest request = (HttpWebRequest) WebRequest.Create($"{this.m_Protocol}://{this.m_Server}/{(!useBasePath || !(this.m_BasePath != "") ? "" : this.m_BasePath + "/")}{(!useStage || !(this.m_Stage != "") ? "" : this.m_Stage + "/")}{(!useVersion || !(this.m_APIVersion != "") ? "" : this.m_APIVersion + "/")}{path}");
    request.Headers["X-API-KEY"] = this.m_X_API_Key;
    return request;
  }

  public HttpWebRequest getAuthenticateRequest()
  {
    HttpWebRequest request = this.getRequest(this.getRequestRoute(RequestType.AuthenticateRequest), false, true, true);
    this.addHTTPDataAttributes(request, HTTPMethodType.GET, false);
    return request;
  }

  public HttpWebRequest getMetadataRequest()
  {
    HttpWebRequest request = this.getRequest(this.getRequestRoute(RequestType.MetadataRequest), true, true, true);
    this.addHTTPDataAttributes(request, HTTPMethodType.GET, true);
    return request;
  }

  public HttpWebRequest getBinaryDataRequest(string fileId)
  {
    HttpWebRequest request = this.getRequest(this.getRequestRoute(RequestType.BinaryRequest) + fileId, true, true, true);
    this.addHTTPDataAttributes(request, HTTPMethodType.GET, true);
    return request;
  }

  public HttpWebRequest getMetadataChecksumRequest()
  {
    HttpWebRequest request = this.getRequest(this.getRequestRoute(RequestType.MetadataChecksumRequest), true, true, true);
    this.addHTTPDataAttributes(request, HTTPMethodType.GET, true);
    return request;
  }

  public HttpWebRequest getMetadataPassPhrase()
  {
    HttpWebRequest request = this.getRequest(this.getRequestRoute(RequestType.MetadataPassPhraseRequest), true, true, true);
    this.addHTTPDataAttributes(request, HTTPMethodType.GET, false);
    return request;
  }

  public HttpWebRequest postFeedbackForm()
  {
    HttpWebRequest request = this.getRequest(this.getRequestRoute(RequestType.PostFeedbackFormRequest), true, true, true);
    this.addHTTPDataAttributes(request, HTTPMethodType.POST, false);
    return request;
  }

  public HttpWebRequest getPpfDataRequest(string vib, string ki, string moduleId, string version)
  {
    HttpWebRequest request = this.getRequest($"ppf/enumber/{vib}/{ki}/module/{moduleId}/version/{version}", true, true, true);
    this.addHTTPDataAttributes(request, HTTPMethodType.GET, false);
    return request;
  }

  public HttpWebRequest getBundlePpfDataRequest(string vib, string ki)
  {
    HttpWebRequest request = this.getRequest($"ppf/enumber/{vib}/{ki}", true, true, true);
    this.addHTTPDataAttributes(request, HTTPMethodType.GET, false);
    return request;
  }

  public HttpWebRequest postSignCertificate()
  {
    HttpWebRequest request = this.getRequest(this.getRequestRoute(RequestType.PostSignCertificate), true, true, true);
    this.addHTTPDataAttributes(request, HTTPMethodType.POST, false);
    return request;
  }

  internal HttpWebRequest addHTTPDataAttributes(
    HttpWebRequest request,
    HTTPMethodType httpMethod,
    bool db2HeaderNeeded)
  {
    request.Accept = "*/*";
    request.Method = httpMethod.ToString();
    if (db2HeaderNeeded)
      this.addHeaders(request, "dbVersion", "2");
    return request;
  }

  private void addHeaders(HttpWebRequest request, string key, string value)
  {
    request.Headers.Add(key, value);
  }
}
