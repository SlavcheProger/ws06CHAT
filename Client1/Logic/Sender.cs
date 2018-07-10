using Newtonsoft.Json;
using System.Text;
using System.Net.Sockets;
using System;

namespace Client1
{
    internal class Sender
    {
        private NetworkStream stream;
        internal Sender(NetworkStream stream)
        {
            this.stream = stream;
        }
        internal void Send(object obj)
        {
            if (obj != null)
            {
                try
                {
                    var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));
                    stream.Write(data, 0, data.Length);
                }
                catch(Exception exception)
                {
                    var log = new Logger();
                    log.SaveLog(exception);
                }
            }
        }
    }
}
