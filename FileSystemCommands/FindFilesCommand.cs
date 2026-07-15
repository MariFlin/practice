using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLib;

namespace FileSystemCommands
{
    public class FindFilesCommand : ICommand
    {
        private readonly string _directoryPath;
        private readonly string _searchPattern;
        
        public List<string> FoundFiles { get; private set; } = new List<string>();

        public FindFilesCommand(string directoryPath, string searchPattern)
        {
            _directoryPath = directoryPath ?? throw new ArgumentNullException(nameof(directoryPath));
            _searchPattern = searchPattern ?? throw new ArgumentNullException(nameof(searchPattern));
        }

        public void Execute()
        {
            if (!Directory.Exists(_directoryPath))
            {
                throw new DirectoryNotFoundException($"Каталог не найден: {_directoryPath}");
            }

            FoundFiles = Directory.GetFiles(_directoryPath, _searchPattern, SearchOption.AllDirectories).ToList();
            
            Console.WriteLine($"Найдено файлов по маске '{_searchPattern}': {FoundFiles.Count}");
            foreach (var file in FoundFiles)
            {
                Console.WriteLine($" - {file}");
            }
        }
    }
}