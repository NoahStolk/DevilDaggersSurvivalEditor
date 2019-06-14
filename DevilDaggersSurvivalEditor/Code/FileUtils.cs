using DevilDaggersCore.Spawnset;
using System;
using System.IO;

namespace DevilDaggersSurvivalEditor.Code
{
	public static class FileUtils
	{
		public static void WriteSpawnsetToFile(Spawnset spawnset, string destinationPath)
		{
			if (spawnset.TryGetBytes(out byte[] bytes))
			{
				File.WriteAllBytes(destinationPath, bytes);
				Program.App.ShowMessage("Success", $"Successfully wrote file to {destinationPath}.");
			}
			else
			{
				Program.App.ShowError("An unexpected error occurred", $"Error while writing file to {destinationPath}.");
			}
		}

		public static void CopyFile(string sourcePath, string destinationPath)
		{
			try
			{
				File.Copy(sourcePath, destinationPath);
				Program.App.ShowMessage("Success", "Successfully replaced file.");
			}
			catch (Exception ex)
			{
				Program.App.ShowError("An unexpected error occurred", "An unexpected error occurred while trying to replace the file.", ex);
			}
		}
	}
}