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
	public partial class CreateMonitoredItemPage : ContentPage
	{
        ClientOPC client;
        uint subscriptionId;
		public CreateMonitoredItemPage (ClientOPC _client, uint _subscriptionId)
		{
            client = _client;
            subscriptionId = _subscriptionId;
			InitializeComponent ();
		}

        private void OnDisplayFilterSettings(object sender, EventArgs e)
        {
            if (FilterForm.IsVisible == false && FilterForm.IsEnabled == false) {
                FilterForm.IsVisible = true;
                FilterForm.IsEnabled = true;
            } else if (FilterForm.IsVisible == true && FilterForm.IsEnabled == true) {
                FilterForm.IsVisible = false;
                FilterForm.IsEnabled = false;
            }
        }

        private async void OnCreateMonitoredItem(object sender, EventArgs e)
        {
            int typeID = TypeNodeIdPicker.SelectedIndex;
            ushort namespaceIndex = Convert.ToUInt16(NodeNamespace.Text);
            string identifierNode = NodeID.Text;
            int samplingInterval = Convert.ToInt32(RequestedSamplingInterval.Text);
            bool discardOldest = DiscardOldest.IsToggled;
            uint queueSize = Convert.ToUInt32(QueueSize.Text);
            int monitoringMode = MonitoringModePicker.SelectedIndex;
            int filterTrigger = TriggerPicker.SelectedIndex;
            uint deadbandType = Convert.ToUInt32(DeadbandTypePicker.SelectedIndex);
            double deadbandValue = Convert.ToDouble(DeadbandValue.Text);
            client.CreateMonitoredItem(subscriptionId, typeID, namespaceIndex, identifierNode, samplingInterval,discardOldest, queueSize, monitoringMode, filterTrigger, deadbandType, deadbandValue);

            await DisplayAlert("Info", "Monitored Item Created Successfully", "Ok");

            ContentPage detailSubPage = new DetailSubscriptionPage(client,subscriptionId);
            detailSubPage.Title = "OPC Subscription Details";
            await Navigation.PushAsync(detailSubPage);
        }

        protected override bool OnBackButtonPressed()
        {
            // Begin an asyncronous task on the UI thread because we intend to ask the users permission.
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (await DisplayAlert("Warning", "Do you want go back to Subscription Detail Page?", "Yes", "No"))
                {
                   

                    base.OnBackButtonPressed();

                    await Navigation.PopAsync();
                }
            });

            // Always return true because this method is not asynchronous.
            // We must handle the action ourselves: see above.
            return true;
        }
    }
}