using DevilDaggersCore.Spawnsets;
using DevilDaggersCore.Spawnsets.Web;
using DevilDaggersSurvivalEditor.Code.Spawnsets.SpawnsetList;
using DevilDaggersSurvivalEditor.Code.Web.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace DevilDaggersSurvivalEditor.Code.Web
{
	public sealed class NetworkHandler
	{
		private const int Timeout = 7500;

		public List<AuthorListEntry> Authors { get; private set; } = new List<AuthorListEntry>();
		public List<SpawnsetListEntry> Spawnsets { get; private set; } = new List<SpawnsetListEntry>();

		public VersionResult VersionResult { get; set; } = new VersionResult(null, string.Empty, "Version has not yet been retrieved.");

		private static readonly Lazy<NetworkHandler> lazy = new Lazy<NetworkHandler>(() => new NetworkHandler());
		public static NetworkHandler Instance => lazy.Value;

		private NetworkHandler()
		{
		}

		public void RetrieveVersion()
		{
			string url = UrlUtils.GetToolVersions;
			string errorMessage = string.Empty;

			try
			{
				using (TimeoutWebClient client = new TimeoutWebClient(Timeout))
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
								string versionOnline = (string)tool.VersionNumber;
								VersionResult = new VersionResult(!string.IsNullOrEmpty(errorMessage) ? null : (bool?)(Version.Parse(versionOnline) <= ApplicationUtils.ApplicationVersionNumber), versionOnline, errorMessage);
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
		}

		public bool RetrieveSpawnsetList()
		{
			try
			{
				string downloadString = string.Empty;
				using (TimeoutWebClient client = new TimeoutWebClient(Timeout))
					downloadString = client.DownloadString(UrlUtils.GetSpawnsets);
				List<SpawnsetFile> spawnsetFiles = JsonConvert.DeserializeObject<List<SpawnsetFile>>(downloadString);

				Authors.Clear();
				Authors.Add(new AuthorListEntry(SpawnsetListHandler.AllAuthors, spawnsetFiles.Count));
				foreach (SpawnsetFile sf in spawnsetFiles)
				{
					AuthorListEntry author = new AuthorListEntry(sf.Author, spawnsetFiles.Where(s => s.Author == sf.Author).Count());
					if (!Authors.Any(a => a.Name == author.Name))
						Authors.Add(author);
				}

				downloadString = string.Empty;
				using (TimeoutWebClient client = new TimeoutWebClient(Timeout))
					downloadString = client.DownloadString(UrlUtils.GetCustomLeaderboards);
				dynamic customLeaderboards = JsonConvert.DeserializeObject(downloadString);

				List<string> customLeaderboardNames = new List<string>();
				foreach (dynamic json in customLeaderboards)
					customLeaderboardNames.Add(json.SpawnsetFileName.ToString());

				foreach (SpawnsetFile sf in spawnsetFiles)
					Spawnsets.Add(new SpawnsetListEntry(sf, customLeaderboardNames.Contains(sf.FileName)));

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

				using (TimeoutWebClient client = new TimeoutWebClient(Timeout))
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