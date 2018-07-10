using System;

namespace Client1
{
    public class Start
    {
        public static void Main(string[] args) =>new Start().LaunchChat();

        void LaunchChat()
        {
            var connection = new ChatLauncher();
            try
            {
                connection.Launch();
            }
            catch (Exception exception)
            {
                var log = new Logger();
                log.SaveLog(exception);
            }
            finally
            {
                Disconnect(connection);
            }
        }

        private void Disconnect(ChatLauncher connection)
        {
            connection.Disconnect();
            var display = new DisplayMessageService();
            display.Display("Disconnected. Try to connect?", DisplayMessageType.System);

            var answer = Console.ReadLine().Trim().ToLower();
            if (answer == "n")
            {
                return;
            }

            LaunchChat();
        }
    }
}