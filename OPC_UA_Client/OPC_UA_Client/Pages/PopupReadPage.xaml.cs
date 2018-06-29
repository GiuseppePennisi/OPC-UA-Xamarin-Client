using Rg.Plugins.Popup.Pages;
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
	public partial class PopupReadPage : PopupPage
    {
        List<NodeView> nodesRead;
        ObservableCollection<NodeView> readCollection = new ObservableCollection<NodeView>();
        public PopupReadPage (List<NodeView> _nodes)
		{
            
            
            InitializeComponent();
            nodesRead =_nodes;
            ReadDisplay.ItemsSource = nodesRead;
            DisplayReads();
            SubscribePage();
            
		}

        public void DisplayReads()
        {

            readCollection.Clear();
            foreach (var node in nodesRead)
            {
                readCollection.Add(node);

            }

            ReadDisplay.ItemsSource = null;
            ReadDisplay.ItemsSource = readCollection;
           }

        private async void OncloseStack(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
            Navigation.RemovePage(this);
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
                            p.Title = "Client OPC";
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