using Dynamitey;
using MLOps.NET.Storage;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MLOps.NET.Catalogs
{
    /// <summary>
    /// Operations related to training of a model
    /// </summary>
    public sealed class TrainingCatalog
    {
        private readonly IMetaDataStore metaDataStore;

        /// <summary>
        /// ctor
        /// </summary>
        public TrainingCatalog(IMetaDataStore metaDataStore)
        {
            this.metaDataStore = metaDataStore;
        }

        /// <summary>
        /// Logs all the hyperparameters used to train the model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="runId"></param>
        /// <param name="trainer"></param>
        /// <returns></returns>
        public async Task LogHyperParametersAsync<T>(Guid runId, T trainer) where T : class
        {
            var trainerType = trainer.GetType();
            // All trainers have an Options object which is used to set the parameters for the training.
            var trainerField = trainerType.GetRuntimeFields().FirstOrDefault(f => f.Name.Contains("Options"));
            if (trainerField != null)
            {
                var options = Dynamic.InvokeGet(trainer, trainerField.Name);
                foreach (var optionField in trainerField.FieldType.GetFields())
                {
                    // Tracking only primitive types as of now
                    if (optionField.FieldType.IsPrimitive || optionField.FieldType == typeof(Decimal) || optionField.FieldType == typeof(String))
                    {
                        var value = optionField.GetValue(options);
                        if (value != null)
                            await metaDataStore.LogHyperParameterAsync(runId, optionField.Name, value.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Logs a hyperparameter used to train a mdoel
        /// </summary>
        /// <param name="runId"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task LogHyperParameterAsync(Guid runId, string name, string value)
        {
            await metaDataStore.LogHyperParameterAsync(runId, name, value);
        }
    }
}
