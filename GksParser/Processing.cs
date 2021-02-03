using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GksParser
{
    class Processing
    {
        public event Action<string> Message;
        private Organization[] organizations;


        public Processing(Organization[] organizations)
        {
            this.organizations = organizations ?? throw new ArgumentNullException(nameof(organizations)); ;
        }


        // Поиск элементов на странице.


        //Попытка установки значения полю на странице


        public void StartProcessing()
        {
            try
            {

                using (IWebDriver driver = new OpenQA.Selenium.Chrome.ChromeDriver())
                {
                    Message?.Invoke("Запуск браузера");
                    System.Threading.Thread.Sleep(3000);
                    driver.Navigate().GoToUrl("https://websbor.gks.ru/webstat/#!/gs/statistic-codes");
                    System.Threading.Thread.Sleep(3000);


                    ElementPageManager pageManager = new ElementPageManager(driver);


                    foreach (var organization in organizations)
                    {
                        Message?.Invoke(string.Format("Установка значение полю ИНН  {0}", organization.Inn));
                        pageManager.TextEntityInn = organization.Inn;
                        pageManager.ButtonClick();
                        pageManager.FindTableInPage(organization);

                        //SendKeyPage(webElements.entityInn, organization.Inn);
                        //webElements.entityButton.Click();



                        //if (element is null)
                        //{
                        //    errorStep = -1;
                        //    throw new Exception("Не найден элемент на странице. Работа программы невозможна");
                        //}
                        //element.Clear();
                        //element.SendKeys(organization.Inn);
                        //btn.Click();

                        System.Threading.Thread.Sleep(1000);

                        //Actions actions = new Actions(browser);
                        // actions.MoveToElement(btn.FirstOrDefault()).Build();//.Perform();
                        // btn.FirstOrDefault().Click();


                        
                    }

                   // System.Threading.Thread.Sleep(5000000);



                }

            }
            catch (Exception ex)
            {
                //errorStep--;
                //if (errorStep < 0)
                    throw ex;
            }
        }

        
    }
}
