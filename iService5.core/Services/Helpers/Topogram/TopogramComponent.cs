// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Helpers.Topogram.TopogramComponent
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.User.ActivityLogging;
using System.Collections.Generic;

#nullable disable
namespace iService5.Core.Services.Helpers.Topogram;

public class TopogramComponent
{
  private TopogramComponentType _type;
  private string _name;
  private string _bitmap;
  private string _values;
  private int _numberOfFrames;
  private string _icon;
  private string _animationType;
  private int _xPosition;
  private int _yPosition;
  private string _numericDisplay;
  private string _displayName;
  private string _dependencies;
  private float _valueDisplayXPosition;
  private float _valueDisplayYPosition;
  private readonly ILoggingService _loggingService;

  public TopogramComponent(TopogramComponent component)
  {
    this._type = component._type;
    this._name = component._name;
    this._bitmap = component._bitmap;
    this._name = component._name;
    this._values = component._values;
    this._numberOfFrames = component._numberOfFrames;
    this._xPosition = component._xPosition;
    this._yPosition = component._yPosition;
    this._numericDisplay = component._numericDisplay;
    this._displayName = component._displayName;
    this._dependencies = component._dependencies;
    this._animationType = component._animationType;
  }

  public TopogramComponent()
  {
    this._type = TopogramComponentType.NONE;
    this._name = "";
    this._bitmap = "";
    this._name = "";
    this._values = "";
    this._numberOfFrames = -1;
    this._animationType = "";
    this._xPosition = -1;
    this._yPosition = -1;
    this._numericDisplay = "";
    this._displayName = "";
    this._dependencies = "";
  }

  public TopogramComponent(
    TopogramComponentType type,
    string name,
    string bitmap,
    string values,
    int numberOfFrames,
    string icon,
    string animationType,
    int xPosition,
    int yPosition,
    string numericDisplay,
    string displayName,
    string dependencies,
    ILoggingService loggingService)
  {
    this._type = type;
    this._name = name;
    this._bitmap = bitmap;
    this._name = name;
    this._values = values;
    this._numberOfFrames = numberOfFrames;
    this._animationType = animationType;
    this._xPosition = xPosition;
    this._yPosition = yPosition;
    this._numericDisplay = numericDisplay;
    this._displayName = displayName;
    this._dependencies = dependencies;
    this._loggingService = loggingService;
  }

  public string GetBitMap() => this._bitmap;

  public int GetXPosition() => this._xPosition;

  public int GetYPosition() => this._yPosition;

  public TopogramComponentType GetComponentType() => this._type;

  public string GetDependencies() => this._dependencies;

  public string GetName() => this._name;

  public void SetValueDisplayXPosition(float valueDisplayXPosition)
  {
    this._valueDisplayXPosition = valueDisplayXPosition;
  }

  public float GetValueDisplayXPosition() => this._valueDisplayXPosition;

  public void SetValueDisplayYPosition(float valueDisplayYPosition)
  {
    this._valueDisplayYPosition = valueDisplayYPosition;
  }

  public float GetValueDisplayYPosition() => this._valueDisplayYPosition;

  public string GetFileName()
  {
    return string.IsNullOrEmpty(this._bitmap) ? string.Empty : this._bitmap + ".svg";
  }

  public int GetNumberOfStates()
  {
    string[] strArray = this._bitmap.Split('_', '.');
    int result;
    return !int.TryParse(strArray[strArray.Length - 1], out result) ? 0 : result;
  }

  public int GetNumberOfFrames()
  {
    string[] strArray = this._bitmap.Split('_', '.');
    bool flag = false;
    int result = 0;
    if (strArray.Length > 2)
      flag = int.TryParse(strArray[strArray.Length - 2], out result);
    if (!flag)
      return 0;
    if (this._numberOfFrames <= 0)
      return result;
    if (this._numberOfFrames == result)
      return this._numberOfFrames;
    this._loggingService.getLogger().LogAppError(LoggingContext.MONITORING, $"{this._bitmap} : Number of frames differ from the topogram field ({this._numberOfFrames.ToString()}) to naming convention ({result.ToString()})", memberName: nameof (GetNumberOfFrames), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/Topogram/TopogramComponent.cs", sourceLineNumber: 168);
    return result;
  }

  public string GetDisplayName() => this._displayName;

  public string GetNumericDisplay() => this._numericDisplay;

  public List<int> GetValues()
  {
    if (string.IsNullOrEmpty(this._values))
      return new List<int>();
    string[] strArray = this._values.Split(',');
    List<int> values = new List<int>();
    for (int index = 0; index < strArray.Length; ++index)
    {
      int result;
      if (int.TryParse(strArray[index], out result))
        values.Add(result);
    }
    return values;
  }

  public string GetAnimationType() => this._animationType;
}
