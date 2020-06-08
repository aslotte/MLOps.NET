using System;
using System.Threading.Tasks;
using MLOps.NET.Storage;

namespace MLOps.NET.Tests.Mocks.Storage
{
    public class MockModelRepository : IModelRepository
    {
        public async Task UploadModelAsync(Guid runId, string filePath)
        {
        }
    }
}