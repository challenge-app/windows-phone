using System;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using ChallengeApp.Resources;
using ChallengeApp.Controllers;
using ChallengeApp.Models;

namespace ChallengeApp.Views.Private
{
    public partial class SearchPage : PhoneApplicationPage
    {
        public static string Query;

        public SearchPage()
        {
            InitializeComponent();

            Loaded += Search_Loaded;
            Unloaded += Search_Unloaded;
        }

        void Search_Loaded(object sender, RoutedEventArgs e)
        {
            // Search Results
            //this.SearchResults.ItemsSource = UserController.Instance.SearchResults;
            if (String.IsNullOrEmpty(Query)) Search("");
        }

        void Search_Unloaded(object sender, RoutedEventArgs e)
        {
           
        }

        async void Search(string query) {
            Query = query;
            SearchResultsListBox.ItemsSource = await UserController.Instance.Find(query, true);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.textBoxQuery.Focus();
        }

        private void textBoxQuery_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter) // && (sender as TextBox).Text != ""
            {
                this.Focus();
                Search((sender as TextBox).Text);
            }
        }
    }
}