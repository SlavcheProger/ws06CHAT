using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Windows.Forms;

namespace Client1
{
    internal class FileSelector
    {
        internal FileSelector() { }

        internal object SelectedFilePath(string recipient, string message)
        {
            var sendingData = new CommandMessage();
            var ofd = new OpenFileDialog { Multiselect = false };
            var display = new DisplayMessageService();
            if ((new Invoker(ofd).Invoke()) == DialogResult.OK)
            {
                ofd.AddExtension = true;
                var byteData = new byte[2097152];
                byteData = File.ReadAllBytes(ofd.FileName);
                var stringData = Convert.ToBase64String(byteData);

                var jobject = new JObject();

                if (!string.IsNullOrEmpty(recipient))
                {
                    jobject["Recipient"] = recipient;
                }
                jobject["Message"] = message;
                jobject["Extension"] = GetExtension(ofd.FileName);
                jobject["File"] = stringData;
                display.Display(ofd.FileName, DisplayMessageType.System);
                sendingData.Args = jobject;
                sendingData.CommandType = CommandType.SendFile;
            }
            else
            {
                display.Display("File chosing canceled", DisplayMessageType.System);
            }
            return sendingData;
        }

        internal void SaveFile(CommandMessage commandFromServer)
        {
            try
            {
                var file = Convert.FromBase64String(commandFromServer.Args["File"].ToString());
                var extension = commandFromServer.Args["Extension"];
                var message = commandFromServer.Args["Message"];
                var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Messanger\\Files\\";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                var guid = Guid.NewGuid().ToString();
                File.WriteAllBytes($"{path}{guid}.{extension}", file);
                var display = new DisplayMessageService();
                if (!string.IsNullOrEmpty(Convert.ToString(message)))
                {
                    display.Display(Convert.ToString(message), DisplayMessageType.Message);
                }
                else
                {
                    display.Display($"You received file.", DisplayMessageType.Success);
                }
            } catch(Exception exception)
            {
                var logger = new Logger();
                logger.SaveLog(exception);
                var displayService = new DisplayMessageService();
                displayService.Display(exception.Message);
            }
         }

        private string GetExtension(string fileName)
        {
            var splitedName = fileName.Split('.');
            var extension = splitedName[splitedName.Length - 1];
            return extension;
        }
    }
}
