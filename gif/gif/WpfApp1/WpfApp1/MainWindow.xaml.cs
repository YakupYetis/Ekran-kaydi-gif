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
        private DispatcherTimer timer; 
        private MemoryStream memoryStream; 
        private GifBitmapEncoder gifEncoder; 

        public MainWindow()
        {
            InitializeComponent();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100); 
            timer.Tick += Timer_Tick;
            timer.Start();

            gifEncoder = new GifBitmapEncoder();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            var screenshot = new RenderTargetBitmap(
                (int)SystemParameters.PrimaryScreenWidth,
                (int)SystemParameters.PrimaryScreenHeight,
                96, 96, PixelFormats.Pbgra32);
            screenshot.Render(Application.Current.MainWindow);

            screenshotImage.Source = screenshot;

            var frame = BitmapFrame.Create(screenshot);
            gifEncoder.Frames.Add(frame);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            memoryStream = new MemoryStream();
            gifEncoder.Save(memoryStream);
            memoryStream.Position = 0;

            var saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.Filter = "GIF files (*.gif)|*.gif"; 

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
