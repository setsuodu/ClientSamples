using Microsoft.AspNetCore.Mvc;

namespace BugReprotAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FormdataController : ControllerBase
    {
        private readonly ILogger<FormdataController> _logger;

        public FormdataController(ILogger<FormdataController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public void Get()
        {
            Console.WriteLine($"[Get Formdata]");
        }

        // [FromForm] 用于接收 form-data 和 x-www-form-urlencoded
        [HttpPost]
        public IActionResult Post([FromForm] Formdata data)
        {
            Console.WriteLine($"[Post Formdata] ID={data.ID}, Key={data.Key}");

            if (ModelState.IsValid)
            {
                // Access form data: model.Name, model.Age
                // Handle file upload: model.ProfilePicture
                // ... process the data ...
                return Ok("Form submitted successfully!");
            }
            return BadRequest(ModelState);
        }
    }
}
