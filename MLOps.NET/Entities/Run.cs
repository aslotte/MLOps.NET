using Microsoft.Azure.Cosmos.Table;
using System;

namespace MLOps.NET.Entities
{
    internal sealed class Run : TableEntity
    {
        public Run(Guid experimentId)
        {
            RowKey = experimentId.ToString();
            RunDate = DateTime.Now;
        }

        public Guid Id { get; set; }

        public DateTime RunDate { get; set; }
    }
}
