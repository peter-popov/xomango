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
    /// <summary>
    /// Converts bool? values to "Off" and "On" strings.
    /// </summary>
    public class OffOnToPlayerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == null)
            {
                throw new ArgumentNullException("targetType");
            }
            if (value.ToString() == "On") return "Cross";
            if (value.ToString() == "Off") return "Zero";
            throw new ArgumentException("Wrong value");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //if (targetType == null)
            //{
            //    throw new ArgumentNullException("targetType");
            //}
            //if (value.ToString() == "Cross") return "On";
            //if (value.ToString() == "Zero") return "Off";
            throw new ArgumentException("Wrong value");
        }
    }

    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            if (settings.Contains("Saved") && settings["Saved"].ToString() == true.ToString())
            {
                hyperlinkResume.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                hyperlinkResume.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
    }
}