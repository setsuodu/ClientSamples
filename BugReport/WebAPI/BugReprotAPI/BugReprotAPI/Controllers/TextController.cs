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

        // ���� Post Text ���� Body �����ַ���������ѡȡ���գ�ֱ�ӽ��� Body��
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
