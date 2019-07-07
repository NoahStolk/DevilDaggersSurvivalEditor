using System;
using System.Collections.Generic;

namespace DevilDaggersSurvivalEditor.Code.Spawnsets.SpawnsetList
{
	public sealed class SpawnsetListHandler
	{
		public string AuthorSearch { get; set; } = string.Empty;
		public string SpawnsetSearch { get; set; } = string.Empty;

		public SpawnsetListSorting ActiveSorting { get; set; }
		public IReadOnlyList<SpawnsetListSorting> Sortings { get; set; } = new List<SpawnsetListSorting>
		{
			new SpawnsetListSorting("Name", s => s.Name, true),
			new SpawnsetListSorting("Author", s => s.Author, true),
			new SpawnsetListSorting("Last updated", s => s.settings.LastUpdated, false),
			new SpawnsetListSorting("Length", s => s.spawnsetData.NonLoopLength, false),
			new SpawnsetListSorting("Spawns", s => s.spawnsetData.NonLoopSpawns, false),
			new SpawnsetListSorting("Start", s => s.spawnsetData.LoopStart, false),
			new SpawnsetListSorting("Length", s => s.spawnsetData.LoopLength, false),
			new SpawnsetListSorting("Spawns", s => s.spawnsetData.LoopSpawns, false)
		};

		private static readonly Lazy<SpawnsetListHandler> lazy = new Lazy<SpawnsetListHandler>(() => new SpawnsetListHandler());
		public static SpawnsetListHandler Instance => lazy.Value;

		private SpawnsetListHandler()
		{
			ActiveSorting = Sortings[2];
		}
	}
}