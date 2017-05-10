using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;

namespace XrmLazyLoading
{
    public static class EntityExtensions
    {
        public static Entity GetRelatedEntity(this Entity entity, OrganizationServiceContext context, string relationshipSchemaName, EntityRole? primaryEntityRole = null)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            return GetRelated(context, entity, relationshipSchemaName, primaryEntityRole, GetRelatedEntity<Entity>);
        }

        public static IEnumerable<Entity> GetRelatedEntities(this Entity entity, OrganizationServiceContext context, string relationshipSchemaName, EntityRole? primaryEntityRole = null)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            return GetRelated(context, entity, relationshipSchemaName, primaryEntityRole, GetRelatedEntities<Entity>);
        }

        private static TEntity GetRelatedEntity<TEntity>(this Entity entity, string relationshipSchemaName, EntityRole? primaryEntityRole = null) where TEntity : Entity
        {
            Relationship relationship = new Relationship(relationshipSchemaName)
            {
                PrimaryEntityRole = primaryEntityRole
            };
            return entity.GetRelatedEntity<TEntity>(relationship);
        }

        private static TEntity GetRelatedEntity<TEntity>(this Entity entity, Relationship relationship) where TEntity : Entity
        {
            if (relationship == null) throw new ArgumentNullException(nameof(relationship));
            if (!entity.RelatedEntities.Contains(relationship))
                return default(TEntity);
            return (TEntity)entity.RelatedEntities[relationship].Entities.FirstOrDefault();
        }

        private static IEnumerable<TEntity> GetRelatedEntities<TEntity>(this Entity entity, string relationshipSchemaName, EntityRole? primaryEntityRole = null) where TEntity : Entity
        {
            Relationship relationship = new Relationship(relationshipSchemaName)
            {
                PrimaryEntityRole = primaryEntityRole
            };
            return entity.GetRelatedEntities<TEntity>(relationship);
        }

        private static IEnumerable<TEntity> GetRelatedEntities<TEntity>(this Entity entity, Relationship relationship) where TEntity : Entity
        {
            if (relationship == null) throw new ArgumentNullException(nameof(relationship));
            IEnumerable<TEntity> entities = entity.RelatedEntities.Contains(relationship) ? entity.RelatedEntities[relationship].Entities.Cast<TEntity>() : null;
            if (entities != null)
            {
                foreach (TEntity entity1 in entities)
                    yield return entity1;
            }
        }

        private static T GetRelated<T>(OrganizationServiceContext context, Entity entity, string relationshipSchemaName, EntityRole? primaryEntityRole, Func<OrganizationServiceContext, Entity, Relationship, T> action)
        {
            Relationship relationship = new Relationship(relationshipSchemaName)
            {
                PrimaryEntityRole = primaryEntityRole
            };
            return action(context, entity, relationship);
        }

        private static T GetRelatedEntity<T>(OrganizationServiceContext context, Entity entity, Relationship relationship) where T : Entity
        {
            if (!context.IsAttached(entity) && entity.RelatedEntities.Contains(relationship))
                entity.RelatedEntities.Remove(relationship);
            context.LoadProperty(entity, relationship);
            return entity.GetRelatedEntity<T>(relationship.SchemaName, relationship.PrimaryEntityRole);
        }

        private static IEnumerable<T> GetRelatedEntities<T>(OrganizationServiceContext context, Entity entity, Relationship relationship) where T : Entity
        {
            if (!context.IsAttached(entity) && entity.RelatedEntities.Contains(relationship))
                entity.RelatedEntities.Remove(relationship);
            context.LoadProperty(entity, relationship);
            return entity.GetRelatedEntities<T>(relationship.SchemaName, relationship.PrimaryEntityRole) ?? new T[0];
        }
    }
}
