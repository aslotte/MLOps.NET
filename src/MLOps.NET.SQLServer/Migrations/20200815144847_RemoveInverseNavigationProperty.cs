using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MLOps.NET.SQLServer.Migrations
{
    public partial class RemoveInverseNavigationProperty : Migration
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        protected override void Up(MigrationBuilder migrationBuilder)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deployment_Experiment_ExperimentId",
                table: "Deployment");

            migrationBuilder.DropForeignKey(
                name: "FK_Deployment_RegisteredModel_RegisteredModelId",
                table: "Deployment");

            migrationBuilder.DropForeignKey(
                name: "FK_RegisteredModel_Experiment_ExperimentId",
                table: "RegisteredModel");

            migrationBuilder.DropForeignKey(
                name: "FK_RegisteredModel_RunArtifact_RunArtifactId",
                table: "RegisteredModel");

            migrationBuilder.DropForeignKey(
                name: "FK_RegisteredModel_Run_RunId",
                table: "RegisteredModel");

            migrationBuilder.DropIndex(
                name: "IX_RegisteredModel_ExperimentId",
                table: "RegisteredModel");

            migrationBuilder.DropIndex(
                name: "IX_RegisteredModel_RunArtifactId",
                table: "RegisteredModel");

            migrationBuilder.DropIndex(
                name: "IX_RegisteredModel_RunId",
                table: "RegisteredModel");

            migrationBuilder.DropIndex(
                name: "IX_Deployment_ExperimentId",
                table: "Deployment");

            migrationBuilder.DropColumn(
                name: "ExperimentId",
                table: "Deployment");

            migrationBuilder.AddForeignKey(
                name: "FK_Deployment_RegisteredModel_RegisteredModelId",
                table: "Deployment",
                column: "RegisteredModelId",
                principalTable: "RegisteredModel",
                principalColumn: "RegisteredModelId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RunArtifact_RegisteredModel_RunArtifactId",
                table: "RunArtifact",
                column: "RunArtifactId",
                principalTable: "RegisteredModel",
                principalColumn: "RegisteredModelId",
                onDelete: ReferentialAction.Cascade);
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        protected override void Down(MigrationBuilder migrationBuilder)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deployment_RegisteredModel_RegisteredModelId",
                table: "Deployment");

            migrationBuilder.DropForeignKey(
                name: "FK_RunArtifact_RegisteredModel_RunArtifactId",
                table: "RunArtifact");

            migrationBuilder.AddColumn<Guid>(
                name: "ExperimentId",
                table: "Deployment",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_RegisteredModel_ExperimentId",
                table: "RegisteredModel",
                column: "ExperimentId");

            migrationBuilder.CreateIndex(
                name: "IX_RegisteredModel_RunArtifactId",
                table: "RegisteredModel",
                column: "RunArtifactId");

            migrationBuilder.CreateIndex(
                name: "IX_RegisteredModel_RunId",
                table: "RegisteredModel",
                column: "RunId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Deployment_ExperimentId",
                table: "Deployment",
                column: "ExperimentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Deployment_Experiment_ExperimentId",
                table: "Deployment",
                column: "ExperimentId",
                principalTable: "Experiment",
                principalColumn: "ExperimentId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Deployment_RegisteredModel_RegisteredModelId",
                table: "Deployment",
                column: "RegisteredModelId",
                principalTable: "RegisteredModel",
                principalColumn: "RegisteredModelId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RegisteredModel_Experiment_ExperimentId",
                table: "RegisteredModel",
                column: "ExperimentId",
                principalTable: "Experiment",
                principalColumn: "ExperimentId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RegisteredModel_RunArtifact_RunArtifactId",
                table: "RegisteredModel",
                column: "RunArtifactId",
                principalTable: "RunArtifact",
                principalColumn: "RunArtifactId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RegisteredModel_Run_RunId",
                table: "RegisteredModel",
                column: "RunId",
                principalTable: "Run",
                principalColumn: "RunId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
