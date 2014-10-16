using System;
using System.Linq;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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

            this.txtUsername.IsTapEnabled = false;
            this.txtPassword.IsTapEnabled = false;

            await App.DropcamClient.LoginLogin(async (result) =>
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
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

                        this.txtUsername.IsTapEnabled = true;
                        this.txtPassword.IsTapEnabled = true;

                        MessageDialog dialog = new MessageDialog(result.status_detail, "Login Failed");
                        await dialog.ShowAsync();
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