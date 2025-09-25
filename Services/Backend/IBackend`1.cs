// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Backend.IBackend`1
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.User;
using System.Threading.Tasks;

#nullable disable
namespace iService5.Core.Services.Backend;

public interface IBackend<T>
{
  BackendRequestStatus executeRequest(T request);

  Task<BackendRequestStatus> executeBinaryRequest(BinaryDownloadTask _bdt, T request);

  void CancelCurrentRequest();

  Task<BackendRequestStatus> executeAsyncRequest(T request);
}
