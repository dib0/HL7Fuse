using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Protocol;
using HL7Fuse.Protocol;
using System.Threading;
using System.IO;

namespace HL7Fuse
{
    public class FileServer : AppServer<FileSession, HL7RequestInfo>
    {
        #region Private properties
        private string incomingDirectory;
        private string outgoingDirectory;
        private string backupDirectory;
        private string errorDirectory;
        private string fileMask;
        private int pollTime;
        private bool isRunning;
        private Thread thread;
        FileSession session;
        HL7RequestInfoParser parser;

        private bool backupFiles
        {
            get
            {
                return !string.IsNullOrEmpty(backupDirectory);
            }
        }

        private bool sendFiles
        {
            get
            {
                return !string.IsNullOrEmpty(outgoingDirectory);
            }
        }
        #endregion

        #region Constructor
        public FileServer() : base (new DefaultReceiveFilterFactory<MLLPBeginEndMarkReceiveFilter, HL7RequestInfo>())
        {
            isRunning = false;

            parser = new HL7RequestInfoParser();
            session = new FileSession();
        }
        #endregion

        #region Overrides
        protected override bool Setup(IRootConfig rootConfig, IServerConfig config)
        {
            incomingDirectory = config.Options["incomingDirectory"];
            backupDirectory = config.Options["backupDirectory"];
            if (!string.IsNullOrEmpty(backupDirectory))
            {
                if (!backupDirectory.EndsWith("/") && !backupDirectory.EndsWith("\\"))
                    backupDirectory += "\\";
            }

            errorDirectory = config.Options["errorDirectory"];
            if (!string.IsNullOrEmpty(errorDirectory))
            {
                if (!errorDirectory.EndsWith("/") && !errorDirectory.EndsWith("\\"))
                    errorDirectory += "\\";
            }

            outgoingDirectory = config.Options["outgoingDirectory"];
            session.OutDirectory = outgoingDirectory;
            
            if (string.IsNullOrEmpty(config.Options["fileMask"]))
                fileMask = "*";
            else
                fileMask = config.Options["fileMask"];

            if (!int.TryParse(config.Options["pollTime"], out pollTime))
                pollTime = 10000;

            // Check configuration
            bool result = true;
            if (string.IsNullOrEmpty(incomingDirectory))
            {
                Logger.Error("When using the FileServer, the incoming directory must be configured.");
                result = false;
            }

            return result && base.Setup(rootConfig, config);
        }

        public override bool Start()
        {
            thread = new Thread(new ThreadStart(WatchDirectory));
            thread.Start();

            return base.Start();
        }

        public override void Stop()
        {
            isRunning = false;
            // Wait for thread to finish
            thread.Join();

            base.Stop();
        }
        #endregion

        #region Methods for filewatching (thread)
        private void WatchDirectory()
        {
            isRunning = true;

            while (isRunning)
            {
                // Check if files are present
                string[] files = Directory.GetFiles(incomingDirectory, fileMask);
                if (files.Count() > 0)
                {
                    // Handle file
                    HandleFile(files[0]);
                }
                else
                    Thread.Sleep(pollTime); // Nothing to do, wait
            }
        }

        private void HandleFile(string path)
        {
            string content = string.Empty;
            using (StreamReader sr = new StreamReader(path))
                content = sr.ReadToEnd();

            // Handle the message
            HL7RequestInfo info = parser.ParseRequestInfo(content, "File");
            ExecuteCommand(session, info);

            // Delete or archive the message
            RemoveFile(path, info);
        }

        private void RemoveFile(string path, HL7RequestInfo requestInfo)
        {
            string toDir = requestInfo.HasError ? errorDirectory : backupDirectory;

            if (requestInfo.HasError || backupFiles)
            {
                FileInfo fInfo = new FileInfo(path);
                File.Move(path, toDir + fInfo.Name);
            }
            else
                File.Delete(path);
        }
        #endregion

        #region Overridden methods
        protected override void ExecuteCommand(FileSession session, HL7RequestInfo requestInfo)
        {
            try
            {
                base.ExecuteCommand(session, requestInfo);

                if (!requestInfo.WasUnknownRequest)
                    session.Send(requestInfo);
            }
            catch (Exception e)
            {
                requestInfo.ErrorMessage = e.Message;
                session.Send(requestInfo);
            }
        }
        #endregion
    }
}
