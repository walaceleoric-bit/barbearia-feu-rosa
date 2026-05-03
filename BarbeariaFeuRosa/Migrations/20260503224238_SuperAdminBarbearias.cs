using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarbeariaFeuRosa.Migrations
{
    /// <inheritdoc />
    public partial class SuperAdminBarbearias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataCadastro",
                table: "Barbearias",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DataVencimento",
                table: "Barbearias",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Observacao",
                table: "Barbearias",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "PagamentoEmDia",
                table: "Barbearias",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Plano",
                table: "Barbearias",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "ValorMensalidade",
                table: "Barbearias",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataCadastro",
                table: "Barbearias");

            migrationBuilder.DropColumn(
                name: "DataVencimento",
                table: "Barbearias");

            migrationBuilder.DropColumn(
                name: "Observacao",
                table: "Barbearias");

            migrationBuilder.DropColumn(
                name: "PagamentoEmDia",
                table: "Barbearias");

            migrationBuilder.DropColumn(
                name: "Plano",
                table: "Barbearias");

            migrationBuilder.DropColumn(
                name: "ValorMensalidade",
                table: "Barbearias");
        }
    }
}
