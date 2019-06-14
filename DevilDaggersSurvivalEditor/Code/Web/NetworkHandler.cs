using DevilDaggersCore.Spawnset;
using DevilDaggersCore.Spawnset.Web;
using DevilDaggersSurvivalEditor.Code.Arena;
using DevilDaggersSurvivalEditor.Code.Web.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace DevilDaggersSurvivalEditor.Code.Web
{
	public sealed class NetworkHandler
	{
		public IReadOnlyList<SpawnsetFile> SpawnsetFiles { get; private set; } = new List<SpawnsetFile>();

		private static readonly Lazy<NetworkHandler> lazy = new Lazy<NetworkHandler>(() => new NetworkHandler());
		public static NetworkHandler Instance => lazy.Value;

		private NetworkHandler()
		{
		}

		public VersionResult RetrieveVersion()
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
						stream.Read(byteArray, 0, 1024);

						dynamic json = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(byteArray));
						foreach (dynamic tool in json)
						{
							if ((string)tool.Name == ApplicationUtils.ApplicationName)
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
				errorMessage = $"Could not connect to '{url}'.";
				Program.App.ShowError("Error", errorMessage, ex);
			}
			catch (Exception ex)
			{
				errorMessage = $"An unexpected error occured while trying to retrieve the latest version number from '{url}'.";
				Program.App.ShowError("Error", errorMessage, ex);
			}

			return new VersionResult(!string.IsNullOrEmpty(errorMessage) ? null : (bool?)(Version.Parse(versionOnline) <= ApplicationUtils.ApplicationVersionNumber), versionOnline, errorMessage);
		}

		public bool RetrieveSpawnsetList()
		{
			try
			{
				string downloadString = string.Empty;
				using (WebClient client = new WebClient())
					downloadString = client.DownloadString(UrlUtils.GetSpawnsets);
				SpawnsetFiles = JsonConvert.DeserializeObject<List<SpawnsetFile>>(downloadString);
				return true;
			}
			catch (WebException ex)
			{
				Program.App.ShowError("Error retrieving spawnset list", $"Could not connect to '{UrlUtils.GetSpawnsets}'.", ex);
				return false;
			}
			catch (Exception ex)
			{
				Program.App.ShowError("An unexpected error occurred", "An unexpected error occurred.", ex);
				return false;
			}
		}

		public Spawnset DownloadSpawnset(string fileName)
		{
			string url = UrlUtils.GetSpawnset(fileName);

			try
			{
				Spawnset spawnset;

				using (WebClient client = new WebClient())
				using (Stream stream = new MemoryStream(client.DownloadData(url)))
					if (!Spawnset.TryParse(stream, out spawnset))
						Program.App.ShowError("Error parsing file", "Could not parse file.");

				return spawnset;
			}
			catch (WebException ex)
			{
				Program.App.ShowError("Error downloading file", $"Could not connect to '{url}'.", ex);

				return null;
			}
			catch (Exception ex)
			{
				Program.App.ShowError("An unexpected error occurred", "An unexpected error occurred.", ex);

				return null;
			}
		}
	}
}