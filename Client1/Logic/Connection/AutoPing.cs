using Newtonsoft.Json.Linq;
using System;
using System.Net.Sockets;
using System.Timers;

namespace Client1
{
    internal class AutoPing
    {
        private NetworkStream stream { get; set; }
        private IDisconnectable disconnector { get; set; }
        private bool autoPingFlag { get; set; } = true;
        private Timer autoPingTimer { get; set; } = null;

        public AutoPing(IDisconnectable disconnector, NetworkStream stream)
        {
            this.disconnector = disconnector;
            this.stream = stream;
            autoPingTimer = new Timer(5 * 1000);
            autoPingTimer.AutoReset = false;
            autoPingTimer.Elapsed += delegate { AutoPingFunc(); };
        }

        internal void Start()
        {
            autoPingTimer.Start();
        }

        private void AutoPingFunc()
        {
            if (autoPingFlag)
            {
                SendPing();
                Start();
            }
            else if(autoPingTimer.Enabled)
            {
                autoPingTimer.Stop();
                var display = new DisplayMessageService();
                display.Display("Server does not responde.", DisplayMessageType.Error);
                disconnector.Disconnect();
            }
        }

        private void SendPing()
        {
            var sender = new Sender(stream);
            var ping = new CommandMessage();
            ping.CommandType = CommandType.Ping;
            var jobject = new JObject();
            jobject["ID"] = new Random().Next(100);
            ping.Args = jobject;
            autoPingFlag = false;
            sender.Send(ping);
        }

        internal void ChangeFlag(bool flag)
        {
            autoPingFlag = flag;
        }

        internal bool GetFlag()
        {
            return autoPingFlag;
        }
        public void Stop()
        {
            autoPingTimer.AutoReset = false;
            autoPingTimer.Stop();
        }
    }
}
