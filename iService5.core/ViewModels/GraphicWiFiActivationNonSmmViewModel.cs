// Decompiled with JetBrains decompiler
// Type: iService5.Core.ViewModels.GraphicWiFiActivationNonSmmViewModel
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
//using Xamarin.Forms;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

#nullable disable
namespace iService5.Core.ViewModels;

public class GraphicWiFiActivationNonSmmViewModel : MvxViewModel
{
  private readonly IMvxNavigationService _navigationService;
  private readonly ILoggingService _loggingService;
  private readonly IPlatformSpecificServiceLocator _locator;
  private readonly IAppliance _appliance;
  private string _NavText = AppResource.APPLIANCE_FLASH_BACK_TEXT;
  private string _Instruction = AppResource.GRAPHIC_PAGE_CONNECT_TEXT;
  private string _WifiStatus;
  private string _RepairEnumber;
  private string _ConnectedColor = "Gray";
  private bool _SmmConnectShow = true;

  public virtual async Task Initialize() => await base.Initialize();

  public bool _viewstatus { get; set; }

  public string _ConnectionGraphic { get; internal set; }

  public GraphicWiFiActivationNonSmmViewModel(
    IMvxNavigationService navigationService,
    IShortTextsService _SService,
    ILoggingService loggingService,
    IMetadataService metadataService,
    IPlatformSpecificServiceLocator locator,
    IUserSession userSession,
    IAppliance appliance)
  {
    this.DismissGraphic = (ICommand) new Command(new Action(this.DismissGraphicFunction));
    this._navigationService = navigationService;
    this._loggingService = loggingService;
    this._appliance = appliance;
    this._locator = locator;
    this._RepairEnumber = userSession.getEnumberSession();
    this._ConnectionGraphic = metadataService.getConnectionGraphicNonSmm(this._RepairEnumber);
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

  internal void DismissGraphicFunction()
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
    this.WifiStatus = this._appliance.StatusOfConnection;
    this.ConnectedColor = this._appliance.ConnectedColor;
    this.SmmConnectShow = this._appliance.boolStatusOfConnection;
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
    SKCanvas canvas = args.Surface.Canvas;
    canvas.Clear();
    string path2 = "";
    try
    {
      if (this._ConnectionGraphic == null)
        return;
      ZipArchive archive = ZipFile.OpenRead(Path.Combine(this._locator.GetPlatformSpecificService().getFolder(), this._ConnectionGraphic));
      if (!UtilityFunctions.CheckCompressedFile(archive))
        throw new ArgumentOutOfRangeException("Uncompressed file exceeds compression thresholds.");
      foreach (ZipArchiveEntry entry in archive.Entries)
      {
        path2 = entry.Name;
        try
        {
          entry.ExtractToFile(Path.Combine(this._locator.GetPlatformSpecificService().getFolder(), path2), true);
        }
        catch (UnauthorizedAccessException ex)
        {
        }
      }
      Stream stream = (Stream) File.OpenRead(Path.Combine(this._locator.GetPlatformSpecificService().getFolder(), path2));
      SkiaSharp.Extended.Svg.SKSvg skSvg = new SkiaSharp.Extended.Svg.SKSvg();
      skSvg.Load(stream);
      canvas.Translate((float) info.Width / 2f, (float) info.Height / 2f);
      SKRect viewBox = skSvg.ViewBox;
      float s = Math.Min((float) info.Width / viewBox.Width, (float) info.Height / viewBox.Height);
      canvas.Scale(s);
      canvas.Translate(-viewBox.MidX, -viewBox.MidY);
      canvas.DrawPicture(skSvg.Picture);
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppError(LoggingContext.METADATA, $"Failed to use svg file{this._ConnectionGraphic} content {path2}", ex, nameof (OnCanvasViewPaintSurface), "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/GraphicWiFiActivationNonSmmViewModel.cs", 237);
    }
  }
}
