using System;
using System.Collections.Generic;

namespace CybersecurityChatbotGUI
{
    public class ChatBot
    {
        private KeywordResponder _keywords;
        private SentimentDetector _sentiment;
        private MemoryStore _memory;
        private ActivityLogger _logger;
        private TaskManager _taskManager;

        private bool _awaitingName = true;
        private bool _awaitingReminder = false;
        private string _pendingTaskTitle = string.Empty;
        private string _pendingTaskDescription = string.Empty;
        private string _lastTopic = null;

        private Random _random = new Random();

        private List<string> _fallbackResponses = new List<string>
        {
            "I'm not sure about that one. Try asking me about passwords, phishing, or malware!",
            "Hmm, I didn't quite get that. Type 'what can you do' to see my topics.",
            "I'm still learning! Could you rephrase that or ask about a cybersecurity topic?",
            "That's outside my expertise for now. Try asking about encryption or 2FA!"
        };

        private List<string> _returningGreetings = new List<string>
        {
            "Welcome back, {name}! Great to have you back! 🎉",
            "Hey {name}! Nice to see you again! 👋",
            "Welcome back, {name}! I missed you! 😊",
            "Good to have you back, {name}! Ready to stay cyber-safe? 🛡️"
        };

        public ChatBot(ActivityLogger logger, TaskManager taskManager)
        {
            _keywords = new KeywordResponder();
            _sentiment = new SentimentDetector();
            _memory = new MemoryStore();
            _logger = logger;
            _taskManager = taskManager;
        }

        public string GetGreeting()
        {
            return "👋 Hello! I'm SecureNet, your Cybersecurity Assistant.\nWhat's your name?";
        }

        private string ExtractName(string input)
        {
            string lower = input.ToLowerInvariant().Trim();
            string[] greetings = { "good morning", "good evening", "good afternoon", "hello", "hey", "hi" };
            foreach (string g in greetings)
                if (lower.StartsWith(g)) { lower = lower.Substring(g.Length).Trim(); break; }

            lower = lower.TrimStart(',').TrimStart('.').Trim();

            string[] phrases = { "my name is", "i am", "i'm", "its", "it's", "they call me", "you can call me", "name is" };
            foreach (string phrase in phrases)
                if (lower.StartsWith(phrase)) { lower = lower.Substring(phrase.Length).Trim(); break; }

            lower = lower.Replace(",", "").Replace(".", "").Replace("!", "").Trim();
            string[] words = lower.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (words.Length == 0) return input.Trim();

            string name = words[0].Trim();
            if (name.Length > 0)
                name = char.ToUpper(name[0]) + name.Substring(1).ToLower();
            return name;
        }

        private string ExtractTaskTitle(string input)
        {
            string lower = input.ToLowerInvariant();
            string[] removePhrases = {
                "add a task to", "add task to", "add a task for", "add task for",
                "add a task", "add task", "create a task to", "create task to",
                "create a task", "create task", "i need to", "remind me to",
                "remind me about", "set a reminder to", "set reminder to",
                "set a reminder for", "set reminder for", "don't forget to",
                "dont forget to", "remember to"
            };

            foreach (string phrase in removePhrases)
            {
                if (lower.Contains(phrase))
                {
                    int idx = lower.IndexOf(phrase);
                    lower = lower.Substring(idx + phrase.Length).Trim();
                    break;
                }
            }

            lower = lower.TrimStart(',').TrimStart('.').Trim();
            if (lower.Length > 0)
                lower = char.ToUpper(lower[0]) + lower.Substring(1);
            return lower;
        }

        public string ProcessInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "Please type something so I can help you! 😊";

            string trimmed = input.Trim();
            string lower = trimmed.ToLowerInvariant();

            // STEP 1: Capture name
            if (_awaitingName)
            {
                string extractedName = ExtractName(trimmed);
                bool isReturning = _memory.LoadUser(extractedName);
                _awaitingName = false;
                _logger.Log("User session started: " + extractedName);

                if (isReturning)
                {
                    string greeting = _returningGreetings[_random.Next(_returningGreetings.Count)]
                        .Replace("{name}", _memory.UserName);
                    string topicLine = !string.IsNullOrWhiteSpace(_memory.FavouriteTopic)
                        ? "\n\nLast time you were interested in " + _memory.FavouriteTopic + ". Want to continue or explore something new?"
                        : "\n\nWhat cybersecurity topic can I help you with today?";
                    return greeting + topicLine;
                }
                else
                {
                    _memory.UserName = extractedName;
                    _memory.SaveToFile();
                    return "Nice to meet you, " + _memory.UserName + "! 🛡️\n" +
                           "I'm here to help you stay safe online.\n\n" +
                           "You can ask me about: passwords, phishing, malware, privacy, scams, " +
                           "firewalls, encryption, 2FA, backup, or VPN.\n\n" +
                           "Or click any topic from the menu on the left!\n\n" +
                           "💡 Type 'add task', 'start quiz', or 'show activity log' to explore more!";
                }
            }

            // STEP 2: Awaiting reminder
            if (_awaitingReminder)
            {
                _awaitingReminder = false;
                if (lower.Contains("yes") || lower.Contains("remind") || lower.Contains("days") || lower.Contains("tomorrow"))
                {
                    string result = _taskManager.AddTask(_pendingTaskTitle, _pendingTaskDescription, trimmed);
                    _pendingTaskTitle = string.Empty;
                    _pendingTaskDescription = string.Empty;
                    return result + "\n\n✅ Task saved to your task list!";
                }
                else
                {
                    string result = _taskManager.AddTask(_pendingTaskTitle, _pendingTaskDescription, "");
                    _pendingTaskTitle = string.Empty;
                    _pendingTaskDescription = string.Empty;
                    return result + "\n\n✅ Task saved without a reminder.";
                }
            }

