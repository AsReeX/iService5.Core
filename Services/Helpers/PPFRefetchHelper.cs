// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Helpers.PPFRefetchHelper
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Data;
using iService5.Core.Services.Platform;
using iService5.Core.Services.User;
using iService5.Core.Services.User.ActivityLogging;
using MvvmCross;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace iService5.Core.Services.Helpers;

public class PPFRefetchHelper
{
  private IMetadataService metadataService = Mvx.IoCProvider.Resolve<IMetadataService>();
  private ILoggingService loggingService = Mvx.IoCProvider.Resolve<ILoggingService>();
  private IUserSession userSession = Mvx.IoCProvider.Resolve<IUserSession>();
  public bool CancelActive;
  private IPlatformSpecificServiceLocator locator = Mvx.IoCProvider.Resolve<IPlatformSpecificServiceLocator>();
  private int failedCount = 0;

  public UpdatePPFCompletion AttemptPPFCallback { get; internal set; }

  public void RequestPpfs(UpdatePPFCompletion ucb, bool isPPFRefetchinSettings)
  {
    this.AttemptPPFCallback = ucb;
    List<DownloadProxy> enumbers = !isPPFRefetchinSettings ? this.GetDownloadedEnumbersFromDB() : this.GetDownloadedEnumbersFromDBForSettingsPage();
    if (enumbers.Count > 0)
    {
      this.loggingService.getLogger().LogAppInformation(LoggingContext.LOCAL, "PPF Refetch Process Starts!", memberName: nameof (RequestPpfs), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/PPFRefetchHelper.cs", sourceLineNumber: 55);
      this.loggingService.getLogger().LogAppInformation(LoggingContext.LOCAL, $"Total Refetch PPF ENumbers count:{enumbers.Count}", memberName: nameof (RequestPpfs), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/PPFRefetchHelper.cs", sourceLineNumber: 56);
      this.DownloadBundlePpfFiles(enumbers);
    }
    else
      this.AttemptPPFCallback(PPFRefetchStatus.Completed);
  }

  public void RequestPpfsToExpire(UpdatePPFCompletion ucb, List<DownloadProxy> eNumbers)
  {
    this.AttemptPPFCallback = ucb;
    if (eNumbers.Count > 0)
    {
      this.loggingService.getLogger().LogAppInformation(LoggingContext.LOCAL, "Expired PPF process starts.", memberName: nameof (RequestPpfsToExpire), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/PPFRefetchHelper.cs", sourceLineNumber: 70);
      this.DownloadBundlePpfFiles(eNumbers);
    }
    else
      this.AttemptPPFCallback(PPFRefetchStatus.Completed);
  }

  public async void DownloadBundlePpfFiles(List<DownloadProxy> enumbers)
  {
    try
    {
      int maxConcurrentTasks = 3;
      SemaphoreSlim semaphore = new SemaphoreSlim(maxConcurrentTasks);
      for (int index = 0; index < enumbers.Count; ++index)
      {
        if (this.CancelActive)
        {
          DownloadProxy proxy = enumbers[index];
          this.loggingService.getLogger().LogAppError(LoggingContext.LOCAL, "CancelActive is true", memberName: nameof (DownloadBundlePpfFiles), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/PPFRefetchHelper.cs", sourceLineNumber: 90);
          this.ProcessDownloadedPpf(PPFRefetchStatus.Interrupted, proxy, index, enumbers.Count);
          break;
        }
        await semaphore.WaitAsync();
        await Task.Run((Func<Task>) (async () =>
        {
          try
          {
            await this.ProcessBatchAsync(enumbers, index);
          }
          finally
          {
            semaphore.Release();
          }
        }));
      }
      DownloadProxy lastProxy = enumbers.LastOrDefault<DownloadProxy>();
      this.ProcessDownloadedPpf(PPFRefetchStatus.Completed, lastProxy, enumbers.Count, enumbers.Count);
      lastProxy = (DownloadProxy) null;
    }
    catch (Exception ex)
    {
      this.loggingService.getLogger().LogAppError(LoggingContext.LOCAL, ex.Message, memberName: nameof (DownloadBundlePpfFiles), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/PPFRefetchHelper.cs", sourceLineNumber: 112 /*0x70*/);
    }
  }

  private async Task ProcessBatchAsync(List<DownloadProxy> enumbers, int index)
  {
    DownloadProxy proxy = enumbers[index];
    string bundledPpfId = proxy.GetBundledPpfId();
    if (!string.IsNullOrEmpty(bundledPpfId))
    {
      BinaryDownloadTask task = this.userSession.GetBinaryTask(proxy, FileType.BundledPpf, (downloadCallback) (ppfStatus =>
      {
        if (ppfStatus != DownloadStatus.COMPLETED)
        {
          this.loggingService.getLogger().LogAppError(LoggingContext.LOCAL, $"Ppf Refetch of {proxy.vib}/{proxy.ki} failed to get downloaded. And Proxy is assigned to NULL here", memberName: nameof (ProcessBatchAsync), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/PPFRefetchHelper.cs", sourceLineNumber: 126);
          proxy = (DownloadProxy) null;
          ++this.failedCount;
          this.ProcessDownloadedPpf(PPFRefetchStatus.Failed, proxy, index, enumbers.Count);
        }
        this.ProcessDownloadedPpf(PPFRefetchStatus.Started, proxy, index, enumbers.Count);
      }), (iService5.Core.Services.User.progressCallback) (progress => { }));
      await this.userSession.ExecuteBinaryDownloadTask(task);
      task = (BinaryDownloadTask) null;
      bundledPpfId = (string) null;
    }
    else
    {
      this.loggingService.getLogger().LogAppError(LoggingContext.LOCAL, "Don't have vib and ki", memberName: nameof (ProcessBatchAsync), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/PPFRefetchHelper.cs", sourceLineNumber: 137);
      bundledPpfId = (string) null;
    }
  }

  private void ProcessDownloadedPpf(
    PPFRefetchStatus status,
    DownloadProxy proxy,
    int dIndex,
    int total)
  {
    switch (status)
    {
      case PPFRefetchStatus.Started:
        if (proxy == null)
          this.loggingService.getLogger().LogAppError(LoggingContext.LOCAL, "Proxy is null", memberName: nameof (ProcessDownloadedPpf), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/PPFRefetchHelper.cs", sourceLineNumber: 165);
        this.storeDownloadedPpfFile(proxy);
        this.AttemptPPFCallback(status, dIndex, total);
        break;
      case PPFRefetchStatus.Completed:
        this.storeDownloadedPpfFile(proxy);
        this.loggingService.getLogger().LogAppInformation(LoggingContext.LOCAL, "PPF Refetch Process Ends!", memberName: nameof (ProcessDownloadedPpf), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/PPFRefetchHelper.cs", sourceLineNumber: 148);
        this.loggingService.getLogger().LogAppInformation(LoggingContext.LOCAL, $"Total Downloaded PPF Refetch ENumbers Files Count:{dIndex - this.failedCount}", memberName: nameof (ProcessDownloadedPpf), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/PPFRefetchHelper.cs", sourceLineNumber: 149);
        this.AttemptPPFCallback(status);
        break;
      case PPFRefetchStatus.Failed:
      case PPFRefetchStatus.Interrupted:
        this.loggingService.getLogger().LogAppInformation(LoggingContext.LOCAL, "PPF Refetch Process Ends!", memberName: nameof (ProcessDownloadedPpf), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/PPFRefetchHelper.cs", sourceLineNumber: 156);
        this.loggingService.getLogger().LogAppInformation(LoggingContext.LOCAL, "PPF Refetch Status:" + status.ToString(), memberName: nameof (ProcessDownloadedPpf), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/PPFRefetchHelper.cs", sourceLineNumber: 157);
        this.AttemptPPFCallback(status);
        break;
    }
  }

  internal void storeDownloadedPpfFile(DownloadProxy proxy)
  {
    if (proxy == null || proxy.PPFData == null)
      return;
    this.metadataService.DeleteExistingPPF(proxy);
    if (!UtilityFunctions.StorePpfFiles(proxy, this.userSession, this.loggingService, this.metadataService))
      this.loggingService.getLogger().LogAppError(LoggingContext.LOCAL, $"PpfRefetch:Ppf's failed to get extracted from PPF information file {proxy.vib}/{proxy.ki}.", memberName: nameof (storeDownloadedPpfFile), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Helpers/PPFRefetchHelper.cs", sourceLineNumber: 181);
  }

  public List<DownloadProxy> GetDownloadedEnumbersFromDB()
  {
    List<string> refetchedEnumbers = this.userSession.RefetchedPPFENumbers();
    return this.GetToDownloadProxies(UtilityFunctions.getDownloadedEnumbersFromDB(this.metadataService).Where<string>((Func<string, bool>) (eNum => !refetchedEnumbers.Contains(eNum))).ToList<string>());
  }

  public List<DownloadProxy> GetDownloadedEnumbersFromDBForSettingsPage()
  {
    return this.GetToDownloadProxies(UtilityFunctions.getDownloadedEnumbersFromDB(this.metadataService));
  }

  public List<DownloadProxy> GetToDownloadProxies(List<string> downloadedEnumebers)
  {
    List<DownloadProxy> toDownloadProxies = new List<DownloadProxy>();
    foreach (string downloadedEnumeber in downloadedEnumebers)
    {
      Tuple<string, string> vibAndKi = UtilityFunctions.getVibAndKi(downloadedEnumeber);
      DownloadProxy downloadProxy = new DownloadProxy()
      {
        vib = vibAndKi.Item1,
        ki = vibAndKi.Item2
      };
      toDownloadProxies.Add(downloadProxy);
    }
    return toDownloadProxies;
  }
}
