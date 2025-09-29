// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Helpers.Constants
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using System.Collections.Generic;

#nullable disable
namespace iService5.Core.Services.Helpers;

public static class Constants
{
  public static string excludeDocumentTypeForNonSMM = "('14115','14123','14118','14126','14124','00425','14102','14692','14148','14149','14113')";
  public static string includeSmmDocumentType = "14147";
  public static int[] nodesToDownload = new int[4]
  {
    0,
    (int) ushort.MaxValue,
    63743,
    63489
  };
  public static int[] nodesToDownloadToElp = new int[2]
  {
    0,
    (int) ushort.MaxValue
  };
  public static int[] nodesToDownloadToSyMaNa = new int[2]
  {
    63743,
    63489
  };
  public static string nodesToDownloadConcat = $"({string.Join<int>(", ", (IEnumerable<int>) Constants.nodesToDownload)})";
  public static string nodesToDownloadToElpConcat = $"({string.Join<int>(", ", (IEnumerable<int>) Constants.nodesToDownloadToElp)})";
  public static string nodesToDownloadToSyMaNaConcat = $"({string.Join<int>(", ", (IEnumerable<int>) Constants.nodesToDownloadToSyMaNa)})";
  public static int certificateExpiryPeriod = 15;
  public static string BridgeLogDirName = "BridgeLogs";
  public static string bridgeMainLogFileName = "iS5BridgeMainLog.zip";
  public static string logSessionsFileName = "iS5BridgeLogSessions.txt";
  public static string zipFileSuffix = ".zip";
  public static string bridgeFileName = "bridge.zip";
  public static string bridgeLogFileAttachmentName = "BridgeLog.zip";
  public static string sessionLogFileAttachmentName = "SessionLog.zip";
  public static string BridgeLogDirPath = $"{UtilityFunctions.GetPresentWorkingDir()}/{Constants.BridgeLogDirName}";
  public static string iService5DirectoryPath = "iService5.Core.resources.";
  public static string certificateInitials = "-----BEGIN CERTIFICATE-----\n";
  public static string certificateEnding = "\n-----END CERTIFICATE-----";
  public static string tempMetadataFileName = "tmpDownloadDB.db3";
  public static string TopogramDocument = "14127";
  public static string ControlDocument = "14117";
  public const string SignedCertificateFilename = "signedCertificate.pem";
  public const string clientCertificateFilename = "clientCertificiate.pem";
  public const string NOT_SENT = "NOT_SENT";
  public const string IN_PROGRESS = "IN_PROGRESS";
  public const string TEXT_FINAL = "FINAL";
  public const string TEXT_IDLE = "IDLE";
  public const string SENT = "SENT";
  public const string REQUEST_SENT = "REQUEST_SENT";
  public const string NOTIFY_TEXT = "NOTIFY";
  public const string RESPONSE_TEXT = "RESPONSE";
  public const string LODIS_URL_SyMaNa_Board_WIFI = "wss://192.168.0.1:28441/maintenance";
  public const string LODIS_URL_SyMaNa_Board_USB = "wss://192.168.217.2:28441/maintenance";
  public const string LODIS_URL_RPI = "wss://192.168.0.1:28441/maintenance";
  public const string LODIS_URL_LocalMock = "wss://127.0.0.1:28441/maintenance";
  public const string rebexKey_webclient = "==FmRMUbfErhzCOpVpdoa6P9Re7Y60fS3vvaK1sy/bw0Tb7pOjtz9f+AJkTbaLxK4Uwj4dp==";
  public const string rebexKey_websocket = "==FbLHiXIYgnC+Xwd/x7lsGEBy9n90RYvCSVOYma3B4FS64Jl4d6RYXimyxnQyGIJ0vluKp==";
  public const string LODIS_ENDPOINT_CI_SERVICES = "/ci/services";
  public const string LODIS_ENDPOINT_DEVICE_READY = "/ei/deviceReady";
  public const string EXTENSION_BIN = ".bin";
  public const string EXTENSION_CPIO = ".cpio";
  public const string EXTENSION_TAR = ".tar.gz";
  public const string EXTENSION_ZIP = ".zip";
  public static string[] extToBeRetained = new string[2]
  {
    ".db3",
    ".txt"
  };
  public const string SMM = "SMM";
  public const string NON_SMM = "NON_SMM";
  public const string BRIDGE = "BRIDGE";
  public const string CONNECTED = "• Connected";
  public const string refetchSettingsDBKey = "PpfRefetch";
  public const string MATERIAL_STATISTICS_TABLE = "materialStatistics";
  public const string ColorGreen = "Green";
  public const string ColorRed = "Red";
  public const string ColorBlack = "Black";
  public const string ColorGray = "#808080";
  public const string BulletSymbol = "• ";
  public const string CheckSymbol = "✓ ";
  public const int Lodis_Empty_Modules_count = 6;
  public const string HTTP_PREFIX = "http://";
  public const string BRIDGE_WIFI_MODE_ENDPOINT = "/activateBridge.php";
  public const string BRIDGE_SETTING_WEB_ENDPOINT = "/Admin.php";
  public const string BRIDGE_DATA_LOGGER_WEB_ENDPOINT = "/datalog/";
  public const string BRIDGE_UPGRADE_WEB_ENDPOINT = "/Admin.php";
  public const string CONTROL_ENDPOINT = "";
  public const string BRIDGE_WIFI_API_KEY = "1yuOALXjovthnxcaZ9VS8oRRh0Ah4U8izu6JEsmGzT8=";
  public const string NOT_MODIFIED = "304";
  public const string WEBSOCKET_ERROR_CHANNEL_CLOSED = "Channel closed while receiving header";
  public const string WEBSOCKET_INTERNAL_ERROR = "Internal error occurred";
  public const int SyMaNaRetryCountForUpload = 3;
  public const int SyMaNaNoOfAttemptsForConnection = 100;
  public const int SyMaNaRetryTimeoutForConnection = 6000;
  public const int SyMaNaRetryTimeoutToDisappearWifi = 5000;
  public const int SyMaNaNoOfAttemptsToCheckWifi = 10;
  public const string BinarySessionFolder = "binarySession";
  public const string ExtractedPpfsFolder = "extractedPpfs";
  public const string InternetNotAvailable = "Internet Connection appears to be offline";

  public static class SettingsDB
  {
    public const string FileName = "Settings.db3";
    public const string OfflineDaysCount = "OfflineDaysCount";
    public const string SinceAppUsedTimestamp = "SinceAppUsedTimestamp";
    public const string MetadataPostponedTimestamp = "MetadataPostponedTimestamp";
    public const string JwtTokenTimestamp = "JwtTokenTimestamp";
    public const string SupportFormSendStatus = "SupportFormSendStatus";
    public const string SupportFormSavedObject = "SupportFormSavedSubject";
    public const string SupportFormSavedNote = "SupportFormSavedNote";
    public const string SupportFormSessionLogFilename = "SupportFormSavedSessionLogFileName";
    public const string NotLogOut = "NotLogOut";
    public const string LogOut = "Logout";
    public const string UserLoggedOutStatus = "UserLoggedOutStatus";
    public const string UnAuthorisedStatusReceivedViewModelName = "UnAuthorisedStatusReceivedViewModelName";
  }
}
