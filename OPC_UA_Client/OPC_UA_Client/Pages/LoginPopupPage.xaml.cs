using OPC_UA_Client.ViewModel;
using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace OPC_UA_Client
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPopupPage : PopupPage
    {
        ClientOPC client;
        int index;
        SessionView sessionView;

        public LoginPopupPage(ClientOPC _client, int _index, SessionView _sessionView)
        {


            CloseWhenBackgroundIsClicked = true;
            index = _index;
            client = _client;
            sessionView = _sessionView;
            InitializeComponent();


        }



        private async void OnLoginButton(object sender, EventArgs e)
        {
            LoadingIndicator.IsRunning = true;

            try
            {

                string username = UsernameEntry.Text;
                string password = PasswordEntry.Text;

                sessionView = await client.CreateSessionChannelAsync(index, username, password);

            }
            catch (BadUserException p)
            {


                IsEnabled = false;
                IsVisible = false;
                LoadingIndicator.IsRunning = false;
                await DisplayAlert("Error", p.Message, "ok");
                return;

            }
            if (sessionView == null)
            {


                IsEnabled = false;
                IsVisible = false;
                LoadingIndicator.IsRunning = false;
                await DisplayAlert("Error", "Cannot connect to an OPC UA Server", "OK");
            }
            else
            {


                IsEnabled = false;
                IsVisible = false;
                LoadingIndicator.IsRunning = false;
                await DisplayAlert("Info", "Session created successfully", "Ok");
                ContentPage sessionPage = new SessionPage(client, sessionView);
                sessionPage.Title = "OPC Session Services";
                await Navigation.PushAsync(sessionPage);
            }

        }

    }
}
