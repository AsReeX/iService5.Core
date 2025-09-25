// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Local.LocalUserAuthentication
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Backend;
using iService5.Core.Services.Helpers;
using iService5.Core.Services.User;
using MvvmCross;

#nullable disable
namespace iService5.Core.Services.Local;

public class LocalUserAuthentication : ILocalUserAuthentication
{
  public BackendRequestStatus authenticateUser(ISecureStorageService _secureStorageService)
  {
    IUserAccount userAccount = Mvx.IoCProvider.Resolve<IUserAccount>();
    string encoded1 = "";
    BackendRequestStatus encoded2 = userAccount.getEncoded(out encoded1);
    if (encoded2 != 0)
      return encoded2;
    string result1 = _secureStorageService.getUsername().Result;
    string result2 = _secureStorageService.getPassword().Result;
    if (result1 == null || result2 == null)
      return BackendRequestStatus.RES_NLocalCredentials;
    string encoded3;
    BackendRequestStatus encoded4 = new UserAccount(result1, result2).getEncoded(out encoded3);
    if (encoded4 != 0)
      return encoded4;
    return encoded3 == encoded1 ? BackendRequestStatus.RES_OK : BackendRequestStatus.RES_NLocal;
  }
}
