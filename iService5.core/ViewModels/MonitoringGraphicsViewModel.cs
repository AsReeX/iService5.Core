// Decompiled with JetBrains decompiler
// Type: iService5.Core.ViewModels.MonitoringGraphicsViewModel
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Appliance;
using iService5.Core.Services.Data;
using iService5.Core.Services.Helpers;
using iService5.Core.Services.Helpers.SVGParser;
using iService5.Core.Services.Helpers.Topogram;
using iService5.Core.Services.Localisation;
using iService5.Core.Services.Platform;
using iService5.Core.Services.User;
using iService5.Core.Services.User.ActivityLogging;
using iService5.Core.Services.VersionReport;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using Newtonsoft.Json;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
//using Xamarin.Forms;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

#nullable disable
namespace iService5.Core.ViewModels;

public class MonitoringGraphicsViewModel : MvxViewModel<bool>
{
    private readonly ILoggingService _loggingService;
    private readonly IUserSession _userSession;
    private readonly IPlatformSpecificServiceLocator _locator;
    private readonly IAlertService _alertService;
    private readonly IMetadataService _metadataService;
    private readonly IMvxNavigationService _navigationService;
    private readonly IAppliance _appliance;
    internal iService5.Core.Services.Helpers.Topogram.Topogram topogram;
    internal List<TopogramComponent> topogramComponents = new List<TopogramComponent>();
    internal MvxObservableCollection<Screen> _screenList = new MvxObservableCollection<Screen>();
    internal MvxObservableCollection<Screen> _extraList = new MvxObservableCollection<Screen>();
    private List<MonitoringGraphicsViewModel.TopogramSvg> topogramSvgs = new List<MonitoringGraphicsViewModel.TopogramSvg>();
    private List<MonitoringGraphicsViewModel.TopogramSvg> renderedTopogramSvgs = new List<MonitoringGraphicsViewModel.TopogramSvg>();
    internal string monitoringGraphicsEnumber = "Z9KDKD0Z01(00)";
    internal string monitoringGraphicsDirectory = "MonitoringGraphics";
    private readonly Is5SshWrapper _sshWrapper;
    internal bool FetchingComponentStates = true;
    internal Task _DisconnectionTask = (Task)null;
    internal CancellationTokenSource _tokenSource = (CancellationTokenSource)null;
    internal bool firstReception = true;
    public string tempJson = (string)null;
    public bool IntialFetch = true;
    private Dictionary<int, string> feedback = new Dictionary<int, string>();
    private int retrievalDelay = 0;
    private string _NavText = AppResource.APPLIANCE_FLASH_BACK_TEXT;
    private string _RepairEnumber = nameof(RepairEnumber);
    private string _WifiBridgeStatus = "• " + AppResource.CONNECTED_TEXT;
    private string _ConnectedColor = "Green";
    private string _ScreenTitle;
    private string _NoScreensText = AppResource.MONITORING_NO_SCREENS_TEXT;
    private string _selectedItemColor;
    private int _screenIndex;
    private Screen _hItem;
    internal string SvgFilename;
    private bool displayActivityIndicator = true;

    public string monitoringGraphic { get; private set; }

    public Action RefreshViewAction { get; set; }

    public bool NoScreens { get; set; }

    public string starterJson { get; set; }

