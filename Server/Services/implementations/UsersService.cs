using System;
using System.Linq;
using Chat.Socket.Server.Domains.Accounts;
using Chat.Socket.Server.Services.Interfaces;

namespace Chat.Socket.Server.Services
{
    public class UsersService : IUsersService
    {
        // Services
        private readonly IUsersService self;
        private readonly IDataStorage dataStorage;
    
        public UsersService()
        {
            dataStorage = DependencyResolver.Get<IDataStorage>();
            self = this as IUsersService;
        }

        #region IUsersService
        bool IUsersService.IsAuthorised(string accessToken)
        {
            using (var context = new DataBaseContext())
            {
                return context.Users.Any(u => u.AccessToken == accessToken);
            }
        }

        bool IUsersService.IsOnline(string name)
        {
            using (var context = new DataBaseContext())
            {
                var user = context.Users.FirstOrDefault(u => u.Name == name);
                return (user != default(UserDB) && user.Status == Status.Online);
            }
        }

        User IUsersService.Find(string accessToken)
        {
            using (var context = new DataBaseContext())
            {
                var userDB = context.Users.FirstOrDefault(u => u.AccessToken == accessToken);
                return new User(userDB.ID, userDB.AccessToken, userDB.Name);
            }
        }

        User IUsersService.Create(string name)
        {
            var newAccessToken = Guid.NewGuid().ToString();

            dataStorage.SaveUser(name, newAccessToken);

            var user = new User(self.Find(newAccessToken).ID, newAccessToken, name);

            return user;
        }

        bool IUsersService.UsernameIsFree(string name)
        {
            using (var context = new DataBaseContext())
            {
                return !context.Users.Any(u => u.Name == name);
            }
        }
        #endregion
    }
}
