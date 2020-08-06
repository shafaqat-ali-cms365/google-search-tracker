using System;

namespace Website.Core
{
    public interface IWebCrawler
    {
        string RemoveTagsFromHTML(string html, string tagToRemove);
    }
}