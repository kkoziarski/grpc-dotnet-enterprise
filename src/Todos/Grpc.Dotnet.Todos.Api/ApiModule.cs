using Autofac;

namespace Grpc.Dotnet.Todos.Api
{
    public class ApiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            ////builder.RegisterGeneric(typeof(ValidationBehavior<,>)).As(typeof(IPipelineBehavior<,>)).InstancePerLifetimeScope();
            ////builder.RegisterType<JsonPatch>().AsImplementedInterfaces();
        }
    }
}
