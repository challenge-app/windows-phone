using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Media;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using ChallengeApp.Controllers;
using Newtonsoft.Json; //preload?

namespace ChallengeApp {
    public partial class Login : PhoneApplicationPage
    {
        private bool creatingAccount = false;

        public Login() {
            InitializeComponent();

            // colors
            ((SolidColorBrush)Resources["PhoneAccentBrush"]).Color = Color.FromArgb(0xFF, 0x00, 0xAB, 0xA9);
            ((SolidColorBrush)Resources["PhoneTextBoxEditBackgroundBrush"]).Color = Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF);

            Loaded += Login_Loaded;
            Unloaded += Login_Unloaded;
        }

        void Login_Loaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Login page loaded.");
            UserController.Instance.UserLoggedIn += OnLogin;

            //preload
            JsonConvert.DeserializeObject<Object>(@"{}");
        }

        void Login_Unloaded(object sender, RoutedEventArgs e)
        {
            UserController.Instance.UserLoggedIn -= OnLogin;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            while (NavigationService.CanGoBack) NavigationService.RemoveBackEntry();
        }

        private void OnLogin(object sender, EventArgs args)
        {
            NavigationService.Navigate(new Uri(App.VIEW_MAIN, UriKind.Relative));
            //while (NavigationService.CanGoBack) NavigationService.RemoveBackEntry();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            creatingAccount = false;
            DoLogin();
        }

        private void CreateAccountButton_Click(object sender, RoutedEventArgs e)
        {
            creatingAccount = true;
            CreateAccount();
        }

        private void LoginInput_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter && (sender as TextBox).Text !="" ) PasswordInput.Focus();
        }

        private void PasswordInput_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter && (sender as TextBox).Text != "")
            {
                if (creatingAccount) CreateAccount(); else DoLogin();
            }
        }

        private bool ValidateForm()
        {
            if (this.LoginInput.Text == "") this.LoginInput.Focus();
            else if (this.PasswordInput.Text == "") this.PasswordInput.Focus();
            else return true;

            return false;
        }

        private void DoLogin()
        {
            if (!ValidateForm()) return;

            UserController.Instance.Login(this.LoginInput.Text, this.PasswordInput.Text);
            this.LoginButton.Focus();
        }

        private void CreateAccount()
        {
            if (!ValidateForm()) return;

            UserController.Instance.CreateAccount(this.LoginInput.Text, this.PasswordInput.Text);
            this.CreateAccountButton.Focus();
        }
    }
}