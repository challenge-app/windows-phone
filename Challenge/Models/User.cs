using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using ChallengeApp.Controllers;
using ChallengeApp.Utils;

namespace ChallengeApp.Models {
    [DataContract]
    public class User : BaseModel
    {
        [DataMember(Name = "_id")]
        public string id { get; set; }

        [DataMember(Name = "authenticationToken")]
        public string authToken { get; set; }

        private string _email;
        [DataMember]
        public string email { get { return _email; } set { if (value != _email) { _email = value; NotifyPropertyChanged("email"); } } }

        //[DataMember]
        //public string password { get; set; }

        private string _username;
        [DataMember]
        public string username { get { return _username; } set { if (value != _username) { _username = value; NotifyPropertyChanged("username"); } } }

        private string _firstName;
        [DataMember]
        public string firstName { get { return _firstName; } set { if (value != _firstName) { _firstName = value; NotifyPropertyChanged("firstName"); } } }

        private string _lastName;
        [DataMember]
        public string lastName { get { return _lastName; } set { if (value != _lastName) { _lastName = value; NotifyPropertyChanged("lastName"); } } }

        [DataMember]
        public CountObject Count { get; set; }

        private ObservableCollection<User> _followingList;
        //[DataMember(Name = "following")]
        public ObservableCollection<User> followingList 
        {
            get
            {
                createFollowingListCollectionIfNull();
                return _followingList; 
            }
            set
            {
                createFollowingListCollectionIfNull();
                _followingList.Clear();
                if (value != null) foreach (var item in value) _followingList.Add(item);
                //FollowingCount = (_followingList == null) ? 0 : _followingList.Count;
            }
        }

        private ObservableCollection<User> _followersList;
        //[DataMember(Name = "followers")]
        public ObservableCollection<User> followersList
        {
            get
            {
                createFollowersListCollectionIfNull();
                return _followersList; 
            }
            set
            {
                createFollowersListCollectionIfNull();
                _followersList.Clear();
                if (value != null) foreach (var item in value) _followersList.Add(item);
                //FollowersCount = (_followersList == null) ? 0 : _followersList.Count;
            }
        }

        /**
         *  MY VARIABLES
         */
        //private bool _loadedFollowingCount = false;
        //private int _followingCount = 0;
        //public int FollowingCount
        //{
        //    get
        //    {
        //        if (!_loadedFollowingCount)
        //        {
        //            _loadedFollowingCount = true;
        //            _followingCount = followingList.Count > 0 ? followingList.Count : 0;
        //        }

        //        return _followingCount;
        //    }
 
        //    set { if (value != _followingCount) { _followingCount = value; NotifyPropertyChanged("FollowingCount"); } } 
        //}

        //private bool _loadedFollowersCount = false;
        //private int _followersCount = 0;
        //public int FollowersCount
        //{
        //    get
        //    {
        //        if (!_loadedFollowersCount)
        //        {
        //            _loadedFollowersCount = true;
        //            _followersCount = followersList.Count > 0 ? followersList.Count : 0;
        //        }

        //        return _followersCount;
        //    }

        //    set { if (value != _followersCount) { _followersCount = value; NotifyPropertyChanged("FollowersCount"); } }
        //}

        private bool _loadedFollowing = false;
        private bool _isFollowing = false;
        public bool IsFollowing
        {
            get
            {
                if (!_loadedFollowing)
                {
                    _loadedFollowing = true;
                    _isFollowing = UserController.Instance.IsFollowing(id);
                }

                return _isFollowing;
            }

            set { if (_isFollowing != value) { _isFollowing = value; NotifyPropertyChanged("IsFollowing"); } }
        }


        /**
         *  Constructor
         */
        //public User()
        //{
        //    UserController.Instance.FollowedOrUnfollowedUser += OnFollowedOrUnfollowedUser;
        //}

        //public void OnFollowedOrUnfollowedUser(object sender, FollowedOrUnfollowedUserEventArgs e)
        //{
        //    if (e.UserID == id)  this.IsFollowing = e.IsFollowing;
        //}

        private void createFollowingListCollectionIfNull()
        {
            if (_followingList != null) return;
            _followingList = new ObservableCollection<User>();
        }

        private void createFollowersListCollectionIfNull()
        {
            if (_followersList != null) return;
            _followersList = new ObservableCollection<User>();
        }
    }

    public class Users: List<User> { }

    public class CountObject : Object {
        private int _following = 0;
        public int Following { get { return _following; } set { _following = value > 0 ? value : 0; } }

        private int _followers = 0;
        public int Followers { get { return _followers; } set { _followers = value > 0 ? value : 0; } }

        private int _reputation = 0;
        public int Reputation { get { return _reputation; } set { _reputation = value > 0 ? value : 0; } }
    }
}
