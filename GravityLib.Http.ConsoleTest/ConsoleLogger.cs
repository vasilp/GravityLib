using Microsoft.Extensions.Logging;

namespace GravityLib.Http.ConsoleTest;

public class ConsoleLogger : ILogger
{
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState message, Exception exception, Func<TState, Exception, string> formatter)
    {
        Console.WriteLine($"{logLevel} {eventId} {message} {exception}");
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        return null;
    }
}