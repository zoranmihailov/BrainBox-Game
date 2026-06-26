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
    /// Interaction logic for StatsWindow.xaml
    /// </summary>
    public partial class StatsWindow : Window
    {
        private PlayerProfile _profile;

        public StatsWindow(PlayerProfile profile)
        {
            InitializeComponent();
            _profile = profile;
            LoadStats();
        }

        private void LoadStats()
        {
            txtPlayerName.Text = $"Играч: {_profile.Name}";

            var s = _profile.Scores;

            txtWordleMax.Text = s["Wordle"].MaxScore.ToString();
            txtWordleHigh.Text = s["Wordle"].HighScore.ToString();
            txtWordleAvg.Text = string.Format("{0:F1}", s["Wordle"].Average);
            txtWordleGames.Text = s["Wordle"].GamesPlayed.ToString();
            txtWordleLast.Text = s["Wordle"].LastScore.ToString();

            txtMathMax.Text = s["Matematika"].MaxScore.ToString();
            txtMathHigh.Text = s["Matematika"].HighScore.ToString();
            txtMathAvg.Text = string.Format("{0:F1}", s["Matematika"].Average);
            txtMathGames.Text = s["Matematika"].GamesPlayed.ToString();
            txtMathLast.Text = s["Matematika"].LastScore.ToString();

            txtCombMax.Text = s["Kombinacija"].MaxScore.ToString();
            txtCombHigh.Text = s["Kombinacija"].HighScore.ToString();
            txtCombAvg.Text = string.Format("{0:F1}", s["Kombinacija"].Average);
            txtCombGames.Text = s["Kombinacija"].GamesPlayed.ToString();
            txtCombLast.Text = s["Kombinacija"].LastScore.ToString();

            txtQuizMax.Text = s["Prasanja"].MaxScore.ToString();
            txtQuizHigh.Text = s["Prasanja"].HighScore.ToString();
            txtQuizAvg.Text = string.Format("{0:F1}", s["Prasanja"].Average);
            txtQuizGames.Text = s["Prasanja"].GamesPlayed.ToString();
            txtQuizLast.Text = s["Prasanja"].LastScore.ToString();

            txtAssocMax.Text = s["Asocijacija"].MaxScore.ToString();
            txtAssocHigh.Text = s["Asocijacija"].HighScore.ToString();
            txtAssocAvg.Text = string.Format("{0:F1}", s["Asocijacija"].Average);
            txtAssocGames.Text = s["Asocijacija"].GamesPlayed.ToString();
            txtAssocLast.Text = s["Asocijacija"].LastScore.ToString();

            int wordleHigh = s["Wordle"].HighScore;
            int mathHigh = s["Matematika"].HighScore;
            int comboHigh = s["Kombinacija"].HighScore;
            int quizHigh = s["Prasanja"].HighScore;
            int assocHigh = s["Asocijacija"].HighScore;

            int total = wordleHigh + mathHigh + comboHigh + quizHigh + assocHigh;

            int wordleMax = s["Wordle"].MaxScore;
            int mathMax = s["Matematika"].MaxScore;
            int comboMax = s["Kombinacija"].MaxScore;
            int quizMax = s["Prasanja"].MaxScore;
            int assocMax = s["Asocijacija"].MaxScore;

            int maxTotal = wordleMax + mathMax + comboMax + quizMax + assocMax;

            int vkupnoLast = s["Wordle"].LastScore + s["Matematika"].LastScore + 
                s["Kombinacija"].LastScore + s["Prasanja"].LastScore +s["Asocijacija"].LastScore;

            txtTotalLast.Text = $"{vkupnoLast} / {maxTotal}";

            txtTotal.Text = $"{total} / {maxTotal}";
        }
    }
}
