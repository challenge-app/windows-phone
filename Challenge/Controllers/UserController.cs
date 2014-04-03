using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.ServiceModel;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using ChallengeApp.Models;
using ChallengeApp.Exceptions;
using ChallengeApp.Resources;
using ChallengeApp.Utils;

using RestSharp;
using Newtonsoft.Json;

namespace ChallengeApp.Controllers
{
    public sealed class UserController : BaseController
    {
        private static volatile UserController _instance = null;

        public event EventHandler UserLoggedIn;
        public event EventHandler UserLoggedOut;
        public event EventHandler UserProfileEdited;
        public event EventHandler<FollowedOrUnfollowedUserEventArgs> FollowedOrUnfollowedUser;
        AppSettings settings = AppSettings.Instance;

        private const string URL_DO_LOGIN               = "user/auth"; //lembrar de cryptografar senha antes de ir para producao
        private const string URL_DO_LOGOUT              = "user/logout";
        private const string URL_LOGGED_USER            = "user";
        private const string URL_CREATE_USER            = "user";
        private const string URL_GET_USER_BY_ID         = "user/{id}";
        private const string URL_USER_EDIT              = "user/edit";
        private const string URL_FIND_USER              = "user/find";
        private const string URL_FOLLOW                 = "user/follow";
        private const string URL_UNFOLLOW               = "user/unfollow";
        private const string URL_GET_USER_FOLLOWING     = "user/{id}/following";
        private const string URL_GET_USER_FOLLOWERS     = "user/{id}/followers";

        public static bool IsLogged { get; private set; }
        public static string AUTH { get; private set; }

        private static User _LoggedUser;
        public User LoggedUser { get { return _LoggedUser; } private set { if (value != _LoggedUser) { _LoggedUser = value; NotifyPropertyChanged("LoggedUser"); } } }

        private RestRequestAsyncHandle asyncHandle;

        public static UserController Instance
        {
            get
            {
                if (_instance == null) {
                    _instance = new UserController();

                    // update login status
                    if (!_instance.LoginFromSession()) _instance.Logout(false);

                    _instance.FollowedOrUnfollowedUser += _instance.OnFollowedOrUnfollowedUser;

                }

                return _instance;
            }
        }

        private UserController()  {  }

        public override void Reset()
        {
            // local variables
            AUTH = "";
            IsLogged = false;
            LoggedUser = null;

            // phone settings
            settings.LoggedUser = null;
            settings.AUTH = "";
        }

        public void MustBeLogged()
        {
            // if not logged, go login
            Debug.WriteLine("Must be logged...");
            if (!IsLogged)
            {
                Debug.WriteLine("But it isn't.");
                Logout();
                return;
            }

            Debug.WriteLine("And it is.");
        }


        public void Logout() { Logout(true); }

        private void Logout(bool dispatchEvent)
        {
            Reset();
            Debug.WriteLine("Logged out.");

            if (dispatchEvent)
            {
                // invoke the subscribed event-handler(s) (make a copy to be more thread-safe)
                EventHandler handler = UserLoggedOut;
                if (handler != null) handler(this, EventArgs.Empty);
            }
        }

        private bool LoginFromSession()
        {
            var auth = settings.AUTH;

            if (String.IsNullOrEmpty(auth))
            {
                Debug.WriteLine("No login session to restore.");
                return false;
            }

            Debug.WriteLine("Restoring login session...");
            DoLocalLogin(auth);

            return true;
        }

        public void Login(string email, string password)
        {
            if (asyncHandle != null) asyncHandle.Abort();
            Logout(false);

            SystemTray.ProgressIndicator = new ProgressIndicator();
            SystemTray.ProgressIndicator.IsIndeterminate = true;
            SystemTray.ProgressIndicator.IsVisible = true;
            SystemTray.ProgressIndicator.Text = String.Format("{0}...", AppResources.LoggingIn);

            var request = new RestRequest(URL_DO_LOGIN, Method.POST);
            request.AddParameter("email", email);
            request.AddParameter("password", password);

            Debug.WriteLine("Logging in...");
            asyncHandle = restClient.MyExecuteAsync<User>(request, response =>
            {
                Debug.WriteLine("Done.");
                if (SystemTray.ProgressIndicator != null) SystemTray.ProgressIndicator.IsVisible = false;
                if (response.Data == null) return;

                var user = response.Data;

                if (!String.IsNullOrEmpty(user.authToken)) DoLocalLogin(user.authToken);
            });
        }

