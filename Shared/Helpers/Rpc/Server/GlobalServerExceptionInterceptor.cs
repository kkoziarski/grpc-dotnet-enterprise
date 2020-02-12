namespace Grpc.Dotnet.Shared.Helpers.Rpc.Server
{
    using System;
    using System.Reflection;
    using System.Threading.Tasks;
    using Grpc.Core;
    using Grpc.Core.Interceptors;
    using Grpc.Dotnet.Shared.Helpers.Exceptions;
    using Microsoft.Extensions.Logging;

    public class GlobalServerExceptionInterceptor : Interceptor
    {
        private readonly ILogger<GlobalServerExceptionInterceptor> logger;

        public GlobalServerExceptionInterceptor(ILogger<GlobalServerExceptionInterceptor> logger)
        {
            this.logger = logger;
        }

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            try
            {
                return await base.UnaryServerHandler(request, context, continuation);
            }
            catch (Exception exception)
            {
                context.Status = ExceptionHandler.Handle(exception, logger);
                return (TResponse)Activator.CreateInstance(typeof(TResponse));
            }
        }

        private static class ExceptionHandler
        {
            public static Status Handle(Exception ex, ILogger<GlobalServerExceptionInterceptor> logger)
            {
                // Mapping GRPC errors to HTTP codes: https://cloud.google.com/apis/design/errors
                var exception = ex;
                if (exception is TargetInvocationException && exception.InnerException != null)
                {
                    exception = exception.InnerException;
                }

                var statusCode = StatusCode.Unknown;
                var message = string.Empty;

                switch (exception)
                {
                    case EntityNotFoundException entityNotFoundException:
                        statusCode = StatusCode.NotFound;
                        message = CreateExceptionMessage(entityNotFoundException.EntityName, entityNotFoundException.EntityId);
                        break;
                    case EntityAlreadyExistsException entityAlreadyExistsException:
                        statusCode = StatusCode.AlreadyExists;
                        message = CreateExceptionMessage(entityAlreadyExistsException.EntityName, entityAlreadyExistsException.EntityId);
                        break;
                }

                if (statusCode == StatusCode.Unknown)
                {
                    logger.LogError(exception, string.Empty);
                }

                return new Status(statusCode, message);
            }

            private static string CreateExceptionMessage(string entityName, string message)
            {
                return $"{entityName}, {message}";
            }
        }
    }
}
