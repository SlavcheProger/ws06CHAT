namespace Chat.Socket.ClientV2.Source
{
    internal class MainCore
    {
        internal static void Main(string[] Args)
        {
            var launch = new ChatLauncher();
            launch.Start();
        }
    }
}
