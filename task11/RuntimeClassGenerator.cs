using System;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace task11;

public static class RuntimeClassGenerator
{
    public static Type GenerateType(string sourceCode)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);

        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
            .Select(a => MetadataReference.CreateFromFile(a.Location));

        references = references.Append(MetadataReference.CreateFromFile(Assembly.GetExecutingAssembly().Location));

        var compilation = CSharpCompilation.Create(
            "RuntimeGeneratedAssembly",
            new[] { syntaxTree },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

        using var ms = new System.IO.MemoryStream();
        var emitResult = compilation.Emit(ms);

        if (!emitResult.Success)
        {
            var errors = string.Join("\n", emitResult.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));
            throw new InvalidOperationException($"Ошибка компиляции динамического класса:\n{errors}");
        }

        ms.Seek(0, System.IO.SeekOrigin.Begin);
        var assembly = Assembly.Load(ms.ToArray());
        
        return assembly.GetType("Calculator") 
               ?? throw new InvalidOperationException("Класс 'Calculator' не найден в скомпилированной сборке.");
    }

    public static ICalculator CreateCalculator(string sourceCode)
    {
        var type = GenerateType(sourceCode);
        
        var instance = Activator.CreateInstance(type) 
                       ?? throw new InvalidOperationException("Не удалось создать экземпляр Calculator");

        return (ICalculator)instance;
    }
}