using BrainBox.Models;
using BrainBox.Services;
using Microsoft.VisualBasic;
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
    /// Interaction logic for MainMenuWindow.xaml
    /// </summary>
    public partial class MainMenuWindow : Window
    {
        private PlayerProfile profile;

        public MainMenuWindow()
        {
            InitializeComponent();
            profile = ScoreManager.Load();

            if (profile.Name == "Player")
            {
                string name = Interaction.InputBox("Внеси го твоето име:", "BrainBox", "Player");
                if (name != "")
                {
                    profile.Name = name;
                }
                ScoreManager.Save(profile);
            }

            txtPlayerName.Text = "Добредојде, " + profile.Name + "!";
        }



        private void StartGame(object sender, RoutedEventArgs e)
        {
            WordleWindow wordle = new WordleWindow(profile);
            wordle.ShowDialog();
            ScoreManager.Save(profile);

            MathWindow math = new MathWindow(profile);
            math.ShowDialog();
            ScoreManager.Save(profile);

            CombinationWindow combo = new CombinationWindow(profile);
            combo.ShowDialog();
            ScoreManager.Save(profile);

            QuizWindow quiz = new QuizWindow(profile);
            quiz.ShowDialog();
            ScoreManager.Save(profile);

            AssociationWindow assoc = new AssociationWindow(profile);
            assoc.ShowDialog();
            ScoreManager.Save(profile);

            MessageBox.Show("Сесијата е завршена! Провери ја статистиката.", "BrainBox");
        }

        private void OpenStats(object sender, RoutedEventArgs e)
        {
            StatsWindow stats = new StatsWindow(profile);
            stats.ShowDialog();
        }
    }
}
