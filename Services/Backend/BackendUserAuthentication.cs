// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Backend.BackendUserAuthentication
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.User;
using iService5.Core.Services.User.ActivityLogging;
using MvvmCross;
using System;
using System.Net;
using System.Threading.Tasks;

#nullable disable
namespace iService5.Core.Services.Backend;

public class BackendUserAuthentication : IBackendUserAuthentication<HttpWebRequest>
{
  public static readonly ILoggingService loggingService = Mvx.IoCProvider.Resolve<ILoggingService>();

  public BackendRequestStatus authenticateUser() => throw new NotImplementedException();

  public async Task<BackendRequestStatus> authenticateUserAsync()
  {
    IUserSession userSession = Mvx.IoCProvider.Resolve<IUserSession>();
    if (!userSession.IsHostReachable())
      return BackendRequestStatus.RES_NInternet;
    IUserAccount user = Mvx.IoCProvider.Resolve<IUserAccount>();
    string encoded = "";
    BackendRequestStatus res = user.getEncoded(out encoded);
    if (res != 0)
      return res;
    IBackendDetails<HttpWebRequest> bEnd = Mvx.IoCProvider.Resolve<IBackendDetails<HttpWebRequest>>();
    HttpWebRequest httpWebRequest;
    try
    {
      httpWebRequest = bEnd.getAuthenticateRequest();
    }
    catch (NotSupportedException ex)
    {
      BackendUserAuthentication.loggingService.getLogger().LogAppError(LoggingContext.BACKEND, "Invoked method is not supported", memberName: nameof (authenticateUserAsync), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/BackendUserAuthentication.cs", sourceLineNumber: 38);
      return BackendRequestStatus.RES_NURL;
    }
    catch (UriFormatException ex)
    {
      BackendUserAuthentication.loggingService.getLogger().LogAppError(LoggingContext.BACKEND, "An invalid URI is detected.", memberName: nameof (authenticateUserAsync), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Backend/BackendUserAuthentication.cs", sourceLineNumber: 43);
      return BackendRequestStatus.RES_NURL;
    }
    httpWebRequest.Headers.Add("Authorization", "Basic " + encoded);
    IBackend<HttpWebRequest> bEndInst = Mvx.IoCProvider.Resolve<IBackend<HttpWebRequest>>();
    BackendRequestStatus backendRequestStatus = await bEndInst.executeAsyncRequest(httpWebRequest);
    return backendRequestStatus;
  }
}
