using System;
using System.Runtime.Serialization;
using ChallengeApp.Resources;

namespace ChallengeApp.Models
{
    [DataContract]
    public class Error
    {
        public const int MISSING_PARAMETER_USERNAME     = 1;
        public const int MISSING_PARAMETER_PASSWORD     = 2;
        public const int MISSING_PARAMETER_ID           = 3;
        public const int MISSING_PARAMETER_RECEIVERID   = 4;
        public const int MISSING_PARAMETER_DESCRIPTION  = 5;
        public const int MISSING_PARAMETER_TYPE         = 6;
        public const int MISSING_PARAMETER_CHALLENGEID  = 7;
        public const int MISSING_PARAMETER_URL          = 8;
        public const int USER_ALREADY_EXISTS            = 9;
        public const int PLEASE_SIGN_IN                 = 10;
        public const int USER_NOT_FOUND                 = 11;
        public const int PASSWORD_INCORRECT             = 12;
        public const int JUST_SIGNOUT                   = 13;
        public const int ALREADY_FOLLOWING              = 14;
        public const int INVALID_PARAMETER_TYPE         = 15;
        public const int CHALLENGE_NOT_FOUND            = 16;
        public const int CHALLENGE_ALREADY_REFUSED      = 17;
        public const int CHALLENGE_ALREADY_KILLED       = 18;
        public const int CHALLENGE_ALREADY_DOUBTED      = 19;
        public const int CHALLENGE_NOT_YOURS            = 20;
        public const int CHALLENGE_IS_FINALIZED         = 21;
        public const int CHALLENGE_YOURSELF             = 22;
        public const int CHALLENGE_NOT_A_FOLLOWER       = 23;
        public const int MISSING_PARAMETER_QUERY        = 24;
        public const int MISSING_PARAMETER_LIMIT        = 25;
        public const int MISSING_PARAMETER_OFFSET       = 26;
        public const int ALREADY_NOT_FOLLOWING          = 27;
        public const int AT_LEAST_ONE_FIELD     = 28;

        [DataMember(Name = "code")]
        public int code { get; private set; }

        private string message = "";

        public string GetMessage()
        {
            if (!String.IsNullOrEmpty(message)) return message;

            switch (code)
            {
                case MISSING_PARAMETER_USERNAME:    message = String.Format(AppResources.ERROR_MISSING_PARAMETER, "username");      break;
                case MISSING_PARAMETER_PASSWORD:    message = String.Format(AppResources.ERROR_MISSING_PARAMETER, "password");      break;
                case MISSING_PARAMETER_ID:          message = String.Format(AppResources.ERROR_MISSING_PARAMETER, "_id");           break;
                case MISSING_PARAMETER_RECEIVERID:  message = String.Format(AppResources.ERROR_MISSING_PARAMETER, "receiverId");    break;
                case MISSING_PARAMETER_DESCRIPTION: message = String.Format(AppResources.ERROR_MISSING_PARAMETER, "description");   break;
                case MISSING_PARAMETER_TYPE:        message = String.Format(AppResources.ERROR_MISSING_PARAMETER, "type");          break;
                case MISSING_PARAMETER_CHALLENGEID: message = String.Format(AppResources.ERROR_MISSING_PARAMETER, "challengeId");   break;
                case MISSING_PARAMETER_URL:         message = String.Format(AppResources.ERROR_MISSING_PARAMETER, "url");           break;
                case MISSING_PARAMETER_QUERY:       message = String.Format(AppResources.ERROR_MISSING_PARAMETER, "query");         break;
                case MISSING_PARAMETER_LIMIT:       message = String.Format(AppResources.ERROR_MISSING_PARAMETER, "limit");         break;
                case MISSING_PARAMETER_OFFSET:      message = String.Format(AppResources.ERROR_MISSING_PARAMETER, "offset");        break;
                case USER_ALREADY_EXISTS:           message = AppResources.ERROR_USER_ALREADY_EXISTS;                               break;
                case PLEASE_SIGN_IN:                message = AppResources.ERROR_PLEASE_SIGN_IN;                                    break;
                case USER_NOT_FOUND:                message = AppResources.ERROR_USER_NOT_FOUND;                                    break;
                case PASSWORD_INCORRECT:            message = AppResources.ERROR_PASSWORD_INCORRECT;                                break;
                case JUST_SIGNOUT:                  message = AppResources.ERROR_JUST_SIGNOUT;                                      break;
                case ALREADY_FOLLOWING:             message = AppResources.ERROR_ALREADY_FOLLOWING;                                 break;
                case INVALID_PARAMETER_TYPE:        message = AppResources.ERROR_INVALID_PARAMETER_TYPE;                            break;
                case CHALLENGE_NOT_FOUND:           message = AppResources.ERROR_CHALLENGE_NOT_FOUND;                               break;
                case CHALLENGE_ALREADY_REFUSED:     message = AppResources.ERROR_CHALLENGE_ALREADY_REFUSED;                         break;
                case CHALLENGE_ALREADY_KILLED:      message = AppResources.ERROR_CHALLENGE_ALREADY_KILLED;                          break;
                case CHALLENGE_ALREADY_DOUBTED:     message = AppResources.ERROR_CHALLENGE_ALREADY_DOUBTED;                         break;
                case CHALLENGE_NOT_YOURS:           message = AppResources.ERROR_CHALLENGE_NOT_YOURS;                               break;
                case CHALLENGE_IS_FINALIZED:        message = AppResources.ERROR_CHALLENGE_IS_FINALIZED;                            break;
                case CHALLENGE_YOURSELF:            message = AppResources.ERROR_CHALLENGE_YOURSELF;                                break;
                case CHALLENGE_NOT_A_FOLLOWER:      message = AppResources.ERROR_CHALLENGE_NOT_A_FOLLOWER;                          break;
                case ALREADY_NOT_FOLLOWING:         message = AppResources.ERROR_ALREADY_NOT_FOLLOWING;                             break;
                case AT_LEAST_ONE_FIELD:            message = AppResources.ERROR_AT_LEAST_ONE_FIELD;                                break;
                default:                            message = "Error code: " + code;                                                break;
            }

            return message;
        }
    }
}