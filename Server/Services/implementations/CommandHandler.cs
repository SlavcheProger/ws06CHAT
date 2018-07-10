using System.Collections.Generic;
using System.Linq;
using Chat.Socket.Server.Domains.Accounts;
using Chat.Socket.Server.Services.Interfaces;

namespace Chat.Socket.Server.Services.Implementations
{
    public class CommandHandler : ICommandHandler
    {
        // Services
        private readonly IConnectionsService connectionsService;
        private readonly IUsersService usersService;
        private readonly ISender sender;
        private readonly IPingIdGenerator pingIdGenerator;

        public CommandHandler()
        {
            connectionsService = DependencyResolver.Get<IConnectionsService>();
            usersService = DependencyResolver.Get<IUsersService>();
            sender = DependencyResolver.Get<ISender>();
            pingIdGenerator = DependencyResolver.Get<IPingIdGenerator>();
        }

        #region ICommandHandler
        void ICommandHandler.Start(ClientConnection connection, string accessToken)
        {
            if (usersService.IsAuthorised(accessToken))
            {
                connection.UserObject = usersService.Find(accessToken);
                connection.UserObject.Status = Status.Online;

                using (var context = new DataBaseContext())
                {
                    var user = context.Users.FirstOrDefault(u => u.AccessToken == accessToken);
                    user.Status = Status.Online;
                    context.SaveChanges();
                }
            }
            else
            {
                connection.UserObject = usersService.Create();
            }

            sender.ServerNotificationToAll($"{connection.UserObject.Name} joined.", connection);
            sender.StartCallback(connection);
        }

        void ICommandHandler.Here(ClientConnection connection)
        {
            List<string> usersNames;

            usersNames = new List<string>();

            foreach (var connectionItem in connectionsService.Connections)
            {
                if (connectionItem.UserObject.Status == Status.Online && !usersNames.Any(name => name == connectionItem.UserObject.Name))
                {
                    usersNames.Add(connectionItem.UserObject.Name);
                }
            }

            sender.HereCallback(connection, usersNames.ToArray());
        }

        void ICommandHandler.SetUsername(ClientConnection connection, string name)
        {
            if (usersService.UsernameIsFree(name))
            {
                connection.UserObject.UpdateName(name);
                sender.SetUsernameCallback(connection, name);
            }
            else
            {
                sender.ServerNotification(connection, $"Name {name} is already taken.");
            }
        }
        #endregion
    }
}
