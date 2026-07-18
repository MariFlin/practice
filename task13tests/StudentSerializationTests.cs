using Xunit;
using System;
using System.IO;
using System.Text.Json;
using task13;

namespace task13tests;

public class StudentSerializationTests : IDisposable
{
    private readonly string _testFilePath;

    public StudentSerializationTests()
    {
        _testFilePath = Path.Combine(Path.GetTempPath(), $"student_test_{Guid.NewGuid()}.json");
    }

    public void Dispose()
    {
        if (File.Exists(_testFilePath)) File.Delete(_testFilePath);
    }

    private Student CreateValidStudent() => new()
    {
        FirstName = "Иван",
        LastName = "Петров",
        BirthDate = new DateTime(2000, 5, 15),
        Grades = new() 
        { 
            new Subject { Name = "Математика", Grade = 5 },
            new Subject { Name = "Физика", Grade = 4 }
        }
    };

    [Fact]
    public void Serialize_ValidStudent_ReturnsValidJson()
    {
        var student = CreateValidStudent();
        var json = StudentSerializationService.Serialize(student);

        Assert.NotNull(json);
        Assert.Contains("firstName", json);
        Assert.Contains("lastName", json);
        Assert.Contains("birthDate", json);
        Assert.Contains("grades", json);
    }

    [Fact]
    public void Serialize_NullGrades_IgnoresNullProperty()
    {
        var student = new Student 
        { 
            FirstName = "Анна", 
            LastName = "Сидорова", 
            BirthDate = DateTime.Now,
            Grades = null 
        };
        
        var json = StudentSerializationService.Serialize(student);
        
        Assert.DoesNotContain("grades", json);
    }

    [Fact]
    public void Deserialize_ValidJson_ReturnsStudent()
    {
        var original = CreateValidStudent();
        var json = StudentSerializationService.Serialize(original);
        
        var restored = StudentSerializationService.Deserialize(json);
        
        Assert.Equal(original.FirstName, restored.FirstName);
        Assert.Equal(original.LastName, restored.LastName);
        Assert.Equal(original.BirthDate, restored.BirthDate);
        Assert.Equal(2, restored.Grades?.Count);
    }

    [Fact]
    public void Deserialize_EmptyJson_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => 
            StudentSerializationService.Deserialize(""));
    }

    [Fact]
    public void Deserialize_MissingRequiredFields_ThrowsJsonException()
    {
        var invalidJson = @"{ ""firstName"": """", ""lastName"": """" }";
        
        Assert.Throws<JsonException>(() => 
            StudentSerializationService.Deserialize(invalidJson));
    }

    [Fact]
    public void SaveAndLoad_FileRoundTrip_PreservesData()
    {
        var original = CreateValidStudent();
        
        StudentSerializationService.SaveToFile(original, _testFilePath);
        var loaded = StudentSerializationService.LoadFromFile(_testFilePath);
        
        Assert.Equal(original.FirstName, loaded.FirstName);
        Assert.Equal(original.LastName, loaded.LastName);
        Assert.Equal(original.BirthDate, loaded.BirthDate);
        Assert.Equal(original.Grades?.Count, loaded.Grades?.Count);
    }

    [Fact]
    public void LoadFromFile_NonExistentFile_ThrowsFileNotFoundException()
    {
        Assert.Throws<FileNotFoundException>(() => 
            StudentSerializationService.LoadFromFile("nonexistent.json"));
    }
}
