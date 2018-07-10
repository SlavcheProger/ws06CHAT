using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Sockets;

namespace Client1
{
    internal class ChatCore
    {
        private NetworkStream stream;
        private AutoPing autoPing { get; set; }

        private IDisconnectable disconnector;

        internal ChatCore(IDisconnectable disconnector, NetworkStream stream)
        {
            this.stream = stream;
            this.disconnector = disconnector;
            autoPing = new AutoPing(disconnector, stream);
        }
        
        internal void SendMessage(string message)
        {
            var chatMessageFromClient = new ChatMessageFromClient();
            chatMessageFromClient.Message = message;
            SendObject(chatMessageFromClient);
        }

        internal void CommandHandler(string message)
        {
            var splitedMessage = SplitCommand(message);
            var obj = new object();
            switch (splitedMessage[0])
            {
                case Commands.Start:
                    obj = StartHandler();
                    break;
                case Commands.SetUserName:
                    obj = SetUsernameHandler(splitedMessage);
                    break;
                case Commands.Logout:
                    LogoutHandler();
                    obj = null;
                    break;
                case Commands.Here:
                    obj = HereHandler();
                    break;
                case Commands.Help:
                    HelpHandler();
                    obj = null;
                    break;
                case Commands.Disconnect:
                    disconnector.Disconnect();
                    obj = null;
                    break;
                case Commands.SendFile:
                    obj = SendFileHandler(splitedMessage);
                    break;
                default:
                    var displayService = new DisplayMessageService();
                    displayService.Display("Sended as message", DisplayMessageType.System);
                    displayService.Display("Incorrect command: unknown command. Try \"/help\" for help.", DisplayMessageType.Error);
                    obj = null;
                    SendMessage(message);
                    break;
            }
            SendObject(obj);
        }

        internal void SendToUserHandler(string message)
        {
            message = message.Remove(0, 1);
            var nameAndMessage = GetNameAndMessage(message);

            var messageToOneClient = new ChatMessageFromClient();
            var jobject = new JObject();
            jobject["Recipient"] = nameAndMessage[0];

            messageToOneClient.Args = jobject;
            messageToOneClient.Message = nameAndMessage[1];
            SendObject(messageToOneClient);
        }

        private string[] SplitCommand(string message)
        {
            var splitedString = message.Split(' ');
            splitedString[0] = splitedString[0].ToLower();
            splitedString = splitedString.Where(s => !string.IsNullOrEmpty(s)).ToArray();
            return splitedString;
        }

        private void SendObject(object obj)
        {
            var sender = new Sender(stream);
            if (obj != null)
            {
                sender.Send(obj);
            }
        }

        private object StartHandler()
        {
            var commandMessage = new CommandMessage();
            var jobject = new JObject();
            jobject["AccessToken"] = TokenStorage.Instance.AccessToken;
            commandMessage.CommandType = CommandType.Start;
            commandMessage.Args = jobject;
            return commandMessage;
        }
        
        private void HelpHandler()
        {
            var display = new DisplayMessageService();
            display.Display();
            display.Display($"{Commands.SetUserName} <name>    | change your name to <name>", DisplayMessageType.System);
            display.Display($"{Commands.Here}                  | show all online users", DisplayMessageType.System);
            display.Display($"{Commands.Logout}                | log out from account", DisplayMessageType.System);
            display.Display($"{Commands.Help}                  | show help", DisplayMessageType.System);
            display.Display($"{Commands.Disconnect}            | disconnect from server", DisplayMessageType.System);
            display.Display($"{Commands.SendFile} <recipient>  | send chosen file to <recipient>. If <recipient> is empty, send to all users>", DisplayMessageType.System);
            display.Display($"To make spaces in your nickname, write </setusername \"example name\">. ", DisplayMessageType.System);
            display.Display($"To send message to user with spaces in nickname, write <@\"example name\"  your_message>", DisplayMessageType.System);
            display.Display($"Blue messages can be seen only by you.", DisplayMessageType.System);
            display.Display();
        }

        private object SetUsernameHandler(string[] splitedMessage)
        {
            if (splitedMessage.Length < 2)
            {
                var display = new DisplayMessageService();
                display.Display("Incorrect command: few arguments.", DisplayMessageType.Error);
                return null;
            }
            var message = string.Join(" ", splitedMessage);
            var NameAndMessage = GetNameAndMessage(message.Remove(0, Commands.SetUserName.Length + 1));

            var commandMessage = new CommandMessage();
            var jobject = new JObject();
            jobject["Name"] = NameAndMessage[0];
            commandMessage.CommandType = CommandType.SetUsername;
            commandMessage.Args = jobject;
            return commandMessage;
        }

