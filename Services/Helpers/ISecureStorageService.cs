// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Helpers.ISecureStorageService
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using System.Threading.Tasks;

#nullable disable
namespace iService5.Core.Services.Helpers;

public interface ISecureStorageService
{
  Task<string> getJWTToken();

  Task<bool> setJWTToken(string jwtToken);

  Task<string> getUsername();

  Task<bool> setUsername(string username);

  Task<bool> setPrivateKeyInfo(string pkistring);

  Task<bool> setPrivateKeyInfoPassword(string pkipassstring);

  Task<string> getPrivateKeyInfo();

  Task<string> getPrivateKeyInfoPassword();

  Task<string> getPassword();

  Task<bool> setPassword(string password);

  Task<string> getAWSCertificatePublicKey();

  Task<bool> setAWSCertificatePublicKey(string serverCertificatePublicKey);

  Task<string> GetWiFiPasswordFromSecureStorage();

  Task<bool> SetWiFiPasswordInSecureStorage(string password);

  Task<string> GetWiFiBridgePasswordFromSecureStorage();

  Task<bool> SetWiFiBridgePasswordInSecureStorage(string password);

  Task<string> GetDBPasswordFromSecureStorage();

  Task<bool> SetDBPasswordInSecureStorage(string password);

  Task<string> GetTempDBPasswordFromSecureStorage();

  Task<bool> SetTempDBPasswordInSecureStorage(string password);

  Task<string> GetValue(SecureStorageKeys key);

  Task<string> GetValue(string key);

  Task<bool> SetValue(SecureStorageKeys key, string value);

  Task<bool> SetSignedCertificate(string password);

  Task<string> GetSignedCertificate(SecureStorageKeys key);

  bool Remove(SecureStorageKeys key);

  void RemoveAll();
}
