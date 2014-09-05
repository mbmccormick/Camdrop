using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Camdrop.API;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Camdrop
{
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            this.InitializeComponent();

            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;
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

            if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("Username"))
            {
                this.txtUsername.Text = ApplicationData.Current.RoamingSettings.Values["Username"] as string;
            }

            if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("Password"))
            {
                this.txtPassword.Password = ApplicationData.Current.RoamingSettings.Values["Password"] as string;
            }

            if (this.txtUsername.Text.Length > 0 &&
                this.txtPassword.Password.Length > 0)
            {
                btnLogin_Click(this, null);
            }

            HideStatusBar();
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            ShowStatusBar();

            this.txtUsername.IsReadOnly = true;
            this.txtPassword.IsEnabled = false;

            await App.DropcamClient.LoginLogin((result) =>
            {
                Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    HideStatusBar();

                    if (result.status == 0)
                    {
                        // login succeeded

                        ApplicationData.Current.RoamingSettings.Values["Username"] = this.txtUsername.Text;
                        ApplicationData.Current.RoamingSettings.Values["Password"] = this.txtPassword.Password;

                        Frame.Navigate(typeof(MainPage));

                        if (Frame.CanGoBack)
                            Frame.BackStack.Remove(Frame.BackStack.LastOrDefault());
                    }
                    else
                    {
                        // login failed

                        ApplicationData.Current.RoamingSettings.Values.Remove("Username");
                        ApplicationData.Current.RoamingSettings.Values.Remove("Password");

                        this.txtUsername.IsReadOnly = false;
                        this.txtPassword.IsEnabled = true;

                        MessageDialog dialog = new MessageDialog(result.status_detail, "Login Failed");
                        dialog.ShowAsync();
                    }
                });
            }, this.txtUsername.Text, this.txtPassword.Password);
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            this.txtUsername.Text = "";
            this.txtPassword.Password = "";
        }
    }
}