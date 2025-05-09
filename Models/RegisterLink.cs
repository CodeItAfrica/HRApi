using System;
using System.Collections.Generic;

namespace HRApi.Models;

public partial class RegisterLink
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string Code { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public DateTime? ExpiredDate { get; set; }
}
