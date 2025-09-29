// Decompiled with JetBrains decompiler
// Type: iService5.Core.ViewModels.BridgeUpgradeInstructionsViewModel
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Helpers;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
//using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Input;

#nullable disable
namespace iService5.Core.ViewModels;

public class BridgeUpgradeInstructionsViewModel : MvxViewModel
{
  private readonly IMvxNavigationService _navigationService;

  public List<InstructionItem> Instructions { get; set; }

  public ICommand BackCommand { internal set; get; }

  public ICommand WiFiBridgeSettingsCommand { internal set; get; }

  public BridgeUpgradeInstructionsViewModel(IMvxNavigationService navigationService)
  {
    this._navigationService = navigationService;
    this.BackCommand = (ICommand) new Command(new Action(this.GoBack));
    this.WiFiBridgeSettingsCommand = (ICommand) new Command(new Action(this.GoToWiFiBridgeSettingsCommand));
    this.Instructions = new List<string>()
    {
      AppResource.BRIDGE_UPGRADE_INSTRUCTION_1,
      AppResource.BRIDGE_UPGRADE_INSTRUCTION_2,
      AppResource.BRIDGE_UPGRADE_INSTRUCTION_3,
      AppResource.BRIDGE_UPGRADE_INSTRUCTION_4,
      AppResource.BRIDGE_UPGRADE_INSTRUCTION_5,
      AppResource.BRIDGE_UPGRADE_INSTRUCTION_6
    }.Select<string, InstructionItem>((Func<string, int, InstructionItem>) ((text, index) => new InstructionItem()
    {
      Number = $"{index + 1}.",
      Text = text
    })).ToList<InstructionItem>();
  }

  internal void GoBack()
  {
    this._navigationService.Close((IMvxViewModel) this, new CancellationToken());
  }

  internal void GoToWiFiBridgeSettingsCommand()
  {
    IMvxNavigationService navigationService = this._navigationService;
    DetailNavigationArgs detailNavigationArgs = new DetailNavigationArgs();
    detailNavigationArgs.detailNavigationPageType = DetailNavigationPageType.BridgeUpgrade;
    CancellationToken cancellationToken = new CancellationToken();
    navigationService.Navigate<InAppBrowserViewModel, DetailNavigationArgs>(detailNavigationArgs, (IMvxBundle) null, cancellationToken);
  }
}
