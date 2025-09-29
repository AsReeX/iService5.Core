// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Backend.Backend
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Data;
using iService5.Core.Services.Helpers;
using iService5.Core.Services.Platform;
using iService5.Core.Services.User;
using iService5.Core.Services.User.ActivityLogging;
using MvvmCross;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace iService5.Core.Services.Backend;

public class Backend : IBackend<HttpWebRequest>
{
  private readonly IPlatformSpecificServiceLocator _locator;
  private readonly ILoggingService _loggingService;

  public Backend(
    IUserSession _session,
    ILoggingService _logService,
    ISecureStorageService secureStorageService,
    IPlatformSpecificServiceLocator locator)
  {
    this._loggingService = _logService;
    this._userSession = _session;
    this._secureStorageService = secureStorageService;
    this._locator = locator;
  }

  public IUserSession _userSession { get; private set; }

  public ISecureStorageService _secureStorageService { get; private set; }

  public bool ActivatedCancel { get; private set; }

  private void TimeoutCallback(object stateObj, bool timedOut)
  {
    if (!timedOut)
      return;
    iService5.Core.Services.Backend.Backend.RequestStatus requestStatus = stateObj as iService5.Core.Services.Backend.Backend.RequestStatus;
    if (requestStatus.request != null)
    {
      this._loggingService.getLogger().LogAppError(LoggingContext.BACKEND, "Timeout passed. Aborting. Request absolute uri: " + requestStatus.request.RequestUri.AbsoluteUri, memberName: nameof (TimeoutCallback), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/Backend.cs", sourceLineNumber: 83);
      requestStatus.status = BackendRequestStatus.RES_TimeOut;
      requestStatus.request.Abort();
      requestStatus.done.Set();
      if (requestStatus.handle != null)
        requestStatus.handle.Unregister((WaitHandle) null);
    }
  }

  private void ReadTimeoutCallback(object stateObj, bool timedOut)
  {
    iService5.Core.Services.Backend.Backend.RequestStatus requestStatus = stateObj as iService5.Core.Services.Backend.Backend.RequestStatus;
    if (timedOut)
    {
      if (requestStatus.request != null)
        requestStatus.request.Abort();
      requestStatus.token.Cancel();
      requestStatus.done.Set();
      if (requestStatus.handle != null)
        requestStatus.handle.Unregister((WaitHandle) null);
    }
    else if (requestStatus.handle != null)
      requestStatus.handle.Unregister((WaitHandle) null);
    if (!this._userSession.isDownloadCanceled() || requestStatus == null)
      return;
    if (requestStatus.request != null)
      requestStatus.request.Abort();
    requestStatus.token.Cancel();
    this.ActivatedCancel = false;
  }

  public async Task<BackendRequestStatus> executeBinaryRequest(
    BinaryDownloadTask _bdt,
    HttpWebRequest request)
  {
    iService5.Core.Services.Backend.Backend.RequestStatus status = new iService5.Core.Services.Backend.Backend.RequestStatus();
    ADNetworkRequestTracker tracker = new ADNetworkRequestTracker(request.RequestUri);
    try
    {
      status.status = BackendRequestStatus.RES_NResp;
      status.request = request;
      status.done = new AutoResetEvent(false);
      status.token = new CancellationTokenSource();
      IAsyncResult result = request.BeginGetResponse(new AsyncCallback(this.ResponseCallback), (object) status);
      status.handle = ThreadPool.RegisterWaitForSingleObject(result.AsyncWaitHandle, new WaitOrTimerCallback(this.TimeoutCallback), (object) status, 30000, true);
      status.done.WaitOne();
      if (status.status != 0)
        return status.status;
      if (status.handle != null)
        status.handle.Unregister((WaitHandle) null);
      result = (IAsyncResult) null;
    }
    catch (Exception ex)
    {
      status.ev.Set();
      tracker.ADTrackException(ex);
      this._loggingService.getLogger().LogAppError(LoggingContext.BACKEND, "Failed with " + ex.Message, ex, nameof (executeBinaryRequest), "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/Backend.cs", 148);
      return BackendRequestStatus.RES_Exception;
    }
    HttpStatusCode statusCode = status.response.StatusCode;
    tracker.ADTrackResponse((int) statusCode);
    DownloadProxy proxy1 = _bdt.proxy;
    int num;
    if ((proxy1 != null ? (proxy1.FileType != FileType.Ppf ? 1 : 0) : 1) != 0 && _bdt.proxy != null)
    {
      DownloadProxy proxy2 = _bdt.proxy;
      num = proxy2 != null ? (proxy2.FileType != FileType.BundledPpf ? 1 : 0) : 1;
    }
    else
      num = 0;
    if (num != 0)
    {
      string headerValue = status.response.Headers["Content-Disposition"];
      string headerValueEnding = "";
      string[] headerValueSubStrings = Regex.Split(headerValue, "\\.");
      headerValueEnding = headerValueSubStrings[headerValueSubStrings.Length - 1];
      if (headerValueEnding.Contains("cpio"))
        _bdt.proxy.fileExtension = iService5.Core.Services.Backend.Backend.binType.cpio;
      else if (headerValueEnding.Contains("zip"))
        _bdt.proxy.fileExtension = iService5.Core.Services.Backend.Backend.binType.zip;
      else if (headerValueEnding.Contains("gz"))
        _bdt.proxy.fileExtension = iService5.Core.Services.Backend.Backend.binType.targz;
      ((UserSession) _bdt._session)._binaryDownloadSession.setResponseFile(Path.Combine(this._locator.GetPlatformSpecificService().getFolder(), _bdt.proxy.ToFileNameNewFdsExtension(_bdt.proxy.fileExtension)));
      headerValue = (string) null;
      headerValueEnding = (string) null;
      headerValueSubStrings = (string[]) null;
    }
    IBinaryDownloadSession<HttpWebRequest> bSession = Mvx.IoCProvider.Resolve<IBinaryDownloadSession<HttpWebRequest>>();
    HttpStatusCode httpStatusCode = statusCode;
    switch (httpStatusCode)
    {
      case HttpStatusCode.OK:
        try
        {
          using (Stream receiveStream = status.response.GetResponseStream())
          {
            if (request.RequestUri.AbsoluteUri.Contains("ppf/"))
            {
              using (StreamReader reader = new StreamReader(receiveStream))
              {
                string responseString = reader.ReadToEnd();
                _bdt.proxy.PPFData = JsonConvert.DeserializeObject<PpfsInfo>(responseString);
                responseString = (string) null;
              }
            }
            else
            {
              ulong totalRead = 0;
              byte[] buffer = new byte[8192 /*0x2000*/];
              int readBytes = 1;
              do
              {
                status.ev = new AutoResetEvent(false);
                status.handle = ThreadPool.RegisterWaitForSingleObject((WaitHandle) status.ev, new WaitOrTimerCallback(this.ReadTimeoutCallback), (object) status, 10000, true);
                readBytes = await receiveStream.ReadAsync(buffer, 0, 8192 /*0x2000*/, status.token.Token).ConfigureAwait(false);
                if (readBytes > 0)
                {
                  if (status.token.IsCancellationRequested)
                  {
                    status.response.Close();
                    bSession?.CompletionCallback(true);
                    return BackendRequestStatus.RES_NIncomplete;
                  }
                  status.ev.Set();
                  totalRead += (ulong) readBytes;
                  if (bSession != null)
                  {
                    bSession.SaveDataBlock(buffer, readBytes);
                    if (request.RequestUri.AbsoluteUri.Contains("firmware/files/"))
                      bSession.ProgressCallback(totalRead, (ulong) status.response.ContentLength, true);
                    else
                      bSession.ProgressCallback(totalRead, (ulong) status.response.ContentLength);
                  }
                }
                else
                  break;
              }
              while (readBytes > 0);
              buffer = (byte[]) null;
            }
            bSession?.CompletionCallback(true);
            if (status.handle != null)
              status.handle.Unregister((WaitHandle) null);
            status.response.Close();
            return BackendRequestStatus.RES_OK;
          }
        }
        catch (Exception ex)
        {
          this._loggingService.getLogger().LogAppError(LoggingContext.BACKEND, $"Failed to complete download for request {status.request.RequestUri} Exception details: {ex.Message}", memberName: nameof (executeBinaryRequest), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/Backend.cs", sourceLineNumber: 242);
          status.response.Close();
          bSession?.CompletionCallback(true);
          return BackendRequestStatus.RES_NIncomplete;
        }
      case HttpStatusCode.Unauthorized:
        return BackendRequestStatus.RES_UnAuthorized;
      default:
        return BackendRequestStatus.RES_NResp;
    }
  }

  private void ResponseCallback(IAsyncResult asynchronousResult)
  {
    iService5.Core.Services.Backend.Backend.RequestStatus asyncState = (iService5.Core.Services.Backend.Backend.RequestStatus) asynchronousResult.AsyncState;
    try
    {
      HttpWebRequest request = asyncState.request;
      asyncState.response = (HttpWebResponse) request.EndGetResponse(asynchronousResult);
      asyncState.status = BackendRequestStatus.RES_OK;
    }
    catch (WebException ex)
    {
      asyncState.response = (HttpWebResponse) ex.Response;
      this._userSession.setServiceError(new ServiceError(ex.Status, ErrorType.Technical, ex.Message, true));
      if (asyncState.response != null)
      {
        this._loggingService.getLogger().LogAppError(LoggingContext.BACKEND, $"Failed to get response for request {asyncState.request.RequestUri}, Status Code {asyncState.response.StatusCode}, Exception Details:" + ex.Message, memberName: nameof (ResponseCallback), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/Backend.cs", sourceLineNumber: 270);
        if (asyncState.response.StatusCode == HttpStatusCode.Unauthorized)
          asyncState.status = BackendRequestStatus.RES_UnAuthorized;
        else if (asyncState.response.StatusCode == HttpStatusCode.BadRequest || asyncState.response.StatusCode == HttpStatusCode.NotFound)
          asyncState.status = BackendRequestStatus.RES_NotFound;
        else if (asyncState.response.StatusCode == HttpStatusCode.GatewayTimeout || asyncState.response.StatusCode == HttpStatusCode.BadGateway)
        {
          asyncState.status = BackendRequestStatus.RES_TimeOut;
          this._loggingService.getLogger().LogAppDebug(LoggingContext.BACKEND, "GatewayTimeout", memberName: nameof (ResponseCallback), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/Backend.cs", sourceLineNumber: 283);
        }
        else
          asyncState.status = BackendRequestStatus.RES_BadRequest;
      }
      else if (ex.Status == WebExceptionStatus.TrustFailure)
        asyncState.status = BackendRequestStatus.RES_CertificateUnknown;
      else if (ex.Status == WebExceptionStatus.Timeout || ex.Status == WebExceptionStatus.RequestCanceled)
      {
        asyncState.status = BackendRequestStatus.RES_TimeOut;
        this._loggingService.getLogger().LogAppDebug(LoggingContext.BACKEND, "Timeout Exception " + ex.Message, memberName: nameof (ResponseCallback), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/Backend.cs", sourceLineNumber: 298);
      }
      else
      {
        asyncState.status = BackendRequestStatus.RES_Exception;
        this._loggingService.getLogger().LogAppError(LoggingContext.BACKEND, "Failed to get response", (Exception) ex, nameof (ResponseCallback), "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/Backend.cs", 303);
      }
    }
    catch (Exception ex)
    {
      asyncState.status = BackendRequestStatus.RES_Exception;
      this._loggingService.getLogger().LogAppError(LoggingContext.BACKEND, "Failed to get response", ex, nameof (ResponseCallback), "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/Backend.cs", 309);
    }
    try
    {
      asyncState.done.Set();
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppError(LoggingContext.BACKEND, "Exception in status.done.Set(): " + ex.Message, memberName: nameof (ResponseCallback), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/Backend.cs", sourceLineNumber: 317);
    }
  }

  public BackendRequestStatus executeRequest(HttpWebRequest request)
  {
    this._loggingService.getLogger().LogAppDebug(LoggingContext.BACKEND, "Requesting " + request.Address?.ToString(), memberName: nameof (executeRequest), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/Backend.cs", sourceLineNumber: 329);
    ADNetworkRequestTracker networkRequestTracker = new ADNetworkRequestTracker(request.RequestUri);
    HttpWebResponse response;
    try
    {
      response = (HttpWebResponse) request.GetResponse();
      this._loggingService.getLogger().LogAppDebug(LoggingContext.BACKEND, "Response received for request:" + request.RequestUri?.ToString(), memberName: nameof (executeRequest), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/Backend.cs", sourceLineNumber: 336);
    }
    catch (WebException ex)
    {
      this._loggingService.getLogger().LogAppError(LoggingContext.BACKEND, "Failed to get response", (Exception) ex, nameof (executeRequest), "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/Backend.cs", 340);
      this._userSession.setServiceError(new ServiceError(ex.Status, ErrorType.Technical, ex.Message, true));
      networkRequestTracker.ADTrackException((Exception) ex);
      response = (HttpWebResponse) ex.Response;
      if (response != null)
      {
        if (response.StatusCode == HttpStatusCode.Unauthorized)
          return BackendRequestStatus.RES_UnAuthorized;
      }
      else
      {
        if (ex.Status == WebExceptionStatus.TrustFailure)
          return BackendRequestStatus.RES_CertificateUnknown;
        if (ex.Status == WebExceptionStatus.Timeout || ex.Status == WebExceptionStatus.RequestCanceled)
        {
          this._loggingService.getLogger().LogAppDebug(LoggingContext.BACKEND, "Timeout Exception " + ex.Message, memberName: nameof (executeRequest), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/Backend.cs", sourceLineNumber: 358);
          return BackendRequestStatus.RES_TimeOut;
        }
        this._loggingService.getLogger().LogAppError(LoggingContext.BACKEND, "Failed to get response", (Exception) ex, nameof (executeRequest), "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/Backend.cs", 363);
        return BackendRequestStatus.RES_Exception;
      }
    }
    HttpStatusCode statusCode = response.StatusCode;
    this._userSession.setProperty((object) response.StatusCode);
    this._userSession.setProperty((object) response.Headers);
    networkRequestTracker.ADTrackResponse((int) statusCode);
    StreamReader streamReader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
    this._userSession.setProperty((object) streamReader.ReadToEnd());
    response.Close();
    streamReader.Close();
    if (statusCode == HttpStatusCode.OK)
      return BackendRequestStatus.RES_OK;
    return statusCode == HttpStatusCode.Unauthorized ? BackendRequestStatus.RES_UnAuthorized : BackendRequestStatus.RES_NResp;
  }

  internal void ResponseAyncCallback(IAsyncResult asynchronousResult)
  {
    iService5.Core.Services.Backend.Backend.RequestStatus asyncState = (iService5.Core.Services.Backend.Backend.RequestStatus) asynchronousResult.AsyncState;
    try
    {
      this._loggingService.getLogger().LogAppDebug(LoggingContext.BACKEND, "EndGetResponse", memberName: nameof (ResponseAyncCallback), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/Backend.cs", sourceLineNumber: 390);
      HttpWebRequest request = asyncState.request;
      asyncState.response = (HttpWebResponse) request.EndGetResponse(asynchronousResult);
      asyncState.status = BackendRequestStatus.RES_OK;
    }
    catch (WebException ex)
    {
      this._loggingService.getLogger().LogAppError(LoggingContext.BACKEND, "Failed to get response", (Exception) ex, nameof (ResponseAyncCallback), "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/Backend.cs", 397);
      this._userSession.setServiceError(new ServiceError(ex.Status, ErrorType.Technical, ex.Message, true));
      asyncState.response = (HttpWebResponse) ex.Response;
      if (asyncState.response != null)
      {
        if (asyncState.response.StatusCode == HttpStatusCode.Unauthorized)
        {
          asyncState.status = BackendRequestStatus.RES_UnAuthorized;
        }
        else
        {
          asyncState.status = BackendRequestStatus.RES_Exception;
          this._loggingService.getLogger().LogAppError(LoggingContext.BACKEND, "Exception Details:", (Exception) ex, nameof (ResponseAyncCallback), "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/Backend.cs", 410);
        }
      }
      else if (ex.Status == WebExceptionStatus.TrustFailure)
        asyncState.status = BackendRequestStatus.RES_CertificateUnknown;
      else if (ex.Status == WebExceptionStatus.Timeout || ex.Status == WebExceptionStatus.RequestCanceled)
      {
        asyncState.status = BackendRequestStatus.RES_TimeOut;
        this._loggingService.getLogger().LogAppDebug(LoggingContext.BACKEND, "Timeout Exception " + ex.Message, memberName: nameof (ResponseAyncCallback), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/Backend.cs", sourceLineNumber: 421);
      }
      else
      {
        asyncState.status = BackendRequestStatus.RES_Exception;
        this._loggingService.getLogger().LogAppError(LoggingContext.BACKEND, "Failed to get response", (Exception) ex, nameof (ResponseAyncCallback), "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/Backend.cs", 426);
      }
    }
    asyncState.done.Set();
  }

  public async Task<BackendRequestStatus> executeAsyncRequest(HttpWebRequest request)
  {
    this._loggingService.getLogger().LogAppDebug(LoggingContext.BACKEND, "Requesting " + request.Address?.ToString(), memberName: nameof (executeAsyncRequest), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/Backend.cs", sourceLineNumber: 433);
    iService5.Core.Services.Backend.Backend.RequestStatus status = new iService5.Core.Services.Backend.Backend.RequestStatus();
    status.status = BackendRequestStatus.RES_NInternet;
    status.request = request;
    status.done = new AutoResetEvent(false);
    status.token = new CancellationTokenSource();
    ADNetworkRequestTracker tracker = new ADNetworkRequestTracker(request.RequestUri);
    try
    {
      request.AllowReadStreamBuffering = false;
      IAsyncResult result = request.BeginGetResponse(new AsyncCallback(this.ResponseAyncCallback), (object) status);
      status.handle = ThreadPool.RegisterWaitForSingleObject(result.AsyncWaitHandle, new WaitOrTimerCallback(this.TimeoutCallback), (object) status, 30000, true);
      this._loggingService.getLogger().LogAppDebug(LoggingContext.BACKEND, $"Have response - {request.HaveResponse.ToString()} for request {request.RequestUri?.ToString()}", memberName: nameof (executeAsyncRequest), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/Backend.cs", sourceLineNumber: 445);
      status.done.WaitOne();
      if (status.status != 0)
      {
        this._loggingService.getLogger().LogAppError(LoggingContext.BACKEND, "Request failed. " + status.status.ToString(), memberName: nameof (executeAsyncRequest), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/Backend.cs", sourceLineNumber: 449);
        return status.status;
      }
      if (status.handle != null)
        status.handle.Unregister((WaitHandle) null);
      this._loggingService.getLogger().LogAppDebug(LoggingContext.BACKEND, "Request passed, trying to read data for request " + request.RequestUri?.ToString(), memberName: nameof (executeAsyncRequest), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/Backend.cs", sourceLineNumber: 457);
      result = (IAsyncResult) null;
    }
    catch (Exception ex)
    {
      if (status.ev != null)
        status.ev.Set();
      this._loggingService.getLogger().LogAppError(LoggingContext.BACKEND, "Failed with " + ex.Message, ex, nameof (executeAsyncRequest), "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/Backend.cs", 463);
      this._userSession.setServiceError(new ServiceError(ErrorType.Technical, ex.Message, true));
      tracker.ADTrackException(ex);
      return BackendRequestStatus.RES_Exception;
    }
    HttpStatusCode retCode = status.response.StatusCode;
    this._userSession.setProperty((object) status.response.StatusCode);
    this._userSession.setProperty((object) status.response.Headers);
    this._loggingService.getLogger().LogAppInformation(LoggingContext.BACKEND, $"Return code was {retCode.ToString()} content {status.response.ContentType.ToLowerInvariant()}", memberName: nameof (executeAsyncRequest), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/Backend.cs", sourceLineNumber: 471);
    Stream receiveStream = status.response.GetResponseStream();
    StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
    string body = readStream.ReadToEnd();
    IBackendDetails<HttpWebRequest> bEnd = Mvx.IoCProvider.Resolve<IBackendDetails<HttpWebRequest>>();
    tracker.ADTrackResponse((int) retCode);
    if (request.HaveResponse && status.response.ResponseUri.AbsoluteUri == bEnd.getAuthenticateRequest().RequestUri.AbsoluteUri)
    {
      JObject details = JObject.Parse(body);
      bool isJWTTokenStored = await this._secureStorageService.setJWTToken(details["token"].ToString());
      CoreApp.settings.UpdateItem(new Settings("JwtTokenTimestamp", DateTime.Now.ToString()));
      if (isJWTTokenStored)
        this._userSession.ResetOnlineSession();
      else
        this._loggingService.getLogger().LogAppDebug(LoggingContext.BACKEND, "JWT Token and LastfetchedJWTTokenDateTime NOT stored successfully in secure storagen.Online session expiration has not reset ", memberName: nameof (executeAsyncRequest), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/Backend.cs", sourceLineNumber: 489);
      details = (JObject) null;
    }
    if (request.HaveResponse && status.response.ResponseUri.AbsoluteUri == bEnd.getMetadataPassPhrase().RequestUri.AbsoluteUri)
    {
      JObject details = JObject.Parse(body);
      bool isDBPasswordStored = await this._secureStorageService.SetDBPasswordInSecureStorage(details["passphrase"].ToString());
      bool isTempDBPasswordStored = await this._secureStorageService.SetTempDBPasswordInSecureStorage(details["passphrase"].ToString());
      try
      {
        bool flag = await this._secureStorageService.SetWiFiPasswordInSecureStorage(details["wifi"].ToString());
        int num = flag ? 1 : 0;
      }
      catch (Exception ex)
      {
        this._loggingService.getLogger().LogAppDebug(LoggingContext.BACKEND, "WiFi password not available", memberName: nameof (executeAsyncRequest), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/Backend.cs", sourceLineNumber: 503);
      }
      try
      {
        bool flag = await this._secureStorageService.SetWiFiBridgePasswordInSecureStorage("iService");
        int num = flag ? 1 : 0;
      }
      catch (Exception ex)
      {
        this._loggingService.getLogger().LogAppDebug(LoggingContext.BACKEND, "WiFi Bridge password not available", memberName: nameof (executeAsyncRequest), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/Backend.cs", sourceLineNumber: 511 /*0x01FF*/);
      }
      if (isDBPasswordStored & isTempDBPasswordStored)
        this._loggingService.getLogger().LogAppDebug(LoggingContext.BACKEND, "DB Password & Temp DB Password stored.", memberName: nameof (executeAsyncRequest), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/Backend.cs", sourceLineNumber: 514);
      details = (JObject) null;
    }
    if (request.HaveResponse && status.response.ResponseUri.AbsoluteUri == bEnd.postSignCertificate().RequestUri.AbsoluteUri)
    {
      bool isNewSignedCertificateStored = false;
      string newSignedCertificate = body;
      try
      {
        isNewSignedCertificateStored = await this._secureStorageService.SetSignedCertificate(newSignedCertificate);
        if (isNewSignedCertificateStored)
        {
          this._userSession.CertificateHasBeenUpdated(true);
          this._loggingService.getLogger().LogAppInformation(LoggingContext.BACKEND, "New Signed Certificate stored.", memberName: nameof (executeAsyncRequest), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/Backend.cs", sourceLineNumber: 526);
          this._userSession.CertificateIsValid(true);
        }
        else
        {
          this._userSession.CertificateHasBeenUpdated(false);
          this._loggingService.getLogger().LogAppDebug(LoggingContext.BACKEND, "New Signed Certificate not stored  ", memberName: nameof (executeAsyncRequest), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/Backend.cs", sourceLineNumber: 532);
        }
      }
      catch (Exception ex)
      {
        this._userSession.CertificateHasBeenUpdated(false);
        this._loggingService.getLogger().LogAppDebug(LoggingContext.BACKEND, "Signed Certificate storage exception : " + ex.Message, memberName: nameof (executeAsyncRequest), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/Backend.cs", sourceLineNumber: 538);
      }
      newSignedCertificate = (string) null;
    }
    if (!UtilityFunctions.isCertificateInResponse(body))
      this._userSession.setProperty((object) body);
    status.response.Close();
    readStream.Close();
    return retCode != HttpStatusCode.OK ? BackendRequestStatus.RES_NResp : BackendRequestStatus.RES_OK;
  }

  public void CancelCurrentRequest() => throw new NotImplementedException();

  private static class binType
  {
    public static readonly string cpio = ".cpio";
    public static readonly string zip = ".zip";
    public static readonly string targz = ".tar.gz";

    public static string getExtension()
    {
      return iService5.Core.Services.Backend.Backend.binType.cpio?.ToString() ?? iService5.Core.Services.Backend.Backend.binType.zip?.ToString() ?? iService5.Core.Services.Backend.Backend.binType.targz?.ToString() ?? (string) null;
    }
  }

  internal class RequestStatus
  {
    public HttpWebResponse response;
    public HttpWebRequest request;
    public Stream receiveStream;
    public BackendRequestStatus status;
    public AutoResetEvent done;
    public CancellationTokenSource token;
    public RegisteredWaitHandle handle;
    public AutoResetEvent ev;
  }
}
