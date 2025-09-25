// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Data.History
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using SQLite;
using System;

#nullable disable
namespace iService5.Core.Services.Data;

public class History
{
  [PrimaryKey]
  [AutoIncrement]
  public int id { get; set; }

  public string eNumber { get; set; }

  public DateTime timestamp { get; set; }

  public string sessionID { get; set; }

  public string infoType { get; set; }

  public string historyData { get; set; }

  public History()
  {
  }

  public History(
    string eNumber,
    DateTime timestamp,
    string sessionID,
    string infoType,
    string historyData)
  {
    this.eNumber = eNumber;
    this.timestamp = timestamp;
    this.sessionID = sessionID;
    this.infoType = infoType;
    this.historyData = historyData;
  }
}
