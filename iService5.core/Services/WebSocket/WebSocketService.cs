// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.WebSocket.WebSocketService
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Appliance;
using iService5.Core.Services.Data;
using iService5.Core.Services.Data.SyMaNa;
using iService5.Core.Services.Helpers;
using iService5.Core.Services.Platform;
using iService5.Core.Services.User.ActivityLogging;
using Newtonsoft.Json;
using Rebex;
using Rebex.Net;
using Rebex.Security.Certificates;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Timers;

#nullable disable
namespace iService5.Core.Services.WebSocket;

public class WebSocketService : IWebSocketService
{
  private readonly ILoggingService _loggingService;
  private readonly IPlatformSpecificServiceLocator _locator;
  private readonly ISecureStorageService _secureStorageService;
  public WebSocketClient client;
  public long edMsgID;
  private SyMaNaWebSocketServices syMaNaServiceData;
  private System.Timers.Timer aTimer;
  private readonly long websocketPingTime = 15000;
  private List<string> trustedThumbprints = new List<string>();
  private CertificateIssuerData iS5ServiceCACertificateData;
  private bool localNetworkPermissionError;

  public bool IsNetworksPermissionError() => this.localNetworkPermissionError;

  public List<string> GetTrustedThumbprints() => this.trustedThumbprints;

  public WebSocketService(
    ILoggingService logService,
    ISecurityService securityService,
    IPlatformSpecificServiceLocator locator,
    ISecureStorageService secureStorageService)
  {
    this._loggingService = logService;
    this._locator = locator;
    this.GetWebSocketClient();
    this._secureStorageService = secureStorageService;
  }

  public bool isConnected()
  {
    return this.client != null && this.client.State == WebSocketState.Open || this.StartWebSocket();
  }

