using Microsoft.ML.Data;

namespace MLOps.NET.Tests.Common.Data
{
    public class ProductReview
    {
        [LoadColumn(0)]
        public bool Sentiment;

        [LoadColumn(1)]
        public string Review;
    }
}
