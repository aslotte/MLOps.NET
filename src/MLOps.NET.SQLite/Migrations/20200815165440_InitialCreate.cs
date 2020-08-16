using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MLOps.NET.SQLite.Migrations
{
    /// <summary>
    /// Data Distribution
    /// </summary>
    public partial class InitialCreate : Migration
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="migrationBuilder"></param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Data",
                columns: table => new
                {
                    DataId = table.Column<Guid>(nullable: false),
                    RunId = table.Column<Guid>(nullable: false),
                    DataHash = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Data", x => x.DataId);
                    table.ForeignKey(
                        name: "FK_Data_Run_RunId",
                        column: x => x.RunId,
                        principalTable: "Run",
                        principalColumn: "RunId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeploymentTarget",
                columns: table => new
                {
                    DeploymentTargetId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeploymentTarget", x => x.DeploymentTargetId);
                });

            migrationBuilder.CreateTable(
                name: "Experiment",
                columns: table => new
                {
                    ExperimentId = table.Column<Guid>(nullable: false),
                    ExperimentName = table.Column<string>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Experiment", x => x.ExperimentId);
                });

            migrationBuilder.CreateTable(
                name: "RegisteredModel",
                columns: table => new
                {
                    RegisteredModelId = table.Column<Guid>(nullable: false),
                    RunArtifactId = table.Column<Guid>(nullable: false),
                    ExperimentId = table.Column<Guid>(nullable: false),
                    RunId = table.Column<Guid>(nullable: false),
                    RegisteredDate = table.Column<DateTime>(nullable: false),
                    RegisteredBy = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    Version = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegisteredModel", x => x.RegisteredModelId);
                    table.ForeignKey(
                        name: "FK_RegisteredModel_Run_RunId",
                        column: x => x.RunId,
                        principalTable: "Run",
                        principalColumn: "RunId",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_RegisteredModel_Experiment_ExperimentId",
                        column: x => x.ExperimentId,
                        principalTable: "Experiment",
                        principalColumn: "ExperimentId",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_RegisteredModel_RunArtifact_RunArtifactId",
                        column: x => x.RunArtifactId,
                        principalTable: "RunArtifact",
                        principalColumn: "RunArtifactId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DataSchema",
                columns: table => new
                {
                    DataSchemaId = table.Column<Guid>(nullable: false),
                    DataId = table.Column<Guid>(nullable: false),
                    ColumnCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataSchema", x => x.DataSchemaId);
                    table.ForeignKey(
                        name: "FK_DataSchema_Data_DataId",
                        column: x => x.DataId,
                        principalTable: "Data",
                        principalColumn: "DataId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Run",
                columns: table => new
                {
                    RunId = table.Column<Guid>(nullable: false),
                    RunDate = table.Column<DateTime>(nullable: false),
                    ExperimentId = table.Column<Guid>(nullable: false),
                    TrainingTime = table.Column<TimeSpan>(nullable: true),
                    GitCommitHash = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Run", x => x.RunId);
                    table.ForeignKey(
                        name: "FK_Run_Experiment_ExperimentId",
                        column: x => x.ExperimentId,
                        principalTable: "Experiment",
                        principalColumn: "ExperimentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Deployment",
                columns: table => new
                {
                    DeploymentId = table.Column<Guid>(nullable: false),
                    DeploymentTargetId = table.Column<Guid>(nullable: false),
                    RegisteredModelId = table.Column<Guid>(nullable: false),
                    DeploymentDate = table.Column<DateTime>(nullable: false),
                    DeployedBy = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deployment", x => x.DeploymentId);
                    table.ForeignKey(
                        name: "FK_Deployment_DeploymentTarget_DeploymentTargetId",
                        column: x => x.DeploymentTargetId,
                        principalTable: "DeploymentTarget",
                        principalColumn: "DeploymentTargetId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Deployment_RegisteredModel_RegisteredModelId",
                        column: x => x.RegisteredModelId,
                        principalTable: "RegisteredModel",
                        principalColumn: "RegisteredModelId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DataColumn",
                columns: table => new
                {
                    DataColumnId = table.Column<Guid>(nullable: false),
                    DataSchemaId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataColumn", x => x.DataColumnId);
                    table.ForeignKey(
                        name: "FK_DataColumn_DataSchema_DataSchemaId",
                        column: x => x.DataSchemaId,
                        principalTable: "DataSchema",
                        principalColumn: "DataSchemaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConfusionMatrix",
                columns: table => new
                {
                    ConfusionMatrixEntityId = table.Column<Guid>(nullable: false),
                    RunId = table.Column<Guid>(nullable: false),
                    SerializedMatrix = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfusionMatrix", x => x.ConfusionMatrixEntityId);
                    table.ForeignKey(
                        name: "FK_ConfusionMatrix_Run_RunId",
                        column: x => x.RunId,
                        principalTable: "Run",
                        principalColumn: "RunId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HyperParameter",
                columns: table => new
                {
                    HyperParameterId = table.Column<Guid>(nullable: false),
                    ParameterName = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: false),
                    RunId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HyperParameter", x => x.HyperParameterId);
                    table.ForeignKey(
                        name: "FK_HyperParameter_Run_RunId",
                        column: x => x.RunId,
                        principalTable: "Run",
                        principalColumn: "RunId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Metric",
                columns: table => new
                {
                    MetricId = table.Column<Guid>(nullable: false),
                    MetricName = table.Column<string>(nullable: false),
                    Value = table.Column<double>(nullable: false),
                    RunId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Metric", x => x.MetricId);
                    table.ForeignKey(
                        name: "FK_Metric_Run_RunId",
                        column: x => x.RunId,
                        principalTable: "Run",
                        principalColumn: "RunId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RunArtifact",
                columns: table => new
                {
                    RunArtifactId = table.Column<Guid>(nullable: false),
                    RunId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RunArtifact", x => x.RunArtifactId);
                    table.ForeignKey(
                        name: "FK_RunArtifact_Run_RunId",
                        column: x => x.RunId,
                        principalTable: "Run",
                        principalColumn: "RunId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConfusionMatrix_RunId",
                table: "ConfusionMatrix",
                column: "RunId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DataColumn_DataSchemaId",
                table: "DataColumn",
                column: "DataSchemaId");

            migrationBuilder.CreateIndex(
                name: "IX_DataSchema_DataId",
                table: "DataSchema",
                column: "DataId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Deployment_DeploymentTargetId",
                table: "Deployment",
                column: "DeploymentTargetId");

            migrationBuilder.CreateIndex(
                name: "IX_Deployment_RegisteredModelId",
                table: "Deployment",
                column: "RegisteredModelId");

            migrationBuilder.CreateIndex(
                name: "IX_HyperParameter_RunId",
                table: "HyperParameter",
                column: "RunId");

            migrationBuilder.CreateIndex(
                name: "IX_Metric_RunId",
                table: "Metric",
                column: "RunId");

            migrationBuilder.CreateIndex(
                name: "IX_Run_ExperimentId",
                table: "Run",
                column: "ExperimentId");

            migrationBuilder.CreateIndex(
                name: "IX_RunArtifact_RunId",
                table: "RunArtifact",
                column: "RunId");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="migrationBuilder"></param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfusionMatrix");

            migrationBuilder.DropTable(
                name: "DataColumn");

            migrationBuilder.DropTable(
                name: "Deployment");

            migrationBuilder.DropTable(
                name: "HyperParameter");

            migrationBuilder.DropTable(
                name: "Metric");

            migrationBuilder.DropTable(
                name: "RunArtifact");

            migrationBuilder.DropTable(
                name: "DataSchema");

            migrationBuilder.DropTable(
                name: "DeploymentTarget");

            migrationBuilder.DropTable(
                name: "RegisteredModel");

            migrationBuilder.DropTable(
                name: "Run");

            migrationBuilder.DropTable(
                name: "Data");

            migrationBuilder.DropTable(
                name: "Experiment");
        }
    }
}
