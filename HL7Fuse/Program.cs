﻿// Type: SuperSocket.SocketService.Program
// Assembly: SuperSocket.SocketService, Version=1.6.1.0, Culture=neutral, PublicKeyToken=6c80000676988ebb
// MVID: 9C172A0A-F91F-4CC9-B921-9FE0407DCF07
// Assembly location: C:\Source\HL7Fuse\Solution Items\SuperSocket\SuperSocket.SocketService.exe

using SuperSocket.Common;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketEngine;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
#if NET48
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
#endif
using System.ServiceProcess;
using HL7Fuse.Services;
using System.Diagnostics;
using HL7Fuse.Logging;
#if NET8_0_OR_GREATER
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Hl7Fuse;
using System.ComponentModel;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Runtime.InteropServices;
using System.Data.Common;
#endif


namespace HL7Fuse
{
    internal static class Program
    {
        private static Dictionary<string, ControlCommand> m_CommandHandlers = new Dictionary<string, ControlCommand>((IEqualityComparer<string>)StringComparer.OrdinalIgnoreCase);
        private static bool setConsoleColor;

        static Program()
        {
        }

        private static void Main(string[] args)
        {
#if NET8_0_OR_GREATER
            AppDomain.CurrentDomain.AssemblyResolve += (sender, asm) =>
            {
                Assembly assembly = null;
                var dirs = Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory).Concat(Directory.GetDirectories(Directory.GetCurrentDirectory())).ToList();
                dirs.Insert(0, Directory.GetCurrentDirectory());
                dirs.Insert(0, AppDomain.CurrentDomain.BaseDirectory);

                foreach (var p in dirs.Select(i => Path.Combine(i, new AssemblyName(asm.Name).Name + ".dll")).Where(i => File.Exists(i)))
                    try
                    {
                        assembly = Assembly.LoadFrom(p);
                        //20250509 is not needed
                        //if (asm.Name.StartsWith("Microsoft.Data.SqlClient"))
                        //{
                        //    var type = assembly.GetType(new AssemblyName(asm.Name).Name + ".SqlClientFactory");
                        //    System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(type.TypeHandle);
                        //    var ci = type.GetProperties(BindingFlags.Static | BindingFlags.Public);
                        //}
                        //20250509 is not needed
                        break;
                    }
                    catch (Exception ex) { Logger.Error(ex.Message, ex); }

                return assembly;
            };
            //DbProviderFactories.RegisterFactory("System.Data.SqlClient", typeof(System.Data.SqlClient.SqlClientFactory));
            //bool bExist2 = DbProviderFactories.GetFactoryClasses().Rows.Find("System.Data.SqlClient") != null;
            //var fact = DbProviderFactories.GetFactory("System.Data.SqlClient");
            //Assembly ass0 = System.Reflection.Assembly.LoadFrom(@"InITeC.Mdl.Integration.HL7.dll");
            //Console.WriteLine("Please press a key to start...");
            //System.String exeArgTest = Console.ReadKey().KeyChar.ToString();
            //String serviceName = ConfigurationManager.AppSettings["ServiceName"];
#endif
            if (Platform.IsMono && (int)Path.DirectorySeparatorChar == 47)
                Program.ChangeScriptExecutable();
            if (!Platform.IsMono && !Environment.UserInteractive || Platform.IsMono && !AppDomain.CurrentDomain.FriendlyName.Equals(Path.GetFileName(Assembly.GetEntryAssembly().CodeBase)))
            {
                try
                {
                    Logger.Info("HL7Fuse-Starting");
#if NET48

                    ServiceBase.Run(new ServiceBase[1] { (ServiceBase)new MainService() });
#else
                    HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
                    Logger.Info("HL7Fuse-CreateWebHostBuilder: done");
                    builder.Services.AddWindowsService(options =>
                    {
                        options.ServiceName = ConfigurationManager.AppSettings["ServiceName"];
                    });
                    IHost host = builder.Build();
                    builder.Services.AddHostedService<HL7BackgroundService>();
                    host.Run();
                    Logger.Info("InITeC.Mdl.Synk-End");
#endif
                }
                catch (Exception ex)
                {
                    Logger.Error($"Sever error {ex.Message}");
                    throw;
                }
            }
            else
            {
                string str = string.Empty;
                if (args == null || args.Length < 1)
                {
                    Console.WriteLine("Welcome to HL7Fuse!");
                    Console.WriteLine("Please press a key to continue...");
                    Console.WriteLine("-[r]: Run this application as a console application;");
#if NET48
                    Console.WriteLine("-[i]: Install this application as a Windows Service;");
                    Console.WriteLine("-[u]: Uninstall this Windows Service application;");
#endif
                    string exeArg;
                    do
                    {
                        exeArg = Console.ReadKey().KeyChar.ToString();
                        Console.WriteLine();
                    }
                    while (!Program.Run(exeArg, (string[])null));
                }
                else
                {
                    string exeArg = args[0];
                    if (!string.IsNullOrEmpty(exeArg))
                        exeArg = exeArg.TrimStart(new char[1]
            {
              '-'
            });
                    Program.Run(exeArg, args);
                }
            }
        }

