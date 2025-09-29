// Decompiled with JetBrains decompiler
// Type: iService5.Core.ViewModels.GraphicWiFiActivationViewModel
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Appliance;
using iService5.Core.Services.Data;
using iService5.Core.Services.Helpers;
using iService5.Core.Services.Localisation;
using iService5.Core.Services.Platform;
using iService5.Core.Services.User;
using iService5.Core.Services.User.ActivityLogging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
//using Xamarin.Essentials;
//using Xamarin.Forms;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

#nullable disable
namespace iService5.Core.ViewModels;

public class GraphicWiFiActivationViewModel : MvxViewModel
{
  private readonly IMvxNavigationService _navigationService;
  private readonly IMetadataService _metadataService;
  private readonly ILoggingService _loggingService;
  private readonly IPlatformSpecificServiceLocator _locator;
  private readonly IAppliance _appliance;
  private readonly IAlertService _alertService;
  private string extractedSVGFilePath;
  private string _NavText = AppResource.APPLIANCE_FLASH_BACK_TEXT;
  internal string _Instruction;
  private string _WifiStatus;
  private string _RepairEnumber;
  private string _ConnectedColor = "Gray";
  private bool _SmmConnectShow = true;

  public virtual async Task Initialize() => await base.Initialize();

  public bool _viewstatus { get; set; }

  public bool _SmmWithWifi { get; set; }

  public string _ConnectionGraphic { get; private set; }

  public GraphicWiFiActivationViewModel(
    IMvxNavigationService navigationService,
    IShortTextsService _SService,
    ILoggingService loggingService,
    IMetadataService metadataService,
    IPlatformSpecificServiceLocator locator,
    IUserSession userSession,
    IAppliance appliance,
    IAlertService alertService)
  {
    this.DismissGraphic = (ICommand) new Command(new Action(this.DismissGraphicFunction));
    this._navigationService = navigationService;
    this._loggingService = loggingService;
    this._alertService = alertService;
    this._metadataService = metadataService;
    this._appliance = appliance;
    this._locator = locator;
    this._RepairEnumber = userSession.getEnumberSession();
    this._SmmWithWifi = metadataService.isSMMWithWifi(this._RepairEnumber);
    if (this._SmmWithWifi)
    {
      this._ConnectionGraphic = this._metadataService.getConnectionGraphic(this._RepairEnumber);
      this.Instruction = $"{AppResource.GRAPHIC_PAGE_CONNECT_TEXT}\n{AppResource.GRAPHIC_PAGE_CONNECT_TEXT_SUBHEADER}";
    }
    else
    {
      this._ConnectionGraphic = this._metadataService.getConnectionGraphicNonSmm(this._RepairEnumber);
      this.Instruction = AppResource.CONN_PAGE_WAIT_FOR_BRIDGE_DEVICE;
    }
  }

