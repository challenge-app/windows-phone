using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Media;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using ChallengeApp.Controllers;
using Coding4Fun.Toolkit.Controls;
using Newtonsoft.Json; //preload?

namespace ChallengeApp {
    public partial class LoginPage : PhoneApplicationPage
    {
        public List<string> MenuIcons = new List<string> { "ICONE1", "ICONE2" };

        //private bool IsCreateAccountOption { get { return CreateAccountButton.IsPressed; } }

        //public bool IsCreateAccountOption { get { return (bool)GetValue(isCreateAccountOption); } set { SetValue(isCreateAccountOption, true); SetValue(isLoginOption, false); } }
        //public static readonly DependencyProperty isCreateAccountOption = DependencyProperty.Register("IsCreateAccountOption", typeof(bool), typeof(PhoneApplicationPage), new PropertyMetadata(null));

        //public bool IsLoginOption { get { return (bool)GetValue(isLoginOption); } set { SetValue(isLoginOption, true); SetValue(isCreateAccountOption, false); } }
        //public static readonly DependencyProperty isLoginOption = DependencyProperty.Register("IsLoginOption", typeof(bool), typeof(PhoneApplicationPage), new PropertyMetadata(null));

        public LoginPage() {
            InitializeComponent();

            // colors
            //((SolidColorBrush)Resources["PhoneAccentBrush"]).Color = Color.FromArgb(0xFF, 0x1a, 0xbc, 0x9c); //Application.Current.Resources["AppBackgroundColor"];

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

        private void FacebookLoginButton_Click(object sender, RoutedEventArgs e)
        {
            // open facebook login...
        }

        private void CreateAccountButton_Click(object sender, RoutedEventArgs e)
        {
            //if ((sender as ToggleButtonBase).IsChecked == true) 
                CreateAccount();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            //if ((sender as ToggleButtonBase).IsChecked == true) 
                DoLogin();
        }

        private void LoginUsernameInput_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter && (sender as TextBox).Text !="" ) LoginPasswordInput.Focus();
        }

        private void LoginPasswordInput_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter && (sender as TextBox).Text != "") DoLogin();
        }

        private void CreateFirstNameInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && (sender as TextBox).Text != "") CreateLastNameInput.Focus();
        }

        private void CreateLastNameInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) CreateUsernameInput.Focus();
        }

        private void CreateLoginInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && (sender as TextBox).Text != "") CreatePasswordInput.Focus();
        }

        private void CreatePasswordInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && (sender as TextBox).Text != "") CreateAccount();
        }

        private bool ValidateLoginForm()
        {
            if (this.LoginUsernameInput.Text == "") this.LoginUsernameInput.Focus();
            else if (this.LoginPasswordInput.Text == "") this.LoginPasswordInput.Focus();
            else return true;

            return false;
        }

        private bool ValidateCreateAccountForm()
        {
            if (this.CreateFirstNameInput.Text == "") this.CreateFirstNameInput.Focus();
            else if (this.CreateUsernameInput.Text == "") this.CreateUsernameInput.Focus();
            else if (this.CreatePasswordInput.Text == "") this.CreatePasswordInput.Focus();
            else return true;

            return false;
        }

        private void DoLogin()
        {
            if (!ValidateLoginForm()) return;

            UserController.Instance.Login(this.LoginUsernameInput.Text, this.LoginPasswordInput.Text);
            this.LoginButton.Focus();
        }

        private void CreateAccount()
        {
            if (!ValidateCreateAccountForm()) return;

            UserController.Instance.CreateAccount(this.CreateUsernameInput.Text, this.CreatePasswordInput.Text);
            this.CreateAccountButton.Focus();
        }
    }
}