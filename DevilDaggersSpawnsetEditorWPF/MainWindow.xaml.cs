using DevilDaggersSpawnsetEditorWPF.Helpers;
using DevilDaggersSpawnsetEditorWPF.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DevilDaggersSpawnsetEditorWPF
{
	public partial class MainWindow : Window
	{
		private Spawnset spawnset;

		public MainWindow()
		{
			InitializeComponent();

			CreateEmptySpawnset();

			ArenaTiles.RowDefinitions.Clear();
			ArenaTiles.ColumnDefinitions.Clear();
			for (int i = 0; i < spawnset.arenaTiles.GetLength(0); i++)
				ArenaTiles.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
			for (int i = 0; i < spawnset.arenaTiles.GetLength(1); i++)
				ArenaTiles.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
		}

		private void CreateEmptySpawnset()
		{
			spawnset = new Spawnset();

			UpdateSpawnsGUI();

			UpdateSettingsGUI();

			UpdateArenaGUI();
		}

		/// <summary>
		/// Updates the internal end loop.
		/// Only call this when the spawns in the spawnset have been modified.
		/// </summary>
		private void UpdateEndLoop()
		{
			for (int i = 0; i < spawnset.spawns.Count; i++)
			{
				spawnset.spawns[i].loop = true;
				if (spawnset.spawns[i].enemy == GameHelper.enemies[-1])
				{
					for (int j = 0; j < i; j++)
					{
						spawnset.spawns[j].loop = false;
					}
				}
			}
		}

		private void UpdateSpawnsGUI()
		{
			// TODO: Optimise (don't regenerate all every time)

			StackPanelSpawnsIndex.Children.Clear();
			StackPanelSpawnsEnemy.Children.Clear();
			StackPanelSpawnsDelay.Children.Clear();
			StackPanelSpawnsSeconds.Children.Clear();
			StackPanelSpawnsGems.Children.Clear();
			StackPanelSpawnsTotalGems.Children.Clear();

			double seconds = 0;
			int totalGems = 0;
			foreach (KeyValuePair<int, Spawn> kvp in spawnset.spawns)
			{
				seconds += kvp.Value.delay;
				totalGems += kvp.Value.enemy.gems;

				StackPanelSpawnsIndex.Children.Add(new Label { Padding = new Thickness(0), Content = kvp.Key });
				StackPanelSpawnsSeconds.Children.Add(new Label { Padding = new Thickness(0), Content = seconds.ToString("0.00") });
				StackPanelSpawnsEnemy.Children.Add(new Label { Padding = new Thickness(0), Content = kvp.Value.enemy.name, FontWeight = (kvp.Value.loop ? FontWeights.Bold : FontWeights.Normal) });
				StackPanelSpawnsDelay.Children.Add(new Label { Padding = new Thickness(0), Content = kvp.Value.delay.ToString("0.00") });
				StackPanelSpawnsGems.Children.Add(new Label { Padding = new Thickness(0), Content = kvp.Value.enemy.gems });
				StackPanelSpawnsTotalGems.Children.Add(new Label { Padding = new Thickness(0), Content = totalGems });
			}
		}

		private void UpdateSettingsGUI()
		{
			TextBlockShrinkStart.Text = spawnset.shrinkStart.ToString();
			TextBlockShrinkEnd.Text = spawnset.shrinkEnd.ToString();
			TextBlockShrinkRate.Text = spawnset.shrinkRate.ToString();
			TextBlockBrightness.Text = spawnset.brightness.ToString();
		}

		private Color GetColorFromHeight(float height)
		{
			if (height < -4)
				return Color.FromRgb(0, 0, 0);

			float colorVal = Math.Max(0, (float)Math.Round((height + 4) * 16 + 32));

			return Color.FromRgb((byte)(colorVal), (byte)(colorVal / 2), (byte)(Math.Floor(height / 16) * 64));
		}

		private void UpdateArenaGUI()
		{
			ArenaTiles.Children.Clear();

			for (int i = 0; i < spawnset.arenaTiles.GetLength(0); i++)
			{
				for (int j = 0; j < spawnset.arenaTiles.GetLength(1); j++)
				{
					float height = spawnset.arenaTiles[i, j];

					TextBlock textBlock = new TextBlock { Background = new SolidColorBrush(GetColorFromHeight(height)) };

					Grid.SetRow(textBlock, i);
					Grid.SetColumn(textBlock, j);
					ArenaTiles.Children.Add(textBlock);
				}
			}
		}

		private void AddSpawnButton_Click(object sender, RoutedEventArgs e)
		{
			if (!double.TryParse(TextBoxDelay.Text, out double delay))
			{
				MessageBox.Show("Please enter a numeric value.", "Invalid delay value");
				return;
			}

			spawnset.spawns.Add(spawnset.spawns.Count, new Spawn(GameHelper.enemies[ComboBoxEnemy.SelectedIndex - 1], delay, true));

			UpdateEndLoop();

			UpdateSpawnsGUI();
		}

		private void FileNew_Click(object sender, RoutedEventArgs e)
		{
			// TODO: Prompt

			CreateEmptySpawnset();
		}

		private void FileOpen_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();
			bool? result = dialog.ShowDialog();

			if (result.HasValue && result.Value)
			{
				if (!SpawnsetParser.TryParseFile(dialog.FileName, out spawnset))
				{
					MessageBox.Show("Please open a valid Devil Daggers V3 spawnset file.", "Could not parse file");
				}
			}

			UpdateSpawnsGUI();

			UpdateSettingsGUI();

			UpdateArenaGUI();
		}

		private void SettingsEditApplyButton_Click(object sender, RoutedEventArgs e)
		{
			Button b = (Button)sender;
			if ((string)b.Content == "Apply")
			{
				if (!float.TryParse(TextBoxShrinkStart.Text, out float shrinkStart))
				{
					MessageBox.Show("Please enter a numeric value.", "Invalid shrink start value");
					return;
				}
				if (!float.TryParse(TextBoxShrinkEnd.Text, out float shrinkEnd))
				{
					MessageBox.Show("Please enter a numeric value.", "Invalid shrink end value");
					return;
				}
				if (!float.TryParse(TextBoxShrinkRate.Text, out float shrinkRate))
				{
					MessageBox.Show("Please enter a numeric value.", "Invalid shrink rate value");
					return;
				}
				if (!float.TryParse(TextBoxBrightness.Text, out float brightness))
				{
					MessageBox.Show("Please enter a numeric value.", "Invalid brightness value");
					return;
				}

				spawnset.shrinkStart = shrinkStart;
				spawnset.shrinkEnd = shrinkEnd;
				spawnset.shrinkRate = shrinkRate;
				spawnset.brightness = brightness;

				UpdateSettingsGUI();

				TextBlockShrinkStart.Visibility = Visibility.Visible;
				TextBlockShrinkEnd.Visibility = Visibility.Visible;
				TextBlockShrinkRate.Visibility = Visibility.Visible;
				TextBlockBrightness.Visibility = Visibility.Visible;

				TextBoxShrinkStart.Visibility = Visibility.Collapsed;
				TextBoxShrinkEnd.Visibility = Visibility.Collapsed;
				TextBoxShrinkRate.Visibility = Visibility.Collapsed;
				TextBoxBrightness.Visibility = Visibility.Collapsed;

				b.Content = "Edit";
			}
			else if ((string)b.Content == "Edit")
			{
				TextBoxShrinkStart.Visibility = Visibility.Visible;
				TextBoxShrinkEnd.Visibility = Visibility.Visible;
				TextBoxShrinkRate.Visibility = Visibility.Visible;
				TextBoxBrightness.Visibility = Visibility.Visible;

				TextBlockShrinkStart.Visibility = Visibility.Collapsed;
				TextBlockShrinkEnd.Visibility = Visibility.Collapsed;
				TextBlockShrinkRate.Visibility = Visibility.Collapsed;
				TextBlockBrightness.Visibility = Visibility.Collapsed;

				b.Content = "Apply";
			}
		}

		private void ArenaTiles_MouseMove(object sender, MouseEventArgs e)
		{
			Point tile = Mouse.GetPosition((IInputElement)sender);
			tile = new Point((int)tile.X / 8, (int)tile.Y / 8);

			float height = spawnset.arenaTiles[(int)tile.Y, (int)tile.X];

			LabelTile.Content = tile.ToString();
			HeightTile.Content = height.ToString();
		}

		private void ArenaTiles_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			Point tile = Mouse.GetPosition((IInputElement)sender);
			tile = new Point((int)tile.X / 8, (int)tile.Y / 8);

			spawnset.arenaTiles[(int)tile.Y, (int)tile.X] = Math.Max(Math.Min(spawnset.arenaTiles[(int)tile.Y, (int)tile.X] + e.Delta / 120, 63), -4);

			ArenaTiles.Children.Remove(ArenaTiles.Children
			  .Cast<UIElement>()
			  .First(ee => Grid.GetRow(ee) == tile.Y && Grid.GetColumn(ee) == tile.X));

			float height = spawnset.arenaTiles[(int)tile.Y, (int)tile.X];

			TextBlock textBlock = new TextBlock { Background = new SolidColorBrush(GetColorFromHeight(height)) };

			Grid.SetRow(textBlock, (int)tile.Y);
			Grid.SetColumn(textBlock, (int)tile.X);
			ArenaTiles.Children.Add(textBlock);
		}

		private void ArenaTiles_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			Point tile = Mouse.GetPosition((IInputElement)sender);
			tile = new Point((int)tile.X / 8, (int)tile.Y / 8);

			if (spawnset.arenaTiles[(int)tile.Y, (int)tile.X] >= 0)
				spawnset.arenaTiles[(int)tile.Y, (int)tile.X] = -1000;
			else
				spawnset.arenaTiles[(int)tile.Y, (int)tile.X] = 0;

			ArenaTiles.Children.Remove(ArenaTiles.Children
			  .Cast<UIElement>()
			  .First(ee => Grid.GetRow(ee) == tile.Y && Grid.GetColumn(ee) == tile.X));

			float height = spawnset.arenaTiles[(int)tile.Y, (int)tile.X];

			TextBlock textBlock = new TextBlock { Background = new SolidColorBrush(GetColorFromHeight(height)) };

			Grid.SetRow(textBlock, (int)tile.Y);
			Grid.SetColumn(textBlock, (int)tile.X);
			ArenaTiles.Children.Add(textBlock);
		}
	}
}