// Type: SuperSocket.SocketService.ControlCommand
// Assembly: SuperSocket.SocketService, Version=1.6.1.0, Culture=neutral, PublicKeyToken=6c80000676988ebb
// MVID: 9C172A0A-F91F-4CC9-B921-9FE0407DCF07
// Assembly location: C:\Source\HL7Fuse\Solution Items\SuperSocket\SuperSocket.SocketService.exe

using SuperSocket.SocketBase;
using System;

namespace HL7Fuse
{
    internal class ControlCommand
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public Func<IBootstrap, string[], bool> Handler { get; set; }
    }
}
