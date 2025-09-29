// Decompiled with JetBrains decompiler
// Type: iService5.Core.ViewModels.NonSmmCodingTransitionViewModel
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Appliance;
using iService5.Core.Services.User.ActivityLogging;
using iService5.Ssh.DTO;
using iService5.Ssh.models;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace iService5.Core.ViewModels;

public class NonSmmCodingTransitionViewModel : MvxViewModel<CodingParameter>
{
  private readonly IMvxNavigationService _navigationService;
  private readonly Is5SshWrapper _sshWrapper;
  private ILoggingService _loggingService;
  private CodingParameter Parameter;

  public override void Prepare(CodingParameter parameter) => this.Parameter = parameter;

  public virtual async Task Initialize() => await base.Initialize();

  public NonSmmCodingTransitionViewModel(
    IMvxNavigationService navigationService,
    ILoggingService loggingService,
    Is5SshWrapper sshWrapper)
  {
    this._navigationService = navigationService;
    this._sshWrapper = sshWrapper;
    this._loggingService = loggingService;
  }

  public string CoderActivation => AppResource.CODING_TRANSITION;

  public override void ViewAppeared()
  {
    Task.Factory.StartNew((Action) (() =>
    {
      try
      {
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Initializing coding.", memberName: nameof (ViewAppeared), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/NonSmmCodingTransitionViewModel.cs", sourceLineNumber: 48 /*0x30*/);
        if (this.Parameter.fromFlashing)
        {
          this._navigationService.Navigate<NonSmmApplianceCodingViewModel, CodingParameter>(this.Parameter, (IMvxBundle) null, new CancellationToken());
        }
        else
        {
          SshResponse<CodingItem> sshResponse = this._sshWrapper.ActivateCoder(this.Parameter.CodingItem.CodingIndex);
          if (sshResponse.Success)
          {
            IMvxNavigationService navigationService = this._navigationService;
            CodingParameter codingParameter = new CodingParameter();
            codingParameter.CodingItem = sshResponse.Response;
            CancellationToken cancellationToken = new CancellationToken();
            navigationService.Navigate<NonSmmApplianceCodingViewModel, CodingParameter>(codingParameter, (IMvxBundle) null, cancellationToken);
          }
        }
        this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
      }
      catch (Exception ex)
      {
        this._loggingService.getLogger().LogAppError(LoggingContext.APPLIANCE, ex.Message, ex, nameof (ViewAppeared), "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/ViewModels/NonSmmCodingTransitionViewModel.cs", 71);
        this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
      }
    }));
  }
}
