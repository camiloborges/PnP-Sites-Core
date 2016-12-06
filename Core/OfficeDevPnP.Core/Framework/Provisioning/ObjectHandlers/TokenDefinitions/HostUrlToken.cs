using Microsoft.SharePoint.Client;
using System;

namespace OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers.TokenDefinitions
{
    internal class HostUrlToken : TokenDefinition
    {
        public HostUrlToken(Web web)
            : base(web, "~hosturl", "{hosturl}")
        {
        }

        public override string GetReplaceValue()
        {
            if (CacheValue == null)
            {
                this.Web.EnsureProperty(w => w.Url);

                using (ClientContext context = this.Web.Context.Clone(this.Web.Url))
                {
                    var site = context.Site;
                    context.Load(site, s => s.Url);
                    context.ExecuteQueryRetry();
                    Uri hostUri;
                    if (Uri.TryCreate(site.Url, UriKind.Absolute, out hostUri))
                    {
                        CacheValue = String.Format("{0}://{1}", hostUri.Scheme, hostUri.Host);
                    }
                }
            }
            return CacheValue;
        }
    }
}