using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Extensions;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.AppModelExtensions
{
    public static partial class ListItemExtensions
    {
        public static DataRow ToDataRow(this ListItem listItem)
        {

            return new DataRow(listItem.ToProvisioningValues());
        }
        public static Dictionary<string, string> ToProvisioningValues(this ListItem listItem)
        {
            var list = listItem.ParentList;
            var fields = list.Fields;
            var web = list.ParentWeb;
            var resultList = new Dictionary<string, string>();
            listItem.Context.Load(fields, fs => fs.IncludeWithDefaultProperties(f => f.TypeAsString, f => f.InternalName, f => f.Title));
            listItem.Context.ExecuteQueryRetry();

            var fieldValues = listItem.FieldValues;

            var fieldValuesAsText = listItem.EnsureProperty(li => li.FieldValuesAsText).FieldValues;

            var fieldstoExclude = new[] {
                "ID",
                "GUID",
                "Author",
                "Editor",
                "FileLeafRef",
                "FileRef",
                "File_x0020_Type",
                "Modified_x0020_By",
                "Created_x0020_By",
                "Created",
                "Modified",
                "FileDirRef",
                "Last_x0020_Modified",
                "Created_x0020_Date",
                "File_x0020_Size",
                "FSObjType",
                "IsCheckedoutToLocal",
                "ScopeId",
                "UniqueId",
                "VirusStatus",
                "_Level",
                "_IsCurrentVersion",
                "ItemChildCount",
                "FolderChildCount",
                "SMLastModifiedDate",
                "owshiddenversion",
                "_UIVersion",
                "_UIVersionString",
                "Order",
                "WorkflowVersion",
                "DocConcurrencyNumber",
                "ParentUniqueId",
                "CheckedOutUserId",
                "SyncClientId",
                "CheckedOutTitle",
                "SMTotalSize",
                "SMTotalFileStreamSize",
                "SMTotalFileCount",
                "ParentVersionString",
                "ParentLeafName",
                "SortBehavior",
                "StreamHash",
                "TaxCatchAll",
                "TaxCatchAllLabel",
                "_ModerationStatus",
                //"HtmlDesignAssociated",
                //"HtmlDesignStatusAndPreview",
                "MetaInfo",
                "CheckoutUser",
                "NoExecute",
                "_HasCopyDestinations",
                "ContentVersion",
                "UIVersion",
            };

            foreach (var fieldValue in fieldValues.Where(f => !fieldstoExclude.Contains(f.Key)))
            {
                if (fieldValue.Value != null && !string.IsNullOrEmpty(fieldValue.Value.ToString()))
                {
                    var field = fields.FirstOrDefault(fs => fs.InternalName == fieldValue.Key);

                    string value = string.Empty;

                    switch (field.TypeAsString)
                    {
                        case "URL":
                            value = fieldValuesAsText[fieldValue.Key].TokenizeUrl(web.Url, web);
                            break;
                        case "User":
                            var userFieldValue = fieldValue.Value as Microsoft.SharePoint.Client.FieldUserValue;
                            if (userFieldValue != null)
                            {
#if !ONPREMISES
                                value = userFieldValue.Email;
#else
                                value = userFieldValue.LookupValue;
#endif
                            }
                            break;
                        case "LookupMulti":
                            var lookupFieldValue = fieldValue.Value as Microsoft.SharePoint.Client.FieldLookupValue[];
                            if (lookupFieldValue != null)
                            {
                                value = JsonUtility.Serialize(lookupFieldValue).TokenizeUrl(web.Url);
                            }
                            break;
                        case "TaxonomyFieldType":
                            var taxonomyFieldValue = fieldValue.Value as Microsoft.SharePoint.Client.Taxonomy.TaxonomyFieldValue;
                            if (taxonomyFieldValue != null)
                            {
                                value = JsonUtility.Serialize(taxonomyFieldValue).TokenizeUrl(web.Url);
                            }
                            break;
                        case "TaxonomyFieldTypeMulti":
                            var taxonomyMultiFieldValue = fieldValue.Value as Microsoft.SharePoint.Client.Taxonomy.TaxonomyFieldValueCollection;
                            if (taxonomyMultiFieldValue != null)
                            {
                                value = JsonUtility.Serialize(taxonomyMultiFieldValue).TokenizeUrl(web.Url);
                            }
                            break;
                        case "ContentTypeIdFieldType":
                        default:
                            value = fieldValue.Value.ToString().TokenizeUrl(web.Url, web);
                            break;
                    }

                    if (fieldValue.Key == "ContentTypeId")
                    {
                        // Replace the content typeid with a token
                        var ct = list.GetContentTypeById(value);
                        if (ct != null)
                        {
                            value = string.Format("{{contenttypeid:{0}}}", ct.Name);
                        }
                    }

                    // We process real values only
                    if (value != null && !String.IsNullOrEmpty(value) && value != "[]")
                    {
                        resultList.Add(fieldValue.Key, value);
                    }
                }
            }
            return resultList;
        }

    }
}

