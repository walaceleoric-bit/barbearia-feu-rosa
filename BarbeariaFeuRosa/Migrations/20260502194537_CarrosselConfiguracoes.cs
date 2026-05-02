using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarbeariaFeuRosa.Migrations
{
    /// <inheritdoc />
    public partial class CarrosselConfiguracoes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CarrosselImagem1",
                table: "Configuracoes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CarrosselImagem2",
                table: "Configuracoes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CarrosselImagem3",
                table: "Configuracoes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CarrosselImagem4",
                table: "Configuracoes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CarrosselImagem5",
                table: "Configuracoes",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CarrosselImagem1",
                table: "Configuracoes");

            migrationBuilder.DropColumn(
                name: "CarrosselImagem2",
                table: "Configuracoes");

            migrationBuilder.DropColumn(
                name: "CarrosselImagem3",
                table: "Configuracoes");

            migrationBuilder.DropColumn(
                name: "CarrosselImagem4",
                table: "Configuracoes");

            migrationBuilder.DropColumn(
                name: "CarrosselImagem5",
                table: "Configuracoes");
        }
    }
}
