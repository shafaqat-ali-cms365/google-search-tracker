using Microsoft.VisualStudio.TestTools.UnitTesting;
using Website.Core;

namespace UnitTests
{
    [TestClass]
    public class WebCrawlerTests
    {
        [TestMethod]
        public void TestRemoveScriptSestionFromHTML()
        {
            var crawler = new WebCrawler();
            string inputHTML = "<html><script>some javascript</script><body><script>some javascript</script></body></html>";
            string requiredHTML = "<html><body></body></html>";

            Assert.IsTrue(crawler.RemoveTagsFromHTML(inputHTML, "script") == requiredHTML);
        }

        [TestMethod]
        public void TestRemoveStyleSestionFromHTML()
        {
            var crawler = new WebCrawler();
            string inputHTML = "<html><style>some tyle</style><body><style>some style</style></body></html>";
            string requiredHTML = "<html><body></body></html>";

            Assert.IsTrue(crawler.RemoveTagsFromHTML(inputHTML, "style") == requiredHTML);
        }
    }
}