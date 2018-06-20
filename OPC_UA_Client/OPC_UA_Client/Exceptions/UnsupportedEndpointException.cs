using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace OPC_UA_Client.Exceptions
{
    class UnsupportedEndpointException : Exception
    {
        public UnsupportedEndpointException()
        {
        }

        public UnsupportedEndpointException(string message) : base(message)
        {
        }

        public UnsupportedEndpointException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UnsupportedEndpointException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
