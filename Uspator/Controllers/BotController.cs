using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Uspator.Model;

namespace Uspator.Controllers
{
    [Route("/")]
    public class BotController : Controller
    {
        [HttpPost]
        public async Task<JsonResult> Post()
        {
            // ASP.NET Core does not allow us to get raw request body in any meaningful way
            // and since BattleCube Server doesn't send us Content-Type 'application/json' header
            // we need to read raw request body and then deserialize it into our object type
            string requestBody;
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {  
                requestBody = await reader.ReadToEndAsync();
            }
            var request = JsonConvert.DeserializeObject<ServerRequest>(requestBody);
            
            // Remember to send a list of tasks, but no more than what the request specifies
            var tasks = new List<TaskBase> {new NoopTask()};
            
            return new JsonResult(tasks);
        }
    }
}