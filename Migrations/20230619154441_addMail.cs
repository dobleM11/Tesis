using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tesis.Migrations {
    /// <inheritdoc />
    public partial class addMail : Migration {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.AddColumn<string>(
                name: "Mail",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropColumn(
                name: "Mail",
                table: "Usuarios");
        }
    }
}
