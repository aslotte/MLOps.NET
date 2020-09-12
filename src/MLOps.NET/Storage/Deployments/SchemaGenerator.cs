using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MLOps.NET.Storage.Deployments
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class SchemaGenerator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="className"></param>
        /// <param name="mLTaskType"></param>
        /// <returns></returns>
        public string GenerateDefinition(string className, MLTaskType mLTaskType)
        {
            var modelOutput = string.Empty;
            switch(mLTaskType)
            {
                case MLTaskType.BinaryClassification:
                    modelOutput = File.ReadAllText("Templates/BinaryClassificatonModelOutput.cs");
                    break;
            }

            return modelOutput;
        }
    }
}
