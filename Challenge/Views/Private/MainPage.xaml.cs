﻿using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using ChallengeApp.Resources;
using ChallengeApp.Controllers;
using ChallengeApp.Models;

namespace ChallengeApp {
    public partial class MainPage : PhoneApplicationPage
    {
        private bool isNotificationsLoaded = false;
        private bool isFeedLoaded = false;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            
            // ApplicationBar
            BuildLocalizedApplicationBar();

            // 
            //this.FeedContent.ItemsSource = FeedController.Instance.Feed;

            Loaded += Main_Loaded;
            Unloaded += Main_Unloaded;
        }

        void Main_Loaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Main page loaded.");

            ChallengeController.Instance.ChallengeCreated += Instance_ChallengeCreated;
            ChallengeController.Instance.ChallengeAccepted += Instance_ChallengeAccepted;

            // Login stuff
            UserController.Instance.MustBeLogged();
            if (!UserController.IsLogged) return;

            // Load content
            if (!isFeedLoaded) LoadFeed();
            if (!isNotificationsLoaded) LoadNotifications();
        }

        void Instance_ChallengeAccepted(object sender, EventArgs e)
        {
            LoadFeed();
        }

        void Instance_ChallengeCreated(object sender, EventArgs e)
        {
            LoadFeed();
        }

        void Main_Unloaded(object sender, RoutedEventArgs e)
        {
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            while (NavigationService.CanGoBack) NavigationService.RemoveBackEntry();
        }

        private void Feed_ImageOpened(object sender, RoutedEventArgs e)
        {
            (sender as Image).Visibility = Visibility.Visible;
        }

        private async void LoadFeed()
        {
            VisualStateManager.GoToState(this, "LoadingFeed", false);

            var Feed = await FeedController.Instance.GetFeed();
            FeedContent.ItemsSource = Feed;
            isFeedLoaded = true;

            VisualStateManager.GoToState(this, "LoadedFeed", false);
            VisualStateManager.GoToState(this, Feed.Count > 0 ? "HasFeed" : "EmptyFeed", false);
        }

        private async void LoadNotifications()
        {
            VisualStateManager.GoToState(this, "LoadingNotifications", false);

            var Notifications = await ChallengeController.Instance.GetReceivedChallenges();
            NotificationsContent.ItemsSource = Notifications;
            isNotificationsLoaded = true;

            VisualStateManager.GoToState(this, "LoadedNotifications", false);
            VisualStateManager.GoToState(this, Notifications.Count > 0 ? "HasNotifications" : "EmptyNotification", false);
        }

        // Building a localized ApplicationBar
        private void BuildLocalizedApplicationBar()
        {
            //ApplicationBarIconButton appBarButton;
            ApplicationBarMenuItem appBarMenuItem;
            ApplicationBarIconButton appBarButton;

            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            ApplicationBar = new ApplicationBar();

            // Appearence
            //ApplicationBar.Mode = ApplicationBarMode.Minimized;
            ApplicationBar.BackgroundColor = (Color)App.Current.Resources["AppBarBackgroundColor"];
            ApplicationBar.ForegroundColor = (Color)App.Current.Resources["AppBarForegroundColor"];

            appBarButton = new ApplicationBarIconButton(new Uri("/Assets/Icons/refresh.png", UriKind.Relative));
            appBarButton.Text = AppResources.Refresh;
            appBarButton.Click += appBarRefreshClick;
            ApplicationBar.Buttons.Add(appBarButton);

            // Create a new button and set the text value to the localized string from AppResources.
            appBarButton = new ApplicationBarIconButton(new Uri("/Assets/Icons/new.png", UriKind.Relative));
            appBarButton.Text = AppResources.NewChallenge;
            appBarButton.Click += appBarNewChallengeClick;
            ApplicationBar.Buttons.Add(appBarButton);

            appBarButton = new ApplicationBarIconButton(new Uri("/Assets/Icons/search.png", UriKind.Relative));
            appBarButton.Text = AppResources.Search;
            appBarButton.Click += appBarSearchClick;
            ApplicationBar.Buttons.Add(appBarButton);

            appBarMenuItem = new ApplicationBarMenuItem(AppResources.MyProfile);
            appBarMenuItem.Click += appBarMyProfileClick;
            ApplicationBar.MenuItems.Add(appBarMenuItem);

            //appBarMenuItem = new ApplicationBarMenuItem(AppResources.Unmute);
            //appBarMenuItem.Click += appBarToggleMuteClick;
            //ApplicationBar.MenuItems.Add(appBarMenuItem);

            appBarMenuItem = new ApplicationBarMenuItem(AppResources.Settings);
            appBarMenuItem.Click += appBarSettingsClick;
            ApplicationBar.MenuItems.Add(appBarMenuItem);
        }

        // app bar buttons
        void appBarToggleMuteClick(object sender, EventArgs e)
        {
            // IsMuted = true;
        }
        void appBarSearchClick(object sender, EventArgs e)
        {

            NavigationService.Navigate(new Uri(App.VIEW_SEARCH, UriKind.Relative)); 
        }
        void appBarNewChallengeClick(object sender, EventArgs e) { NavigationService.Navigate(new Uri(App.VIEW_NEW_CHALLENGE, UriKind.Relative)); }

        void appBarRefreshClick(object sender, EventArgs e)
        {
            //if(FeedContent.SelectedIndex >= 0) FeedContent.DefaultItem = FeedContent.Items[FeedContent.SelectedIndex]; 

            Debug.WriteLine((MainPivot.SelectedItem as PivotItem).Tag);
            switch ((MainPivot.SelectedItem as PivotItem).Tag.ToString())
            {
                case "profile":
                    UserController.Instance.UpdateLoggedUserData();
                    break;

                case "notifications":
                    LoadNotifications();
                    break;

                default:
                    if (UserController.Instance.LoggedUser == null) UserController.Instance.UpdateLoggedUserData();
                    LoadFeed();
                    break;
            }
        }


        //void appBarArchiveClick(object sender, EventArgs e)
        //{
        //    if (FeedContent.SelectedIndex >= 0)
        //    {
        //        int newIndex = FeedContent.SelectedIndex;
        //        FeedController.Instance.Feed.RemoveAt(FeedContent.SelectedIndex);

        //        if (FeedController.Instance.Feed.Count > 1)
        //        {
        //            if (newIndex >= FeedController.Instance.Feed.Count - 1) newIndex = FeedController.Instance.Feed.Count - 1;
        //            if (newIndex <= 0) newIndex = 0;

        //            //FeedContent.DefaultItem = FeedContent.Items[newIndex];
        //        }
        //    }
        //}

        void appBarSettingsClick(object sender, EventArgs e) {
            NavigationService.Navigate(new Uri(App.VIEW_SETTINGS, UriKind.Relative)); 
        }

        //private void FeedContent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
            //Thread.Sleep(300);
            //salva para o panorama nao mudar de item quando o ItemsSource for atualizado (ex: receber property change)
            //FeedContent.DefaultItem = FeedContent.SelectedIndex < 0 ? null : FeedContent.Items[FeedContent.SelectedIndex]; 
        //}

        private void appBarMyProfileClick(object sender, EventArgs e)
        {
            var user = UserController.Instance.LoggedUser;
            if (user == null || String.IsNullOrEmpty(user.id)) return;

            App.UserProfileObject = user;
            (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri(String.Format(App.VIEW_PROFILE, user.id), UriKind.Relative));
        }

        private void UserProfileClick(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var user = (sender as FrameworkElement).DataContext as User;
            if (user == null || String.IsNullOrEmpty(user.id)) return;

            App.UserProfileObject = user;
            (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri(String.Format(App.VIEW_PROFILE, user.id), UriKind.Relative));
        }

        private void DisableSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            (sender as ListBox).SelectedIndex = -1;
        }
    }
}