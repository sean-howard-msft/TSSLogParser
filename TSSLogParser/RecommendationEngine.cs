using HtmlAgilityPack;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Google.Apis.CustomSearchAPI.v1;
using Google.Apis.CustomSearchAPI.v1.Data;

namespace TSSLogParser
{
    internal class RecommendationEngine
    {
        SocketsHttpHandler socketsHandler;
        HttpClient client;
        string googleSearchAPIKey;
        string googleSearchEngineID;

        public RecommendationEngine()
        {
            socketsHandler = new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromSeconds(60),
                PooledConnectionIdleTimeout = TimeSpan.FromMinutes(20),
                MaxConnectionsPerServer = 2
            };
            client = new HttpClient(socketsHandler);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;            
        }

        public RecommendationEngine(string GoogleSearchAPIKey, string GoogleSearchEngineID)
        {
            this.googleSearchAPIKey = GoogleSearchAPIKey;
            this.googleSearchEngineID = GoogleSearchEngineID;
        }

        public Recommendation SearchGoogle(string searchString, string SiteSearch, bool include)
        {
            var service = new CustomSearchAPIService(new Google.Apis.Services.BaseClientService.Initializer()
            {
                ApplicationName = "TSS Parser",
                ApiKey = googleSearchAPIKey
            });

            var request = service.Cse.List();
            request.Q = searchString;
            request.Cx = googleSearchEngineID;
            request.SiteSearch = SiteSearch;
            request.SiteSearchFilter = string.IsNullOrEmpty(SiteSearch) ? 
                CseResource.ListRequest.SiteSearchFilterEnum.SiteSearchFilterUndefined : 
                include ? 
                    CseResource.ListRequest.SiteSearchFilterEnum.I : 
                    CseResource.ListRequest.SiteSearchFilterEnum.E;
            var response = request.Execute();
            return new Recommendation(response);
        }

        public Recommendation SearchMSFTDocs(string searchString)
        {
            return SearchBing(searchString + " site: docs.microsoft.com");
        }

        public Recommendation SearchBing(string searchString)
        {
            string searchUri = "https://www.bing.com/search";
            var param = new Dictionary<string, string>() {
                { "q", searchString }
            };
            Recommendation recommendation = new Recommendation
            {
                SearchURL = QueryHelpers.AddQueryString(searchUri, param)
            };
            string response = CallUrl(recommendation.SearchURL).Result; 
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(response);
            var programmerLinks = htmlDoc.DocumentNode
                .Descendants("li")
                .FirstOrDefault(node => node.Attributes.Any() ? node.Attributes["class"].Value.Contains("b_algo") : false);
            if (programmerLinks != null)
                recommendation.FirstResult = programmerLinks
                .Descendants("a")
                .FirstOrDefault()
                .Attributes["href"].Value;
            return recommendation;
        }


        private async Task<string> CallUrl(string fullUrl)
        {
            //client.DefaultRequestHeaders.Accept.Clear();
            var response = client.GetStringAsync(fullUrl);
            return await response;
        }
    }

    internal class Recommendation
    {
        public Recommendation()
        {
        }

        public Recommendation(Search response)
        {
            SearchURL = string.Format($"https://www.google.com/search?client=ms-google-coop&q={HttpUtility.UrlEncode(response.Queries.Request.First().SearchTerms)}");
            FirstResult = response.Items?.FirstOrDefault()?.Link;
        }

        public string SearchURL { get; set; }
        public string FirstResult { get; set; }
    }
}
