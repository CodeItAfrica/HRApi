using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRApi.Migrations
{
    /// <inheritdoc />
    public partial class CreateVariant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PayrollAllowances_AllowanceLists_AllowanceListId",
                table: "PayrollAllowances");

            migrationBuilder.DropForeignKey(
                name: "FK_PayrollDeductions_DeductionLists_DeductionListId",
                table: "PayrollDeductions");

            migrationBuilder.AlterColumn<int>(
                name: "DeductionListId",
                table: "PayrollDeductions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "AllowanceListId",
                table: "PayrollAllowances",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "VariantAllowances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VariantAllowances", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VariantDeductions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VariantDeductions", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_PayrollAllowances_AllowanceLists_AllowanceListId",
                table: "PayrollAllowances",
                column: "AllowanceListId",
                principalTable: "AllowanceLists",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PayrollDeductions_DeductionLists_DeductionListId",
                table: "PayrollDeductions",
                column: "DeductionListId",
                principalTable: "DeductionLists",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PayrollAllowances_AllowanceLists_AllowanceListId",
                table: "PayrollAllowances");

            migrationBuilder.DropForeignKey(
                name: "FK_PayrollDeductions_DeductionLists_DeductionListId",
                table: "PayrollDeductions");

            migrationBuilder.DropTable(
                name: "VariantAllowances");

            migrationBuilder.DropTable(
                name: "VariantDeductions");

            migrationBuilder.AlterColumn<int>(
                name: "DeductionListId",
                table: "PayrollDeductions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AllowanceListId",
                table: "PayrollAllowances",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PayrollAllowances_AllowanceLists_AllowanceListId",
                table: "PayrollAllowances",
                column: "AllowanceListId",
                principalTable: "AllowanceLists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PayrollDeductions_DeductionLists_DeductionListId",
                table: "PayrollDeductions",
                column: "DeductionListId",
                principalTable: "DeductionLists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
