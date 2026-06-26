using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CybersecurityChatbotGUI
{
    public partial class MainWindow : Window
    {
        private ChatBot _chatBot;
        private ActivityLogger _logger;
        private TaskManager _taskManager;
        private QuizManager _quizManager;
        private string _selectedAnswer = string.Empty;
        private List<RadioButton> _quizRadioButtons = new List<RadioButton>();

        public MainWindow()
        {
            InitializeComponent();
            _logger = new ActivityLogger();
            _taskManager = new TaskManager(_logger);
            _chatBot = new ChatBot(_logger, _taskManager);
            _quizManager = new QuizManager(_logger);
            PlayVoiceGreeting();
            AppendBotMessage(_chatBot.GetGreeting());
            LoadTaskList();
        }

        private void PlayVoiceGreeting()
        {
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "greeting.wav");
                if (File.Exists(path))
                {
                    SoundPlayer player = new SoundPlayer(path);
                    player.Play();
                }
            }
            catch { }
        }

        private void NavChat_Click(object sender, RoutedEventArgs e)
        {
            ChatPanel.Visibility = Visibility.Visible;
            TaskPanel.Visibility = Visibility.Collapsed;
            QuizPanel.Visibility = Visibility.Collapsed;
        }

        private void NavTasks_Click(object sender, RoutedEventArgs e)
        {
            ChatPanel.Visibility = Visibility.Collapsed;
            TaskPanel.Visibility = Visibility.Visible;
            QuizPanel.Visibility = Visibility.Collapsed;
            LoadTaskList();
        }

        private void NavQuiz_Click(object sender, RoutedEventArgs e)
        {
            ChatPanel.Visibility = Visibility.Collapsed;
            TaskPanel.Visibility = Visibility.Collapsed;
            QuizPanel.Visibility = Visibility.Visible;
        }

        private void SendButton_Click(object sender, RoutedEventArgs e) => SendMessage();

        private void UserInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) SendMessage();
        }

        private void TopicButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;
            UserInput.Text = btn.Tag.ToString();
            SendMessage();
        }

        private void SendMessage()
        {
            string input = UserInput.Text.Trim();
            if (string.IsNullOrWhiteSpace(input)) return;

            AppendUserMessage(input);
            UserInput.Text = string.Empty;
            UserInput.Foreground = Brushes.White;

            string response = _chatBot.ProcessInput(input);

            if (response == "LAUNCH_QUIZ")
            {
                AppendBotMessage("🎮 Launching the Cybersecurity Quiz! Click the Quiz tab on the left.");
                NavQuiz_Click(null, null);
                return;
            }

            AppendBotMessage(response);
            StatusBar.Text = "SecureNet replied • " + DateTime.Now.ToString("HH:mm:ss");

            if (response.Contains("✅ Task added") || response.Contains("Task saved"))
                LoadTaskList();
        }

        private void AppendUserMessage(string text)
        {
            var container = new Grid { Margin = new Thickness(60, 6, 8, 6) };
            container.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            var bubble = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(0x23, 0x8B, 0xE6)),
                CornerRadius = new CornerRadius(14, 14, 2, 14),
                Padding = new Thickness(14, 10, 14, 10),
                HorizontalAlignment = HorizontalAlignment.Right,
                MaxWidth = 480
            };

            bubble.Child = new TextBlock
            {
                Text = text,
                Foreground = Brushes.White,
                FontFamily = new FontFamily("Segoe UI"),
                FontSize = 13,
                TextWrapping = TextWrapping.Wrap
            };

            Grid.SetColumn(bubble, 0);
            container.Children.Add(bubble);
            ChatMessages.Children.Add(container);
            ScrollToBottom();
        }

        private void AppendBotMessage(string text)
        {
            ChatMessages.Children.Add(new TextBlock
            {
                Text = "SECURENET",
                FontFamily = new FontFamily("Segoe UI"),
                FontSize = 10,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(0x00, 0xD4, 0xFF)),
                Margin = new Thickness(12, 8, 0, 2)
            });

            var container = new Grid { Margin = new Thickness(8, 0, 60, 4) };
            container.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            var bubble = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(0x16, 0x1B, 0x22)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(0x21, 0x26, 0x2D)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(2, 14, 14, 14),
                Padding = new Thickness(14, 10, 14, 10),
                HorizontalAlignment = HorizontalAlignment.Left,
                MaxWidth = 480
            };

            bubble.Child = new TextBlock
            {
                Text = text,
                Foreground = new SolidColorBrush(Color.FromRgb(0xE6, 0xED, 0xF3)),
                FontFamily = new FontFamily("Segoe UI"),
                FontSize = 13,
                TextWrapping = TextWrapping.Wrap,
                LineHeight = 20
            };

            Grid.SetColumn(bubble, 0);
            container.Children.Add(bubble);
            ChatMessages.Children.Add(container);
            ScrollToBottom();
        }

        private void ScrollToBottom()
        {
            ChatScrollViewer.UpdateLayout();
            ChatScrollViewer.ScrollToEnd();
        }

        private void LoadTaskList()
        {
            TaskListBox.Items.Clear();
            var tasks = _taskManager.GetAllTasks();

            if (tasks.Count == 0)
            {
                TaskListBox.Items.Add("No tasks yet. Add one above!");
                return;
            }

            foreach (var task in tasks)
            {
                string status = task.IsComplete ? "✅" : "⏳";
                string reminder = !string.IsNullOrWhiteSpace(task.Reminder) ? " | ⏰ " + task.Reminder : "";
                string display = status + " [" + task.Id + "] " + task.Title + reminder;
                if (!string.IsNullOrWhiteSpace(task.Description) &&
                    task.Description != "Cybersecurity task: " + task.Title)
                    display += "\n      " + task.Description;
                TaskListBox.Items.Add(display);
            }
        }

        private void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            string title = TaskTitleInput.Text.Trim();
            string desc = TaskDescInput.Text.Trim();
            string reminder = TaskReminderInput.Text.Trim();

            if (string.IsNullOrWhiteSpace(title))
            {
                TaskStatusLabel.Text = "⚠️ Please enter a task title!";
                return;
            }

            if (string.IsNullOrWhiteSpace(desc))
                desc = "Cybersecurity task: " + title;

            _taskManager.AddTask(title, desc, reminder);
            LoadTaskList();
            TaskTitleInput.Clear();
            TaskDescInput.Clear();
            TaskReminderInput.Clear();
            TaskStatusLabel.Text = "✅ Task added successfully!";
        }

        private void MarkCompleteButton_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = TaskListBox.SelectedIndex;
            if (selectedIndex < 0) { TaskStatusLabel.Text = "⚠️ Please select a task first!"; return; }

            var tasks = _taskManager.GetAllTasks();
            if (selectedIndex < tasks.Count)
            {
                _taskManager.MarkAsComplete(tasks[selectedIndex].Id);
                LoadTaskList();
                TaskStatusLabel.Text = "✅ Task marked as complete!";
            }
        }

        private void DeleteTaskButton_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = TaskListBox.SelectedIndex;
            if (selectedIndex < 0) { TaskStatusLabel.Text = "⚠️ Please select a task first!"; return; }

            var tasks = _taskManager.GetAllTasks();
            if (selectedIndex < tasks.Count)
            {
                _taskManager.DeleteTask(tasks[selectedIndex].Id);
                LoadTaskList();
                TaskStatusLabel.Text = "🗑️ Task deleted!";
            }
        }

        private void StartQuizButton_Click(object sender, RoutedEventArgs e)
        {
            _quizManager.ResetQuiz();
            _logger.Log("Quiz started by user");
            QuizStartScreen.Visibility = Visibility.Collapsed;
            QuizResultsScreen.Visibility = Visibility.Collapsed;
            QuizQuestionScreen.Visibility = Visibility.Visible;
            FeedbackBorder.Visibility = Visibility.Collapsed;
            ShowCurrentQuestion();
        }

        private void ShowCurrentQuestion()
        {
            var question = _quizManager.GetCurrentQuestion();
            if (question == null) return;

            QuizQuestionText.Text = "Q" + _quizManager.GetCurrentQuestionNumber() + ": " + question.Question;
            QuizProgressLabel.Text = "Question " + _quizManager.GetCurrentQuestionNumber() + " of " + _quizManager.GetTotalQuestions();
            QuizScoreLabel.Text = "Score: " + _quizManager.GetCurrentScore();
            QuizStatusBar.Text = "Select your answer and click Submit";

            QuizOptionsPanel.Children.Clear();
            _quizRadioButtons.Clear();
            _selectedAnswer = string.Empty;

            foreach (string option in question.Options)
            {
                var rb = new RadioButton
                {
                    Content = option,
                    Foreground = new SolidColorBrush(Color.FromRgb(0xE6, 0xED, 0xF3)),
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 14,
                    Margin = new Thickness(0, 6, 0, 6),
                    GroupName = "QuizOptions",
                    Cursor = Cursors.Hand
                };

                string optionCopy = option;
                rb.Checked += (s, ev) =>
                {
                    _selectedAnswer = question.IsTrueFalse ? optionCopy : optionCopy.Substring(0, 1);
                };

                _quizRadioButtons.Add(rb);
                QuizOptionsPanel.Children.Add(rb);
            }

            FeedbackBorder.Visibility = Visibility.Collapsed;
            SubmitAnswerButton.Visibility = Visibility.Visible;
        }

        private void SubmitAnswerButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_selectedAnswer))
            {
                QuizStatusBar.Text = "⚠️ Please select an answer first!";
                return;
            }

            bool correct = _quizManager.SubmitAnswer(_selectedAnswer);
            string feedback = _quizManager.GetFeedback();

            FeedbackText.Text = correct ? "✅ " + feedback : "❌ Incorrect. " + feedback;
            FeedbackBorder.Background = new SolidColorBrush(correct
                ? Color.FromRgb(0x1A, 0x3A, 0x1A)
                : Color.FromRgb(0x3A, 0x1A, 0x1A));

            FeedbackBorder.Visibility = Visibility.Visible;
            SubmitAnswerButton.Visibility = Visibility.Collapsed;
            QuizScoreLabel.Text = "Score: " + _quizManager.GetCurrentScore();
            NextQuestionButton.Content = _quizManager.IsFinished() ? "See Results 🏆" : "Next Question ➤";

            if (_quizManager.IsFinished())
                _logger.Log("Quiz completed — score: " + _quizManager.GetFinalScore());
        }

        private void NextQuestionButton_Click(object sender, RoutedEventArgs e)
        {
            if (_quizManager.IsFinished())
            {
                QuizQuestionScreen.Visibility = Visibility.Collapsed;
                QuizResultsScreen.Visibility = Visibility.Visible;
                QuizFinalMessage.Text = _quizManager.GetFinalMessage();
                QuizStatusBar.Text = "Quiz complete!";
            }
            else
            {
                ShowCurrentQuestion();
            }
        }
    }
}