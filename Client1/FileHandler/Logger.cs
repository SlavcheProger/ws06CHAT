using System;
using System.IO;

namespace Client1
{
    class Logger
    {
        private string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Messanger\\";
        internal Logger() { }

        internal void SaveLog(Exception exception)
        {
            if(!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var file = "client.log";
            if (!File.Exists(path+file))
            {
                File.WriteAllText(path+file,exception.Message);
            }else
            {
                File.AppendAllText(path+file, exception.Message);
            }
        }
    }
}