// Decompiled with JetBrains decompiler
// Type: iService5.Core.Services.Data.MetadataService
// Assembly: iService5.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D058254E-CC93-44A4-A5C7-66EA8C408758
// Assembly location: D:\AGD\Apps\iS5\iService5_1.64_APKPure\com.bshg.iservice5.droid\out\iService5.Core.dll

using iService5.Core.Services.Helpers;
using iService5.Core.Services.Platform;
using iService5.Core.Services.User.ActivityLogging;
using iService5.Core.ViewModels;
using iService5.Ssh.DTO;
using MvvmCross;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

#nullable disable
namespace iService5.Core.Services.Data;

public class MetadataService : IMetadataService
{
    private readonly IPlatformSpecificServiceLocator _locator;
    private readonly ILoggingService _loggingService;
    private readonly ISecureStorageService _secureStorageService;
    internal MetadataDB metadataDB;
    private bool _isValid;
    private long _fileSize = 0;
    private long usedStorage = 0;
    private readonly string _tempDbPassword;
    private const string US_COUNTRY_CODE = "US";
    private readonly Dictionary<string, string> iDecodeBrands = new Dictionary<string, string>();

    public bool isValid
    {
        get => this._isValid;
        internal set
        {
            if (!value)
            {
                CoreApp.settings.DeleteItem(new Settings("lastUpdate", ""));
                CoreApp.settings.DeleteItem(new Settings("lastCheck", ""));
            }
            this._isValid = value;
        }
    }

    public bool updateOutdated { get; private set; }

    public List<string> refetchedPPFsEnumbersFromOldDB { get; set; }

    public string GetFileSize() => FileSizeFormatter.FormatSize(this.usedStorage);

    public bool UpdatePendingMoreThan14Days() => this.updateOutdated;

    public void updateUsedStorage() => this.usedStorage = this.metadataDB.GetDownloadedFileSize();

    public string GetMetaDataFileSize()
    {
        string metaDataFileSize = "100";
        Settings settings = CoreApp.settings.GetItem("metadataContentLength");
        if (settings != null && settings.Value != "")
            metaDataFileSize = FileSizeFormatter.FormatSize(Convert.ToInt64(settings.Value));
        return metaDataFileSize;
    }

    public MetadataService(
      IPlatformSpecificServiceLocator locator,
      ILoggingService _logService,
      ISecureStorageService secureStorageService,
      string dbPassword = null,
      string tempDbPassword = null)
    {
        this._locator = locator;
        this._loggingService = _logService;
        this._secureStorageService = secureStorageService;
        string dbPassword1 = this._locator.GetPlatformSpecificService().getValueFromSecureStorageForKey(SecureStorageKeys.DB_PASSWORD.ToString()).Result ?? dbPassword;
        this._tempDbPassword = tempDbPassword;
        if (File.Exists(Path.Combine(this._locator.GetPlatformSpecificService().getFolder(), "Metadata.db3")))
        {
            try
            {
                this.metadataDB = new MetadataDB(Path.Combine(this._locator.GetPlatformSpecificService().getFolder(), "Metadata.db3"), dbPassword1);
                Settings settings = CoreApp.settings.GetItem("lastUpdate");
                if (settings == null)
                {
                    DateTime dateTime = DateTime.Today;
                    dateTime = dateTime.AddDays(-16.0);
                    settings = new Settings("lastUpdate", dateTime.ToString((IFormatProvider)CultureInfo.InvariantCulture));
                }
                CultureInfo invariantCulture = CultureInfo.InvariantCulture;
                DateTime.TryParseExact(settings.Value, "dd/MM/yyyy", (IFormatProvider)invariantCulture, DateTimeStyles.None, out DateTime _);
                this.isValid = true;
                this.usedStorage = this.metadataDB.GetDownloadedFileSize();
            }
            catch (Exception ex)
            {
                this._loggingService.getLogger().LogAppError(LoggingContext.METADATA, "Failed to retrieve metadata DB", ex, ".ctor", "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataService.cs", 116);
                this.metadataDB?.Database?.Close();
                this.metadataDB = (MetadataDB)null;
                CoreApp.settings.GetItem("lastUpdate");
                this.isValid = false;
                if (!this.IsFileLocked(new FileInfo(Path.Combine(this._locator.GetPlatformSpecificService().getFolder(), "Metadata.db3"))))
                    File.Delete(Path.Combine(this._locator.GetPlatformSpecificService().getFolder(), "Metadata.db3"));
                this.usedStorage = 0L;
            }
        }
        else
        {
            this.isValid = false;
            this.metadataDB = (MetadataDB)null;
            this.usedStorage = 0L;
        }
        try
        {
            this.iDecodeBrands["555"] = "TPM";
            this.iDecodeBrands["556"] = "Gram";
            this.iDecodeBrands["557"] = "Alpro Medical";
            this.iDecodeBrands["558"] = "IC Medical";
            this.iDecodeBrands["559"] = "Interservice";
            this.iDecodeBrands["560"] = "Interservice B2C";
            this.iDecodeBrands["561"] = "Interservice B2E";
            this.iDecodeBrands["562"] = "ACESOL";
            this.iDecodeBrands["563"] = "SAIVOD";
            this.iDecodeBrands["564"] = "Ansonic";
            this.iDecodeBrands["565"] = "TECLINE";
            this.iDecodeBrands["A01"] = "Bosch";
            this.iDecodeBrands["A02"] = "Siemens";
            this.iDecodeBrands["A03"] = "Constructa";
            this.iDecodeBrands["A04"] = "Neff";
            this.iDecodeBrands["A11"] = "Junker&Ruh";
            this.iDecodeBrands["A12"] = "Protos";
            this.iDecodeBrands["A13"] = "Viva";
            this.iDecodeBrands["A15"] = "Gaggenau";
            this.iDecodeBrands["A16"] = "Solitaire";
            this.iDecodeBrands["A17"] = "Junker";
            this.iDecodeBrands["A18"] = "Zelmer";
            this.iDecodeBrands["A23"] = "Balay";
            this.iDecodeBrands["A24"] = "Lynx";
            this.iDecodeBrands["A25"] = "Superser";
            this.iDecodeBrands["A26"] = "Crolls";
            this.iDecodeBrands["A27"] = "Agni";
            this.iDecodeBrands["A28"] = "Corcho";
            this.iDecodeBrands["A29"] = "Ufesa";
            this.iDecodeBrands["A31"] = "Pitsos";
            this.iDecodeBrands["A41"] = "Profilo";
            this.iDecodeBrands["A51"] = "Yangzi";
            this.iDecodeBrands["A60"] = "Continental";
            this.iDecodeBrands["A61"] = "Continental";
            this.iDecodeBrands["A62"] = "Coldex";
            this.iDecodeBrands["A70"] = "Thermador";
            this.iDecodeBrands["A99"] = "Home Connect";
        }
        catch (Exception ex)
        {
            this._loggingService.getLogger().LogAppError(LoggingContext.APPLIANCE, "Failed to decode brand", ex, ".ctor", "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataService.cs", 178);
        }
    }

