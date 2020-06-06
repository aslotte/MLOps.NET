using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MLOps.NET.Storage
{
    public interface IModelRepository
    {
        Task UploadModelAsync(Guid runId, string filePath);
    }
}
