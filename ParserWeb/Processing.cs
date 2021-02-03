using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserWeb
{
    class Processing
    {
        public event Action<string> Message;
        private Organization[] organizations;
        private ILog log;


        public Processing(Organization[] organizations, ILog log)
        {
            this.organizations = organizations ?? throw new ArgumentNullException(nameof(organizations));
            this.log = log ?? throw new ArgumentNullException(nameof(log));
        }


        public void StartProcessing()
        {
            using (IWebDriver driver = new OpenQA.Selenium.Chrome.ChromeDriver())
            {
                log.Info("Запуск браузера");
                System.Threading.Thread.Sleep(3000);
                driver.Navigate().GoToUrl("https://websbor.gks.ru/webstat/#!/gs/statistic-codes");
                System.Threading.Thread.Sleep(3000);


                ElementPageManager pageManager = new ElementPageManager(driver, log);


                foreach (var organization in organizations)
                {
                    log.Debug(string.Format("Установка значение полю ИНН  {0}", organization.Inn));
                    pageManager.TextEntityInn = organization.Inn;
                    pageManager.ButtonClick();
                    pageManager.FindTableInPage(organization);

                    System.Threading.Thread.Sleep(1000);

                }

                log.Info("Работа с браузером закончена");

            }


            //DirectoryInfo dirInfo = new DirectoryInfo(string.Format("{0}\\{1}", programSetting.Path, check.ToString()));
            //if (!dirInfo.Exists)
            //    dirInfo.Create();

            //String NewDateFormat = DateTime.Now.ToString("yyyy-MM-dd-HH-mm");

            //var fileInfo = new FileInfo(string.Format(dirInfo.FullName + @"\\{0} {1}.xlsx", NewDateFormat, Guid.NewGuid().ToString()));
            var fileInfo = new FileInfo(string.Format("Report\\{0}.xlsx",Guid.NewGuid()));
            
            if(!fileInfo.Directory.Exists)
                fileInfo.Directory.Create();

            DataSetProcessing(out DataSet bookStore);
            new ExcelManager(log).Save(bookStore, fileInfo);

        }

        private void DataSetProcessing(out DataSet bookStore)
        {
            bookStore = new DataSet( string.Format("Report"));
            DataTable booksTable = new DataTable("websbor.gks.ru");
            bookStore.Tables.Add(booksTable);



            var objOrganization = typeof(Organization).GetProperties()
                .Where(a => a.GetCustomAttributes(false).Any(t => t is TitleAttribute))
                .Select(a => new
                {
                    property = a,
                    title = (a.GetCustomAttributes(false).Where(f => f is TitleAttribute).FirstOrDefault() as TitleAttribute).Title
                });

            var objReport = typeof(Organization.Report).GetProperties()
                .Where(a => a.GetCustomAttributes(false).Any(t => t is TitleAttribute))
                .Select(a => new
                {
                    property = a,
                    title = (a.GetCustomAttributes(false).Where(f => f is TitleAttribute).FirstOrDefault() as TitleAttribute).Title
                });



            booksTable.Columns.AddRange(objOrganization.Select(a => new DataColumn(a.title, Type.GetType("System.String"))).ToArray());
            booksTable.Columns.AddRange(objReport.Select(a => new DataColumn(a.title, Type.GetType("System.String"))).ToArray());

            foreach (var  organization in organizations.Where(a=>a.Reports?.Length>0))
            {

                foreach (var report in organization.Reports)
                {

                    DataRow dataRow = booksTable.NewRow();
                    dataRow.ItemArray = new object[booksTable.Columns.Count];

                    for (int i = 0; i < dataRow.ItemArray.Length; i++)
                    {
                        var propOrg = objOrganization.Where(a => a.title == booksTable.Columns[i].ColumnName).FirstOrDefault();
                        if (propOrg != null)
                        {
                            dataRow[i] = propOrg.property.GetValue(organization) as string;
                        }

                        var propReport = objReport.Where(a => a.title == booksTable.Columns[i].ColumnName).FirstOrDefault();
                        if (propReport != null)
                        {
                            dataRow[i] = propReport.property.GetValue(report) as string;
                        }
                    }

                    booksTable.Rows.Add(dataRow);
                }

            }


        }

        
    }
}
