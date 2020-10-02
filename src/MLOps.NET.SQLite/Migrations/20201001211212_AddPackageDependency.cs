using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MLOps.NET.SQLite.Migrations
{
    /// <summary>
    /// 
    /// </summary>
    public partial class AddPackageDependency : Migration
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="migrationBuilder"></param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PackageDependency",
                columns: table => new
                {
                    PackageDependencyId = table.Column<Guid>(nullable: false),
                    RunId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Version = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageDependency", x => x.PackageDependencyId);
                    table.ForeignKey(
                        name: "FK_PackageDependency_Run_RunId",
                        column: x => x.RunId,
                        principalTable: "Run",
                        principalColumn: "RunId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PackageDependency_RunId",
                table: "PackageDependency",
                column: "RunId");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="migrationBuilder"></param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PackageDependency");
        }
    }
}
