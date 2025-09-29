// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Helpers.UtilityFunctions
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Appliance;
using iService5.Core.Services.Backend;
using iService5.Core.Services.Data;
using iService5.Core.Services.Platform;
using iService5.Core.Services.User;
using iService5.Core.Services.User.ActivityLogging;
using iService5.Core.Services.VersionReport;
using iService5.Core.ViewModels;
using iService5.Ssh.DTO;
using iService5.Ssh.enums;
using iService5.Ssh.models;
//using Xamarin.Essentials;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using MvvmCross;
using MvvmCross.Navigation;
using MvvmCross.Presenters.Hints;
using MvvmCross.ViewModels;
using MvvmHelpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace iService5.Core.Services.Helpers;

public static class UtilityFunctions
{
    private static string dateTimeFormatterString = "yyyyMMddHHmmssffff";
    private static string sessionID;

    public static Stream GenerateStreamFromString(string value)
    {
        MemoryStream streamFromString = new MemoryStream();
        StreamWriter streamWriter = new StreamWriter((Stream)streamFromString);
        streamWriter.Write(value);
        streamWriter.Flush();
        streamFromString.Position = 0L;
        return (Stream)streamFromString;
    }

    public static string ConvertDateTimeToString(DateTime dateTime)
    {
        return dateTime.ToString(UtilityFunctions.dateTimeFormatterString);
    }

    public static DateTime ConvertStringToDateTime(string timestamp)
    {
        return string.IsNullOrWhiteSpace(timestamp) ? new DateTime(1970, 1, 1, 0, 0, 0, 0) : DateTime.ParseExact(timestamp, UtilityFunctions.dateTimeFormatterString, (IFormatProvider)null);
    }

    public static string GetBootModeResponseText(BootMode bootModeResponse)
    {
        return bootModeResponse == BootMode.Recovery || bootModeResponse == BootMode.Maintenance ? AppResource.APPLIANCE_BOOT_MODE_RECOVERY : AppResource.APPLIANCE_BOOT_MODE_NORMAL;
    }

    public static Dictionary<string, string> GetFeedbackFormPostParameters()
    {
        return new Dictionary<string, string>()
    {
      {
        "subject",
        CoreApp.settings.GetItem("SupportFormSavedSubject").Value
      },
      {
        "message",
        CoreApp.settings.GetItem("SupportFormSavedNote").Value
      },
      {
        "sessionLogFilename",
        CoreApp.settings.GetItem("SupportFormSavedNote").Value
      }
    };
    }

    public static string GetDeviceTimeZone() => TimeZoneInfo.Local.StandardName;

    public static bool isDeviceTimeZoneChina()
    {
        return TimeZoneInfo.Local.Id.Contains("Asia/") && (UtilityFunctions.GetDeviceTimeZone().Equals("CST") || UtilityFunctions.GetDeviceTimeZone().Equals("HKT"));
    }

    public static string GetStorageUtilization()
    {
        IPlatformSpecificServiceLocator locator = Mvx.IoCProvider.Resolve<IPlatformSpecificServiceLocator>();
        long fileSize = 0;
        List<string> stringList = new List<string>();
        stringList.AddRange((IEnumerable<string>)Directory.GetFiles(locator.GetPlatformSpecificService().getFolder()));
        stringList.ForEach((Action<string>)(file => fileSize += new FileInfo(Path.Combine(locator.GetPlatformSpecificService().getFolder(), file)).Length));
        return FileSizeFormatter.FormatSize(fileSize);
    }

