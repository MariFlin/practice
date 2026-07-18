using PluginSystem;

namespace BaseLoggerPlugin;

[PluginLoad(priority: 1)]
public class Logger : IPluginCommand
{
    public string Name => "BaseLogger";
    public void Execute() => Console.WriteLine("[BaseLogger] Система логирования активна");
}