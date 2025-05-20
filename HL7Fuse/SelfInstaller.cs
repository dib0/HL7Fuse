// Type: SuperSocket.SocketService.SelfInstaller
// Assembly: SuperSocket.SocketService, Version=1.6.1.0, Culture=neutral, PublicKeyToken=6c80000676988ebb
// MVID: 9C172A0A-F91F-4CC9-B921-9FE0407DCF07
// Assembly location: C:\Source\HL7Fuse\Solution Items\SuperSocket\SuperSocket.SocketService.exe
#if NET48
using System.Configuration.Install;
#endif
using System;
using System.Reflection;
using System.Security.Cryptography.Xml;
using System.ServiceProcess;

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
#if NET48
               ManagedInstallerClass.InstallHelper(new string[1]
                    {
                      SelfInstaller._exePath
                    });
#else
                System.Management.Automation.PowerShell ps = System.Management.Automation.PowerShell.Create();
                //ps.AddCommand($"sc create \"{System.Configuration.ConfigurationManager.AppSettings["ServiceName"]}\" binpath= \"dotnet.exe {SelfInstaller._exePath}\"")
                ps.AddCommand("sc")
                    .AddArgument($"create")
                    .AddArgument($"{System.Configuration.ConfigurationManager.AppSettings["ServiceName"]}")
                    .AddParameter("binPath=", $"dotnet.exe {SelfInstaller._exePath}");
                var res = ps.Invoke();
                foreach (var entry in res)
                    Console.WriteLine(entry.ToString());
#endif
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public static bool UninstallMe()
        {
            try
            {
#if NET48
                ManagedInstallerClass.InstallHelper(new string[2]
                {
                  "/u",
                  SelfInstaller._exePath
                });
#else
                System.Management.Automation.PowerShell ps = System.Management.Automation.PowerShell.Create();
                ps.AddCommand("sc")
                    .AddArgument($"delete")
                    .AddArgument($"{System.Configuration.ConfigurationManager.AppSettings["ServiceName"]}");
                var res = ps.Invoke();
                foreach (var entry in res)
                    Console.WriteLine(entry.ToString());
#endif
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
