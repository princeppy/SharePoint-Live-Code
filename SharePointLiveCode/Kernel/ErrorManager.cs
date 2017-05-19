using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;

using Microsoft.SharePoint.WebControls;

namespace SharePointLiveCode.Kernel
{
	internal class ErrorManager
	{
		public static HtmlGenericControl ErrorControlEditMode
		{
			get
			{
				HtmlGenericControl htmlGenericControl = new HtmlGenericControl("div");
				htmlGenericControl.InnerHtml = "La WebPart non renderizza i contenuti in modalità di Edit";
				return htmlGenericControl;
			}
		}

		public static HtmlGenericControl ErrorControlCurrentCodeId
		{
			get
			{
				HtmlGenericControl htmlGenericControl = new HtmlGenericControl("div");
				htmlGenericControl.InnerHtml = "Impostare la CustomProperty: Current Code Id<br/>Può contenere solo lettere e numeri.";
				return htmlGenericControl;
			}
		}
		public static HtmlGenericControl ErrorControlContainerDivId
		{
			get
			{
				HtmlGenericControl htmlGenericControl = new HtmlGenericControl("div");
				htmlGenericControl.InnerHtml = "Impostare la CustomProperty: Container Div Id<br/>Può contenere solo lettere e numeri.";
				return htmlGenericControl;
			}
		}
	}
}
