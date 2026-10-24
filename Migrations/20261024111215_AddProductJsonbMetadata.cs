using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EFCoreTraining.Migrations
{
    /// <inheritdoc />
    public partial class AddProductJsonbMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JsonMetadata",
                table: "Products",
                type: "jsonb",
                nullable: false,
                defaultValueSql: "'{}'::jsonb");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JsonMetadata",
                table: "Products");
        }
    }
}
