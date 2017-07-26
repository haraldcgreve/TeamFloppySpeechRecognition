namespace SpeechToText.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading;
    using System.Web.Configuration;
    using Newtonsoft.Json;
    using System.Threading.Tasks;

    public sealed class Authentication
    {
        private static readonly object LockObject;
        private static readonly string ApiKey;
        private string token;
        private Timer timer;

        static Authentication()
        {
            LockObject = new object();
            ApiKey = WebConfigurationManager.AppSettings["MicrosoftSpeechApiKey"];
        }

        private Authentication()
        {
        }

        public static Authentication Instance { get; } = new Authentication();

        /// <summary>
        /// Gets the current access token.
        /// </summary>
        /// <returns>Current access token</returns>
        public string GetAccessToken()
        {
            // Token will be null first time the function is called.
            if (this.token == null)
            {
                lock (LockObject)
                {
                    // This condition will be true only once in the lifetime of the application
                    if (this.token == null)
                    {
                        this.RefreshToken();
                    }
                }
            }

            return this.token;
        }

        /// <summary>
        /// Issues a new AccessToken from the Speech Api
        /// </summary>
        /// This method couldn't be async because we are calling it inside of a lock.
        /// <returns>AccessToken</returns>
        private static Task<string> GetNewToken()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "17b90ccb9195465b8d6c835c0e3027be");
                UriBuilder uriBuilder = new UriBuilder("https://api.cognitive.microsoft.com/sts/v1.0");
                uriBuilder.Path += "/issueToken";

                HttpResponseMessage result = null;
                try
                {
                    result = client.PostAsync(uriBuilder.Uri.AbsoluteUri, null).Result;
                }
                catch (Exception e)
                { }
                    Console.WriteLine("Token Uri: {0}", uriBuilder.Uri.AbsoluteUri);
                //var values = new Dictionary<string, string>
                //{
                //    { "grant_type", "client_credentials" },
                //    { "client_id", ApiKey },
                //    { "client_secret", ApiKey },
                //    { "scope", "https://speech.platform.bing.com" }
                //};

                //var content = new FormUrlEncodedContent(values);

                //var response = client.PostAsync("https://oxford-speech.cloudapp.net/token/issueToken", content).Result;

                var responseString = result.Content.ReadAsStringAsync();

                return responseString;

                //return JsonConvert.DeserializeObject<AccessTokenInfo>(responseString);
            }
        }

        /// <summary>
        /// Refreshes the current token before it expires. This method will refresh the current access token.
        /// It will also schedule itself to run again before the newly acquired token's expiry by one minute.
        /// </summary>
        private void RefreshToken()
        {
            this.token = GetNewToken().Result;
            //this.timer?.Dispose();
            //this.timer = new Timer(
            //    x => this.RefreshToken(), 
            //    null, 
            //    TimeSpan.FromSeconds(this.token.expires_in).Subtract(TimeSpan.FromMinutes(1)), // Specifies the delay before RefreshToken is invoked.
            //    TimeSpan.FromMilliseconds(-1)); // Indicates that this function will only run once
        }
    }
}