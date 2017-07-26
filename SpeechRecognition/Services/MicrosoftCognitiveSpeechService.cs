namespace SpeechToText.Services
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using RestSharp;

    public class MicrosoftCognitiveSpeechService
    {
        /// <summary>
        /// Gets text from an audio stream.
        /// </summary>
        /// <param name="audiostream"></param>
        /// <returns>Transcribed text. </returns>
        public async Task<string> GetTextFromAudioAsync(Stream audiostream)
        {
            var requestUri = @"https://speech.platform.bing.com/recognize?scenarios=smd&appid=D4D52672-91D7-4C74-8AD8-42B1D98141A5&locale=en-US&device.os=bot&form=BCSSTT&version=3.0&format=json&instanceid=565D69FF-E928-4B7E-87DA-9A750B96D9E3&requestid=" + Guid.NewGuid();

            using (var client = new HttpClient())
            {
                var token = Authentication.Instance.GetAccessToken();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                using (var binaryContent = new ByteArrayContent(StreamToBytes(audiostream)))
                {
                    binaryContent.Headers.TryAddWithoutValidation("content-type", "audio/wav; codec=\"audio/pcm\"; samplerate=16000");

                    //var restclient = new RestClient(requestUri);

                    //var request = new RestRequest(Method.POST);

                    //request.AddParameter("audio/wav; codec=\"audio/pcm\"; samplerate=16000", binaryContent, ParameterType.RequestBody);

                    //request.AddHeader("Authorization", string.Format("Bearer {0}", token));

                    //var responseObj = restclient.Execute(request);
                    //var response = 
                    var task1 = client.PostAsync(requestUri, binaryContent);

                    Task.WaitAll(task1);

                    var taskResult1 = task1.Result;
                    var task2 = taskResult1.Content.ReadAsStringAsync();
                    Task.WaitAll(task2);
                    var taskResult2 = task2.Result;
                    dynamic data = JsonConvert.DeserializeObject(taskResult2);
                    return data.header.name;
                }
            }
        }

        /// <summary>
        /// Converts Stream into byte[].
        /// </summary>
        /// <param name="input">Input stream</param>
        /// <returns>Output byte[]</returns>
        private static byte[] StreamToBytes(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}