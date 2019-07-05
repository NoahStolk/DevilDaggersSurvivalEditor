using System;

namespace DevilDaggersSurvivalEditor.Code.Spawnsets
{
	public sealed class SpawnsetListStateHandler
	{
		public string AuthorSearch { get; set; } = string.Empty;
		public string SpawnsetSearch { get; set; } = string.Empty;

		private static readonly Lazy<SpawnsetListStateHandler> lazy = new Lazy<SpawnsetListStateHandler>(() => new SpawnsetListStateHandler());
		public static SpawnsetListStateHandler Instance => lazy.Value;

		private SpawnsetListStateHandler()
		{
		}
	}
}