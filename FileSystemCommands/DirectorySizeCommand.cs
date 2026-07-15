using System;
using System.IO;
using System.Linq;
using CommandLib;

namespace FileSystemCommands
{
    public class DirectorySizeCommand : ICommand
    {
        private readonly string _directoryPath;
        
        public long TotalSize { get; private set; }

        public DirectorySizeCommand(string directoryPath)
        {
            _directoryPath = directoryPath ?? throw new ArgumentNullException(nameof(directoryPath));
        }

        public void Execute()
        {
            if (!Directory.Exists(_directoryPath))
            {
                throw new DirectoryNotFoundException($"Каталог не найден: {_directoryPath}");
            }

            var dirInfo = new DirectoryInfo(_directoryPath);
            TotalSize = dirInfo.GetFiles("*", SearchOption.AllDirectories).Sum(fi => fi.Length);
            
            Console.WriteLine($"Размер каталога '{_directoryPath}': {TotalSize} байт");
        }
    }
}