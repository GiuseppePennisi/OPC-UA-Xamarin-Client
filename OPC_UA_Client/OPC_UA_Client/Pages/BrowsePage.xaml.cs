using Acr.UserDialogs;
using OPC_UA_Client.ViewModel;
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
	public partial class BrowsePage : ContentPage
	{
        ClientOPC client;
        Stack<Tree> hierarchyAddressSpace; //Mantiene la gerarchia di nodi percorsi
        Stack<string> hierarchyStringAddressSpace; //Mantiene la gerarchia di nodi parent 
        public ObservableCollection<ListNode> nodes = new ObservableCollection<ListNode>();
        public Tree storedTree;
        public bool useMode; //Se true permette di leggere o scrivere i nodi dell'address space senza children nodes
                             //Se false permette di creare per un nodo dell'address space un monitored Item su una subscription
        private uint subId; //Necessario per la creazione del monitoredItem su una subscription nota 

        public BrowsePage (ClientOPC _client, Tree tree)
		{
            useMode = true;
            client = _client;
            storedTree = tree;
            BindingContext = nodes;
            hierarchyAddressSpace = new Stack<Tree>();
            hierarchyStringAddressSpace = new Stack<string>();
            hierarchyAddressSpace.Push(tree);
            InitializeComponent ();
            DisplayNodes();
        }

        public BrowsePage(ClientOPC _client, Tree tree, uint _subscriptionID)
        {
            subId = _subscriptionID;
            useMode = false;
            client = _client;
            storedTree = tree;
            BindingContext = nodes;
            hierarchyAddressSpace = new Stack<Tree>();
            hierarchyStringAddressSpace = new Stack<string>();
            hierarchyAddressSpace.Push(tree);
            InitializeComponent();
            DisplayNodes();
        }

        private void DisplayNodes()
        {
            nodes.Clear();
            foreach (var node in storedTree.currentView)
            {
                nodes.Add(node);
            }
            treeView.ItemsSource = null;
            treeView.ItemsSource = nodes;
        }

        private async void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
            {
                return;
            }

            ListNode selected = e.Item as ListNode;

            if (selected.Children == true)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        UserDialogs.Instance.ShowLoading();
                    });
                });

                storedTree = await Task.Run(() =>
                {
                    Task.Delay(200).Wait();
                    return client.GetChildren(selected.Id);
                });
               
                Device.BeginInvokeOnMainThread(() =>
                {
                    UserDialogs.Instance.HideLoading();
                });
                hierarchyAddressSpace.Push(storedTree);
                hierarchyStringAddressSpace.Push(selected.NodeName);
                ParentNodeEntry.Text = selected.NodeName;
                ParentLayout.IsVisible = true;
                DisplayNodes();
                
            }
            else
            {
                await DisplayAlert("Info", "There are no children for this node!", "Ok");
             
                if (selected.NodeClass.Equals("Variable")) {
                    if (useMode)
                    {
                        var selection = await DisplayActionSheet("Select Action: ", "cancel", null, "Read", "Write");
                        if (selection.Equals("Read"))
                        {
                            ContentPage readPage = new ReadPage(client, selected.Id);
                            readPage.Title = "OPC Read Service";
                            await Navigation.PushAsync(readPage);
                            Navigation.RemovePage(this);
                        }
                        else if (selection.Equals("Write"))
                        {
                            ContentPage writePage = new WritePage(client, selected.Id);
                            writePage.Title = "OPC Write Service";
                            await Navigation.PushAsync(writePage);
                            Navigation.RemovePage(this);
                        }
                    }
                    else {
                        var choice = await DisplayAlert("Info", "Do you want to create a monitored item for this variable node?", "yes", "no");

                        if (choice) {
                            MessagingCenter.Send<BrowsePage,string> (this, "update", selected.Id);
                            Navigation.RemovePage(this);
                        }
                    }
                }
            }
        }

        private async void OnBackTree(object sender, EventArgs e)
        {
            Console.WriteLine("STRING HIERARCHY" + hierarchyStringAddressSpace.Count);
            Console.WriteLine("TREE HIERARCHY" + hierarchyAddressSpace.Count);
            if (hierarchyAddressSpace.Count == 1 && hierarchyStringAddressSpace.Count == 0)
            {
                await DisplayAlert("Info", "This is the address space root node!", "Ok");
                storedTree = hierarchyAddressSpace.First();
                ParentLayout.IsVisible = false;
                DisplayNodes();
                return;
            }
            if (hierarchyAddressSpace.Count == 2 && hierarchyStringAddressSpace.Count == 1)
            {
                hierarchyAddressSpace.Pop();
                storedTree = hierarchyAddressSpace.First();
                hierarchyStringAddressSpace.Pop();
                ParentLayout.IsVisible = false;
                DisplayNodes();
                return;
            }
            hierarchyAddressSpace.Pop();
            storedTree = hierarchyAddressSpace.First();
            hierarchyStringAddressSpace.Pop();
            ParentNodeEntry.Text = hierarchyStringAddressSpace.First();
            ParentLayout.IsVisible = true;
            DisplayNodes();
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
            if (Navigation.NavigationStack.Count == 6)
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
            else if (Navigation.NavigationStack.Count == 7)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    {
                        base.OnBackButtonPressed();
                        Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 2]);
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