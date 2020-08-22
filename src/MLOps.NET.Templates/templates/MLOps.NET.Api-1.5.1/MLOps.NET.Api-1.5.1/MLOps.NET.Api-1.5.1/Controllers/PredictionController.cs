using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.ML;
using MLOps.NET.ApiTemplate.Models;

namespace MLOps.NET.ApiTemplate.Controllers
{
    /// <summary>
    /// Entry point for an ML.NET Prediction
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PredictionController : ControllerBase
    {
        private readonly PredictionEnginePool<ModelInput, ModelOutput> predictionEnginePool;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="predictionEnginePool"></param>
        public PredictionController(PredictionEnginePool<ModelInput, ModelOutput> predictionEnginePool)
        {
            this.predictionEnginePool = predictionEnginePool;
        }

        /// <summary>
        /// Make a prediction
        /// </summary>
        /// <param name="modelInput"></param>
        /// <returns></returns>
        [HttpPost]
        public ModelOutput Predict(ModelInput modelInput)
        {
            return this.predictionEnginePool.Predict(modelInput);
        }
    }
}
