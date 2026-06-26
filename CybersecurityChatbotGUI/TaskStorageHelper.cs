using System;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace CybersecurityChatbotGUI
{
    // Attribution: Newtonsoft.Json - https://www.newtonsoft.com/json
    public class TaskStorageHelper
    {
        private const string FilePath = "tasks.json";

        public List<CyberTask> LoadTasks()
        {
            try
            {
                if (!File.Exists(FilePath))
                    return new List<CyberTask>();

                string json = File.ReadAllText(FilePath);
                return JsonConvert.DeserializeObject<List<CyberTask>>(json) ?? new List<CyberTask>();
            }
            catch
            {
                return new List<CyberTask>();
            }
        }

        public void SaveTasks(List<CyberTask> tasks)
        {
            try
            {
                string json = JsonConvert.SerializeObject(tasks, Formatting.Indented);
                File.WriteAllText(FilePath, json);
            }
            catch { }
        }

        public void AddTask(string title, string description, string reminder)
        {
            var tasks = LoadTasks();
            int newId = tasks.Count > 0 ? tasks[tasks.Count - 1].Id + 1 : 1;

            tasks.Add(new CyberTask
            {
                Id = newId,
                Title = title,
                Description = description,
                Reminder = reminder,
                IsComplete = false,
                CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm")
            });

            SaveTasks(tasks);
        }

        public void MarkAsComplete(int id)
        {
            var tasks = LoadTasks();
            foreach (var task in tasks)
                if (task.Id == id) { task.IsComplete = true; break; }
            SaveTasks(tasks);
        }

        public void DeleteTask(int id)
        {
            var tasks = LoadTasks();
            tasks.RemoveAll(t => t.Id == id);
            SaveTasks(tasks);
        }
    }
}