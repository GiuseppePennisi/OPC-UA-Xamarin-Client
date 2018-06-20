using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;
using OPC_UA_Client.ViewModel;
using Xamarin.Forms;

namespace OPC_UA_Client
{
    public class ClientOPC
    {
        ApplicationInstance application;
        public Session session;
        public EndpointDescriptionCollection endpoints { get; set; }
        public ApplicationConfiguration config;
        public bool haveAppCertificate;
        public uint clientHandle { get; set; }
        public List<DataChangeView> dataChangeViews;

        public ClientOPC()
        {
            session = null;
            dataChangeViews = new List<DataChangeView>();
            config = null;
            clientHandle = 0;
        }

        // Recuperare l'elenco degli endpoint esposti dal server
        public ListEndpoint DiscoveryEndpoints(String endpointURL)
        {

            ListEndpoint list = new ListEndpoint();
            var endpointConfiguration = EndpointConfiguration.Create(config);
            try
            {
                Uri uri = new Uri(endpointURL);

                DiscoveryClient discoveryC = DiscoveryClient.Create(uri, endpointConfiguration);

                endpoints = discoveryC.GetEndpoints(null);

                for (int i = 0; i < endpoints.Count; i++)
                {
                    list.endpointList.Add(new EndpointView(endpoints[i].EndpointUrl, endpoints[i].SecurityMode.ToString(), endpoints[i].TransportProfileUri, i));

                }

                return list;
            }
            catch (System.UriFormatException)
            {

                return list = null;
            }

            catch (ServiceResultException p)
            {

                throw new BadConnectException("Impossible to connect to server!", p);

            }

        }

        public async void CreateCertificate()
        {

            application = new ApplicationInstance
            {
                ApplicationType = ApplicationType.Client,
                ConfigSectionName = "Opc.Ua.Client"
            };

            if (Device.RuntimePlatform == "Android")
            {

                string currentFolder = DependencyService.Get<IPathService>().PublicExternalFolder.ToString();
                string filename = application.ConfigSectionName + ".Config.xml";

                string content = DependencyService.Get<IAssetService>().LoadFile(filename);

                File.WriteAllText(currentFolder + filename, content);
                // load the application configuration.

                config = await application.LoadApplicationConfiguration(currentFolder + filename, false);


            }
            else
            {
                // load the application configuration.
                config = await application.LoadApplicationConfiguration(false);
            }

            // check the application certificate.
            haveAppCertificate = await application.CheckApplicationInstanceCertificate(false, 0);
            config.ApplicationName = "OPC UA Sample Client";
            if (haveAppCertificate)
            {

                config.ApplicationUri = Utils.GetApplicationUriFromCertificate(config.SecurityConfiguration.ApplicationCertificate.Certificate);

                config.CertificateValidator.CertificateValidation += new CertificateValidationEventHandler(CertificateValidator_CertificateValidation);

            }
            else
            {

                //Console.WriteLine("PIPPO: haveappcertificateFalse");
            }
        }

        public async Task<SessionView> CreateSessionChannelAsync(int indexEndpoint)
        {
            SessionView sessionView;

            UserIdentity userI = new UserIdentity(new AnonymousIdentityToken());
            var endpoint = new ConfiguredEndpoint(null, endpoints[indexEndpoint]);

            Console.WriteLine("Prima della creazione");
            session = await Session.Create(config, endpoint, false, "OPC Client", 60000, userI, null);
            if (session == null)
            {
                Console.WriteLine("Creazione sessione fallita: dentro il client");
                sessionView = null;
            }
            else
            {
                Console.WriteLine("Sessione creata: dentro il client");
                EndpointView endpointView = new EndpointView(session.Endpoint.EndpointUrl, session.Endpoint.SecurityMode.ToString(), session.Endpoint.TransportProfileUri, 0);
                sessionView = new SessionView(session.SessionId.Identifier.ToString(), session.SessionId.NamespaceIndex.ToString(), session.SessionName, endpointView);
            }
            return sessionView;

        }

