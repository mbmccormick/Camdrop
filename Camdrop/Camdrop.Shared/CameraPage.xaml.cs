using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Graphics.Display;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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

            this.SizeChanged += Frame_SizeChanged;

#if WINDOWS_PHONE_APP
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Landscape;
#endif
        }

        private void Frame_SizeChanged(object sender, SizeChangedEventArgs e)
        {
#if WINDOWS_APP
            if (e.NewSize.Width <= 800)
            {
                this.stkCamera.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Center;

                this.txtTimestampBanner.Margin = new Thickness(0, -84, 0, 0);
                this.txtTimestamp.FontSize = 24;
                this.txtTimestamp.Margin = new Thickness(6, 6, 6, 6);
            }
            else
            {
                this.stkCamera.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Top;

                this.txtTimestampBanner.Margin = new Thickness(0, -84, 0, 0);
                this.txtTimestamp.FontSize = 48;
                this.txtTimestamp.Margin = new Thickness(12, 12, 12, 12);
            }
#endif
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            UUID = e.Parameter as string;

            RenderStatusBar();

            LoadData();
        }

        private async void RenderStatusBar()
        {
#if WINDOWS_PHONE_APP
            var statusBar = StatusBar.GetForCurrentView();

            await statusBar.HideAsync();
#endif

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
            }, UUID, 1280);

            HideStatusBar();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
                Frame.GoBack();
        }
    }
}
