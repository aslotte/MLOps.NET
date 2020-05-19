using Microsoft.Azure.Cosmos.Table;
using System;

namespace MLOps.NET.Entities
{
    internal sealed class Run : TableEntity
    {
        public Guid Id { get; set; }

        public DateTime RunDate { get; set; }
    }
}
