// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Backend.BinaryDownloadSession
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Data;
using iService5.Core.Services.Helpers;
using iService5.Core.Services.Platform;
using iService5.Core.Services.User;
using iService5.Core.Services.User.ActivityLogging;
using MvvmCross;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;

#nullable disable
namespace iService5.Core.Services.Backend;

public class BinaryDownloadSession : IBinaryDownloadSession<HttpWebRequest>
{
  private string _targetFile;
  private BinaryWriter writer = (BinaryWriter) null;
  private iService5.Core.Services.User.progressCallback _progressCallback = (iService5.Core.Services.User.progressCallback) null;
  public static readonly ILoggingService loggingService = Mvx.IoCProvider.Resolve<ILoggingService>();
  public static readonly ISecureStorageService secureStorageService = Mvx.IoCProvider.Resolve<ISecureStorageService>();

  public void CompletionCallback(bool errorless)
  {
    if (this.writer == null)
      return;
    this.writer.Close();
    this.writer = (BinaryWriter) null;
  }

  public void ProgressCallback(ulong progress, ulong total, bool isBinaryDownload = false)
  {
    if (this._progressCallback == null)
      return;
    if (isBinaryDownload)
      this._progressCallback((double) progress);
    else
      this._progressCallback((double) progress / (double) total);
  }

  public void SaveDataBlock(byte[] buffer, int length)
  {
    if (this.writer == null)
      this.writer = new BinaryWriter((Stream) System.IO.File.Open(this._targetFile, FileMode.Create));
    this.writer.Write(buffer, 0, length);
  }

  public void setResponseFile(string path)
  {
    if (this.writer != null)
    {
      this.writer.Close();
      this.writer = (BinaryWriter) null;
    }
    this._targetFile = path;
  }

