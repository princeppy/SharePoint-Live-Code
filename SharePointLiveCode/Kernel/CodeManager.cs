using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.HtmlControls;

using Microsoft.SharePoint;
using System.IO;
using Microsoft.CSharp;
using System.CodeDom.Compiler;

namespace SharePointLiveCode.Kernel
{
	internal class CodeManager
	{
		public Page SharePointPage { get; set; }

		//---------------------------------------------------------------------------------------
		#region " GENERIC "

		public String CurrentCodeId { get; set; } = String.Empty;

		public String ContainerDivId { get; set; } = String.Empty;

		#endregion

		//---------------------------------------------------------------------------------------
		#region " CLIENT "

		public String CodeLibraryClientName { get; set; } = String.Empty;

		public String CodeCommonClientName { get; set; } = String.Empty;

		public String CodeFolderClientName { get; set; } = String.Empty;

		public String EntryPointClientFunction { get; set; } = String.Empty;

		#endregion

		//---------------------------------------------------------------------------------------
		#region " SERVER "

		public Boolean RenderAsynchronousMode { get; set; } = false;

		public String CodeLibraryServerName { get; set; } = String.Empty;

		public String CodeCommonServerName { get; set; } = String.Empty;

		public String CodeFolderServerName { get; set; } = String.Empty;

		public String FileNameCS { get; set; } = String.Empty;

		public String ClassNameCS { get; set; } = String.Empty;

		public String MethodNameCS { get; set; } = String.Empty;

		#endregion

		//---------------------------------------------------------------------------------------
		#region " DATA "

		public String JsonData { get; set; } = String.Empty;

		#endregion

		//---------------------------------------------------------------------------------------
		#region " CLIENT PROCESS "

		public void RenderClientCode()
		{
			#region " CHECK CLIENT PROPERTIES "

			CheckClientProperties();

			#endregion

			#region " COMMON FOLDER "
			List<String> listCommonClientFile = GetFolderAndFile(this.CodeLibraryClientName + "/" + this.CodeCommonClientName, String.Empty);
			if (listCommonClientFile.Count == 0)
			{
				//return;
			}
			else
			{
				foreach (String commonClientFile in listCommonClientFile)
				{
					String fileName = commonClientFile.Substring(commonClientFile.LastIndexOf("/") + 1);
					if (this.SharePointPage.ClientScript.IsClientScriptIncludeRegistered("ScriptSharePointLiveCode.Common." + fileName) == false)
					{
						this.SharePointPage.ClientScript.RegisterClientScriptInclude(this.GetType(),
							"ScriptSharePointLiveCode.Common." + fileName,
							SPContext.Current.Web.Url + commonClientFile
						);
					}
				}
			}
			#endregion

			#region " FILE FOLDER "
			List<String> listFolderClientFile = GetFolderAndFile(this.CodeLibraryClientName + "/" + this.CodeFolderClientName, String.Empty);
			if (listFolderClientFile.Count == 0)
			{
				//return;
			}
			else
			{
				foreach (String folderClientFile in listFolderClientFile)
				{
					String fileName = folderClientFile.Substring(folderClientFile.LastIndexOf("/") + 1);
					if (this.SharePointPage.ClientScript.IsClientScriptIncludeRegistered("ScriptSharePointLiveCode." + fileName) == false)
					{
						this.SharePointPage.ClientScript.RegisterClientScriptInclude(this.GetType(),
							"ScriptSharePointLiveCode." + fileName,
							folderClientFile
						);
					}
				}
			}
			#endregion

			#region " JSON DATA "
			if (String.IsNullOrEmpty(this.EntryPointClientFunction) == false)
			{
				String jsonData = SetJsonData();
				if (this.SharePointPage.ClientScript.IsClientScriptBlockRegistered("ScriptSharePointLiveCode.JsonData." + this.ContainerDivId) == false)
				{
					StringBuilder sb = new StringBuilder();
					sb.Append("$(document).ready(function() { ");
					sb.Append("SP.SOD.executeFunc('sp.js','SP.ClientContext', " + this.ContainerDivId + "_inizializer); ");
					sb.Append("function " + this.ContainerDivId + "_inizializer() { var jsonData = " + jsonData + "; " + this.EntryPointClientFunction + "(jsonData); } ");
					sb.Append("}); ");

					this.SharePointPage.ClientScript.RegisterClientScriptBlock(this.GetType(),
						"ScriptSharePointLiveCode.JsonData." + this.ContainerDivId,
						sb.ToString(),
						true
					);
				}
			}
			#endregion
		}

		#endregion

		//---------------------------------------------------------------------------------------
		#region " SERVER PROCESS "

