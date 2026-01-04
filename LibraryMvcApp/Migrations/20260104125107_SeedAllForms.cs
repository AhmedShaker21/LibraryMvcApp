using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LibraryMvcApp.Migrations
{
    /// <inheritdoc />
    public partial class SeedAllForms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "Id", "Code", "Name", "StartFormNumber" },
                values: new object[,]
                {
                    { 3, 73, "تأهيل وتدريب العاملين", 200 },
                    { 4, 74, "الإدارة الطبية", 200 },
                    { 5, 81, "إدارة المشتريات", 200 }
                });

            migrationBuilder.InsertData(
                table: "FormEntries",
                columns: new[] { "Id", "CreatedAt", "DepartmentId", "DepartmentNo", "FormName", "FormNumber", "FullNumber", "ProcedureCode", "ProcedureName" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 53, "خريطة تحليل الأنشطة والعمليات", 230, "ن / 53 / 230", "ACFE/HS P 53-01", "تحديد وتقييم مظاهر التأثير البيئي والسلامة" },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 53, "جدول الحصر العام لمصادر التأثير البيئى", 231, "ن / 53 / 231", "ACFE/HS P 53-01", "تحديد وتقييم مظاهر التأثير البيئي والسلامة" },
                    { 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 53, "جدول تقييم العناصر البيئية", 232, "ن / 53 / 232", "ACFE/HS P 53-01", "تحديد وتقييم مظاهر التأثير البيئي والسلامة" },
                    { 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 53, "جدول الحصر العام للمصادر الهامة", 233, "ن / 53 / 233", "ACFE/HS P 53-01", "تحديد وتقييم مظاهر التأثير البيئي والسلامة" },
                    { 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 53, "تحديد مخاطر العمل بالموقع", 240, "ن / 53 / 240", "ACFE/HSP-53-06", "أسلوب مواجهة حالات الطوارئ" },
                    { 20, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 73, "حصر أسماء العاملين فى العمليات الخاصة", 239, "ن / 73 / 239", "ACFQ/E/HS P 73-02", "تأهيل وتدريب العاملين بالعمليات الخاصة" },
                    { 30, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, 74, "طلب توقيع كشف طبى", 200, "ن / 74 / 200", "ACFQ/E/HSP 74-01", "الإدارة الطبية" },
                    { 31, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, 74, "نموذج تحويل للمستشفى", 201, "ن / 74 / 201", "ACFQ/E/HSP 74-01", "الإدارة الطبية" },
                    { 40, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 81, "طلب الشراء", 254, "ن / 81 / 254", "ACFQP 81-02", "إجراء عمليات الشراء" },
                    { 41, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 81, "سجل الموردين المعتمدين", 242, "ن / 81 / 242", "ACFQP 81-03", "تقييم الموردين" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "FormEntries",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "FormEntries",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "FormEntries",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "FormEntries",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "FormEntries",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "FormEntries",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "FormEntries",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "FormEntries",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "FormEntries",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "FormEntries",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 5);
        }
    }
}
