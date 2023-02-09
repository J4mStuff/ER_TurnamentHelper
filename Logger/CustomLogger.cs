using System.Diagnostics;
using Serilog;
using Serilog.Events;

namespace Logger;

public class CustomLogger
{
    public CustomLogger()
    {
        var consoleRestriction = Debugger.IsAttached ? LogEventLevel.Debug : LogEventLevel.Warning;
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console(restrictedToMinimumLevel: consoleRestriction)
            .WriteTo.File(Path.Combine("logs", "main.log"), rollingInterval: RollingInterval.Day)
            .CreateLogger();
    }
    
    public void Debug(string message)
    {
        Log.Debug(message);
    }
    
    public void Info(string message)
    {
        Log.Information(message);
    }
    
    public void Warning(string message)
    {
        Log.Warning(message);
    }
    
    public void Error(string message)
    {
        Log.Error(message);
    }
    
    public void Fatal(string message)
    {
        Log.Fatal(message);
    }
}