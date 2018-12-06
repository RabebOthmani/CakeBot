using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CakeBot.Helpers
{
    public class BingSearchAPIs
    {
        private const string SubscriptionKey = "3f78937a9e514fbb9511254a23755f20";

        private const string UriBase = "https://api.cognitive.microsoft.com/bing/v7.0/images/search";

        public string SearchTerm { get; set; }

        public struct SearchResult
        {
            public string JsonResult;
            public Dictionary<string, string> RelevantHeaders;
        }

        public static SearchResult BingImageSearch(string toSearch)
        {
            var uriQuery = UriBase + "?q=" + Uri.EscapeDataString(toSearch);

            WebRequest request = WebRequest.Create(uriQuery);
            request.Headers["Ocp-Apim-Subscription-Key"] = SubscriptionKey;
            HttpWebResponse response = (HttpWebResponse)request.GetResponseAsync().Result;
            string json = new StreamReader(response.GetResponseStream()).ReadToEnd();

            var searchResult = new SearchResult()
            {
                JsonResult = json,
                RelevantHeaders = new Dictionary<string, string>(),
            };

            // Extract Bing HTTP headers
            foreach (string header in response.Headers)
            {
                if (header.StartsWith("BingAPIs-") || header.StartsWith("X-MSEdge-"))
                {
                    searchResult.RelevantHeaders[header] = response.Headers[header];
                }
            }

            return searchResult;
        }

        public string GetSearchResultUrl()
        {
            SearchResult result = BingImageSearch(SearchTerm);

            // deserialize the JSON response from the Bing Image Search API
            dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(result.JsonResult);

            var firstJsonObj = jsonObj["value"][0];

            // After running the application, copy the output URL into a browser to see the image.
            return $"Here's a {SearchTerm} for you: " + firstJsonObj["webSearchUrl"] + "\n";
        }
    }
}
