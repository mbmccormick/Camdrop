using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace Camdrop
{
    public sealed partial class CameraPage : Page
    {
        public string UUID;

        public CameraPage()
        {
            this.InitializeComponent();

            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Landscape;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            UUID = e.Parameter as string;

            RenderStatusBar();

            LoadData();
        }

        private void RenderStatusBar()
        {
            this.prgStatusBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void ShowStatusBar()
        {
            this.prgStatusBar.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        private void HideStatusBar()
        {
            this.prgStatusBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void LoadData()
        {
            DispatcherTimer dt = new DispatcherTimer();
            dt.Interval = new TimeSpan(0, 0, 0, 1, 0);
            dt.Tick += DispatchTimer_Tick;

            dt.Start();
        }

        private async void DispatchTimer_Tick(object sender, object e)
        {
            ShowStatusBar();

            await App.DropcamClient.CamerasGetImage(async (result) =>
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    HideStatusBar();

                    var stream = new InMemoryRandomAccessStream();

                    await stream.WriteAsync(result.AsBuffer());

                    stream.Seek(0);

                    var image = new BitmapImage();
                    image.SetSource(stream);

                    stream.Dispose();

                    this.imgCameraFeed.Source = image;
                    this.txtTimestamp.Text = DateTime.Now.ToString();
                });
            }, UUID, 2560);

            HideStatusBar();
        }
    }
}