  public BackendRequestStatus getMetadataChecksum()
  {
    IUserAccount userAccount = Mvx.IoCProvider.Resolve<IUserAccount>();
    string encoded1 = "";
    BackendRequestStatus encoded2 = userAccount.getEncoded(out encoded1);
    if (encoded2 != 0)
      return encoded2;
    BinaryDownloadSession.loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "Begin Metadata Async Request", memberName: nameof (getMetadataChecksum), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/BinaryDownloadSession.cs", sourceLineNumber: 71);
    IBackendDetails<HttpWebRequest> backendDetails = Mvx.IoCProvider.Resolve<IBackendDetails<HttpWebRequest>>();
    HttpWebRequest metadataChecksumRequest;
    try
    {
      metadataChecksumRequest = backendDetails.getMetadataChecksumRequest();
      metadataChecksumRequest.KeepAlive = false;
      metadataChecksumRequest.ProtocolVersion = HttpVersion.Version11;
    }
    catch (NotSupportedException ex)
    {
      BinaryDownloadSession.loggingService.getLogger().LogAppError(LoggingContext.BACKEND, "Invoked method is not supported", memberName: nameof (getMetadataChecksum), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/BinaryDownloadSession.cs", sourceLineNumber: 84);
      return BackendRequestStatus.RES_NURL;
    }
    catch (UriFormatException ex)
    {
      BinaryDownloadSession.loggingService.getLogger().LogAppError(LoggingContext.BACKEND, "An invalid URI is detected.", memberName: nameof (getMetadataChecksum), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/BinaryDownloadSession.cs", sourceLineNumber: 89);
      return BackendRequestStatus.RES_NURL;
    }
    metadataChecksumRequest.Headers.Add("Authorization", "Bearer " + this.fetchJWTToken());
    return Mvx.IoCProvider.Resolve<IBackend<HttpWebRequest>>().executeRequest(metadataChecksumRequest);
  }

  public async Task<BackendRequestStatus> updateMetadataAsync(
    BinaryDownloadTask _bdt,
    iService5.Core.Services.User.progressCallback _pcb)
  {
    this._progressCallback = _pcb;
    IUserAccount user = Mvx.IoCProvider.Resolve<IUserAccount>();
    string encoded = "";
    BackendRequestStatus res = user.getEncoded(out encoded);
    if (res != 0)
      return res;
    BinaryDownloadSession.loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "Begin Metadata Async Request", memberName: nameof (updateMetadataAsync), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/BinaryDownloadSession.cs", sourceLineNumber: 105);
    IBackendDetails<HttpWebRequest> bEnd = Mvx.IoCProvider.Resolve<IBackendDetails<HttpWebRequest>>();
    HttpWebRequest httpWebRequest;
    try
    {
      httpWebRequest = bEnd.getMetadataRequest();
    }
    catch (NotSupportedException ex)
    {
      BinaryDownloadSession.loggingService.getLogger().LogAppError(LoggingContext.BACKEND, "Invoked method is not supported", memberName: nameof (updateMetadataAsync), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/BinaryDownloadSession.cs", sourceLineNumber: 115);
      return BackendRequestStatus.RES_NURL;
    }
    catch (UriFormatException ex)
    {
      BinaryDownloadSession.loggingService.getLogger().LogAppError(LoggingContext.BACKEND, "An invalid URI is detected.", memberName: nameof (updateMetadataAsync), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/BinaryDownloadSession.cs", sourceLineNumber: 120);
      return BackendRequestStatus.RES_NURL;
    }
    httpWebRequest.Headers.Add("Authorization", "Bearer " + this.fetchJWTToken());
    IBackend<HttpWebRequest> bEndInst = Mvx.IoCProvider.Resolve<IBackend<HttpWebRequest>>();
    BackendRequestStatus backendRequestStatus = await bEndInst.executeBinaryRequest(_bdt, httpWebRequest);
    return backendRequestStatus;
  }

  public async Task<BackendRequestStatus> authenticateDBPassword()
  {
    IUserSession userSession = Mvx.IoCProvider.Resolve<IUserSession>();
    if (!userSession.IsHostReachable())
      return BackendRequestStatus.RES_NInternet;
    IUserAccount user = Mvx.IoCProvider.Resolve<IUserAccount>();
    string encoded = "";
    BackendRequestStatus res = user.getEncoded(out encoded);
    if (res != 0)
      return res;
    IBackendDetails<HttpWebRequest> bEnd = Mvx.IoCProvider.Resolve<IBackendDetails<HttpWebRequest>>();
    HttpWebRequest httpWebRequest;
    try
    {
      httpWebRequest = bEnd.getMetadataPassPhrase();
    }
    catch (NotSupportedException ex)
    {
      BinaryDownloadSession.loggingService.getLogger().LogAppError(LoggingContext.BACKEND, "Invoked method is not supported", memberName: nameof (authenticateDBPassword), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/BinaryDownloadSession.cs", sourceLineNumber: 146);
      return BackendRequestStatus.RES_NURL;
    }
    catch (UriFormatException ex)
    {
      BinaryDownloadSession.loggingService.getLogger().LogAppError(LoggingContext.BACKEND, "An invalid URI is detected.", memberName: nameof (authenticateDBPassword), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/BinaryDownloadSession.cs", sourceLineNumber: 151);
      return BackendRequestStatus.RES_NURL;
    }
    string jwtToken = BinaryDownloadSession.secureStorageService.getJWTToken().Result;
    if (!string.IsNullOrWhiteSpace(jwtToken))
      httpWebRequest.Headers.Add("Authorization", "Bearer " + jwtToken);
    IBackend<HttpWebRequest> bEndInst = Mvx.IoCProvider.Resolve<IBackend<HttpWebRequest>>();
    BackendRequestStatus backendRequestStatus = await bEndInst.executeAsyncRequest(httpWebRequest);
    return backendRequestStatus;
  }

  public static void CreateZipFile(string fileName, IEnumerable<string> files)
  {
    try
    {
      string str = $"{Constants.BridgeLogDirPath}/{fileName}";
      if (UtilityFunctions.fileExists($"{Constants.BridgeLogDirName}/{fileName}"))
        System.IO.File.Delete(str);
      ZipArchive destination = ZipFile.Open(str, ZipArchiveMode.Create);
      foreach (string file in files)
      {
        string sourceFileName = $"{Constants.BridgeLogDirPath}/{file}";
        destination.CreateEntryFromFile(sourceFileName, file, CompressionLevel.Optimal);
      }
      BinaryDownloadSession.loggingService.getLogger().LogAppInformation(LoggingContext.LOCAL, "Files are zipped", memberName: nameof (CreateZipFile), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/BinaryDownloadSession.cs", sourceLineNumber: 185);
      destination.Dispose();
    }
    catch (Exception ex)
    {
      BinaryDownloadSession.loggingService.getLogger().LogAppError(LoggingContext.BACKEND, "Exception occured while zipping files: " + ex?.ToString(), memberName: nameof (CreateZipFile), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/BinaryDownloadSession.cs", sourceLineNumber: 190);
    }
  }

  public async Task<BackendRequestStatus> sendFeedbackForm(
    Dictionary<string, string> feedbackFormPostParameters)
  {
    UtilityFunctions.MergeLogFiles();
    IPlatformSpecificServiceLocator locator = Mvx.IoCProvider.Resolve<IPlatformSpecificServiceLocator>();
    Dictionary<string, object> postParameters = new Dictionary<string, object>();
    postParameters.Add("subject", (object) feedbackFormPostParameters["subject"]);
    postParameters.Add("message", (object) feedbackFormPostParameters["message"]);
    string fileName = Path.Combine(locator.GetPlatformSpecificService().getFolder(), "iS5LogsMerged.txt");
    FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
    byte[] data = new byte[fs.Length];
    fs.Read(data, 0, data.Length);
    fs.Close();
    BinaryDownloadSession.loggingService.getLogger().LogAppInformation(LoggingContext.LOCAL, "Support iS5Log file data length:" + data.Length.ToString(), memberName: nameof (sendFeedbackForm), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/BinaryDownloadSession.cs", sourceLineNumber: 212);
    postParameters.Add("file", (object) new FormUpload.FileParameter(data, "iS5Log.txt", "text/plain"));
    if (feedbackFormPostParameters.ContainsKey("MainLogFilename"))
    {
      string mainLogFileName = $"{Constants.BridgeLogDirName}/{Constants.bridgeMainLogFileName}";
      if (Directory.Exists(Constants.BridgeLogDirPath) && UtilityFunctions.fileExists(mainLogFileName))
      {
        string bridgeFileName = Constants.bridgeMainLogFileName;
        if (!string.IsNullOrEmpty(feedbackFormPostParameters["sessionLogFilename"]))
        {
          IEnumerable<string> names = (IEnumerable<string>) new string[2]
          {
            bridgeFileName,
            feedbackFormPostParameters["sessionLogFilename"]
          };
          BinaryDownloadSession.loggingService.getLogger().LogAppInformation(LoggingContext.LOCAL, "Zip both files in one", memberName: nameof (sendFeedbackForm), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/BinaryDownloadSession.cs", sourceLineNumber: 225);
          BinaryDownloadSession.CreateZipFile(Constants.bridgeFileName, names);
          bridgeFileName = Constants.bridgeFileName;
          names = (IEnumerable<string>) null;
        }
        string bridgeFilePath = $"{Constants.BridgeLogDirPath}/{bridgeFileName}";
        fs = new FileStream(bridgeFilePath, FileMode.Open, FileAccess.Read);
        byte[] bridgeData = new byte[fs.Length];
        fs.Read(bridgeData, 0, bridgeData.Length);
        fs.Close();
        BinaryDownloadSession.loggingService.getLogger().LogAppInformation(LoggingContext.LOCAL, "Support bridge log file data length:" + bridgeData.Length.ToString(), memberName: nameof (sendFeedbackForm), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/BinaryDownloadSession.cs", sourceLineNumber: 235);
        postParameters.Add("bridgelog", (object) new FormUpload.FileParameter(bridgeData, bridgeFileName, "text/plain"));
        bridgeFileName = (string) null;
        bridgeFilePath = (string) null;
        bridgeData = (byte[]) null;
      }
      mainLogFileName = (string) null;
    }
    IUserSession userSession = Mvx.IoCProvider.Resolve<IUserSession>();
    if (!userSession.IsHostReachable())
      return BackendRequestStatus.RES_NInternet;
    IBackendDetails<HttpWebRequest> bEnd = Mvx.IoCProvider.Resolve<IBackendDetails<HttpWebRequest>>();
    HttpWebRequest httpWebRequest;
    try
    {
      httpWebRequest = bEnd.postFeedbackForm();
    }
    catch (NotSupportedException ex)
    {
      BinaryDownloadSession.loggingService.getLogger().LogAppInformation(LoggingContext.LOCAL, "FeedbackForm NotSupportedException - BackendRequestStatus.RES_NURL", memberName: nameof (sendFeedbackForm), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/BinaryDownloadSession.cs", sourceLineNumber: 252);
      return BackendRequestStatus.RES_NURL;
    }
    catch (UriFormatException ex)
    {
      BinaryDownloadSession.loggingService.getLogger().LogAppInformation(LoggingContext.LOCAL, "FeedbackForm UriFormatException - BackendRequestStatus.RES_NURL", memberName: nameof (sendFeedbackForm), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/BinaryDownloadSession.cs", sourceLineNumber: 257);
      return BackendRequestStatus.RES_NURL;
    }
    string jwtToken = BinaryDownloadSession.secureStorageService.getJWTToken().Result;
    if (!string.IsNullOrWhiteSpace(jwtToken))
      httpWebRequest.Headers.Add("Authorization", "Bearer " + jwtToken);
    BackendRequestStatus backendRequestStatus = await FormUpload.MultipartFormDataPost(httpWebRequest, postParameters);
    return backendRequestStatus;
  }

  public async Task<BackendRequestStatus> downloadBinaryFileAsync(
    BinaryDownloadTask _bdt,
    iService5.Core.Services.User.progressCallback _pcb,
    DownloadProxy proxy)
  {
    this._progressCallback = _pcb;
    IUserAccount user = Mvx.IoCProvider.Resolve<IUserAccount>();
    string encoded = "";
    BackendRequestStatus res = user.getEncoded(out encoded);
    if (res != 0)
      return res;
    IBackendDetails<HttpWebRequest> bEnd = Mvx.IoCProvider.Resolve<IBackendDetails<HttpWebRequest>>();
    HttpWebRequest httpWebRequest;
    try
    {
      httpWebRequest = proxy.FileType != FileType.Ppf ? (proxy.FileType != FileType.BundledPpf ? bEnd.getBinaryDataRequest(proxy.GetFileId()) : bEnd.getBundlePpfDataRequest(proxy.vib, proxy.ki)) : bEnd.getPpfDataRequest(proxy.vib, proxy.ki, proxy.moduleId.ToString(), proxy.version);
    }
    catch (NotSupportedException ex)
    {
      BinaryDownloadSession.loggingService.getLogger().LogAppError(LoggingContext.BACKEND, "Invoked method is not supported", memberName: nameof (downloadBinaryFileAsync), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/BinaryDownloadSession.cs", sourceLineNumber: 294);
      return BackendRequestStatus.RES_NURL;
    }
    catch (UriFormatException ex)
    {
      BinaryDownloadSession.loggingService.getLogger().LogAppError(LoggingContext.BACKEND, "An invalid URI is detected.", memberName: nameof (downloadBinaryFileAsync), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/BinaryDownloadSession.cs", sourceLineNumber: 299);
      return BackendRequestStatus.RES_NURL;
    }
    string jwtToken = BinaryDownloadSession.secureStorageService.getJWTToken().Result;
    if (!string.IsNullOrWhiteSpace(jwtToken))
      httpWebRequest.Headers.Add("Authorization", "Bearer " + jwtToken);
    IBackend<HttpWebRequest> bEndInst = Mvx.IoCProvider.Resolve<IBackend<HttpWebRequest>>();
    BackendRequestStatus backendRequestStatus = await bEndInst.executeBinaryRequest(_bdt, httpWebRequest);
    return backendRequestStatus;
  }

  public string fetchJWTToken()
  {
    string result = BinaryDownloadSession.secureStorageService.getJWTToken().Result;
    if (result == "")
      BinaryDownloadSession.loggingService.getLogger().LogAppFatal(LoggingContext.BACKEND, "JWT Token is empty", memberName: nameof (fetchJWTToken), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/BinaryDownloadSession.cs", sourceLineNumber: 316);
    else
      BinaryDownloadSession.loggingService.getLogger().LogAppInformation(LoggingContext.BACKEND, "JWT Token has value", memberName: nameof (fetchJWTToken), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/BinaryDownloadSession.cs", sourceLineNumber: 320);
    return result;
  }

  internal Task updateMetadataAsync(object value) => throw new NotImplementedException();

  internal Task downloadBinaryFileAsync(object value, DownloadProxy proxy)
  {
    throw new NotImplementedException();
  }
}
