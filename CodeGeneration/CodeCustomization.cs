using Microsoft.Crm.Services.Utility;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;

namespace XrmLazyLoading.CodeGeneration
{
    public class CodeCustomization : ICustomizeCodeDomService
    {
        private static readonly Type ClientEntityClassBaseType = typeof(EntityWrapper);
        private static readonly Type ClientServiceContextBaseType = typeof(OrganizationServiceContextWrapper);
        private static readonly Type EntityClassBaseType = typeof(Entity);
        private static readonly Type ServiceContextBaseType = typeof(OrganizationServiceContext);
        private readonly string _namespace;

        public CodeCustomization(IDictionary<string, string> parameters)
        {
            parameters.TryGetValue("namespace", out _namespace);
        }

        public void CustomizeCodeDom(CodeCompileUnit codeUnit, IServiceProvider services)
        {
            CodeNamespace codeNamespace = codeUnit.Namespaces
                .OfType<CodeNamespace>()
                .FirstOrDefault(n => n.Name == _namespace);

            if (codeNamespace != null)
            {
                ReplaceServiceContextBaseType(codeNamespace);
                ReplaceEntityBaseType(codeNamespace);
            }
        }

        private static void ReplaceEntityBaseType(CodeNamespace codeNamespace)
        {
            foreach (CodeTypeDeclaration entityClass in GetClassesInheriting(codeNamespace, EntityClassBaseType))
                ReplaceBaseType(entityClass, EntityClassBaseType, ClientEntityClassBaseType);
        }

        private static void ReplaceServiceContextBaseType(CodeNamespace codeNamespace)
        {
            CodeTypeDeclaration codeTypeDeclaration = GetClassesInheriting(codeNamespace, ServiceContextBaseType).SingleOrDefault();
            ReplaceBaseType(codeTypeDeclaration, ServiceContextBaseType, ClientServiceContextBaseType);
        }

        private static IEnumerable<CodeTypeDeclaration> GetClassesInheriting(CodeNamespace codeNamespace, Type classBaseType)
        {
            return codeNamespace.Types
                .Cast<CodeTypeDeclaration>()
                .Where(type => type.BaseTypes.Cast<CodeTypeReference>().Any(typeRef => typeRef.BaseType == TypeRef(classBaseType).BaseType))
                .ToList();
        }

        private static void ReplaceBaseType(CodeTypeDeclaration entityClass, Type oldType, Type newType)
        {
            CodeTypeReference oldBaseType = entityClass.BaseTypes.Cast<CodeTypeReference>()
                .FirstOrDefault(typeRef => typeRef.BaseType == TypeRef(oldType).BaseType);
            if (oldBaseType != null)
                entityClass.BaseTypes.Remove(oldBaseType);
            entityClass.BaseTypes.Insert(0, TypeRef(newType));
        }

        private static CodeTypeReference TypeRef(Type type, params CodeTypeReference[] typeArguments)
        {
            return TypeRef(type.FullName, typeArguments);
        }

        private static CodeTypeReference TypeRef(string typeFullName, params CodeTypeReference[] typeArguments)
        {
            return new CodeTypeReference(typeFullName, typeArguments);
        }
    }
}
