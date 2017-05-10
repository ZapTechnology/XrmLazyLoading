using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;

namespace XrmLazyLoading
{
    public class EntityWrapper : Entity
    {
        private OrganizationServiceContext _context;

        public EntityWrapper(string entityName) : base(entityName)
        {
        }

        internal void Attach(OrganizationServiceContext context)
        {
            _context = context;
        }

        protected override TEntity GetRelatedEntity<TEntity>(string relationshipSchemaName, EntityRole? primaryEntityRole)
        {
            if (_context == null)
                return base.GetRelatedEntity<TEntity>(relationshipSchemaName, primaryEntityRole);
            return this.GetRelatedEntity(_context, relationshipSchemaName, primaryEntityRole) as TEntity;
        }

        protected override IEnumerable<TEntity> GetRelatedEntities<TEntity>(string relationshipSchemaName, EntityRole? primaryEntityRole)
        {
            if (_context == null)
                return base.GetRelatedEntities<TEntity>(relationshipSchemaName, primaryEntityRole);
            return this.GetRelatedEntities(_context, relationshipSchemaName, primaryEntityRole).Cast<TEntity>();
        }
    }
}

