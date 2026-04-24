using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Models;
using backend.Data;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParticipantController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ParticipantController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("department/{departmentId}")]
        public async Task<IActionResult> GetByDepartment(int departmentId)
        {
            var participants = await _context.Participants
                .Include(p => p.Session)
                .Where(p => p.DepartmentId == departmentId)
                .Select(p => new {
                    p.Id,
                    p.Name,
                    IsAssigned = p.SessionId != null,
                    SessionName = p.Session != null ? p.Session.Slot.ToString() : "Unassigned" 
                })
                .ToListAsync();

            return Ok(participants);
        }
    }
}