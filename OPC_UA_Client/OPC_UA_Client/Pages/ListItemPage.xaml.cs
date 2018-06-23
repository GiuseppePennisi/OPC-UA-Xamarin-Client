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
		}

        public ListItemPage(ClientOPC _client, uint subId)
        {
            BindingContext = monitoredItemViews;
            client = _client;
            subscriptionId = subId;
            InitializeComponent();
            displayItems();
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
            Console.WriteLine("Client handle Detail sub: " + selected.clientHandle);
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
    }
}