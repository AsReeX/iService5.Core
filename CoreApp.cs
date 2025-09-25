// Decompiled with JetBrains decompiler
// Type: iService5.Core.CoreApp
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Appliance;
using iService5.Core.Services.Backend;
using iService5.Core.Services.Data;
using iService5.Core.Services.Helpers;
using iService5.Core.Services.Local;
using iService5.Core.Services.Localisation;
using iService5.Core.Services.Platform;
using iService5.Core.Services.Security;
using iService5.Core.Services.User;
using iService5.Core.Services.User.ActivityLogging;
using iService5.Core.Services.VersionReport;
using iService5.Core.Services.WebSocket;
using iService5.Core.ViewModels;
using MvvmCross;
using MvvmCross.IoC;
using MvvmCross.ViewModels;
using System;
using System.Globalization;
using System.IO;
using System.Net;
using Xamarin.Forms;

#nullable disable
namespace iService5.Core;

public class CoreApp : MvxApplication
{
  private static iService5.Core.Services.Data.SettingsDB settingsDB;
  private static HistoryDB historyDB;

  public override void Initialize()
  {
    Mvx.IoCProvider.RegisterSingleton<IPlatformSpecificServiceLocator>((IPlatformSpecificServiceLocator) new PlatformSpecificServiceLocator());
    IPlatformSpecificServiceLocator locator = Mvx.IoCProvider.Resolve<IPlatformSpecificServiceLocator>();
    LoggingService loggingService = new LoggingService(locator);
    AlertService theObject1 = new AlertService((ILoggingService) loggingService);
    SecureStorageService _securestorageService = new SecureStorageService((ILoggingService) loggingService, locator);
    SecurityService theObject2 = new SecurityService((ILoggingService) loggingService, (ISecureStorageService) _securestorageService);
    Mvx.IoCProvider.RegisterSingleton<ISecurityService>((ISecurityService) theObject2);
    Mvx.IoCProvider.RegisterType<IBackend<HttpWebRequest>, iService5.Core.Services.Backend.Backend>();
    Mvx.IoCProvider.RegisterType<ILocalUserAuthentication, LocalUserAuthentication>();
    Mvx.IoCProvider.RegisterType<IBackendUserAuthentication<HttpWebRequest>, BackendUserAuthentication>();
    Mvx.IoCProvider.ConstructAndRegisterSingleton<IShortTextsService, ShortTextsService>();
    Mvx.IoCProvider.RegisterSingleton<ILoggingService>((ILoggingService) loggingService);
    Mvx.IoCProvider.ConstructAndRegisterSingleton<ISecureStorageService, SecureStorageService>();
    Mvx.IoCProvider.ConstructAndRegisterSingleton<ISecurityService, SecurityService>();
    Mvx.IoCProvider.ConstructAndRegisterSingleton<IBinaryDownloadSession<HttpWebRequest>, BinaryDownloadSession>();
    Mvx.IoCProvider.ConstructAndRegisterSingleton<ISigningCertificate<HttpWebRequest>, SigningCertificate>();
    Mvx.IoCProvider.RegisterSingleton<IUserAccount>((IUserAccount) new UserAccount());
    Mvx.IoCProvider.RegisterSingleton<IAlertService>((IAlertService) theObject1);
    Mvx.IoCProvider.RegisterSingleton<IWebSocketService>((IWebSocketService) new WebSocketService((ILoggingService) loggingService, (ISecurityService) theObject2, locator, (ISecureStorageService) _securestorageService));
    this.ClearKeyChainOnFirstLaunch();
    if (UtilityFunctions.isDeviceTimeZoneChina())
      Mvx.IoCProvider.RegisterSingleton<IBackendDetails<HttpWebRequest>>((IBackendDetails<HttpWebRequest>) new BackendDetails(BuildProperties.protocol, BuildProperties.server_China, BuildProperties.basepath, BuildProperties.version, BuildProperties.stage_China, BuildProperties.apikey_China));
    else
      Mvx.IoCProvider.RegisterSingleton<IBackendDetails<HttpWebRequest>>((IBackendDetails<HttpWebRequest>) new BackendDetails(BuildProperties.protocol, BuildProperties.server, BuildProperties.basepath, BuildProperties.version, BuildProperties.stage, BuildProperties.apikey));
    Mvx.IoCProvider.ConstructAndRegisterSingleton<IAppliance, iService5.Core.Services.Appliance.Appliance>();
    Mvx.IoCProvider.ConstructAndRegisterSingleton<IConnectivityService, ConnectivityService>();
    Mvx.IoCProvider.ConstructAndRegisterSingleton<IMetadataService, MetadataService>();
    Mvx.IoCProvider.ConstructAndRegisterSingleton<IUserSession, UserSession>();
    Mvx.IoCProvider.ConstructAndRegisterSingleton<IVersionReport, iService5.Core.Services.VersionReport.VersionReport>();
    Mvx.IoCProvider.ConstructAndRegisterSingleton<IApplianceSession, ApplianceSession>();
    Stream streamFromString = UtilityFunctions.GenerateStreamFromString(Mvx.IoCProvider.Resolve<IMetadataService>().getSSHKeyValue());
    if (streamFromString.Length > 0L)
      Mvx.IoCProvider.RegisterSingleton<Is5SshWrapper>(new Is5SshWrapper(BuildProperties.LoginUser, streamFromString, (ILoggingService) loggingService));
    AppResource.Culture = new CultureInfo(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToLowerInvariant());
    SslValidator.initialize();
    UtilityFunctions.CleanOldLogFiles();
    this.RegisterAppStart<LoginViewModel>();
  }

