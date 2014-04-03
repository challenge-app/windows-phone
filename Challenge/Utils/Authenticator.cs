using System;
using System.Diagnostics;

using ChallengeApp.Controllers;
using RestSharp;

namespace ChallengeApp.Utils
{
    public class Authenticator : IAuthenticator
    {
        public void Authenticate(IRestClient client, IRestRequest request)
        {
            // no-cache
            request.AddParameter("no-cache", DateTime.Now.Ticks);

            // auth
            if( !String.IsNullOrEmpty(UserController.AUTH) ) request.AddHeader("X-AUTH-TOKEN", UserController.AUTH);
        }
    }
}
