using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MLOps.NET.SQLServer.Migrations
{
    /// <summary>
    /// 
    /// </summary>
    public partial class AddedModelSchema : Migration
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="migrationBuilder"></param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ModelSchema",
                columns: table => new
                {
                    ModelSchemaId = table.Column<Guid>(nullable: false),
                    RunId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Definition = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelSchema", x => x.ModelSchemaId);
                    table.ForeignKey(
                        name: "FK_ModelSchema_Run_RunId",
                        column: x => x.RunId,
                        principalTable: "Run",
                        principalColumn: "RunId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ModelSchema_RunId",
                table: "ModelSchema",
                column: "RunId");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="migrationBuilder"></param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModelSchema");
        }
    }
}
