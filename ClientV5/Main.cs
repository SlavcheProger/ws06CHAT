
namespace ClientV5
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var serverConection = new ServerConnection("localhost", 8888);
            serverConection.Launch();
        }
    }
}
