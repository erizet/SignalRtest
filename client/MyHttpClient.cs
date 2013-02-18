using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Microsoft.AspNet.SignalR.Client.Http;

namespace client
{
    public class MyHttpClient : IHttpClient
    {
        DefaultHttpClient dhc = new DefaultHttpClient();
        /// <summary>
        /// Makes an asynchronous http GET request to the specified url.
        /// </summary>
        /// <param name="url">The url to send the request to.</param>
        /// <param name="prepareRequest">A callback that initializes the request with default values.</param>
        /// <returns>A <see cref="Task{IResponse}"/>.</returns>
        public Task<IResponse> Get(string url, Action<IRequest> prepareRequest)
        {
            Log.WriteLine("GET url: " + url);
            return dhc.Get(url, prepareRequest);
        }



        private string PostData2String(IDictionary<string, string> postData)
        {
            StringBuilder sb = new StringBuilder();
            if (postData != null)
            {
                foreach (var item in postData)
                {
                    sb.Append(item.Key.ToString());
                    sb.Append("=");
                    sb.Append(item.Value);
                    sb.Append(";");
                }
            }
            return sb.ToString();
        }


        /// <summary>
        /// Makes an asynchronous http POST request to the specified url.
        /// </summary>
        /// <param name="url">The url to send the request to.</param>
        /// <param name="prepareRequest">A callback that initializes the request with default values.</param>
        /// <param name="postData">form url encoded data.</param>
        /// <returns>A <see cref="Task{IResponse}"/>.</returns>
        public Task<IResponse> Post(string url, Action<IRequest> prepareRequest, IDictionary<string, string> postData)
        {
            Log.WriteLine("POST url: " + url);
            Log.WriteLine("POST-data: " + PostData2String(postData));

            Task<IResponse> t = dhc.Post(url, prepareRequest, postData);
            t.ContinueWith(tr => { Log.WriteLine("Response: " + tr.Result.ReadAsString()); }, TaskContinuationOptions.OnlyOnRanToCompletion);
            return t;
        }
    }
}
