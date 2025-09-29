// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Localisation.ShortTextsService
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

#nullable disable
namespace iService5.Core.Services.Localisation;

public class ShortTextsService : IShortTextsService
{
  public string getPlatformString(string textid)
  {
    switch (textid)
    {
      case "FP_POPUP_PROMPT":
        return AppResource.FP_POPUP_PROMPT;
      case "FP_POPUP_HEADER":
        return AppResource.FP_POPUP_HEADER;
      case "CANCEL_LABEL":
        return AppResource.CANCEL_LABEL;
      case "LOGIN_PAGE_USERNAME_PLACEHOLDER":
        return AppResource.LOGIN_PAGE_USERNAME_PLACEHOLDER;
      case "PASSWORD_TEXT":
        return AppResource.PASSWORD_TEXT;
      default:
        return "no text for id: " + textid;
    }
  }
}
