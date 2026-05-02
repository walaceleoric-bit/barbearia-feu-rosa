using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarbeariaFeuRosa.Migrations
{
    /// <inheritdoc />
    public partial class VinculoUsuarioBarbeiro : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BarbeiroId",
                table: "Usuarios",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_BarbeiroId",
                table: "Usuarios",
                column: "BarbeiroId");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Barbeiros_BarbeiroId",
                table: "Usuarios",
                column: "BarbeiroId",
                principalTable: "Barbeiros",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Barbeiros_BarbeiroId",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_BarbeiroId",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "BarbeiroId",
                table: "Usuarios");
        }
    }
}
