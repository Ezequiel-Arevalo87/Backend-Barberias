using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrudApi.Migrations
{
    /// <inheritdoc />
    public partial class AddSucursalIdToBarbero : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SucursalId",
                table: "Barberos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Barberos_SucursalId",
                table: "Barberos",
                column: "SucursalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Barberos_SucursalesBarberia_SucursalId",
                table: "Barberos",
                column: "SucursalId",
                principalTable: "SucursalesBarberia",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Barberos_SucursalesBarberia_SucursalId",
                table: "Barberos");

            migrationBuilder.DropIndex(
                name: "IX_Barberos_SucursalId",
                table: "Barberos");

            migrationBuilder.DropColumn(
                name: "SucursalId",
                table: "Barberos");
        }
    }
}
