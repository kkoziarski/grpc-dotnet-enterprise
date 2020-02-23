using System;
using Autofac;

namespace Grpc.Dotnet.Permissions.Domain
{
    public class DomainModule : Module
    {
        public static readonly Guid AdminId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

        protected override void Load(ContainerBuilder builder)
        {
            //builder.RegisterType<MyService>().As<IMyService>().InstancePerLifetimeScope();
        }
    }
}
