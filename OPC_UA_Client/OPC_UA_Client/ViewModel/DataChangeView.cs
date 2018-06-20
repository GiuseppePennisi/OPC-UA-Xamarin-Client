using OPC_UA_Client.Pages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace OPC_UA_Client.ViewModel
{
    public class DataChangeView 
    {
  
        public string DisplayName { get; set; }
        public string Value { get; set; }

        public string SourceTimestamp { get; set; }
        public string ServerTimestamp { get; set; }
        public string StatusCode { get; set; }
        public uint ClientHandle { get; set; }
       

        public DataChangeView(string displayName, string value, string sourceTimestamp, string serverTimestamp, string statusCode, uint clientHandle)
        {
            DisplayName = displayName;
            Value = value;
            SourceTimestamp = sourceTimestamp;
            ServerTimestamp = serverTimestamp;
            StatusCode = statusCode;
            ClientHandle = clientHandle;
           
        }

     
        public DataChangeView(uint clientHandle){
            ClientHandle = clientHandle;
           

        }
        
    }
}