        private static void ChangeScriptExecutable()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "supersocket.sh");
            try
            {
                if (!File.Exists(path))
                    return;
                File.SetAttributes(path, File.GetAttributes(path) | (FileAttributes) (-2147483648));
            }
            catch
            {
            }
        }

        private static bool Run(string exeArg, string[] startArgs)
        {
            switch (exeArg.ToLower())
            {
#if NET48
                case "i":
                    SelfInstaller.InstallMe();
                    return true;
                case "u":
                    SelfInstaller.UninstallMe();
                    return true;
#endif
                case "r":
                    Program.RunAsConsole();
                    return true;
#if NET48
                case "c":
                    Program.RunAsController(startArgs);
                    return true;
#endif
                default:
                    Console.WriteLine("Invalid argument!");
                    return false;
            }
        }

        private static void CheckCanSetConsoleColor()
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.ResetColor();
                Program.setConsoleColor = true;
            }
            catch
            {
                Program.setConsoleColor = false;
            }
        }

        private static void SetConsoleColor(ConsoleColor color)
        {
            if (!Program.setConsoleColor)
                return;
            Console.ForegroundColor = color;
        }

        private static void AddCommand(string name, string description, Func<IBootstrap, string[], bool> handler)
        {
            ControlCommand controlCommand = new ControlCommand()
            {
                Name = name,
                Description = description,
                Handler = handler
            };
            Program.m_CommandHandlers.Add(controlCommand.Name, controlCommand);
        }

        private static void RunAsConsole()
        {
            Console.WriteLine("Welcome to HL7Fuse!");
            Program.CheckCanSetConsoleColor();
            Console.WriteLine("Initializing...");
            IBootstrap bootstrap = BootstrapFactory.CreateBootstrap();
            if (!bootstrap.Initialize())
            {
                Program.SetConsoleColor(ConsoleColor.Red);
                Console.WriteLine("Failed to initialize HL7Fuse! Please check error log for more information!");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("Starting...");

                StartResult startResult = bootstrap.Start();
                Console.WriteLine("-------------------------------------------------------------------");
                foreach (IWorkItem workItem in bootstrap.AppServers)
                {
                    if (workItem.State == ServerState.Running)
                    {
                        Program.SetConsoleColor(ConsoleColor.Green);
                        Console.WriteLine("- {0} has been started", (object)workItem.Name);
                    }
                    else
                    {
                        Program.SetConsoleColor(ConsoleColor.Red);
                        Console.WriteLine("- {0} failed to start", (object)workItem.Name);
                    }
                }
                Console.ResetColor();
                Console.WriteLine("-------------------------------------------------------------------");
                switch (startResult)
                {
                    case StartResult.None:
                        Program.SetConsoleColor(ConsoleColor.Red);
                        Console.WriteLine("No server is configured, please check you configuration!");
                        Console.ReadKey();
                        return;
                    case StartResult.Success:
                        Console.WriteLine("The HL7Fuse has been started!");
                        break;
                    case StartResult.PartialSuccess:
                        Program.SetConsoleColor(ConsoleColor.Red);
                        Console.WriteLine("Some server instances were started successfully, but the others failed! Please check error log for more information!");
                        break;
                    case StartResult.Failed:
                        Program.SetConsoleColor(ConsoleColor.Red);
                        Console.WriteLine("Failed to start the HL7Fuse! Please check error log for more information!");
                        Console.ReadKey();
                        return;
                }
                Console.ResetColor();
                Console.WriteLine("Enter key 'quit' to stop the ServiceEngine.");
                Program.RegisterCommands();
                Program.ReadConsoleCommand(bootstrap);
                bootstrap.Stop();
                Console.WriteLine("The HL7Fuse has been stopped!");
            }
        }

        private static void RegisterCommands()
        {
            Program.AddCommand("List", "List all server instances", new Func<IBootstrap, string[], bool>(Program.ListCommand));
            Program.AddCommand("Start", "Start a server instance: Start {ServerName}", new Func<IBootstrap, string[], bool>(Program.StartCommand));
            Program.AddCommand("Stop", "Stop a server instance: Stop {ServerName}", new Func<IBootstrap, string[], bool>(Program.StopCommand));
        }
