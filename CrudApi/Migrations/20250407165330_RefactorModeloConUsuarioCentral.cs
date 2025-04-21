using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrudApi.Migrations
{
    /// <inheritdoc />
    public partial class RefactorModeloConUsuarioCentral : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Barberias_Roles_RoleId",
                table: "Barberias");

            migrationBuilder.DropForeignKey(
                name: "FK_Barberias_TipoDocumento_TipoDocumentoId",
                table: "Barberias");

            migrationBuilder.DropForeignKey(
                name: "FK_Barberos_Roles_RoleId",
                table: "Barberos");

            migrationBuilder.DropForeignKey(
                name: "FK_Barberos_TipoDocumento_TipoDocumentoId",
                table: "Barberos");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_Email",
                table: "Clientes");

            migrationBuilder.DropIndex(
                name: "IX_Barberos_TipoDocumentoId",
                table: "Barberos");

            migrationBuilder.DropIndex(
                name: "IX_Barberias_TipoDocumentoId",
                table: "Barberias");

            migrationBuilder.DropColumn(
                name: "HoraInicio",
                table: "Turnos");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "Nombre",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "Direccion",
                table: "Barberos");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Barberos");

            migrationBuilder.DropColumn(
                name: "Nombre",
                table: "Barberos");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Barberos");

            migrationBuilder.DropColumn(
                name: "Telefono",
                table: "Barberos");

            migrationBuilder.DropColumn(
                name: "Direccion",
                table: "Barberias");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Barberias");

            migrationBuilder.DropColumn(
                name: "Nombre",
                table: "Barberias");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Barberias");

            migrationBuilder.DropColumn(
                name: "Telefono",
                table: "Barberias");

            migrationBuilder.RenameColumn(
                name: "FechaHora",
                table: "Turnos",
                newName: "HoraFin");

            migrationBuilder.RenameColumn(
                name: "TurnoId",
                table: "Turnos",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "Servicio",
                table: "Servicios",
                newName: "Nombre");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "Clientes",
                newName: "UsuarioId");

            migrationBuilder.RenameColumn(
                name: "TipoDocumentoId",
                table: "Barberias",
                newName: "UsuarioId");

            migrationBuilder.AlterColumn<string>(
                name: "NombreBarberia",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Correo",
                table: "Usuarios",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "Fecha",
                table: "Turnos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<decimal>(
                name: "PrecioEspecial",
                table: "Servicios",
                type: "decimal(10,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Estado",
                table: "Clientes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "RoleId",
                table: "Barberos",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "NumeroDocumento",
                table: "Barberos",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "FotoBarbero",
                table: "Barberos",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "Barberos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "RoleId",
                table: "Barberias",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "NumeroDocumento",
                table: "Barberias",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "FotoBarberia",
                table: "Barberias",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<bool>(
                name: "Estado",
                table: "Barberias",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "TipoDocumento",
                table: "Barberias",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Correo",
                table: "Usuarios",
                column: "Correo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_UsuarioId",
                table: "Clientes",
                column: "UsuarioId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Barberos_UsuarioId",
                table: "Barberos",
                column: "UsuarioId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Barberias_UsuarioId",
                table: "Barberias",
                column: "UsuarioId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Barberias_Roles_RoleId",
                table: "Barberias",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Barberias_Usuarios_UsuarioId",
                table: "Barberias",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Barberos_Roles_RoleId",
                table: "Barberos",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Barberos_Usuarios_UsuarioId",
                table: "Barberos",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Clientes_Usuarios_UsuarioId",
                table: "Clientes",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Barberias_Roles_RoleId",
                table: "Barberias");

            migrationBuilder.DropForeignKey(
                name: "FK_Barberias_Usuarios_UsuarioId",
                table: "Barberias");

            migrationBuilder.DropForeignKey(
                name: "FK_Barberos_Roles_RoleId",
                table: "Barberos");

            migrationBuilder.DropForeignKey(
                name: "FK_Barberos_Usuarios_UsuarioId",
                table: "Barberos");

            migrationBuilder.DropForeignKey(
                name: "FK_Clientes_Usuarios_UsuarioId",
                table: "Clientes");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_Correo",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_UsuarioId",
                table: "Clientes");

            migrationBuilder.DropIndex(
                name: "IX_Barberos_UsuarioId",
                table: "Barberos");

            migrationBuilder.DropIndex(
                name: "IX_Barberias_UsuarioId",
                table: "Barberias");

            migrationBuilder.DropColumn(
                name: "Fecha",
                table: "Turnos");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Barberos");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Barberias");

            migrationBuilder.DropColumn(
                name: "TipoDocumento",
                table: "Barberias");

            migrationBuilder.RenameColumn(
                name: "HoraFin",
                table: "Turnos",
                newName: "FechaHora");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Turnos",
                newName: "TurnoId");

            migrationBuilder.RenameColumn(
                name: "Nombre",
                table: "Servicios",
                newName: "Servicio");

            migrationBuilder.RenameColumn(
                name: "UsuarioId",
                table: "Clientes",
                newName: "RoleId");

            migrationBuilder.RenameColumn(
                name: "UsuarioId",
                table: "Barberias",
                newName: "TipoDocumentoId");

            migrationBuilder.AlterColumn<string>(
                name: "NombreBarberia",
                table: "Usuarios",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Correo",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "HoraInicio",
                table: "Turnos",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AlterColumn<decimal>(
                name: "PrecioEspecial",
                table: "Servicios",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Clientes",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Nombre",
                table: "Clientes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Clientes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "RoleId",
                table: "Barberos",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NumeroDocumento",
                table: "Barberos",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FotoBarbero",
                table: "Barberos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Direccion",
                table: "Barberos",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Barberos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Nombre",
                table: "Barberos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Barberos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Telefono",
                table: "Barberos",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "RoleId",
                table: "Barberias",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NumeroDocumento",
                table: "Barberias",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FotoBarberia",
                table: "Barberias",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Direccion",
                table: "Barberias",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Barberias",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Nombre",
                table: "Barberias",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Barberias",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Telefono",
                table: "Barberias",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_Email",
                table: "Clientes",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Barberos_TipoDocumentoId",
                table: "Barberos",
                column: "TipoDocumentoId");

            migrationBuilder.CreateIndex(
                name: "IX_Barberias_TipoDocumentoId",
                table: "Barberias",
                column: "TipoDocumentoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Barberias_Roles_RoleId",
                table: "Barberias",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Barberias_TipoDocumento_TipoDocumentoId",
                table: "Barberias",
                column: "TipoDocumentoId",
                principalTable: "TipoDocumento",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Barberos_Roles_RoleId",
                table: "Barberos",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Barberos_TipoDocumento_TipoDocumentoId",
                table: "Barberos",
                column: "TipoDocumentoId",
                principalTable: "TipoDocumento",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
