namespace OPC_UA_Client
{
    public class MonitoredItemView
    {
        public uint clientHandle { get; set; }
        public ushort namespaceIndex { get; set; }
        public string identifier { get; set; }
        public uint subscriptionId { get; set; }
        public int samplingInterval { get; set; }
        public string filterTrigger { get; set; }
        public string deadbandType { get; set; }
        public double deadbandValue { get; set; }

        public MonitoredItemView() {
        }

        public MonitoredItemView(uint clientHandle, ushort namespaceIndex, string identifier, uint subscriptionId, int samplingInterval, string filterTrigger, string deadbandType, double deadbandValue)
        {
            this.clientHandle = clientHandle;
            this.namespaceIndex = namespaceIndex;
            this.identifier = identifier;
            this.subscriptionId = subscriptionId;
            this.samplingInterval = samplingInterval;
            this.filterTrigger = filterTrigger;
            this.deadbandType = deadbandType;
            this.deadbandValue = deadbandValue;
        }
    }
}