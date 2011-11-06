using System;
using System.IO.IsolatedStorage;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Windows.Data;
using System.Globalization;

namespace xomango
{
    public partial class MainPage : PhoneApplicationPage
    {
        GameComponents.GameStatistics stat = null;
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            playerCheckBox.IsChecked = true;
        }

        void updateButtonsTag(Button btn, GameComponents.DifficultyLevel level)
        {
            var easyStat = stat[level];
            if (easyStat.Count > 0)
            {
                btn.Tag = string.Format("{0} played: {1}%",
                                               easyStat.Count,
                                               (int)100 * easyStat.Victory / easyStat.Count);
            }
            else
            {
                btn.Tag = "not yet played";
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            stat = new GameComponents.GameStatistics();
            updateButtonsTag(hyperlinkEasy, GameComponents.DifficultyLevel.EASY);
            updateButtonsTag(hyperlinkHard, GameComponents.DifficultyLevel.HARD);

            updateStatistics();
            updateProgress();

            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            if (settings.Contains("Saved") && settings["Saved"].ToString() == true.ToString())
            {
                hyperlinkResume.Visibility = System.Windows.Visibility.Visible;

                if (settings.Contains("SavedTime"))
                {
                    hyperlinkResume.Tag = settings["SavedTime"];
                }
            }
            else
            {
                hyperlinkResume.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void playerCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;

            if ((bool)cb.IsChecked)
            {
                cb.Content = "Cross";
            }
            else
            {
                cb.Content = "Zero";            
            }
        }

        private void hyperlinkResume_Click(object sender, RoutedEventArgs e)
        {
            startGamePage(true);
        }

        private void hyperlinkEasy_Click(object sender, RoutedEventArgs e)
        {
            startGamePage(false, "easy");
        }

        private void hyperlinkHard_Click(object sender, RoutedEventArgs e)
        {
            startGamePage(false, "hard");
        }


        private void startGamePage(bool resume, string ai="")
        {
            string uriString = String.Format("/GamePage.xaml?resume={0}&side={1}",
                                               resume, playerCheckBox.Content);
            if (!resume && ai != "")
            {
                uriString += String.Format("&ai={0}", ai);
            }
            Uri uri = new Uri(uriString, UriKind.Relative);
            System.Diagnostics.Debug.WriteLine(uri.ToString());
            NavigationService.Navigate(uri);
        }


        private string format(int x)
        {
            if (x <= 0) return "-";
            return x.ToString();
        }

        private string format(double x)
        {
            if (x <= 0.001 || double.IsNaN(x) ) return "-";
            return x.ToString("0.0");
        }



        private void updateStatistics()
        {
            txtAll.Text = format(stat.Overall.Count);
            txtWin.Text = format(stat.Overall.Victory);
            txtAvgTurns.Text = format((double)stat.Overall.TurnsAmount / stat.Overall.Count);

            var easy = stat[GameComponents.DifficultyLevel.EASY];
            txtEasyAll.Text = format(easy.Count);
            txtEasyWin.Text = format(easy.Victory);
            txtEasyAvgTurns.Text = format((double)easy.TurnsAmount / easy.Count);

            var hard = stat[GameComponents.DifficultyLevel.HARD];
            
            txtHardAll.Text = format(hard.Count);
            txtHardWin.Text = format(hard.Victory);
            txtHardAvgTurns.Text = format((double)hard.TurnsAmount / hard.Count);
        }





        //(x-1)^2 + (y)^2 = 1

        private void updateProgress()
        {

            int easy_win = stat[GameComponents.DifficultyLevel.EASY].Victory;
            int hard_win = stat[GameComponents.DifficultyLevel.HARD].Victory;

            double easy_way = Math.Min(1.0, easy_win / 100.0);
            double hard_way = Math.Min(1.0, hard_win / 50.0);

            double x = Math.Min(1.0,  0.5*easy_way + hard_way);

            double dprogress = 1 - (x - 1) * (x - 1);
            progressBar1.Value = (int)(320 * dprogress);
            progressText.Text = ((int)(100 * dprogress)).ToString();
        }
    }
}