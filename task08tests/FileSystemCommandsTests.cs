using Xunit;
using System.IO;
using FileSystemCommands;

public class FileSystemCommandsTests
{
    [Fact]
    public void DirectorySizeCommand_ShouldCalculateSize()
    {
        var testDir = Path.Combine(Path.GetTempPath(), "TestDirSize");
        Directory.CreateDirectory(testDir);
        File.WriteAllText(Path.Combine(testDir, "test1.txt"), "Hello"); 
        File.WriteAllText(Path.Combine(testDir, "test2.txt"), "World");

        var command = new DirectorySizeCommand(testDir);
        command.Execute();  // Проверяем, что не возникает исключений

        Assert.True(command.TotalSize >= 10, $"Ожидалось >= 10 байт, получено {command.TotalSize}");
        
        Directory.Delete(testDir, true);
    }

    [Fact]
    public void FindFilesCommand_ShouldFindMatchingFiles()
    {
        var testDir = Path.Combine(Path.GetTempPath(), "TestDirFind");
        Directory.CreateDirectory(testDir);
        File.WriteAllText(Path.Combine(testDir, "file1.txt"), "Text");
        File.WriteAllText(Path.Combine(testDir, "file2.log"), "Log");

        var command = new FindFilesCommand(testDir, "*.txt");

        command.Execute();  // Должен найти 1 файл

        Assert.Single(command.FoundFiles);
        Assert.Contains("file1.txt", command.FoundFiles[0]);
        
        Directory.Delete(testDir, true);
    }

    [Fact]
    public void DirectorySizeCommand_NullPath_ThrowsException()
    {
        Assert.Throws<System.ArgumentNullException>(() => new DirectorySizeCommand(null!));
    }

    [Fact]
    public void FindFilesCommand_NullPattern_ThrowsException()
    {
        Assert.Throws<System.ArgumentNullException>(() => new FindFilesCommand("C:\\", null!));
    }
}