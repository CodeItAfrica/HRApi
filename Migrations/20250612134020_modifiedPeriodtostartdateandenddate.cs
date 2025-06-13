using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRApi.Migrations
{
    /// <inheritdoc />
    public partial class modifiedPeriodtostartdateandenddate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Period",
                table: "PayrollHistories",
                newName: "StartDate");

            migrationBuilder.AddColumn<DateOnly>(
                name: "StartDate",
                table: "Payrolls",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<DateOnly>(
                name: "EndDate",
                table: "PayrollHistories",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<DateOnly>(
                name: "StartDate",
                table: "PayrollDeductions",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<DateOnly>(
                name: "StartDate",
                table: "PayrollAllowances",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Payrolls");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "PayrollHistories");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "PayrollDeductions");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "PayrollAllowances");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "PayrollHistories",
                newName: "Period");
        }
    }
}
