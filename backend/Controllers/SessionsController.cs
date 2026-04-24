using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models;

namespace backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SessionsController : ControllerBase
{
    private readonly AppDbContext _context;

    public SessionsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/sessions
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var sessions = await _context.Sessions
            .Include(s => s.Participants)
                .ThenInclude(p => p.Department)
            .Select(s => new
            {
                s.Id,
                s.Slot,
                s.TimeRange,
                s.Capacity,
                CurrentCount = s.Participants.Count,
                AvailableSeats = s.Capacity - s.Participants.Count,
                DepartmentBreakdown = s.Participants
                    .GroupBy(p => p.Department.Name)
                    .Select(g => new { Department = g.Key, Count = g.Count() })
                    .ToList()
            })
            .ToListAsync();

        return Ok(sessions);
    }

    // GET: api/sessions/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var session = await _context.Sessions
            .Include(s => s.Participants)
                .ThenInclude(p => p.Department)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (session == null)
            return NotFound();

        return Ok(new
        {
            session.Id,
            session.Slot,
            session.TimeRange,
            session.Capacity,
            CurrentCount = session.Participants.Count,
            AvailableSeats = session.Capacity - session.Participants.Count,
            Participants = session.Participants.Select(p => new { p.Id, p.Name, Department = p.Department.Name })
        });
    }

    // POST: api/sessions
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SessionCreateDto dto)
    {
        if (!Enum.TryParse<SessionSlot>(dto.Slot, true, out var slot))
            return BadRequest("Invalid slot. Use Morning, Midday, or Afternoon.");

        var session = new Session
        {
            Slot = slot,
            TimeRange = dto.TimeRange,
            Capacity = dto.Capacity
        };

        _context.Sessions.Add(session);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = session.Id }, session);
    }

    // PUT: api/sessions/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] SessionUpdateDto dto)
    {
        var session = await _context.Sessions.FindAsync(id);
        if (session == null)
            return NotFound();

        if (!Enum.TryParse<SessionSlot>(dto.Slot, true, out var slot))
            return BadRequest("Invalid slot.");

        session.Slot = slot;
        session.TimeRange = dto.TimeRange;
        session.Capacity = dto.Capacity;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/sessions/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var session = await _context.Sessions.FindAsync(id);
        if (session == null)
            return NotFound();

        _context.Sessions.Remove(session);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

// DTOs for create and update
public class SessionCreateDto
{
    public string Slot { get; set; } = "";
    public string TimeRange { get; set; } = "";
    public int Capacity { get; set; }
}

public class SessionUpdateDto
{
    public string Slot { get; set; } = "";
    public string TimeRange { get; set; } = "";
    public int Capacity { get; set; }
}