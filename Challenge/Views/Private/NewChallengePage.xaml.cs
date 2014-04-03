using System;
//using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

namespace ChallengeApp.Views.Private
{
    public partial class NewChallengePage : PhoneApplicationPage
    {
        public static bool IsFriendsLoaded { get; private set; }
        public static ObservableCollection<User> FriendsList = new ObservableCollection<User>();

        public NewChallengePage()
        {
            InitializeComponent();

            // colors
            ((SolidColorBrush)Resources["PhoneTextBoxEditBackgroundBrush"]).Color = Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF);

            // ApplicationBar
            BuildLocalizedApplicationBar();

            // initial stuff
            IsFriendsLoaded = false;
            LoadFriendsList();

            Loaded += NewChallenge_Loaded;
            Unloaded += NewChallenge_Unloaded;
        }

        void NewChallenge_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Debug.WriteLine("NewChallenge page loaded.");
            ChallengeController.Instance.ChallengeCreated += Instance_ChallengeCreated;
        }

        void NewChallenge_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            ChallengeController.Instance.ChallengeCreated -= Instance_ChallengeCreated;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            // Login stuff
            UserController.Instance.MustBeLogged();
            if (!UserController.IsLogged) return;

            // Friends
            if (!IsFriendsLoaded) LoadFriendsList();
        }

        async void LoadFriendsList()
        {
            if (UserController.Instance.LoggedUser == null || String.IsNullOrEmpty(UserController.Instance.LoggedUser.id)) return;
            FriendsList = await UserController.Instance.GetUserFollowingList(UserController.Instance.LoggedUser.id);
            this.FriendsListPicker.ItemsSource = FriendsList;
            IsFriendsLoaded = true;
        }

        void Instance_ChallengeCreated(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri(App.VIEW_MAIN, UriKind.Relative));
        }

        // Building a localized ApplicationBar
        private void BuildLocalizedApplicationBar()
        {
            ApplicationBarIconButton appBarButton;

            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            ApplicationBar = new ApplicationBar();

            // Appearence
            ApplicationBar.BackgroundColor = (Color)App.Current.Resources["AppBarBackgroundColor"];
            ApplicationBar.ForegroundColor = (Color)App.Current.Resources["AppBarForegroundColor"];

            // Create a new button and set the text value to the localized string from AppResources.
            appBarButton = new ApplicationBarIconButton(new Uri("/Assets/Icons/check.png", UriKind.Relative));
            appBarButton.Text = AppResources.ChallengeVerb + "!";
            appBarButton.Click += appBarButton_Click;
            ApplicationBar.Buttons.Add(appBarButton);
        }

        private bool ValidateForm()
        {
            if (this.TypeListPicker.SelectedIndex < 0 ||  (this.TypeListPicker.SelectedItem as ListPickerItem).Tag.ToString() == "") this.FriendsListPicker.Focus();
            else if (this.DescriptionInput.Text == "") this.DescriptionInput.Focus();
            else if (this.RewardInput.Text == "") this.RewardInput.Focus();
            else return true;

            return false;
        }


        void appBarButton_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                ChallengeController.Instance.CreateChallenge((FriendsListPicker.SelectedItem as User).id, DescriptionInput.Text, (TypeListPicker.SelectedItem as ListPickerItem).Tag.ToString(), Int32.Parse(RewardInput.Text));
            }
        }
    }
}