using DevilDaggersSurvivalEditor.Clients;
using System;
using System.Collections.Generic;

namespace DevilDaggersSurvivalEditor.Spawnsets
{
	public sealed class SpawnsetListHandler
	{
		public const string AllAuthors = "[All]";

		private static readonly Lazy<SpawnsetListHandler> _lazy = new(() => new());

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
			new("Name", "Name", s => s.Name, true) { Ascending = true },
			new("Spawnset amount", "Spawnsets", s => s.SpawnsetCount, false),
		};

		public SpawnsetListSorting<SpawnsetFile> ActiveSpawnsetSorting { get; set; }
		public IReadOnlyList<SpawnsetListSorting<SpawnsetFile>> SpawnsetSortings { get; } = new List<SpawnsetListSorting<SpawnsetFile>>
		{
			new("Name", "Name", s => s.Name, true),
			new("Author", "Author", s => s.AuthorName, true),
			new("Last updated", "Last updated", s => s.LastUpdated, false),
			new("Custom leaderboard", "LB", s => s.HasCustomLeaderboard, false) { Ascending = true },
			new("Non-loop length", "Length", s => s.SpawnsetData.NonLoopLength ?? 0, false),
			new("Non-loop spawns", "Spawns", s => s.SpawnsetData.NonLoopSpawnCount, false),
			new("Loop length", "Length", s => s.SpawnsetData.LoopLength ?? 0, false),
			new("Loop spawns", "Spawns", s => s.SpawnsetData.LoopSpawnCount, false),
		};
	}
}
