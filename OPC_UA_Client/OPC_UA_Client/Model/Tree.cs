using System;
using System.Collections.Generic;
using System.Text;

namespace OPC_UA_Client.ViewModel
{
    public class Tree
    {
        public List<ListNode> currentView;

        public Tree()
        {
            currentView = new List<ListNode>();
        }
    }
}
