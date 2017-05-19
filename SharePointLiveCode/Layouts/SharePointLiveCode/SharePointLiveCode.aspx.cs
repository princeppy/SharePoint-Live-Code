using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.IO;
using System.Web.Script.Serialization;
using System.Web.Security.AntiXss;

using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Administration;
using SharePointLiveCode.Kernel;

namespace SharePointLiveCode
{
	public class SharePointLiveCode : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			StringBuilder sb = new StringBuilder();

			Int32 statusCode = 200;

			try
			{
				String currentCodeId = AntiXssEncoder.HtmlEncode(Request["CurrentCodeId"], true);
				String containerDivId = AntiXssEncoder.HtmlEncode(Request["ContainerDivId"], true);
				String codeLibraryServerName = AntiXssEncoder.HtmlEncode(Request["CodeLibraryServerName"], true);
				String codeCommonServerName = AntiXssEncoder.HtmlEncode(Request["CodeCommonServerName"], true);
				String codeFolderServerName = AntiXssEncoder.HtmlEncode(Request["CodeFolderServerName"], true);
				String fileNameCS = AntiXssEncoder.HtmlEncode(Request["FileNameCS"], true);
				String classNameCS = AntiXssEncoder.HtmlEncode(Request["ClassNameCS"], true);
				String methodNameCS = AntiXssEncoder.HtmlEncode(Request["MethodNameCS"], true);
				String jsonData = AntiXssEncoder.HtmlEncode(Request["JsonData"], true);
				jsonData = jsonData.Replace("&sect;", "'");

				if (
					codeFolderServerName.Equals(String.Empty) == false &&
					fileNameCS.Equals(String.Empty) == false &&
					classNameCS.Equals(String.Empty) == false &&
					methodNameCS.Equals(String.Empty) == false
				)
				{
					CodeManager codeManager = new CodeManager();
					codeManager.CurrentCodeId = currentCodeId;
					codeManager.ContainerDivId = containerDivId;
					codeManager.CodeLibraryClientName = String.Empty;
					codeManager.CodeCommonClientName = String.Empty;
					codeManager.CodeFolderClientName = String.Empty;
					codeManager.EntryPointClientFunction = String.Empty;
					codeManager.JsonData = jsonData;

					codeManager.RenderAsynchronousMode = false;
					codeManager.CodeLibraryServerName = codeLibraryServerName;
					codeManager.CodeCommonServerName = codeCommonServerName;
					codeManager.CodeFolderServerName = codeFolderServerName;
					codeManager.FileNameCS = fileNameCS;
					codeManager.ClassNameCS = classNameCS;
					codeManager.MethodNameCS = methodNameCS;

					codeManager.SharePointPage = null;

					sb.Append(codeManager.RenderServerCode());
				}
			}
			catch (Exception ex)
			{
				statusCode = 500;

				SPDiagnosticsService.Local.WriteTrace(0,
					new SPDiagnosticsCategory("SharePointLiveCode", TraceSeverity.Unexpected, EventSeverity.Error),
					TraceSeverity.Unexpected,
						"SharePointLiveCode - " + ex.Message, null);
			}
			finally
			{
				//String json = "{\"name\":\"PAOLO\"}";
				//Response.ContentType = "application/json; charset=utf-8";

				Response.ClearHeaders();
				Response.ClearContent();
				Response.StatusCode = statusCode;
				Response.ContentType = "text/html; charset=utf-8";
				Response.Write(sb.ToString());
				Response.End();
			}
		}
	}
}
