using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrudApi.Migrations
{
    /// <inheritdoc />
    public partial class AddSucursalBarberia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SucursalesBarberias_Barberias_BarberiaId",
                table: "SucursalesBarberias");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SucursalesBarberias",
                table: "SucursalesBarberias");

            migrationBuilder.RenameTable(
                name: "SucursalesBarberias",
                newName: "SucursalesBarberia");

            migrationBuilder.RenameIndex(
                name: "IX_SucursalesBarberias_BarberiaId",
                table: "SucursalesBarberia",
                newName: "IX_SucursalesBarberia_BarberiaId");

            migrationBuilder.AlterColumn<string>(
                name: "Telefono",
                table: "SucursalesBarberia",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "SucursalesBarberia",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Direccion",
                table: "SucursalesBarberia",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<int>(
                name: "Estado",
                table: "SucursalesBarberia",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "FotoSucursal",
                table: "SucursalesBarberia",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NumeroDocumento",
                table: "SucursalesBarberia",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TipoDocumentoId",
                table: "SucursalesBarberia",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SucursalesBarberia",
                table: "SucursalesBarberia",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SucursalesBarberia_Barberias_BarberiaId",
                table: "SucursalesBarberia",
                column: "BarberiaId",
                principalTable: "Barberias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SucursalesBarberia_Barberias_BarberiaId",
                table: "SucursalesBarberia");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SucursalesBarberia",
                table: "SucursalesBarberia");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "SucursalesBarberia");

            migrationBuilder.DropColumn(
                name: "FotoSucursal",
                table: "SucursalesBarberia");

            migrationBuilder.DropColumn(
                name: "NumeroDocumento",
                table: "SucursalesBarberia");

            migrationBuilder.DropColumn(
                name: "TipoDocumentoId",
                table: "SucursalesBarberia");

            migrationBuilder.RenameTable(
                name: "SucursalesBarberia",
                newName: "SucursalesBarberias");

            migrationBuilder.RenameIndex(
                name: "IX_SucursalesBarberia_BarberiaId",
                table: "SucursalesBarberias",
                newName: "IX_SucursalesBarberias_BarberiaId");

            migrationBuilder.AlterColumn<string>(
                name: "Telefono",
                table: "SucursalesBarberias",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "SucursalesBarberias",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Direccion",
                table: "SucursalesBarberias",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SucursalesBarberias",
                table: "SucursalesBarberias",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SucursalesBarberias_Barberias_BarberiaId",
                table: "SucursalesBarberias",
                column: "BarberiaId",
                principalTable: "Barberias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
