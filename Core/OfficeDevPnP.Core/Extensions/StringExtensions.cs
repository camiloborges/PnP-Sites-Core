using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.Extensions
{
    public static partial class StringExtensions
    {
        /// <summary>
        /// Tokenize a template item url based attribute with {themecatalog} or {masterpagecatalog} or {site}+
        /// </summary>
        /// <param name="htmlBody">the url to tokenize as String</param>
        /// <param name="webUrl">web url of the actual web as String</param>
        /// <param name="hostUrl"> protocol + domain of current web. Ie.: https://contoso.sharepoint.com</param>
        /// <returns>tokenized urls within html body as String</returns>
        public static string TokenizeHtml(this string htmlBody, string webUrl, Web web=null)
        {

            String result = htmlBody.TokenizeUrl(webUrl, web);
            var hostUrl = "";
            Uri webUrlUri;
            if (Uri.TryCreate(webUrl, UriKind.Absolute, out webUrlUri))
                hostUrl = webUrlUri.AbsoluteUri;
            if (String.IsNullOrEmpty(webUrl))
            {
                return htmlBody;
            }
            if (webUrl == "/")
            {
                result = Regex.Replace(htmlBody, "<(.*?)(src|href)=\"/(.*?)\"(.*?)>", String.Format("<$1$2=\"{0}$3\"$4>", "{site}"),
                                              RegexOptions.IgnoreCase & RegexOptions.Multiline);
            }
            else
            {
                result = Regex.Replace(htmlBody, String.Format("<(.*?)(src|href)=\"{0}(.*?)\"(.*?)>", webUrl), String.Format("<$1$2=\"{0}$3\"$4>", "{site}"),
                                             RegexOptions.IgnoreCase & RegexOptions.Multiline);
            }
            if (!String.IsNullOrEmpty(hostUrl))
            {
                result = Regex.Replace(result, String.Format("<(.*?)(src|href)=\"{0}{1}(.*?)\"(.*?)>", hostUrl, webUrl), String.Format("<$1$2=\"{0}$3\"$4>", "{hosturl}{site}"),
                                             RegexOptions.IgnoreCase & RegexOptions.Multiline);
            }
            return result;
        }
        /// <summary>
        /// Tokenize a template item url based attribute with {themecatalog} or {masterpagecatalog} or {site}+
        /// </summary>
        /// <param name="url">the url to tokenize as String</param>
        /// <param name="webUrl">web url of the actual web as String</param>
        /// <returns>tokenized url as String</returns>
        public static string TokenizeUrl(this string url, string webUrl, Web web = null)
        {
            String result = null;

            if (string.IsNullOrEmpty(url))
            {
                // nothing to tokenize...
                result = String.Empty;
            }
            else
            {
                // Decode URL
                url = Uri.UnescapeDataString(url);
                // Try with theme catalog
                if (url.IndexOf("/_catalogs/theme", StringComparison.InvariantCultureIgnoreCase) > -1)
                {
                    var subsite = false;
                    if (web != null)
                    {
                        subsite = web.IsSubSite();
                    }
                    if (subsite)
                    {
                        result = url.Substring(url.IndexOf("/_catalogs/theme", StringComparison.InvariantCultureIgnoreCase)).Replace("/_catalogs/theme", "{sitecollection}/_catalogs/theme");
                    }
                    else
                    {
                        result = url.Substring(url.IndexOf("/_catalogs/theme", StringComparison.InvariantCultureIgnoreCase)).Replace("/_catalogs/theme", "{themecatalog}");
                    }
                }
                else if (url.IndexOf("/_catalogs/masterpage", StringComparison.InvariantCultureIgnoreCase) > -1)
                {
                    var subsite = false;

                    if (web != null)
                    {
                        subsite = web.IsSubSite();
                    }
                    if (subsite)
                    {
                        result = url.Substring(url.IndexOf("/_catalogs/masterpage", StringComparison.InvariantCultureIgnoreCase)).Replace("/_catalogs/masterpage", "{sitecollection}/_catalogs/masterpage");
                    }
                    else
                    {
                        result = url.Substring(url.IndexOf("/_catalogs/masterpage", StringComparison.InvariantCultureIgnoreCase)).Replace("/_catalogs/masterpage", "{masterpagecatalog}");
                    }
                }

                // Try with site URL
                if (result != null)
                {
                    url = result;
                }
                Uri webUrlUri;
                if (Uri.TryCreate(webUrl, UriKind.Absolute, out webUrlUri))
                {
                    string webUrlPathAndQuery = System.Web.HttpUtility.UrlDecode(webUrlUri.PathAndQuery);
                    // Don't do additional replacement when masterpagecatalog and themecatalog (see #675)
                    if (url.IndexOf(webUrlPathAndQuery, StringComparison.InvariantCultureIgnoreCase) > -1 && (url.IndexOf("{masterpagecatalog}") == -1) && (url.IndexOf("{themecatalog}") == -1))
                    {
                        //look
                        //url can be a full url, that is pointing to same tenant, therefore should be tokenised.
                        //when full url tken is different
                        //if(url.StartsWith(uri.AbsoluteUri)
                        Uri urlUri;
                        if (Uri.TryCreate(url, UriKind.Absolute, out urlUri))
                        {
                            result = (webUrlUri.PathAndQuery.Equals("/") && url.StartsWith(webUrlUri.AbsoluteUri))
                            ? "{hosturl}{site}" + urlUri.PathAndQuery // we need this for DocumentTemplate attribute of pnp:ListInstance also on a root site ("/") without managed path
                            : String.Format("{hosturl}{0}", urlUri.PathAndQuery.Replace(webUrlPathAndQuery, "{site}"));
                        }
                        else
                        {
                            result = (webUrlUri.PathAndQuery.Equals("/") && url.StartsWith(webUrlUri.PathAndQuery))
                            ? "{site}" + url // we need this for DocumentTemplate attribute of pnp:ListInstance also on a root site ("/") without managed path
                            : url.Replace(webUrlPathAndQuery, "{site}");
                        }
                    }
                }

                // Default action
                if (String.IsNullOrEmpty(result))
                {
                    result = url;
                }
            }

            return (result);
        }



    }
}

