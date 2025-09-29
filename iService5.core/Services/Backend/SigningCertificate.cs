// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Backend.SigningCertificate
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Helpers;
using iService5.Core.Services.User;
using MvvmCross;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

#nullable disable
namespace iService5.Core.Services.Backend;

public class SigningCertificate : ISigningCertificate<HttpWebRequest>
{
  public static readonly ISecureStorageService secureStorageService = Mvx.IoCProvider.Resolve<ISecureStorageService>();

  public async Task<BackendRequestStatus> sendSignCertificate(string csr)
  {
    IUserSession userSession = Mvx.IoCProvider.Resolve<IUserSession>();
    if (!userSession.IsHostReachable())
      return BackendRequestStatus.RES_NInternet;
    IBackendDetails<HttpWebRequest> bEnd = Mvx.IoCProvider.Resolve<IBackendDetails<HttpWebRequest>>();
    HttpWebRequest httpWebRequest;
    try
    {
      httpWebRequest = bEnd.postSignCertificate();
    }
    catch (NotSupportedException ex)
    {
      return BackendRequestStatus.RES_NURL;
    }
    catch (UriFormatException ex)
    {
      return BackendRequestStatus.RES_NURL;
    }
    string jwtToken = SigningCertificate.secureStorageService.getJWTToken().Result;
    if (!string.IsNullOrWhiteSpace(jwtToken))
      httpWebRequest.Headers.Add("Authorization", "Bearer " + jwtToken);
    byte[] data = Encoding.ASCII.GetBytes(csr);
    httpWebRequest.ContentType = "text/plain";
    using (Stream stream = httpWebRequest.GetRequestStream())
      stream.Write(data, 0, data.Length);
    IBackend<HttpWebRequest> bEndInst = Mvx.IoCProvider.Resolve<IBackend<HttpWebRequest>>();
    BackendRequestStatus backendRequestStatus = await bEndInst.executeAsyncRequest(httpWebRequest);
    return backendRequestStatus;
  }
}
