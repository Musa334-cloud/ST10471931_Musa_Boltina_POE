using System;
using System.Collections.Generic;

namespace CybersecurityChatbotGUI
{
    public class KeywordResponder
    {
        private Dictionary<string, List<string>> _responses;
        private Random _random = new Random();

        public KeywordResponder()
        {
            _responses = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                ["password"] = new List<string>
                {
                    "Use a mix of uppercase, lowercase, numbers, and special characters. Never reuse passwords across sites!",
                    "A strong password is at least 12 characters long. Consider using a passphrase like 'PurpleTiger$RunsFast9'.",
                    "Never share your password with anyone — not even IT support. Use a password manager to keep track of them.",
                    "Avoid obvious passwords like '123456' or 'password'. They are the first ones hackers try."
                },
                ["phishing"] = new List<string>
                {
                    "Phishing emails often create urgency — 'Act now or your account will be closed!' Always pause and verify before clicking.",
                    "Check the sender's email address carefully. Attackers use domains like 'support@paypa1.com' to fool you.",
                    "Hover over links before clicking to see the real URL. If it looks suspicious, don't click it.",
                    "Legitimate companies will never ask for your password via email. If in doubt, go directly to the website."
                },
                ["privacy"] = new List<string>
                {
                    "Review the privacy settings on your social media accounts. Limit who can see your personal information.",
                    "Be cautious about the data you share with apps. Many collect more than they need.",
                    "Use a VPN when connecting to public Wi-Fi to protect your data from snooping.",
                    "Regularly review which apps have access to your camera, microphone, and location."
                },
                ["scam"] = new List<string>
                {
                    "If an offer sounds too good to be true, it probably is. Online scams often promise prizes or urgent refunds.",
                    "Verify before you trust — always confirm unexpected calls or emails through official channels.",
                    "Romance scams are on the rise. Be wary of people online who ask for money or gift cards.",
                    "Tech support scams often start with a fake pop-up. Microsoft will never call you unsolicited."
                },
                ["malware"] = new List<string>
                {
                    "Install reputable antivirus software and keep it updated to detect and remove malware threats.",
                    "Avoid downloading files from untrusted sources or clicking links in unexpected emails.",
                    "Ransomware can encrypt all your files. Regular offline backups are your best defence.",
                    "Keep your operating system updated — patches often fix vulnerabilities that malware exploits."
                },
                ["firewall"] = new List<string>
                {
                    "A firewall monitors traffic and blocks unauthorised access. Make sure Windows Firewall is always enabled.",
                    "Both hardware and software firewalls are important. Your router has one; your PC should too.",
                    "Firewalls can be configured to block specific ports and applications — review your settings regularly."
                },
                ["encryption"] = new List<string>
                {
                    "Encryption scrambles your data so only someone with the right key can read it. Use it for sensitive files.",
                    "Look for HTTPS in your browser's address bar — it means your connection to the site is encrypted.",
                    "Enable full-disk encryption (BitLocker on Windows) to protect your data if your device is stolen."
                },
                ["2fa"] = new List<string>
                {
                    "Two-factor authentication adds a second layer of security. Even if your password is stolen, your account stays safe.",
                    "Use an authenticator app like Google Authenticator rather than SMS for stronger 2FA.",
                    "Enable 2FA on all important accounts — especially email, banking, and social media."
                },
                ["backup"] = new List<string>
                {
                    "Follow the 3-2-1 backup rule: 3 copies, 2 different media, 1 offsite. This protects against ransomware.",
                    "Test your backups regularly — a backup you cannot restore from is worthless.",
                    "Cloud backups are convenient, but also encrypt them so your cloud provider cannot access your data."
                },
                ["vpn"] = new List<string>
                {
                    "A VPN encrypts your internet traffic and hides your IP address — essential on public Wi-Fi.",
                    "Not all VPNs are trustworthy. Choose one with a strict no-logs policy and good reviews.",
                    "A VPN protects your privacy but does not make you completely anonymous online."
                }
            };
        }

        public string GetResponse(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return null;

            foreach (var key in _responses.Keys)
            {
                if (input.IndexOf(key, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    var list = _responses[key];
                    return list[_random.Next(list.Count)];
                }
            }
            return null;
        }

        public List<string> GetAllKeywords()
        {
            return new List<string>(_responses.Keys);
        }
    }
}