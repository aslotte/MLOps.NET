using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.TypeSystem;
using System;
using System.IO;
using System.Linq;
using System.Text;

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
            {
                throw new Exception($"Type {typeof(T).FullName} does not exist in the app domain");
            }

            var settings = new DecompilerSettings 
            { 
                ThrowOnAssemblyResolveErrors = false 
            };
            var fileName = Path.GetFileName(typeInfo.Assembly.Location);

            var decompiler = new CSharpDecompiler(fileName, settings);

            var definition = decompiler.DecompileTypeAsString(new FullTypeName(typeInfo.FullName));
            definition = definition.Replace($"class {typeof(T).Name}", $"class {className}");

            return ReplaceNamespace(definition);
        }

        private string ReplaceNamespace(string definition)
        {
            var newDefinition = new StringBuilder();

            definition.Split(Environment.NewLine).ToList().ForEach(row =>
            {
                if (row.StartsWith("namespace"))
                {
                    newDefinition.AppendLine("namespace ML.NET.Web.Embedded.Schema");               
                }
                else
                {
                    newDefinition.AppendLine(row);
                }
            });

            return newDefinition.ToString(); ;
        }
    }
}