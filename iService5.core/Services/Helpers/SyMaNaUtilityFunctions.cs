// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Helpers.SyMaNaUtilityFunctions
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Data;
using iService5.Core.Services.Data.SyMaNa;
using iService5.Core.Services.User.ActivityLogging;
using iService5.Ssh.DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
//using Xamarin.Forms;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

#nullable disable
namespace iService5.Core.Services.Helpers;

public static class SyMaNaUtilityFunctions
{
  public static ObservableCollection<SyMaNaECUInfoData> CreateModuleList(
    SyMaNaECU[] ECUs,
    List<smm_module> modulesFromDB,
    ILoggingService loggingService)
  {
    ObservableCollection<SyMaNaECUInfoData> source = new ObservableCollection<SyMaNaECUInfoData>();
    foreach (smm_module smmModule in modulesFromDB)
    {
      ObservableCollection<SyMaNaECUInfoData> observableCollection = source;
      SyMaNaECUInfoData syMaNaEcuInfoData = new SyMaNaECUInfoData();
      long num = smmModule.moduleid;
      syMaNaEcuInfoData.FirmwareId = num.ToString();
      syMaNaEcuInfoData.InstalledVersion = (iService5.Core.Services.Data.Version) null;
      syMaNaEcuInfoData.InstalledVersionDisplay = AppResource.NOT_INSTALLED_TEXT;
      syMaNaEcuInfoData.AvailableVersion = iService5.Core.Services.Data.Version.FromString(smmModule.version);
      syMaNaEcuInfoData.AvailableVersionDisplay = SyMaNaUtilityFunctions.GetAvailableVersionText(smmModule.version.ToString(), "");
      num = smmModule.node;
      syMaNaEcuInfoData.Node = num.ToString();
      syMaNaEcuInfoData.AvailableVersionTextColor = Color.FromHex("#007AFF");
      observableCollection.Add(syMaNaEcuInfoData);
    }
    foreach (SyMaNaECU ecU in ECUs)
    {
      foreach (FirmwareModule firmwareModule in ecU.firmware)
      {
        string firmwareId = firmwareModule.firmwareId.ToString();
        List<SyMaNaECUInfoData> list = source.Where<SyMaNaECUInfoData>((Func<SyMaNaECUInfoData, bool>) (module => module.FirmwareId == firmwareId)).ToList<SyMaNaECUInfoData>();
        if (list.Count > 0)
        {
          foreach (SyMaNaECUInfoData syMaNaEcuInfoData in list)
          {
            syMaNaEcuInfoData.InstalledVersion = iService5.Core.Services.Data.Version.FromString(firmwareModule.version.ToString());
            syMaNaEcuInfoData.InstalledVersionDisplay = $"{AppResource.INSTALLED_TEXT}: {firmwareModule.version.ToString()}";
            syMaNaEcuInfoData.AvailableVersionDisplay = SyMaNaUtilityFunctions.GetAvailableVersionText(syMaNaEcuInfoData.AvailableVersion.ToString(), syMaNaEcuInfoData.InstalledVersion.ToString());
            syMaNaEcuInfoData.AvailableVersionTextColor = SyMaNaUtilityFunctions.GetAvailableVersionTextColor(syMaNaEcuInfoData.AvailableVersion.ToString(), syMaNaEcuInfoData.InstalledVersion.ToString());
          }
        }
        else
          source.Add(new SyMaNaECUInfoData()
          {
            FirmwareId = firmwareId,
            InstalledVersion = iService5.Core.Services.Data.Version.FromString(firmwareModule.version.ToString()),
            InstalledVersionDisplay = $"{AppResource.INSTALLED_TEXT}: {firmwareModule.version.ToString()}",
            AvailableVersion = (iService5.Core.Services.Data.Version) null,
            Node = SyMaNaUtilityFunctions.GetNodeText(firmwareId, modulesFromDB),
            AvailableVersionTextColor = Color.FromHex("#000000")
          });
      }
    }
    return source;
  }

  private static string GetNodeText(string firmwareId, List<smm_module> modulesFromDB)
  {
    smm_module smmModule = modulesFromDB.Find((Predicate<smm_module>) (x => x.moduleid.ToString() == firmwareId));
    return smmModule != null ? smmModule.node.ToString() : "";
  }

  private static string GetAvailableVersionText(string availableVersion, string installedVersion)
  {
    iService5.Core.Services.Data.Version cmp = iService5.Core.Services.Data.Version.FromString(installedVersion);
    iService5.Core.Services.Data.Version version = iService5.Core.Services.Data.Version.FromString(availableVersion);
    if (version == NullVersion.Instance)
      return "";
    return version.IsVersionEqual(cmp) ? AppResource.UP_TO_DATE_TEXT : $"{AppResource.AVAILABLE_TEXT} {availableVersion}";
  }

  public static Color GetAvailableVersionTextColor(string availableVersion, string installedVersion)
  {
    iService5.Core.Services.Data.Version cmp = iService5.Core.Services.Data.Version.FromString(installedVersion);
    iService5.Core.Services.Data.Version version = iService5.Core.Services.Data.Version.FromString(availableVersion);
    return version.IsVersionEqual(cmp) ? Colors.Black : (version > cmp ? Color.FromHex("#007AFF") : Colors.Gray);
  }

  public static List<ppf> GetPPFDataForEnumber(string vibAndKi, IMetadataService metadataService)
  {
    return metadataService.GetAllEnumbersDownloadedPpfs(vibAndKi, true);
  }

