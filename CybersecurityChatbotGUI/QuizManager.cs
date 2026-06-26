using System.Collections.Generic;

namespace CybersecurityChatbotGUI
{
    public class QuizManager
    {
        private List<QuizQuestion> _questions;
        private int _currentIndex = 0;
        private int _score = 0;
        private ActivityLogger _logger;

        public QuizManager(ActivityLogger logger)
        {
            _logger = logger;
            _questions = new List<QuizQuestion>
            {
                new QuizQuestion { Question = "What should you do if you receive an email asking for your password?", Options = new List<string> { "A) Reply with your password", "B) Delete the email", "C) Report the email as phishing", "D) Ignore it" }, CorrectAnswer = "C", Explanation = "Reporting phishing emails helps prevent scams and protects others.", IsTrueFalse = false },
                new QuizQuestion { Question = "True or False: Using the same password for multiple accounts is safe.", Options = new List<string> { "True", "False" }, CorrectAnswer = "False", Explanation = "If one account is hacked, all your accounts become vulnerable.", IsTrueFalse = true },
                new QuizQuestion { Question = "What does HTTPS mean in a website address?", Options = new List<string> { "A) The website is fast", "B) The website is encrypted and secure", "C) The website is free", "D) The website is popular" }, CorrectAnswer = "B", Explanation = "HTTPS means the connection between your browser and the website is encrypted.", IsTrueFalse = false },
                new QuizQuestion { Question = "True or False: Public Wi-Fi is always safe to use for banking.", Options = new List<string> { "True", "False" }, CorrectAnswer = "False", Explanation = "Public Wi-Fi can be monitored by attackers. Use a VPN for sensitive tasks.", IsTrueFalse = true },
                new QuizQuestion { Question = "What is two-factor authentication (2FA)?", Options = new List<string> { "A) Two passwords for one account", "B) A backup email address", "C) A second verification step after your password", "D) A type of antivirus software" }, CorrectAnswer = "C", Explanation = "2FA adds a second layer of security so even if your password is stolen your account stays safe.", IsTrueFalse = false },
                new QuizQuestion { Question = "What is ransomware?", Options = new List<string> { "A) Software that speeds up your PC", "B) Malware that encrypts your files and demands payment", "C) A type of firewall", "D) An antivirus program" }, CorrectAnswer = "B", Explanation = "Ransomware locks your files and demands money. Regular backups are your best defence.", IsTrueFalse = false },
                new QuizQuestion { Question = "True or False: You should click links in emails from unknown senders to check if they are safe.", Options = new List<string> { "True", "False" }, CorrectAnswer = "False", Explanation = "Never click links from unknown senders. Go directly to the website instead.", IsTrueFalse = true },
                new QuizQuestion { Question = "What is social engineering in cybersecurity?", Options = new List<string> { "A) Building social media platforms", "B) Manipulating people into revealing confidential information", "C) Engineering social networks", "D) Creating fake profiles" }, CorrectAnswer = "B", Explanation = "Social engineering exploits human trust rather than technical vulnerabilities.", IsTrueFalse = false },
                new QuizQuestion { Question = "How often should you back up your important data?", Options = new List<string> { "A) Once a year", "B) Only when you remember", "C) Never — cloud storage is enough", "D) Regularly, following the 3-2-1 rule" }, CorrectAnswer = "D", Explanation = "The 3-2-1 rule: 3 copies, 2 different media, 1 offsite. This protects against ransomware and hardware failure.", IsTrueFalse = false },
                new QuizQuestion { Question = "True or False: A strong password should be at least 12 characters long.", Options = new List<string> { "True", "False" }, CorrectAnswer = "True", Explanation = "Longer passwords are much harder to crack. Use a mix of letters, numbers, and symbols.", IsTrueFalse = true },
                new QuizQuestion { Question = "What should you do before installing an app on your phone?", Options = new List<string> { "A) Check the permissions it requests", "B) Install it immediately", "C) Share it with friends first", "D) Nothing — all apps are safe" }, CorrectAnswer = "A", Explanation = "Always check what permissions an app requests. Be suspicious of apps asking for unnecessary access.", IsTrueFalse = false },
                new QuizQuestion { Question = "What is a VPN used for?", Options = new List<string> { "A) Making your internet faster", "B) Blocking all websites", "C) Encrypting your internet traffic and hiding your IP", "D) Storing passwords safely" }, CorrectAnswer = "C", Explanation = "A VPN encrypts your connection and hides your IP address — essential on public Wi-Fi.", IsTrueFalse = false }
            };
        }

        public QuizQuestion GetCurrentQuestion()
        {
            return _currentIndex < _questions.Count ? _questions[_currentIndex] : null;
        }

        public bool SubmitAnswer(string answer)
        {
            if (_currentIndex >= _questions.Count) return false;
            bool correct = answer.Trim().ToUpper() == _questions[_currentIndex].CorrectAnswer.ToUpper();
            if (correct) _score++;
            _currentIndex++;
            return correct;
        }

        public string GetFeedback()
        {
            int index = _currentIndex - 1;
            return (index >= 0 && index < _questions.Count) ? _questions[index].Explanation : string.Empty;
        }

        public bool IsFinished() => _currentIndex >= _questions.Count;
        public string GetFinalScore() => _score + " out of " + _questions.Count;
        public int GetCurrentQuestionNumber() => _currentIndex + 1;
        public int GetTotalQuestions() => _questions.Count;
        public int GetCurrentScore() => _score;

        public string GetFinalMessage()
        {
            double percent = (double)_score / _questions.Count * 100;
            if (percent >= 80) return "🎉 Excellent! You scored " + GetFinalScore() + "! You're a cybersecurity champion!";
            if (percent >= 60) return "👍 Good job! You scored " + GetFinalScore() + ". Keep learning to improve!";
            return "📚 You scored " + GetFinalScore() + ". Keep studying — cybersecurity knowledge is important!";
        }

        public void ResetQuiz()
        {
            _currentIndex = 0;
            _score = 0;
            _logger.Log("Quiz started");
        }
    }
}
