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
    /// Interaction logic for AssociationWindow.xaml
    /// </summary>
    public partial class AssociationWindow : Window
    {
        PlayerProfile profile;
        AssociationData tekovnaAsocijacija;
        int score = 0;
        int timeLeft = 90;
        DispatcherTimer timer;
        List<TextBox> resenijaBoxovi = new List<TextBox>();
        bool[] kolonaResena;
        bool finalnotoReseno = false;

        public AssociationWindow(PlayerProfile playerProfile)
        {
            InitializeComponent();
            profile = playerProfile;

            LoadAssociation();
            BuildUI();
            StartTimer();

            txtFinalAnswer.KeyDown += TxtFinalAnswer_KeyDown;
        }

        private void TxtFinalAnswer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ConfirmFinal(null, null);
            }
        }

        private void LoadAssociation()
        {
            string json = File.ReadAllText("Data/associations.json");
            AssociationsRoot data = JsonSerializer.Deserialize<AssociationsRoot>(json);

            Random rnd = new Random();
            int index = rnd.Next(data.associations.Count);
            tekovnaAsocijacija = data.associations[index];

            kolonaResena = new bool[4];
        }

        private void BuildUI()
        {
            columnsGrid.Children.Clear();
            solutionsGrid.Children.Clear();
            resenijaBoxovi.Clear();

            for (int kol = 0; kol < 4; kol++)
            {
                StackPanel kolPanel = new StackPanel();
                kolPanel.Margin = new Thickness(4);

                AssociationColumn kolona = tekovnaAsocijacija.columns[kol];

                for (int i = 0; i < kolona.words.Count; i++)
                {
                    TextBlock tb = new TextBlock();
                    tb.Text = kolona.words[i];
                    tb.FontSize = 15;
                    tb.FontWeight = FontWeights.Bold;
                    tb.Foreground = Brushes.White;
                    tb.HorizontalAlignment = HorizontalAlignment.Center;
                    tb.VerticalAlignment = VerticalAlignment.Center;
                    tb.TextWrapping = TextWrapping.Wrap;
                    tb.TextAlignment = TextAlignment.Center;

                    Border border = new Border();
                    border.Height = 55;
                    border.Margin = new Thickness(2);
                    border.Background = new SolidColorBrush(Color.FromRgb(70, 100, 50));
                    border.Child = tb;

                    kolPanel.Children.Add(border);
                }

                columnsGrid.Children.Add(kolPanel);

                int zacuvanKol = kol;

                StackPanel reseniePanel = new StackPanel();
                reseniePanel.Margin = new Thickness(4);

                TextBox resenieBox = new TextBox();
                resenieBox.FontSize = 14;
                resenieBox.Height = 40;
                resenieBox.Padding = new Thickness(5);
                resenieBox.Tag = zacuvanKol;
                resenieBox.KeyDown += ResenieBox_KeyDown;

                resenijaBoxovi.Add(resenieBox);
                reseniePanel.Children.Add(resenieBox);

                Button potvrdiBtn = new Button();
                potvrdiBtn.Content = "✓";
                potvrdiBtn.Height = 35;
                potvrdiBtn.Margin = new Thickness(0, 4, 0, 0);
                potvrdiBtn.Background = new SolidColorBrush(Color.FromRgb(100, 160, 70));
                potvrdiBtn.Foreground = Brushes.White;
                potvrdiBtn.FontSize = 16;
                potvrdiBtn.Tag = zacuvanKol;
                potvrdiBtn.Click += PotvrdiBtn_Click;

                reseniePanel.Children.Add(potvrdiBtn);
                solutionsGrid.Children.Add(reseniePanel);
            }

            UpdateScoreUI();
        }

        private void ResenieBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox box = (TextBox)sender;
                int kol = (int)box.Tag;
                ProveriResenieKolona(box, kol);
            }
        }

        private void PotvrdiBtn_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            int kol = (int)btn.Tag;
            TextBox box = resenijaBoxovi[kol];
            ProveriResenieKolona(box, kol);
        }

        private void ProveriResenieKolona(TextBox box, int kol)
        {
            if (kolonaResena[kol]) return;

            string odgovor = box.Text.Trim().ToUpper();
            string tocen = tekovnaAsocijacija.columns[kol].solution.ToUpper();

            if (odgovor == tocen)
            {
                kolonaResena[kol] = true;
                score += 5;
                box.Background = new SolidColorBrush(Colors.Green);
                box.IsEnabled = false;
                UpdateScoreUI();

                bool site = true;
                for (int i = 0; i < 4; i++)
                {
                    if (!kolonaResena[i])
                    {
                        site = false;
                        break;
                    }
                }
                if (site)
                {
                    txtFinalAnswer.Focus();
                }
            }
            else
            {
                box.Background = new SolidColorBrush(Colors.DarkRed);
            }
        }

        private void ConfirmFinal(object sender, RoutedEventArgs e)
        {
            if (finalnotoReseno) return;

            string odgovor = txtFinalAnswer.Text.Trim().ToUpper();
            string tocen = tekovnaAsocijacija.finalAnswer.ToUpper();

            if (odgovor == tocen)
            {
                finalnotoReseno = true;

                for (int i = 0; i < 4; i++)
                {
                    if (!kolonaResena[i])
                    {
                        kolonaResena[i] = true;
                        score += 5;
                        resenijaBoxovi[i].Text = tekovnaAsocijacija.columns[i].solution;
                        resenijaBoxovi[i].Background = new SolidColorBrush(Colors.Green);
                        resenijaBoxovi[i].IsEnabled = false;
                    }
                }

                score += 20;
                txtFinalAnswer.Background = new SolidColorBrush(Colors.Green);
                UpdateScoreUI();
                timer.Stop();
                EndGame();
            }
            else
            {
                txtFinalAnswer.Background = new SolidColorBrush(Colors.DarkRed);
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
                EndGame();
            }
        }

        private void UpdateScoreUI()
        {
            txtScore.Text = "Поени: " + score + " / 40";
        }

        private void EndGame()
        {
            for (int i = 0; i < 4; i++)
            {
                if (!kolonaResena[i])
                {
                    resenijaBoxovi[i].Text = tekovnaAsocijacija.columns[i].solution;
                    resenijaBoxovi[i].Background = new SolidColorBrush(Colors.DarkRed);
                    resenijaBoxovi[i].IsEnabled = false;
                }
            }

            if (!finalnotoReseno)
            {
                txtFinalAnswer.Text = tekovnaAsocijacija.finalAnswer;
                txtFinalAnswer.Background = new SolidColorBrush(Colors.DarkRed);
            }

            profile.Scores["Asocijacija"].UpdateScore(score);
            ScoreManager.Save(profile);

            MessageBox.Show("Играта заврши!\nОсвоивте " + score + " / 40 поени.", "Асоцијација - Крај");
            Close();
        }
    }

    public class AssociationsRoot
    {
        public List<AssociationData> associations { get; set; }
    }

    public class AssociationData
    {
        public List<AssociationColumn> columns { get; set; }
        public string finalAnswer { get; set; }
    }

    public class AssociationColumn
    {
        public List<string> words { get; set; }
        public string solution { get; set; }
    }
}
