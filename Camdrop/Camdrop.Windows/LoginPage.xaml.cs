using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
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
                    }
                    else
                    {
                        // login failed

                        ApplicationData.Current.RoamingSettings.Values.Remove("Username");
                        ApplicationData.Current.RoamingSettings.Values.Remove("Password");

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
