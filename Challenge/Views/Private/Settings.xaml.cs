using System;
//using System.Collections.Generic;
using System.Diagnostics;
using System.IO.IsolatedStorage;
//using System.Linq;
//using System.Net;
//using System.Windows;
//using System.Windows.Controls;
using System.Windows.Media;
//using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using ChallengeApp.Controllers;
using ChallengeApp.Models;
using ChallengeApp.Resources;
using ChallengeApp.Utils;

namespace ChallengeApp.Views.Private
{
    public partial class Settings : PhoneApplicationPage
    {
        AppSettings settings = AppSettings.Instance;

        public Settings()
        {
            InitializeComponent();

            // colors
            ((SolidColorBrush)Resources["PhoneAccentBrush"]).Color = Color.FromArgb(0xFF, 0x00, 0xAB, 0xA9);
            ((SolidColorBrush)Resources["PhoneTextBoxEditBackgroundBrush"]).Color = Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF);

            // ApplicationBar
            BuildLocalizedApplicationBar();

            Loaded += Settings_Loaded;
        }

        void Settings_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Debug.WriteLine("Settings page loaded.");
        }

        // Building a localized ApplicationBar
        private void BuildLocalizedApplicationBar()
        {
            ApplicationBarIconButton appBarSaveButton;

            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            ApplicationBar = new ApplicationBar();

            // Appearence
            ApplicationBar.BackgroundColor = (Color)App.Current.Resources["AppConfigBackgroundColor"];
            ApplicationBar.ForegroundColor = (Color)App.Current.Resources["AppConfigForegroundColor"];

            // Create a new button and set the text value to the localized string from AppResources.
            appBarSaveButton = new ApplicationBarIconButton(new Uri("/Assets/Icons/save.png", UriKind.Relative));
            appBarSaveButton.Text = AppResources.Save;
            appBarSaveButton.Click += new EventHandler(doneButton_Click);
            ApplicationBar.Buttons.Add(appBarSaveButton);
        }
        void doneButton_Click(object sender, EventArgs e)
        {
            // Try save in the cloud
            // ...

            settings.LoggedUser.firstName   = textBoxName.Text;
            settings.LoggedUser.username    = textBoxUsername.Text;
            settings.LoggedUser.email       = textBoxEmail.Text;

            NavigationService.GoBack();
        }

        private void LoginButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            UserController.Instance.Logout();
        }
    }
}