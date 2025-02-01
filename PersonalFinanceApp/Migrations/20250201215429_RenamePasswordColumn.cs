using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalFinanceApp.Migrations
{
    /// <inheritdoc />
    public partial class RenamePasswordColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
            name: "Password",  // Old column name
            table: "Users",    // Table name
            newName: "PasswordHash"); // New column name
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
            name: "PasswordHash",  // New column name
            table: "Users",        // Table name
            newName: "Password");  // Old column name
        }
    }
}
