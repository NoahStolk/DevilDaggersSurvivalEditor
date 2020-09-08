using DevilDaggersCore.Spawnsets;
using DevilDaggersSurvivalEditor.Clients;
using DevilDaggersSurvivalEditor.Spawnsets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DevilDaggersSurvivalEditor.Network
{
	public sealed class NetworkHandler
	{
#if TESTING
		public static readonly string BaseUrl = "http://localhost:2963";
#else
		public static readonly string BaseUrl = "https://devildaggers.info";
#endif

		private static readonly Lazy<NetworkHandler> _lazy = new Lazy<NetworkHandler>(() => new NetworkHandler());

		private NetworkHandler()
		{
			HttpClient httpClient = new HttpClient
			{
				BaseAddress = new Uri(BaseUrl),
			};
			ApiClient = new DevilDaggersInfoApiClient(httpClient);
		}

		public static NetworkHandler Instance => _lazy.Value;

		public DevilDaggersInfoApiClient ApiClient { get; }

		public Tool? Tool { get; private set; }

		public List<AuthorListEntry> Authors { get; private set; } = new List<AuthorListEntry>();

		public List<SpawnsetFile> Spawnsets { get; private set; } = new List<SpawnsetFile>();

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
					AuthorListEntry author = new AuthorListEntry(sf.AuthorName, spawnsetFiles.Count(s => s.AuthorName == sf.AuthorName));
					if (!Authors.Any(a => a.Name == author.Name))
						Authors.Add(author);
				}

				Spawnsets.AddRange(spawnsetFiles);

				return true;
			}
			catch (Exception ex)
			{
				App.Instance.ShowError("Error retrieving spawnset list", "An error occurred while attempting to retrieve spawnsets from the API.", ex);
				return false;
			}
		}

		public async Task<Spawnset?> DownloadSpawnset(string fileName)
		{
			try
			{
				using FileResponse fileResponse = await ApiClient.Spawnsets_GetSpawnsetFileAsync(fileName);
				using MemoryStream memoryStream = new MemoryStream();
				fileResponse.Stream.CopyTo(memoryStream);
				byte[] bytes = memoryStream.ToArray();
				if (!Spawnset.TryParse(bytes, out Spawnset spawnset))
					App.Instance.ShowError("Error parsing file", "Could not parse file.");

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