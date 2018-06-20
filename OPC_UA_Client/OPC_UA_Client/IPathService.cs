using System;
using System.Collections.Generic;
using System.Text;

namespace OPC_UA_Client
{
   public interface IPathService
    {
        string InternalFolder { get; }
        string PublicExternalFolder { get; }
        string PrivateExternalFolder { get; }
    }
}
