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
            .Where(p => p.DepartmentId == departmentId)
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
            .Where(p => p.DepartmentId == departmentId)
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
            .Where(p => p.DepartmentId == departmentId)
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

    // Assign a participant to a session with full rule enforcement
    public async Task<AssignmentResult> AssignParticipantAsync(int coordinatorId, int participantId, int sessionId)
    {
        var coordinator = await _context.Coordinators.FindAsync(coordinatorId);
        if (coordinator == null)
            return AssignmentResult.Fail("coordinator_not_found", "Coordinator does not exist");

        var participant = await _context.Participants
            .Include(p => p.Department)
            .FirstOrDefaultAsync(p => p.Id == participantId);

        if (participant == null)
            return AssignmentResult.Fail("participant_not_found", "Participant does not exist");

        var session = await _context.Sessions.FindAsync(sessionId);
        if (session == null)
            return AssignmentResult.Fail("session_not_found", "Session does not exist");

        // Rule 4: Coordinator can only manage own department
        if (participant.DepartmentId != coordinator.DepartmentId)
            return AssignmentResult.Fail(
                "department_scope_violation",
                "Coordinator can only assign participants from their own department"
            );

        // Rule 2: Participant must not already be assigned
        if (participant.SessionId != null)
            return AssignmentResult.Fail(
                "already_assigned",
                $"{participant.Name} is already assigned to another session"
            );

        // Count participants in session once, reuse below
        var currentInSession = await _context.Participants
            .Where(p => p.SessionId == sessionId)
            .ToListAsync();

        // Rule 1: Session capacity
        if (currentInSession.Count >= session.Capacity)
            return AssignmentResult.Fail(
                "session_full",
                $"Session is full ({session.Capacity}/{session.Capacity})"
            );

        // Rule 3: Department per-session limit
        var department = participant.Department;
        if (department == null)
            return AssignmentResult.Fail("department_not_found", "Participant's department is missing");

        var sameDeptCount = currentInSession.Count(p => p.DepartmentId == department.Id);
        if (sameDeptCount >= department.MaxPerSession)
            return AssignmentResult.Fail(
                "department_limit_reached",
                $"{department.Name} has no seats left in this session"
            );

        // All rules passed
        participant.SessionId = sessionId;
        await _context.SaveChangesAsync();
        return AssignmentResult.Ok();
    }
}

// DTO for Participant
public class ParticipantAssignmentDto
{
    public string Name { get; set; } = string.Empty;
    public bool IsAssigned { get; set; }
}

// Structured result for assignment operations
public class AssignmentResult
{
    public bool Success { get; set; }
    public string? Reason { get; set; }
    public string? Message { get; set; }

    public static AssignmentResult Ok() => new() { Success = true };

    public static AssignmentResult Fail(string reason, string message) => new()
    {
        Success = false,
        Reason = reason,
        Message = message
    };
}