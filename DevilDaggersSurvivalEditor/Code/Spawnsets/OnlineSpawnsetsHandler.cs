using System;

namespace DevilDaggersSurvivalEditor.Code.Spawnsets
{
	public sealed class OnlineSpawnsetsHandler
	{
		public string AuthorSearch { get; set; } = string.Empty;
		public string SpawnsetSearch { get; set; } = string.Empty;

		private static readonly Lazy<OnlineSpawnsetsHandler> lazy = new Lazy<OnlineSpawnsetsHandler>(() => new OnlineSpawnsetsHandler());
		public static OnlineSpawnsetsHandler Instance => lazy.Value;

		private OnlineSpawnsetsHandler()
		{
		}
	}
}