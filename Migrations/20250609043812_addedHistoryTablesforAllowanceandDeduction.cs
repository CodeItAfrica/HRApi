using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRApi.Migrations
{
    /// <inheritdoc />
    public partial class addedHistoryTablesforAllowanceandDeduction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PayrollAllowances_Users_GrantedByUserId",
                table: "PayrollAllowances"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_PayrollDeductions_Users_DeductedByUserId",
                table: "PayrollDeductions"
            );

            migrationBuilder.DropIndex(
                name: "IX_PayrollDeductions_DeductedByUserId",
                table: "PayrollDeductions"
            );

            migrationBuilder.DropIndex(
                name: "IX_PayrollAllowances_GrantedByUserId",
                table: "PayrollAllowances"
            );

            migrationBuilder.DropColumn(name: "DeductedByUserId", table: "PayrollDeductions");

            migrationBuilder.DropColumn(name: "GrantedByUserId", table: "PayrollAllowances");

            migrationBuilder.RenameColumn(
                name: "DeductedOn",
                table: "PayrollDeductions",
                newName: "LastDeductedOn"
            );

            migrationBuilder.RenameColumn(
                name: "GrantedOn",
                table: "PayrollAllowances",
                newName: "LastGrantedOn"
            );

            migrationBuilder.AddColumn<string>(
                name: "LastDeductedBy",
                table: "PayrollDeductions",
                type: "nvarchar(max)",
                nullable: true
            );

            migrationBuilder.AddColumn<string>(
                name: "LastGrantedBy",
                table: "PayrollAllowances",
                type: "nvarchar(max)",
                nullable: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "LastDeductedBy", table: "PayrollDeductions");

            migrationBuilder.DropColumn(name: "LastGrantedBy", table: "PayrollAllowances");

            migrationBuilder.RenameColumn(
                name: "LastDeductedOn",
                table: "PayrollDeductions",
                newName: "DeductedOn"
            );

            migrationBuilder.RenameColumn(
                name: "LastGrantedOn",
                table: "PayrollAllowances",
                newName: "GrantedOn"
            );

            migrationBuilder.AddColumn<int>(
                name: "DeductedByUserId",
                table: "PayrollDeductions",
                type: "int",
                nullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "GrantedByUserId",
                table: "PayrollAllowances",
                type: "int",
                nullable: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_PayrollDeductions_DeductedByUserId",
                table: "PayrollDeductions",
                column: "DeductedByUserId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_PayrollAllowances_GrantedByUserId",
                table: "PayrollAllowances",
                column: "GrantedByUserId"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_PayrollAllowances_Users_GrantedByUserId",
                table: "PayrollAllowances",
                column: "GrantedByUserId",
                principalTable: "Users",
                principalColumn: "Id"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_PayrollDeductions_Users_DeductedByUserId",
                table: "PayrollDeductions",
                column: "DeductedByUserId",
                principalTable: "Users",
                principalColumn: "Id"
            );
        }
    }
}
