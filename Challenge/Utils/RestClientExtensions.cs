using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;

using ChallengeApp.Exceptions;
using ChallengeApp.Resources;
using RestSharp;
using Newtonsoft.Json;

namespace ChallengeApp.Utils
{
    public static class RestClientExtensions
    {
        public static RestRequestAsyncHandle MyExecuteAsync(this IRestClient restClient, RestRequest request, Action<IRestResponse> callback)
        {
            return restClient.ExecuteAsync(request, response =>
            {
                callback(response);

                if (String.IsNullOrEmpty(response.Content))
                    throw new WebException(AppResources.NetworkError);
                else if (response.StatusCode != HttpStatusCode.OK)
                    throw new APIException(response.Content);
            });
        }

        public static RestRequestAsyncHandle MyExecuteAsync<T>(this IRestClient restClient, RestRequest request, Action<IRestResponse<T>> callback) where T : new()
        {
            return restClient.ExecuteAsync<T>(request, response =>
            {
                callback(response);

                if (response.Data == null)
                    throw new WebException(AppResources.NetworkError);
                else if (response.StatusCode != HttpStatusCode.OK) 
                    throw new APIException(response.Content);
            });
        }

        public static Task<IRestResponse> ExecuteTask(this IRestClient restClient, RestRequest request)
        {
            var tcs = new TaskCompletionSource<IRestResponse>();
            restClient.ExecuteAsync(request, response =>
            {
                if (response == null) tcs.SetCanceled();

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    if (String.IsNullOrEmpty(response.Content))
                        tcs.SetException(new WebException(AppResources.NetworkError));
                    else
                        tcs.SetException(new APIException(response.Content));
                }
                else if (response.ResponseStatus == ResponseStatus.Error)
                    tcs.SetException(response.ErrorException);
                else
                    tcs.SetResult(response);
            });

            return tcs.Task;
        }

        public static Task<T> ExecuteTask<T>(this IRestClient restClient, RestRequest request) where T : new()
        {
            var tcs = new TaskCompletionSource<T>();
            restClient.ExecuteAsync<T>(request, response =>
            {
                if (response == null) tcs.SetCanceled();

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    if (String.IsNullOrEmpty(response.Content))
                        tcs.SetException(new WebException(AppResources.NetworkError));
                    else
                        tcs.SetException(new APIException(response.Content));
                } else if (response.ResponseStatus == ResponseStatus.Error)
                    tcs.SetException(response.ErrorException);
                else
                    tcs.SetResult(response.Data);
            });

            return tcs.Task;
        }
    }
}