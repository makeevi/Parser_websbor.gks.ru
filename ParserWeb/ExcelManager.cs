using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserWeb
{
    class ExcelManager
    {
        private ILog log;

        public ExcelManager(ILog log)
        {
            this.log = log ?? throw new ArgumentNullException(nameof(log));
        }
        public void Save(DataSet dataSet, FileInfo fileInfo)
        {
            
            using (ExcelPackage excelPackege = new ExcelPackage(fileInfo))
            {
                foreach (var item in dataSet.Tables)
                {
                    WriteToSheet(excelPackege, item as DataTable);
                }
                excelPackege.Save();
            }

            log.Info(string.Format("Отчет помещен в {0}", fileInfo.FullName));
            log.Info(string.Format("Попытка открытия файла {0}", fileInfo.FullName));
            System.Diagnostics.Process.Start(fileInfo.FullName);
        }

        private static ExcelPackage WriteToSheet(ExcelPackage package, DataTable dt)
        {
            ExcelWorksheet sheet = package.Workbook.Worksheets.Add(dt.TableName);

            int rows = dt.Rows.Count;
            int cols = dt.Columns.Count;

            //header

            for (int i = 0; i < cols; i++)
            {
                sheet.Cells[1, i + 1].Value = dt.Columns[i].ToString();
            }

            //body


            for (int i = 0, y = 2; i < rows; i++, y++)
            {
                DataRow dr = dt.Rows[i];

                for (int j = 0; j < cols; j++)
                {
                    sheet.Cells[y, j + 1].Value = dr[j].ToString();
                }
            }

            return package;
        }
    }
}
