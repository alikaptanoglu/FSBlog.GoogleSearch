using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FSBlog.GoogleSearch.GoogleClient
{
    internal static class SearchResultParser
    {
        // PRIVATE MEMBERS
        private static readonly Regex RX_SEARCH_HITS = new Regex(@"<h3 class=""r""><a href=""/.*?\?q=(.*?)"">(.*?)</a>", RegexOptions.IgnoreCase);

        // PUBLIC METHODS
        public static IList<SearchResultHit> Parse(string response) {
            // preparing the containers for the search hits
            var hits = new List<SearchResultHit>();
            
            // iterate over matches, processing each into a SearchResultHit
            var matches = RX_SEARCH_HITS.Match(response);
            while (matches.Success && matches.Groups.Count == 3)
            {
                var uriString = matches.Groups[1].Value;

                Uri uri;
                if (!Uri.TryCreate(uriString, UriKind.Absolute, out uri))
                {
                    Debug.WriteLine("Discarded: {0}", uriString);
                }
                else
                {
                    var text = matches.Groups[2].Value;
                    hits.Add(new SearchResultHit(uri, text));
                }

                matches = matches.NextMatch();
            }

            // done
            return hits;
        }
    }
}