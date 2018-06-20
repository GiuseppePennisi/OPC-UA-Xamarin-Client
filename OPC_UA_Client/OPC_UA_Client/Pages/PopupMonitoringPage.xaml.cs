using OPC_UA_Client.ViewModel;
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
        public  PopupMonitoringPage (ClientOPC _client, uint clientHandle)
		{
           
            CHandle = clientHandle;
            client = _client;
            string message = "update: " + CHandle;
            //devi verificare che il client Handle sia uguale a quello che passi con a sub e la send 
            MessagingCenter.Subscribe<ClientOPC,DataChangeView>(this,message,(client,view)=> {
                Console.WriteLine("Sono dentro la subscribe");
                Device.BeginInvokeOnMainThread(() => {
                    ClientHandleEntry.Text = view.ClientHandle.ToString();
                SourceTimeEntry.Text = view.SourceTimestamp.ToString();
              //  ServerTimeEntry.Text = view.ServerTimestamp.ToString();
                StatusCodeEntry.Text = view.StatusCode.ToString();
                ValueEntry.Text = view.Value.ToString();
                });

            });
            InitializeComponent();
            
            
        }

        protected override bool OnBackButtonPressed()
        {
            // Begin an asyncronous task on the UI thread because we intend to ask the users permission.
            Device.BeginInvokeOnMainThread(async () =>
            {
                    await Navigation.PopAsync();
                
            });

            // Always return true because this method is not asynchronous.
            // We must handle the action ourselves: see above.
            return true;
        }





    }
    }

    