    protected virtual bool IsFileLocked(FileInfo file)
    {
        FileStream fileStream = (FileStream)null;
        try
        {
            fileStream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
        }
        catch (IOException ex)
        {
            return true;
        }
        finally
        {
            fileStream?.Close();
        }
        return false;
    }

    public List<material> getMatchingEntries(string needle, MatchPattern criteria)
    {
        return this.metadataDB != null ? this.metadataDB.getMaterialMatches(needle, criteria) : new List<material>();
    }

    public List<uploadDocument> getMaterialDocuments(string enumber)
    {
        this._loggingService.getLogger().LogAppDebug(LoggingContext.METADATA, "getting documents for enumber=" + enumber, memberName: nameof(getMaterialDocuments), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataService.cs", sourceLineNumber: 218);
        if (this.metadataDB == null)
            return new List<uploadDocument>();
        List<uploadDocument> materialDocs = this.metadataDB.getMaterialDocs(enumber);
        this._loggingService.getLogger().LogAppDebug(LoggingContext.METADATA, $"Retrieved={materialDocs.Count.ToString()} documents for enumber={enumber}", memberName: nameof(getMaterialDocuments), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataService.cs", sourceLineNumber: 223);
        return materialDocs;
    }

    public uploadDocument GetCodingFileAvailable(string enumber)
    {
        try
        {
            return this.getMaterialDocuments(enumber).FirstOrDefault<uploadDocument>((Func<uploadDocument, bool>)(x => x.type == "14850"));
        }
        catch (Exception ex)
        {
            this._loggingService.getLogger().LogAppDebug(LoggingContext.METADATA, "Exception while fetching material documents " + ex.Message, memberName: nameof(GetCodingFileAvailable), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataService.cs", sourceLineNumber: 237);
            return (uploadDocument)null;
        }
    }

    public string GetCodingFilePath(string enumber)
    {
        uploadDocument codingFileAvailable = this.GetCodingFileAvailable(enumber);
        return codingFileAvailable == null ? string.Empty : Path.Combine(this._locator.GetPlatformSpecificService().getFolder(), codingFileAvailable.toFileName());
    }

    public IEnumerable<varcodes> getOldVarCodingStatus(string enumber)
    {
        this._loggingService.getLogger().LogAppDebug(LoggingContext.METADATA, "check material document for coding availabiltiy " + enumber, memberName: nameof(getOldVarCodingStatus), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataService.cs", sourceLineNumber: 254);
        return this.metadataDB != null ? this.metadataDB.checkOldVarCodingPrerequisites(enumber) : Enumerable.Empty<varcodes>();
    }

    public (string, string) getBrand(string enumber)
    {
        (string, string) brand = ("", "");
        if (this.isValid)
        {
            List<material> materialMatches = this.metadataDB.getMaterialMatches(enumber, MatchPattern.ALL);
            if (materialMatches.Count != 0)
            {
                brand.Item1 = materialMatches[0].brand;
                brand.Item2 = !this.iDecodeBrands.ContainsKey(materialMatches[0].brand) ? materialMatches[0].brand : this.iDecodeBrands[materialMatches[0].brand];
            }
        }
        return brand;
    }

    internal string MD5Bytes2String(byte[] md5)
    {
        string str = "";
        foreach (byte num in md5)
            str += num.ToString("X2");
        return str;
    }

    internal string MD5ComputeFile(string file)
    {
        if (!File.Exists(file))
            return "0";
        try
        {
            using (MD5 md5 = MD5.Create())
            {
                using (FileStream inputStream = File.OpenRead(file))
                    return this.MD5Bytes2String(md5.ComputeHash((Stream)inputStream));
            }
        }
        catch (Exception ex)
        {
            this._loggingService.getLogger().LogAppFatal(LoggingContext.BINARY, "Failed to calculate MD5 of " + file, ex, nameof(MD5ComputeFile), "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataService.cs", 320);
            return "0";
        }
    }

    public async Task<bool> resetDatabase(string checksum)
    {
        try
        {
            string _dbPassword = await this._locator.GetPlatformSpecificService().getValueFromSecureStorageForKey(SecureStorageKeys.DB_PASSWORD.ToString());
            this._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "Begin temp database setup", memberName: nameof(resetDatabase), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataService.cs", sourceLineNumber: 331);
            string md5 = this.MD5ComputeFile(Path.Combine(this._locator.GetPlatformSpecificService().getFolder(), "tmpDownloadDB.db3"));
            string headerChecksum = this.MD5Bytes2String(Convert.FromBase64String(checksum));
            this._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "End temp database setup", memberName: nameof(resetDatabase), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataService.cs", sourceLineNumber: 334);
            if (md5 != headerChecksum)
            {
                this._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, $"MD5: {md5} does not match metadata {headerChecksum}", memberName: nameof(resetDatabase), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataService.cs", sourceLineNumber: 338);
                return false;
            }
            this._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "Verifying new DB", memberName: nameof(resetDatabase), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataService.cs", sourceLineNumber: 341);
            if (MetadataDB.testDBFile(Path.Combine(this._locator.GetPlatformSpecificService().getFolder(), "tmpDownloadDB.db3"), _dbPassword))
            {
                this._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "New DB file is OK", memberName: nameof(resetDatabase), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataService.cs", sourceLineNumber: 345);
                bool oldDBExists = this.isValid;
                bool mergeSuccessful = false;
                try
                {
                    this._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "Preparing new DB schema", memberName: nameof(resetDatabase), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataService.cs", sourceLineNumber: 350);
                    string str1 = this._tempDbPassword;
                    string str2 = str1;
                    if (str2 == null)
                        str2 = await this._secureStorageService.GetTempDBPasswordFromSecureStorage();
                    string tmpDBpassword = str2;
                    str1 = (string)null;
                    str2 = (string)null;
                    this._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "Prepare Columns", memberName: nameof(resetDatabase), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataService.cs", sourceLineNumber: 353);
                    MetadataDB.PrepareColumns(Path.Combine(this._locator.GetPlatformSpecificService().getFolder(), "tmpDownloadDB.db3"), tmpDBpassword);
                    this._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "Prepare PpfTable", memberName: nameof(resetDatabase), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataService.cs", sourceLineNumber: 356);
                    MetadataDB.PreparePpfTable(Path.Combine(this._locator.GetPlatformSpecificService().getFolder(), "tmpDownloadDB.db3"), tmpDBpassword);
                    if (oldDBExists)
                    {
                        this._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "Getting already downloaded files from existing DB", memberName: nameof(resetDatabase), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataService.cs", sourceLineNumber: 361);
                        this.metadataDB.initializeColumns(Path.Combine(this._locator.GetPlatformSpecificService().getFolder(), "tmpDownloadDB.db3"), tmpDBpassword);
                        this._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "Completed initialization of temp database", memberName: nameof(resetDatabase), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataService.cs", sourceLineNumber: 363);
                        this._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "Store Refetched PPF Enumbers", memberName: nameof(resetDatabase), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataService.cs", sourceLineNumber: 365);
                        List<string> refetchedEnumbers = this.GetRefetchedPPFEnumbers();
                        this.StoreRefetchedPPFENumbersFromOldDB(refetchedEnumbers);
                        this._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "End Storage of Refetched PPF Enumbers", memberName: nameof(resetDatabase), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataService.cs", sourceLineNumber: 368);
                        refetchedEnumbers = (List<string>)null;
                    }
                    this.closeDB();
                    this._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "Removing existing DB", memberName: nameof(resetDatabase), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataService.cs", sourceLineNumber: 374);
                    FileInfo fi = new FileInfo(Path.Combine(this._locator.GetPlatformSpecificService().getFolder(), "Metadata.db3"));
                    if (!this.IsFileLocked(fi))
                        File.Delete(Path.Combine(this._locator.GetPlatformSpecificService().getFolder(), "Metadata.db3"));
                    try
                    {
                        this._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "Removing existing DB - wal", memberName: nameof(resetDatabase), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataService.cs", sourceLineNumber: 381);
                        File.Delete(Path.Combine(this._locator.GetPlatformSpecificService().getFolder(), "Metadata.db3-wal"));
                    }
                    catch (Exception ex)
                    {
                        this._loggingService.getLogger().LogAppError(LoggingContext.METADATA, "Failed to erase wal", ex, nameof(resetDatabase), "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataService.cs", 387);
                    }
                    try
                    {
                        this._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "Removing existing DB - shm", memberName: nameof(resetDatabase), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataService.cs", sourceLineNumber: 391);
                        File.Delete(Path.Combine(this._locator.GetPlatformSpecificService().getFolder(), "Metadata.db3-shm"));
                    }
                    catch (Exception ex)
                    {
                        this._loggingService.getLogger().LogAppError(LoggingContext.METADATA, "Failed to erase shm", ex, nameof(resetDatabase), "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataService.cs", 397);
                    }
                    this._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "Setting the FileProtection free to the DB", memberName: nameof(resetDatabase), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataService.cs", sourceLineNumber: 402);
                    this._locator.GetPlatformSpecificService().SetFileProtectionNone(Path.Combine(this._locator.GetPlatformSpecificService().getFolder()));
                    this._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "Renaming new DB", memberName: nameof(resetDatabase), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataService.cs", sourceLineNumber: 405);
                    File.Copy(Path.Combine(this._locator.GetPlatformSpecificService().getFolder(), "tmpDownloadDB.db3"), Path.Combine(this._locator.GetPlatformSpecificService().getFolder(), "Metadata.db3"));
                    this._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "Saving new password in Secure Storage", memberName: nameof(resetDatabase), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataService.cs", sourceLineNumber: 410);
                    int num = await this._secureStorageService.SetDBPasswordInSecureStorage(tmpDBpassword) ? 1 : 0;
                    this.metadataDB = new MetadataDB(Path.Combine(this._locator.GetPlatformSpecificService().getFolder(), "Metadata.db3"), _dbPassword);
                    this._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "Activating new DB", memberName: nameof(resetDatabase), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataService.cs", sourceLineNumber: 414);
                    mergeSuccessful = true;
                    tmpDBpassword = (string)null;
                    fi = (FileInfo)null;
                }
                catch (Exception ex)
                {
                    this._loggingService.getLogger().LogAppError(LoggingContext.METADATA, "Failed to merge DB", ex, nameof(resetDatabase), "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataService.cs", 419);
                }
                if (oldDBExists & mergeSuccessful)
                {
                    (List<module> modules, List<document> documents) = this.metadataDB.getObsoleteBinaries();
                    this._loggingService.getLogger().LogAppInformation(LoggingContext.BINARY, "Count of obsolete Files to be deleted:" + (modules.Count + documents.Count).ToString(), memberName: nameof(resetDatabase), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataService.cs", sourceLineNumber: 425);
                    this._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "Begin deletion of obsolete modules", memberName: nameof(resetDatabase), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataService.cs", sourceLineNumber: 427);
                    this.deleteObsoleteModules(modules);
                    this._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "End deletion of obsolete modules", memberName: nameof(resetDatabase), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataService.cs", sourceLineNumber: 429);
                    this._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "Begin deletion of obsolete documents", memberName: nameof(resetDatabase), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataService.cs", sourceLineNumber: 431);
                    this.deleteObsoleteDocuments(documents);
                    this._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "End deletion of obsolete documents", memberName: nameof(resetDatabase), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataService.cs", sourceLineNumber: 433);
                    modules = (List<module>)null;
                    documents = (List<document>)null;
                }
                this.isValid = true;
                _dbPassword = (string)null;
                md5 = (string)null;
                headerChecksum = (string)null;
            }
            else
            {
                this._loggingService.getLogger().LogAppError(LoggingContext.METADATA, "Downloaded File is corrupted", memberName: nameof(resetDatabase), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataService.cs", sourceLineNumber: 440);
                return false;
            }
        }
        catch (Exception ex)
        {
            this._loggingService.getLogger().LogAppError(LoggingContext.METADATA, "Failed to init", ex, nameof(resetDatabase), "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataService.cs", 446);
            this.isValid = false;
            this.metadataDB = (MetadataDB)null;
            return false;
        }
        return true;
    }

    public void deleteObsoleteModules(List<module> modules)
    {
        string folder = this._locator.GetPlatformSpecificService().getFolder();
        this._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "Delete obsolete modules", memberName: nameof(deleteObsoleteModules), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataService.cs", sourceLineNumber: 458);
        List<Tuple<long, string>> listOfObsoleteModules = new List<Tuple<long, string>>();
        foreach (module module in modules)
        {
            this._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "Binary to be deleted " + module?.ToString(), memberName: nameof(deleteObsoleteModules), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataService.cs", sourceLineNumber: 464);
            try
            {
                string path1 = Path.Combine(folder, module.toFileName());
                if (File.Exists(path1))
                {
                    File.Delete(path1);
                    this._loggingService.getLogger().LogAppInformation(LoggingContext.BINARY, "Erased obsolete file " + path1, memberName: nameof(deleteObsoleteModules), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataService.cs", sourceLineNumber: 471);
                }
                string path2 = Path.Combine(folder, module.toFileNameNewFdsExtension(".cpio"));
                string path3 = Path.Combine(folder, module.toFileNameNewFdsExtension(".zip"));
                string path4 = Path.Combine(folder, module.toFileNameNewFdsExtension(".tar.gz"));
                if (File.Exists(path2))
                {
                    File.Delete(path2);
                    this._loggingService.getLogger().LogAppInformation(LoggingContext.BINARY, "Erased obsolete file " + path2, memberName: nameof(deleteObsoleteModules), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataService.cs", sourceLineNumber: 481);
                }
                if (File.Exists(path3))
                {
                    File.Delete(path3);
                    this._loggingService.getLogger().LogAppInformation(LoggingContext.BINARY, "Erased obsolete file " + path3, memberName: nameof(deleteObsoleteModules), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataService.cs", sourceLineNumber: 486);
                }
                if (File.Exists(path4))
                {
                    File.Delete(path4);
                    this._loggingService.getLogger().LogAppInformation(LoggingContext.BINARY, "Erased obsolete file " + path4, memberName: nameof(deleteObsoleteModules), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataService.cs", sourceLineNumber: 491);
                }
                this._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, $"Begin Deletion of {module?.ToString()} from module table", memberName: nameof(deleteObsoleteModules), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataService.cs", sourceLineNumber: 493);
                this.metadataDB.removeDownloadedObsoleteModule(module);
                this._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, $"End Deletion of {module?.ToString()} from module table", memberName: nameof(deleteObsoleteModules), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataService.cs", sourceLineNumber: 495);
                this._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "Populate ppf ids to be deleted", memberName: nameof(deleteObsoleteModules), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataService.cs", sourceLineNumber: 497);
                listOfObsoleteModules.Add(new Tuple<long, string>(module.moduleid, module.version));
            }
            catch (Exception ex)
            {
                this._loggingService.getLogger().LogAppError(LoggingContext.BINARY, "Failed to erase obsolete file " + module.toFileName(), ex, nameof(deleteObsoleteModules), "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataService.cs", 502);
            }
        }
        this.deleteObsoleteModulesFromPPFTable(listOfObsoleteModules);
        this._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "Completed deletion process", memberName: nameof(deleteObsoleteModules), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataService.cs", sourceLineNumber: 508);
        this._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, " Populate extensions of remaining modules", memberName: nameof(deleteObsoleteModules), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataService.cs", sourceLineNumber: 510);
        this.LogRemainingModulesExtensions(folder);
        this._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "Completed Population of remaining modules", memberName: nameof(deleteObsoleteModules), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataService.cs", sourceLineNumber: 512 /*0x0200*/);
    }

    public void deleteObsoleteModulesFromPPFTable(List<Tuple<long, string>> listOfObsoleteModules)
    {
        this._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "Delete obsolete ppfs", memberName: nameof(deleteObsoleteModulesFromPPFTable), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataService.cs", sourceLineNumber: 518);
        if (listOfObsoleteModules.Count > 0)
            this.metadataDB.deleteObsoleteModulesFromPPFTable(listOfObsoleteModules);
        this._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "Completed deletion of obsolete ppfs", memberName: nameof(deleteObsoleteModulesFromPPFTable), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataService.cs", sourceLineNumber: 523);
    }

    public void LogRemainingModulesExtensions(string mainPath)
    {
        Regex reg = new Regex("[0-9]+\\.[0-9]+\\.[0-9]+\\.[0-9]+\\.");
        List<string> list = ((IEnumerable<string>)Directory.GetFiles(mainPath, "*")).Where<string>((Func<string, bool>)(x => reg.IsMatch(x))).ToList<string>();
        int num1 = 0;
        int num2 = 0;
        int num3 = 0;
        int num4 = 0;
        int num5 = 0;
        foreach (string str in list)
        {
            if (str.Contains(".bin"))
                ++num1;
            else if (str.Contains(".cpio"))
                ++num2;
            else if (str.Contains(".tar.gz"))
                ++num3;
            else if (str.Contains(".zip"))
                ++num4;
            else
                ++num5;
        }
        this._loggingService.getLogger().LogAppInformation(LoggingContext.BINARY, $"Remaining module files are {num1} .bin, {num2} .cpio, {num3} .tar.gz, {num4} .zip and {num5} other.", memberName: nameof(LogRemainingModulesExtensions), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataService.cs", sourceLineNumber: 549);
    }

    public void deleteObsoleteDocuments(List<document> documents)
    {
        string folder = this._locator.GetPlatformSpecificService().getFolder();
        foreach (document document in documents)
        {
            this._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "Binary to be deleted " + document?.ToString(), memberName: nameof(deleteObsoleteDocuments), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataService.cs", sourceLineNumber: 556);
            try
            {
                string path = Path.Combine(folder, document.toFileName());
                if (File.Exists(path))
                {
                    File.Delete(path);
                    this._loggingService.getLogger().LogAppInformation(LoggingContext.BINARY, "Erased DB obsolete file: " + path, memberName: nameof(deleteObsoleteDocuments), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataService.cs", sourceLineNumber: 564);
                }
                this.metadataDB.removeDownloadedObsoleteDocument(document);
            }
            catch (Exception ex)
            {
                this._loggingService.getLogger().LogAppError(LoggingContext.BINARY, "Failed to erase obsolete file " + document.toFileName(), ex, nameof(deleteObsoleteDocuments), "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataService.cs", 570);
            }
        }
    }

    public int getDeltaSize() => this.isValid ? this.metadataDB.getDelta() : 0;

    public int getLocalSize() => this.isValid ? this.metadataDB.getNumberOfLocalFiles() : 0;

    public List<module_with_enumber> getModulesDeltaSet()
    {
        return this.isValid ? this.metadataDB.getModulesDeltaSet() : new List<module_with_enumber>();
    }

    public List<document> getMissingDocuments()
    {
        return this.isValid ? this.metadataDB.getDocumentsDeltaSet() : new List<document>();
    }

    public List<document> getZ9KDfiles(bool bIsDownloaded = false)
    {
        return this.isValid ? this.metadataDB.getZ9KDDeltaSet(bIsDownloaded) : new List<document>();
    }

    public List<module_with_enumber> getModulesDeltaSetForDownloadSetting(
      Dictionary<DeviceClass, List<MaterialStatistics>> materialList,
      bool bIsDownloaded = false)
    {
        return this.isValid ? this.metadataDB.getModulesDeltaSetForDownloadSetting(materialList, bIsDownloaded) : new List<module_with_enumber>();
    }

    public List<module_with_enumber> getModulesForENumber(string eNumber)
    {
        return this.isValid ? this.metadataDB.getModulesForENumber(eNumber) : new List<module_with_enumber>();
    }

    public List<document> getDocumentsForENumber(string eNumber, DeviceClass deviceClass)
    {
        return this.isValid ? this.metadataDB.getDocumentsForENumber(eNumber, deviceClass) : new List<document>();
    }

    public List<document> getDocumentsDeltaSetForSingleDownloadAndPrepareYourWork(
      Dictionary<DeviceClass, List<MaterialStatistics>> materialList,
      bool bIsDownloaded = false)
    {
        return this.metadataDB != null ? this.metadataDB.getDocumentsDeltaSetForSingleDownloadAndPrepareYourWork(materialList, bIsDownloaded) : new List<document>();
    }

    public bool markAsDownloaded(DownloadProxy proxy)
    {
        if (!this.isValid)
            return false;
        this._fileSize = 0L;
        string nameNewFdsExtension = proxy.ToFileNameNewFdsExtension(proxy.fileExtension);
        try
        {
            string fileId = proxy.GetFileId();
            string md5 = proxy.GetMD5();
            string file = this.MD5ComputeFile(Path.Combine(this._locator.GetPlatformSpecificService().getFolder(), nameNewFdsExtension));
            string str = this.MD5Bytes2String(Convert.FromBase64String(md5));
            if (file != str)
            {
                this._loggingService.getLogger().LogAppInformation(LoggingContext.BINARY, $"{nameNewFdsExtension} MD5 : {file} does not match metadata {str}", memberName: nameof(markAsDownloaded), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataService.cs", sourceLineNumber: 657);
                try
                {
                    File.Delete(Path.Combine(this._locator.GetPlatformSpecificService().getFolder(), nameNewFdsExtension));
                    this._loggingService.getLogger().LogAppInformation(LoggingContext.BINARY, "Erased corrupted file " + nameNewFdsExtension, memberName: nameof(markAsDownloaded), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataService.cs", sourceLineNumber: 661);
                }
                catch (Exception ex)
                {
                    this._loggingService.getLogger().LogAppError(LoggingContext.BINARY, "Failed to erase corrupted file " + nameNewFdsExtension, ex, nameof(markAsDownloaded), "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataService.cs", 665);
                }
                return false;
            }
            this._fileSize = proxy.GetFileSize();
            this.usedStorage += this._fileSize;
            this.metadataDB.updateDownloadedStatus(fileId, proxy.Document != null, true);
            return true;
        }
        catch (Exception ex)
        {
            this._loggingService.getLogger().LogAppError(LoggingContext.BINARY, "Failed to erase corrupted file " + nameNewFdsExtension, ex, nameof(markAsDownloaded), "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataService.cs", 677);
            return false;
        }
    }

    public void UpdateDownloadedStatus(string fileId, bool isDocument, bool setAsDownloaded)
    {
        if (this.metadataDB == null)
            return;
        this.metadataDB.updateDownloadedStatus(fileId, isDocument, setAsDownloaded);
    }

    public List<enumber_modules> getMissingModules(string enumber)
    {
        List<enumber_modules> missingModules = new List<enumber_modules>();
        if (this.isValid)
        {
            foreach (enumber_modules enumberModule in this.metadataDB.getEnumberModules(enumber))
            {
                if (!this.metadataDB.CheckIfModuleFileIsDownloaded(enumberModule.fileId))
                {
                    this._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "Missing Module " + enumberModule.toFileName(), memberName: nameof(getMissingModules), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataService.cs", sourceLineNumber: 702);
                    missingModules.Add(enumberModule);
                }
            }
        }
        return missingModules;
    }

    public List<enumber_modules> getFilesForEnumber(string enumber)
    {
        List<enumber_modules> filesForEnumber = new List<enumber_modules>();
        if (this.isValid)
        {
            if (this.isSMM(enumber))
                filesForEnumber.AddRange((IEnumerable<enumber_modules>)this.metadataDB.getEnumberModules(enumber));
            foreach (document enumberDocument in this.metadataDB.getEnumberDocuments(enumber))
            {
                if (!enumberDocument.IsDownloaded)
                    filesForEnumber.Add(new enumber_modules(enumberDocument._document, enumberDocument.version, 14147, enumberDocument.fileId, enumberDocument.fileId, enumberDocument.md5, enumberDocument, enumberDocument.fileSize, enumberDocument.type, enumberDocument.harnessOK));
            }
        }
        return filesForEnumber;
    }

    public List<enumber_modules> getModules(string enumber)
    {
        List<enumber_modules> enumberModulesList = new List<enumber_modules>();
        return this.isValid ? this.metadataDB.getEnumberModules(enumber) : enumberModulesList;
    }

    public string getFirmwareName(string moduleid, iService5.Ssh.DTO.Version version, string type)
    {
        return this.metadataDB.getdModuleIdName(moduleid, version, type);
    }

    public bool isSMMWithWifi(string enumber)
    {
        return this.isValid && this.metadataDB.isSMMWithWifi(enumber);
    }

    public List<MaterialStatistics> getMaterialStatisticsOfENumbers(string eNumbers)
    {
        return this.isValid ? this.metadataDB.getMaterialStatisticsOfENumbers(eNumbers) : new List<MaterialStatistics>();
    }

    public long GetTotalFileSizeForENumber(string eNumbers)
    {
        return this.isValid ? this.metadataDB.GetTotalFileSizeForENumber(eNumbers) : 0L;
    }

    public HaInfoDto getHAInfo(string enumber)
    {
        HaInfoDto haInfo = (HaInfoDto)null;
        if (this.isValid)
        {
            smm smm = this.metadataDB.getSMM(enumber);
            if (smm != null)
            {
                haInfo = new HaInfoDto();
                haInfo.CustomerIndex = smm.ki;
                haInfo.Vib = smm.vib;
                haInfo.DeviceType = smm.deviceType;
                List<material> materialMatches = this.metadataDB.getMaterialMatches(enumber, MatchPattern.ALL);
                if (materialMatches.Count != 0)
                {
                    haInfo.Brand = !this.iDecodeBrands.ContainsKey(materialMatches[0].brand) ? materialMatches[0].brand : this.iDecodeBrands[materialMatches[0].brand];
                    haInfo.CountrySettings = this.GetCountrySettings(enumber, materialMatches[0]);
                }
                haInfo.ManufacturingTimeStamp = DateTime.Now.ToString("yyyy-MM-dd") + "T00:00:00";
            }
        }
        return haInfo;
    }

    public List<smm_module> getSMMModulesRecords(string _vib, string _ki, long _fwid)
    {
        return this.metadataDB.getSMMModulesRecords(_vib, _ki, _fwid);
    }

    public string getNodeName(long _node, string _devicetype)
    {
        return this.metadataDB.getNodeName(_node, _devicetype);
    }

    public int GetCountrySettings(string enumber, material material)
    {
        string materialType = this.GetMaterialType(enumber);
        int countrySettings = 0;
        switch (materialType)
        {
            case "SMM_WITH_WIFI":
                string wifiCountrySetting = material.wifiCountrySetting;
                if (!string.IsNullOrEmpty(wifiCountrySetting))
                {
                    int result;
                    if (int.TryParse(wifiCountrySetting, out result))
                    {
                        countrySettings = result;
                        break;
                    }
                    this._loggingService.getLogger().LogAppInformation(LoggingContext.METADATA, "The country settings value couldn't be parsed", memberName: nameof(GetCountrySettings), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataService.cs", sourceLineNumber: 826);
                    countrySettings = this.GetCountrySettingsBasedOnCountryCode();
                    break;
                }
                countrySettings = this.GetCountrySettingsBasedOnCountryCode();
                break;
            case "SMM_WITHOUT_WIFI":
                countrySettings = (int)byte.MaxValue;
                break;
        }
        return countrySettings;
    }

    public int GetCountrySettingsBasedOnCountryCode()
    {
        return !(UtilityFunctions.GetUserCountryCode(this._secureStorageService).ToUpperInvariant() == "US") ? 2 : 1;
    }

    public bool isDBValid() => this.isValid;

    public void closeDB()
    {
        if (!this.isValid)
            return;
        this.metadataDB.Database.Close();
    }

    public string getConnectionGraphic(string enumber)
    {
        if (this.isValid)
        {
            document connectionGraphic = this.metadataDB.getConnectionGraphic(enumber);
            if (connectionGraphic != null && this.metadataDB.CheckIfDocumentIsDownloaded(connectionGraphic.fileId))
                return connectionGraphic.toFileName();
        }
        return (string)null;
    }

    public string getConnectionGraphicNonSmm(string enumber)
    {
        if (this.isValid)
        {
            document connectionGraphicNonSmm = this.metadataDB.getConnectionGraphicNonSmm(enumber);
            if (connectionGraphicNonSmm != null && this.metadataDB.CheckIfDocumentIsDownloaded(connectionGraphicNonSmm.fileId))
                return connectionGraphicNonSmm.toFileName();
        }
        return (string)null;
    }

    public document GetMonitoringGraphicsDocument(string enumber)
    {
        if (this.isValid)
        {
            document graphicsDocument = this.metadataDB.GetMonitoringGraphicsDocument(enumber);
            if (graphicsDocument != null && this.metadataDB.CheckIfDocumentIsDownloaded(graphicsDocument.fileId))
                return graphicsDocument;
        }
        return (document)null;
    }

    public string getSSHKeyValue()
    {
        string sshKeyValue = "";
        if (this.metadataDB != null)
            sshKeyValue = this.metadataDB.getSSHKeyValue();
        return sshKeyValue;
    }

    public string getErrorCodeMessage(string errorCode, string eNumber)
    {
        string errorCodeMessage = "";
        if (this.metadataDB != null)
            errorCodeMessage = this.metadataDB.getErrorCodeMessage(errorCode, eNumber);
        return errorCodeMessage;
    }

    public string getErrorCodeDescription(string errorCode, string eNumber)
    {
        string errorCodeDescription = "";
        if (this.metadataDB != null)
            errorCodeDescription = this.metadataDB.getErrorCodeDescription(errorCode, eNumber);
        return errorCodeDescription;
    }

    public string getMemoryCodeDescription(string errorCode)
    {
        string memoryCodeDescription = "";
        if (this.metadataDB != null)
            memoryCodeDescription = this.metadataDB.getMemoryCodeDescription(errorCode);
        return memoryCodeDescription;
    }

    public string getShortText(string shortTextCode, string lang)
    {
        string shortText = "";
        if (this.metadataDB != null)
            shortText = this.metadataDB.getShortText(shortTextCode, lang);
        return shortText;
    }

    public string[] getShortTextPlaceholder(string[] shortTextList)
    {
        string lowerInvariant = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToLowerInvariant();
        List<string> stringList = new List<string>();
        for (int index = 0; index < shortTextList.Length; ++index)
            stringList.Add(this.getShortText(shortTextList[index], lowerInvariant));
        return stringList.ToArray();
    }

    public string getMessageWithShortTextMapping(string message)
    {
        string input = message;
        List<string> stringList = new List<string>();
        MatchCollection matchCollection = new Regex("\\${[\\s\\S]*?\\}", RegexOptions.None, TimeSpan.FromSeconds(2.0)).Matches(input);
        if (matchCollection.Count > 0)
        {
            for (int i = 0; i < matchCollection.Count; ++i)
                stringList.Add(matchCollection[i].Value.Substring(2, matchCollection[i].Value.Length - 3));
            string[] shortTextPlaceholder = this.getShortTextPlaceholder(stringList.ToArray());
            for (int i = 0; i < matchCollection.Count; ++i)
                input = input.Replace(matchCollection[i].Value, shortTextPlaceholder[i]);
        }
        return input;
    }

    public List<AugmentedModule> GetSMMModuleWithLabels(string enumber)
    {
        List<AugmentedModule> moduleWithLabels = new List<AugmentedModule>();
        try
        {
            List<AugmentedModule> augmentedModuleList = new List<AugmentedModule>();
            foreach (SmmQueryHelperFinal smmModuleWithLabel in this.metadataDB.GetSMMModuleWithLabels(enumber))
            {
                SmmQueryHelperFinal queryModule = smmModuleWithLabel;
                if (!augmentedModuleList.Any<AugmentedModule>((Func<AugmentedModule, bool>)(x => x.moduleid == queryModule.Moduleid && x.type == queryModule.Type)))
                    augmentedModuleList.Add(new AugmentedModule(new module(queryModule.Moduleid, queryModule.Version, queryModule.Node, queryModule.Message != "" ? queryModule.Message : queryModule.Name, queryModule.FileId, queryModule.MD5, queryModule.FileSize, queryModule.Type), queryModule.Package != null && queryModule.Package.ToLower() == "recovery", queryModule.Package != null ? queryModule.Package.ToLower() : ""));
            }
            moduleWithLabels.AddRange((IEnumerable<AugmentedModule>)this.sortAugmentedModuleList(augmentedModuleList));
        }
        catch (Exception ex)
        {
            this._loggingService.getLogger().LogAppError(LoggingContext.METADATA, ex.Message, ex, nameof(GetSMMModuleWithLabels), "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataService.cs", 1030);
        }
        return moduleWithLabels;
    }

    private List<AugmentedModule> sortAugmentedModuleList(List<AugmentedModule> moduleList)
    {
        List<AugmentedModule> augmentedModuleList = new List<AugmentedModule>();
        List<AugmentedModule> collection1 = new List<AugmentedModule>();
        List<AugmentedModule> collection2 = new List<AugmentedModule>();
        foreach (AugmentedModule module in moduleList)
        {
            string type1 = module.type;
            ModuleType moduleType = ModuleType.FIRMWARE;
            string str1 = moduleType.ToString();
            if (type1 == str1)
            {
                augmentedModuleList.Add(module);
            }
            else
            {
                string type2 = module.type;
                moduleType = ModuleType.SPAU_FIRMWARE;
                string str2 = moduleType.ToString();
                if (type2 == str2)
                {
                    collection1.Add(module);
                }
                else
                {
                    string type3 = module.type;
                    moduleType = ModuleType.INITIAL_CONTENT;
                    string str3 = moduleType.ToString();
                    if (type3 == str3)
                        collection2.Add(module);
                }
            }
        }
        augmentedModuleList.AddRange((IEnumerable<AugmentedModule>)collection1);
        augmentedModuleList.AddRange((IEnumerable<AugmentedModule>)collection2);
        return augmentedModuleList;
    }

    public bool CheckRecovery(long moduleId) => this.metadataDB.IsRecovery(moduleId);

    public bool deleteAllBinaries(IPlatformSpecificServiceLocator _locator)
    {
        return this.metadataDB.DeleteAllBinaries(_locator);
    }

    public List<smm_whitelist> GetDowngradePass(long module)
    {
        return this.metadataDB != null ? this.metadataDB.getDowngradePass(module) : (List<smm_whitelist>)null;
    }

    public bool IsDocumentAvailable(string enumber, string docType)
    {
        return this.metadataDB != null && this.metadataDB.IsDocumentAvailable(enumber, docType);
    }

    public List<Version> GetVersionFromFeatureTable(string name)
    {
        return this.metadataDB != null ? this.metadataDB.GetVersionFromFeatureTable(name) : new List<Version>();
    }

    public bool CheckFeature(string fname)
    {
        return this.metadataDB == null || this.metadataDB.CheckFeature(fname);
    }

    public string GetMaterialType(string enumber)
    {
        return this.metadataDB != null ? this.metadataDB.GetmaterialType(enumber) : (string)null;
    }

    public bool IsMaterialAvailable(string enumber)
    {
        return this.metadataDB != null && this.metadataDB.IsMaterialAvailable(enumber);
    }

    public DownloadFilesStatistics GetDownloadFilesStatistics(
      Dictionary<DeviceClass, List<MaterialStatistics>> deviceClassBasedMaterials)
    {
        return this.isValid ? this.metadataDB.GetDownloadFilesStatistics(deviceClassBasedMaterials) : new DownloadFilesStatistics();
    }

    public DownloadFilesStatistics GetDownloadFilesStatisticsForFullDownloadAndCountryRelevant(
      string deviceClass,
      Dictionary<DeviceClass, List<MaterialStatistics>> deviceClassBasedMaterials)
    {
        return this.isValid ? this.metadataDB.GetDownloadFilesStatisticsForFullDownloadAndCountryRelevant(deviceClass, deviceClassBasedMaterials) : new DownloadFilesStatistics();
    }

    public List<MaterialStatistics> GetDownloadFilesStatisticsModules()
    {
        return this.metadataDB != null ? this.metadataDB.GetDownloadFilesStatisticsModules() : new List<MaterialStatistics>();
    }

    public List<MaterialStatistics> GetDownloadFilesStatisticsDocuments()
    {
        return this.metadataDB != null ? this.metadataDB.GetDownloadFilesStatisticsDocuments() : new List<MaterialStatistics>();
    }


    /// <summary>
    ///  ZMIANA ZMIANA ZMIANA ZMIANA 
    /// </summary>
    public void UpdateMaterialStatistics()
    {
        if (this.metadataDB == null)
            return;
     this.metadataDB.UpdateMaterialStatistics();
    }


//      public void UpdateMaterialStatistics()
//      {
//          // --- TYMCZASOWE OBEJŚCIE ---
//         // Logujemy informację, że krok został pominięty i nic nie robimy,
//          // aby uniknąć błędu NotImplementedException.
//  
//          var logger = Mvx.IoCProvider.Resolve<ILoggingService>().getLogger();
//          logger.LogAppWarning(LoggingContext.METADATA, "Pominięto krok aktualizacji statystyk materiałów (UpdateMaterialStatistics), ponieważ GetMaterialList nie jest zaimplementowane.");
//  
//          // Nie wykonujemy żadnych operacji na bazie danych.
//          return;
//     }


    public void UpdatePpfTable(
      string _vib,
      string _ki,
      long _moduleid,
      string _version,
      long _expdate,
      long _type,
      string _ca,
      string _ppffile)
    {
        if (this.metadataDB == null)
            return;
        this.metadataDB.StorePpfEntity(_vib, _ki, _moduleid, _version, _expdate, _type, _ca, _ppffile);
    }

    public List<MaterialStatistics> GetMaterialStatistics()
    {
        return this.isValid ? this.metadataDB.GetMaterialStatistics() : new List<MaterialStatistics>();
    }

    public bool CheckIfTableExists(string tableName)
    {
        return this.isValid && this.metadataDB.CheckIfTableExists(tableName);
    }

    public bool IsModuleInRecoveryReboot(AugmentedModule module)
    {
        return this.metadataDB != null && this.metadataDB.IsModuleInRecoveryReboot(module);
    }

    public List<Country> GetCountryList()
    {
        return this.metadataDB != null ? this.metadataDB.GetCountryList() : new List<Country>();
    }

    public bool isSMM(string enumber) => this.isValid && this.metadataDB.isSMM(enumber);

    public bool isSMMFlintStone(string enumber)
    {
        return this.isValid && this.metadataDB.isSMMFlintStone(enumber);
    }

    public bool isNONSMM(string enumber) => this.isValid && this.metadataDB.isNONSMM(enumber);

    public List<DownloadProxy> GetExpiredAndAboutToExpireENumbers()
    {
        return this.metadataDB != null ? this.metadataDB.GetExpiredAndAboutToExpireENumbers() : new List<DownloadProxy>();
    }

    public void DeletePpfEntries(ppf ppf)
    {
        if (this.metadataDB == null)
            return;
        this.metadataDB.DeletePpfEntries(ppf);
    }

    public List<ppf> GetRelatedPpfs(
      string vib,
      string ki,
      long moduleid,
      string version,
      bool ppf6supported)
    {
        return this.metadataDB != null ? this.metadataDB.GetRelatedPpfs(vib, ki, moduleid, version, ppf6supported) : new List<ppf>();
    }

    public List<ppf> GetAllEnumbersDownloadedPpfs(string enumber, bool onlyType6Ppfs = false)
    {
        return this.metadataDB != null ? this.metadataDB.GetAllEnumbersDownloadedPpfs(enumber, onlyType6Ppfs) : new List<ppf>();
    }

    public string GetDeviceTypeForSMM(string enumber) => this.metadataDB.getSMM(enumber)?.deviceType;

    public bool checkIfAppHasOldDB()
    {
        return this.metadataDB != null && this.metadataDB.DBModulesHaveNoArchitecture();
    }

    public bool IsENumberSyMaNa(string eNumber)
    {
        return this.isValid && this.metadataDB.checkIfEnoIsSyMaNa(eNumber);
    }

    public List<smm_module> GetModulesDetails(string enumber)
    {
        return this.metadataDB != null ? this.metadataDB.GetModulesDetails(enumber) : new List<smm_module>();
    }

    public List<module> GetModuleSpecs(smm_module smmModule)
    {
        return this.metadataDB != null ? this.metadataDB.GetModuleSpecs(smmModule) : new List<module>();
    }

    public void clearCacheUsedStorage() => this.usedStorage = 0L;

    public void updatePPFRefetchStatus(string material, bool setAsDownloaded)
    {
        if (this.metadataDB == null)
            return;
        this.metadataDB.updatePPFRefetchStatus(material, setAsDownloaded);
    }

    public List<string> GetRefetchedPPFEnumbers()
    {
        return this.metadataDB != null ? this.metadataDB.GetRefetchedPPFEnumbers() : new List<string>();
    }

    public void StoreRefetchedPPFENumbersFromOldDB(List<string> refetchedPPFsEnumbers)
    {
        this.refetchedPPFsEnumbersFromOldDB = refetchedPPFsEnumbers;
    }

    public List<string> GetRefetchedPPFENumbersFromOldDB() => this.refetchedPPFsEnumbersFromOldDB;

    public void DeleteExistingPPF(DownloadProxy proxy)
    {
        if (this.metadataDB == null)
            return;
        this.metadataDB.DeleteExistingPPF(proxy);
    }

    public List<ppf> GetPpfs()
    {
        return this.metadataDB != null ? this.metadataDB.GetPpfs() : new List<ppf>();
    }

    public List<string> GetDownloadedEnumbersFromPPF()
    {
        List<string> downloadedEnumbersFromPpf = new List<string>();
        if (this.metadataDB != null)
            downloadedEnumbersFromPpf = this.metadataDB.GetDownloadedEnumbersFromPPF();
        return downloadedEnumbersFromPpf;
    }

    public bool IsPPFTableMigrationNeded()
    {
        bool flag = false;
        if (this.metadataDB != null)
            flag = this.metadataDB.IsPPFTableMigrationNeded();
        return flag;
    }

    public bool MigratePPFTable() => this.metadataDB == null || this.metadataDB.MigratePPFTable();
}
