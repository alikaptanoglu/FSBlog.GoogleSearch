using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FSBlog.GoogleSearch.GoogleClient
{
    public struct SearchResultHit
    {
        // PRIVATE MEMBERS
        private static readonly Regex RX_CLEAN_URI = new Regex("^[^&]+", RegexOptions.IgnoreCase);
        private readonly Uri _uri;
        private readonly Uri _cleanUri;
        private readonly string _text;

        // CTOR
        public SearchResultHit(Uri uri, string text) {
            _uri = uri;
            _cleanUri = new Uri(RX_CLEAN_URI.Match(uri.OriginalString).Groups[0].Value);
            _text = text;
        }

        // PROPERTIES
        public Uri Uri      { get { return _uri; } }
        public Uri CleanUri { get { return _cleanUri; } }
        public string Text  { get { return _text; } }

        // PUBLIC METHODS
        public override string ToString()
        {
            return String.Format("{0}{1}[{2}]", Text, Environment.NewLine, CleanUri);
        }
    }
}