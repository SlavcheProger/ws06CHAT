using System;
using System.IO;
using System.Windows.Forms;
using Chat.Socket.ClientV2.MessageTypes;
using Chat.Socket.ClientV2.Services.Interfaces;
using Chat.Socket.ClientV2.Services;

namespace Chat.Socket.ClientV2.Source
{
    internal class FileHandler
    {
        private static ISender sender;

        internal static void SendFile(CommandMessage commandMessage)
        {
            sender = DependencyResolver.Get<ISender>();
            GetInfoFromFile(commandMessage);
        }

        private static void GetInfoFromFile(CommandMessage commandMessage)
        {
            commandMessage.CommandType = CommandType.Sendfile;
            var ofd = new OpenFileDialog { Multiselect = false };

            if (DialogResult.OK == (new Invoker(ofd).Invoke()))
            {
                var FileName = ofd.FileName;
                var byteData = File.ReadAllBytes(FileName);
                var byteMessage = Convert.ToBase64String(byteData);

                commandMessage.Args["File"] = byteMessage;

                var wordArray = FileName.Split('.');

                commandMessage.Args["Extension"] = wordArray[wordArray.Length - 1];

                sender.Launch(commandMessage);
            }
            else
            {
                Console.WriteLine("User cancelled file choosing");
            }
        }


    }
}