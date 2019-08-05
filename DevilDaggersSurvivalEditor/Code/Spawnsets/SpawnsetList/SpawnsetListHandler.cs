using System;
using System.Collections.Generic;

namespace DevilDaggersSurvivalEditor.Code.Spawnsets.SpawnsetList
{
	public sealed class SpawnsetListHandler
	{
		public const string AllAuthors = "[All]";

		public string AuthorSearch { get; set; } = string.Empty;
		public string SpawnsetSearch { get; set; } = string.Empty;

		public SpawnsetListSorting<AuthorListEntry> ActiveAuthorSorting { get; set; }
		public IReadOnlyList<SpawnsetListSorting<AuthorListEntry>> AuthorSortings { get; set; } = new List<SpawnsetListSorting<AuthorListEntry>>
		{
			new SpawnsetListSorting<AuthorListEntry>("Name", "Name", s => s.Name, true) { Ascending = true },
			new SpawnsetListSorting<AuthorListEntry>("Spawnset amount", "Spawnsets", s => s.SpawnsetCount, false)
		};

		public SpawnsetListSorting<SpawnsetListEntry> ActiveSpawnsetSorting { get; set; }
		public IReadOnlyList<SpawnsetListSorting<SpawnsetListEntry>> SpawnsetSortings { get; set; } = new List<SpawnsetListSorting<SpawnsetListEntry>>
		{
			new SpawnsetListSorting<SpawnsetListEntry>("Name", "Name", s => s.SpawnsetFile.Name, true),
			new SpawnsetListSorting<SpawnsetListEntry>("Author", "Author", s => s.SpawnsetFile.Author, true),
			new SpawnsetListSorting<SpawnsetListEntry>("Last updated", "Last updated", s => s.SpawnsetFile.settings.LastUpdated, false),
			new SpawnsetListSorting<SpawnsetListEntry>("Custom leaderboard", "LB", s => s.HasLeaderboard, false) { Ascending = true },
			new SpawnsetListSorting<SpawnsetListEntry>("Non-loop length", "Length", s => s.SpawnsetFile.spawnsetData.NonLoopLength, false),
			new SpawnsetListSorting<SpawnsetListEntry>("Non-loop spawns", "Spawns", s => s.SpawnsetFile.spawnsetData.NonLoopSpawns, false),
			new SpawnsetListSorting<SpawnsetListEntry>("Loop start", "Start", s => s.SpawnsetFile.spawnsetData.LoopStart, false),
			new SpawnsetListSorting<SpawnsetListEntry>("Loop length", "Length", s => s.SpawnsetFile.spawnsetData.LoopLength, false),
			new SpawnsetListSorting<SpawnsetListEntry>("Loop spawns", "Spawns", s => s.SpawnsetFile.spawnsetData.LoopSpawns, false)
		};

		private static readonly Lazy<SpawnsetListHandler> lazy = new Lazy<SpawnsetListHandler>(() => new SpawnsetListHandler());
		public static SpawnsetListHandler Instance => lazy.Value;

		private SpawnsetListHandler()
		{
			ActiveAuthorSorting = AuthorSortings[0];
			ActiveSpawnsetSorting = SpawnsetSortings[2];
		}
	}
}