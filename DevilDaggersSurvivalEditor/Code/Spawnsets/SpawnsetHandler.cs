using DevilDaggersCore.Spawnsets;
using DevilDaggersSurvivalEditor.Code.Arena;
using DevilDaggersSurvivalEditor.Gui.Windows;
using Microsoft.Win32;
using System;
using System.IO;

namespace DevilDaggersSurvivalEditor.Code.Spawnsets
{
	internal sealed class SpawnsetHandler
	{
		private bool unsavedChanges = false;
		internal bool HasUnsavedChanges
		{
			get => unsavedChanges;
			set
			{
				unsavedChanges = value;
				App.Instance.UpdateMainWindowTitle();
			}
		}

		internal string SpawnsetName { get; private set; } = "(new spawnset)";
		internal string SpawnsetFileLocation { get; private set; } = string.Empty;

		// Must be a field since properties can't be used as out parameters.
		internal Spawnset spawnset;

		private static readonly Lazy<SpawnsetHandler> lazy = new Lazy<SpawnsetHandler>(() => new SpawnsetHandler());
		internal static SpawnsetHandler Instance => lazy.Value;

		private SpawnsetHandler()
		{
			spawnset = new Spawnset { ArenaTiles = ArenaPresetHandler.Instance.DefaultPreset.GetTiles() };
		}

		internal void UpdateSpawnsetState(string name, string fileLocation)
		{
			HasUnsavedChanges = false;

			SpawnsetName = name;
			SpawnsetFileLocation = fileLocation;

			App.Instance.UpdateMainWindowTitle();
		}

		internal void ProceedWithUnsavedChanges()
		{
			if (!HasUnsavedChanges)
				return;

			ConfirmWindow confirmWindow = new ConfirmWindow("Save changes?", "The current spawnset has unsaved changes. Save before proceeding?");
			confirmWindow.ShowDialog();

			if (confirmWindow.Confirmed)
				FileSave();
		}

		internal void FileSave()
		{
			if (File.Exists(SpawnsetFileLocation))
			{
				if (SpawnsetFileUtils.TryWriteSpawnsetToFile(spawnset, SpawnsetFileLocation))
					HasUnsavedChanges = false;
			}
			else
			{
				FileSaveAs();
			}
		}

		internal void FileSaveAs()
		{
			SaveFileDialog dialog = new SaveFileDialog();
			bool? result = dialog.ShowDialog();
			if (result.HasValue && result.Value && SpawnsetFileUtils.TryWriteSpawnsetToFile(spawnset, dialog.FileName))
				UpdateSpawnsetState(Path.GetFileName(dialog.FileName), dialog.FileName);
		}
	}
}