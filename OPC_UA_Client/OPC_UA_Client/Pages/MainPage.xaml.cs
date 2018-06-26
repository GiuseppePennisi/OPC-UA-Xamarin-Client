
using Acr.UserDialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace OPC_UA_Client
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
	{
        
        
        ClientOPC OpcClient = new ClientOPC();

        
        string endpointUrl = null;
        public MainPage()
		{   
            
			InitializeComponent();
           
            
		}

    async void OnConnect(object sender, EventArgs e)
        {
           
            endpointUrl = EntryUrl.Text;
            if (endpointUrl != null)
            {
                bool connectToServer = true;
               
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        UserDialogs.Instance.ShowLoading();
                    });
                
                ConnectButton.IsEnabled = false;
                ListEndpoint results;
                await Task.Run(() =>OpcClient.CreateCertificate());
                if (OpcClient.haveAppCertificate == false)
                {
                    connectToServer = await DisplayAlert("Warning!", "missing application certificate!, \nusing unsecure connection. \nDo you want to continue?", "Yes", "No");
                }
                if (connectToServer == true)
                {
                    try{ 
                    /*Esecuzione su un task parallelo per vedere la connectIndicator ruotare*/
                    results = await Task.Run(() => OpcClient.DiscoveryEndpoints(endpointUrl));
                    if (results == null)
                    {

                            Device.BeginInvokeOnMainThread(() =>
                            {
                                UserDialogs.Instance.HideLoading();
                            });
                            ConnectButton.IsEnabled = true;
                        await DisplayAlert("Error", "The URI Format is invalid!", "Ok");
                        return;

                    }
                    ContentPage listEndpointRoot = new EndpointsPage(results, OpcClient);
                    listEndpointRoot.Title = "Endpoints";
                    Device.BeginInvokeOnMainThread(() =>
                        {
                            UserDialogs.Instance.HideLoading();
                        });
                        ConnectButton.IsEnabled = true;
                    await Navigation.PushAsync(listEndpointRoot);
                }
                    catch (BadConnectException p)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            UserDialogs.Instance.HideLoading();
                        });
                        ConnectButton.IsEnabled = true;
                        await DisplayAlert("Error", p.Message, "Ok");
                        return;

                    }
                }
                else {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        UserDialogs.Instance.HideLoading();
                    });
                    await DisplayAlert("Warning", "Cannot connect to an OPC UA server!", "Ok");
                }

                
            }
            else
            {
                await DisplayAlert("Warning", "Server endpoint URL cannot be null!", "Ok");
            }


        }
    }
}
