using System;
using System.Collections.Generic;
using System.IO;

namespace CybersecurityChatbotGUI
{
    public class MemoryStore
    {
        public string UserName { get; set; } = string.Empty;
        public string FavouriteTopic { get; set; } = string.Empty;
        public bool IsReturningUser { get; set; } = false;

        private Dictionary<string, string> _store = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private string _memoryFolderPath;

        public MemoryStore()
        {
            _memoryFolderPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "SecureNet");
        }

        public void Store(string key, string value)
        {
            if (!string.IsNullOrWhiteSpace(key))
                _store[key] = value;
        }

        public string Recall(string key)
        {
            return _store.TryGetValue(key, out string val) ? val : null;
        }

        public string GetPersonalisedOpener()
        {
            if (!string.IsNullOrWhiteSpace(FavouriteTopic) && !string.IsNullOrWhiteSpace(UserName))
                return "As someone interested in " + FavouriteTopic + ", " + UserName + ", here's something relevant: ";
            if (!string.IsNullOrWhiteSpace(FavouriteTopic))
                return "As someone interested in " + FavouriteTopic + ", here's something relevant: ";
            if (!string.IsNullOrWhiteSpace(UserName))
                return "Great question, " + UserName + "! ";
            return string.Empty;
        }

        public bool LoadUser(string name)
        {
            try
            {
                if (!Directory.Exists(_memoryFolderPath))
                    Directory.CreateDirectory(_memoryFolderPath);

                string filePath = GetFilePath(name);

                if (File.Exists(filePath))
                {
                    string[] lines = File.ReadAllLines(filePath);
                    foreach (string line in lines)
                        if (line.StartsWith("FavouriteTopic="))
                            FavouriteTopic = line.Substring("FavouriteTopic=".Length).Trim();

                    UserName = name;
                    IsReturningUser = true;
                    return true;
                }
            }
            catch { }

            IsReturningUser = false;
            return false;
        }

        public void SaveToFile()
        {
            try
            {
                if (!Directory.Exists(_memoryFolderPath))
                    Directory.CreateDirectory(_memoryFolderPath);

                string filePath = GetFilePath(UserName);

                using (StreamWriter writer = new StreamWriter(filePath, false))
                {
                    writer.WriteLine("UserName=" + UserName);
                    writer.WriteLine("FavouriteTopic=" + FavouriteTopic);
                }
            }
            catch { }
        }

        private string GetFilePath(string name)
        {
            string safeName = name.ToLowerInvariant().Replace(" ", "_");
            return Path.Combine(_memoryFolderPath, safeName + ".txt");
        }
    }
}