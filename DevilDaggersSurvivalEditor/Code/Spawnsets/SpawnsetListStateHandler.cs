using DevilDaggersCore.Spawnset.Web;
using System;
using System.Collections.Generic;

namespace DevilDaggersSurvivalEditor.Code.Spawnsets
{
	public sealed class SpawnsetListStateHandler
	{
		public string AuthorSearch { get; set; } = string.Empty;
		public string SpawnsetSearch { get; set; } = string.Empty;

		public SpawnsetSorting ActiveSorting { get; set; }
		public IReadOnlyList<SpawnsetSorting> Sortings { get; set; } = new List<SpawnsetSorting>
		{
			new SpawnsetSorting("Name", s => s.Name, true),
			new SpawnsetSorting("Author", s => s.Author, true),
			new SpawnsetSorting("Last updated", s => s.settings.LastUpdated, false),
			new SpawnsetSorting("Length", s => s.spawnsetData.NonLoopLength, false),
			new SpawnsetSorting("Spawns", s => s.spawnsetData.NonLoopSpawns, false),
			new SpawnsetSorting("Start", s => s.spawnsetData.LoopStart, false),
			new SpawnsetSorting("Length", s => s.spawnsetData.LoopLength, false),
			new SpawnsetSorting("Spawns", s => s.spawnsetData.LoopSpawns, false)
		};

		private static readonly Lazy<SpawnsetListStateHandler> lazy = new Lazy<SpawnsetListStateHandler>(() => new SpawnsetListStateHandler());
		public static SpawnsetListStateHandler Instance => lazy.Value;

		private SpawnsetListStateHandler()
		{
			ActiveSorting = Sortings[0];
		}
	}
}