		public String RenderServerCode()
		{
			// ---------------------------------------------------------------------------------- CLIENT
			#region " COMMON FOLDER "
			List<String> listCommonClientFile = GetFolderAndFile(this.CodeLibraryClientName + "/" + this.CodeCommonClientName, String.Empty);
			if (listCommonClientFile.Count == 0)
			{
				//return;
			}
			else
			{
				foreach (String commonClientFile in listCommonClientFile)
				{
					String fileName = commonClientFile.Substring(commonClientFile.LastIndexOf("/") + 1);
					if (this.SharePointPage.ClientScript.IsClientScriptIncludeRegistered("ScriptSharePointLiveCode.Common." + fileName) == false)
					{
						this.SharePointPage.ClientScript.RegisterClientScriptInclude(this.GetType(),
							"ScriptSharePointLiveCode.Common." + fileName,
							SPContext.Current.Web.Url + commonClientFile
						);
					}
				}
			}
			#endregion

			#region " FILE FOLDER "
			List<String> listFolderClientFile = GetFolderAndFile(this.CodeLibraryClientName + "/" + this.CodeFolderClientName, String.Empty);
			if (listFolderClientFile.Count == 0)
			{
				//return;
			}
			else
			{
				foreach (String folderClientFile in listFolderClientFile)
				{
					String fileName = folderClientFile.Substring(folderClientFile.LastIndexOf("/") + 1);
					if (this.SharePointPage.ClientScript.IsClientScriptIncludeRegistered("ScriptSharePointLiveCode." + fileName) == false)
					{
						this.SharePointPage.ClientScript.RegisterClientScriptInclude(this.GetType(),
							"ScriptSharePointLiveCode." + fileName,
							folderClientFile
						);
					}
				}
			}
			#endregion

			// ---------------------------------------------------------------------------------- SERVER
			#region " COMMON FOLDER "

			//TO-DO

			#endregion

			#region " FILE FOLDER "

			StringBuilder sb = new StringBuilder();

			//ATTENZIONE:
			//SE E' RICHIESTA LA MODALITA' ASINCRONA RENDERIZZO UN jQuery PER LA CHIAMATA AJAX ALLA PAGINA D'APPOGGIO
			if (this.RenderAsynchronousMode == true)
			{
				sb.AppendLine("<script type='text/javascript'>");

				StringBuilder jsonData = new StringBuilder();
				jsonData.Append("{ ");
				jsonData.Append("'CurrentCodeId':'" + this.CurrentCodeId + "', ");
				jsonData.Append("'ContainerDivId':'" + this.ContainerDivId + "', ");
				jsonData.Append("'CodeLibraryServerName':'" + this.CodeLibraryServerName + "', ");
				jsonData.Append("'CodeCommonServerName':'" + this.CodeCommonServerName + "', ");
				jsonData.Append("'CodeFolderServerName':'" + this.CodeFolderServerName + "', ");
				jsonData.Append("'FileNameCS':'" + this.FileNameCS + "', ");
				jsonData.Append("'ClassNameCS':'" + this.ClassNameCS + "', ");
				jsonData.Append("'MethodNameCS':'" + this.MethodNameCS + "', ");
				jsonData.Append("'JsonData':'" + this.JsonData.Replace("'", "§") + "' ");
				jsonData.Append("} ");

				sb.AppendLine("var sharePointLiveCodeJsonData_" + this.CurrentCodeId + " = " + jsonData + ";");

				sb.AppendLine("jQuery(document).ready(function() { ");

					sb.AppendLine("jQuery.ajax({ ");
						sb.AppendLine("url:_spPageContextInfo.siteAbsoluteUrl + '/_layouts/15/SharePointLiveCode/SharePointLiveCode.aspx', ");
						sb.AppendLine("method:'POST', ");
						sb.AppendLine("data:sharePointLiveCodeJsonData_" + this.CurrentCodeId + ", ");
						sb.AppendLine("success:onSharePointLiveCodeSuccess_" + this.CurrentCodeId + ", ");
						sb.AppendLine("error:onSharePointLiveCodeError_" + this.CurrentCodeId + ", ");
						sb.AppendLine("timeout:120000 ");
					sb.AppendLine("}); ");

				sb.AppendLine("}); ");

				sb.AppendLine("function onSharePointLiveCodeSuccess_" + this.CurrentCodeId + "(data, status) { ");
					sb.AppendLine("jQuery('#" + this.ContainerDivId + "').html(data);");
				sb.AppendLine("} ");

				sb.AppendLine("function onSharePointLiveCodeError_" + this.CurrentCodeId + "(response, textStatus) { ");
					sb.AppendLine("jQuery('#" + this.ContainerDivId + "').html(response.statusText);");
				sb.AppendLine("}");
				sb.AppendLine("</script>");
			}
			else
			{
				
				if (
					this.CodeFolderServerName.Equals(String.Empty) == false &&
					this.FileNameCS.Equals(String.Empty) == false &&
					this.ClassNameCS.Equals(String.Empty) == false &&
					this.MethodNameCS.Equals(String.Empty) == false
				)
				{
					String jsonData = SetJsonData();
					sb.Append(CodeProcess(this.CodeLibraryServerName + "/" + this.CodeFolderServerName,
																								this.FileNameCS,
																								this.ClassNameCS,
																								this.MethodNameCS,
																								jsonData));
				}
			}
			return sb.ToString();
			#endregion
		}

