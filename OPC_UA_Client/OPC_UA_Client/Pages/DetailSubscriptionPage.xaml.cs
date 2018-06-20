﻿using OPC_UA_Client.ViewModel;
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
        ObservableCollection<MonitoredItemView> monitoredItemViews = new ObservableCollection<MonitoredItemView>();
        ClientOPC client;
        uint subscriptionId;
		public DetailSubscriptionPage (ClientOPC _client, uint _subscriptionId)
		{
            InitializeComponent();
            BindingContext = monitoredItemViews;
            subscriptionId = _subscriptionId;
            client = _client;
            DisplaySubscription();
            displayItems();
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

        void displayItems()
        {
            monitoredItemViews.Clear();
            foreach (MonitoredItemView item in client.GetMonitoredItemViews(subscriptionId))
            {
                monitoredItemViews.Add(item);
            }
            MonitoredItemsDisplay.ItemsSource = null;
            MonitoredItemsDisplay.SeparatorColor = Color.Blue;
            MonitoredItemsDisplay.ItemsSource = monitoredItemViews;
        }

        private async void OnSelectedItem(object sender, ItemTappedEventArgs e)
        {
            MonitoredItemView selected = e.Item as MonitoredItemView;
            Console.WriteLine("Client handle Detail sub: " + selected.clientHandle);
            var _monitorPopup = new PopupMonitoringPage(client, selected.clientHandle);
            await Navigation.PushAsync(_monitorPopup);
        }
    }
}