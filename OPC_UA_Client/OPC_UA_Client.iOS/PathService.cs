using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(OPC_UA_Client.PathService))]
namespace OPC_UA_Client
{
    class PathService :IPathService
    {
        public string InternalFolder
        {
            get
            {
                return null;
            }
        }

        public string PublicExternalFolder
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
        }

        public string PrivateExternalFolder
        {
            get
            {
                return null;
            }
        }
    }
}