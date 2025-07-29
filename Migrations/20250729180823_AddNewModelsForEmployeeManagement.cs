using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRApi.Migrations
{
    /// <inheritdoc />
    public partial class AddNewModelsForEmployeeManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PayrollPayments_PayrollHistories_PayrollHistoryId",
                table: "PayrollPayments"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_PerformanceReviews_Employees_EmployeeId",
                table: "PerformanceReviews"
            );

            migrationBuilder.CreateTable(
                name: "AcademicQualifications",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    QualificationName = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    YearStarted = table.Column<int>(type: "int", nullable: false),
                    YearOfCompletion = table.Column<int>(type: "int", nullable: false),
                    InstitutionName = table.Column<string>(
                        type: "nvarchar(250)",
                        maxLength: 250,
                        nullable: true
                    ),
                    CertificateId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(
                        type: "nvarchar(500)",
                        maxLength: 500,
                        nullable: true
                    ),
                    Photo = table.Column<string>(
                        type: "nvarchar(250)",
                        maxLength: 250,
                        nullable: true
                    ),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademicQualifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AcademicQualifications_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "InterestHobbies",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Hobby = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    Description = table.Column<string>(
                        type: "nvarchar(500)",
                        maxLength: 500,
                        nullable: true
                    ),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterestHobbies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterestHobbies_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "MedicalInformations",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BloodGroup = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    Genotype = table.Column<string>(
                        type: "nvarchar(500)",
                        maxLength: 500,
                        nullable: true
                    ),
                    MajorAllergies = table.Column<string>(
                        type: "nvarchar(250)",
                        maxLength: 250,
                        nullable: true
                    ),
                    ChronicConditions = table.Column<string>(
                        type: "nvarchar(500)",
                        maxLength: 500,
                        nullable: true
                    ),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalInformations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MedicalInformations_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "StaffCompensations",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IncidentDate = table.Column<DateOnly>(type: "date", nullable: true),
                    NotifyDate = table.Column<DateOnly>(type: "date", nullable: true),
                    InjuryDetails = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LocationDetails = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DaysAway = table.Column<int>(type: "int", nullable: false),
                    MedicalCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OtherCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffCompensations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StaffCompensations_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "WorkExperiences",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    JobTitle = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    CompanyName = table.Column<string>(
                        type: "nvarchar(250)",
                        maxLength: 250,
                        nullable: false
                    ),
                    YearStarted = table.Column<int>(type: "int", nullable: false),
                    YearEnded = table.Column<int>(type: "int", nullable: false),
                    Industry = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Achievement = table.Column<string>(
                        type: "nvarchar(500)",
                        maxLength: 500,
                        nullable: true
                    ),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkExperiences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkExperiences_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_AcademicQualifications_EmployeeId",
                table: "AcademicQualifications",
                column: "EmployeeId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_InterestHobbies_EmployeeId",
                table: "InterestHobbies",
                column: "EmployeeId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_MedicalInformations_EmployeeId",
                table: "MedicalInformations",
                column: "EmployeeId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_StaffCompensations_EmployeeId",
                table: "StaffCompensations",
                column: "EmployeeId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_WorkExperiences_EmployeeId",
                table: "WorkExperiences",
                column: "EmployeeId"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_PayrollPayments_PayrollHistories_PayrollHistoryId",
                table: "PayrollPayments",
                column: "PayrollHistoryId",
                principalTable: "PayrollHistories",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction
            );

            migrationBuilder.AddForeignKey(
                name: "FK_PerformanceReviews_Employees_EmployeeId",
                table: "PerformanceReviews",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PayrollPayments_PayrollHistories_PayrollHistoryId",
                table: "PayrollPayments"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_PerformanceReviews_Employees_EmployeeId",
                table: "PerformanceReviews"
            );

            migrationBuilder.DropTable(name: "AcademicQualifications");

            migrationBuilder.DropTable(name: "InterestHobbies");

            migrationBuilder.DropTable(name: "MedicalInformations");

            migrationBuilder.DropTable(name: "StaffCompensations");

            migrationBuilder.DropTable(name: "WorkExperiences");

            migrationBuilder.AddForeignKey(
                name: "FK_PayrollPayments_PayrollHistories_PayrollHistoryId",
                table: "PayrollPayments",
                column: "PayrollHistoryId",
                principalTable: "PayrollHistories",
                principalColumn: "Id"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_PerformanceReviews_Employees_EmployeeId",
                table: "PerformanceReviews",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id"
            );
        }
    }
}
