using DevilDaggersSurvivalEditor.Code.Logging;
using DevilDaggersSurvivalEditor.Code.Web.Models;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;

namespace DevilDaggersSurvivalEditor.Code.Utils
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

		public static VersionResult CheckVersion()
		{
			string url = UrlUtils.GetToolVersions;

			string versionOnline = string.Empty;
			string errorMessage = string.Empty;

			try
			{
				using (WebClient client = new WebClient())
				{
					using (MemoryStream stream = new MemoryStream(client.DownloadData(url)))
					{
						byte[] byteArray = new byte[1024];
						int count = stream.Read(byteArray, 0, 1024);
						string str = Encoding.UTF8.GetString(byteArray);
						dynamic json = JsonConvert.DeserializeObject(str);
						foreach (dynamic tool in json)
						{
							if ((string)tool.Name == ApplicationName)
							{
								versionOnline = (string)tool.VersionNumber;
								break;
							}
						}
					}
				}
			}
			catch (WebException ex)
			{
				errorMessage = $"Could not connect to {url}.";
				Logger.Log.Error($"Could not connect to {url}.", ex);
			}
			catch (Exception ex)
			{
				errorMessage = "An unexpected error occured while trying to retrieve the latest version number.";
				Logger.Log.Error("An unexpected error occured while trying to retrieve the latest version number.", ex);
			}

			return new VersionResult(!string.IsNullOrEmpty(errorMessage) ? null : (bool?)(Version.Parse(versionOnline) <= ApplicationVersionNumber), versionOnline, errorMessage);
		}
	}
}