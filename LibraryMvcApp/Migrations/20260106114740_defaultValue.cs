using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryMvcApp.Migrations
{
    /// <inheritdoc />
    public partial class defaultValue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "FormEntries",
                keyColumn: "Id",
                keyValue: 1,
                column: "Review",
                value: 1);

            migrationBuilder.UpdateData(
                table: "FormEntries",
                keyColumn: "Id",
                keyValue: 2,
                column: "Review",
                value: 1);

            migrationBuilder.UpdateData(
                table: "FormEntries",
                keyColumn: "Id",
                keyValue: 3,
                column: "Review",
                value: 1);

            migrationBuilder.UpdateData(
                table: "FormEntries",
                keyColumn: "Id",
                keyValue: 4,
                column: "Review",
                value: 1);

            migrationBuilder.UpdateData(
                table: "FormEntries",
                keyColumn: "Id",
                keyValue: 5,
                column: "Review",
                value: 1);

            migrationBuilder.UpdateData(
                table: "FormEntries",
                keyColumn: "Id",
                keyValue: 20,
                column: "Review",
                value: 1);

            migrationBuilder.UpdateData(
                table: "FormEntries",
                keyColumn: "Id",
                keyValue: 30,
                column: "Review",
                value: 1);

            migrationBuilder.UpdateData(
                table: "FormEntries",
                keyColumn: "Id",
                keyValue: 31,
                column: "Review",
                value: 1);

            migrationBuilder.UpdateData(
                table: "FormEntries",
                keyColumn: "Id",
                keyValue: 40,
                column: "Review",
                value: 1);

            migrationBuilder.UpdateData(
                table: "FormEntries",
                keyColumn: "Id",
                keyValue: 41,
                column: "Review",
                value: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "FormEntries",
                keyColumn: "Id",
                keyValue: 1,
                column: "Review",
                value: 0);

            migrationBuilder.UpdateData(
                table: "FormEntries",
                keyColumn: "Id",
                keyValue: 2,
                column: "Review",
                value: 0);

            migrationBuilder.UpdateData(
                table: "FormEntries",
                keyColumn: "Id",
                keyValue: 3,
                column: "Review",
                value: 0);

            migrationBuilder.UpdateData(
                table: "FormEntries",
                keyColumn: "Id",
                keyValue: 4,
                column: "Review",
                value: 0);

            migrationBuilder.UpdateData(
                table: "FormEntries",
                keyColumn: "Id",
                keyValue: 5,
                column: "Review",
                value: 0);

            migrationBuilder.UpdateData(
                table: "FormEntries",
                keyColumn: "Id",
                keyValue: 20,
                column: "Review",
                value: 0);

            migrationBuilder.UpdateData(
                table: "FormEntries",
                keyColumn: "Id",
                keyValue: 30,
                column: "Review",
                value: 0);

            migrationBuilder.UpdateData(
                table: "FormEntries",
                keyColumn: "Id",
                keyValue: 31,
                column: "Review",
                value: 0);

            migrationBuilder.UpdateData(
                table: "FormEntries",
                keyColumn: "Id",
                keyValue: 40,
                column: "Review",
                value: 0);

            migrationBuilder.UpdateData(
                table: "FormEntries",
                keyColumn: "Id",
                keyValue: 41,
                column: "Review",
                value: 0);
        }
    }
}
