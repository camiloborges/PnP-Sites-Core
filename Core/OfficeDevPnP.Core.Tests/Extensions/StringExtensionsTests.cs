using Microsoft.VisualStudio.TestTools.UnitTesting;
using OfficeDevPnP.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.Tests.Extensions
{
    [TestClass()]
    public class StringExtensionsTests
    {
        [TestMethod]
        public void TokenizeHtmlWithRelativeAndAbsolutePaths()
        {
            var htmlContent = "<a href=\"/blablabla/bla.html\">contoso</a>" +
                " <a href=\"https://contoso.sharepoint.com/blablabla/bla.html\">bontent</a>" +
                "<a href =\"https://litware.sharepoint.com/blablabla/bla.html\">bontent</a>";
            var result = htmlContent.TokenizeHtml("https://contoso.sharepoint.com/");
            var expectedResult = "<a href=\"{site}blablabla/bla.html\">contoso</a>" +
                " <a href=\"{hosturl}{site}blablabla/bla.html\">bontent</a>" +
                "<a href =\"https://litware.sharepoint.com/blablabla/bla.html\">bontent</a>";
            Assert.AreEqual(expectedResult, result);


        }
        [TestMethod]
        public void TokenizeHtmlBlankContent()
        {
            var htmlContent = "";
            var result = htmlContent.TokenizeHtml("/");
            Assert.AreEqual(htmlContent, result);


        }

      
        [TestMethod]
        public void TokenizeUrlAbsoluteUrlTest()
        {
            var itemUrl = "https://contoso.sharepoint.com/sites/test/pages/home.aspx";
            var webUrl = "https://contoso.sharepoint.com/sites/test";
            var expectedResult = "{hosturl}{site}pages/home.aspx";
            var result = itemUrl.TokenizeUrl(webUrl);
            Assert.AreEqual(expectedResult, result);

        }

        [TestMethod]
        public void TokenizeUrlAbsoluteUrlRootTest()
        {
            var itemUrl = "https://contoso.sharepoint.com/pages/home.aspx";
            var webUrl = "https://contoso.sharepoint.com/";
            var expectedResult = "{hosturl}{site}pages/home.aspx";
            var result = itemUrl.TokenizeUrl(webUrl);
            Assert.AreEqual(expectedResult, result);

        }


        [TestMethod]
        public void TokenizeUrlNotUrlTest()
        {
            var itemUrl = "";
            var webUrl = "https://contoso.sharepoint.com/";
            var expectedResult = "";
            var result = itemUrl.TokenizeUrl(webUrl);
            Assert.AreEqual(expectedResult, result);


        }

        [TestMethod]
        public void TokenizeUrlRelativeRootTest()
        {
            var itemUrl = "/pages/home.aspx";
            var webUrl = "https://contoso.sharepoint.com/";
            var expectedResult = "{site}pages/home.aspx";
            var result = itemUrl.TokenizeUrl(webUrl);
            Assert.AreEqual(expectedResult, result);

        }

        [TestMethod]
        public void TokenizeUrlRelativeTest()
        {
            var itemUrl = "/sites/test/pages/home.aspx";
            var webUrl = "https://contoso.sharepoint.com/sites/test";
            var expectedResult = "{site}pages/home.aspx";
            var result = itemUrl.TokenizeUrl(webUrl);
            Assert.AreEqual(expectedResult, result);

        }

    }
}