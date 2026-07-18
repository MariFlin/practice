using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using PluginSystem;

namespace PluginLoader;

class Program
{
    static void Main(string[] args)
    {
        string pluginsDir = args.Length > 0 ? args[0] : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins");

        if (!Directory.Exists(pluginsDir))
        {
            Console.WriteLine($"Папка плагинов не найдена: {pluginsDir}");
            return;
        }

        var dllFiles = Directory.GetFiles(pluginsDir, "*.dll", SearchOption.TopDirectoryOnly);
        Console.WriteLine($"Найдено библиотек: {dllFiles.Length}\n");

        var pluginMetadata = new List<PluginInfo>();
        var loadedAssemblies = new Dictionary<string, Assembly>();

        foreach (var dll in dllFiles)
        {
            try
            {
                var asm = Assembly.LoadFrom(dll);
                loadedAssemblies[Path.GetFileName(dll)] = asm;

                foreach (var type in asm.GetTypes().Where(t => 
                    typeof(IPluginCommand).IsAssignableFrom(t) && 
                    !t.IsInterface && !t.IsAbstract))
                {
                    var attr = type.GetCustomAttribute<PluginLoadAttribute>();
                    if (attr != null)
                    {
                        pluginMetadata.Add(new PluginInfo
                        {
                            Type = type,
                            DllName = Path.GetFileName(dll),
                            Priority = attr.Priority,
                            Dependencies = attr.Dependencies ?? Array.Empty<string>()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"️Ошибка загрузки {Path.GetFileName(dll)}: {ex.Message}");
            }
        }

        var sortedPlugins = TopologicalSort(pluginMetadata, loadedAssemblies.Keys.ToHashSet());

        if (sortedPlugins == null)
        {
            Console.WriteLine("Обнаружен цикл в зависимостях плагинов!");
            return;
        }

        Console.WriteLine("Запуск плагинов");
        foreach (var plugin in sortedPlugins)
        {
            try
            {
                var instance = (IPluginCommand)Activator.CreateInstance(plugin.Type)!;
                Console.WriteLine($"- [{plugin.Type.Name}] Priority={plugin.Priority}");
                instance.Execute();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка выполнения {plugin.Type.Name}: {ex.Message}");
            }
        }
    }

    static List<PluginInfo>? TopologicalSort(List<PluginInfo> plugins, HashSet<string> availableDlls)
    {
        var inDegree = plugins.ToDictionary(p => p.Type, _ => 0);
        var graph = plugins.ToDictionary(p => p.Type, _ => new List<Type>());

        foreach (var plugin in plugins)
        {
            foreach (var depDllName in plugin.Dependencies)
            {
                var dependencyPlugins = plugins.Where(p => 
                    p.DllName.Equals(depDllName, StringComparison.OrdinalIgnoreCase));
                
                foreach (var depPlugin in dependencyPlugins)
                {
                    graph[depPlugin.Type].Add(plugin.Type);
                    inDegree[plugin.Type]++;
                }
            }
        }

        var queue = new Queue<Type>(
            inDegree.Where(kv => kv.Value == 0)
                    .OrderBy(kv => plugins.First(p => p.Type == kv.Key).Priority)
                    .Select(kv => kv.Key));

        var result = new List<PluginInfo>();

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            result.Add(plugins.First(p => p.Type == current));

            foreach (var neighbor in graph[current])
            {
                inDegree[neighbor]--;
                if (inDegree[neighbor] == 0)
                {
                    queue.Enqueue(neighbor);
                }
            }
        }

        return result.Count == plugins.Count ? result : null;
    }

    record PluginInfo
    {
        public required Type Type { get; init; }
        public required string DllName { get; init; }
        public int Priority { get; init; }
        public string[] Dependencies { get; init; } = Array.Empty<string>();
    }
}