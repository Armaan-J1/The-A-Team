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

    // Get all sessions with seat availability
    public async Task<List<SessionDto>> GetSessionsAsync()
    {
        return await _context.Sessions
            .Include(s => s.Participants)
            .OrderBy(s => s.Slot)
            .ThenBy(s => s.TimeRange)
            .Select(s => new SessionDto
            {
                Id = s.Id,
                Name = s.TimeRange,
                Slot = s.Slot.ToString(),
                TotalCapacity = s.Capacity,
                RemainingSeats = s.Capacity - s.Participants.Count
            })
            .ToListAsync();
    }

    // Get session by ID
    public async Task<SessionDto?> GetSessionByIdAsync(int sessionId)
    {
        var session = await _context.Sessions
            .Include(s => s.Participants)
            .Where(s => s.Id == sessionId)
            .Select(s => new SessionDto
            {
                Id = s.Id,
                Name = s.TimeRange,
                Slot = s.Slot.ToString(),
                TotalCapacity = s.Capacity,
                RemainingSeats = s.Capacity - s.Participants.Count
            })
            .FirstOrDefaultAsync();

        return session;
    }

    // Get sessions by slot (Morning, Midday, Afternoon)
    public async Task<List<SessionDto>> GetSessionsBySlotAsync(string slot)
    {
        return await _context.Sessions
            .Include(s => s.Participants)
            .Where(s => s.Slot.ToString() == slot)
            .OrderBy(s => s.TimeRange)
            .Select(s => new SessionDto
            {
                Id = s.Id,
                Name = s.TimeRange,
                Slot = s.Slot.ToString(),
                TotalCapacity = s.Capacity,
                RemainingSeats = s.Capacity - s.Participants.Count
            })
            .ToListAsync();
    }
}

// DTO for Session
public class SessionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slot { get; set; } = string.Empty;
    public int TotalCapacity { get; set; }
    public int RemainingSeats { get; set; }
}