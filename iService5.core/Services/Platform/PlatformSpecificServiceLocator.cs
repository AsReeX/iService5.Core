// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Platform.PlatformSpecificServiceLocator
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

// Usunięto 'using Xamarin.Forms;', ponieważ nie jest już potrzebny

#nullable disable
namespace iService5.Core.Services.Platform
{
    // Zmieniono 'internal' na 'public', aby klasa była widoczna dla projektu MAUI
    public class PlatformSpecificServiceLocator : IPlatformSpecificServiceLocator
    {
        // Prywatne pole do przechowywania naszej implementacji serwisu
        private IPlatformSpecificService _service;

        // Metoda zwraca teraz przechowywany serwis
        public IPlatformSpecificService GetPlatformSpecificService()
        {
            if (_service == null)
            {
                throw new System.InvalidOperationException("PlatformSpecificService nie został zainicjowany. Upewnij się, że został ustawiony w MauiProgram.cs.");
            }
            return this._service;
        }

        // Publiczna metoda, która pozwala nam "włożyć" nasz serwis z zewnątrz
        public void set(IPlatformSpecificService service)
        {
            this._service = service;
        }
    }
}