using BrainBox.Models;
using BrainBox.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
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
    /// Interaction logic for MathWindow.xaml
    /// </summary>
    public partial class MathWindow : Window
    {
        PlayerProfile profile;
        int target;
        List<int> numbers = new List<int>();
        string solution = "";
        int timeLeft = 80;
        bool zavrsena = false;
        DispatcherTimer timer;

        public MathWindow(PlayerProfile playerProfile)
        {
            InitializeComponent();
            profile = playerProfile;

            LoadMathCombination();
            StartTimer();
            this.Loaded += (s, e) => txtExpression.Focus();

            txtExpression.KeyDown += TxtExpression_KeyDown;
        }

        private void TxtExpression_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ConfirmAnswer(sender, null);
            }
        }

        private void LoadMathCombination()
        {
            string json = File.ReadAllText("Data/math_combinations.json");
            List<MathCombination> lista = JsonSerializer.Deserialize<List<MathCombination>>(json);

            Random rnd = new Random();
            int index = rnd.Next(lista.Count);
            MathCombination izbrana = lista[index];

            numbers = izbrana.numbers;
            target = izbrana.target;
            solution = izbrana.solution;

            txtTarget.Text = target.ToString();
            BuildNumbersUI();
        }

        private void BuildNumbersUI()
        {
            numbersPanel.Children.Clear();

            for (int i = 0; i < numbers.Count; i++)
            {
                TextBlock tb = new TextBlock();
                tb.Text = numbers[i].ToString();
                tb.FontSize = 22;
                tb.FontWeight = FontWeights.Bold;
                tb.Foreground = Brushes.White;
                tb.HorizontalAlignment = HorizontalAlignment.Center;
                tb.VerticalAlignment = VerticalAlignment.Center;

                Border b = new Border();
                b.Width = 70;
                b.Height = 70;
                b.Margin = new Thickness(5);
                b.Background = new SolidColorBrush(Color.FromRgb(70, 70, 70));
                b.Child = tb;

                numbersPanel.Children.Add(b);
            }
        }

        private void StartTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
            txtTimer.Text = timeLeft + "s";
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            timeLeft--;
            txtTimer.Text = timeLeft + "s";

            if (timeLeft <= 0)
            {
                timer.Stop();
                string expr = txtExpression.Text.Trim();
                if (expr == "")
                {
                    EndGame(false, false, 0, "Времето истече!");
                }
                else
                {
                    ConfirmAnswer(null, null);
                }
            }

        }

        private void ConfirmAnswer(object sender, RoutedEventArgs e)
        {
            string expr = txtExpression.Text.Trim();

            if (expr == "")
            {
                txtMessage.Text = "Внеси математички израз!";
                return;
            }

            // proveruva koi broevi gi koristime vo izrazot
            MatchCollection matches = Regex.Matches(expr, @"\d+");
            List<int> koristeni = new List<int>();
            foreach (Match m in matches)
            {
                koristeni.Add(int.Parse(m.Value));
            }

            List<int> dostapni = new List<int>(numbers);
            for (int i = 0; i < koristeni.Count; i++)
            {
                int n = koristeni[i];
                if (dostapni.Contains(n))
                {
                    dostapni.Remove(n);
                }
                else
                {
                    txtMessage.Text = "Бројот " + n + " не е дозволен или веќе е употребен!";
                    return;
                }
            }

            try
            {
                object res = new DataTable().Compute(expr, null);
                double rezultat = Convert.ToDouble(res);
                double razlika = Math.Abs(rezultat - target);

                if (razlika < 0.0001)
                {
                    timer.Stop();
                    EndGame(true, false, rezultat, "");
                }
                else if (razlika <= 10)
                {
                    timer.Stop();
                    EndGame(false, true, rezultat, "");
                }
                else
                {
                    txtMessage.Text = "Резултатот е " + rezultat + ", а треба " + target + ". Обиди се повторно!";
                }
            }
            catch
            {
                txtMessage.Text = "Невалиден израз!";
            }
        }

        private void EndGame(bool exact, bool close, double rezultat, string reason)
        {
            if (zavrsena) return;
            zavrsena = true;
            int score = 0;
            string msg;

            if (exact)
            {
                score = 30;
                msg = "Точно! Освоивте 30 / 30 поени!";
            }
            else if (close)
            {
                score = 15;
                msg = "Близу! Резултатот е " + rezultat + ", таргетот беше " + target +
                      ".\nОсвоивте 15 / 30 поени (во опсег +/-10).";
            }
            else
            {
                msg = reason + "\nТаргетот беше: " + target + "\nОсвоивте 0 / 30 поени.";
                if (solution != "")
                {
                    msg = msg + "\n\nРешение: " + solution;
                }
            }

            profile.Scores["Matematika"].UpdateScore(score);
            ScoreManager.Save(profile);

            MessageBox.Show(msg, "Математика - Крај");
            Close();
        }
    }

    public class MathCombination
    {
        public List<int> numbers { get; set; }
        public int target { get; set; }
        public string solution { get; set; }
    }
}

