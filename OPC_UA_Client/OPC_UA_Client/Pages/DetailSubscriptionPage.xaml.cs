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
            SubscribePage();
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
                    base.OnBackButtonPressed();
                    await Navigation.PopAsync();
                    Navigation.RemovePage(this);
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

        private void gotoSessionClicked(object sender, EventArgs e)
        {
            if (Navigation.NavigationStack.Count == 4)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {

                    {

                        base.OnBackButtonPressed();

                        await Navigation.PopAsync();

                        Navigation.RemovePage(this);
                    }
                });
            }
            else if(Navigation.NavigationStack.Count == 5) {
                Device.BeginInvokeOnMainThread(async () =>
                {

                    {

                        base.OnBackButtonPressed();
                        Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 2]);
                        await Navigation.PopAsync();

                        Navigation.RemovePage(this);
                    }
                });
            }

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