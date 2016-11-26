using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.Extensions
{
    public static partial class StringExtensions
    {
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

                // Try with master page catalog
                if (url.IndexOf("/_catalogs/masterpage", StringComparison.InvariantCultureIgnoreCase) > -1)
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
                Uri uri;
                if (Uri.TryCreate(webUrl, UriKind.Absolute, out uri))
                {
                    string webUrlPathAndQuery = System.Web.HttpUtility.UrlDecode(uri.PathAndQuery);
                    // Don't do additional replacement when masterpagecatalog and themecatalog (see #675)
                    if (url.IndexOf(webUrlPathAndQuery, StringComparison.InvariantCultureIgnoreCase) > -1 && (url.IndexOf("{masterpagecatalog}") == -1) && (url.IndexOf("{themecatalog}") == -1))
                    {
                        result = (uri.PathAndQuery.Equals("/") && url.StartsWith(uri.PathAndQuery))
                            ? "{site}" + url // we need this for DocumentTemplate attribute of pnp:ListInstance also on a root site ("/") without managed path
                            : url.Replace(webUrlPathAndQuery, "{site}");
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

