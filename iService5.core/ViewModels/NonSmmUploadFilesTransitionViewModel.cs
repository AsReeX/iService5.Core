// Decompiled with JetBrains decompiler
// Type: iService5.Core.ViewModels.NonSmmUploadFilesTransitionViewModel
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
using iService5.Ssh.models;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
//using Xamarin.Forms;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

#nullable disable
namespace iService5.Core.ViewModels;

public class NonSmmUploadFilesTransitionViewModel : MvxViewModel
{
  private readonly IMvxNavigationService _navigationService;
  private readonly IUserSession _userSession;
  private readonly IShortTextsService _ShortTextsService = (IShortTextsService) null;
  private readonly IMetadataService _metadataService;
  private readonly IPlatformSpecificServiceLocator _locator;
  private readonly ILoggingService _loggingService;
  private readonly Is5SshWrapper _sshWrapper;
  private readonly IAlertService _alertService;
  private CancellationTokenSource tokenSource;
  private CancellationToken token;
  private string _UploadingSessionDocumentsLabel = AppResource.NONSMM_UPLOAD_TRANSITION_PAGE_COLLECTING;
  private string _RepairEnumber;
  private List<uploadDocument> _materialdocsList;
  private bool _canGoBack = false;

  public virtual async Task Initialize() => await base.Initialize();

  public IApplianceSession Session { get; }

  private NonSmmUploadFilesTransitionViewModel.SessionDetails EnumberSessionDetails { get; set; }

  public NonSmmUploadFilesTransitionViewModel(
    IMvxNavigationService navigationService,
    IShortTextsService _SService,
    ILoggingService loggingService,
    IMetadataService metadataService,
    IPlatformSpecificServiceLocator locator,
    IUserSession userSession,
    Is5SshWrapper sshWrapper,
    IAlertService alertService,
    IApplianceSession session)
  {
    this._ShortTextsService = _SService;
    this._userSession = userSession;
    this._metadataService = metadataService;
    this._navigationService = navigationService;
    this._locator = locator;
    this._loggingService = loggingService;
    this._sshWrapper = sshWrapper;
    this._alertService = alertService;
    this.Session = session;
    this.EnumberSessionDetails = new NonSmmUploadFilesTransitionViewModel.SessionDetails();
    this.tokenSource = new CancellationTokenSource();
    this.token = this.tokenSource.Token;
  }

