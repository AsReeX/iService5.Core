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
using Microsoft.Maui;

// using Xamarin.Forms; // Usunięto zależność od Xamarin.Forms, bo nie jest już potrzebna

namespace iService5.Core;

public class CoreApp : MvxApplication
{
  private static iService5.Core.Services.Data.SettingsDB settingsDB;
  private static HistoryDB historyDB;

    public override void Initialize()
    {
        // --- ZMIANA #1: Usunięto linię, która tworzyła PUSTY lokalizator ---
        // Ta linia była głównym źródłem błędu.
        // Mvx.IoCProvider.RegisterSingleton<IPlatformSpecificServiceLocator>((IPlatformSpecificServiceLocator) new PlatformSpecificServiceLocator());

        // Ta linia pozostaje bez zmian. Teraz pobierze lokalizator,
        // który został SKONFIGUROWANY WCZEŚNIEJ w MauiProgram.cs

        // Cała reszta oryginalnej rejestracji pozostaje NIETKNIĘTA,
        // ponieważ teraz `locator` jest poprawnie skonfigurowany.

        IPlatformSpecificServiceLocator locator = Mvx.IoCProvider.Resolve<IPlatformSpecificServiceLocator>();
        LoggingService loggingService = new LoggingService(locator);
        AlertService theObject1 = new AlertService((ILoggingService)loggingService);
        SecureStorageService _securestorageService = new SecureStorageService((ILoggingService)loggingService, locator);
        SecurityService theObject2 = new SecurityService((ILoggingService)loggingService, (ISecureStorageService)_securestorageService);
        Mvx.IoCProvider.RegisterSingleton<ISecurityService>((ISecurityService)theObject2);
        Mvx.IoCProvider.RegisterType<IBackend<HttpWebRequest>, iService5.Core.Services.Backend.Backend>();
        Mvx.IoCProvider.RegisterType<ILocalUserAuthentication, LocalUserAuthentication>();
        Mvx.IoCProvider.RegisterType<IBackendUserAuthentication<HttpWebRequest>, BackendUserAuthentication>();
        Mvx.IoCProvider.ConstructAndRegisterSingleton<IShortTextsService, ShortTextsService>();
        Mvx.IoCProvider.RegisterSingleton<ILoggingService>((ILoggingService)loggingService);
        Mvx.IoCProvider.ConstructAndRegisterSingleton<ISecureStorageService, SecureStorageService>();
        Mvx.IoCProvider.ConstructAndRegisterSingleton<ISecurityService, SecurityService>();

        var platformService = Mvx.IoCProvider.Resolve<IPlatformSpecificServiceLocator>().GetPlatformSpecificService();
        if (platformService == null)
            throw new InvalidOperationException("IPlatformSpecificService nie jest dostępny. Błąd w konfiguracji platformy startowej (np. MauiProgram.cs).");


        Mvx.IoCProvider.ConstructAndRegisterSingleton<IBinaryDownloadSession<HttpWebRequest>, BinaryDownloadSession>();
        Mvx.IoCProvider.ConstructAndRegisterSingleton<ISigningCertificate<HttpWebRequest>, SigningCertificate>();
        Mvx.IoCProvider.RegisterSingleton<IUserAccount>((IUserAccount)new UserAccount());
        Mvx.IoCProvider.RegisterSingleton<IAlertService>((IAlertService)theObject1);
        Mvx.IoCProvider.RegisterSingleton<IWebSocketService>((IWebSocketService)new WebSocketService((ILoggingService)loggingService, (ISecurityService)theObject2, locator, (ISecureStorageService)_securestorageService));
        this.ClearKeyChainOnFirstLaunch();
        if (UtilityFunctions.isDeviceTimeZoneChina())
            Mvx.IoCProvider.RegisterSingleton<IBackendDetails<HttpWebRequest>>((IBackendDetails<HttpWebRequest>)new BackendDetails(BuildProperties.protocol, BuildProperties.server_China, BuildProperties.basepath, BuildProperties.version, BuildProperties.stage_China, BuildProperties.apikey_China));
        else
            Mvx.IoCProvider.RegisterSingleton<IBackendDetails<HttpWebRequest>>((IBackendDetails<HttpWebRequest>)new BackendDetails(BuildProperties.protocol, BuildProperties.server, BuildProperties.basepath, BuildProperties.version, BuildProperties.stage, BuildProperties.apikey));
        Mvx.IoCProvider.ConstructAndRegisterSingleton<IAppliance, iService5.Core.Services.Appliance.Appliance>();
        Mvx.IoCProvider.ConstructAndRegisterSingleton<IConnectivityService, ConnectivityService>();
        Mvx.IoCProvider.ConstructAndRegisterSingleton<IMetadataService, MetadataService>();
        Mvx.IoCProvider.ConstructAndRegisterSingleton<IUserSession, UserSession>();
        Mvx.IoCProvider.ConstructAndRegisterSingleton<IVersionReport, iService5.Core.Services.VersionReport.VersionReport>();
        Mvx.IoCProvider.ConstructAndRegisterSingleton<IApplianceSession, ApplianceSession>();
        Stream streamFromString = UtilityFunctions.GenerateStreamFromString(Mvx.IoCProvider.Resolve<IMetadataService>().getSSHKeyValue());
        if (streamFromString.Length > 0L)
            Mvx.IoCProvider.RegisterSingleton<Is5SshWrapper>(new Is5SshWrapper(BuildProperties.LoginUser, streamFromString, (ILoggingService)loggingService));
        AppResource.Culture = new CultureInfo(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToLowerInvariant());
        SslValidator.initialize();
    
    //    UtilityFunctions.CleanOldLogFiles();
        this.RegisterAppStart<LoginViewModel>();
    //     var settingsDbPath = Path.Combine(platformService.getFolder(), "Settings.db3");
    //            Mvx.IoCProvider.RegisterSingleton<SettingsDB>(new SettingsDB(settingsDbPath));
    //            var historyDbPath = Path.Combine(platformService.getFolder(), "History.db3");
    //            Mvx.IoCProvider.RegisterSingleton<HistoryDB>(new HistoryDB(historyDbPath));

    }
    public static iService5.Core.Services.Data.SettingsDB settings
    {
        get
        {
            if (CoreApp.settingsDB == null)
            {
                // --- POPRAWIONY KOD ---
                // Używamy teraz JEDNEGO, spójnego i poprawnego sposobu na pobranie serwisu platformy.
                var platformService = Mvx.IoCProvider.Resolve<IPlatformSpecificServiceLocator>().GetPlatformSpecificService();

                if (platformService != null)
                {
                    // Tworzymy bazę danych, używając poprawnie pobranej ścieżki do folderu.
                    var dbPath = Path.Combine(platformService.getFolder(), "Settings.db3");
                    CoreApp.settingsDB = new iService5.Core.Services.Data.SettingsDB(dbPath);
                }
                else
                {
                    // Ta sytuacja nie powinna już wystąpić, ale zostawiamy zabezpieczenie.
                    Console.WriteLine("BŁĄD KRYTYCZNY: Nie można utworzyć SettingsDB, ponieważ IPlatformSpecificService jest null.");
                }
            }
            return CoreApp.settingsDB;
        }
    }

