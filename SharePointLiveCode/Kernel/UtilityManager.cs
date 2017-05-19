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
	internal class UtilityManager
	{
		private static String _pattern = @"^[a-zA-Z0-9_]+$";

		public static Boolean CheckCurrentCodeId(String currentCodeId)
		{
			Boolean result = true;
			Regex regex = new Regex(_pattern);

			if (String.IsNullOrEmpty(currentCodeId) || regex.IsMatch(currentCodeId) == false)
			{
				result = false;
			}
			return result;
		}
		public static Boolean CheckContainerDivId(String containerDivId)
		{
			Boolean result = true;
			Regex regex = new Regex(_pattern);

			if (String.IsNullOrEmpty(containerDivId) || regex.IsMatch(containerDivId) == false || containerDivId.StartsWith("div") == false)
			{
				result = false;
			}
			return result;
		}
		public static Boolean CheckIsDisplayMode()
		{
			Boolean result = true;
			if (Microsoft.SharePoint.SPContext.Current.FormContext.FormMode != SPControlMode.Display)
			{
				result = false;
			}
			return result;
		}
		public static HtmlGenericControl CreateDebugPanel(Dictionary<String, String> customProperties)
		{
			HtmlGenericControl containerDiv = new HtmlGenericControl("div");
			foreach (KeyValuePair<String, String> customPropery in customProperties)
			{
				HtmlGenericControl rowDiv = new HtmlGenericControl("div");
				rowDiv.InnerHtml = customPropery.Key + ": " + customPropery.Value;
				containerDiv.Controls.Add(rowDiv);
			}
			return containerDiv;
		}
	}
}
