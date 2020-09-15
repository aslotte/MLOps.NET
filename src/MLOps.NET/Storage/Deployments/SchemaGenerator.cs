using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.TypeSystem;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MLOps.NET.Storage.Deployments
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class SchemaGenerator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public string GenerateDefinition(string className)
        {
            var assembly = Assembly.GetCallingAssembly();
            var typeFullName = "";
            foreach (var item in assembly.DefinedTypes.ToList())
            {
                if (item.Name.ToLowerInvariant() == className.ToLowerInvariant())
                {
                    typeFullName = item.FullName;
                    break;
                }
            }
            var decompiler = new CSharpDecompiler(Path.GetFileName(assembly.Location), new DecompilerSettings() { ThrowOnAssemblyResolveErrors = false });
            return decompiler.DecompileTypeAsString(new FullTypeName(typeFullName));
        }
    }
}
