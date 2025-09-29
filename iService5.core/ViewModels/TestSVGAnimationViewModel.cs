// Decompiled with JetBrains decompiler
// Type: iService5.Core.ViewModels.TestSVGAnimationViewModel
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.User.ActivityLogging;
using MvvmCross.ViewModels;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Input;
//using Xamarin.Forms;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

#nullable disable
namespace iService5.Core.ViewModels;

public class TestSVGAnimationViewModel : MvxViewModel
{
  private readonly TestSVGAnimationViewModel.AnimatedSvg hoodFan = new TestSVGAnimationViewModel.AnimatedSvg()
  {
    SVGFileName = "hood_fan_4_2.svg",
    CurrentState = 1,
    FrameIndex = 0
  };
  private readonly TestSVGAnimationViewModel.AnimatedSvg AnimatedSVG;

  public ILoggingService _loggingService { get; private set; }

  public ICommand ChangeAnimationStateCommand { internal set; get; }

  public TestSVGAnimationViewModel(ILoggingService loggingService)
  {
    this._loggingService = loggingService;
    this.ChangeAnimationStateCommand = (ICommand) new Command(new Action(this.ChangeAnimationState));
    this.AnimatedSVG = this.hoodFan;
    this.GetStatesAndFramesOfSVGFile(this.AnimatedSVG);
  }

  public void GetStatesAndFramesOfSVGFile(TestSVGAnimationViewModel.AnimatedSvg animatedSvg)
  {
    string[] strArray = animatedSvg.SVGFileName.Split('_', '.');
    int result1;
    int.TryParse(strArray[strArray.Length - 3], out result1);
    int result2;
    int.TryParse(strArray[strArray.Length - 2], out result2);
    animatedSvg.NOfFrames = result1;
    animatedSvg.NOfStates = result2;
    animatedSvg.FrameLimit = animatedSvg.NOfFrames;
  }

  public void ChangeAnimationState()
  {
    ++this.AnimatedSVG.CurrentState;
    if (this.AnimatedSVG.CurrentState > this.AnimatedSVG.NOfStates)
      this.AnimatedSVG.CurrentState = 1;
    this.AnimatedSVG.FrameIndex = (this.AnimatedSVG.CurrentState - 1) * this.AnimatedSVG.NOfFrames;
    this.AnimatedSVG.FrameLimit = this.AnimatedSVG.CurrentState * this.AnimatedSVG.NOfFrames;
  }

  public void OnCanvasViewPaintInstructions(Assembly assembly, SKPaintSurfaceEventArgs args)
  {
    SKCanvas canvas = args.Surface.Canvas;
    canvas.Clear();
    try
    {
      Stream stream = new iService5.Core.Services.Helpers.SVGParser.SVGParser(this.AnimatedSVG.SVGFileName, this._loggingService).GetMonitoringGraphicsSVG().GetStream(this.AnimatedSVG.FrameIndex);
      SkiaSharp.Extended.Svg.SKSvg skSvg = new SkiaSharp.Extended.Svg.SKSvg();
      skSvg.Load(stream);
      SKMatrix scale = SKMatrix.CreateScale(10f, 10f);
      canvas.DrawPicture(skSvg.Picture, ref scale);
      ++this.AnimatedSVG.FrameIndex;
      if (this.AnimatedSVG.FrameIndex < this.AnimatedSVG.FrameLimit)
        return;
      this.AnimatedSVG.FrameIndex = this.AnimatedSVG.CurrentState * this.AnimatedSVG.NOfFrames - this.AnimatedSVG.NOfFrames;
    }
    catch (Exception ex)
    {
      this._loggingService.getLogger().LogAppError(LoggingContext.METADATA, "Failed to use svg file" + this.AnimatedSVG.SVGFileName, ex, nameof (OnCanvasViewPaintInstructions), "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/TestSVGAnimationViewModel.cs", 94);
    }
  }

  public class AnimatedSvg
  {
    public string SVGFileName { get; set; }

    public int NOfFrames { get; set; }

    public int NOfStates { get; set; }

    public int CurrentState { get; set; }

    public int FrameIndex { get; set; }

    public int FrameLimit { get; set; }
  }
}
