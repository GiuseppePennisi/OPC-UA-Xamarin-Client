using System;
using System.Runtime.Serialization;

namespace OPC_UA_Client
{
    [Serializable]
    internal class NoNodeToReadException : Exception
    {
        public NoNodeToReadException()
        {
        }

        public NoNodeToReadException(string message) : base(message)
        {
        }

        public NoNodeToReadException(string message, Exception innerException) : base(message, innerException)
        {
        }

      

        protected NoNodeToReadException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}