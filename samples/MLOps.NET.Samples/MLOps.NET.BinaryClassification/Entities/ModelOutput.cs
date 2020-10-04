namespace MLOps.NET.BinaryClassification.Entities
{
    public class ModelOutput
    {
        public string PredictedLabel { get; set; }

        public float Score { get; set; }

        public float Probability { get; set; }
    }
}