		#endregion

		//---------------------------------------------------------------------------------------
		#region " PRIVATE CODE "

		private String SetJsonData()
		{
			String jsonData = String.Empty;
			String jsonDataFixed = "'CurrentCodeId': '" + this.CurrentCodeId + "', 'ContainerDivId': '" + this.ContainerDivId + "' }";
			if (String.IsNullOrEmpty(this.JsonData) == true)
			{
				jsonData = "{ " + jsonDataFixed;
			}
			else
			{
				jsonData = this.JsonData.Substring(0, this.JsonData.LastIndexOf("}"));
				jsonData = jsonData + ", " + jsonDataFixed;
			}

			return jsonData;
		}

		private void CheckClientProperties()
		{
			#region " VERIFICHE PROPRIETA' CLIENT "

			if (String.IsNullOrEmpty(this.CodeLibraryClientName))
			{
				//HtmlGenericControl errorDiv = new HtmlGenericControl("div");
				//errorDiv.InnerHtml = "Impostare la CustomProperty: Code Library Client Name";
				//this.Controls.Add(errorDiv);
				throw new Exception("Impostare la CustomProperty: Code Library Client Name");
			}
			if (String.IsNullOrEmpty(this.CodeCommonClientName))
			{
				//HtmlGenericControl errorDiv = new HtmlGenericControl("div");
				//errorDiv.InnerHtml = "Impostare la CustomProperty: Code Common Client Name";
				//this.Controls.Add(errorDiv);
				throw new Exception("Impostare la CustomProperty: Code Common Client Name");
			}
			if (String.IsNullOrEmpty(this.CodeFolderClientName))
			{
				//HtmlGenericControl errorDiv = new HtmlGenericControl("div");
				//errorDiv.InnerHtml = "Impostare la CustomProperty: Code Folder Client Name";
				//this.Controls.Add(errorDiv);
				throw new Exception("Impostare la CustomProperty: Code Folder Client Name");
			}
			if (String.IsNullOrEmpty(this.EntryPointClientFunction))
			{
				//HtmlGenericControl errorDiv = new HtmlGenericControl("div");
				//errorDiv.InnerHtml = "Impostare la CustomProperty: File Name Javascript";
				//this.Controls.Add(errorDiv);
				//throw new Exception("Impostare la CustomProperty: Entry Point Client Function");
			}

			#endregion
		}

		private List<String> GetFolderAndFile(String folderPath, String fileName)
		{
			List<String> listFile = new List<String>();
			if (String.IsNullOrEmpty(folderPath) || folderPath.Equals("/"))
			{
				return listFile;
			}

			try
			{
				using (SPSite site = new SPSite(SPContext.Current.Site.ID))
				{
					using (SPWeb web = site.RootWeb)
					{
						String urlFolder = web.ServerRelativeUrl + folderPath;
						SPFolder spFolder = web.GetFolder(urlFolder);
						if (spFolder.Exists == true)
						{
							SPFileCollection files = spFolder.Files;
							if (files != null)
							{
								foreach (SPFile file in files)
								{
									listFile.Add(file.ServerRelativeUrl);

									//HtmlGenericControl errorDiv = new HtmlGenericControl("div");
									//errorDiv.InnerHtml = "file: " + file.ServerRelativeUrl;
									//this.Controls.Add(errorDiv);
								}
							}
						}
						else
						{
							throw new Exception("La Folder: " + urlFolder + " non esiste");
						}
					}
				}

			}
			catch (Exception ex)
			{
				throw new Exception("La Folder:  non esiste");
			}

			return listFile;
		}

