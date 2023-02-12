using System.Data;
using System.Text.Json;
using Logger;

namespace Helpers;

public class StupidClone
{
    private readonly CustomLogger _logger;

    public StupidClone()
    {
        _logger = new CustomLogger();
    }
    
    public T PerformStupidClone<T>(T toClone) 
    {
        var str = JsonSerializer.Serialize(toClone);
        var output = JsonSerializer.Deserialize<T>(str);

        if (output != null) return output;
        
        const string message = "Tried to clone the object which resulted in a null exception";
        _logger.Fatal(message);
        throw new NoNullAllowedException(message);
    }
}