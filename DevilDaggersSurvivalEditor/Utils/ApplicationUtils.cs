using DevilDaggersSurvivalEditor.Models;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DevilDaggersSurvivalEditor.Utils
{
	public static class ApplicationUtils
	{
		public const string ApplicationName = "DevilDaggersSurvivalEditor";
		public const string ApplicationVersionNumber = "2.0.0.0 WIP";

		/// <summary>
		/// Gets the latest version number from the website.
		/// </summary>
		/// <returns>The version number from the website, or an empty string if it could not retrieve the value.</returns>
		private static async Task<string> GetLatestVersionNumber()
		{
			string version = string.Empty;

			string url = UrlUtils.GetToolVersions;

			try
			{
				using (WebClient client = new WebClient())
				{
					using (MemoryStream stream = new MemoryStream(client.DownloadData(url)))
					{
						byte[] byteArray = new byte[1024];
						int count = await stream.ReadAsync(byteArray, 0, 1024);
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
			// TODO: Not here...
			catch (WebException)
			{
				MessageBox.Show($"Could not connect to {url}.", "Error checking for updates");
			}
			catch
			{
				MessageBox.Show($"An unexpected error occured while trying to retrieve the latest version number.", "Error checking for updates");
			}

			return version;
		}

		/// <summary>
		/// Checks whether the application is up to date or not.
		/// </summary>
		/// <returns>The <see cref="VersionResult"/>.</returns>
		public static async Task<VersionResult> IsUpToDate()
		{
			string versionOnline = await GetLatestVersionNumber();

			if (int.TryParse(versionOnline.Replace(".", ""), out int versionNumberOnline) && int.TryParse(ApplicationVersionNumber.Replace(".", ""), out int versionNumberCurrent))
				return new VersionResult(versionNumberOnline <= versionNumberCurrent, versionOnline);

			return new VersionResult(null, versionOnline);
		}
	}
}