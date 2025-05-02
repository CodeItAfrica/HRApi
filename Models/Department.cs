using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRApi.Models
{
    public partial class Department
    {
        public int Id { get; set; }

        public string DepartmentName { get; set; } = null!;

        public DateTime? CreatedAt { get; set; }
    }
}