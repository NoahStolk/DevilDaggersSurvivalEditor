using DevilDaggersCore.Spawnsets;
using System;
using System.IO;

namespace DevilDaggersSurvivalEditor.Utils
{
	public static class SpawnsetFileUtils
	{
		public static bool TryWriteSpawnsetToFile(Spawnset spawnset, string destinationPath)
		{
			try
			{
				if (spawnset.TryGetBytes(out byte[] bytes))
				{
					File.WriteAllBytes(destinationPath, bytes);
					return true;
				}
				else
				{
					App.Instance.ShowError("Unexpected error", "Error while trying to convert spawnset to binary.");
					return false;
				}
			}
			catch (Exception ex)
			{
				App.Instance.ShowError("Unexpected error", $"Error while trying to write file to {destinationPath}.", ex);
				return false;
			}
		}
	}
}
