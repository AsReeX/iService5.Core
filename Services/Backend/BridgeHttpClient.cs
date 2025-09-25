// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Backend.BridgeHttpClient
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using AppDynamics.Agent;
using iService5.Core.Services.Platform;
using iService5.Core.Services.User.ActivityLogging;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

#nullable disable
namespace iService5.Core.Services.Backend;

public class BridgeHttpClient : IBridgeHttpClient
{
  private readonly IPlatformSpecificServiceLocator locator;
  private readonly ILoggingService loggingService;

  public BridgeHttpClient(IPlatformSpecificServiceLocator _locator, ILoggingService _loggingService)
  {
    this.locator = _locator;
    this.loggingService = _loggingService;
  }

  public async Task<BridgeWifiResponse> GetRequest(BridgeRequestType reqType)
  {
    BridgeWifiResponse bridgeResponse = new BridgeWifiResponse();
    string url = this.GetURL(reqType);
    this.loggingService.getLogger().LogAppInformation(LoggingContext.BRIDGESETTINGS, "URL for Bridge API mode change:  " + url, memberName: nameof (GetRequest), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/BridgeHttpClient.cs", sourceLineNumber: 35);
    try
    {
      using (HttpClient httpClient = new HttpClient((HttpMessageHandler) new HttpRequestTrackerHandler()))
      {
        httpClient.DefaultRequestHeaders.Add("X-API-KEY", "1yuOALXjovthnxcaZ9VS8oRRh0Ah4U8izu6JEsmGzT8=");
        HttpResponseMessage response = await httpClient.GetAsync(url);
        response.StatusCode = HttpStatusCode.NotModified;
        HttpStatusCode statusCode = response.StatusCode;
        this.loggingService.getLogger().LogAppInformation(LoggingContext.BRIDGESETTINGS, "Bridge Status: " + statusCode.ToString(), memberName: nameof (GetRequest), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/BridgeHttpClient.cs", sourceLineNumber: 44);
        this.loggingService.getLogger().LogAppInformation(LoggingContext.BRIDGESETTINGS, "Bridge Response: " + response.Content.ReadAsStringAsync().Result, memberName: nameof (GetRequest), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/BridgeHttpClient.cs", sourceLineNumber: 45);
        if (statusCode == HttpStatusCode.OK || statusCode == HttpStatusCode.NotModified || statusCode.ToString().Contains("304"))
          bridgeResponse.isSuccess = true;
        response = (HttpResponseMessage) null;
      }
    }
    catch (Exception ex)
    {
      bridgeResponse.isSuccess = false;
      bridgeResponse.errorMessage = ex.Message;
      this.loggingService.getLogger().LogAppError(LoggingContext.BRIDGESETTINGS, "Error Message : " + ex.Message, memberName: nameof (GetRequest), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/BridgeHttpClient.cs", sourceLineNumber: 56);
    }
    BridgeWifiResponse request = bridgeResponse;
    bridgeResponse = (BridgeWifiResponse) null;
    url = (string) null;
    return request;
  }

  private string GetURL(BridgeRequestType reqType)
  {
    string str = "http://" + this.locator.GetPlatformSpecificService().GetIp();
    return reqType == BridgeRequestType.WIFI_MODE ? str + "/activateBridge.php" : "";
  }
}
