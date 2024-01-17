using DevilDaggersSurvivalEditor.Clients;
using DevilDaggersSurvivalEditor.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DevilDaggersSurvivalEditor.Network;

public sealed class NetworkHandler
{
#if TESTING
	private const string _baseUrl = "https://localhost:44318";
#else
	private const string _baseUrl = "https://devildaggers.info";
#endif

	private static readonly Lazy<NetworkHandler> _lazy = new(() => new());

	private readonly DevilDaggersInfoApiClient _apiClient;

	private NetworkHandler()
	{
		_apiClient = new(new() { BaseAddress = new(_baseUrl) });
	}

	public static NetworkHandler Instance => _lazy.Value;

	public List<GetSpawnsetDdse> Spawnsets { get; } = new();

	public async Task<bool> RetrieveSpawnsetList()
	{
		try
		{
			Spawnsets.Clear();
			Spawnsets.AddRange(await _apiClient.Spawnsets_GetSpawnsetsAsync());

			return true;
		}
		catch (Exception ex)
		{
			App.Instance.ShowError("Error retrieving spawnset list", "An error occurred while attempting to retrieve spawnsets from the API.", ex);
			return false;
		}
	}

	public async Task<Spawnset?> DownloadSpawnset(int spawnsetId)
	{
		try
		{
			using FileResponse fileResponse = await _apiClient.Spawnsets_GetSpawnsetFileAsync(spawnsetId);
			await using MemoryStream memoryStream = new();
			await fileResponse.Stream.CopyToAsync(memoryStream);
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
