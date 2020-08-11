using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MLOps.NET.SQLServer.Migrations
{
    /// <summary>
    /// Data Distribution
    /// </summary>
    public partial class DataDistribution : Migration
    {
        /// <summary>
        /// Data Distribution
        /// </summary>
        /// <param name="migrationBuilder"></param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DataDistribution",
                columns: table => new
                {
                    DataDistributionId = table.Column<Guid>(nullable: false),
                    DataColumnId = table.Column<Guid>(nullable: false),
                    Value = table.Column<string>(nullable: true),
                    Count = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataDistribution", x => x.DataDistributionId);
                    table.ForeignKey(
                        name: "FK_DataDistribution_DataColumn_DataColumnId",
                        column: x => x.DataColumnId,
                        principalTable: "DataColumn",
                        principalColumn: "DataColumnId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DataDistribution_DataColumnId",
                table: "DataDistribution",
                column: "DataColumnId");
        }

        /// <summary>
        /// Data Distribution
        /// </summary>
        /// <param name="migrationBuilder"></param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataDistribution");
        }
    }
}
