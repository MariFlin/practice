using PluginSystem;

namespace Plugins;

[PluginLoad(priority: 2, dependencies: new[] { "BaseLoggerPlugin.dll" })]
public class FileLogger : IPluginCommand
{
    public string Name => "FileLogger";
    public void Execute() => Console.WriteLine("[FileLogger] Запись логов в файл включена");
}