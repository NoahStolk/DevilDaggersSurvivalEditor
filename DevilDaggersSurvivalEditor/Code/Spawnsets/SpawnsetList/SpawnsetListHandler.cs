using System;
using System.Collections.Generic;

namespace DevilDaggersSurvivalEditor.Code.Spawnsets.SpawnsetList
{
	public sealed class SpawnsetListHandler
	{
		public const string AllAuthors = "[All]";

		private static readonly Lazy<SpawnsetListHandler> lazy = new Lazy<SpawnsetListHandler>(() => new SpawnsetListHandler());

		private SpawnsetListHandler()
		{
			ActiveAuthorSorting = AuthorSortings[0];
			ActiveSpawnsetSorting = SpawnsetSortings[2];
		}

		public static SpawnsetListHandler Instance => lazy.Value;

		public string AuthorSearch { get; set; } = string.Empty;
		public string SpawnsetSearch { get; set; } = string.Empty;

		public SpawnsetListSorting<AuthorListEntry> ActiveAuthorSorting { get; set; }
		public IReadOnlyList<SpawnsetListSorting<AuthorListEntry>> AuthorSortings { get; } = new List<SpawnsetListSorting<AuthorListEntry>>
		{
			new SpawnsetListSorting<AuthorListEntry>("Name", "Name", s => s.Name, true) { Ascending = true },
			new SpawnsetListSorting<AuthorListEntry>("Spawnset amount", "Spawnsets", s => s.SpawnsetCount, false),
		};

		public SpawnsetListSorting<SpawnsetListEntry> ActiveSpawnsetSorting { get; set; }
		public IReadOnlyList<SpawnsetListSorting<SpawnsetListEntry>> SpawnsetSortings { get; } = new List<SpawnsetListSorting<SpawnsetListEntry>>
		{
			new SpawnsetListSorting<SpawnsetListEntry>("Name", "Name", s => s.SpawnsetFile.Name, true),
			new SpawnsetListSorting<SpawnsetListEntry>("Author", "Author", s => s.SpawnsetFile.Author, true),
			new SpawnsetListSorting<SpawnsetListEntry>("Last updated", "Last updated", s => s.SpawnsetFile.settings.LastUpdated, false),
			new SpawnsetListSorting<SpawnsetListEntry>("Custom leaderboard", "LB", s => s.HasLeaderboard, false) { Ascending = true },
			new SpawnsetListSorting<SpawnsetListEntry>("Non-loop length", "Length", s => s.SpawnsetFile.spawnsetData.NonLoopLength, false),
			new SpawnsetListSorting<SpawnsetListEntry>("Non-loop spawns", "Spawns", s => s.SpawnsetFile.spawnsetData.NonLoopSpawnCount, false),
			new SpawnsetListSorting<SpawnsetListEntry>("Loop length", "Length", s => s.SpawnsetFile.spawnsetData.LoopLength, false),
			new SpawnsetListSorting<SpawnsetListEntry>("Loop spawns", "Spawns", s => s.SpawnsetFile.spawnsetData.LoopSpawnCount, false),
		};
	}
}