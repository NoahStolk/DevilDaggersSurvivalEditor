using DevilDaggersSurvivalEditor.Code;
using DevilDaggersSurvivalEditor.Code.ArenaPresets;
using DevilDaggersSurvivalEditor.Code.Utils;
using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DevilDaggersSurvivalEditor.GUI.Windows
{
	public partial class ArenaPresetWindow : Window
	{
		public AbstractArena Preset { get; set; }

		public ArenaPresetWindow(string presetName)
		{
			InitializeComponent();

			Title = $"{presetName} arena preset";

			Preset = ArenaPresetHandler.Instance.ArenaPresets.Where(a => a.GetType().Name == presetName).FirstOrDefault();

			foreach (PropertyInfo p in Preset.GetType().GetProperties())
			{
				OptionLabels.Children.Add(new Label()
				{
					Content = p.Name.ToUserFriendlyString(),
					Padding = new Thickness()
				});

				TextBox textBox = new TextBox()
				{
					Name = p.Name,
					Text = p.GetValue(Preset).ToString(),
					Padding = new Thickness(),
					Tag = p.PropertyType
				};
				textBox.TextChanged += TextBox_TextChanged;
				OptionInputs.Children.Add(textBox);
			}
		}

		// TODO: Use binding instead (not sure if possible with programmatically generating elements through reflection)
		private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			TextBox textBox = sender as TextBox;

			Type t = textBox.Tag as Type;
			bool valid;
			if (t == typeof(float))
			{
				valid = float.TryParse(textBox.Text, out _);
			}
			else if (t == typeof(int))
			{
				valid = int.TryParse(textBox.Text, out _);
			}
			else
			{
				Exception ex = new Exception($"Type {t} not supported in ArenaPreset TextBox.");
				Logging.Log.Error($"Type {t} not supported in ArenaPreset TextBox.", ex);
				throw ex;
			}

			if (valid)
				textBox.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
			else
				textBox.Background = new SolidColorBrush(Color.FromRgb(255, 128, 128));
		}

		private void GenerateButton_Click(object sender, RoutedEventArgs e)
		{
			foreach (PropertyInfo p in Preset.GetType().GetProperties())
			{
				foreach (UIElement child in OptionInputs.Children)
				{
					if (child is TextBox textBox)
					{
						if (textBox.Name == p.Name)
						{
							Type t = p.PropertyType;

							if (t == typeof(float))
							{
								if (!float.TryParse(textBox.Text, out float value))
									return;
								p.SetValue(Preset, value);
							}
							else if (t == typeof(int))
							{
								if (!int.TryParse(textBox.Text, out int value))
									return;
								p.SetValue(Preset, value);
							}
							else
							{
								Exception ex = new Exception($"Type {t} not supported in ArenaPreset TextBox.");
								Logging.Log.Error($"Type {t} not supported in ArenaPreset TextBox.", ex);
								throw ex;
							}
						}
					}
				}
			}

			DialogResult = true;
		}
	}
}