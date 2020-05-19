using Microsoft.Azure.Cosmos.Table;
using MLOps.NET.Entities;
using System.Threading.Tasks;

namespace MLOps.NET.Storage
{
    internal sealed class StorageAccountMetaDataStore : IMetaDataStore
    {
        private readonly CloudStorageAccount storageAccount;

        public StorageAccountMetaDataStore(string connectionString)
        {
            this.storageAccount = EnsureStorageIsCreated.CreateStorageAccountFromConnectionString(connectionString);
        }

        public async Task<Experiment> CreateExperiementAsync(Experiment experiment)
        {
            var table = await GetTable(nameof(Experiment));
            var insertOrMergeOperation = TableOperation.InsertOrMerge(experiment);

            TableResult result = await table.ExecuteAsync(insertOrMergeOperation);

            var insertedExperiement = result.Result as Experiment;

            return insertedExperiement;
        }

        private async Task<CloudTable> GetTable(string tableName)
        {
            var tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());

            CloudTable table = tableClient.GetTableReference(tableName);

            await table.CreateIfNotExistsAsync();

            return table;
        }
    }
}
