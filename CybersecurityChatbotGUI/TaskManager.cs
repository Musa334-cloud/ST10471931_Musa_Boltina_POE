using System.Collections.Generic;

namespace CybersecurityChatbotGUI
{
    public class TaskManager
    {
        private TaskStorageHelper _storage;
        private ActivityLogger _logger;

        public TaskManager(ActivityLogger logger)
        {
            _storage = new TaskStorageHelper();
            _logger = logger;
        }

        public string AddTask(string title, string description, string reminder)
        {
            _storage.AddTask(title, description, reminder);

            string logEntry = "Task added: '" + title + "'";
            if (!string.IsNullOrWhiteSpace(reminder))
                logEntry += " (Reminder: " + reminder + ")";
            _logger.Log(logEntry);

            string response = "✅ Task added: '" + title + "'\n" + description;
            if (!string.IsNullOrWhiteSpace(reminder))
                response += "\n⏰ Reminder set: " + reminder;

            return response;
        }

        public List<CyberTask> GetAllTasks()
        {
            return _storage.LoadTasks();
        }

        public void MarkAsComplete(int id)
        {
            var tasks = _storage.LoadTasks();
            foreach (var task in tasks)
            {
                if (task.Id == id)
                {
                    _storage.MarkAsComplete(id);
                    _logger.Log("Task marked complete: '" + task.Title + "'");
                    break;
                }
            }
        }

        public void DeleteTask(int id)
        {
            var tasks = _storage.LoadTasks();
            foreach (var task in tasks)
            {
                if (task.Id == id)
                {
                    _storage.DeleteTask(id);
                    _logger.Log("Task deleted: '" + task.Title + "'");
                    break;
                }
            }
        }
    }
}
