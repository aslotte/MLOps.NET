using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.TypeSystem;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MLOps.NET.Storage.Deployments
{
    ///<inheritdoc cref="ISchemaGenerator"/>
    internal sealed class SchemaGenerator : ISchemaGenerator
    {
        ///<inheritdoc cref="ISchemaGenerator"/>
        public string GenerateDefinition<T>(string className) where T : class
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            var assemblies = currentDomain.GetAssemblies();
            var typeInfo = assemblies.SelectMany(a => a.DefinedTypes).ToList()
                                     .Where(t => t.AssemblyQualifiedName.ToLowerInvariant() == typeof(T).AssemblyQualifiedName.ToLowerInvariant())
                                     .FirstOrDefault();

            if (typeInfo == null)
                throw new Exception($"Type {typeof(T).FullName} does not exist in the app domain");

            var decompiler = new CSharpDecompiler(Path.GetFileName(typeInfo.Assembly.Location), new DecompilerSettings() { ThrowOnAssemblyResolveErrors = false });
            return decompiler.DecompileTypeAsString(new FullTypeName(typeInfo.FullName)).Replace($"class {typeof(T).Name}", $"class {className}");
        }
    }
}
