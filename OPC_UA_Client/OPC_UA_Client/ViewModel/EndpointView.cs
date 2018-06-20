using System;
using System.Collections.Generic;
using System.Text;
//Singolo Endpoint
namespace OPC_UA_Client
{
 public   class EndpointView
    {
        public string endpointURL { get; set; }
        public string securityMode { get; set; }
        public string transportProfileURI { get; set; }
        public int endpointID { get; set; }
        public EndpointView(string _endPointURL, string _securityMode, string _transportProfileURI,int _endpointID) {

            endpointURL = _endPointURL;
            securityMode = _securityMode;
            transportProfileURI = _transportProfileURI;
            endpointID = _endpointID;

        }

        public EndpointView() {

        }
    }
}
