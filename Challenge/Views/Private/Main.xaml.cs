using System;
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
    public partial class Main : PhoneApplicationPage
    {
        // Constructor
        public Main()
        {
            InitializeComponent();

            // ApplicationBar
            BuildLocalizedApplicationBar();

            Loaded += Main_Loaded;
            Unloaded += Main_Unloaded;
        }

        void Main_Loaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Main page loaded.");

            // Login stuff
            UserController.Instance.MustBeLogged();
            if (!UserController.IsLogged) return;

            // Feed
            this.FeedContent.ItemsSource = FeedController.Instance.Feed;
            if (!FeedController.Instance.isFeedLoaded) FeedController.Instance.LoadFeed();

            // Notifications
            this.NotificationsContent.ItemsSource = FeedController.Instance.Feed;
            if (!FeedController.Instance.isFeedLoaded) FeedController.Instance.LoadFeed();
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

        // Building a localized ApplicationBar
        private void BuildLocalizedApplicationBar()
        {
            ApplicationBarIconButton appBarButton;
            ApplicationBarMenuItem appBarMenuItem;

            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            ApplicationBar = new ApplicationBar();

            // Appearence
            ApplicationBar.BackgroundColor = (Color)App.Current.Resources["AppBarBackgroundColor"];
            ApplicationBar.ForegroundColor = (Color)App.Current.Resources["AppBarForegroundColor"];

            appBarButton = new ApplicationBarIconButton(new Uri("/Assets/Icons/refresh.png", UriKind.Relative));
            appBarButton.Text = AppResources.Search;
            appBarButton.Click += appBarRefreshClick;
            ApplicationBar.Buttons.Add(appBarButton);

            //appBarButton = new ApplicationBarIconButton(new Uri("/Assets/Icons/archive.png", UriKind.Relative));
            //appBarButton.Text = AppResources.Archive;
            //appBarButton.Click += appBarArchiveClick;
            //ApplicationBar.Buttons.Add(appBarButton);

            // Create a new button and set the text value to the localized string from AppResources.
            appBarButton = new ApplicationBarIconButton(new Uri("/Assets/Icons/new.png", UriKind.Relative));
            appBarButton.Text = AppResources.NewChallenge;
            appBarButton.Click += appBarNewChallengeClick;
            ApplicationBar.Buttons.Add(appBarButton);

            //appBarButton = new ApplicationBarIconButton(new Uri("/Assets/Icons/like.png", UriKind.Relative));
            //appBarButton.Text = AppResources.Like;
            //appBarButton.Click += appBarLikeClick;
            //ApplicationBar.Buttons.Add(appBarButton);

            appBarButton = new ApplicationBarIconButton(new Uri("/Assets/Icons/search.png", UriKind.Relative));
            appBarButton.Text = AppResources.Search;
            appBarButton.Click += appBarSearchClick;
            ApplicationBar.Buttons.Add(appBarButton);

            // Create a new menu item with the localized string from AppResources.
            //appBarMenuItem = new ApplicationBarMenuItem(AppResources.Refresh);
            //appBarMenuItem.Click += appBarRefreshClick;
            //ApplicationBar.MenuItems.Add(appBarMenuItem);

            //appBarMenuItem = new ApplicationBarMenuItem(AppResources.MyProfile);
            //appBarMenuItem.Click += appBarProfileClick;
            //ApplicationBar.MenuItems.Add(appBarMenuItem);

            appBarMenuItem = new ApplicationBarMenuItem(AppResources.Settings);
            appBarMenuItem.Click += appBarSettingsClick;
            ApplicationBar.MenuItems.Add(appBarMenuItem);
        }

        // app bar buttons
        void appBarSearchClick(object sender, EventArgs e) { NavigationService.Navigate(new Uri(App.VIEW_SEARCH, UriKind.Relative)); }
        void appBarNewChallengeClick(object sender, EventArgs e) { NavigationService.Navigate(new Uri(App.VIEW_NEW_CHALLENGE, UriKind.Relative)); }
        //void appBarLikeClick(object sender, EventArgs e) { }

        void appBarRefreshClick(object sender, EventArgs e)
        {
            //if(FeedContent.SelectedIndex >= 0) FeedContent.DefaultItem = FeedContent.Items[FeedContent.SelectedIndex]; 
            FeedController.Instance.UpdateFeed();
        }


        void appBarArchiveClick(object sender, EventArgs e)
        {
            if (FeedContent.SelectedIndex >= 0)
            {
                int newIndex = FeedContent.SelectedIndex;
                FeedController.Instance.Feed.RemoveAt(FeedContent.SelectedIndex);

                if (FeedController.Instance.Feed.Count > 1)
                {
                    if (newIndex >= FeedController.Instance.Feed.Count - 1) newIndex = FeedController.Instance.Feed.Count - 1;
                    if (newIndex <= 0) newIndex = 0;

                    //FeedContent.DefaultItem = FeedContent.Items[newIndex];
                }
            }
        }

        // app bar menu items
        //void appBarProfileClick(object sender, EventArgs e) { NavigationService.Navigate(new Uri(App.VIEW_PROFILE, UriKind.Relative)); }
        void appBarSettingsClick(object sender, EventArgs e) { NavigationService.Navigate(new Uri(App.VIEW_SETTINGS, UriKind.Relative)); }

        //private void FeedContent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
            //Thread.Sleep(300);
            //salva para o panorama nao mudar de item quando o ItemsSource for atualizado (ex: receber property change)
            //FeedContent.DefaultItem = FeedContent.SelectedIndex < 0 ? null : FeedContent.Items[FeedContent.SelectedIndex]; 
        //}
    }
}