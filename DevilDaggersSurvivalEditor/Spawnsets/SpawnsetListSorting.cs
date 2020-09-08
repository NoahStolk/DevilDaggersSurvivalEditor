using System;

namespace DevilDaggersSurvivalEditor.Spawnsets
{
	public class SpawnsetListSorting<TListEntry>
		where TListEntry : IListEntry
	{
		public SpawnsetListSorting(string fullName, string displayName, Func<TListEntry, object> sortingFunction, bool isAscendingDefault)
		{
			FullName = fullName;
			DisplayName = displayName;
			SortingFunction = sortingFunction;
			IsAscendingDefault = isAscendingDefault;
		}

		public string FullName { get; }
		public string DisplayName { get; }
		public Func<TListEntry, object> SortingFunction { get; }
		public bool IsAscendingDefault { get; }

		public bool Ascending { get; set; }
	}
}