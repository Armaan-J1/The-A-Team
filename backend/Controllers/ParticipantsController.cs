using Microsoft.AspNetCore.Mvc;
using backend.Services;

namespace backend.Controllers;

[ApiController]
[Route("api/participants")]
public class ParticipantsController : ControllerBase
{
    private readonly ParticipantService _service;

    public ParticipantsController(ParticipantService service)
    {
        _service = service;
    }

    // GET /api/participants?coordinatorId=1
    // Returns all participants in the coordinator's department
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int coordinatorId)
    {
        if (coordinatorId <= 0)
            return BadRequest(new { message = "coordinatorId query parameter is required" });

        var participants = await _service.GetAllParticipantsAsync(coordinatorId);
        return Ok(participants);
    }

    // GET /api/participants/assigned?coordinatorId=1
    [HttpGet("assigned")]
    public async Task<IActionResult> GetAssigned([FromQuery] int coordinatorId)
    {
        if (coordinatorId <= 0)
            return BadRequest(new { message = "coordinatorId query parameter is required" });

        var participants = await _service.GetAssignedParticipantsAsync(coordinatorId);
        return Ok(participants);
    }

    // GET /api/participants/unassigned?coordinatorId=1
    [HttpGet("unassigned")]
    public async Task<IActionResult> GetUnassigned([FromQuery] int coordinatorId)
    {
        if (coordinatorId <= 0)
            return BadRequest(new { message = "coordinatorId query parameter is required" });

        var participants = await _service.GetNonAssignedParticipantsAsync(coordinatorId);
        return Ok(participants);
    }
}