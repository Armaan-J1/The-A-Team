using Microsoft.AspNetCore.Mvc;
using backend.Services;

namespace backend.Controllers;

[ApiController]
[Route("api/assignments")]
public class AssignmentsController : ControllerBase
{
    private readonly ParticipantService _service;

    public AssignmentsController(ParticipantService service)
    {
        _service = service;
    }

    public class AssignRequest
    {
        public int CoordinatorId { get; set; }
        public int ParticipantId { get; set; }
        public int SessionId { get; set; }
    }

    // POST /api/assignments
    // Body: { "coordinatorId": 1, "participantId": 5, "sessionId": 1 }
    // Enforces all 4 rules via ParticipantService.AssignParticipantAsync
    [HttpPost]
    public async Task<IActionResult> Assign([FromBody] AssignRequest body)
    {
        var result = await _service.AssignParticipantAsync(
            body.CoordinatorId,
            body.ParticipantId,
            body.SessionId
        );

        if (!result.Success)
        {
            return BadRequest(new
            {
                success = false,
                reason = result.Reason,
                message = result.Message
            });
        }

        return Ok(new
        {
            success = true,
            assignment = new
            {
                participantId = body.ParticipantId,
                sessionId = body.SessionId
            }
        });
    }
}