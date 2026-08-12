using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Account.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class new_audit_log_properties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "action",
                table: "audit_log",
                newName: "request_name");

            migrationBuilder.AddColumn<string>(
                name: "error_message",
                table: "audit_log",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "execution_time_in_ms",
                table: "audit_log",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_successful",
                table: "audit_log",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "request_data",
                table: "audit_log",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "resource_id",
                table: "audit_log",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "resource_name",
                table: "audit_log",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "error_message",
                table: "audit_log");

            migrationBuilder.DropColumn(
                name: "execution_time_in_ms",
                table: "audit_log");

            migrationBuilder.DropColumn(
                name: "is_successful",
                table: "audit_log");

            migrationBuilder.DropColumn(
                name: "request_data",
                table: "audit_log");

            migrationBuilder.DropColumn(
                name: "resource_id",
                table: "audit_log");

            migrationBuilder.DropColumn(
                name: "resource_name",
                table: "audit_log");

            migrationBuilder.RenameColumn(
                name: "request_name",
                table: "audit_log",
                newName: "action");
        }
    }
}
