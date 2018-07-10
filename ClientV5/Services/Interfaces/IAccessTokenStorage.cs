using System;
namespace ClientV5.Services.Interfaces
{
    public interface IAccessTokenStorage
    {
        string Get();

        void Set(string accessToken);
    }
}
