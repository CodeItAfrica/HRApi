using System;
using System.Collections.Generic;

namespace HRApi.Models;

public partial class EmploymentType
{
    public int Id { get; set; }

    public string TypeName { get; set; } = null!;
}
