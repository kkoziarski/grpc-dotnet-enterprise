using AutoMapper;
using Grpc.Dotnet.Todos.Domain.Domain;
using Shouldly;
using Xunit;

namespace Grpc.Dotnet.Todos.Domain.UnitTests
{
    public class AutoMapperTest
    {
        [Fact]
        public void ConfigurationIsValid_WhenAllMapsConfiguredCorrectly_ShouldNotThrowException()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapperProfile>();
            });

            // Act + Assert
            Should.NotThrow(() => configuration.AssertConfigurationIsValid());
        }
    }
}
