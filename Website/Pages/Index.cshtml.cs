using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Website.Core;

namespace Website.Pages
{
    public class IndexModel : PageModel
    {
        #region PRIVATE_VARIABLES

        private readonly IWebCrawler _webCrawler;
        private readonly string googleSearchPageURL = "https://www.google.com.au/search?q=";
        private string[] removeSections = new string[] { "script", "style", "header", "footer" };
        private int maxResultCount = 100;

        #endregion PRIVATE_VARIABLES

        /// <summary>
        /// Instance of IWebCrawler interface, used to call web crawler methds, passed through dependecy injection
        /// </summary>
        /// <param name="webCrawler"></param>
        public IndexModel(IWebCrawler webCrawler)
        {
            _webCrawler = webCrawler;
        }

        #region PROPS

        public string Positions { get; set; }

        [Required]
        [BindProperty]
        public string Keywords { get; set; }

        [Required]
        [BindProperty]
        [Display(Name = "URL to search")]
        public string URLToSearch { get; set; }

        #endregion PROPS

        public void OnGet()
        {
        }

        /// <summary>
        /// Method is caleed when search button is clicked
        /// </summary>
        public void OnPostSearchAsync()
        {
            if (ModelState.IsValid)
            {
                string body = "";
                try
                {
                    int pageResultsCounter = 0;
                    var positionNumbers = new List<int>();
                    // Run until pageResultsCounter is lessa than maxResultCount
                    while (pageResultsCounter < maxResultCount)
                    {
                        string searchURL = $"{googleSearchPageURL}{Keywords}&start={pageResultsCounter}";
                        string result = String.Empty;
                        using (HttpClient client = new HttpClient())
                        {
                            result = client.GetStringAsync(searchURL).Result;
                        }
                        var startIndex = result.IndexOf("<body");
                        var endIndex = result.IndexOf("</body>");
                        //Getting the body section
                        body = result.Substring(startIndex, endIndex - startIndex + 7);

                        foreach (var section in removeSections)
                        {
                            //Remoe Style, Script and other sections, they are not requried for manipulation.
                            body = _webCrawler.RemoveTagsFromHTML(body, section);
                        }
                        body = body.Replace("<hr class=\"BUybKe\">", "");
                        body = body.Replace("&nbsp", "&amp;nbsp;");

                        //Trying to parse HTML into XML for manipulation, HtmlAgilityPack or other packages are handy/recommended here but I can't use.
                        var xml = XElement.Parse(body);
                        var resultNodes = xml.Descendants().Where(x => x.Attribute("class") != null && x.Attribute("class").Value.StartsWith("ZINbbc"));

                        //Find at what index target URL is found and store in a list.
                        for (int i = 0; i < resultNodes.Count(); i++)
                        {
                            var linkTag = resultNodes.ElementAt(i).Descendants().Where(x => x.Attribute("href") != null).FirstOrDefault();
                            if (linkTag != null && linkTag.Attribute("href").Value.StartsWith($"/url?q={URLToSearch}"))
                            {
                                positionNumbers.Add(i + 1);
                            }
                        }
                        pageResultsCounter += 10;
                    }
                    Positions = $"Found at {(positionNumbers.Any() ? string.Join(", ", positionNumbers) : "0")}";
                }
                catch (Exception ex)
                {
                    Positions = ex.Message;
                }
            }
        }
    }
}