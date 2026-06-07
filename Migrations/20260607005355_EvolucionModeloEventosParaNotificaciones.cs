using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agenda.Migrations
{
    /// <inheritdoc />
    public partial class EvolucionModeloEventosParaNotificaciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Notificado",
                table: "Eventos",
                newName: "SmsEnviado");

            migrationBuilder.AddColumn<bool>(
                name: "AlertaConfirmada",
                table: "Eventos",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "EmailDestino",
                table: "Eventos",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "MailEnviado",
                table: "Eventos",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TelefonoDestino",
                table: "Eventos",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlertaConfirmada",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "EmailDestino",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "MailEnviado",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "TelefonoDestino",
                table: "Eventos");

            migrationBuilder.RenameColumn(
                name: "SmsEnviado",
                table: "Eventos",
                newName: "Notificado");
        }
    }
}
