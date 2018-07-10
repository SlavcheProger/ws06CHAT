using System;
using System.Threading;
using System.Timers;
using Chat.Socket.ClientV2.MessageTypes;
using Chat.Socket.ClientV2.Services.Interfaces;

namespace Chat.Socket.ClientV2.Services.Implementations
{
    class AutoPinger : IAutoPinger
    {
        public bool AutoPingAnswerer { get; set; }
        public bool AutoPongAnswerer { get; set; }

        private IAutoPinger self;
        private readonly ISender sender;
        private readonly IDisconnector disconnector;
        private System.Timers.Timer timer = new System.Timers.Timer();

        public AutoPinger()
        {
            self = this as IAutoPinger;
            sender = DependencyResolver.Get<ISender>();
            disconnector = DependencyResolver.Get<IDisconnector>();
        }

        void IAutoPinger.PingCallbackStart()
        {
            AutoPongAnswerer = false;
            timer.AutoReset = false;
            timer.Interval = 60 * 1000;
            timer.Elapsed += self.PingTimer;
            timer.Enabled = true;
        }

        void IAutoPinger.PongCallback()
        {
            AutoPongAnswerer = true;
            timer.Enabled = false;
        }

        void IAutoPinger.PingTimer(Object obj, ElapsedEventArgs elEvArt)
        {
             disconnector.Disconnect();
        }

        void IAutoPinger.AutoPingStart()
        {
            AutoPingAnswerer = true;
            var timerCallback = new TimerCallback(self.AutoPing);
            var pingTimer = new System.Threading.Timer(timerCallback, null, 15000, 60000);
        }

        void IAutoPinger.AutoPing(object obj)
        {
            if (AutoPingAnswerer)
            {
                var pingObject = new CommandMessage();
                pingObject.Args["ID"] = 1180;
                pingObject.CommandType = CommandType.Ping;
                sender.Launch(pingObject);
                AutoPingAnswerer = false;
            }
            else
            {
                Console.WriteLine(AutoPingAnswerer);
                disconnector.Disconnect();
            }
        }
    }
}
