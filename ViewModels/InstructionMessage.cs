// Decompiled with JetBrains decompiler
// Type: iService5.Core.ViewModels.InstructionMessage
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

#nullable disable
namespace iService5.Core.ViewModels;

public class InstructionMessage
{
  public string index { get; set; }

  public string message { get; set; }

  public bool hasImage { get; set; }

  public InstructionMessage(string index, string message, bool hasImage)
  {
    this.index = index;
    this.message = message;
    this.hasImage = hasImage;
  }
}
