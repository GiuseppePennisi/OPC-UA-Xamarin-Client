using System;
using System.Collections.Generic;
using System.Text;

namespace OPC_UA_Client.ViewModel
{
    public class ListNode
    {
        public string Id { get; set; }
        public string NodeClass { get; set; }
        public string AccessLevel { get; set; }
        public string Executable { get; set; }
        public string EventNotifier { get; set; }
        public bool Children { get; set; }

        public string NodeName { get; set; }

        public ListNode()
        {
        }
    }
}
