using System;
using System.Collections.Generic;
using System.Text;

namespace DomainTypes.Contracts
{
    public class ConsoleLoggerFactory
    {
        public static ConsoleLogger CreateConsoleLogger()
        {
            //TODO get the log level from configuration files.
            ConsoleLogger consoleLogger = new ConsoleLogger(LogLevel.Trace);

            return consoleLogger;
        }
    }
}
