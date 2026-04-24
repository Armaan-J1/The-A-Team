using backend.Data;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class ParticipantService
{
    private readonly AppDbContext _context;

    public ParticipantService(AppDbContext context)
    {
        _context = context;
    }

    // Helper method to get DepartmentId from CoordinatorId
    private async Task<int?> GetDepartmentIdByCoordinatorIdAsync(int coordinatorId)
    {
        var coordinator = await _context.Coordinators
            .Where(c => c.Id == coordinatorId)
            .Select(c => c.DepartmentId)
            .FirstOrDefaultAsync();

        return coordinator != 0 ? coordinator : null;
    }

    // Get assigned participants filtered by coordinator's department
    public async Task<List<ParticipantAssignmentDto>> GetAssignedParticipantsAsync(int coordinatorId)
    {
        var departmentId = await GetDepartmentIdByCoordinatorIdAsync(coordinatorId);
        
        if (departmentId == null)
            return new List<ParticipantAssignmentDto>();

        return await _context.Participants
            .Where(p => p.SessionId != null)
            .Where(p => p.DepartmentId == departmentId) // Direct department check
            .Select(p => new ParticipantAssignmentDto
            {
                Name = p.Name,
                IsAssigned = true
            })
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    // Get non-assigned participants filtered by coordinator's department
    public async Task<List<ParticipantAssignmentDto>> GetNonAssignedParticipantsAsync(int coordinatorId)
    {
        var departmentId = await GetDepartmentIdByCoordinatorIdAsync(coordinatorId);
        
        if (departmentId == null)
            return new List<ParticipantAssignmentDto>();

        return await _context.Participants
            .Where(p => p.SessionId == null)
            .Where(p => p.DepartmentId == departmentId) // Direct department check
            .Select(p => new ParticipantAssignmentDto
            {
                Name = p.Name,
                IsAssigned = false
            })
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    // Get all participants (both assigned and unassigned) filtered by coordinator's department
    public async Task<List<ParticipantAssignmentDto>> GetAllParticipantsAsync(int coordinatorId)
    {
        var departmentId = await GetDepartmentIdByCoordinatorIdAsync(coordinatorId);
        
        if (departmentId == null)
            return new List<ParticipantAssignmentDto>();

        return await _context.Participants
            .Where(p => p.DepartmentId == departmentId) // Direct department check
            .Select(p => new ParticipantAssignmentDto
            {
                Name = p.Name,
                IsAssigned = p.SessionId != null
            })
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    // Check if a specific participant is assigned
    public async Task<bool> IsAssignedAsync(int participantId)
    {
        return await _context.Participants
            .Where(p => p.Id == participantId)
            .Select(p => p.SessionId != null)
            .FirstOrDefaultAsync();
    }

    // Assign a participant to a session with department capacity validation
    public async Task<bool> AssignParticipantAsync(int participantId, int sessionId)
    {
        // Step 1: Get the participant with their department
        var participant = await _context.Participants
            .Include(p => p.Department)
            .FirstOrDefaultAsync(p => p.Id == participantId);
        
        if (participant == null)
            return false;

        // Step 2: Get the session
        var session = await _context.Sessions.FindAsync(sessionId);
        if (session == null)
            return false;

        // Step 3: Get the department directly from participant
        var department = participant.Department;
        if (department == null)
            return false;

        // Step 4: Count participants from this department already assigned to this session
        var departmentParticipantsCount = await _context.Participants
            .Where(p => p.SessionId == sessionId)
            .Where(p => p.DepartmentId == department.Id)
            .CountAsync();

        // Step 5: Check capacity
        if (departmentParticipantsCount >= department.MaxPerSession)
            return false;

        // Step 6: Assign the participant
        participant.SessionId = sessionId;
        await _context.SaveChangesAsync();
        
        return true;
    }
}

// DTO for Participant
public class ParticipantAssignmentDto
{
    public string Name { get; set; } = string.Empty;
    public bool IsAssigned { get; set; }
}