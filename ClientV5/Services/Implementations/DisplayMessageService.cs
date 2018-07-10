using System;
using ClientV5.Services.Interfaces;

namespace ClientV5.Services.Implementations
{
    public class DisplayMessageService: IDisplayMessageService
    {
        public DisplayMessageService() {}

        void IDisplayMessageService.Write(string message)
        {
            Console.WriteLine(message);
        }
    }
}
