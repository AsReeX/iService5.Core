// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Platform.IPlatformSpecificService
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
//using Xamarin.Essentials;
//using Microsoft.Maui;
//using Microsoft.Maui.Controls;
//using Microsoft.Maui.Networking;

#nullable disable
namespace iService5.Core.Services.Platform;

public interface IPlatformSpecificService
{
  NetworkAccess getCurrentNetworkStatus();

  IEnumerable<ConnectionProfile> getCurrentNetworkProfile();

  bool isFingerprintSupported();

  bool isDeviceSecured();

  bool isFingerprintAvailable();

  bool isFingerprintPermissionGranted();

  bool isFingerprintValid(fingerprintCompletionCallback cb);

  void initialiseScheduledMetadataSession();

  string getExportFolder();

  string getInternalFolder();

  string getFolder();

  HttpClientHandler getClientHandler();

  void setConnectivityCallback(NetworkConnected NetworkConnectivityCallback);

  NetworkConnected getConnectivityCallback();

  void OpenSettings();

  void OpenWifiSettings();

  string GetSSID();

  void GetLocationConsent();

  string GetIp();

  Task<bool> SetValueInSecureStorageForKey(string key, string value);

  Task<string> getValueFromSecureStorageForKey(string key);

  string GetMemoryStorage(bool isTotalSpace);

  Task<bool> IsNetworkAvailable();

  void SaveImageFromByte(byte[] imageByte, string filename);

  void setEnumber(string repairEnumber);

  void ResetSession();

  string GetDeviceRegion();

  string GetDeviceID();

  bool IsLocationServiceEnabled();

  bool RemoveFromSecureStorage(string key);

  void SetFileProtectionNone(string filePath);
}