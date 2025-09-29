// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Data.SyMaNa.SyMaNaWebSocketServices
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Appliance;
using iService5.Ssh.DTO;
using Newtonsoft.Json;
using System.Collections.Generic;

#nullable disable
namespace iService5.Core.Services.Data.SyMaNa;

public class SyMaNaWebSocketServices
{
  public long sID;
  public long msgID;
  public long version;
  public string ppfData;
  private IApplianceSession Session;

  public SyMaNaWebSocketServices(long _sID, long _msgID, long _version)
  {
    this.sID = _sID;
    this.msgID = _msgID;
    this.version = _version;
  }

  public void SetApplianceSession(IApplianceSession _session) => this.Session = _session;

  public SyMaNaWebsocketData getSyMaNaWebSocketObject(string action, string resource)
  {
    return new SyMaNaWebsocketData()
    {
      sID = this.sID,
      msgID = this.msgID,
      version = this.version,
      resource = resource,
      action = action
    };
  }

  public void UpdateMsgIDCounter() => ++this.msgID;

  public string PrepareJSONRequest(SyMaNaRequestCommandName jsonReqType)
  {
    SyMaNaLodisDataModel naLodisDataModel = new SyMaNaLodisDataModel(this.getSyMaNaWebSocketObject("POST", "/mtc/command"));
    string str = (string) null;
    if (naLodisDataModel != null)
    {
      List<SyMaNaLodisRequestData> lodisRequestDataList = new List<SyMaNaLodisRequestData>()
      {
        this.createLodisRequestCmndData(jsonReqType)
      };
      naLodisDataModel.data = lodisRequestDataList;
      naLodisDataModel.version = 1L;
      str = JsonConvert.SerializeObject((object) naLodisDataModel);
    }
    return str;
  }

  private SyMaNaLodisRequestData createLodisRequestCmndData(SyMaNaRequestCommandName jsonReqType)
  {
    SyMaNaLodisRequestData lodisRequestCmndData = new SyMaNaLodisRequestData();
    lodisRequestCmndData.command = jsonReqType.ToString();
    switch (jsonReqType)
    {
      case SyMaNaRequestCommandName.preparePackageUpload:
        lodisRequestCmndData.parameter = this.createPackagePropertiesParams();
        break;
      case SyMaNaRequestCommandName.startInstallation:
        lodisRequestCmndData.parameter = this.createInstallAllPackageProperyParams();
        break;
      case SyMaNaRequestCommandName.setHaInfo:
        lodisRequestCmndData.parameter = this.CreateHaInfoData();
        break;
      case SyMaNaRequestCommandName.setHaCountrySettings:
      case SyMaNaRequestCommandName.setHaCustomerIndex:
      case SyMaNaRequestCommandName.setHaManufacturingTimestamp:
      case SyMaNaRequestCommandName.setHaVib:
      case SyMaNaRequestCommandName.setHaBrand:
      case SyMaNaRequestCommandName.setHaDeviceType:
        lodisRequestCmndData.parameter = this.CreateHaInfoRequestData(jsonReqType);
        break;
    }
    return lodisRequestCmndData;
  }

