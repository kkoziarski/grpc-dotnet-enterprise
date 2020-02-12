using System;
using System.Globalization;

namespace Grpc.Dotnet.Shared.Helpers.Exceptions
{
    public abstract class EntityNotFoundException : Exception
    {
        protected EntityNotFoundException(string entityId)
            : this(nameof(EntityNotFoundException), entityId)
        {
        }

        protected EntityNotFoundException(string entityName, string entityId)
        {
            EntityId = entityId;
            EntityName = entityName;
        }

        public string EntityName { get; private set; }

        public string EntityId { get; private set; }
    }

    public class EntityNotFoundException<TEntity> : EntityNotFoundException
    {
        public EntityNotFoundException(string entityName, string entityId)
            : base(entityName, entityId)
        {
        }

        public EntityNotFoundException(string entityId)
            : this(typeof(TEntity).Name, entityId)
        {
        }

        public EntityNotFoundException(long entityId)
            : this(entityId.ToString(CultureInfo.InvariantCulture))
        {
        }

        public EntityNotFoundException(Guid entityId)
            : this(entityId.ToString())
        {
        }
    }
}
