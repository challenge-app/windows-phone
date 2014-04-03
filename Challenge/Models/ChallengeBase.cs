using System.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace ChallengeApp.Models
{
    [DataContract]
    public class ChallengeBase : BaseModel
    {
        public static ChallengeBase Instance;

        public ChallengeBase()
        {
            Instance = this;
        }

        [DataMember(Name = "_id")]
        public string id { get; private set; }


        [DataMember]
        public string description { get; set; }

        private int _generalLikes;
        [DataMember]
        public int generalLikes { get { return _generalLikes; } set { if (value != _generalLikes) { _generalLikes = value; NotifyPropertyChanged("generalLikes"); } } }

        private int _generalDoubts;
        [DataMember]
        public int generalDoubts { get { return _generalDoubts; } set { if (value != _generalDoubts) { _generalDoubts = value; NotifyPropertyChanged("generalDoubts"); } } }

        [DataMember]
        public List<Challenge> challenges { get; set; }

        [DataMember]
        public bool def { get; private set; }

        [DataMember]
        public int difficulty { get; private set; }

        [DataMember]
        public string timestamp { get; private set; }
    }
}
