using System;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows.Input;
using System.IO;
using System.Windows.Media;

namespace MyCustomApp
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer; // Zamanlayıcı değişkeni, belirli aralıklarla ekran görüntüsü almak için kullanılır
        private MemoryStream memoryStream; // Ekran görüntülerini geçici olarak hafızada tutmak için kullanılır
        private GifBitmapEncoder gifEncoder; // Ekran görüntülerini GIF formatına dönüştürmek için kullanılır

        public MainWindow()
        {
            InitializeComponent();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100); // 100ms'lik aralıklarla ekran görüntüsü alınacak
            timer.Tick += Timer_Tick;
            timer.Start();

            gifEncoder = new GifBitmapEncoder();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Ekranın anlık görüntüsünü alır
            var screenshot = new RenderTargetBitmap(
                (int)SystemParameters.PrimaryScreenWidth,
                (int)SystemParameters.PrimaryScreenHeight,
                96, 96, PixelFormats.Pbgra32);
            screenshot.Render(Application.Current.MainWindow);

            // Ekran görüntüsünü Image kontrolüne atar
            screenshotImage.Source = screenshot;

            // Ekran görüntüsünü GIF'e ekler
            var frame = BitmapFrame.Create(screenshot);
            gifEncoder.Frames.Add(frame);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Kaydet düğmesine tıklandığında GIF'i kaydeder
            memoryStream = new MemoryStream();
            gifEncoder.Save(memoryStream);
            memoryStream.Position = 0;

            var saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.Filter = "GIF files (*.gif)|*.gif"; // Sadece GIF formatında kaydetme seçeneği sunar

            if (saveFileDialog.ShowDialog() == true)
            {
                using (var fileStream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                {
                    memoryStream.WriteTo(fileStream);
                }
            }
        }
    }
}