		private String CodeProcess(String folderServerRelativePath, String fileNameCS, String classNameCS, String methodNameCS, String jsonData)
		{
			//HtmlGenericControl htmlDivRoot = new HtmlGenericControl("div");
			String resultHtml = String.Empty;

			ReflectionSPSecurity.RunInAdminMode(() =>
			{
				SPUser currentUser2 = SPContext.Current.Web.CurrentUser;

				#region " LETTURA CODICE DALLA LIBRARY "
				String code = String.Empty;
				try
				{
					using (SPSite site = new SPSite(SPContext.Current.Site.ID))
					{
						using (SPWeb web = site.RootWeb)
						{
							String subFolderUrl = web.ServerRelativeUrl + folderServerRelativePath;
							SPFolder subFolder = web.GetFolder(subFolderUrl);


							SPFile file = subFolder.Files[fileNameCS];
							using (StreamReader reader = new StreamReader(file.OpenBinaryStream()))
							{
								code = reader.ReadToEnd();
							}
						}
					}
				}
				catch (Exception ex)
				{
					//htmlDivRoot.InnerHtml = ex.ToString();
					resultHtml = "<div>" + ex.ToString() + "</div>";
				}
				#endregion

				#region " CARICAMENTO CODICE "
				if (code != null && String.IsNullOrEmpty(code) == false)
				{
					Dictionary<string, string> providerOptions = new Dictionary<string, string>
							{
								{"CompilerVersion", "v4.0"}
							};

					// Create a new instance of the C# compiler
					using (CSharpCodeProvider compiler = new CSharpCodeProvider(providerOptions))
					{
						// Create some parameters for the compiler
						var parms = new System.CodeDom.Compiler.CompilerParameters
						{
							GenerateExecutable = false,
							GenerateInMemory = true
						};
						parms.ReferencedAssemblies.Add("System.dll");
						parms.ReferencedAssemblies.Add("System.ComponentModel.DataAnnotations.dll");
						parms.ReferencedAssemblies.Add("System.Configuration.dll");
						parms.ReferencedAssemblies.Add("System.Core.dll");
						parms.ReferencedAssemblies.Add("System.Data.dll");
						parms.ReferencedAssemblies.Add("System.Data.DataSetExtensions.dll");
						parms.ReferencedAssemblies.Add("System.Drawing.dll");
						parms.ReferencedAssemblies.Add("System.EnterpriseServices.dll");
						parms.ReferencedAssemblies.Add("System.Web.dll");
						parms.ReferencedAssemblies.Add("System.Web.ApplicationServices.dll");
						parms.ReferencedAssemblies.Add("System.Web.DynamicData.dll");
						parms.ReferencedAssemblies.Add("System.Web.Entity.dll");
						parms.ReferencedAssemblies.Add("System.Web.Extensions.dll");
						parms.ReferencedAssemblies.Add("System.Web.Services.dll");
						parms.ReferencedAssemblies.Add("System.Xml.dll");
						parms.ReferencedAssemblies.Add("System.Xml.Linq.dll");

						parms.ReferencedAssemblies.Add(@"C:\Program Files\Common Files\microsoft shared\Web Server Extensions\15\ISAPI\Microsoft.SharePoint.dll");

						//var results = compiler.CompileAssemblyFromSource(parms, new string[] { " using System; class MyClass { public String Message(string message) { return message; } }" });
						CompilerResults results = compiler.CompileAssemblyFromSource(parms, code);

						if (results.Errors.Count == 0)
						{
							object[] parameters = new object[1];
							parameters[0] = jsonData;
							//parameters[1] = currentUser2;

							var myClass = results.CompiledAssembly.CreateInstance(classNameCS);
							var returValue = myClass.GetType().
												GetMethod(methodNameCS).
												Invoke(myClass, parameters);

							if (returValue != null)
							{
								//htmlDivRoot.InnerHtml = returValue.ToString();
								resultHtml = returValue.ToString();
							}
						}
						else
						{
							//htmlDivRoot.Controls.Add(CodeProcessErrorFormatter(results.Errors));
							resultHtml = "<div>" + CodeProcessErrorFormatter(results.Errors) + "</div>";
						}
					}
				}
				#endregion
			});

			//return htmlDivRoot;
			return resultHtml;
		}

		//private HtmlGenericControl CodeProcessErrorFormatter(CompilerErrorCollection compilerErrorCollection)
		private String CodeProcessErrorFormatter(CompilerErrorCollection compilerErrorCollection)
		{
			//HtmlGenericControl htmlDivError = new HtmlGenericControl("div");
			StringBuilder sb = new StringBuilder();
			sb.Append("<div>");

			foreach (CompilerError compileError in compilerErrorCollection)
			{
				//htmlDivError.InnerHtml += compileError.ToString();
				//htmlDivError.InnerHtml += "<br/>";
				sb.Append(compileError.ToString());
				sb.Append("<br/>");
			}

			//return htmlDivError;
			return sb.ToString();
		}
		#endregion
	}
}