#if NET48
        private static void RunAsController(string[] arguments)
        {
            if (arguments == null || arguments.Length < 2)
            {
                Console.WriteLine("Invalid arguments!");
            }
            else
            {
                IConfigurationSource configurationSource = ConfigurationManager.GetSection("superSocket") as IConfigurationSource;
                if (configurationSource == null)
                {
                    Console.WriteLine("SuperSocket configiration is required!");
                }
                else
                {
                    ChannelServices.RegisterChannel((IChannel)new IpcClientChannel(), false);
                    IBootstrap bootstrap = (IBootstrap)null;
                    try
                    {
                        bootstrap = (IBootstrap)Activator.GetObject(typeof(IBootstrap), string.Format("ipc://SuperSocket.Bootstrap[{0}]/Bootstrap.rem", (object)Math.Abs(AppDomain.CurrentDomain.BaseDirectory.TrimEnd(new char[1]
            {
              Path.DirectorySeparatorChar
            }).GetHashCode())));
                    }
                    catch (RemotingException)
                    {
                        if (configurationSource.Isolation != IsolationMode.Process)
                        {
                            Console.WriteLine("Error: the SuperSocket engine has not been started!");
                            return;
                        }
                    }
                    Program.RegisterCommands();
                    string key = arguments[1];
                    ControlCommand controlCommand;
                    if (!Program.m_CommandHandlers.TryGetValue(key, out controlCommand))
                    {
                        Console.WriteLine("Unknown command");
                    }
                    else
                    {
                        try
                        {
                            if (controlCommand.Handler(bootstrap, Enumerable.ToArray<string>(Enumerable.Skip<string>((IEnumerable<string>)arguments, 1))))
                                Console.WriteLine("Ok");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Failed. " + ex.Message);
                        }
                    }
                }
            }
        }
#endif
        private static bool ListCommand(IBootstrap bootstrap, string[] arguments)
        {
            foreach (IWorkItem workItem in bootstrap.AppServers)
            {
                IProcessServer processServer = workItem as IProcessServer;
                if (processServer != null && processServer.ProcessId > 0)
                    Console.WriteLine("{0}[PID:{1}] - {2}", (object)workItem.Name, (object)processServer.ProcessId, (object)workItem.State);
                else
                    Console.WriteLine("{0} - {1}", (object)workItem.Name, (object)workItem.State);
            }
            return false;
        }

        private static bool StopCommand(IBootstrap bootstrap, string[] arguments)
        {
            string name = arguments[1];
            if (string.IsNullOrEmpty(name))
            {
                Console.WriteLine("Server name is required!");
                return false;
            }
            else
            {
                IWorkItem workItem = Enumerable.FirstOrDefault<IWorkItem>(bootstrap.AppServers, (Func<IWorkItem, bool>)(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase)));
                if (workItem == null)
                {
                    Console.WriteLine("The server was not found!");
                    return false;
                }
                else
                {
                    workItem.Stop();
                    return true;
                }
            }
        }

        private static bool StartCommand(IBootstrap bootstrap, string[] arguments)
        {
            string name = arguments[1];
            if (string.IsNullOrEmpty(name))
            {
                Console.WriteLine("Server name is required!");
                return false;
            }
            else
            {
                IWorkItem workItem = Enumerable.FirstOrDefault<IWorkItem>(bootstrap.AppServers, (Func<IWorkItem, bool>)(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase)));
                if (workItem == null)
                {
                    Console.WriteLine("The server was not found!");
                    return false;
                }
                else
                {
                    workItem.Start();
                    return true;
                }
            }
        }

        private static void ReadConsoleCommand(IBootstrap bootstrap)
        {
            string str = Console.ReadLine();
            if (string.IsNullOrEmpty(str))
            {
                Program.ReadConsoleCommand(bootstrap);
            }
            else
            {
                if ("quit".Equals(str, StringComparison.OrdinalIgnoreCase))
                    return;
                string[] strArray = str.Split(new char[1]
        {
          ' '
        });
                ControlCommand controlCommand;
                if (!Program.m_CommandHandlers.TryGetValue(strArray[0], out controlCommand))
                {
                    Console.WriteLine("Unknown command");
                    Program.ReadConsoleCommand(bootstrap);
                }
                else
                {
                    try
                    {
                        if (controlCommand.Handler(bootstrap, strArray))
                            Console.WriteLine("Ok");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Failed. " + ex.Message + Environment.NewLine + ex.StackTrace);
                    }
                    Program.ReadConsoleCommand(bootstrap);
                }
            }
        }
    }
}
