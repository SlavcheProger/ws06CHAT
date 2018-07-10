using System;

namespace Client1
{
    internal class DisplayMessageService
    {
        internal DisplayMessageService(){}

        internal void Display(string message = null, DisplayMessageType type = DisplayMessageType.Message)
        {
            switch (type)
            {
                case DisplayMessageType.Success:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case DisplayMessageType.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case DisplayMessageType.ServerNotification:
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    break;
                case DisplayMessageType.System:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case DisplayMessageType.PrivateMessage:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
            }
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
