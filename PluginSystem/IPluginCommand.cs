namespace PluginSystem;

public interface IPluginCommand
{
    string Name { get; }
    void Execute();
}