    public static void MergeLogFiles()
    {
        try
        {
            IPlatformSpecificServiceLocator specificServiceLocator = Mvx.IoCProvider.Resolve<IPlatformSpecificServiceLocator>();
            string mergedFileName = "iS5LogsMerged.txt";
            string path = Path.Combine(specificServiceLocator.GetPlatformSpecificService().getFolder(), mergedFileName);
            File.Delete(path);
            DirectoryInfo directoryInfo = new DirectoryInfo(specificServiceLocator.GetPlatformSpecificService().getFolder());
            using (StreamWriter streamWriter = File.AppendText(path))
            {
                streamWriter.WriteLine("****Additional Information****");
                streamWriter.WriteLine("------------------------------------------------------");
                streamWriter.WriteLine("Device Specific");
                streamWriter.WriteLine("-----------------");
                if (DeviceInfo.Platform != DevicePlatform.Unknown)
                    streamWriter.Write(string.Format("Device Platform- {1}{0}Device Model - {2}{0}Device Manufacturer - {3}{0}OS Version - {4}{0}", (object)Environment.NewLine, (object)DeviceInfo.Platform, (object)DeviceInfo.Model, (object)DeviceInfo.Manufacturer, (object)DeviceInfo.VersionString));
                streamWriter.WriteLine("Device TimeZone - {0} - {1}", (object)TimeZoneInfo.Local.DisplayName, (object)UtilityFunctions.GetDeviceTimeZone());
                streamWriter.WriteLine("Region - {0}", (object)specificServiceLocator.GetPlatformSpecificService().GetDeviceRegion());
                streamWriter.WriteLine("Language - {0}", (object)CultureInfo.InvariantCulture);
                streamWriter.WriteLine("Memory Utilization - {0}", (object)specificServiceLocator.GetPlatformSpecificService().GetMemoryStorage(true));
                streamWriter.WriteLine();
                streamWriter.WriteLine("App Specific");
                streamWriter.WriteLine("-----------------");
                IVersionReport versionReport = Mvx.IoCProvider.Resolve<IVersionReport>();
                streamWriter.WriteLine("iService5 App Version - {0} {1}", (object)BuildProperties.stage, (object)versionReport.getVersion());
                streamWriter.WriteLine("Storage Utilization - {0}", (object)UtilityFunctions.GetStorageUtilization());
                Settings settings = CoreApp.settings.GetItem("lastCheck");
                if (settings != null && settings.Value != "")
                    streamWriter.WriteLine("Last update of Metadata-DB - {0}", (object)settings.Value);
                else
                    streamWriter.WriteLine("Last update of Metadata-DB - {0}", (object)"");
                streamWriter.WriteLine("------------------------------------------------------");
                streamWriter.WriteLine();
                FileInfo[] array1 = ((IEnumerable<FileInfo>)directoryInfo.GetFiles("iS5Log*")).Where<FileInfo>((Func<FileInfo, bool>)(x => x.Name != mergedFileName)).ToArray<FileInfo>();
                Array.Sort<FileInfo>(array1, (Comparison<FileInfo>)((f1, f2) => f1.CreationTime.CompareTo(f2.CreationTime)));
                string str1 = "";
                foreach (FileSystemInfo fileSystemInfo in array1)
                {
                    using (StreamReader streamReader = new StreamReader(fileSystemInfo.FullName))
                    {
                        str1 += streamReader.ReadToEnd();
                        streamReader.Close();
                    }
                }
                streamWriter.Write(str1);
                FileInfo[] array2 = ((IEnumerable<FileInfo>)directoryInfo.GetFiles("crash*")).ToArray<FileInfo>();
                if (array2.Length != 0)
                {
                    streamWriter.WriteLine("------------------------------------------------------");
                    streamWriter.WriteLine("***Crash Report***");
                    streamWriter.WriteLine("-----------------");
                    string str2 = "";
                    Array.Sort<FileInfo>(array2, (Comparison<FileInfo>)((f1, f2) => f1.CreationTime.CompareTo(f2.CreationTime)));
                    using (StreamReader streamReader = new StreamReader(array2[array2.Length - 1].FullName))
                    {
                        str2 += streamReader.ReadToEnd();
                        streamReader.Close();
                    }
                    streamWriter.Write(str2);
                }
                streamWriter.Close();
            }
        }
        catch (Exception ex)
        {
            Mvx.IoCProvider.Resolve<ILoggingService>().getLogger().LogAppError(LoggingContext.BACKEND, "Exception occured while merging log files:" + ex?.ToString(), memberName: nameof(MergeLogFiles), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/UtilityFunctions.cs", sourceLineNumber: 183);
        }
    }

    public static string GetCurrentCultureDateFormat()
    {
        return CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
    }

    public static string GetCurrentCultureTimeFormat()
    {
        return CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern;
    }

    public static CultureInfo GetCurrentCulture() => CultureInfo.CurrentCulture;

    public static string GetCountryNameBasedOnSettings(int countrySettings)
    {
        string str = countrySettings.ToString();
        string nameBasedOnSettings;
        switch (countrySettings)
        {
            case 1:
                nameBasedOnSettings = $"{str} ({AppResource.COUNTRY_NORTH_AMERICA})";
                break;
            case 2:
                nameBasedOnSettings = $"{str} ({AppResource.COUNTRY_EUROPE})";
                break;
            case 3:
                nameBasedOnSettings = $"{str} ({AppResource.COUNTRY_ASIA_PACIFIC})";
                break;
            default:
                nameBasedOnSettings = str.ToString();
                break;
        }
        return nameBasedOnSettings;
    }

    public static void setSessionIDForHistoryTable(string eNo, DateTime timeStamp)
    {
        UtilityFunctions.sessionID = $"{eNo}-{timeStamp.ToString()}";
    }

    internal static string getSessionIDForHistoryTable() => UtilityFunctions.sessionID;

    public static async Task CheckAndSendPendingSupportForm(ILoggingService loggingService)
    {
        IUserSession userSession = Mvx.IoCProvider.Resolve<IUserSession>();
        IPlatformSpecificServiceLocator locator = Mvx.IoCProvider.Resolve<IPlatformSpecificServiceLocator>();
        Settings settings = CoreApp.settings.GetItem("SupportFormSendStatus");
        if (settings == null || !(settings.Value == "NOT_SENT"))
        {
            userSession = (IUserSession)null;
            locator = (IPlatformSpecificServiceLocator)null;
            settings = (Settings)null;
        }
        else
        {
            if (await locator.GetPlatformSpecificService().IsNetworkAvailable())
            {
                CoreApp.settings.UpdateItem(new Settings("SupportFormSendStatus", "IN_PROGRESS"));
                loggingService.getLogger().LogAppError(LoggingContext.BACKEND, "sendPendingSupportForm() - SetSupportFormSendStatus: IN_PROGRESS", memberName: nameof(CheckAndSendPendingSupportForm), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/UtilityFunctions.cs", sourceLineNumber: 237);
                userSession.sendPendingSupportForm();
            }
            userSession = (IUserSession)null;
            locator = (IPlatformSpecificServiceLocator)null;
            settings = (Settings)null;
        }
    }

    public static bool IsTokenValid()
    {
        return (DateTime.Now - Convert.ToDateTime(CoreApp.settings.GetItem("JwtTokenTimestamp").Value)).TotalHours <= 12.0;
    }

    public static bool BridgeSettingExists()
    {
        try
        {
            return CoreApp.settings.GetItem("BridgeOff").Value != string.Empty;
        }
        catch (Exception ex)
        {
            string message = ex.Message;
            CoreApp.settings.SaveItem(new Settings("BridgeOff", "true"));
            return true;
        }
    }

    public static void handleURLSchemeNavigation(
      string selectedOption,
      ISecureStorageService secureStorageService)
    {
        IMvxNavigationService navigationService = Mvx.IoCProvider.Resolve<IMvxNavigationService>();
        IUserSession userSession = Mvx.IoCProvider.Resolve<IUserSession>();
        if (navigationService == null)
            return;
        if (userSession.getIsUserLoggedIn() && UtilityFunctions.IsTokenValid())
        {
            if (UtilityFunctions.ShouldUpdateMetadata())
            {
                navigationService.ChangePresentation((MvxPresentationHint)new MvxPagePresentationHint(typeof(StatusViewModel)), new CancellationToken());
            }
            else
            {
                switch (selectedOption)
                {
                    case "eNumberSearch":
                        navigationService.Navigate<TabViewModel>((IMvxBundle)null, new CancellationToken());
                        break;
                    case "prepareWork":
                        navigationService.Navigate<WorkPreparationViewModel>((IMvxBundle)null, new CancellationToken());
                        break;
                }
            }
        }
        else
            navigationService.ChangePresentation((MvxPresentationHint)new MvxPagePresentationHint(typeof(LoginViewModel)), new CancellationToken());
    }

    public static void StoreURLSchemePrepareWorkEnumbers(string eNumbers)
    {
        List<string> list = ((IEnumerable<string>)eNumbers.Split(',')).ToList<string>();
        Mvx.IoCProvider.Resolve<IUserSession>().SetURLSchemePrepareWorkEnumbers(list);
    }

    public static void StoreURLSchemeEnumber(string eNumber)
    {
        Mvx.IoCProvider.Resolve<IUserSession>().SetURLSchemeEnumber(eNumber);
    }

    public static bool ShouldUpdateMetadata()
    {
        Settings settings = CoreApp.settings.GetItem("lastCheck");
        Mvx.IoCProvider.Resolve<IUserSession>();
        return (settings == null || !(settings.Value != "") || !(settings.Value == DateTime.Now.ToString((IFormatProvider)CultureInfo.InvariantCulture))) && (settings == null || !(settings.Value != ""));
    }

    public static void CleanOldLogFiles()
    {
        foreach (FileSystemInfo fileSystemInfo in ((IEnumerable<FileInfo>)UtilityFunctions.GetDirectory().GetFiles("is5Log*")).ToArray<FileInfo>())
            File.Delete(fileSystemInfo.FullName);
    }

    public static StringBuilder SetStringBuilderForHistoryEntry(
      HaInfoDto ha,
      ObservableCollection<HaInfoItems> FWversion,
      string bootModeResponseText)
    {
        StringBuilder stringBuilder = new StringBuilder();
        FWversion.Add(new HaInfoItems()
        {
            Name = AppResource.SMM_CONN_BRAND,
            Data = ha.Brand.ToUpper()
        });
        stringBuilder.AppendLine($"{AppResource.SMM_CONN_BRAND}: {ha.Brand.ToUpper()}");
        string nameBasedOnSettings = UtilityFunctions.GetCountryNameBasedOnSettings(ha.CountrySettings);
        FWversion.Add(new HaInfoItems()
        {
            Name = AppResource.SMM_CONN_COUNTRY_SETTINGS,
            Data = nameBasedOnSettings
        });
        stringBuilder.AppendLine($"{AppResource.SMM_CONN_COUNTRY_SETTINGS}: {nameBasedOnSettings}");
        string str = $"{ha.Vib}/{ha.CustomerIndex}";
        FWversion.Add(new HaInfoItems()
        {
            Name = AppResource.SMM_CONN_E_NUMBER,
            Data = str
        });
        stringBuilder.AppendLine($"{AppResource.SMM_CONN_E_NUMBER}: {str}");
        FWversion.Add(new HaInfoItems()
        {
            Name = AppResource.SMM_CONN_DEVICE_TYPE,
            Data = ha.DeviceType
        });
        stringBuilder.AppendLine($"{AppResource.SMM_CONN_DEVICE_TYPE}: {ha.DeviceType}");
        string timeStampForHaInfo = UtilityFunctions.getTimeStampForHAInfo(ha.ManufacturingTimeStamp);
        FWversion.Add(new HaInfoItems()
        {
            Name = AppResource.SMM_CONN_MANUF_TIME,
            Data = timeStampForHaInfo
        });
        stringBuilder.AppendLine($"{AppResource.SMM_CONN_MANUF_TIME}: {timeStampForHaInfo}");
        FWversion.Add(new HaInfoItems()
        {
            Name = AppResource.SMM_CONN_BOOT_MODE,
            Data = bootModeResponseText.ToUpper()
        });
        stringBuilder.Append($"{AppResource.SMM_CONN_BOOT_MODE}: {bootModeResponseText.ToUpper()}");
        return stringBuilder;
    }

    public static string getTimeStampForHAInfo(string originalTimeStamp)
    {
        string timeStampForHaInfo;
        try
        {
            DateTime exact = DateTime.ParseExact(originalTimeStamp, "yyyy'-'MM'-'dd'T'HH':'mm':'ss", (IFormatProvider)null);
            timeStampForHaInfo = $"{exact.ToString(UtilityFunctions.GetCurrentCultureDateFormat())} {exact.ToString(UtilityFunctions.GetCurrentCultureTimeFormat())}";
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            timeStampForHaInfo = "Unknown";
        }
        return timeStampForHaInfo;
    }

    public static UtilityFunctions.HistoryStringBuilders SetHistoryStringBuilders(
      UtilityFunctions.HistoryStringBuilders hsbs,
      List<History> historyDetailsList)
    {
        foreach (History historyDetails in historyDetailsList)
        {
            string infoType1 = historyDetails.infoType;
            HistoryDBInfoType historyDbInfoType = HistoryDBInfoType.HAInfo;
            string str1 = historyDbInfoType.ToString();
            if (infoType1 == str1)
            {
                if (hsbs.haInfoSB == null)
                    hsbs.haInfoSB = new StringBuilder();
                else
                    hsbs.haInfoSB.AppendLine("\n");
                hsbs.haInfoSB.Append(AppResource.FUNCTION_USED_ON);
                hsbs.haInfoSB.AppendLine(" " + historyDetails.timestamp.ToString());
                hsbs.haInfoSB.Append(historyDetails.historyData);
            }
            else
            {
                string infoType2 = historyDetails.infoType;
                historyDbInfoType = HistoryDBInfoType.ErrorLog;
                string str2 = historyDbInfoType.ToString();
                if (infoType2 == str2)
                {
                    if (hsbs.errorLogSB == null)
                        hsbs.errorLogSB = new StringBuilder();
                    else
                        hsbs.errorLogSB.AppendLine("\n");
                    hsbs.errorLogSB.Append(AppResource.FUNCTION_USED_ON);
                    hsbs.errorLogSB.AppendLine(" " + historyDetails.timestamp.ToString());
                    hsbs.errorLogSB.Append(historyDetails.historyData);
                }
                else
                {
                    string infoType3 = historyDetails.infoType;
                    historyDbInfoType = HistoryDBInfoType.MemoryLog;
                    string str3 = historyDbInfoType.ToString();
                    if (infoType3 == str3)
                    {
                        if (hsbs.memoryLogSB == null)
                            hsbs.memoryLogSB = new StringBuilder();
                        else
                            hsbs.memoryLogSB.AppendLine("\n");
                        hsbs.memoryLogSB.Append(AppResource.FUNCTION_USED_ON);
                        hsbs.memoryLogSB.AppendLine(" " + historyDetails.timestamp.ToString());
                        hsbs.memoryLogSB.Append(historyDetails.historyData);
                    }
                    else
                    {
                        string infoType4 = historyDetails.infoType;
                        historyDbInfoType = HistoryDBInfoType.ProgramLogBefore;
                        string str4 = historyDbInfoType.ToString();
                        if (infoType4 == str4)
                        {
                            if (hsbs.programLogSB == null)
                                hsbs.programLogSB = new StringBuilder();
                            else
                                hsbs.programLogSB.AppendLine("\n");
                            hsbs.programLogSB.Append(AppResource.FUNCTION_USED_ON);
                            hsbs.programLogSB.AppendLine(" " + historyDetails.timestamp.ToString());
                            hsbs.programLogSB.Append(historyDetails.historyData);
                        }
                        else
                        {
                            string infoType5 = historyDetails.infoType;
                            historyDbInfoType = HistoryDBInfoType.ProgramLogAfter;
                            string str5 = historyDbInfoType.ToString();
                            if (infoType5 == str5)
                            {
                                if (hsbs.programLogSB == null)
                                    hsbs.programLogSB = new StringBuilder();
                                hsbs.programLogSB.AppendLine("\n");
                                hsbs.programLogSB.Append(historyDetails.historyData);
                            }
                            else
                            {
                                string infoType6 = historyDetails.infoType;
                                historyDbInfoType = HistoryDBInfoType.FlashLogBefore;
                                string str6 = historyDbInfoType.ToString();
                                if (infoType6 == str6)
                                {
                                    if (hsbs.flashLogSB == null)
                                        hsbs.flashLogSB = new StringBuilder();
                                    else
                                        hsbs.flashLogSB.AppendLine("\n");
                                    hsbs.flashLogSB.Append(AppResource.FUNCTION_USED_ON);
                                    hsbs.flashLogSB.AppendLine(" " + historyDetails.timestamp.ToString());
                                    hsbs.flashLogSB.Append(historyDetails.historyData);
                                }
                                else
                                {
                                    string infoType7 = historyDetails.infoType;
                                    historyDbInfoType = HistoryDBInfoType.FlashLogAfter;
                                    string str7 = historyDbInfoType.ToString();
                                    if (infoType7 == str7)
                                    {
                                        if (hsbs.flashLogSB == null)
                                            hsbs.flashLogSB = new StringBuilder();
                                        hsbs.flashLogSB.AppendLine("\n");
                                        hsbs.flashLogSB.Append(historyDetails.historyData);
                                    }
                                    else
                                    {
                                        string infoType8 = historyDetails.infoType;
                                        historyDbInfoType = HistoryDBInfoType.MeasureLog;
                                        string str8 = historyDbInfoType.ToString();
                                        if (infoType8 == str8)
                                        {
                                            if (hsbs.measureLogSB == null)
                                                hsbs.measureLogSB = new StringBuilder();
                                            hsbs.measureLogSB.Append(AppResource.FUNCTION_USED_ON);
                                            hsbs.measureLogSB.AppendLine(" " + historyDetails.timestamp.ToString());
                                            hsbs.measureLogSB.Append(historyDetails.historyData);
                                        }
                                        else
                                        {
                                            string infoType9 = historyDetails.infoType;
                                            historyDbInfoType = HistoryDBInfoType.MonitoringLog;
                                            string str9 = historyDbInfoType.ToString();
                                            if (infoType9 == str9)
                                            {
                                                if (hsbs.monitoringLogSB == null)
                                                    hsbs.monitoringLogSB = new StringBuilder();
                                                hsbs.monitoringLogSB.Append(AppResource.FUNCTION_USED_ON);
                                                hsbs.monitoringLogSB.AppendLine(" " + historyDetails.timestamp.ToString());
                                                hsbs.monitoringLogSB.Append(historyDetails.historyData);
                                            }
                                            else
                                            {
                                                string infoType10 = historyDetails.infoType;
                                                historyDbInfoType = HistoryDBInfoType.CodingLog;
                                                string str10 = historyDbInfoType.ToString();
                                                if (infoType10 == str10)
                                                {
                                                    if (hsbs.codingLogSB == null)
                                                        hsbs.codingLogSB = new StringBuilder();
                                                    hsbs.codingLogSB.Append(AppResource.FUNCTION_USED_ON);
                                                    hsbs.codingLogSB.AppendLine(" " + historyDetails.timestamp.ToString());
                                                    hsbs.codingLogSB.Append(historyDetails.historyData);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        return hsbs;
    }

    public static string GetUserCountryCode(ISecureStorageService secureStorageService)
    {
        try
        {
            return UtilityFunctions.GetUserDetails(secureStorageService)["userCountryCode"].ToString();
        }
        catch (Exception ex)
        {
            Mvx.IoCProvider.Resolve<ILoggingService>().getLogger().LogAppError(LoggingContext.BACKEND, "Exception occured while fetching user country code:" + ex?.ToString(), memberName: nameof(GetUserCountryCode), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/UtilityFunctions.cs", sourceLineNumber: 538);
        }
        return "";
    }

    public static string GetUser(
      ISecureStorageService secureStorageService,
      ILoggingService loggingService)
    {
        try
        {
            return UtilityFunctions.GetUserDetails(secureStorageService)["username"].ToString();
        }
        catch (Exception ex)
        {
            loggingService.getLogger().LogAppError(LoggingContext.BACKEND, "Exception occured while fetching user name:" + ex?.ToString(), memberName: nameof(GetUser), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/UtilityFunctions.cs", sourceLineNumber: 551);
        }
        return "";
    }

    private static Dictionary<string, object> GetUserDetails(
      ISecureStorageService secureStorageService)
    {
        return JsonConvert.DeserializeObject<Dictionary<string, object>>(new JwtSecurityTokenHandler().ReadJwtToken(secureStorageService.getJWTToken().Result).Claims.FirstOrDefault<Claim>((Func<Claim, bool>)(c => c.Type == "X-UserDetail"))?.Value);
    }

    public static int RandomIntFromRNG(int min, int max)
    {
        RNGCryptoServiceProvider cryptoServiceProvider = new RNGCryptoServiceProvider();
        byte[] data = new byte[4];
        cryptoServiceProvider.GetBytes(data);
        uint uint32 = BitConverter.ToUInt32(data, 0);
        return (int)((double)min + (double)(max - min) * ((double)uint32 / 4294967296.0));
    }

    public static bool CheckCompressedFile(ZipArchive archive)
    {
        int num1 = int.Parse(BuildProperties.extractionThresholdEntries);
        int num2 = int.Parse(BuildProperties.extractionThresholdSize);
        double num3 = double.Parse(BuildProperties.extractionThresholdRatio);
        int num4 = 0;
        int num5 = 0;
        foreach (ZipArchiveEntry entry in archive.Entries)
        {
            ++num5;
            using (Stream stream = entry.Open())
            {
                byte[] buffer = new byte[1024 /*0x0400*/];
                int num6 = 0;
                int num7;
                do
                {
                    num7 = stream.Read(buffer, 0, 1024 /*0x0400*/);
                    num6 += num7;
                    num4 += num7;
                    if ((double)num6 / (double)entry.CompressedLength > num3)
                        return false;
                }
                while (num7 > 0);
            }
            if (num4 > num2 || num5 > num1)
                return false;
        }
        return true;
    }

    public static bool IsValidJson(string strInput)
    {
        if (string.IsNullOrWhiteSpace(strInput))
            return false;
        strInput = strInput.Trim();
        if ((!strInput.StartsWith("{") || !strInput.EndsWith("}")) && (!strInput.StartsWith("[") || !strInput.EndsWith("]")))
            return false;
        try
        {
            JToken.Parse(strInput);
            return true;
        }
        catch (JsonReaderException ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return false;
        }
    }

    public static string GetActualFilePath(string directory, string fileName, string filePath)
    {
        FileInfo[] array = ((IEnumerable<FileInfo>)new DirectoryInfo(directory).GetFiles()).Where<FileInfo>((Func<FileInfo, bool>)(item => item.FullName.ToLowerInvariant() == filePath.ToLowerInvariant())).ToArray<FileInfo>();
        if (!((IEnumerable<FileInfo>)array).Any<FileInfo>())
            throw new FileNotFoundException("File not found: " + fileName);
        return ((IEnumerable<FileInfo>)array).Count<FileInfo>() <= 1 ? ((IEnumerable<FileInfo>)array).First<FileInfo>().FullName : throw new AmbiguousMatchException("Ambiguous File reference for " + fileName);
    }

    

    public static List<Country> GetSelectedCountryList(IMetadataService _metadataService)
    {
        List<string> listOfCountryCode = Enumerable.OfType<string>(CoreApp.settings.GetItem("CountryCodes").Value.ToString().Split(',')).ToList<string>();
        List<Country> countryList = _metadataService.GetCountryList();
        if (countryList == null || countryList.Count <= 0)
            return (List<Country>)null;
        List<Country> list = countryList.Where<Country>((Func<Country, bool>)(item => listOfCountryCode.Contains(item.country))).ToList<Country>();
        list.ForEach((Action<Country>)(i => i.isSelected = true));
        return list;
    }

    public static DownloadFilesStatistics GetDownloadStatistics(
      bool isCountryRelevant,
      string selectedDeviceClass,
      bool isFileSizeSwitchToggled,
      ISecureStorageService secureStorageService,
      List<Country> country = null)
    {
        IMetadataService metadataService = Mvx.IoCProvider.Resolve<IMetadataService>();
        DownloadFilesStatistics downloadStatistics = new DownloadFilesStatistics();
        if (((country == null ? 0 : (country.Count > 0 ? 1 : 0)) & (isCountryRelevant ? 1 : 0)) != 0 && country.Where<Country>((Func<Country, bool>)(item => item.isSelected)).ToList<Country>().Count == 0)
            return downloadStatistics;
        Dictionary<DeviceClass, List<MaterialStatistics>> relevantMaterials = UtilityFunctions.GetRelevantMaterials(isCountryRelevant, selectedDeviceClass, isFileSizeSwitchToggled, secureStorageService, country);
        DeviceClass key = EnumHelper.FromString<DeviceClass>(selectedDeviceClass);
        if (relevantMaterials.ContainsKey(key) || key == DeviceClass.ALL)
            downloadStatistics = metadataService.GetDownloadFilesStatisticsForFullDownloadAndCountryRelevant(selectedDeviceClass, relevantMaterials);
        return downloadStatistics;
    }

    public static Dictionary<DeviceClass, List<MaterialStatistics>> GetRelevantMaterials(
      bool isCountryRelevant,
      string selectedDeviceClass,
      bool isFileSizeSwitchToggled,
      ISecureStorageService secureStorageService,
      List<Country> country = null)
    {
        ILoggingService loggingService = Mvx.IoCProvider.Resolve<ILoggingService>();
        List<MaterialStatistics> materialStatistics = Mvx.IoCProvider.Resolve<IUserSession>().GetMaterialStatistics();
        List<MaterialStatistics> materialStatisticsList1 = new List<MaterialStatistics>();
        List<MaterialStatistics> materialStatisticsList2 = new List<MaterialStatistics>();
        List<MaterialStatistics> materialStatisticsList3 = new List<MaterialStatistics>();
        string str = DeviceClass.ALL.ToString();
        string smm = DeviceClass.SMM.ToString();
        string nonsmm = DeviceClass.NON_SMM.ToString();
        try
        {
            if (selectedDeviceClass.Equals(str) || selectedDeviceClass.Equals(smm) || selectedDeviceClass.Equals(nonsmm))
            {
                List<MaterialStatistics> source;
                if (isCountryRelevant)
                {
                    List<string> listOfCountryCode = new List<string>();
                    if (country != null && country.Count > 0)
                    {
                        foreach (Country country1 in country)
                        {
                            if (country1.isSelected)
                                listOfCountryCode.Add(country1.country);
                        }
                    }
                    else
                    {
                        string upperInvariant = UtilityFunctions.GetUserCountryCode(secureStorageService).ToUpperInvariant();
                        listOfCountryCode.Add(upperInvariant);
                    }
                    source = isFileSizeSwitchToggled ? materialStatistics.Where<MaterialStatistics>((Func<MaterialStatistics, bool>)(item => listOfCountryCode.Intersect<string>((IEnumerable<string>)((IEnumerable<string>)item.country.Split(',')).ToList<string>()).Count<string>() > 0)).ToList<MaterialStatistics>() : materialStatistics.Where<MaterialStatistics>((Func<MaterialStatistics, bool>)(item => listOfCountryCode.Intersect<string>((IEnumerable<string>)((IEnumerable<string>)item.country.Split(',')).ToList<string>()).Count<string>() > 0 && item.fileSize <= (long)UtilityFunctions.GetBinarySizeThreshold())).ToList<MaterialStatistics>();
                }
                else
                    source = isFileSizeSwitchToggled ? materialStatistics : materialStatistics.Where<MaterialStatistics>((Func<MaterialStatistics, bool>)(item => item.fileSize <= (long)UtilityFunctions.GetBinarySizeThreshold())).ToList<MaterialStatistics>();
                if (selectedDeviceClass.Equals(str))
                {
                    materialStatisticsList2 = source.Where<MaterialStatistics>((Func<MaterialStatistics, bool>)(item => item.deviceClass.StartsWith(smm))).ToList<MaterialStatistics>();
                    materialStatisticsList3 = source.Where<MaterialStatistics>((Func<MaterialStatistics, bool>)(item => item.deviceClass.StartsWith(nonsmm))).ToList<MaterialStatistics>();
                }
                else if (selectedDeviceClass.Equals(smm))
                    materialStatisticsList2 = source.Where<MaterialStatistics>((Func<MaterialStatistics, bool>)(item => item.deviceClass.StartsWith(smm))).ToList<MaterialStatistics>();
                else
                    materialStatisticsList3 = source.Where<MaterialStatistics>((Func<MaterialStatistics, bool>)(item => item.deviceClass.StartsWith(nonsmm))).ToList<MaterialStatistics>();
            }
            else
                loggingService.getLogger().LogAppError(LoggingContext.BINARY, "Device Class didnt match with any predefined items.", memberName: nameof(GetRelevantMaterials), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/UtilityFunctions.cs", sourceLineNumber: 810);
        }
        catch (Exception ex)
        {
            loggingService.getLogger().LogAppError(LoggingContext.LOCAL, ex.Message, memberName: nameof(GetRelevantMaterials), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/UtilityFunctions.cs", sourceLineNumber: 815);
        }
        return new Dictionary<DeviceClass, List<MaterialStatistics>>()
        {
            [DeviceClass.SMM] = materialStatisticsList2,
            [DeviceClass.NON_SMM] = materialStatisticsList3
        };
    }

    public static int GetBinarySizeThreshold()
    {
        ILoggingService loggingService = Mvx.IoCProvider.Resolve<ILoggingService>();
        int result = 0;
        if (int.TryParse(BuildProperties.BinarySizeThreshold, out result))
            result = result * 1024 /*0x0400*/ * 1024 /*0x0400*/;
        else
            loggingService.getLogger().LogAppError(LoggingContext.BINARY, "BinarySizeThreshold couldnot be retreived.", memberName: nameof(GetBinarySizeThreshold), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/UtilityFunctions.cs", sourceLineNumber: 835);
        return result;
    }

    public static bool StorePpfFiles(
      DownloadProxy proxy,
      IUserSession userSession,
      ILoggingService loggingService,
      IMetadataService metadataService)
    {
        try
        {
            int num1 = 0;
            int num2 = 0;
            PpfsInfo ppfData = proxy.PPFData;
            foreach (PPF ppF in ppfData.PPFs)
            {
                if (!string.IsNullOrEmpty(ppF.ppf))
                {
                    long moduleid = ppF.moduleid;
                    string moduleVersion = ppF.moduleVersion;
                    metadataService.UpdatePpfTable(proxy.vib, proxy.ki, moduleid, moduleVersion, ppF.validity, ppF.version, ppF.CA, ppF.ppf);
                }
                else
                {
                    loggingService.getLogger().LogAppError(LoggingContext.LOCAL, $"PPF entry at index {num1} was empty", memberName: nameof(StorePpfFiles), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/UtilityFunctions.cs", sourceLineNumber: 858);
                    ++num2;
                }
                ++num1;
            }
            if (num2 == ppfData.PPFs.Count)
                throw new MissingFieldException("All ppf values were empty.");
            string str = $"{proxy.vib}/{proxy.ki}";
            userSession.updatePPFRefetchStatus(str, true);
            metadataService.updatePPFRefetchStatus(str, true);
            proxy.PPFData = (PpfsInfo)null;
            return true;
        }
        catch (Exception ex)
        {
            loggingService.getLogger().LogAppError(LoggingContext.LOCAL, ex.Message, memberName: nameof(StorePpfFiles), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/UtilityFunctions.cs", sourceLineNumber: 878);
            return false;
        }
    }

    public static Tuple<string, string> getVibAndKi(string enumber)
    {
        string str1 = enumber;
        string str2 = "";
        if (enumber.Contains("/"))
        {
            str1 = enumber.Substring(0, enumber.IndexOf("/"));
            str2 = enumber.Substring(enumber.IndexOf("/") + 1);
        }
        return new Tuple<string, string>(str1, str2);
    }

    public static List<string> getDownloadedEnumbersFromDB(IMetadataService _metadataService)
    {
        return _metadataService.GetDownloadedEnumbersFromPPF();
    }

    public static string GetProperSpauNaming(
      string _enumber,
      long firmwareId,
      IMetadataService _metadataService,
      IApplianceSession _session,
      ILoggingService _loggingService)
    {
        Tuple<string, string> vibAndKi = UtilityFunctions.getVibAndKi(_enumber);
        List<smm_module> smmModulesRecords = _metadataService.getSMMModulesRecords(vibAndKi.Item1, vibAndKi.Item2, firmwareId);
        List<smm_module> list1 = smmModulesRecords.Where<smm_module>((Func<smm_module, bool>)(x => x.node != 0L && x.node != (long)ushort.MaxValue)).ToList<smm_module>();
        long _node = 0;
        int count1 = list1.Count;
        if (count1 > 0)
        {
            _node = list1[0].node;
            if (count1 > 1)
                _loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSMM, $"More than one node was found for module {firmwareId} and appliance {_enumber}. Picked first node {list1[0].node} to fetch device name.", memberName: nameof(GetProperSpauNaming), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/UtilityFunctions.cs", sourceLineNumber: 912);
        }
        else
        {
            List<smm_module> list2 = smmModulesRecords.Where<smm_module>((Func<smm_module, bool>)(x => x.type == ModuleType.SPAU_FIRMWARE.ToString())).ToList<smm_module>();
            int count2 = list2.Count;
            if (count2 > 0)
            {
                _node = list2[0].node;
                if (count2 > 1)
                    _loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSMM, $"More than one node was found for SPAU module {firmwareId} and appliance {_enumber}. Picked first node {list1[0].node} to fetch device name.", memberName: nameof(GetProperSpauNaming), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/UtilityFunctions.cs", sourceLineNumber: 924);
            }
        }
        string key = _session.DeviceDictionary.FirstOrDefault<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>)(d => d.Value.Equals(_session.HaInfoFromMetadata.DeviceType))).Key;
        if (key != null)
            return _metadataService.getNodeName(_node, key);
        _loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSMM, "Devicetype does not exist in inline dictionary", memberName: nameof(GetProperSpauNaming), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/UtilityFunctions.cs", sourceLineNumber: 933);
        return "Unknown";
    }

    public static void ContinueWithPpfExtraction(
      DownloadProxy proxy,
      DownloadStatus status,
      IUserSession userSession,
      ILoggingService _loggingService,
      IPlatformSpecificServiceLocator _locator,
      IMetadataService _metadataService)
    {
        if (status != DownloadStatus.COMPLETED)
            _loggingService.getLogger().LogAppDebug(LoggingContext.LOCAL, "PPF information failed to get downloaded.", memberName: nameof(ContinueWithPpfExtraction), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/UtilityFunctions.cs", sourceLineNumber: 943);
        else if (!UtilityFunctions.StorePpfFiles(proxy, userSession, _loggingService, _metadataService))
            _loggingService.getLogger().LogAppDebug(LoggingContext.LOCAL, "PPF files failed to get extracted from PPF.", memberName: nameof(ContinueWithPpfExtraction), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/UtilityFunctions.cs", sourceLineNumber: 949);
    }

    public static bool getSShCommandsStatus(SshResponse response, string commandName)
    {
        string rawResponse = response.RawResponse;
        string str1;
        if (rawResponse == null)
            str1 = (string)null;
        else
            str1 = rawResponse.ToLower().Trim('\t', ' ', '\r', '\n');
        string str2 = str1;
        bool sshCommandsStatus = false;
        if (str2 != null && response.Success && str2.Contains(commandName))
            sshCommandsStatus = true;
        return sshCommandsStatus;
    }

    internal static bool ISAvailableWithInRange(iService5.Core.Services.Data.Version availableVersion, List<iService5.Core.Services.Data.Version> versionRange)
    {
        bool flag = false;
        switch (versionRange.Count)
        {
            case 1:
                iService5.Core.Services.Data.Version version1 = versionRange[0];
                if (availableVersion >= version1)
                {
                    flag = true;
                    break;
                }
                break;
            case 2:
                iService5.Core.Services.Data.Version version2 = versionRange[0];
                iService5.Core.Services.Data.Version version3 = versionRange[1];
                if (availableVersion >= version2 && availableVersion <= version3)
                    flag = true;
                break;
        }
        return flag;
    }

    public static async Task ShowBridgeModeWarningPopup()
    {
        AlertService _alertService = (AlertService)Mvx.IoCProvider.Resolve<IAlertService>();
        if (UtilityFunctions.IsLocalNetworkPermissionNeeded() && CoreApp.settings.GetItem("localNetworkPopupAppeared").Value == "true")
        {
            await _alertService.ShowMessageAlertBoxWithKey("SSH_COMMAND_ERROR_IOS_14", AppResource.INFORMATION_TEXT);
            _alertService = (AlertService)null;
        }
        else
        {
            await _alertService.ShowMessageAlertBoxWithKey("SSH_COMMAND_ERROR", AppResource.INFORMATION_TEXT);
            _alertService = (AlertService)null;
        }
    }

    public static string GetDeviceType() => DeviceInfo.DeviceType.ToString();

    public static string GetDeviceName() => DeviceInfo.Name.ToString();

    public static string CreateFilePath(string _filename)
    {
        return Path.Combine(Mvx.IoCProvider.Resolve<IPlatformSpecificServiceLocator>().GetPlatformSpecificService().getFolder(), _filename);
    }

    public static string loadLocalJsonFile(string fileName)
    {
        using (StreamReader streamReader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream($"{Constants.iService5DirectoryPath}{fileName}.json")))
            return streamReader.ReadToEnd();
    }

    public static DirectoryInfo GetDirectory()
    {
        return new DirectoryInfo(Mvx.IoCProvider.Resolve<IPlatformSpecificServiceLocator>().GetPlatformSpecificService().getFolder());
    }

    public static string GetPresentWorkingDir()
    {
        return Path.Combine(Mvx.IoCProvider.Resolve<IPlatformSpecificServiceLocator>().GetPlatformSpecificService().getFolder());
    }

    public static bool fileExists(string filename)
    {
        return ((IEnumerable<FileInfo>)UtilityFunctions.GetDirectory().GetFiles(filename)).ToArray<FileInfo>().Length != 0;
    }

    public static bool isCertificateInResponse(string response)
    {
        return response.Contains(Constants.certificateInitials);
    }

    public static string getActualCertificateString(string certificateString)
    {
        return new StringBuilder(certificateString).Replace(Constants.certificateInitials, (string)null).Replace(Constants.certificateEnding, (string)null).ToString().Trim();
    }

    public static string[] getAllFilesForExtension(string ext)
    {
        string presentWorkingDir = UtilityFunctions.GetPresentWorkingDir();
        return Directory.Exists(presentWorkingDir) ? Directory.GetFiles(presentWorkingDir, "*" + ext) : (string[])null;
    }

    public static async Task<bool> isCertificateValid(
      ISecureStorageService _secureStorageService,
      ILoggingService _loggingService)
    {
        string existingCertificate = await _secureStorageService.GetSignedCertificate(SecureStorageKeys.CERTIFICATE_CLIENT);
        if (!string.IsNullOrEmpty(existingCertificate))
        {
            try
            {
                int noOfCertificates = Regex.Matches(existingCertificate, Constants.certificateInitials).Count;
                if (noOfCertificates > 1)
                {
                    string[] certificates = existingCertificate.Split(new string[1]
                    {
            Constants.certificateEnding
                    }, StringSplitOptions.None);
                    if (certificates.Length > 2)
                        existingCertificate = UtilityFunctions.getActualCertificateString(certificates[0]);
                    certificates = (string[])null;
                }
                StringBuilder sb = new StringBuilder(existingCertificate);
                sb = sb.Replace(Constants.certificateInitials, (string)null);
                sb = sb.Replace(Constants.certificateEnding, (string)null);
                string eccPem = sb.ToString().Trim();
                X509Certificate2 cert = new X509Certificate2(Convert.FromBase64String(eccPem));
                DateTime certNotAfter = cert.NotAfter;
                return (certNotAfter - DateTime.Now).Days >= Constants.certificateExpiryPeriod;
            }
            catch (Exception ex)
            {
                _loggingService.getLogger().LogAppError(LoggingContext.CSR, "Certificate Update error: " + ex.Message, memberName: nameof(isCertificateValid), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/UtilityFunctions.cs", sourceLineNumber: 1108);
            }
        }
        return false;
    }

    public static string getCommaSeperatedValues<T>(List<T> values)
    {
        return $"({string.Join("','", string.Join(",", values.Select<T, string>((Func<T, string>)(v => $"'{v?.ToString()}'"))))})";
    }

    public static bool isRefetchRequired()
    {
        bool flag = false;
        if (FeatureConfiguration.PpfRefetchEnabled && string.IsNullOrEmpty(CoreApp.settings.GetItem("PpfRefetch").Value))
            flag = true;
        return flag;
    }

    public static string getPathOf(IPlatformSpecificServiceLocator _locator, string folder)
    {
        return Path.Combine($"{_locator.GetPlatformSpecificService().getFolder()}/{folder}");
    }

    public static byte[] ConvertStreamToByteArray(Stream stream)
    {
        byte[] buffer = new byte[stream.Length];
        stream.Read(buffer, 0, buffer.Length);
        return buffer;
    }

    public static bool IsPPFTableMigrationNeeded()
    {
        return Mvx.IoCProvider.Resolve<IMetadataService>().IsPPFTableMigrationNeded();
    }

    public static bool PPFTableMigration()
    {
        return Mvx.IoCProvider.Resolve<IMetadataService>().MigratePPFTable();
    }

    public static async Task<BridgeWifiResponse> EnableBridgeWIFIUsingApi(
      IPlatformSpecificServiceLocator Locator,
      ILoggingService LoggingService)
    {
        IBridgeHttpClient httpClient = (IBridgeHttpClient)new BridgeHttpClient(Locator, LoggingService);
        BridgeWifiResponse request = await httpClient.GetRequest(BridgeRequestType.WIFI_MODE);
        httpClient = (IBridgeHttpClient)null;
        return request;
    }

    public static bool IsLocalNetworkPemrissionError(string msg)
    {
        return msg.Contains("Internet Connection appears to be offline") || msg.Contains("Internet Connection") || msg.Contains("offline");
    }

    public static bool IsLocalNetworkPermissionNeeded()
    {
        if (DeviceInfo.Platform == DevicePlatform.iOS && DeviceInfo.Version.Major >= 18)
            CoreApp.settings.UpdateItem(new Settings("localNetworkPopupAppeared", "true"));
        return DeviceInfo.Platform == DevicePlatform.iOS && DeviceInfo.Version.Major >= 14 && DeviceInfo.Version.Major < 18;
    }

    /// <summary>
    ///  ZMIANA ZMIANA
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>

    internal static List<MaterialStatistics> GetMaterialList()
    {
        try
        {
            // Pobieramy dostęp do serwisu metadanych, który jest już zainicjowany
            var metadataService = Mvx.IoCProvider.Resolve<IMetadataService>();

            // Wywołujemy dwie istniejące w MetadataDB metody, które pobierają statystyki
            List<MaterialStatistics> modulesStatistics = metadataService.GetDownloadFilesStatisticsModules();
            List<MaterialStatistics> documentsStatistics = metadataService.GetDownloadFilesStatisticsDocuments();

            // Łączymy obie listy w jedną
            // Używamy Union, aby uniknąć duplikatów, jeśli jakiś materiał ma zarówno moduły, jak i dokumenty
            List<MaterialStatistics> combinedList = modulesStatistics
                .Union(documentsStatistics)
                .ToList();

            return combinedList;
        }
        catch (Exception ex)
        {
            // Jeśli z jakiegoś powodu pobranie statystyk się nie powiedzie,
            // logujemy błąd i zwracamy pustą listę, aby uniknąć awarii aplikacji.
            Debug.WriteLine($"[BŁĄD KRYTYCZNY] Nie udało się pobrać listy materiałów w GetMaterialList: {ex.Message}");
            return new List<MaterialStatistics>();
        }
    }

//    internal static List<MaterialStatistics> GetMaterialList()
//    {
//        throw new NotImplementedException();
//    }

    public class HistoryStringBuilders
    {
        public StringBuilder haInfoSB { get; set; }

        public StringBuilder errorLogSB { get; set; }

        public StringBuilder programLogSB { get; set; }

        public StringBuilder flashLogSB { get; set; }

        public StringBuilder measureLogSB { get; set; }

        public StringBuilder monitoringLogSB { get; set; }

        public StringBuilder codingLogSB { get; set; }

        public StringBuilder memoryLogSB { get; set; }
    }
}
