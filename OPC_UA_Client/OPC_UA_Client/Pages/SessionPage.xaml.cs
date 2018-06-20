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
	public partial class SessionPage : ContentPage
	{
        public ClientOPC client;
        public SessionView sessionView;
        Stack<Tree> hierarchyAddressSpace;
        Stack<string> hierarchyStringAddressSpace;
        public SessionPage (ClientOPC _client, SessionView _sessionView, Tree tree)
		{
            BindingContext = nodes;
            storedTree = tree;
            hierarchyAddressSpace = new Stack<Tree>();
            hierarchyStringAddressSpace = new Stack<string>();
            hierarchyAddressSpace.Push(tree);
            InitializeComponent();
            client = _client;
            sessionView = _sessionView;
            DisplaySession();
            DisplayNodes();
        }
        public ObservableCollection<ListNode> nodes = new ObservableCollection<ListNode>();
        public Tree storedTree;

        private void DisplaySession() {
            NamespaceIndex.Text = sessionView.indexNameSpace;
            Identifier.Text = sessionView.identifier;
            SessionName.Text = sessionView.sessionName;
            EndpointUrl.Text = sessionView.endpointView.endpointURL;
            SecurityMode.Text = sessionView.endpointView.securityMode;
            TransportUri.Text = sessionView.endpointView.transportProfileURI;

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

        private async void OnRead(object sender, EventArgs e)
        {
            ContentPage readPage = new ReadPage(client);
            readPage.Title = "OPC Read Service";
            await Navigation.PushAsync(readPage);
        }

        private async void OnWrite(object sender, EventArgs e)
        {
            ContentPage writePage = new WritePage(client);
            writePage.Title = "OPC Write Service";
            await Navigation.PushAsync(writePage);

        }

        private async void OnSubscription(object sender, EventArgs e)
        {
            ContentPage subPage = new CreateSubscriptionPage(client);
            subPage.Title = "OPC Subscription Service";
            await Navigation.PushAsync(subPage);
        }

        protected override bool OnBackButtonPressed()
        {
            // Begin an asyncronous task on the UI thread because we intend to ask the users permission.
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (await DisplayAlert("Warning", "Are you sure you want to close session?", "Yes", "No"))
                {
                    client.session.Close();
                   
                    base.OnBackButtonPressed();
                    
                    await Navigation.PopToRootAsync();
                }
            });

            // Always return true because this method is not asynchronous.
            // We must handle the action ourselves: see above.
            return true;
        }

        private void OnItemTapped(object sender, ItemTappedEventArgs e)
        {

            if (e.Item == null)
            {
                return;
            }
            ListNode selected = e.Item as ListNode;

            if (selected.Children == true)
            {
                Console.WriteLine("PIPPO" + selected.Id);
                storedTree = client.GetChildren(selected.Id);
                hierarchyAddressSpace.Push(storedTree);
                hierarchyStringAddressSpace.Push(selected.NodeName);
                ParentNodeEntry.Text = selected.NodeName;
                ParentLayout.IsVisible = true;
                DisplayNodes();

            }
        }

        private void OnBackTree(object sender, EventArgs e)
        {
            hierarchyAddressSpace.Pop();
            hierarchyStringAddressSpace.Pop();
            storedTree = hierarchyAddressSpace.First();
            ParentNodeEntry.Text = hierarchyStringAddressSpace.First();
            DisplayNodes();
            Console.WriteLine("HELP" + hierarchyAddressSpace.Count());
        }
    }
}