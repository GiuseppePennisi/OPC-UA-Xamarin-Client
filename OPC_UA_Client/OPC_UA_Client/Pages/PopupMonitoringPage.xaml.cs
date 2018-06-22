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
        string message;
        bool isNumeric;
        double result;
        public  PopupMonitoringPage (ClientOPC _client, uint clientHandle)
		{


          
            
            CloseWhenBackgroundIsClicked = true;
            CHandle = clientHandle;
            client = _client;
            message = "update: " + CHandle;
       

        //devi verificare che il client Handle sia uguale a quello che passi con a sub e la send 




        MessagingCenter.Subscribe<ClientOPC,DataChangeView>(this,message,(client,view)=> {
               
                
                Device.BeginInvokeOnMainThread(() => {
                
                ClientHandleEntry.Text = view.ClientHandle.ToString();
                SourceTimeEntry.Text = view.SourceTimestamp.ToString();
              //ServerTimeEntry.Text = view.ServerTimestamp.ToString();
                StatusCodeEntry.Text = view.StatusCode.ToString();
                ValueEntry.Text = view.Value.ToString();
                isNumeric = Double.TryParse(view.Value, out result);
                GraphButton.IsEnabled = isNumeric;
                });
                
            });
            InitializeComponent();
             
            
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
            var _GraphPage = new GraphPage(client,message, ClientHandleEntry.Text);
            await Navigation.PushAsync(_GraphPage);
        }

       
    }
    }

    