  public string NavText
  {
    get => this._NavText;
    internal set
    {
      this._NavText = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.NavText));
    }
  }

  public string Instruction
  {
    get => this._Instruction;
    internal set
    {
      this._Instruction = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.Instruction));
    }
  }

  private void DismissGraphicFunction()
  {
    this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
  }

  public ICommand DismissGraphic { protected set; get; }

  public override void ViewDisappearing()
  {
    base.ViewDisappearing();
    this._viewstatus = false;
  }

  public override void ViewDisappeared()
  {
    base.ViewDisappeared();
    this._viewstatus = false;
    this.DeleteExtractedSVG();
  }

  private void DeleteExtractedSVG()
  {
    Task.Run((Action) (() =>
    {
      if (this.extractedSVGFilePath == null || !File.Exists(this.extractedSVGFilePath))
        return;
      File.Delete(this.extractedSVGFilePath);
    }));
  }

  public override void ViewAppearing()
  {
    base.ViewAppearing();
    this._viewstatus = true;
  }

  public override void ViewAppeared()
  {
    base.ViewAppeared();
    this._viewstatus = true;
    this._locator.GetPlatformSpecificService().GetLocationConsent();
    this._appliance.CheckConnectivity(new connectivityUpdateCallback(this.updateStatus));
  }

  public string WifiStatus
  {
    get => this._WifiStatus;
    internal set
    {
      this._WifiStatus = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.WifiStatus));
    }
  }

  public string RepairEnumber
  {
    get => this._RepairEnumber;
    internal set
    {
      this._RepairEnumber = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.RepairEnumber));
    }
  }

  public string ConnectedColor
  {
    get => this._ConnectedColor;
    internal set
    {
      this._ConnectedColor = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.ConnectedColor));
    }
  }

  internal void updateStatus()
  {
    if (this._SmmWithWifi)
    {
      this.WifiStatus = this._appliance.StatusOfConnection;
      this.ConnectedColor = this._appliance.ConnectedColor;
      this.SmmConnectShow = this._appliance.boolStatusOfConnection;
    }
    else
    {
      this.WifiStatus = this._appliance.StatusOfBridgeConnection;
      this.ConnectedColor = this._appliance.ConnectedColor;
      this.SmmConnectShow = this._appliance.boolStatusOfBridgeConnection;
    }
  }

  public bool SmmConnectShow
  {
    get => this._SmmConnectShow;
    internal set
    {
      this._SmmConnectShow = !value;
      this.RaisePropertyChanged<bool>((Expression<Func<bool>>) (() => this.SmmConnectShow));
    }
  }

  public void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
  {
    SKImageInfo info = args.Info;
    args.Surface.Canvas.Clear();
    string str = "";
    try
    {
      if (this._ConnectionGraphic == null)
        return;
      ZipArchive archive = ZipFile.OpenRead(Path.Combine(this._locator.GetPlatformSpecificService().getFolder(), this._ConnectionGraphic));
      if (!UtilityFunctions.CheckCompressedFile(archive))
        throw new ArgumentOutOfRangeException("Uncompressed file exceeds compression thresholds.");
      foreach (ZipArchiveEntry entry in archive.Entries)
      {
        str = entry.Name;
        try
        {
          this.extractedSVGFilePath = Path.Combine(this._locator.GetPlatformSpecificService().getFolder(), str);
          entry.ExtractToFile(this.extractedSVGFilePath, true);
        }
        catch (UnauthorizedAccessException ex)
        {
        }
      }
      archive.Dispose();
      this.extractedSVGFilePath = Path.Combine(this._locator.GetPlatformSpecificService().getFolder(), str);
      Stream stream = (Stream) File.OpenRead(this.extractedSVGFilePath);
      Svg.Skia.SKSvg svg = new Svg.Skia.SKSvg();
      svg.Load(stream);
      stream.Dispose();
      this.translationWithSkiaSvg(args, str, svg);
    }
    catch (Exception ex)
    {
      this._alertService.ShowMessageAlertWithKey("SVG_PARSING_FAILED", AppResource.ERROR_TITLE);
      this._loggingService.getLogger().LogAppError(LoggingContext.METADATA, $"Failed to use svg file{this._ConnectionGraphic} content {str}", ex, nameof (OnCanvasViewPaintSurface), "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/GraphicWiFiActivationViewModel.cs", 274);
    }
  }

  public void translationWithSkiaSvg(SKPaintSurfaceEventArgs args, string svgFileName, Svg.Skia.SKSvg svg)
  {
    try
    {
      SKImageInfo info = args.Info;
      SkiaSharp.SKCanvas canvas = args.Surface.Canvas;
      ShimSkiaSharp.SKRect cullRect = svg.Model.CullRect;
      if ((double) cullRect.Width != 0.0 && (double) cullRect.Height != 0.0)
      {
        float num1 = (float) info.Width / cullRect.Width;
        float num2 = (float) info.Height / cullRect.Height;
        float s = Math.Min(num1, num2);
        if (DeviceInfo.Idiom == DeviceIdiom.Phone && DeviceInfo.Platform == DevicePlatform.iOS && DeviceDisplay.MainDisplayInfo.Height > 1700.0)
          num2 = (float) (-(double) cullRect.Width - (double) cullRect.Height / 4.0);
        SkiaSharp.SKMatrix.CreateScaleTranslation(num1, num2, 0.0f, 0.0f);
        canvas.Scale(s);
        canvas.DrawPicture(svg.Picture);
      }
      else
      {
        this._alertService.ShowMessageAlertWithKey("SVG_PARSING_FAILED", AppResource.ERROR_TITLE);
        this._loggingService.getLogger().LogAppError(LoggingContext.LOCAL, $"Failed to use svg file {svgFileName} as viewbox for svg is = {svg.Model.CullRect.ToString()}", memberName: nameof (translationWithSkiaSvg), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/GraphicWiFiActivationViewModel.cs", sourceLineNumber: 308);
      }
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppError(LoggingContext.METADATA, $"Failed to use svg file{this._ConnectionGraphic} content {svgFileName}", ex, nameof (translationWithSkiaSvg), "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/GraphicWiFiActivationViewModel.cs", 314);
    }
  }
}
