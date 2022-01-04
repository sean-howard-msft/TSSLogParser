/* Parser for Walter's TSS Tool https://github.com/walter-1/TSS */

using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading;
using TSSLogParser.EFCore;

namespace TSSLogParser
{
    internal class Program
    {
        public static IConfiguration Configuration { get; set; }

        static TSSParserContext db;

        [Verb("parse", HelpText = "Traverse the given directory and parse TSS output.")]
        class ParseTSSOptions
        {

            [Option('d', "directory", Required = false, HelpText = "Directory to traverse.")]
            public string Directory { get; set; }

            [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
            public bool Verbose { get; set; }

            [Option('r', "read", Required = false, HelpText = "Input files to be processed.")]
            public IEnumerable<string> InputFiles { get; set; }

            [Option('x', "xpath", Required = false, HelpText = "XPath query to filter Event Logs.")]
            public string xPath { get; set; }
        }

        [Verb("report", HelpText = "Connect to the specified DB and output reports.")]
        class OutputReportsOptions
        {
            [Option('d', "directory", Required = false, HelpText = "Target directory for report output.")]
            public string Directory { get; set; }

            [Option('r', "regionals", Required = false, HelpText = "Include Regional Reports.")]
            public bool IncludeRegionals { get; set; }

            [Option('g', "globals", Required = false, HelpText = "Include Global Reports.")]
            public bool IncludeGlobals { get; set; }

            [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
            public bool Verbose { get; set; }
            
            [Option('c', "recommendations", Required = false, HelpText = "Get top links from searches based on log entries.")]
            public bool GetRecommendations { get; set; }

            [Option('w', "writerecommendations", Required = false, HelpText = "Write the final report.")]
            public bool IncludeRecommendations { get; internal set; }
        }

        [Verb("recommend", HelpText = "Get top links from searches based on log entries")]
        class RecommendationOptions
        {
            [Option('a', "all", Required = false, HelpText = "Get recommendations for all entries. Omit to retry missing recommendations.")]
            public bool All { get; set; }

            [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
            public bool Verbose { get; set; }

        }

        static void Main(string[] args)
        {
            Configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true)
            .AddUserSecrets<Program>()
            .Build();

            using (db = new TSSParserContext(Configuration["TSSLogParser:ConnectionString"]))
            {
                CreateDbIfNotExists();

                Parser.Default.ParseArguments<ParseTSSOptions, OutputReportsOptions>(args)
                  .MapResult(
                      (OutputReportsOptions opts) => RunReports(opts),
                      (ParseTSSOptions opts) => ParseTSS(opts),
                      (RecommendationOptions opts) => GetRecommendations(opts),
                      errs => 1);
            }
        }

        static void CreateDbIfNotExists()
        {
            try
            {
                db.Database.EnsureCreated();
                // DbInitializer.Initialize(context);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred creating the DB: {ex}");
            }
        }

