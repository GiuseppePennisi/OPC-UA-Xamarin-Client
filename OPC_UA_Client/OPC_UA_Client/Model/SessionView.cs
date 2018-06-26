using System;
using System.Collections.Generic;
using System.Text;

namespace OPC_UA_Client.ViewModel
{
    public class SessionView
    {
        public string identifier;
        public string indexNameSpace;
        public string sessionName;
        public EndpointView endpointView;

        public SessionView(string _identifier,string _indexNameSpace, string _sessionName, EndpointView _endpointView){
            identifier = _identifier;
            indexNameSpace = _indexNameSpace;
            sessionName = _sessionName;
            endpointView = _endpointView;
        }
        public SessionView() {

        }
    }
    
}
