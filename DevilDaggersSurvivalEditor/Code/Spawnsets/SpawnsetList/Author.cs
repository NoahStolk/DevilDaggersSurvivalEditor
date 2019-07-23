namespace DevilDaggersSurvivalEditor.Code.Spawnsets.SpawnsetList
{
	public class Author
	{
		public string Name { get; set; }
		public int SpawnsetCount { get; set; }

		public Author(string name, int spawnsetCount)
		{
			Name = name;
			SpawnsetCount = spawnsetCount;
		}
	}
}