using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HRApi.Models
{
    public class EmploymentType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string TypeName { get; set; } = null!;

        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
