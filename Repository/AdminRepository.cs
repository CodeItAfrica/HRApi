using System.Net;
using System.Net.Mail;
using HRApi.Data;
using HRApi.Models;
using Microsoft.EntityFrameworkCore;
using static HRApi.Models.Admin;

namespace HRApi.Repository
{
    public class AdminRepository
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AdminRepository(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<bool> SendNotificationAsync(CreateNotificationDto dto)
        {
            var employee = await _context
                .Employees.Where(e => e.Id == dto.EmployeeId.ToString())
                .FirstOrDefaultAsync();
            if (employee == null)
                return false;

            string fullName = employee.Surname + " " + (employee.OtherNames ?? "");

            var notification = new Notification
            {
                Receiver = fullName.Trim(),
                Subject = dto.Subject,
                Body = dto.Body,
                CreatedAt = DateTime.Now,
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            // Send email
            await SendEmailAsync(employee.Email, dto.Subject, dto.Body);

            return true;
        }

        public async Task<IEnumerable<Notification>> GetAllNotificationsAsync()
        {
            return await _context.Notifications.OrderByDescending(n => n.CreatedAt).ToListAsync();
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpHost = _config["Smtp:Host"];
            var smtpPort = int.Parse(_config["Smtp:Port"]);
            var smtpUser = _config["Smtp:Username"];
            var smtpPass = _config["Smtp:Password"];
            var sender = _config["Smtp:Sender"];

            using (var client = new SmtpClient(smtpHost, smtpPort))
            {
                client.Credentials = new NetworkCredential(smtpUser, smtpPass);
                client.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(smtpUser, "Human Resource"), // Replace with your name
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(toEmail);

                try
                {
                    // Console.WriteLine("I ran 22")
                    await client.SendMailAsync(mailMessage);
                }
                catch (SmtpException ex)
                {
                    // Print more detailed exception message for debugging
                    Console.WriteLine($"Error sending email: {ex.ToString()}");
                }
            }
        }

        public async Task<IEnumerable<Announcement>> GetAllAnnouncementsAsync()
        {
            return await _context.Announcements.ToListAsync();
        }

        public async Task<Announcement> GetAnnouncementByIdAsync(int id)
        {
            return await _context.Announcements.FindAsync(id);
        }

        public async Task<Announcement> CreateAnnouncementAsync(Announcement announcement)
        {
            announcement.CreatedAt = DateTime.Now;
            announcement.PostedAt = DateTime.Now;

            _context.Announcements.Add(announcement);
            await _context.SaveChangesAsync();

            var emails = await _context.Employees.Select(e => e.Email).ToListAsync();
            foreach (var email in emails)
            {
                await SendEmailAsync(email, announcement.Title, announcement.Message);
            }

            return announcement;
        }

        public async Task<bool> UpdateAnnouncementAsync(int id, Announcement updatedAnnouncement)
        {
            var announcement = await _context.Announcements.FindAsync(id);
            if (announcement == null)
                return false;

            announcement.Title = updatedAnnouncement.Title;
            announcement.Message = updatedAnnouncement.Message;
            announcement.PostedBy = updatedAnnouncement.PostedBy;
            announcement.PostedAt = updatedAnnouncement.PostedAt;

            await _context.SaveChangesAsync();

            var emails = await _context.Employees.Select(e => e.Email).ToListAsync();
            foreach (var email in emails)
            {
                await SendEmailAsync(
                    email,
                    $"[Updated] {announcement.Title}",
                    announcement.Message
                );
            }

            return true;
        }

        public async Task<bool> DeleteAnnouncementAsync(int id)
        {
            var announcement = await _context.Announcements.FindAsync(id);
            if (announcement == null)
                return false;

            _context.Announcements.Remove(announcement);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<JobTitle>> GetJobTitlesAsync()
        {
            return await _context.JobTitles.ToListAsync();
        }

        public async Task<JobTitle> GetJobTitleAsync(int id)
        {
            return await _context.JobTitles.FindAsync(id);
        }

        public async Task AddJobTitleAsync(JobTitle jobTitle)
        {
            _context.JobTitles.Add(jobTitle);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateJobTitleAsync(JobTitle jobTitle)
        {
            _context.Entry(jobTitle).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteJobTitleAsync(int id)
        {
            var jobTitle = await _context.JobTitles.FindAsync(id);
            if (jobTitle != null)
            {
                _context.JobTitles.Remove(jobTitle);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Branch>> GetBranchesAsync()
        {
            return await _context.Branches.ToListAsync();
        }

        public async Task<Branch> GetBranchAsync(int id)
        {
            return await _context.Branches.FindAsync(id);
        }

        public async Task AddBranchAsync(CreateBranchRequest branch)
        {
            var newBranch = new Branch
            {
                BranchName = branch.BranchName,
                Address = branch.Address,
                City = branch.City,
                State = branch.State,
                Country = branch.Country,
                CreatedAt = DateTime.UtcNow,
            };

            _context.Branches.Add(newBranch);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateBranchAsync(CreateBranchRequest branch)
        {
            _context.Entry(branch).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBranchAsync(int id)
        {
            var branch = await _context.Branches.FindAsync(id);
            if (branch != null)
            {
                _context.Branches.Remove(branch);
                await _context.SaveChangesAsync();
            }
        }
    }
}
