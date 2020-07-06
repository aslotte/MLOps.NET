using System;
using System.Collections.Generic;
using System.Text;

namespace MLOps.NET.Clustering.Entities
{
    class ModelOutput
    {
        // Original label (not used during training, just for comparison).
        public uint Label { get; set; }
        // Predicted label from the trainer.
        public uint PredictedLabel { get; set; }
    }
}