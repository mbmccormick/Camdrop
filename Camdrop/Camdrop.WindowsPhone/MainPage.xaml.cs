using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public sealed partial class MainPage : Page
    {
        public static ObservableCollection<CameraItem> VisibleCameras { get; set; }

        public MainPage()
        {
            this.InitializeComponent();

            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;

            VisibleCameras = new ObservableCollection<CameraItem>();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            RenderStatusBar();

            LoadData();
        }

        private async void RenderStatusBar()
        {
            var statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
            statusBar.ForegroundColor = Color.FromArgb(255, 0, 176, 237);
            statusBar.BackgroundOpacity = 0.0;
            statusBar.ProgressIndicator.ProgressValue = 0.0;

            await statusBar.ProgressIndicator.ShowAsync();
        }

        private async void ShowStatusBar()
        {
            var statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
            statusBar.ProgressIndicator.ProgressValue = null;

            await statusBar.ProgressIndicator.ShowAsync();
        }

        private async void HideStatusBar()
        {
            var statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
            statusBar.ProgressIndicator.ProgressValue = 0.0;

            await statusBar.ProgressIndicator.ShowAsync();
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
                            CameraItem viewModel = new CameraItem();
                            viewModel.Camera = item;

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

            Frame.Navigate(typeof(CameraPage), item.Camera.uuid);
        }
    }

    public class CameraItem
    {
        public Camera Camera { get; set; }
        public BitmapImage Thumbnail { get; set; }
    }
}
