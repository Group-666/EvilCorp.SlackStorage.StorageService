using System;
using System.Collections.Generic;
using System.Text;

namespace DomainTypes.Contracts
{
    public class ConsoleFactory
    {
        public static ConsoleLogger CreateConsoleLogger()
        {
            //TODO get the log level from configuration files.
            ConsoleLogger consoleLogger = new ConsoleLogger(LogLevel.Trace);

            return consoleLogger;
        }
        public static HttpServiceLogger CreateLogger()
        {
            //Get connection strings from a configuration file. 
            HttpServiceLogger logger = new HttpServiceLogger("localhost:49752", "StorageService", LogLevel.Trace);

            return logger;
        }
    }
}
