using DevilDaggersCore.Spawnsets;
using DevilDaggersSurvivalEditor.Code.Clients;
using DevilDaggersSurvivalEditor.Code.Spawnsets.SpawnsetList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DevilDaggersSurvivalEditor.Code.Network
{
	public sealed class NetworkHandler
	{
#if DEBUG
		public static readonly string BaseUrl = "http://localhost:2963";
#else
		public static readonly string BaseUrl = "https://devildaggers.info";
#endif

		private static readonly Lazy<NetworkHandler> lazy = new Lazy<NetworkHandler>(() => new NetworkHandler());

		private NetworkHandler()
		{
			HttpClient httpClient = new HttpClient
			{
				BaseAddress = new Uri(BaseUrl),
			};
			ApiClient = new DevilDaggersInfoApiClient(httpClient);
		}

		public static NetworkHandler Instance => lazy.Value;

		public DevilDaggersInfoApiClient ApiClient { get; }

		public Tool? Tool { get; private set; }

		public List<AuthorListEntry> Authors { get; private set; } = new List<AuthorListEntry>();

		public List<SpawnsetListEntry> Spawnsets { get; private set; } = new List<SpawnsetListEntry>();

		public List<CustomLeaderboard> CustomLeaderboards { get; private set; } = new List<CustomLeaderboard>();

		public async Task<bool> GetOnlineTool()
		{
			try
			{
				Tool = (await ApiClient.Tools_GetToolsAsync(App.ApplicationName)).First();
				return true;
			}
			catch (Exception ex)
			{
				App.Instance.ShowError("Error retrieving tool information", "An error occurred while attempting to retrieve tool information from the API.", ex);
				return false;
			}
		}

		public async Task<bool> RetrieveSpawnsetList()
		{
			try
			{
				Spawnsets.Clear();
				Authors.Clear();

				List<SpawnsetFile> spawnsetFiles = await ApiClient.Spawnsets_GetSpawnsetsAsync();

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
			catch (Exception ex)
			{
				App.Instance.ShowError("Error retrieving spawnset list", "An error occurred while attempting to retrieve spawnsets from the API.", ex);
				return false;
			}
		}

		public async Task<bool> RetrieveCustomLeaderboardList()
		{
			try
			{
				// The spawnset list must be retrieved first.
				if (Spawnsets.Count == 0)
				{
					App.Instance.ShowError("Could not retrieve custom leaderboard list", "The internal spawnset list is empty. This must be initialized before retrieving the custom leaderboard list.");
					return false;
				}

				CustomLeaderboards = await ApiClient.CustomLeaderboards_GetCustomLeaderboardsAsync();

				foreach (SpawnsetListEntry entry in Spawnsets)
					entry.HasLeaderboard = CustomLeaderboards.Any(l => l.SpawnsetFileName == entry.SpawnsetFile.FileName);

				return true;
			}
			catch (Exception ex)
			{
				App.Instance.ShowError("Error retrieving custom leaderboard list", "An error occurred while attempting to retrieve custom leaderboards from the API.", ex);
				return false;
			}
		}

		public async Task<Spawnset?> DownloadSpawnset(string fileName)
		{
			try
			{
				Spawnset spawnset;

				await ApiClient.Spawnsets_GetSpawnsetFileAsync(fileName); // TODO
				using (Stream stream = new MemoryStream())
				{
					if (!Spawnset.TryParse(stream, out spawnset))
						App.Instance.ShowError("Error parsing file", "Could not parse file.");
				}

				return spawnset;
			}
			catch (Exception ex)
			{
				App.Instance.ShowError("Error downloading file", "An error occurred while attempting to download spawnset from the API.", ex);
				return null;
			}
		}
	}
}