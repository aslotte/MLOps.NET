using Microsoft.EntityFrameworkCore;
using MLOps.NET.Entities.Impl;
using MLOps.NET.Storage.Database;
using MLOps.NET.Storage.Interfaces;
using MLOps.NET.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MLOps.NET.Storage
{
    ///<inheritdoc cref="IRunRepository"/>
    public sealed class RunRepository : IRunRepository
    {
        private readonly IDbContextFactory contextFactory;
        private readonly IClock clock;

        ///<inheritdoc cref="IRunRepository"/>
        public RunRepository(IDbContextFactory contextFactory, IClock clock)
        {
            this.contextFactory = contextFactory;
            this.clock = clock;
        }

        ///<inheritdoc cref="IRunRepository"/>
        public async Task<Guid> CreateRunAsync(Guid experimentId, string gitCommitHash = "")
        {
            using var db = this.contextFactory.CreateDbContext();
            var run = new Run(experimentId)
            {
                GitCommitHash = gitCommitHash
            };

            await db.Runs.AddAsync(run);
            await db.SaveChangesAsync();

            return run.RunId;
        }

        ///<inheritdoc cref="IRunRepository"/>

        public Run GetRun(Guid runId)
        {
            using var db = this.contextFactory.CreateDbContext();
            var run = db.Runs
                .Include(x => x.RunArtifacts)
                .FirstOrDefault(x => x.RunId == runId);

            PopulateRun(db, run);
            return run;
        }

        ///<inheritdoc cref="IRunRepository"/>
        public Run GetRun(string commitHash)
        {
            using var db = this.contextFactory.CreateDbContext();
            var run = db.Runs.FirstOrDefault(x => x.GitCommitHash == commitHash);

            PopulateRun(db, run);
            return run;
        }

        ///<inheritdoc cref="IRunRepository"/>
        public List<Run> GetRuns(Guid experimentId)
        {
            using var db = this.contextFactory.CreateDbContext();
            var runs = db.Runs.Where(x => x.ExperimentId == experimentId).ToList();
            runs.ForEach(run => PopulateRun(db, run));

            return runs;
        }

        ///<inheritdoc cref="IRunRepository"/>
        public async Task SetTrainingTimeAsync(Guid runId, TimeSpan timeSpan)
        {
            using var db = this.contextFactory.CreateDbContext();
            var existingRun = db.Runs.FirstOrDefault(x => x.RunId == runId);
            if (existingRun == null) throw new InvalidOperationException($"The run with id {runId} does not exist");

            existingRun.TrainingTime = timeSpan;

            await db.SaveChangesAsync();
        }

        ///<inheritdoc cref="IRunRepository"/>
        public async Task CreateRunArtifact(Guid runId, string name)
        {
            using var db = this.contextFactory.CreateDbContext();
            var runArtifact = new RunArtifact
            {
                RunId = runId,
                Name = name
            };

            db.RunArtifacts.Add(runArtifact);

            await db.SaveChangesAsync();
        }

        ///<inheritdoc cref="IRunRepository"/>
        public List<RunArtifact> GetRunArtifacts(Guid runId)
        {
            using var db = this.contextFactory.CreateDbContext();
            return db.RunArtifacts.Where(x => x.RunId == runId).ToList();
        }

        ///<inheritdoc cref="IRunRepository"/>
        public async Task CreateRegisteredModelAsync(Guid experimentId, Guid runArtifactId, string registeredBy)
        {
            using var db = this.contextFactory.CreateDbContext();

            var runArtifact = db.RunArtifacts.FirstOrDefault(x => x.RunArtifactId == runArtifactId);
            if (runArtifact == null)
            {
                throw new InvalidOperationException($"The RunArtifact with id {runArtifactId} does not exist. Unable to register a model");
            }

            var run = db.Runs.FirstOrDefault(x => x.RunId == runArtifact.RunId);
            var registeredModels = db.RegisteredModels.Where(x => x.ExperimentId == experimentId);

            var version = registeredModels.Count() > 0 ? registeredModels.Max(x => x.Version) + 1 : 1;

            var registeredModel = new RegisteredModel
            {
                RunArtifactId = runArtifactId,
                RegisteredBy = registeredBy,
                RegisteredDate = this.clock.UtcNow,
                Version = version,
                ExperimentId = experimentId,
                RunId = run.RunId
            };

            db.RegisteredModels.Add(registeredModel);

            await db.SaveChangesAsync();
        }

        ///<inheritdoc cref="IRunRepository"/>
        public List<RegisteredModel> GetRegisteredModels(Guid experimentId)
        {
            using var db = this.contextFactory.CreateDbContext();

            var registeredModels = db.RegisteredModels.Where(x => x.ExperimentId == experimentId);

            registeredModels.ToList().ForEach(x => PopulateRegisteredModel(db, x));

            return registeredModels.ToList();
        }

        ///<inheritdoc cref="IRunRepository"/>
        public RegisteredModel GetLatestRegisteredModel(Guid experimentId)
        {
            using var db = this.contextFactory.CreateDbContext();

            var registeredModel = db.RegisteredModels
                .Where(x => x.ExperimentId == experimentId)
                .OrderByDescending(x => x.Version)
                .FirstOrDefault();

            PopulateRegisteredModel(db, registeredModel);

            return registeredModel;
        }

        private void PopulateRegisteredModel(IMLOpsDbContext db, RegisteredModel registeredModel)
        {
            registeredModel.RunArtifact = db.RunArtifacts.First(x => x.RunArtifactId == registeredModel.RunArtifactId);
            registeredModel.Run = db.Runs.First(x => x.RunId == registeredModel.RunId);
            registeredModel.Experiment = db.Experiments.First(x => x.ExperimentId == registeredModel.ExperimentId);
        }

        private void PopulateRun(IMLOpsDbContext db, Run run)
        {
            if (run == null) return;

            run.HyperParameters = db.HyperParameters.Where(x => x.RunId == run.RunId).ToList();
            run.Metrics = db.Metrics.Where(x => x.RunId == run.RunId).ToList();
            run.ConfusionMatrix = db.ConfusionMatrices.FirstOrDefault(x => x.RunId == run.RunId);
        }

        ///<inheritdoc cref="IRunRepository"/>
        public async Task CreateModelLabel(Guid registeredModelId, string labelName, string labelValue)
        {
            using var db = this.contextFactory.CreateDbContext();

            var registeredModel = db.RegisteredModels.FirstOrDefault(x => x.RegisteredModelId == registeredModelId);
            if (registeredModel == null)
            {
                throw new InvalidOperationException($"The registered model with id {registeredModelId} does not exist. Unable to associate a label");
            }

            var modelLabel = new ModelLabel
            {
                RegisteredModelId = registeredModelId,
                LabelName = labelName,
                LabelValue = labelValue
            };

            db.ModelLabels.Add(modelLabel);

            await db.SaveChangesAsync();

        }

        ///<inheritdoc cref="IRunRepository"/>
        public List<ModelLabel> GetModelLabels(Guid registeredModelId)
        {
            using var db = this.contextFactory.CreateDbContext();

            var modelLabels = db.ModelLabels.Where(x => x.RegisteredModelId == registeredModelId);

            return modelLabels.ToList();
        }
    }
}
