using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

using System.Reflection;
using System.IO;
using System.Linq;
using PluginBase;


namespace Lab3
{
    public partial class Form1 : Form
    {
        private readonly string accessKey = "Zizz4KyENxpec0ovQ7O-CFSfqroazPharotKGks8PgQ"; 
        private CancellationTokenSource cts;

        private void LoadPlugins()
        {

            string pluginDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");
            if (!Directory.Exists(pluginDir))
                Directory.CreateDirectory(pluginDir);

            foreach (var file in Directory.GetFiles(pluginDir, "*.dll"))
            {
                try
                {
                    var asm = Assembly.LoadFrom(file);
                    var types = asm.GetTypes().Where(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

                    foreach (var type in types)
                    {
                        IPlugin plugin = (IPlugin)Activator.CreateInstance(type);
                        plugin.Initialize(pluginPanel, pictureBox1);

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке плагина {Path.GetFileName(file)}: {ex.Message}");
                }
            }
        }


        public Form1()
        {
            InitializeComponent();
            LoadPlugins();
        }

        private async Task<string> GetImageUrlAsync(string query)
        {
            string url = $"https://api.unsplash.com/search/photos?page=1&query={query}&client_id={accessKey}";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string response = await client.GetStringAsync(url);
                    JObject json = JObject.Parse(response);

                    var firstImage = json["results"]?[0]?["urls"]?["regular"]?.ToString();
                    return firstImage;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при загрузке: " + ex.Message);
                    return null;
                }
            }
        }

        private async void button1_Click_1(object sender, EventArgs e)
        {
            string query = textBox1.Text.Trim();
            if (string.IsNullOrWhiteSpace(query))
            {
                MessageBox.Show("Введите запрос для первой картинки");
                return;
            }

   

            cts = new CancellationTokenSource();

            // Запускаем обе загрузки в параллели
            _ = LoadImagesLoop(pictureBox1, query, cts.Token);
            _ = LoadImagesLoop(pictureBox2, query, cts.Token);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            cts?.Cancel();
        }

        private async Task LoadImagesLoop(PictureBox box, string query, CancellationToken token)
        {
            using (HttpClient client = new HttpClient())
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        string url = $"https://api.unsplash.com/search/photos?page=1&query={query}&client_id={accessKey}";
                        string response = await client.GetStringAsync(url);
                        JObject json = JObject.Parse(response);

                        var images = json["results"];
                        if (images != null && images.HasValues)
                        {
                            // Выбираем случайное изображение из результатов
                            var rand = new Random();
                            int index = rand.Next(images.Count());
                            string imageUrl = images[index]["urls"]?["regular"]?.ToString();

                            if (!string.IsNullOrEmpty(imageUrl))
                            {
                                box.Invoke((MethodInvoker)(() =>
                                {
                                    box.LoadAsync(imageUrl);
                                }));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Ошибка загрузки: " + ex.Message);
                    }

                    await Task.Delay(5000, token); // ждем 5 секунд
                }
            }
        }

    }
}

