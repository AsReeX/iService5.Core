// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Appliance.Is5SshWrapper
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.User.ActivityLogging;
using iService5.Ssh;
using iService5.Ssh.DTO;
using iService5.Ssh.enums;
using iService5.Ssh.interfaces;
using iService5.Ssh.models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Authentication;
using System.Text;

#nullable disable
namespace iService5.Core.Services.Appliance;

public class Is5SshWrapper : IDisposable
{
  private readonly ILoggingService _loggingService;
  private ISshClient _sshClient;
  private string _IPAddress = "";
  private readonly string _LoginUser = "";
  private readonly string[] haCmd = new string[6]
  {
    "set-ha-brand",
    "set-ha-customer-index",
    "set-ha-device-type",
    "set-ha-manufacturing-time-stamp",
    "set-ha-vib",
    "set-ha-country-settings"
  };

  public LoginUserType? LoginType { get; private set; }

  public BootMode? CurrentBootMode { get; internal set; }

  public string IPAddress
  {
    get => this._IPAddress;
    set
    {
      if (!(value != this._IPAddress))
        return;
      this._IPAddress = value;
      MemoryStream keypath = new MemoryStream(this.KeyBuffer);
      if (keypath != null && this._IPAddress != "" && this._IPAddress != null)
      {
        try
        {
          this._sshClient = (ISshClient) new Is5SshClient(this._LoginUser, this._IPAddress, (Stream) keypath);
        }
        catch (Exception ex)
        {
          Debug.WriteLine(ex.Message);
          this._IPAddress = "";
        }
      }
    }
  }

  public byte[] KeyBuffer { get; internal set; }

  public Is5SshWrapper(string userType, Stream keystream, ILoggingService loggingService)
  {
    this._loggingService = loggingService;
    this.KeyBuffer = Encoding.ASCII.GetBytes(new StreamReader(keystream).ReadToEnd());
    this._LoginUser = userType.ToLowerInvariant();
    if (this._LoginUser == "cs-light")
      this.LoginType = new LoginUserType?(LoginUserType.CsLight);
    if (this._LoginUser == "cs")
      this.LoginType = new LoginUserType?(LoginUserType.Cs);
    if (!(this._LoginUser == "ifp"))
      return;
    this.LoginType = new LoginUserType?(LoginUserType.Ifp);
  }

  public Is5SshWrapper(ISshClient sshClient, ILoggingService loggingService)
  {
    this._sshClient = sshClient;
    this._loggingService = loggingService;
  }

