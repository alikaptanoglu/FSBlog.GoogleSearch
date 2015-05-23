using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FSBlog.GoogleSearch.GoogleClient
{
    public class SearchClient
    {
        // CONSTS
        private const int DEFAULT_HITS_PER_PAGE = 10;
        private const string DEFAULT_USER_AGENT_STRING = "FSBlog.GoogleSearch.GoogleClient";
        private static readonly Encoding DEFAULT_ENCODING = Encoding.UTF8;

        // PRIVATE MEMBERS
        private readonly string _query;
        
        // CTORS
        public SearchClient(string query) {
            if (String.IsNullOrWhiteSpace(query))
            {
                throw new ArgumentException("query");
            }

            _query = query;
        }

        // PUBLIC METHODS
        public IEnumerable<SearchResultHit> Query()
        {
            return Query(DEFAULT_HITS_PER_PAGE);
        }
        public IEnumerable<SearchResultHit> Query(int hitsPerPage, string overrideUserAgent = null)
        {
            // init query run
            var currentHitIndex = 0;
            IList<SearchResultHit> hitsForPage;

            // iterate over "pages" and yield search hits one by one
            // stop yielding when there are no more hits
            do
            {
                // retrieve hits for current "page" (as defined by start index and HITS_PER_PAGE)
                hitsForPage = QueryPaged(currentHitIndex, hitsPerPage);
                if (hitsForPage == null)
                {
                    break;
                }

                // yield hits found
                foreach (var hit in hitsForPage)
                {
                    yield return hit;
                }

                // increment start index to get to next "page"
                currentHitIndex += hitsPerPage;
                
            } while (hitsForPage.Any());
        }
        public IList<SearchResultHit> QueryPaged(int startIndex, int hitsPerPage, string overrideUserAgent = null) {
            // instantiate web request
            var uri = AssembleQueryUri(_query, startIndex, hitsPerPage);
            var request = InstantiateWebRequest(uri, overrideUserAgent ?? DEFAULT_USER_AGENT_STRING);

            // send request and process result
            var response = SendRequestAndRetrieveResponse(request);
            var encoding = GetEncoding(response, DEFAULT_ENCODING);
            var result = ProcessSearchResult(response, encoding);

            // done
            return result;
        }

        // PRIVATE METHODS
        private static Uri AssembleQueryUri(string query, int startIndex, int hitsPerPage)
        {
            var uri = String.Format("https://www.google.de/search?q={0}&start={1}&num={2}", HttpUtility.UrlEncode(query), startIndex, hitsPerPage);
            return new Uri(uri);
        }
        private static HttpWebRequest InstantiateWebRequest(Uri uri, string userAgentString)
        {
            var request = WebRequest.Create(uri) as HttpWebRequest;
            if (request == null)
            {
                throw new InvalidOperationException("Could not instantiate web request.");
            }

            // configure request
            request.UserAgent = userAgentString ?? DEFAULT_USER_AGENT_STRING;
            return request;
        }
        private static HttpWebResponse SendRequestAndRetrieveResponse(WebRequest webRequest)
        {
            var response = webRequest.GetResponse() as HttpWebResponse;
            if (response == null)
            {
                throw new InvalidOperationException("Failed to retrieve response.");
            }

            return response;
        }
        private static Encoding GetEncoding(HttpWebResponse response, Encoding defaultTo)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(response.CharacterSet))
                {
                    return Encoding.GetEncoding(response.CharacterSet);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            
            // default
            return defaultTo;
        }
        private static IList<SearchResultHit> ProcessSearchResult(WebResponse response, Encoding encoding)
        {
            using(var responseStream = response.GetResponseStream())
            using(var streamReader = new StreamReader(responseStream, encoding))
            {
                var responseText = streamReader.ReadToEnd();
                var results = SearchResultParser.Parse(responseText);
                return results;
            }
        }
    }
}