        static int RunReports(OutputReportsOptions options)
        {
            var startTime = DateTime.Now;
            Console.WriteLine($"Start Time { startTime }");
            try
            {
                var savePath = options.Directory;
                if (string.IsNullOrEmpty(options.Directory))
                    savePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "TSS Reports");
                if (!Directory.Exists(savePath))
                    Directory.CreateDirectory(savePath);

                if (options.IncludeRegionals)
                {
                    var metadata = db.MachineMetadata
                            .GroupBy(mm => new { mm.CountryCode, mm.Region, mm.InfraCode })
                            .Select(mm => mm.Key)
                            .ToList();

                    var regionalMessageCounts = db.RegionalMessageCounts.ToList();
                    var regionalMachineCounts = db.RegionalMachineCounts.ToList();
                    foreach (var data in metadata)
                    {
                        DataSet regionalDataSet = new DataSet();

                        var regionalMessageCountsData = regionalMessageCounts
                            .Where(rd => rd.CountryCode == data.CountryCode
                                      && rd.Region == data.Region
                                      && rd.InfraCode == data.InfraCode)
                            .ToList();
                        var regionalMachineCountsData = regionalMachineCounts
                            .Where(rd => rd.CountryCode == data.CountryCode
                                      && rd.Region == data.Region
                                      && rd.InfraCode == data.InfraCode)
                            .ToList();

                        var regionalTotals = regionalMessageCountsData
                            .Join(regionalMachineCountsData,
                                  messageCount =>
                                    new
                                    {
                                        messageCount.ProviderName,
                                        messageCount.LogName,
                                        messageCount.TruncatedMessage,
                                        messageCount.CountryCode,
                                        messageCount.Region,
                                        messageCount.InfraCode
                                    },
                                  machineCount =>
                                  new
                                  {
                                      machineCount.ProviderName,
                                      machineCount.LogName,
                                      machineCount.TruncatedMessage,
                                      machineCount.CountryCode,
                                      machineCount.Region,
                                      machineCount.InfraCode
                                  },
                                  (msgCnt, machCnt) => new RegionalTotal
                                  {
                                      LogName = msgCnt.LogName,
                                      ProviderName = msgCnt.ProviderName,
                                      TruncatedMessage = msgCnt.TruncatedMessage,
                                      LevelDisplayName = msgCnt.LevelDisplayName,
                                      CountryCode = msgCnt.CountryCode,
                                      Region = msgCnt.Region,
                                      AppCode = msgCnt.AppCode,
                                      InfraCode = msgCnt.InfraCode,
                                      MachineCount = machCnt.MachineCount,
                                      MessageCount = msgCnt.MessageCount
                                  })
                            .ToDataTable();
                        regionalDataSet.Tables.Add(regionalTotals);

                        var machineList = regionalMessageCountsData
                            .Select(rd => rd.MachineName)
                            .Distinct();
                        foreach (var machineName in machineList)
                        {
                            var regionalCounts = regionalMessageCounts
                                .Where(rd => rd.MachineName == machineName)
                                .ToDataTable();
                            regionalCounts.TableName = machineName;
                            regionalDataSet.Tables.Add(regionalCounts);
                        }

                        string fileNameRegional = $"{savePath}\\{data.CountryCode}{data.Region}{data.InfraCode}.xlsx";
                        ExcelHelper.CreateWorkbook(fileNameRegional, "Regional Reports", regionalDataSet);
                    }
                }

                if (options.IncludeGlobals)
                {
                    DataSet globalDataSet = new DataSet();
                    var globalTotals = db.GlobalTotals
                        .ToDataTable();
                    globalDataSet.Tables.Add(globalTotals);

                    if (options.GetRecommendations)
                    {
                        GetRecommendations(new RecommendationOptions { All = false }, globalTotals);
                    }

                    string fileNameGlobal = $"{savePath}\\GlobalReport.xlsx";
                    ExcelHelper.CreateWorkbook(fileNameGlobal, "Global Reports", globalDataSet);
                    new Process
                    {
                        StartInfo = new ProcessStartInfo(fileNameGlobal)
                        {
                            UseShellExecute = true
                        }
                    }.Start();
                }

                if (options.IncludeRecommendations)
                {
                    DataSet recommendationsDataset = new DataSet();
                    var recommendations = db.Recommendations
                        .ToDataTable();
                    recommendationsDataset.Tables.Add(recommendations);

                    string fileNameRecommend = $"{savePath}\\Recommendations.xlsx";
                    ExcelHelper.CreateWorkbook(fileNameRecommend, "Recommendations report", recommendationsDataset);
                    new Process
                    {
                        StartInfo = new ProcessStartInfo(fileNameRecommend)
                        {
                            UseShellExecute = true
                        }
                    }.Start();
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred running reports: {ex}");
            }

            Console.WriteLine($"End Time { DateTime.Now }");
            var duration = DateTime.Now - startTime;
            Console.WriteLine($"Duration: {duration}");
            return 0;
        }

        static int GetRecommendations(RecommendationOptions options = null, DataTable globalTotals = null)
        {
            if (globalTotals == null)
            {
                if (options.All)
                   globalTotals = db.GlobalTotals
                   .ToDataTable();
                else
                    globalTotals = db.GlobalTotals
                    .Where(gt => gt.MsftDocsSearch == null)
                    .ToDataTable();
            }

            RecommendationEngine recommendationEngine = new(
                Configuration["TSSLogParser:GoogleSearchAPIKey"], 
                Configuration["TSSLogParser:GoogleSearchEngineID"]);

            var i = 0;
            foreach (DataRow row in globalTotals.Rows)
            {
                try
                {
                    string searchString = string.Join(' ', row["ProviderName"], row["Id"], row["TruncatedMessage"]);
                    var msftResult = recommendationEngine.SearchGoogle(searchString, "docs.microsoft.com", true);
                    var webResult = recommendationEngine.SearchGoogle(searchString, "docs.microsoft.com", false);

                    var rowCount = db.Procedures.RecommendationImportAsync(
                        row["ProviderName"].ToString(),
                        row["LogName"].ToString(),
                        row["TruncatedMessage"].ToString(),
                        msftResult.SearchURL,
                        msftResult.FirstResult,
                        webResult.SearchURL,
                        webResult.FirstResult).Result;

                    row["MsftDocsSearch"] = msftResult.SearchURL;
                    row["MsftDocsTopResult"] = msftResult.FirstResult;
                    row["WebSearch"] = webResult.SearchURL;
                    row["WebTopResult"] = webResult.FirstResult;
                    Thread.Sleep(1000);
                    i++;
                }
                catch (Google.GoogleApiException ex)
                {
                    if (ex.Message.Contains("per user"))
                    {
                        Thread.Sleep(60000);
                        i = 0;
                    }
                }
            }
            return 1;
        }

