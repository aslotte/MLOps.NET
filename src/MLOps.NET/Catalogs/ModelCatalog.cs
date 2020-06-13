using MLOps.NET.Storage;
using System;
using System.Threading.Tasks;

namespace MLOps.NET.Catalogs
{
    /// <summary>
    /// Operations related to a model
    /// </summary>
    public sealed class ModelCatalog
    {
        private readonly IModelRepository modelRepository;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="modelRepository"></param>
        public ModelCatalog(IModelRepository modelRepository)
        {
            this.modelRepository = modelRepository;
        }

        ///<inheritdoc/>
        public async Task UploadAsync(Guid runId, string filePath)
        {
            await modelRepository.UploadModelAsync(runId, filePath);
        }
    }
}
