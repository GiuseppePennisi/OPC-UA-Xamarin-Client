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
        MessagingCenter.Subscribe<BrowsePage, string>(this, "update", (page, nodeId) => { //Node Id Format: ns=1;i=1003
            Device.BeginInvokeOnMainThread(() => {
                    string[] tmp = nodeId.Split(';');
                    string nSIndex = tmp[0].Substring(3);
                    string idNode = tmp[1].Substring(2);
                    NodeID.Text = idNode;
                    NodeNamespace.Text = nSIndex;
                    this.NodeID.Text = idNode;
                    this.NodeNamespace.Text = nSIndex;
                });
            });

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
            int typeID;
            ushort namespaceIndex;
            string identifierNode=null;
            uint identifierNodeInt=0;
            int samplingInterval;
            bool discardOldest;
            uint queueSize;
            int monitoringMode;
            int filterTrigger;
            uint deadbandType;
            double deadbandValue;

            try
            {
                typeID = TypeNodeIdPicker.SelectedIndex;
                if (string.IsNullOrEmpty(NodeID.Text))
                    throw new EmptyEntryException("Empty Node ID Entry!");
                if (typeID == 0)
                {
                    try
                    {
                        identifierNodeInt = Convert.ToUInt32(NodeID.Text);
                    }
                    catch (FormatException p)
                    {
                        throw new FormatException("Node ID Format not valid!", p);
                    }
                }
                else { 
                identifierNode = NodeID.Text;
                }
                if (string.IsNullOrEmpty(NodeNamespace.Text))
                    throw new EmptyEntryException("Empty Node Namespace Entry!");
                try { 
                namespaceIndex = Convert.ToUInt16(NodeNamespace.Text);
                }
                catch(FormatException p)
                {
                    throw new FormatException("Node Namespace not valid!", p);
                }

                if (string.IsNullOrEmpty(RequestedSamplingInterval.Text))
                    throw new EmptyEntryException("Requested Sampling Interval Empty!");
                try { 
                samplingInterval = Convert.ToInt32(RequestedSamplingInterval.Text);
                }
                catch(FormatException p){
                    throw new FormatException("Requested Sampling Interval format not valid!", p);
                }
                  discardOldest = DiscardOldest.IsToggled;
                if (string.IsNullOrEmpty(QueueSize.Text))
                    throw new EmptyEntryException("Empty Queue Size Entry!");
                try
                {
                    queueSize = Convert.ToUInt32(QueueSize.Text);
                }
                catch(FormatException p)
                {
                    throw new FormatException("Queue Size format non valid!", p);
                }
                monitoringMode = MonitoringModePicker.SelectedIndex;
                filterTrigger = TriggerPicker.SelectedIndex;
                
                deadbandType = Convert.ToUInt32(DeadbandTypePicker.SelectedIndex);
                if (string.IsNullOrEmpty(DeadbandValue.Text))
                    throw new EmptyEntryException("Empty Deadband Value Entry!");
                try { 
                deadbandValue = Convert.ToDouble(DeadbandValue.Text);
                    if ((deadbandType == 2) && (deadbandValue < 0 || deadbandValue > 1))
                        throw new FormatException("Deadband value is not between 0 and 1!");
                }
                catch(FormatException p)
                {
                    throw new FormatException("Deadband Value Format is not valid!", p);
                }

                if (typeID == 0)
                {
                    client.CreateMonitoredItem(subscriptionId, namespaceIndex, identifierNodeInt, samplingInterval, discardOldest, queueSize, monitoringMode, filterTrigger, deadbandType, deadbandValue);
                }
                else
                {
                    client.CreateMonitoredItem(subscriptionId, namespaceIndex, identifierNode, samplingInterval, discardOldest, queueSize, monitoringMode, filterTrigger, deadbandType, deadbandValue);

                }
                await DisplayAlert("Info", "Monitored Item Created Successfully", "Ok");

                ContentPage detailSubPage1 = new DetailSubscriptionPage(client, subscriptionId);
                detailSubPage1.Title = "OPC Subscription Details";
                Navigation.RemovePage(Navigation.NavigationStack[this.Navigation.NavigationStack.Count - 2]);
                await Navigation.PushAsync(detailSubPage1);
                
                Navigation.RemovePage(this);
            }
            catch(FormatException p)
            {
                await DisplayAlert("Error", p.Message, "Ok");
            }
            catch (EmptyEntryException p)
            {
                await DisplayAlert("Error", p.Message, "Ok");
            }
            catch (NoNodeToReadException p)
            {
                await DisplayAlert("Error", p.Message, "Ok");
            }
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
                    Navigation.RemovePage(this);
                }
            });

            // Always return true because this method is not asynchronous.
            // We must handle the action ourselves: see above.
            return true;
        }

        private async void OnBrowse(object sender, EventArgs e)
        {
            ContentPage browsePage = new BrowsePage(client, client.GetRootNode(), subscriptionId);
            browsePage.Title = "OPC Browse Service";
            await Navigation.PushAsync(browsePage);
            //Gestire on Back Navigation
        }

        private void gotoSessionClicked(object sender, EventArgs e)
        {
            Console.WriteLine("PIPPOOOOO"+Navigation.NavigationStack.Count);
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
    }
}