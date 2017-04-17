using System;
using System.Collections.Generic;
using System.Text;

namespace DomainTypes.Contracts
{
    public class ConsoleLogger : ILogger
    {
        private int _logLevel;
        public ConsoleLogger(LogLevel logLevel)
        {
            _logLevel = (int)logLevel;
        }
        public void Log(string message, LogLevel level)
        {
            if ((int)level < _logLevel)
                return;

            Console.WriteLine(message);
        }
    }
}
