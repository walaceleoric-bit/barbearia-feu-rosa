using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BarbeariaFeuRosa.Migrations
{
    /// <inheritdoc />
    public partial class MultiBarbeariasBase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BarbeariaId",
                table: "Usuarios",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BarbeariaId",
                table: "Clientes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BarbeariaId",
                table: "Barbeiros",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BarbeariaId",
                table: "Agendamentos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Barbearias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Slug = table.Column<string>(type: "text", nullable: false),
                    LogoUrl = table.Column<string>(type: "text", nullable: false),
                    Ativa = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Barbearias", x => x.Id);
                });

            migrationBuilder.InsertData(
    table: "Barbearias",
    columns: new[] { "Id", "Nome", "Slug", "LogoUrl", "Ativa" },
    values: new object[]
    {
        1,
        "Barbearia Feu Rosa",
        "feu-rosa",
        "",
        true
    });

            migrationBuilder.Sql(@"
    UPDATE ""Usuarios"" SET ""BarbeariaId"" = 1;
    UPDATE ""Clientes"" SET ""BarbeariaId"" = 1;
    UPDATE ""Barbeiros"" SET ""BarbeariaId"" = 1;
    UPDATE ""Agendamentos"" SET ""BarbeariaId"" = 1;
");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_BarbeariaId",
                table: "Usuarios",
                column: "BarbeariaId");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_BarbeariaId",
                table: "Clientes",
                column: "BarbeariaId");

            migrationBuilder.CreateIndex(
                name: "IX_Barbeiros_BarbeariaId",
                table: "Barbeiros",
                column: "BarbeariaId");

            migrationBuilder.CreateIndex(
                name: "IX_Agendamentos_BarbeariaId",
                table: "Agendamentos",
                column: "BarbeariaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Agendamentos_Barbearias_BarbeariaId",
                table: "Agendamentos",
                column: "BarbeariaId",
                principalTable: "Barbearias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Barbeiros_Barbearias_BarbeariaId",
                table: "Barbeiros",
                column: "BarbeariaId",
                principalTable: "Barbearias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Clientes_Barbearias_BarbeariaId",
                table: "Clientes",
                column: "BarbeariaId",
                principalTable: "Barbearias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Barbearias_BarbeariaId",
                table: "Usuarios",
                column: "BarbeariaId",
                principalTable: "Barbearias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agendamentos_Barbearias_BarbeariaId",
                table: "Agendamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Barbeiros_Barbearias_BarbeariaId",
                table: "Barbeiros");

            migrationBuilder.DropForeignKey(
                name: "FK_Clientes_Barbearias_BarbeariaId",
                table: "Clientes");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Barbearias_BarbeariaId",
                table: "Usuarios");

            migrationBuilder.DropTable(
                name: "Barbearias");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_BarbeariaId",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_BarbeariaId",
                table: "Clientes");

            migrationBuilder.DropIndex(
                name: "IX_Barbeiros_BarbeariaId",
                table: "Barbeiros");

            migrationBuilder.DropIndex(
                name: "IX_Agendamentos_BarbeariaId",
                table: "Agendamentos");

            migrationBuilder.DropColumn(
                name: "BarbeariaId",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "BarbeariaId",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "BarbeariaId",
                table: "Barbeiros");

            migrationBuilder.DropColumn(
                name: "BarbeariaId",
                table: "Agendamentos");
        }
    }
}
