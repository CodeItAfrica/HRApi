using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRApi.Migrations
{
    /// <inheritdoc />
    public partial class madeAnotherChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Period",
                table: "PayrollHistories",
                newName: "Year");

            migrationBuilder.RenameColumn(
                name: "Period",
                table: "PayrollDeductionHistories",
                newName: "Year");

            migrationBuilder.RenameColumn(
                name: "Period",
                table: "PayrollAllowanceHistories",
                newName: "Year");

            migrationBuilder.AddColumn<int>(
                name: "Month",
                table: "PayrollHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Month",
                table: "PayrollDeductionHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Month",
                table: "PayrollAllowanceHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Month",
                table: "PayrollHistories");

            migrationBuilder.DropColumn(
                name: "Month",
                table: "PayrollDeductionHistories");

            migrationBuilder.DropColumn(
                name: "Month",
                table: "PayrollAllowanceHistories");

            migrationBuilder.RenameColumn(
                name: "Year",
                table: "PayrollHistories",
                newName: "Period");

            migrationBuilder.RenameColumn(
                name: "Year",
                table: "PayrollDeductionHistories",
                newName: "Period");

            migrationBuilder.RenameColumn(
                name: "Year",
                table: "PayrollAllowanceHistories",
                newName: "Period");
        }
    }
}
