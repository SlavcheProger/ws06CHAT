using System;
using System.IO;
using ClientV5.Services.Interfaces;

namespace ClientV5.Services.Implementations
{
    public class AccessTokenStorage: IAccessTokenStorage
    {
        private readonly string tokenPath = "accessToken.txt";

        public AccessTokenStorage() {}

        string IAccessTokenStorage.Get()
        {
            if (File.Exists(tokenPath))
            {
                return File.ReadAllText(tokenPath);
            }
            return string.Empty;
        }

        void IAccessTokenStorage.Set(string accessToken)
        {
            File.WriteAllText(tokenPath, accessToken);
        }
    }
}
