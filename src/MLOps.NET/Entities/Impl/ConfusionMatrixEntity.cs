using MLOps.NET.Entities.Entities;
using System;

namespace MLOps.NET.Entities.Impl
{
    ///<inheritdoc cref="IConfusionMatrixEntity"/>
    public sealed class ConfusionMatrixEntity : IConfusionMatrixEntity
    {
        ///<inheritdoc cref="IConfusionMatrixEntity"/>
        public ConfusionMatrixEntity() { }

        ///<inheritdoc cref="IConfusionMatrixEntity"/>
        public ConfusionMatrixEntity(Guid runId)
        {
            RunId = runId;
            Id = Guid.NewGuid();
        }

        ///<inheritdoc cref="IConfusionMatrixEntity"/>
        public Guid Id { get; set; }

        ///<inheritdoc cref="IConfusionMatrixEntity"/>
        public Guid RunId { get ; set ; }

        ///<inheritdoc cref="IConfusionMatrixEntity"/>
        public string SerializedMatrix { get ; set ; }
    }
}
