using System;
using System.Collections.Generic;
using System.Text;

namespace OPC_UA_Client.ViewModel
{
   public class SubscriptionView
    {
       
        public uint SubscriptionID { get; set; }
        public double PublishingInterval { get; set; }
        public uint LifeTimeCount { get; set; }
        public uint KeepAliveCount { get; set; }
        public uint MaxNotificationPerPublish { get; set; }
        public bool PublishEnabled { get; set; }
        public byte Priority { get; set; }

        public SubscriptionView(uint subscriptionID, double publishingInterval, uint lifeTimeCount, uint keepAliveCount, uint maxNotificationPerPublish, bool publishEnabled, byte priority)
        {
            SubscriptionID = subscriptionID;
            PublishingInterval = publishingInterval;
            LifeTimeCount = lifeTimeCount;
            KeepAliveCount = keepAliveCount;
            MaxNotificationPerPublish = maxNotificationPerPublish;
            PublishEnabled = publishEnabled;
            Priority = priority;
        }

        public SubscriptionView()
        {

        }



    }
}
