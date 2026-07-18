using Xunit;
using System;
using System.IO;
using System.Reflection;
using System.Linq;

namespace task09tests
{
    public class MetadataReaderTests
    {
        private string CreateTestAssembly()
        {
            var task07DllPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory, 
                "..", "..", "..", "..", "..", 
                "task07", "bin", "Debug", "net8.0", "task07.dll");
            
            if (!File.Exists(task07DllPath))
            {
                task07DllPath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory, 
                    "task07.dll");
            }

            return task07DllPath;
        }

        [Fact]
        public void AssemblyLoadFrom_ValidDll_DoesNotThrow()
        {
            var dllPath = CreateTestAssembly();
            
            var ex = Record.Exception(() => Assembly.LoadFrom(dllPath));
            
            Assert.Null(ex);
        }

        [Fact]
        public void GetTypes_ReturnsAtLeastOneType()
        {
            var dllPath = CreateTestAssembly();
            var assembly = Assembly.LoadFrom(dllPath);
            
            Type[] types;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types.OfType<Type>().ToArray();
            }
            
            Assert.NotEmpty(types);
        }

        [Fact]
        public void GetTypes_FindsSampleClass()
        {
            var dllPath = CreateTestAssembly();
            var assembly = Assembly.LoadFrom(dllPath);
            
            Type[] types;
            try { types = assembly.GetTypes(); }
            catch (ReflectionTypeLoadException ex) { types = ex.Types.OfType<Type>().ToArray(); }
            
            var sampleClass = types.FirstOrDefault(t => t.Name == "SampleClass");
            Assert.NotNull(sampleClass);
            Assert.True(sampleClass!.IsClass);
        }

        [Fact]
        public void GetMethods_ReturnsMethodsForClass()
        {
            var dllPath = CreateTestAssembly();
            var assembly = Assembly.LoadFrom(dllPath);
            
            Type[] types;
            try { types = assembly.GetTypes(); }
            catch (ReflectionTypeLoadException ex) { types = ex.Types.OfType<Type>().ToArray(); }
            
            var sampleClass = types.FirstOrDefault(t => t.Name == "SampleClass");
            Assert.NotNull(sampleClass);
            
            var methods = sampleClass!.GetMethods(
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            
            Assert.NotEmpty(methods);
            Assert.Contains(methods, m => m.Name == "TestMethod");
        }

        [Fact]
        public void GetConstructors_ReturnsConstructorsForClass()
        {
            var dllPath = CreateTestAssembly();
            var assembly = Assembly.LoadFrom(dllPath);
            
            Type[] types;
            try { types = assembly.GetTypes(); }
            catch (ReflectionTypeLoadException ex) { types = ex.Types.OfType<Type>().ToArray(); }
            
            var sampleClass = types.FirstOrDefault(t => t.Name == "SampleClass");
            Assert.NotNull(sampleClass);
            
            var constructors = sampleClass!.GetConstructors(
                BindingFlags.Public | BindingFlags.Instance);
            
            Assert.NotEmpty(constructors);
        }

        [Fact]
        public void GetParameters_ReturnsParametersForMethod()
        {
            var dllPath = CreateTestAssembly();
            var assembly = Assembly.LoadFrom(dllPath);
            
            Type[] types;
            try { types = assembly.GetTypes(); }
            catch (ReflectionTypeLoadException ex) { types = ex.Types.OfType<Type>().ToArray(); }
            
            var sampleClass = types.FirstOrDefault(t => t.Name == "SampleClass");
            Assert.NotNull(sampleClass);
            
            var methodWithoutParams = sampleClass!.GetMethod("TestMethod");
            Assert.NotNull(methodWithoutParams);
            
            var parameters = methodWithoutParams!.GetParameters();
            Assert.Empty(parameters);
        }

        [Fact]
        public void GetCustomAttributes_ReturnsAttributesForClass()
        {
            var dllPath = CreateTestAssembly();
            var assembly = Assembly.LoadFrom(dllPath);
            
            Type[] types;
            try { types = assembly.GetTypes(); }
            catch (ReflectionTypeLoadException ex) { types = ex.Types.OfType<Type>().ToArray(); }
            
            var sampleClass = types.FirstOrDefault(t => t.Name == "SampleClass");
            Assert.NotNull(sampleClass);
            
            var attributes = sampleClass!.GetCustomAttributesData();
            
            Assert.NotEmpty(attributes);
            Assert.True(attributes.Count >= 2);
        }

        [Fact]
        public void LoadFrom_NonExistentFile_ThrowsFileNotFoundException()
        {
            var nonExistentPath = "C:\\NonExistent\\File.dll";
            
            Assert.Throws<FileNotFoundException>(() => Assembly.LoadFrom(nonExistentPath));
        }

        [Fact]
        public void LoadFrom_InvalidFile_ThrowsBadImageFormatException()
        {
            var tempFile = Path.GetTempFileName();
            File.WriteAllText(tempFile, "This is not a DLL");
            
            try
            {
                Assert.Throws<BadImageFormatException>(() => Assembly.LoadFrom(tempFile));
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void BindingFlags_IncludeNonPublic_FindsNoPrivateMethods()
        {
            var dllPath = CreateTestAssembly();
            var assembly = Assembly.LoadFrom(dllPath);
            
            Type[] types;
            try { types = assembly.GetTypes(); }
            catch (ReflectionTypeLoadException ex) { types = ex.Types.OfType<Type>().ToArray(); }
            
            var sampleClass = types.FirstOrDefault(t => t.Name == "SampleClass");
            Assert.NotNull(sampleClass);
            
            var privateMethods = sampleClass!.GetMethods(
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            
            Assert.Empty(privateMethods);
        }

        [Fact]
        public void GetProperties_ReturnsPropertiesForClass()
        {
            var dllPath = CreateTestAssembly();
            var assembly = Assembly.LoadFrom(dllPath);
            
            Type[] types;
            try { types = assembly.GetTypes(); }
            catch (ReflectionTypeLoadException ex) { types = ex.Types.OfType<Type>().ToArray(); }
            
            var sampleClass = types.FirstOrDefault(t => t.Name == "SampleClass");
            Assert.NotNull(sampleClass);
            
            var properties = sampleClass!.GetProperties(
                BindingFlags.Public | BindingFlags.Instance);
            
            Assert.NotEmpty(properties);
            Assert.Contains(properties, p => p.Name == "Number");
        }
    }
}