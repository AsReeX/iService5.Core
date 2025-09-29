// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Helpers.SVGParser.SVGParser
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Platform;
using iService5.Core.Services.User.ActivityLogging;
using MvvmCross;
using SkiaSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;

#nullable disable
namespace iService5.Core.Services.Helpers.SVGParser;

public class SVGParser
{
  private MonitoringGraphicsSVG monitoringGraphicsSVG;
  private string _svgName;
  private readonly ILoggingService _loggingService;
  internal string monitoringGraphicsDirectory = "MonitoringGraphics";
  internal List<string> SvgsForPreservingTransforms = new List<string>()
  {
    "plug_1_2.svg"
  };

  public SVGParser(string svgName, ILoggingService logService)
  {
    this._svgName = svgName;
    this._loggingService = logService;
    this.ParseSVG();
  }

  public void ParseSVG()
  {
    try
    {
      string str1 = Path.Combine($"{Mvx.IoCProvider.Resolve<IPlatformSpecificServiceLocator>().GetPlatformSpecificService().getFolder()}/{this.monitoringGraphicsDirectory}");
      string str2 = Path.Combine(str1, this._svgName);
      bool flag1 = File.Exists(str2);
      if (!flag1)
        str2 = UtilityFunctions.GetActualFilePath(str1, this._svgName, str2);
      if (flag1 || !string.IsNullOrEmpty(str2))
      {
        StreamReader streamReader = new StreamReader(str2);
        string end = streamReader.ReadToEnd();
        streamReader.Close();
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(end);
        XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDocument.NameTable);
        nsmgr.AddNamespace("a", "http://www.w3.org/2000/svg");
        try
        {
          foreach (XmlAttribute selectNode in xmlDocument.SelectNodes("//@transform", nsmgr))
          {
            if (selectNode.Value.Contains("scale") && selectNode.OwnerElement.ParentNode.ParentNode.ChildNodes.Count > 1)
            {
              if (this.SvgsForPreservingTransforms.Contains(this._svgName))
                throw new InvalidDataException($"SVG {this._svgName} does not get to change dimensions based on its transform attribute");
              string[] strArray = selectNode.Value.Split(',', ' ');
              double num1 = Convert.ToDouble(strArray[0].Remove(0, 6), (IFormatProvider) CultureInfo.InvariantCulture);
              int index = strArray.Length - 1;
              double num2 = Convert.ToDouble(strArray[index].Remove(strArray[index].Length - 1).Trim(), (IFormatProvider) CultureInfo.InvariantCulture);
              XmlAttributeCollection attributes1 = selectNode.OwnerElement.FirstChild.Attributes;
              XmlAttributeCollection attributes2 = selectNode.OwnerElement.FirstChild.FirstChild.Attributes;
              foreach (XmlAttribute xmlAttribute in (XmlNamedNodeMap) attributes1)
              {
                if (xmlAttribute.Name == "x")
                {
                  selectNode.Value = "scale(1, 1)";
                  xmlAttribute.Value = (num1 * Convert.ToDouble(xmlAttribute.Value, (IFormatProvider) CultureInfo.InvariantCulture)).ToString();
                }
                if (xmlAttribute.Name == "y")
                {
                  selectNode.Value = "scale(1, 1)";
                  xmlAttribute.Value = (num2 * Convert.ToDouble(xmlAttribute.Value, (IFormatProvider) CultureInfo.InvariantCulture)).ToString();
                }
              }
              string svgPathData = SKPath.ParseSvgPathData(selectNode.OwnerElement.FirstChild.FirstChild.Attributes["d"].Value).ToSvgPathData();
              selectNode.OwnerElement.FirstChild.FirstChild.Attributes["d"].Value = svgPathData;
            }
          }
        }
        catch (Exception ex)
        {
          this._loggingService.getLogger().LogAppError(LoggingContext.MONITORING, "exception : " + ex.Message, memberName: nameof (ParseSVG), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/SVGParser/SVGParser.cs", sourceLineNumber: 100);
        }
        float result1 = 0.0f;
        float result2 = 0.0f;
        float result3 = 0.0f;
        float result4 = 0.0f;
        bool flag2 = false;
        bool flag3 = false;
        XmlNode frames = xmlDocument.GetElementsByTagName("a:svg")[0];
        XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName("a:svg");
        if (frames == null)
        {
          frames = xmlDocument.GetElementsByTagName("svg")[0];
          elementsByTagName = xmlDocument.GetElementsByTagName("svg");
        }
        IEnumerator enumerator = elementsByTagName.GetEnumerator();
        while (enumerator.MoveNext())
        {
          if (((XmlNode) enumerator.Current).Attributes["width"] != null)
            flag2 = float.TryParse(((XmlNode) enumerator.Current).Attributes["width"].Value, NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result1);
          if (((XmlNode) enumerator.Current).Attributes["height"] != null)
            flag3 = float.TryParse(((XmlNode) enumerator.Current).Attributes["height"].Value, NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result2);
          if (((XmlNode) enumerator.Current).Attributes["x"] != null)
            float.TryParse(((XmlNode) enumerator.Current).Attributes["x"].Value, NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result3);
          if (((XmlNode) enumerator.Current).Attributes["y"] != null)
            float.TryParse(((XmlNode) enumerator.Current).Attributes["y"].Value, NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result4);
          if (flag2 | flag3)
            break;
        }
        if (!(flag3 & flag2))
          throw new SVGParserException("SVG height and width could not be parsed for " + this._svgName);
        this.monitoringGraphicsSVG = new MonitoringGraphicsSVG(frames, result1, result2, result3, result4);
      }
      else
        this._loggingService.getLogger().LogAppError(LoggingContext.MONITORING, "Missing SVG: " + this._svgName, memberName: nameof (ParseSVG), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/SVGParser/SVGParser.cs", sourceLineNumber: 136);
    }
    catch (Exception ex)
    {
      throw new Exception("Failed to get stream for svg element:" + ex?.ToString());
    }
  }

  public MonitoringGraphicsSVG GetMonitoringGraphicsSVG() => this.monitoringGraphicsSVG;
}
