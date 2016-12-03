using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Diagnostics;
using OfficeDevPnP.Core.Framework.Provisioning.Connectors;
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace OfficeDevPnP.Core.AppModelExtensions
{
    public static partial class ProvisioningTemplateCreationInformationExtensions
    {

        public static void PersistFile(this  ProvisioningTemplateCreationInformation creationInfo, string folderPath, string fileName, Web web, PnPMonitoredScope scope, Boolean decodeFileName = false)
        {
            if (creationInfo.FileConnector != null)
            {
                SharePointConnector connector = new SharePointConnector(web.Context, web.Url, "dummy");

                Uri u = new Uri(web.Url);
                if (folderPath.IndexOf(u.PathAndQuery, StringComparison.InvariantCultureIgnoreCase) > -1 && u.PathAndQuery != "/")
                {
                    folderPath = folderPath.Replace(u.PathAndQuery, "");
                }

                String container = folderPath.Trim('/').Replace("%20", " ").Replace("/", "\\");
                String persistenceFileName = (decodeFileName ? HttpUtility.UrlDecode(fileName) : fileName).Replace("%20", " ");

                using (Stream s = connector.GetFileStream(fileName, folderPath))
                {
                    if (s != null)
                    {
                        creationInfo.FileConnector.SaveFileStream(
                            persistenceFileName, container, s);
                    }
                }
            }
            else
            {
              //  WriteWarning("No connector present to persist homepage.", ProvisioningMessageType.Error);
                scope.LogError("No connector present to persist homepage");
            }
        }

    }
}
