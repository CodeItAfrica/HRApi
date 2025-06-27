using System;
using System.Collections.Generic;
using HRApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using static HRApi.Models.Admin;

namespace HRApi.Data;

public partial class AppDbContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.ConfigureWarnings(warnings =>
            warnings.Ignore(RelationalEventId.PendingModelChangesWarning)
        );
    }

    public AppDbContext() { }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public virtual DbSet<Announcement> Announcements { get; set; }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<Branch> Branches { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Document> Documents { get; set; }
    public virtual DbSet<DocumentType> DocumentTypes { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<EmploymentType> EmploymentTypes { get; set; }

    public virtual DbSet<ForgottenPassword> ForgottenPassword { get; set; }

    public virtual DbSet<Grade> Grades { get; set; }

    public virtual DbSet<JobApplication> JobApplications { get; set; }

    public virtual DbSet<JobApplicationHistory> JobApplicationHistories { get; set; }

    public virtual DbSet<JobPosting> JobPostings { get; set; }

    public virtual DbSet<LeaveHistory> LeaveHistories { get; set; }

    public virtual DbSet<LeaveRequest> LeaveRequests { get; set; }

    public virtual DbSet<LeaveType> LeaveTypes { get; set; }

    public virtual DbSet<PayAuditLog> PayAuditLogs { get; set; }

    public virtual DbSet<Payroll> Payrolls { get; set; }

    public virtual DbSet<PayrollAllowance> PayrollAllowances { get; set; }

    public virtual DbSet<PayrollDeduction> PayrollDeductions { get; set; }

    public virtual DbSet<PayrollHistory> PayrollHistories { get; set; }

    public virtual DbSet<PayrollPayment> PayrollPayments { get; set; }

    public virtual DbSet<PerformanceReview> PerformanceReviews { get; set; }

    public virtual DbSet<PerformanceReviewHistory> PerformanceReviewHistories { get; set; }

    public virtual DbSet<RegisterLink> RegisterLink { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Unit> Units { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<JobTitle> JobTitles { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<FAQ> FAQs { get; set; }

    public virtual DbSet<Bank> Banks { get; set; }

    public virtual DbSet<TrainingProgram> TrainingPrograms { get; set; }

    public DbSet<IssueReport> IssueReports { get; set; }
    public DbSet<IssueAttachment> IssueAttachments { get; set; }
    public DbSet<PaySheet> PaySheets { get; set; }
    public DbSet<EmployeeGradeHistory> EmployeeGradeHistories { get; set; }
    public DbSet<DeductionList> DeductionLists { get; set; }
    public DbSet<AllowanceList> AllowanceLists { get; set; }
    public DbSet<PayrollAllowanceHistory> PayrollAllowanceHistories { get; set; }
    public DbSet<PayrollDeductionHistory> PayrollDeductionHistories { get; set; }
    public DbSet<VariantDeduction> VariantDeductions { get; set; }
    public DbSet<VariantAllowance> VariantAllowances { get; set; }

    //     protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // #warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
    //         => optionsBuilder.UseSqlServer("Server=tcp:108.181.199.153,10875;Initial Catalog=HRNewDb;User ID=sa;Password=Gibs@321.;Encrypt=False;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<IssueReport>()
            .HasMany(ir => ir.Attachments)
            .WithOne(a => a.IssueReport)
            .HasForeignKey(a => a.IssueReportId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
