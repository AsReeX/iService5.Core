// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Helpers.SecureStorageService
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Platform;
using iService5.Core.Services.User.ActivityLogging;
using System;
using System.Threading.Tasks;
//using Xamarin.Essentials;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

#nullable disable
namespace iService5.Core.Services.Helpers;

public class SecureStorageService : ISecureStorageService
{
  private readonly ILoggingService _loggingService;
  private readonly IPlatformSpecificServiceLocator _locator;

  public SecureStorageService(
    ILoggingService loggingService,
    IPlatformSpecificServiceLocator locator)
  {
    this._loggingService = loggingService;
    this._locator = locator;
  }

  public async Task<string> getJWTToken()
  {
    string jwtToken = await this.GetValue(SecureStorageKeys.JWT_TOKEN);
    return jwtToken;
  }

  public async Task<bool> setJWTToken(string jwtToken)
  {
    bool flag = await this.SetValue(SecureStorageKeys.JWT_TOKEN, jwtToken);
    return flag;
  }

  public async Task<string> getUsername()
  {
    string username = await this.GetValue(SecureStorageKeys.USERNAME);
    return username;
  }

  public async Task<bool> setUsername(string username)
  {
    bool flag = await this.SetValue(SecureStorageKeys.USERNAME, username);
    return flag;
  }

  public async Task<bool> setPrivateKeyInfo(string pkistring)
  {
    bool flag = await this.SetValue(SecureStorageKeys.PRIVATE_KEY_INFO, pkistring);
    return flag;
  }

  public async Task<string> getPrivateKeyInfo()
  {
    string privateKeyInfo = await this.GetValue(SecureStorageKeys.PRIVATE_KEY_INFO);
    return privateKeyInfo;
  }

  public async Task<string> getPrivateKeyInfoPassword()
  {
    string privateKeyInfoPassword = await this.GetValue(SecureStorageKeys.PRIVATE_KEY_INFO_PASSWORD);
    return privateKeyInfoPassword;
  }

  public async Task<bool> setPrivateKeyInfoPassword(string pkipassstring)
  {
    bool flag = await this.SetValue(SecureStorageKeys.PRIVATE_KEY_INFO_PASSWORD, pkipassstring);
    return flag;
  }

  public async Task<string> getPassword()
  {
    string password = await this.GetValue(SecureStorageKeys.PASSWORD);
    return password;
  }

  public async Task<bool> setPassword(string password)
  {
    bool flag = await this.SetValue(SecureStorageKeys.PASSWORD, password);
    return flag;
  }

  public async Task<string> getAWSCertificatePublicKey()
  {
    string certificatePublicKey = await this.GetValue(SecureStorageKeys.AWS_CERTIFICATE_PUBLIC_KEY);
    return certificatePublicKey;
  }

  public async Task<bool> setAWSCertificatePublicKey(string serverCertificatePublicKey)
  {
    bool flag = await this.SetValue(SecureStorageKeys.AWS_CERTIFICATE_PUBLIC_KEY, serverCertificatePublicKey);
    return flag;
  }

  public async Task<string> GetWiFiPasswordFromSecureStorage()
  {
    string fromSecureStorage = await this.GetValue(SecureStorageKeys.WIFI_PASSWORD);
    return fromSecureStorage;
  }

  public async Task<bool> SetWiFiPasswordInSecureStorage(string password)
  {
    bool flag = await this.SetValue(SecureStorageKeys.WIFI_PASSWORD, password);
    return flag;
  }

  public async Task<string> GetWiFiBridgePasswordFromSecureStorage()
  {
    string fromSecureStorage = await this.GetValue(SecureStorageKeys.WIFI_BRIDGE_PASSWORD);
    return fromSecureStorage;
  }

  public async Task<bool> SetWiFiBridgePasswordInSecureStorage(string password)
  {
    bool flag = await this.SetValue(SecureStorageKeys.WIFI_BRIDGE_PASSWORD, password);
    return flag;
  }

  public async Task<string> GetDBPasswordFromSecureStorage()
  {
    string fromSecureStorage = await this.GetValue(SecureStorageKeys.DB_PASSWORD);
    return fromSecureStorage;
  }

