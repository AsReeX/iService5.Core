using iService5.Core.Services.Helpers;
using iService5.Core.Services.Platform;
using iService5.Core.Services.User.ActivityLogging;
using iService5.Core.ViewModels;
using MvvmCross;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;

#nullable disable
namespace iService5.Core.Services.Data;

public class MetadataDB
{
    private SQLiteConnection database;
    private readonly string _path;
    private readonly string _dbPassword;
    public static readonly ILoggingService loggingService = Mvx.IoCProvider.Resolve<ILoggingService>();
    private readonly object lockObj = new object();
    private string excludeDocumentTypeForNonSMM = Constants.excludeDocumentTypeForNonSMM;
    private string includeSmmDocumentType = Constants.includeSmmDocumentType;
    private const string virtualEnum = "Z9KDKD0Z01";

    public SQLiteConnection Database => this.database;

    public int ppftableCount { get; private set; }

    public int deprtableCount { get; private set; }

    public MetadataDB(string dbPath, string dbPassword)
    {
        this._path = dbPath;
        this._dbPassword = dbPassword;
        try
        {
            this.database = new SQLiteConnection(new SQLiteConnectionString(dbPath, true, (object)this._dbPassword, postKeyAction: (Action<SQLiteConnection>)(c => c.Execute("PRAGMA cipher_compatibility = 3"))));
            if (this.IsDeprecatedDB() && this.deprtableCount.Equals(1) && !this.ppftableCount.Equals(0))
                this.UpdateDBSchema();
            else if (this.IsDeprecatedDB() && this.deprtableCount.Equals(0) && this.ppftableCount.Equals(0))
            {
                this.UpdateDBSchemaPpf();
            }
            else
            {
                if (!this.IsDeprecatedDB() || !this.deprtableCount.Equals(1) || !this.ppftableCount.Equals(0))
                    return;
                this.UpdateDBSchema();
                this.UpdateDBSchemaPpf();
            }
        }
        catch (SQLiteException ex) when (ex.Result == SQLite3.Result.NonDBFile)
        {
            this.database = new SQLiteConnection(new SQLiteConnectionString(dbPath));
            MetadataDB.loggingService.getLogger().LogAppError(LoggingContext.METADATA, "Failed to create metadata DB connection", (Exception)ex, ".ctor", "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataDB.cs", 69);
            if (this.IsDeprecatedDB() && this.deprtableCount.Equals(1) && !this.ppftableCount.Equals(0))
                this.UpdateDBSchema();
            else if (this.IsDeprecatedDB() && this.deprtableCount.Equals(0) && this.ppftableCount.Equals(0))
            {
                this.UpdateDBSchemaPpf();
            }
            else
            {
                if (!this.IsDeprecatedDB() || !this.deprtableCount.Equals(1) || !this.ppftableCount.Equals(0))
                    return;
                this.UpdateDBSchema();
                this.UpdateDBSchemaPpf();
            }
        }
    }

    internal bool IsDeprecatedDB()
    {
        SQLiteCommand sqLiteCommand = new SQLiteCommand(this.database)
        {
            CommandText = "SELECT COUNT(1) FROM sqlite_master WHERE type = 'table' AND name = 'module_downloaded'"
        };
        this.deprtableCount = sqLiteCommand.ExecuteScalar<int>();
        sqLiteCommand.CommandText = "SELECT COUNT(1) FROM sqlite_master WHERE type = 'table' AND name = 'ppf'";
        this.ppftableCount = sqLiteCommand.ExecuteScalar<int>();
        return !this.deprtableCount.Equals(0) || this.ppftableCount.Equals(0);
    }

