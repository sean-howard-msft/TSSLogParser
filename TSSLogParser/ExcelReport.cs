using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Threading;
using Excel = Microsoft.Office.Interop.Excel;

namespace TSSLogParser
{
    public class ExcelHelper
    {
        public static void CreateWorkbook(string fileName, string reportDescription, DataSet dataSet)
        {
            using(new ExcelUILanguageHelper())
            {
                Excel.Application xlsApp;
                Excel.Workbook xlsWorkbook;
                Excel.Worksheet xlsWorksheet;
                object misValue = System.Reflection.Missing.Value;

                // Remove the old excel report file
                try
                {
                    FileInfo oldFile = new FileInfo(fileName);
                    if(oldFile.Exists)
                    {
                        File.SetAttributes(oldFile.FullName, FileAttributes.Normal);
                        oldFile.Delete();
                    }
                }
                catch(Exception ex)
                {
                    Console.Error.WriteLine($"Error removing old Excel report: {ex.Message}");
                    return;
                }

                try
                {
                    xlsApp = new Excel.Application();
                    xlsWorkbook = xlsApp.Workbooks.Add(misValue);

                    foreach (DataTable table in dataSet.Tables)
                    {
                        xlsWorksheet = (Excel.Worksheet)xlsWorkbook.Sheets[dataSet.Tables.IndexOf(table) + 1];
                        xlsWorksheet.Name = table.TableName;

                        if (table.Rows.Count > 0)
                        {
                            foreach (DataColumn column in table.Columns)
                            {
                                xlsWorksheet.Cells[1, table.Columns.IndexOf(column) + 1] = column.ColumnName;
                            }
                        }

                        foreach (DataRow row in table.Rows)
                        {
                            foreach (DataColumn column in table.Columns)
                            {
                                xlsWorksheet.Cells[table.Rows.IndexOf(row) + 1, table.Columns.IndexOf(column) + 1] = row[column];
                            }
                        }

                        Excel.Range range = xlsWorksheet.get_Range("A1", ExcelColumnFromNumber(table.Columns.Count) + (table.Rows.Count).ToString());
                        range.Columns.AutoFit();

                        ReleaseObject(xlsWorksheet);
                    }

                    xlsWorkbook.SaveAs(fileName, Excel.XlFileFormat.xlWorkbookDefault, misValue, misValue, misValue, misValue,
                        Excel.XlSaveAsAccessMode.xlExclusive, Excel.XlSaveConflictResolution.xlLocalSessionChanges, misValue, misValue, misValue, misValue);
                    xlsWorkbook.Close(true, misValue, misValue);
                    xlsApp.Quit();

                    ReleaseObject(xlsWorkbook);
                    ReleaseObject(xlsApp);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("Error creating Excel report: " + ex.Message);
                }
            }
        }

        static private void ReleaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch(Exception ex)
            {
                obj = null;
                Console.Error.WriteLine("Exception occured while releasing object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }

        public static string ExcelColumnFromNumber(int column)
        {
            string columnString = "";
            decimal columnNumber = column;
            while (columnNumber > 0)
            {
                decimal currentLetterNumber = (columnNumber - 1) % 26;
                char currentLetter = (char)(currentLetterNumber + 65);
                columnString = currentLetter + columnString;
                columnNumber = (columnNumber - (currentLetterNumber + 1)) / 26;
            }
            return columnString;
        }

        public static int NumberFromExcelColumn(string column)
        {
            int retVal = 0;
            string col = column.ToUpper();
            for (int iChar = col.Length - 1; iChar >= 0; iChar--)
            {
                char colPiece = col[iChar];
                int colNum = colPiece - 64;
                retVal = retVal + colNum * (int)Math.Pow(26, col.Length - (iChar + 1));
            }
            return retVal;
        }
    }

    class ExcelUILanguageHelper : IDisposable
    {
        private CultureInfo m_CurrentCulture;

        public ExcelUILanguageHelper()
        {
            // save current culture and set culture to en-US
            m_CurrentCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        }

        // return to normal culture
        public void Dispose() => Thread.CurrentThread.CurrentCulture = m_CurrentCulture;
    }
}
