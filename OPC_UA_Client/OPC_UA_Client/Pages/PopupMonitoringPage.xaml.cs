using OPC_UA_Client.ViewModel;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Xamarin.Forms;
using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace OPC_UA_Client.Pages
{
    
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PopupMonitoringPage : PopupPage
    {
        public ClientOPC client;
        public uint CHandle;
        public DataChangeView v;
        public Color default_color;
        string message;
        bool isNumeric;
        double result;
        string reset = "--/--/---- --:--:--";
        public  PopupMonitoringPage (ClientOPC _client, uint clientHandle)
		{
            
            CloseWhenBackgroundIsClicked = true;
            CHandle = clientHandle;
            client = _client;
            message = "update: " + CHandle;

            MessagingCenter.Subscribe<ClientOPC,DataChangeView>(this,message,(client,view)=> {

            Device.BeginInvokeOnMainThread(() => {
                ClientHandleEntry.Text = view.ClientHandle.ToString();

                if (string.IsNullOrEmpty(view.SourceTimestamp.ToString()))
                    SourceTimeEntry.Text = reset;
                else
                    SourceTimeEntry.Text = view.SourceTimestamp.ToString();

                if (string.IsNullOrEmpty(view.ServerTimestamp.ToString()))
                    ServerTimeEntry.Text = reset;
                else
                    ServerTimeEntry.Text = view.ServerTimestamp.ToString();

                StatusCodeEntry.Text = view.StatusCode.ToString();

                ValueEntry.Text = view.Value.ToString();

                isNumeric = Double.TryParse(view.Value, out result);
                
                GraphButton.IsEnabled = isNumeric;
                if (!FrameContainer.IsVisible) {
                    if (LabelEmptyFrame.IsVisible == true)
                    {
                        BackgroundColor = default_color;
                        LabelEmptyFrame.IsVisible = false;
                    }
                    FrameContainer.IsVisible = true;
                }
            });
                
            });
            InitializeComponent();
            FrameContainer.IsVisible = false;
            LabelEmptyFrame.IsVisible = true;
            default_color= this.BackgroundColor;
            this.BackgroundColor = Color.White;
            GraphButton.IsEnabled = false;
            SubscribePage();
        }

        protected override bool OnBackButtonPressed()
        {
            // Begin an asyncronous task on the UI thread because we intend to ask the users permission.
            Device.BeginInvokeOnMainThread(async () =>
            {
                MessagingCenter.Unsubscribe<ClientOPC, DataChangeView>(this,message);
                    await Navigation.PopAsync();
                 Navigation.RemovePage(this);
            });

            // Always return true because this method is not asynchronous.
            // We must handle the action ourselves: see above.
            return true;
        }

        private void CloseMonitoring(object sender, EventArgs e)
        {
            // Begin an asyncronous task on the UI thread because we intend to ask the users permission.
            Device.BeginInvokeOnMainThread(async () =>
            {
                MessagingCenter.Unsubscribe<ClientOPC, DataChangeView>(this, message);
                await Navigation.PopAsync();
                Navigation.RemovePage(this);
            });

            // Always return true because this method is not asynchronous.
            // We must handle the action ourselves: see above.
            return;
        }

       

        private async Task GoToGraph(object sender, EventArgs e)
        {
            var _GraphPage = new GraphPage(client,message);
            _GraphPage.Title = "Item Client Handle: " + ClientHandleEntry.Text;
            await Navigation.PushAsync(_GraphPage);
        }

        private void SubscribePage()
        {
            MessagingCenter.Subscribe<ClientOPC>(this, "SessionClose",
                async (sender) => {

                    await Task.Run(() =>
                    {
                        Device.BeginInvokeOnMainThread(() => DisplayAlert("Error", "Session Expired!", "Ok"));
                    });


                    await Task.Run(() =>
                    {
                        Device.BeginInvokeOnMainThread(() => {
                            Page p = new MainPage();
                            Navigation.PushAsync(p);
                            foreach (var page in Navigation.NavigationStack.ToList())
                            {
                                if (page != p)
                                {
                                    Navigation.RemovePage(page);
                                }
                            }
                            MessagingCenter.Unsubscribe<ClientOPC>(this, "SessionClose");
                        });
                    });
                });
        }
    }
    }

    
