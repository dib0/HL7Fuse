// Type: SuperSocket.SocketService.SelfInstaller
// Assembly: SuperSocket.SocketService, Version=1.6.1.0, Culture=neutral, PublicKeyToken=6c80000676988ebb
// MVID: 9C172A0A-F91F-4CC9-B921-9FE0407DCF07
// Assembly location: C:\Source\HL7Fuse\Solution Items\SuperSocket\SuperSocket.SocketService.exe

using System.Configuration.Install;
using System.Reflection;

namespace HL7Fuse
{
    public static class SelfInstaller
    {
        private static readonly string _exePath = Assembly.GetExecutingAssembly().Location;

        static SelfInstaller()
        {
        }

        public static bool InstallMe()
        {
            try
            {
                ManagedInstallerClass.InstallHelper(new string[1]
        {
          SelfInstaller._exePath
        });
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool UninstallMe()
        {
            try
            {
                ManagedInstallerClass.InstallHelper(new string[2]
        {
          "/u",
          SelfInstaller._exePath
        });
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
