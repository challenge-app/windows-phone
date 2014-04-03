using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace ChallengeApp.Models
{
    [DataContract]
    public class FeedItem : BaseModel
    {
        [DataMember(Name = "_id")]
        public string id { get; set; }

        [DataMember]
        public Challenge challenge { get; set; }

        //0: Has been challenged; 1: Accepted a challenge; 2: Refused a challenge; 3: Someone doubted; 4: Someone liked
        [DataMember]
        public int type { get; set; }
        //public enum type { HAS_BEEN_CHALLENGED = 0, CHALLENGE_ACCEPTED, CHALLENGE_REFUSED, SOMEONE_DOUBTED, SOMEONE_LIKED }

        //If type equals 3 or 4, here will be the last user that doubted/liked
        [DataMember]
        public User culprit { get; set; }

        //Array of users for type 3 or 4
        [DataMember]
        public List<User> whoElse { get; set; }

        [DataMember]
        public string timestamp { get; set; }
    }

    public class Feed : List<FeedItem> { }
}
