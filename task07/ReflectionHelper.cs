using System;
using System.Reflection;

namespace task07
{
    public static class ReflectionHelper
    {
        public static void PrintTypeInfo(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            Console.WriteLine($"Анализ типа: {type.Name}");

            var classDisplayName = type.GetCustomAttribute<DisplayNameAttribute>();
            if (classDisplayName != null)
            {
                Console.WriteLine($"Отображаемое имя класса: {classDisplayName.DisplayName}");
            }

            var classVersion = type.GetCustomAttribute<VersionAttribute>();
            if (classVersion != null)
            {
                Console.WriteLine($"Версия класса: {classVersion.Major}.{classVersion.Minor}");
            }

            Console.WriteLine("\nМетоды:");
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (var method in methods)
            {
                var methodAttr = method.GetCustomAttribute<DisplayNameAttribute>();
                if (methodAttr != null)
                {
                    Console.WriteLine($"  - {method.Name}: {methodAttr.DisplayName}");
                }
            }

            Console.WriteLine("\nСвойства:");
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in properties)
            {
                var propAttr = prop.GetCustomAttribute<DisplayNameAttribute>();
                if (propAttr != null)
                {
                    Console.WriteLine($"  - {prop.Name}: {propAttr.DisplayName}");
                }
            }
        }
    }
}