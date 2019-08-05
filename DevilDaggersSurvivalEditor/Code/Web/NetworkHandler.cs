using DevilDaggersCore.CustomLeaderboards;
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

		public List<CustomLeaderboardBase> CustomLeaderboards { get; private set; } = new List<CustomLeaderboardBase>();

		public VersionResult VersionResult { get; set; } = new VersionResult(null, string.Empty, "Version has not yet been retrieved.");

		private static readonly Lazy<NetworkHandler> lazy = new Lazy<NetworkHandler>(() => new NetworkHandler());
		public static NetworkHandler Instance => lazy.Value;

		private NetworkHandler()
		{
		}

		public void RetrieveVersion()
		{
			string url = UrlUtils.GetToolVersions;
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
								VersionResult = new VersionResult(Version.Parse(versionOnline) <= ApplicationUtils.ApplicationVersionNumber, versionOnline, string.Empty);
								break;
							}
						}
					}
				}
			}
			catch (WebException ex)
			{
				string errorMessage = $"Could not connect to '{url}'.";

				VersionResult = new VersionResult(null, string.Empty, errorMessage);
				Program.App.ShowError("Error retrieving latest version number", errorMessage, ex);
			}
			catch (Exception ex)
			{
				string errorMessage = $"An unexpected error occured while trying to retrieve the latest version number from '{url}'.";

				VersionResult = new VersionResult(null, string.Empty, errorMessage);
				Program.App.ShowError("Unexpected error", errorMessage, ex);
			}
		}

		public bool RetrieveSpawnsetList()
		{
			try
			{
				Spawnsets.Clear();
				Authors.Clear();

				string downloadString = string.Empty;
				using (TimeoutWebClient client = new TimeoutWebClient(Timeout))
					downloadString = client.DownloadString(UrlUtils.GetSpawnsets);
				List<SpawnsetFile> spawnsetFiles = JsonConvert.DeserializeObject<List<SpawnsetFile>>(downloadString);

				Authors.Add(new AuthorListEntry(SpawnsetListHandler.AllAuthors, spawnsetFiles.Count));
				foreach (SpawnsetFile sf in spawnsetFiles)
				{
					AuthorListEntry author = new AuthorListEntry(sf.Author, spawnsetFiles.Where(s => s.Author == sf.Author).Count());
					if (!Authors.Any(a => a.Name == author.Name))
						Authors.Add(author);
				}

				foreach (SpawnsetFile sf in spawnsetFiles)
					Spawnsets.Add(new SpawnsetListEntry { SpawnsetFile = sf });

				return true;
			}
			catch (WebException ex)
			{
				Program.App.ShowError("Error retrieving spawnset list", $"Could not connect to '{UrlUtils.GetSpawnsets}'.", ex);
				return false;
			}
			catch (Exception ex)
			{
				Program.App.ShowError("Unexpected error", "An unexpected error occurred.", ex);
				return false;
			}
		}

		public bool RetrieveCustomLeaderboardList()
		{
			try
			{
				// The spawnset list must be retrieved first.
				if (Spawnsets.Count == 0)
				{
					Program.App.ShowError("Could not retrieve custom leaderboard list", "The internal spawnset list is empty. This must be initialized before retrieving the custom leaderboard list.");
					return false;
				}

				string downloadString = string.Empty;
				using (TimeoutWebClient client = new TimeoutWebClient(Timeout))
					downloadString = client.DownloadString(UrlUtils.GetCustomLeaderboards);
				CustomLeaderboards = JsonConvert.DeserializeObject<List<CustomLeaderboardBase>>(downloadString);

				foreach (SpawnsetListEntry entry in Spawnsets)
					entry.HasLeaderboard = CustomLeaderboards.Any(l => l.SpawnsetFileName == entry.SpawnsetFile.FileName);

				return true;
			}
			catch (WebException ex)
			{
				Program.App.ShowError("Error retrieving custom leaderboard list", $"Could not connect to '{UrlUtils.GetCustomLeaderboards}'.", ex);
				return false;
			}
			catch (Exception ex)
			{
				Program.App.ShowError("Unexpected error", "An unexpected error occurred.", ex);
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
				Program.App.ShowError("Unexpected error", "An unexpected error occurred.", ex);

				return null;
			}
		}
	}
}