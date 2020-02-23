using System;
using System.Globalization;

namespace Grpc.Dotnet.Shared.Helpers.Exceptions
{
    public abstract class EntityAlreadyExistsException : Exception
    {
        protected EntityAlreadyExistsException(string entityId)
            : this(nameof(EntityAlreadyExistsException), entityId)
        {
        }

        protected EntityAlreadyExistsException(string entityName, string entityId)
        {
            EntityName = entityName;
            EntityId = entityId;
        }

        public string EntityName { get; private set; }

        public string EntityId { get; private set; }
    }

    public class EntityAlreadyExistsException<TEntity> : EntityAlreadyExistsException
    {
        public EntityAlreadyExistsException(string entityId)
            : base(typeof(TEntity).Name, entityId)
        {
        }

        public EntityAlreadyExistsException(long entityId)
            : this(entityId.ToString(CultureInfo.InvariantCulture))
        {
        }

        public EntityAlreadyExistsException(Guid entityId)
            : this(entityId.ToString())
        {
        }
    }
}
