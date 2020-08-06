using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Website.Core
{
    public class WebCrawler : IWebCrawler
    {
        public string RemoveTagsFromHTML(string html, string tagToRemove)
        {
            var rRemScript = new Regex(@$"<{tagToRemove}[^>]*>[\s\S]*?</{tagToRemove}>");
            return rRemScript.Replace(html, "");
        }
    }
}