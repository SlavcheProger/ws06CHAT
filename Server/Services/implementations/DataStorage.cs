using Chat.Socket.Server.Domains.Accounts;
using Chat.Socket.Server.Domains.Messages;
using Chat.Socket.Server.Services.Interfaces;
using System;
using System.IO;

namespace Chat.Socket.Server.Services.Implementations
{
    public class DataStorage: IDataStorage
    {
        private readonly ILogger logger;

        public DataStorage()
        {
            logger = DependencyResolver.Get<ILogger>();
        }

        void IDataStorage.SaveFile(int senderId, int recipientId, byte[] contentOfFile, string extension)
        {
           var filename = $"{Guid.NewGuid().ToString()}.{extension}";

           using (var context = new DataBaseContext())
           {
               var fileDB = new FileDB(senderId, recipientId, filename);
               context.Files.Add(fileDB);
               context.SaveChanges();
           }

           File.WriteAllBytes($"./../../usersfiles/{filename}", contentOfFile);
        }             

        void IDataStorage.SaveMessage(string chatMessage, int recipientId, int senderId)
        {
            using (var context = new DataBaseContext())
            {
                var chatMessageDB = new ChatMessageFromCLientDB(senderId, recipientId, chatMessage);
                context.ChatMessages.Add(chatMessageDB);
                context.SaveChanges();
            }
        }

        void IDataStorage.SaveUser(string username, string accessToken)
        {
            using (var context = new DataBaseContext())
            {
                var userDB = new UserDB(username, accessToken);
                context.Users.Add(userDB);
                context.SaveChanges();
            }
        }
    }
}
