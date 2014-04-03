using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Resources;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using ChallengeApp.Resources;
using ChallengeApp.Exceptions;
using ChallengeApp.Controllers;
using ChallengeApp.Models;
using ChallengeApp.Utils;

using Newtonsoft.Json;

namespace ChallengeApp
{
    public partial class App : Application
    {
        public const string SERVER_BASE_URL     = "http://mauriciogiordano.com:3000";
        public const string VIEW_LOGIN          = "/Views/Public/LoginPage.xaml";
        public const string VIEW_NEW_CHALLENGE  = "/Views/Private/NewChallengePage.xaml";
        public const string VIEW_MAIN           = "/Views/Private/MainPage.xaml";
        public const string VIEW_SEARCH         = "/Views/Private/SearchPage.xaml";
        public const string VIEW_SETTINGS       = "/Views/Private/SettingsPage.xaml";
        public const string VIEW_PROFILE        = "/Views/Private/ProfilePage.xaml?id={0}";
        //public const string VIEW_CHALLENGE      = "/Views/Private/ChallengePage.xaml?id={0}";
        public const string VIEW_CHALLENGED     = "/Views/Private/Challenged.xaml?id={0}";
        public const string VIEW_FOLLOWERS      = "/Views/Private/FollowersPage.xaml?tab={0}";
        public const string VIEW_CAMERA         = "/Views/Private/CameraPage.xaml";

        // Objects used through pages
        public static Challenge ChallengeObject = null;
        public static User UserProfileObject    = null;

        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public static PhoneApplicationFrame RootFrame { get; private set; }

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions.
            UnhandledException += Application_UnhandledException;

            // Global Resources
            App.Current.Resources.Add("UserController", UserController.Instance);

            // Standard XAML initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            //Resources.Remove("PhoneAccentColor");
            //Resources.Add("PhoneAccentColor", Colors.White);
            //((SolidColorBrush)Resources["PhoneAccentBrush"]).Color = Colors.White;
            //((SolidColorBrush)Resources["PhoneTextBoxEditBackgroundBrush"]).Color = Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF);

            // Language display initialization
            InitializeLanguage();

            // Initial stuff
            UserController.Instance.UserLoggedOut += OnLogout;



            // Show graphics profiling information while debugging.
            if (Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode,
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Prevent the screen from turning off while under the debugger by disabling
                // the application's idle detection.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }

        }

