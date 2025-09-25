// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Data.ApplianceInstruction
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

#nullable disable
namespace iService5.Core.Services.Data;

public class ApplianceInstruction
{
  public string instruction { get; set; }

  public string textDecoration { get; set; }

  public string linkText { get; set; }

  public ActionNames action { get; set; }

  public string sNo { get; set; }

  public ApplianceInstruction(
    string sNo,
    string instruction,
    string textDecoration,
    string linkText,
    ActionNames action)
  {
    this.sNo = sNo;
    this.instruction = instruction;
    this.textDecoration = textDecoration;
    this.linkText = linkText;
    this.action = action;
  }
}
