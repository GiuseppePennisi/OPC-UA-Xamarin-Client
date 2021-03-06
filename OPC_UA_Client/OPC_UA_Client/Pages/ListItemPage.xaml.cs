﻿using Acr.UserDialogs;
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
	public partial class ListItemPage : ContentPage
	{
        public ClientOPC client;
        public uint subscriptionId;
        ObservableCollection<MonitoredItemView> monitoredItemViews = new ObservableCollection<MonitoredItemView>();

        public ListItemPage ()
		{
			InitializeComponent ();
            SubscribePage();
		}

        public ListItemPage(ClientOPC _client, uint subId)
        {
            BindingContext = monitoredItemViews;
            client = _client;
            subscriptionId = subId;
            InitializeComponent();
            displayItems();
            SubscribePage();
        }

        private void displayItems()
        {
            monitoredItemViews.Clear();
            foreach (MonitoredItemView item in client.GetMonitoredItemViews(subscriptionId))
            {
                monitoredItemViews.Add(item);
            }
            if (monitoredItemViews.Count == 0) {
                NoItemsLabel.IsVisible = true;
            }
            MonitoredItemsDisplay.ItemsSource = null;
            MonitoredItemsDisplay.SeparatorColor = Color.FromHex("#ff9800");
            MonitoredItemsDisplay.ItemsSource = monitoredItemViews;
        }

        private async void OnSelectedItem(object sender, ItemTappedEventArgs e)
        {
            MonitoredItemView selected = e.Item as MonitoredItemView;
           var _monitorPopup = new PopupMonitoringPage(client, selected.clientHandle);
            _monitorPopup.Title = "Item Monitoring Service";
            await Navigation.PushAsync(_monitorPopup);
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

        private void gotoSessionClicked(object sender, EventArgs e)
        {
            if (Navigation.NavigationStack.Count == 5)
            {
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
            else if (Navigation.NavigationStack.Count == 6)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {

                    {

                        base.OnBackButtonPressed();
                        Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 2]);
                        Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 2]);
                        await Navigation.PopAsync();

                        Navigation.RemovePage(this);
                    }
                });
            }
        }

        private async void OnItemDelete(object sender, EventArgs e)
        {
            if (await DisplayAlert("Warning", "Do you want to cancel the selected monitored item?", "yes", "no"))
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    UserDialogs.Instance.ShowLoading();
                });
                var button = sender as Button;
                var itemView = button.BindingContext as MonitoredItemView;
                await Task.Run(() => client.DeleteMonitoredItem(itemView.subscriptionId, itemView.clientHandle));
                displayItems();
                Device.BeginInvokeOnMainThread(() =>
                {
                    UserDialogs.Instance.HideLoading();
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
                            p.Title = "Client OPC";
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