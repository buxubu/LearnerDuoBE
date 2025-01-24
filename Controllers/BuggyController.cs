using LearnerDuo.Data;
using LearnerDuo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LearnerDuo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuggyController : ControllerBase
    {
        private readonly LearnerDuoContext _db;

        public BuggyController(LearnerDuoContext db)
        {
            _db = db;
        }

        [Authorize]
        [HttpGet("auth")]
        public ActionResult<String> GetSecret()
        {
            return "Secret text";
        }

        [HttpGet("not_found")]
        public ActionResult<User> GetNotFound()
        {
            var thing = _db.Users.Find(-1);
            if (thing == null) return NotFound();
            return Ok(thing);
        }

        [HttpGet("server_error")]
        public IActionResult GetServerError()
        {
            var thing = _db.Users.Find(-1);
            return BadRequest(thing.ToString());
        }

        [HttpGet("bad_request")]
        public IActionResult GetBadRequest()
        {
            return BadRequest(new { error = "This wasn't good request." });
        }
    }
}