            // STEP 3: Show full log
            if (lower.Contains("show more") || lower.Contains("full log") || lower.Contains("all log"))
            {
                _logger.Log("Full activity log viewed");
                return _logger.GetFullLog();
            }

            // STEP 4: Show recent log
            if (lower.Contains("show activity log") || lower.Contains("what have you done") ||
                lower.Contains("what did you do") || lower.Contains("show log") ||
                lower.Contains("recent actions") || lower.Contains("activity log") ||
                lower.Contains("what have you done for me"))
            {
                _logger.Log("Activity log viewed by " + _memory.UserName);
                return _logger.GetRecentLog(10);
            }

            // STEP 5: Start quiz
            if (lower.Contains("start quiz") || lower.Contains("take quiz") ||
                lower.Contains("quiz me") || lower.Contains("test my knowledge") ||
                lower.Contains("play the game") || lower.Contains("begin quiz") ||
                lower.Contains("i want to take a quiz"))
            {
                _logger.Log("Quiz launched by " + _memory.UserName);
                return "LAUNCH_QUIZ";
            }

            // STEP 6: Add task
            if (lower.Contains("add task") || lower.Contains("add a task") ||
                lower.Contains("create task") || lower.Contains("create a task") ||
                lower.Contains("i need to") || lower.Contains("remind me to") ||
                lower.Contains("remind me about") || lower.Contains("set a reminder") ||
                lower.Contains("set reminder") || lower.Contains("don't forget") ||
                lower.Contains("dont forget") || lower.Contains("remember to"))
            {
                string title = ExtractTaskTitle(trimmed);
                if (string.IsNullOrWhiteSpace(title) || title.Length < 2)
                    return "What task would you like to add? Please describe it briefly.";

                _pendingTaskTitle = title;
                _pendingTaskDescription = "Cybersecurity task: " + title;
                _awaitingReminder = true;
                _logger.Log("NLP recognised task intent: '" + title + "'");
                return "Got it! I'll add the task: '" + title + "'\n\nWould you like a reminder? If yes, tell me when (e.g. 'remind me in 3 days' or 'remind me tomorrow').";
            }

            // STEP 7: Follow-up
            if ((lower.Contains("tell me more") || lower.Contains("explain more") ||
                 lower.Contains("more info") || lower.Contains("more about that"))
                && _lastTopic != null)
            {
                string followUp = _keywords.GetResponse(_lastTopic);
                return "Sure! Here's more on " + _lastTopic + ":\n\n" + followUp;
            }

            // STEP 8: Sentiment
            Sentiment detected = _sentiment.Detect(trimmed);
            string sentimentOpener = _sentiment.GetSentimentResponse(detected);

            // STEP 9: Keywords
            string keywordResponse = _keywords.GetResponse(trimmed);
            if (keywordResponse != null)
            {
                foreach (string kw in _keywords.GetAllKeywords())
                {
                    if (lower.Contains(kw.ToLowerInvariant()))
                    {
                        _lastTopic = kw;
                        _memory.FavouriteTopic = kw;
                        _memory.Store("favouriteTopic", kw);
                        _memory.SaveToFile();
                        _logger.Log("Keyword matched: " + kw + " — response delivered");
                        break;
                    }
                }

                if (detected != Sentiment.Neutral && !string.IsNullOrWhiteSpace(sentimentOpener))
                    return sentimentOpener + "\n\n" + keywordResponse;

                string opener = _memory.GetPersonalisedOpener();
                if (!string.IsNullOrWhiteSpace(opener))
                    return opener + keywordResponse;

                return keywordResponse;
            }

            // STEP 10: Sentiment only
            if (detected == Sentiment.Worried)
                return "I can hear that you're worried. That's completely okay! You're already doing the right thing by asking questions. Try asking me about phishing, malware, or passwords.";
            if (detected == Sentiment.Frustrated)
                return "I completely understand your frustration — cybersecurity can feel overwhelming at first. What topic is confusing you?";
            if (detected == Sentiment.Curious)
                return "I love your curiosity! Ask me about passwords, phishing, malware, privacy, scams, firewall, encryption, 2FA, backup, or VPN!";
            if (detected == Sentiment.Happy)
                return "That's great to hear, " + _memory.UserName + "! 😊 Ask me about any cybersecurity topic!";

            // STEP 11: Special phrases
            if (lower.Contains("how are you"))
                return "I'm doing great, thanks for asking " + _memory.UserName + "! 😊 Ready to help you stay cyber-safe!";

            if (lower.Contains("what can you do") || lower.Contains("help") || lower.Contains("topics"))
            {
                var topics = _keywords.GetAllKeywords();
                string topicList = string.Join("\n🔒 ", topics);
                return "Here are the topics I can help you with, " + _memory.UserName + ":\n\n🔒 " + topicList +
                       "\n\n💡 You can also:\n• Type 'add task' to add a cybersecurity task\n• Type 'start quiz' to test your knowledge\n• Type 'show activity log' to see recent actions";
            }

            if (lower.Contains("who are you") || lower.Contains("what are you"))
                return "I'm SecureNet — your personal Cybersecurity Assistant! I help you stay safe online, manage security tasks, and test your knowledge with quizzes. 🛡️";

            if (lower.Contains("bye") || lower.Contains("goodbye"))
                return "Goodbye, " + _memory.UserName + "! 👋 Stay safe online. Remember: think before you click!";

            // STEP 12: Fallback
            _logger.Log("Fallback response triggered");
            return _fallbackResponses[_random.Next(_fallbackResponses.Count)];
        }
    }
}