  public SshResponse ConnectToService(LoginUserType type)
  {
    return this.WrapError((Func<SshResponse>) (() =>
    {
      this._sshClient.Connect();
      this.LoginType = new LoginUserType?(type);
      this.GetBootMode();
      this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "User connected to service as " + type.ToString(), memberName: nameof (ConnectToService), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 99);
      return SshResponse.Valid();
    }));
  }

  public SshResponse SetHa(HAEnum cmd, string content)
  {
    return this.WrapError((Func<SshResponse>) (() =>
    {
      this.CheckAuthorization(nameof (SetHa), LoginUserType.CsLight, LoginUserType.Cs, LoginUserType.Ifp);
      this.CheckAvailability(nameof (SetHa), BootMode.Recovery, BootMode.Maintenance);
      MemoryStream extraBuffer = new MemoryStream();
      extraBuffer.Write(Encoding.ASCII.GetBytes(content), 0, content.Length);
      extraBuffer.Flush();
      extraBuffer.Position = 0L;
      SshResponse sshResponse = this._sshClient.ExecuteCommand(this.haCmd[(int) cmd], (Stream) extraBuffer);
      if (sshResponse.Success)
      {
        if (cmd == HAEnum.brand)
          this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, $"Setting of HA Brand to '{content}' was successful", memberName: nameof (SetHa), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 134);
        if (cmd == HAEnum.ki)
          this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, $"Setting of HA Customer Index to '{content}' was successful", memberName: nameof (SetHa), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 138);
        if (cmd == HAEnum.type)
          this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, $"Setting of HA Device Type to '{content}' was successful", memberName: nameof (SetHa), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 142);
        if (cmd == HAEnum.ts)
          this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, $"Setting of HA Manufacturing Time Stamp to '{content}' was successful", memberName: nameof (SetHa), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 146);
        if (cmd == HAEnum.vib)
          this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, $"Setting of HA VIB to '{content}' was successful", memberName: nameof (SetHa), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 150);
        if (cmd == HAEnum.countrySettings)
          this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, $"Setting of HA Country Settings to '{content}' was successful", memberName: nameof (SetHa), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 154);
      }
      else
      {
        if (cmd == HAEnum.brand)
          this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, $"Setting of HA Brand to '{content}' failed. {sshResponse.ErrorMessage}", memberName: nameof (SetHa), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 161);
        if (cmd == HAEnum.ki)
          this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, $"Setting of HA Customer Index to '{content}' failed. {sshResponse.ErrorMessage}", memberName: nameof (SetHa), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 165);
        if (cmd == HAEnum.type)
          this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, $"Setting of HA Device Type to '{content}' failed. {sshResponse.ErrorMessage}", memberName: nameof (SetHa), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 169);
        if (cmd == HAEnum.ts)
          this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, $"Setting of HA Manufacturing Time Stamp to '{content}' failed. {sshResponse.ErrorMessage}", memberName: nameof (SetHa), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 173);
        if (cmd == HAEnum.vib)
          this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, $"Setting of HA VIB to '{content}' failed. {sshResponse.ErrorMessage}", memberName: nameof (SetHa), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 177);
        if (cmd == HAEnum.countrySettings)
          this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, $"Setting of HA Country Settings to '{content}' failed. {sshResponse.ErrorMessage}", memberName: nameof (SetHa), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 181);
      }
      return sshResponse;
    }));
  }

  public SshResponse<HaInfoDto> GetHaInfo()
  {
    return this.WrapError<HaInfoDto>((Func<SshResponse<HaInfoDto>>) (() =>
    {
      this.CheckAuthorization(nameof (GetHaInfo), LoginUserType.Cs, LoginUserType.CsLight, LoginUserType.Ifp);
      this.CheckAvailability(nameof (GetHaInfo), BootMode.Recovery, BootMode.Maintenance, BootMode.Elp);
      SshResponse sshResponse = this._sshClient.ExecuteCommand("get-ha-info");
      if (sshResponse.Success)
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "HA information was successfully retrieved. " + sshResponse.RawResponse, memberName: nameof (GetHaInfo), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 200);
      else
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Failed to retrieve HA information. " + sshResponse.ErrorMessage, memberName: nameof (GetHaInfo), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 204);
      return sshResponse.Cast<HaInfoDto>();
    }));
  }

  public SshResponse<object> GetErrorLog()
  {
    return this.WrapError<object>((Func<SshResponse<object>>) (() =>
    {
      this.CheckAuthorization(nameof (GetErrorLog), LoginUserType.Cs, LoginUserType.CsLight, LoginUserType.Ifp);
      this.CheckAvailability(nameof (GetErrorLog), BootMode.Recovery, BootMode.Maintenance, BootMode.Elp);
      SshResponse sshResponse = this._sshClient.ExecuteCommand("get-errors");
      if (sshResponse.Success)
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Error Log was successfully retrieved", memberName: nameof (GetErrorLog), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 219);
      else
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Failed to retrieve Error Log. " + sshResponse.ErrorMessage, memberName: nameof (GetErrorLog), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 223);
      return sshResponse.Cast<object>();
    }));
  }

  public SshResponse<object> GetMemoryLog()
  {
    return this.WrapError<object>((Func<SshResponse<object>>) (() =>
    {
      SshResponse sshResponse = this._sshClient.ExecuteCommand("get-memory", (Stream) new MemoryStream());
      if (sshResponse.Success)
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Memory log was sucessfully retrieved", memberName: nameof (GetMemoryLog), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 237);
      else
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Failed to retrieve Memory Log. " + sshResponse.ErrorMessage, memberName: nameof (GetMemoryLog), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 241);
      return sshResponse.Cast<object>();
    }));
  }

  public SshResponse<ProductionInfoDto> GetProductionInfo()
  {
    return this.WrapError<ProductionInfoDto>((Func<SshResponse<ProductionInfoDto>>) (() =>
    {
      this.CheckAuthorization(nameof (GetProductionInfo), LoginUserType.Cs, LoginUserType.CsLight, LoginUserType.Ifp);
      this.CheckAvailability(nameof (GetProductionInfo), BootMode.Recovery, BootMode.Maintenance, BootMode.Elp);
      SshResponse sshResponse = this._sshClient.ExecuteCommand("get-smm-production-info");
      if (sshResponse.Success)
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "SMM Production Information was successfully retrieved", memberName: nameof (GetProductionInfo), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 259);
      else
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Failed to retrieve SMM Production Information. " + sshResponse.ErrorMessage, memberName: nameof (GetProductionInfo), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 263);
      return sshResponse.Cast<ProductionInfoDto>();
    }));
  }

  public SshResponse<SystemInfoDto> GetSystemInfo()
  {
    return this.WrapError<SystemInfoDto>((Func<SshResponse<SystemInfoDto>>) (() =>
    {
      this.CheckAuthorization(nameof (GetSystemInfo), LoginUserType.Cs, LoginUserType.CsLight, LoginUserType.Ifp);
      this.CheckAvailability(nameof (GetSystemInfo), BootMode.Recovery, BootMode.Maintenance, BootMode.Elp);
      SshResponse sshResponse = this._sshClient.ExecuteCommand("get-system-information");
      if (sshResponse.Success)
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "SMM System Information was successfully retrieved", memberName: nameof (GetSystemInfo), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 282);
      else
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Failed to retrieve SMM System Information. " + sshResponse.ErrorMessage, memberName: nameof (GetSystemInfo), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 286);
      return sshResponse.Cast<SystemInfoDto>();
    }));
  }

  public SshResponse<InventoryDto> GetInventory()
  {
    return this.WrapError<InventoryDto>((Func<SshResponse<InventoryDto>>) (() =>
    {
      this.CheckAuthorization(nameof (GetInventory), LoginUserType.Cs, LoginUserType.CsLight, LoginUserType.Ifp);
      this.CheckAvailability(nameof (GetInventory), BootMode.Elp);
      SshResponse sshResponse = this._sshClient.ExecuteCommand("get-inventory");
      if (sshResponse.Success)
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Inventory was successfully retrieved", memberName: nameof (GetInventory), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 305);
      else
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Failed to retrieve Inventory. " + sshResponse.ErrorMessage, memberName: nameof (GetInventory), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 309);
      return sshResponse.Cast<InventoryDto>();
    }));
  }

  public SshResponse<InventoryDto> GetCachedInventory()
  {
    return this.WrapError<InventoryDto>((Func<SshResponse<InventoryDto>>) (() =>
    {
      this.CheckAuthorization(nameof (GetCachedInventory), LoginUserType.Cs, LoginUserType.CsLight, LoginUserType.Ifp);
      this.CheckAvailability(nameof (GetCachedInventory), BootMode.Recovery, BootMode.Maintenance);
      SshResponse sshResponse = this._sshClient.ExecuteCommand("get-cached-inventory");
      if (sshResponse.Success)
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Cached Inventory was successfully retrieved", memberName: nameof (GetCachedInventory), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 328);
      else
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Failed to retrieve Cached Inventory. " + sshResponse.ErrorMessage, memberName: nameof (GetCachedInventory), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 332);
      return sshResponse.Cast<InventoryDto>();
    }));
  }

  public SshResponse<InventoryDto> GetLocalInventory()
  {
    return this.WrapError<InventoryDto>((Func<SshResponse<InventoryDto>>) (() =>
    {
      this.CheckAuthorization(nameof (GetLocalInventory), LoginUserType.Cs, LoginUserType.CsLight, LoginUserType.Ifp);
      this.CheckAvailability(nameof (GetLocalInventory), BootMode.Recovery, BootMode.Maintenance, BootMode.Elp);
      SshResponse sshResponse = this._sshClient.ExecuteCommand("get-local-inventory2");
      if (sshResponse.Success)
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Local Inventory was successfully retrieved", memberName: nameof (GetLocalInventory), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 350);
      else
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Failed to retrieve Local Inventory. " + sshResponse.ErrorMessage, memberName: nameof (GetLocalInventory), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 354);
      return sshResponse.Cast<InventoryDto>();
    }));
  }

  public SshResponse InstallRepair()
  {
    return this.WrapError((Func<SshResponse>) (() =>
    {
      this.CheckAuthorization("InstallUpdate", LoginUserType.Cs, LoginUserType.CsLight, LoginUserType.Ifp);
      this.CheckAvailability("InstallUpdate", BootMode.Recovery, BootMode.Maintenance);
      SshResponse sshResponse = this._sshClient.ExecuteCommand("install-repair");
      if (sshResponse.Success)
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "install-repair succeeded", memberName: nameof (InstallRepair), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 373);
      else
        this._loggingService.getLogger().LogAppError(LoggingContext.APPLIANCE, "install-repair failed. " + sshResponse.ErrorMessage, memberName: nameof (InstallRepair), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 377);
      return sshResponse;
    }));
  }

  public SshResponse SetBootMode(BootMode mode)
  {
    return this.WrapError((Func<SshResponse>) (() =>
    {
      this.CheckAuthorization(nameof (SetBootMode), LoginUserType.Cs, LoginUserType.CsLight, LoginUserType.Ifp);
      this.CheckAvailability(nameof (SetBootMode), BootMode.Recovery, BootMode.Elp, BootMode.Maintenance);
      SshResponse sshResponse = this._sshClient.ExecuteCommand("set-bootmode " + mode.ToString().ToLowerInvariant());
      if (sshResponse.Success)
      {
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, $"Setting of boot mode to {mode.ToString()} was successful", memberName: nameof (SetBootMode), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 396);
        return SshResponse.Valid();
      }
      this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, $"Setting of Boot Mode to {mode.ToString()} failed. {sshResponse.ErrorOut}", memberName: nameof (SetBootMode), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 401);
      return sshResponse;
    }));
  }

  public SshResponse ActivateFlasher(
    string flashingIndex,
    Stream extraBuffer,
    iService5.Ssh.models.ProgressCallback progressCallback)
  {
    return this.WrapError((Func<SshResponse>) (() =>
    {
      SshResponse sshResponse = this._sshClient.ExecuteCommand("start-flasher " + flashingIndex, extraBuffer, progressCallback);
      if (sshResponse.Success)
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, $"Starting flasher. flashingIndex={flashingIndex} was successful", memberName: nameof (ActivateFlasher), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 414);
      else
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, $"Starting flasher. flashingIndex= {flashingIndex} failed. Error={sshResponse.ErrorOut}", memberName: nameof (ActivateFlasher), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 418);
      return sshResponse;
    }));
  }

  public SshResponse<CodingItem> ActivateCoder(string coderIndex)
  {
    return this.WrapError<CodingItem>((Func<SshResponse<CodingItem>>) (() =>
    {
      SshResponse sshResponse = this._sshClient.ExecuteCommand("start-coding " + coderIndex);
      if (sshResponse.Success)
      {
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, $"start-coding codingIndex={coderIndex} was successful", memberName: nameof (ActivateCoder), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 431);
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Response: " + sshResponse.RawResponse, memberName: nameof (ActivateCoder), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 432);
      }
      else
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, $"start-coding. codingIndex={coderIndex} failed. Error {sshResponse.ErrorOut} with message {sshResponse.ErrorMessage}", memberName: nameof (ActivateCoder), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 436);
      if (!sshResponse.RawResponse.Contains("CodingIndex"))
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Incorrect Response from start-coding command", memberName: nameof (ActivateCoder), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 442);
      return sshResponse.Cast<CodingItem>();
    }));
  }

  public SshResponse DeactivateFlasher()
  {
    return this.WrapError((Func<SshResponse>) (() =>
    {
      SshResponse sshResponse = this._sshClient.ExecuteCommand("stop-flasher");
      if (sshResponse.Success)
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "stop-flasher was successful", memberName: nameof (DeactivateFlasher), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 455);
      else
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "stop-flasher failed. Error=" + sshResponse.ErrorOut, memberName: nameof (DeactivateFlasher), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 459);
      return sshResponse;
    }));
  }

  public SshResponse SetMeasurement(string measurement)
  {
    return this.WrapError((Func<SshResponse>) (() =>
    {
      MemoryStream extraBuffer = new MemoryStream();
      extraBuffer.Write(Encoding.ASCII.GetBytes(measurement), 0, measurement.Length);
      extraBuffer.Flush();
      extraBuffer.Position = 0L;
      Encoding.UTF8.GetString(extraBuffer.ToArray());
      return this._sshClient.ExecuteCommand("set-measurement", (Stream) extraBuffer);
    }));
  }

  public SshResponse DeactivateCoder()
  {
    return this.WrapError((Func<SshResponse>) (() =>
    {
      SshResponse sshResponse = this._sshClient.ExecuteCommand("cancel-coding");
      if (sshResponse.Success)
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "cancel-coding was successful", memberName: nameof (DeactivateCoder), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 488);
      else
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "cancel-coding failed. Error=" + sshResponse.ErrorOut, memberName: nameof (DeactivateCoder), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 492);
      return sshResponse;
    }));
  }

  public SshResponse<BootMode> GetBootMode()
  {
    return this.WrapError<BootMode>((Func<SshResponse<BootMode>>) (() =>
    {
      this.CheckAuthorization(nameof (GetBootMode), LoginUserType.Cs, LoginUserType.CsLight, LoginUserType.Ifp);
      SshResponse sshResponse = this._sshClient.ExecuteCommand("get-bootmode");
      if (sshResponse.Success)
      {
        string rawResponse = sshResponse.RawResponse;
        string str;
        if (rawResponse == null)
          str = (string) null;
        else
          str = rawResponse.ToLower().Trim('\t', ' ', '\r', '\n');
        BootMode response;
        switch (str)
        {
          case "recovery":
            response = BootMode.Recovery;
            this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, $"Boot Mode {BootMode.Recovery.ToString()} was successfully retrieved", memberName: nameof (GetBootMode), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 515);
            break;
          case "elp":
            response = BootMode.Elp;
            this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, $"Boot Mode {BootMode.Elp.ToString()} was successfully retrieved", memberName: nameof (GetBootMode), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 519);
            break;
          case "maintenance":
            response = BootMode.Maintenance;
            this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, $"Boot Mode {BootMode.Maintenance.ToString()} was successfully retrieved", memberName: nameof (GetBootMode), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 523);
            break;
          default:
            this.CurrentBootMode = new BootMode?();
            this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, $"Can't map {sshResponse.RawResponse} to a valid BootMode", memberName: nameof (GetBootMode), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 527);
            throw new InvalidDataException($"Can't map {sshResponse.RawResponse} to a valid BootMode");
        }
        this.CurrentBootMode = new BootMode?(response);
        return SshResponse<BootMode>.Ok(response);
      }
      this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Request of Boot Mode failed. " + sshResponse.ErrorMessage, memberName: nameof (GetBootMode), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 535);
      return SshResponse<BootMode>.Error(sshResponse.ErrorMessage);
    }));
  }

  public SshResponse UploadUpdate(Stream extraBuffer)
  {
    return this.WrapError((Func<SshResponse>) (() =>
    {
      this.CheckAuthorization(nameof (UploadUpdate), LoginUserType.Cs, LoginUserType.CsLight, LoginUserType.Ifp);
      this.CheckAvailability(nameof (UploadUpdate), BootMode.Recovery, BootMode.Maintenance);
      SshResponse sshResponse = this._sshClient.ExecuteCommand("upload-update", extraBuffer);
      if (sshResponse.Success)
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "upload-update was successful", memberName: nameof (UploadUpdate), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 554);
      else
        this._loggingService.getLogger().LogAppError(LoggingContext.APPLIANCE, "upload-update failed. " + sshResponse.ErrorMessage, memberName: nameof (UploadUpdate), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 558);
      return sshResponse;
    }));
  }

  public SshResponse UploadSessionDoc(Stream extraBuffer)
  {
    return this.WrapError((Func<SshResponse>) (() =>
    {
      SshResponse sshResponse = this._sshClient.ExecuteCommand("upload-session-doc", extraBuffer);
      if (sshResponse.Success)
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "upload-session-doc was successful", memberName: nameof (UploadSessionDoc), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 572);
      else
        this._loggingService.getLogger().LogAppError(LoggingContext.APPLIANCE, "upload-session-doc failed. " + sshResponse.ErrorMessage, memberName: nameof (UploadSessionDoc), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 576);
      return sshResponse;
    }));
  }

  public SshResponse StartSession(string parameter)
  {
    return this.WrapError((Func<SshResponse>) (() =>
    {
      MemoryStream extraBuffer = new MemoryStream();
      extraBuffer.Write(Encoding.ASCII.GetBytes(parameter), 0, parameter.Length);
      extraBuffer.Flush();
      extraBuffer.Position = 0L;
      Encoding.UTF8.GetString(extraBuffer.ToArray());
      SshResponse sshResponse = this._sshClient.ExecuteCommand("start-session", (Stream) extraBuffer);
      if (sshResponse.Success)
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "start-session was successful", memberName: nameof (StartSession), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 599);
      else
        this._loggingService.getLogger().LogAppError(LoggingContext.APPLIANCE, "start-session failed. " + sshResponse.ErrorMessage, memberName: nameof (StartSession), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 603);
      return sshResponse;
    }));
  }

  public SshResponse RestartSession()
  {
    return this.WrapError((Func<SshResponse>) (() =>
    {
      SshResponse sshResponse = this._sshClient.ExecuteCommand("restart-session");
      if (sshResponse.Success)
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "restart-session was successful", memberName: nameof (RestartSession), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 616);
      else
        this._loggingService.getLogger().LogAppError(LoggingContext.APPLIANCE, "restart-session failed. " + sshResponse.ErrorMessage, memberName: nameof (RestartSession), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 620);
      return sshResponse;
    }));
  }

  public SshResponse GetSshAvailability()
  {
    return this.WrapError((Func<SshResponse>) (() =>
    {
      SshResponse sshAvailability = this._sshClient.ExecuteCommand("get-ssh-availability");
      if (sshAvailability.Success)
      {
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "ssh-availability is successful", memberName: nameof (GetSshAvailability), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 639);
      }
      else
      {
        this._loggingService.getLogger().LogAppError(LoggingContext.APPLIANCE, "ssh-availability failed. " + sshAvailability.ErrorMessage, memberName: nameof (GetSshAvailability), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 643);
        sshAvailability.Success = sshAvailability.ErrorMessage.Contains("command not found");
      }
      return sshAvailability;
    }));
  }

  public SshResponse UploadFieldsToJson(string content, iService5.Ssh.models.ProgressCallback progressCallback)
  {
    return this.WrapError((Func<SshResponse>) (() =>
    {
      MemoryStream extraBuffer = new MemoryStream();
      extraBuffer.Write(Encoding.ASCII.GetBytes(content), 0, content.Length);
      extraBuffer.Flush();
      extraBuffer.Position = 0L;
      return this._sshClient.ExecuteCommand("set-ha-params", (Stream) extraBuffer, progressCallback);
    }));
  }

  public SshResponse<ApplianceCodingDto> PrepareCoding()
  {
    return this.WrapError<ApplianceCodingDto>((Func<SshResponse<ApplianceCodingDto>>) (() =>
    {
      SshResponse sshResponse = this._sshClient.ExecuteCommand("prepare-coding");
      if (sshResponse.Success)
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "prepare-coding was successful", memberName: nameof (PrepareCoding), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 676);
      else
        this._loggingService.getLogger().LogAppError(LoggingContext.APPLIANCE, "prepare-coding failed. " + sshResponse.ErrorMessage, memberName: nameof (PrepareCoding), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 680);
      return sshResponse.Cast<ApplianceCodingDto>();
    }));
  }

  public SshResponse<ApplianceFlashingDto> PrepareFlashing()
  {
    return this.WrapError<ApplianceFlashingDto>((Func<SshResponse<ApplianceFlashingDto>>) (() =>
    {
      SshResponse sshResponse = this._sshClient.ExecuteCommand("prepare-flashing");
      if (sshResponse.Success)
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "prepare-flashing was successful", memberName: nameof (PrepareFlashing), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 693);
      else
        this._loggingService.getLogger().LogAppError(LoggingContext.APPLIANCE, "prepare-flashing failed. " + sshResponse.ErrorMessage, memberName: nameof (PrepareFlashing), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 697);
      return sshResponse.Cast<ApplianceFlashingDto>();
    }));
  }

  public SshResponse Reboot()
  {
    return this.WrapError((Func<SshResponse>) (() =>
    {
      this.CheckAuthorization(nameof (Reboot), LoginUserType.Cs, LoginUserType.CsLight, LoginUserType.Ifp);
      this.CheckAvailability(nameof (Reboot), BootMode.Recovery, BootMode.Maintenance, BootMode.Elp);
      SshResponse sshResponse = this._sshClient.ExecuteCommand("reboot");
      if (sshResponse.Success)
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Reboot was successful", memberName: nameof (Reboot), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 712);
      else
        this._loggingService.getLogger().LogAppError(LoggingContext.APPLIANCE, "Reboot failed. " + sshResponse.ErrorMessage, memberName: nameof (Reboot), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 716);
      return sshResponse;
    }));
  }

  public SshResponse EnableDbusECUUpdate()
  {
    return this.WrapError((Func<SshResponse>) (() =>
    {
      this.CheckAuthorization(nameof (EnableDbusECUUpdate), LoginUserType.Cs, LoginUserType.CsLight, LoginUserType.Ifp);
      this.CheckAvailability(nameof (EnableDbusECUUpdate), BootMode.Recovery, BootMode.Maintenance, BootMode.Elp);
      SshResponse sshResponse = this._sshClient.ExecuteCommand("enable-dbus2-ecu-update");
      if (sshResponse.Success)
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Enabling Dbus ECU Update was successful", memberName: nameof (EnableDbusECUUpdate), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 731);
      else
        this._loggingService.getLogger().LogAppError(LoggingContext.APPLIANCE, "Enabling Dbus ECU Update failed. " + sshResponse.ErrorMessage, memberName: nameof (EnableDbusECUUpdate), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 735);
      return sshResponse;
    }));
  }

  public SshResponse InstallUpdate(Stream extraBuffer)
  {
    return this.WrapError((Func<SshResponse>) (() =>
    {
      this.CheckAuthorization(nameof (InstallUpdate), LoginUserType.Cs, LoginUserType.CsLight, LoginUserType.Ifp);
      this.CheckAvailability(nameof (InstallUpdate), BootMode.Recovery, BootMode.Maintenance);
      SshResponse sshResponse = this._sshClient.ExecuteCommand("install-update", extraBuffer);
      if (sshResponse.Success)
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "install-update succeeded", memberName: nameof (InstallUpdate), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 754);
      else
        this._loggingService.getLogger().LogAppError(LoggingContext.APPLIANCE, "install-update failed. " + sshResponse.ErrorMessage, memberName: nameof (InstallUpdate), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 758);
      return sshResponse;
    }));
  }

  public SshResponse InstallUpdateNonSmm(Stream extraBuffer, iService5.Ssh.models.ProgressCallback progressCallback)
  {
    return this.WrapError((Func<SshResponse>) (() =>
    {
      SshResponse sshResponse = this._sshClient.ExecuteCommand("install-update-nonsmm", extraBuffer, progressCallback);
      if (sshResponse.Success)
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Install Update Non SMM was successful", memberName: nameof (InstallUpdateNonSmm), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 775);
      else
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Install Update Non SMM failed. " + sshResponse.ErrorMessage, memberName: nameof (InstallUpdateNonSmm), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 779);
      return sshResponse;
    }));
  }

  public SshResponse GetValues(Stream extraBuffer, iService5.Ssh.models.ProgressCallback progressCallback)
  {
    return this.WrapError((Func<SshResponse>) (() =>
    {
      SshResponse values = this._sshClient.ExecuteCommand("get-values", extraBuffer, progressCallback);
      if (values.Success)
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Get Values was successful", memberName: nameof (GetValues), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 792);
      else
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Get Values failed. " + values.ErrorMessage, memberName: nameof (GetValues), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 796);
      return values;
    }));
  }

  public SshResponse CheckDBusConnection()
  {
    return this.WrapError((Func<SshResponse>) (() =>
    {
      SshResponse sshResponse = this._sshClient.ExecuteCommand("bridge-status");
      if (!sshResponse.Success)
        this._loggingService.getLogger().LogAppDebug(LoggingContext.PROGRAMNONSMM, "Checking DBus Connection failed. " + sshResponse.ErrorMessage, memberName: nameof (CheckDBusConnection), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 809);
      return sshResponse;
    }));
  }

  public SshResponse InstallStream(string streamLocation)
  {
    return this.WrapError((Func<SshResponse>) (() =>
    {
      this.CheckAuthorization(nameof (InstallStream), LoginUserType.Ifp);
      this.CheckAvailability(nameof (InstallStream), BootMode.Recovery, BootMode.Maintenance);
      SshResponse sshResponse = this._sshClient.ExecuteCommand("install-stream " + streamLocation);
      if (sshResponse.Success)
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Installing of Stream was successful", memberName: nameof (InstallStream), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 827);
      else
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Installing of Stream failed. " + sshResponse.ErrorMessage, memberName: nameof (InstallStream), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 831);
      return sshResponse;
    }));
  }

  public SshResponse SecureDevice()
  {
    return this.WrapError((Func<SshResponse>) (() =>
    {
      this.CheckAuthorization(nameof (SecureDevice), LoginUserType.Cs, LoginUserType.CsLight, LoginUserType.Ifp);
      this.CheckAvailability(nameof (SecureDevice), BootMode.Recovery, BootMode.Maintenance, BootMode.Elp);
      SshResponse sshResponse = this._sshClient.ExecuteCommand("secure-device");
      if (sshResponse.Success)
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Securing Device was successful", memberName: nameof (SecureDevice), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 851);
      else
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Securing Device failed. " + sshResponse.ErrorMessage, memberName: nameof (SecureDevice), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 855);
      return sshResponse;
    }));
  }

  public SshResponse IsDeviceSecure()
  {
    return this.WrapError((Func<SshResponse>) (() =>
    {
      this.CheckAuthorization(nameof (IsDeviceSecure), LoginUserType.Cs, LoginUserType.CsLight, LoginUserType.Ifp);
      this.CheckAvailability(nameof (IsDeviceSecure), BootMode.Recovery, BootMode.Maintenance, BootMode.Elp);
      SshResponse sshResponse = this._sshClient.ExecuteCommand("is-device-secured");
      if (sshResponse.Success)
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Inquiring if Device is secure was successful", memberName: nameof (IsDeviceSecure), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 873);
      else
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Inquiring if Device is secure failed. " + sshResponse.ErrorMessage, memberName: nameof (IsDeviceSecure), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 877);
      return sshResponse;
    }));
  }

  public SshResponse Purge()
  {
    return this.WrapError((Func<SshResponse>) (() =>
    {
      this.CheckAuthorization(nameof (Purge), LoginUserType.Cs, LoginUserType.CsLight, LoginUserType.Ifp);
      this.CheckAvailability(nameof (Purge), BootMode.Recovery, BootMode.Maintenance, BootMode.Elp);
      SshResponse sshResponse = this._sshClient.ExecuteCommand("purge");
      if (sshResponse.Success)
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Purge was successful", memberName: nameof (Purge), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 895);
      else
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Purge failed. " + sshResponse.ErrorMessage, memberName: nameof (Purge), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 899);
      return sshResponse;
    }));
  }

  public SshResponse StartMonitoring()
  {
    return this.WrapError((Func<SshResponse>) (() =>
    {
      SshResponse sshResponse = this._sshClient.ExecuteCommand("monitor-start");
      if (sshResponse.Success)
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Monitoring started successfully.", memberName: nameof (StartMonitoring), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 915);
      else
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Monitoring failed to start. " + sshResponse.ErrorMessage, memberName: nameof (StartMonitoring), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 919);
      return sshResponse;
    }));
  }

  private void CheckAuthorization(string methodName, params LoginUserType[] userTypes)
  {
    if (!this.LoginType.HasValue)
      throw new AuthenticationException("You must login to the service.");
    if (!((IEnumerable<LoginUserType>) userTypes).Any<LoginUserType>((Func<LoginUserType, bool>) (x => x == this.LoginType.Value)))
      throw new UnauthorizedAccessException($"{this.LoginType} is not allowed for action {methodName}");
  }

  internal void CheckAvailability(string methodName, params BootMode[] valid)
  {
    if (!this.CurrentBootMode.HasValue)
      throw new AuthenticationException("You must request / set current availability before running other operations.");
    if (!((IEnumerable<BootMode>) valid).Any<BootMode>((Func<BootMode, bool>) (x => x == this.CurrentBootMode.Value)))
      throw new UnauthorizedAccessException($"{this.CurrentBootMode} is not allowed for action {methodName}");
  }

  private SshResponse WrapError(Func<SshResponse> del)
  {
    try
    {
      return del();
    }
    catch (Exception ex)
    {
      return SshResponse.Error(ex.Message);
    }
  }

  private SshResponse<T> WrapError<T>(Func<SshResponse<T>> del)
  {
    try
    {
      return del();
    }
    catch (Exception ex)
    {
      return SshResponse<T>.Error(ex.Message);
    }
  }

  public SshResponse TestPowerSocket(string testResult)
  {
    return this.WrapError((Func<SshResponse>) (() =>
    {
      SshResponse sshResponse = this._sshClient.ExecuteCommand("pe-check " + testResult);
      if (sshResponse.Success)
      {
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, $"Calling pe-check with {testResult} was successfull", memberName: nameof (TestPowerSocket), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 1018);
        return SshResponse.Valid();
      }
      this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, $"Calling pe-check with {testResult} was not successfull{sshResponse.ErrorOut}", memberName: nameof (TestPowerSocket), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 1023 /*0x03FF*/);
      return sshResponse;
    }));
  }

  public SshResponse GetSSHCommands()
  {
    return this.WrapError((Func<SshResponse>) (() =>
    {
      SshResponse sshCommands = this._sshClient.ExecuteCommand("get-ssh-commands");
      if (sshCommands.Success)
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "ssh-commands was successful", memberName: nameof (GetSSHCommands), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 1036);
      else
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "ssh-commands failed. Error=" + sshCommands.ErrorOut, memberName: nameof (GetSSHCommands), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 1040);
      return sshCommands;
    }));
  }

  public SshResponse<List<BridgeSettingDto>> GetBridgeSettings()
  {
    return this.WrapError<List<BridgeSettingDto>>((Func<SshResponse<List<BridgeSettingDto>>>) (() =>
    {
      SshResponse sshResponse = this._sshClient.ExecuteCommand("get-settings");
      if (sshResponse.Success)
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Getting bridge settings was successful: " + sshResponse.RawResponse, memberName: nameof (GetBridgeSettings), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 1053);
      else
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Getting bridge settings failed. Error: " + sshResponse.ErrorMessage, memberName: nameof (GetBridgeSettings), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 1057);
      return sshResponse.Cast<List<BridgeSettingDto>>();
    }));
  }

  public SshResponse<List<BridgeSettingDto>> GetCATSettings()
  {
    return this.WrapError<List<BridgeSettingDto>>((Func<SshResponse<List<BridgeSettingDto>>>) (() =>
    {
      SshResponse sshResponse = this._sshClient.ExecuteCommand("get-cat-settings");
      if (sshResponse.Success)
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Getting bridge settings was successful: " + sshResponse.RawResponse, memberName: nameof (GetCATSettings), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 1070);
      else
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Getting bridge settings failed. Error: " + sshResponse.ErrorMessage, memberName: nameof (GetCATSettings), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 1074);
      return sshResponse.Cast<List<BridgeSettingDto>>();
    }));
  }

  public SshResponse<Dictionary<string, string>> SetBridgeSettings(string content)
  {
    return this.WrapError<Dictionary<string, string>>((Func<SshResponse<Dictionary<string, string>>>) (() =>
    {
      MemoryStream extraBuffer = new MemoryStream();
      extraBuffer.Write(Encoding.ASCII.GetBytes(content), 0, content.Length);
      extraBuffer.Flush();
      extraBuffer.Position = 0L;
      SshResponse sshResponse = this._sshClient.ExecuteCommand("set-settings", (Stream) extraBuffer);
      if (sshResponse.Success)
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Updating bridge settings was successful", memberName: nameof (SetBridgeSettings), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 1091);
      else
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Updating bridge settings failed. Error: " + sshResponse.ErrorMessage, memberName: nameof (SetBridgeSettings), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 1095);
      return sshResponse.Cast<Dictionary<string, string>>();
    }));
  }

  public SshResponse<LogSessionDto> GetLogSessions()
  {
    return this.WrapError<LogSessionDto>((Func<SshResponse<LogSessionDto>>) (() =>
    {
      SshResponse sshResponse = this._sshClient.ExecuteCommand("get-log-sessions");
      if (sshResponse.Success)
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Log session was successfully retrieved", memberName: nameof (GetLogSessions), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 1114);
      else
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Failed to retrieve Log session. " + sshResponse.ErrorMessage, memberName: nameof (GetLogSessions), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 1118);
      return sshResponse.Cast<LogSessionDto>();
    }));
  }

  public SshResponse GetBridgeLogForSingleSession(SessionDto session)
  {
    return this.WrapError((Func<SshResponse>) (() =>
    {
      string ts = session.ts;
      StringBuilder stringBuilder = new StringBuilder("get-log");
      stringBuilder.Append(" ");
      stringBuilder.Append(session.material);
      stringBuilder.Append("@");
      stringBuilder.Append(ts);
      SshResponse forSingleSession = this._sshClient.ExecuteCommand(stringBuilder.ToString());
      if (forSingleSession.Success)
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Log Session was successfully retrieved", memberName: nameof (GetBridgeLogForSingleSession), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 1145);
      else
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Failed to retrieve Log Sessions. ", memberName: nameof (GetBridgeLogForSingleSession), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 1149);
      return forSingleSession;
    }));
  }

  public SshResponse GetLogFile()
  {
    return this.WrapError((Func<SshResponse>) (() =>
    {
      SshResponse logFile = this._sshClient.ExecuteCommand("get-log");
      if (logFile.Success)
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Main Log session was successfully retrieved", memberName: nameof (GetLogFile), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 1165);
      else
        this._loggingService.getLogger().LogAppInformation(LoggingContext.APPLIANCE, "Failed to retrieve main Log session. ", memberName: nameof (GetLogFile), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Appliance/Is5SshWrapper.cs", sourceLineNumber: 1169);
      return logFile;
    }));
  }

  public void Dispose() => this._sshClient.Dispose();
}
