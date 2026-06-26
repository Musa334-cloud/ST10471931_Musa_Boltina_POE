using System;
using System.Collections.Generic;

namespace CybersecurityChatbotGUI
{
    public class ActivityLogger
    {
        private List<string> _log = new List<string>();

        public void Log(string action)
        {
            string entry = DateTime.Now.ToString("[HH:mm] ") + action;
            _log.Add(entry);
        }

        public string GetRecentLog(int count = 10)
        {
            if (_log.Count == 0)
                return "No actions logged yet. Start chatting to see your activity!";

            int start = Math.Max(0, _log.Count - count);
            var recent = _log.GetRange(start, _log.Count - start);

            string result = "📋 Recent Activity Log:\n\n";
            for (int i = 0; i < recent.Count; i++)
                result += (i + 1) + ". " + recent[i] + "\n";

            if (_log.Count > count)
                result += "\n💡 Type 'show more' to see the full log.";

            return result.Trim();
        }

        public string GetFullLog()
        {
            if (_log.Count == 0)
                return "No actions logged yet.";

            string result = "📋 Full Activity Log:\n\n";
            for (int i = 0; i < _log.Count; i++)
                result += (i + 1) + ". " + _log[i] + "\n";

            return result.Trim();
        }

        public int GetCount()
        {
            return _log.Count;
        }
    }
}