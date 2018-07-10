using Autofac;
using ClientV5.Services.Implementations;
using ClientV5.Services.Interfaces;

namespace Chat.Socket.Server.Services
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

            result.RegisterType<Logger>().As<ILogger>();

            result.RegisterType<CommandHandler>().As<ICommandHandler>();

            result.RegisterType<Sender>().As<ISender>().SingleInstance();

            result.RegisterType<ChatCore>().As<IChatCore>().SingleInstance();

            result.RegisterType<FileSelector>().As<IFileSelector>();

            result.RegisterType<AccessTokenStorage>().As<IAccessTokenStorage>();

            result.RegisterType<DisplayMessageService>().As<IDisplayMessageService>();

            result.RegisterType<Reciever>().As<IReciever>().SingleInstance();

            result.RegisterType<PingIdGenerator>().As<IPingIdGenerator>().SingleInstance();

            result.RegisterType<Disconnector>().As<IDisconnector>().SingleInstance();

            result.RegisterType<FileSelector>().As<IFileSelector>().SingleInstance();

            return result;
        }

        public static TService Get<TService>() => container.Resolve<TService>();
    }
}
