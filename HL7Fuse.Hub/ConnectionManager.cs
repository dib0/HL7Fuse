using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HL7Fuse.Hub.Configuration;
using HL7Fuse.Hub.EndPoints;
using HL7Fuse.Logging;
using NHapi.Base.Model;
using SuperSocket.SocketBase.Logging;

namespace HL7Fuse.Hub
{
    internal class ConnectionManager
    {
        #region Private properties
        private static ConnectionManager self;
        private Dictionary<string, IEndPoint> clients;
        private Queue<IMessage> queue;
        private List<RoutingRuleSet> routingRules;
        private ThreadStart queueThreadStart = null;
        private Thread queueThread=null;
        private int retrySleep, retryCount;
        #endregion

        #region Public properties
        public static ConnectionManager Instance
        {
            get
            {
                if (self == null)
                    self = new ConnectionManager();

                return self;
            }
        }
        #endregion

        #region Constructor
        private ConnectionManager()
        {
            queue = new Queue<IMessage>();

            LoadEndPoints();
            LoadRoutingRules();

            // Start queue thread
            queueThreadStart = new ThreadStart(HandleQueue);
            queueThread = new Thread(queueThreadStart);

            // Load other config values
            if (!int.TryParse("SendRetryPause", out retrySleep))
                retrySleep = 1000;
            if (!int.TryParse("SendRetryCount", out retryCount))
                retryCount = 10;
        }
        #endregion

        #region Public methods
        public void SendMessage(IMessage message)
        {
            lock (self)
                queue.Enqueue(message);

            if (!queueThread.IsAlive)
            {
                queueThread = new Thread(queueThreadStart);
                queueThread.Start();
            }
        }
        #endregion

        #region Private methods
        private void LoadEndPoints()
        {
            clients = (Dictionary<string, IEndPoint>) ConfigurationManager.GetSection("endpoints");
        }

        private void LoadRoutingRules()
        {
            routingRules = (List<RoutingRuleSet>)ConfigurationManager.GetSection("messageRouting");
        }

        private void HandleQueue()
        {
            Logger.Debug("Processing queue.");
            while (queue.Count > 0)
            {
                IMessage item = null;
                lock (self)
                    item = queue.Dequeue();
                Logger.InfoFormat("Processing {0} message.", item.GetStructureName());
                
//TODO: add IMessageHandler handling

                // Send the message to the relevant end points
                Dictionary<string, IEndPoint> endpoints = GetRelevantEndPoints(item);
                // Handle all the endpoint parallel, but keep the message queue synchronised over all
                // end points.
                Parallel.For(0, endpoints.Keys.Count, (i) =>
                {
                    IEndPoint endp = endpoints[endpoints.Keys.ElementAt(i)];

                    try
                    {
                        bool result = false;
                        int tries = 0;
                        while (!result && (tries < retryCount))
                        {
                            tries++;
                            result = endp.Send(item);

                            if (!result)
                            {
                                Logger.InfoFormat("Couldn't deliver message to endpoint '{0}'. Message: {1}. Retrying after sleep.", endpoints.Keys.ElementAt(i), item.GetStructureName());

                                Thread.Sleep(retrySleep);
                            }
                        }

                        if (!result)
                            Logger.ErrorFormat("Couldn't deliver message to endpoint '{0}'. Message: {1}. Stopping to try.", endpoints.Keys.ElementAt(i), item.GetStructureName());
                    }
                    catch (Exception e)
                    {
                        Console.Write(e.Message);
                    }
                });
            }
            Logger.Debug("Queue done.");
        }

        private Dictionary<string, IEndPoint> GetRelevantEndPoints(IMessage message)
        {
            // Filter the messages to get the relevant endpoints
            Dictionary<string, IEndPoint> endpoints = new Dictionary<string, IEndPoint>();
            foreach (RoutingRuleSet rule in routingRules)
            {
                if (rule.IncludeEndpoint(message))
                {
                    KeyValuePair<string, IEndPoint> endp = clients.FirstOrDefault(ep => ep.Key == rule.EndPoint);
                    if (endp.Key != null)
                    {
                        Logger.DebugFormat("Endpoint '{0}' is relevant for '{1}'.", endp.Key, message.GetStructureName());
                        endpoints.Add(endp.Key, endp.Value);
                    }
                }
                else
                    Logger.DebugFormat("Endpoint '{0}' is not relevant for '{1}'.", rule.EndPoint, message.GetStructureName());
            }

            return endpoints;
        }
        #endregion
    }
}
