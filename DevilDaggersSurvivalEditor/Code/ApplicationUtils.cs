using DevilDaggersSurvivalEditor.Code.Web;
using DevilDaggersSurvivalEditor.Code.Web.Models;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;

namespace DevilDaggersSurvivalEditor.Code
{
	public static class ApplicationUtils
	{
		public const string ApplicationName = "DevilDaggersSurvivalEditor";

		private static Version applicationVersionNumber;
		public static Version ApplicationVersionNumber
		{
			get
			{
				if (applicationVersionNumber == null)
					applicationVersionNumber = Version.Parse(FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion);
				return applicationVersionNumber;
			}
		}
	}
}