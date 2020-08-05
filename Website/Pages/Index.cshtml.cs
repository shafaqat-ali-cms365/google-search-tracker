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

namespace Website.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public string Positions { get; set; }

        [Required]
        [BindProperty]
        public string Keywords { get; set; }

        [Required]
        [BindProperty]
        [Display(Name = "URL to search")]
        public string URLToSearch { get; set; }

        public void OnGet()
        {
        }

        public void OnPostSearchAsync()
        {
            string body = "";
            try
            {
                var removeSections = new string[] { "script", "style", "header", "footer" };
                //Keywords = "online title search";
                int pageCounter = 0;
                var positionNumbers = new List<int>();
                while (pageCounter < 100)
                {
                    string searchURL = $"https://www.google.com.au/search?q={Keywords}&start={pageCounter}";
                    //URLToSearch = "https://www.infotrack.com.au";
                    string result = String.Empty;
                    using (HttpClient client = new HttpClient())
                    {
                        result = client.GetStringAsync(searchURL).Result;
                    }
                    var startIndex = result.IndexOf("<body");
                    var endIndex = result.IndexOf("</body>");
                    body = result.Substring(startIndex, endIndex - startIndex + 7);
                    Regex rRemScript = null;
                    foreach (var section in removeSections)
                    {
                        rRemScript = new Regex(@$"<{section}[^>]*>[\s\S]*?</{section}>");
                        body = rRemScript.Replace(body, "");
                    }

                    body = body.Replace("<hr class=\"BUybKe\">", "");
                    body = body.Replace("&nbsp", "&amp;nbsp;");
                    var xml = XElement.Parse(body);
                    var resultNodes = xml.Descendants().Where(x => x.Attribute("class") != null && x.Attribute("class").Value.StartsWith("ZINbbc"));

                    for (int i = 0; i < resultNodes.Count(); i++)
                    {
                        var linkTag = resultNodes.ElementAt(i).Descendants().Where(x => x.Attribute("href") != null).FirstOrDefault();
                        if (linkTag != null && linkTag.Attribute("href").Value.StartsWith($"/url?q={URLToSearch}"))
                        {
                            positionNumbers.Add(i + 1);
                        }
                    }
                    pageCounter += 10;
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