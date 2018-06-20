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

        private void OncloseStack(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }
      
    }
}