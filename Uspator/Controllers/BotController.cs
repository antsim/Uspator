using System;
using System.Diagnostics;
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
            Console.WriteLine(request.GameInfo.CurrentTick);

            try
            {
                var battleFactory = new BattleTaskFactory(request);
                var tasks = battleFactory.GetTaskTypes(request.GameInfo.NumOfTasksPerTick);
                foreach (var task in tasks)
                {
                    battleFactory.ApplyTask(task);
                }
            
                // Remember to send a list of tasks, but no more than what the request specifies
                Console.WriteLine(JsonConvert.SerializeObject(tasks));
                return new JsonResult(tasks);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }
    }
}