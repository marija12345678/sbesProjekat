﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Manager
{
	public enum AuditEventTypes
	{
		AuthenticationSuccess = 0,
		AuthorizationSuccess = 1,
		AuthorizationFailure = 2,
		AddingToDatabaseSuccess = 3,
		AddingToDatabaseFailure = 4
	}

	public class AuditEvents
	{
		private static ResourceManager resourceManager = null;
		private static object resourceLock = new object();

		private static ResourceManager ResourceMgr
		{
			get
			{
				lock (resourceLock)
				{
					if (resourceManager == null)
					{
						resourceManager = new ResourceManager
							(typeof(AuditEventFile).ToString(),
							Assembly.GetExecutingAssembly());
					}
					return resourceManager;
				}
			}
		}

		public static string AuthenticationSuccess
		{
			get
			{
				return ResourceMgr.GetString(AuditEventTypes.AuthenticationSuccess.ToString());
			}
		}

		public static string AuthorizationSuccess
		{
			get
			{
				return ResourceMgr.GetString(AuditEventTypes.AuthorizationSuccess.ToString());
			}
		}

		public static string AuthorizationFailure
		{
			get
			{
				return ResourceMgr.GetString(AuditEventTypes.AuthorizationFailure.ToString());
			}
		}

		public static string AddingToDatabaseSuccess
		{
			get
			{
				return ResourceMgr.GetString(AuditEventTypes.AddingToDatabaseSuccess.ToString());
			}
		}

		public static string AddingToDatabaseFailure
		{
			get
			{
				return ResourceMgr.GetString(AuditEventTypes.AddingToDatabaseFailure.ToString());
			}
		}
	}
}
