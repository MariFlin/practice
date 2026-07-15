using System;
using System.IO;
using System.Reflection;
using CommandLib;

namespace CommandRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            string dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FileSystemCommands.dll");
            
            if (!File.Exists(dllPath))
            {
                dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "FileSystemCommands", "bin", "Debug", "net8.0", "FileSystemCommands.dll");
            }

            if (!File.Exists(dllPath))
            {
                Console.WriteLine($"Не удалось найти DLL по пути: {dllPath}");
                Console.WriteLine("Убедитесь, что проект FileSystemCommands был собран.");
                return;
            }

            Console.WriteLine($"Загрузка сборки из: {dllPath}\n");
            
            Assembly assembly = Assembly.LoadFrom(dllPath);

            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(ICommand).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                {
                    Console.WriteLine($"[Найден класс команды]: {type.Name}");
                    
                    try 
                    {
                        if (type.Name == "DirectorySizeCommand")
                        {
                            var cmd = (ICommand)Activator.CreateInstance(type, Environment.CurrentDirectory)!;
                            cmd.Execute();
                        }
                        else if (type.Name == "FindFilesCommand")
                        {
                            var cmd = (ICommand)Activator.CreateInstance(type, Environment.CurrentDirectory, "*.cs")!;
                            cmd.Execute();
                        }
                        Console.WriteLine(new string('-', 40));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка при выполнении команды {type.Name}: {ex.Message}");
                    }
                }
            }
        }
    }
}