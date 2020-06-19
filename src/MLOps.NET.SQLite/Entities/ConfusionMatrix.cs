using MLOps.NET.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MLOps.NET.SQLite.Entities
{
    internal sealed class ConfusionMatrix : IConfusionMatrix
    {
        public ConfusionMatrix() { }
        public ConfusionMatrix(Guid runId, int numberOfClasses, IReadOnlyList<double> perClassPrecision, IReadOnlyList<double> perClassRecall, IReadOnlyList<IReadOnlyList<double>> counts)
        {
            RunId = runId;
            NumberOfClasses = numberOfClasses;
            PerClassPrecision = perClassPrecision;
            PerClassRecall = perClassRecall;
            Counts = counts;
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public Guid RunId { get ; set ; }
        public int NumberOfClasses { get ; set ; }

        [NotMapped]
        public IReadOnlyList<double> PerClassPrecision { get ; set ; }

        [NotMapped]
        public IReadOnlyList<double> PerClassRecall { get ; set ; }

        [NotMapped]
        public IReadOnlyList<IReadOnlyList<double>> Counts { get; set; }

        public string SerializedDetails { get ; set ; }
    }
}
