using Microsoft.EntityFrameworkCore.Migrations;
using MLOps.NET.Entities.Impl;

namespace MLOps.NET.SQLServer.Migrations
{
    /// <summary>
    /// 
    /// </summary>
    public partial class AddFKConstraints : Migration
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="migrationBuilder"></param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RunArtifact_RegisteredModel_RunArtifactId",
                table: "RunArtifact");

            migrationBuilder.AddForeignKey("FK_Data_Run_RunId", nameof(Data), "RunId", nameof(Run), principalColumn: "RunId", onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey("FK_RegisteredModel_Run_RunId", nameof(RegisteredModel), "RunId", nameof(Run), principalColumn: "RunId");
            migrationBuilder.AddForeignKey("FK_RegisteredModel_Experiment_ExperimentId", nameof(RegisteredModel), "ExperimentId", nameof(Experiment), principalColumn: "ExperimentId");
            migrationBuilder.AddForeignKey("FK_RegisteredModel_RunArtifact_RunArtifactId", nameof(RegisteredModel), "RunArtifactId", nameof(RunArtifact), principalColumn: "RunArtifactId", onDelete: ReferentialAction.Cascade);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="migrationBuilder"></param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_RunArtifact_RegisteredModel_RunArtifactId",
                table: "RunArtifact",
                column: "RunArtifactId",
                principalTable: "RegisteredModel",
                principalColumn: "RegisteredModelId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.DropForeignKey("FK_Data_Run_RunId", "Data");
            migrationBuilder.DropForeignKey("FK_RegisteredModel_Run_RunId", "RegisteredModel");
            migrationBuilder.DropForeignKey("FK_RegisteredModel_Experiment_ExperimentId", "RegisteredModel");
            migrationBuilder.DropForeignKey("FK_RegisteredModel_RunArtifact_RunArtifactId", "RegisteredModel");
        }
    }
}
