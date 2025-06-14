using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRApi.Migrations
{
    /// <inheritdoc />
    public partial class modifiedSomeTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "DeductionType", table: "PayrollDeductions");

            migrationBuilder.DropColumn(name: "Period", table: "PayrollDeductions");

            migrationBuilder.DropColumn(name: "AllowanceType", table: "PayrollAllowances");

            migrationBuilder.DropColumn(name: "Period", table: "PayrollAllowances");

            migrationBuilder.AddColumn<int>(
                name: "DeductionListId",
                table: "PayrollDeductions",
                type: "int",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder.AddColumn<int>(
                name: "AllowanceListId",
                table: "PayrollAllowances",
                type: "int",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder.CreateIndex(
                name: "IX_PayrollDeductions_DeductionListId",
                table: "PayrollDeductions",
                column: "DeductionListId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_PayrollAllowances_AllowanceListId",
                table: "PayrollAllowances",
                column: "AllowanceListId"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_PayrollAllowances_AllowanceLists_AllowanceListId",
                table: "PayrollAllowances",
                column: "AllowanceListId",
                principalTable: "AllowanceLists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_PayrollDeductions_DeductionLists_DeductionListId",
                table: "PayrollDeductions",
                column: "DeductionListId",
                principalTable: "DeductionLists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PayrollAllowances_AllowanceLists_AllowanceListId",
                table: "PayrollAllowances"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_PayrollDeductions_DeductionLists_DeductionListId",
                table: "PayrollDeductions"
            );

            migrationBuilder.DropIndex(
                name: "IX_PayrollDeductions_DeductionListId",
                table: "PayrollDeductions"
            );

            migrationBuilder.DropIndex(
                name: "IX_PayrollAllowances_AllowanceListId",
                table: "PayrollAllowances"
            );

            migrationBuilder.DropColumn(name: "DeductionListId", table: "PayrollDeductions");

            migrationBuilder.DropColumn(name: "AllowanceListId", table: "PayrollAllowances");

            migrationBuilder.AddColumn<string>(
                name: "DeductionType",
                table: "PayrollDeductions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: ""
            );

            migrationBuilder.AddColumn<DateOnly>(
                name: "Period",
                table: "PayrollDeductions",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1)
            );

            migrationBuilder.AddColumn<string>(
                name: "AllowanceType",
                table: "PayrollAllowances",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: ""
            );

            migrationBuilder.AddColumn<DateOnly>(
                name: "Period",
                table: "PayrollAllowances",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1)
            );
        }
    }
}
