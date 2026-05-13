using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarbeariaFeuRosa.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarFotoBarbeiro : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FotoUrl",
                table: "Barbeiros",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FotoUrl",
                table: "Barbeiros");
        }
    }
}
