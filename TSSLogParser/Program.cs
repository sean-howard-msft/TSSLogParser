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
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Management.Automation;
using TSSLogParser.EFCore;

namespace TSSLogParser
{
    internal class Program
    {
        public static IConfiguration Configuration { get; set; }

        [Verb("parse", HelpText = "Traverse the given directory and parse TSS output.")]
        class ParseTSSOptions
        {

            [Option('d', "directory", Required = false, HelpText = "Directory to traverse.")]
            public string Directory { get; set; }

            [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
            public bool Verbose { get; set; }

            [Option('r', "read", Required = false, HelpText = "Input files to be processed.")]
            public IEnumerable<string> InputFiles { get; set; }
        }
        [Verb("report", HelpText = "Connect to the specified DB and output reports.")]
        class OutputReportsOptions
        {
            [Option('d', "directory", Required = false, HelpText = "Target directory for report output.")]
            public string Directory { get; set; }

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

            // CreateDbIfNotExists();

            Parser.Default.ParseArguments<ParseTSSOptions, OutputReportsOptions>(args)
              .MapResult(
                  (OutputReportsOptions opts) => RunReports(opts),
                  (ParseTSSOptions opts) => ParseTSS(opts),
                  errs => 1);
        }

        static void CreateDbIfNotExists()
        {
            try
            {
                using (var db = new TSSParserContext())
                {
                    db.Database.EnsureCreated();
                }
                // DbInitializer.Initialize(context);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred creating the DB: {ex.Message}");
            }
        }

        static int RunReports(OutputReportsOptions options)
        {
            try
            {
                var savePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "TSS Reports");
                if (!Directory.Exists(savePath))
                    Directory.CreateDirectory(savePath);

                using (var db = new TSSParserContext(Configuration["TSSLogParser:ConnectionString"]))
                {
                    var metadata = db.MachineMetadata
                        .GroupBy(mm => new { mm.CountryCode, mm.Region })
                        .Select(mm => mm.Key)
                        .ToList();
                    foreach (var data in metadata)
                    {
                        DataSet regionalDataSet = new DataSet();

                        var regionalTotals = db.RegionalTotals
                            .Where(rc => rc.CountryCode == rc.CountryCode
                                      && rc.Region == data.Region)
                            .OrderBy(gc => gc.LogName)
                            .ThenBy(gc => gc.ProviderName)
                            .ThenByDescending(gc => gc.MessageCount)
                            .ToDataTable();
                        regionalDataSet.Tables.Add(regionalTotals);

                        var regionalData = db.RegionalMessageCounts
                            .Where(rc => rc.CountryCode == rc.CountryCode
                                      && rc.Region == data.Region)
                            .ToList();
                        var machineList = regionalData
                            .Select(rd => rd.MachineName)
                            .Distinct();
                        foreach (var machineName in machineList)
                        {
                            var regionalCounts = regionalData
                                .Where(rd => rd.MachineName == machineName)
                                .OrderBy(gc => gc.LogName)
                                .ThenBy(gc => gc.ProviderName)
                                .ThenByDescending(gc => gc.MessageCount)
                                .ToDataTable();
                            regionalCounts.TableName = machineName;
                            regionalDataSet.Tables.Add(regionalCounts);
                        }

                        //var regionalEvents = db.EventLogsFreshes
                        //    .Where(rc => rc.CountryCode == rc.CountryCode
                        //              && rc.Region == data.Region)
                        //    .ToDataTable();
                        //regionalEvents.TableName = "Raw Events";
                        //regionalDataSet.Tables.Add(regionalEvents);

                        string fileNameRegional = $"{savePath}\\{data.CountryCode}{data.Region}.xlsx";
                        ExcelHelper.CreateWorkbook(fileNameRegional, "Regional Reports", regionalDataSet);
                        new Process
                        {
                            StartInfo = new ProcessStartInfo(fileNameRegional)
                            {
                                UseShellExecute = true
                            }
                        }.Start();
                    }

                    DataSet globalDataSet = new DataSet();
                    var globalCounts = db.GlobalTotals
                        .OrderBy(gc => gc.LogName)
                        .ThenBy(gc => gc.ProviderName)
                        .ThenByDescending(gc => gc.MachineCount)
                        .ToDataTable();
                    string fileNameGlobal = $"{savePath}\\GlobalReport.xlsx";
                    globalDataSet.Tables.Add(globalCounts);
                    ExcelHelper.CreateWorkbook(fileNameGlobal, "Global Reports", globalDataSet);
                    new Process
                    {
                        StartInfo = new ProcessStartInfo(fileNameGlobal)
                        {
                            UseShellExecute = true
                        }
                    }.Start();

                }
                // DbInitializer.Initialize(context);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred running reports: {ex.Message}");
            }

