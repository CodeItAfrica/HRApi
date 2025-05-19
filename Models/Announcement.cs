using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRApi.Models;

public class Announcement
{
    [Key]
    public int Id { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public string PostedBy { get; set; }
    public DateTime PostedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
