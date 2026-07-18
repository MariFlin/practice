using PluginSystem;

namespace Plugins;

[PluginLoad(priority: 1)] 
public class BaseLoggerPlugin : IPluginCommand
{
    public string Name => "BaseLogger";
    public void Execute() => Console.WriteLine("[BaseLogger] Инициализирован");
}