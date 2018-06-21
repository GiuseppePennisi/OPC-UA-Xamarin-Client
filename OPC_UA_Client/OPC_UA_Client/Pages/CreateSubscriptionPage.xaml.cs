using OPC_UA_Client.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace OPC_UA_Client.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CreateSubscriptionPage : ContentPage
	{
        ClientOPC client;
		public CreateSubscriptionPage (ClientOPC _client)
		{
            client = _client;
			InitializeComponent ();
		}

        private async void OnCreateSubscription(object sender, EventArgs e)
        {
          bool createItem;
          Double reqPubInterval = Convert.ToDouble(RequestedPublishingInterval.Text);
          uint reqLifeTimeCount = Convert.ToUInt32(RequestedLifetimeCount.Text);
          uint reqMaxKeepAliveCount = Convert.ToUInt32(RequestedMaxKeepAliveCount.Text);
          uint maxNotPerPublish= Convert.ToUInt32(MaxNotificationPerPublish.Text);
          byte priority = Convert.ToByte(Priority.Text);
          
          
            SubscriptionView subView = client.CreateSub(reqPubInterval, reqLifeTimeCount, reqMaxKeepAliveCount, maxNotPerPublish, true, priority);
            if (subView.PublishingInterval != reqPubInterval || subView.KeepAliveCount != reqMaxKeepAliveCount || subView.LifeTimeCount != reqLifeTimeCount)
            {
                createItem = await DisplayAlert("Info", "Subscription created successfully with revised parameters.\nDo you want to create a monitored item?", "yes", "no");
            }
            else {
                createItem = await DisplayAlert("Info", "Subscription created successfully with requested parameters.\nDo you want to create a monitored item?", "yes", "no");
            }
            if (createItem)
            {
                ContentPage monPage = new CreateMonitoredItemPage(client,subView.SubscriptionID);
                monPage.Title = "Create Monitored Item Section";
                await Navigation.PushAsync(monPage);
            }
            else {
                ContentPage detailSubPage = new DetailSubscriptionPage(client, subView.SubscriptionID);
                detailSubPage.Title = "OPC Subscription Details";
                await Navigation.PushAsync(detailSubPage);
            }
        }
    }
}