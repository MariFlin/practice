using System;

namespace PluginSystem;

[AttributeUsage(AttributeTargets.Class)]
public class PluginLoadAttribute : Attribute
{
    public int Priority { get; }
    public string[] Dependencies { get; }
    
    public PluginLoadAttribute(int priority = 0, params string[] dependencies)
    {
        Priority = priority;
        Dependencies = dependencies;
    }
}