        private void DoLocalLogin(string auth)
        {
            if (String.IsNullOrEmpty(auth)) return;

            AUTH = auth;
            IsLogged = true;
            LoggedUser = settings.LoggedUser == null ? new User() : settings.LoggedUser;
            Debug.WriteLine("Logged. Auth: " + auth);

            // save locally
            settings.AUTH = AUTH;

            // try to load user data
            UpdateLoggedUserData();

            // invoke the subscribed event-handler(s) (make a copy to be more thread-safe)
            EventHandler handler = UserLoggedIn;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public void CreateAccount(string email, string password)
        {
            if (asyncHandle != null) asyncHandle.Abort();

            SystemTray.ProgressIndicator = new ProgressIndicator();
            SystemTray.ProgressIndicator.IsIndeterminate = true;
            SystemTray.ProgressIndicator.IsVisible = true;
            SystemTray.ProgressIndicator.Text = String.Format("{0}...", AppResources.CreatingAccount);

            var request = new RestRequest(URL_CREATE_USER, Method.POST);
            request.AddParameter("email", email);
            request.AddParameter("password", password);

            Debug.WriteLine("Creating account...");
            asyncHandle = restClient.MyExecuteAsync<User>(request, response =>
            {
                Debug.WriteLine("Done.");
                Debug.WriteLine("Response: " + response.Content);
                if (SystemTray.ProgressIndicator != null) SystemTray.ProgressIndicator.IsVisible = false;
                if (response.Data == null) return;

                var user = response.Data;

                if (!String.IsNullOrEmpty(user.authToken))
                    DoLocalLogin(user.authToken);
                //else
                //    Login(email, password);
            });
        }
        
        private async Task<User> GetLoggedUserData()
        {
            var request = new RestRequest(URL_LOGGED_USER, Method.GET);

            Debug.WriteLine("Getting logged user data...");
            var task = restClient.ExecuteTask<User>(request);
            await task;
            Debug.WriteLine("Done.");

            return task.Result;
        }
        
        public void UpdateLoggedUserData()
        {
            var request = new RestRequest(URL_LOGGED_USER, Method.GET);

            Debug.WriteLine("Updating logged user data...");
            asyncHandle = restClient.MyExecuteAsync<User>(request, response =>
            {
                Debug.WriteLine("Done.");
                if (response == null || response.Data == null) return;

                LoggedUser = response.Data;
                settings.LoggedUser = LoggedUser;
            });
        }

        public async Task<User> GetUserById(string id)
        {
            var request = new RestRequest(URL_GET_USER_BY_ID, Method.GET);
            request.AddUrlSegment("id", id);

            Debug.WriteLine("Getting user...");
            var task = restClient.ExecuteTask<User>(request);
            await task;
            Debug.WriteLine("Done.");

            return task.Result;
        }
        
        public async Task<ObservableCollection<User>> GetUserFollowingList(string id)
        {
            var request = new RestRequest(URL_GET_USER_FOLLOWING, Method.GET);
            request.AddUrlSegment("id", id);

            Debug.WriteLine("Getting user following list...");
            var task = restClient.ExecuteTask<ObservableCollection<User>>(request);
            await task;
            Debug.WriteLine("Done.");

            return task.Result;
        }

        public async Task<ObservableCollection<User>> GetUserFollowersList(string id)
        {
            var request = new RestRequest(URL_GET_USER_FOLLOWERS, Method.GET);
            request.AddUrlSegment("id", id);

            Debug.WriteLine("Getting user followers list...");
            var task = restClient.ExecuteTask<ObservableCollection<User>>(request);
            await task;
            Debug.WriteLine("Done.");

            return task.Result;
        }

        public async Task<bool> EditLoggedUser(string firstName = "", string lastName = "", string username = "", string email = "")
        {
            if (!UserController.IsLogged) return false;

            bool hasParameter = false;

            var request = new RestRequest(URL_USER_EDIT, Method.POST);
            if (!String.IsNullOrEmpty(firstName))   { request.AddParameter("firstName", firstName); hasParameter = true; }
            if (!String.IsNullOrEmpty(lastName))    { request.AddParameter("lastName", lastName);   hasParameter = true; }
            if (!String.IsNullOrEmpty(username))    { request.AddParameter("username", username);   hasParameter = true; }
            if (!String.IsNullOrEmpty(email))       { request.AddParameter("email", email);         hasParameter = true; }
            if (!hasParameter) return false;

            SystemTray.ProgressIndicator = new ProgressIndicator();
            SystemTray.ProgressIndicator.IsIndeterminate = true;
            SystemTray.ProgressIndicator.IsVisible = true;
            SystemTray.ProgressIndicator.Text = String.Format("{0}...", AppResources.Saving);

            Debug.WriteLine("Editing user...");
            var task = restClient.ExecuteTask<User>(request);
            await task;
            Debug.WriteLine("Done.");

            bool Result = false;
            await task.ContinueWith((t) =>
            {
                if (t.Result == null)
                    Result = false;
                else
                    Result = true;
            });

            return Result;
        }

        public async Task<ObservableCollection<User>> Find(string query, bool loadAll = false) 
        {
            if (!loadAll && String.IsNullOrEmpty(query)) return null;
            if (!UserController.IsLogged) return null;

            var request = new RestRequest(URL_FIND_USER, Method.POST);
            request.AddParameter("query", query);

            Debug.WriteLine("Searching users...");
            var task = restClient.ExecuteTask<ObservableCollection<User>>(request);
            await task;
            Debug.WriteLine("Done.");

            return task.Result;
        }

        public User FindUserInFollowingList(string id)
        {
            return (LoggedUser == null || LoggedUser.followingList == null) ? null : LoggedUser.followingList.Where<User>(u => u.id == id).FirstOrDefault<User>();
        }

        public User FindUserInFollowersList(string id)
        {
            return (LoggedUser == null || LoggedUser.followersList == null) ? null : LoggedUser.followersList.Where<User>(u => u.id == id).FirstOrDefault<User>();
        }

        public bool IsFollowing(string id)
        {
            return FindUserInFollowingList(id) == null ? false : true;
        }


        public void Follow(User user)
        {
            if (user == null) return;

            // invoke the subscribed event-handler(s) (make a copy to be more thread-safe)
            EventHandler<FollowedOrUnfollowedUserEventArgs> handler = FollowedOrUnfollowedUser;
            if (handler != null) handler(this, new FollowedOrUnfollowedUserEventArgs(user.id, true));

            // update in following list
            var _user = FindUserInFollowingList(user.id);

            //if (_user == null || (_user != null && !_user.IsFollowing))
            //{
                if (_user != null) _user.IsFollowing = true;

                // follow
                var request = new RestRequest(URL_FOLLOW, Method.POST);
                request.AddParameter("_id", user.id);

                Debug.WriteLine(String.Format("Following user {0}...", user.id));
                asyncHandle = restClient.MyExecuteAsync(request, response =>
                {
                    Debug.WriteLine("Done.");

                    try
                    {
                        switch (response.StatusCode)
                        {
                            case HttpStatusCode.OK:
                                if (_user == null) UserController.Instance.LoggedUser.followingList.Add(user);
                                break;

                            default:
                                if (_user != null) _user.IsFollowing = false;

                                Error error = JsonConvert.DeserializeObject<Error>(response.Content);
                                if (error.code != Error.ALREADY_FOLLOWING)
                                {
                                    // if error, reinvoke the subscribed event-handler(s)
                                    handler = FollowedOrUnfollowedUser;
                                    if (handler != null) handler(this, new FollowedOrUnfollowedUserEventArgs(user.id, false));

                                    throw new APIException(response.Content);
                                }

                                break;
                        }
                    }
                    catch (JsonSerializationException)
                    {
                        throw new APIException(response.Content);
                    }
                });
            //}
        }

        public void Unfollow(User user)
        {
            if (user == null) return;

            // invoke the subscribed event-handler(s) (make a copy to be more thread-safe)
            EventHandler<FollowedOrUnfollowedUserEventArgs> handler = FollowedOrUnfollowedUser;
            if (handler != null) handler(this, new FollowedOrUnfollowedUserEventArgs(user.id, false));

            // get user to unfollow
            //var _user = FindUserInFollowingList(user.id);

            //if (_user != null && _user.IsFollowing)
            //{
                // unfollow
                user.IsFollowing = false;

                // follow
                var request = new RestRequest(URL_UNFOLLOW, Method.POST);
                request.AddParameter("_id", user.id);

                Debug.WriteLine(String.Format("Unfollowing user {0}...", user.id));
                asyncHandle = restClient.MyExecuteAsync(request, response =>
                {
                    Debug.WriteLine("Done.");

                    try
                    {
                        switch (response.StatusCode)
                        {
                            case HttpStatusCode.OK:
                                //if (_user != null) UserController.Instance.LoggedUser.followingList.Remove(user);
                                break;

                            default:
                                //if (_user != null) _user.IsFollowing = true;

                                Error error = JsonConvert.DeserializeObject<Error>(response.Content);
                                if (error.code != Error.ALREADY_NOT_FOLLOWING)
                                {
                                    // if error, reinvoke the subscribed event-handler(s)
                                    handler = FollowedOrUnfollowedUser;
                                    if (handler != null) handler(this, new FollowedOrUnfollowedUserEventArgs(user.id, false));

                                    throw new APIException(response.Content);
                                }

                                break;
                        }
                    }
                    catch (JsonSerializationException)
                    {
                        throw new APIException(response.Content);
                    }
                });
            //}
        }

        void OnFollowedOrUnfollowedUser(object sender, FollowedOrUnfollowedUserEventArgs e)
        {
            if (e.IsFollowing) LoggedUser.Count.Following++; else LoggedUser.Count.Following--;
        }
    }
}