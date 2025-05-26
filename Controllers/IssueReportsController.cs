using HRApi.Data;
using HRApi.Models;
using HRApi.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IssueReportsController : ControllerBase
    {
        private readonly AppDbContext _context;

        private readonly IssueService _issueService;

        public IssueReportsController(AppDbContext context, IssueService issueService)
        {
            _context = context;
            _issueService = issueService;
        }

        [HttpPost]
        //[RequestSizeLimit(52428800)] // Optional: 50MB limit
        public async Task<IActionResult> SubmitIssue([FromForm] IssueReportDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var issueId = await _issueService.SubmitIssueAsync(dto);
            return Ok(new { IssueId = issueId, Message = "Issue submitted successfully." });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<IssueReport>>> GetIssueReports()
        {
            var issues = await _context.IssueReports
                .Include(i => i.Attachments)
                .OrderByDescending(i => i.SubmittedAt)
                .ToListAsync();

            return Ok(issues);
        }

    }

}
