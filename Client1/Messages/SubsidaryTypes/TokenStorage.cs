using System;
using System.IO;

namespace Client1
{
    public class TokenStorage
    {
        internal static TokenStorage Instance { get; } = new TokenStorage();
        private string filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Messanger\\";

        internal string AccessToken { get; private set; }

        private TokenStorage()
        {
            AccessToken = ReadAccessToken();
        }

        internal void Save(string accessToken)
        {
            AccessToken = accessToken;
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            File.WriteAllText(filePath+"access_token.txt", accessToken);
        }

        internal void Delete()
        {
            if (File.Exists(filePath+"access_token.txt"))
            {
                File.Delete(filePath+"access_token.txt");
            }
            AccessToken = "";
        }

        private string ReadAccessToken()
        {
            if (!File.Exists(filePath+"access_token.txt"))
            {
                return null;
            }

            return File.ReadAllText(filePath+"access_token.txt");
        }
        
    }
}