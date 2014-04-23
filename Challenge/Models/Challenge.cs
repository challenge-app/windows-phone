using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Phone.Controls;
using System.Runtime.Serialization;

namespace ChallengeApp.Models
{
    [DataContract]
    public class Challenge : BaseModel
    {
        [DataMember(Name = "_id")]
        public string id { get; set; }

        private ChallengeBase _info;
        [DataMember]
        public ChallengeBase info { get { return _info; } set { if (value != _info) { _info = value; NotifyPropertyChanged("info"); } } }

        private int _likes;
        [DataMember]
        public int likes { get { return _likes; } set { if (value != _likes) { _likes = value; NotifyPropertyChanged("likes"); } } }

        private int _doubts;
        [DataMember]
        public int doubts { get { return _doubts; } set { if (value != _doubts) { _doubts = value; NotifyPropertyChanged("doubts"); } } }

        [DataMember]
        public User sender { get; set; }

        [DataMember]
        public User receiver { get; set; }

        [DataMember]
        public string type { get; set; }

        private string _url = "";
        [DataMember]
        public string url { get { return _url.Replace("s3-website-us-east-1", "s3"); } set { _url = value; } }

        [DataMember]
        public int reward { get; set; }

        //-1: not seen; 0: seen; 1: done; 2: refused
        [DataMember]
        public int status { get; set; }

        [DataMember]
        public string timestamp { get; set; }

        //FIX FOR THE PANORAMA SELECTEDINDEX BUG
        //public override bool Equals(object obj)
        //{
        //    if ((obj != null) && (obj.GetType() == typeof(PanoramaItem)))
        //    {
        //        var thePanoItem = (PanoramaItem)obj;

        //        return base.Equals(thePanoItem.Header);
        //    }
        //    else
        //    {
        //        return base.Equals(obj);
        //    }
        //}

        //public override int GetHashCode()
        //{
        //    return base.GetHashCode();
        //}
    }

    public class Challenges : List<Challenge> { }
}
