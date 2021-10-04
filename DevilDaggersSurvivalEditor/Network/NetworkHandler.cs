using DevilDaggersCore.Spawnsets;
using DevilDaggersSurvivalEditor.Clients;
using System;
using System.Collections.Generic;
using System.IO;
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

		private static readonly Lazy<NetworkHandler> _lazy = new(() => new());

		private NetworkHandler()
		{
			ApiClient = new(new() { BaseAddress = new(BaseUrl) });
		}

		public static NetworkHandler Instance => _lazy.Value;

		public DevilDaggersInfoApiClient ApiClient { get; }

		public Tool? Tool { get; private set; }

		public List<SpawnsetFile> Spawnsets { get; } = new();

		public bool GetOnlineTool()
		{
			try
			{
				Tool = ApiClient.Tools_GetToolAsync(App.ApplicationName).Result;

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
				Spawnsets.AddRange(await ApiClient.Spawnsets_GetSpawnsetsForDdseAsync());

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
				using MemoryStream memoryStream = new();
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
