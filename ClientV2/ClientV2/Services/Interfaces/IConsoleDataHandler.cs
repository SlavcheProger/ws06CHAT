using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chat.Socket.ClientV2.Services.Interfaces
{
    interface IConsoleDataHandler
    {
        Thread Thread { get; set; }

        void DataHandlerForSending();

        void Sorter(string[] splittedString);

        void SetUserName(string[] wordArray);

        void Here();

        void Ping();

        void SendFile(string[] splittedString);

        void CommonMessage(string message);

    }
}