    internal void UpdateDBSchema()
    {
        try
        {
            this.database.BeginTransaction();
            MetadataDB.PrepareColumns(this.database);
            this.database.Execute("UPDATE main.module SET harnessOK = 'TRUE' WHERE EXISTS (SELECT main.module_downloaded.fileId FROM main.module_downloaded WHERE main.module_downloaded.fileId = main.module.fileId )");
            this.database.Execute("UPDATE main.document SET harnessOK = 'TRUE' WHERE EXISTS (SELECT main.module_downloaded.fileId FROM main.module_downloaded WHERE main.module_downloaded.fileId = main.document.fileId )");
            this.database.Execute("DROP TABLE IF EXISTS main.module_downloaded");
            this.database.Commit();
        }
        catch (SQLiteException ex)
        {
            MetadataDB.loggingService.getLogger().LogAppError(LoggingContext.METADATA, "SQL returns exception while updating Database schema: " + ex?.ToString(), memberName: nameof(UpdateDBSchema), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataDB.cs", sourceLineNumber: 120);
            this.database.Rollback();
            throw;
        }
    }

    internal void UpdateDBSchemaPpf()
    {
        try
        {
            this.database.BeginTransaction();
            MetadataDB.PrepareTable(this.database);
            this.database.Commit();
        }
        catch (SQLiteException ex)
        {
            MetadataDB.loggingService.getLogger().LogAppError(LoggingContext.METADATA, "SQL returns exception while updating Database schema :" + ex?.ToString(), memberName: nameof(UpdateDBSchemaPpf), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataDB.cs", sourceLineNumber: 137);
            this.database.Rollback();
            this.database.Close();
            throw;
        }
    }

    internal static void PrepareColumns(string newDBPath, string key)
    {
        SQLiteConnection db = new SQLiteConnection(new SQLiteConnectionString(newDBPath, true, (object)key, postKeyAction: (Action<SQLiteConnection>)(c => c.Execute("PRAGMA cipher_compatibility = 3"))));
        try
        {
            db.BeginTransaction();
            MetadataDB.PrepareColumns(db);
            db.Commit();
            db.Close();
        }
        catch (SQLiteException ex)
        {
            MetadataDB.loggingService.getLogger().LogAppError(LoggingContext.METADATA, "SQL returns exception while preparing Database columns: " + ex?.ToString(), memberName: nameof(PrepareColumns), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataDB.cs", sourceLineNumber: 165);
            db.Rollback();
            db.Close();
            throw;
        }
    }

    internal static void PreparePpfTable(string newDBPath, string key)
    {
        SQLiteConnection db = new SQLiteConnection(new SQLiteConnectionString(newDBPath, true, (object)key, postKeyAction: (Action<SQLiteConnection>)(c => c.Execute("PRAGMA cipher_compatibility = 3"))));
        try
        {
            db.BeginTransaction();
            MetadataDB.PrepareTable(db);
            db.Commit();
            db.Close();
        }
        catch (SQLiteException ex)
        {
            MetadataDB.loggingService.getLogger().LogAppError(LoggingContext.METADATA, "SQL returns exception while preparing Database table:" + ex?.ToString(), memberName: nameof(PreparePpfTable), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataDB.cs", sourceLineNumber: 193);
            db.Rollback();
            db.Close();
            throw;
        }
    }

    public bool IsPPFTableMigrationNeded()
    {
        lock (this.lockObj)
            return new SQLiteCommand(this.database)
            {
                CommandText = "SELECT EXISTS(SELECT 1 FROM sqlite_master WHERE name = 'ppf' AND sql LIKE '%ppfid%')"
            }.ExecuteScalar<bool>();
    }

    public bool MigratePPFTable()
    {
        try
        {
            List<ppf> ppfs = this.GetPpfs();
            this.database.Execute("DROP TABLE IF EXISTS ppf");
            MetadataDB.PrepareTable(this.database);
            foreach (ppf ppf in ppfs)
            {
                string[] strArray = ppf.ppfid.Split('_');
                string _vib = strArray[0];
                string _ki = strArray[1];
                int result;
                int.TryParse(strArray[2], out result);
                string _version = strArray[3];
                this.StorePpfEntity(_vib, _ki, (long)result, _version, ppf.expirydate, ppf.type, ppf.ca, ppf.ppffile);
            }
            return true;
        }
        catch (SQLiteException ex)
        {
            MetadataDB.loggingService.getLogger().LogAppError(LoggingContext.METADATA, "SQL returns exception while Migrating PPF table :" + ex?.ToString(), memberName: nameof(MigratePPFTable), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataDB.cs", sourceLineNumber: 234);
            this.database.Close();
            return false;
        }
    }

    private static void PrepareTable(SQLiteConnection db)
    {
        if (!db.ExecuteScalar<bool>("SELECT NOT EXISTS(SELECT 1 FROM sqlite_master WHERE name = 'ppf')"))
            return;
        db.Execute("CREATE TABLE IF NOT EXISTS ppf(vib NVARCHAR(20) NOT NULL, ki NVARCHAR(20) NOT NULL, moduleid BIGINT NOT NULL, version NVARCHAR (20) NOT NULL, expirydate BIGINT NOT NULL, type BIGINT NOT NULL, ca NVARCHAR(20) NOT NULL, ppffile TEXT NOT NULL, PRIMARY KEY(vib, ki, moduleid, version, type, ca))");
    }

    private static void PrepareColumns(SQLiteConnection db)
    {
        if (db.ExecuteScalar<bool>("SELECT NOT EXISTS(SELECT 1 FROM sqlite_master WHERE name = 'document' AND sql like '%harnessOK%')"))
            db.Execute("ALTER TABLE main.document ADD harnessOK NVARCHAR (20) ");
        if (db.ExecuteScalar<bool>("SELECT NOT EXISTS(SELECT 1 FROM sqlite_master WHERE name = 'module' AND sql like '%harnessOK%')"))
            db.Execute("ALTER TABLE main.module ADD harnessOK NVARCHAR (20) ");
        db.Execute("UPDATE main.document SET harnessOK = 'FALSE'");
        db.Execute("UPDATE main.module SET harnessOK = 'FALSE' ");
        db.Execute("CREATE TABLE IF NOT EXISTS main.docsToBeDeleted as SELECT document, version, type, fileId, md5, harnessOK, fileSize FROM main.document LIMIT 1");
        db.Execute("CREATE TABLE IF NOT EXISTS main.modulesToBeDeleted as SELECT moduleid, version, node, name, fileId, md5, harnessOK, fileSize FROM main.module LIMIT 1");
        db.Execute("DELETE FROM main.docsToBeDeleted ");
        db.Execute("DELETE FROM main.modulesToBeDeleted ");
    }

    internal void initializeColumns(string newDBPath, string key)
    {
        try
        {
            this.ExecuteSQL("PRAGMA cipher_default_compatibility=3");
            this.ExecuteSQL($"ATTACH database '{newDBPath}' AS newDB key '{key}' ");
            this.database.BeginTransaction();
            this.ExecuteSQL("UPDATE newDB.document SET harnessOK = 'TRUE' WHERE EXISTS (SELECT main.document.fileId FROM main.document WHERE newDB.document.document = main.document.document AND newDB.document.version = main.document.version AND newDB.document.md5 = main.document.md5 AND main.document.harnessOK = 'TRUE' AND newDB.document.md5 != '-1')");
            this.ExecuteSQL("INSERT INTO newDB.docsToBeDeleted SELECT document, version, type, fileId, md5, harnessOK, fileSize FROM main.document WHERE main.document.harnessOK = 'TRUE' AND NOT EXISTS (SELECT newDB.document.fileId FROM newDB.document WHERE newDB.document.document = main.document.document AND newDB.document.md5 = main.document.md5 AND newDB.document.version = main.document.version AND newDB.document.md5 != '-1' )");
            this.ExecuteSQL("UPDATE newDB.module SET harnessOK = 'TRUE' WHERE EXISTS (SELECT main.module.fileId FROM main.module WHERE newDB.module.fileId = main.module.fileId AND main.module.harnessOK = 'TRUE' AND newDB.module.md5 = main.module.md5 )");
            this.ExecuteSQL("INSERT INTO newDB.modulesToBeDeleted SELECT moduleid, version, node, name, fileId, md5, harnessOK, fileSize FROM main.module WHERE main.module.harnessOK = 'TRUE' AND NOT EXISTS (SELECT newDB.module.fileId FROM newDB.module WHERE newDB.module.md5 = main.module.md5)");
            this.ExecuteSQL("DELETE FROM newDB.modulesToBeDeleted WHERE EXISTS(SELECT newDB.module.fileId FROM newDB.module WHERE newDB.module.fileId = newDB.modulesToBeDeleted.fileId AND newDB.module.harnessOK = 'TRUE')");
            this.database.Commit();
        }
        catch (SQLiteException ex)
        {
            MetadataDB.loggingService.getLogger().LogAppError(LoggingContext.METADATA, "SQL returns exception while initialising new Dabatase values: " + ex?.ToString(), memberName: nameof(initializeColumns), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataDB.cs", sourceLineNumber: 299);
            this.database.Rollback();
            throw;
        }
        Task.Delay(500);
        try
        {
            this.database.BeginTransaction();
            this.ExecuteSQL("INSERT INTO newDB.ppf SELECT * from main.ppf ");
            this.database.Commit();
        }
        catch (SQLiteException ex)
        {
            MetadataDB.loggingService.getLogger().LogAppError(LoggingContext.METADATA, "SQL returns exception while initialising new Dabatase values: " + ex?.ToString(), memberName: nameof(initializeColumns), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataDB.cs", sourceLineNumber: 315);
            this.database.Rollback();
            throw;
        }
    }

    private readonly string DbPath = @"C:\Users\Adrian\AppData\Local\User Name\com.companyname.iservice.maui.ui\Data\tmpDownloadDB.db3";
  //  private readonly string dbPassword = "0ac545e7-1f49-4afa-99e2-c70d9c5166b4";

    public static bool testDBFile(string DbPath, string dbPassword)
    {
        try
        {
            SQLiteConnection sqLiteConnection = new SQLiteConnection(new SQLiteConnectionString(DbPath, true, (object)dbPassword, postKeyAction: (Action<SQLiteConnection>)(c => c.Execute("PRAGMA cipher_compatibility = 3"))));
            sqLiteConnection.Table<material>().ToList();
            sqLiteConnection.Table<smm>().ToList();
            sqLiteConnection.Table<smm_module>().ToList();
            sqLiteConnection.Table<module>().ToList();
            sqLiteConnection.Table<configuration>().ToList();
            sqLiteConnection.Close();
        }
        catch (Exception ex)
        {
            MetadataDB.loggingService.getLogger().LogAppError(LoggingContext.METADATA, "Exception occured: " + ex.Message, memberName: nameof(testDBFile), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataDB.cs", sourceLineNumber: 339);
            return false;
        }
        return true;
    }

    internal void updateDownloadedStatus(string fileId, bool isDocument, bool setAsDownloaded)
    {
        string str = isDocument ? "document" : "module";
        lock (this.lockObj)
        {
            try
            {
                this.database.BeginTransaction();
                if (setAsDownloaded)
                    this.database.Execute($"UPDATE main.{str} SET harnessOK = 'TRUE' WHERE fileId = '{fileId}' ");
                else
                    this.database.Execute($"UPDATE main.{str} SET harnessOK = 'FALSE' WHERE fileId = '{fileId}' ");
                this.database.Commit();
            }
            catch (SQLiteException ex)
            {
                MetadataDB.loggingService.getLogger().LogAppError(LoggingContext.METADATA, "SQL returns exception when updating downloaded status: " + ex?.ToString(), memberName: nameof(updateDownloadedStatus), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataDB.cs", sourceLineNumber: 361);
                this.database.Rollback();
            }
        }
    }

    internal void updatePPFRefetchStatus(string material, bool setAsDownloaded)
    {
        string str = setAsDownloaded ? "TRUE" : "FALSE";
        lock (this.lockObj)
        {
            try
            {
                this.database.BeginTransaction();
                this.database.Execute($"UPDATE main.MaterialStatistics SET PPFRefetchStatus = '{str}' WHERE material = '{material}'");
                this.database.Commit();
            }
            catch (SQLiteException ex)
            {
                MetadataDB.loggingService.getLogger().LogAppError(LoggingContext.METADATA, "SQL returns exception when updating PPFRefetch status: " + ex?.ToString(), memberName: nameof(updatePPFRefetchStatus), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataDB.cs", sourceLineNumber: 380);
                this.database.Rollback();
            }
        }
    }

    internal List<string> GetRefetchedPPFEnumbers()
    {
        lock (this.lockObj)
        {
            List<string> refetchedPpfEnumbers = new List<string>();
            try
            {
                refetchedPpfEnumbers = this.database.Query<MaterialStatistics>("SELECT material FROM materialStatistics WHERE PPFRefetchStatus = 'TRUE'").Select<MaterialStatistics, string>((Func<MaterialStatistics, string>)(x => x.material)).ToList<string>();
            }
            catch (SQLiteException ex)
            {
                MetadataDB.loggingService.getLogger().LogAppError(LoggingContext.METADATA, "SQL returns exception when fetching PPFRefetch status: " + ex?.ToString(), memberName: nameof(GetRefetchedPPFEnumbers), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataDB.cs", sourceLineNumber: 400);
            }
            return refetchedPpfEnumbers;
        }
    }

    public int getNumberOfLocalFiles()
    {
        lock (this.lockObj)
            return new SQLiteCommand(this.database)
            {
                CommandText = $"SELECT(SELECT COUNT(DISTINCT fileId) FROM module WHERE harnessOK = 'TRUE') + ( SELECT COUNT(DISTINCT fileId) FROM document WHERE harnessOK = 'TRUE' AND type NOT IN {this.excludeDocumentTypeForNonSMM}) AS total"
            }.ExecuteScalar<int>();
    }

    public long GetDownloadedFileSize()
    {
        long downloadedFileSize = 0;
        lock (this.lockObj)
        {
            SQLiteCommand sqLiteCommand = new SQLiteCommand(this.database)
            {
                CommandText = "SELECT((SELECT COALESCE(SUM(fileSize), 0) FROM (select fileId, fileSize from module WHERE harnessOK = 'TRUE' group by fileId)) + (SELECT COALESCE(SUM(fileSize), 0) FROM(select fileId, fileSize from document WHERE harnessOK = 'TRUE' AND document.md5 != '-1' group by fileId))) AS total"
            };
            try
            {
                downloadedFileSize = sqLiteCommand.ExecuteScalar<long>();
            }
            catch (SQLiteException ex)
            {
                MetadataDB.loggingService.getLogger().LogAppDebug(LoggingContext.METADATA, "SQL returns exception: " + ex?.ToString(), memberName: nameof(GetDownloadedFileSize), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataDB.cs", sourceLineNumber: 434);
            }
            return downloadedFileSize;
        }
    }

    public List<module> getDownloadedModules()
    {
        lock (this.lockObj)
            return this.database.Query<module>("SELECT * FROM module WHERE harnessOK = 'TRUE'");
    }

    public List<module> GetModuleSpecs(smm_module smmModule)
    {
        lock (this.lockObj)
            return this.database.Query<module>($"SELECT * FROM module where moduleid = '{smmModule.moduleid}' AND version = '{smmModule.version}' AND node = '{smmModule.node}' AND harnessOk = 'TRUE'");
    }

    public string getdModuleIdName(string _moduleid, iService5.Ssh.DTO.Version _version, string type)
    {
        string str = $"{_version.major.ToString()}.{_version.minor.ToString()}.{_version.revision.ToString()}";
        SQLiteCommand sqLiteCommand;
        lock (this.lockObj)
            sqLiteCommand = new SQLiteCommand(this.database)
            {
                CommandText = $"SELECT message FROM smmQuery_helperFinal WHERE moduleid = '{_moduleid}'  AND version = '{str}' AND type = '{type}'"
            };
        return sqLiteCommand.ExecuteScalar<string>() ?? "Unknown";
    }

    public bool iSHarnessTrueForFileId(string fileId)
    {
        lock (this.lockObj)
            return new SQLiteCommand(this.database)
            {
                CommandText = $"SELECT COUNT(DISTINCT fileId) FROM module WHERE harnessOK = 'TRUE' AND module.fileId = '{fileId}'"
            }.ExecuteScalar<int>() > 0;
    }

    public List<smm_module> getSMMModules()
    {
        lock (this.lockObj)
            return this.database.Query<smm_module>("SELECT DISTINCT smm_module.moduleid, smm_module.version FROM smm_module");
    }

    public int getModuleListTotal() => this.database.Table<module>().Count();

    public int getDelta()
    {
        lock (this.lockObj)
        {
            List<int> intList = new List<int>() { 0, 0 };
            if (CoreApp.settings.GetItem("BridgeOff").Value.ToLower().Equals("false"))
                return new SQLiteCommand(this.database)
                {
                    CommandText = $"SELECT((SELECT COUNT(DISTINCT fileId) FROM module WHERE harnessOK = 'FALSE') + (SELECT COUNT(fileId) FROM document WHERE document.harnessOK = 'FALSE' AND document.md5 != '-1' AND document.type NOT IN {this.excludeDocumentTypeForNonSMM})) AS total"
                }.ExecuteScalar<int>();
            return new SQLiteCommand(this.database)
            {
                CommandText = $"SELECT((SELECT COUNT(DISTINCT fileId) FROM module WHERE harnessOK = 'FALSE') + (SELECT COUNT(fileId) FROM document WHERE document.harnessOK = 'FALSE' AND document.type = '{this.includeSmmDocumentType}' AND document.md5 != '-1')) AS total"
            }.ExecuteScalar<int>();
        }
    }

    public document getConnectionGraphic(string enumber)
    {
        lock (this.lockObj)
        {
            List<document> documentList = this.database.Query<document>($"SELECT material_document.document, document.version, document.fileId, document.type, document.md5 FROM material_document INNER JOIN document ON material_document.document = document.document WHERE material_document.material = '{enumber}' AND type = '{this.includeSmmDocumentType}' AND md5 != '-1'");
            return documentList.Count > 0 ? documentList[0] : (document)null;
        }
    }

    public document getConnectionGraphicNonSmm(string enumber)
    {
        lock (this.lockObj)
        {
            List<document> documentList = this.database.Query<document>($"SELECT DISTINCT material_document.document, document.version, document.fileId, document.type, document.md5,document.fileSize FROM material_document INNER JOIN document ON material_document.document = document.document WHERE type = '14403' AND  material_document.material = '{enumber}'  AND md5 != '-1'");
            return documentList.Count > 0 ? documentList[0] : (document)null;
        }
    }

    public document GetMonitoringGraphicsDocument(string enumber)
    {
        lock (this.lockObj)
        {
            List<document> documentList = this.database.Query<document>($"SELECT material_document.document, document.version, document.fileId, document.type, document.md5 FROM material_document INNER JOIN document ON material_document.document = document.document WHERE material_document.material= '{enumber}' AND type = '14404' AND md5 != '-1'");
            return documentList.Count > 0 ? documentList[0] : (document)null;
        }
    }

    public List<document> getEnumberDocuments(string enumber)
    {
        lock (this.lockObj)
        {
            List<document> enumberDocuments = new List<document>();
            try
            {
                string query1 = $"SELECT material_document.document, document.version, document.fileId, document.type, document.md5,document.fileSize, document.harnessOK FROM material_document INNER JOIN document ON material_document.document = document.document WHERE material_document.material = '{enumber}' AND document.type = '{this.includeSmmDocumentType}' AND document.type NOT IN {this.excludeDocumentTypeForNonSMM}";
                string query2 = $"SELECT material_document.document, document.version, document.fileId, document.type, document.md5,document.fileSize, document.harnessOK FROM material_document INNER JOIN document ON material_document.document = document.document WHERE material_document.material = '{enumber}' AND document.type NOT IN {this.excludeDocumentTypeForNonSMM}";
                enumberDocuments = !this.isSMM(enumber) ? this.database.Query<document>(query2) : this.database.Query<document>(query1);
            }
            catch (Exception ex)
            {
                MetadataDB.loggingService.getLogger().LogAppDebug(LoggingContext.BACKEND, "SQL Exception occured while fetching enumber documents : " + ex.Message, memberName: nameof(getEnumberDocuments), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataDB.cs", sourceLineNumber: 598);
            }
            return enumberDocuments;
        }
    }

    public List<enumber_modules> getEnumberModules(string enumber)
    {
        lock (this.lockObj)
        {
            Tuple<string, string> vibAndKi = UtilityFunctions.getVibAndKi(enumber);
            return this.database.Query<enumber_modules>($"SELECT smm_module.moduleid, smm_module.version, smm_module.vib, smm_module.ki, module.node, module.fileId, module.md5, module.fileSize, module.harnessOK FROM smm_module INNER JOIN module ON smm_module.moduleid = module.moduleid AND smm_module.version = module.version AND smm_module.node=module.node WHERE smm_module.vib = \"{vibAndKi.Item1}\" AND smm_module.ki = \"{vibAndKi.Item2}\" AND smm_module.node IN {Constants.nodesToDownloadConcat}");
        }
    }

    public smm getSMM(string enumber)
    {
        lock (this.lockObj)
        {
            Tuple<string, string> vibAndKi = UtilityFunctions.getVibAndKi(enumber);
            List<smm> smmList = this.database.Query<smm>($"SELECT * FROM smm WHERE smm.vib = \"{vibAndKi.Item1}\" AND smm.ki = \"{vibAndKi.Item2}\"");
            return smmList.Count != 0 ? smmList[0] : (smm)null;
        }
    }

    public bool checkIfEnoIsSyMaNa(string enumber)
    {
        lock (this.lockObj)
        {
            Tuple<string, string> vibAndKi = UtilityFunctions.getVibAndKi(enumber);
            return new SQLiteCommand(this.database)
            {
                CommandText = $"SELECT COUNT(*) FROM smm WHERE architecture = '{MODULE_ARCHITECTURE_TYPE.SYMANA.ToString()}' AND vib = '{vibAndKi.Item1}' AND ki = '{vibAndKi.Item2}'"
            }.ExecuteScalar<int>() != 0;
        }
    }

    public (List<module>, List<document>) getObsoleteBinaries()
    {
        lock (this.lockObj)
            return (this.database.Query<module>("SELECT * FROM modulesToBeDeleted"), this.database.Query<document>("SELECT * FROM docsToBeDeleted"));
    }

    public List<ppf> GetPpfs()
    {
        lock (this.lockObj)
            return this.database.Query<ppf>("SELECT * FROM ppf");
    }

    public List<string> GetDownloadedEnumbersFromPPF()
    {
        lock (this.lockObj)
            return this.database.Query<ppf>("SELECT DISTINCT vib || '/' || ki  AS enumber FROM ppf").Select<ppf, string>((Func<ppf, string>)(x => x.enumber)).ToList<string>();
    }

    public List<smm_whitelist> getDowngradePass(long module)
    {
        lock (this.lockObj)
        {
            List<smm_whitelist> smmWhitelistList = this.database.Query<smm_whitelist>($"SELECT * FROM smm_whitelist where moduleid = '{module}'");
            return smmWhitelistList.Count > 0 ? smmWhitelistList : (List<smm_whitelist>)null;
        }
    }

    public void removeDownloadedObsoleteModule(module recordToBeRemoved)
    {
        lock (this.lockObj)
            this.database.Execute("DELETE FROM modulesToBeDeleted WHERE moduleid =? AND version=? AND node=?", (object)recordToBeRemoved.moduleid, (object)recordToBeRemoved.version, (object)recordToBeRemoved.node);
    }

    public void removeDownloadedObsoleteDocument(document recordToBeRemoved)
    {
        lock (this.lockObj)
            this.database.Execute("DELETE FROM docsToBeDeleted WHERE document =?", (object)recordToBeRemoved._document);
    }

    public void deleteObsoleteModulesFromPPFTable(List<Tuple<long, string>> listOfObsoleteModules)
    {
        lock (this.lockObj)
        {
            try
            {
                List<long> values1 = new List<long>();
                List<string> values2 = new List<string>();
                foreach (Tuple<long, string> ofObsoleteModule in listOfObsoleteModules)
                {
                    values1.Add(ofObsoleteModule.Item1);
                    values2.Add(ofObsoleteModule.Item2);
                }
                string commaSeperatedValues1 = UtilityFunctions.getCommaSeperatedValues<long>(values1);
                string commaSeperatedValues2 = UtilityFunctions.getCommaSeperatedValues<string>(values2);
                this.database.BeginTransaction();
                this.database.Execute($"DELETE FROM ppf WHERE moduleid IN {commaSeperatedValues1} AND version IN {commaSeperatedValues2}");
                this.database.Commit();
            }
            catch (SQLiteException ex)
            {
                MetadataDB.loggingService.getLogger().LogAppError(LoggingContext.METADATA, "SQL returns exception while updating Database schema: " + ex?.ToString(), memberName: nameof(deleteObsoleteModulesFromPPFTable), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataDB.cs", sourceLineNumber: 724);
                this.database.Rollback();
                throw;
            }
        }
    }

    public bool CheckIfModuleFileIsDownloaded(string fileid)
    {
        lock (this.lockObj)
            return new SQLiteCommand(this.database)
            {
                CommandText = $"SELECT COUNT(fileId) FROM module WHERE module.fileId = \"{fileid}\" AND harnessOK = 'TRUE'"
            }.ExecuteScalar<int>() != 0;
    }

    public bool CheckIfDocumentIsDownloaded(string fileid)
    {
        lock (this.lockObj)
            return new SQLiteCommand(this.database)
            {
                CommandText = $"SELECT COUNT(fileId) FROM document WHERE fileId = \"{fileid}\" AND harnessOK = 'TRUE'"
            }.ExecuteScalar<int>() != 0;
    }

    public List<module_with_enumber> getModulesDeltaSet()
    {
        lock (this.lockObj)
            return this.database.Query<module_with_enumber>("SELECT module.fileId, module.fileSize,module.moduleid,module.version,module.name,module.md5,module.harnessOK,module.node FROM module INNER JOIN smm_module on smm_module.moduleid = module.moduleid AND smm_module.version = module.version AND smm_module.node = module.node WHERE module.harnessOK = 'FALSE' GROUP BY module.fileId");
    }

    public List<document> getDocumentsDeltaSet()
    {
        lock (this.lockObj)
            return !CoreApp.settings.GetItem("BridgeOff").Value.ToLower().Equals("false") ? this.database.Query<document>($"SELECT * FROM document WHERE document.harnessOK = 'FALSE' AND document.type= '{this.includeSmmDocumentType}' AND document.md5 != '-1' AND document.fileId != ''") : this.database.Query<document>("SELECT * FROM document WHERE document.harnessOK = 'FALSE' AND document.md5 != '-1' AND document.fileId != '' AND document.type NOT IN " + this.excludeDocumentTypeForNonSMM);
    }

    public List<material> getMaterialMatches(string prefix, MatchPattern criteria)
    {
        lock (this.lockObj)
        {
            bool flag = CoreApp.settings.GetItem("BridgeOff").Value.ToLower().Equals("true");
            string str = $" ('{prefix}%')";
            return this.database.Query<material>("SELECT * FROM material WHERE material like" + (criteria != MatchPattern.PREFIX ? $"('%{prefix}%')" : (!flag ? str : str + " and type LIKE 'SMM_WITH_WIFI%' OR type LIKE 'SMM_WITHOUT_WIFI%'")));
        }
    }

    public List<uploadDocument> getMaterialDocs(string enumber)
    {
        lock (this.lockObj)
            return this.database.Query<uploadDocument>($"SELECT material_document.document, document.version, document.type FROM material_document INNER JOIN document ON material_document.document = document.document WHERE material_document.material = '{enumber}' AND  md5 != '-1' AND document.type NOT IN {this.excludeDocumentTypeForNonSMM}");
    }

    public IEnumerable<varcodes> checkOldVarCodingPrerequisites(string enumber)
    {
        List<varcodes> varcodesList = new List<varcodes>();
        lock (this.lockObj)
        {
            try
            {
                Tuple<string, string> vibAndKi = UtilityFunctions.getVibAndKi(enumber);
                varcodesList = this.database.Query<varcodes>($"SELECT varcodes.enumber, varcodes.msgids_and_data FROM varcodes INNER JOIN material ON material.material = varcodes.enumber WHERE material.type = 'NON_SMM' AND material.material = '{enumber}'");
                if (varcodesList.Count == 0)
                    varcodesList = this.database.Query<varcodes>($"SELECT varcodes.enumber, varcodes.msgids_and_data FROM varcodes where enumber = '{vibAndKi.Item1}'");
            }
            catch (Exception ex)
            {
                MetadataDB.loggingService.getLogger().LogAppDebug(LoggingContext.BACKEND, "Error checkOldCodingPrerequisites: " + ex?.ToString(), memberName: nameof(checkOldVarCodingPrerequisites), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataDB.cs", sourceLineNumber: 821);
            }
            MetadataDB.loggingService.getLogger().LogAppDebug(LoggingContext.BACKEND, "materialOldCoding items for checkCodingPrerequisites: " + varcodesList.Count.ToString(), memberName: nameof(checkOldVarCodingPrerequisites), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataDB.cs", sourceLineNumber: 823);
            return (IEnumerable<varcodes>)varcodesList;
        }
    }

    internal List<shorttexts> getShortText(string enumber)
    {
        lock (this.lockObj)
            return this.database.Query<shorttexts>($"SELECT shorttexts.textnum, shorttexts.lang, shorttexts.shorttext FROM shorttexts WHERE textnum = '{enumber}'");
    }

    internal List<module> getModules()
    {
        lock (this.lockObj)
            return this.database.Query<module>("SELECT * FROM module");
    }

    public List<Country> GetCountryList()
    {
        try
        {
            lock (this.lockObj)
                return this.database.Query<Country>("SELECT DISTINCT country FROM priority");
        }
        catch (Exception ex)
        {
            MetadataDB.loggingService.getLogger().LogAppDebug(LoggingContext.BACKEND, ex.Message, memberName: nameof(GetCountryList), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataDB.cs", sourceLineNumber: 856);
            return new List<Country>();
        }
    }

    internal bool isSMMWithWifi(string enumber)
    {
        lock (this.lockObj)
        {
            List<material> materialList = this.database.Query<material>($"SELECT * FROM material WHERE material = '{enumber.ToUpperInvariant()}' ");
            if (materialList == null || materialList.Count == 0)
                return false;
            foreach (material material in materialList)
            {
                if (material.type.StartsWith("SMM_WITH_WIFI"))
                    return true;
            }
            return false;
        }
    }

    public string getSSHKeyValue()
    {
        lock (this.lockObj)
            return new SQLiteCommand(this.database)
            {
                CommandText = "SELECT value FROM configuration WHERE key = 'SSH.KEY'"
            }.ExecuteScalar<string>();
    }

    public string getErrorCodeMessage(string errorCode, string eNumber)
    {
        lock (this.lockObj)
        {
            Tuple<string, string> vibAndKi = UtilityFunctions.getVibAndKi(eNumber);
            return new SQLiteCommand(this.database)
            {
                CommandText = $"SELECT smmerrorcodesTable.message FROM smm_error_codes smmerrorcodesTable LEFT JOIN smm smmTable ON smmerrorcodesTable.devicetype = smmTable.devicetypecode WHERE smmTable.vib = '{vibAndKi.Item1}' AND smmTable.ki = '{vibAndKi.Item2}' AND smmerrorcodesTable.code = '{errorCode}'"
            }.ExecuteScalar<string>();
        }
    }

    public string getErrorCodeDescription(string errorCode, string eNumber)
    {
        Tuple<string, string> vibAndKi = UtilityFunctions.getVibAndKi(eNumber);
        lock (this.lockObj)
            return new SQLiteCommand(this.database)
            {
                CommandText = $"SELECT smmerrorcodesTable.description FROM smm_error_codes smmerrorcodesTable LEFT JOIN smm smmTable ON smmerrorcodesTable.devicetype = smmTable.devicetypecode WHERE smmTable.vib = '{vibAndKi.Item1}' AND smmTable.ki = '{vibAndKi.Item2}' AND smmerrorcodesTable.code = '{errorCode}'"
            }.ExecuteScalar<string>();
    }

    public string getMemoryCodeDescription(string errorCode)
    {
        lock (this.lockObj)
            return new SQLiteCommand(this.database)
            {
                CommandText = $"SELECT shorttext FROM shorttexts WHERE textnum = '{errorCode}'"
            }.ExecuteScalar<string>();
    }

    public string getShortText(string shortTextCode, string lang)
    {
        lock (this.lockObj)
            return new SQLiteCommand(this.database)
            {
                CommandText = $"SELECT COALESCE(MAX(shorttext),(SELECT shorttext FROM shorttexts WHERE textnum = '{shortTextCode}' AND lang = 'en')) FROM shorttexts WHERE textnum = '{shortTextCode}' AND lang = '{lang}';"
            }.ExecuteScalar<string>();
    }

    private bool ExecuteSQL(string command)
    {
        this.database.Execute(command);
        return true;
    }

    public List<SmmQueryHelperFinal> GetSMMModuleWithLabels(string enumber)
    {
        lock (this.lockObj)
        {
            Tuple<string, string> vibAndKi = UtilityFunctions.getVibAndKi(enumber);
            this.ExecuteSQL("CREATE TABLE IF NOT EXISTS nodemap (node INTEGER NOT NULL, devicetype INTEGER NOT NULL, message NVARCHAR(1000) NOT NULL, PRIMARY KEY (node, devicetype) );");
            this.ExecuteSQL("CREATE TABLE IF NOT EXISTS smm_firmware_ids (id BIGINT NOT NULL, package NVARCHAR(100) NOT NULL, model NVARCHAR(100) NOT NULL, PRIMARY KEY(id) );");
            this.ExecuteSQL("DROP TABLE IF EXISTS smmQuery_helper0;");
            this.ExecuteSQL("DROP TABLE IF EXISTS smmQuery_helper1;");
            this.ExecuteSQL("DROP TABLE IF EXISTS smmQuery_helperFinal;");
            this.ExecuteSQL("DROP TABLE IF EXISTS smmQuery_helperFinal_with_recovery;");
            this.ExecuteSQL($"CREATE TABLE smmQuery_helper0 as SELECT smm.*, smm_module.moduleid, smm_module.version, smm_module.node, smm_module.type FROM smm INNER JOIN smm_module ON smm.vib = smm_module.vib AND smm.ki = smm_module.ki WHERE smm.vib = \"{vibAndKi.Item1}\" AND smm.ki = \"{vibAndKi.Item2}\" AND smm_module.node IN {Constants.nodesToDownloadConcat};");
            this.ExecuteSQL("CREATE TABLE smmQuery_helper1 as SELECT smmQuery_helper0.*, module.name, module.fileId, module.md5, module.fileSize FROM smmQuery_helper0 LEFT JOIN module WHERE smmQuery_helper0.moduleid = module.moduleid AND smmQuery_helper0.version = module.version AND smmQuery_helper0.node = module.node;");
            this.ExecuteSQL("CREATE TABLE smmQuery_helperFinal as SELECT smmQuery_helper1.*, nodemap.message FROM smmQuery_helper1 LEFT JOIN nodemap ON smmQuery_helper1.devicetypecode = nodemap.devicetype AND smmQuery_helper1.node = nodemap.node;");
            this.ExecuteSQL("CREATE TABLE smmQuery_helperFinal_with_recovery as SELECT smmQuery_helperFinal.*, smm_firmware_ids.package FROM smmQuery_helperFinal LEFT JOIN smm_firmware_ids ON smmQuery_helperFinal.moduleid = smm_firmware_ids.id;");
            return this.database.Query<SmmQueryHelperFinal>("SELECT * FROM smmQuery_helperFinal_with_recovery");
        }
    }

    public List<smm_module> GetModulesDetails(string enumber)
    {
        lock (this.lockObj)
        {
            Tuple<string, string> vibAndKi = UtilityFunctions.getVibAndKi(enumber);
            string str = this.checkIfEnoIsSyMaNa(enumber) ? Constants.nodesToDownloadToSyMaNaConcat : Constants.nodesToDownloadToElpConcat;
            return this.database.Query<smm_module>($"SELECT DISTINCT * FROM smm_module SM WHERE SM.vib = \"{vibAndKi.Item1}\" AND SM.ki = \"{vibAndKi.Item2}\" AND SM.node IN {str}");
        }
    }

    public bool IsRecovery(long moduleId)
    {
        lock (this.lockObj)
        {
            List<smm_firmware_ids> smmFirmwareIdsList = this.database.Query<smm_firmware_ids>($"SELECT * FROM smm_firmware_ids WHERE id = '{moduleId.ToString()}' ");
            if (smmFirmwareIdsList == null || smmFirmwareIdsList.Count == 0)
                return false;
            foreach (smm_firmware_ids smmFirmwareIds in smmFirmwareIdsList)
            {
                if (smmFirmwareIds.Package.ToLower() == "recovery")
                    return true;
            }
            return false;
        }
    }

    public bool DeleteAllBinaries(IPlatformSpecificServiceLocator _locator)
    {
        try
        {
            string[] array = ((IEnumerable<string>)Directory.GetFiles(_locator.GetPlatformSpecificService().getFolder(), "*")).Where<string>((Func<string, bool>)(file => !((IEnumerable<string>)Constants.extToBeRetained).Any<string>((Func<string, bool>)(x => file.EndsWith(x, StringComparison.OrdinalIgnoreCase))))).ToArray<string>();
            int num = 0;
            foreach (string path in array)
            {
                File.Delete(path);
                ++num;
            }
            MetadataDB.loggingService.getLogger().LogAppError(LoggingContext.METADATA, "Total Deleted Binary files: " + num.ToString(), memberName: nameof(DeleteAllBinaries), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataDB.cs", sourceLineNumber: 1011);
        }
        catch (Exception ex)
        {
            return false;
        }
        lock (this.lockObj)
        {
            this.database.BeginTransaction();
            try
            {
                this.database.Execute("UPDATE main.document SET harnessOK = 'FALSE' WHERE harnessOK = 'TRUE' ");
                this.database.Execute("UPDATE main.module SET harnessOK = 'FALSE' WHERE harnessOK = 'TRUE' ");
                this.database.Execute("DELETE FROM ppf");
            }
            catch (SQLiteException ex)
            {
                MetadataDB.loggingService.getLogger().LogAppError(LoggingContext.METADATA, "SQL returns exception: " + ex?.ToString(), memberName: nameof(DeleteAllBinaries), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataDB.cs", sourceLineNumber: 1029);
                return false;
            }
            this.database.Commit();
        }
        return true;
    }

    public bool IsDocumentAvailable(string enumber, string docType)
    {
        lock (this.lockObj)
            return this.database.Query<document>($"SELECT material_document.document, document.version, document.type FROM material_document INNER JOIN document ON material_document.document = document.document WHERE material_document.material = '{enumber}'  AND type = '{docType}' AND md5 != '-1'").Count > 0;
    }

    public List<Version> GetVersionFromFeatureTable(string name)
    {
        lock (this.lockObj)
        {
            string str = new SQLiteCommand(this.database)
            {
                CommandText = $"SELECT version FROM smm_features WHERE name = '{name}'"
            }.ExecuteScalar<string>();
            if (str == null)
                return new List<Version>();
            string[] source = str.Split(';');
            if (((IEnumerable<string>)source).Count<string>() == 1)
                return new List<Version>()
        {
          Version.FromString(source[0])
        };
            if (((IEnumerable<string>)source).Count<string>() != 2)
                return new List<Version>();
            return new List<Version>()
      {
        Version.FromString(source[0]),
        Version.FromString(source[1])
      };
        }
    }

    public bool CheckFeature(string fname)
    {
        lock (this.lockObj)
            return new SQLiteCommand(this.database)
            {
                CommandText = $"SELECT COUNT(*) FROM smm_features WHERE name = '{fname}'"
            }.ExecuteScalar<int>() != 0;
    }

    public string GetmaterialType(string enumber)
    {
        lock (this.lockObj)
            return new SQLiteCommand(this.database)
            {
                CommandText = $"SELECT type FROM material WHERE material = '{enumber}'"
            }.ExecuteScalar<string>();
    }

    public bool IsMaterialAvailable(string enumber)
    {
        lock (this.lockObj)
            return new SQLiteCommand(this.database)
            {
                CommandText = $"SELECT COUNT(*) from material WHERE material = '{enumber}'"
            }.ExecuteScalar<int>() > 0;
    }

    public List<MaterialStatistics> GetDownloadFilesStatisticsModules()
    {
        lock (this.lockObj)
            return this.database.Query<MaterialStatistics>("SELECT material, group_concat(DISTINCT country) as country, count(DISTINCT fileid) as fileCount, SUM(DISTINCT FileSize) AS fileSize, 'SMM' AS deviceClass\n                                    FROM (SELECT DISTINCT sm.vib ||'/'|| sm.ki as material, p.country AS country, m.fileid as fileid, m.fileSize AS FileSize\n                                    FROM smm_module sm LEFT JOIN priority p ON p.material = sm.vib ||'/'|| sm.ki \n                                    INNER JOIN module m on m.moduleid = sm.moduleid AND m.version = sm.version AND m.node = sm.node)\n                                    GROUP BY material");
    }

    public List<MaterialStatistics> GetDownloadFilesStatisticsDocuments()
    {
        lock (this.lockObj)
            return this.database.Query<MaterialStatistics>($"SELECT material, group_concat(DISTINCT COUNTRY) as country, count(DISTINCT FileID) as fileCount, SUM(DISTINCT FileSize) AS fileSize, deviceClass\n                                    FROM (SELECT md.material, p.country as COUNTRY, d.fileid AS FileID, d.fileSize AS fileSize, m.type as deviceClass\n                                    FROM material_document md\n                                    INNER JOIN document d ON md.document = d.document \n                                    LEFT JOIN priority p ON p.material = md.material\n                                    INNER JOIN material m on md.material = m.material\n                                    WHERE d.type NOT IN {this.excludeDocumentTypeForNonSMM} AND m.type NOT LIKE 'ECU' AND m.material NOT LIKE 'Z9KDKD0Z01%')\n                                    GROUP BY material");
    }

    public void UpdateMaterialStatistics()
    {
        lock (this.lockObj)
        {
            try
            {
                List<MaterialStatistics> materialList = UtilityFunctions.GetMaterialList();
                this.database.BeginTransaction();
                this.database.Execute("DROP TABLE IF EXISTS main.materialStatistics");
                this.database.Execute("CREATE TABLE main.materialStatistics(material NVARCHAR (20), country NVARCHAR (5),deviceClass NVARCHAR (50),fileCount BIGINT,fileSize BIGINT,PPFRefetchStatus NVARCHAR (20),PRIMARY KEY(material))");
                foreach (MaterialStatistics materialStatistics in materialList)
                {
                    string ppfRefetchStatus = this.GetPPFRefetchStatus(materialStatistics.material);
                    this.database.Execute($"INSERT INTO main.materialStatistics (material, country, deviceClass, fileCount, fileSize, PPFRefetchStatus)\n                                             VALUES ('{materialStatistics.material}','{materialStatistics.country}', '{materialStatistics.deviceClass}',{materialStatistics.fileCount},{materialStatistics.fileSize},'{ppfRefetchStatus}')");
                }
                this.database.Commit();
            }
            catch (SQLiteException ex)
            {
                MetadataDB.loggingService.getLogger().LogAppError(LoggingContext.METADATA, "SQL returns exception while updating Material Statistics schema: " + ex?.ToString(), memberName: nameof(UpdateMaterialStatistics), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataDB.cs", sourceLineNumber: 1178);
                this.database.Rollback();
                throw;
            }
        }
    }

    public string GetPPFRefetchStatus(string material)
    {
        List<string> numbersFromOldDb = Mvx.IoCProvider.Resolve<IMetadataService>().GetRefetchedPPFENumbersFromOldDB();
        return numbersFromOldDb != null && numbersFromOldDb.Count > 0 && numbersFromOldDb.Any<string>((Func<string, bool>)(ppfEnumbers => ppfEnumbers.Contains(material))) ? "TRUE" : "FALSE";
    }

    public void StorePpfEntity(
      string _vib,
      string _ki,
      long _moduleid,
      string _version,
      long _expdate,
      long _type,
      string _ca,
      string _ppffile)
    {
        lock (this.lockObj)
        {
            try
            {
                this.database.BeginTransaction();
                this.database.Execute($"INSERT OR REPLACE INTO main.ppf (vib, ki, moduleid, version, expirydate, type, ca, ppffile)\n                                             VALUES ('{_vib}', '{_ki}', {_moduleid}, '{_version}', {_expdate}, {_type}, '{_ca}', '{_ppffile}')");
                this.database.Commit();
            }
            catch (SQLiteException ex)
            {
                MetadataDB.loggingService.getLogger().LogAppError(LoggingContext.METADATA, "SQL returns exception while updating ppf table :" + ex?.ToString(), memberName: nameof(StorePpfEntity), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataDB.cs", sourceLineNumber: 1212);
                this.database.Rollback();
                throw;
            }
        }
    }

    public List<MaterialStatistics> GetMaterialStatistics()
    {
        lock (this.lockObj)
            return this.database.Query<MaterialStatistics>("SELECT * FROM materialStatistics");
    }

    public DownloadFilesStatistics GetDownloadFilesStatistics(
      Dictionary<DeviceClass, List<MaterialStatistics>> deviceClassBasedMaterials)
    {
        lock (this.lockObj)
        {
            int _fileCount = 0;
            long _fileSize1 = 0;
            long _fileSize2;
            try
            {
                List<MaterialStatistics> classBasedMaterial1 = deviceClassBasedMaterials[DeviceClass.SMM];
                List<MaterialStatistics> classBasedMaterial2 = deviceClassBasedMaterials[DeviceClass.NON_SMM];
                string seperatedEnumbers1 = this.getCommaSeperatedENumbers(classBasedMaterial1);
                string seperatedEnumbers2 = this.getCommaSeperatedENumbers(classBasedMaterial2);
                DownloadFilesStatistics downloadFilesStatistics1 = this.executeDBQueryOf(this.virtualENumberDBQuery());
                if (classBasedMaterial1 != null && classBasedMaterial1.Count > 0)
                {
                    DownloadFilesStatistics downloadFilesStatistics2 = this.executeDBQueryOf(this.smmModulesDBQuery(seperatedEnumbers1));
                    DownloadFilesStatistics downloadFilesStatistics3 = this.executeDBQueryOf(this.smmDocumentsDBQuery(seperatedEnumbers1));
                    _fileCount = downloadFilesStatistics2.fileCount + downloadFilesStatistics3.fileCount;
                    _fileSize1 = downloadFilesStatistics2.fileSize + downloadFilesStatistics3.fileSize;
                }
                if (classBasedMaterial2 != null && classBasedMaterial2.Count > 0)
                {
                    DownloadFilesStatistics downloadFilesStatistics4 = this.executeDBQueryOf(this.nonSmmModulesDBQuery(seperatedEnumbers2));
                    _fileCount += downloadFilesStatistics4.fileCount;
                    _fileSize1 += downloadFilesStatistics4.fileSize;
                }
                _fileCount += downloadFilesStatistics1.fileCount;
                _fileSize2 = _fileSize1 + downloadFilesStatistics1.fileSize;
            }
            catch (SQLiteException ex)
            {
                MetadataDB.loggingService.getLogger().LogAppError(LoggingContext.METADATA, "SQL returns exception: " + ex?.ToString(), memberName: nameof(GetDownloadFilesStatistics), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataDB.cs", sourceLineNumber: 1270);
                return new DownloadFilesStatistics(_fileCount, _fileSize1);
            }
            return new DownloadFilesStatistics(_fileCount, _fileSize2);
        }
    }

    public DownloadFilesStatistics GetDownloadFilesStatisticsForFullDownloadAndCountryRelevant(
      string deviceClass,
      Dictionary<DeviceClass, List<MaterialStatistics>> deviceClassBasedMaterials)
    {
        lock (this.lockObj)
        {
            int _fileCount = 0;
            long _fileSize = 0;
            try
            {
                List<MaterialStatistics> classBasedMaterial1 = deviceClassBasedMaterials[DeviceClass.SMM];
                List<MaterialStatistics> classBasedMaterial2 = deviceClassBasedMaterials[DeviceClass.NON_SMM];
                string seperatedEnumbers1 = this.getCommaSeperatedENumbers(classBasedMaterial1);
                string seperatedEnumbers2 = this.getCommaSeperatedENumbers(classBasedMaterial2);
                DownloadFilesStatistics downloadFilesStatistics1 = this.executeDBQueryOf(this.virtualENumberDBQuery());
                switch (EnumHelper.FromString<DeviceClass>(deviceClass))
                {
                    case DeviceClass.ALL:
                        DownloadFilesStatistics downloadFilesStatistics2 = this.executeDBQueryOf(this.smmModulesDBQuery(seperatedEnumbers1));
                        DownloadFilesStatistics downloadFilesStatistics3 = this.executeDBQueryOf(this.smmDocumentsDBQuery(seperatedEnumbers1));
                        DownloadFilesStatistics downloadFilesStatistics4 = this.executeDBQueryOf(this.nonSmmModulesDBQuery(seperatedEnumbers2));
                        _fileCount = downloadFilesStatistics1.fileCount + downloadFilesStatistics2.fileCount + downloadFilesStatistics3.fileCount + downloadFilesStatistics4.fileCount;
                        _fileSize = downloadFilesStatistics1.fileSize + downloadFilesStatistics2.fileSize + downloadFilesStatistics3.fileSize + downloadFilesStatistics4.fileSize;
                        break;
                    case DeviceClass.SMM:
                        DownloadFilesStatistics downloadFilesStatistics5 = this.executeDBQueryOf(this.smmModulesDBQuery(seperatedEnumbers1));
                        DownloadFilesStatistics downloadFilesStatistics6 = this.executeDBQueryOf(this.smmDocumentsDBQuery(seperatedEnumbers1));
                        _fileCount = downloadFilesStatistics1.fileCount + downloadFilesStatistics5.fileCount + downloadFilesStatistics6.fileCount;
                        _fileSize = downloadFilesStatistics1.fileSize + downloadFilesStatistics5.fileSize + downloadFilesStatistics6.fileSize;
                        break;
                    case DeviceClass.NON_SMM:
                        DownloadFilesStatistics downloadFilesStatistics7 = this.executeDBQueryOf(this.nonSmmModulesDBQuery(seperatedEnumbers2));
                        _fileCount = downloadFilesStatistics1.fileCount + downloadFilesStatistics7.fileCount;
                        _fileSize = downloadFilesStatistics1.fileSize + downloadFilesStatistics7.fileSize;
                        break;
                    default:
                        MetadataDB.loggingService.getLogger().LogAppError(LoggingContext.BINARY, "Option did not match with any predefined items. deviceClass=" + deviceClass, memberName: nameof(GetDownloadFilesStatisticsForFullDownloadAndCountryRelevant), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataDB.cs", sourceLineNumber: 1333);
                        break;
                }
            }
            catch (SQLiteException ex)
            {
                MetadataDB.loggingService.getLogger().LogAppError(LoggingContext.METADATA, "SQL returns exception: " + ex?.ToString(), memberName: nameof(GetDownloadFilesStatisticsForFullDownloadAndCountryRelevant), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataDB.cs", sourceLineNumber: 1340);
                return new DownloadFilesStatistics(_fileCount, _fileSize);
            }
            return new DownloadFilesStatistics(_fileCount, _fileSize);
        }
    }

    private string virtualENumberDBQuery()
    {
        return $"SELECT COALESCE(COUNT(fileid),0) AS fileCount,COALESCE(SUM(fileSize), 0) AS fileSize FROM \n                                        (SELECT DISTINCT document.fileid,document.fileSize FROM document\n                                        WHERE document.harnessOK='FALSE'  AND \n                                        document.md5 != '-1' AND  document.fileId != '' \n                                        AND document.type NOT IN {this.excludeDocumentTypeForNonSMM} AND document.document  IN \n                                        (SELECT material_document.document FROM material_document\n                                         WHERE  material_document.material like 'Z9KDKD0Z01%' ))";
    }

    private string smmModulesDBQuery(string eNumbers)
    {
        return $"SELECT COALESCE(COUNT(fileid),0) AS fileCount,COALESCE(SUM(fileSize), 0) AS fileSize FROM \n                                       (SELECT DISTINCT module.fileId,module.fileSize,module.moduleid,module.version,module.md5 FROM module\n                                        INNER JOIN smm_module on smm_module.moduleid = module.moduleid \n\t                                    AND smm_module.version = module.version\n                                        WHERE module.harnessOK = 'FALSE' AND smm_module.vib || '/' || smm_module.ki IN {eNumbers}\n                                        AND smm_module.node IN {Constants.nodesToDownloadConcat})";
    }

    private string smmDocumentsDBQuery(string eNumbers)
    {
        return $"SELECT COALESCE(COUNT(fileid),0) AS fileCount,COALESCE(SUM(fileSize), 0) AS fileSize FROM \n                                        (SELECT document.fileid,document.fileSize,document.version FROM document\n                                        WHERE document.harnessOK='FALSE' AND \n                                        document.md5 != '-1' AND document.fileId != ''\n                                        AND document.type = '{this.includeSmmDocumentType}'\n                                        AND document.document  IN \n                                        (SELECT material_document.document FROM material_document\n                                       WHERE material_document.material NOT LIKE 'Z9KDKD0Z01%' AND material_document.material IN {eNumbers}) GROUP BY document.fileid)";
    }

    private string nonSmmModulesDBQuery(string eNumbers)
    {
        return $"SELECT COALESCE(COUNT(fileid),0) AS fileCount,COALESCE(SUM(fileSize), 0) AS fileSize FROM \n                                        (SELECT DISTINCT document.fileid,document.fileSize,document.version FROM document\n                                        WHERE document.harnessOK='FALSE' AND \n                                        document.md5 != '-1' AND document.fileId != '' AND\n                                         document.type NOT IN {this.excludeDocumentTypeForNonSMM} AND document.document IN \n                                        (SELECT material_document.document FROM material_document\n                                         WHERE material_document.material NOT LIKE 'Z9KDKD0Z01%' AND material_document.material IN {eNumbers}) GROUP BY document.fileid)";
    }

    private DownloadFilesStatistics executeDBQueryOf(string query)
    {
        return this.database.Query<DownloadFilesStatistics>(query)[0];
    }

    public List<module_with_enumber> getNotDownloadedOrWithoutPpfModulesForDownloadSetting(
      Dictionary<DeviceClass, List<MaterialStatistics>> materialList)
    {
        lock (this.lockObj)
        {
            List<module_with_enumber> forDownloadSetting = new List<module_with_enumber>();
            try
            {
                List<MaterialStatistics> material = materialList[DeviceClass.SMM];
                if (material == null || material.Count == 0)
                    return forDownloadSetting;
                return this.database.Query<module_with_enumber>($"SELECT module.fileId,module.fileSize,module.moduleid,module.version,module.md5,module.harnessOK,smm_module.vib,smm_module.ki,smm_module.node FROM module\n                                        INNER JOIN smm_module on smm_module.moduleid = module.moduleid\n                                        AND smm_module.version = module.version\n                                        INNER JOIN ppf ON ppf.vib = smm_module.vib AND ppf.ki = smm_module.ki AND ppf.moduleid = smm_module.moduleid AND ppf.version = smm_module.version\n                                        WHERE smm_module.vib || '/' || smm_module.ki IN {this.getCommaSeperatedENumbers(material)}\n                                        AND smm_module.node IN {Constants.nodesToDownloadConcat}\n                                        AND (SELECT smm_module.vib,smm_module.ki, smm_module.moduleid, smm_module.version) NOT IN SELECT (ppf.vib,ppf.ki, ppf.moduleid, ppf.version)\n                                        OR (module.harnessOK = 'FALSE' AND (SELECT smm_module.vib,smm_module.ki, smm_module.moduleid, smm_module.version) IN SELECT (ppf.vib,ppf.ki, ppf.moduleid, ppf.version)))\n                                        GROUP BY module.fileId");
            }
            catch (SQLiteException ex)
            {
                MetadataDB.loggingService.getLogger().LogAppError(LoggingContext.METADATA, "SQL returns exception: " + ex?.ToString(), memberName: nameof(getNotDownloadedOrWithoutPpfModulesForDownloadSetting), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataDB.cs", sourceLineNumber: 1428);
                return forDownloadSetting;
            }
        }
    }

    public List<module_with_enumber> getModulesDeltaSetForDownloadSetting(
      Dictionary<DeviceClass, List<MaterialStatistics>> materialList,
      bool bIsDownloaded)
    {
        lock (this.lockObj)
        {
            List<module_with_enumber> forDownloadSetting = new List<module_with_enumber>();
            try
            {
                List<MaterialStatistics> material = materialList[DeviceClass.SMM];
                if (material == null || material.Count == 0)
                    return forDownloadSetting;
                return this.database.Query<module_with_enumber>($"SELECT module.fileId,module.fileSize,module.moduleid,module.version,module.md5,module.harnessOK,smm_module.node FROM module\n                                        INNER JOIN smm_module on smm_module.moduleid = module.moduleid \n\t                                    AND smm_module.version = module.version\n                                        WHERE smm_module.vib || '/' || smm_module.ki IN {this.getCommaSeperatedENumbers(material)}\n                                        AND module.harnessOK = '{bIsDownloaded.ToString().ToUpper()}'\n                                        AND smm_module.node IN {Constants.nodesToDownloadConcat}\n                                        GROUP BY module.fileId");
            }
            catch (SQLiteException ex)
            {
                MetadataDB.loggingService.getLogger().LogAppError(LoggingContext.METADATA, "SQL returns exception: " + ex?.ToString(), memberName: nameof(getModulesDeltaSetForDownloadSetting), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataDB.cs", sourceLineNumber: 1462);
                return forDownloadSetting;
            }
        }
    }

    public List<module_with_enumber> getModulesForENumber(string eNumber)
    {
        lock (this.lockObj)
        {
            List<module_with_enumber> modulesForEnumber = new List<module_with_enumber>();
            try
            {
                return this.database.Query<module_with_enumber>($"SELECT module.fileId,module.fileSize,module.moduleid,module.version,module.md5,module.harnessOK,smm_module.node FROM module\n                                        INNER JOIN smm_module on smm_module.moduleid = module.moduleid \n\t                                    AND smm_module.version = module.version\n                                        WHERE smm_module.vib || '/' || smm_module.ki IN ('{eNumber}')\n                                        AND module.harnessOK = 'FALSE'\n                                        AND smm_module.node IN {Constants.nodesToDownloadConcat}\n                                        GROUP BY module.fileId");
            }
            catch (SQLiteException ex)
            {
                MetadataDB.loggingService.getLogger().LogAppError(LoggingContext.METADATA, "SQL returns exception: " + ex?.ToString(), memberName: nameof(getModulesForENumber), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataDB.cs", sourceLineNumber: 1488);
                return modulesForEnumber;
            }
        }
    }

    public List<document> getDocumentsForENumber(string eNumber, DeviceClass deviceClass)
    {
        lock (this.lockObj)
        {
            List<document> documentsForEnumber = new List<document>();
            try
            {
                string materials = $"('{eNumber}')";
                return this.database.Query<document>(deviceClass != DeviceClass.SMM ? this.getNonSMMDocumentsDeltaSetDBQuery(materials) : this.getSMMDocumentsDeltaSetDBQuery(materials));
            }
            catch (SQLiteException ex)
            {
                MetadataDB.loggingService.getLogger().LogAppError(LoggingContext.METADATA, "SQL returns exception: " + ex?.ToString(), memberName: nameof(getDocumentsForENumber), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataDB.cs", sourceLineNumber: 1517);
                return documentsForEnumber;
            }
        }
    }

    public List<document> getDocumentsDeltaSetForSingleDownloadAndPrepareYourWork(
      Dictionary<DeviceClass, List<MaterialStatistics>> materialList,
      bool bIsDownloaded)
    {
        List<document> andPrepareYourWork = new List<document>();
        try
        {
            List<MaterialStatistics> material1 = materialList[DeviceClass.SMM];
            List<MaterialStatistics> material2 = materialList[DeviceClass.NON_SMM];
            List<MaterialStatistics> material3 = materialList[DeviceClass.SMM_FLINTSTONE];
            List<document> first = new List<document>();
            List<document> second1 = new List<document>();
            List<document> second2 = new List<document>();
            if (material1 != null && material1.Count > 0)
                first = this.database.Query<document>(this.getSMMDocumentsDeltaSetDBQuery(this.getCommaSeperatedENumbers(material1), bIsDownloaded));
            if (material2 != null && material2.Count > 0)
                second1 = this.database.Query<document>(this.getNonSMMDocumentsDeltaSetDBQuery(this.getCommaSeperatedENumbers(material2), bIsDownloaded));
            if (material3 != null && material3.Count > 0)
                second2 = this.database.Query<document>(this.getNonSMMDocumentsDeltaSetDBQuery(this.getCommaSeperatedENumbers(material3), bIsDownloaded));
            return first.Concat<document>((IEnumerable<document>)second1).ToList<document>().Concat<document>((IEnumerable<document>)second2).ToList<document>();
        }
        catch (SQLiteException ex)
        {
            MetadataDB.loggingService.getLogger().LogAppError(LoggingContext.METADATA, "SQL returns exception: " + ex?.ToString(), memberName: nameof(getDocumentsDeltaSetForSingleDownloadAndPrepareYourWork), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataDB.cs", sourceLineNumber: 1559);
            return andPrepareYourWork;
        }
    }

    public List<document> getDocumentsDeltaSetForFullDownloadAndCountryRelevant(
      string deviceClass,
      Dictionary<DeviceClass, List<MaterialStatistics>> materialList)
    {
        List<document> andCountryRelevant = new List<document>();
        try
        {
            List<MaterialStatistics> material1 = materialList[DeviceClass.SMM];
            List<MaterialStatistics> material2 = materialList[DeviceClass.NON_SMM];
            List<MaterialStatistics> material3 = materialList[DeviceClass.SMM_FLINTSTONE];
            switch (EnumHelper.FromString<DeviceClass>(deviceClass))
            {
                case DeviceClass.ALL:
                    andCountryRelevant = this.database.Query<document>(this.getSMMDocumentsDeltaSetDBQuery(this.getCommaSeperatedENumbers(material1)));
                    List<document> collection = this.database.Query<document>(this.getNonSMMDocumentsDeltaSetDBQuery(this.getCommaSeperatedENumbers(material2)));
                    andCountryRelevant.AddRange((IEnumerable<document>)collection);
                    break;
                case DeviceClass.SMM:
                    andCountryRelevant = this.database.Query<document>(this.getSMMDocumentsDeltaSetDBQuery(this.getCommaSeperatedENumbers(material1)));
                    break;
                case DeviceClass.NON_SMM:
                    andCountryRelevant = this.database.Query<document>(this.getNonSMMDocumentsDeltaSetDBQuery(this.getCommaSeperatedENumbers(material2)));
                    break;
                case DeviceClass.SMM_FLINTSTONE:
                    andCountryRelevant = this.database.Query<document>(this.getNonSMMDocumentsDeltaSetDBQuery(this.getCommaSeperatedENumbers(material3)));
                    break;
                default:
                    return andCountryRelevant;
            }
            return andCountryRelevant;
        }
        catch (SQLiteException ex)
        {
            MetadataDB.loggingService.getLogger().LogAppError(LoggingContext.METADATA, "SQL returns exception: " + ex?.ToString(), memberName: nameof(getDocumentsDeltaSetForFullDownloadAndCountryRelevant), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataDB.cs", sourceLineNumber: 1622);
            return andCountryRelevant;
        }
    }

    private string getCommaSeperatedENumbers(List<MaterialStatistics> materials)
    {
        return $"({string.Join("','", string.Join(",", materials.Select<MaterialStatistics, string>((Func<MaterialStatistics, string>)(mat => $"'{mat.material}'"))))})";
    }

    public List<document> getDocumentsDeltaSetForDownloadSetting(
      string deviceClass,
      Dictionary<DeviceClass, List<MaterialStatistics>> materialList)
    {
        List<document> forDownloadSetting = new List<document>();
        try
        {
            List<MaterialStatistics> material1 = materialList[DeviceClass.SMM];
            List<MaterialStatistics> material2 = materialList[DeviceClass.NON_SMM];
            string documentsDeltaSetDbQuery;
            switch (EnumHelper.FromString<DeviceClass>(deviceClass))
            {
                case DeviceClass.ALL:
                    documentsDeltaSetDbQuery = this.getNonSMMDocumentsDeltaSetDBQuery(this.getCommaSeperatedENumbers(material1.Concat<MaterialStatistics>((IEnumerable<MaterialStatistics>)material2).ToList<MaterialStatistics>()));
                    break;
                case DeviceClass.SMM:
                    documentsDeltaSetDbQuery = this.getSMMDocumentsDeltaSetDBQuery(this.getCommaSeperatedENumbers(material1));
                    break;
                case DeviceClass.NON_SMM:
                    documentsDeltaSetDbQuery = this.getNonSMMDocumentsDeltaSetDBQuery(this.getCommaSeperatedENumbers(material2));
                    break;
                default:
                    return forDownloadSetting;
            }
            return this.database.Query<document>(documentsDeltaSetDbQuery);
        }
        catch (SQLiteException ex)
        {
            MetadataDB.loggingService.getLogger().LogAppError(LoggingContext.METADATA, "SQL returns exception: " + ex?.ToString(), memberName: nameof(getDocumentsDeltaSetForDownloadSetting), sourceFilePath: "/Users/macmini2/bamboo-agent-home/xml-data/build-dir/UFMS-ACI54-JOB1/iService5/iService5.Core/Services/Data/MetadataDB.cs", sourceLineNumber: 1673);
            return forDownloadSetting;
        }
    }

    private string getNonSMMDocumentsDeltaSetDBQuery(string materials, bool bIsDownloaded = false)
    {
        return $"SELECT DISTINCT document.document,document.version,document.fileId,\n                                     document.fileSize,document.md5,document.type FROM document\n                                        WHERE document.harnessOK='{bIsDownloaded.ToString().ToUpper()}' AND \n                                        document.md5 != '-1' AND document.fileId != '' AND\n                                         document.type NOT IN {this.excludeDocumentTypeForNonSMM} AND document.document IN \n                                        (SELECT material_document.document FROM material_document\n                                         WHERE material_document.material NOT LIKE 'Z9KDKD0Z01%' AND material_document.material IN {materials}) GROUP BY document.fileId";
    }

    private string getSMMDocumentsDeltaSetDBQuery(string materials, bool bIsDownloaded = false)
    {
        return $"SELECT DISTINCT document.document,document.version,document.fileId,\n                                        document.fileSize,document.md5,document.type FROM document\n                                        WHERE document.harnessOK='{bIsDownloaded.ToString().ToUpper()}'  AND \n                                        document.md5 != '-1' AND document.fileId != ''\n                                        AND document.type = '{this.includeSmmDocumentType}'\n                                        AND document.document IN \n                                        (SELECT material_document.document FROM material_document\n                                        WHERE material_document.material NOT LIKE 'Z9KDKD0Z01%' AND material_document.material IN {materials}) GROUP BY document.fileId";
    }

    public List<document> getZ9KDDeltaSet(bool bIsDownloaded)
    {
        lock (this.lockObj)
        {
            List<document> documentList = new List<document>();
            return this.database.Query<document>($"SELECT document.document,document.version,document.fileId,\n                                     document.fileSize,document.md5,document.type FROM material_document\n                                     INNER JOIN document ON material_document.document = document.document\n                                     WHERE material_document.material like 'Z9KDKD0Z01%' AND document.harnessOK = '{bIsDownloaded.ToString().ToUpper()}' \n                                    AND document.md5 != '-1' AND document.fileId != '' AND document.type NOT IN {this.excludeDocumentTypeForNonSMM}");
        }
    }

    public bool CheckIfTableExists(string tableName)
    {
        lock (this.lockObj)
            return new SQLiteCommand(this.database)
            {
                CommandText = $"SELECT count(*) FROM sqlite_master WHERE type='table' AND name='{tableName}'"
            }.ExecuteScalar<int>() != 0;
    }

    public List<smm_module> getSMMModulesRecords(string _vib, string _ki, long _fwid)
    {
        lock (this.lockObj)
            return this.database.Query<smm_module>($"SELECT * FROM smm_module WHERE vib ='{_vib}' AND ki ='{_ki}' AND moduleid = {_fwid} ");
    }

    public string getNodeName(long node, string _devicetype)
    {
        lock (this.lockObj)
            return new SQLiteCommand(this.database)
            {
                CommandText = $"SELECT message FROM nodemap WHERE node = {node} AND devicetype = '{_devicetype}'"
            }.ExecuteScalar<string>();
    }

    public bool IsModuleInRecoveryReboot(AugmentedModule module)
    {
        lock (this.lockObj)
        {
            List<smm_recovery_reboot> source = this.database.Query<smm_recovery_reboot>("SELECT * from smm_recovery_reboot WHERE moduleid = " + module.moduleid.ToString());
            if (source != null && source.Count > 0)
            {
                smm_recovery_reboot smmRecoveryReboot = source.First<smm_recovery_reboot>();
                Version version1 = Version.FromString(smmRecoveryReboot.existing_version_min);
                Version version2 = Version.FromString(smmRecoveryReboot.existing_version_max);
                Version version3 = Version.FromString(smmRecoveryReboot.install_version_min);
                Version version4 = Version.FromString(smmRecoveryReboot.install_version_max);
                if ((version1 is NullVersion || module.Installed >= version1) && (version2 is NullVersion || module.Installed <= version2) && (version3 is NullVersion || module.Available >= version3) && (version4 is NullVersion || module.Available <= version4))
                    return true;
            }
            return false;
        }
    }

    internal bool isSMM(string enumber)
    {
        lock (this.lockObj)
        {
            List<material> materialList = this.database.Query<material>($"SELECT * FROM material WHERE material = '{enumber.ToUpperInvariant()}' ");
            if (materialList == null || materialList.Count == 0)
                return false;
            foreach (material material in materialList)
            {
                if (material.type.StartsWith("SMM_WITH_"))
                    return true;
            }
            return false;
        }
    }

    internal bool isSMMFlintStone(string enumber)
    {
        lock (this.lockObj)
        {
            List<material> materialList = this.database.Query<material>($"SELECT * FROM material WHERE material = '{enumber.ToUpperInvariant()}' AND type='SMM_WITHOUT_WIFI'");
            return materialList != null && materialList.Count != 0;
        }
    }

    internal List<MaterialStatistics> getMaterialStatisticsOfENumbers(string eNumber)
    {
        lock (this.lockObj)
            return this.database.Query<MaterialStatistics>("SELECT * FROM materialStatistics WHERE material IN " + eNumber);
    }

    internal long GetTotalFileSizeForENumber(string eNumber)
    {
        lock (this.lockObj)
            return new SQLiteCommand(this.database)
            {
                CommandText = $"SELECT fileSize FROM materialStatistics WHERE material='{eNumber}'"
            }.ExecuteScalar<long>();
    }

    internal bool isNONSMM(string enumber)
    {
        lock (this.lockObj)
        {
            List<material> materialList = this.database.Query<material>($"SELECT * FROM material WHERE material = '{enumber.ToUpperInvariant()}' ");
            if (materialList == null || materialList.Count == 0)
                return false;
            foreach (material material in materialList)
            {
                if (material.type.StartsWith("NON_SMM"))
                    return true;
            }
            return false;
        }
    }

    internal List<DownloadProxy> GetExpiredAndAboutToExpireENumbers()
    {
        lock (this.lockObj)
        {
            try
            {
                DateTimeOffset dateTimeOffset = DateTimeOffset.Now;
                dateTimeOffset = dateTimeOffset.AddDays(14.0);
                long timeMilliseconds1 = dateTimeOffset.ToUnixTimeMilliseconds();
                dateTimeOffset = DateTimeOffset.Now;
                long timeMilliseconds2 = dateTimeOffset.ToUnixTimeMilliseconds();
                return this.database.Query<DownloadProxy>($"SELECT vib,ki FROM ppf WHERE expirydate <= {timeMilliseconds1} OR expirydate <= {timeMilliseconds2} group by vib,ki");
            }
            catch
            {
                return new List<DownloadProxy>();
            }
        }
    }

    internal void DeletePpfEntries(ppf ppf)
    {
        lock (this.lockObj)
            this.database.Execute($"DELETE FROM ppf WHERE vib='{ppf.vib}' AND ki='{ppf.ki}' AND moduleid = '{ppf.moduleid}' AND version = '{ppf.version}' And type ='{ppf.type}'");
    }

    public List<ppf> GetRelatedPpfs(
      string vib,
      string ki,
      long moduleid,
      string version,
      bool ppf6supported)
    {
        lock (this.lockObj)
            return ppf6supported ? this.database.Query<ppf>($"SELECT * FROM ppf WHERE vib='{vib}' AND ki='{ki}' AND moduleid = '{moduleid}' AND version = '{version}' AND type=6") : this.database.Query<ppf>($"SELECT * FROM ppf WHERE vib='{vib}' AND ki='{ki}' AND moduleid = '{moduleid}' AND version = '{version}' AND type=5");
    }

    public List<ppf> GetAllEnumbersDownloadedPpfs(string enumber, bool onlyType6Ppfs = false)
    {
        lock (this.lockObj)
        {
            Tuple<string, string> vibAndKi = UtilityFunctions.getVibAndKi(enumber);
            return onlyType6Ppfs ? this.database.Query<ppf>($"SELECT * FROM ppf WHERE vib = '{vibAndKi.Item1}' AND ki = '{vibAndKi.Item2}' AND type=6") : this.database.Query<ppf>($"SELECT * FROM ppf WHERE vib = '{vibAndKi.Item1}' AND ki = '{vibAndKi.Item2}'");
        }
    }

    internal bool DBModulesHaveNoArchitecture()
    {
        return this.database.ExecuteScalar<bool>("SELECT NOT EXISTS(SELECT 1 FROM sqlite_master WHERE name = 'module' AND sql like '%architecture%')");
    }

    public void DeleteExistingPPF(DownloadProxy proxy)
    {
        lock (this.lockObj)
            this.database.Execute($"DELETE FROM ppf WHERE vib='{proxy.vib}' AND ki='{proxy.ki}'");
    }
}