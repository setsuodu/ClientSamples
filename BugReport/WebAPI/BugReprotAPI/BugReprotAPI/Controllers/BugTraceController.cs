using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BugReprotAPI.Controllers
{
    [EnableCors("AllowCors")] //内网调试，跨域
    [ApiController]
    [Route("[controller]")]
    public class BugTraceController : ControllerBase
    {
        private readonly ILogger<BugTraceController> _logger;

        public BugTraceController(ILogger<BugTraceController> logger)
        {
            _logger = logger;
        }

        // 查询 SQL 记录的 BUG，反馈给程序员修改
        [HttpGet]
        public async Task<string> Get()
        {
            //Console.WriteLine("[Get BugTrace]");
            string array = await Provider.Query();
            return array;
        }

        // Json Raw 数据默认是 [FromBody]，加不加都可以
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] BugTrace data)
        {
            //Console.WriteLine($"[Post BugTrace] {data.Stack_trace}");

            // 写入数据库
            string json = JsonConvert.SerializeObject(data);
            await Provider.InsertJsonB("log_bugtrace", "bug", json);

            // 返回提交成功
            return Ok();  // empty & status 200
        }
    }
}
