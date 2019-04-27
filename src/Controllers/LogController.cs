using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace src.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly FileQueue _q;

        public LogController(FileQueue q)
        {
            _q = q;
        }
        [HttpPost]
        public async Task<IActionResult> Post()
        {
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                var rawData = await reader.ReadToEndAsync();
                try
                {
                    var obj = JObject.Parse(rawData);
                    _q.Add(obj);
                }
                catch (Exception)
                {
                    _q.Add(rawData);
                }
                
                return Content(rawData);
            }
        }
    }
}
