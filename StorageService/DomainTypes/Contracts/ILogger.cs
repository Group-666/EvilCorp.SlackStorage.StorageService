using System;

namespace DomainTypes.Contracts
{
    public interface ILogger
    {
        void Log(String message, LogLevel level);
    }
}
