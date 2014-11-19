// Type: SuperSocket.SocketService.MainService
// Assembly: SuperSocket.SocketService, Version=1.6.1.0, Culture=neutral, PublicKeyToken=6c80000676988ebb
// MVID: 9C172A0A-F91F-4CC9-B921-9FE0407DCF07
// Assembly location: C:\Source\HL7Fuse\Solution Items\SuperSocket\SuperSocket.SocketService.exe

using SuperSocket.SocketBase;
using SuperSocket.SocketEngine;
using System.ComponentModel;
using System.Configuration;
using System.ServiceProcess;

namespace HL7Fuse
{
    internal class MainService : ServiceBase
    {
        private IContainer components = (IContainer)null;
        private IBootstrap m_Bootstrap;

        public MainService()
        {
            this.InitializeComponent();
            this.m_Bootstrap = BootstrapFactory.CreateBootstrap();
        }

        protected override void OnStart(string[] args)
        {
            if (!this.m_Bootstrap.Initialize())
                return;
            int num = (int)this.m_Bootstrap.Start();
        }

        protected override void OnStop()
        {
            this.m_Bootstrap.Stop();
            base.OnStop();
        }

        protected override void OnShutdown()
        {
            this.m_Bootstrap.Stop();
            base.OnShutdown();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
                this.components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = (IContainer)new Container();
            this.ServiceName = ConfigurationManager.AppSettings["ServiceName"];
        }
    }
}
