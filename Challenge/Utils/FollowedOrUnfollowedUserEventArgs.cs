using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallengeApp.Utils
{
    public class FollowedOrUnfollowedUserEventArgs : EventArgs
    {
        public string UserID { get; private set; }
        public bool IsFollowing { get; private set; }

        public FollowedOrUnfollowedUserEventArgs(string UserID, bool IsFollowing)
        {
            this.UserID = UserID;
            this.IsFollowing = IsFollowing;
        }
    }
}