  public string UploadingSessionDocumentsLabel
  {
    get => this._UploadingSessionDocumentsLabel;
    internal set
    {
      this._UploadingSessionDocumentsLabel = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.UploadingSessionDocumentsLabel));
    }
  }

  public string RepairEnumber
  {
    get => this._RepairEnumber;
    private set
    {
      this._RepairEnumber = value;
      this.RaisePropertyChanged<string>((Expression<Func<string>>) (() => this.RepairEnumber));
    }
  }

  public List<uploadDocument> materialdocsList
  {
    get => this._materialdocsList;
    private set => this._materialdocsList = value;
  }

  public bool canGoback
  {
    get => this._canGoBack;
    private set => this._canGoBack = value;
  }

  public override void ViewAppeared()
  {
    base.ViewAppeared();
    Task.Factory.StartNew<Task>((Func<Task>) (async () =>
    {
      Thread.Sleep(1000);
      string _smmip = this._locator.GetPlatformSpecificService().GetIp();
      this._sshWrapper.IPAddress = _smmip;
      try
      {
        this.materialdocsList = this._metadataService.getMaterialDocuments(this._userSession.getEnumberSession());
      }
      catch (Exception ex)
      {
        this._loggingService.getLogger().LogAppDebug(LoggingContext.METADATA, "error in get materialdocs list : " + ex.Message, memberName: nameof (ViewAppeared), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/NonSmmUploadFilesTransitionViewModel.cs", sourceLineNumber: 137);
      }
      this.materialdocsList.ForEach((Action<uploadDocument>) (updoc => updoc.uploadSucces = false));
      int uploadAttempts = 0;
      int numOfUploadedDocs = 0;
      if (this._sshWrapper.GetSshAvailability().Success)
      {
        this._loggingService.getLogger().LogAppDebug(LoggingContext.BACKEND, "Successfully connected ssh - start-session successful", memberName: nameof (ViewAppeared), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/NonSmmUploadFilesTransitionViewModel.cs", sourceLineNumber: 147);
        while (this.materialdocsList.Any<uploadDocument>((Func<uploadDocument, bool>) (_doc => !_doc.uploadSucces)) && uploadAttempts < 4 && !this.canGoback)
        {
          ++uploadAttempts;
          this._loggingService.getLogger().LogAppDebug(LoggingContext.BACKEND, $"Upload files for {this._userSession.getEnumberSession()}; attempt: {uploadAttempts.ToString()}", memberName: nameof (ViewAppeared), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/NonSmmUploadFilesTransitionViewModel.cs", sourceLineNumber: 151);
          foreach (uploadDocument MaterialDocument in this.materialdocsList)
          {
            if (!MaterialDocument.uploadSucces)
            {
              string fileName = MaterialDocument.toFileName();
              if (File.Exists(Path.Combine(this._locator.GetPlatformSpecificService().getFolder(), fileName)))
              {
                this._loggingService.getLogger().LogAppDebug(LoggingContext.BACKEND, "start upload process with file=" + fileName, memberName: nameof (ViewAppeared), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/NonSmmUploadFilesTransitionViewModel.cs", sourceLineNumber: 161);
                try
                {
                  using (FileStream stream = new FileStream(Path.Combine(this._locator.GetPlatformSpecificService().getFolder(), fileName), FileMode.Open))
                  {
                    SshResponse response = this._sshWrapper.UploadSessionDoc((Stream) stream);
                    if (response.Success)
                    {
                      ++numOfUploadedDocs;
                      this.EnumberSessionDetails.uploadedfilesList.Add($"{numOfUploadedDocs.ToString()}:{fileName}:{MaterialDocument.type}");
                      MaterialDocument.uploadSucces = true;
                      this._loggingService.getLogger().LogAppDebug(LoggingContext.BACKEND, $"Successfully uploaded file: {fileName}; NumberOfUploadedDocs={numOfUploadedDocs.ToString()}", memberName: nameof (ViewAppeared), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/NonSmmUploadFilesTransitionViewModel.cs", sourceLineNumber: 174);
                    }
                    else
                    {
                      MaterialDocument.uploadSucces = false;
                      this._loggingService.getLogger().LogAppError(LoggingContext.BACKEND, $"Error uploading file: {fileName}; Error: {response.ErrorMessage}", memberName: nameof (ViewAppeared), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/NonSmmUploadFilesTransitionViewModel.cs", sourceLineNumber: 180);
                    }
                    response = (SshResponse) null;
                  }
                }
                catch (Exception ex)
                {
                  MaterialDocument.uploadSucces = false;
                  this._loggingService.getLogger().LogAppDebug(LoggingContext.BACKEND, $"upload exception : {fileName}, {ex.InnerException?.ToString()}", memberName: nameof (ViewAppeared), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/NonSmmUploadFilesTransitionViewModel.cs", sourceLineNumber: 188);
                }
              }
              else
                this._loggingService.getLogger().LogAppDebug(LoggingContext.BACKEND, "file missing :  " + Path.Combine(this._locator.GetPlatformSpecificService().getFolder(), fileName), memberName: nameof (ViewAppeared), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/NonSmmUploadFilesTransitionViewModel.cs", sourceLineNumber: 194);
              fileName = (string) null;
            }
          }
        }
        this._loggingService.getLogger().LogAppDebug(LoggingContext.BACKEND, "finished file upload ", memberName: nameof (ViewAppeared), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/NonSmmUploadFilesTransitionViewModel.cs", sourceLineNumber: 200);
        this.EnumberSessionDetails.enumber = this._userSession.getEnumberSession();
        this.EnumberSessionDetails.timestamp = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
        this.EnumberSessionDetails.Varcodes = this._metadataService.getOldVarCodingStatus(this._userSession.getEnumberSession());
        try
        {
          string sessionDetailsJson = JsonConvert.SerializeObject((object) this.EnumberSessionDetails);
          this._loggingService.getLogger().LogAppDebug(LoggingContext.BACKEND, "Calling start-session with sessionDetails = " + sessionDetailsJson, memberName: nameof (ViewAppeared), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/NonSmmUploadFilesTransitionViewModel.cs", sourceLineNumber: 210);
          if (this._sshWrapper.StartSession(sessionDetailsJson).Success)
            this._loggingService.getLogger().LogAppDebug(LoggingContext.BACKEND, "Successfully start-session: uploadedFiles=" + this.EnumberSessionDetails.uploadedfilesList.Count.ToString(), memberName: nameof (ViewAppeared), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/NonSmmUploadFilesTransitionViewModel.cs", sourceLineNumber: 215);
          else
            this._loggingService.getLogger().LogAppError(LoggingContext.BACKEND, "Error start-session: uploadedFiles=" + this.EnumberSessionDetails.uploadedfilesList.Count.ToString(), memberName: nameof (ViewAppeared), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/NonSmmUploadFilesTransitionViewModel.cs", sourceLineNumber: 220);
          sessionDetailsJson = (string) null;
        }
        catch (Exception ex)
        {
          this._loggingService.getLogger().LogAppDebug(LoggingContext.BACKEND, "start-session. exception=" + ex.Message, memberName: nameof (ViewAppeared), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/NonSmmUploadFilesTransitionViewModel.cs", sourceLineNumber: 226);
        }
        try
        {
          if (!this.canGoback)
          {
            int num = await this._navigationService.Navigate<NonSmmConnectionViewModel>((IMvxBundle) null, new CancellationToken()) ? 1 : 0;
          }
        }
        catch (Exception ex)
        {
          this._loggingService.getLogger().LogAppError(LoggingContext.BACKEND, "Error navigation to nonsmm view : ", ex, nameof (ViewAppeared), "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/NonSmmUploadFilesTransitionViewModel.cs", 236);
        }
        int num1 = await this._navigationService.Close((IMvxViewModel) this, new CancellationToken()) ? 1 : 0;
        _smmip = (string) null;
      }
      else
      {
        this._loggingService.getLogger().LogAppDebug(LoggingContext.BACKEND, "Could not connect ssh: bridge mode possibly occupied by IS4 or local networks usage permission denied", memberName: nameof (ViewAppeared), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/NonSmmUploadFilesTransitionViewModel.cs", sourceLineNumber: 242);
        Device.BeginInvokeOnMainThread((Action) (async () => await UtilityFunctions.ShowBridgeModeWarningPopup()));
        int num = await this._navigationService.Navigate<StatusViewModel>((IMvxBundle) null, new CancellationToken()) ? 1 : 0;
        _smmip = (string) null;
      }
    }), this.token);
  }

  public class SessionDetails
  {
    public IEnumerable<varcodes> Varcodes;

    public string enumber { get; set; }

    public string techId { get; set; }

    public string repairId { get; set; }

    public string timestamp { get; set; }

    public List<string> uploadedfilesList { get; set; }

    public SessionDetails() => this.uploadedfilesList = new List<string>();
  }
}