        public async Task<SessionView> CreateSessionChannelAsync(int indexEndpoint, string username, string password)
        {

            SessionView sessionView;
            UserIdentity userI = new UserIdentity(username, password);
            var endpoint = new ConfiguredEndpoint(null, endpoints[indexEndpoint]);
            Console.WriteLine("Prima della creazione");
            try
            {
                session = await Session.Create(config, endpoint, false, "OPC Client", 60000, userI, null);

                if (session == null)
                {
                    Console.WriteLine("Creazione sessione fallita: dentro il client");
                    sessionView = null;
                }
                else
                {
                    Console.WriteLine("Sessione creata: dentro il client");
                    EndpointView endpointView = new EndpointView(session.Endpoint.EndpointUrl, session.Endpoint.SecurityMode.ToString(), session.Endpoint.TransportProfileUri, 0);
                    sessionView = new SessionView(session.SessionId.Identifier.ToString(), session.SessionId.NamespaceIndex.ToString(), session.SessionName, endpointView);
                }

                return sessionView;
            }
            catch (ServiceResultException p)
            {
                Console.WriteLine("Sono dentro l'eccezione client");
                throw new BadUserException("Username or Password wrong!", p);

            }

        }

        /*TimestampsToReturn*/
        public List<NodeView> readVariable(string identifier, ushort namespaceIndex, Double maxAge, int timestamp, uint attribute)
        {
            NodeId node = new NodeId(identifier, namespaceIndex);
            List<NodeView> nodesRead = new List<NodeView>();

            DataValueCollection results = null;
            DiagnosticInfoCollection diagnosticInfos = null;
            ReadValueId nodeToRead = new ReadValueId
            {

                NodeId = node,
                AttributeId = attribute,
                IndexRange = null, // Da aggiungere al metodo dopo 
                DataEncoding = null // da aggiungere al metodo dopo
            };

            ReadValueIdCollection nodesToRead = new ReadValueIdCollection();
            nodesToRead.Add(nodeToRead);
            TimestampsToReturn t = TimestampsToReturn.Invalid;
            switch (timestamp)
            {
                case 0:
                    t = TimestampsToReturn.Source;
                    break;
                case 1:
                    t = TimestampsToReturn.Server;
                    break;
                case 2:
                    t = TimestampsToReturn.Both;
                    break;
                case 3:
                    t = TimestampsToReturn.Neither;
                    break;
            }
            try
            {
                session.Read(null, maxAge, t, nodesToRead, out results, out diagnosticInfos);

                foreach (DataValue result in results)
                {

                    nodesRead.Add(new NodeView(result.Value.ToString(), result.StatusCode.ToString(), result.SourceTimestamp.ToString(), result.ServerTimestamp.ToString()));
                }
            }
            catch (NullReferenceException p)
            {

                throw new NoNodeToReadException("Node not found", p);

            }


            return nodesRead;
        }

        public List<NodeView> readVariable(UInt32 identifier, ushort namespaceIndex, Double maxAge, int timestamp, uint attribute)
        {
            NodeId node = new NodeId(identifier, namespaceIndex);
            List<NodeView> nodesRead = new List<NodeView>();

            DataValueCollection results = null;
            DiagnosticInfoCollection diagnosticInfos = null;
            ReadValueId nodeToRead = new ReadValueId
            {

                NodeId = node,
                AttributeId = attribute,
                IndexRange = null, // Da aggiungere al metodo dopo 
                DataEncoding = null // da aggiungere al metodo dopo
            };

            ReadValueIdCollection nodesToRead = new ReadValueIdCollection();
            nodesToRead.Add(nodeToRead);
            TimestampsToReturn t = TimestampsToReturn.Invalid;
            switch (timestamp)
            {
                case 0:
                    t = TimestampsToReturn.Source;
                    break;
                case 1:
                    t = TimestampsToReturn.Server;
                    break;
                case 2:
                    t = TimestampsToReturn.Both;
                    break;
                case 3:
                    t = TimestampsToReturn.Neither;
                    break;
            }
            try
            {
                session.Read(null, maxAge, t, nodesToRead, out results, out diagnosticInfos);
                string reset = "--/--/---- --:--:--";
                foreach (DataValue result in results)
                {
                    switch (timestamp)
                    {
                        case 0:

                            nodesRead.Add(new NodeView(result.Value.ToString(), result.StatusCode.ToString(), result.SourceTimestamp.ToString(), reset));

                            break;
                        case 1:
                            nodesRead.Add(new NodeView(result.Value.ToString(), result.StatusCode.ToString(), reset, result.ServerTimestamp.ToString()));

                            break;
                        case 2:
                            nodesRead.Add(new NodeView(result.Value.ToString(), result.StatusCode.ToString(), result.SourceTimestamp.ToString(), result.ServerTimestamp.ToString()));
                            t = TimestampsToReturn.Both;
                            break;
                        case 3:
                            nodesRead.Add(new NodeView(result.Value.ToString(), result.StatusCode.ToString(), reset, reset));
                            t = TimestampsToReturn.Neither;
                            break;
                    }


                }
            }
            catch (NullReferenceException p)
            {

                throw new NoNodeToReadException("Node not found", p);

            }


            return nodesRead;
        }

