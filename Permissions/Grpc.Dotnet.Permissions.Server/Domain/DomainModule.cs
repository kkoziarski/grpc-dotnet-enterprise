using Autofac;

namespace Grpc.Dotnet.Permissions.Domain
{
    public class DomainModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //builder.RegisterType<MyService>().As<IMyService>().InstancePerLifetimeScope();
        }
    }
}
