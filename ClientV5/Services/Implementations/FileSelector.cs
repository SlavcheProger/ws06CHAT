using System;
using System.IO;
using System.Windows.Forms;
using ClientV5.Domains.Messages;
using ClientV5.Services.Interfaces;

namespace ClientV5.Services.Implementations
{
    public class FileSelector : IFileSelector
    {
        public FileSelector() { }

        CommandMessageContainer IFileSelector.GetDataFromFile()
        {
            var transportContainer = new CommandMessageContainer(CommandType.SendFile);
            var ofd = new OpenFileDialog { Multiselect = false };

            if (DialogResult.OK == (new Invoker(ofd).Invoke()))
            {
                var fileName = ofd.FileName;
                var byteData = File.ReadAllBytes(fileName);
                var byteMessage = Convert.ToBase64String(byteData);

                var wordArray = fileName.Split('.');
                var extension = wordArray[wordArray.Length - 1];

                transportContainer.Args["File"] = byteMessage;
                transportContainer.Args["Extension"] = extension;
            }
            else
            {
                Console.WriteLine("File choosing failed.");
            }

            return transportContainer;
        }
    }
}
