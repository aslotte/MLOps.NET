﻿using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace MLOps.NET.SQLServer.Migrations
{
    public partial class Distribution : Migration
    {
        /// <summary>
        /// 
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
                    Value = table.Column<string>(nullable: false),
                    Count = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataDistribution", x => x.DataDistributionId);
                });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="migrationBuilder"></param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
              name: "DataDistribution");
        }
    }
}