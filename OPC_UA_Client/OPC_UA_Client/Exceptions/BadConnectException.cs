using System;
using System.Runtime.Serialization;

namespace OPC_UA_Client
{
    [Serializable]
    internal class BadConnectException : Exception
    {
        public BadConnectException()
        {
        }

        public BadConnectException(string message) : base(message)
        {
        }

        public BadConnectException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BadConnectException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}