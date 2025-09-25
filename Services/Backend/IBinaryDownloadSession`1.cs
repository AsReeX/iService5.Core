// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Backend.IBinaryDownloadSession`1
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Data;
using iService5.Core.Services.User;
using System.Collections.Generic;
using System.Threading.Tasks;

#nullable disable
namespace iService5.Core.Services.Backend;

public interface IBinaryDownloadSession<T>
{
  void setResponseFile(string path);

  void ProgressCallback(ulong progress, ulong total, bool isBinaryDownload = false);

  void CompletionCallback(bool errorless);

  void SaveDataBlock(byte[] buffer, int length);

  BackendRequestStatus getMetadataChecksum();

  Task<BackendRequestStatus> authenticateDBPassword();

  Task<BackendRequestStatus> sendFeedbackForm(
    Dictionary<string, string> feedbackFormPostParameters);

  Task<BackendRequestStatus> updateMetadataAsync(BinaryDownloadTask _bdt, iService5.Core.Services.User.progressCallback _pcb);

  Task<BackendRequestStatus> downloadBinaryFileAsync(
    BinaryDownloadTask bdt,
    iService5.Core.Services.User.progressCallback _pcb,
    DownloadProxy proxy);
}
