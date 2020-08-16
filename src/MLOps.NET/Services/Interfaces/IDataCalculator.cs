using Microsoft.ML;
using MLOps.NET.Entities.Impl;
using System;
using System.Collections.Generic;

namespace MLOps.NET.Services.Interfaces
{
    /// <summary>
    /// Service exposing data related operations
    /// </summary>
    public interface IDataCalculator
    {
        /// <summary>
        /// Calculates the data distribution for a column
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataView"></param>
        /// <param name="columnName"></param>
        /// <param name="dataColumnId"></param>
        /// <returns></returns>
        IEnumerable<DataDistribution> CalculateDataDistributions<T>(IDataView dataView, string columnName, Guid dataColumnId) where T : struct;

        /// <summary>
        /// Calculates a unique hash for an IDataView
        /// </summary>
        /// <param name="dataView"></param>
        /// <returns></returns>
        string CalculateDataHash(IDataView dataView);
    }
}
