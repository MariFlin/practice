using System;
using System.IO;
using System.Reflection;
using System.Linq;

namespace MetadataReader
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Ошибка: Не указан путь к библиотеке.");
                Console.WriteLine("Использование: dotnet run -- <путь_к_библиотеке.dll>");
                return;
            }

            string dllPath = args[0];

            if (!File.Exists(dllPath))
            {
                Console.WriteLine($"Ошибка: Файл не найден: {dllPath}");
                return;
            }

            try
            {
                Assembly assembly = Assembly.LoadFrom(dllPath);
                Console.WriteLine($"Метаданные сборки: {assembly.GetName().Name}\n");

                Type[] types;
                try
                {
                    types = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    Console.WriteLine("Не удалось загрузить некоторые типы:");
                    
                    foreach (var loaderEx in ex.LoaderExceptions.OfType<Exception>())
                    {
                        Console.WriteLine($"- {loaderEx.Message}");
                    }
                    
                    types = ex.Types.OfType<Type>().ToArray();
                }

                foreach (Type type in types)
                {
                    if (type.IsClass)
                    {
                        Console.WriteLine($"Класс: {type.FullName}");

                        PrintAttributes(type.GetCustomAttributesData(), "класса");

                        var constructors = type.GetConstructors(
                            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
                        );
                        foreach (var ctor in constructors)
                        {
                            Console.WriteLine($"Конструктор: {type.Name}");
                            PrintAttributes(ctor.GetCustomAttributesData(), "конструктора");
                            PrintParameters(ctor.GetParameters());
                        }

                        var methods = type.GetMethods(
                            BindingFlags.Public | 
                            BindingFlags.NonPublic | 
                            BindingFlags.Instance | 
                            BindingFlags.DeclaredOnly
                        );
                        foreach (var method in methods)
                        {
                            Console.WriteLine($"  [МЕТОД]: {method.Name} (возвращает {method.ReturnType.Name})");
                            PrintAttributes(method.GetCustomAttributesData(), "метода");
                            PrintParameters(method.GetParameters());
                        }

                        Console.WriteLine(new string('-', 50));
                    }
                }
            }
            catch (BadImageFormatException)
            {
                Console.WriteLine("Ошибка: Файл не является корректной .NET сборкой");
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"Ошибка: Файл не найден - {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Непредвиденная ошибка: {ex.Message}");
            }
        }

        static void PrintAttributes(IList<CustomAttributeData> attributes, string targetName)
        {
            if (attributes.Count > 0)
            {
                Console.WriteLine($"Атрибуты {targetName}:");
                foreach (var attr in attributes)
                {
                    Console.WriteLine($"- {attr.AttributeType.Name}");
                }
            }
        }

        static void PrintParameters(ParameterInfo[] parameters)
        {
            if (parameters.Length > 0)
            {
                Console.WriteLine("Параметры:");
                foreach (var param in parameters)
                {
                    Console.WriteLine("Тип: {param.ParameterType.Name}, Имя: {param.Name}");
                }
            }
            else
            {
                Console.WriteLine("Параметры: нет");
            }
        }
    }
}