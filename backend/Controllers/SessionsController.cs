using Microsoft.AspNetCore.Mvc;
using backend.Services;

namespace backend.Controllers;

[ApiController]
[Route("api/sessions")]
public class SessionsController : ControllerBase
{
    private readonly SessionService _service;

    public SessionsController(SessionService service)
    {
        _service = service;
    }

    // GET /api/sessions
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var sessions = await _service.GetSessionsAsync();
        return Ok(sessions);
    }

    // GET /api/sessions/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var session = await _service.GetSessionByIdAsync(id);
        if (session == null)
            return NotFound(new { message = $"Session {id} not found" });
        return Ok(session);
    }

    // GET /api/sessions/slot/{slot}
    // e.g. /api/sessions/slot/Morning
    [HttpGet("slot/{slot}")]
    public async Task<IActionResult> GetBySlot(string slot)
    {
        var sessions = await _service.GetSessionsBySlotAsync(slot);
        return Ok(sessions);
    }
}