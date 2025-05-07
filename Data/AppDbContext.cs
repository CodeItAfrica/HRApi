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

    public virtual DbSet<ForgottenPassword> ForgottenPassword { get; set; }

    public virtual DbSet<JobApplication> JobApplications { get; set; }

    public virtual DbSet<JobApplicationHistory> JobApplicationHistories { get; set; }

    public virtual DbSet<JobPosting> JobPostings { get; set; }

    public virtual DbSet<LeaveHistory> LeaveHistories { get; set; }

    public virtual DbSet<LeaveRequest> LeaveRequests { get; set; }

    public virtual DbSet<PayAuditLog> PayAuditLogs { get; set; }

    public virtual DbSet<Payroll> Payrolls { get; set; }

    public virtual DbSet<PayrollAllowance> PayrollAllowances { get; set; }

    public virtual DbSet<PayrollDeduction> PayrollDeductions { get; set; }

    public virtual DbSet<PayrollHistory> PayrollHistories { get; set; }

    public virtual DbSet<PayrollPayment> PayrollPayments { get; set; }

    public virtual DbSet<PerformanceReview> PerformanceReviews { get; set; }

    public virtual DbSet<PerformanceReviewHistory> PerformanceReviewHistories { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<RegisterLink> RegisterLink { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RolePermission> RolePermissions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

//     protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
// #warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//         => optionsBuilder.UseSqlServer("Server=tcp:108.181.199.153,10875;Initial Catalog=HRNewDb;User ID=sa;Password=Gibs@321.;Encrypt=False;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Latin1_General_CI_AS");

        modelBuilder.Entity<Announcement>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Announce__3214EC079131EAFC");

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
            entity.HasKey(e => e.Id).HasName("PK__AuditLog__3214EC070D50CDB8");

            entity.ToTable("AuditLog");

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
            entity.HasKey(e => e.Id).HasName("PK__Departme__3214EC078CB93B7A");

            entity.HasIndex(e => e.DepartmentName, "UQ__Departme__D949CC34D2C6D368").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DepartmentName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Document__3214EC07262371E9");

            entity.Property(e => e.DocumentType)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.EmployeeId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.EmployeeName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.FileUrl).HasColumnType("text");
            entity.Property(e => e.UploadedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC07349E4901");

            entity.HasIndex(e => e.StaffIdNo, "UQ__Employee__0BF17005B3063D81").IsUnique();

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.AcctName1)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.AcctName2)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.AcctNo1)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.AcctNo2)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Active).HasDefaultValue(true);
            entity.Property(e => e.Address).HasColumnType("text");
            entity.Property(e => e.Branch)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.BranchId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ConfirmStatus).HasDefaultValue(false);
            entity.Property(e => e.Country)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Deleted).HasDefaultValue(false);
            entity.Property(e => e.Dept)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.DeptId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.DisableType)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Email2)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Grade)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.GradeId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Height).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.HmoId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.HmoName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.KinAddress).HasColumnType("text");
            entity.Property(e => e.KinPhone)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.KinRelationship)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.MaritalStatus)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MobilePhone)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.NationalIdNo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.NextKin)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.OtherNames)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PayFirstMonth).HasDefaultValue(true);
            entity.Property(e => e.Photo).HasColumnType("text");
            entity.Property(e => e.Remarks).HasColumnType("text");
            entity.Property(e => e.Sex)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.SheetId2)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Smoker).HasDefaultValue(false);
            entity.Property(e => e.StaffIdNo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.State)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.StateOrigin)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.SubmitBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.SubmitOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Surname)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Tag)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Telephone)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Title)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Unit)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.UnitId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Weight).HasColumnType("decimal(5, 2)");
        });

        modelBuilder.Entity<EmploymentType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Employme__3214EC07E2E1995D");

            entity.HasIndex(e => e.TypeName, "UQ__Employme__D4E7DFA895D52474").IsUnique();

            entity.Property(e => e.TypeName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ForgottenPassword>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Forgotte__3214EC07992B881C");

            entity.ToTable("ForgottenPassword");

            entity.HasIndex(e => e.Code, "UQ__Forgotte__A25C5AA7B375C049").IsUnique();

            entity.Property(e => e.Code)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ExpiredDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<JobApplication>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__JobAppli__3214EC0745E0A674");

            entity.Property(e => e.ApplicantName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.AppliedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.JobTitle)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ResumeUrl).HasColumnType("text");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<JobApplicationHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__JobAppli__3214EC07480394B3");

            entity.ToTable("JobApplicationHistory");

            entity.Property(e => e.CandidateName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ChangeDatetime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.JobApplicationId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PositionApplied)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<JobPosting>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__JobPosti__3214EC0742465592");

            entity.Property(e => e.DepartmentName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.PostedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Requirements).HasColumnType("text");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<LeaveHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__LeaveHis__3214EC078CD232BC");

            entity.ToTable("LeaveHistory");

            entity.Property(e => e.ChangeDatetime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EmployeeId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.EmployeeName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.LeaveRequestId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LeaveType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<LeaveRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__LeaveReq__3214EC07AA3E8216");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EmployeeId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.EmployeeName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.LeaveType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<PayAuditLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PayAudit__3214EC07EB374547");

            entity.ToTable("PayAuditLog");

            entity.Property(e => e.Action)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Datetime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TableName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.UserName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Payroll>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Payroll__3214EC0788506A14");

            entity.ToTable("Payroll");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.AccountNumber)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Allowances)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.BankName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.BasicSalary).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Bonus)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EmployeeId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.EmployeeName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.HealthInsurance)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.LoanDeduction)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.OtherDeductions)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.OvertimeRate)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PensionRate)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(5, 2)");
            entity.Property(e => e.TaxRate)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(5, 2)");
        });

        modelBuilder.Entity<PayrollAllowance>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PayrollA__3214EC070DE1BEFB");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.AllowanceType)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.EmployeeId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.EmployeeName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.GrantedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<PayrollDeduction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PayrollD__3214EC075B989507");

            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.DeductionType)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<PayrollHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PayrollH__3214EC07C0F55957");

            entity.ToTable("PayrollHistory");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.BasicSalary).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EmployeeId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.EmployeeName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.GrossSalary).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.HealthInsurance).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.LoanRepayment).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.NetSalary).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.OtherDeductions)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PaidOn).HasColumnType("datetime");
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("Pending");
            entity.Property(e => e.PensionDeducted).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ProcessedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TaxDeducted).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TotalAllowances).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TotalBonus)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TotalDeductions).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TotalOvertime)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<PayrollPayment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PayrollP__3214EC0794C7A656");

            entity.HasIndex(e => e.TransactionId, "UQ__PayrollP__55433A6A742EEE6F").IsUnique();

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EmployeeId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.EmployeeName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PaymentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PayrollId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TransactionId)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<PerformanceReview>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Performa__3214EC072A4925AC");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EmployeeId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.EmployeeName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Feedback).HasColumnType("text");
            entity.Property(e => e.PerformanceScore).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.ReviewerId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ReviewerName)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<PerformanceReviewHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Performa__3214EC0726B4F52F");

            entity.ToTable("PerformanceReviewHistory");

            entity.Property(e => e.ChangeDatetime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EmployeeId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.EmployeeName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Feedback).HasColumnType("text");
            entity.Property(e => e.PerformanceReviewId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PerformanceScore).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.ReviewerId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ReviewerName)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Permissi__3214EC070EAB915D");

            entity.HasIndex(e => e.PermissionName, "UQ__Permissi__0FFDA357DBCA3ECC").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PermissionName)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<RegisterLink>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Register__3214EC07663DD0F4");

            entity.ToTable("RegisterLink");

            entity.HasIndex(e => e.Code, "UQ__Register__A25C5AA78CC0245D").IsUnique();

            entity.Property(e => e.Code)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ExpiredDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC07D21A82B4");

            entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B6160BFB3DCA4").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.RoleName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RolePerm__3214EC070AD5BF67");

            entity.Property(e => e.AssignedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PermissionId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PermissionName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.RoleId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.RoleName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07FCD31307");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D1053462449F57").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.EmployeeId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.EmployeeName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PasswordHash).HasColumnType("text");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserRole__3214EC07B8AB54B4");

            entity.Property(e => e.AssignedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.RoleName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.UserEmail)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
