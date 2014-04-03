using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ChallengeApp.Models
{
    [DataContract]
    public class LikeDoubt : BaseModel
    {
        [DataMember(Name = "_id")]
        public string id { get; private set; }

        [DataMember]
        public String userId { get; private set; }

        [DataMember]
        public String challengeId { get; set; }

        private int _liked;
        [DataMember(Name = "liked")]
        public int liked { get { return _liked; } set { if (value != _liked) { _liked = value; NotifyPropertyChanged("liked"); } } }

        private int _doubted;
        [DataMember(Name = "doubted")]
        public int doubted { get { return _doubted; } set { if (value != _doubted) { _doubted = value; NotifyPropertyChanged("doubted"); } } }
    }
}
