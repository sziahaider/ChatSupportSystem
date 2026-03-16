using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/chatsession")]
    public class ChatSessionController : ControllerBase
    {
        private readonly ChatSessionService _service;

        public ChatSessionController(ChatSessionService service)
        {
            _service = service;
        }

        [HttpGet("poll/{id}")]
        public async Task<IActionResult> Poll(string id)
        {
            var ok = await _service.Poll(Guid.Parse(id));

            if (!ok)
                return NotFound();

            return Ok("OK");
        }

        [HttpPost("request")]
        public async Task<IActionResult> RequestChat()
        {
            var r = await _service.CreateAsync();

            if (!r.Success)
                return BadRequest("Queue full");

            return Ok(r.SessionId);
        }
    }
}
