// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Helpers.ADNetworkRequestTracker
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

// using AppDynamics.Agent; // Usunięte, bo generuje błąd w projekcie Core na Windows

using AppDynamics.Agent;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

#nullable disable
namespace iService5.Core.Services.Helpers;

// Ta klasa jest aktywnie używana przez kod Core, ale AppDynamics nie działa na WinUI.
// Tworzymy tu warunkowy kod, aby uniknąć błędów na platformach, które nie są Androidem/iOS.
// ZMINA ZMIANA ZMIANA ZMIANA

#if ANDROID || IOS || MACCATALYST || TIZEN
// WŁAŚCIWA IMPLEMENTACJA DLA PLATFORM MOBILNYCH (AppDynamics działa)

public class ADNetworkRequestTracker
{
    private static IHttpRequestTracker tracker;

    public ADNetworkRequestTracker(Uri requestUri)
    {
        // Ta linia powoduje błąd "NotImplementedException" na Windows, gdy AppDynamics
        // próbuje użyć natywnych funkcji, których nie ma.
        // ADNetworkRequestTracker.tracker = HttpRequestTracker.Create(requestUri); 
    }

    public void ADTrackException(Exception ex)
    {
        /* ... (pozostawiamy oryginalną logikę z AppDynamics) ... */
    }

    public void ADTrackResponse(int statusCode)
    {
        /* ... (pozostawiamy oryginalną logikę z AppDynamics) ... */
    }
}

#else
// PUSTA IMPLEMENTACJA DLA PLATFORMY WINDOWS (aby uniknąć NotImplementedException)

public class ADNetworkRequestTracker
{
    public ADNetworkRequestTracker(Uri requestUri)
    {
        // Pusty konstruktor na Windows
    }

    public void ADTrackException(Exception ex)
    {
        // Ignorowanie na Windows
    }

    public void ADTrackResponse(int statusCode)
    {
        // Ignorowanie na Windows
    }
}

#endif
