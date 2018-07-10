using Autofac;
using Chat.Socket.ClientV2.Services.Implementations;
using Chat.Socket.ClientV2.Services.Interfaces;

namespace Chat.Socket.ClientV2.Services
{
    public class DependencyResolver
    {
        private static IContainer container;
        static DependencyResolver()
        {
            var builder = Register();
            container = builder.Build(); 
        }
        private static ContainerBuilder Register()
        {
            var result = new ContainerBuilder();



            result.RegisterType<Receiver>().As<IReceiver>().SingleInstance();

            result.RegisterType<ConsoleDataHandler>().As<IConsoleDataHandler>().SingleInstance();

            result.RegisterType<Disconnector>().As<IDisconnector>().SingleInstance();

            result.RegisterType<Sender>().As<ISender>();

            result.RegisterType<AutoPinger>().As<IAutoPinger>();

            return result;
        }

        public static TService Get<TService>() => container.Resolve<TService>();
    }
}