  public SyMaNaLodisParameters createPackagePropertiesParams()
  {
    string str = "CUcABvj_AACSMpdGSh0R7gACocxTSEEtMjU2APiXx9rEwf5weJeS4m6Kln9s-wFaBLTS1M2sfZ_b0F8aXP4QcUUMS0I5Nk5WRkUwLzAxAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAZghdMGQCMH1vVuAfStwVNKm7bZU-ygwAL9IP-rgwUolcVB2iAxwS3TOulc-lS5hRCU-AWjhdXAIwIYp7bfya10LmY0m9tC0qDXC_RghCGgJ3yHFHPScktgQuv1ldcmAMaFWVYj3e3LsZBEMwggQ_MIIDxaADAgECAhAbXyadF4ww5plj1LWdbJXuMAoGCCqGSM49BAMDMGQxCzAJBgNVBAYTAkRFMRowGAYDVQQKDBFIb21lIENvbm5lY3QgR21iSDEZMBcGA1UECwwQUHJvZHVjdCBTZWN1cml0eTEeMBwGA1UEAwwVVXBkYXRlIFBhY2thZ2luZyBDQSAxMB4XDTIzMDUxNzAwMDAwMFoXDTI0MDIxNTIzNTk1OVowdDEuMCwGA1UEAwwlQlNIX1VwZGF0ZV9QYWNrYWdlX1NpZ25pbmdfUHJvZHVjdGlvbjEZMBcGA1UECwwQUHJvZHVjdCBTZWN1cml0eTEaMBgGA1UECgwRSG9tZSBDb25uZWN0IEdtYkgxCzAJBgNVBAYTAkRFMHYwEAYHKoZIzj0CAQYFK4EEACIDYgAEOKf7smCNtgod-fOqA9dukhBrrUETFP15Bj3OSw7s2tMyX7Q6qJtFQf1eZEJVByaQKp8wOp1kuDy4Il967Y5gm2QsWFwtHXnbHrr1WB0xyEeyxM1lBQbemcLi0B37L_9Go4ICKjCCAiYwDAYDVR0TAQH_BAIwADAOBgNVHQ8BAf8EBAMCB4AwFgYDVR0lAQH_BAwwCgYIKwYBBQUHAwMwHQYDVR0OBBYEFD3XXiiZFGXbhNHoc8MLqZTKrt0jMEwGA1UdIARFMEMwQQYLKwYBBAGBnXsDAQEwMjAwBggrBgEFBQcCARYkaHR0cHM6Ly93d3cuYnNoLWdyb3VwLmNvbS9kaWdpdGFsLWlkMGQGA1UdHwRdMFswWaBXoFWGU2h0dHA6Ly9wa2ktY3JsMTQuaG9tZS1jb25uZWN0LmNvbS9jYV81M2YzZmI5ZDA0ZWVlMDhiMmUwYTMyYTQ0MzhkYzM0Ni9MYXRlc3RDUkwuY3JsMIGOBggrBgEFBQcBAQSBgTB_MC4GCCsGAQUFBzABhiJodHRwOi8vcGtpLW9jc3AxNC5ob21lLWNvbm5lY3QuY29tME0GCCsGAQUFBzAChkFodHRwOi8vcGtpLWNhMTQuaG9tZS1jb25uZWN0LmNvbS83NGU5Zjg1NTQzMDgwMzlkYTU1YjEyZGIwM2RkY2E0OTAfBgNVHSMEGDAWgBS5mRWN1UgmXMGhgDqO7PeXuRPBoDAuBgpghkgBhvhFARADBCAwHgYTYIZIAYb4RQEQAQQpAQGFmPaSaBYHMTE0MzUwMjA5BgpghkgBhvhFARAFBCswKQIBABYkYUhSMGNITTZMeTl3YTJrdGNtRXVjM2x0WVhWMGFDNWpiMjA9MAoGCCqGSM49BAMDA2gAMGUCMQD1mh0_GG_4fwiF043IVoT2g88l3Rf9xLTSnvNkH2ts7ICnkNAcuZM1J9xmjse6UZYCMFLeHi2uyEmKd5bnxXP3E2EuLWWyP9UoGR-rNwlmfPydaI2TP987Lh4v2AhczGeWEQQWMIIEEjCCA5igAwIBAgIKIAGgAAAAAAAABjAKBggqhkjOPQQDAzBXMQswCQYDVQQGEwJERTEaMBgGA1UECgwRSG9tZSBDb25uZWN0IEdtYkgxGTAXBgNVBAsMEFByb2R1Y3QgU2VjdXJpdHkxETAPBgNVBAMMCEhDQiBDQSAxMB4XDTIwMDExMzEyMDAwMFoXDTQwMDExMjEyMDAwMFowZDELMAkGA1UEBhMCREUxGjAYBgNVBAoMEUhvbWUgQ29ubmVjdCBHbWJIMRkwFwYDVQQLDBBQcm9kdWN0IFNlY3VyaXR5MR4wHAYDVQQDDBVVcGRhdGUgUGFja2FnaW5nIENBIDEwdjAQBgcqhkjOPQIBBgUrgQQAIgNiAASoPIkKJwGBkNohgc3qe9aIk_IKQaJIX5uWcpYe9rZLaHuEF8jq_MxJTPQxX8WXF_YxaNdi7O8ztQiv2OPNEHflKIcSptBUIfRUPDxhf9PKRzZJb47UjTAWR6rLM-whxWWjggIgMIICHDAdBgNVHQ4EFgQUuZkVjdVIJlzBoYA6juz3l7kTwaAwHwYDVR0jBBgwFoAUmTpTUi5SD-DSxU0Gv8qNr_2HMwowgYsGCCsGAQUFBwEBBH8wfTBLBggrBgEFBQcwAoY_aHR0cDovL3BraS1jYS5ob21lLWNvbm5lY3QuY29tL2Q3MDY5YWI2MTgwNGZmOTg0NzlkZjNkMDIzNmM5Y2FjMC4GCCsGAQUFBzABhiJodHRwOi8vcGtpLW9jc3AxMi5ob21lLWNvbm5lY3QuY29tMFEGA1UdHwRKMEgwRqBEoEKGQGh0dHA6Ly9wa2ktY3JsLmhvbWUtY29ubmVjdC5jb20vMzRiMDYxZTUyYzdhZTdjYWRmYjhjYmY1YmNlYjQ5YjcwEgYDVR0TAQH_BAgwBgEB_wIBADAOBgNVHQ8BAf8EBAMCAQYwgacGA1UdIASBnzCBnDCBmQYLKwYBBAGBnXsDAQEwgYkwMAYIKwYBBQUHAgEWJGh0dHBzOi8vd3d3LmJzaC1ncm91cC5jb20vZGlnaXRhbC1pZDBVBggrBgEFBQcCAjBJGkdUaGlzIENlcnRpZmljYXRlIFBvbGljeSByZWd1bGF0ZXMgcmVxdWlyZW1lbnRzIGZvciBCU0ggUHJvZHVjdCBDQSAxIFBLSTArBgNVHREEJDAipCAwHjEcMBoGA1UEAwwTU1lNQy1FQ0MtQ0EtcDM4NC0xMzAKBggqhkjOPQQDAwNoADBlAjASwHnm-RdDmGiptaUVQUSQqBa9zWO24azuDvLfYhqAfKw-dZQNlTcsPmEZPKUwnLMCMQCKP6CsFzboXd5mFjRXJdCFi4IZZx_I7O3iRa4GjewLVJQ98NbkDwS2OfztU6XParg";
    return new SyMaNaLodisParameters()
    {
      packageProperties = this.ppfData == null ? str : this.ppfData
    };
  }

