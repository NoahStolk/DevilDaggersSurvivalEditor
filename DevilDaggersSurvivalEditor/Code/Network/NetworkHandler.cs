using DevilDaggersCore.CustomLeaderboards;
using DevilDaggersCore.Spawnsets;
using DevilDaggersCore.Spawnsets.Web;
using DevilDaggersCore.Tools;
using DevilDaggersSurvivalEditor.Code.Spawnsets.SpawnsetList;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace DevilDaggersSurvivalEditor.Code.Network
{
	internal sealed class NetworkHandler
	{
		/// <summary>
		/// Timeout in milliseconds.
		/// </summary>
		private const int timeout = 7500;

		internal List<AuthorListEntry> Authors { get; private set; } = new List<AuthorListEntry>();
		internal List<SpawnsetListEntry> Spawnsets { get; private set; } = new List<SpawnsetListEntry>();

		internal List<CustomLeaderboardBase> CustomLeaderboards { get; private set; } = new List<CustomLeaderboardBase>();

		private static readonly Lazy<NetworkHandler> lazy = new Lazy<NetworkHandler>(() => new NetworkHandler());
		internal static NetworkHandler Instance => lazy.Value;

		private NetworkHandler()
		{
		}

		internal bool RetrieveSpawnsetList()
		{
			try
			{
				Spawnsets.Clear();
				Authors.Clear();

				string downloadString = string.Empty;
				using (TimeoutWebClient client = new TimeoutWebClient(timeout))
					downloadString = client.DownloadString(UrlUtils.ApiGetSpawnsets);
				List<SpawnsetFile> spawnsetFiles = JsonConvert.DeserializeObject<List<SpawnsetFile>>(downloadString);

				Authors.Add(new AuthorListEntry(SpawnsetListHandler.AllAuthors, spawnsetFiles.Count));
				foreach (SpawnsetFile sf in spawnsetFiles)
				{
					AuthorListEntry author = new AuthorListEntry(sf.Author, spawnsetFiles.Where(s => s.Author == sf.Author).Count());
					if (!Authors.Any(a => a.Name == author.Name))
						Authors.Add(author);
				}

				foreach (SpawnsetFile spawnsetFile in spawnsetFiles)
					Spawnsets.Add(new SpawnsetListEntry { SpawnsetFile = spawnsetFile });

				return true;
			}
			catch (WebException ex)
			{
				App.Instance.ShowError("Error retrieving spawnset list", $"Could not connect to '{UrlUtils.ApiGetSpawnsets}'.", ex);
				return false;
			}
			catch (Exception ex)
			{
				App.Instance.ShowError("Unexpected error", "An unexpected error occurred.", ex);
				return false;
			}
		}

		internal bool RetrieveCustomLeaderboardList()
		{
			try
			{
				// The spawnset list must be retrieved first.
				if (Spawnsets.Count == 0)
				{
					App.Instance.ShowError("Could not retrieve custom leaderboard list", "The internal spawnset list is empty. This must be initialized before retrieving the custom leaderboard list.");
					return false;
				}

				string downloadString = string.Empty;
				using (TimeoutWebClient client = new TimeoutWebClient(timeout))
					downloadString = client.DownloadString(UrlUtils.ApiGetCustomLeaderboards);
				CustomLeaderboards = JsonConvert.DeserializeObject<List<CustomLeaderboardBase>>(downloadString);

				foreach (SpawnsetListEntry entry in Spawnsets)
					entry.HasLeaderboard = CustomLeaderboards.Any(l => l.SpawnsetFileName == entry.SpawnsetFile.FileName);

				return true;
			}
			catch (WebException ex)
			{
				App.Instance.ShowError("Error retrieving custom leaderboard list", $"Could not connect to '{UrlUtils.ApiGetCustomLeaderboards}'.", ex);
				return false;
			}
			catch (Exception ex)
			{
				App.Instance.ShowError("Unexpected error", "An unexpected error occurred.", ex);
				return false;
			}
		}

		internal Spawnset DownloadSpawnset(string fileName)
		{
			string url = UrlUtils.ApiGetSpawnset(fileName);

			try
			{
				Spawnset spawnset;

				using (TimeoutWebClient client = new TimeoutWebClient(timeout))
				using (Stream stream = new MemoryStream(client.DownloadData(url)))
					if (!Spawnset.TryParse(stream, out spawnset))
						App.Instance.ShowError("Error parsing file", "Could not parse file.");

				return spawnset;
			}
			catch (WebException ex)
			{
				App.Instance.ShowError("Error downloading file", $"Could not connect to '{url}'.", ex);

				return null;
			}
			catch (Exception ex)
			{
				App.Instance.ShowError("Unexpected error", "An unexpected error occurred.", ex);

				return null;
			}
		}
	}
}