  public static iService5.Core.Services.Data.SettingsDB settings
  {
    get
    {
      if (CoreApp.settingsDB == null)
      {
        if (Mvx.IoCProvider.Resolve<IPlatformSpecificServiceLocator>().GetPlatformSpecificService() == null)
        {
          try
          {
            CoreApp.settingsDB = new iService5.Core.Services.Data.SettingsDB(Path.Combine(DependencyService.Get<IPlatformSpecificService>().getFolder(), "Settings.db3"));
            CoreApp.settingsDB.SaveItem(new Settings("BridgeOff", "true"));
          }
          catch (Exception ex)
          {
            Console.WriteLine("Settings db creation error : " + ex.Message);
          }
        }
        else
          CoreApp.settingsDB = new iService5.Core.Services.Data.SettingsDB(Path.Combine(Mvx.IoCProvider.Resolve<IPlatformSpecificServiceLocator>().GetPlatformSpecificService().getFolder(), "Settings.db3"));
      }
      return CoreApp.settingsDB;
    }
  }

  public static HistoryDB history
  {
    get
    {
      if (CoreApp.historyDB == null)
        CoreApp.historyDB = Mvx.IoCProvider.Resolve<IPlatformSpecificServiceLocator>().GetPlatformSpecificService() != null ? new HistoryDB(Path.Combine(Mvx.IoCProvider.Resolve<IPlatformSpecificServiceLocator>().GetPlatformSpecificService().getFolder(), "History.db3")) : new HistoryDB(Path.Combine(DependencyService.Get<IPlatformSpecificService>().getFolder(), "History.db3"));
      return CoreApp.historyDB;
    }
  }

  private void ClearKeyChainOnFirstLaunch()
  {
    if (System.IO.File.Exists(Path.Combine(DependencyService.Get<IPlatformSpecificService>().getFolder(), "Settings.db3")))
      return;
    Mvx.IoCProvider.Resolve<ISecureStorageService>().RemoveAll();
    Mvx.IoCProvider.Resolve<ILoggingService>().getLogger().LogAppInformation(LoggingContext.USER, "Cleared Key chain values on First Launch!", memberName: nameof (ClearKeyChainOnFirstLaunch), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/CoreApp.cs", sourceLineNumber: 160 /*0xA0*/);
  }

  public enum EventsNames
  {
    MetadataFailed,
    MetadataSuccess,
    MetadataUpdated,
    TokenExpiredPopUpShown,
    BridgeConnected,
    BridgeDisconnected,
    ForegroundEvent,
    ApplianceBinariesFinished,
    InstallRepairPopupRaised,
    InstallRepairPopupDismissed,
    CertificateUpdated,
    FeedBackFormPending,
  }
}
