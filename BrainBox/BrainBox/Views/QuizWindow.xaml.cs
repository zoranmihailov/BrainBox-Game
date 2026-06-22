using BrainBox.Models;
using BrainBox.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace BrainBox.Views
{
    /// <summary>
    /// Interaction logic for QuizWindow.xaml
    /// </summary>
    public partial class QuizWindow : Window
    {
        PlayerProfile profile;
        List<QuizQuestion> prasanja;
        QuizQuestion tekovnoPrasanje;
        int tekovenIndex = 0;
        int score = 0;
        int timeLeft = 11;
        bool odgovoreno = false;
        bool zavrsena = false;
        DispatcherTimer timer;

        public QuizWindow(PlayerProfile playerProfile)
        {
            InitializeComponent();
            profile = playerProfile;

            LoadQuestions();
            StartTimer();
            ShowQuestion();
        }

        private void LoadQuestions()
        {
            string json = File.ReadAllText("Data/questions.json");
            QuizData data = JsonSerializer.Deserialize<QuizData>(json);

            Random rnd = new Random();
            prasanja = new List<QuizQuestion>();
            List<QuizQuestion> site = new List<QuizQuestion>(data.questions);

            for (int i = 0; i < 10; i++)
            {
                int index = rnd.Next(site.Count);
                prasanja.Add(site[index]);
                site.RemoveAt(index);
            }
        }

        private void StartTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            timeLeft--;
            txtTimer.Text = timeLeft + "s";

            if (timeLeft <= 0 && !odgovoreno)
            {
                odgovoreno = true;
                timer.Stop();
                ShowCorrectAnswer();
                GoToNext();
            }
        }

        private void ShowQuestion()
        {
            if (tekovenIndex >= prasanja.Count)
            {
                EndGame();
                return;
            }

            odgovoreno = false;
            tekovnoPrasanje = prasanja[tekovenIndex];
            timeLeft = 11;
            txtTimer.Text = "11s";

            txtQuestionNum.Text = "Прашање " + (tekovenIndex + 1) + " / 10";
            txtCategory.Text = tekovnoPrasanje.category;
            txtQuestion.Text = tekovnoPrasanje.question;
            txtScore.Text = "Поени: " + score;

            btnA.Content = tekovnoPrasanje.options[0];
            btnB.Content = tekovnoPrasanje.options[1];
            btnC.Content = tekovnoPrasanje.options[2];
            btnD.Content = tekovnoPrasanje.options[3];

            btnA.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA02E"));
            btnB.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA02E"));
            btnC.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA02E"));
            btnD.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA02E"));

            btnA.IsEnabled = true;
            btnB.IsEnabled = true;
            btnC.IsEnabled = true;
            btnD.IsEnabled = true;

            if (!timer.IsEnabled)
                timer.Start();
        }

        private void AnswerClick(object sender, RoutedEventArgs e)
        {
            if (odgovoreno) return;
            odgovoreno = true;
            timer.Stop();

            Button btn = (Button)sender;
            string izbran = btn.Content.ToString();

            if (izbran == tekovnoPrasanje.answer)
            {
                btn.Background = new SolidColorBrush(Colors.Green);
                score += 5;
            }
            else
            {
                btn.Background = new SolidColorBrush(Colors.Red);
                if (score > 0) score -= 5;
                ShowCorrectAnswer();
            }

            DisableButtons();
            txtScore.Text = "Поени: " + score;
            GoToNext();
        }

        private void SkipClick(object sender, RoutedEventArgs e)
        {
            if (odgovoreno) return;
            odgovoreno = true;
            timer.Stop();
            ShowCorrectAnswer();
            DisableButtons();
            GoToNext();
        }

        private void ShowCorrectAnswer()
        {
            if (btnA.Content.ToString() == tekovnoPrasanje.answer)
                btnA.Background = new SolidColorBrush(Colors.Green);
            if (btnB.Content.ToString() == tekovnoPrasanje.answer)
                btnB.Background = new SolidColorBrush(Colors.Green);
            if (btnC.Content.ToString() == tekovnoPrasanje.answer)
                btnC.Background = new SolidColorBrush(Colors.Green);
            if (btnD.Content.ToString() == tekovnoPrasanje.answer)
                btnD.Background = new SolidColorBrush(Colors.Green);
        }

        private void DisableButtons()
        {
            btnA.IsEnabled = false;
            btnB.IsEnabled = false;
            btnC.IsEnabled = false;
            btnD.IsEnabled = false;
        }

        private void GoToNext()
        {
            DispatcherTimer delay = new DispatcherTimer();
            delay.Interval = TimeSpan.FromSeconds(1);
            delay.Tick += Delay_Tick;
            delay.Start();
        }

        private void Delay_Tick(object sender, EventArgs e)
        {
            DispatcherTimer delay = (DispatcherTimer)sender;
            delay.Stop();
            tekovenIndex++;
            ShowQuestion();
        }

        private void EndGame()
        {
            if (zavrsena) return;
            zavrsena = true;

            profile.Scores["Prasanja"].UpdateScore(score);
            ScoreManager.Save(profile);

            MessageBox.Show("Играта заврши!\nОсвоени поени: " + score + " / 50", "Прашања - Крај");
            Close();
        }
    }

    public class QuizQuestion
    {
        public int id { get; set; }
        public string category { get; set; }
        public string question { get; set; }
        public List<string> options { get; set; }
        public string answer { get; set; }
    }

    public class QuizData
    {
        public List<QuizQuestion> questions { get; set; }
    }
}