// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.User.IUserSession
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Appliance;
using iService5.Core.Services.Backend;
using iService5.Core.Services.Data;
using iService5.Core.Services.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

#nullable disable
namespace iService5.Core.Services.User;

public interface IUserSession
{
  bool ValidateUserAccess();

  void ResetOnlineSession();

  void TerminateDownloadSession();

  void StartDownloadSession();

  void StartFailedDownloadSession();

  void TerminateFailedDownloadSession();

  void setEnumberSession(string enumber);

  void setZ9kdListCalc();

  List<DownloadProxy> getZ9kdList();

  bool z9kdcalculated();

  string getEnumberSession();

  bool isActive();

  bool isDownloadActive();

  bool isDownloadCanceled();

  void cancelDownload();

  void activate(activationCompletionCallback _cb);

  void deactivate(deactivationCompletionCallback _cb);

  void setProperty(object obj);

  string getProperty(string key);

  Dictionary<string, string> getProperties();

  bool IsHostReachable();

  BackendRequestStatus GetBackendRequestStatus();

  bool isMetadataSessionActive();

  void getMetadataAvailable(metadataUpdateCallback _cb);

  void getMetadata(metadataUpdateCallback _cb, progressCallback _pcb);

  void getBinary(
    DownloadProxy proxy,
    FileType fileType,
    downloadCallback _cb,
    progressCallback _pcb);

  BinaryDownloadTask GetBinaryTask(
    DownloadProxy proxy,
    FileType fileType,
    downloadCallback _cb,
    progressCallback _pcb);

  Task ExecuteBinaryDownloadTask(BinaryDownloadTask bdt);

  void setTracingID(string tid);

  string getTracingID();

  void sendSupportForm(
    Dictionary<string, string> postParameters,
    sendSupportFormCompletionCallback _cb);

  Dictionary<string, string> getFeedbackFormPostParameters();

  void sendPendingSupportForm();

  ServiceError getServiceError();

  void setServiceError(ServiceError serviceError);

  void signalTasks();

  void designalTasks();

  bool getIsUserLoggedIn();

  void setIsUserLoggedIn(bool isUserLoggedIn);

  void SetSenderScreen(string screen);

  string GetSenderScreen();

  void SetURLSchemeEnumber(string eNumber);

  string GetURLSchemeEnumber();

  void SetURLSchemePrepareWorkEnumbers(List<string> prepareWorkeNumberList);

  List<string> GetURLSchemePrepareWorkEnumbers();

  bool ShouldGoToPrepareWork();

  void SetMaterialStatistics(List<MaterialStatistics> statistics);

  List<MaterialStatistics> GetMaterialStatistics();

  void DbDetected(bool _dbdetected);

  void CertificateCanBeUpdated(bool _certupdated);

  bool IsGetValuesOpen();

  void SetIsGetValuesOpen(bool isOpen);

  void signCertificate(string csr, signCertificateCompletionCallback _cb);

  void MetadataStatusSuccess(bool v);

  void CertificateIsValid(bool v);

  void CertificateHasBeenUpdated(bool v);

  bool CheckCertificateValidity();

  void updatePPFRefetchStatus(string eNumber, bool setAsDownloaded);

  List<string> RefetchedPPFENumbers();

  IAppliance GetApplianceSession();
}
