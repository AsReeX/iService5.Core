// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.User.ActivityLogging.LoggerExtensions
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using Serilog;
using Serilog.Context;
using System;
using System.IO;
using System.Runtime.CompilerServices;

#nullable disable
namespace iService5.Core.Services.User.ActivityLogging;

public static class LoggerExtensions
{
  public static void LogAppVerbose(
    this ILogger logger,
    LoggingContext context,
    string message,
    Exception exception = null,
    [CallerMemberName] string memberName = "",
    [CallerFilePath] string sourceFilePath = "",
    [CallerLineNumber] int sourceLineNumber = 0)
  {
    LoggerExtensions.SetLogContextProperty(logger, LoggingLevel.Verbose, context, message, exception, memberName, sourceFilePath, sourceLineNumber);
  }

  public static void LogAppDebug(
    this ILogger logger,
    LoggingContext context,
    string message,
    Exception exception = null,
    [CallerMemberName] string memberName = "",
    [CallerFilePath] string sourceFilePath = "",
    [CallerLineNumber] int sourceLineNumber = 0)
  {
    LoggerExtensions.SetLogContextProperty(logger, LoggingLevel.Debug, context, message, exception, memberName, sourceFilePath, sourceLineNumber);
  }

  public static void LogAppInformation(
    this ILogger logger,
    LoggingContext context,
    string message,
    Exception exception = null,
    [CallerMemberName] string memberName = "",
    [CallerFilePath] string sourceFilePath = "",
    [CallerLineNumber] int sourceLineNumber = 0)
  {
    LoggerExtensions.SetLogContextProperty(logger, LoggingLevel.Information, context, message, exception, memberName, sourceFilePath, sourceLineNumber);
  }

  public static void LogAppWarning(
    this ILogger logger,
    LoggingContext context,
    string message,
    Exception exception = null,
    [CallerMemberName] string memberName = "",
    [CallerFilePath] string sourceFilePath = "",
    [CallerLineNumber] int sourceLineNumber = 0)
  {
    LoggerExtensions.SetLogContextProperty(logger, LoggingLevel.WARNING, context, message, exception, memberName, sourceFilePath, sourceLineNumber);
  }

  public static void LogAppError(
    this ILogger logger,
    LoggingContext context,
    string message = "",
    Exception exception = null,
    [CallerMemberName] string memberName = "",
    [CallerFilePath] string sourceFilePath = "",
    [CallerLineNumber] int sourceLineNumber = 0)
  {
    LoggerExtensions.SetLogContextProperty(logger, LoggingLevel.ERROR, context, message, exception, memberName, sourceFilePath, sourceLineNumber);
  }

  public static void LogAppErrorWithParams(
    this ILogger logger,
    LoggingContext context,
    string message = "",
    Exception exception = null,
    [CallerMemberName] string memberName = "",
    [CallerFilePath] string sourceFilePath = "",
    [CallerLineNumber] int sourceLineNumber = 0,
    params object[] parameters)
  {
    LoggerExtensions.SetLogContextProperty(logger, LoggingLevel.ERROR, context, LoggerExtensions.FormatString(message, parameters), exception, memberName, sourceFilePath, sourceLineNumber);
  }

  public static void LogAppFatal(
    this ILogger logger,
    LoggingContext context,
    string message = "",
    Exception exception = null,
    [CallerMemberName] string memberName = "",
    [CallerFilePath] string sourceFilePath = "",
    [CallerLineNumber] int sourceLineNumber = 0)
  {
    LoggerExtensions.SetLogContextProperty(logger, LoggingLevel.FATAL, context, message, exception, memberName, sourceFilePath, sourceLineNumber);
  }

  private static string FormatString(string msg, params object[] parameters)
  {
    string str;
    try
    {
      str = string.Format(msg, parameters);
    }
    catch (Exception ex)
    {
      str = "Error format";
    }
    return str;
  }

  public static void SetLogContextProperty(
    ILogger logger,
    LoggingLevel level,
    LoggingContext context,
    string message,
    Exception exception,
    string memberName,
    string sourceFilePath,
    int sourceLineNumber)
  {
    try
    {
      using (LogContext.PushProperty("LoggingContext", (object) context))
      {
        LogContext.PushProperty("Method", (object) memberName);
        LogContext.PushProperty("FileName", (object) LoggerExtensions.GetFileName(sourceFilePath));
        LogContext.PushProperty("LineNumber", (object) sourceLineNumber);
        switch (level)
        {
          case LoggingLevel.Verbose:
            logger.Verbose(exception, message);
            break;
          case LoggingLevel.Debug:
            logger.Debug(exception, message);
            break;
          case LoggingLevel.Information:
            logger.Information(exception, message);
            break;
          case LoggingLevel.WARNING:
            logger.Warning(exception, message);
            break;
          case LoggingLevel.ERROR:
            logger.Error(exception, message);
            break;
          case LoggingLevel.FATAL:
            logger.Fatal(exception, message);
            break;
          default:
            logger.Information(exception, message);
            break;
        }
      }
    }
    catch (Exception ex)
    {
    }
  }

  public static string GetFileName(string sourceFilePath)
  {
    if (sourceFilePath == null)
      sourceFilePath = "\\uknown";
    if (!sourceFilePath.Contains("\\"))
    {
      sourceFilePath = "\\" + sourceFilePath;
      sourceFilePath = Path.GetFileName(sourceFilePath);
    }
    return sourceFilePath.Substring(sourceFilePath.LastIndexOf('\\') + 1);
  }
}
