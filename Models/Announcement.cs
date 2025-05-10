using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRApi.Models;

public class Announcement
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = null!;

    [Required]
    [StringLength(2000)]
    public string Message { get; set; } = null!;

    [ForeignKey("PostedByUser")]
    public int? PostedByUserId { get; set; }
    public virtual User? PostedByUser { get; set; }

    public DateTime PostedAt { get; set; } = DateTime.UtcNow;
}
