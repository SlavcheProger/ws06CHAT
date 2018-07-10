using Chat.Socket.ClientV2.MessageTypes;
using System;

namespace Chat.Socket.ClientV2.Services.Interfaces
{
    interface ISender
    {
        void Launch(Object obj);

        void Send(byte[] data);
    }
}
