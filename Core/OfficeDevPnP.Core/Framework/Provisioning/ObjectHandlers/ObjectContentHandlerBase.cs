using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.AppModelExtensions;
using OfficeDevPnP.Core.Diagnostics;
using OfficeDevPnP.Core.Framework.Provisioning.Connectors;
using OfficeDevPnP.Core.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers
{
    internal abstract class ObjectContentHandlerBase : ObjectHandlerBase
    {
        public Model.File RetrieveFieldValues(Web web, Microsoft.SharePoint.Client.File file, Model.File modelFile)
        {
            ListItem listItem = null;
            try
            {
                listItem = file.EnsureProperty(f => f.ListItemAllFields);
            }
            catch { }

            if (listItem != null) { 
            
               modelFile.Properties = listItem.ToProvisioningValues();
            }

            return modelFile;
        }

      

        public void PersistFile(Web web, ProvisioningTemplateCreationInformation creationInfo, PnPMonitoredScope scope, string folderPath, string fileName, Boolean decodeFileName = false)
        {
            creationInfo.PersistFile(folderPath, fileName, web, scope, decodeFileName);
        }
    }
}
