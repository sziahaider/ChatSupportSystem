using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/agent")]
    public class AgentController : ControllerBase
    {
        private readonly AgentService _service;

        public AgentController(AgentService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Post(Agent agent)
        {
            return Ok(await _service.CreateAsync(agent));
        }
    }
}
