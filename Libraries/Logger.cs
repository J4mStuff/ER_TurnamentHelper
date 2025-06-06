using System.Diagnostics;
using Serilog;
using Serilog.Events;
namespace Libraries;

public class Logger
{
    public Logger()
    {
        var consoleRestriction = Debugger.IsAttached ? LogEventLevel.Debug : LogEventLevel.Warning;
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console(restrictedToMinimumLevel: consoleRestriction)
            .WriteTo.File(Path.Combine("logs", "main.log"), rollingInterval: RollingInterval.Day)
            .CreateLogger();
    }
}