// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Helpers.BridgeResponse
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

#nullable disable
namespace iService5.Core.Services.Helpers;

public class BridgeResponse
{
  [JsonProperty("dbusconnection")]
  public bool dbusconnection { get; set; }

  [JsonProperty("hsidevice")]
  public bool hsidevice { get; set; }

  [JsonProperty("udaconnection")]
  public (Tuple<string, string>, Tuple<string, bool>) udaconnection { get; set; }

  [JsonProperty("catconnection")]
  public Dictionary<string, bool> catconnection { get; set; }

  [JsonProperty("drmconnection")]
  public Dictionary<string, bool> drmconnection { get; set; }

  [JsonProperty("psuconnection")]
  public Dictionary<string, bool> psuconnection { get; set; }

  [JsonProperty("datalogger")]
  public (Tuple<string, string>, Tuple<string, bool>, Tuple<string, bool>) datalogger { get; set; }

  [JsonProperty("wifi")]
  public (Tuple<string, bool>, Tuple<string, bool>, Tuple<string, string>) wifi { get; set; }

  [JsonProperty("usbdevices")]
  public List<string> usbdevices { get; set; }
}
