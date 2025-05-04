using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using System.Text;
using HRApi.Data;
using HRApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace HRApi.Repository
{
    public class AuthRepository
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthRepository(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public string GenerateJwt(string email, int userId, List<string> roles)
        {
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, email),
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<bool> CreateRegisterLinkAsync(string email)
        {
            var existingLink = await _context.RegisterLink
                                      .FirstOrDefaultAsync(r => r.Email == email && r.ExpiredDate > DateTime.UtcNow);

            if (existingLink != null)
            {
                _context.RegisterLink.Remove(existingLink);
                await _context.SaveChangesAsync();
            }
            
            var code = new Random().Next(100000, 999999).ToString(); // 6-digit code
            var link = new RegisterLink
            {
                Email = email,
                Code = code,
                CreatedDate = DateTime.UtcNow,
                ExpiredDate = DateTime.UtcNow.AddMinutes(15)
            };
            _context.RegisterLink.Add(link);
            await _context.SaveChangesAsync();
            // Console.WriteLine("I ran 33")

            string registerUrl = $"https://yourfrontend.com/register";
            string body = $"Click this link to register: {registerUrl}. Your code: {code}";

            await SendEmailAsync(email, "Registration Link", body);
            return true;
        }

        public async Task<bool> VerifyRegisterCodeAsync(string email, string code)
        {
            var record = await _context.RegisterLink.FirstOrDefaultAsync(x => x.Email == email && x.Code == code);
            if (record == null || record.ExpiredDate < DateTime.UtcNow)
            {
                return false;
            }

            // return GenerateJwt(email, 0, new List<string> { "register" });
            _context.RegisterLink.Remove(record);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CreateForgottenPasswordAsync(string email)
        {
            var code = new Random().Next(100000, 999999).ToString(); // 6-digit code
            var forgot = new ForgottenPassword
            {
                Email = email,
                Code = code,
                CreatedDate = DateTime.UtcNow,
                ExpiredDate = DateTime.UtcNow.AddMinutes(15)
            };
            _context.ForgottenPassword.Add(forgot);
            await _context.SaveChangesAsync();

            string body = $"Reset your password using this code: {code}";

            await SendEmailAsync(email, "Reset Password", body);
            return true;
        }

        public async Task<bool> ResetPasswordAsync(string email, string code, string newPassword)
        {
            var record = await _context.ForgottenPassword.FirstOrDefaultAsync(x => x.Email == email && x.Code == code);
            if (record == null || record.ExpiredDate < DateTime.UtcNow)
            {
                return false;
            }

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
            if (user == null) return false;

            user.PasswordHash = newPassword; // No hashing as requested
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
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
                    From = new MailAddress(smtpUser, "Edward Philip"),  // Replace with your name
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
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
    }
}
