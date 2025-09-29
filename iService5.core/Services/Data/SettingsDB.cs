// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Data.SettingsDB
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using SQLite;
using System;
using System.Collections.Generic;

#nullable disable
namespace iService5.Core.Services.Data;

public class SettingsDB
{
  public readonly SQLiteConnection database;
  private readonly object lockObj = new object();

  public SettingsDB(string dbPath)
  {
    try
    {
      this.database = new SQLiteConnection(new SQLiteConnectionString(dbPath, true, (object) "xaIOsmdhakj", postKeyAction: (Action<SQLiteConnection>) (c => c.Execute("PRAGMA cipher_compatibility = 3"))));
      int table = (int) this.database.CreateTable<Settings>();
    }
    catch (SQLiteException ex) when (ex.Result == SQLite3.Result.NonDBFile)
    {
      this.database = new SQLiteConnection(new SQLiteConnectionString(dbPath));
      int table = (int) this.database.CreateTable<Settings>();
    }
  }

  public List<Settings> GetSettings() => this.database.Table<Settings>().ToList();

  public Settings GetItem(string name)
  {
    return this.database.Query<Settings>($"select Name , Value from Settings WHERE Name = '{name}' ").Find((Predicate<Settings>) (i => i.Name == name));
  }

  public void SaveItem(Settings item)
  {
    lock (this.lockObj)
    {
      this.database.BeginTransaction();
      this.database.Insert((object) item);
      this.database.Commit();
    }
  }

  public void UpdateItem(Settings item)
  {
    lock (this.lockObj)
    {
      this.database.BeginTransaction();
      if (this.GetItem(item.Name) != null)
        this.database.Update((object) item);
      else
        this.database.Insert((object) item);
      this.database.Commit();
    }
  }

  public void DeleteItem(Settings item)
  {
    lock (this.lockObj)
    {
      this.database.BeginTransaction();
      this.database.Delete((object) item);
      this.database.Commit();
    }
  }

  internal void GetItem(Settings settings) => throw new NotImplementedException();
}
