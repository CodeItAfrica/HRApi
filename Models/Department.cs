using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HRApi.Models
{
    public partial class Department
    {
        [Key]
        public int Id { get; set; }

        public string DepartmentName { get; set; } = null!;

        public DateTime? CreatedAt { get; set; }

        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
        
        public virtual ICollection<JobPosting> JobPostings { get; set; } = new List<JobPosting>();
    }
}


