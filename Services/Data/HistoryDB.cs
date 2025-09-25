// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Data.HistoryDB
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Helpers;
using SQLite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

#nullable disable
namespace iService5.Core.Services.Data;

public class HistoryDB
{
  public readonly SQLiteConnection database;

  public HistoryDB(string dbPath)
  {
    try
    {
      this.database = new SQLiteConnection(new SQLiteConnectionString(dbPath, true, (object) "xaIOsmdhakj", postKeyAction: (Action<SQLiteConnection>) (c => c.Execute("PRAGMA cipher_compatibility = 3"))));
      int table = (int) this.database.CreateTable<History>();
    }
    catch (SQLiteException ex) when (ex.Result == SQLite3.Result.NonDBFile)
    {
      this.database = new SQLiteConnection(new SQLiteConnectionString(dbPath));
      int table = (int) this.database.CreateTable<History>();
    }
  }

  public List<History> GetHistoryDetails(string sessionID)
  {
    return this.database.Query<History>($"SELECT * FROM History WHERE sessionID = '{sessionID}'");
  }

  public void MigrateHistoryDB()
  {
    List<History> objects = this.database.Query<History>("SELECT * FROM History WHERE sessionID IS NULL OR sessionID IS ''");
    if (objects.Count <= 0)
      return;
    foreach (History history in objects)
    {
      history.sessionID = $"{history.eNumber}-{history.timestamp.Millisecond.ToString()}";
      history.infoType = HistoryDBInfoType.NewSessionStarts.ToString();
      history.historyData = HistoryDBInfoType.NewSessionStarts.ToString();
    }
    this.database.BeginTransaction();
    this.database.UpdateAll((IEnumerable) objects);
    this.database.Commit();
  }

  public List<History> GetHistoryList()
  {
    this.MigrateHistoryDB();
    return this.database.Query<History>($"SELECT * FROM History WHERE infoType = '{HistoryDBInfoType.NewSessionStarts.ToString()}' ORDER BY id DESC");
  }

  public History GetItem(string eNumber)
  {
    try
    {
      return this.database.Table<History>().FirstOrDefault((Expression<Func<History, bool>>) (i => i.eNumber == eNumber));
    }
    catch (Exception ex)
    {
      return (History) null;
    }
  }

  public void SaveItem(History item)
  {
    this.database.BeginTransaction();
    this.database.Insert((object) item);
    this.database.Commit();
  }

  public void SaveItems(IList<History> entries)
  {
    this.database.BeginTransaction();
    foreach (object entry in (IEnumerable<History>) entries)
      this.database.Insert(entry);
    this.database.Commit();
  }

  public void UpdateItem(Settings item)
  {
    this.database.BeginTransaction();
    if (this.GetItem(item.Name) != null)
      this.database.Update((object) item);
    else
      this.database.Insert((object) item);
    this.database.Commit();
  }

  public void DeleteItem(Settings item)
  {
    this.database.BeginTransaction();
    this.database.Delete((object) item);
    this.database.Commit();
  }

  public void DeleteHistoryEntriesOfSpecificSessionId(string sessionId)
  {
    List<History> historyList = this.database.Query<History>($"SELECT * FROM History WHERE sessionID = '{sessionId}'");
    this.database.BeginTransaction();
    foreach (object objectToDelete in historyList)
      this.database.Delete(objectToDelete);
    this.database.Commit();
  }

  internal void GetItem(Settings settings) => throw new NotImplementedException();
}
