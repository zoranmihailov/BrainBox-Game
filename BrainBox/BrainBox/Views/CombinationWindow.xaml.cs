using BrainBox.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BrainBox.Views
{
    /// <summary>
    /// Interaction logic for CombinationWindow.xaml
    /// </summary>
    public partial class CombinationWindow : Window
    {
        PlayerProfile profile;
        List<string> simboli = new List<string> { "★", "♥", "♠", "♦", "♣", "●" };
        List<string> tajnaKomb = new List<string>();
        List<string> pogoduvanje = new List<string>();
        int obidiLeft = 6;
        int score = 0;
        int timeLeft = 60;
        DispatcherTimer timer;

        public CombinationWindow(PlayerProfile playerProfile)
        {
            InitializeComponent();
            profile = playerProfile;

            GenerateSecret();
            BuildSymbolButtons();
            UpdateCurrentGuessUI();
            UpdateUI();
            StartTimer();
        }

        private void GenerateSecret()
        {
            Random rnd = new Random();
            tajnaKomb = new List<string>();

            List<string> kopija = new List<string>(simboli);
            for (int i = 0; i < 4; i++)
            {
                int index = rnd.Next(kopija.Count);
                tajnaKomb.Add(kopija[index]);
                kopija.RemoveAt(index);
            }
        }

        private void BuildSymbolButtons()
        {
            Grid grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());

            for (int i = 0; i < simboli.Count; i++)
            {
                int row = i / 3;
                int col = i % 3;

                TextBlock tb = new TextBlock();
                tb.Text = simboli[i];
                tb.FontSize = 20;
                tb.Foreground = Brushes.White;
                tb.HorizontalAlignment = HorizontalAlignment.Center;
                tb.VerticalAlignment = VerticalAlignment.Center;

                Button btn = new Button();
                btn.Width = 55;
                btn.Height = 55;
                btn.Margin = new Thickness(2);
                btn.Background = new SolidColorBrush(Color.FromRgb(70, 70, 70));
                btn.Content = tb;
                btn.Tag = simboli[i];
                btn.Click += SymbolClick;

                Grid.SetRow(btn, row);
                Grid.SetColumn(btn, col);
                grid.Children.Add(btn);
            }

            symbolPanel.Children.Add(grid);
        }

        private void SymbolClick(object sender, RoutedEventArgs e)
        {
            if (pogoduvanje.Count >= 4) return;

            Button btn = (Button)sender;
            string simbol = btn.Tag.ToString();
            pogoduvanje.Add(simbol);
            UpdateCurrentGuessUI();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ConfirmGuess(null, null);
                return;
            }

            if (e.Key == Key.Back)
            {
                if (pogoduvanje.Count > 0)
                {
                    pogoduvanje.RemoveAt(pogoduvanje.Count - 1);
                    UpdateCurrentGuessUI();
                }
                return;
            }

            if (e.Key == Key.Delete)
            {
                pogoduvanje.Clear();
                UpdateCurrentGuessUI();
                return;
            }

            int index = -1;
            if (e.Key == Key.D1) index = 0;
            else if (e.Key == Key.D2) index = 1;
            else if (e.Key == Key.D3) index = 2;
            else if (e.Key == Key.D4) index = 3;
            else if (e.Key == Key.D5) index = 4;
            else if (e.Key == Key.D6) index = 5;

            if (index >= 0 && pogoduvanje.Count < 4)
            {
                pogoduvanje.Add(simboli[index]);
                UpdateCurrentGuessUI();
            }
        }

        private void RemoveLast(object sender, RoutedEventArgs e)
        {
            if (pogoduvanje.Count > 0)
            {
                pogoduvanje.RemoveAt(pogoduvanje.Count - 1);
                UpdateCurrentGuessUI();
            }
        }

        private void ClearGuess(object sender, RoutedEventArgs e)
        {
            pogoduvanje.Clear();
            UpdateCurrentGuessUI();
        }

        private void UpdateCurrentGuessUI()
        {
            currentGuess.Children.Clear();

            for (int i = 0; i < 4; i++)
            {
                TextBlock tb = new TextBlock();
                tb.FontSize = 20;
                tb.Foreground = Brushes.White;
                tb.HorizontalAlignment = HorizontalAlignment.Center;
                tb.VerticalAlignment = VerticalAlignment.Center;

                if (i < pogoduvanje.Count)
                    tb.Text = pogoduvanje[i];
                else
                    tb.Text = "";

                Border border = new Border();
                border.Width = 44;
                border.Height = 44;
                border.Margin = new Thickness(2);
                border.Child = tb;

                if (i < pogoduvanje.Count)
                    border.Background = new SolidColorBrush(Color.FromRgb(70, 70, 100));
                else
                    border.Background = new SolidColorBrush(Color.FromRgb(35, 35, 55));

                currentGuess.Children.Add(border);
            }
        }

        private void ConfirmGuess(object sender, RoutedEventArgs e)
        {
            if (pogoduvanje.Count < 4)
            {
                MessageBox.Show("Избери 4 симболи!", "BrainBox");
                return;
            }

            AddAttemptRow();
            obidiLeft--;

            bool tocno = true;
            for (int i = 0; i < 4; i++)
            {
                if (pogoduvanje[i] != tajnaKomb[i])
                {
                    tocno = false;
                    break;
                }
            }

            if (tocno)
            {
                timer.Stop();
                score = CalculateScore();
                EndGame(true);
                return;
            }

            if (obidiLeft <= 0)
            {
                timer.Stop();
                EndGame(false);
                return;
            }

            pogoduvanje.Clear();
            UpdateCurrentGuessUI();
            UpdateUI();
        }

        private void AddAttemptRow()
        {
            StackPanel panel = new StackPanel();
            panel.Orientation = Orientation.Horizontal;
            panel.Margin = new Thickness(0, 5, 0, 5);

            for (int i = 0; i < 4; i++)
            {
                string sim = pogoduvanje[i];
                Color boja;

                if (sim == tajnaKomb[i])
                    boja = Colors.Green;
                else if (tajnaKomb.Contains(sim))
                    boja = Colors.Goldenrod;
                else
                    boja = Colors.Red;

                TextBlock tb = new TextBlock();
                tb.Text = sim;
                tb.FontSize = 22;
                tb.Foreground = Brushes.White;
                tb.HorizontalAlignment = HorizontalAlignment.Center;
                tb.VerticalAlignment = VerticalAlignment.Center;

                Border border = new Border();
                border.Width = 50;
                border.Height = 50;
                border.Margin = new Thickness(3);
                border.Background = new SolidColorBrush(boja);
                border.Child = tb;

                panel.Children.Add(border);
            }

            attemptsPanel.Children.Add(panel);
        }

        private int CalculateScore()
        {
            int bonusObidi = obidiLeft * 2;
            int bonusVreme = timeLeft / 5;
            int vkupno = 10 + bonusObidi + bonusVreme;

            if (vkupno > 20) vkupno = 20;
            return vkupno;
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

            if (timeLeft <= 0)
            {
                timer.Stop();
                EndGame(false);
            }
        }

        private void UpdateUI()
        {
            txtTimer.Text = timeLeft + "s";
            txtAttempts.Text = "Обиди: " + obidiLeft + " / 6";
            txtScore.Text = "Поени: " + score;
        }

        private void EndGame(bool won)
        {
            string tajnaStr = "";
            for (int i = 0; i < tajnaKomb.Count; i++)
            {
                tajnaStr = tajnaStr + tajnaKomb[i] + " ";
            }

            string msg;
            if (won)
                msg = "Точно! Освоивте " + score + " / 20 поени!";
            else
                msg = "Играта заврши! Тајната комбинација беше: " + tajnaStr;

            profile.Scores["Kombinacija"].UpdateScore(score);
            ScoreManager.Save(profile);

            MessageBox.Show(msg, "Комбинација - Крај");
            Close();
        }
    }
}