        public List<String> WriteVariable(int typeId, string identifier, ushort namespaceIndex, Object value, uint attribute)
        {
            NodeId node = null;
            List<String> statusCodeWrite = new List<String>();

            if (typeId == 0)
            {
                uint id;
                try
                {
                    id = Convert.ToUInt32(identifier);
                    node = new NodeId(id, namespaceIndex);
                }
                catch (FormatException p)
                {
                    throw new FormatException("Invalid Node ID Format", p);
                }


            }
            else if (typeId == 1)
            {
                node = new NodeId(identifier, namespaceIndex);
            }

            DataValue valueToWrite = new DataValue()
            {
                Value = (new Variant(value))

            };

            DiagnosticInfoCollection diagnosticInfos = null;
            WriteValueCollection nodesTowrite = new WriteValueCollection();

            WriteValue nodeToWrite = new WriteValue()
            {

                NodeId = node,
                AttributeId = attribute,
                Value = valueToWrite,
                IndexRange = null // da aggiungere al metodo dopo
            };

            nodesTowrite.Add(nodeToWrite);
            StatusCodeCollection writeResults;
            session.Write(null, nodesTowrite, out writeResults, out diagnosticInfos);
            for (int i = 0; i < writeResults.Count; i++)
            {
                Console.WriteLine("-----------------------------------------");
                Console.WriteLine(writeResults[i].ToString());
                statusCodeWrite.Add(writeResults[i].ToString());
            }
            return statusCodeWrite;
        }

        private void CertificateValidator_CertificateValidation(CertificateValidator validator, CertificateValidationEventArgs e)
        {
            if (e.Error.StatusCode == StatusCodes.BadCertificateUntrusted)
            {
                e.Accept = config.SecurityConfiguration.AutoAcceptUntrustedCertificates;
                if (config.SecurityConfiguration.AutoAcceptUntrustedCertificates)
                {
                    Console.WriteLine("Accepted Certificate: " + e.Certificate.Subject.ToString());
                }
                else
                {
                    Console.WriteLine("Rejected Certificate: " + e.Certificate.Subject.ToString());
                }
            }
        }

        //Create subscriptions
        public SubscriptionView CreateSub(double requestedPublishingInterval, uint requestedLifeTimeCount, uint requestedKeepAliveCount, uint MaxNotificationPerPublish, bool _PublishingEnabled, byte Priority)
        {
            SubscriptionView sub;

            Subscription subscription = new Subscription()
            {
                KeepAliveCount = requestedKeepAliveCount,
                LifetimeCount = requestedLifeTimeCount,
                MaxNotificationsPerPublish = MaxNotificationPerPublish,
                Priority = Priority,
                PublishingInterval = Convert.ToInt32(requestedPublishingInterval),
                PublishingEnabled = _PublishingEnabled
            };

            //Aggiunge la subscription al campo subscriptions della sessione
            session.AddSubscription(subscription);

            /*
             * La create comunica con il server e crea effettivamente la subscription, salvando nei campi current
             * i valori revised (PublishingInterval, KeepAliveCount, LifetimeCount) 
            */
            subscription.Create();

            sub = new SubscriptionView(subscription.Id, subscription.PublishingInterval, subscription.LifetimeCount, subscription.KeepAliveCount, subscription.MaxNotificationsPerPublish, subscription.PublishingEnabled, subscription.Priority);

            return sub;
        }

