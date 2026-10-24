using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EFCoreTraining.Migrations
{
    /// <inheritdoc />
    public partial class AddTagsToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<string>>(
                name: "Tags",
                table: "Products",
                type: "text[]",
                nullable: false,
                defaultValueSql: "ARRAY[]::text[]");

            // Fill existing rows with an empty array, then remove the database default.
            migrationBuilder.Sql("ALTER TABLE \"Products\" ALTER COLUMN \"Tags\" DROP DEFAULT;");

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "Products",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tags",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "Products");
        }
    }
}


