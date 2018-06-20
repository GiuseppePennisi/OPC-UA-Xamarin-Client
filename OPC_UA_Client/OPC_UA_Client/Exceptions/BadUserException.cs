using System;
using System.Runtime.Serialization;

namespace OPC_UA_Client
{
    [Serializable]
    internal class BadUserException : Exception
    {
        public BadUserException()
        {
        }

        public BadUserException(string message) : base(message)
        {
        }

        public BadUserException(string message, Exception innerException) : base(message, innerException)
        {
        }

   

        protected BadUserException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}