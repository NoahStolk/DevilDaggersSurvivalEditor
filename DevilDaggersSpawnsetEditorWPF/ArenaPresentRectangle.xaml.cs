using System.Windows;

namespace DevilDaggersSpawnsetEditorWPF
{
	public partial class ArenaPresentRectangle : Window
	{
		public Rect rect;

		public ArenaPresentRectangle()
		{
			InitializeComponent();
		}

		private void OKButton_Click(object sender, RoutedEventArgs e)
		{
			if (!int.TryParse(TextBoxX1.Text, out int x1))
			{
				MessageBox.Show("Please enter a numeric value.", "Invalid X1 value");
				return;
			}
			if (!int.TryParse(TextBoxY1.Text, out int y1))
			{
				MessageBox.Show("Please enter a numeric value.", "Invalid Y1 value");
				return;
			}
			if (!int.TryParse(TextBoxX2.Text, out int x2))
			{
				MessageBox.Show("Please enter a numeric value.", "Invalid X2 value");
				return;
			}
			if (!int.TryParse(TextBoxY2.Text, out int y2))
			{
				MessageBox.Show("Please enter a numeric value.", "Invalid Y2 value");
				return;
			}

			if (x1 < 0 || y1 < 0 || x2 < 0 || y2 < 0 || x1 > 51 || y1 > 51 || x2 > 51 || y2 > 51)
			{
				MessageBox.Show("Values must be between 0 and 51.", "Invalid value(s)");
				return;
			}

			if (x1 >= x2 || y1 >= y2)
			{
				MessageBox.Show("The first position's X and Y must be smaller than the second position's X and Y.", "Invalid value(s)");
				return;
			}

			rect = new Rect(new Point(x1, y1), new Point(x2, y2));
			DialogResult = true;
		}
	}
}