using System;
using System.Reflection;
using System.Collections.Generic;

namespace task05
{
    public class ClassAnalyzer
    {
        private Type _type;

        public ClassAnalyzer(Type type)
        {
            _type = type  ?? throw new ArgumentNullException(nameof(type));
        }

        public IEnumerable<string> GetPublicMethods()
        {
            return _type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                        .Where(m => !m.IsSpecialName) 
                        .Select(m => m.Name)
                        .Distinct();
        }

        public IEnumerable<string> GetMethodParams(string methodName)
        {
            var method = _type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            
            if (method == null)
            {
                return new List<string>();
            }
            
            var paramNames = method.GetParameters().Select(p => p.Name!);
            
            return paramNames.Append(method.ReturnType.Name);
        }

        public IEnumerable<string> GetAllFields()
        {
            return _type.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                        .Select(f => f.Name);
        }

        public IEnumerable<string> GetProperties()
        {
            return _type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                        .Select(p => p.Name);
        }
        public bool HasAttribute<T>() where T : Attribute
        {
            return _type.GetCustomAttribute<T>() != null;
        }
    }
}