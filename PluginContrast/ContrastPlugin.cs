using PluginBase;
using System;
using System.Drawing;
using System.Windows.Forms;

public class ContrastPlugin : IPlugin
{
    public string Name => "Contrast Boost";

    public void Initialize(Control container, PictureBox pictureBox)
    {
        Button button = new Button
        {
            Text = "Увеличить контраст",
            Location = new Point(5, container.Controls.Count * 40),
            AutoSize = true
        };

        button.Click += (sender, args) =>
        {
            if (pictureBox.Image != null)
            {
                Bitmap original = new Bitmap(pictureBox.Image);
                Bitmap contrast = new Bitmap(original.Width, original.Height);

                float contrastLevel = 1.2f;

                for (int y = 0; y < original.Height; y++)
                {
                    for (int x = 0; x < original.Width; x++)
                    {
                        Color p = original.GetPixel(x, y);

                        int r = Truncate((int)((p.R - 128) * contrastLevel + 128));
                        int g = Truncate((int)((p.G - 128) * contrastLevel + 128));
                        int b = Truncate((int)((p.B - 128) * contrastLevel + 128));

                        contrast.SetPixel(x, y, Color.FromArgb(r, g, b));
                    }
                }

                pictureBox.Image = contrast;
            }
        };

        container.Controls.Add(button);
    }

    private int Truncate(int value)
    {
        return Math.Min(255, Math.Max(0, value));
    }
}
