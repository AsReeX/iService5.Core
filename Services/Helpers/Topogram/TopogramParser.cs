// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Helpers.Topogram.TopogramParser
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Data;
using iService5.Core.Services.Platform;
using iService5.Core.Services.User.ActivityLogging;
using MvvmCross;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#nullable disable
namespace iService5.Core.Services.Helpers.Topogram;

public class TopogramParser
{
  private string _enumber = "";
  private IPlatformSpecificServiceLocator _locator = Mvx.IoCProvider.Resolve<IPlatformSpecificServiceLocator>();
  private IMetadataService _metadataService = Mvx.IoCProvider.Resolve<IMetadataService>();
  private List<TopogramScreen> _topogramScreens = new List<TopogramScreen>();
  private iService5.Core.Services.Helpers.Topogram.Topogram topogram = new iService5.Core.Services.Helpers.Topogram.Topogram();
  public const string AIS_TOPOGRAM_TYPE = "14127";
  public const string AIS_CONTROL_TYPE = "14117";
  public const string PASSIVE_COMPONENT = "P;";
  public const string SENSOR_COMPONENT = "S;";
  public const string CONSUMER_COMPONENT = "C;";
  private readonly ILoggingService _loggingService;

  public TopogramParser()
  {
  }

  public TopogramParser(string enumber, ILoggingService loggingService)
  {
    this._loggingService = loggingService;
    this._enumber = enumber;
    this.ParseTopogram(this._enumber);
  }

  public string GetTopogramFilePath(string enumber)
  {
    try
    {
      uploadDocument uploadDocument = this._metadataService.getMaterialDocuments(enumber).Where<uploadDocument>((Func<uploadDocument, bool>) (x => x.type == "14127")).First<uploadDocument>();
      return uploadDocument == null ? string.Empty : Path.Combine(this._locator.GetPlatformSpecificService().getFolder(), uploadDocument.toFileName());
    }
    catch (Exception ex)
    {
      return string.Empty;
    }
  }

  public string GetControlUrlParameter(string topogramDocumentpath)
  {
    string str1 = File.ReadAllText(topogramDocumentpath);
    if (str1.StartsWith("T;"))
      str1 = "\n" + str1;
    string str2 = "\nT;";
    return ((IEnumerable<string>) ((IEnumerable<string>) str1.Split(new string[1]
    {
      str2
    }, StringSplitOptions.None)).Skip<string>(1).ToList<string>().FirstOrDefault<string>().Split(';')).FirstOrDefault<string>();
  }

  private void ParseTopogram(string enumber)
  {
    try
    {
      List<string> stringList = new List<string>();
      string topogramFilePath = this.GetTopogramFilePath(enumber);
      if (string.IsNullOrEmpty(topogramFilePath))
        throw new TopogramException("Topogram Document not found");
      if (!File.Exists(topogramFilePath))
        throw new TopogramException("Topogram Document is not yet downloaded");
      File.ReadAllLines(topogramFilePath);
      string str1 = File.ReadAllText(topogramFilePath);
      if (str1.StartsWith("T;"))
        str1 = "\n" + str1;
      string str2 = "\nT;";
      this.SetTopogramScreens(((IEnumerable<string>) str1.Split(new string[1]
      {
        str2
      }, StringSplitOptions.None)).Skip<string>(1).ToList<string>());
    }
    catch (Exception ex)
    {
      throw new TopogramException("Failed to get screen:" + ex.Message);
    }
  }

  private void SetTopogramScreens(List<string> screens)
  {
    foreach (string screen in screens)
    {
      List<TopogramComponent> components = new List<TopogramComponent>();
      string screenName = "";
      string[] source = screen.Split('\n');
      List<string> stringList1 = new List<string>();
      List<string> stringList2 = new List<string>();
      string str1 = string.Empty;
      foreach (string str2 in source)
      {
        try
        {
          if (((IEnumerable<string>) source).First<string>() == str2)
            screenName = str2.Split(';')[0];
          if (!str2.StartsWith("#") && (str2.Contains("P;") || str2.Contains("S;") || str2.Contains("C;")))
          {
            string[] strArray = str2.Split(';');
            int length = strArray.Length;
            TopogramComponentType componentType = this.GetComponentType(0 < length ? strArray[0] : "");
            string name = 1 < length ? strArray[1] : "";
            string bitmap = 2 < length ? strArray[2] : "";
            string values = 3 < length ? strArray[3] : "";
            int result;
            int numberOfFrames = 4 < length ? (int.TryParse(strArray[4], out result) ? result : -1) : -1;
            string icon = 5 < length ? strArray[5] : "";
            string animationType = 6 < length ? strArray[6] : "";
            int xPosition = 7 < length ? (int.TryParse(strArray[7], out result) ? result : -1) : -1;
            int yPosition = 8 < length ? (int.TryParse(strArray[8], out result) ? result : -1) : -1;
            string numericDisplay = 9 < length ? strArray[9] : "";
            string displayName = 10 < length ? strArray[10] : "";
            string dependencies = 11 < length ? strArray[11].Replace("\r", "") : "";
            components.Add(new TopogramComponent(componentType, name, bitmap, values, numberOfFrames, icon, animationType, xPosition, yPosition, numericDisplay, displayName, dependencies, this._loggingService));
          }
        }
        catch (Exception ex)
        {
          str1 = $"{str1}{str2}\n";
        }
      }
      if (!string.IsNullOrEmpty(str1))
        this._loggingService.getLogger().LogAppWarning(LoggingContext.MONITORING, "Failed to parse lines\n" + str1, memberName: nameof (SetTopogramScreens), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/Topogram/TopogramParser.cs", sourceLineNumber: 146);
      this._topogramScreens.Add(new TopogramScreen(screenName, components));
    }
    this.topogram = new iService5.Core.Services.Helpers.Topogram.Topogram(this._topogramScreens);
  }

  private TopogramComponentType GetComponentType(string type)
  {
    switch (type)
    {
      case "P":
        return TopogramComponentType.PASSIVE;
      case "S":
        return TopogramComponentType.SENSOR;
      case "C":
        return TopogramComponentType.CONSUMER;
      default:
        return TopogramComponentType.NONE;
    }
  }

  public iService5.Core.Services.Helpers.Topogram.Topogram GetTopogram() => this.topogram;

  public string GetControlFilePath(string enumber)
  {
    try
    {
      uploadDocument uploadDocument = this._metadataService.getMaterialDocuments(enumber).Where<uploadDocument>((Func<uploadDocument, bool>) (x => x.type == "14117")).FirstOrDefault<uploadDocument>();
      return uploadDocument == null ? string.Empty : Path.Combine(this._locator.GetPlatformSpecificService().getFolder(), uploadDocument.toFileName());
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppError(LoggingContext.USER, "Error while fetching the Control Files from the DB : " + ex.Message, memberName: nameof (GetControlFilePath), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/Topogram/TopogramParser.cs", sourceLineNumber: 187);
      return string.Empty;
    }
  }
}
