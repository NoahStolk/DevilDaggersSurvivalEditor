using DevilDaggersCore.Spawnset;
using DevilDaggersSurvivalEditor.Code.Arena;
using DevilDaggersSurvivalEditor.Code.Logging;
using DevilDaggersSurvivalEditor.Code.Utils;
using DevilDaggersSurvivalEditor.Code.Web.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace DevilDaggersSurvivalEditor.Code.Web
{
	public sealed class SpawnsetListHandler
	{
		public IReadOnlyList<SpawnsetFile> SpawnsetFiles { get; private set; } = new List<SpawnsetFile>();

		private static readonly Lazy<SpawnsetListHandler> lazy = new Lazy<SpawnsetListHandler>(() => new SpawnsetListHandler());
		public static SpawnsetListHandler Instance => lazy.Value;

		private SpawnsetListHandler()
		{
		}

		public void RetrieveSpawnsetList()
		{
			try
			{
				string downloadString = string.Empty;
				using (WebClient client = new WebClient())
					downloadString = client.DownloadString(UrlUtils.GetSpawnsets);
				SpawnsetFiles = JsonConvert.DeserializeObject<List<SpawnsetFile>>(downloadString);
			}
			catch (WebException ex)
			{
				Program.App.ShowError("Error retrieving spawnset list", $"Could not connect to '{UrlUtils.GetSpawnsets}'.", ex);
				Logger.Log.Error($"Could not connect to '{UrlUtils.GetSpawnsets}'.", ex);
			}
			catch (Exception ex)
			{
				Program.App.ShowError("An unexpected error occurred", "An unexpected error occurred.", ex);
				Logger.Log.Error("An unexpected error occurred.", ex);
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
						Program.App.ShowError("Error parsing file", "Could not parse file.", null);

				return spawnset;
			}
			catch (WebException ex)
			{
				Program.App.ShowError("Error downloading file", $"Could not connect to '{url}'.", ex);
				Logger.Log.Error($"Could not connect to '{url}'.", ex);

				return new Spawnset
				{
					ArenaTiles = ArenaPresetHandler.Instance.DefaultPreset.GetTiles()
				};
			}
			catch (Exception ex)
			{
				Program.App.ShowError("An unexpected error occurred", "An unexpected error occurred.", ex);
				Logger.Log.Error("An unexpected error occurred.", ex);

				return new Spawnset
				{
					ArenaTiles = ArenaPresetHandler.Instance.DefaultPreset.GetTiles()
				};
			}
		}
	}
}