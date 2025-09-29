// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Helpers.SVGParser.MonitoringGraphicsSVG
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using SkiaSharp;
using SkiaSharp.Views.Forms;
using SkiaSharp.Views.Maui;
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

#nullable disable
namespace iService5.Core.Services.Helpers.SVGParser;

public class MonitoringGraphicsSVG
{
  private XmlNode _frames;
  private float _width;
  private float _height;
  private float _x;
  private float _y;
  private float _frameXValue = 0.0f;
  private float _frameYValue = 0.0f;

  public MonitoringGraphicsSVG(XmlNode frames, float width, float height, float x, float y)
  {
    this._frames = frames;
    this._width = width;
    this._height = height;
    this._x = x;
    this._y = y;
  }

  public XmlNode GetFrames() => this._frames;

  public XmlNode GetFrame(int index) => this._frames.ChildNodes[index];

  public float GetFrameXValue(int svgFrameIndex)
  {
    XmlNode frame = this.GetFrame(svgFrameIndex);
    float result = 0.0f;
    return frame.Attributes["x"] != null && float.TryParse(frame.Attributes["x"].Value, NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result) ? result : result;
  }

  public float GetFrameYValue(int svgFrameIndex)
  {
    XmlNode frame = this.GetFrame(svgFrameIndex);
    float result = 0.0f;
    return frame.Attributes["y"] != null && float.TryParse(frame.Attributes["y"].Value, NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result) ? result : result;
  }

  public Stream GetStream(int svgFrameIndex)
  {
    try
    {
      XmlNode frame = this.GetFrame(svgFrameIndex);
      if (frame == null)
      {
        Console.WriteLine("Frame is empty.");
        return (Stream) null;
      }
      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.LoadXml(frame.OuterXml);
      if (frame.Attributes["x"] != null || frame.Attributes["y"] != null)
        this.RemoveAttributes(xmlDocument.DocumentElement, "x", "y");
      else
        this.RemoveNamespaces((XmlNode) xmlDocument.DocumentElement);
      MemoryStream outStream = new MemoryStream();
      xmlDocument.Save((Stream) outStream);
      outStream.Position = 0L;
      return (Stream) outStream;
    }
    catch (Exception ex)
    {
      throw new Exception("Failed to get stream for svg element: " + ex.Message);
    }
  }

  private void RemoveAttributes(XmlElement element, params string[] attributeNames)
  {
    foreach (string attributeName in attributeNames)
    {
      if (element.HasAttribute(attributeName))
        element.RemoveAttribute(attributeName);
    }
    foreach (XmlElement childNode in element.ChildNodes)
      this.RemoveAttributes(childNode, attributeNames);
  }

  private void RemoveNamespaces(XmlNode node)
  {
    if (node.NodeType != XmlNodeType.Element)
      return;
    node.Prefix = string.Empty;
    foreach (XmlAttribute attribute in (XmlNamedNodeMap) node.Attributes)
    {
      if (attribute.Name.StartsWith("xmlns"))
      {
        if (node != node.OwnerDocument.DocumentElement || !(attribute.Name == "xmlns"))
          attribute.OwnerElement.Attributes.Remove(attribute);
      }
      else
        attribute.Prefix = string.Empty;
    }
    foreach (XmlNode childNode in node.ChildNodes)
      this.RemoveNamespaces(childNode);
  }

    public void ApplyTransparentColor(int index, SKColor _sktransparentcolor)
    {
        // --- POPRAWIONA LINIA ---
        // 1. Konwertujemy SKColor na natywny kolor MAUI (Microsoft.Maui.Graphics.Color).
        // 2. Metoda ToHex() w MAUI zwraca od razu format #RRGGBB, którego potrzebujemy.
        string pattern = _sktransparentcolor.ToMauiColor().ToHex();

        try
        {
            XmlNode frame = this.GetFrame(index);
            frame.SelectNodes($"//*[@fill='{pattern}']");
            StringBuilder stringBuilder1 = new StringBuilder(frame.InnerXml);
            try
            {
                if (!Regex.IsMatch(stringBuilder1.ToString(), pattern, RegexOptions.IgnoreCase))
                    return;
                string str = Regex.Replace(Regex.Replace(stringBuilder1.ToString(), $"fill-opacity=\"1.00\" fill=\"{pattern}\"", $"fill-opacity=\"0.00\" fill=\"{pattern}\"", RegexOptions.IgnoreCase), $"stroke-opacity=\"1.00\" stroke=\"{pattern}\"", $"stroke-opacity=\"0.00\" stroke=\"{pattern}\"", RegexOptions.IgnoreCase);
                stringBuilder1.Clear();
                StringBuilder stringBuilder2 = new StringBuilder(str);
                try
                {
                    bool hasChildNodes = this._frames.ChildNodes[index].HasChildNodes;
                    if (this._frames.ChildNodes.Count > 0)
                        this._frames.ChildNodes[index].InnerXml = MonitoringGraphicsSVG.XmlStringToXmlNode(stringBuilder2.ToString()).InnerXml;
                }
                catch (Exception ex)
                {
                    throw new SVGParserException("Failed to get stream for svg element:" + ex.Message);
                }
            }
            catch (Exception ex)
            {
                throw new SVGParserException("Failed to get stream for svg element:" + ex.Message);
            }
        }
        catch (Exception ex)
        {
            throw new SVGParserException("Failed to get stream for svg element:" + ex.Message);
        }
    }

    public static XmlNode XmlStringToXmlNode(string xmlInputString)
  {
    if (string.IsNullOrEmpty(xmlInputString.Trim()))
      throw new ArgumentNullException(nameof (xmlInputString));
    XmlDocument xmlNode = new XmlDocument();
    using (StringReader txtReader = new StringReader(xmlInputString))
      xmlNode.Load((TextReader) txtReader);
    return (XmlNode) xmlNode;
  }

  public float GetHeight() => this._height;

  public float GetWidth() => this._width;

  public float GetXValue() => this._x;

  public float GetYValue() => this._y;
}
