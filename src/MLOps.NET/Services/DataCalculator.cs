using Microsoft.ML;
using Microsoft.ML.Data;
using MLOps.NET.Entities.Impl;
using MLOps.NET.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace MLOps.NET.Services
{
    internal sealed class DataCalculator : IDataCalculator
    {
        public IEnumerable<DataDistribution> CalculateDataDistributions<T>(IDataView dataView, string columnName, Guid dataColumnId) where T : struct
        {
            var column = dataView.Schema.First(c => c.Name == columnName);

            var allValues = dataView.GetColumn<T>(column);
            var distinctValues = allValues.Distinct();

            foreach (var item in distinctValues)
            {
                yield return  new DataDistribution
                {
                    DataColumnId = dataColumnId,
                    Value = item.ToString(),
                    Count = allValues.Count(v => v.ToString() == item.ToString())
                };
            }
        }

        public string CalculateDataHash(IDataView dataView)
        {
            using SHA256 sha256Hash = SHA256.Create();
            using var stream = new MemoryStream();

            new MLContext().Data.SaveAsBinary(dataView, stream);
            var bytes = sha256Hash.ComputeHash(stream);
            return Convert.ToBase64String(bytes);
        }
    }
}
