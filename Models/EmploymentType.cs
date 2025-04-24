using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRApi.Models
{
    [Table("employment_types")]
    public partial class EmploymentType
    {
        public int Id { get; set; }

        public string TypeName { get; set; } = null!;
    }
}