        private string[] GetNameAndMessage(string message)
        {
            var rezult = new string[2];
            if (message[0] == '\"')
            {
                message = message.Remove(0, 1);
                for (int i = 0; i < message.Length; i++)
                {
                    if (message[i] == '\"')
                    {
                        rezult[0] = message.Substring(0, i);
                        message = message.Remove(0, i + 1);
                        break;
                    }
                }

            }
            else
            {
                var firstSpaceIndex = message.IndexOf(' ');
                if (firstSpaceIndex == -1)
                {
                    rezult[0] = message;
                    return rezult;
                }
                rezult[0] = message.Substring(0, firstSpaceIndex);
                message = message.Remove(0, firstSpaceIndex + 1);
            }
            rezult[1] = message;

            return rezult;
        }

        private object HereHandler()
        {
            var commandMessage = new CommandMessage();
            commandMessage.CommandType = CommandType.Here;
            return commandMessage;
        }

        private object SendFileHandler(string[] message)
        {
            var selectedFile = new FileSelector();
            var unmessage = string.Join(" ", message);
            var obj = new object();
            if (unmessage.Length > Commands.SendFile.Length + 1)
            {
                unmessage = unmessage.Remove(0, Commands.SendFile.Length + 1);
                var nameAndMessage = GetNameAndMessage(unmessage);
                obj = selectedFile.SelectedFilePath(nameAndMessage[0], nameAndMessage[1]);
            }
            else
            {
                obj = selectedFile.SelectedFilePath(null, null);
            }
            return obj;
        }

        private void LogoutHandler()
        {
            TokenStorage.Instance.Delete();
            disconnector.Disconnect();
        }

        internal void ReceiveHandler(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                var baseMessage = new BaseMessage();
                baseMessage = JsonConvert.DeserializeObject<BaseMessage>(message);
                switch (baseMessage.MessageType)
                {
                    case MessageType.ChatMessageFromServer:
                        MessageFromServerHandler(message);
                        break;
                    case MessageType.ServerNotification:
                        NotificationHandler(message);
                        break;
                    case MessageType.CommandMessage:
                        ServerCommandHandler(message);
                        break;
                }
            }
        }

        private void SendFileCallback(CommandMessage commandFromServer)
        {
            var file = new FileSelector();
            file.SaveFile(commandFromServer);
        }

        private void ServerCommandHandler(string message)
        {
            var commandFromServer = JsonConvert.DeserializeObject<CommandMessage>(message);
            switch (commandFromServer.CommandType)
            {
                case CommandType.Start:
                    StartCallback(commandFromServer);
                    break;
                case CommandType.SetUsername:
                    break;
                case CommandType.Here:
                    HereCallback(commandFromServer);
                    break;
                case CommandType.Pong:
                    PongCallback();
                    break;
                case CommandType.Ping:
                    PingCallback(commandFromServer);
                    break;
                case CommandType.SendFile:
                    SendFileCallback(commandFromServer);
                    break;
                default:
                    var display = new DisplayMessageService();
                    display.Display("Incorrect command from server.", DisplayMessageType.Error);
                    break;
            }
        }
        private void MessageFromServerHandler(string message)
        {
            var chatMessageFromServer = JsonConvert.DeserializeObject<ChatMessageFromServer>(message);
            if (!string.IsNullOrEmpty(chatMessageFromServer.Message))
            {
                var sendedString = chatMessageFromServer.Name;
                var display = new DisplayMessageService();
                if (sendedString[0] == '[' && sendedString[1] == '@' && sendedString[2] == ']')
                {
                    chatMessageFromServer.Name = chatMessageFromServer.Name.Remove(0, 3);
                    display.Display($"{chatMessageFromServer.Name}: {chatMessageFromServer.Message}", DisplayMessageType.PrivateMessage);
                }
                else
                {
                    display.Display($"{chatMessageFromServer.Name}: {chatMessageFromServer.Message}", DisplayMessageType.Message);
                }
            }
        }

        private void NotificationHandler(string message)
        {
            var notification = JsonConvert.DeserializeObject<ServerNotification>(message);
            var display = new DisplayMessageService();
            display.Display(notification.Message, DisplayMessageType.ServerNotification);
        }

        private void StartCallback(CommandMessage commandFromServer)
        {
            TokenStorage.Instance.Save(commandFromServer.Args["AccessToken"].ToString());
            autoPing.Start();
        }

        private void HereCallback(CommandMessage commandFromServer)
        {
            var usersString = commandFromServer.Args["Users"].ToString();
            var display = new DisplayMessageService();
            if (!string.IsNullOrEmpty(usersString))
            {
                display.Display("People in the chat:", DisplayMessageType.ServerNotification);
                var users = usersString.Split(',');
                for (int i = 0; i < users.Length; i++)
                {
                    display.Display(users[i], DisplayMessageType.ServerNotification);
                }
            }
            else
            {
                display.Display("You are alone in the void.", DisplayMessageType.Error);
            }
        }

        private void PingCallback(CommandMessage commandFromServer)
        {
            commandFromServer.CommandType = CommandType.Pong;
            var sender = new Sender(stream);
            sender.Send(commandFromServer);
        }

        private void PongCallback()
        {
            autoPing.ChangeFlag(true);
        }

        internal void StopTimer()
        {
            autoPing.Stop();
        }
    }
}
