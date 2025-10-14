using Microsoft.AspNetCore.Mvc;

namespace BugReprotAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TextController : ControllerBase
    {
        private readonly ILogger<TextController> _logger;

        public TextController(ILogger<TextController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public void Get()
        {
            Console.WriteLine($"[Get Text]");
        }

        // 由于 Post Text 整个 Body 都是字符串，不用选取接收，直接解析 Body。
        [HttpPost]
        public async Task<IActionResult> Post()
        {
            using (var reader = new StreamReader(Request.Body))
            {
                string rawText = await reader.ReadToEndAsync();

                Console.WriteLine($"[Post Text] {rawText}");
            }
            return Ok();
        }
    }
}