  private SyMaNaLodisParameters createInstallAllPackageProperyParams()
  {
    return new SyMaNaLodisParameters()
    {
      stringList = new List<StringListToStartInstallation>()
    };
  }

  private SyMaNaLodisParameters CreateHaInfoData()
  {
    HaInfoDto infoFromMetadata = this.Session.HaInfoFromMetadata;
    return new SyMaNaLodisParameters();
  }

  private SyMaNaLodisParameters CreateHaInfoRequestData(SyMaNaRequestCommandName jsonReqType)
  {
    HaInfoDto infoFromMetadata = this.Session.HaInfoFromMetadata;
    SyMaNaLodisParameters haInfoRequestData = new SyMaNaLodisParameters();
    switch (jsonReqType)
    {
      case SyMaNaRequestCommandName.setHaCountrySettings:
        haInfoRequestData.parameter = (object) infoFromMetadata.CountrySettings;
        break;
      case SyMaNaRequestCommandName.setHaCustomerIndex:
        haInfoRequestData.parameter = (object) infoFromMetadata.CustomerIndex;
        break;
      case SyMaNaRequestCommandName.setHaManufacturingTimestamp:
        haInfoRequestData.parameter = (object) infoFromMetadata.ManufacturingTimeStamp;
        break;
      case SyMaNaRequestCommandName.setHaVib:
        haInfoRequestData.parameter = (object) infoFromMetadata.Vib;
        break;
      case SyMaNaRequestCommandName.setHaBrand:
        haInfoRequestData.parameter = (object) infoFromMetadata.Brand.ToUpper();
        break;
      case SyMaNaRequestCommandName.setHaDeviceType:
        haInfoRequestData.parameter = (object) infoFromMetadata.DeviceType;
        break;
    }
    return haInfoRequestData;
  }

  public static SyMaNaLodisResponseData PrepareLodisResponseData(SyMaNaLodisResponse response)
  {
    if (response.lodisResponseError == null)
      return response.data.Count > 0 ? response.data[0] : (SyMaNaLodisResponseData) null;
    return new SyMaNaLodisResponseData()
    {
      error = response.lodisResponseError
    };
  }
}
