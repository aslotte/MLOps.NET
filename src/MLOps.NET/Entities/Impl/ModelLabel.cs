using System;

namespace MLOps.NET.Entities.Impl
{
    /// <summary>
    /// Label associated with a given run
    /// </summary>
    public sealed class ModelLabel
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="runArtifactId"></param>
        /// <param name="labelName"></param>
        /// <param name="labelValue"></param>
        public ModelLabel(Guid runArtifactId, string labelName, string labelValue)
        {
            ModelLabelId = Guid.NewGuid();
            LabelName = labelName;
            LabelValue = labelValue;
            RunArtifactId = runArtifactId;
        }

        /// <summary>
        /// ModelLabelId
        /// </summary>
        public Guid ModelLabelId { get; set; }

        /// <summary>
        /// Name of label. E.g Department, Model type, Status
        /// </summary>
        public string LabelName { get; set; }

        /// <summary>
        /// Label Value. E.g Engineering, Multi Class, Ready
        /// </summary>
        public string LabelValue { get; set; }

        /// <summary>
        /// RunArtifactId
        /// </summary>
        public Guid RunArtifactId { get; set; }
    }
}