    public static HistoryDB history
    {
        get
        {
            if (CoreApp.historyDB == null)
            {
                // --- POPRAWIONY KOD ---
                // Używamy tego samego, poprawnego mechanizmu, co dla SettingsDB.
                var platformService = Mvx.IoCProvider.Resolve<IPlatformSpecificServiceLocator>().GetPlatformSpecificService();

                if (platformService != null)
                {
                    var dbPath = Path.Combine(platformService.getFolder(), "History.db3");
                    CoreApp.historyDB = new HistoryDB(dbPath);
                }
                else
                {
                    Console.WriteLine("BŁĄD KRYTYCZNY: Nie można utworzyć HistoryDB, ponieważ IPlatformSpecificService jest null.");
                }
            }
            return CoreApp.historyDB;
        }
    }

    private void ClearKeyChainOnFirstLaunch()
    {
        // --- POPRAWIONY KOD ---
        // Zastępujemy przestarzały `DependencyService` nowoczesnym mechanizmem MvvmCross.
        var platformService = Mvx.IoCProvider.Resolve<IPlatformSpecificServiceLocator>().GetPlatformSpecificService();

        // Dodajemy zabezpieczenie przed `null`, aby uniknąć awarii.
        if (platformService == null || string.IsNullOrEmpty(platformService.getFolder()))
        {
            return;
        }

        if (System.IO.File.Exists(Path.Combine(platformService.getFolder(), "Settings.db3")))
            return;

        // Reszta metody pozostaje bez zmian.
        Mvx.IoCProvider.Resolve<ISecureStorageService>().RemoveAll();
        Mvx.IoCProvider.Resolve<ILoggingService>().getLogger().LogAppInformation(LoggingContext.USER, "Cleared Key chain values on First Launch!", memberName: nameof(ClearKeyChainOnFirstLaunch), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/Core/CoreApp.cs", sourceLineNumber: 160);
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
