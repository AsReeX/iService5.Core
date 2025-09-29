// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Helpers.FormUpload
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Backend;
using iService5.Core.Services.User;
using iService5.Core.Services.User.ActivityLogging;
using MvvmCross;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
//using Xamarin.Forms;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

#nullable disable
namespace iService5.Core.Services.Helpers;

public static class FormUpload
{
  private static readonly Encoding encoding = Encoding.UTF8;
  public static readonly ILoggingService loggingService = Mvx.IoCProvider.Resolve<ILoggingService>();
  public static readonly IAlertService alertService = Mvx.IoCProvider.Resolve<IAlertService>();

  public static async Task<BackendRequestStatus> MultipartFormDataPost(
    HttpWebRequest request,
    Dictionary<string, object> postParameters)
  {
    string formDataBoundary = $"----------{Guid.NewGuid():N}";
    string contentType = "multipart/form-data; boundary=" + formDataBoundary;
    byte[] formData = FormUpload.GetMultipartFormData(postParameters, formDataBoundary);
    BackendRequestStatus backendRequestStatus = await FormUpload.PostForm(request, contentType, formData);
    formDataBoundary = (string) null;
    contentType = (string) null;
    formData = (byte[]) null;
    return backendRequestStatus;
  }

  private static async Task<BackendRequestStatus> PostForm(
    HttpWebRequest request,
    string contentType,
    byte[] formData)
  {
    try
    {
      if (request == null)
        throw new ArgumentNullException("request is not a http request");
      request.Method = "POST";
      request.ContentType = contentType;
      request.CookieContainer = new CookieContainer();
      request.ContentLength = (long) formData.Length;
      using (Stream requestStream = request.GetRequestStream())
      {
        requestStream.Write(formData, 0, formData.Length);
        requestStream.Close();
      }
      IBackend<HttpWebRequest> bEndInst = Mvx.IoCProvider.Resolve<IBackend<HttpWebRequest>>();
      BackendRequestStatus backendRequestStatus = await bEndInst.executeAsyncRequest(request);
      return backendRequestStatus;
    }
    catch (Exception ex)
    {
      FormUpload.loggingService.getLogger().LogAppError(LoggingContext.USER, "Exception occured while posting form:" + ex?.ToString(), memberName: nameof (PostForm), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/FormUpload.cs", sourceLineNumber: 60);
      IUserSession userSession = Mvx.IoCProvider.Resolve<IUserSession>();
      userSession.setServiceError(new ServiceError(ErrorType.Technical, ex.Message, false));
      return BackendRequestStatus.RES_Exception;
    }
  }

  private static byte[] GetMultipartFormData(
    Dictionary<string, object> postParameters,
    string boundary)
  {
    try
    {
      Stream stream = (Stream) new MemoryStream();
      bool flag = false;
      foreach (KeyValuePair<string, object> postParameter in postParameters)
      {
        if (flag)
          stream.Write(FormUpload.encoding.GetBytes("\r\n"), 0, FormUpload.encoding.GetByteCount("\r\n"));
        flag = true;
        if (postParameter.Value is FormUpload.FileParameter)
        {
          FormUpload.FileParameter fileParameter = (FormUpload.FileParameter) postParameter.Value;
          string s = $"--{boundary}\r\nContent-Disposition: form-data; name=\"{postParameter.Key}\"; filename=\"{fileParameter.FileName ?? postParameter.Key}\"\r\nContent-Type: {fileParameter.ContentType ?? "application/octet-stream"}\r\n\r\n";
          stream.Write(FormUpload.encoding.GetBytes(s), 0, FormUpload.encoding.GetByteCount(s));
          stream.Write(fileParameter.File, 0, fileParameter.File.Length);
        }
        else
        {
          string s = $"--{boundary}\r\nContent-Disposition: form-data; name=\"{postParameter.Key}\"\r\n\r\n{postParameter.Value}";
          stream.Write(FormUpload.encoding.GetBytes(s), 0, FormUpload.encoding.GetByteCount(s));
        }
      }
      string s1 = $"\r\n--{boundary}--\r\n";
      stream.Write(FormUpload.encoding.GetBytes(s1), 0, FormUpload.encoding.GetByteCount(s1));
      stream.Position = 0L;
      byte[] buffer = new byte[stream.Length];
      stream.Read(buffer, 0, buffer.Length);
      stream.Close();
      return buffer;
    }
    catch (Exception ex)
    {
      FormUpload.loggingService.getLogger().LogAppError(LoggingContext.SECURESTORAGE, "Exception occured in GetMultipartFormData:" + ex?.ToString(), memberName: nameof (GetMultipartFormData), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/FormUpload.cs", sourceLineNumber: 123);
      Device.BeginInvokeOnMainThread((Action) (async () =>
      {
        FormUpload.loggingService.getLogger().LogAppInformation(LoggingContext.USERSESSION, $"Alert popup appeared with message: Exception occured while posting form: {ex}", memberName: nameof (GetMultipartFormData), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/FormUpload.cs", sourceLineNumber: 126);
        await Application.Current.MainPage.DisplayAlert(AppResource.INFORMATION_TEXT, "Exception occured while posting form:" + ex?.ToString(), AppResource.WARNING_OK);
      }));
      return new byte[0];
    }
  }

  public class FileParameter
  {
    public byte[] File { get; set; }

    public string FileName { get; set; }

    public string ContentType { get; set; }

    public FileParameter(byte[] file)
      : this(file, (string) null)
    {
    }

    public FileParameter(byte[] file, string filename)
      : this(file, filename, (string) null)
    {
    }

    public FileParameter(byte[] file, string filename, string contenttype)
    {
      this.File = file;
      this.FileName = filename;
      this.ContentType = contenttype;
    }
  }
}
