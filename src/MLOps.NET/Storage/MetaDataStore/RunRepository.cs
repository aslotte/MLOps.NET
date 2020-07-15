using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using MLOps.NET.Entities.Impl;
using MLOps.NET.Storage.Database;
using MLOps.NET.Storage.Interfaces;
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

        ///<inheritdoc cref="IRunRepository"/>
        public RunRepository(IDbContextFactory contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        ///<inheritdoc cref="IRunRepository"/>
        public async Task<Guid> CreateRunAsync(Guid experimentId, string gitCommitHash = "")
        {
            using (var db = this.contextFactory.CreateDbContext())
            {
                var run = new Run(experimentId)
                {
                    GitCommitHash = gitCommitHash
                };

                await db.Runs.AddAsync(run);
                await db.SaveChangesAsync();

                return run.RunId;
            }
        }

        ///<inheritdoc cref="IRunRepository"/>

        public Run GetRun(Guid runId)
        {
            using (var db = this.contextFactory.CreateDbContext())
            {
                var run = db.Runs
                    .Include(x => x.RunArtifacts)
                    .FirstOrDefault(x => x.RunId == runId);

                PopulateRun(db, run);
                return run;
            }
        }

        ///<inheritdoc cref="IRunRepository"/>
        public Run GetRun(string commitHash)
        {
            using (var db = this.contextFactory.CreateDbContext())
            {
                var run = db.Runs.FirstOrDefault(x => x.GitCommitHash == commitHash);

                PopulateRun(db, run);
                return run;
            }
        }

        ///<inheritdoc cref="IRunRepository"/>
        public List<Run> GetRuns(Guid experimentId)
        {
            using (var db = this.contextFactory.CreateDbContext())
            {
                var runs = db.Runs.Where(x => x.ExperimentId == experimentId).ToList();
                runs.ForEach(run => PopulateRun(db, run));

                return runs;
            }
        }

        ///<inheritdoc cref="IRunRepository"/>
        public async Task SetTrainingTimeAsync(Guid runId, TimeSpan timeSpan)
        {
            using (var db = this.contextFactory.CreateDbContext())
            {
                var existingRun = db.Runs.FirstOrDefault(x => x.RunId == runId);
                if (existingRun == null) throw new InvalidOperationException($"The run with id {runId} does not exist");

                existingRun.TrainingTime = timeSpan;

                await db.SaveChangesAsync();
            }
        }

        ///<inheritdoc cref="IRunRepository"/>
        public async Task CreateRunArtifact(Guid runId, string name)
        {
            using (var db = this.contextFactory.CreateDbContext())
            {
                var runArtifact = new RunArtifact
                {
                    RunId = runId,
                    Name = name
                };

                db.RunArtifacts.Add(runArtifact);

                await db.SaveChangesAsync();
            }
        }

        ///<inheritdoc cref="IRunRepository"/>
        public List<RunArtifact> GetRunArtifacts(Guid runId)
        {
            using (var db = this.contextFactory.CreateDbContext())
            {
                return db.RunArtifacts.Where(x => x.RunId == runId).ToList();
            }
        }

        public async Task CreateRegisteredModel(Guid runArtifactId, string registeredBy)
        {
            using (var db = this.contextFactory.CreateDbContext())
            {
                var artifact = db.RunArtifacts.FirstOrDefault(x => x.RunArtifactId == runArtifactId);

                var registeredModels = db.RegisteredModels
                    .Where(x => x.RunArtifact.RunId == artifact.RunId);

                var version = registeredModels.Any() ? registeredModels.Max(x => x.Version) + 1 : 1;

                var registeredModel = new RegisteredModel
                {
                    RunArtifactId = runArtifactId,
                    RegisteredBy = registeredBy,
                    RegisteredDate = DateTime.UtcNow,
                    Version = version
                };

                db.RegisteredModels.Add(registeredModel);

                await db.SaveChangesAsync();
            }
        }

        private void PopulateRun(IMLOpsDbContext db, Run run)
        {
            if (run == null) return;

            run.HyperParameters = db.HyperParameters.Where(x => x.RunId == run.RunId).ToList();
            run.Metrics = db.Metrics.Where(x => x.RunId == run.RunId).ToList();
            run.ConfusionMatrix = db.ConfusionMatrices.FirstOrDefault(x => x.RunId == run.RunId);
        }
    }
}
