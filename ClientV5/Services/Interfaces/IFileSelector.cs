using System;
using ClientV5.Domains.Messages;

namespace ClientV5.Services.Interfaces
{
    public interface IFileSelector
    {
        CommandMessageContainer GetDataFromFile();
    }
}
