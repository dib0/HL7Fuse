#if NET8_0_OR_GREATER
using HL7Fuse.Logging;
using Microsoft.Extensions.Hosting;
using SuperSocket.SocketBase;
using SuperSocket.SocketEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hl7Fuse
{
    public sealed class HL7BackgroundService : BackgroundService
    {
        IBootstrap m_Bootstrap;
        public HL7BackgroundService()
            :base()
        {

        }
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            Logger.Debug("HL7Fuse StartAsync");
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 100;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.
            if (!cancellationToken.IsCancellationRequested)
            {
                Logger.Debug("HL7Fuse CreateBootstrap");
                m_Bootstrap = BootstrapFactory.CreateBootstrap();

                if (m_Bootstrap==null && !m_Bootstrap.Initialize())
                    Logger.Error("Failed to initialize SuperSocket!");
                else
                {

                    var result = m_Bootstrap.Start();
                    switch (result)
                    {
                        case (StartResult.None):
                            Logger.Warn("No server is configured, please check you configuration!");
                            break;

                        case (StartResult.Success):
                            Logger.Info("The server has been started!");
                            break;

                        case (StartResult.Failed):
                            Logger.Error("Failed to start SuperSocket server! Please check error log for more information!");
                            break;

                        case (StartResult.PartialSuccess):
                            Logger.Warn("Some server instances were started successfully, but the others failed to start! Please check error log for more information!");
                            break;
                    }

                }
            }
            Logger.Debug("HL7Fuse StartAsync ending...");
            return base.StartAsync(cancellationToken);
        }
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            Logger.Debug("HL7Fuse service is stopping");
            m_Bootstrap?.Stop();
            Logger.Debug("HL7Fuse has stopped"); 
            await base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    //string joke = jokeService.GetJoke();
                    Logger.DebugFormat("HL7BackgroundService is running");
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                // When the stopping token is canceled, for example, a call made from services.msc,
                // we shouldn't exit with a non-zero exit code. In other words, this is expected...
            }
            catch (Exception ex)
            {
                Logger.Error($"HL7BackgroundService error:{ex.Message}");

                // Terminates this process and returns an exit code to the operating system.
                // This is required to avoid the 'BackgroundServiceExceptionBehavior', which
                // performs one of two scenarios:
                // 1. When set to "Ignore": will do nothing at all, errors cause zombie services.
                // 2. When set to "StopHost": will cleanly stop the host, and log errors.
                //
                // In order for the Windows Service Management system to leverage configured
                // recovery options, we need to terminate the process with a non-zero exit code.
                Environment.Exit(1);
            }
        }
    }
}
#endif