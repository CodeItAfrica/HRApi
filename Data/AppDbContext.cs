using System;
using System.Collections.Generic;
using HRApi.Models;
using Microsoft.EntityFrameworkCore;

namespace HRApi.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Announcement> Announcements { get; set; }
    public virtual DbSet<AuditLog> AuditLogs { get; set; }
    public virtual DbSet<Department> Departments { get; set; }
    public virtual DbSet<Document> Documents { get; set; }
    public virtual DbSet<Employee> Employees { get; set; }
    public virtual DbSet<EmploymentType> EmploymentTypes { get; set; }
    public virtual DbSet<JobApplication> JobApplications { get; set; }
    public virtual DbSet<JobApplicationHistory> JobApplicationHistories { get; set; }
    public virtual DbSet<JobPosting> JobPostings { get; set; }
    public virtual DbSet<LeaveHistory> LeaveHistories { get; set; }
    public virtual DbSet<LeaveRequest> LeaveRequests { get; set; }
    public virtual DbSet<PasswordReset> PasswordResets { get; set; }
    public virtual DbSet<PayAuditLog> PayAuditLogs { get; set; }
    public virtual DbSet<Payroll> Payrolls { get; set; }
    public virtual DbSet<PayrollAllowance> PayrollAllowances { get; set; }
    public virtual DbSet<PayrollDeduction> PayrollDeductions { get; set; }
    public virtual DbSet<PayrollHistory> PayrollHistories { get; set; }
    public virtual DbSet<PayrollPayment> PayrollPayments { get; set; }
    public virtual DbSet<PerformanceReview> PerformanceReviews { get; set; }
    public virtual DbSet<PerformanceReviewHistory> PerformanceReviewHistories { get; set; }
    public virtual DbSet<Permission> Permissions { get; set; }
    public virtual DbSet<Role> Roles { get; set; }
    public virtual DbSet<RolePermission> RolePermissions { get; set; }
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<ForgottenPassword> ForgottenPassword { get; set; }
    public virtual DbSet<RegisterLink> RegisterLink { get; set; }
    public virtual DbSet<UserRole> UserRoles { get; set; }

//     protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
// #warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//         => optionsBuilder.UseSqlServer("Server=localhost;Database=HRMSNewDb;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Latin1_General_CI_AS");

        modelBuilder.Entity<Announcement>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__announce__3213E83FA5E09B80");

            entity.Property(e => e.Message).HasColumnType("text");
            entity.Property(e => e.PostedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PostedBy)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__audit_lo__3213E83F61FC453C");

            entity.ToTable("audit_log");

            entity.Property(e => e.Action)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Details).HasColumnType("text");
            entity.Property(e => e.IpAddress)
                .HasMaxLength(45)
                .IsUnicode(false);
            entity.Property(e => e.UserEmail)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UserId)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__departme__3213E83FA6F6269F");

            entity.HasIndex(e => e.DepartmentName, "UQ__departme__226ED15700BC291A").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DepartmentName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        // Continue converting other entities similarly...

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