  public static List<SyMaNaFirmwareUploadModel> GetUploadFirmwareModels(
    List<smm_module> modulesFromDB,
    ILoggingService loggingService,
    IMetadataService metaDataService)
  {
    List<SyMaNaFirmwareUploadModel> uploadFirmwareModels = new List<SyMaNaFirmwareUploadModel>();
    foreach (smm_module smmModule in modulesFromDB)
    {
      string path2 = $"{smmModule.moduleid.ToString()}.{smmModule.version}.tar.gz";
      try
      {
        string str = Path.Combine(UtilityFunctions.GetPresentWorkingDir(), path2);
        long fileSize = SyMaNaUtilityFunctions.GetFileSize(metaDataService, smmModule, str);
        if (File.Exists(str))
        {
          uploadFirmwareModels.Add(new SyMaNaFirmwareUploadModel()
          {
            path = str,
            FileName = path2,
            module = smmModule.moduleid,
            version = smmModule.version,
            fileSize = fileSize
          });
        }
        else
        {
          loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSYMANA, "File does not exists: " + path2, memberName: nameof (GetUploadFirmwareModels), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/SyMaNaUtilityFunctions.cs", sourceLineNumber: 148);
          break;
        }
      }
      catch (Exception ex)
      {
        loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSYMANA, "Error in reading file : " + path2, memberName: nameof (GetUploadFirmwareModels), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/SyMaNaUtilityFunctions.cs", sourceLineNumber: 154);
        loggingService.getLogger().LogAppInformation(LoggingContext.PROGRAMSYMANA, ex.Message, memberName: nameof (GetUploadFirmwareModels), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/SyMaNaUtilityFunctions.cs", sourceLineNumber: 155);
        break;
      }
    }
    return uploadFirmwareModels;
  }

  private static long GetFileSize(
    IMetadataService metaDataService,
    smm_module smmModule,
    string filePath)
  {
    List<module> moduleSpecs = metaDataService.GetModuleSpecs(smmModule);
    return moduleSpecs != null && moduleSpecs.Count > 0 ? moduleSpecs.First<module>().fileSize : new FileInfo(filePath).Length;
  }

  public static bool SetPPFDataForAllTheModules(
    List<SyMaNaFirmwareUploadModel> uploadFirmwareModels,
    Tuple<string, string> vibAndKi,
    List<ppf> ppfsListForEno,
    ILoggingService loggingService)
  {
    bool flag = true;
    try
    {
      foreach (SyMaNaFirmwareUploadModel uploadFirmwareModel in uploadFirmwareModels)
      {
        SyMaNaFirmwareUploadModel module = uploadFirmwareModel;
        if (string.IsNullOrEmpty(module.ppfFile))
        {
          loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, $"PPF Details : {vibAndKi.Item1} {vibAndKi.Item2} {module.module.ToString()} {module.version}", memberName: nameof (SetPPFDataForAllTheModules), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/SyMaNaUtilityFunctions.cs", sourceLineNumber: 185);
          List<ppf> list = ppfsListForEno.Where<ppf>((Func<ppf, bool>) (ppf => ppf.moduleid == module.module && ppf.version == module.version)).ToList<ppf>();
          if (list.Count > 0)
          {
            ppf ppf = list.First<ppf>();
            string str = $"{ppf.vib}_{ppf.ki}_{ppf.moduleid.ToString()}_{ppf.version}";
            if (string.IsNullOrEmpty(ppf.ppffile) || string.IsNullOrEmpty(str))
            {
              flag = false;
              SyMaNaUtilityFunctions.PrintLogs(ppf, loggingService);
              break;
            }
            module.ppfFile = ppf.ppffile;
          }
          else
          {
            flag = false;
            loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, $"NO data found for PPF: {vibAndKi.Item1}_{vibAndKi.Item2}_{module.module.ToString()}_{module.version}", memberName: nameof (SetPPFDataForAllTheModules), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/SyMaNaUtilityFunctions.cs", sourceLineNumber: 209);
            break;
          }
        }
      }
    }
    catch (Exception ex)
    {
      flag = false;
      loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Failed to find PPF data: " + ex.Message, memberName: nameof (SetPPFDataForAllTheModules), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/SyMaNaUtilityFunctions.cs", sourceLineNumber: 218);
    }
    return flag;
  }

  private static void PrintLogs(ppf ppf, ILoggingService loggingService)
  {
    string str = $"{ppf.vib}_{ppf.ki}_{ppf.moduleid.ToString()}_{ppf.version}";
    if (ppf.ppffile == null && str == null)
      loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "PPFID and PPFFile both are null", memberName: nameof (PrintLogs), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/SyMaNaUtilityFunctions.cs", sourceLineNumber: 228);
    else if (ppf.ppffile == null)
      loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "PPFFile is null", memberName: nameof (PrintLogs), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/SyMaNaUtilityFunctions.cs", sourceLineNumber: 232);
    else if (ppf.ppfid == null)
      loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "PPFID is null", memberName: nameof (PrintLogs), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/SyMaNaUtilityFunctions.cs", sourceLineNumber: 236);
    else
      loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, $"PPFID: {ppf.ppfid}\nPPF FILE: {ppf.ppffile}", memberName: nameof (PrintLogs), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/SyMaNaUtilityFunctions.cs", sourceLineNumber: 239);
  }
}
