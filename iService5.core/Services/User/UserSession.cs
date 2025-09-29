// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.User.UserSession
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Appliance;
using iService5.Core.Services.Backend;
using iService5.Core.Services.Data;
using iService5.Core.Services.Helpers;
using iService5.Core.Services.Local;
using iService5.Core.Services.Platform;
using iService5.Core.Services.User.ActivityLogging;
using MvvmCross;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
//using Xamarin.Essentials;
//using Xamarin.Forms;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Networking;

#nullable disable
namespace iService5.Core.Services.User;

public class UserSession : IUserSession
{
  private readonly IPlatformSpecificServiceLocator _locator;
  private readonly IBackendUserAuthentication<HttpWebRequest> _backendUserAuthentication;
  private readonly ILocalUserAuthentication _localUserAuthentication;
  private readonly ILoggingService _loggingService;
  public readonly IBinaryDownloadSession<HttpWebRequest> _binaryDownloadSession;
  private readonly IMetadataService _metadataService;
  private readonly IAlertService _alertService;
  private readonly ISecureStorageService _secureStorageService;
  private CancellationTokenSource cts;
  private CancellationToken token;
  private readonly ISigningCertificate<HttpWebRequest> _signingCertificate;
  public static int MaxDownloadRetries;
  private readonly Dictionary<string, string> sessionAttributes = new Dictionary<string, string>();
  internal bool m_Active;
  internal bool isUserLoggedIn;
  private readonly IConnectivityService _connectivityService;
  public readonly IAppliance _appliance;
  private string m_userType;
  internal List<string> urlSchemePrepareWorkeNumbers;
  internal string urlSchemeEnumber;
  internal List<MaterialStatistics> materialStatistics;
  private Dictionary<string, string> feedbackFormPostParameters = new Dictionary<string, string>();
  private iService5.Core.Services.User.sendSupportFormCompletionCallback sendSupportFormSendCallback;
  private readonly Action<object> sendSupportFormTask = (Action<object>) (async obj =>
  {
    BinaryDownloadTask bdt = (BinaryDownloadTask) obj;
    UserSession _this = (UserSession) ((BinaryDownloadTask) obj)._session;
    BackendRequestStatus res = BackendRequestStatus.RES_NInternet;
    IPlatformSpecificServiceLocator locator1 = Mvx.IoCProvider.Resolve<IPlatformSpecificServiceLocator>();
    if (await locator1.GetPlatformSpecificService().IsNetworkAvailable())
    {
      CoreApp.settings.UpdateItem(new Settings("SupportFormSendStatus", "REQUEST_SENT"));
      res = await ((UserSession) bdt._session)._binaryDownloadSession.sendFeedbackForm(_this.getFeedbackFormPostParameters());
      switch (res)
      {
        case BackendRequestStatus.RES_OK:
          _this.sendSupportFormSendCallback(SupportMailSendStatus.SUCCESS);
          bdt = (BinaryDownloadTask) null;
          _this = (UserSession) null;
          locator1 = (IPlatformSpecificServiceLocator) null;
          break;
        case BackendRequestStatus.RES_NInternet:
          _this.sendSupportFormSendCallback(SupportMailSendStatus.INTERNET_ISSUE);
          bdt = (BinaryDownloadTask) null;
          _this = (UserSession) null;
          locator1 = (IPlatformSpecificServiceLocator) null;
          break;
        case BackendRequestStatus.RES_Exception:
          _this.sendSupportFormSendCallback(SupportMailSendStatus.EXCEPTION);
          bdt = (BinaryDownloadTask) null;
          _this = (UserSession) null;
          locator1 = (IPlatformSpecificServiceLocator) null;
          break;
        case BackendRequestStatus.RES_TimeOut:
          _this.sendSupportFormSendCallback(SupportMailSendStatus.TIMEOUT);
          bdt = (BinaryDownloadTask) null;
          _this = (UserSession) null;
          locator1 = (IPlatformSpecificServiceLocator) null;
          break;
        default:
          _this.sendSupportFormSendCallback(SupportMailSendStatus.FAILURE);
          bdt = (BinaryDownloadTask) null;
          _this = (UserSession) null;
          locator1 = (IPlatformSpecificServiceLocator) null;
          break;
      }
    }
    else
    {
      _this.sendSupportFormSendCallback(SupportMailSendStatus.INTERNET_ISSUE);
      bdt = (BinaryDownloadTask) null;
      _this = (UserSession) null;
      locator1 = (IPlatformSpecificServiceLocator) null;
    }
  });
  private string csr = "";
  private signCertificateCompletionCallback signCertificateCallback;
  private readonly Action<object> signCertificateTask = (Action<object>) (async obj =>
  {
    SignCertificateTask signCertTask = (SignCertificateTask) obj;
    UserSession _this = (UserSession) ((SignCertificateTask) obj)._session;
    IPlatformSpecificServiceLocator locator2 = Mvx.IoCProvider.Resolve<IPlatformSpecificServiceLocator>();
    if (await locator2.GetPlatformSpecificService().IsNetworkAvailable())
    {
      BackendRequestStatus res = await ((UserSession) signCertTask._session)._signingCertificate.sendSignCertificate(_this.csr);
      switch (res)
      {
        case BackendRequestStatus.RES_OK:
          _this.signCertificateCallback(SignCertificateStatus.SUCCESS);
          signCertTask = (SignCertificateTask) null;
          _this = (UserSession) null;
          locator2 = (IPlatformSpecificServiceLocator) null;
          break;
        case BackendRequestStatus.RES_NInternet:
          _this.signCertificateCallback(SignCertificateStatus.INTERNET_ISSUE);
          signCertTask = (SignCertificateTask) null;
          _this = (UserSession) null;
          locator2 = (IPlatformSpecificServiceLocator) null;
          break;
        case BackendRequestStatus.RES_Exception:
          _this.signCertificateCallback(SignCertificateStatus.EXCEPTION);
          signCertTask = (SignCertificateTask) null;
          _this = (UserSession) null;
          locator2 = (IPlatformSpecificServiceLocator) null;
          break;
        case BackendRequestStatus.RES_TimeOut:
          _this.signCertificateCallback(SignCertificateStatus.TIMEOUT);
          signCertTask = (SignCertificateTask) null;
          _this = (UserSession) null;
          locator2 = (IPlatformSpecificServiceLocator) null;
          break;
        default:
          _this.signCertificateCallback(SignCertificateStatus.FAILURE);
          signCertTask = (SignCertificateTask) null;
          _this = (UserSession) null;
          locator2 = (IPlatformSpecificServiceLocator) null;
          break;
      }
    }
    else
    {
      _this.signCertificateCallback(SignCertificateStatus.INTERNET_ISSUE);
      signCertTask = (SignCertificateTask) null;
      _this = (UserSession) null;
      locator2 = (IPlatformSpecificServiceLocator) null;
    }
  });
  internal activationCompletionCallback ActivationCallback;
  internal readonly Action<object> authTask;
  private downloadCallback DownloadCallback;
  private metadataUpdateCallback MetadataCallback;
  private readonly Action<object> MetadataChecksumRetrievalTask = (Action<object>) (async obj =>
  {
    BinaryDownloadTask bdt = (BinaryDownloadTask) obj;
    Settings lastUpdate = CoreApp.settings.GetItem("lastUpdate");
    if (lastUpdate == null || string.IsNullOrEmpty(lastUpdate.Value) || lastUpdate.Value.StartsWith("USE_TEXT"))
    {
      ((UserSession) bdt._session)._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "No DB was found", memberName: nameof (MetadataChecksumRetrievalTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/User/UserSession.cs", sourceLineNumber: 546);
      if (lastUpdate == null)
      {
        iService5.Core.Services.Data.SettingsDB settings1 = CoreApp.settings;
        DateTime dateTime = DateTime.Now;
        dateTime = dateTime.AddDays(-1.0);
        Settings settings2 = new Settings("lastUpdate", dateTime.ToString("G", (IFormatProvider) CultureInfo.InvariantCulture));
        settings1.SaveItem(settings2);
      }
    }
    else
      ((UserSession) bdt._session)._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "Last DB Update was on: " + lastUpdate.Value, memberName: nameof (MetadataChecksumRetrievalTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/User/UserSession.cs", sourceLineNumber: 554);
    BackendRequestStatus res = ((UserSession) bdt._session)._binaryDownloadSession.getMetadataChecksum();
    ((UserSession) bdt._session).metadataSessionActive = false;
    switch (res)
    {
      case BackendRequestStatus.RES_OK:
        ((UserSession) bdt._session).sessionError = res;
        if (!((UserSession) bdt._session).sessionAttributes.ContainsKey("checksum"))
        {
          bdt = (BinaryDownloadTask) null;
          lastUpdate = (Settings) null;
          break;
        }
        Settings xsum = CoreApp.settings.GetItem("checksum");
        if (xsum == null || xsum.Value == "")
        {
          string contentLength = ((UserSession) bdt._session).sessionAttributes["contentLength"];
          Settings xcontentLen = CoreApp.settings.GetItem("metadataContentLength");
          if (xcontentLen == null || xcontentLen.Value == "")
            CoreApp.settings.SaveItem(new Settings("metadataContentLength", contentLength));
          else
            CoreApp.settings.UpdateItem(new Settings("metadataContentLength", contentLength));
          bdt._mcb(MetadataStatus.ABSENT);
          contentLength = (string) null;
          xcontentLen = (Settings) null;
        }
        else if (xsum.Value != ((UserSession) bdt._session).sessionAttributes["checksum"])
        {
          ((UserSession) bdt._session)._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "Checksum doesn't matches, proceeding with download", memberName: nameof (MetadataChecksumRetrievalTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/User/UserSession.cs", sourceLineNumber: 582);
          string contentLength = ((UserSession) bdt._session).sessionAttributes["contentLength"];
          Settings xcontentLen = CoreApp.settings.GetItem("metadataContentLength");
          if (xcontentLen == null || xcontentLen.Value == "")
            CoreApp.settings.SaveItem(new Settings("metadataContentLength", contentLength));
          else
            CoreApp.settings.UpdateItem(new Settings("metadataContentLength", contentLength));
          if (((UserSession) bdt._session).dbDetected)
            bdt._mcb(MetadataStatus.OLD);
          else
            bdt._mcb(MetadataStatus.ABSENT);
          contentLength = (string) null;
          xcontentLen = (Settings) null;
        }
        else
        {
          ((UserSession) bdt._session)._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "Checksum matches, proceeding without download", memberName: nameof (MetadataChecksumRetrievalTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/User/UserSession.cs", sourceLineNumber: 600);
          CoreApp.settings.DeleteItem(new Settings("lastCheck", ""));
          DateTime dt;
          DateTime.TryParse(((UserSession) bdt._session).sessionAttributes["lastmodified"], out dt);
          string tmpDate = dt.ToString("G", (IFormatProvider) CultureInfo.InvariantCulture);
          CoreApp.settings.SaveItem(new Settings("lastCheck", tmpDate));
          bdt._mcb(MetadataStatus.UPDATED);
          tmpDate = (string) null;
        }
        xsum = (Settings) null;
        bdt = (BinaryDownloadTask) null;
        lastUpdate = (Settings) null;
        break;
      case BackendRequestStatus.RES_UnAuthorized:
        ((UserSession) bdt._session)._loggingService.getLogger().LogAppDebug(LoggingContext.BACKEND, "Unauthorized request-Login session might have expired", memberName: nameof (MetadataChecksumRetrievalTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/User/UserSession.cs", sourceLineNumber: 613);
        ((UserSession) bdt._session).sessionError = res;
        bdt._mcb(MetadataStatus.UNAUTHORIZED);
        bdt = (BinaryDownloadTask) null;
        lastUpdate = (Settings) null;
        break;
      case BackendRequestStatus.RES_CertificateUnknown:
        ((UserSession) bdt._session)._loggingService.getLogger().LogAppDebug(LoggingContext.BACKEND, "Unauthorized request", memberName: nameof (MetadataChecksumRetrievalTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/User/UserSession.cs", sourceLineNumber: 619);
        ((UserSession) bdt._session).sessionError = res;
        bdt._mcb(MetadataStatus.SERVER_CERTIFICATE_UNKNOWN);
        bdt = (BinaryDownloadTask) null;
        lastUpdate = (Settings) null;
        break;
      case BackendRequestStatus.RES_Exception:
        ((UserSession) bdt._session).sessionError = res;
        MessagingCenter.Send<UserSession>((UserSession) bdt._session, CoreApp.EventsNames.MetadataFailed.ToString());
        ((UserSession) bdt._session).updateLastDateUponMetaDataFailure();
        bdt._mcb(MetadataStatus.EXCEPTION);
        bdt = (BinaryDownloadTask) null;
        lastUpdate = (Settings) null;
        break;
      case BackendRequestStatus.RES_TimeOut:
        ((UserSession) bdt._session).sessionError = res;
        MessagingCenter.Send<UserSession>((UserSession) bdt._session, CoreApp.EventsNames.MetadataFailed.ToString());
        ((UserSession) bdt._session).updateLastDateUponMetaDataFailure();
        bdt._mcb(MetadataStatus.TIMEOUT);
        bdt = (BinaryDownloadTask) null;
        lastUpdate = (Settings) null;
        break;
      default:
        ((UserSession) bdt._session)._loggingService.getLogger().LogAppDebug(LoggingContext.BACKEND, "Metadata download Failed" + res.ToString(), memberName: nameof (MetadataChecksumRetrievalTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/User/UserSession.cs", sourceLineNumber: 639);
        MessagingCenter.Send<UserSession>((UserSession) bdt._session, CoreApp.EventsNames.MetadataFailed.ToString());
        ((UserSession) bdt._session).sessionError = res;
        ((UserSession) bdt._session).updateLastDateUponMetaDataFailure();
        bdt._mcb(MetadataStatus.FAILURE);
        bdt = (BinaryDownloadTask) null;
        lastUpdate = (Settings) null;
        break;
    }
  });
  private readonly Action<object> DownloadRetrievalTask = (Action<object>) (async obj =>
  {
    BinaryDownloadTask bdt = (BinaryDownloadTask) obj;
    DownloadStatus status = DownloadStatus.STARTED;
    try
    {
      ((UserSession) bdt._session).CancelDownload = false;
      status = await UserSession.RetryOnFailure(((UserSession) bdt._session)._binaryDownloadSession.downloadBinaryFileAsync(bdt, bdt._pcb, bdt.proxy), bdt);
      bdt._cb(status);
      bdt = (BinaryDownloadTask) null;
    }
    catch (Exception ex)
    {
      bdt._exCb(ex);
      bdt = (BinaryDownloadTask) null;
    }
  });
  internal readonly Action<object> MetadataRetrievalTask = (Action<object>) (async obj =>
  {
    UserSession _this = (UserSession) ((BinaryDownloadTask) obj)._session;
    BinaryDownloadTask bdt = (BinaryDownloadTask) obj;
    try
    {
      ((UserSession) bdt._session)._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "Set Response File", memberName: nameof (MetadataRetrievalTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/User/UserSession.cs", sourceLineNumber: 815);
      ((UserSession) bdt._session).CancelDownload = false;
      string tmpDownloadDBFilePath = Path.Combine(((UserSession) bdt._session)._locator.GetPlatformSpecificService().getFolder(), Constants.tempMetadataFileName);
      ((UserSession) bdt._session)._binaryDownloadSession.setResponseFile(tmpDownloadDBFilePath);
      BackendRequestStatus res = await ((UserSession) bdt._session)._binaryDownloadSession.updateMetadataAsync(bdt, bdt._pcb);
      ((UserSession) bdt._session).metadataSessionActive = false;
      switch (res)
      {
        case BackendRequestStatus.RES_OK:
          BackendRequestStatus authenticateDBPasswordStatus = await _this._binaryDownloadSession.authenticateDBPassword();
          if (authenticateDBPasswordStatus == BackendRequestStatus.RES_OK)
          {
            ((UserSession) bdt._session)._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "Begin PROCESSING ", memberName: nameof (MetadataRetrievalTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/User/UserSession.cs", sourceLineNumber: 826);
            bdt._mcb(MetadataStatus.PROCESSING);
            ((UserSession) bdt._session)._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "Begin Reset Database", memberName: nameof (MetadataRetrievalTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/User/UserSession.cs", sourceLineNumber: 828);
            bool dbResetSucceeded = await ((UserSession) bdt._session)._metadataService.resetDatabase(((UserSession) bdt._session).getProperty("checksum"));
            ((UserSession) bdt._session)._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "End Reset Database", memberName: nameof (MetadataRetrievalTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/User/UserSession.cs", sourceLineNumber: 830);
            if (dbResetSucceeded)
            {
              ((UserSession) bdt._session)._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "Begin Update Material Statistics", memberName: nameof (MetadataRetrievalTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/User/UserSession.cs", sourceLineNumber: 834);
              ((UserSession) bdt._session)._metadataService.UpdateMaterialStatistics();
              ((UserSession) bdt._session)._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "End Update Material Statistics", memberName: nameof (MetadataRetrievalTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/User/UserSession.cs", sourceLineNumber: 836);
              ((UserSession) bdt._session)._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "Begin Set Material Statistics", memberName: nameof (MetadataRetrievalTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/User/UserSession.cs", sourceLineNumber: 838);
              ((UserSession) bdt._session).SetMaterialStatistics(((UserSession) bdt._session)._metadataService.GetMaterialStatistics());
              ((UserSession) bdt._session)._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "End Set Material Statistics", memberName: nameof (MetadataRetrievalTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/User/UserSession.cs", sourceLineNumber: 840);
              ((UserSession) bdt._session)._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "Begin Get SSH Key", memberName: nameof (MetadataRetrievalTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/User/UserSession.cs", sourceLineNumber: 842);
              Stream stream = UtilityFunctions.GenerateStreamFromString(((UserSession) bdt._session)._metadataService.getSSHKeyValue());
              ((UserSession) bdt._session)._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "End Get SSH Key", memberName: nameof (MetadataRetrievalTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/User/UserSession.cs", sourceLineNumber: 844);
              Mvx.IoCProvider.RegisterSingleton<Is5SshWrapper>(new Is5SshWrapper(BuildProperties.LoginUser, stream, ((UserSession) ((BinaryDownloadTask) obj)._session)._loggingService));
              CoreApp.settings.DeleteItem(new Settings("lastCheck", ""));
              DateTime dt;
              DateTime.TryParse(((UserSession) bdt._session).sessionAttributes["lastmodified"], out dt);
              string tmpDate = dt.ToString("G", (IFormatProvider) CultureInfo.InvariantCulture);
              CoreApp.settings.SaveItem(new Settings("lastCheck", tmpDate));
              CoreApp.settings.DeleteItem(new Settings("lastUpdate", ""));
              CoreApp.settings.SaveItem(new Settings("lastUpdate", DateTime.Now.ToString("G", (IFormatProvider) CultureInfo.InvariantCulture)));
              if (((UserSession) bdt._session).sessionAttributes.ContainsKey("checksum"))
                CoreApp.settings.UpdateItem(new Settings("checksum", ((UserSession) bdt._session).sessionAttributes["checksum"]));
              bdt._mcb(MetadataStatus.SUCCESS);
              ((UserSession) bdt._session)._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "End Processing", memberName: nameof (MetadataRetrievalTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/User/UserSession.cs", sourceLineNumber: 857);
              if (System.IO.File.Exists(tmpDownloadDBFilePath))
                System.IO.File.Delete(tmpDownloadDBFilePath);
              stream = (Stream) null;
              tmpDate = (string) null;
              break;
            }
            MessagingCenter.Send<UserSession>((UserSession) bdt._session, CoreApp.EventsNames.MetadataFailed.ToString());
            ((UserSession) bdt._session).sessionError = res;
            CoreApp.settings.DeleteItem(new Settings("lastUpdate", ""));
            CoreApp.settings.SaveItem(new Settings("lastUpdate", "USE_TEXT:ST_PAGE_FAILED_DB"));
            bdt._mcb(MetadataStatus.FAILURE);
            break;
          }
          ((UserSession) bdt._session)._loggingService.getLogger().LogAppError(LoggingContext.USER, "DB password authentication error", memberName: nameof (MetadataRetrievalTask), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/User/UserSession.cs", sourceLineNumber: 874);
          bdt._mcb(MetadataStatus.EXCEPTION);
          break;
        case BackendRequestStatus.RES_UnAuthorized:
          ((UserSession) bdt._session).sessionError = res;
          bdt._mcb(MetadataStatus.UNAUTHORIZED);
          break;
        case BackendRequestStatus.RES_CertificateUnknown:
          ((UserSession) bdt._session).sessionError = res;
          bdt._mcb(MetadataStatus.SERVER_CERTIFICATE_UNKNOWN);
          break;
        case BackendRequestStatus.RES_Exception:
          ((UserSession) bdt._session).sessionError = res;
          bdt._mcb(MetadataStatus.EXCEPTION);
          break;
        case BackendRequestStatus.RES_TimeOut:
          ((UserSession) bdt._session).sessionError = res;
          bdt._mcb(MetadataStatus.TIMEOUT);
          break;
        default:
          MessagingCenter.Send<UserSession>((UserSession) bdt._session, CoreApp.EventsNames.MetadataFailed.ToString());
          ((UserSession) bdt._session).sessionError = res;
          CoreApp.settings.DeleteItem(new Settings("lastUpdate", ""));
          CoreApp.settings.SaveItem(new Settings("lastUpdate", "USE_TEXT:ST_PAGE_FAILED_DB"));
          bdt._mcb(MetadataStatus.FAILURE);
          break;
      }
      tmpDownloadDBFilePath = (string) null;
      _this = (UserSession) null;
      bdt = (BinaryDownloadTask) null;
    }
    catch (Exception ex)
    {
      if (!ex.Message.ToLowerInvariant().Contains("cancel"))
        MessagingCenter.Send<UserSession>((UserSession) bdt._session, CoreApp.EventsNames.MetadataFailed.ToString());
      bdt._exCb(ex);
      _this = (UserSession) null;
      bdt = (BinaryDownloadTask) null;
    }
  });
  internal readonly TimeSpan SessionDuration;
  internal readonly TimeSpan SessionNotificationDuration;
  internal Action OnSessionExpiring;
  private string _tracingID = "";
  private bool _z9kdcalculated = false;

  public UserSession(
    IBackendUserAuthentication<HttpWebRequest> _bauth,
    ILocalUserAuthentication _lauth,
    ILoggingService _logService,
    IBinaryDownloadSession<HttpWebRequest> binaryDownloadSession,
    IMetadataService metadataService,
    IPlatformSpecificServiceLocator locator,
    IConnectivityService connectivityService,
    IAppliance appliance,
    IAlertService alertService,
    ISecureStorageService secureStorageService,
    ISigningCertificate<HttpWebRequest> signingCertificate)
  {
    this._alertService = alertService;
    this._locator = locator;
    this._backendUserAuthentication = _bauth;
    this._localUserAuthentication = _lauth;
    this._loggingService = _logService;
    this._binaryDownloadSession = binaryDownloadSession;
    this._metadataService = metadataService;
    this.m_Active = false;
    this.isUserLoggedIn = false;
    this._connectivityService = connectivityService;
    this._appliance = appliance;
    this._secureStorageService = secureStorageService;
    this._signingCertificate = signingCertificate;
    this.SessionDuration = TimeSpan.FromDays((double) int.Parse(BuildProperties.maxOfflineDays));
    this.SessionNotificationDuration = TimeSpan.FromDays((double) int.Parse(BuildProperties.maxOfflineDaysWithNotification));
    this.cts = new CancellationTokenSource();
    this.token = this.cts.Token;
    this.authTask = (Action<object>) (async o => await this.authTaskImpl(o));
    this._senderScreen = "";
    this.urlSchemePrepareWorkeNumbers = new List<string>();
    this.urlSchemeEnumber = "";
    this.materialStatistics = new List<MaterialStatistics>();
    this.metadataUpdated = false;
    this.certificateCanBeUpdated = false;
    this.certificateHasBeenUpdated = false;
    this.certificateIsValid = true;
    UserSession.MaxDownloadRetries = int.Parse(BuildProperties.maxDownloadRetries);
  }

  public void signalTasks()
  {
    if (this.cts.IsCancellationRequested)
      return;
    this.cts.Cancel(true);
    this.cts.Dispose();
  }

  public void designalTasks()
  {
    this.cts = new CancellationTokenSource();
    this.token = this.cts.Token;
  }

  public BackendRequestStatus sessionError { get; private set; }

  public ServiceError serviceError { get; set; }

  public bool ppu { get; set; }

  public IAppliance GetApplianceSession() => this._appliance;

  public string userType => this.m_userType;

  public HttpStatusCode retCode { get; internal set; }

  public bool metadataSessionActive { get; internal set; }

  public bool dbDetected { get; internal set; }

  internal bool isGetValuesOpen { get; set; }

  public BackendRequestStatus GetBackendRequestStatus() => this.sessionError;

  public ServiceError getServiceError() => this.serviceError;

  public void setServiceError(ServiceError serviceError)
  {
    if (serviceError.isWebException)
      this.serviceError = new ServiceError(serviceError.errorKey, serviceError.errorType, serviceError.errorMessage, serviceError.isWebException);
    else
      this.serviceError = new ServiceError(serviceError.errorType, serviceError.errorMessage, serviceError.isWebException);
  }

  public bool isActive() => this.m_Active;

  public bool getIsUserLoggedIn() => this.isUserLoggedIn;

  public void setIsUserLoggedIn(bool isUserLoggedIn) => this.isUserLoggedIn = isUserLoggedIn;

  public Dictionary<string, string> getFeedbackFormPostParameters()
  {
    return this.feedbackFormPostParameters;
  }

  public void sendPendingSupportForm()
  {
    string str = CoreApp.settings.GetItem("SupportFormSendStatus").Value;
    this._loggingService.getLogger().LogAppInformation(LoggingContext.USER, "Trying to send pending support form.Current support form send status is " + str, memberName: nameof (sendPendingSupportForm), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/User/UserSession.cs", sourceLineNumber: 171);
    this.sendSupportForm(UtilityFunctions.GetFeedbackFormPostParameters(), new iService5.Core.Services.User.sendSupportFormCompletionCallback(this.sendSupportFormCompletionCallback));
  }

  public void sendSupportFormCompletionCallback(SupportMailSendStatus res)
  {
    string str = "NOT_SENT";
    switch (res)
    {
      case SupportMailSendStatus.SUCCESS:
        str = "SENT";
        this._loggingService.getLogger().LogAppInformation(LoggingContext.USER, "Successful in sending support form", memberName: nameof (sendSupportFormCompletionCallback), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/User/UserSession.cs", sourceLineNumber: 180);
        MessagingCenter.Send<UserSession>(this, CoreApp.EventsNames.FeedBackFormPending.ToString());
        break;
      case SupportMailSendStatus.INTERNET_ISSUE:
        this._loggingService.getLogger().LogAppInformation(LoggingContext.USER, "There occured a problem while sending the feedback form due to internet connectivity issues", memberName: nameof (sendSupportFormCompletionCallback), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/User/UserSession.cs", sourceLineNumber: 185);
        break;
      default:
        this._loggingService.getLogger().LogAppInformation(LoggingContext.USER, "There occured a problem while sending the feedback form", memberName: nameof (sendSupportFormCompletionCallback), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/User/UserSession.cs", sourceLineNumber: 189);
        break;
    }
    CoreApp.settings.UpdateItem(new Settings("SupportFormSendStatus", str));
  }

  public void sendSupportForm(
    Dictionary<string, string> postParameters,
    iService5.Core.Services.User.sendSupportFormCompletionCallback _cb)
  {
    this.feedbackFormPostParameters = postParameters;
    this.sendSupportFormSendCallback = _cb;
    Task.Factory.StartNew(this.sendSupportFormTask, (object) new BinaryDownloadTask()
    {
      _exCb = new exceptionCallback(this.OnSendSupportFormException),
      _session = (object) this
    });
  }

  public void signCertificate(string csrToPost, signCertificateCompletionCallback _cb)
  {
    this.csr = csrToPost;
    this.signCertificateCallback = _cb;
    Task.Factory.StartNew(this.signCertificateTask, (object) new SignCertificateTask()
    {
      _exCb = new exceptionCallback(this.OnSignCertificateException),
      _session = (object) this
    });
  }

  public void activate(activationCompletionCallback _cb)
  {
    this.ActivationCallback = _cb;
    Task.Factory.StartNew(this.authTask, (object) new BinaryDownloadTask()
    {
      _exCb = new exceptionCallback(this.OnActivationException),
      _session = (object) this
    });
  }

  internal async Task authTaskImpl(object obj)
  {
    try
    {
      UserSession _this = (UserSession) ((BinaryDownloadTask) obj)._session;
      BackendRequestStatus res = BackendRequestStatus.RES_NInternet;
      res = await _this._backendUserAuthentication.authenticateUserAsync();
      switch (res)
      {
        case BackendRequestStatus.RES_OK:
          _this.sessionError = res;
          _this.m_Active = true;
          _this.setIsUserLoggedIn(true);
          await _this.ActivationCallback(SessionActivation.SUCCESS);
          return;
        case BackendRequestStatus.RES_NInternet:
          res = _this._localUserAuthentication.authenticateUser(this._secureStorageService);
          if (res == BackendRequestStatus.RES_OK)
          {
            _this.sessionError = res;
            _this.m_Active = true;
            _this.setIsUserLoggedIn(true);
            await _this.ActivationCallback(SessionActivation.SUCCESS);
            break;
          }
          _this.sessionError = res;
          _this.setIsUserLoggedIn(false);
          await _this.ActivationCallback(SessionActivation.FAILURE);
          _this.m_Active = false;
          break;
        default:
          _this.sessionError = res;
          await _this.ActivationCallback(SessionActivation.FAILURE);
          _this.m_Active = false;
          _this.setIsUserLoggedIn(false);
          break;
      }
      _this = (UserSession) null;
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppFatal(LoggingContext.USER, ex.Message, memberName: nameof (authTaskImpl), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/User/UserSession.cs", sourceLineNumber: 339);
    }
  }

  internal void OnActivationException(Exception ex)
  {
    this._loggingService.getLogger().LogAppFatal(LoggingContext.USER, ex.Message, ex, nameof (OnActivationException), "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/User/UserSession.cs", 344);
    Task task = this.ActivationCallback(SessionActivation.FAILURE);
  }

  internal void OnSendSupportFormException(Exception ex)
  {
    this._loggingService.getLogger().LogAppFatal(LoggingContext.USER, ex.Message, ex, nameof (OnSendSupportFormException), "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/User/UserSession.cs", 349);
    this.sendSupportFormSendCallback(SupportMailSendStatus.FAILURE);
  }

  internal void OnSignCertificateException(Exception ex)
  {
    this._loggingService.getLogger().LogAppFatal(LoggingContext.USER, ex.Message, ex, nameof (OnSignCertificateException), "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/User/UserSession.cs", 354);
    this.signCertificateCallback(SignCertificateStatus.FAILURE);
  }

  public void deactivate(deactivationCompletionCallback _cb) => this.m_Active = false;

  public void setProperty(object obj)
  {
    switch (obj)
    {
      case string response:
        this.ProcessResponse(response);
        break;
      case BackendRequestStatus backendRequestStatus:
        this.sessionError = backendRequestStatus;
        if (this.sessionAttributes.ContainsKey("sessionError"))
        {
          this.sessionAttributes["sessionError"] = backendRequestStatus.ToString();
          break;
        }
        this.sessionAttributes.Add("sessionError", backendRequestStatus.ToString());
        break;
      case HttpStatusCode httpStatusCode:
        if (this.sessionAttributes.ContainsKey("retCode"))
        {
          this.sessionAttributes["retCode"] = httpStatusCode.ToString();
          break;
        }
        this.sessionAttributes.Add("retCode", httpStatusCode.ToString());
        break;
      case WebHeaderCollection headerCollection:
        foreach (string allKey in headerCollection.AllKeys)
        {
          if (allKey == "BSH_PAYPERUSE")
            this.ppu = headerCollection[allKey].ToString().Equals("true", StringComparison.InvariantCultureIgnoreCase);
          if (allKey == "BSH_USERTYPE")
            this.m_userType = headerCollection[allKey];
          if (this.sessionAttributes.ContainsKey(allKey))
            this.sessionAttributes[allKey] = headerCollection[allKey];
          else
            this.sessionAttributes.Add(allKey, headerCollection[allKey]);
        }
        break;
      case HttpResponseHeaders httpResponseHeaders:
        IEnumerator<KeyValuePair<string, IEnumerable<string>>> enumerator1 = httpResponseHeaders.GetEnumerator();
        while (enumerator1.MoveNext())
        {
          KeyValuePair<string, IEnumerable<string>> current = enumerator1.Current;
          IEnumerator<string> enumerator2 = current.Value.GetEnumerator();
          if (enumerator2.MoveNext())
          {
            current = enumerator1.Current;
            if (current.Key == "BSH_PAYPERUSE")
              this.ppu = enumerator2.Current.ToString().Equals("true", StringComparison.InvariantCultureIgnoreCase);
            current = enumerator1.Current;
            if (current.Key == "BSH_USERTYPE")
              this.m_userType = enumerator2.Current.ToString();
            Dictionary<string, string> sessionAttributes1 = this.sessionAttributes;
            current = enumerator1.Current;
            string key1 = current.Key;
            if (sessionAttributes1.ContainsKey(key1))
            {
              Dictionary<string, string> sessionAttributes2 = this.sessionAttributes;
              current = enumerator1.Current;
              string key2 = current.Key;
              string str = enumerator2.Current.ToString();
              sessionAttributes2[key2] = str;
            }
            else
            {
              Dictionary<string, string> sessionAttributes3 = this.sessionAttributes;
              current = enumerator1.Current;
              string key3 = current.Key;
              string str = enumerator2.Current.ToString();
              sessionAttributes3.Add(key3, str);
            }
          }
        }
        break;
    }
  }

    public bool IsHostReachable()
    {
        this._loggingService.getLogger().LogAppDebug(LoggingContext.BACKEND, "Checking for connectivity ", memberName: nameof(IsHostReachable), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/User/UserSession.cs", sourceLineNumber: 430);
        return this._connectivityService.NetworkAccess == Microsoft.Maui.Networking.NetworkAccess.Internet && !this._appliance.isConnectedToApplianceWifi() && !this._appliance.isConnectedToBridgeWifi();
    }


    private void ProcessResponse(string response)
  {
    try
    {
      Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(response);
      foreach (string key in dictionary.Keys)
      {
        if (this.sessionAttributes.ContainsKey(key))
          this.sessionAttributes[key] = dictionary[key];
        else
          this.sessionAttributes.Add(key, dictionary[key]);
      }
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppError(LoggingContext.METADATA, "Response Body Error " + ex.Message, memberName: nameof (ProcessResponse), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/User/UserSession.cs", sourceLineNumber: 457);
    }
  }

  public string getProperty(string key)
  {
    return this.sessionAttributes.ContainsKey(key) ? this.sessionAttributes[key] : (string) null;
  }

  public bool isMetadataSessionActive() => this.metadataSessionActive;

  public void CertificateCanBeUpdated(bool _certificateCanBeUpdated)
  {
    this.certificateCanBeUpdated = _certificateCanBeUpdated;
  }

  public void MetadataStatusSuccess(bool v) => this.metadataUpdated = v;

  public void DbDetected(bool _dbdetected) => this.dbDetected = _dbdetected;

  public bool downloadSessionActive { get; private set; }

  public bool certificateCanBeUpdated { get; set; }

  public bool metadataUpdated { get; set; }

  public bool isDownloadActive() => this.downloadSessionActive;

  public void TerminateDownloadSession() => this.downloadSessionActive = false;

  public void StartDownloadSession() => this.downloadSessionActive = true;

  public void StartFailedDownloadSession() => this.downloadSessionActive = true;

  public void TerminateFailedDownloadSession() => this.downloadSessionActive = true;

  public string _enumber { get; private set; }

  public List<DownloadProxy> zk9dlist { get; private set; }

  public string _senderScreen { get; private set; }

  public void getMetadataAvailable(metadataUpdateCallback _cb)
  {
    this.metadataSessionActive = true;
    this.MetadataCallback = _cb;
    Task.Factory.StartNew(this.MetadataChecksumRetrievalTask, (object) new BinaryDownloadTask()
    {
      _mcb = _cb,
      _exCb = new exceptionCallback(this.OnChecksumException),
      _session = (object) this
    });
  }

  private void updateLastDateUponMetaDataFailure()
  {
    CoreApp.settings.DeleteItem(new Settings("lastUpdate", ""));
    CoreApp.settings.SaveItem(new Settings("lastUpdate", "USE_TEXT:ST_PAGE_FAILED_XSUM"));
  }

  public DateTime? ExtractDateTimeFromSettings(string _dt)
  {
    DateTime? timeFromSettings = new DateTime?();
    CultureInfo invariantCulture = CultureInfo.InvariantCulture;
    string str = _dt;
    try
    {
      timeFromSettings = new DateTime?(Convert.ToDateTime(str, (IFormatProvider) invariantCulture));
    }
    catch (FormatException ex)
    {
      this._loggingService.getLogger().LogAppError(LoggingContext.METADATA, $"{str} is not in the correct format :{ex.Message}", memberName: nameof (ExtractDateTimeFromSettings), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/User/UserSession.cs", sourceLineNumber: 664);
      timeFromSettings = new DateTime?(Convert.ToDateTime(str, (IFormatProvider) invariantCulture));
    }
    return timeFromSettings;
  }

  private void OnChecksumException(Exception ex)
  {
    this.metadataSessionActive = false;
    this._loggingService.getLogger().LogAppError(LoggingContext.METADATA, "Retrieval Failure" + ex.Message, memberName: nameof (OnChecksumException), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/User/UserSession.cs", sourceLineNumber: 673);
    this.MetadataCallback(MetadataStatus.FAILURE);
  }

  private void OnDownloadException(Exception ex)
  {
    try
    {
      this._loggingService.getLogger().LogAppError(LoggingContext.BINARY, "Binaries download exception: " + ex.Message, memberName: nameof (OnDownloadException), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/User/UserSession.cs", sourceLineNumber: 680);
      this.DownloadCallback(DownloadStatus.EXCEPTION);
      this._loggingService.getLogger().LogAppInformation(LoggingContext.BINARY, "After calling DownloadCallback in caller's file", memberName: nameof (OnDownloadException), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/User/UserSession.cs", sourceLineNumber: 682);
    }
    catch (Exception ex1)
    {
      this._loggingService.getLogger().LogAppError(LoggingContext.BINARY, "Exception: " + ex1.Message, memberName: nameof (OnDownloadException), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/User/UserSession.cs", sourceLineNumber: 686);
    }
  }

  public void getBinary(
    DownloadProxy proxy,
    FileType fileType,
    downloadCallback _cb,
    progressCallback _pcb)
  {
    this.DownloadCallback = _cb;
    BinaryDownloadTask state = new BinaryDownloadTask();
    Exception exception = new Exception();
    proxy.FileType = fileType;
    state._cb = _cb;
    state._exCb = new exceptionCallback(this.OnDownloadException);
    state._pcb = _pcb;
    state._session = (object) this;
    state.proxy = proxy;
    this.designalTasks();
    Task.Factory.StartNew(this.DownloadRetrievalTask, (object) state, this.token);
  }

  public BinaryDownloadTask GetBinaryTask(
    DownloadProxy proxy,
    FileType fileType,
    downloadCallback _cb,
    progressCallback _pcb)
  {
    this.DownloadCallback = _cb;
    BinaryDownloadTask binaryTask = new BinaryDownloadTask();
    Exception exception = new Exception();
    proxy.FileType = fileType;
    binaryTask._cb = _cb;
    binaryTask._exCb = new exceptionCallback(this.OnDownloadException);
    binaryTask._pcb = _pcb;
    binaryTask._session = (object) this;
    binaryTask.proxy = proxy;
    this.designalTasks();
    return binaryTask;
  }

  public async Task ExecuteBinaryDownloadTask(BinaryDownloadTask bdt)
  {
    DownloadStatus status = DownloadStatus.STARTED;
    try
    {
      ((UserSession) bdt._session).CancelDownload = false;
      status = await UserSession.RetryOnFailure(((UserSession) bdt._session)._binaryDownloadSession.downloadBinaryFileAsync(bdt, bdt._pcb, bdt.proxy), bdt);
      bdt._cb(status);
    }
    catch (Exception ex)
    {
      bdt._exCb(ex);
    }
  }

  internal static async Task<DownloadStatus> RetryOnFailure(
    Task<BackendRequestStatus> downloadTask,
    BinaryDownloadTask bdt)
  {
    string fileId = bdt.proxy.GetFileId();
    string fileName = bdt.proxy.FileType != FileType.BundledPpf ? bdt.proxy.ToFileNameNewFdsExtension(bdt.proxy.fileExtension) : bdt.proxy.GetEnumber();
    for (int triesCount = 1; triesCount <= FeatureConfiguration.MaxBinaryDownloadRetries + 1; ++triesCount)
    {
      BackendRequestStatus res = await downloadTask;
      if (res == BackendRequestStatus.RES_OK)
        return DownloadStatus.COMPLETED;
      ((UserSession) bdt._session).sessionError = res;
      if (res == BackendRequestStatus.RES_UnAuthorized)
        return DownloadStatus.UNAUTHORIZED;
      if (res == BackendRequestStatus.RES_CertificateUnknown)
        return DownloadStatus.SERVER_CERTIFICATE_UNKNOWN;
      if (res == BackendRequestStatus.RES_Exception)
        return DownloadStatus.EXCEPTION;
      if (res == BackendRequestStatus.RES_TimeOut)
        return DownloadStatus.TIMEOUT;
      if (res == BackendRequestStatus.RES_NotFound)
        return DownloadStatus.FILE_NOT_FOUND;
      ((UserSession) bdt._session)._loggingService.getLogger().LogAppInformation(LoggingContext.BINARY, $"Failed to download file {fileName} with id {fileId} at attempt {triesCount}", memberName: nameof (RetryOnFailure), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/User/UserSession.cs", sourceLineNumber: 792);
    }
    return DownloadStatus.FAILED;
  }

  public void getMetadata(metadataUpdateCallback _cb, progressCallback _pcb)
  {
    this.metadataSessionActive = true;
    this.MetadataCallback = _cb;
    Task.Factory.StartNew(this.MetadataRetrievalTask, (object) new BinaryDownloadTask()
    {
      _mcb = _cb,
      _exCb = new exceptionCallback(this.OnChecksumException),
      _pcb = _pcb,
      _session = (object) this
    });
  }

  public bool ValidateUserAccess()
  {
    DateTime now = DateTime.Now;
    bool shouldLogin = false;
    try
    {
      Settings settings = CoreApp.settings.GetItem("SinceAppUsedTimestamp");
      if (settings == null || string.IsNullOrEmpty(settings.Value))
      {
        this._loggingService.getLogger().LogAppInformation(LoggingContext.USER, "lastUseSettings is null or empty", memberName: nameof (ValidateUserAccess), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/User/UserSession.cs", sourceLineNumber: 936);
        return false;
      }
      string str1 = settings.Value;
      this._loggingService.getLogger().LogAppInformation(LoggingContext.USER, "Last used: " + str1, memberName: nameof (ValidateUserAccess), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/User/UserSession.cs", sourceLineNumber: 940);
      DateTime dateTime1 = Convert.ToDateTime(str1);
      string str2 = CoreApp.settings.GetItem("JwtTokenTimestamp").Value;
      this._loggingService.getLogger().LogAppInformation(LoggingContext.USER, "jwtTokenTimestamp: " + str2, memberName: nameof (ValidateUserAccess), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/User/UserSession.cs", sourceLineNumber: 943);
      DateTime dateTime2 = Convert.ToDateTime(str2);
      TimeSpan timeSpan = now - dateTime2;
      bool flag = timeSpan.Days >= 1 && dateTime2.Year != 1970;
      if (now < dateTime1 || now < dateTime2)
      {
        this._loggingService.getLogger().LogAppError(LoggingContext.USER, "Backdating has been recorded", memberName: nameof (ValidateUserAccess), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/User/UserSession.cs", sourceLineNumber: 948);
        shouldLogin = true;
      }
      else
      {
        int num1;
        if (flag)
        {
          timeSpan = now - dateTime1;
          num1 = timeSpan.Days >= 1 ? 1 : 0;
        }
        else
          num1 = 0;
        if (num1 != 0)
        {
          int num2;
          CoreApp.settings.UpdateItem(new Settings("OfflineDaysCount", (num2 = int.Parse(CoreApp.settings.GetItem("OfflineDaysCount").Value) + 1).ToString()));
          CoreApp.settings.UpdateItem(new Settings("SinceAppUsedTimestamp", now.ToString()));
          int num3 = num2;
          timeSpan = this.SessionDuration;
          int days1 = timeSpan.Days;
          if (num3 > days1)
          {
            timeSpan = this.SessionDuration;
            int days2 = timeSpan.Days;
            timeSpan = this.SessionNotificationDuration;
            int days3 = timeSpan.Days;
            int notificationDays = days2 + days3 - num2 + 1;
            this.OnSessionExpiring = notificationDays <= 0 ? (Action) (() =>
            {
              this.DeleteLocalData();
              shouldLogin = true;
            }) : (Action) (async () =>
            {
              this._loggingService.getLogger().LogAppInformation(LoggingContext.USERSESSION, "Service alert popup appeared with message: " + this._alertService.GetEnValue("OFFLINE_SESSION_WARNING").Replace("_PLACEHOLDER_", notificationDays.ToString()), memberName: nameof (ValidateUserAccess), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/User/UserSession.cs", sourceLineNumber: 965);
              await Application.Current.MainPage.DisplayAlert(AppResource.WARNING_TEXT, AppResource.OFFLINE_SESSION_WARNING.Replace("_PLACEHOLDER_", notificationDays.ToString()), AppResource.WARNING_OK);
            });
            Action onSessionExpiring = this.OnSessionExpiring;
            if (onSessionExpiring != null)
              onSessionExpiring();
          }
        }
      }
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppInformation(LoggingContext.USERSESSION, ex.Message, memberName: nameof (ValidateUserAccess), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/User/UserSession.cs", sourceLineNumber: 983);
    }
    return shouldLogin;
  }

  public void ResetOnlineSession()
  {
    try
    {
      CoreApp.settings.UpdateItem(new Settings("OfflineDaysCount", 0.ToString()));
      CoreApp.settings.UpdateItem(new Settings("SinceAppUsedTimestamp", DateTime.Now.ToString()));
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppInformation(LoggingContext.USERSESSION, ex.Message, memberName: nameof (ResetOnlineSession), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/User/UserSession.cs", sourceLineNumber: 999);
    }
  }

  private void DeleteLocalData()
  {
    string folder = this._locator.GetPlatformSpecificService().getFolder();
    try
    {
      this._loggingService.getLogger().LogAppInformation(LoggingContext.USER, "User exceeded maximum days of offline use.\n Trying to remove local data.", memberName: nameof (DeleteLocalData), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/User/UserSession.cs", sourceLineNumber: 1007);
      if (Directory.EnumerateFileSystemEntries(folder).Any<string>())
      {
        foreach (string path in Directory.EnumerateFiles(folder).Except<string>(Directory.EnumerateFiles(folder, "*txt")))
          System.IO.File.Delete(path);
        this._loggingService.getLogger().LogAppInformation(LoggingContext.USER, "Removal of local data has succeeded.", memberName: nameof (DeleteLocalData), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/User/UserSession.cs", sourceLineNumber: 1014);
      }
      else
        this._loggingService.getLogger().LogAppInformation(LoggingContext.USER, "No local data were found", memberName: nameof (DeleteLocalData), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/User/UserSession.cs", sourceLineNumber: 1017);
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppInformation(LoggingContext.USER, "Exception occurred while deleting local data ", ex, nameof (DeleteLocalData), "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/User/UserSession.cs", 1021);
    }
  }

  public Dictionary<string, string> getProperties() => this.sessionAttributes;

  public void setEnumberSession(string enumber)
  {
    this._tracingID = "";
    this.setProperty((object) "{\"reboot\":\"\"}");
    this._enumber = enumber;
  }

  public void setZ9kdListCalc()
  {
    this.zk9dlist = this._metadataService.getFilesForEnumber("Z9KDKD0Z01(00)").Where<enumber_modules>((Func<enumber_modules, bool>) (x => !x.IsDownloaded)).Select<enumber_modules, DownloadProxy>((Func<enumber_modules, DownloadProxy>) (x => new DownloadProxy()
    {
      EnumberModule = x,
      Document = x.Document,
      vib = "Z9KDKD0Z01(00)",
      ki = ""
    })).ToList<DownloadProxy>();
    this._z9kdcalculated = true;
  }

  public List<DownloadProxy> getZ9kdList() => this.zk9dlist;

  public bool z9kdcalculated() => this._z9kdcalculated;

  public string getEnumberSession() => this._enumber;

  public bool CancelDownload { get; private set; }

  public bool certificateHasBeenUpdated { get; internal set; }

  public bool certificateIsValid { get; internal set; }

  public void cancelDownload()
  {
    this._loggingService.getLogger().LogAppInformation(LoggingContext.BACKEND, "Downloading session completed or canceled", memberName: nameof (cancelDownload), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/User/UserSession.cs", sourceLineNumber: 1070);
    this.CancelDownload = true;
  }

  public bool isDownloadCanceled() => this.CancelDownload;

  public void setTracingID(string tid) => this._tracingID = tid;

  public string getTracingID() => this._tracingID;

  public void setSupportFormParameters(Dictionary<string, string> postParameters)
  {
    throw new NotImplementedException();
  }

  public void SetSenderScreen(string screen) => this._senderScreen = screen;

  public string GetSenderScreen() => this._senderScreen;

  public void SetURLSchemePrepareWorkEnumbers(List<string> prepareWorkeNumberList)
  {
    this.urlSchemePrepareWorkeNumbers = prepareWorkeNumberList;
  }

  public List<string> GetURLSchemePrepareWorkEnumbers() => this.urlSchemePrepareWorkeNumbers;

  public bool ShouldGoToPrepareWork()
  {
    Settings settings = CoreApp.settings.GetItem("SelectedURLScheme");
    bool prepareWork = false;
    if (settings != null && settings.Value != "" && this.urlSchemePrepareWorkeNumbers != null && settings.Value == "prepareWork")
      prepareWork = true;
    return prepareWork;
  }

  public void SetURLSchemeEnumber(string eNumber) => this.urlSchemeEnumber = eNumber;

  public string GetURLSchemeEnumber() => this.urlSchemeEnumber;

    //  public void SetMaterialStatistics(List<MaterialStatistics> statistics)
    //  {
    //    this.materialStatistics = statistics;
    //  }

    public void SetMaterialStatistics(List<MaterialStatistics> statistics)
    {
        // --- TYMCZASOWE OBEJŚCIE ---
        // Logujemy informację, że krok został pominięty i nic nie robimy.
        // Oryginalnie: this.materialStatistics = statistics;

        var logger = Mvx.IoCProvider.Resolve<ILoggingService>().getLogger();
        logger.LogAppWarning(LoggingContext.METADATA, "Pominięto krok zapisu statystyk materiałów (SetMaterialStatistics).");

        // Nie przypisujemy nowej listy, aby uniknąć problemów.
        return;
    }



    public List<MaterialStatistics> GetMaterialStatistics() => this.materialStatistics;

  public bool IsGetValuesOpen() => this.isGetValuesOpen;

  public void SetIsGetValuesOpen(bool isOpen) => this.isGetValuesOpen = isOpen;

  public void CertificateIsValid(bool v) => this.certificateIsValid = v;

  public void CertificateHasBeenUpdated(bool v) => this.certificateHasBeenUpdated = v;

  public bool CheckCertificateValidity() => this.certificateIsValid;

  public void updatePPFRefetchStatus(string eNumber, bool setAsDownloaded)
  {
    this.materialStatistics.Where<MaterialStatistics>((Func<MaterialStatistics, bool>) (m => m.material.Equals(eNumber))).ToList<MaterialStatistics>()[0].PPFRefetchStatus = setAsDownloaded ? "TRUE" : "FALSE";
  }

  public List<string> RefetchedPPFENumbers()
  {
    return this.materialStatistics.Where<MaterialStatistics>((Func<MaterialStatistics, bool>) (m => m.PPFRefetchStatus == "TRUE")).Select<MaterialStatistics, string>((Func<MaterialStatistics, string>) (m => m.material)).ToList<string>();
  }
}
