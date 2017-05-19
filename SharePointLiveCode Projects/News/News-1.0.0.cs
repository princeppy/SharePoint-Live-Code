using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SharePointLiveCode_Projects.News
{
	public class NewsJsonData
	{
		public String ListName { get; set; }
		public String ListTitle { get; set; }
		public String NewsFolder { get; set; }
		public String CurrentCodeId { get; set; }
		public String ContainerDivId { get; set; }
	}

	public class News_1
	{
		public String InizializeNews(String jsonData)
		{
			StringBuilder sb = new StringBuilder();

			try
			{
				JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
				NewsJsonData newsJsonData = javaScriptSerializer.Deserialize<NewsJsonData>(jsonData);

				using (SPSite site = new SPSite(SPContext.Current.Site.ID))
				{
					using (SPWeb web = site.RootWeb)
					{
						SPList list = web.Lists.TryGetList(newsJsonData.ListTitle);

						SPQuery queryFolder = new SPQuery();
						queryFolder.Query = "<Where><Eq><FieldRef Name='Title' /><Value Type='Text'>" + newsJsonData.NewsFolder + "</Value></Eq></Where>";
						SPListItemCollection folders = list.GetItems(queryFolder);

						if (folders.Count > 0)
						{
							foreach (SPListItem folder in folders)
							{
								SPQuery queryItem = new SPQuery();
								queryItem.Folder = folder.Folder;

								SPListItemCollection items = list.GetItems(queryItem);

								foreach (SPListItem item in items)
								{
									sb.Append(item.Title);
									sb.Append("<br/>");
								}
							}
						}
						else
						{
							sb.Append("La Folder '" + newsJsonData.NewsFolder + "' non esiste");
						}
					}
				}
			}
			catch (Exception ex)
			{
				sb.Append(ex.ToString());
			}

			return sb.ToString();
		}
	}
}
