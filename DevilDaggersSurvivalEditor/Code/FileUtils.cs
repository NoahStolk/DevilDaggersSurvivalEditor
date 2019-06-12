using System;
using System.IO;
using System.Windows;

namespace DevilDaggersSurvivalEditor.Code
{
	public static class FileUtils
	{
		public static void WriteSpawnsetToFile(string path)
		{
			if (Program.App.spawnset.TryGetBytes(out byte[] bytes))
			{
				File.WriteAllBytes(path, bytes);
				MessageBox.Show($"Successfully wrote file to {path}.", "Success");
			}
			else
			{
				Program.App.ShowError("An unexpected error occurred", $"Error while writing file to {path}.", null);
			}
		}

		public static void ReplaceFile(string sourceFileName, string destinationFileName)
		{
			try
			{
				File.Replace(sourceFileName, destinationFileName, null);
				MessageBox.Show("Successfully replaced file.", "Success");
			}
			catch (Exception ex)
			{
				Program.App.ShowError("An unexpected error occurred", "An unexpected error occurred while trying to replace the file.", ex);
			}
		}
	}
}