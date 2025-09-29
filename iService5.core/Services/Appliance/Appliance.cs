// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Appliance.Appliance
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Helpers;
using iService5.Core.Services.Platform;
using iService5.Core.Services.User.ActivityLogging;
using iService5.Core.Services.WebSocket;
using Rebex.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace iService5.Core.Services.Appliance;

public class Appliance : IAppliance
{
  private readonly IWebSocketService _websocketService;
  private readonly ISecureStorageService _secureStorageService;
  private readonly IPlatformSpecificServiceLocator _locator;
  private readonly ILoggingService _loggingService;
  private const int thousandmsec = 1000;
  private string _ClientIp;
  private bool _serviceRunning = false;
  private bool websocketstarted = false;
  private readonly string[] bridgeWifiSSID = new string[2]
  {
    "bsh",
    "isb"
  };
  private readonly string applianceWifiSSIDRegex = "[b][s][h]([0-9]{5}|.{18}|.{29})$";
  private readonly string applianceWifiMockSSIDRegex = "[b][s][h][_]?([0-9]{5,29})$";
  private bool _boolStatusOfConnection = false;
  internal connectivityUpdateCallback ConnectivityUpdateCallback;
  private static readonly Action<object> GetWifiInfo = (Action<object>) (obj =>
  {
    while (true)
    {
      ((iService5.Core.Services.Appliance.Appliance) obj).connectivityCallback();
      Thread.Sleep(750);
    }
  });

  public string StatusOfConnection { get; set; }

  public string StatusOfBridgeConnection { get; set; }

  public bool boolStatusOfBridgeConnection { get; set; }

  public string ConnectedColor { get; set; }

  public string ConnectedColorBridge { get; set; }

  public bool DbusConnected { get; set; }

  public bool boolStatusOfConnection
  {
    get => this._boolStatusOfConnection;
    set => this._boolStatusOfConnection = value;
  }

  public Appliance(
    IPlatformSpecificServiceLocator locator,
    IWebSocketService websocketService,
    ISecureStorageService secureStorageService,
    ILoggingService loggingService)
  {
    this._locator = locator;
    this._serviceRunning = false;
    this._loggingService = loggingService;
    this._websocketService = websocketService;
    this._secureStorageService = secureStorageService;
    AsymmetricKeyAlgorithm.Register(new Func<string, object>(EllipticCurveAlgorithm.Create));
  }

  public void CheckConnectivity(connectivityUpdateCallback _cb)
  {
    this.ConnectivityUpdateCallback = _cb;
    if (!this._serviceRunning)
      Task.Factory.StartNew(iService5.Core.Services.Appliance.Appliance.GetWifiInfo, (object) this);
    this._serviceRunning = true;
  }

  internal void connectivityCallback()
  {
    this.setApplianceWifiConnectionProperties(this.isConnectedToApplianceWifi());
    if (this.isConnectedToBridgeWifi())
    {
      this.StatusOfBridgeConnection = !this.DbusConnected ? "✓ " + AppResource.CONNECTED_BRIDGE_TEXT : $"✓ {AppResource.CONNECTED_BRIDGE_TEXT} ✓ {AppResource.CONNECTED_DBUS_TEXT}";
      this.ConnectedColorBridge = "Green";
      this.boolStatusOfBridgeConnection = true;
    }
    if (!this.isConnectedToApplianceWifi() && !this.isConnectedToBridgeWifi())
    {
      this.StatusOfConnection = AppResource.DISCONNECTED_TEXT;
      this.StatusOfBridgeConnection = AppResource.DISCONNECTED_TEXT;
      this.ConnectedColor = "#808080";
      this.ConnectedColorBridge = "#808080";
      this.boolStatusOfConnection = false;
      this.boolStatusOfBridgeConnection = false;
    }
    this.ConnectivityUpdateCallback();
  }

  public bool isConnectedToApplianceWifi()
  {
    try
    {
      this._ClientIp = this._locator.GetPlatformSpecificService().GetIp();
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppError(LoggingContext.APPLIANCE, "Exception occured while fetching client ip:", ex, nameof (isConnectedToApplianceWifi), "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Appliance.cs", 129);
    }
    string ssid = this._locator.GetPlatformSpecificService().GetSSID();
    return this._ClientIp != null && this.isConnectedWithBSHWifi(ssid) && !this.isDhcpIP(this._ClientIp);
  }

  public bool isConnectedToBridgeWifi()
  {
    try
    {
      this._ClientIp = this._locator.GetPlatformSpecificService().GetIp();
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppError(LoggingContext.APPLIANCE, "Exception occured while fetching client ip:", ex, nameof (isConnectedToBridgeWifi), "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Appliance.cs", 154);
    }
    string ssid = this._locator.GetPlatformSpecificService().GetSSID();
    return this._ClientIp != null && this.isConnectedWithBridgeWifi(ssid) && !this.isDhcpIP(this._ClientIp);
  }

  public bool isConnectedWithBSHWifi(string ssid)
  {
    return Regex.IsMatch(ssid, this.applianceWifiSSIDRegex) || Regex.IsMatch(ssid, this.applianceWifiMockSSIDRegex);
  }

  private bool isConnectedWithBridgeWifi(string ssid)
  {
    return ((IEnumerable<string>) this.bridgeWifiSSID).Any<string>((Func<string, bool>) (s => ssid.ToLower().Contains(s)));
  }

  private bool isDhcpIP(string clientIp) => clientIp.StartsWith("169.");

  private void setApplianceWifiConnectionProperties(bool isConnected)
  {
    this.boolStatusOfConnection = isConnected;
    if (isConnected)
    {
      this.StatusOfConnection = "• " + AppResource.CONNECTED_TEXT;
      this.ConnectedColor = "Green";
    }
    else
    {
      this.StatusOfConnection = AppResource.DISCONNECTED_TEXT;
      this.ConnectedColor = "#808080";
    }
  }
}
