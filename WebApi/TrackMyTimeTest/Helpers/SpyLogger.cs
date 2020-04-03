using Microsoft.Extensions.Logging;
using System;

namespace TrackMyTimeTest.Helpers
{
    public class SpyLogger<T> : ILogger<T>
    {
        public bool LoggerWasCalled { get; private set; }
        public LogLevel LogLevel { get; private set; }
        public Exception Exception { get; private set; }
        public string Message { get; private set; }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            LogLevel = logLevel;
            Exception = exception;
            Message = state.ToString();
            LoggerWasCalled = true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            throw new NotImplementedException();
        }
    }
}
