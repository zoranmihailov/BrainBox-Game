using BrainBox.Models;
using BrainBox.Services;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Threading;

namespace BrainBox.Views
{
    /// <summary>
    /// Interaction logic for WordleWindow.xaml
    /// </summary>
    public partial class WordleWindow : Window
    {
        PlayerProfile profile;
        List<string> words = new List<string>();
        string secretWord;
        string currentGuess = "";
        int currentRow = 0;
        int maxRows = 6;
        int wordLength = 10;
        int score = 0;
        int timeLeft = 90;
        DispatcherTimer timer;

        Border[][] grid;
        Dictionary<string, Border> keyButtons = new Dictionary<string, Border>();

        string[] keyboardRows = new string[]
        {
            "А Б В Г Д Ѓ Е Ж З Ѕ И",
            "Ј К Л Љ М Н Њ О П Р С",
            "Т Ќ У Ф Х Ц Ч Џ Ш"
        };

        public WordleWindow(PlayerProfile playerProfile)
        {
            InitializeComponent();
            profile = playerProfile;

            LoadWords();
            BuildGrid();
            BuildKeyboard();
            StartTimer();
        }

        private void LoadWords()
        {
            string[] lines = File.ReadAllLines("Data/words.txt");
            foreach (string line in lines)
            {
                string word = line.Trim().ToUpper();
                if (word.Length == wordLength)
                {
                    words.Add(word);
                }
            }

            Random rnd = new Random();
            int index = rnd.Next(words.Count);
            secretWord = words[index];
        }

        private void BuildGrid()
        {
            grid = new Border[maxRows][];

            for (int row = 0; row < maxRows; row++)
            {
                StackPanel rowPanel = new StackPanel();
                rowPanel.Orientation = Orientation.Horizontal;
                rowPanel.Margin = new Thickness(0, 3, 0, 3);

                grid[row] = new Border[wordLength];

                for (int col = 0; col < wordLength; col++)
                {
                    TextBlock letterText = new TextBlock();
                    letterText.FontSize = 20;
                    letterText.FontWeight = FontWeights.Bold;
                    letterText.Foreground = Brushes.White;
                    letterText.HorizontalAlignment = HorizontalAlignment.Center;
                    letterText.VerticalAlignment = VerticalAlignment.Center;

                    Border cell = new Border();
                    cell.Width = 52;
                    cell.Height = 52;
                    cell.Margin = new Thickness(2);
                    cell.Background = new SolidColorBrush(Color.FromRgb(70, 70, 70));
                    cell.Child = letterText;

                    grid[row][col] = cell;
                    rowPanel.Children.Add(cell);
                }

                gridPanel.Children.Add(rowPanel);
            }
        }

        private void BuildKeyboard()
        {
            for (int r = 0; r < keyboardRows.Length; r++)
            {
                StackPanel rowPanel = new StackPanel();
                rowPanel.Orientation = Orientation.Horizontal;
                rowPanel.HorizontalAlignment = HorizontalAlignment.Center;
                rowPanel.Margin = new Thickness(0, 3, 0, 3);

                string[] letters = keyboardRows[r].Split(' ');

                for (int i = 0; i < letters.Length; i++)
                {
                    string letter = letters[i];
                    if (letter == "") continue;

                    TextBlock keyText = new TextBlock();
                    keyText.Text = letter;
                    keyText.FontSize = 14;
                    keyText.FontWeight = FontWeights.Bold;
                    keyText.Foreground = Brushes.White;
                    keyText.HorizontalAlignment = HorizontalAlignment.Center;
                    keyText.VerticalAlignment = VerticalAlignment.Center;

                    Border keyBorder = new Border();
                    keyBorder.Width = 42;
                    keyBorder.Height = 42;
                    keyBorder.Margin = new Thickness(2);
                    keyBorder.Background = new SolidColorBrush(Color.FromRgb(120, 120, 120));
                    keyBorder.Child = keyText;
                    keyBorder.Cursor = Cursors.Hand;
                    keyBorder.MouseLeftButtonUp += KeyButton_Click;

                    keyButtons[letter] = keyBorder;
                    rowPanel.Children.Add(keyBorder);
                }

                keyboardPanel.Children.Add(rowPanel);
            }
        }

