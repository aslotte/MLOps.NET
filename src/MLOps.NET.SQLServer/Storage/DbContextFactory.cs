using MLOps.NET.SQLServer.Storage.Interfaces;

namespace MLOps.NET.SQLServer.Storage
{
    /// <summary>
    /// Factory to create Db Context
    /// </summary>
    internal interface IDbContextFactory
    {
        /// <summary>
        /// Creates a DbContext based on name
        /// </summary>
        /// <returns></returns>
        IMLOpsDbContext CreateDbContext();
    }

    internal sealed class DbContextFactory : IDbContextFactory
    {
        private readonly string connectionString;

        public DbContextFactory(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public IMLOpsDbContext CreateDbContext() => new MLOpsDbContext(this.connectionString);
    }
}
