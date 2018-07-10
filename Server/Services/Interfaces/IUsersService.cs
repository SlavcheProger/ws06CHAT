using Chat.Socket.Server.Domains.Accounts;

namespace Chat.Socket.Server.Services.Interfaces
{
    public interface IUsersService
    {
        bool IsAuthorised(string accessToken);

        User Find(string accessToken);

        User Create(string name = null);

        bool UsernameIsFree(string name);

        bool IsOnline(string name);
    }
}
