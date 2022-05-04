using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using SuperSocket.SocketBase;
using SuperSocket.SocketEngine;

namespace HL7FuseWorkerRole
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);
        IBootstrap m_Bootstrap;

        public override void Run()
        {
            Trace.TraceInformation("HL7FuseWorkerRole is running");
            try
            {
                this.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }
        public override bool OnStart()
        {
            Trace.TraceInformation("HL7FuseWorkerRole is OnStart");
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 100;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            Trace.TraceInformation("HL7FuseWorkerRole CreateBootstrap");
            m_Bootstrap = BootstrapFactory.CreateBootstrap();

            if (!m_Bootstrap.Initialize(RoleEnvironment.CurrentRoleInstance.InstanceEndpoints.ToDictionary(p => p.Key, p => p.Value.IPEndpoint)))
            {
                Trace.WriteLine("Failed to initialize SuperSocket!", "Error");
                return false;
            }

            var result = m_Bootstrap.Start();

            switch (result)
            {
                case (StartResult.None):
                    Trace.WriteLine("No server is configured, please check you configuration!");
                    return false;

                case (StartResult.Success):
                    Trace.WriteLine("The server has been started!");
                    break;

                case (StartResult.Failed):
                    Trace.WriteLine("Failed to start SuperSocket server! Please check error log for more information!");
                    return false;

                case (StartResult.PartialSuccess):
                    Trace.WriteLine("Some server instances were started successfully, but the others failed to start! Please check error log for more information!");
                    break;
            }

            return base.OnStart();
        }

        public override void OnStop()
        {
            Trace.TraceInformation("HL7FuseWorkerRole is stopping");
            m_Bootstrap.Stop();
            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("HL7FuseWorkerRole has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following with your own logic.
            while (!cancellationToken.IsCancellationRequested)
            {
                Trace.TraceInformation("Working");
                await Task.Delay(1000);
            }
        }
    }
}
