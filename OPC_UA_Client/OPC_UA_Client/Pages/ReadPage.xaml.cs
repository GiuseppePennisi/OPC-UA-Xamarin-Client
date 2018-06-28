using OPC_UA_Client.Pages;
using OPC_UA_Client.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace OPC_UA_Client
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReadPage : ContentPage
    {
        List<NodeView> nodesRead = new List<NodeView>();
        ClientOPC client;
        ObservableCollection<NodeView> readCollection = new ObservableCollection<NodeView>();
        public ReadPage(ClientOPC _client)
        {
            InitializeComponent();
            client = _client;
            DisplayReads();
            SubscribePage();
        }

        public ReadPage(ClientOPC _client, string _nodeId) //Node Id Format: ns=1;i=1003
        {
            InitializeComponent();

            string[] tmp = _nodeId.Split(';');
            string nSIndex = tmp[0].Substring(3);
            string idNode = tmp[1].Substring(2);
            NodeID.Text = idNode;
            NodeNamespace.Text = nSIndex;

            client = _client;
            DisplayReads();
            SubscribePage();
        }

        public ReadPage(ClientOPC _client, ListNode node) {
            NodeID.Text = node.Id;
            SubscribePage();
        }
        public void DisplayReads()
        {

            readCollection.Clear();
            foreach (var node in nodesRead)
            {
                readCollection.Add(node);

            }



        }

        public async void onRead(object sender, EventArgs e)
        {
            int selectedTypeID = TypeNodeIdPicker.SelectedIndex;
            int timeStamp = TimestampPicker.SelectedIndex;
            string nodeID = NodeID.Text;
            ushort nodeNameSpace = 0;
            double maxAge = 0;


            try
            {
                if (string.IsNullOrEmpty(NodeNamespace.Text))
                    throw new EmptyEntryException();
                nodeNameSpace = Convert.ToUInt16(NodeNamespace.Text);
                try
                {
                    if (string.IsNullOrEmpty(MaxAge.Text))
                        throw new EmptyEntryException();
                    maxAge = Convert.ToDouble(MaxAge.Text);


                    try
                    {
                        if (string.IsNullOrEmpty(NodeID.Text))
                            throw new EmptyEntryException();
                        if (selectedTypeID == 0)
                        {
                            uint id;

                            id = Convert.ToUInt32(nodeID);

                            nodesRead = client.readVariable(id, nodeNameSpace, maxAge, timeStamp, 13); /*13 = tipo enumerativo Attributes.Value*/
                        }

                        else if (selectedTypeID == 1)
                        {
                            nodesRead = client.readVariable(nodeID, nodeNameSpace, maxAge, timeStamp, 13); /*13 = tipo enumerativo Attributes.Value*/
                        }

                        ContentPage popReadPage = new PopupReadPage(nodesRead);
                        popReadPage.Title = "Read Details";
                        await Navigation.PushAsync(popReadPage);


                    }
                    catch (EmptyEntryException)
                    {
                        await DisplayAlert("Error", "Empty Node Identifier Entry!", "Ok");
                    }
                    catch (FormatException)
                    {
                        await DisplayAlert("Error", "Invalid Node Identifier Format!", "Ok");
                    }
                    catch (NoNodeToReadException p)
                    {
                        await DisplayAlert("Error", p.Message, "Ok");

                    }


                }
                catch (FormatException)
                {

                    await DisplayAlert("Error", "Invalid Max Age Format!", "Ok");
                }
                catch (EmptyEntryException)
                {
                    await DisplayAlert("Error", "Empty Max Age Entry!", "Ok");
                }
            }
            catch (FormatException)
            {

                await DisplayAlert("Error", "Invalid Namespace Format!", "Ok");
            }
            catch (EmptyEntryException)
            {
                await DisplayAlert("Error", "Empty Namespace  Entry!", "Ok");
            }

        }

        protected override bool OnBackButtonPressed()
        {
            // Begin an asyncronous task on the UI thread because we intend to ask the users permission.
            Device.BeginInvokeOnMainThread(async () =>
            {

                {

                    base.OnBackButtonPressed();

                    await Navigation.PopAsync();
                }
            });

            // Always return true because this method is not asynchronous.
            // We must handle the action ourselves: see above.
            return true;
        }

        private void gotoSessionClicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {

                {

                    base.OnBackButtonPressed();

                    await Navigation.PopAsync();

                    Navigation.RemovePage(this);
                }
            });

            // Always return true because this method is not asynchronous.
            // We must handle the action ourselves: see above.
            
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