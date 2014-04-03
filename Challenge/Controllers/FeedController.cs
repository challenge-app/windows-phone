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
    public class FeedController : BaseController
    {
        public static readonly FeedController Instance = new FeedController();
        AppSettings settings = AppSettings.Instance;

        private const string URL_GET_FEED = "feed/";

        //public bool IsFeedLoaded { get; private set; }
        //public ObservableCollection<FeedItem> Feed = new ObservableCollection<FeedItem>();

        private FeedController()
        {
            UserController.Instance.UserLoggedOut += (sender, e) => Reset();

            //if (settings.LoadedFeed != null)
            //{
                //Feed.Clear();
                //foreach (var item in settings.LoadedFeed) Feed.Add(item);
                //IsFeedLoaded = true;
            //}
        }

        public override void Reset()
        {
            //IsFeedLoaded = false;
            //Feed.Clear();

            // phone settings
            //settings.LoadedFeed = null;
        }

        public async Task<List<FeedItem>> GetFeed(int limit = 10, int offset = 0)
        {
            if (!UserController.IsLogged) return null;

            var request = new RestRequest(URL_GET_FEED, Method.GET);
            request.AddParameter("limit", limit);
            request.AddParameter("offset", offset);

            Debug.WriteLine("Getting logged user feed...");
            var task = restClient.ExecuteTask<List<FeedItem>>(request);
            await task;
            Debug.WriteLine("Done.");

            var Result = task.Result;

            await task.ContinueWith((t) =>
            {
                var content = t.Result;
                //content = content.OrderByDescending(x => x.timestamp).ToList<FeedItem>();

                List<FeedItem> Feed = new List<FeedItem>();

                if (content != null)
                {
                    foreach (var item in t.Result)
                    {
                        // remove repetido -> temporario enquanto o feed esta trazendo item repetido
                        var repeatedItem = Feed.Where<FeedItem>(u => u.challenge.id == item.challenge.id && u.id != item.id).FirstOrDefault<FeedItem>();
                        if (repeatedItem != null)
                        {
                            if ((item.type == 1 || item.type == 2) && repeatedItem.type == 0) Feed.Remove(repeatedItem);
                            else if (item.type == 0 && (repeatedItem.type == 1 || repeatedItem.type == 2)) continue;
                        }

                        Feed.Add(item);
                    }

                    //IsFeedLoaded = true;
                    Result = Feed;
                }

                //settings.LoadedFeed = Feed.Cast<FeedItem>().ToList();
            });

            return Result;
        }

        //public void UpdateFeed(int limit = 10, int offset = 0)
        //{
        //    LoadFeed(limit, offset); return;

        //    if (!UserController.IsLogged) return;

        //    Debug.WriteLine("Updating Feed...");
        //    SystemTray.ProgressIndicator = new ProgressIndicator();
        //    SystemTray.ProgressIndicator.IsIndeterminate = true;
        //    SystemTray.ProgressIndicator.IsVisible = true;

        //    var request = new RestRequest(URL_GET_FEED, Method.GET);
        //    request.AddParameter("limit", limit);
        //    request.AddParameter("offset", offset);

        //    restClient.MyExecuteAsync<List<FeedItem>>(request, response =>
        //    {
        //        Debug.WriteLine("Done.");
        //        //Debug.WriteLine("Response: " + response.Content);
        //        if (SystemTray.ProgressIndicator != null) SystemTray.ProgressIndicator.IsVisible = false;

        //        var content = response.Data;
        //        //content = content.OrderByDescending(x => x.timestamp).ToList<Challenge>();

        //        Feed.Clear();
        //        foreach (var item in content) Feed.Add(item);

        //        //int index;
        //        //foreach (var item in content.Reverse<FeedItem>())
        //        //{
        //        //    var old_item = Feed.Where<FeedItem>(c => (c as FeedItem).id == (item as FeedItem).id).FirstOrDefault<FeedItem>();
        //        //    index = old_item == null ? -1 : Feed.IndexOf(old_item);

        //        //    if (index >= 0)
        //        //    {
        //        //        // atualiza desafio, apenas os campos com mudaveis (possuem NotifyPropertyChanged)
        //        //        (Feed[index] as FeedItem).challenge.info        = (item as FeedItem).challenge.info;
        //        //        (Feed[index] as FeedItem).challenge.status      = (item as FeedItem).challenge.status;
        //        //        (Feed[index] as FeedItem).challenge.timestamp   = (item as FeedItem).challenge.timestamp;
        //        //        (Feed[index] as FeedItem).challenge.likes       = (item as FeedItem).challenge.likes;
        //        //        (Feed[index] as FeedItem).challenge.doubts      = (item as FeedItem).challenge.doubts;
        //        //    }
        //        //    else
        //        //    {
        //        //        Feed.Insert(0, item);
        //        //    }
        //        //}

        //        IsFeedLoaded = true;
        //        //settings.LoadedFeed = Feed.Cast<FeedItem>().ToList();
        //    });
        //}
    }
}