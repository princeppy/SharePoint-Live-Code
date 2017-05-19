using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Collections.Generic;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Text;

using SharePointLiveCode.Kernel;

namespace SharePointLiveCode.ClientCode
{
	[ToolboxItemAttribute(false)]
	public class ClientCode : WebPart
	{
		private const String _newCategoryGeneric = "Generic Properties";
		private const String _newCategoryClient = "Client Properties";
		private const String _newCategoryData = "Data Properties";

		#region " GENERIC "
		[WebBrowsable(true),
		WebDisplayName("Current Code Id:"),
		WebDescription("Identificativo del codice Javascript, non utilizzare caratteri speciali o spazi. Es: b1"),
		Personalizable(PersonalizationScope.Shared),
		Category(_newCategoryGeneric)]
		public String CurrentCodeId { get; set; } = String.Empty;

		[WebBrowsable(true),
		WebDisplayName("Container Div Id:"),
		WebDescription("Identificativo del div che conterrà la webpart, non utilizzare caratteri speciali o spazi.. Es: divBanner1"),
		Personalizable(PersonalizationScope.Shared),
		Category(_newCategoryGeneric)]
		public String ContainerDivId { get; set; } = String.Empty;

		//[WebBrowsable(true),
		//WebDisplayName("Render Client Side:"),
		//WebDescription("Selezionare questa opzione se si vuole una renderizzazione lato client. Deselezionare questa opzione se si vuole una renderizzazione lato server. Se si seglie una renderizzazione lato client si potrà utilizzare solo del codice Javascript e nessun richiamo a codice c#. Nel caso in cui ci fosse la necessità di utilizzare del codice c# in modalità Asincrona occorre abilitare la proprietà Render Server Side e, nelle Server Properties, abilitare anche la proprietà Render Asynchronous Mode in questo modo il codice c# verrà eseguito da una pagina nascosta attraverso una chiamata Ajax"),
		//Personalizable(PersonalizationScope.Shared),
		//Category(_newCategoryGeneric)]
		//public Boolean RenderClientSide { get; set; } = false;

		#endregion

		#region " CLIENT "
		[WebBrowsable(true),
		WebDisplayName("Code Library Client Name:"),
		WebDescription("Inserire il nome della document library di SharePoint contentente il codice sorgente lato Client (Javascript). Es: CodeLibraryClient"),
		Personalizable(PersonalizationScope.Shared),
		Category(_newCategoryClient)]
		public String CodeLibraryClientName { get; set; } = String.Empty;

		[WebBrowsable(true),
		WebDisplayName("Code Folder Common Client Name:"),
		WebDescription("Inserire il nome della Cartella Comune dei file sorgenti. Es: _Common"),
		Personalizable(PersonalizationScope.Shared),
		Category(_newCategoryClient)]
		public String CodeCommonClientName { get; set; } = String.Empty;

		[WebBrowsable(true),
		WebDisplayName("Code Folder Client Name:"),
		WebDescription("Inserire il nome della Cartella di Origine dei file sorgenti. Es: Banner"),
		Personalizable(PersonalizationScope.Shared),
		Category(_newCategoryClient)]
		public String CodeFolderClientName { get; set; } = String.Empty;

		[WebBrowsable(true),
		WebDisplayName("Entry Point Client Function:"),
		WebDescription("Inserire il nome della funzione Main del codice sorgente lato Client. Es: InizializeBanner. A runtime verrà passato il valore del JsonData"),
		Personalizable(PersonalizationScope.Shared),
		Category(_newCategoryClient)]
		public String EntryPointClientFunction { get; set; } = String.Empty;

		#endregion

		#region " DATA "
		[WebBrowsable(true),
		WebDisplayName("JSON Data:"),
		WebDescription("Dati applicativi strutturati nel formato JSON. Questi dati sono definiti dal programmatore della WebPart e vengono registrati in pagina o mandati via Ajax alla pagina lato Server. Verranno aggiunti i valori del Current Code Id e del Container Div Id. Es: { 'key1': 'val1', 'key': 'val2', 'CurrentCodeId': 'b1', 'ContainerDivId': 'divBanner1' }"),
		Personalizable(PersonalizationScope.Shared),
		Category(_newCategoryData)]
		public String JsonData { get; set; } = String.Empty;
		#endregion

		protected override void CreateChildControls()
		{
			try
			{
				//CHECK IF PAGE IS IN DISPLAY MODE. 
				if (UtilityManager.CheckIsDisplayMode() == false)
				{
					Dictionary<String, String> customProperties = new Dictionary<String, String>();

					customProperties.Add("CurrentCodeId", this.CurrentCodeId);
					customProperties.Add("ContainerDivId", this.ContainerDivId);
					customProperties.Add("CodeLibraryClientName", this.CodeLibraryClientName);
					customProperties.Add("CodeCommonClientName", this.CodeCommonClientName);
					customProperties.Add("CodeFolderClientName", this.CodeFolderClientName);
					customProperties.Add("EntryPointClientFunction", this.EntryPointClientFunction);
					customProperties.Add("JsonData", this.JsonData);

					//TO-DO
					//var propertyInfos = this.GetType().GetRuntimeProperties();
					//foreach (var propertyInfo in propertyInfos)
					//{
					//	if (this.GetType().GetRuntimeProperty(propertyInfo.Name).CustomAttributes
					//}

					this.Controls.Add(UtilityManager.CreateDebugPanel(customProperties));
					return;
				}
				//CHECK IF CURRENT CODE ID IS IN CORRECT FORMAT
				if (UtilityManager.CheckCurrentCodeId(this.CurrentCodeId) == false)
				{
					this.Controls.Add(ErrorManager.ErrorControlCurrentCodeId);
					return;
				}
				//CHECK IF CONTAINER DIV ID IS IN CORRECT FORMAT
				if (UtilityManager.CheckContainerDivId(this.ContainerDivId) == false)
				{
					this.Controls.Add(ErrorManager.ErrorControlContainerDivId);
					return;
				}
				else
				{
					HtmlGenericControl containerDiv = new HtmlGenericControl("div");
					containerDiv.Attributes.Add("id", this.ContainerDivId);
					containerDiv.Style.Add(HtmlTextWriterStyle.Display, "none");
					this.Controls.Add(containerDiv);
				}

				CodeManager codeManager = new CodeManager();
				codeManager.CurrentCodeId = this.CurrentCodeId;
				codeManager.ContainerDivId = this.ContainerDivId;
				codeManager.CodeLibraryClientName = this.CodeLibraryClientName;
				codeManager.CodeCommonClientName = this.CodeCommonClientName;
				codeManager.CodeFolderClientName = this.CodeFolderClientName;
				codeManager.EntryPointClientFunction = this.EntryPointClientFunction;
				codeManager.JsonData = this.JsonData;

				codeManager.SharePointPage = this.Page;

				codeManager.RenderClientCode();
			}
			catch (Exception ex)
			{
				HtmlGenericControl errorDiv = new HtmlGenericControl("div");
				errorDiv.InnerHtml = "CreateChildControls: " + ex.ToString();
				this.Controls.Add(errorDiv);
			}
		}
	}
}
