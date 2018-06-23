using OPC_UA_Client.ViewModel;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace OPC_UA_Client.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DetailSubscriptionPage : ContentPage
	{
        ClientOPC client;
        uint subscriptionId;
		public DetailSubscriptionPage (ClientOPC _client, uint _subscriptionId)
		{
            InitializeComponent();
            subscriptionId = _subscriptionId;
            client = _client;
            DisplaySubscription();
		}

        private void DisplaySubscription()
        {
            SubscriptionView subView = client.GetSubscriptionViewById(subscriptionId);
            subscriptionIdentifier.Text = subView.SubscriptionID.ToString();
            publishInterval.Text = subView.PublishingInterval.ToString();
            lifetimeCount.Text = subView.LifeTimeCount.ToString();
            keepAliveCount.Text = subView.KeepAliveCount.ToString();
            maxNotifications.Text = subView.MaxNotificationPerPublish.ToString();
            priority.Text = subView.Priority.ToString();
            publishingEnabled.Text = subView.PublishEnabled.ToString();
        }

        private async void NewMonitoredItem(object sender, EventArgs e)
        {
            ContentPage monPage = new CreateMonitoredItemPage(client, subscriptionId);
            monPage.Title = "Create Monitored Item Section";
           
            await Navigation.PushAsync(monPage);
            
        }

        protected override bool OnBackButtonPressed()
        {
            // Begin an asyncronous task on the UI thread because we intend to ask the users permission.
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (await DisplayAlert("Warning", "Do you want to close Subscription?", "Yes", "No"))
                {
                    client.CloseSubscription(subscriptionId);

                    base.OnBackButtonPressed();
                  
                    await Navigation.PopAsync();
                    Navigation.RemovePage(this);
                }
            });

            // Always return true because this method is not asynchronous.
            // We must handle the action ourselves: see above.
            return true;
        }

        private async void OnViewItems(object sender, EventArgs e)
        {
            ContentPage listItemPage = new ListItemPage(client, subscriptionId);
            listItemPage.Title = "Monitored Items View";
            await Navigation.PushAsync(listItemPage);
        }
    }
}