using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserWeb
{
    class ElementPageManager
    {
        private IWebDriver driver;
        private IWebElement entityInn;
        private IWebElement entityButton;
        private ILog log;
        private int maxStep = 60;
        public string TextEntityInn
        {
            get => entityInn.GetAttribute("value");
            set => SendKeyPage(entityInn, value);
        }

        public ElementPageManager(IWebDriver driver, ILog log)
        {
            this.driver = driver ?? throw new ArgumentNullException(nameof(driver));
            this.log = log ?? throw new ArgumentNullException(nameof(log));

            var entitys = FindElementPage(driver);
            entityInn = entitys.entityInn;
            entityButton = entitys.entityButton;
        }

        // Клик по кнопке найти
        public void ButtonClick()
        {
            for (int i = 0; i < maxStep; i++)
            {
                if (entityButton.Enabled)
                {
                    entityButton.Click();
                    return;
                }
                    

                System.Threading.Thread.Sleep(1000);
            }

            throw new Exception("Ошибка при загрузке страницы. #1");

        } 

        // Поиск элементов на странице
        private (IWebElement entityInn, IWebElement entityButton) FindElementPage(IWebDriver driver)
        {
            for (int i = 0; i < maxStep; i++)
            {
                IWebElement entityInn = driver.FindElement(By.Id("inn"));// Поиск поля ИНН на странице
                IWebElement entityButton = driver.FindElements(By.XPath("//button"))?.Where(a => a.Text == "Получить").FirstOrDefault();// Поиск кнопки "Получить" на странице

                if (entityInn != null && entityButton != null)
                    return (entityInn, entityButton);

                System.Threading.Thread.Sleep(1000);
            }

            throw new Exception("Ошибка при загрузке страницы. #2");
        }

        // Устанавливаем значение полю
        private void SendKeyPage(IWebElement webElement, string text)
        {
            for (int i = 0; i < maxStep; i++)
            {
                entityInn.Clear();
                webElement.SendKeys(text);
                System.Threading.Thread.Sleep(1000);
                if (text == webElement.GetAttribute("value"))
                    return;

                entityInn.Clear();
                System.Threading.Thread.Sleep(1000);
            }

            throw new Exception("Ошибка при установки значения полю на странице");
        }


        private bool FindErrorInTable()
        {
            string[] msg = new string[]
            {
                "По вашему запросу ничего не найдено"
            };

            if (driver.FindElements(By.TagName("span"))?.Any(a => msg.Contains(a.Text)) == true)
            {
                log.Warn("Пустой результат запроса");
                driver.FindElements(By.XPath("//button"))?.Where(a => a.Text == "Скрыть").FirstOrDefault()?.Click();
                return true;
            }
                

            return false;
        }

        private bool FindWaiter()
        {
            if (driver?.FindElements(By.Id("waiter"))?.Any(a => a.GetAttribute("class") == "waiter d-flex align-items-center show") == true)
                return true;
            return false;
        }

        private bool FindNoTable()
        {
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    if (driver.FindElements(By.TagName("p")).Any(a => a.Text == "Не найдены формы статистической отчётности"))
                        return true;
                    return false;
                }
                catch 
                {
                    System.Threading.Thread.Sleep(1000);
                }
            }

            throw new Exception("Ошибка при загрузке страницы. #4");
        }

        public void FindTableInPage(Organization organization)
        {
            // Тут propertyInfo класса и название атрибута 
            var obj = typeof(Organization.Report).GetProperties()
                    .Where(a => a.GetCustomAttributes(false).Any(t => t is TitleAttribute))
                    .Select(a => new
                    {
                        property = a,
                        title = (a.GetCustomAttributes(false).Where(f => f is TitleAttribute).FirstOrDefault() as TitleAttribute).Title
                    });



            for (int i = 0; i < 10; i++)
            {
                log.Debug("Ожидание загрузки страницы ....");

                while (FindWaiter())
                {
                    
                }

                log.Debug(String.Format("Поиск таблицы на странице. Попытка {0}", i + 1));



                // Если с первого раза мы не получаем таблицу то смотрим есть ли ошибки на странице
                if (i > 0)
                {
                    if (FindErrorInTable())
                        return;
                }



                foreach (IWebElement table in driver.FindElements(By.TagName("table"))?.Where(a => a.GetAttribute("class").Contains("ng-star-inserted")))
                {
                    
                    // Заголовки таблицы 
                    var thead = table.FindElements(By.TagName("thead")).FirstOrDefault()?.FindElements(By.TagName("th"))?.Select(a => a.Text).ToArray();
                   // Содержание таблицы
                    var tbody = table.FindElements(By.TagName("tbody")).FirstOrDefault()?.FindElements(By.TagName("tr"))?
                        .Select(a => a.FindElements(By.TagName("td"))
                        .Select(t => t.Text).ToArray()).ToArray();
                    
                    if(tbody?.Any(a=>a.Length == thead?.Length) == true)
                    {
                        log.Debug(string.Format("Найдена таблица по ИНН {0}", organization.Inn));

                        organization.Reports = new Organization.Report[tbody.Length];

                        for (int y = 0; y < tbody.Length; y++)
                        {
                            organization.Reports[y] = new Organization.Report();

                            for (int z = 0; z < tbody[y].Length; z++)
                            {
                                var name = thead[z];
                                var value = tbody[y][z];

                                if (string.IsNullOrWhiteSpace(name))
                                    continue;

                                var prop = obj.Where(a => a.title == name)?.Select(a => a.property).FirstOrDefault();
                                if (prop is null)
                                {
                                    log.Debug(string.Format("Не найдено поле {0}", name ));
                                    continue;
                                }

                                log.Debug(string.Format("Сохранение поля {0} значение {1}", name, value));


                                prop.SetValue(organization.Reports[y], value);


                            }
                            
                        }

                    }

                    return;
                }
                System.Threading.Thread.Sleep(1000);
            }

            log.Warn("Ожидание таблицы превысило максимального значения");

        }
    }
}
