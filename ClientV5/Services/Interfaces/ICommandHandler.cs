namespace ClientV5.Services.Interfaces
{
    public interface ICommandHandler
    {
        void GettingMessagesProcess();

        void Parse(string data);
    }

}
