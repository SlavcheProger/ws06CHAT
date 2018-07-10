using Autofac;
using Chat.Socket.Server.Services.Implementations;
using Chat.Socket.Server.Services.Interfaces;

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

            result.RegisterType<UsersService>().As<IUsersService>();

            result.RegisterType<ConnectionsService>().As<IConnectionsService>().SingleInstance();

            result.RegisterType<CommandHandler>().As<ICommandHandler>();

            result.RegisterType<Sender>().As<ISender>();

            result.RegisterType<PingIdGenerator>().As<IPingIdGenerator>();

            result.RegisterType<DataStorage>().As<IDataStorage>();

            return result;
        }

        public static TService Get<TService>() => container.Resolve<TService>();
    }
}
