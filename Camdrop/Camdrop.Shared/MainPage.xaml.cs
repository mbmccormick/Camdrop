using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using Camdrop.API;
using Windows.ApplicationModel;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace Camdrop
{
    public sealed partial class MainPage : Page
    {
        public static ObservableCollection<CameraItem> VisibleCameras { get; set; }

        public MainPage()
        {
            this.InitializeComponent();

#if WINDOWS_PHONE_APP
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;
#endif

            VisibleCameras = new ObservableCollection<CameraItem>();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            RenderStatusBar();

            LoadData();
        }

        private async void RenderStatusBar()
        {
#if WINDOWS_PHONE_APP
            var statusBar = StatusBar.GetForCurrentView();
            statusBar.ForegroundColor = Color.FromArgb(255, 0, 0, 0);
            statusBar.BackgroundColor = Color.FromArgb(255, 0, 176, 237);
            statusBar.BackgroundOpacity = 1.0;

            await statusBar.ShowAsync();
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

        private async void LoadData()
        {
            ShowStatusBar();

            await App.DropcamClient.CamerasGetVisible(async (result) =>
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    HideStatusBar();

                    if (result.status == 0)
                    {
                        // request succeeded

                        VisibleCameras.Clear();

                        foreach (Camera item in result.items)
                        {
                            if (String.IsNullOrEmpty(item.location) == true)
                                item.location = "(location unknown)";

                            CameraItem viewModel = new CameraItem();
                            viewModel.Camera = item;

                            if (item.is_streaming_enabled == true)
                            {
                                await App.DropcamClient.CamerasGetImage(async (result2) =>
                                {
                                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                                    {
                                        HideStatusBar();

                                        var stream = new InMemoryRandomAccessStream();

                                        await stream.WriteAsync(result2.AsBuffer());

                                        stream.Seek(0);

                                        var image = new BitmapImage();
                                        image.SetSource(stream);

                                        stream.Dispose();

                                        viewModel.Thumbnail = image;
                                    });
                                }, item.uuid, 150);
                            }
                            else
                            {
                                StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(Package.Current.InstalledLocation.Path);
                                Stream stream = await folder.OpenStreamForReadAsync("DropcamOffline.png");

                                //var memStream = new MemoryStream();
                                //stream.CopyTo(memStream);
                                //memStream.Position = 0;

                                var image = new BitmapImage();
                                image.SetSource(stream.AsRandomAccessStream());

                                viewModel.Thumbnail = image;
                            }

                            VisibleCameras.Add(viewModel);
                        }
                    }
                    else
                    {
                        // request failed

                        MessageDialog dialog = new MessageDialog(result.status_detail, "Request Failed");
                        await dialog.ShowAsync();
                    }
                });
            });

            HideStatusBar();
        }

        private void ItemContent_Tapped(object sender, TappedRoutedEventArgs e)
        {
            CameraItem item = ((FrameworkElement)sender).DataContext as CameraItem;

            if (item.Camera.is_streaming_enabled == true)
            {
                Frame.Navigate(typeof(CameraPage), item.Camera);
            }
        }
        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }
    }

    public class CameraItem
    {
        public Camera Camera { get; set; }
        public BitmapImage Thumbnail { get; set; }
    }
}
