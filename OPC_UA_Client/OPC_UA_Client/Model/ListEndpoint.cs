using System;
using System.Collections.Generic;
using System.Text;

namespace OPC_UA_Client
{
public  class ListEndpoint
    {

        public List<EndpointView> endpointList;
        public ListEndpoint()
        {
            endpointList = new List<EndpointView>();
            
        }
    }
}
