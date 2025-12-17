using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryMvcApp.Migrations
{
    /// <inheritdoc />
    public partial class updateAuthentication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AllowedRole",
                table: "Folders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowedRole",
                table: "Folders");
        }
    }
}