        #region Default
        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                Debugger.Break();
            }
        }
        
        // Initialize the app's font and flow direction as defined in its localized resource strings.
        //
        // To ensure that the font of your application is aligned with its supported languages and that the
        // FlowDirection for each of those languages follows its traditional direction, ResourceLanguage
        // and ResourceFlowDirection should be initialized in each resx file to match these values with that
        // file's culture. For example:
        //
        // AppResources.es-ES.resx
        //    ResourceLanguage's value should be "es-ES"
        //    ResourceFlowDirection's value should be "LeftToRight"
        //
        // AppResources.ar-SA.resx
        //     ResourceLanguage's value should be "ar-SA"
        //     ResourceFlowDirection's value should be "RightToLeft"
        //
        // For more info on localizing Windows Phone apps see http://go.microsoft.com/fwlink/?LinkId=262072.
        //
        private void InitializeLanguage()
        {
            try
            {
                // Set the font to match the display language defined by the
                // ResourceLanguage resource string for each supported language.
                //
                // Fall back to the font of the neutral language if the Display
                // language of the phone is not supported.
                //
                // If a compiler error is hit then ResourceLanguage is missing from
                // the resource file.
                RootFrame.Language = XmlLanguage.GetLanguage(AppResources.ResourceLanguage);

                // Set the FlowDirection of all elements under the root frame based
                // on the ResourceFlowDirection resource string for each
                // supported language.
                //
                // If a compiler error is hit then ResourceFlowDirection is missing from
                // the resource file.
                FlowDirection flow = (FlowDirection)Enum.Parse(typeof(FlowDirection), AppResources.ResourceFlowDirection);
                RootFrame.FlowDirection = flow;
            }
            catch
            {
                // If an exception is caught here it is most likely due to either
                // ResourceLangauge not being correctly set to a supported language
                // code or ResourceFlowDirection is set to a value other than LeftToRight
                // or RightToLeft.

                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }

                throw;
            }
        }
        #endregion

        #region UnhandledException
        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is UnauthorizedAccessException)
            {
                UserController.Instance.Logout();

                e.Handled = true;
                return;
            }
            else if (e.ExceptionObject is Newtonsoft.Json.JsonSerializationException)
            {
                Debug.WriteLine("JsonSerializationException: " + e.ExceptionObject.Message);
                e.Handled = true;
                return;
            }
            else if (e.ExceptionObject is WebException)
            {
                Debug.WriteLine("WebException: " + e.ExceptionObject.Message);
                //MessageBox.Show(e.ExceptionObject.Message);
                e.Handled = true;
                return;
            }
            else if (e.ExceptionObject is APIException)
            {
                if (!String.IsNullOrEmpty(e.ExceptionObject.Message))
                {
                    Debug.WriteLine("API Exception: " + e.ExceptionObject.Message);
                    Error error = JsonConvert.DeserializeObject<Error>(e.ExceptionObject.Message);

                    switch (error.code)
                    {
                        case Error.PLEASE_SIGN_IN:
                            UserController.Instance.Logout();
                            break;

                        case Error.ALREADY_FOLLOWING:
                            break;

                        case Error.ALREADY_NOT_FOLLOWING:
                            break;

                        default:
                            MessageBox.Show(error.GetMessage());
                            break;
                    }
                }

                // Recover from the error
                e.Handled = true;
                return;
            }

            if (Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                Debugger.Break();
            }
        }
        #endregion

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Handle reset requests for clearing the backstack
            RootFrame.Navigated += CheckForResetNavigation;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        private void CheckForResetNavigation(object sender, NavigationEventArgs e)
        {
            // If the app has received a 'reset' navigation, then we need to check
            // on the next navigation to see if the page stack should be reset
            if (e.NavigationMode == NavigationMode.Reset)
                RootFrame.Navigated += ClearBackStackAfterReset;
        }

        private void ClearBackStackAfterReset(object sender, NavigationEventArgs e)
        {
            // Unregister the event so it doesn't get called again
            RootFrame.Navigated -= ClearBackStackAfterReset;

            // Only clear the stack for 'new' (forward) and 'refresh' navigations
            if (e.NavigationMode != NavigationMode.New && e.NavigationMode != NavigationMode.Refresh)
                return;

            // For UI consistency, clear the entire page stack
            while (RootFrame.RemoveBackEntry() != null)
            {
                ; // do nothing
            }
        }

        #endregion

        #region MyFunctions
        private void OnLogout(object sender, EventArgs args)
        {
            Debug.WriteLine("Redirecting to Login Page...");
            (Application.Current.RootVisual as PhoneApplicationFrame).StopLoading();
            (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri(App.VIEW_LOGIN, UriKind.Relative));
        }

        private void Feed_Like_Click(object sender, RoutedEventArgs e)
        {
            //string challengeId = ((sender as Button).DataContext as Challenge).id;
            ((sender as Button).DataContext as Challenge).likes++;
            //Debug.WriteLine(((sender as Button).DataContext as Challenge).id);
        }

        private void ChallengeClick(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var challenge = (sender as FrameworkElement).DataContext as Challenge;
            if (challenge == null || String.IsNullOrEmpty(challenge.id)) return;

            ChallengeObject = challenge;

            if (challenge.receiver.id == UserController.Instance.LoggedUser.id) // && challenge.status <= 0
                (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri(String.Format(VIEW_CHALLENGED, challenge.id), UriKind.Relative));
            //else    
            //    (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri(String.Format(VIEW_CHALLENGE, challenge.id), UriKind.Relative));
        }

        private void UserProfileClick(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var user = (sender as FrameworkElement).DataContext as User;
            if (user == null || String.IsNullOrEmpty(user.id)) return;

            UserProfileObject = user;
            (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri(String.Format(VIEW_PROFILE, user.id), UriKind.Relative));
        }

        private void FollowingClick(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var user = (sender as FrameworkElement).DataContext as User;
            if (user == null) return;

            UserProfileObject = user;
            (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri(String.Format(VIEW_FOLLOWERS, FollowersPage.Tab.FOLLOWING.ToString("D")), UriKind.Relative));
        }

        private void FollowersClick(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var user = (sender as FrameworkElement).DataContext as User;
            if (user == null) return;

            UserProfileObject = user;
            (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri(String.Format(VIEW_FOLLOWERS, FollowersPage.Tab.FOLLOWERS.ToString("D")), UriKind.Relative));
        }
        #endregion
    }
}