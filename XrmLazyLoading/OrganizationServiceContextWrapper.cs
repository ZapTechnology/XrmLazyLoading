using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System.Collections.Generic;
using System.Linq;

namespace XrmLazyLoading
{
    public class OrganizationServiceContextWrapper : OrganizationServiceContext
    {
        public OrganizationServiceContextWrapper(IOrganizationService service)
            : base(service)
        { }

        protected override void OnBeginEntityTracking(Entity entity)
        {
            EntityWrapper crmEntity = entity as EntityWrapper;
            crmEntity?.Attach(this);
            base.OnBeginEntityTracking(entity);
        }

        protected override void OnEndEntityTracking(Entity entity)
        {
            EntityWrapper crmEntity = entity as EntityWrapper;
            crmEntity?.Attach(null);
            base.OnEndEntityTracking(entity);
        }

        protected override void OnSaveChanges(SaveChangesResultCollection results)
        {
            if (results.HasError)
                return;
            foreach (Entity entity in results.SelectMany(result => GetEntitiesForObject(result.Request)))
            {
                entity.EntityState = new EntityState?();
                Attach(entity);
            }
        }

        private IEnumerable<Entity> GetEntitiesForObject(object query, IEnumerable<object> path = null)
        {
            if (query is OrganizationRequest)
            {
                OrganizationRequest request = query as OrganizationRequest;
                IEnumerable<object> path1 = path;
                if (path1 == null)
                    path1 = new List<object>
                    {
                        query
                    };
                return GetEntities(request, path1);
            }
            if (query is OrganizationResponse)
            {
                OrganizationResponse response = query as OrganizationResponse;
                IEnumerable<object> path1 = path;
                if (path1 == null)
                    path1 = new List<object>
                    {
                        query
                    };
                return GetEntities(response, path1);
            }
            if (query is IEnumerable<Entity>)
                return query as IEnumerable<Entity>;
            if (query is EntityCollection)
                return GetEntities(query as EntityCollection);
            if (query is Entity)
                return GetEntities(query as Entity);
            return GetEntitiesEmpty();
        }

        private static IEnumerable<Entity> GetEntitiesEmpty()
        {
            yield break;
        }

        private static IEnumerable<Entity> GetEntities(Entity entity)
        {
            yield return entity;
        }

        private IEnumerable<Entity> GetEntities(OrganizationRequest request, IEnumerable<object> path)
        {
            foreach (KeyValuePair<string, object> parameter in request.Parameters)
            {
                object value = parameter.Value;
                if (value != null && !path.Contains(value))
                {
                    foreach (Entity entity in GetEntitiesForObject(value, path.Concat(new[] { value })))
                        yield return entity;
                }
            }
        }

        private IEnumerable<Entity> GetEntities(OrganizationResponse response, IEnumerable<object> path)
        {
            foreach (KeyValuePair<string, object> result in response.Results)
            {
                object value = result.Value;
                if (value != null && !path.Contains(value))
                {
                    foreach (Entity entity in GetEntitiesForObject(value, path.Concat(new[] { value })))
                        yield return entity;
                }
            }
        }

        private static IEnumerable<Entity> GetEntities(EntityCollection entities)
        {
            return entities.Entities;
        }
    }
}