        public List<SubscriptionView> GetSubscriptionViews()
        {
            List<SubscriptionView> listSubView = new List<SubscriptionView>();
            IEnumerable<Subscription> collectionSubscription = session.Subscriptions;

            foreach (Subscription sub in collectionSubscription)
            {
                listSubView.Add(new SubscriptionView(sub.Id, sub.CurrentPublishingInterval, sub.CurrentLifetimeCount, sub.CurrentKeepAliveCount, sub.MaxNotificationsPerPublish, sub.PublishingEnabled, sub.Priority));
            }
            return listSubView;
        }

        //Funzione che permette di recuperare la subscription a cui bisogna aggiungere il monitoredItem
        public Subscription GetSubscription(uint subscriptionId)
        {
            foreach (Subscription sub in session.Subscriptions)
            {
                if (sub.Id == subscriptionId)
                    return sub;
            }
            return null;
        }

        // string nodeClass: Parametro per gestire MonitoredItem con NodeClass diversa da Variable
        public MonitoredItemView CreateMonitoredItem(uint subscriptionId, int typeId, ushort namespaceIndex, string identifierNode, int samplingInterval, bool discardOldest, uint queueSize, int monitoringMode, int filterTrigger, uint deadbandType, double deadbandValue)
        {

            Subscription sub = GetSubscription(subscriptionId);

            //Initialize Filter Parameters
            DataChangeTrigger _trigger;
            string filterTriggerView;
            switch (filterTrigger)
            {
                case 0:
                    _trigger = DataChangeTrigger.Status;
                    filterTriggerView = "Status";
                    break;
                case 1:
                    _trigger = DataChangeTrigger.StatusValue;
                    filterTriggerView = "StatusValue";
                    break;
                case 2:
                    _trigger = DataChangeTrigger.StatusValueTimestamp;
                    filterTriggerView = "StatusValueTimestamp";
                    break;
                default:
                    _trigger = DataChangeTrigger.StatusValue;
                    filterTriggerView = "StatusValue";
                    break;
            }

            string deadbandTypeView;
            switch (deadbandType)
            {
                case 0:
                    deadbandTypeView = "None";
                    break;
                case 1:
                    deadbandTypeView = "Absolute";
                    break;
                case 2:
                    deadbandTypeView = "Percent";
                    break;
                default:
                    deadbandTypeView = null;
                    break;

            }

            DataChangeFilter filter = new DataChangeFilter()
            {
                Trigger = _trigger,
                DeadbandType = deadbandType,
                DeadbandValue = deadbandValue
            };

            //Initialize Monitored Item Parameters
            MonitoringMode _monitoringMode;
            switch (monitoringMode)
            {
                case 0:
                    _monitoringMode = MonitoringMode.Disabled;
                    break;
                case 1:
                    _monitoringMode = MonitoringMode.Sampling;
                    break;
                case 2:
                    _monitoringMode = MonitoringMode.Reporting;
                    break;
                default:
                    _monitoringMode = MonitoringMode.Reporting;
                    break;

            }

            //Set NodeId della variabile che si vuole leggere con gestione dell'identifier sia string che integer
            NodeId node = null;
            if (typeId == 0)
            {
                uint id;
                try
                {
                    id = Convert.ToUInt32(identifierNode);
                    node = new NodeId(id, namespaceIndex);
                }
                catch (FormatException p)
                {
                    throw new FormatException("Invalid Node ID Format", p);
                }


            }
            else if (typeId == 1)
            {
                node = new NodeId(identifierNode, namespaceIndex);
            }


            MonitoredItem monitoredItem = new MonitoredItem(clientHandle)
            {
                AttributeId = Attributes.Value,
                DiscardOldest = discardOldest,
                Filter = filter,
                MonitoringMode = _monitoringMode,
                NodeClass = NodeClass.Variable,
                QueueSize = queueSize,
                SamplingInterval = samplingInterval,
                StartNodeId = node
            };
            clientHandle++; //Identifier di un singolo monitored item --> univoco solo all'interno della subscription

            monitoredItem.Notification += OnNotificationItem;

            //Aggiunge l'item tra i monitored items della subscription senza crearlo

            sub.AddItem(monitoredItem);

            //Se aggiungiamo altri monitoredItem la funzione successiva li creerà tutti

            //Comunica con il server e crea effettivamente il monitoredItem
            IList<MonitoredItem> createdMonitoredItems = sub.CreateItems();
            //Questa funzione ritorna la lista dei monitoredItems creati al momento della chiamata

            return new MonitoredItemView(monitoredItem.ClientHandle, monitoredItem.ResolvedNodeId.NamespaceIndex, monitoredItem.ResolvedNodeId.Identifier.ToString(), subscriptionId, monitoredItem.SamplingInterval, filterTriggerView, deadbandTypeView, deadbandValue);
        }

