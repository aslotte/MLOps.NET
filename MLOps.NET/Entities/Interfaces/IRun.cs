using System;

namespace MLOps.NET.Entities.Entities
{
    public interface IRun
    {
        Guid Id { get; set; }
        DateTime RunDate { get; set; }
    }
}