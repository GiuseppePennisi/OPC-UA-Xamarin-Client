using System;
using System.Runtime.Serialization;

namespace OPC_UA_Client
{
    [Serializable]
    internal class EmptyEntryException : Exception
    {
        private Guid id;

        public EmptyEntryException()
        {
        }

        public EmptyEntryException(string message) : base(message)
        {
        }

        public EmptyEntryException(Guid id)
        {
            this.id = id;
        }

        public EmptyEntryException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected EmptyEntryException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}