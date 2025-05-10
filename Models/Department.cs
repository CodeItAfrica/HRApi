using System;
using System.Collections.Generic;

namespace HRApi.Models
{
    public partial class Department
    {
        public int Id { get; set; }

        public string DepartmentName { get; set; } = null!;

        public DateTime? CreatedAt { get; set; }
    }

    public class CreateDepartmentRequest
    {
        public required string DepartmentName { get; set; }
    }
}