        private void KeyButton_Click(object sender, MouseButtonEventArgs e)
        {
            Border clicked = (Border)sender;
            TextBlock text = (TextBlock)clicked.Child;
            TypeLetter(text.Text);
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
                EndGame(false);
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            //MessageBox.Show(e.Key.ToString());
            if (e.Key == Key.Back)
            {
                if (currentGuess.Length > 0)
                {
                    currentGuess = currentGuess.Substring(0, currentGuess.Length - 1);
                }
                UpdateCurrentRow();
            }
            else if (e.Key == Key.Enter)
            {
                SubmitGuess();
            }
            else
            {
                string typed = KeyToMacedonianLetter(e.Key);
                if (typed != null)
                {
                    TypeLetter(typed);
                }
            }
        }

        private string KeyToMacedonianLetter(Key key)
        {
            switch (key)
            {
                case Key.A: return "А";
                case Key.B: return "Б";
                case Key.V: return "В";
                case Key.G: return "Г";
                case Key.D: return "Д";
                case Key.OemCloseBrackets: return "Ѓ";
                case Key.E: return "Е";
                case Key.Oem5: return "Ж";
                case Key.Z: return "З";
                case Key.Y: return "Ѕ";
                case Key.I: return "И";
                case Key.J: return "Ј";
                case Key.K: return "К";
                case Key.L: return "Л";
                case Key.Q: return "Љ";
                case Key.M: return "М";
                case Key.N: return "Н";
                case Key.W: return "Њ";
                case Key.O: return "О";
                case Key.P: return "П";
                case Key.R: return "Р";
                case Key.S: return "С";
                case Key.T: return "Т";
                case Key.OemQuotes: return "Ќ";
                case Key.U: return "У";
                case Key.F: return "Ф";
                case Key.H: return "Х";
                case Key.C: return "Ц";
                case Key.OemSemicolon: return "Ч";
                case Key.X: return "Џ";
                case Key.OemOpenBrackets: return "Ш";
                default: return null;
            }
        }

        private void TypeLetter(string letter)
        {
            if (currentGuess.Length < wordLength)
            {
                currentGuess = currentGuess + letter;
                UpdateCurrentRow();
            }
        }

        private void UpdateCurrentRow()
        {
            Border[] row = grid[currentRow];
            for (int i = 0; i < wordLength; i++)
            {
                TextBlock cellText = (TextBlock)row[i].Child;
                if (i < currentGuess.Length)
                {
                    cellText.Text = currentGuess[i].ToString();
                }
                else
                {
                    cellText.Text = "";
                }
            }
        }

        private void SubmitGuess()
        {
            if (currentGuess.Length < wordLength)
            {
                MessageBox.Show("Внеси 10 букви!", "Wordle");
                return;
            }

            Border[] row = grid[currentRow];

            for (int i = 0; i < wordLength; i++)
            {
                string letter = currentGuess[i].ToString();
                Color color;

                if (letter == secretWord[i].ToString())
                {
                    color = Colors.Green;
                    SetKeyColor(letter, Colors.Green);
                }
                else if (secretWord.Contains(letter))
                {
                    color = Colors.Goldenrod;
                    SetKeyColor(letter, Colors.Goldenrod);
                }
                else
                {
                    color = Color.FromRgb(80, 80, 80);
                    SetKeyColor(letter, Colors.Red);
                }

                row[i].Background = new SolidColorBrush(color);
            }

            if (currentGuess == secretWord)
            {
                timer.Stop();
                score = CalculateScore();
                EndGame(true);
                return;
            }

            currentRow++;
            currentGuess = "";

            if (currentRow >= maxRows)
            {
                timer.Stop();
                EndGame(false);
            }
        }

        private void SetKeyColor(string letter, Color color)
        {
            if (keyButtons.ContainsKey(letter))
            {
                Color current = ((SolidColorBrush)keyButtons[letter].Background).Color;

                if (current == Colors.Green) return;
                if (current == Colors.Goldenrod && color != Colors.Green) return;

                keyButtons[letter].Background = new SolidColorBrush(color);
            }
        }

        private int CalculateScore()
        {
            int attemptBonus = (maxRows - currentRow) * 2;
            int timeBonus = timeLeft / 5;
            int total = 8 + attemptBonus + timeBonus;

            if (total > 20) total = 20;
            return total;
        }

        private void EndGame(bool won)
        {
            string message;
            if (won)
            {
                message = "Точно! Зборот беше: " + secretWord + "\nОсвоивте " + score + " / 20 поени!";
            }
            else
            {
                message = "Играта заврши! Зборот беше: " + secretWord;
            }

            profile.Scores["Wordle"].UpdateScore(score);
            ScoreManager.Save(profile);

            MessageBox.Show(message, "Wordle - Крај");
            Close();
        }
    }
}
