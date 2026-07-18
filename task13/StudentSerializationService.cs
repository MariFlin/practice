using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace task13;

public static class StudentSerializationService
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };

    public static string Serialize(Student student)
    {
        if (student == null) throw new ArgumentNullException(nameof(student));
        return JsonSerializer.Serialize(student, Options);
    }

    public static Student Deserialize(string json)
    {
        if (string.IsNullOrWhiteSpace(json)) 
            throw new ArgumentException("JSON не может быть пустым", nameof(json));

        var student = JsonSerializer.Deserialize<Student>(json, Options);
        
        if (student == null) 
            throw new JsonException("Не удалось десериализовать объект Student");

        Validate(student);
        
        return student;
    }

    public static void SaveToFile(Student student, string filePath)
    {
        var json = Serialize(student);
        File.WriteAllText(filePath, json);
    }

    public static Student LoadFromFile(string filePath)
    {
        if (!File.Exists(filePath)) 
            throw new FileNotFoundException($"Файл не найден: {filePath}");

        var json = File.ReadAllText(filePath);
        return Deserialize(json);
    }

    private static void Validate(Student student)
    {
        if (string.IsNullOrWhiteSpace(student.FirstName))
            throw new JsonException("Поле FirstName обязательно для заполнения");
            
        if (string.IsNullOrWhiteSpace(student.LastName))
            throw new JsonException("Поле LastName обязательно для заполнения");
            
        if (student.BirthDate == default)
            throw new JsonException("Поле BirthDate должно быть установлено");
    }
}