// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.User.ActivityLogging.LoggingService
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Platform;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using System;
using System.IO;

#nullable disable
namespace iService5.Core.Services.User.ActivityLogging;

public class LoggingService : ILoggingService
{
  private Logger log;
  private readonly IPlatformSpecificServiceLocator _locator;

  public LoggingService(IPlatformSpecificServiceLocator locator)
  {
    this._locator = locator;
    this.createLogger();
  }

  private void createLogger()
  {
    try
    {
      string path2 = "iS5Log.txt";
      string str1 = "{Timestamp:dd MMMM yyyy HH:mm:ss.fff} [{Level}] {FileName}:{LineNumber} {Method} {NewLine}{LoggingContext} - {Message:lj}{NewLine}{Exception}";
      string str2 = Path.Combine(this._locator.GetPlatformSpecificService().getFolder(), path2);
      int result1 = 0;
      int result2 = 0;
      if (int.TryParse(BuildProperties.FileSizeLimit, out result1) & int.TryParse(BuildProperties.RetainedFileCountLimit, out result2))
      {
        LoggerSinkConfiguration writeTo = new LoggerConfiguration().MinimumLevel.Debug().Enrich.FromLogContext().WriteTo;
        string path = str2;
        string outputTemplate = str1;
        long? fileSizeLimitBytes = new long?((long) (result1 * 1024 /*0x0400*/ * 1024 /*0x0400*/));
        int? nullable = new int?(result2);
        TimeSpan? flushToDiskInterval = new TimeSpan?();
        int? retainedFileCountLimit = nullable;
        TimeSpan? retainedFileTimeLimit = new TimeSpan?();
        this.log = writeTo.File(path, outputTemplate: outputTemplate, fileSizeLimitBytes: fileSizeLimitBytes, flushToDiskInterval: flushToDiskInterval, rollOnFileSizeLimit: true, retainedFileCountLimit: retainedFileCountLimit, retainedFileTimeLimit: retainedFileTimeLimit).CreateLogger();
      }
      else
        this.createExceptionLogFile("Couldnt fetch RetainedFileCountLimit and FileSizeLimitBytes from configuration");
    }
    catch (Exception ex)
    {
      this.createExceptionLogFile("1. " + ex.ToString());
      this.createExceptionLogFile("2. Message:\n" + ex.Message);
    }
  }

  private void createExceptionLogFile(string error)
  {
    string str = DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss");
    File.WriteAllText(Path.Combine(this._locator.GetPlatformSpecificService().getFolder(), $"error-{str}-.txt"), error);
  }

  public Logger getLogger() => this.log;

  public LoggingContext LoggingContextMask { get; } = LoggingContext.LOGIN | LoggingContext.BACKEND | LoggingContext.METADATA | LoggingContext.LOCAL | LoggingContext.BINARY;
}