  public async Task<bool> SetDBPasswordInSecureStorage(string password)
  {
    bool flag = await this.SetValue(SecureStorageKeys.DB_PASSWORD, password);
    return flag;
  }

  public async Task<string> GetTempDBPasswordFromSecureStorage()
  {
    string fromSecureStorage = await this.GetValue(SecureStorageKeys.TEMP_DB_PASSWORD);
    return fromSecureStorage;
  }

  public async Task<bool> SetTempDBPasswordInSecureStorage(string password)
  {
    bool flag = await this.SetValue(SecureStorageKeys.TEMP_DB_PASSWORD, password);
    return flag;
  }

  public async Task<string> GetValue(SecureStorageKeys key)
  {
    string value = "";
    try
    {
      value = await this._locator.GetPlatformSpecificService().getValueFromSecureStorageForKey(key.ToString());
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppError(LoggingContext.SECURESTORAGE, $"Exception occured while fetching {key.ToString()}:{ex?.ToString()}", memberName: nameof (GetValue), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/SecureStorageService.cs", sourceLineNumber: 143);
    }
    string str = value;
    value = (string) null;
    return str;
  }

  public async Task<string> GetValue(string key)
  {
    string value = "";
    try
    {
      value = await this._locator.GetPlatformSpecificService().getValueFromSecureStorageForKey(key);
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppError(LoggingContext.SECURESTORAGE, $"Exception occured while fetching {key}:{ex?.ToString()}", memberName: nameof (GetValue), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/SecureStorageService.cs", sourceLineNumber: 157);
    }
    string str = value;
    value = (string) null;
    return str;
  }

  public async Task<bool> SetValue(SecureStorageKeys key, string value)
  {
    bool isStoredSuccesfully = false;
    try
    {
      isStoredSuccesfully = await this._locator.GetPlatformSpecificService().SetValueInSecureStorageForKey(key.ToString(), value);
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppError(LoggingContext.SECURESTORAGE, $"Exception occured while setting {key.ToString()}:{ex?.ToString()}", memberName: nameof (SetValue), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/SecureStorageService.cs", sourceLineNumber: 171);
    }
    return isStoredSuccesfully;
  }

  public bool Remove(SecureStorageKeys key)
  {
    bool flag = false;
    try
    {
      flag = this._locator.GetPlatformSpecificService().RemoveFromSecureStorage(key.ToString());
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppError(LoggingContext.SECURESTORAGE, $"Exception occured while removing {key.ToString()}:{ex?.ToString()}", memberName: nameof (Remove), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/SecureStorageService.cs", sourceLineNumber: 185);
    }
    return flag;
  }

  public async Task<bool> SetSignedCertificate(string signedCertificate)
  {
    string[] certificates = signedCertificate.Split(new string[1]
    {
      Constants.certificateEnding
    }, StringSplitOptions.None);
    if (certificates.Length == 0)
      return false;
    string clientCertificate = Constants.certificateInitials + UtilityFunctions.getActualCertificateString(certificates[0]) + Constants.certificateEnding;
    bool isClientCSaved = await this.SetValue(SecureStorageKeys.CERTIFICATE_CLIENT, clientCertificate);
    this._loggingService.getLogger().LogAppDebug(LoggingContext.SECURESTORAGE, "CLIENT_CERTIFICATE is successfully stored in secured Storage", memberName: nameof (SetSignedCertificate), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/SecureStorageService.cs", sourceLineNumber: 197);
    string serviceAccessCA = Constants.certificateInitials + UtilityFunctions.getActualCertificateString(certificates[1]) + Constants.certificateEnding;
    bool isServiceCASaved = await this.SetValue(SecureStorageKeys.CERTIFICATE_SERVICE_ACCESS_CA, serviceAccessCA);
    this._loggingService.getLogger().LogAppDebug(LoggingContext.SECURESTORAGE, "SIGNED_CERTIFICATE is successfully stored in secured Storage", memberName: nameof (SetSignedCertificate), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/SecureStorageService.cs", sourceLineNumber: 201);
    return isClientCSaved & isServiceCASaved;
  }

  public async Task<string> GetSignedCertificate(SecureStorageKeys key)
  {
    string signedCertificate = await this.GetValue(key);
    return signedCertificate;
  }

  public void RemoveAll() => SecureStorage.RemoveAll();
}
