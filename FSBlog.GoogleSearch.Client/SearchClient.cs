using System;
using System.Collections.Generic;
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
        private const int HITS_PER_PAGE = 10;
        private const string USER_AGENT_STRING_FAKE = "Mozilla/5.0 (Windows; U; MSIE 9.0; Windows NT 9.0; en-US)";
        private const string USER_AGENT_STRING = "FSBlog.GoogleSearch.GoogleClient";

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
            var startIndex = 0;
            IList<SearchResultHit> hitsForPage;

            do
            {
                hitsForPage = QueryPaged(startIndex, HITS_PER_PAGE);
                if (hitsForPage == null)
                {
                    break;
                }

                foreach (var hit in hitsForPage)
                {
                    yield return hit;
                }
                startIndex += HITS_PER_PAGE;
                
            } while (hitsForPage.Any());
        }
        public IList<SearchResultHit> QueryPaged(int startIndex, int hitsPerPage) {
            var uri = String.Format("https://www.google.de/search?q={0}&start={1}&num={2}", HttpUtility.UrlEncode(_query), startIndex, hitsPerPage);
            var request = WebRequest.Create(uri) as HttpWebRequest;
            if (request == null)
            {
                throw new InvalidOperationException("Request failed.");
            }

            // configure request
            request.UserAgent = USER_AGENT_STRING;

            // send request and read result
            try
            {
                var response = request.GetResponse() as HttpWebResponse;
                if (response == null) {
                    throw new InvalidOperationException("Failed to retrieve response.");
                }

                var encoding = Encoding.GetEncoding(response.CharacterSet);
                using (var sr = new StreamReader(response.GetResponseStream(), encoding)) {
                    var responseText = sr.ReadToEnd();
                    var results = SearchResultParser.Parse(responseText);
                    return results;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}