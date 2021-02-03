using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ParserWeb
{

    class Program
    {
        // путь к файлу ИНН клиентов
        private static string pathFile = "INN.txt";
        private static Organization[] organizations;


        private static void PrintMessageTitle(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("{0} {1}", DateTime.Now, message);
            Console.ResetColor();
        }
        private static void PrintMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("{0} {1}", DateTime.Now, message);
            Console.ResetColor();
        }

        static void Main(string[] args)
        {         
            ILog log = new Loger();   

            try
            {
                
                log.Info("Запуск программы");

                if (!File.Exists(pathFile))
                    throw new Exception(string.Format("Нет файла {0} в каталоге программы", pathFile));



                organizations = File.ReadAllLines(pathFile, Encoding.GetEncoding("windows-1251"))//
                    ?.Select( a=> a.Split(new char[] { '\t' }))
                    ?.Where(a=>a.Length == 2)
                    ?.Select(a=> new Organization() { Name = a[0], Inn = a[1] }).ToArray();

                if(organizations is null || organizations.Length == 0)
                    throw new Exception(string.Format("Неверный формат файла или файл {0} пуст ", pathFile));

                var processing = new Processing(organizations, log);
                processing.Message += (msg) => PrintMessage(msg);

                processing.StartProcessing();

            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                log.Error(string.Format("Ошибка!!! {0}", ex.Message));
                Console.ResetColor();
            }
            finally
            {
                log.Info("Остановка программы");
            }

            Console.ReadKey();
            return;


        }
    }
}
