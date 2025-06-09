using System.Net;
using System.Net.Mail;
using HRApi.Data;
using HRApi.Models;

namespace HRApi.Repository
{
    public class IssueService
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public IssueService(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<int> SubmitIssueAsync(IssueReportDto issueDto)
        {
            var report = new IssueReport
            {
                ClientName = issueDto.ClientName,
                Department = issueDto.Department,
                IssueDescription = issueDto.IssueDescription,
                IssueUrl = issueDto.IssueUrl,
                SubmittedAt = DateTime.UtcNow,
            };

            _context.IssueReports.Add(report);

            await _context.SaveChangesAsync();

            await SendEmailAsync(report);

            if (issueDto.Attachments != null && issueDto.Attachments.Any())
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
                Directory.CreateDirectory(uploadsFolder);

                foreach (var file in issueDto.Attachments)
                {
                    var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var attachment = new IssueAttachment
                    {
                        IssueReportId = report.Id,
                        FileName = file.FileName,
                        FilePath = $"/uploads/{fileName}",
                        UploadedAt = DateTime.UtcNow,
                    };

                    _context.IssueAttachments.Add(attachment);
                }

                await _context.SaveChangesAsync();
            }

            return report.Id;
        }

        private async Task SendEmailAsync(IssueReport issue)
        {
            var smtpClient = new SmtpClient("smtp.elasticemail.com")
            {
                Port = 2525,
                Credentials = new NetworkCredential(
                    "support@gibsonline.com",
                    "D76A3BA83EAC13C9426AB2D1CCC42D2556DA"
                ),
                EnableSsl = true,
            };

            var mail = new MailMessage
            {
                From = new MailAddress("support@gibsonline.com"),
                Subject = "New Issue Submitted",
                Body =
                    $@"
<b>Name:</b> {issue.ClientName}<br/>
<b>Department:</b> {issue.Department}<br/>
<b>URL:</b> {issue.IssueUrl}<br/>
<b>Details:</b> {issue.IssueDescription}<br/>
<b>Attachments:</b> ({issue.Attachments?.Count ?? 0} files)
",
                IsBodyHtml = true,
            };

            mail.To.Add("oseniwasiu@inttecktechnologies.com");
            mail.To.Add("oakinwunmi@inttecktechnologies.com");

            string uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

            if (issue.Attachments != null)
            {
                foreach (var attachment in issue.Attachments)
                {
                    var fullPath = Path.Combine(uploadsPath, attachment.FilePath);
                    if (System.IO.File.Exists(fullPath))
                    {
                        mail.Attachments.Add(new Attachment(fullPath));
                    }
                }
            }

            await smtpClient.SendMailAsync(mail);
        }
    }
}
