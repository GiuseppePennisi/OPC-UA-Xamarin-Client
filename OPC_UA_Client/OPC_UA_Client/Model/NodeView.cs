using System;
using System.Collections.Generic;
using System.Text;

namespace OPC_UA_Client
{
    public class NodeView
    {
        public string value { get; set; }
        public string statusCode { get; set; }
        public string timeStampSource { get; set; }
        public string timeStampServer { get; set; }

        public NodeView(string _value, string _statusCode, string _timeStampSource,string _timeStampServer )
        {
    
            value = _value;
            statusCode = _statusCode;
            timeStampSource = _timeStampSource;
            timeStampServer = _timeStampServer;
        }
    public NodeView()
        {

        }
    }
    }

