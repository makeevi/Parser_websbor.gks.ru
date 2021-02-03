using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserWeb
{
    class Loger : ILog
    {
        public event DelegateEventLog EventMessage;

        public void Debug(string message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("{0} {1}", DateTime.Now, message);
            Console.ResetColor();
        }

        public void Error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("{0} {1}", DateTime.Now, message);
            Console.ResetColor();
        }

        public void Error(Exception exception, string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("{0} {1}", DateTime.Now, message);
            Console.ResetColor();
        }

        public void Error(Exception exception)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("{0} {1}", DateTime.Now, exception.Message);
            Console.ResetColor();
        }

        public void Info(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("{0} {1}", DateTime.Now, message);
            Console.ResetColor();
        }

        public void Warn(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("{0} {1}", DateTime.Now, message);
            Console.ResetColor();
        }
    }
}
