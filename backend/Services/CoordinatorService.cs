using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class CoordinatorService
{
    private readonly AppDbContext _context;

    public CoordinatorService(AppDbContext context)
    {
        _context = context;
    }

    // Get all coordinators
    public async Task<List<CoordinatorDto>> GetCoordinatorsAsync()
    {
        return await _context.Coordinators
            .Include(c => c.Department)
            .OrderBy(c => c.Name)
            .Select(c => new CoordinatorDto
            {
                Name = c.Name,
                Department = c.Department != null ? c.Department.Name : "No Department"
            })
            .ToListAsync();
    }

    // Get coordinator by ID
    public async Task<CoordinatorDto?> GetCoordinatorByIdAsync(int coordinatorId)
    {
        var coordinator = await _context.Coordinators
            .Include(c => c.Department)
            .Where(c => c.Id == coordinatorId)
            .Select(c => new CoordinatorDto
            {
                Name = c.Name,
                Department = c.Department != null ? c.Department.Name : "No Department"
            })
            .FirstOrDefaultAsync();

        return coordinator;
    }



    // Get department ID from coordinator ID
    public async Task<int?> GetDepartmentIdByCoordinatorIdAsync(int coordinatorId)
    {
        var coordinator = await _context.Coordinators
            .Where(c => c.Id == coordinatorId)
            .Select(c => c.DepartmentId)
            .FirstOrDefaultAsync();

        return coordinator != 0 ? coordinator : null;
    }
}

// DTO for Coordinator
public class CoordinatorDto
{
    public string Name { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
}