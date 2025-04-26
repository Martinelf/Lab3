using PluginBase;
using System.Drawing;
using System.Windows.Forms;

public class GrayscalePlugin : IPlugin
{
    public string Name => "Grayscale Filter";

    public void Initialize(Control container, PictureBox pictureBox)
    {
        Button button = new Button
        {
            Text = "Ч/Б фильтр",
            Location = new Point(5, container.Controls.Count * 40),
            AutoSize = true
        };
        MessageBox.Show("here");

        button.Click += (sender, args) =>
        {
            if (pictureBox.Image != null)
            {
                Bitmap original = new Bitmap(pictureBox.Image);
                Bitmap gray = new Bitmap(original.Width, original.Height);

                for (int y = 0; y < original.Height; y++)
                {
                    for (int x = 0; x < original.Width; x++)
                    {
                        Color pixel = original.GetPixel(x, y);
                        int avg = (pixel.R + pixel.G + pixel.B) / 3;
                        gray.SetPixel(x, y, Color.FromArgb(avg, avg, avg));
                    }
                }

                pictureBox.Image = gray;
            }
        };

        container.Controls.Add(button);
    }
}
