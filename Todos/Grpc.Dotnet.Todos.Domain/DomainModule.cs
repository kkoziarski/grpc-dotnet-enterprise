using Autofac;

namespace Grpc.Dotnet.Todos.Domain
{
    public class DomainModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //builder.RegisterType<MyService>().As<IMyService>().InstancePerLifetimeScope();
        }
    }
}
