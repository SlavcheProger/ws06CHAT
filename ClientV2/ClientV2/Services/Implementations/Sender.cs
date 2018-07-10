using Chat.Socket.ClientV2.Services.Interfaces;
using Chat.Socket.ClientV2.Source;
using Newtonsoft.Json;
using System;
using System.Text;

namespace Chat.Socket.ClientV2.Services.Implementations
{
    class Sender : ISender
    {
        private ISender self;

        public Sender()
        {
            self = this as ISender;
        }


        void ISender.Launch(Object obj)
        {
            var jsonString = JsonConvert.SerializeObject(obj);
            var data = Encoding.UTF8.GetBytes(jsonString);

            self.Send(data);
        }

        void ISender.Send(byte[] data) => ServerConnection.Stream.Write(data, 0, data.Length);
    }
}
