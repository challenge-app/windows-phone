using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Controls;

using ChallengeApp.Utils;
using RestSharp;
using Newtonsoft.Json;

namespace ChallengeApp.Controllers
{
    public abstract class BaseController : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public RestClient restClient = new RestClient(App.SERVER_BASE_URL);

        public BaseController()
        {
            // Force RestSharp use Json.NET Deserializer
            restClient.AddHandler("application/json", new MyDeserializer());
            restClient.AddHandler("text/json", new MyDeserializer());

            // cache
            restClient.AddDefaultHeader("Cache-Control", "no-cache");

            // auth
            restClient.Authenticator = new Authenticator();
        }

        public abstract void Reset();

        protected void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
