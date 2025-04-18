using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace FileLoaderApp
{
    public partial class MainWindow : Window
    {
        private const string ChromePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe";
        private const string DownloadUrl = "https://dl.google.com/chrome/install/ChromeSetup.exe";
        private readonly string _tempPath = Path.Combine(Path.GetTempPath(), "ChromeSetup.exe");

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            DownloadButton.IsEnabled = false;

            try
            {
                // Проверяем, установлен ли Chrome
                if (File.Exists(ChromePath))
                {
                    MessageBox.Show("Всё есть, можно работать!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Если Chrome не найден, начинаем загрузку
                MessageBox.Show("Chrome не найден. Начинаем загрузку...", "Информация",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                using var client = new HttpClient();
                using var response = await client.GetAsync(DownloadUrl);
                response.EnsureSuccessStatusCode();

                // Сохраняем файл во временную папку
                using var fileStream = new FileStream(_tempPath, FileMode.Create, FileAccess.Write);
                await response.Content.CopyToAsync(fileStream);

                // Запускаем установщик
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = _tempPath,
                    UseShellExecute = true
                });
                MessageBox.Show("Установщик Chrome запущен. Следуйте инструкциям для установки.",
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                DownloadButton.IsEnabled = true;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}