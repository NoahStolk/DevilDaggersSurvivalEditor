using DevilDaggersSpawnsetEditorWPF.Helpers;
using DevilDaggersSpawnsetEditorWPF.Models;
using DevilDaggersSpawnsetEditorWPF.Presets;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DevilDaggersSpawnsetEditorWPF.Windows
{
	public partial class MainWindow : Window
	{
		public static UserSettings userSettings;
		public static Spawnset spawnset;

		public MainWindow()
		{
			InitializeComponent();

			CreateEmptySpawnset();

			// Add height map
			TextBlock textBlock = new TextBlock { Background = new SolidColorBrush(GetColorFromHeight(-1)) };
			Grid.SetRow(textBlock, 0);
			Grid.SetColumn(textBlock, 0);
			HeightMap.Children.Add(textBlock);

			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 16; j++)
				{
					textBlock = new TextBlock { Background = new SolidColorBrush(GetColorFromHeight(i * 16 + j)) };

					Grid.SetRow(textBlock, i + 1);
					Grid.SetColumn(textBlock, j);
					HeightMap.Children.Add(textBlock);
				}
			}

			// Add arena tiles
			for (int i = 0; i < spawnset.arenaTiles.GetLength(0); i++)
			{
				for (int j = 0; j < spawnset.arenaTiles.GetLength(1); j++)
				{
					float height = spawnset.arenaTiles[i, j];

					Rectangle rect = new Rectangle
					{
						Width = 8,
						Height = 8
					};
					Canvas.SetLeft(rect, i * 8);
					Canvas.SetTop(rect, j * 8);
					SetTileColor(rect);

					ArenaTiles.Children.Add(rect);
				}
			}

			InitializeUserSettings();
		}

		private void InitializeUserSettings()
		{
			userSettings = new UserSettings();

			if (File.Exists(Settings.USER_SETTINGS_FILENAME))
			{
				using (StreamReader sr = new StreamReader(File.OpenRead(Settings.USER_SETTINGS_FILENAME)))
				{
					userSettings = JsonConvert.DeserializeObject<UserSettings>(sr.ReadToEnd());
				}
			}
		}

		private void CreateEmptySpawnset()
		{
			spawnset = new Spawnset();

			UpdateSpawnsGUI();

			UpdateSettingsGUI();

			UpdateArenaGUI();
		}

		/// <summary>
		/// Updates the internal end loop (not GUI).
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

			ListBoxSpawns.Items.Clear();

			double seconds = 0;
			int totalGems = 0;
			foreach (KeyValuePair<int, Spawn> kvp in spawnset.spawns)
			{
				seconds += kvp.Value.delay;
				totalGems += kvp.Value.enemy.gems;

				Grid grid = new Grid
				{
					Width = 372,
					HorizontalAlignment = HorizontalAlignment.Left,
					Margin = new Thickness(0)
				};
				for (int i = 0; i < 6; i++)
					grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength((i == 0) ? 1 : 2, GridUnitType.Star) });

				Label l1 = new Label { Padding = new Thickness(4, 0, 0, 0), Content = kvp.Key };
				Label l2 = new Label { Padding = new Thickness(4, 0, 0, 0), Content = seconds.ToString("0.0000") };
				Label l3 = new Label { Padding = new Thickness(4, 0, 0, 0), Content = kvp.Value.enemy.name, FontWeight = (kvp.Value.loop ? FontWeights.Bold : FontWeights.Normal) };
				Label l4 = new Label { Padding = new Thickness(4, 0, 0, 0), Content = kvp.Value.delay.ToString("0.0000") };
				Label l5 = new Label { Padding = new Thickness(4, 0, 0, 0), Content = kvp.Value.enemy.gems };
				Label l6 = new Label { Padding = new Thickness(4, 0, 0, 0), Content = totalGems };

				Grid.SetColumn(l1, 0);
				Grid.SetColumn(l2, 1);
				Grid.SetColumn(l3, 2);
				Grid.SetColumn(l4, 3);
				Grid.SetColumn(l5, 4);
				Grid.SetColumn(l6, 5);

				grid.Children.Add(l1);
				grid.Children.Add(l2);
				grid.Children.Add(l3);
				grid.Children.Add(l4);
				grid.Children.Add(l5);
				grid.Children.Add(l6);

				ListBoxSpawns.Items.Add(grid);
			}
		}

		private void UpdateSettingsGUI()
		{
			TextBlockShrinkStart.Text = spawnset.shrinkStart.ToString();
			TextBlockShrinkEnd.Text = spawnset.shrinkEnd.ToString();
			TextBlockShrinkRate.Text = spawnset.shrinkRate.ToString();
			TextBlockBrightness.Text = spawnset.brightness.ToString();

			ShrinkStart.Width = spawnset.shrinkStart * 4;
			ShrinkStart.Height = spawnset.shrinkStart * 4;
			Canvas.SetLeft(ShrinkStart, ArenaTiles.Width / 2 - ShrinkStart.Width / 2);
			Canvas.SetTop(ShrinkStart, ArenaTiles.Height / 2 - ShrinkStart.Height / 2);

			ShrinkEnd.Width = spawnset.shrinkEnd * 4;
			ShrinkEnd.Height = spawnset.shrinkEnd * 4;
			Canvas.SetLeft(ShrinkEnd, ArenaTiles.Width / 2 - ShrinkEnd.Width / 2);
			Canvas.SetTop(ShrinkEnd, ArenaTiles.Height / 2 - ShrinkEnd.Height / 2);

			if (spawnset.shrinkRate > 0)
			{
				ShrinkCurrentSlider.Maximum = (spawnset.shrinkStart - spawnset.shrinkEnd) / spawnset.shrinkRate;
				ShrinkCurrentSlider.IsEnabled = true;
			}
			else
			{
				ShrinkCurrentSlider.Value = 0;
				ShrinkCurrentSlider.Maximum = 1;
				ShrinkCurrentSlider.IsEnabled = false;
			}

			ShrinkCurrent.Width = spawnset.shrinkStart * 4 - (ShrinkCurrentSlider.Value / ShrinkCurrentSlider.Maximum * (spawnset.shrinkStart - spawnset.shrinkEnd) * 4);
			ShrinkCurrent.Height = spawnset.shrinkStart * 4 - (ShrinkCurrentSlider.Value / ShrinkCurrentSlider.Maximum * (spawnset.shrinkStart - spawnset.shrinkEnd) * 4);
			Canvas.SetLeft(ShrinkCurrent, ArenaTiles.Width / 2 - ShrinkCurrent.Width / 2);
			Canvas.SetTop(ShrinkCurrent, ArenaTiles.Height / 2 - ShrinkCurrent.Height / 2);

			UpdateArenaGUI();
		}

		private void UpdateArenaGUI()
		{
			foreach (UIElement elem in ArenaTiles.Children)
			{
				if (elem is Rectangle rect)
				{
					SetTileColor(rect);
				}
			}
		}

		private void SetTileColor(Rectangle rect)
		{
			Point arenaCenter = new Point(204, 204);

			int i = (int)Canvas.GetLeft(rect) / 8;
			int j = (int)Canvas.GetTop(rect) / 8;
			float height = spawnset.arenaTiles[i, j];

			int x, y;
			if (i > 25)
				x = i * 8 + 8;
			else
				x = i * 8;

			if (j > 25)
				y = j * 8 + 8;
			else
				y = j * 8;

			double distance = Math.Sqrt(Math.Pow(x - arenaCenter.X, 2) + Math.Pow(y - arenaCenter.Y, 2)) / 8;

			SolidColorBrush color = new SolidColorBrush(GetColorFromHeight(height));
			rect.Fill = color;

			if (Math.Abs(distance) <= (ShrinkCurrent.Width) / 16)
			{
				rect.Width = 8;
				rect.Height = 8;
				if (Canvas.GetLeft(rect) % 8 != 0 || Canvas.GetTop(rect) % 8 != 0)
				{
					Canvas.SetLeft(rect, i * 8);
					Canvas.SetTop(rect, j * 8);
				}
			}
			else
			{
				rect.Width = 4;
				rect.Height = 4;
				Canvas.SetLeft(rect, i * 8 + 2);
				Canvas.SetTop(rect, j * 8 + 2);
			}
		}

		private Color GetColorFromHeight(float height)
		{
			if (height < Settings.TILE_MIN)
				return Color.FromRgb(0, 0, 0);

			float colorVal = Math.Max(0, (float)Math.Round((height - Settings.TILE_MIN) * 12 + 32));

			return Color.FromRgb((byte)(colorVal), (byte)(colorVal / 2), (byte)(Math.Floor((height - Settings.TILE_MIN) / 16) * 64));
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

		private void InsertSpawnButton_Click(object sender, RoutedEventArgs e)
		{
			if (!double.TryParse(TextBoxDelay.Text, out double delay))
			{
				MessageBox.Show("Please enter a numeric value.", "Invalid delay value");
				return;
			}

			int index = ListBoxSpawns.SelectedIndex;
			if (index == -1)
				return; // nothing selected

			List<Spawn> shift = new List<Spawn>();
			int originalCount = spawnset.spawns.Count;
			for (int i = index; i < originalCount; i++)
			{
				shift.Add(spawnset.spawns[i]);
				spawnset.spawns.Remove(i);
			}

			spawnset.spawns.Add(index, new Spawn(GameHelper.enemies[ComboBoxEnemy.SelectedIndex - 1], delay, true));

			int max = spawnset.spawns.Count;
			for (int i = 0; i < shift.Count; i++)
				spawnset.spawns.Add(max + i, shift[i]);

			UpdateEndLoop();

			UpdateSpawnsGUI();
		}

		private void EditSpawnButton_Click(object sender, RoutedEventArgs e)
		{
			if (!double.TryParse(TextBoxDelay.Text, out double delay))
			{
				MessageBox.Show("Please enter a numeric value.", "Invalid delay value");
				return;
			}

			List<int> indices = (from object obj in ListBoxSpawns.SelectedItems
								 select ListBoxSpawns.Items.IndexOf(obj)).ToList();

			foreach (int i in indices)
			{
				spawnset.spawns[i].enemy = GameHelper.enemies[ComboBoxEnemy.SelectedIndex - 1];
				spawnset.spawns[i].delay = delay;
			}

			UpdateEndLoop();

			UpdateSpawnsGUI();
		}

		private void DeleteSpawnButton_Click(object sender, RoutedEventArgs e)
		{
			List<int> indices = (from object obj in ListBoxSpawns.SelectedItems
								 select ListBoxSpawns.Items.IndexOf(obj)).ToList();

			foreach (int i in indices)
			{
				spawnset.spawns.Remove(i);
			}

			// Reset the keys (we don't want gaps in the sorted dictionary)
			SortedDictionary<int, Spawn> newSpawns = new SortedDictionary<int, Spawn>();
			int j = 0;
			foreach (KeyValuePair<int, Spawn> kvp in spawnset.spawns)
			{
				newSpawns.Add(j, kvp.Value);
				j++;
			}

			spawnset.spawns = newSpawns;

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
				if (!Spawnset.TryParse(dialog.FileName, out spawnset))
				{
					MessageBox.Show("Please open a valid Devil Daggers V3 spawnset file.", "Could not parse file");
				}
			}

			UpdateSpawnsGUI();

			UpdateSettingsGUI();

			UpdateArenaGUI();
		}

		private void FileSave_Click(object sender, RoutedEventArgs e)
		{
			SaveFileDialog dialog = new SaveFileDialog();
			bool? result = dialog.ShowDialog();

			if (result.HasValue && result.Value)
			{
				File.WriteAllBytes(dialog.FileName, spawnset.GetBytes());
			}
		}

		private void ReplaceSurvival_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				File.WriteAllBytes(System.IO.Path.Combine(userSettings.ddLocation, "survival"), spawnset.GetBytes());
				MessageBox.Show("Survival file replaced!");
			}
			catch
			{
				MessageBox.Show("Error replacing file.");
			}
		}

		private void Exit_Click(object sender, RoutedEventArgs e)
		{
			Environment.Exit(0);
		}

		private void Settings_Click(object sender, RoutedEventArgs e)
		{
			WindowSettings settingsDialog = new WindowSettings();
			if (settingsDialog.ShowDialog() == true)
			{
				using (StreamWriter sw = new StreamWriter(File.Create(Settings.USER_SETTINGS_FILENAME)))
				{
					sw.Write(JsonConvert.SerializeObject(userSettings, Formatting.Indented));
				}
			}
		}

		private void About_Click(object sender, RoutedEventArgs e)
		{
			WindowAbout windowAbout = new WindowAbout();
			windowAbout.Show();
		}

		private void Contact_Click(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("", "Contact");
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

				if (shrinkStart <= shrinkEnd)
				{
					MessageBox.Show("Shrink end value must be smaller than shrink start value.", "Invalid shrink values");
					return;
				}

				spawnset.shrinkStart = Math.Min(100, shrinkStart);
				spawnset.shrinkEnd = Math.Max(1, shrinkEnd);
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

				TextBoxShrinkStart.Text = TextBlockShrinkStart.Text;
				TextBoxShrinkEnd.Text = TextBlockShrinkEnd.Text;
				TextBoxShrinkRate.Text = TextBlockShrinkRate.Text;
				TextBoxBrightness.Text = TextBlockBrightness.Text;

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
			tile = new Point((int)Math.Min(Settings.ARENA_WIDTH - 1, tile.X / 8), (int)Math.Min(Settings.ARENA_HEIGHT - 1, tile.Y / 8));

			float height = spawnset.arenaTiles[(int)tile.X, (int)tile.Y];

			LabelTile.Content = tile.ToString();
			HeightTile.Content = height.ToString();
		}

		private void ArenaTiles_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			Point tile = Mouse.GetPosition((IInputElement)sender);
			tile = new Point((int)tile.X / 8, (int)tile.Y / 8);

			spawnset.arenaTiles[(int)tile.X, (int)tile.Y] = Math.Max(Math.Min(spawnset.arenaTiles[(int)tile.X, (int)tile.Y] + e.Delta / 120, Settings.TILE_MAX), Settings.TILE_MIN);

			HeightTile.Content = spawnset.arenaTiles[(int)tile.X, (int)tile.Y].ToString();

			UpdateArenaGUI();
		}

		private void ArenaTiles_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			Point tile = Mouse.GetPosition((IInputElement)sender);
			tile = new Point((int)tile.X / 8, (int)tile.Y / 8);

			if (spawnset.arenaTiles[(int)tile.X, (int)tile.Y] >= Settings.TILE_MIN)
				spawnset.arenaTiles[(int)tile.X, (int)tile.Y] = Settings.TILE_VOID_DEFAULT;
			else
				spawnset.arenaTiles[(int)tile.X, (int)tile.Y] = Settings.TILE_DEFAULT;

			HeightTile.Content = spawnset.arenaTiles[(int)tile.X, (int)tile.Y].ToString();

			UpdateArenaGUI();
		}

		private void ButtonArenaGenerate_Click(object sender, RoutedEventArgs e)
		{
			int type = ComboBoxArenaPreset.SelectedIndex;

			byte[] defaultArenaBuffer = new byte[Settings.ARENA_BUFFER_SIZE];
			FileStream fs = new FileStream("Content/V3_Sorath", FileMode.Open, FileAccess.Read)
			{
				Position = Settings.HEADER_BUFFER_SIZE
			};
			fs.Read(defaultArenaBuffer, 0, Settings.ARENA_BUFFER_SIZE);
			fs.Close();

			switch (type)
			{
				case 0:
				case 1:
					for (int i = 0; i < defaultArenaBuffer.Length; i += 4)
					{
						int x = (i) / (Settings.ARENA_WIDTH * 4);
						int y = ((i) / 4) % Settings.ARENA_HEIGHT;
						spawnset.arenaTiles[x, y] = (type == 0) ? BitConverter.ToSingle(defaultArenaBuffer, i) : (float)Math.Round(BitConverter.ToSingle(defaultArenaBuffer, i));
					}
					break;
				case 2:
					for (int i = 0; i < Settings.ARENA_WIDTH; i++)
						for (int j = 0; j < Settings.ARENA_HEIGHT; j++)
							spawnset.arenaTiles[i, j] = Settings.TILE_DEFAULT;
					break;
				case 3:
					for (int i = 0; i < Settings.ARENA_WIDTH; i++)
						for (int j = 0; j < Settings.ARENA_HEIGHT; j++)
							spawnset.arenaTiles[i, j] = Settings.TILE_VOID_DEFAULT;
					break;
				case 5:
					WindowArenaRectangular rectangleDialog = new WindowArenaRectangular();
					if (rectangleDialog.ShowDialog() == true)
					{
						ArenaRectangular arena = rectangleDialog.arena;
						for (int i = 0; i < Settings.ARENA_WIDTH; i++)
						{
							for (int j = 0; j < Settings.ARENA_HEIGHT; j++)
							{
								spawnset.arenaTiles[i, j] = Settings.TILE_VOID_DEFAULT;
							}
						}

						for (int i = arena.x1; i < arena.x2; i++)
							for (int j = arena.y1; j < arena.y2; j++)
								spawnset.arenaTiles[i, j] = arena.height;
					}
					break;
				case 6:
					WindowArenaPyramid pyramidDialog = new WindowArenaPyramid();
					if (pyramidDialog.ShowDialog() == true)
					{
						ArenaPyramid arena = pyramidDialog.arena;
						for (int i = 0; i < Settings.ARENA_WIDTH; i++)
						{
							for (int j = 0; j < Settings.ARENA_HEIGHT; j++)
							{
								spawnset.arenaTiles[i, j] = Settings.TILE_VOID_DEFAULT;
							}
						}

						float stepX = (arena.startHeight - arena.endHeight) / (arena.x2 - arena.x1 - 1);
						float stepY = (arena.startHeight - arena.endHeight) / (arena.y2 - arena.y1 - 1);
						for (int i = arena.x1; i < arena.x2; i++)
							for (int j = arena.y1; j < arena.y2; j++)
								spawnset.arenaTiles[i, j] = arena.endHeight + (Math.Abs(i - 25) * stepX + Math.Abs(j - 25) * stepY);
					}
					break;
				case 7:
					for (int i = 0; i < defaultArenaBuffer.Length; i += 4)
					{
						int x = (i) / (Settings.ARENA_WIDTH * 4);
						int y = ((i) / 4) % Settings.ARENA_HEIGHT;
						spawnset.arenaTiles[x, y] = (float)Math.Round(BitConverter.ToSingle(defaultArenaBuffer, i));
					}

					for (int i = 1; i < Settings.ARENA_WIDTH - 1; i++)
						for (int j = 1; j < Settings.ARENA_HEIGHT - 1; j++)
							if ((spawnset.arenaTiles[i - 1, j] < Settings.TILE_DEFAULT
							 || spawnset.arenaTiles[i + 1, j] < Settings.TILE_DEFAULT
							 || spawnset.arenaTiles[i, j - 1] < Settings.TILE_DEFAULT
							 || spawnset.arenaTiles[i, j + 1] < Settings.TILE_DEFAULT)
							 && spawnset.arenaTiles[i, j] == Settings.TILE_DEFAULT)
								spawnset.arenaTiles[i, j] = 16;
					break;
				case 8:
					float a = 0, b = 0;
					for (int i = 0; i < Settings.ARENA_WIDTH; i++)
					{
						a += 0.05f;
						for (int j = 0; j < Settings.ARENA_HEIGHT; j++)
						{
							b += 0.05f;
							spawnset.arenaTiles[i, j] = Utils.r.Next(-1, 1) + a + b - 60;
						}
					}
					break;
			}

			UpdateArenaGUI();
		}

		private void ShrinkCurrentSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			ShrinkCurrent.Width = spawnset.shrinkStart * 4 - (ShrinkCurrentSlider.Value / ShrinkCurrentSlider.Maximum * (spawnset.shrinkStart - spawnset.shrinkEnd) * 4);
			ShrinkCurrent.Height = spawnset.shrinkStart * 4 - (ShrinkCurrentSlider.Value / ShrinkCurrentSlider.Maximum * (spawnset.shrinkStart - spawnset.shrinkEnd) * 4);
			Canvas.SetLeft(ShrinkCurrent, ArenaTiles.Width / 2 - ShrinkCurrent.Width / 2);
			Canvas.SetTop(ShrinkCurrent, ArenaTiles.Height / 2 - ShrinkCurrent.Height / 2);

			UpdateArenaGUI();
		}
	}
}