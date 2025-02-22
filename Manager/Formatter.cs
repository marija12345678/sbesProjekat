﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manager
{
	public class Formatter
	{
		/// <summary>
		/// Returns username based on the Windows Logon Name. 
		/// </summary>
		/// <param name="winLogonName"> Windows logon name can be formatted either as a UPN (<username>@<domain_name>) or a SPN (<domain_name>\<username>) </param>
		/// <returns> username </returns>
		public static string ParseName(string winLogonName)
		{
			string[] parts = new string[] { };

			if (winLogonName.Contains("@"))
			{
				///UPN format
				parts = winLogonName.Split('@');
				return parts[0];
			}
			else if (winLogonName.Contains("\\"))
			{
				/// SPN format
				parts = winLogonName.Split('\\');
				return parts[1];
			}
			else if (winLogonName.Contains("CN"))
			{
				int startIndex = winLogonName.IndexOf("=") + 1;
				int endIndex = winLogonName.IndexOf(";");
				string s = winLogonName.Substring(startIndex, endIndex - startIndex);
				string name = s.Split(',')[0];
				return name;
			}
			else
			{
				return winLogonName;
			}
		}
	}
}
