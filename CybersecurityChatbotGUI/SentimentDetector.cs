using System.Collections.Generic;

namespace CybersecurityChatbotGUI
{
    public enum Sentiment { Neutral, Worried, Curious, Frustrated, Happy }

    public class SentimentDetector
    {
        private Dictionary<Sentiment, List<string>> _triggerWords;

        public SentimentDetector()
        {
            _triggerWords = new Dictionary<Sentiment, List<string>>();

            _triggerWords[Sentiment.Worried] = new List<string>
            {
                "worried", "scared", "afraid", "anxious", "nervous",
                "unsafe", "frightened", "concerned", "panic"
            };

            _triggerWords[Sentiment.Curious] = new List<string>
            {
                "curious", "wondering", "interested", "want to know",
                "how does", "what is", "explain", "learn"
            };

            _triggerWords[Sentiment.Frustrated] = new List<string>
            {
                "frustrated", "annoyed", "confused", "dont understand",
                "hate", "difficult", "complicated", "angry"
            };

            _triggerWords[Sentiment.Happy] = new List<string>
            {
                "great", "thanks", "helpful", "awesome", "love it",
                "amazing", "fantastic", "happy"
            };
        }

        public Sentiment Detect(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return Sentiment.Neutral;

            string lower = input.ToLowerInvariant();

            foreach (var kvp in _triggerWords)
                foreach (var word in kvp.Value)
                    if (lower.Contains(word))
                        return kvp.Key;

            return Sentiment.Neutral;
        }

        public string GetSentimentResponse(Sentiment sentiment)
        {
            switch (sentiment)
            {
                case Sentiment.Worried:
                    return "I understand you are feeling a bit worried - that is completely valid. Let me help ease your concern. ";
                case Sentiment.Curious:
                    return "I love your curiosity! Great question. Here is what you should know: ";
                case Sentiment.Frustrated:
                    return "I hear you - cybersecurity can feel overwhelming. Let me break this down clearly for you. ";
                case Sentiment.Happy:
                    return "That is great to hear! Keep up the positive attitude. Here is something useful: ";
                default:
                    return string.Empty;
            }
        }
    }
}