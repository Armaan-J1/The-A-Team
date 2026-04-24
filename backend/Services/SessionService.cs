using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class SessionService
{
    private readonly AppDbContext _context;

    public SessionService(AppDbContext context)
    {
        _context = context;
    }

    // Get all sessions
    public async Task<List<SessionDto>> GetSessionsAsync()
    {
        return await _context.Sessions
            .OrderBy(s => s.Slot)
            .ThenBy(s => s.TimeRange)
            .Select(s => new SessionDto
            {
                Name = s.TimeRange,
                Slot = s.Slot.ToString()
            })
            .ToListAsync();
    }

    // Get session by ID
    public async Task<SessionDto?> GetSessionByIdAsync(int sessionId)
    {
        var session = await _context.Sessions
            .Where(s => s.Id == sessionId)
            .Select(s => new SessionDto
            {
                Name = s.TimeRange,
                Slot = s.Slot.ToString()
            })
            .FirstOrDefaultAsync();

        return session;
    }

    // Get sessions by slot (Morning, Midday, Afternoon)
    public async Task<List<SessionDto>> GetSessionsBySlotAsync(string slot)
    {
        return await _context.Sessions
            .Where(s => s.Slot.ToString() == slot)
            .OrderBy(s => s.TimeRange)
            .Select(s => new SessionDto
            {
                Name = s.TimeRange,
                Slot = s.Slot.ToString()
            })
            .ToListAsync();
    }
}

// DTO for Session
public class SessionDto
{
    public string Name { get; set; } = string.Empty;
    public string Slot { get; set; } = string.Empty;
}