    public MonitoringGraphicsViewModel(
      IMvxNavigationService navigationService,
      IPlatformSpecificServiceLocator locator,
      IUserSession userSession,
      ILoggingService loggingService,
      IShortTextsService _ShortTextsService,
      IVersionReport versionReport,
      IAlertService alertService,
      IMetadataService metadataService,
      IAppliance appliance,
      Is5SshWrapper sshWrapper)
    {
        this._userSession = userSession;
        this._locator = locator;
        this._alertService = alertService;
        this._metadataService = metadataService;
        this._navigationService = navigationService;
        this._appliance = appliance;
        this._loggingService = loggingService;
        this._sshWrapper = sshWrapper;
        this.GoBackCommand = (ICommand)new Command(new Action(this.GoBack));
        this.RepairEnumber = this._userSession.getEnumberSession();
        try
        {
            this.topogram = new TopogramParser(this.RepairEnumber, this._loggingService).GetTopogram();
            List<TopogramScreen> allScreens = this.topogram.GetAllScreens();
            this.screenList = new MvxObservableCollection<Screen>();
            for (int index = 0; index < allScreens.Count; ++index)
                ((Collection<Screen>)this.screenList).Add(new Screen()
                {
                    Id = index,
                    Title = allScreens[index].GetScreenName(),
                    IsSelected = index == 0
                });
            this.extraList = this.screenList;
            this.screenIndex = 0;
            this.selItem = ((IEnumerable<Screen>)this.screenList).First<Screen>();
            this.SwipeLeftCommand = (ICommand)new Command((Action)(() =>
            {
                if (this.selItem == ((IEnumerable<Screen>)this.screenList).Last<Screen>())
                    return;
                this.selItem = ((IEnumerable<Screen>)this.screenList).SingleOrDefault<Screen>((Func<Screen, bool>)(x => x.Id == this.selItem.Id + 1));
            }));
            this.SwipeRightCommand = (ICommand)new Command((Action)(() =>
            {
                if (this.selItem == ((IEnumerable<Screen>)this.screenList).First<Screen>())
                    return;
                this.selItem = ((IEnumerable<Screen>)this.screenList).SingleOrDefault<Screen>((Func<Screen, bool>)(x => x.Id == this.selItem.Id - 1));
            }));
        }
        catch (Exception ex)
        {
            this._alertService.ShowMessageAlertWithKey("MONITORING_PARSING_ERROR", AppResource.ERROR_TITLE);
            loggingService.getLogger().LogAppError(LoggingContext.MONITORING, "Error while parsing topogram: " + ex.Message, memberName: ".ctor", sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/MonitoringGraphicsViewModel.cs", sourceLineNumber: 112 /*0x70*/);
            this.DisplayActivityIndicator = false;
            this.NoScreens = true;
        }
        try
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            CoreApp.history.SaveItem(new History(this.RepairEnumber, DateTime.Now, UtilityFunctions.getSessionIDForHistoryTable(), HistoryDBInfoType.MonitoringLog.ToString(), stringBuilder.ToString()));
        }
        catch (Exception ex)
        {
            this._loggingService.getLogger().LogAppInformation(LoggingContext.MONITORING, "Failed to save item in the History DB, " + ex?.ToString(), memberName: ".ctor", sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/MonitoringGraphicsViewModel.cs", sourceLineNumber: 125);
        }
        this.ExtractMonitoringGraphics();
    }

    public ICommand GoBackCommand { internal set; get; }

    public ICommand DisplayMonitoringGraphics { internal set; get; }

    public ICommand SwipeLeftCommand { internal set; get; }

    public ICommand SwipeRightCommand { internal set; get; }

    private void SaveJsonData(int id, string tempJson)
    {
        if (string.IsNullOrEmpty(tempJson))
            return;
        if (this.feedback.ContainsKey(id))
            this.feedback[id] = tempJson;
        else
            this.feedback.Add(id, tempJson);
    }

    public string RetrieveJsonData(int id)
    {
        if (!this.feedback.ContainsKey(id))
            return string.Empty;
        try
        {
            JsonConvert.DeserializeObject<Values>(this.feedback[id]);
        }
        catch (Exception ex)
        {
            this._loggingService.getLogger().LogAppError(LoggingContext.MONITORING, "Error during JSON response parsing in Retrival: " + ex.Message, memberName: nameof(RetrieveJsonData), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/MonitoringGraphicsViewModel.cs", sourceLineNumber: 161);
            return string.Empty;
        }
        return this.feedback[id];
    }

    public MvxObservableCollection<Screen> screenList
    {
        get => this._screenList;
        set
        {
            this._screenList = value;
            this.RaisePropertyChanged<MvxObservableCollection<Screen>>((Expression<Func<MvxObservableCollection<Screen>>>)(() => this.screenList));
        }
    }

    public MvxObservableCollection<Screen> extraList
    {
        get => this._extraList;
        set
        {
            this._extraList = value;
            this.RaisePropertyChanged<MvxObservableCollection<Screen>>((Expression<Func<MvxObservableCollection<Screen>>>)(() => this.extraList));
        }
    }

    public string NavText
    {
        get => this._NavText;
        internal set
        {
            this._NavText = value;
            this.RaisePropertyChanged<string>((Expression<Func<string>>)(() => this.NavText));
        }
    }

    public string RepairEnumber
    {
        get => this._RepairEnumber;
        internal set
        {
            this._RepairEnumber = value;
            this.RaisePropertyChanged<string>((Expression<Func<string>>)(() => this.RepairEnumber));
        }
    }

    public string WifiBridgeStatus
    {
        get => this._WifiBridgeStatus;
        internal set
        {
            this._WifiBridgeStatus = value;
            this.RaisePropertyChanged<string>((Expression<Func<string>>)(() => this.WifiBridgeStatus));
        }
    }

    public string ConnectedColor
    {
        get => this._ConnectedColor;
        internal set
        {
            this._ConnectedColor = value;
            this.RaisePropertyChanged<string>((Expression<Func<string>>)(() => this.ConnectedColor));
        }
    }

    public string ScreenTitle
    {
        get => this._ScreenTitle;
        internal set
        {
            this._ScreenTitle = value;
            this.RaisePropertyChanged<string>((Expression<Func<string>>)(() => this.ScreenTitle));
        }
    }

    public string NoScreensText
    {
        get => this._NoScreensText;
        internal set
        {
            this._NoScreensText = value;
            this.RaisePropertyChanged<string>((Expression<Func<string>>)(() => this.NoScreensText));
        }
    }

    public string selectedItemColor
    {
        get => this._selectedItemColor;
        set
        {
            this._selectedItemColor = value;
            Device.BeginInvokeOnMainThread((Action)(() =>
            {
                for (int index = 0; index < ((Collection<Screen>)this.screenList).Count; ++index)
                {
                    if (this.screenIndex == ((Collection<Screen>)this.extraList)[index].Id)
                        ((Collection<Screen>)this.extraList)[index].IsSelected = true;
                    else
                        ((Collection<Screen>)this.extraList)[index].IsSelected = false;
                }
                this.screenList = new MvxObservableCollection<Screen>((IEnumerable<Screen>)this.extraList);
                this.RaisePropertyChanged<MvxObservableCollection<Screen>>((Expression<Func<MvxObservableCollection<Screen>>>)(() => this.extraList));
            }));
            this.RaisePropertyChanged<string>((Expression<Func<string>>)(() => this.selectedItemColor));
        }
    }

    public int screenIndex
    {
        get => this._screenIndex;
        set
        {
            this._screenIndex = value;
            this.RaisePropertyChanged<int>((Expression<Func<int>>)(() => this.screenIndex));
        }
    }

    public Screen selItem
    {
        get => this._hItem;
        set
        {
            if (this.selItem != null)
                this.SaveJsonData(this.selItem.Id, this.tempJson);
            this._hItem = value;
            this.screenIndex = value.Id;
            this.ScreenTitle = value.Title;
            this.topogramComponents = this.topogram.GetScreen(this.screenIndex).GetComponents();
            this.topogramSvgs = new List<MonitoringGraphicsViewModel.TopogramSvg>();
            foreach (TopogramComponent topogramComponent in this.topogramComponents)
            {
                List<MonitoringGraphicsViewModel.TopogramSvg> topogramSvgs = this.topogramSvgs;
                MonitoringGraphicsViewModel.TopogramSvg topogramSvg1 = new MonitoringGraphicsViewModel.TopogramSvg();
                topogramSvg1.CurrentState = 1;
                topogramSvg1.FrameIndex = 0;
                topogramSvg1.FrameLimit = topogramComponent.GetNumberOfFrames();
                topogramSvg1.NOfFrames = topogramComponent.GetNumberOfFrames();
                topogramSvg1.NOfStates = topogramComponent.GetNumberOfStates();
                topogramSvg1.SVGFileName = topogramComponent.GetFileName();
                topogramSvg1.AnimationType = topogramComponent.GetAnimationType();
                topogramSvg1.XPosition = (float)topogramComponent.GetXPosition();
                topogramSvg1.YPosition = (float)topogramComponent.GetYPosition();
                topogramSvg1.Name = topogramComponent.GetName();
                topogramSvg1.DisplayName = topogramComponent.GetDisplayName();
                topogramSvg1.NumericDisplay = topogramComponent.GetNumericDisplay();
                MonitoringGraphicsViewModel.TopogramSvg topogramSvg2 = topogramSvg1;
                string[] strArray;
                if (!(topogramComponent.GetDependencies() == ""))
                    strArray = topogramComponent.GetDependencies().Split(',');
                else
                    strArray = new string[0];
                topogramSvg2.Dependencies = strArray;
                topogramSvg1.Type = topogramComponent.GetComponentType();
                topogramSvg1.Values = topogramComponent.GetValues();
                topogramSvg1.IsRendered = topogramComponent.GetComponentType() != TopogramComponentType.PASSIVE || topogramComponent.GetValues().Count != 0;
                topogramSvg1.IsStateValueChanging = false;
                MonitoringGraphicsViewModel.TopogramSvg topogramSvg3 = topogramSvg1;
                topogramSvgs.Add(topogramSvg3);
            }
            this.renderedTopogramSvgs = this.topogramSvgs;
            this.selectedItemColor = "Red";
            this.retrievalDelay = 0;
            this.RaisePropertyChanged<Screen>((Expression<Func<Screen>>)(() => this.selItem));
        }
    }

    public bool DisplayActivityIndicator
    {
        get => this.displayActivityIndicator;
        internal set
        {
            this.displayActivityIndicator = value;
            this.RaisePropertyChanged<bool>((Expression<Func<bool>>)(() => this.DisplayActivityIndicator));
        }
    }

    public override void ViewAppeared()
    {
        base.ViewAppeared();
        this._appliance.CheckConnectivity(new connectivityUpdateCallback(this.ConnectivityCheck));
        MessagingCenter.Send<MonitoringGraphicsViewModel>(this, "InvokeGetValues");
        if (this.NoScreens)
            return;
        this.ChangeAnimationState();
    }

    public override void ViewDisappearing() => base.ViewDisappearing();

    public void OnCanvasViewPaintInstructions(Assembly assembly, SKPaintSurfaceEventArgs args)
    {
        SKImageInfo info = args.Info;
        SKSurface surface = args.Surface;
        SKCanvas canvas = surface.Canvas;
        string svgName = string.Empty;
        string str = string.Empty;
        SKColor empty = SKColor.Empty;
        canvas.Clear(SKColors.White);
        float num1 = 242f;
        float num2 = 310f;
        float num3 = Math.Min((float)((double)info.Width / (double)num1 * 0.89999997615814209), (float)((double)info.Height / (double)num2 * 0.89999997615814209));
        canvas.Translate(30f, -45f);
        foreach (MonitoringGraphicsViewModel.TopogramSvg renderedTopogramSvg in this.renderedTopogramSvgs)
        {
            try
            {
                float xposition = renderedTopogramSvg.XPosition;
                float yposition = renderedTopogramSvg.YPosition;
                svgName = renderedTopogramSvg.SVGFileName;
                float num4;
                float num5;
                if (string.IsNullOrEmpty(svgName))
                {
                    num4 = xposition * num3;
                    num5 = (float)info.Height - (yposition + 35f) * num3;
                }
                else
                {
                    iService5.Core.Services.Helpers.SVGParser.SVGParser svgParser = new iService5.Core.Services.Helpers.SVGParser.SVGParser(svgName, this._loggingService);
                    int frameIndex = renderedTopogramSvg.FrameIndex;
                    MonitoringGraphicsSVG monitoringGraphicsSvg = svgParser.GetMonitoringGraphicsSVG();
                    if (monitoringGraphicsSvg == null || monitoringGraphicsSvg.GetFrame(frameIndex) == null)
                    {
                        renderedTopogramSvg.IsRendered = false;
                        continue;
                    }
                    float tx = (xposition + monitoringGraphicsSvg.GetFrameXValue(frameIndex) + monitoringGraphicsSvg.GetXValue()) * num3;
                    float ty = (float)info.Height - (float)((double)yposition - (double)monitoringGraphicsSvg.GetFrameYValue(frameIndex) - (double)monitoringGraphicsSvg.GetYValue() + 45.0) * num3;
                    Stream stream = monitoringGraphicsSvg.GetStream(frameIndex);
                    Svg.Skia.SKSvg skSvg = new Svg.Skia.SKSvg();
                    skSvg.Load(stream);
                    SKMatrix scaleTranslation = SKMatrix.CreateScaleTranslation(num3, num3, tx, ty);
                    canvas.DrawPicture(skSvg.Picture, ref scaleTranslation);
                    float width = monitoringGraphicsSvg.GetWidth();
                    float height = monitoringGraphicsSvg.GetHeight();
                    num4 = tx + (float)((double)width * (double)num3 / 2.0);
                    num5 = (float)((double)ty + (double)height * (double)num3 + 35.0);
                }
                renderedTopogramSvg.LabelXPosition = num4;
                renderedTopogramSvg.LabelYPosition = num5;
                this.DisplayLabelAndValue(renderedTopogramSvg, canvas, num3);
            }
            catch (Exception ex)
            {
                this._loggingService.getLogger().LogAppError(LoggingContext.LOCAL, $"Failed to display graphics..{svgName}:{ex.Message}", memberName: nameof(OnCanvasViewPaintInstructions), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/MonitoringGraphicsViewModel.cs", sourceLineNumber: 465);
            }
        }
        canvas.Flush();
        if (this.RefreshViewAction == null)
            return;
        this.RefreshViewAction();
    }

    public void ChangeAnimationFrame(
      MonitoringGraphicsViewModel.TopogramSvg animatedSvg)
    {
        if (animatedSvg.FrameIndex + 1 <= animatedSvg.FrameLimit)
            ++animatedSvg.FrameIndex;
        if (!(animatedSvg.AnimationType == "cyclic") || animatedSvg.FrameIndex + 1 <= animatedSvg.FrameLimit)
            return;
        animatedSvg.FrameIndex = animatedSvg.CurrentState * animatedSvg.NOfFrames - animatedSvg.NOfFrames;
    }

    public void ChangeAnimationState()
    {
        if (this.RefreshViewAction != null)
            this.RefreshViewAction();
        this._sshWrapper.IPAddress = this._locator.GetPlatformSpecificService().GetIp();
        Task.Run((Action)(() =>
        {
            while (this.FetchingComponentStates)
            {
                try
                {
                    this._sshWrapper.GetValues((Stream)null, (iService5.Ssh.models.ProgressCallback)((progress, step, total) =>
                    {
                        try
                        {
                            this._loggingService.getLogger().LogAppDebug(LoggingContext.MONITORING, "Raw Json :" + progress, memberName: nameof(ChangeAnimationState), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/MonitoringGraphicsViewModel.cs", sourceLineNumber: 575);
                            if (this.IntialFetch)
                            {
                                this.starterJson = progress;
                                this.IntialFetch = false;
                            }
                            if (this.retrievalDelay == 0)
                            {
                                if (FeatureConfiguration.MonitoringDebugEnabled)
                                    this._loggingService.getLogger().LogAppDebug(LoggingContext.MONITORING, "Initial Json :" + this.starterJson, memberName: nameof(ChangeAnimationState), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/MonitoringGraphicsViewModel.cs", sourceLineNumber: 586);
                                this.ProcessValuesFeedback(JsonConvert.DeserializeObject<Values>(this.starterJson));
                            }
                            else if (this.retrievalDelay <= 2)
                            {
                                string str = this.RetrieveJsonData(this.selItem.Id);
                                if (!string.IsNullOrEmpty(str))
                                {
                                    this.tempJson = str;
                                    this.ProcessValuesFeedback(JsonConvert.DeserializeObject<Values>(str));
                                }
                                else
                                    this.ProcessValuesFeedback(JsonConvert.DeserializeObject<Values>(progress));
                            }
                            else
                            {
                                this.tempJson = progress;
                                this.ProcessValuesFeedback(JsonConvert.DeserializeObject<Values>(progress));
                            }
                            ++this.retrievalDelay;
                        }
                        catch (Exception ex)
                        {
                            this._loggingService.getLogger().LogAppError(LoggingContext.MONITORING, "Error during JSON response parsing: " + ex.Message, memberName: nameof(ChangeAnimationState), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/MonitoringGraphicsViewModel.cs", sourceLineNumber: 616);
                            ++this.retrievalDelay;
                        }
                    }));
                }
                catch (Exception ex)
                {
                    this._loggingService.getLogger().LogAppError(LoggingContext.APPLIANCE, "Error during GetValues: " + ex.Message, memberName: nameof(ChangeAnimationState), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/MonitoringGraphicsViewModel.cs", sourceLineNumber: 623);
                }
            }
        }));
    }

    public void ProcessValuesFeedback(Values valuesFeedback)
    {
        try
        {
            foreach (ApplianceStatus applianceStatu in valuesFeedback.appliance_status)
            {
                ApplianceStatus res = applianceStatu;
                MonitoringGraphicsViewModel.TopogramSvg topogramSvg = this.topogramSvgs.SingleOrDefault<MonitoringGraphicsViewModel.TopogramSvg>((Func<MonitoringGraphicsViewModel.TopogramSvg, bool>)(x => string.Equals(x.Name, res.partname, StringComparison.CurrentCultureIgnoreCase)));
                int int32 = Convert.ToInt32(res.numvalue);
                string dispvalue = res.dispvalue;
                if (FeatureConfiguration.MonitoringDebugEnabled)
                {
                    this._loggingService.getLogger().LogAppDebug(LoggingContext.MONITORING, $"Label from the appliace for {res.partname} : {dispvalue}", memberName: nameof(ProcessValuesFeedback), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/MonitoringGraphicsViewModel.cs", sourceLineNumber: 641);
                    this._loggingService.getLogger().LogAppDebug(LoggingContext.MONITORING, $"Value from the appliace for {res.partname} : {int32.ToString()}", memberName: nameof(ProcessValuesFeedback), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/MonitoringGraphicsViewModel.cs", sourceLineNumber: 642);
                }
                if (topogramSvg != null)
                {
                    int currentState = topogramSvg.CurrentState;
                    int stateIndex = this.GetStateIndex(int32, topogramSvg.Values);
                    topogramSvg.CurrentState = topogramSvg.Values.Count == 0 ? stateIndex + 1 : topogramSvg.Values.IndexOf(stateIndex) + 1;
                    topogramSvg.LabelValue = dispvalue;
                    if (this.firstReception)
                    {
                        topogramSvg.IsStateValueChanging = false;
                        this.SetFrames(topogramSvg);
                        this.CheckDependencies(topogramSvg);
                    }
                    else if (currentState != topogramSvg.CurrentState)
                    {
                        topogramSvg.IsStateValueChanging = true;
                        this.SetFrames(topogramSvg);
                        this.CheckDependencies(topogramSvg);
                    }
                    else
                        topogramSvg.IsStateValueChanging = false;
                }
            }
            this.firstReception = false;
            this.renderedTopogramSvgs = this.topogramSvgs.Select<MonitoringGraphicsViewModel.TopogramSvg, MonitoringGraphicsViewModel.TopogramSvg>((Func<MonitoringGraphicsViewModel.TopogramSvg, MonitoringGraphicsViewModel.TopogramSvg>)(x => x.Clone())).ToList<MonitoringGraphicsViewModel.TopogramSvg>();
        }
        catch (Exception ex)
        {
            this._loggingService.getLogger().LogAppError(LoggingContext.APPLIANCE, "Error during  Proccesing the GetValues: " + ex.Message, memberName: nameof(ProcessValuesFeedback), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/MonitoringGraphicsViewModel.cs", sourceLineNumber: 674);
        }
    }

    public void CheckDependencies(MonitoringGraphicsViewModel.TopogramSvg component)
    {
        try
        {
            foreach (MonitoringGraphicsViewModel.TopogramSvg topogramSvg1 in this.topogramSvgs.Where<MonitoringGraphicsViewModel.TopogramSvg>((Func<MonitoringGraphicsViewModel.TopogramSvg, bool>)(x => x.Dependencies.Length != 0 && ((IEnumerable<string>)x.Dependencies).Contains<string>(component.Name))))
            {
                MonitoringGraphicsViewModel.TopogramSvg animatedSVG = topogramSvg1;
                MonitoringGraphicsViewModel.TopogramSvg topogramSvg2 = this.topogramSvgs.SingleOrDefault<MonitoringGraphicsViewModel.TopogramSvg>((Func<MonitoringGraphicsViewModel.TopogramSvg, bool>)(x => x.Name == animatedSVG.Dependencies[0]));
                if (animatedSVG.Dependencies.Length == 1)
                {
                    if (topogramSvg2 != null)
                    {
                        animatedSVG.CurrentState = topogramSvg2.CurrentState == 1 ? 1 : 2;
                    }
                    else
                    {
                        animatedSVG.CurrentState = 1;
                        this._loggingService.getLogger().LogAppError(LoggingContext.LOCAL, $"Topogram entry for dependency {animatedSVG.Dependencies[0]} is missing.", memberName: nameof(CheckDependencies), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/MonitoringGraphicsViewModel.cs", sourceLineNumber: 695);
                    }
                }
                else if (animatedSVG.Dependencies.Length > 1)
                {
                    MonitoringGraphicsViewModel.TopogramSvg topogramSvg3 = this.topogramSvgs.SingleOrDefault<MonitoringGraphicsViewModel.TopogramSvg>((Func<MonitoringGraphicsViewModel.TopogramSvg, bool>)(x => x.Name == animatedSVG.Dependencies[1]));
                    if (topogramSvg2 != null && topogramSvg3 != null)
                    {
                        if (topogramSvg2.CurrentState == 1 && topogramSvg3.CurrentState == 1 || topogramSvg2.CurrentState > 1 && topogramSvg3.CurrentState > 1)
                            animatedSVG.IsRendered = false;
                        else if (topogramSvg2.CurrentState > 1 && topogramSvg3.CurrentState == 1)
                        {
                            animatedSVG.IsRendered = true;
                            animatedSVG.CurrentState = 1;
                        }
                        else if (topogramSvg2.CurrentState == 1 && topogramSvg3.CurrentState > 1)
                        {
                            animatedSVG.IsRendered = true;
                            animatedSVG.CurrentState = 2;
                        }
                    }
                    else
                    {
                        animatedSVG.CurrentState = 1;
                        if (topogramSvg2 == null)
                            this._loggingService.getLogger().LogAppError(LoggingContext.LOCAL, $"Topogram entry for dependency {animatedSVG.Dependencies[0]} is missing.", memberName: nameof(CheckDependencies), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/MonitoringGraphicsViewModel.cs", sourceLineNumber: 721);
                        if (topogramSvg3 == null)
                            this._loggingService.getLogger().LogAppError(LoggingContext.LOCAL, $"Topogram entry for dependency {animatedSVG.Dependencies[1]} is missing.", memberName: nameof(CheckDependencies), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/MonitoringGraphicsViewModel.cs", sourceLineNumber: 723);
                    }
                }
                this.SetFrames(animatedSVG);
            }
        }
        catch (Exception ex)
        {
            this._loggingService.getLogger().LogAppError(LoggingContext.MONITORING, "Exception during dependency check " + ex.Message, memberName: nameof(CheckDependencies), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/MonitoringGraphicsViewModel.cs", sourceLineNumber: 731);
        }
    }

    public void SetFrames(
      MonitoringGraphicsViewModel.TopogramSvg animatedSVG)
    {
        animatedSVG.FrameIndex = (animatedSVG.CurrentState - 1) * animatedSVG.NOfFrames;
        animatedSVG.FrameLimit = animatedSVG.CurrentState * animatedSVG.NOfFrames;
        this.RefreshViewAction();
    }

    public void DisplayLabelAndValue(
      MonitoringGraphicsViewModel.TopogramSvg topogramSvg,
      SKCanvas canvas,
      float ratio)
    {
       
    }

    public void OnBackButtonPressed() => this.GoBack();

    internal void GoBack()
    {
        this.FetchingComponentStates = false;
        MessagingCenter.Unsubscribe<NonSmmConnectionViewModel, Values>((object)this, "ValuesRetrieved");
        this._navigationService.Close((IMvxViewModel)this, new CancellationToken());
    }

    internal void ConnectivityCheck()
    {
        this.WifiBridgeStatus = this._appliance.StatusOfBridgeConnection;
        this.ConnectedColor = this._appliance.ConnectedColorBridge;
        if (this._appliance.boolStatusOfBridgeConnection)
        {
            if (this._DisconnectionTask == null || this._tokenSource == null || this._DisconnectionTask.IsCompleted)
                return;
            this._tokenSource.Cancel();
        }
        else
        {
            this._tokenSource = new CancellationTokenSource();
            this._DisconnectionTask = Task.Factory.StartNew((Action)(() =>
            {
                Thread.Sleep(3000);
                if (this._tokenSource.Token.IsCancellationRequested)
                    return;
                this.GoBack();
            }), this._tokenSource.Token);
        }
    }

    public void ExtractMonitoringGraphics()
    {
        try
        {
            document graphicsDocument = this._metadataService.GetMonitoringGraphicsDocument(this.monitoringGraphicsEnumber);
            if (graphicsDocument != null)
            {
                this.monitoringGraphic = graphicsDocument.toFileName();
                ZipArchive archive = ZipFile.OpenRead(Path.Combine(this._locator.GetPlatformSpecificService().getFolder(), this.monitoringGraphic));
                string str = Path.Combine(this._locator.GetPlatformSpecificService().getFolder(), this.monitoringGraphicsDirectory);
                if (Directory.Exists(str))
                {
                    Settings settings = CoreApp.settings.GetItem("MonitoringGraphicsMD5");
                    if (settings != null && settings.Value != "" && !string.Equals(graphicsDocument.md5, settings.Value))
                    {
                        this._loggingService.getLogger().LogAppInformation(LoggingContext.LOCAL, "detected stale monitoring graphics. Deleting monitoring-folder.", memberName: nameof(ExtractMonitoringGraphics), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/MonitoringGraphicsViewModel.cs", sourceLineNumber: 856);
                        Directory.Delete(str, true);
                    }
                }
                if (!Directory.Exists(str))
                {
                    this._loggingService.getLogger().LogAppInformation(LoggingContext.LOCAL, "Monitoring-folder not found. Creating new one.", memberName: nameof(ExtractMonitoringGraphics), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/MonitoringGraphicsViewModel.cs", sourceLineNumber: 864);
                    CoreApp.settings.UpdateItem(new Settings("MonitoringGraphicsMD5", graphicsDocument.md5));
                    Directory.CreateDirectory(str);
                    if (!UtilityFunctions.CheckCompressedFile(archive))
                        throw new ArgumentOutOfRangeException("Uncompressed file exceeds compression thresholds.");
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        try
                        {
                            entry.ExtractToFile(Path.Combine(str, entry.Name));
                        }
                        catch (UnauthorizedAccessException ex)
                        {
                        }
                    }
                }
            }
            else
                this._loggingService.getLogger().LogAppError(LoggingContext.LOCAL, "No monitoring-folder found or not yet downloaded", memberName: nameof(ExtractMonitoringGraphics), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/MonitoringGraphicsViewModel.cs", sourceLineNumber: 891);
            this.DisplayActivityIndicator = false;
        }
        catch (Exception ex)
        {
            this._loggingService.getLogger().LogAppError(LoggingContext.LOCAL, "Failed to extract monitoring graphics document.." + ex.Message, memberName: nameof(ExtractMonitoringGraphics), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/MonitoringGraphicsViewModel.cs", sourceLineNumber: 897);
            this.DisplayActivityIndicator = false;
        }
    }

    public int GetStateIndex(int latestComponentValue, List<int> componentValuesInTopogram)
    {
        int stateIndex = 0;
        if (componentValuesInTopogram.Count > 0)
            stateIndex = !componentValuesInTopogram.Contains(latestComponentValue) ? (latestComponentValue >= componentValuesInTopogram[0] ? (latestComponentValue <= componentValuesInTopogram[componentValuesInTopogram.Count - 1] ? componentValuesInTopogram.OrderBy<int, int>((Func<int, int>)(n => n)).FirstOrDefault<int>((Func<int, bool>)(n => latestComponentValue < n)) : componentValuesInTopogram[componentValuesInTopogram.Count - 1]) : componentValuesInTopogram[0]) : latestComponentValue;
        return stateIndex;
    }

    public override void Prepare(bool parameter) => throw new NotImplementedException();

    public class TopogramSvg
    {
        public string SVGFileName { get; set; }

        public int NOfFrames { get; set; }

        public int NOfStates { get; set; }

        public int CurrentState { get; set; }

        public int FrameIndex { get; set; }

        public int FrameLimit { get; set; }

        public string AnimationType { get; set; }

        public float XPosition { get; set; }

        public float YPosition { get; set; }

        public string Name { get; set; }

        public string LabelValue { get; set; }

        public float LabelXPosition { get; set; }

        public float LabelYPosition { get; set; }

        public string DisplayName { get; set; }

        public string NumericDisplay { get; set; }

        public string[] Dependencies { get; set; }

        public TopogramComponentType Type { get; set; }

        public List<int> Values { get; set; }

        public bool IsRendered { get; set; }

        public int StateValue { get; set; }

        public bool IsStateValueChanging { get; set; }

        public MonitoringGraphicsViewModel.TopogramSvg Clone()
        {
            return new MonitoringGraphicsViewModel.TopogramSvg()
            {
                Name = this.Name,
                XPosition = this.XPosition,
                YPosition = this.YPosition,
                SVGFileName = this.SVGFileName,
                FrameIndex = this.FrameIndex,
                IsRendered = this.IsRendered,
                FrameLimit = this.FrameLimit,
                AnimationType = this.AnimationType,
                CurrentState = this.CurrentState,
                NOfFrames = this.NOfFrames,
                LabelXPosition = this.LabelXPosition,
                LabelYPosition = this.LabelYPosition,
                NumericDisplay = this.NumericDisplay,
                LabelValue = this.LabelValue,
                IsStateValueChanging = this.IsStateValueChanging,
                DisplayName = this.DisplayName
            };
        }
    }
}
