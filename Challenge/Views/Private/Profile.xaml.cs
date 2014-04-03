using System;
//using System.Collections.Generic;
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
    public partial class Profile : PhoneApplicationPage
    {
        public Profile()
        {
            InitializeComponent();

            Loaded += Profile_Loaded;
        }

        void Profile_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Debug.WriteLine("Login page loaded.");
        }
    }
}