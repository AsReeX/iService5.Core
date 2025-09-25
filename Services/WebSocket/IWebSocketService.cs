// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.WebSocket.IWebSocketService
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Appliance;
using iService5.Core.Services.Data.SyMaNa;
using System.Collections.Generic;
using System.Threading.Tasks;

#nullable disable
namespace iService5.Core.Services.WebSocket;

public interface IWebSocketService
{
  bool isConnected();

  bool StartWebSocket();

  Task<SyMaNaLodisResponseData> RequestDataFromLodis(SyMaNaRequestCommandName commandName);

  bool SetPPFData(string ppfData);

  void ResetPPFData();

  void CloseWebSocket();

  List<string> GetTrustedThumbprints();

  void SetApplianceSessoin(IApplianceSession session);

  bool IsNetworksPermissionError();
}
