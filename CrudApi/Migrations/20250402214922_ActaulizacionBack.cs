using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrudApi.Migrations
{
    /// <inheritdoc />
    public partial class ActaulizacionBack : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FotoBarberia",
                table: "Barberias",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FotoBarberia",
                table: "Barberias");
        }
    }
}
