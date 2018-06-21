﻿using System;
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
        public WritePage(ClientOPC _client)
        {
            client = _client;
            InitializeComponent();
        }

        private async void OnWrite(object sender, EventArgs e)
        {
            List<String> statusCodes;
            string nodeID;
            uint id = 0;
            ushort nodeNameSpace;
            int typeNodeId = TypeNodeIdPicker.SelectedIndex;
            int selectedTypeID = TypeNodeIdPicker.SelectedIndex;
            object value = null;

            try
            {
                if (string.IsNullOrEmpty(NodeID.Text))
                    throw new EmptyEntryException("Empty Node ID Entry!");
                nodeID = NodeID.Text;
                if (typeNodeId == 0)
                {
                    try
                    {
                        id = Convert.ToUInt32(nodeID);
                    }
                    catch (FormatException p)
                    {
                        throw new FormatException("Node ID Format not valid!", p);
                    }
                }
                if (string.IsNullOrEmpty(NodeNamespace.Text))
                    throw new EmptyEntryException("Empty Node Namespace Entry!");
                try
                {
                    nodeNameSpace = Convert.ToUInt16(NodeNamespace.Text);
                }
                catch (FormatException p)
                {
                    throw new FormatException("Node Namespace not valid!", p);
                }
                switch (DataTypePicker.SelectedIndex)
                {
                    case 0:
                        try
                        {
                            if (string.IsNullOrEmpty(ValueToWrite.Text))
                                throw new EmptyEntryException("Empty Value Entry!");
                            value = Convert.ToUInt32(ValueToWrite.Text);
                        }
                        catch (FormatException p)
                        {
                            throw new FormatException("Value not valid!", p);
                        }

                        break;
                    case 1:
                        value = ValueToWrite.Text;
                        break;
                    case 2:
                        value = BoolSwitch.IsToggled;
                        break;
                }
                if (typeNodeId == 0)
                    statusCodes = client.WriteVariable(id, nodeNameSpace, value, 13); /*13 = tipo enumerativo Attributes.Value, Versione con uint*/
                else
                {
                    statusCodes = client.WriteVariable(nodeID, nodeNameSpace, value, 13); /*13 = tipo enumerativo Attributes.Value, Versione con string*/

                }
                foreach (var s in statusCodes)
                {
                    await DisplayAlert("Info", "Write of node (" + nodeID + ", " + nodeNameSpace.ToString() + ") with status: " + s, "OK");
                }
            }
            catch (EmptyEntryException p)
            {
                await DisplayAlert("Error", p.Message, "Ok");
            }
            catch (FormatException p)
            {

                await DisplayAlert("Error", p.Message, "Ok");
            }

            catch (NoNodeToReadException p)
            {
                await DisplayAlert("Error", p.Message, "Ok");

            }

        }
        //Questa funzione permette di abilitare o disabilitare le entry del data type value
        private void OnSelectedType(object sender, EventArgs e)
        {
            if (DataTypePicker.SelectedIndex == 2)
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