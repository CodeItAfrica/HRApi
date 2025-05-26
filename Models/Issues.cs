namespace HRApi.Models
{
    public class IssueReport
    {
        public int Id { get; set; }
        public string ClientName { get; set; }
        public string Department { get; set; }
        public string IssueDescription { get; set; }
        public string IssueUrl { get; set; }
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        public ICollection<IssueAttachment> Attachments { get; set; }
    }

    public class IssueAttachment
    {
        public int Id { get; set; }
        public int? IssueReportId { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public IssueReport? IssueReport { get; set; }
    }


    public class IssueReportDto
    {
        public string? ClientName { get; set; }
        public string? Department { get; set; }
        public string? IssueDescription { get; set; }
        public string? IssueUrl { get; set; }
        public List<IFormFile>? Attachments { get; set; }
    }

}
