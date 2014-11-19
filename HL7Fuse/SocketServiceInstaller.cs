// Type: SuperSocket.SocketService.SocketServiceInstaller
// Assembly: SuperSocket.SocketService, Version=1.6.1.0, Culture=neutral, PublicKeyToken=6c80000676988ebb
// MVID: 9C172A0A-F91F-4CC9-B921-9FE0407DCF07
// Assembly location: C:\Source\HL7Fuse\Solution Items\SuperSocket\SuperSocket.SocketService.exe

using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Configuration.Install;
using System.ServiceProcess;

namespace HL7Fuse
{
    [RunInstaller(true)]
    public class SocketServiceInstaller : Installer
    {
        private IContainer components = (IContainer)null;
        private ServiceInstaller serviceInstaller;
        private ServiceProcessInstaller processInstaller;

        public SocketServiceInstaller()
        {
            this.InitializeComponent();
            this.processInstaller = new ServiceProcessInstaller();
            this.serviceInstaller = new ServiceInstaller();
            this.processInstaller.Account = ServiceAccount.LocalSystem;
            this.serviceInstaller.StartType = ServiceStartMode.Automatic;
            this.serviceInstaller.ServiceName = ConfigurationManager.AppSettings["ServiceName"];
            string str1 = ConfigurationManager.AppSettings["ServiceDescription"];
            if (!string.IsNullOrEmpty(str1))
                this.serviceInstaller.Description = str1;
            List<string> list = new List<string>()
      {
        "tcpip"
      };
            string str2 = ConfigurationManager.AppSettings["ServicesDependedOn"];
            if (!string.IsNullOrEmpty(str2))
                list.AddRange((IEnumerable<string>)str2.Split(new char[2]
        {
          ',',
          ';'
        }));
            this.serviceInstaller.ServicesDependedOn = list.ToArray();
            this.Installers.Add((Installer)this.serviceInstaller);
            this.Installers.Add((Installer)this.processInstaller);
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
        }
    }
}
