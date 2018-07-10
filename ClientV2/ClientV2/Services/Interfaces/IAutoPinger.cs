using System;
using System.Timers;

namespace Chat.Socket.ClientV2.Services.Interfaces
{
    interface IAutoPinger
    {
        bool AutoPingAnswerer { get; set; }
        bool AutoPongAnswerer { get; set; }

        void PingCallbackStart();

        void PongCallback();

        void PingTimer(Object obj, ElapsedEventArgs elEvArt);

        void AutoPingStart();

        void AutoPing(object obj);
    }
}
