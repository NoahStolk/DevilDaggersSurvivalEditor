using DevilDaggersSurvivalEditor.Clients;
using System;
using System.Collections.Generic;

namespace DevilDaggersSurvivalEditor.Spawnsets
{
	public sealed class SpawnsetListHandler
	{
		public const string AllAuthors = "[All]";

		private static readonly Lazy<SpawnsetListHandler> _lazy = new Lazy<SpawnsetListHandler>(() => new SpawnsetListHandler());

		private SpawnsetListHandler()
		{
			ActiveAuthorSorting = AuthorSortings[0];
			ActiveSpawnsetSorting = SpawnsetSortings[2];
		}

		public static SpawnsetListHandler Instance => _lazy.Value;

		public string AuthorSearch { get; set; } = string.Empty;
		public string SpawnsetSearch { get; set; } = string.Empty;

		public SpawnsetListSorting<AuthorListEntry> ActiveAuthorSorting { get; set; }
		public IReadOnlyList<SpawnsetListSorting<AuthorListEntry>> AuthorSortings { get; } = new List<SpawnsetListSorting<AuthorListEntry>>
		{
			new SpawnsetListSorting<AuthorListEntry>("Name", "Name", s => s.Name, true) { Ascending = true },
			new SpawnsetListSorting<AuthorListEntry>("Spawnset amount", "Spawnsets", s => s.SpawnsetCount, false),
		};

		public SpawnsetListSorting<SpawnsetFile> ActiveSpawnsetSorting { get; set; }
		public IReadOnlyList<SpawnsetListSorting<SpawnsetFile>> SpawnsetSortings { get; } = new List<SpawnsetListSorting<SpawnsetFile>>
		{
			new SpawnsetListSorting<SpawnsetFile>("Name", "Name", s => s.Name, true),
			new SpawnsetListSorting<SpawnsetFile>("Author", "Author", s => s.AuthorName, true),
			new SpawnsetListSorting<SpawnsetFile>("Last updated", "Last updated", s => s.LastUpdated, false),
			new SpawnsetListSorting<SpawnsetFile>("Custom leaderboard", "LB", s => s.HasCustomLeaderboard, false) { Ascending = true },
			new SpawnsetListSorting<SpawnsetFile>("Non-loop length", "Length", s => s.SpawnsetData.NonLoopLength ?? 0, false),
			new SpawnsetListSorting<SpawnsetFile>("Non-loop spawns", "Spawns", s => s.SpawnsetData.NonLoopSpawnCount, false),
			new SpawnsetListSorting<SpawnsetFile>("Loop length", "Length", s => s.SpawnsetData.LoopLength ?? 0, false),
			new SpawnsetListSorting<SpawnsetFile>("Loop spawns", "Spawns", s => s.SpawnsetData.LoopSpawnCount, false),
		};
	}
}