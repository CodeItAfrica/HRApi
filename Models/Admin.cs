using System.ComponentModel.DataAnnotations;

namespace HRApi.Models
{
    public class Admin
    {
        public class Branch
        {
            [Key]
            public int BranchID { get; set; }
            public string? BranchName { get; set; }
            public string? Location { get; set; }
            public DateTime CreatedAt { get; set; } = DateTime.Now;
        }

        public class JobTitle
        {
            [Key]

            public int JobTitleID { get; set; }
            public string? TitleName { get; set; }
            public int DepartmentID { get; set; }
            public DateTime CreatedAt { get; set; } = DateTime.Now;
        }

        public class Grade
        {
            [Key]

            public int JobGradeID { get; set; }
            public string? GradeName { get; set; }
            public string? Description { get; set; }
            public DateTime CreatedAt { get; set; } = DateTime.Now;
        }

        public class LeaveType
        {
            [Key]

            public int LeaveTypeID { get; set; }
            public string? TypeName { get; set; }
            public int DefaultDays { get; set; }
            public DateTime CreatedAt { get; set; } = DateTime.Now;
        }

        public class Notification
        {
            [Key]
            public int ID { get; set; }
            public string? Receiver { get; set; }
            public string? Subject { get; set; }
            public string? Body { get; set; }
            public DateTime CreatedAt { get; set; } = DateTime.Now;
        }

        public class FAQ
        {
            [Key]
            public int FAQID { get; set; }
            public string? Question { get; set; }
            public string? Answer { get; set; }
            public DateTime CreatedAt { get; set; } = DateTime.Now;
        }

        public class Bank
        {
            [Key]
            public int BankID { get; set; }
            public string? BankName { get; set; }
            public DateTime CreatedAt { get; set; } = DateTime.Now;
        }

        public class TrainingProgram
        {
            [Key]
            public int TrainingID { get; set; }
            public string? Title { get; set; }
            public string? Description { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public DateTime CreatedAt { get; set; } = DateTime.Now;
        }

    }
}
