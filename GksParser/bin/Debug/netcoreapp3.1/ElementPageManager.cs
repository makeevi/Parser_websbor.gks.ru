using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GksParser
{
    class ElementPageManager
    {
        private IWebDriver driver;
        private IWebElement entityInn;
        private IWebElement entityButton;

        public string TextEntityInn
        {
            get => entityInn.GetAttribute("value");
            set => SendKeyPage(entityInn, value);
        }

        public ElementPageManager(IWebDriver driver)
        {
            this.driver = driver ?? throw new ArgumentNullException(nameof(driver));

            var entitys = FindElementPage(driver);
            entityInn = entitys.entityInn;
            entityButton = entitys.entityButton;
        }

        // Клик по кнопке найти
        public void ButtonClick() => entityButton.Click();

        // Поиск элементов на странице
        private (IWebElement entityInn, IWebElement entityButton) FindElementPage(IWebDriver driver)
        {
            for (int i = 0; i < 10; i++)
            {
                IWebElement entityInn = driver.FindElement(By.Id("inn"));// Поиск поля ИНН на странице
                IWebElement entityButton = driver.FindElements(By.XPath("//button"))?.Where(a => a.Text == "Получить").FirstOrDefault();// Поиск кнопки "Получить" на странице

                if (entityInn != null && entityButton != null)
                    return (entityInn, entityButton);

                System.Threading.Thread.Sleep(1000);
            }

            throw new Exception("Ошибка при загрузке страницы");
        }

        // Устанавливаем значение полю
        private void SendKeyPage(IWebElement webElement, string text)
        {
            for (int i = 0; i < 10; i++)
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

        public void FindTableInPage(Organization organization)
        {
            for (int i = 0; i < 10; i++)
            {
                bool flag = false;

                foreach (IWebElement table in driver.FindElements(By.TagName("table"))?.Where(a => a.GetAttribute("class").Contains("ng-star-inserted")))
                {
                    var thead = table.FindElements(By.TagName("thead")).FirstOrDefault()?.FindElements(By.TagName("th"))?.Select(a => a.Text).ToArray();
                    var tbody = table.FindElements(By.TagName("tbody")).FirstOrDefault()?.FindElements(By.TagName("tr"))?
                        .Select(a => a.FindElements(By.TagName("td"))
                        .Select(t => t.Text).ToArray()).ToArray();
                    
                    if(tbody?.Any(a=>a.Length == thead.FirstOrDefault()?.Length) == true)
                    {
                        organization.Reports = new Organization.Report[tbody.Length];

                        for (int y = 0; y < tbody.Length; y++)
                        {
                            organization.Reports[y] = new Organization.Report();

                            
                        }



                    }


                    







                }
                if (flag)
                    break;
                System.Threading.Thread.Sleep(1000);
            }

        }
    }
}
