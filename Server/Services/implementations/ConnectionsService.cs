using System;
using System.Collections.Generic;
using System.Linq;
using Chat.Socket.Server.Domains.Accounts;
using Chat.Socket.Server.Services.Interfaces;

namespace Chat.Socket.Server.Services.Implementations
{
    public class ConnectionsService : IConnectionsService
    {
        public List<ClientConnection> connections;
		private readonly ILogger logger;

        public ConnectionsService()
        {

            connections = new List<ClientConnection>();
			logger = DependencyResolver.Get<ILogger>();
        }


        #region IConnectionsService
        ClientConnection[] IConnectionsService.Connections => connections.ToArray();

        void IConnectionsService.Add(ClientConnection connection)
        {
            connections.Add(connection);
        }
        void IConnectionsService.Remove(ClientConnection connection)
        {
            connections.Remove(connection);
            connection.UserObject.Status = Status.Offline;
			try
			{
				using (var context = new DataBaseContext())
				{
					var user = context.Users.FirstOrDefault(u => u.AccessToken == connection.UserObject.AccessToken);
					user.Status = Status.Offline;
					context.SaveChanges();
				}
			}
			catch(Exception exception)
			{
				logger.Write(exception);
			}

            connection.Close();
        }
        #endregion
    }
}
