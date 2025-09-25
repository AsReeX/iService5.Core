// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.User.UserAccount
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Backend;
using System;
using System.Text;

#nullable disable
namespace iService5.Core.Services.User;

public class UserAccount : IUserAccount
{
  private string _username;
  private string _password;

  public UserAccount(string username, string password)
  {
    this._username = username;
    this._password = password;
  }

  public UserAccount()
  {
  }

  public BackendRequestStatus getEncoded(out string encoded)
  {
    if (this._username.Length == 0)
    {
      encoded = "";
      return BackendRequestStatus.RES_NName;
    }
    if (this._password.Length == 0)
    {
      encoded = "";
      return BackendRequestStatus.RES_NPassword;
    }
    encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{this._username.ToLowerInvariant()}:{this._password}"));
    return BackendRequestStatus.RES_OK;
  }

  public void setAccount(string username, string password)
  {
    this._username = username;
    this._password = password;
  }

  public string getUserName() => this._username;
}
