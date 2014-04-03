using System;
using System.IO.IsolatedStorage;
using System.Diagnostics;
using System.Collections.Generic;
using ChallengeApp.Models;
using Newtonsoft.Json;

namespace ChallengeApp.Utils
{
    public class AppSettings
    {
        public static readonly AppSettings Instance = new AppSettings();
        IsolatedStorageSettings settings;

        // The key names of our settings
        const string AUTHKeyName                = "AUTHSetting";
        const string LoggedUserKeyName          = "LoggedUserSetting";
        //const string LoadedFeedKeyName          = "LoadedFeedSetting";

        // The default value of our settings
        const string AUTHDefault                = "";
        const User LoggedUserDefault            = null;
        //const List<FeedItem> LoadedFeedDefault  = null;

        public AppSettings()
        {
            // Get the settings for this application.
            if (!System.ComponentModel.DesignerProperties.IsInDesignTool) {
                settings = IsolatedStorageSettings.ApplicationSettings;
            }
        }

        public bool AddOrUpdateValue(string Key, Object value)
        {
            bool valueChanged = false;

            // If the key exists
            if (settings.Contains(Key))
            {
                // If the value has changed
                if (settings[Key] != value)
                {
                    // Store the new value
                    settings[Key] = value;
                    valueChanged = true;
                }
            }
            // Otherwise create the key.
            else
            {
                settings.Add(Key, value);
                valueChanged = true;
            }

            return valueChanged;
        }

        public T GetValueOrDefault<T>(string Key, T defaultValue)
        {
            T value;

            // If the key exists, retrieve the value.
            if (settings.Contains(Key))
            {
                value = (T)settings[Key];
            }
            // Otherwise, use the default value.
            else
            {
                value = defaultValue;
            }

            return value;
        }

        public void Save()
        {
            settings.Save();
        }

        public string AUTH
        {
            get { return GetValueOrDefault<string>(AUTHKeyName, AUTHDefault); }
            set { if (AddOrUpdateValue(AUTHKeyName, value)) Save(); }
        }

        public User LoggedUser
        {
            get { return GetValueOrDefault<User>(LoggedUserKeyName, LoggedUserDefault); }
            set { if (AddOrUpdateValue(LoggedUserKeyName, value)) Save(); }
        }

        //public User LoggedUser
        //{
        //    get { string json = GetValueOrDefault<string>(LoggedUserKeyName, ""); return String.IsNullOrEmpty(json) ? LoggedUserDefault : JsonConvert.DeserializeObject<User>(json); }
        //    set { if (AddOrUpdateValue(LoggedUserKeyName, JsonConvert.SerializeObject(value))) Save(); }
        //}

        //public List<FeedItem> LoadedFeed
        //{
        //    get { return GetValueOrDefault<List<FeedItem>>(LoadedFeedKeyName, LoadedFeedDefault); }
        //    set { if (AddOrUpdateValue(LoadedFeedKeyName, value)) Save(); }
        //}
    }
}