            return 0;
        }

        static int ParseTSS(ParseTSSOptions opts)
        {
            // Get TSS ZIP files from specified directory
            var zipPaths = Directory
                .EnumerateFiles(opts.Directory)
                .Where(p => Path.GetExtension(p) == ".zip" && p.Contains("tss_"));
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
                        string unzipFolder = $"{Path.GetDirectoryName(zipPath)}\\{Path.GetFileNameWithoutExtension(zipPath)}";
                        if (!Directory.Exists(unzipFolder))
                        {
                            Directory.CreateDirectory(unzipFolder);
                        }

                        var entries = zip.Entries
                            .Where(e => e.Name.EndsWith(".evtx"));
                        if (opts.Verbose)
                        {
                            Console.WriteLine($"EventLog files in {Path.GetFileName(zipPath)} are: ");
                            entries.ToList().ForEach(e => Console.WriteLine(e));
                        }

                        foreach (var entry in entries)
                        {
                            string destinationFileName = $"{unzipFolder}\\{entry.Name}";
                            string csvPath = $"app_data\\{Path.GetFileNameWithoutExtension(destinationFileName)}.csv";
                            try
                            {
                                // Extract the logs to a folder, use PS to parse, export to CSV
                                entry.ExtractToFile(destinationFileName, true);
                                using (var powerShellInstance = PowerShell.Create())
                                {
                                    string script = "Get-WinEvent" +
                                                    $" -Path \"{destinationFileName}\" " +
                                                    $" -FilterXPath \"*[System[(Level=2 or Level=3)]]\" |" +
                                                    $" select RecordId,MachineName,LogName,ProviderName,TimeCreated,LevelDisplayName,Level,ID,Message,ContainerLog | " +
                                                    $" Export-Csv \"{csvPath}\" -NoTypeInformation";
                                    if (opts.Verbose) Console.WriteLine($"PS Script for {destinationFileName}: {script}");
                                    powerShellInstance.AddScript(script);
                                    Collection<PSObject> PSOutput = powerShellInstance.Invoke();
                                }

                                // Load CSV to DataTable
                                DataTable csvData = new DataTable();
                                using (TextFieldParser csvReader = new TextFieldParser(csvPath))
                                {
                                    csvReader.SetDelimiters(new string[] { "," });
                                    csvReader.HasFieldsEnclosedInQuotes = true;
                                    string[] colFields = csvReader.ReadFields();
                                    if (colFields != null)
                                    {
                                        foreach (string column in colFields)
                                        {
                                            DataColumn datacolumn = new DataColumn(column)
                                            {
                                                Unique = column == "RecordId",
                                                AllowDBNull = true
                                            };
                                            csvData.Columns.Add(datacolumn);
                                        }
                                        while (!csvReader.EndOfData)
                                        {
                                            try
                                            {
                                                string[] fieldData = csvReader.ReadFields();
                                                //Making empty value as null
                                                for (int i = 0; i < fieldData.Length; i++)
                                                {
                                                    if (fieldData[i] == "")
                                                    {
                                                        fieldData[i] = null;
                                                    }
                                                }
                                                csvData.Rows.Add(fieldData);
                                            }
                                            catch (Exception)
                                            {
                                                // Silently Deduplicate
                                            }
                                        }
                                    }
                                }

                                if (csvData.Rows.Count > 0)
                                {
                                    try
                                    {
                                        // Load DataTable to SQL DB using bulkcopy
                                        using (SqlConnection dbConnection = new SqlConnection(Configuration["TSSLogParser:ConnectionString"]))
                                        {
                                            dbConnection.Open();
                                            using (SqlBulkCopy s = new SqlBulkCopy(dbConnection))
                                            {
                                                s.DestinationTableName = "EventLogs";

                                                foreach (var column in csvData.Columns)
                                                    s.ColumnMappings.Add(column.ToString(), column.ToString());

                                                s.WriteToServer(csvData);
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.Error.WriteLine(ex.Message);
                                    }
                                }

                                if (File.Exists(csvPath))
                                    File.Delete(csvPath);
                            }
                            catch (Exception ex)
                            {
                                Console.Error.WriteLine($"Error processing {entry.Name}: {ex.Message}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Error processing {zipPath}: {ex.Message}");
                }
            }

            // Run Reports
            RunReports(new OutputReportsOptions() { Directory = opts.Directory, Verbose = opts.Verbose });

            return 0;
        }

        static void HandleParseError(IEnumerable<Error> errs)
        {
            //handle errors
        }
    }
}
