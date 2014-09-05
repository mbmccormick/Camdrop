using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Camdrop.API;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Popups;
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

        private async void RenderStatusBar()
        {
            var statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
            statusBar.ForegroundColor = Color.FromArgb(255, 0, 176, 237);
            statusBar.BackgroundOpacity = 0.0;
            
            await statusBar.HideAsync();
        }

        private async void ShowStatusBar()
        {
            //var statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
            //statusBar.ProgressIndicator.ProgressValue = null;

            //await statusBar.ProgressIndicator.ShowAsync();
        }

        private async void HideStatusBar()
        {
            //var statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
            //statusBar.ProgressIndicator.ProgressValue = 0.0;

            //await statusBar.ProgressIndicator.ShowAsync();
        }

        private void LoadData()
        {
            DispatcherTimer dt = new DispatcherTimer();
            dt.Interval = new TimeSpan(0, 0, 0, 2, 0);
            dt.Tick += DispatchTimer_Tick;

            dt.Start();
        }

        private async void DispatchTimer_Tick(object sender, object e)
        {
            ShowStatusBar();

            await App.DropcamClient.CamerasGetImage((result) =>
            {
                Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    HideStatusBar();

                    var stream = new InMemoryRandomAccessStream();
                    stream.WriteAsync(result.AsBuffer());
                    stream.Seek(0);

                    var image = new BitmapImage();
                    image.SetSource(stream);

                    stream.Dispose();
                    
                    this.imgCameraFeed.Source = image;
                    this.txtTimestamp.Text = DateTime.Now.ToString();
                });
            }, UUID, 1280);

            HideStatusBar();
        }
    }
}