        public List<MonitoredItemView> GetMonitoredItemViews(uint subscriptionId)
        {
            List<MonitoredItemView> ItemViews = new List<MonitoredItemView>();

            Subscription sub = GetSubscription(subscriptionId);
            IEnumerable<MonitoredItem> monitoredItems = sub.MonitoredItems;

            foreach (MonitoredItem monitoredItem in monitoredItems)
            {
                DataChangeFilter filter = (DataChangeFilter)monitoredItem.Filter;
                string filterTriggerView;
                switch (filter.Trigger)
                {
                    case DataChangeTrigger.Status:
                        filterTriggerView = "Status";
                        break;
                    case DataChangeTrigger.StatusValue:
                        filterTriggerView = "StatusValue";
                        break;
                    case DataChangeTrigger.StatusValueTimestamp:
                        filterTriggerView = "StatusValueTimestamp";
                        break;
                    default:
                        filterTriggerView = "StatusValue";
                        break;
                }

                string deadbandTypeView;
                switch (filter.DeadbandType)
                {
                    case 0:
                        deadbandTypeView = "None";
                        break;
                    case 1:
                        deadbandTypeView = "Absolute";
                        break;
                    case 2:
                        deadbandTypeView = "Percent";
                        break;
                    default:
                        deadbandTypeView = null;
                        break;

                }

                ItemViews.Add(
                    new MonitoredItemView(monitoredItem.ClientHandle, monitoredItem.ResolvedNodeId.NamespaceIndex, monitoredItem.ResolvedNodeId.Identifier.ToString(), subscriptionId, monitoredItem.SamplingInterval, filterTriggerView, deadbandTypeView, filter.DeadbandValue)
                    );
            }

            return ItemViews;
        }

        public SubscriptionView GetSubscriptionViewById(uint subscriptionId)
        {
            Subscription sub = GetSubscription(subscriptionId);
            return new SubscriptionView(sub.Id, sub.PublishingInterval, sub.LifetimeCount, sub.KeepAliveCount, sub.MaxNotificationsPerPublish, sub.PublishingEnabled, sub.Priority);
        }

        private void OnNotificationItem(MonitoredItem item, MonitoredItemNotificationEventArgs e)
        {
            DataChangeView view = IsCreatedDataChangeView(item.ClientHandle);
            if (view == null)
            {
                view = new DataChangeView(item.ClientHandle);
                dataChangeViews.Add(view);
            }

            string message = "update: " + item.ClientHandle;
            foreach (var value in item.DequeueValues())
            {

                Console.WriteLine("{0}: {1}, {2}, {3}", item.DisplayName, value.Value, value.SourceTimestamp, value.StatusCode);
                view.DisplayName = item.DisplayName;
                view.Value = value.Value.ToString();
                view.SourceTimestamp = value.SourceTimestamp.ToString();
                view.ServerTimestamp = value.ServerTimestamp.ToString();
                view.StatusCode = value.StatusCode.ToString();
                MessagingCenter.Send<ClientOPC, DataChangeView>(this, message, view);
                Console.WriteLine("ho inviato la view");
            }
            UpdateDataChangeView(view);


        }

        public DataChangeView IsCreatedDataChangeView(uint clientHandle)
        {
            foreach (DataChangeView view in dataChangeViews)
            {
                if (view.ClientHandle == clientHandle)
                    return view;
            }
            return null;
        }


        private void UpdateDataChangeView(DataChangeView view)
        {
            foreach (DataChangeView v in dataChangeViews)
            {
                if (v.ClientHandle == view.ClientHandle)
                {
                    dataChangeViews.Remove(v);
                    dataChangeViews.Add(view);
                }
            }

        }
    }
}


