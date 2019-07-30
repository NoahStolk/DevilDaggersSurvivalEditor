using DevilDaggersCore.Spawnsets.Web;
using System;
using System.Collections.Generic;

namespace DevilDaggersSurvivalEditor.Code.Spawnsets.SpawnsetList
{
	public sealed class SpawnsetListHandler
	{
		public const string AllAuthors = "[All]";

		public string AuthorSearch { get; set; } = string.Empty;
		public string SpawnsetSearch { get; set; } = string.Empty;

		public SpawnsetListSorting<Author> ActiveAuthorSorting { get; set; }
		public IReadOnlyList<SpawnsetListSorting<Author>> AuthorSortings { get; set; } = new List<SpawnsetListSorting<Author>>
		{
			new SpawnsetListSorting<Author>("Name", s => s.Name, true) { Ascending = true },
			new SpawnsetListSorting<Author>("Spawnsets", s => s.SpawnsetCount, false)
		};

		public SpawnsetListSorting<SpawnsetFile> ActiveSpawnsetSorting { get; set; }
		public IReadOnlyList<SpawnsetListSorting<SpawnsetFile>> SpawnsetSortings { get; set; } = new List<SpawnsetListSorting<SpawnsetFile>>
		{
			new SpawnsetListSorting<SpawnsetFile>("Name", s => s.Name, true),
			new SpawnsetListSorting<SpawnsetFile>("Author", s => s.Author, true),
			new SpawnsetListSorting<SpawnsetFile>("Last updated", s => s.settings.LastUpdated, false),
			new SpawnsetListSorting<SpawnsetFile>("Length", s => s.spawnsetData.NonLoopLength, false),
			new SpawnsetListSorting<SpawnsetFile>("Spawns", s => s.spawnsetData.NonLoopSpawns, false),
			new SpawnsetListSorting<SpawnsetFile>("Start", s => s.spawnsetData.LoopStart, false),
			new SpawnsetListSorting<SpawnsetFile>("Length", s => s.spawnsetData.LoopLength, false),
			new SpawnsetListSorting<SpawnsetFile>("Spawns", s => s.spawnsetData.LoopSpawns, false)
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