        static int ParseTSS(ParseTSSOptions opts)
        {
            // Get TSS ZIP files from specified directory
            var zipPaths = opts.InputFiles;
            if (!string.IsNullOrEmpty(opts.Directory))
                zipPaths = Directory
                    .EnumerateFiles(opts.Directory)
                    .Where(p => Path.GetExtension(p) == ".zip");
            if (opts.Verbose)
            {
                Console.WriteLine("Zip files in path are: ");
                zipPaths.ToList().ForEach(p => Console.WriteLine(p));
            }

            foreach (var zipPath in zipPaths)
            {
                try
                {
                    using (var file = File.OpenRead(zipPath))
                    using (var zip = new ZipArchive(file, ZipArchiveMode.Read))
                    {
                        string unzipFolder = Path.Combine(Path.GetDirectoryName(zipPath), Path.GetFileNameWithoutExtension(zipPath));
                        if (!Directory.Exists(unzipFolder))
                        {
                            Directory.CreateDirectory(unzipFolder);

                            ParseEVTX(opts, zipPath, zip, unzipFolder);
                            ParseEventLogXML(opts, zipPath, zip, unzipFolder);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Error processing {zipPath}: {ex}");
                }
            }

            // Run Reports
            RunReports(new OutputReportsOptions() { Directory = opts.Directory, Verbose = opts.Verbose, IncludeRecommendations = true });

            return 0;
        }

        private static string GetMessage(EventLogRecord l)
        {
            return string.Join(" ", l.Properties
                .Where(l =>
                {
                    string value = l.Value
                    .ToString()
                    .Replace("{", "")
                    .Replace("}", "");
                    bool isDate = DateTime.TryParse(value, out _);
                    bool isGuid = Guid.TryParse(value, out _);
                    return !isDate && !isGuid;
                })
                .Select(l => l.Value.ToString()));
        }

        private static IEnumerable<EventLogRecord> LogRecordCollection(string filename, string xpathquery = "*")
        {
            var eventLogQuery = new EventLogQuery(filename, PathType.FilePath, xpathquery);

            using (var eventLogReader = new EventLogReader(eventLogQuery))
            {
                EventLogRecord eventLogRecord;

                while ((eventLogRecord = (EventLogRecord)eventLogReader.ReadEvent()) != null)
                    yield return eventLogRecord;
            } 
        }

        private static void ParseEVTX(ParseTSSOptions opts, string zipPath, ZipArchive zip, string unzipFolder)
        {
            var entries = zip.Entries.Where(e => e.Name.EndsWith(".evtx"));
            if (opts.Verbose)
            {
                Console.WriteLine($"EventLog files in {Path.GetFileName(zipPath)} are: ");
                entries.ToList().ForEach(e => Console.WriteLine(e));
            }

            foreach (var entry in entries)
            {
                string destinationFileName = Path.Combine(unzipFolder, entry.Name);
                try
                {
                    // Extract the logs to a folder
                    entry.ExtractToFile(destinationFileName, true);
                    while (!File.Exists(destinationFileName)) 
                    {
                        Thread.Sleep(500);
                    }

                    var eventLog = LogRecordCollection(destinationFileName, opts.xPath)
                        .Select(l => new EventLog 
                        { 
                            ContainerLog = l.ContainerLog,
                            Id = l.Id,
                            Level = l.Level,
                            LevelDisplayName = GetLevelDisplayName(l.Level),
                            LogName = l.LogName,
                            MachineName = l.MachineName,
                            Message = GetMessage(l),
                            ProviderName = l.ProviderName,
                            RecordId = (int)l.RecordId.Value,
                            TimeCreated = l.TimeCreated.Value
                        })
                        .ToDataTable();
                    
                    using (SqlConnection dbConnection = new SqlConnection(Configuration["TSSLogParser:ConnectionString"]))
                    {
                        dbConnection.Open();
                        using (SqlBulkCopy s = new SqlBulkCopy(dbConnection))
                        {
                            s.DestinationTableName = "EventLogs";

                            foreach (var column in eventLog.Columns)
                                s.ColumnMappings.Add(column.ToString(), column.ToString());

                            s.WriteToServer(eventLog);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Error processing {entry.Name}: {ex}");
                }
            }
        }

        private static string GetLevelDisplayName(byte? level)
        {
            switch (level)
            {
                case 1: return "Critical";
                case 2: return "Error";
                case 3: return "Warning";
                case 4: return "Information";
                case 5: return "Verbose";
                default: return "Unknown";
            }
        }

        private static void ParseEventLogXML(ParseTSSOptions opts, string zipPath, ZipArchive zip, string unzipFolder)
        {
            var entries = zip.Entries.Where(e => e.Name.ToLower().EndsWith("eventlog.xml"));
            if (opts.Verbose)
            {
                Console.WriteLine($"EventLog files in {Path.GetFileName(zipPath)} are: ");
                entries.ToList().ForEach(e => Console.WriteLine(e));
            }

            foreach (var entry in entries)
            {
                try
                {
                    var filePath = Path.Combine(unzipFolder, entry.FullName);
                    if (!Directory.Exists(Path.GetDirectoryName(filePath)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    }
                    entry.ExtractToFile(filePath , true);

                    var fe = db.Procedures.RunSSISPkgXMLImportAsync(filePath).Result;

                    var re = db.Procedures.FlatEventImportAsync(Path.GetFileNameWithoutExtension(zipPath)).Result;
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.Message);
                }
            }
        }
    }
}