  public bool StartWebSocket()
  {
    try
    {
      if (this.InitiateNewConnection())
      {
        string initialValuesResponse = this.client.Receive<string>();
        this._loggingService.getLogger().LogAppDebug(LoggingContext.PROGRAMSYMANA, "First Response from Lodis: " + initialValuesResponse, memberName: nameof (StartWebSocket), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/WebSocket/WebSocketService.cs", sourceLineNumber: 69);
        this.SendInitialValues(initialValuesResponse);
        this.SendCiService();
        this.SendDeviceReady();
        this.SetTimerForWebsocketPing();
        return true;
      }
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppError(LoggingContext.WEBSOCKET, "Exception while initializing websocket: " + ex.Message, memberName: nameof (StartWebSocket), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/WebSocket/WebSocketService.cs", sourceLineNumber: 79);
      this._loggingService.getLogger().LogAppError(LoggingContext.WEBSOCKET, "Exception while initializing websocket Stack Trace : " + ex.StackTrace, memberName: nameof (StartWebSocket), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/WebSocket/WebSocketService.cs", sourceLineNumber: 80 /*0x50*/);
      return ex.Message == "The WebSocket has already been connected.";
    }
    return false;
  }

  private bool InitiateNewConnection()
  {
    TargetLodisType lodisType = FeatureConfiguration.LodisTargetType();
    this.localNetworkPermissionError = false;
    try
    {
      if (this.checkIfConnectionIsValid())
        return true;
      this.GetWebSocketClient();
      RebexHelper.RegisterCipherSuitesForWebSocket(this.client);
      bool isRealBoard = RebexHelper.IsTargetUsingRealBoard(lodisType);
      CertificateData certificateData = new CertificateData(this._secureStorageService, isRealBoard);
      CertificateChain certificateChain1 = new CertificateChain(new Certificate[1]
      {
        Certificate.LoadDerWithKey(certificateData.clientCertStream, certificateData.privateKeyStream, certificateData.password)
      });
      certificateChain1.Add(Certificate.LoadDer(certificateData.serviceCACertStream));
      CertificateChain certificateChain2 = certificateChain1;
      this.client.Settings.SslClientCertificateRequestHandler = CertificateRequestHandler.CreateRequestHandler(certificateChain2);
      string baseUrl = RebexHelper.GetBaseURL(lodisType);
      this.client.Options.KeepAliveInterval = new TimeSpan(0, 0, 10);
      this.iS5ServiceCACertificateData = RebexHelper.GetCertificateIssuerData(certificateChain2[1]);
      this._loggingService.getLogger().LogAppDebug(LoggingContext.PROGRAMSYMANA, $"iService5 iS5ServiceCACertificateData:\n CN: {this.iS5ServiceCACertificateData.CN} & OU: {this.iS5ServiceCACertificateData.OU} & O: {this.iS5ServiceCACertificateData.O} & Subject: {this.iS5ServiceCACertificateData.subject}", memberName: nameof (InitiateNewConnection), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/WebSocket/WebSocketService.cs", sourceLineNumber: 113);
      if (!FeatureConfiguration.LodisSslAcceptAllCertificates)
      {
        this.trustedThumbprints.Add(certificateChain2[1].Thumbprint);
        this.client.ValidatingCertificate += new EventHandler<SslCertificateValidationEventArgs>(this.client_ValidatingCertificate);
        this._loggingService.getLogger().LogAppDebug(LoggingContext.PROGRAMSYMANA, "Registered custom validation event handler for certificate validation", memberName: nameof (InitiateNewConnection), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/WebSocket/WebSocketService.cs", sourceLineNumber: 118);
      }
      this._loggingService.getLogger().LogAppDebug(LoggingContext.LOCAL, "Current SSID : " + this._locator.GetPlatformSpecificService().GetSSID(), memberName: nameof (InitiateNewConnection), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/WebSocket/WebSocketService.cs", sourceLineNumber: 120);
      this._loggingService.getLogger().LogAppDebug(LoggingContext.PROGRAMSYMANA, $"Connecting to URL... {baseUrl} & isRealBoard : {isRealBoard.ToString()}", memberName: nameof (InitiateNewConnection), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/WebSocket/WebSocketService.cs", sourceLineNumber: 121);
      this.client.Connect(baseUrl);
      return true;
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppError(LoggingContext.LOCAL, "Exception while initializing websocket: " + ex.Message, memberName: nameof (InitiateNewConnection), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/WebSocket/WebSocketService.cs", sourceLineNumber: (int) sbyte.MaxValue);
      this._loggingService.getLogger().LogAppError(LoggingContext.LOCAL, "Exception while initializing websocket Stack Trace : " + ex.StackTrace, memberName: nameof (InitiateNewConnection), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/WebSocket/WebSocketService.cs", sourceLineNumber: 128 /*0x80*/);
      this._loggingService.getLogger().LogAppDebug(LoggingContext.LOCAL, $"Current SSID : {this._locator.GetPlatformSpecificService().GetSSID()}And IP Address: {this._locator.GetPlatformSpecificService().GetIp()}", memberName: nameof (InitiateNewConnection), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/WebSocket/WebSocketService.cs", sourceLineNumber: 129);
      if (ex.Message.Contains("unreachable host"))
        this.localNetworkPermissionError = true;
      if (ex.Message == "The WebSocket has already been connected.")
        return true;
    }
    return false;
  }

  public void client_ValidatingCertificate(object sender, SslCertificateValidationEventArgs e)
  {
    this._loggingService.getLogger().LogAppDebug(LoggingContext.PROGRAMSYMANA, "Inside client_ValidatingCertificate method", memberName: nameof (client_ValidatingCertificate), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/WebSocket/WebSocketService.cs", sourceLineNumber: 144 /*0x90*/);
    if (e.CertificateChain.Validate(e.ServerName, ValidationOptions.None).Valid)
    {
      e.Accept();
      this._loggingService.getLogger().LogAppDebug(LoggingContext.PROGRAMSYMANA, "Certificate default Validation Successful", memberName: nameof (client_ValidatingCertificate), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/WebSocket/WebSocketService.cs", sourceLineNumber: 151);
    }
    else
    {
      Certificate certificate1 = e.CertificateChain[0];
      Certificate certificate2 = e.CertificateChain[1];
      if (this.trustedThumbprints.Contains(certificate1.Thumbprint) || this.trustedThumbprints.Contains(certificate2.Thumbprint))
      {
        e.Accept();
        this._loggingService.getLogger().LogAppDebug(LoggingContext.PROGRAMSYMANA, "Certificate Validation Successful, Thumbprint matched!", memberName: nameof (client_ValidatingCertificate), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/WebSocket/WebSocketService.cs", sourceLineNumber: 162);
      }
      else
      {
        CertificateIssuerData certificateIssuerData = RebexHelper.GetCertificateIssuerData(certificate2);
        this._loggingService.getLogger().LogAppDebug(LoggingContext.PROGRAMSYMANA, "SyMaNa intermedCert lodisCertData:\n OU " + certificateIssuerData.OU, memberName: nameof (client_ValidatingCertificate), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/WebSocket/WebSocketService.cs", sourceLineNumber: 167);
        if (RebexHelper.IsLodisCertValid(this.iS5ServiceCACertificateData, certificateIssuerData))
        {
          e.Accept();
          this.trustedThumbprints.Add(certificate1.Thumbprint);
          this.trustedThumbprints.Add(certificate2.Thumbprint);
          this._loggingService.getLogger().LogAppDebug(LoggingContext.PROGRAMSYMANA, "Certificate Validation Successful, Accepted!", memberName: nameof (client_ValidatingCertificate), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/WebSocket/WebSocketService.cs", sourceLineNumber: 173);
        }
        else
        {
          e.Reject();
          this._loggingService.getLogger().LogAppDebug(LoggingContext.PROGRAMSYMANA, "Certificate validation is Failed, Hence REJECTED!", memberName: nameof (client_ValidatingCertificate), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/WebSocket/WebSocketService.cs", sourceLineNumber: 178);
        }
      }
    }
  }

  private void SendInitialValues(string initialValuesResponse)
  {
    try
    {
      InitialValuesReceived initialValuesReceived = JsonConvert.DeserializeObject<InitialValuesReceived>(initialValuesResponse);
      long sId = initialValuesReceived.sID;
      long msgId = initialValuesReceived.msgID;
      long version = initialValuesReceived.version;
      string resource = initialValuesReceived.resource;
      this.edMsgID = long.Parse(initialValuesReceived.data[0].edMsgID);
      if (this.syMaNaServiceData == null)
      {
        this.syMaNaServiceData = new SyMaNaWebSocketServices(sId, msgId, version);
      }
      else
      {
        this.syMaNaServiceData.sID = sId;
        this.syMaNaServiceData.msgID = msgId;
        this.syMaNaServiceData.version = version;
      }
      string message = RebexHelper.InitialValuesForWebsocketSetup(this.syMaNaServiceData.getSyMaNaWebSocketObject("RESPONSE", resource), this._locator.GetPlatformSpecificService().GetDeviceID(), UtilityFunctions.GetDeviceName());
      this._loggingService.getLogger().LogAppDebug(LoggingContext.LOCAL, message, memberName: nameof (SendInitialValues), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/WebSocket/WebSocketService.cs", sourceLineNumber: 205);
      this.client.Send(message);
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppError(LoggingContext.LOCAL, "Exception while sending initialValuesSending: " + ex.Message, memberName: nameof (SendInitialValues), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/WebSocket/WebSocketService.cs", sourceLineNumber: 210);
      this._loggingService.getLogger().LogAppError(LoggingContext.LOCAL, "Exception while sending initialValuesSending: Stack Trace : " + ex.StackTrace, memberName: nameof (SendInitialValues), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/WebSocket/WebSocketService.cs", sourceLineNumber: 211);
    }
  }

  private void SendCiService()
  {
    this.syMaNaServiceData.msgID = this.edMsgID;
    SyMaNaWebsocketData naWebSocketObject = this.syMaNaServiceData.getSyMaNaWebSocketObject(HTTPMethodType.GET.ToString(), "/ci/services");
    naWebSocketObject.version = 1L;
    string message = JsonConvert.SerializeObject((object) naWebSocketObject);
    this._loggingService.getLogger().LogAppDebug(LoggingContext.PROGRAMSYMANA, message, memberName: nameof (SendCiService), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/WebSocket/WebSocketService.cs", sourceLineNumber: 221);
    this.client.Send(message);
    this.client.Receive<string>();
  }

  private void SendDeviceReady()
  {
    this.syMaNaServiceData.UpdateMsgIDCounter();
    SyMaNaWebsocketData naWebSocketObject = this.syMaNaServiceData.getSyMaNaWebSocketObject("NOTIFY", "/ei/deviceReady");
    naWebSocketObject.version = 2L;
    string message = JsonConvert.SerializeObject((object) naWebSocketObject);
    this.client.Send(message);
    this._loggingService.getLogger().LogAppDebug(LoggingContext.PROGRAMSYMANA, message, memberName: nameof (SendDeviceReady), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/WebSocket/WebSocketService.cs", sourceLineNumber: 233);
  }

  public void SetApplianceSessoin(IApplianceSession session)
  {
    if (this.syMaNaServiceData == null)
      return;
    this.syMaNaServiceData.SetApplianceSession(session);
  }

  public void CloseWebSocket()
  {
    try
    {
      if (this.client != null)
      {
        this._loggingService.getLogger().LogAppDebug(LoggingContext.WEBSOCKET, "Websocket Conn State: " + this.client.State.ToString(), memberName: nameof (CloseWebSocket), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/WebSocket/WebSocketService.cs", sourceLineNumber: 250);
        this.client.Close();
      }
    }
    catch (Exception ex)
    {
      this.client = (WebSocketClient) null;
      this._loggingService.getLogger().LogAppError(LoggingContext.WEBSOCKET, ex.Message, memberName: nameof (CloseWebSocket), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/WebSocket/WebSocketService.cs", sourceLineNumber: 257);
    }
    finally
    {
      try
      {
        if (this.client != null)
        {
          this.client.Dispose();
          this.client = (WebSocketClient) null;
        }
      }
      catch (Exception ex)
      {
        this._loggingService.getLogger().LogAppError(LoggingContext.WEBSOCKET, ex.Message, memberName: nameof (CloseWebSocket), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/WebSocket/WebSocketService.cs", sourceLineNumber: 271);
      }
    }
    this.syMaNaServiceData = (SyMaNaWebSocketServices) null;
    this.DisposeTimer();
  }

  public async Task<SyMaNaLodisResponse> WebSocketClientConnection(
    SyMaNaRequestCommandName jsonReqType)
  {
    this.syMaNaServiceData.UpdateMsgIDCounter();
    string jsonRequest = this.syMaNaServiceData.PrepareJSONRequest(jsonReqType);
    this._loggingService.getLogger().LogAppDebug(LoggingContext.PROGRAMSYMANA, "Request: " + jsonRequest, memberName: nameof (WebSocketClientConnection), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/WebSocket/WebSocketService.cs", sourceLineNumber: 283);
    SyMaNaLodisResponse syMaNaLodisResponse = (SyMaNaLodisResponse) null;
    try
    {
      bool isConnected = true;
      if (this.client.State != WebSocketState.Open)
        isConnected = this.InitiateNewConnection();
      await this.client.SendAsync(jsonRequest);
      string responseString = "";
      do
      {
        responseString = this.client.Receive<string>();
        if (string.IsNullOrEmpty(responseString))
        {
          string errorMsg = "ResponseString is NULL\nconnection State: " + this.client.State.ToString();
          this._loggingService.getLogger().LogAppError(LoggingContext.LOCAL, errorMsg, memberName: nameof (WebSocketClientConnection), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/WebSocket/WebSocketService.cs", sourceLineNumber: 300);
          return this.SetWebSocketConnectionException(errorMsg);
        }
        this._loggingService.getLogger().LogAppDebug(LoggingContext.PROGRAMSYMANA, "   -- Response: " + responseString, memberName: nameof (WebSocketClientConnection), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/WebSocket/WebSocketService.cs", sourceLineNumber: 303);
        if (!string.IsNullOrEmpty(responseString))
          syMaNaLodisResponse = JsonConvert.DeserializeObject<SyMaNaLodisResponse>(responseString);
        if (syMaNaLodisResponse.action == null && syMaNaLodisResponse.code != 0 || syMaNaLodisResponse.action.ToUpper() == "RESPONSE" && syMaNaLodisResponse.code != 0)
        {
          string errorMsg = "Error Code in response : " + syMaNaLodisResponse.code.ToString();
          return this.SetWebSocketConnectionException(errorMsg);
        }
      }
      while (syMaNaLodisResponse != null && syMaNaLodisResponse.action.ToUpper() != "NOTIFY");
      return syMaNaLodisResponse;
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppError(LoggingContext.LOCAL, $"Stack Trace : {ex.StackTrace} and state is {this.client.State.ToString()}", memberName: nameof (WebSocketClientConnection), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/WebSocket/WebSocketService.cs", sourceLineNumber: 320);
      this._loggingService.getLogger().LogAppDebug(LoggingContext.LOCAL, "Current SSID : " + this._locator.GetPlatformSpecificService().GetSSID(), memberName: nameof (WebSocketClientConnection), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/WebSocket/WebSocketService.cs", sourceLineNumber: 321);
      SyMaNaLodisResponse lodisResponse = this.SetWebSocketConnectionException(ex.Message);
      if (ex.Message.Contains("Internal error occurred"))
      {
        this._loggingService.getLogger().LogAppError(LoggingContext.LOCAL, "closing websocket connection on 'Internal error occurred'", memberName: nameof (WebSocketClientConnection), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/WebSocket/WebSocketService.cs", sourceLineNumber: 325);
        this.CloseWebSocket();
      }
      return lodisResponse;
    }
  }

  public async Task<SyMaNaLodisResponseData> RequestDataFromLodis(
    SyMaNaRequestCommandName commandName)
  {
    this.EnableWebSocketPing(false);
    SyMaNaLodisResponse response = await this.WebSocketClientConnection(commandName);
    SyMaNaLodisResponseData responseData = SyMaNaWebSocketServices.PrepareLodisResponseData(response);
    if (commandName != SyMaNaRequestCommandName.getInstallationProgress)
      this.EnableWebSocketPing(true);
    SyMaNaLodisResponseData lodisResponseData = responseData;
    response = (SyMaNaLodisResponse) null;
    responseData = (SyMaNaLodisResponseData) null;
    return lodisResponseData;
  }

  public bool SetPPFData(string ppfData)
  {
    if (this.syMaNaServiceData != null && !string.IsNullOrEmpty(ppfData))
    {
      this.syMaNaServiceData.ppfData = ppfData;
      this._loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSYMANA, "Successfuly set SetPPFData", memberName: nameof (SetPPFData), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/WebSocket/WebSocketService.cs", sourceLineNumber: 354);
      return true;
    }
    this._loggingService.getLogger().LogAppError(LoggingContext.PROGRAMSYMANA, "Either ppfData in paramter is NULL or syMaNaServiceData object is null", memberName: nameof (SetPPFData), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/WebSocket/WebSocketService.cs", sourceLineNumber: 359);
    return false;
  }

  public void ResetPPFData()
  {
    if (this.syMaNaServiceData == null)
      return;
    this.syMaNaServiceData.ppfData = (string) null;
  }

  private SyMaNaLodisResponse SetWebSocketConnectionException(string errorMsg)
  {
    this._loggingService.getLogger().LogAppError(LoggingContext.LOCAL, "Exception: " + errorMsg, memberName: nameof (SetWebSocketConnectionException), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/WebSocket/WebSocketService.cs", sourceLineNumber: 372);
    SyMaNaLodisResponse maNaLodisResponse = new SyMaNaLodisResponse()
    {
      lodisResponseError = new ServiceError(ErrorType.SyMaNa_Websocket_Error, errorMsg, false)
    };
    if (this.client != null && this.client.State == WebSocketState.CloseReceived)
      maNaLodisResponse.lodisResponseError.errorKey = WebExceptionStatus.ConnectionClosed;
    return maNaLodisResponse;
  }

  private void GetWebSocketClient()
  {
    if (this.client != null)
      return;
    this.client = new WebSocketClient();
    if (FeatureConfiguration.RebexLog)
    {
      string path = UtilityFunctions.GetPresentWorkingDir() + "/rebex_log.txt";
      System.IO.File.WriteAllText(path, "----websocket log----");
      this.client.LogWriter = (ILogWriter) new FileLogWriter(path, LogLevel.Verbose);
    }
  }

  private bool checkIfConnectionIsValid()
  {
    if (this.client != null)
    {
      if (this.client.State == WebSocketState.Open)
        return true;
      this.CloseWebSocket();
    }
    return false;
  }

  private void SetTimerForWebsocketPing()
  {
    if (this.aTimer != null)
      this.DisposeTimer();
    this.aTimer = new System.Timers.Timer((double) this.websocketPingTime);
    this.aTimer.Elapsed += new ElapsedEventHandler(this.OnTimedEvent);
    this.EnableWebSocketPing(true);
  }

  private void EnableWebSocketPing(bool enable)
  {
    if (this.aTimer == null)
      return;
    this.aTimer.Enabled = enable;
  }

  private void DisposeTimer()
  {
    if (this.aTimer == null)
      return;
    this.aTimer.Stop();
    this.aTimer.Dispose();
    this.aTimer = (System.Timers.Timer) null;
  }

  private void OnTimedEvent(object source, ElapsedEventArgs e)
  {
    if (!this.aTimer.Enabled)
      return;
    this._loggingService.getLogger().LogAppDebug(LoggingContext.WEBSOCKET, "iS5 is calling poll()", memberName: nameof (OnTimedEvent), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/WebSocket/WebSocketService.cs", sourceLineNumber: 452);
    this.client.Poll(0);
    this._loggingService.getLogger().LogAppDebug(LoggingContext.WEBSOCKET, "iS5 successfully called poll()", memberName: nameof (OnTimedEvent), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/WebSocket/WebSocketService.cs", sourceLineNumber: 454);
  }
}
