using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Windows;
using System.Threading.Tasks;
using Microsoft.Phone.Shell;

using ChallengeApp.Models;
using ChallengeApp.Exceptions;
using ChallengeApp.Resources;
using ChallengeApp.Utils;

using RestSharp;
using Newtonsoft.Json;

namespace ChallengeApp.Controllers
{
    public class ChallengeController : BaseController
    {
        public static readonly ChallengeController Instance = new ChallengeController();
        public event EventHandler ChallengeCreated;
        public event EventHandler ChallengeAccepted;

        private const string URL_CREATE_CHALLENGE = "challenge";
        private const string URL_GET_RANDOM_PRE_CHALLENGES = "challenge/random";
        private const string URL_GET_RECEIVED_CHALLENGES = "challenge/received";
        private const string URL_GET_SENT_CHALLENGES = "challenge/sent";
        private const string URL_LIKE_CHALLENGE = "challenge/like";
        private const string URL_ACCEPT_CHALLENGE = "challenge/accept";
        private const string URL_REFUSE_CHALLENGE = "challenge/refuse";

        private ChallengeController() 
        {
            //UserController.Instance.UserLoggedOut += (sender, e) => Reset();
        }

        public override void Reset()
        {
        }

        public void CreateChallenge(string receiverId, string description, string type, int reward)
        {
            if (!UserController.IsLogged) return;

            Debug.WriteLine("Creating challenge...");
            SystemTray.ProgressIndicator = new ProgressIndicator();
            SystemTray.ProgressIndicator.IsIndeterminate = true;
            SystemTray.ProgressIndicator.IsVisible = true;

            var request = new RestRequest(URL_CREATE_CHALLENGE, Method.POST);
            request.AddParameter("receiverId", receiverId);
            request.AddParameter("description", description);
            request.AddParameter("type", type);
            request.AddParameter("reward", reward);

            restClient.MyExecuteAsync<Challenge>(request, response =>
            {
                Debug.WriteLine("Done.");
                if (SystemTray.ProgressIndicator != null) SystemTray.ProgressIndicator.IsVisible = false;

                var challenge = response.Data;
                //FeedController.Instance.UpdateFeed(); //ChallengeFeed.Insert(0, content);

                // make a copy to be more thread-safe and invoke the subscribed event-handler(s)
                EventHandler handler = ChallengeCreated;
                if (handler != null) handler(this, EventArgs.Empty);
            });
        }

        public async Task<ObservableCollection<Challenge>> GetReceivedChallenges()
        {
            if (!UserController.IsLogged) return null;

            var request = new RestRequest(URL_GET_RECEIVED_CHALLENGES, Method.GET);

            Debug.WriteLine("Getting received challenges...");
            var task = restClient.ExecuteTask<ObservableCollection<Challenge>>(request);
            await task;
            Debug.WriteLine("Done.");

            return task.Result;
        }

        public void AcceptChallenge(string id, string url)
        {
            if (!UserController.IsLogged) return;

            Debug.WriteLine("Accepting challenge...");
            SystemTray.ProgressIndicator = new ProgressIndicator();
            SystemTray.ProgressIndicator.Text = AppResources.AcceptingChallenge + "...";
            SystemTray.ProgressIndicator.IsIndeterminate = true;
            SystemTray.ProgressIndicator.IsVisible = true;

            var request = new RestRequest(URL_ACCEPT_CHALLENGE, Method.POST);
            request.AddParameter("challengeId", id);
            request.AddParameter("url", url);

            restClient.MyExecuteAsync<Challenge>(request, response =>
            {
                Debug.WriteLine("Done.");
                if (SystemTray.ProgressIndicator != null) SystemTray.ProgressIndicator.IsVisible = false;

                App.ChallengeObject = response.Data;

                // make a copy to be more thread-safe and invoke the subscribed event-handler(s)
                EventHandler handler = ChallengeAccepted;
                if (handler != null) handler(this, EventArgs.Empty);
            });
        }

        public void RefuseChallenge(string id)
        {
            if (!UserController.IsLogged) return;

            Debug.WriteLine("Refusing challenge...");
            SystemTray.ProgressIndicator = new ProgressIndicator();
            SystemTray.ProgressIndicator.Text = AppResources.RefusingChallenge + "...";
            SystemTray.ProgressIndicator.IsIndeterminate = true;
            SystemTray.ProgressIndicator.IsVisible = true;

            var request = new RestRequest(URL_REFUSE_CHALLENGE, Method.POST);
            request.AddParameter("challengeId", id);

            restClient.MyExecuteAsync<Challenge>(request, response =>
            {
                Debug.WriteLine("Done.");
                if (SystemTray.ProgressIndicator != null) SystemTray.ProgressIndicator.IsVisible = false;
            });
        }
    }
}