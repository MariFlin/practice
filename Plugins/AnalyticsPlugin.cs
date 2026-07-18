using PluginSystem;

namespace Plugins;

[PluginLoad(priority: 3, dependencies: new[] { "Plugins.dll" })]
public class AnalyticsPlugin : IPluginCommand
{
    public string Name => "Analytics";
    public void Execute() => Console.WriteLine("[Analytics] Сбор метрик запущен");
}