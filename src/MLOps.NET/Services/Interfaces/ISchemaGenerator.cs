namespace MLOps.NET.Services.Interfaces
{
    /// <summary>
    /// Schema generator
    /// </summary>
    public interface ISchemaGenerator
    {
        /// <summary>
        /// Searches the entire app domain to return the class definition as a string, using ilSpy
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="className"></param>
        /// <returns></returns>
        string GenerateDefinition<T>(string className) where T : class;
    }
}