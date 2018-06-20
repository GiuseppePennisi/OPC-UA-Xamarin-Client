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
	public partial class WritePage : ContentPage
	{
        ClientOPC client;
		public WritePage (ClientOPC _client)
		{
            client = _client;
            InitializeComponent ();
		}

        private  void OnWrite(object sender, EventArgs e)
        {
            List<String> statusCodes;
            string nodeID = NodeID.Text;
            ushort nodeNameSpace = Convert.ToUInt16(NodeNamespace.Text);
            
            int typeNodeId = TypeNodeIdPicker.SelectedIndex;
            int selectedTypeID = TypeNodeIdPicker.SelectedIndex;
            object value = null;
            switch(DataTypePicker.SelectedIndex){
                case 0:
                    value =Convert.ToUInt32(ValueToWrite.Text);
                    break;
                case 1:
                    value = ValueToWrite.Text;
                    break;
                case 2:
                    value = BoolSwitch.IsToggled;
                    break;
            }
            try
            {

                statusCodes = client.WriteVariable(typeNodeId, nodeID, nodeNameSpace,value, 13); /*13 = tipo enumerativo Attributes.Value*/
                foreach(var s in statusCodes)
                {
                     DisplayAlert("Info", "Write of node ("+nodeID+", "+ nodeNameSpace.ToString()+") with status: "+s, "OK");
                }
            }
            catch (NoNodeToReadException p)
            {
                DisplayAlert("Error", p.Message, "Ok");

            }
            catch (FormatException p)
            {
                DisplayAlert("Error", p.Message, "ok");
            }
            

        }
        //Questa funzione permette di abilitare o disabilitare le entry del data type value
        private void OnSelectedType(object sender, EventArgs e)
        {
            if(DataTypePicker.SelectedIndex==2)
            {
                IntStringEntry.IsEnabled = false;
                IntStringEntry.IsVisible = false;
                BooleanEntry.IsVisible = true;
                BooleanEntry.IsEnabled = true;

            }
            else
            {
                IntStringEntry.IsEnabled = true;
                IntStringEntry.IsVisible = true;
                BooleanEntry.IsVisible = false;
                BooleanEntry.IsEnabled = false;


            }
        }
    }
}