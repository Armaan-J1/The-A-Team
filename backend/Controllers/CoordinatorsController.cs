using Microsoft.AspNetCore.Mvc;
using backend.Services;

namespace backend.Controllers;

[ApiController]
[Route("api/coordinators")]
public class CoordinatorsController : ControllerBase
{
    private readonly CoordinatorService _service;

    public CoordinatorsController(CoordinatorService service)
    {
        _service = service;
    }

    // GET /api/coordinators
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var coordinators = await _service.GetCoordinatorsAsync();
        return Ok(coordinators);
    }

    // GET /api/coordinators/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var coordinator = await _service.GetCoordinatorByIdAsync(id);
        if (coordinator == null)
            return NotFound(new { message = $"Coordinator {id} not found" });
        return Ok(coordinator);
    }
}