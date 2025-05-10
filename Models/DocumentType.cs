using System.ComponentModel.DataAnnotations;

namespace HRApi.Models
{
    public class DocumentType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string TypeName { get; set; } = null!;

        [StringLength(500)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<Document> Documents { get; set; } = new List<Document>();
    }
}