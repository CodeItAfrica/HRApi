using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRApi.Migrations
{
    /// <inheritdoc />
    public partial class AddForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VariantDeductionId",
                table: "PayrollDeductions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VariantAllowanceId",
                table: "PayrollAllowances",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PayrollDeductions_VariantDeductionId",
                table: "PayrollDeductions",
                column: "VariantDeductionId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollAllowances_VariantAllowanceId",
                table: "PayrollAllowances",
                column: "VariantAllowanceId");

            migrationBuilder.AddForeignKey(
                name: "FK_PayrollAllowances_VariantAllowances_VariantAllowanceId",
                table: "PayrollAllowances",
                column: "VariantAllowanceId",
                principalTable: "VariantAllowances",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PayrollDeductions_VariantDeductions_VariantDeductionId",
                table: "PayrollDeductions",
                column: "VariantDeductionId",
                principalTable: "VariantDeductions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PayrollAllowances_VariantAllowances_VariantAllowanceId",
                table: "PayrollAllowances");

            migrationBuilder.DropForeignKey(
                name: "FK_PayrollDeductions_VariantDeductions_VariantDeductionId",
                table: "PayrollDeductions");

            migrationBuilder.DropIndex(
                name: "IX_PayrollDeductions_VariantDeductionId",
                table: "PayrollDeductions");

            migrationBuilder.DropIndex(
                name: "IX_PayrollAllowances_VariantAllowanceId",
                table: "PayrollAllowances");

            migrationBuilder.DropColumn(
                name: "VariantDeductionId",
                table: "PayrollDeductions");

            migrationBuilder.DropColumn(
                name: "VariantAllowanceId",
                table: "PayrollAllowances");
        }
    }
}
