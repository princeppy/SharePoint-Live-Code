using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using System.Reflection;

namespace SharePointLiveCode
{
	/// <summary>
	/// Classe che setta la variabile s_AdministrationAllowedInCurrentProcess = null 
	/// per l'esecuzione del codce in ambiente adminisrator
	/// </summary>
	public static class ReflectionSPSecurity
	{
		/// <summary>
		/// Esegue del codice sia in modalita' Amministrazione sia con privilegi elevati
		/// </summary>
		/// <param name="codeToRunInAdminMode"></param>
		public static void RunInAdminMode(SPSecurity.CodeToRunElevated codeToRunInAdminMode)
		{
			var adminProp = typeof(SPSecurity).GetField("s_AdministrationAllowedInCurrentProcess", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
			var originalValue = adminProp.GetValue(null);

			// imposta a true la modalita' di amministrazione
			adminProp.SetValue(null, true);
			try
			{
				SPSecurity.RunWithElevatedPrivileges(codeToRunInAdminMode);
			}
			catch
			{
				throw;
			}
			finally
			{
				// imposta il valore a come era all'inizio
				adminProp.SetValue(null, originalValue);
			}
		}
	}
}
