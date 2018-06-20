using OPC_UA_Client.Pages;
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
    }
}