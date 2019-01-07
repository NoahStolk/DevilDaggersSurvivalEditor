using DevilDaggersSurvivalEditor.Models;
using NetBase.Extensions;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Text;

namespace DevilDaggersSurvivalEditor.Utils
{
	public static class ApplicationUtils
	{
		public const string ApplicationName = "DevilDaggersSurvivalEditor";
		public const string ApplicationVersionNumber = "2.0.0.0 WIP";

		public static VersionResult CheckVersion()
		{
			string url = UrlUtils.GetToolVersions;

			string version = string.Empty;
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
								version = (string)tool.VersionNumber;
								break;
							}
						}
					}
				}
			}
			catch (WebException)
			{
				errorMessage = $"Could not connect to {url}.";
			}
			catch
			{
				errorMessage = $"An unexpected error occured while trying to retrieve the latest version number.";
			}

			return new VersionResult(errorMessage != null ? null : (bool?)(int.Parse(version.Numeric()) <= int.Parse(ApplicationVersionNumber.Numeric())), version, errorMessage);
		}
	}
}